using EntityFramework.Models;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class ArticleManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;

        public Task<List<Artical>> Articals
        {
            get
            {
                return _databaseService.GetAllArticals();
            }
        }

        public ArticleManagementViewModel(IDatabaseService databaseService)
        {
             _databaseService = databaseService;
        }
    }
}
