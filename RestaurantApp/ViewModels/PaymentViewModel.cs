using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Enums;
using RestaurantApp.Models;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class PaymentViewModel : BindableBase, INavigationAware
    {
        private decimal _totalPrice;

        private IDatabaseService _databaseService;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private DelegateCommand _issueFakeBillCommand;

        private DelegateCommand _issueBillCommand;

        private DelegateCommand _getTotalPriceCommand;

        public PaymentViewModel(IDatabaseService databaseService, IRegionManager regionManager, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        public List<TableArticleQuantity> TableArticleQuantities { get; set; }

        public Table Table { get; set; }

        public PaymentType PaymentType { get; set; }

        public OnlineOrder OnlineOrder { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                _totalPrice = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand GetTotalPriceCommand
        {
            get
            {
                _getTotalPriceCommand = new DelegateCommand(GetTotalPrice);
                return _getTotalPriceCommand;
            }
        }

        public DelegateCommand IssueFakeBillCommand
        {
            get
            {
                _issueFakeBillCommand = new DelegateCommand(IssueFakeBill);
                return _issueFakeBillCommand;
            }
        }

        public DelegateCommand IssueBillCommand
        {
            get
            {
                _issueBillCommand = new DelegateCommand(IssueBill);
                return _issueBillCommand;
            }
        }

        private async Task<Bill> AddBillOnlineOrder(decimal cash, decimal change)
        {
            using EFContext efContext = new EFContext();

            decimal totalPrice = CalculateTotalPrice();
            Configuration configuration = await _databaseService.GetConfiguration();

            int billCounter = await IncreaseBillCounter();
            string registrationNumber = billCounter.ToString() + "/" + DateTime.Now.ToString("ddMMyyyy");

            Bill bill = new Bill
            {
                TotalPrice = totalPrice,
                Cash = cash,
                Change = change,
                PaymentType = PaymentType,
                OnlineOrderID = OnlineOrder.ID,
                UserID = (int)OnlineOrder.UserID,
                RegistrationNumber = registrationNumber
            };

            List<SoldArticleDetails> soldArticleDetails = null;

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

                ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                {
                    TableArticleQuantity = tableArticleQuantity,
                    EFContext = efContext,
                    DatabaseService = _databaseService,
                    ArticleDetails = articleDetails,
                };

                await QuantityLogicHelper.DecreaseReservedQuantity(articleHelperDetails);
                soldArticleDetails = await QuantityLogicHelper.DecreaseOriginalQuantity(articleHelperDetails);
            }

            await _databaseService.CreateBill(bill, efContext);

            foreach (SoldArticleDetails soldArticleDetail in soldArticleDetails)
            {
                soldArticleDetail.BillID = bill.ID;
                await _databaseService.AddSoldArticleDetails(soldArticleDetail, efContext);
            }

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                Article article = await _databaseService.GetArticleByID(tableArticleQuantity.ArticleID, efContext);
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

                SoldTableArticleQuantity soldTableArticleQuantity = new SoldTableArticleQuantity
                {
                    Article = article,
                    ArticleDetails = articleDetails,
                    Quantity = tableArticleQuantity.Quantity,
                    OnlineOrderID = OnlineOrder.ID,
                    Bill = bill
                };

                TableArticleQuantity tableArticleQuantityLoad = await _databaseService.GetTableArticleQuantityByID(tableArticleQuantity.ID, efContext);

                await _databaseService.DeleteTableArticleQuantity(tableArticleQuantityLoad, efContext);
                await _databaseService.AddTableArticleQuantity(soldTableArticleQuantity, efContext);
            }

            OnlineOrder.IsPayed = true;
            await _databaseService.EditOnlineOrder(OnlineOrder, efContext);

            User user = await _databaseService.GetUserByID(OnlineOrder.UserID ?? (int)default, efContext);
            user.IsActive = false;
            await _databaseService.EditUser(user, efContext);
            return bill;
        }

        private async Task<Bill> AddBill(decimal cash, decimal change)
        {
            using EFContext efContext = new EFContext();

            decimal totalPrice = CalculateTotalPrice();
            Configuration configuration = await _databaseService.GetConfiguration();

            int billCounter = await IncreaseBillCounter();
            string registrationNumber = billCounter.ToString() + "/" + DateTime.Now.ToString("ddMMyyyy");

            Bill bill = new Bill
            {
                TableID = Table.ID,
                TotalPrice = totalPrice,
                Cash = cash,
                Change = change,
                PaymentType = PaymentType,
                UserID = (int)Table.UserID,
                RegistrationNumber = registrationNumber
            };

            List<SoldArticleDetails> soldArticleDetails = null;
            List<SoldArticleDetails> totalSoldArticleDetails = new List<SoldArticleDetails>();

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

                ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                {
                    TableArticleQuantity = tableArticleQuantity,
                    EFContext = efContext,
                    DatabaseService = _databaseService,
                    ArticleDetails = articleDetails,
                };

                await QuantityLogicHelper.DecreaseReservedQuantity(articleHelperDetails);
                soldArticleDetails = await QuantityLogicHelper.DecreaseOriginalQuantity(articleHelperDetails);

                totalSoldArticleDetails.AddRange(soldArticleDetails);
            }

            await _databaseService.CreateBill(bill, efContext);

            foreach (SoldArticleDetails soldArticleDetail in totalSoldArticleDetails)
            {
                soldArticleDetail.BillID = bill.ID;
                await _databaseService.AddSoldArticleDetails(soldArticleDetail, efContext);
            }

            Table.InUse = false;
            await _databaseService.EditTable(Table, efContext);

            User user = await _databaseService.GetUserByID(Table.UserID ?? (int)default, efContext);
            user.IsActive = false;
            await _databaseService.EditUser(user, efContext);

            List<SoldTableArticleQuantity> soldTableArticleQuantities = new List<SoldTableArticleQuantity>();
            List<TableArticleQuantity> tableArticleQuantities = TableArticleQuantities.Select(x => x).Where(x => !(x is SoldTableArticleQuantity)).ToList();

            foreach (TableArticleQuantity tableArticleQuantity in tableArticleQuantities)
            {
                Article article = await _databaseService.GetArticleByID(tableArticleQuantity.ArticleID, efContext);
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

                SoldTableArticleQuantity soldTableArticleQuantity = new SoldTableArticleQuantity
                {
                    Article = article,
                    ArticleDetails = articleDetails,
                    TableID = tableArticleQuantity.TableID,
                    Quantity = tableArticleQuantity.Quantity,
                    BillID = bill.ID
                };

                TableArticleQuantity tableArticleQuantityLoad = await _databaseService.GetTableArticleQuantityByID(tableArticleQuantity.ID, efContext);

                await _databaseService.DeleteTableArticleQuantity(tableArticleQuantityLoad, efContext);
                await _databaseService.AddTableArticleQuantity(soldTableArticleQuantity, efContext);
            }

            return bill;
        }
        private void IssueFakeBill()
        {
            decimal change = 0;
            decimal cash = 0;
            decimal totalPrice = CalculateTotalPrice();

            Bill fakeBill = new Bill
            {
                TotalPrice = totalPrice,
                Cash = cash,
                Change = change,

                Table = new Table
                {
                    TableArticleQuantities = TableArticleQuantities
                },
            };

            DrawningHelper.DrawFakeBill(fakeBill);
        }

        private async Task<int> IncreaseBillCounter()
        {
            using EFContext efContext = new EFContext();

            Configuration configuration = await _databaseService.GetConfiguration();
            configuration.BillCounter += 1;

            await _databaseService.EditConfiguration(configuration, efContext);
            return configuration.BillCounter;
        }

        private async void IssueBill()
        {
            using EFContext efContext = new EFContext();

            decimal change = 0;
            decimal cash = 0;
            decimal totalPrice = CalculateTotalPrice();

            if (PaymentType == PaymentType.Cash)
            {
                DialogParameters dialogParameters = new DialogParameters()
                {
                    { "totalPrice", totalPrice },
                };

                _dialogService.ShowDialog("paymentDialog", dialogParameters, async result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        change = result.Parameters.GetValue<decimal>("change");
                        cash = result.Parameters.GetValue<decimal>("cash");

                        Bill bill = null;
                        User user = null;

                        if (Table is null)
                        {
                            user = await _databaseService.GetUserByID((int)OnlineOrder.UserID, efContext);
                            bill = await AddBillOnlineOrder(cash, change);
                            _regionManager.RequestNavigate("MainRegion", "OnlineOrders");
                        }
                        else
                        {
                            user = await _databaseService.GetUserByID((int)Table.UserID, efContext);
                            bill = await AddBill(cash, change);
                            _regionManager.RequestNavigate("MainRegion", "TableOrder");
                        }

                        DrawningHelper.DrawBill(bill, TableArticleQuantities, user);
                    }
                });
            }

            if (PaymentType == PaymentType.Card)
            {
                Bill bill = null;
                User user = null;

                if (Table is null)
                {
                    user = await _databaseService.GetUserByID((int)OnlineOrder.UserID, efContext);
                    bill = await AddBillOnlineOrder(0, 0);
                    _regionManager.RequestNavigate("MainRegion", "OnlineOrders");
                }
                else
                {
                    user = await _databaseService.GetUserByID((int)Table.UserID, efContext);
                    bill = await AddBill(0, 0);
                    _regionManager.RequestNavigate("MainRegion", "TableOrder");
                }

                DrawningHelper.DrawBill(bill, TableArticleQuantities, user);
            }
        }

        private void GetTotalPrice()
        {
            _totalPrice = CalculateTotalPrice();
            RaisePropertyChanged(nameof(TotalPrice));
        }

        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                totalPrice += tableArticleQuantity.Article.Price * tableArticleQuantity.Quantity;
            }

            return totalPrice;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Table = (Table)navigationContext.Parameters["table"];
            OnlineOrder = (OnlineOrder)navigationContext.Parameters["onlineOrder"];
            TableArticleQuantities = (List<TableArticleQuantity>)navigationContext.Parameters["tableArticleQuantities"];
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
