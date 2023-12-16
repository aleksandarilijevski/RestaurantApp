using EntityFramework.Enums;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class EditUserViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<User> _editUserCommand;
        private User _user;

        public event Action<IDialogResult> RequestClose;

        public EditUserViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public User User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
                RaisePropertyChanged();
            }
        }

        public UserRole UserRole { get; set; }

        public string Title { get; set; } = "Edit user";

        public DelegateCommand<User> EditUserCommand
        {
            get
            {
                _editUserCommand = new DelegateCommand<User>(EditUser);
                return _editUserCommand;
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
            User = parameters.GetValue<User>("user");
        }

        private async void EditUser(User user)
        {
            using EFContext efContext = new EFContext();

            User userBarcodeCheck = await _databaseService.GetUserByBarcode(user.Barcode, efContext);

            if (userBarcodeCheck is null)
            {
                efContext.Entry(userBarcodeCheck).State = EntityState.Detached;
            }

            if (userBarcodeCheck is not null && userBarcodeCheck.ID != user.ID)
            {
                MessageBox.Show("User with entered barcode already exists!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User userJMBGCheck = await _databaseService.GetUserByJMBG(user.JMBG, efContext);

            if (userJMBGCheck is not null && userJMBGCheck.ID != user.ID)
            {
                MessageBox.Show("User with entered jmbg already exists!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (userJMBGCheck is null)
            {
                efContext.Entry(userJMBGCheck).State = EntityState.Detached;
            }

            await _databaseService.EditUser(user, efContext);
            CloseDialog("true");
        }
    }
}
