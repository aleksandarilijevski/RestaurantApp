using ClosedXML.Excel;
using EntityFramework.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
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
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand _loadAllBillsCommand;
        private List<Bill> _originalBills = new List<Bill>();
        private ObservableCollection<Bill> _bills = new ObservableCollection<Bill>();
        private DelegateCommand _showReportDetailsCommand;
        private DelegateCommand _filterCommand;
        private DelegateCommand _clearFiltersCommand;
        private DelegateCommand _exportToExcelCommand;
        private decimal _total;
        private string _filterBillCounter;
        private string _filterTableID;
        private string _totalProfit;

        public ReportManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _dialogService = dialogService;
        }

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

        public DateTime FilterDateFrom { get; set; }

        public DateTime FilterDateTo { get; set; }

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

        public List<SoldArticleDetails> SoldArticleDetails { get; set; }

        public Bill SelectedBill { get; set; }

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

        private async void LoadAllBills()
        {
            List<Bill> bills = await _databaseService.GetAllBills();
            bills.OrderBy(x => x.CreatedDateTime);

            OriginalBills = bills;
            Bills = new ObservableCollection<Bill>(OriginalBills);

            SoldArticleDetails = await _databaseService.GetAllSoldArticleDetails();

            decimal totalProfit = CalculateTotalProfit();
            TotalProfit = "Total profit : " + totalProfit.ToString("0.00");
        }

        private void Filter()
        {
            List<Bill> originalBills = OriginalBills;

            if (int.TryParse(FilterTableID, out int tableId) == true)
            {
                originalBills = originalBills.Where(x => x.TableID == tableId).ToList();
            }

            if (int.TryParse(FilterBillCounter, out int billCounter) == true)
            {
                originalBills = originalBills.Where(x => x.RegistrationNumber.Contains(billCounter.ToString() + "/")).ToList();
            }

            if (FilterDateFrom != DateTime.MinValue && FilterDateTo != DateTime.MinValue)
            {
                originalBills = FilterByDateTime(originalBills);
            }

            Bills = new ObservableCollection<Bill>(originalBills);
            decimal totalProfit = CalculateTotalProfit();
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
            FilterDateFrom = DateTime.MinValue;
            FilterDateTo = DateTime.MinValue;

            List<Bill> bills = await _databaseService.GetAllBills();
            Bills = new ObservableCollection<Bill>(bills);

            decimal totalProfit = CalculateTotalProfit();
            TotalProfit = "Total profit : " + totalProfit.ToString("0.00");
        }

        private void ExportToExcel()
        {
            if (Bills.Count == 0)
            {
                MessageBox.Show("There is nothing to be exported!", "Export to excel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string defaultFileName = DateTime.Now.ToString("dd MM yyyy hh mm ss");

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

                decimal profit = 0;

                var filteredSoldTableArticleQuantities = soldTableArticleQuantities.Where(x => x.BillID == bill.ID).ToList();

                foreach (SoldTableArticleQuantity soldTableArticleQuantity in filteredSoldTableArticleQuantities)
                {
                    int quantity = filteredSoldTableArticleQuantities.Sum(x => x.Quantity);

                    foreach (ArticleDetails articleDetail in soldTableArticleQuantity.ArticleDetails)
                    {
                        profit = bill.TotalPrice - (quantity * articleDetail.EntryPrice);
                    }
                }

                worksheet.Cell(cellIndex, 4).Value = profit;
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

            decimal totalProfit = CalculateTotalProfit();
            worksheet.Row(cellIndex).Style.Font.Bold = true;
            worksheet.Cell(cellIndex, 1).Value = "Total profit";
            worksheet.Cell(cellIndex, 2).Value = totalProfit;

            workbook.SaveAs(fileLocation);
        }

        private decimal CalculateTotalProfit()
        {
            using EFContext efContext = new EFContext();

            decimal totalProfit = 0;
            decimal totalPrice = 0;

            foreach (Bill bill in Bills)
            {
                totalPrice += bill.TotalPrice;
            }

            foreach (SoldArticleDetails soldArticleDetail in SoldArticleDetails)
            {
                totalProfit += soldArticleDetail.EntryPrice * soldArticleDetail.SoldQuantity;
            }

            totalPrice -= totalProfit;

            return totalProfit;
        }
    }
}
