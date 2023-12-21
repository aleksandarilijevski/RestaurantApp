using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class OnlineOrdersViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private DelegateCommand _loadOnlineOrdersCommand;
        private DelegateCommand _addOnlineOrderCommand;
        private DelegateCommand _deleteOnlineOrdersCommand;
        private DelegateCommand<OnlineOrder> _openOnlineOrderCommand;
        private ObservableCollection<OnlineOrder> _onlineOrders;

        public OnlineOrdersViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public ObservableCollection<OnlineOrder> OnlineOrders
        {
            get
            {
                return _onlineOrders;
            }

            set
            {
                _onlineOrders = value;
                RaisePropertyChanged();
            }
        }

        public OnlineOrder SelectedOnlineOrder { get; set; }

        public DelegateCommand LoadOnlineOrdersCommand
        {
            get
            {
                _loadOnlineOrdersCommand = new DelegateCommand(LoadOnlineOrders);
                return _loadOnlineOrdersCommand;
            }
        }

        public DelegateCommand AddOnlineOrderCommand
        {
            get
            {
                _addOnlineOrderCommand = new DelegateCommand(AddOnlineOrder);
                return _addOnlineOrderCommand;
            }
        }

        public DelegateCommand DeleteOnlineOrderCommand
        {
            get
            {
                _deleteOnlineOrdersCommand = new DelegateCommand(DeleteOnlineOrder);
                return _deleteOnlineOrdersCommand;
            }
        }

        public DelegateCommand<OnlineOrder> OpenOnlineOrderCommand
        {
            get
            {
                _openOnlineOrderCommand = new DelegateCommand<OnlineOrder>(OpenOnlineOrder);
                return _openOnlineOrderCommand;
            }
        }

        private async void LoadOnlineOrders()
        {
            using EFContext efContext = new EFContext();

            ObservableCollection<OnlineOrder> onlineOrders = await _databaseService.GetAllOnlineOrders(efContext);
            OnlineOrders = new ObservableCollection<OnlineOrder>(onlineOrders.Where(x => x.CreatedDateTime?.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")));
            OnlineOrders = new ObservableCollection<OnlineOrder>(OnlineOrders.OrderByDescending(x => x.CreatedDateTime));
        }

        public async void AddOnlineOrder()
        {
            using EFContext efContext = new EFContext();

            int onlineOrderId = await _databaseService.GetMaxOnlineOrderID(efContext) + 1;

            OnlineOrder onlineOrder = new OnlineOrder();
            onlineOrder.ID = onlineOrderId;

            await _databaseService.AddOnlineOrder(onlineOrder, efContext);

            OnlineOrders.Add(onlineOrder);
            OnlineOrders = new ObservableCollection<OnlineOrder>(OnlineOrders.OrderByDescending(x => x.CreatedDateTime));
        }

        private async void DeleteOnlineOrder()
        {
            using EFContext efContext = new EFContext();

            if (SelectedOnlineOrder is null)
            {
                MessageBox.Show("Online order is not selected!", "Online ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedOnlineOrder.IsPayed)
            {
                MessageBox.Show("You can't delete proceseed order!", "Online ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedOnlineOrder.TableArticleQuantities.Count != 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("There is articles on selected online order, are you sure you want to delete it?", "Online ordering", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            await _databaseService.DeleteOnlineOrder(SelectedOnlineOrder, efContext);
            OnlineOrders.Remove(SelectedOnlineOrder);
        }

        private void OpenOnlineOrder(OnlineOrder onlineOrder)
        {
            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "onlineOrder", onlineOrder }
            };

            _regionManager.RequestNavigate("MainRegion", "OnlineOrdering", navigationParameters);
        }
    }
}
