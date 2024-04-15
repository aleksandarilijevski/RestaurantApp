using EntityFramework.Enums;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
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

        public string Title { get; set; } = ViewConstants.EditUserTitle;

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
                MessageBox.Show(MessageBoxConstants.UserWithEnteredBarcodeAlreadyExists, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (userBarcodeCheck is not null)
            {
                efContext.Entry(userBarcodeCheck).State = EntityState.Detached;
            }

            if (user.Barcode == 0 || user.Barcode < 0)
            {
                MessageBox.Show(MessageBoxConstants.UserBarcodeCannotBeZeroOrLess, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User userJMBGCheck = await _databaseService.GetUserByJMBG(user.JMBG, efContext);

            if (userJMBGCheck is not null && userJMBGCheck.ID != user.ID)
            {
                MessageBox.Show(MessageBoxConstants.UserWithEnteredJMBGAlreadyExists, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (User.JMBG.ToString().Length != 13)
            {
                MessageBox.Show(MessageBoxConstants.JMBGFieldShouldHaveThirteenDigits, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (userJMBGCheck is not null)
            {
                efContext.Entry(userJMBGCheck).State = EntityState.Detached;
            }

            if (user.IsActive)
            {
                MessageBox.Show(MessageBoxConstants.YouCannotChangeUserWhichIsCurrentlyActive, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                CloseDialog("true");
                return;
            }

            if (user.UserRole == UserRoleBeforeChange && LoggedUser.UserRole == UserRole.Manager)
            {
                MessageBox.Show(MessageBoxConstants.ManagersCannotEditOtherManagers, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.UserRole != UserRoleBeforeChange && LoggedUser.UserRole == UserRole.Manager)
            {
                MessageBox.Show(MessageBoxConstants.ManagersCannotEditRolesOfOtherUsers, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ObservableCollection<User> users = await _databaseService.GetAllUsers(new EFContext());

            User.UserRole = UserRole;

            if (UserRoleBeforeChange != User.UserRole)
            {
                User findAdmin = users.FirstOrDefault(x => x.UserRole == UserRole.Admin && x.ID != User.ID);

                if (findAdmin is null)
                {
                    MessageBox.Show(MessageBoxConstants.PleaseCreateAnotherAdminIfYouWantToChangeRoleOfTheSelectedOne, MessageBoxConstants.EditUserTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            await _databaseService.EditUser(user, efContext);

            if (user.ID == LoggedUser.ID)
            {
                LoggedUserHelper.LoggedUser = null;
                MessageBox.Show(MessageBoxConstants.YouAreLoggedOut, MessageBoxConstants.UserManagementTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                _regionManger.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.OptionsViewName);
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
