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
        private int _id;
        private ObservableCollection<Table> _tables;

        public TableOrderViewModel(IRegionManager regionManager, IDatabaseService databaseService)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
        }

        public int ID
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
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

        public ObservableCollection<Table> Tables
        {
            get
            {
                return _tables;
            }

            set
            {
                SetProperty(ref _tables, value);
            }
        }

        private async void LoadAllTables()
        {
            List<Table> tables = await _databaseService.GetAllTables();
            Tables = new ObservableCollection<Table>(tables);
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
