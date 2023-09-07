using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Enums;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class PaymentViewModel : BindableBase, INavigationAware
    {
        //Private fields
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private IDialogService _dialogService;
        private decimal _totalPrice;
        private Table _table;
        private OnlineOrder _onlineOrder;
        private List<TableArticleQuantity> _tableArticleQuantities;
        private DelegateCommand _issueFakeBillCommand;
        private DelegateCommand _issueBillCommand;
        private DelegateCommand _getTotalPriceCommand;
        private PaymentType _paymentType;

        public PaymentViewModel(IDatabaseService databaseService, IRegionManager regionManager, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        //Public properties
        public List<TableArticleQuantity> TableArticleQuantities
        {
            get
            {
                return _tableArticleQuantities;
            }

            set
            {
                _tableArticleQuantities = value;
            }
        }

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

        public PaymentType PaymentType
        {
            get
            {
                return _paymentType;
            }

            set
            {
                _paymentType = value;
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

        //DelegateCommands
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
                RegistrationNumber = registrationNumber
            };

            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(tableArticleQuantity.ArticleID,efContext);
                await DecreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity, efContext);
                await DecreaseOriginalQuantity(articleDetails, tableArticleQuantity.Quantity, efContext);
            }

            await _databaseService.CreateBillContext(bill,efContext);


            List<SoldTableArticleQuantity> soldTableArticleQuantities = new List<SoldTableArticleQuantity>();

            //OnlineOrder.TableArticleQuantities
            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                Article article = await _databaseService.GetArticleByIDContext(tableArticleQuantity.ArticleID, efContext);
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(tableArticleQuantity.ArticleID, efContext);

                SoldTableArticleQuantity soldTableArticleQuantity = new SoldTableArticleQuantity
                {
                    //ArticleID = tableArticleQuantity.ArticleID,
                    Article = article,
                    ArticleDetails = articleDetails,
                    Quantity = tableArticleQuantity.Quantity,
                    Bill = bill
                };

                //soldTableArticleQuantities.Add(soldTableArticleQuantity);
                await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity);
                await _databaseService.AddTableArticleQuantityContext(soldTableArticleQuantity, efContext);
            }

            //await _databaseService.EditOnlineOrder(OnlineOrder);
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
                TableID = _table.ID,
                TotalPrice = totalPrice,
                Cash = cash,
                Change = change,
                PaymentType = PaymentType,
                RegistrationNumber = registrationNumber
            };


            foreach (TableArticleQuantity tableArticleQuantity in TableArticleQuantities)
            {
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(tableArticleQuantity.ArticleID, efContext);
                await DecreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity,efContext);
                await DecreaseOriginalQuantity(articleDetails, tableArticleQuantity.Quantity,efContext);
            }

            await _databaseService.CreateBillContext(bill,efContext);

            _table.InUse = false;
            await _databaseService.EditTable(_table);

            List<SoldTableArticleQuantity> soldTableArticleQuantities = new List<SoldTableArticleQuantity>();
            List<TableArticleQuantity> tableArticleQuantities = TableArticleQuantities.Select(x => x).Where(x => !(x is SoldTableArticleQuantity)).ToList();

            foreach (TableArticleQuantity tableArticleQuantity in tableArticleQuantities)
            {
                Article article = await _databaseService.GetArticleByIDContext(tableArticleQuantity.ArticleID,efContext);
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(tableArticleQuantity.ArticleID,efContext);

                SoldTableArticleQuantity soldTableArticleQuantity = new SoldTableArticleQuantity
                {
                    //ArticleID = tableArticleQuantity.ArticleID,
                    Article = article,
                    ArticleDetails = articleDetails,
                    TableID = tableArticleQuantity.TableID,
                    Quantity = tableArticleQuantity.Quantity,
                    BillID = bill.ID
                };

                //soldTableArticleQuantities.Add(soldTableArticleQuantity);
                await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity);
                await _databaseService.AddTableArticleQuantityContext(soldTableArticleQuantity,efContext);
            }

            //await _databaseService.ModifyTableArticlesContext(TableArticleQuantities, soldTableArticleQuantities,efContext);
            return bill;
        }

        private async Task DecreaseOriginalQuantity(List<ArticleDetails> articleDetails, int originalQuantityToDecrease, EFContext efContext)
        {
            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (originalQuantityToDecrease <= 0)
                    break;

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, originalQuantityToDecrease);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = articleDetail.ReservedQuantity;
                }

                articleDetail.OriginalQuantity -= quantityToReserve;

                originalQuantityToDecrease -= quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail);
            }
        }

        private async Task DecreaseReservedQuantity(List<ArticleDetails> articleDetails, int reservedQuantityToBeDecreased, EFContext efContext)
        {
            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (reservedQuantityToBeDecreased <= 0)
                    break; // No more quantity to sell, exit the loop

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, reservedQuantityToBeDecreased);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = articleDetail.ReservedQuantity;
                }

                if (availableQuantity == 0)
                {
                    quantityToReserve = reservedQuantityToBeDecreased;
                }

                if (articleDetail.OriginalQuantity >= articleDetail.ReservedQuantity)
                {
                    quantityToReserve = reservedQuantityToBeDecreased;
                }

                // Reserve quantityToReserve for the current articleDetail
                articleDetail.ReservedQuantity -= quantityToReserve;

                // Reduce remainingQuantityToSell
                reservedQuantityToBeDecreased -= quantityToReserve;

                await _databaseService.EditArticleDetailsContext(articleDetail,efContext);
            }
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
                    TableArticleQuantities = _tableArticleQuantities
                },
            };

            DrawningHelper.DrawFakeBill(fakeBill);
        }

        private async Task<int> IncreaseBillCounter()
        {
            Configuration configuration = await _databaseService.GetConfiguration();
            configuration.BillCounter += 1;
            await _databaseService.EditConfiguration(configuration);
            return configuration.BillCounter;
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

                        if (_table is null)
                        {
                            bill = await AddBillOnlineOrder(cash, change);
                        }
                        else
                        {
                            bill = await AddBill(cash, change);
                        }

                        DrawningHelper.DrawBill(bill, _tableArticleQuantities);
                        _regionManager.RequestNavigate("MainRegion", "TableOrder");
                    }
                });
            }

            if (PaymentType == PaymentType.Card)
            {
                Bill bill = null;

                if (_table is null)
                {
                    bill = await AddBillOnlineOrder(0, 0);
                }
                else
                {
                    bill = await AddBill(0, 0);
                }


                DrawningHelper.DrawBill(bill, _tableArticleQuantities);
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

            foreach (TableArticleQuantity tableArticleQuantity in _tableArticleQuantities)
            {
                totalPrice += tableArticleQuantity.Article.Price * tableArticleQuantity.Quantity;
            }

            return totalPrice;
        }

        private async Task<Bill> CreateBill(Bill bill)
        {
            await _databaseService.CreateBill(bill);
            return bill;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _table = (Table)navigationContext.Parameters["table"];
            _onlineOrder = (OnlineOrder)navigationContext.Parameters["onlineOrder"];
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
