using EntityFramework.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels
{
    class ReportDetailsViewModel : BindableBase, IDialogAware
    {
        private string _title = "Report details";
        private string _totalPrice = "Total price : ";
        private Bill _bill;
        private ObservableCollection<TableArticleQuantity> _soldTableArticleQuantities;

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

        public Bill Bill
        {
            get
            {
                return _bill;
            }

            set
            {
                _bill = value;
            }
        }

        public ObservableCollection<TableArticleQuantity> SoldTableArticleQuantities
        {
            get
            {
                return _soldTableArticleQuantities;
            }

            set
            {
                _soldTableArticleQuantities = value;
                RaisePropertyChanged();
            }
        }

        public string TotalPrice
        {
            get
            {
                return _totalPrice + Bill.TotalPrice.ToString("0.00");
            }

            set
            {
                _totalPrice = value;
                RaisePropertyChanged();
            }
        }

        public event Action<IDialogResult> RequestClose;

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
            Bill = parameters.GetValue<Bill>("bill");
            GetSoldArticles();
        }

        private void GetSoldArticles()
        {
            SoldTableArticleQuantities = new ObservableCollection<TableArticleQuantity>(_bill.Table.TableArticleQuantities.OfType<SoldTableArticleQuantity>().ToList());
        }
    }
}
