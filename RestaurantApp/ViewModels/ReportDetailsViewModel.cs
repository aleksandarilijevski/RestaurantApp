using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels
{
    class ReportDetailsViewModel : BindableBase, IDialogAware
    {
        private string _totalPrice = "Total price : ";

        private IDatabaseService _databaseService;

        private DelegateCommand _printBillCommand;

        private ObservableCollection<TableArticleQuantity> _soldTableArticleQuantities;

        public string Title { get; set; } = "Report details";

        public Bill Bill { get; set; }

        public ReportDetailsViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public event Action<IDialogResult> RequestClose;

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

        public DelegateCommand PrintBillCommand
        {
            get
            {
                _printBillCommand = new DelegateCommand(PrintBill);
                return _printBillCommand;
            }
        }

        private void GetSoldArticles()
        {
            if (Bill.Table is not null)
            {
                SoldTableArticleQuantities = new ObservableCollection<TableArticleQuantity>(Bill.Table.TableArticleQuantities.Select(x => x).Where(x => x.BillID == Bill.ID && (x is SoldTableArticleQuantity)).OfType<SoldTableArticleQuantity>().ToList());
            }

            if (Bill.OnlineOrder is not null)
            {
                SoldTableArticleQuantities = new ObservableCollection<TableArticleQuantity>(Bill.OnlineOrder.TableArticleQuantities.Select(x => x).Where(x => x.BillID == Bill.ID && (x is SoldTableArticleQuantity)).OfType<SoldTableArticleQuantity>().ToList());
            }
        }

        private async void PrintBill()
        {
            using EFContext efContext = new EFContext();
            List<TableArticleQuantity> soldTableArticleQuantities = null;

            if (Bill.Table is not null)
            {
                soldTableArticleQuantities = Bill.Table.TableArticleQuantities.OfType<SoldTableArticleQuantity>().Select(sold => (TableArticleQuantity)sold).ToList();
            }

            if (Bill.OnlineOrder is not null)
            {
                soldTableArticleQuantities = Bill.OnlineOrder.TableArticleQuantities.OfType<SoldTableArticleQuantity>().Select(sold => (TableArticleQuantity)sold).ToList();
            }

            User user = await _databaseService.GetUserByID(Bill.UserID, efContext);
            DrawningHelper.RedrawBill(Bill, soldTableArticleQuantities, user);
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.OK;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.Cancel;
            }

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Bill = parameters.GetValue<Bill>("bill");
            GetSoldArticles();
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
