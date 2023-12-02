using EntityFramework.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;

namespace RestaurantApp.ViewModels
{
    public class TableInvoiceHistoryViewModel : IDialogAware
    {
        private IDialogService _dialogService;

        public List<Bill> Bills { get; set; }

        public int TableID { get; set; }

        public string DateNow { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public string Title => "Table invoice history";

        public Bill SelectedBill { get; set; }

        private DelegateCommand _showReportDetailsCommand;

        public TableInvoiceHistoryViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand ShowReportDetailsCommand
        {
            get
            {
                _showReportDetailsCommand = new DelegateCommand(ShowReportDetails);
                return _showReportDetailsCommand;
            }
        }

        private void ShowReportDetails()
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                { "bill", SelectedBill}
            };

            _dialogService.ShowDialog("reportDetailsDialog", dialogParameters, r => { });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Bills = parameters.GetValue<List<Bill>>("bills");
            TableID = parameters.GetValue<int>("tableID");
        }
    }
}
