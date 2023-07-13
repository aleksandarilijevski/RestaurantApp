using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<string> _addArticleToTableCommand;
        private int _id;
        private Table _table = new Table();
        private Article _article;
        private DelegateCommand<Table> _getTableCommand;
        private DelegateCommand<Article> _deleteArticleFromTableCommand;

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

        public List<Article> Articles
        {

            get
            {
                return _table.Articles;
            }

            set
            {
                _table.Articles = value;
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
                _getTableCommand = new DelegateCommand<Table>(async x => await GetTable(_id));
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

        public DelegateCommand<Article> DeleteArticleFromTableCommand
        {
            get
            {
                _deleteArticleFromTableCommand = new DelegateCommand<Article>(async x => await DeleteArticleFromTable(_article));
                return _deleteArticleFromTableCommand;
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
                await CheckIfArticleExists(article);
                await EditTable(_table);
            }

            Barcode = string.Empty;
        }

        private async Task CheckIfArticleExists(Article article)
        {
            article.ArticleQuantity = await _databaseService.GetArticleQuantityByArticleID(article.ID);

            if (_table.Articles.Contains(article))
            {
                article.Quantity++;
            }
            else
            {
                _table.Available = false;
                article.Quantity = 1;
                _table.Articles.Add(article);
            }

            article.ArticleQuantity.Quantity--;
            Articles = _table.Articles;
        }

        private async Task GetTable(int id)
        {
            _table = await _databaseService.GetTableByID(id);

            if (_table.Articles is null)
            {
                _table.Articles = new List<Article>();
            }

            if (_table.Articles is not null)
            {
                Articles = _table.Articles;
            }
        }

        private async Task DeleteArticleFromTable(Article article)
        {
            _table.Articles.Remove(article);
            await EditTable(_table);
        }

        private async Task EditTable(Table table)
        {
            await _databaseService.EditTable(table);
        }
    }
}
