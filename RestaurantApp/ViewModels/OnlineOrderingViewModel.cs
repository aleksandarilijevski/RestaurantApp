using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
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
        private DelegateCommand _checkIfOnlineOrderExistsCommand;
        private OnlineOrder _onlineOrder = new OnlineOrder();
        private int _quantityValueBeforeChange = 0;
        private EFContext _efContext;

        public OnlineOrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager, EFContext efContext)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
            _efContext = efContext;
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

        public DelegateCommand CheckIfOnlineOrderExistsCommand
        {
            get
            {
                _checkIfOnlineOrderExistsCommand = new DelegateCommand(CheckIfOnlineOrderExists);
                return _checkIfOnlineOrderExistsCommand;
            }
        }

        private void GoToPayment()
        {
            if (TableArticleQuantities.Count == 0)
            {
                MessageBox.Show("There are no articles to be paid!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //if (OnlineOrder.Firstname is null || OnlineOrder.Firstname == string.Empty)
            //{
            //    MessageBox.Show("Firstname field can not be empty!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //if (OnlineOrder.Lastname is null || OnlineOrder.Lastname == string.Empty)
            //{
            //    MessageBox.Show("Lastname field can not be empty!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //if (OnlineOrder.Address is null || OnlineOrder.Address == string.Empty)
            //{
            //    MessageBox.Show("Address field can not be empty!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //if (OnlineOrder.PhoneNumber.ToString().Length < 8)
            //{
            //    MessageBox.Show("Phone number field is not valid!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            NavigationParameters navigationParameters = new NavigationParameters()
            {
                {"tableArticleQuantities",TableArticleQuantities.ToList() },
                {"onlineOrder",OnlineOrder }
            };

            _regionManager.RequestNavigate("MainRegion", "Payment", navigationParameters);
        }

        private async void CheckIfOnlineOrderExists()
        {
            TableArticleQuantities.Clear();
            using EFContext efContext = new EFContext();
            OnlineOrder onlineOrder = await _databaseService.GetLastOnlineOrder();

            if (onlineOrder is null)
            {
                onlineOrder = new OnlineOrder();
                await _databaseService.AddOnlineOrder(onlineOrder);
            }

            if (onlineOrder is not null && onlineOrder.IsPayed == false)
            {
                OnlineOrder = onlineOrder;
                TableArticleQuantities = new ObservableCollection<TableArticleQuantity>(OnlineOrder.TableArticleQuantities);
            }
            else if (onlineOrder.IsPayed == true)
            {
                onlineOrder = new OnlineOrder();
                await _databaseService.AddOnlineOrder(onlineOrder);
                OnlineOrder = onlineOrder;
            }
        }

        private async void AddArticleToOnlineOrder(string barcode)
        {
            using EFContext efContext = new EFContext();

            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcodeContext(barcodeLong, efContext);

            if (article is null)
            {
                MessageBox.Show("Article with entered barcode doesn't exist in the system!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            bool isQuantityAvailable = await IfQuantityIsAvailable(article);

            if (isQuantityAvailable)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(article.ID, efContext);

                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    ArticleID = article.ID,
                    OnlineOrderID = OnlineOrder.ID,
                    Quantity = 1,
                    ArticleDetails = articleDetails
                };


                await IncreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity);
                TableArticleQuantities.Add(tableArticleQuantity);
                OnlineOrder.TableArticleQuantities.Add(tableArticleQuantity);

                await _databaseService.AddTableArticleQuantityContext(tableArticleQuantity, efContext);
                await _databaseService.EditOnlineOrderContext(OnlineOrder, efContext);
            }

            Barcode = string.Empty;
        }

        /// <summary>
        /// Calculating if quantity is available
        /// </summary>
        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID);
            int quantity = GetAvailableQuantity(articleDetails);

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
            using EFContext efContext = new EFContext();
            TableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(article.ID, efContext);
            int availableQuantity = GetAvailableQuantity(articleDetails);

            if (_quantityValueBeforeChange < TableArticleQuantity.Quantity)
            {
                if (availableQuantity >= TableArticleQuantity.Quantity - _quantityValueBeforeChange)
                {
                    await IncreaseReservedQuantity(articleDetails, TableArticleQuantity.Quantity);
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
                await DecreaseReservedQuantity(articleDetails, quantityToRemove);
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

        private async void DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            List<ArticleDetails>? articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID);

            int quantityToRemove = TableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                Article article = await _databaseService.GetArticleByID(articleDetail.ArticleID);

                if (article.IsDeleted == false && article.ID == tableArticleQuantity.Article.ID)
                {

                    if (articleDetail.ReservedQuantity != 0)
                    {
                        if (articleDetail.ReservedQuantity < TableArticleQuantity.Quantity)
                        {
                            int reservedDelete = Math.Min(articleDetail.ReservedQuantity, quantityToRemove);
                            articleDetail.ReservedQuantity -= reservedDelete;
                            quantityToRemove -= reservedDelete;

                            if (quantityToRemove != 0)
                            {
                                continue;
                            }

                        }
                        else
                        {
                            articleDetail.ReservedQuantity -= TableArticleQuantity.Quantity;

                        }
                    }
                    else
                    {
                        continue;
                    }

                    await _databaseService.EditArticleDetails(articleDetail);
                    break;
                }
            }

            //Table.TableArticleQuantities.Remove(tableArticleQuantity);
            TableArticleQuantities.Remove(tableArticleQuantity);
            //await _databaseService.DeleteTableArticleQuantityContext(tableArticleQuantity,_efContext);


            //Should be deleted from same EFCOntext
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity);
        }

    }
}
