using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Models;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
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
    public class OnlineOrderingViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private IDialogService _dialogService;
        private string _barcode;
        private TableArticleQuantity _selectedTableArticleQuantity;
        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities = new ObservableCollection<TableArticleQuantity>();
        private DelegateCommand<string> _addArticleToOnlineOrderCommand;
        private DelegateCommand<TableArticleQuantity> _deleteTableArticleQuantityCommand;
        private DelegateCommand _goToPaymentCommand;
        private DelegateCommand _loadOnlineOrderCommand;
        private DelegateCommand _editOnlineOrder;
        private DelegateCommand _logoutCommand;
        private OnlineOrder _onlineOrder;

        public OnlineOrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager, IDialogService dialogService, EFContext efContext)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
            _dialogService = dialogService;
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

        public User User { get; set; }

        public OnlineOrder OnlineOrder
        {
            get
            {
                return _onlineOrder;
            }

            set
            {
                _onlineOrder = value;
                RaisePropertyChanged();
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

        public DelegateCommand EditOnlineOrderCommand
        {
            get
            {
                _editOnlineOrder = new DelegateCommand(EditOnlineOrder);
                return _editOnlineOrder;
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

        public DelegateCommand LogoutCommand
        {
            get
            {
                _logoutCommand = new DelegateCommand(Logout);
                return _logoutCommand;
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

        public DelegateCommand LoadOnlineOrderCommand
        {
            get
            {
                _loadOnlineOrderCommand = new DelegateCommand(LoadOnlineOrder);
                return _loadOnlineOrderCommand;
            }
        }

        private async void EditOnlineOrder()
        {
            using EFContext efContext = new EFContext();
            await _databaseService.EditOnlineOrder(OnlineOrder, efContext);
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

            if (OnlineOrder.PhoneNumber.ToString().Length < 8)
            {
                MessageBox.Show("Phone number field is not valid!", "Online Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationParameters navigationParameters = new NavigationParameters()
            {
                {"tableArticleQuantities",TableArticleQuantities.ToList() },
                {"onlineOrder",OnlineOrder }
            };

            _regionManager.RequestNavigate("MainRegion", "Payment", navigationParameters);
        }

        private bool UserLogin()
        {
            bool isResultGood = false;

            _dialogService.ShowDialog("userLoginDialog", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    isResultGood = true;
                    User = r.Parameters.GetValue<User>("user");
                }
                else
                {
                    isResultGood = false;
                }
            });

            return isResultGood;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            OnlineOrder = (OnlineOrder)navigationContext.Parameters["onlineOrder"];
        }

        private async void LoadOnlineOrder()
        {
            //Create one more onlineOrder
            //We are doing this because we don't want to display any data while user is not logged!
            OnlineOrder onlineOrder = OnlineOrder;
            OnlineOrder = null;

            using EFContext efContext = new EFContext();

            if (onlineOrder.IsPayed)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to print invoice again?", "Online ordering", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _regionManager.RequestNavigate("MainRegion", "OnlineOrders");

                    int userId = onlineOrder.UserID ?? (int)default;
                    User user = await _databaseService.GetUserByID(userId, efContext);

                    List<Bill> bills = await _databaseService.GetAllBills();
                    Bill bill = bills.FirstOrDefault(x => x.OnlineOrderID == onlineOrder.ID);

                    DrawningHelper.RedrawBill(bill, onlineOrder.TableArticleQuantities, user);
                    return;
                }

                if (messageBoxResult == MessageBoxResult.No)
                {
                    _regionManager.RequestNavigate("MainRegion", "OnlineOrders");
                    return;
                }
            }

            bool dialogResult = UserLogin();

            if (dialogResult)
            {
                if (onlineOrder.UserID is not null && User.ID != onlineOrder.UserID)
                {
                    MessageBox.Show("Selected online order is in use by someone else!", "Online ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                    _regionManager.RequestNavigate("MainRegion", "OnlineOrders");
                    return;
                }

                onlineOrder.UserID = User.ID;
                await _databaseService.EditOnlineOrder(onlineOrder, efContext);

                User.IsActive = true;
                await _databaseService.EditUser(User, efContext);
            }

            if (!dialogResult)
            {
                _regionManager.RequestNavigate("MainRegion", "OnlineOrders");
                return;
            }

            OnlineOrder = onlineOrder;
            TableArticleQuantities = new ObservableCollection<TableArticleQuantity>(OnlineOrder.TableArticleQuantities);
        }

        private async void AddArticleToOnlineOrder(string barcode)
        {
            using EFContext efContext = new EFContext();

            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcode(barcodeLong, efContext);

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

                ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                {
                    TableArticleQuantity = tableArticleQuantity,
                    ArticleDetails = articleDetails,
                    Quantity = tableArticleQuantity.Quantity,
                    DatabaseService = _databaseService,
                    EFContext = efContext
                };

                await QuantityLogicHelper.IncreaseReservedQuantity(articleHelperDetails);
            }

            Barcode = string.Empty;
        }

        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            using EFContext efContext = new EFContext();
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID, efContext);
            int quantity = QuantityLogicHelper.GetAvailableQuantity(articleDetails);

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
                _ = IsQuantityAvailableForArticleOnOnlineOrder(SelectedTableArticleQuantity);
            }
        }

        private async Task IsQuantityAvailableForArticleOnOnlineOrder(TableArticleQuantity selectedTableArticleQuantity)
        {
            _selectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;

            using EFContext efContext = new EFContext();

            Debug.WriteLine("Trigger method");

            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantityByID(selectedTableArticleQuantity.ID, efContext);
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(selectedTableArticleQuantity.ArticleID, efContext);

            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(selectedTableArticleQuantity.ArticleID, efContext);
            int availableReservedQuantity = QuantityLogicHelper.GetAvailableQuantity(articleDetails) + tableArticleQuantity.Quantity;

            if (availableReservedQuantity >= selectedTableArticleQuantity.Quantity)
            {
                int oldQuantity = tableArticleQuantity.Quantity;

                tableArticleQuantity.Quantity = selectedTableArticleQuantity.Quantity;
                await _databaseService.EditTableArticleQuantity(tableArticleQuantity, efContext);

                if (selectedTableArticleQuantity.Quantity > oldQuantity)
                {
                    int quantityToIncrease = selectedTableArticleQuantity.Quantity - oldQuantity;

                    ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                    {
                        TableArticleQuantity = tableArticleQuantity,
                        ArticleDetails = articleDetails,
                        Quantity = quantityToIncrease,
                        DatabaseService = _databaseService,
                        EFContext = efContext
                    };

                    await QuantityLogicHelper.IncreaseReservedQuantity(articleHelperDetails);
                }
                else
                {
                    int quantityToDecrease = Math.Abs(selectedTableArticleQuantity.Quantity - oldQuantity);

                    ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                    {
                        TableArticleQuantity = tableArticleQuantity,
                        ArticleDetails = articleDetails,
                        Quantity = quantityToDecrease,
                        DatabaseService = _databaseService,
                        EFContext = efContext
                    };

                    await QuantityLogicHelper.DecreaseQuantityFromCell(articleHelperDetails);
                }
            }
            else
            {
                SelectedTableArticleQuantity.Quantity = tableArticleQuantity.Quantity;
                MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RaisePropertyChanged(nameof(TableArticleQuantities));
        }

        private async void DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            int quantityToRemove = SelectedTableArticleQuantity.Quantity;

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

            ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
            {
                TableArticleQuantity = tableArticleQuantity,
                ArticleDetails = articleDetails,

                DatabaseService = _databaseService,
                EFContext = efContext
            };

            await QuantityLogicHelper.DecreaseReservedQuantity(articleHelperDetails);

            TableArticleQuantity tableArticleQuantityLoad = await _databaseService.GetTableArticleQuantityByID(tableArticleQuantity.ID, efContext);

            TableArticleQuantities.Remove(tableArticleQuantity);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantityLoad, new EFContext());
        }

        private async void Logout()
        {
            using EFContext efContext = new EFContext();
            OnlineOrder.UserID = null;
            await _databaseService.EditOnlineOrder(OnlineOrder,efContext);

            _regionManager.RequestNavigate("MainRegion", "OnlineOrders");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
