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
using System.Reflection;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

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
        private Bill _selectedBill;
        private decimal _total;
        private string _filterBillCounter;
        private string _filterTableID;
        private DateTime _filterDateFrom;
        private DateTime _filterDateTo;
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

        public DateTime FilterDateFrom
        {
            get
            {
                return _filterDateFrom;
            }

            set
            {
                _filterDateFrom = value;
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

        public Bill SelectedBill
        {
            get
            {
                return _selectedBill;
            }

            set
            {
                _selectedBill = value;
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

        private async void LoadAllBills()
        {
            List<Bill> bills = await _databaseService.GetAllBills();
            bills.OrderBy(x => x.CreatedDateTime);

            OriginalBills = bills;
            Bills = new ObservableCollection<Bill>(OriginalBills);

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
            string defaultFileName = DateTime.Now.ToString("dd MM yyyy hh mm ss");

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = defaultFileName;
            saveFileDialog.DefaultExt = ".xls";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Microsoft Excel (.xls)|*.xls";
            bool? result = saveFileDialog.ShowDialog();

            if (result == false)
            {
                return;
            }

            string fileLocation = saveFileDialog.FileName;

            if (Path.GetExtension(fileLocation) != ".xls")
            {
                MessageBox.Show("Invalid format!", "Export to excel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = (Excel.Workbook)(excel.Workbooks.Add(Missing.Value));
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;

            worksheet.Columns[1].ColumnWidth = 20;
            worksheet.Columns[2].ColumnWidth = 15;
            worksheet.Columns[3].ColumnWidth = 15;
            worksheet.Columns[4].ColumnWidth = 15;
            worksheet.Columns[5].ColumnWidth = 20;

            int cellIndex = 1;

            foreach (Bill bill in Bills)
            {
                List<TableArticleQuantity> soldTableArticleQuantities = bill.Table.TableArticleQuantities.Where(x => (x is SoldTableArticleQuantity)).ToList();

                worksheet.Cells[cellIndex, 1] = "Table ID";
                worksheet.Cells[cellIndex, 2] = "Payment type";
                worksheet.Cells[cellIndex, 3] = "Total price";
                worksheet.Cells[cellIndex, 4] = "Total profit";
                worksheet.Cells[cellIndex, 5] = "Created date";

                worksheet.Cells[cellIndex, 1].EntireRow.Font.Bold = true;

                cellIndex++;

                worksheet.Cells[cellIndex, 1] = bill.TableID;
                worksheet.Cells[cellIndex, 2] = bill.PaymentType;
                worksheet.Cells[cellIndex, 3] = bill.TotalPrice;

                decimal profit = 0;

                List<TableArticleQuantity> filteredSoldTableArticleQuantities = soldTableArticleQuantities.Where(x => x.BillID == bill.ID).ToList();

                foreach (SoldTableArticleQuantity soldTableArticleQuantity in filteredSoldTableArticleQuantities)
                {
                    int quantity = filteredSoldTableArticleQuantities.Sum(x => x.Quantity);

                    foreach (ArticleDetails articleDetail in soldTableArticleQuantity.ArticleDetails)
                    {
                        profit = bill.TotalPrice - (quantity * articleDetail.EntryPrice);
                    }
                }

                worksheet.Cells[cellIndex, 4] = profit;
                worksheet.Cells[cellIndex, 5] = bill.CreatedDateTime;

                cellIndex++;
                worksheet.Cells[cellIndex, 1] = "Article name";
                worksheet.Cells[cellIndex, 2] = "Article price";
                worksheet.Cells[cellIndex, 3] = "Sold quantity";
                worksheet.Cells[cellIndex, 4] = "Total price";
                worksheet.Cells[cellIndex, 1].EntireRow.Font.Bold = true;

                cellIndex++;

                foreach (SoldTableArticleQuantity soldTableArticleQuantity in soldTableArticleQuantities)
                {
                    if (soldTableArticleQuantity.BillID == bill.ID)
                    {
                        worksheet.Cells[cellIndex, 1] = soldTableArticleQuantity.Article.Name;
                        worksheet.Cells[cellIndex, 2] = soldTableArticleQuantity.Article.Price;
                        worksheet.Cells[cellIndex, 3] = soldTableArticleQuantity.Quantity;
                        worksheet.Cells[cellIndex, 4] = soldTableArticleQuantity.Quantity * soldTableArticleQuantity.Article.Price;
                        cellIndex++;
                    }
                }

                cellIndex += 3;
                worksheet.Cells[cellIndex] = string.Empty;
            }

            decimal totalProfit = CalculateTotalProfit();
            worksheet.Cells[cellIndex, 1].EntireRow.Font.Bold = true;
            worksheet.Cells[cellIndex, 1] = "Total profit";
            worksheet.Cells[cellIndex, 2] = totalProfit;

            workbook.SaveAs(fileLocation, Excel.XlFileFormat.xlWorkbookNormal);
        }

        private decimal CalculateTotalProfit()
        {
            decimal totalProfit = 0;
            decimal totalPrice = 0;

            foreach (Bill bill in Bills)
            {
                List<TableArticleQuantity> filteredSoldTableArticleQuantity = bill.Table.TableArticleQuantities.Where(x => x.BillID == bill.ID).ToList();
                totalPrice += bill.TotalPrice;

                foreach (SoldTableArticleQuantity soldTableArticleQuantity in filteredSoldTableArticleQuantity)
                {
                    foreach (ArticleDetails articleDetail in soldTableArticleQuantity.ArticleDetails)
                    {
                        totalProfit += articleDetail.EntryPrice * soldTableArticleQuantity.Quantity;
                    }
                }
            }

            totalProfit = totalPrice - totalProfit;

            return totalProfit;
        }

    }
}
