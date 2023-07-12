using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<string> _addArticleToTableCommand;
        private int _id;
        private Table _table;
        private DelegateCommand<Table> _getTableCommand;

        public int ID
        {
            get { return _id; }
        }

        public OrderingViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Table Table
        {
            get
            {
                return _table;
            }

            set
            {
                _table = value;
                RaisePropertyChanged();
            }
        }

        public string Barcode
        {
            set
            {
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<Table> GetTableCommand
        {
            get
            {
                _getTableCommand = new DelegateCommand<Table>(async async => await GetTable(_id));
                return _getTableCommand;
            }
        }

        public DelegateCommand<string> AddArticleToTableCommand
        {
            get
            {
                _addArticleToTableCommand = new DelegateCommand<string>(AddArticleToTable);
                return _addArticleToTableCommand;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _id = int.Parse(navigationContext.Parameters["id"].ToString());
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async void AddArticleToTable(string barcode)
        {
            Article article = await _databaseService.GetArticleByBarcode(long.Parse(barcode));

            if (article is not null)
            {
                CheckIfArticleExists(article);
            }

            Barcode = string.Empty;
        }

        private async void CheckIfArticleExists(Article article)
        {
            if (_table.Articles.Contains(article))
            {
                article.Quantity++;
            }
            else
            {
                article.Quantity = 1;
                _table.Articles.Add(article);
            }
        }

        private async Task GetTable(int id)
        {
            _table = await _databaseService.GetTableByID(id);
        }
    }
}
    