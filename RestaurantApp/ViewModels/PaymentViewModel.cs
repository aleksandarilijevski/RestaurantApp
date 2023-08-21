using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Enums;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
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
        private Table _table;
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

        private async Task<Bill> AddBill(decimal cash, decimal change)
        {
            decimal totalPrice = CalculateTotalPrice();

            Bill bill = new Bill
            {
                TableID = _table.ID,
                TotalPrice = totalPrice,
                Cash = cash,
                Change = change,
                PaymentType = PaymentType
            };

            await CreateBill(bill);

            List<SoldTableArticleQuantity> soldTableArticleQuantities = new List<SoldTableArticleQuantity>();
            List<TableArticleQuantity> tableArticleQuantities = _table.TableArticleQuantities.Select(x => x).Where(x => !(x is SoldTableArticleQuantity)).ToList();

            foreach (TableArticleQuantity tableArticleQuantity in tableArticleQuantities)
            {
               SoldTableArticleQuantity  soldTableArticleQuantity = new SoldTableArticleQuantity
                {
                    ArticleID = tableArticleQuantity.ArticleID,
                    Article = tableArticleQuantity.Article,
                    TableID = tableArticleQuantity.TableID,
                    Quantity = tableArticleQuantity.Quantity,
                    Bill = bill
                };

                soldTableArticleQuantities.Add(soldTableArticleQuantity);
            }

            await _databaseService.ModifyTableArticles(_table.ID, soldTableArticleQuantities, _table.TableArticleQuantities);
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
                        Bill bill = await AddBill(cash, change);
                        int billCounter = await IncreaseBillCounter();
                        DrawningHelper.DrawBill(bill, billCounter, _tableArticleQuantities);
                        _regionManager.RequestNavigate("MainRegion", "TableOrder");
                    }
                });
            }

            if (PaymentType == PaymentType.Card)
            {
                Bill bill = await AddBill(0, 0);
                int billCounter = await IncreaseBillCounter();
                DrawningHelper.DrawBill(bill, billCounter, _tableArticleQuantities);
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
            _tableArticleQuantities = (List<TableArticleQuantity>)navigationContext.Parameters["tableArticleQuantities"];
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
