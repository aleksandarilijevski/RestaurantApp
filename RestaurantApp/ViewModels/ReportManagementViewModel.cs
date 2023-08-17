using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class ReportManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DateTime _dateFrom = DateTime.Now;
        private DateTime _dateTo = DateTime.Now;
        private ObservableCollection<Bill> _bills = new ObservableCollection<Bill>();
        private DelegateCommand _confirmCommand;
        private DelegateCommand _showReportDetailsCommand;
        private Bill _selectedBill;

        public ReportManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _dialogService = dialogService;
        }

        public DateTime DateFrom
        {
            get
            {
                return _dateFrom;
            }

            set
            {
                _dateFrom = value;
            }
        }

        public DateTime DateTo
        {
            get
            {
                return _dateTo;
            }

            set
            {
                _dateTo = value;
            }
        }

        public ObservableCollection<Bill> Bills
        {
            get
            {
                return _bills;
            }

            set
            {
                _bills = value;
                RaisePropertyChanged();
            }
        }

        public Bill SelectedBill
        {
            get
            {
                return _selectedBill;
            }

            set
            {
                _selectedBill = value;
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
                { "bill", SelectedBill }
            };

            _dialogService.ShowDialog("reportDetailsDialog", dialogParameters, r => { });
        }

        private async void Confirm()
        {
            _bills.Clear();
            List<Bill> bills = await _databaseService.GetAllBills();

            foreach (Bill bill in bills)
            {
                if (bill.CreatedDateTime >= DateFrom && bill.CreatedDateTime <= DateTo)
                {
                    _bills.Add(bill);
                }
            }
        }
    }
}
