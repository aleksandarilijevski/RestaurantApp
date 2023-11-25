using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.ComponentModel;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class UserLoginDialogViewModel : BindableBase, IDialogAware
    {
        private User _user;
        private string _barcode;
        private IDatabaseService _databaseService;
        private DelegateCommand<string> _loginCommand;

        public UserLoginDialogViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Title { get; set; } = "User login";

        public string Barcode
        {
            get
            {
                return _barcode;
            }

            set
            {
                _barcode = value;
                RaisePropertyChanged();
            }
        }

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<string> LoginCommand
        {
            get
            {
                _loginCommand = new DelegateCommand<string>(Login);
                return _loginCommand;
            }
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

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;

            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            DialogParameters dialogParametars = new DialogParameters()
            {
                {"user", _user}
            };

            DialogResult dialogResult = new DialogResult(result, dialogParametars);

            RaiseRequestClose(dialogResult);
        }

        private async void Login(string barcode)
        {
            if (long.TryParse(barcode, out long barcodeLong) == false)
            {
                MessageBox.Show("Barcode is not in right format!", "User login", MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            using EFContext efContext = new EFContext();
            User user = await _databaseService.GetUserByBarcode(barcodeLong, efContext);

            if (user is null)
            {
                MessageBox.Show("User with entered barcode does not exist in system!", "User login", MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            if (user.IsDeleted)
            {
                MessageBox.Show("User with entered barcode is deleted!", "User login", MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            _user = user;

            CloseDialog("true");
        }
    }
}
