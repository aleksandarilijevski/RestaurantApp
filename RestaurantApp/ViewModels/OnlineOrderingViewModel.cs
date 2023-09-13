using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
                _selectedTableArticleQuantity = value;
                RaisePropertyChanged();

                if (_selectedTableArticleQuantity != null)
                {
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

                TableArticleQuantities.Add(tableArticleQuantity);
                await _databaseService.AddTableArticleQuantity(tableArticleQuantity, efContext);
                await IncreaseQuantity(tableArticleQuantity, articleDetails, tableArticleQuantity.Quantity, efContext);
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
            _selectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;

            using EFContext efContext = new EFContext();

            Debug.WriteLine("Trigger method");

            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantityByID(selectedTableArticleQuantity.ID, efContext);

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(selectedTableArticleQuantity.ArticleID, efContext);

            bool isQuantityAvailable = await IfQuantityIsAvailable(selectedTableArticleQuantity.Article);

            if (isQuantityAvailable)
            {
                int oldQuantity = tableArticleQuantity.Quantity;

                tableArticleQuantity.Quantity = selectedTableArticleQuantity.Quantity;
                await _databaseService.EditTableArticleQuantity(tableArticleQuantity, efContext);

                if (selectedTableArticleQuantity.Quantity > oldQuantity)
                {
                    int calculateHowMuchToIncrease = selectedTableArticleQuantity.Quantity - oldQuantity;
                    await IncreaseQuantity(tableArticleQuantity, articleDetails, calculateHowMuchToIncrease, efContext);
                }
                else
                {
                    int calculateHowMuchToDecrease = Math.Abs(selectedTableArticleQuantity.Quantity - oldQuantity);
                    await DecreaseQuantityFromCell(selectedTableArticleQuantity, articleDetails, calculateHowMuchToDecrease, efContext);
                }
            }
            else
            {
                SelectedTableArticleQuantity.Quantity = tableArticleQuantity.Quantity;
                MessageBox.Show("Not in stock!!!!!");
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

        private async Task IncreaseQuantity(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, int quantity, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (quantity <= 0)
                {
                    break;
                }

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, quantity);

                articleDetail.ReservedQuantity += quantityToReserve;
                quantity -= quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail, efContext);
            }
        }


        private async Task DecreaseQuantityFromCell(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, int quantityToBeRemoved, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.ReservedQuantity != 0)
                {
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (quantityToBeRemoved == 1)
                    {
                        articleDetail.ReservedQuantity--; ;
                        await _databaseService.EditArticleDetails(articleDetail, efContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        private async Task DecreaseQuantity(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            int quantityToBeRemoved = tableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.ReservedQuantity != 0)
                {
                    //if (articleDetail.ReservedQuantity < tableArticleQuantity.Quantity)
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (quantityToBeRemoved == 1)
                    {
                        articleDetail.ReservedQuantity--; ;
                        await _databaseService.EditArticleDetails(articleDetail, efContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        private async void DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            int quantityToRemove = SelectedTableArticleQuantity.Quantity;

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);
            await DecreaseQuantity(tableArticleQuantity, articleDetails, efContext);

            TableArticleQuantities.Remove(tableArticleQuantity);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity, new EFContext());
        }

    }
}
