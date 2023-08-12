﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class PaymentDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _calculateChangeCommand;
        private DelegateCommand _confirmCommand;
        private string _title = "Payment Dialog";
        private decimal _totalPrice;
        private string _cashBox;
        private decimal _change;

        public DelegateCommand<string> CalculateChangeCommand
        {
            get
            {
                _calculateChangeCommand = new DelegateCommand<string>(CalculateChange);
                return _calculateChangeCommand;
            }
        }

        public DelegateCommand ConfirmCommand
        {
            get
            {
                _confirmCommand = new DelegateCommand(Confirm);
                return _confirmCommand;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                _totalPrice = value;
                RaisePropertyChanged();
            }
        }

        public string CashBox
        {
            get
            {
                return _cashBox;
            }

            set
            {
                _cashBox = value;
            }
        }

        public decimal Change
        {
            get
            {
                return _change;
            }

            set
            {
                _change = value;
                RaisePropertyChanged();
            }
        }

        public event Action<IDialogResult> RequestClose;

        private void Confirm()
        {
            if (CashBox == string.Empty || CashBox == "")
            {
                MessageBox.Show("Cash property can't be empty!", "Payment confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Change == 0)
            {
                MessageBox.Show("Change has not been calculated!", "Payment confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CloseDialog("true");
        }

        private void CalculateChange(string cashBox)
        {
            decimal change = TotalPrice - int.Parse(cashBox);
            change = Math.Abs(change);
            Change = change;
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
                {"cash", int.Parse(CashBox)},
                {"change", Change}
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
            _totalPrice = parameters.GetValue<decimal>("totalPrice");
        }
    }
}
