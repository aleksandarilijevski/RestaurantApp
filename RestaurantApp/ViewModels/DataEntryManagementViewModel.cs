using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class DataEntryManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private ObservableCollection<DataEntry> _dataEntries;
        private DelegateCommand _loadDataEntriesCommand;
        private DelegateCommand _showDataEntryDetailsCommand;

        public DataEntryManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _dialogService = dialogService;
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

        private async void LoadDataEntries()
        {
            List<DataEntry> allDataEntries = await _databaseService.GetAllDataEntries();
            DataEntries = new ObservableCollection<DataEntry>(allDataEntries);
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
