using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Windows.Documents;

namespace RestaurantApp.ViewModels
{
    public class ArticalManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;

        public List<Artical> Articals
        {
            get
            {
                return _databaseService.GetAllArticals();
            }
        }

        public ArticalManagementViewModel(IDatabaseService databaseService)
        {
             _databaseService = databaseService;
        }
    }
}
