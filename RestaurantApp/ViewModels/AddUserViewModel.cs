﻿using EntityFramework.Enums;
using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Windows;

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

        public User LoggedUser { get; set; }

        public string Title { get; set; } = "Add user";

        public UserRole UserRole { get; set; } = UserRole.Waiter;

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

        private async void AddUser(User user)
        {
            if (LoggedUser is not null && LoggedUser.UserRole != UserRole.Admin)
            {
                MessageBox.Show(MessageBoxConstants.ManagerCantCreateUsers, MessageBoxConstants.AccessForbiddenTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                CloseDialog("true");
            }

            user.UserRole = UserRole;

            if (user.Barcode == 0)
            {
                MessageBox.Show(MessageBoxConstants.BarcodeFieldCanNotBeZeroOrEmpty, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.FirstAndLastName == null || user.FirstAndLastName == string.Empty)
            {
                MessageBox.Show(MessageBoxConstants.FirstAndLastnameCanNotBeEmpty, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.DateOfBirth == DateTime.MinValue)
            {
                MessageBox.Show(MessageBoxConstants.PleaseEnterTheDateOfBirth, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.JMBG.ToString().Length != 13)
            {
                MessageBox.Show(MessageBoxConstants.JMBGFieldShouldHaveThirteenDigits, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using EFContext efContext = new EFContext();
            ObservableCollection<User> users = await _databaseService.GetAllUsers(efContext);

            if (users.Count == 0 && user.UserRole != UserRole.Admin)
            {
                MessageBox.Show(MessageBoxConstants.FirstCreatedUserShouldHaveAdminRole, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User userBarcodeCheck = await _databaseService.GetUserByBarcode(user.Barcode, efContext);

            if (userBarcodeCheck is not null)
            {
                if (userBarcodeCheck.Barcode == user.Barcode)
                {
                    MessageBox.Show(MessageBoxConstants.UserWithEnteredBarcodeAlreadyExists, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            User userJMBGCheck = await _databaseService.GetUserByJMBG(user.JMBG, efContext);

            if (userJMBGCheck is not null)
            {
                if (userJMBGCheck.JMBG == user.JMBG)
                {
                    MessageBox.Show(MessageBoxConstants.UserWithEnteredJMBGAlreadyExists, MessageBoxConstants.AddUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            await _databaseService.AddUser(user, efContext);
            CloseDialog("true");
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            User.DateOfBirth = DateTime.Now;
            LoggedUser = parameters.GetValue<User>("loggedUser");
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
    }
}
