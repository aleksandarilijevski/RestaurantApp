﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class EditWaiterViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private string _title = "Edit waiter";
        private Waiter _waiter;
        private DelegateCommand<Waiter> _editWaiterCommand;

        public event Action<IDialogResult> RequestClose;

        public EditWaiterViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Waiter Waiter
        {
            get { return _waiter; }
            set { SetProperty(ref _waiter, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand<Waiter> EditWaiterCommand
        {
            get
            {
                _editWaiterCommand = new DelegateCommand<Waiter>(EditWaiter);
                return _editWaiterCommand;
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
            Waiter = parameters.GetValue<Waiter>("waiter");
        }

        private async void EditWaiter(Waiter waiter)
        {
            await _databaseService.EditWaiter(waiter);
            CloseDialog("true");
        }
    }
}
