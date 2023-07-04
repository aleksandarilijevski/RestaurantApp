using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace RestaurantApp.ViewModels
{
    public class WaiterManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<Waiter> _deleteWaiterCommand;
        private DelegateCommand _getAllWaitersCommand;
        private ObservableCollection<Waiter> _waiters;

        public ObservableCollection<Waiter> Waiters
        {
            get => _waiters;

            set
            {
                _waiters = value;
                RaisePropertyChanged();
            }
        }

        public WaiterManagementViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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
    }
}
