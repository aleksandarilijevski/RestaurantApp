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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class EditUserViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<User> _editUserCommand;
        private User _user;
        private IRegionManager _regionManger;

        public event Action<IDialogResult> RequestClose;

        public EditUserViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManger = regionManager;
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

        //Ask about this
        //Why i can't simply directly get logged user from static class
        public User LoggedUser = LoggedUserHelper.LoggedUser;

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

            if (userJMBGCheck is not null)
            {
                efContext.Entry(userJMBGCheck).State = EntityState.Detached;
            }

            //List<Table> tables = await _databaseService.GetAllTables(efContext);
            //ObservableCollection<OnlineOrder> onlineOrders = await _databaseService.GetAllOnlineOrders(efContext);

            if (user.IsActive)
            {
                MessageBox.Show("You cannot change user which is currently active!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseDialog("true");
                return;
            }

            //OnlineOrder onlineOrder = onlineOrders.FirstOrDefault(x => x.UserID == user.ID);

            //if (onlineOrder.UserID == user.ID)
            //{
            //    MessageBox.Show("You cannot change user which is currently active!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
            //    CloseDialog("true");
            //    return;
            //}

            //Table table = tables.FirstOrDefault(x => x.UserID == user.ID);

            //if (table is not null)
            //{
            //    MessageBox.Show("You cannot change user which is currently active!", "Edit user", MessageBoxButton.OK, MessageBoxImage.Error);
            //    CloseDialog("true");
            //    return;
            //}

            await _databaseService.EditUser(user, efContext);

            if (user.ID == LoggedUser.ID)
            {
                LoggedUserHelper.LoggedUser = null;
                MessageBox.Show("You're logged out!", "User management", MessageBoxButton.OK, MessageBoxImage.Information);
                _regionManger.RequestNavigate("MainRegion", "Options");
            }

            CloseDialog("true");
        }
    }
}
