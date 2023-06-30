using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System;
using System.Windows.Documents;

namespace RestaurantApp.ViewModels
{
    public class TableOrderViewModel : BindableBase
    {
        private IDatabaseService _databaseService;

        public TableOrderViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
    }
}
