using ClosedXML.Excel;
using EntityFramework.Enums;
using EntityFramework.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class ReportManagementViewModel : BindableBase
    {
        private decimal _total;

        private string _filterBillCounter;

        private string _filterTableID;

        private string _totalProfit;

        private string _filterOnlineOrderID;

        private DateTime _filterDateFrom;

        private DateTime _filterDateTo;

        private IDatabaseService _databaseService;

        private IDialogService _dialogService;

        private IRegionManager _regionManager;

        private DelegateCommand _loadAllBillsCommand;

        private DelegateCommand _showReportDetailsCommand;

        private DelegateCommand _filterCommand;

        private DelegateCommand _clearFiltersCommand;

        private DelegateCommand _exportToExcelCommand;

        private List<Bill> _originalBills = new List<Bill>();

        private ObservableCollection<Bill> _bills = new ObservableCollection<Bill>();

        public ReportManagementViewModel(IDatabaseService databaseService, IDialogService dialogService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _dialogService = dialogService;
            _regionManager = regionManager;
        }

        public User User { get; set; }

        public List<SoldArticleDetails> SoldArticleDetails { get; set; }

        public Bill SelectedBill { get; set; }

        public decimal Total
        {
            get
            {
                return _total;
            }

            set
            {
                _total = value;
                RaisePropertyChanged();
            }
        }

        public string TotalProfit
        {
            get
            {
                return _totalProfit;
            }

            set
            {
                _totalProfit = value;
                RaisePropertyChanged();
            }
        }

        public string FilterBillCounter
        {
            get
            {
                return _filterBillCounter;
            }

            set
            {
                _filterBillCounter = value;
                RaisePropertyChanged();
            }
        }

        public string FilterTableID
        {
            get
            {
                return _filterTableID;
            }

            set
            {
                _filterTableID = value;
                RaisePropertyChanged();
            }
        }

        public string FilterOnlineOrderID
        {
            get
            {
                return _filterOnlineOrderID;
            }

            set
            {
                _filterOnlineOrderID = value;
                RaisePropertyChanged();
            }
        }

        public DateTime FilterDateFrom
        {
            get
            {
                return _filterDateFrom;
            }

            set
            {
                _filterDateFrom = value;
                RaisePropertyChanged();
            }
        }

        public DateTime FilterDateTo
        {
            get
            {
                return _filterDateTo;
            }

            set
            {
                _filterDateTo = value;
                RaisePropertyChanged();
            }
        }

        public List<Bill> OriginalBills
        {
            get
            {
                return _originalBills;
            }

            set
            {
                _originalBills = value;
            }
        }

        public ObservableCollection<Bill> Bills
        {
            get
            {
                return _bills;
            }

            set
            {
                _bills = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ShowReportDetailsCommand
        {
            get
            {
                _showReportDetailsCommand = new DelegateCommand(ShowReportDetails);
                return _showReportDetailsCommand;
            }
        }

        public DelegateCommand LoadAllBillsCommand
        {
            get
            {
                _loadAllBillsCommand = new DelegateCommand(LoadAllBills);
                return _loadAllBillsCommand;
            }
        }

        public DelegateCommand FilterCommand
        {
            get
            {
                _filterCommand = new DelegateCommand(Filter);
                return _filterCommand;
            }
        }

        public DelegateCommand ClearFiltersCommand
        {
            get
            {
                _clearFiltersCommand = new DelegateCommand(ClearFilters);
                return _clearFiltersCommand;
            }
        }

        public DelegateCommand ExportToExcelCommand
        {
            get
            {

                _exportToExcelCommand = new DelegateCommand(ExportToExcel);
                return _exportToExcelCommand;
            }
        }

        private void ShowReportDetails()
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                { "bill", SelectedBill }
            };

            _dialogService.ShowDialog("reportDetailsDialog", dialogParameters, r => { });
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

        private async void LoadAllBills()
        {
            if (Bills is not null)
            {
                Bills.Clear();
            }

            TotalProfit = "Total profit : N/A";

            if (LoggedUserHelper.LoggedUser is null)
            {
                bool result = UserLogin();

                if (!result)
                {
                    _regionManager.RequestNavigate("MainRegion", "Options");
                    return;
                }

                if (User.UserRole is UserRole.Waiter)
                {
                    MessageBox.Show("Waiter can't access to report management!", "Access forbidden", MessageBoxButton.OK, MessageBoxImage.Error);
                    _regionManager.RequestNavigate("MainRegion", "Options");
                    return;
                }

                LoggedUserHelper.LoggedUser = User;
            }

            SoldArticleDetails = await _databaseService.GetAllSoldArticleDetails();

            List<Bill> bills = await _databaseService.GetAllBills();
            bills.OrderBy(x => x.CreatedDateTime);

            OriginalBills = bills;
            Bills = new ObservableCollection<Bill>(OriginalBills);

            decimal totalProfit = CalculateTotalProfit(SoldArticleDetails);
            TotalProfit = "Total profit : " + totalProfit.ToString("0.00");

            FilterDateFrom = DateTime.MinValue;
            FilterDateTo = DateTime.MinValue;
        }

        private async void Filter()
        {
            List<Bill> originalBills = OriginalBills;
            List<Bill> filteredBills = new List<Bill>();

            if (int.TryParse(FilterTableID, out int tableId) == true)
            {
                filteredBills.AddRange(originalBills.Where(x => x.TableID == tableId).ToList());
            }

            if (int.TryParse(FilterOnlineOrderID, out int onlineOrderId) == true)
            {
                filteredBills.AddRange(originalBills.Where(x => x.OnlineOrderID == onlineOrderId).ToList());
            }

            if (int.TryParse(FilterBillCounter, out int billCounter) == true)
            {
                filteredBills.AddRange(originalBills.Where(x => x.RegistrationNumber.Contains(billCounter.ToString() + "/")).ToList());
            }

            if (FilterDateFrom != DateTime.MinValue && FilterDateTo != DateTime.MinValue)
            {
                filteredBills.AddRange(FilterByDateTime(originalBills));
            }

            List<SoldArticleDetails> soldArticleDetails = await _databaseService.GetAllSoldArticleDetails();
            List<SoldArticleDetails> filteredSoldArticleDetails = new List<SoldArticleDetails>();

            foreach (SoldArticleDetails soldArticleDetail in soldArticleDetails)
            {
                foreach (Bill bill in filteredBills)
                {
                    if (soldArticleDetail.BillID == bill.ID)
                    {
                        filteredSoldArticleDetails.Add(soldArticleDetail);
                    }
                }
            }

            Bills = new ObservableCollection<Bill>(filteredBills);
            decimal totalProfit = CalculateTotalProfit(filteredSoldArticleDetails);
            TotalProfit = "Total profit : " + totalProfit.ToString("0.00");
        }

        private List<Bill> FilterByDateTime(List<Bill> bills)
        {
            List<Bill> filteredBills = new List<Bill>();

            foreach (Bill bill in bills)
            {
                DateTime billCreatedDateTime = (DateTime)bill.CreatedDateTime;

                if (billCreatedDateTime.Date >= FilterDateFrom.Date && billCreatedDateTime.Date <= FilterDateTo.Date)
                {
                    filteredBills.Add(bill);
                }
            }

            return filteredBills;
        }

        private async void ClearFilters()
        {
            FilterBillCounter = string.Empty;
            FilterTableID = string.Empty;
            FilterOnlineOrderID = string.Empty;
            FilterDateFrom = DateTime.Now;
            FilterDateTo = DateTime.Now;

            List<Bill> bills = await _databaseService.GetAllBills();
            Bills = new ObservableCollection<Bill>(bills);

            decimal totalProfit = CalculateTotalProfit(SoldArticleDetails);
            TotalProfit = "Total profit : " + totalProfit.ToString("0.00");
        }

        private async void ExportToExcel()
        {
            if (Bills.Count == 0)
            {
                MessageBox.Show("There is nothing to be exported!", "Export to excel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string defaultFileName = DateTime.Now.ToString("dd MM yyyy hh mm ss") + " Report";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = defaultFileName;
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Microsoft Excel (.xlsx)|*.xlsx";
            bool? result = saveFileDialog.ShowDialog();

            if (result == false)
            {
                return;
            }

            string fileLocation = saveFileDialog.FileName;

            if (Path.GetExtension(fileLocation) != ".xlsx")
            {
                MessageBox.Show("Invalid format!", "Export to excel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 20;

            int cellIndex = 1;

            foreach (Bill bill in Bills)
            {
                List<SoldTableArticleQuantity> soldTableArticleQuantities = null;

                if (bill.Table is not null)
                {
                    soldTableArticleQuantities = bill.Table.TableArticleQuantities.OfType<SoldTableArticleQuantity>().ToList();
                }

                if (bill.OnlineOrder is not null)
                {
                    soldTableArticleQuantities = bill.OnlineOrder.TableArticleQuantities.OfType<SoldTableArticleQuantity>().ToList();
                }

                worksheet.Cell(cellIndex, 1).Value = "Table ID";
                worksheet.Cell(cellIndex, 2).Value = "Payment type";
                worksheet.Cell(cellIndex, 3).Value = "Total price";
                worksheet.Cell(cellIndex, 4).Value = "Total profit";
                worksheet.Cell(cellIndex, 5).Value = "Created date";
                worksheet.Row(cellIndex).Style.Font.Bold = true;
                cellIndex++;

                worksheet.Cell(cellIndex, 1).Value = bill.TableID;
                worksheet.Cell(cellIndex, 2).Value = bill.PaymentType.ToString();
                worksheet.Cell(cellIndex, 3).Value = bill.TotalPrice;

                decimal totalProfitForArticle = 0;

                List<SoldArticleDetails> filteredSoldArticleDetails = SoldArticleDetails.Where(x => x.BillID == bill.ID).ToList();

                foreach (SoldArticleDetails soldArticleDetail in filteredSoldArticleDetails)
                {
                    decimal profit = soldArticleDetail.ArticlePrice - soldArticleDetail.EntryPrice;
                    totalProfitForArticle += soldArticleDetail.SoldQuantity * profit;
                }

                worksheet.Cell(cellIndex, 4).Value = totalProfitForArticle;
                worksheet.Cell(cellIndex, 5).Value = bill.CreatedDateTime;
                cellIndex++;

                worksheet.Cell(cellIndex, 1).Value = "Article name";
                worksheet.Cell(cellIndex, 2).Value = "Article price";
                worksheet.Cell(cellIndex, 3).Value = "Sold quantity";
                worksheet.Cell(cellIndex, 4).Value = "Total price";
                worksheet.Row(cellIndex).Style.Font.Bold = true;
                cellIndex++;

                foreach (SoldTableArticleQuantity soldTableArticleQuantity in soldTableArticleQuantities)
                {
                    if (soldTableArticleQuantity.BillID == bill.ID)
                    {
                        worksheet.Cell(cellIndex, 1).Value = soldTableArticleQuantity.Article.Name;
                        worksheet.Cell(cellIndex, 2).Value = soldTableArticleQuantity.Article.Price;
                        worksheet.Cell(cellIndex, 3).Value = soldTableArticleQuantity.Quantity;
                        worksheet.Cell(cellIndex, 4).Value = soldTableArticleQuantity.Quantity * soldTableArticleQuantity.Article.Price;
                        cellIndex++;
                    }
                }

                cellIndex += 3;
                worksheet.Cell(cellIndex, 1).Value = string.Empty;
            }

            decimal totalProfit = CalculateTotalProfit(SoldArticleDetails);
            worksheet.Row(cellIndex).Style.Font.Bold = true;
            worksheet.Cell(cellIndex, 1).Value = "Total profit";
            worksheet.Cell(cellIndex, 2).Value = totalProfit;

            workbook.SaveAs(fileLocation);
        }

        private decimal CalculateTotalProfit(List<SoldArticleDetails> soldArticleDetails)
        {
            decimal totalProfit = 0;

            foreach (SoldArticleDetails soldArticleDetail in soldArticleDetails)
            {
                decimal profit = soldArticleDetail.ArticlePrice - soldArticleDetail.EntryPrice;
                totalProfit += soldArticleDetail.SoldQuantity * profit;
            }

            return totalProfit;
        }
    }
}
