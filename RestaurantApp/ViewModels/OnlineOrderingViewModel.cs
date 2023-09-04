using EntityFramework.Models;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class OnlineOrderingViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private string _barcode;
        private TableArticleQuantity _tableArticleQuantity;
        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities = new ObservableCollection<TableArticleQuantity>();
        private DelegateCommand<string> _addArticleToOnlineOrderCommand;
        private DelegateCommand<TableArticleQuantity> _deleteTableArticleQuantityCommand;
        private DelegateCommand _goToPaymentCommand;
        private OnlineOrder _onlineOrder = new OnlineOrder();
        private int _quantityValueBeforeChange = 0;

        public OnlineOrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public string Barcode
        {
            get
            {
                return _barcode;
            }

            set
            {
                _barcode = value;
                RaisePropertyChanged();
            }
        }

        public TableArticleQuantity TableArticleQuantity
        {
            get
            {
                return _tableArticleQuantity;
            }

            set
            {
                if (_tableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _tableArticleQuantity.Quantity;

                    _tableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
                }

                _tableArticleQuantity = value;
                RaisePropertyChanged();

                if (_tableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _tableArticleQuantity.Quantity;

                    _tableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                }
            }
        }

        public OnlineOrder OnlineOrder
        {
            get
            {
                return _onlineOrder;
            }

            set
            {
                _onlineOrder = value;
            }
        }

        public DelegateCommand<TableArticleQuantity> DeleteTableArticleQuantityCommand
        {
            get
            {
                _deleteTableArticleQuantityCommand = new DelegateCommand<TableArticleQuantity>(DeleteTableArticleQuantity);
                return _deleteTableArticleQuantityCommand;
            }
        }

        public DelegateCommand GoToPaymentCommand
        {
            get
            {
                _goToPaymentCommand = new DelegateCommand(GoToPayment);
                return _goToPaymentCommand;
            }
        }

        public ObservableCollection<TableArticleQuantity> TableArticleQuantities
        {
            get
            {
                return _tableArticleQuantities;
            }

            set
            {
                _tableArticleQuantities = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> AddArticleToOnlineOrderCommand
        {
            get
            {
                _addArticleToOnlineOrderCommand = new DelegateCommand<string>(AddArticleToOnlineOrder);
                return _addArticleToOnlineOrderCommand;
            }
        }

        private void GoToPayment()
        {
            if (TableArticleQuantities.Count == 0)
            {
                MessageBox.Show("There are no articles to be paid!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OnlineOrder.Firstname is null || OnlineOrder.Firstname == string.Empty)
            {
                MessageBox.Show("Firstname field can not be empty!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OnlineOrder.Lastname is null || OnlineOrder.Lastname == string.Empty)
            {
                MessageBox.Show("Lastname field can not be empty!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OnlineOrder.Address is null || OnlineOrder.Address == string.Empty)
            {
                MessageBox.Show("Address field can not be empty!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OnlineOrder.PhoneNumber.ToString().Length != 8)
            {
                MessageBox.Show("Phone number field is not valid!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationParameters navigationParameters = new NavigationParameters()
            {
                {"tableArticleQuantities",TableArticleQuantities.ToList() }
            };

            _regionManager.RequestNavigate("MainRegion", "Payment", navigationParameters);
        }

        private async void AddArticleToOnlineOrder(string barcode)
        {
            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcode(barcodeLong);

            if (article is null)
            {
                MessageBox.Show("Article with entered barcode doesn't exist in the system!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            bool isQuantityAvailable = await IfQuantityIsAvailable(article);

            if (isQuantityAvailable)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID);

                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    ArticleID = article.ID,
                    Article = article,
                    Quantity = 1,
                    ArticleDetails = articleDetails
                };

                await IncreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity);
                TableArticleQuantities.Add(tableArticleQuantity);
            }

            Barcode = string.Empty;
        }

        /// <summary>
        /// Calculating if quantity is available
        /// </summary>
        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            int quantity = GetAvailableQuantity(article.ArticleDetails);

            if (quantity != 0)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// This functions is triggers every time Quantity cell in dataGrid is changed.
        /// </summary>
        private void OnQuantityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableArticleQuantity.Quantity))
            {
                Task isQuantityAvailableForArticleOnTable = Task.Run(() => IsQuantityAvailableForArticleOnTable(TableArticleQuantity.Article));
            }
        }

        /// <summary>
        /// Checking if quantity is available for article on table.
        /// This function is called when quantity is changed from dataGrid cell.
        /// </summary>
        private async Task IsQuantityAvailableForArticleOnTable(Article article)
        {
            TableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
            int availableQuantity = GetAvailableQuantity(article.ArticleDetails);

            if (_quantityValueBeforeChange < TableArticleQuantity.Quantity)
            {
                if (availableQuantity >= TableArticleQuantity.Quantity - _quantityValueBeforeChange)
                {
                    await IncreaseReservedQuantity(article.ArticleDetails, TableArticleQuantity.Quantity);
                    await _databaseService.EditTableArticleQuantity(TableArticleQuantity);
                    TableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                }
                else
                {
                    TableArticleQuantity.Quantity = _quantityValueBeforeChange;
                    MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                int quantityToRemove = Math.Abs(TableArticleQuantity.Quantity - _quantityValueBeforeChange);
                await DecreaseReservedQuantity(article.ArticleDetails, quantityToRemove);
            }

            RaisePropertyChanged(nameof(TableArticleQuantities));
        }

        private int GetAvailableQuantity(List<ArticleDetails> articleDetails)
        {
            int quantity = 0;

            if (articleDetails != null)
            {
                quantity = articleDetails.Sum(x => x.OriginalQuantity - x.ReservedQuantity);
            }

            return quantity;
        }

        /// <summary>
        /// Decreasing reserved quantity.
        /// </summary>
        private async Task DecreaseReservedQuantity(List<ArticleDetails> articleDetails, int reservedQuantityToBeDecreased)
        {
            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (reservedQuantityToBeDecreased <= 0)
                    break; // No more quantity to sell, exit the loop

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, reservedQuantityToBeDecreased);

                // Reserve quantityToReserve for the current articleDetail
                articleDetail.ReservedQuantity -= quantityToReserve;

                // Reduce remainingQuantityToSell
                reservedQuantityToBeDecreased += quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail);
            }
        }


        /// <summary>
        /// Increasing reserved quantity
        /// </summary>
        private async Task IncreaseReservedQuantity(List<ArticleDetails> articleDetails, int quantityToBeReserved)
        {
            if (quantityToBeReserved != 1)
            {
                quantityToBeReserved = Math.Abs(_quantityValueBeforeChange - quantityToBeReserved);
            }

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (quantityToBeReserved <= 0)
                    break; // No more quantity to sell, exit the loop

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, quantityToBeReserved);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = articleDetail.ReservedQuantity;
                }

                // Reserve quantityToReserve for the current articleDetail
                articleDetail.ReservedQuantity += quantityToReserve;

                // Reduce remainingQuantityToSell
                quantityToBeReserved -= quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail);
            }
        }

        private void DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            TableArticleQuantities.Remove(tableArticleQuantity);
        }

    }
}
