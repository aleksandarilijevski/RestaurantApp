using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class UserManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand<User> _deleteUserCommand;
        private DelegateCommand _getAllUsersCommand;
        private ObservableCollection<User> _users;
        private DelegateCommand<User> _showEditUserDialogCommand;
        private DelegateCommand _showAddUserDialogCommand;

        public UserManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
        }

        public ObservableCollection<User> Users
        {
            get
            {
                return _users;
            }

            set
            {
                _users = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ShowAddUserDialogCommand
        {
            get
            {
                _showAddUserDialogCommand = new DelegateCommand(ShowAddUserDialog);
                return _showAddUserDialogCommand;
            }
        }

        public DelegateCommand<User> ShowEditUserDialogCommand
        {
            get
            {
                _showEditUserDialogCommand = new DelegateCommand<User>(ShowEditUserDialog);
                return _showEditUserDialogCommand;
            }
        }

        public DelegateCommand GetAllUsersCommand
        {
            get
            {
                _getAllUsersCommand = new DelegateCommand(GetAllUsers);
                return _getAllUsersCommand;
            }
        }

        public DelegateCommand<User> DeleteUserCommand
        {
            get
            {
                _deleteUserCommand = new DelegateCommand<User>(DeleteUser);
                return _deleteUserCommand;
            }
        }

        private async void GetAllUsers()
        {
            Users = await _databaseService.GetAllUsers();
        }

        private async void DeleteUser(User user)
        {
            using EFContext efContext = new EFContext();
            await _databaseService.DeleteUser(user, efContext);
            Users.Remove(user);

            if (Users.Count == 0)
            {
                Environment.Exit(0);
            }
        }

        private void ShowEditUserDialog(User user)
        {
            DialogParameters dialogParameters = new DialogParameters()
            {
                {"user",user}
            };

            _dialogService.ShowDialog("editUserDialog", dialogParameters, r => { });
        }

        private void ShowAddUserDialog()
        {
            _dialogService.ShowDialog("addUserDialog");
            GetAllUsers();
        }
    }
}
