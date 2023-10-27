﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Windows;

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
        private DelegateCommand _clearFiltersCommand;
        private DelegateCommand _filterUsersCommand;
        private DelegateCommand _getUserByJMBGCommand;
        private string _firstOrLastname;
        private long _jmbg;

        public UserManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
        }

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

            Users = await _databaseService.GetAllUsers();
        }

        private async void GetAllUsers()
        {
            Users = await _databaseService.GetAllUsers();
        }

        private async void DeleteUser(User user)
        {
            using EFContext efContext = new EFContext();

            user.IsDeleted = true;

            await _databaseService.EditUser(user, efContext);
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
                {"user", user}
            };

            _dialogService.ShowDialog("editUserDialog", dialogParameters, r => { });
        }

        private void FilterUsers()
        {


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
                    Users.Add(user);
                }
            }
            else
            {
                MessageBox.Show("JMBG should be 13 digits long!", "User Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAddUserDialog()
        {
            _dialogService.ShowDialog("addUserDialog");
            GetAllUsers();
        }
    }
}
