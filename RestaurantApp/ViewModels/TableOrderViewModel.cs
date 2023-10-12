using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class TableOrderViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private IDatabaseService _databaseService;
        private DelegateCommand<string> _orderingCommand;
        private DelegateCommand _loadAllTablesCommand;
        private ObservableCollection<Table> _tables;

        public TableOrderViewModel(IRegionManager regionManager, IDatabaseService databaseService)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
        }

        public int ID { get; set; }

        public ObservableCollection<Table> Tables
        {
            get
            {
                return _tables;
            }

            set
            {
                _tables = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> OrderingCommand
        {
            get
            {
                _orderingCommand = new DelegateCommand<string>(ShowOrderingUsercontrol);
                return _orderingCommand;
            }
        }

        public DelegateCommand LoadAllTablesCommand
        {
            get
            {
                _loadAllTablesCommand = new DelegateCommand(LoadAllTables);
                return _loadAllTablesCommand;
            }
        }

        private async void LoadAllTables()
        {
            Tables = new ObservableCollection<Table>(await _databaseService.GetAllTables());
            //Tables = new ObservableCollection<Table>(tables);
        }

        private void ShowOrderingUsercontrol(string id)
        {
            ID = int.Parse(id);

            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "id", ID }
            };

            _regionManager.RequestNavigate("MainRegion", "Ordering", navigationParameters);
        }
    }
}
