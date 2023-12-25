using EntityFramework.Enums;
using EntityFramework.Models;
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
    public class UserManagementViewModel : BindableBase
    {
        private string _firstOrLastname;

        private long _jmbg;

        private IDatabaseService _databaseService;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private DelegateCommand<User> _deleteUserCommand;

        private DelegateCommand _getAllUsersCommand;

        private DelegateCommand<User> _showEditUserDialogCommand;

        private DelegateCommand _showAddUserDialogCommand;

        private DelegateCommand _clearFiltersCommand;

        private DelegateCommand _filterUsersCommand;

        private DelegateCommand _getUserByJMBGCommand;

        private ObservableCollection<User> _users;

        public UserManagementViewModel(IDatabaseService databaseService, IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public User User { get; set; }

        public User LoggedUser { get; set; }

        public string FirstOrLastname
        {
            get
            {
                return _firstOrLastname;
            }

            set
            {
                _firstOrLastname = value;
                RaisePropertyChanged();
            }
        }

        public long JMBG
        {
            get
            {
                return _jmbg;
            }

            set
            {
                _jmbg = value;
                RaisePropertyChanged();
            }
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

        public DelegateCommand GetUserByJMBGCommand
        {
            get
            {
                _getUserByJMBGCommand = new DelegateCommand(GetUserByJMBG);
                return _getUserByJMBGCommand;
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

        public DelegateCommand FilterUsersCommand
        {
            get
            {
                _filterUsersCommand = new DelegateCommand(FilterUsers);
                return _filterUsersCommand;
            }
        }

        public DelegateCommand ClearFiltersCommand
        {
            get
            {
                _clearFiltersCommand = new DelegateCommand(ClearFilters);
                return _clearFiltersCommand;
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

        private async void ClearFilters()
        {
            FirstOrLastname = string.Empty;
            JMBG = 0;

            EFContext efContext = new EFContext();
            Users = await _databaseService.GetAllUsers(efContext);

            if (LoggedUser.UserRole == UserRole.Manager)
            {
                List<User> admins = new List<User>();
                admins = Users.Where(x => x.UserRole == UserRole.Admin).ToList();

                foreach (User admin in admins)
                {
                    Users.Remove(admin);
                }
            }
        }

        private bool UserLogin()
        {
            bool isResultGood = false;

            _dialogService.ShowDialog("userLoginDialog", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    isResultGood = true;
                    User = r.Parameters.GetValue<User>("user");
                }
                else
                {
                    isResultGood = false;
                }
            });

            return isResultGood;
        }

        private async void GetAllUsers()
        {
            if (Users is not null)
            {
                Users.Clear();
            }

            if (LoggedUserHelper.LoggedUser is null)
            {
                bool result = UserLogin();

                if (!result)
                {
                    _regionManager.RequestNavigate("MainRegion", "Options");
                    return;
                }

                if (User.UserRole is UserRole.Waiter)
                {
                    MessageBox.Show("Waiter can't access to user management!", "Access forbidden", MessageBoxButton.OK, MessageBoxImage.Error);
                    _regionManager.RequestNavigate("MainRegion", "Options");
                    return;
                }

                LoggedUserHelper.LoggedUser = User;
                LoggedUser = User;
            }
            else
            {
                LoggedUser = LoggedUserHelper.LoggedUser;
            }

            EFContext efContext = new EFContext();
            Users = await _databaseService.GetAllUsers(efContext);

            if (LoggedUser.UserRole == UserRole.Manager)
            {
                List<User> admins = new List<User>();
                admins = Users.Where(x => x.UserRole == UserRole.Admin).ToList();

                foreach (User admin in admins)
                {
                    Users.Remove(admin);
                }
            }
        }

        private async void DeleteUser(User user)
        {
            using EFContext efContext = new EFContext();

            if (LoggedUser.UserRole == UserRole.Manager && user.UserRole == UserRole.Manager)
            {
                MessageBox.Show("Manager cannot delete other managers!", "User management", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            user.IsDeleted = true;

            await _databaseService.EditUser(user, efContext);
            Users.Remove(user);

            ObservableCollection<User> users = await _databaseService.GetAllUsers(efContext);

            User admin = users.FirstOrDefault(x => x.UserRole == UserRole.Admin);

            if (admin is null)
            {
                Environment.Exit(0);
            }
        }

        private void ShowEditUserDialog(User user)
        {
            DialogParameters dialogParameters = new DialogParameters()
            {
                {"user", user},
                {"loggedUser", LoggedUser}
            };

            _dialogService.ShowDialog("editUserDialog", dialogParameters, r => { });
        }

        private async void FilterUsers()
        {
            EFContext efContext = new EFContext();
            ObservableCollection<User> originalUsers = await _databaseService.GetAllUsers(efContext);
            ObservableCollection<User> filteredUsers = new ObservableCollection<User>();

            if (FirstOrLastname != null && FirstOrLastname != string.Empty)
            {
                if (LoggedUser.UserRole == UserRole.Admin)
                {
                    filteredUsers = new ObservableCollection<User>(originalUsers.Where(x => x.FirstAndLastName.ToLower().Contains(FirstOrLastname.ToLower())));
                }
                else
                {
                    filteredUsers = new ObservableCollection<User>(originalUsers.Where(x => x.FirstAndLastName.ToLower().Contains(FirstOrLastname.ToLower()) && x.UserRole != UserRole.Admin));
                }

                Users = filteredUsers;
            }
        }

        private async void GetUserByJMBG()
        {
            if (JMBG.ToString().Length == 13)
            {
                using EFContext efContext = new EFContext();
                User user = await _databaseService.GetUserByJMBG(JMBG, efContext);

                Users.Clear();

                if (user is not null)
                {
                    if (LoggedUser.UserRole == UserRole.Manager && user.UserRole != UserRole.Admin)
                    {
                        Users.Add(user);
                    }

                    if (LoggedUser.UserRole == UserRole.Admin)
                    {
                        Users.Add(user);
                    }
                }
            }
            else
            {
                MessageBox.Show("JMBG should be 13 digits long!", "User Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAddUserDialog()
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                {"loggedUser", User}
            };

            _dialogService.ShowDialog("addUserDialog", dialogParameters, r => { });
            GetAllUsers();
        }
    }
}