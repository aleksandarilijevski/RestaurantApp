using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class AddUserViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<User> _addUserCommand;

        public AddUserViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public event Action<IDialogResult> RequestClose;

        public User User { get; set; } = new User();

        public string Title { get; set; } = "Add user";

        public DelegateCommand<User> AddUserCommand
        {
            get
            {
                _addUserCommand = new DelegateCommand<User>(AddUser);
                return _addUserCommand;
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

        private async void AddUser(User user)
        {
            using EFContext efContext = new EFContext();
            await _databaseService.AddUser(user, efContext);
            CloseDialog("true");
        }
    }
}
