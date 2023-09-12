using EntityFramework.Models;
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
        private TableArticleQuantity _selectedTableArticleQuantity;
        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities = new ObservableCollection<TableArticleQuantity>();
        private DelegateCommand<string> _addArticleToOnlineOrderCommand;
        private DelegateCommand<TableArticleQuantity> _deleteTableArticleQuantityCommand;
        private DelegateCommand _goToPaymentCommand;
        private DelegateCommand _checkIfOnlineOrderExistsCommand;
        private int _quantityValueBeforeChange = 0;

        public OnlineOrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager, EFContext efContext)
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

        public TableArticleQuantity SelectedTableArticleQuantity
        {
            get
            {
                return _selectedTableArticleQuantity;
            }

            set
            {
                if (_selectedTableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _selectedTableArticleQuantity.Quantity;
                    _selectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
                }

                _selectedTableArticleQuantity = value;
                RaisePropertyChanged();

                if (_selectedTableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _selectedTableArticleQuantity.Quantity;

                    _selectedTableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                }
            }
        }

        public OnlineOrder OnlineOrder { get; set; } = new OnlineOrder();

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
            OnlineOrder onlineOrder = await _databaseService.GetLastOnlineOrder(efContext);

            if (onlineOrder is null)
            {
                onlineOrder = new OnlineOrder();
                await _databaseService.AddOnlineOrder(onlineOrder, efContext);
            }

            if (onlineOrder is not null && onlineOrder.IsPayed == false)
            {
                OnlineOrder = onlineOrder;
                TableArticleQuantities = new ObservableCollection<TableArticleQuantity>(OnlineOrder.TableArticleQuantities);
            }
            else if (onlineOrder.IsPayed == true)
            {
                onlineOrder = new OnlineOrder();
                await _databaseService.AddOnlineOrder(onlineOrder, efContext);
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
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID, efContext);

                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    ArticleID = article.ID,
                    OnlineOrderID = OnlineOrder.ID,
                    Quantity = 1,
                    ArticleDetails = articleDetails
                };


                await IncreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity, efContext);
                TableArticleQuantities.Add(tableArticleQuantity);

                await _databaseService.AddTableArticleQuantity(tableArticleQuantity, efContext);
            }

            Barcode = string.Empty;
        }

        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            using EFContext efContext = new EFContext();
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID, efContext);
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

        private void OnQuantityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedTableArticleQuantity.Quantity))
            {
                _ = IsQuantityAvailableForArticleOnTable(SelectedTableArticleQuantity);
            }
        }

        private async Task IsQuantityAvailableForArticleOnTable(TableArticleQuantity selectedTableArticleQuantity)
        {
            using EFContext efContext = new EFContext();
            //SelectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(selectedTableArticleQuantity.ArticleID, efContext);
            int availableQuantity = GetAvailableQuantity(articleDetails);

            if (_quantityValueBeforeChange < selectedTableArticleQuantity.Quantity)
            {
                if (availableQuantity >= selectedTableArticleQuantity.Quantity - _quantityValueBeforeChange)
                {
                    await IncreaseReservedQuantity(articleDetails, selectedTableArticleQuantity.Quantity, efContext);
                    await _databaseService.EditTableArticleQuantity(selectedTableArticleQuantity, efContext);
                    //SelectedTableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                }
                else
                {
                    selectedTableArticleQuantity.Quantity = _quantityValueBeforeChange;
                    MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                int quantityToRemove = Math.Abs(selectedTableArticleQuantity.Quantity - _quantityValueBeforeChange);
                await DecreaseReservedQuantity(articleDetails, quantityToRemove, efContext);
                await _databaseService.EditTableArticleQuantity(selectedTableArticleQuantity, efContext);
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

        private async Task DecreaseReservedQuantity(List<ArticleDetails> articleDetails, int reservedQuantityToBeDecreased, EFContext efContext)
        {
            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (reservedQuantityToBeDecreased <= 0)
                    break;

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, reservedQuantityToBeDecreased);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = reservedQuantityToBeDecreased;
                }

                articleDetail.ReservedQuantity -= quantityToReserve;
                reservedQuantityToBeDecreased += quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail, efContext);
            }
        }

        private async Task IncreaseReservedQuantity(List<ArticleDetails> articleDetails, int quantityToBeReserved, EFContext efContext)
        {
            if (quantityToBeReserved != 1)
            {
                quantityToBeReserved = Math.Abs(_quantityValueBeforeChange - quantityToBeReserved);
            }

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (quantityToBeReserved <= 0)
                    break;

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, quantityToBeReserved);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = articleDetail.ReservedQuantity;
                }

                articleDetail.ReservedQuantity += quantityToReserve;
                quantityToBeReserved -= quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail, efContext);
            }
        }

        private async void DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();
            List<ArticleDetails>? articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

            int quantityToRemove = SelectedTableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (tableArticleQuantity.Article.IsDeleted == false && tableArticleQuantity.Article.ID == tableArticleQuantity.Article.ID)
                {

                    if (articleDetail.ReservedQuantity != 0)
                    {
                        if (articleDetail.ReservedQuantity < SelectedTableArticleQuantity.Quantity)
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
                            articleDetail.ReservedQuantity -= SelectedTableArticleQuantity.Quantity;

                        }
                    }
                    else
                    {
                        continue;
                    }

                    await _databaseService.EditArticleDetails(articleDetail, efContext);
                    break;
                }
            }

            TableArticleQuantities.Remove(tableArticleQuantity);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity, new EFContext());
        }

    }
}
