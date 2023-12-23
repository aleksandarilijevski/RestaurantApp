using EntityFramework.Enums;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class EditUserViewModel : BindableBase, IDialogAware
    {
        private User _user;

        private IDatabaseService _databaseService;

        private IRegionManager _regionManger;

        private DelegateCommand<User> _editUserCommand;

        public EditUserViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManger = regionManager;
        }

        public event Action<IDialogResult> RequestClose;

        //Ask about this
        //Why i can't simply directly get logged user from static class
        public User LoggedUser { get; set; }

        public UserRole UserRole { get; set; }

        public string Title { get; set; } = "Edit user";

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

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            User user  = parameters.GetValue<User>("user");
            User = (User)user.Clone();
            LoggedUser = parameters.GetValue<User>("loggedUser");
        }

        private async void EditUser(User user)
        {
            using EFContext efContext = new EFContext();

            User userBarcodeCheck = await _databaseService.GetUserByBarcode(user.Barcode, efContext);

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

            //Check
            if (userJMBGCheck is not null)
            {
                efContext.Entry(userJMBGCheck).State = EntityState.Detached;
            }

            if (user.IsActive)
            {
                MessageBox.Show("You cannot change user which is currently active!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseDialog("true");
                return;
            }

            if (user.UserRole == UserRole.Manager && LoggedUser.UserRole == UserRole.Manager)
            {
                MessageBox.Show("Managers cannot edit other managers!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await _databaseService.EditUser(user, efContext);

            if (user.ID == LoggedUser.ID)
            {
                LoggedUserHelper.LoggedUser = null;
                MessageBox.Show("You're logged out!", "User management", MessageBoxButton.OK, MessageBoxImage.Information);
                _regionManger.RequestNavigate("MainRegion", "Options");
            }

            CloseDialog("true");
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
