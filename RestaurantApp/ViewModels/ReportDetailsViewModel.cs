using EntityFramework.Models;
using PdfSharp.Drawing;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RestaurantApp.ViewModels
{
    class ReportDetailsViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private string _title = "Report details";
        private string _totalPrice = "Total price : ";
        private Bill _bill;
        private DelegateCommand _printBillCommand;
        private ObservableCollection<TableArticleQuantity> _soldTableArticleQuantities;

        public ReportDetailsViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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

        public DelegateCommand PrintBillCommand
        {
            get
            {
                _printBillCommand = new DelegateCommand(PrintBill);
                return _printBillCommand;
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
            SoldTableArticleQuantities = new ObservableCollection<TableArticleQuantity>(_bill.Table.TableArticleQuantities.Select(x => x).Where(x => x.BillID == Bill.ID && (x is SoldTableArticleQuantity)).OfType<SoldTableArticleQuantity>().ToList());
        }

        private async Task<XImage> GetCustomQRCode(string text)
        {
            string url = "https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + text;

            HttpClient client = new HttpClient();
            Stream stream = await client.GetStreamAsync(url);

            XImage image = XImage.FromStream(stream);
            return image;
        }

        private async void PrintBill()
        {
            XImage qrCode = await GetCustomQRCode("test");
            List<TableArticleQuantity> soldTableArticleQuantities = _bill.Table.TableArticleQuantities.OfType<SoldTableArticleQuantity>().Select(sold => (TableArticleQuantity)sold).ToList();
            DrawningHelper.RedrawBill(Bill, soldTableArticleQuantities);
        }
    }
}
