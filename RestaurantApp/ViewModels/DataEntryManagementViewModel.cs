using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class DataEntryManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private ObservableCollection<DataEntry> _dataEntries;
        private DelegateCommand _loadDataEntriesCommand;

        public DataEntryManagementViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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

        public DelegateCommand LoadDataEntriesCommand
        {
            get
            {
                _loadDataEntriesCommand = new DelegateCommand(LoadDataEntries);
                return _loadDataEntriesCommand;
            }
        }

        private async void LoadDataEntries()
        {
            List<DataEntry> allDataEntries = await _databaseService.GetAllDataEntries();
            DataEntries = new ObservableCollection<DataEntry>(allDataEntries);
        }
    }
}
