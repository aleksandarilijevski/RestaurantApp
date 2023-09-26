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
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private IDialogService _dialogService;
        private decimal _totalPrice;
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

        public PaymentType PaymentType { get; set; }

        public OnlineOrder OnlineOrder { get; set; }

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

            await _databaseService.CreateBill(bill, efContext);

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

                ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                {
                    TableArticleQuantity = tableArticleQuantity,
                    EFContext = efContext,
                    DatabaseService = _databaseService,
                    ArticleDetails = articleDetails,
                    BillID = bill.ID
                };

                await QuantityLogicHelper.DecreaseReservedQuantity(articleHelperDetails);
                await QuantityLogicHelper.DecreaseOriginalQuantity(articleHelperDetails);
            }

            List<SoldTableArticleQuantity> soldTableArticleQuantities = new List<SoldTableArticleQuantity>();

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
            OnlineOrder.UserID = null;
            await _databaseService.EditOnlineOrder(OnlineOrder, efContext);
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

            await _databaseService.CreateBill(bill, efContext);

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

                ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                {
                    TableArticleQuantity = tableArticleQuantity,
                    EFContext = efContext,
                    DatabaseService = _databaseService,
                    ArticleDetails = articleDetails,
                    BillID = bill.ID
                };

                await QuantityLogicHelper.DecreaseReservedQuantity(articleHelperDetails);
                await QuantityLogicHelper.DecreaseOriginalQuantity(articleHelperDetails);
            }

            //Moved to executed before decreasing original quantity
            //await _databaseService.CreateBill(bill, efContext);

            Table.InUse = false;
            await _databaseService.EditTable(Table, efContext);

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

        private async Task RemoveUserFromTable()
        {
            Table.UserID = null;
            await _databaseService.EditTable(Table, new EFContext());
        }

        private async Task RemoveUserFromOnlineOrder()
        {
            OnlineOrder.UserID = null;
            await _databaseService.EditOnlineOrder(OnlineOrder, new EFContext());
        }

        private async void IssueBill()
        {
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
                            user = await _databaseService.GetUserByID((int)OnlineOrder.UserID, new EFContext());
                            bill = await AddBillOnlineOrder(cash, change);
                            await RemoveUserFromTable();
                        }
                        else
                        {
                            user = await _databaseService.GetUserByID((int)Table.UserID, new EFContext());
                            bill = await AddBill(cash, change);
                            await RemoveUserFromOnlineOrder();
                        }

                        DrawningHelper.DrawBill(bill, TableArticleQuantities, user);
                        _regionManager.RequestNavigate("MainRegion", "TableOrder");
                    }
                });
            }

            if (PaymentType == PaymentType.Card)
            {
                Bill bill = null;
                User user = null;

                if (Table is null)
                {
                    user = await _databaseService.GetUserByID((int)OnlineOrder.UserID, new EFContext());
                    bill = await AddBillOnlineOrder(0, 0);
                    await RemoveUserFromOnlineOrder();
                }
                else
                {
                    user = await _databaseService.GetUserByID((int)Table.UserID, new EFContext());
                    bill = await AddBill(0, 0);
                    await RemoveUserFromTable();
                }

                DrawningHelper.DrawBill(bill, TableArticleQuantities, user);
                _regionManager.RequestNavigate("MainRegion", "TableOrder");
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
    }
}
