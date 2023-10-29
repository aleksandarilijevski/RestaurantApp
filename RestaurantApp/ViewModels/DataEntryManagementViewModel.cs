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
    public class DataEntryManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private ObservableCollection<DataEntry> _dataEntries;
        private DelegateCommand _loadDataEntriesCommand;
        private DelegateCommand _showDataEntryDetailsCommand;
        private DelegateCommand _clearFiltersCommand;
        private DelegateCommand _filterDataEntriesCommand;
        private int _dataEntryNumber;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private decimal _priceFrom;
        private decimal _priceTo;

        public DataEntryManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _dialogService = dialogService;
        }

        public int DataEntryNumber
        {
            get
            {
                return _dataEntryNumber;
            }

            set
            {
                _dataEntryNumber = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DateFrom
        {
            get
            {
                return _dateFrom;
            }

            set
            {
                _dateFrom = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DateTo
        {
            get
            {
                return _dateTo;
            }

            set
            {
                _dateTo = value;
                RaisePropertyChanged();
            }
        }

        public decimal PriceFrom
        {
            get
            {
                return _priceFrom;
            }

            set
            {
                _priceFrom = value;
                RaisePropertyChanged();
            }
        }

        public decimal PriceTo
        {
            get
            {
                return _priceTo;
            }

            set
            {
                _priceTo = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DataEntry> DataEntries
        {
            get
            {
                return _dataEntries;
            }

            set
            {
                _dataEntries = value;
                RaisePropertyChanged();
            }
        }

        public DataEntry SelectedDataEntry { get; set; }

        public DelegateCommand LoadDataEntriesCommand
        {
            get
            {
                _loadDataEntriesCommand = new DelegateCommand(LoadDataEntries);
                return _loadDataEntriesCommand;
            }
        }

        public DelegateCommand ShowDataEntryDetailsCommand
        {
            get
            {
                _showDataEntryDetailsCommand = new DelegateCommand(ShowDataEntryDetails);
                return _showDataEntryDetailsCommand;
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

        public DelegateCommand FilterDataEntriesCommand
        {
            get
            {
                _filterDataEntriesCommand = new DelegateCommand(FilterDataEntries);
                return _filterDataEntriesCommand;
            }
        }


        private async void ClearFilters()
        {
            DataEntries = await _databaseService.GetAllDataEntries();

            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            DataEntryNumber = 0;
            PriceFrom = 0;
            PriceTo = 0;
        }

        private async void FilterDataEntries()
        {
            ObservableCollection<DataEntry> originalDataEntries = await _databaseService.GetAllDataEntries();
            ObservableCollection<DataEntry> filteredDataEntries = new ObservableCollection<DataEntry>();

            if (DataEntryNumber != 0)
            {
                filteredDataEntries.AddRange(originalDataEntries.Where(x => x.DataEntryNumber == DataEntryNumber));
            }

            if (DateFrom != DateTime.MinValue && DateTo != DateTime.MinValue)
            {
                filteredDataEntries.AddRange(FilterByDateTime(originalDataEntries));
            }

            if (PriceFrom > 0 && PriceTo > 0)
            {
                filteredDataEntries.AddRange(originalDataEntries.Where(x => x.TotalAmount >= PriceFrom && x.TotalAmount <= PriceTo));
            }

            DataEntries = filteredDataEntries;
        }

        private List<DataEntry> FilterByDateTime(ObservableCollection<DataEntry> dataEntries)
        {
            List<DataEntry> filteredDataEntries = new List<DataEntry>();

            foreach (DataEntry dataEntry in dataEntries)
            {
                DateTime dataEntryCreatedDateTime = (DateTime)dataEntry.CreatedDateTime;

                if (dataEntryCreatedDateTime.Date >= DateFrom.Date && dataEntryCreatedDateTime.Date <= DateTo.Date)
                {
                    filteredDataEntries.Add(dataEntry);
                }
            }

            return filteredDataEntries;
        }

        private async void LoadDataEntries()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;

            DataEntries = await _databaseService.GetAllDataEntries();
        }

        private void ShowDataEntryDetails()
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                { "dataEntry",SelectedDataEntry }
            };

            _dialogService.ShowDialog("dataEntryDetailsDialog", dialogParameters, r => { });
        }
    }
}
