using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Services.Interface;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class UserLoginDialogViewModel : BindableBase, IDialogAware
    {
        private string _barcode;

        private IDatabaseService _databaseService;

        private DelegateCommand<string> _loginCommand;

        private User _user;

        public UserLoginDialogViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = ViewConstants.UserLoginTitle;

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

        public DelegateCommand<string> LoginCommand
        {
            get
            {
                _loginCommand = new DelegateCommand<string>(Login);
                return _loginCommand;
            }
        }

        private async void Login(string barcode)
        {
            if (long.TryParse(barcode, out long barcodeLong) == false)
            {
                MessageBox.Show(MessageBoxConstants.BarcodeIsNotInRightFormat, MessageBoxConstants.UserLoginTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            using EFContext efContext = new EFContext();
            User user = await _databaseService.GetUserByBarcode(barcodeLong, efContext);

            if (user is null)
            {
                MessageBox.Show(MessageBoxConstants.UserWithEnteredBarcodeDoesNotExistInSystem, MessageBoxConstants.UserLoginTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            if (user.IsDeleted)
            {
                MessageBox.Show(MessageBoxConstants.UserWithEnteredBarcodeIsDeleted, MessageBoxConstants.UserLoginTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            _user = user;

            CloseDialog("true");
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
    }
}
