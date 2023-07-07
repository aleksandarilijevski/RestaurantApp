using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class AddWaiterViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private string _title = "Add waiter";
        private DelegateCommand<Waiter> _addWaiterCommand;
        private Waiter _waiter;

        public event Action<IDialogResult> RequestClose;

        public AddWaiterViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _waiter = new Waiter();
        }

        public Waiter Waiter
        {
            get
            {
                return _waiter;
            }
            set
            {
                _waiter = value;
                SetProperty(ref _waiter, value);
            }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand<Waiter> AddWaiterCommand
        {
            get
            {
                _addWaiterCommand = new DelegateCommand<Waiter>(AddWaiter);
                return _addWaiterCommand; 
            }
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        private async void AddWaiter(Waiter waiter)
        {
            await _databaseService.AddWaiter(waiter);
        }
    }
}
