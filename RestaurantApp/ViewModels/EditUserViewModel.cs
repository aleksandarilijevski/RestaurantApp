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
using System.Collections.ObjectModel;
using System.Linq;
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

        public User LoggedUser { get; set; }

        public UserRole UserRole { get; set; }

        public UserRole UserRoleBeforeChange { get; set; }

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
            User user = parameters.GetValue<User>("user");
            User = (User)user.Clone();
            LoggedUser = parameters.GetValue<User>("loggedUser");
            UserRoleBeforeChange = User.UserRole;
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

            if (userBarcodeCheck is not null)
            {
                efContext.Entry(userBarcodeCheck).State = EntityState.Detached;
            }

            if (user.Barcode == 0 || user.Barcode < 0)
            {
                MessageBox.Show("User barcode cannot be zero or less!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User userJMBGCheck = await _databaseService.GetUserByJMBG(user.JMBG, efContext);

            if (userJMBGCheck is not null && userJMBGCheck.ID != user.ID)
            {
                MessageBox.Show("User with entered jmbg already exists!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (User.JMBG.ToString().Length != 13)
            {
                MessageBox.Show("JMBG should be 13 digits long!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            if (user.UserRole == UserRoleBeforeChange && LoggedUser.UserRole == UserRole.Manager)
            {
                MessageBox.Show("Managers cannot edit other managers!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.UserRole != UserRoleBeforeChange && LoggedUser.UserRole == UserRole.Manager)
            {
                MessageBox.Show("Managers cannot edit roles of other users!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ObservableCollection<User> users = await _databaseService.GetAllUsers(new EFContext());

            User.UserRole = UserRole;

            if (UserRoleBeforeChange != User.UserRole)
            {
                User findAdmin = users.FirstOrDefault(x => x.UserRole == UserRole.Admin && x.ID != User.ID);

                if (findAdmin is null)
                {
                    MessageBox.Show("Please create another admin if you want to change role of the selected one.", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
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
