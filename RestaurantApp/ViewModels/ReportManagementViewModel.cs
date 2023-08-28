using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels
{
    public class ReportManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand _loadAllBillsCommand;
        private DateTime _dateFrom = DateTime.Now;
        private DateTime _dateTo = DateTime.Now;
        private ObservableCollection<Bill> _bills = new ObservableCollection<Bill>();
        private DelegateCommand _showReportDetailsCommand;
        private DelegateCommand _filterCommand;
        private DelegateCommand _clearFiltersCommand;
        private Bill _selectedBill;
        private decimal _total;
        private string _filterBillCounter;
        private string _filterTableID;
        private DateTime _filterDateFrom;
        private DateTime _filterDateTo;

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
            Bills = new ObservableCollection<Bill>(bills);
        }

        private async void Filter()
        {
            List<Bill> bills = await _databaseService.GetAllBills();
            List<Bill> filteredBills = new List<Bill>();

            if (int.TryParse(FilterTableID, out int tableId) == true)
            {
                filteredBills.AddRange(FilterByTableID(bills, tableId));
            }

            if (int.TryParse(FilterBillCounter, out int billCounter) == true)
            {
                filteredBills.AddRange(FilterByBillCounter(bills, billCounter));
            }

            if (FilterDateFrom != DateTime.MinValue && FilterDateTo != DateTime.MinValue)
            {
                filteredBills.AddRange(FilterByDateTime(bills));
            }

            Bills = new ObservableCollection<Bill>(filteredBills);
        }

        private List<Bill> FilterByTableID(List<Bill> bills, int tableId)
        {
            return bills.Where(x => x.TableID == tableId).ToList();
        }

        private List<Bill> FilterByBillCounter(List<Bill> bills, int billCounter)
        {
            return bills.Where(x => x.RegistrationNumber.Contains(billCounter.ToString() + "/")).ToList();
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
        }
    }
}
