using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class WaiterManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand<Waiter> _deleteWaiterCommand;
        private DelegateCommand _getAllWaitersCommand;
        private ObservableCollection<Waiter> _waiters;
        private DelegateCommand<Waiter> _showEditWaiterDialogCommand;
        private DelegateCommand _showAddWaiterDialogCommand;

        public ObservableCollection<Waiter> Waiters
        {
            get => _waiters;

            set
            {
                _waiters = value;
                RaisePropertyChanged();
            }
        }

        public WaiterManagementViewModel(IDatabaseService databaseService,IDialogService dialogService)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
        }

        public DelegateCommand ShowAddWaiterDialogCommand
        {
            get
            {
                _showAddWaiterDialogCommand = new DelegateCommand(ShowAddWaiterDialog);
                return _showAddWaiterDialogCommand;
            }
        }

        public DelegateCommand<Waiter> ShowEditWaiterDialogCommand
        {
            get
            {
                _showEditWaiterDialogCommand = new DelegateCommand<Waiter>(ShowEditWaiterDialog);
                return _showEditWaiterDialogCommand;
            }
        }

        public DelegateCommand GetAllWaitersCommand
        {
            get
            {
                _getAllWaitersCommand = new DelegateCommand(GetAllWaiters);
                return _getAllWaitersCommand;
            }
        }

        public DelegateCommand<Waiter> DeleteWaiterCommand
        {
            get
            {
                _deleteWaiterCommand = new DelegateCommand<Waiter>(DeleteWaiter);
                return _deleteWaiterCommand;
            }
        }

        private async void GetAllWaiters()
        {
            Waiters = await _databaseService.GetAllWaiters();
        }

        private async void DeleteWaiter(Waiter waiter)
        {
            await _databaseService.DeleteWaiter(waiter);
            Waiters.Remove(waiter);
        }

        private void ShowEditWaiterDialog(Waiter waiter)
        {
            DialogParameters dialogParameters = new DialogParameters()
            {
                {"waiter",waiter}
            };

            _dialogService.ShowDialog("editWaiterDialog",dialogParameters, r => 
            {
                Waiter resultData = r.Parameters.GetValue<Waiter>("waiter");
            });
        }

        private void ShowAddWaiterDialog()
        {
            _dialogService.ShowDialog("addWaiterDialog");
        }
    }
}
