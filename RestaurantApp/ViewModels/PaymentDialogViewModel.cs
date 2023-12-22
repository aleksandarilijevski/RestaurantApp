using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class PaymentDialogViewModel : BindableBase, IDialogAware
    {
        private decimal _totalPrice;

        private decimal _change;

        private bool _buttonVisible;

        private DelegateCommand<string> _calculateChangeCommand;

        private DelegateCommand _confirmCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = "Payment dialog";

        public string CashBox { get; set; }

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

        public bool IsButtonEnabled
        {
            get
            {
                return _buttonVisible;
            }

            set
            {
                _buttonVisible = value;
                RaisePropertyChanged();
            }
        }

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

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            _totalPrice = parameters.GetValue<decimal>("totalPrice");
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

        private void CalculateChange(string cashBox)
        {
            CashBox = cashBox.ToString();
            decimal cashBoxDecimal = 0;

            if (decimal.TryParse(cashBox, out cashBoxDecimal) == false)
            {
                MessageBox.Show("Cash field is empty or it's not in valid format!", "Payment confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cashBoxDecimal >= TotalPrice)
            {
                decimal change = Math.Abs(TotalPrice - cashBoxDecimal);
                Change = change;
            }
            else
            {
                MessageBox.Show("Cash price can't be lower than total price!", "Payment dialog", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IsButtonEnabled = true;
        }

        private void Confirm()
        {
            CloseDialog("true");
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
                {"cash", decimal.Parse(CashBox)},
                {"change", Change}
            };

            DialogResult dialogResult = new DialogResult(result, dialogParametars);

            RaiseRequestClose(dialogResult);
        }
    }
}
