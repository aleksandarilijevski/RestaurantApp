using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private DelegateCommand<string> _addArticleToTableCommand;
        private int _tableId;
        private Table _table = new Table();
        private Article _article;
        private string _barcode;
        private DelegateCommand<Table> _getTableCommand;
        private DelegateCommand _showPaymentUserControlCommand;
        private DelegateCommand<Article> _deleteArticleFromTableCommand;

        public int TableID
        {
            get { return _tableId; }
        }

        public OrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public Article Article
        {
            get
            {
                return _article;
            }

            set
            {
                SetProperty(ref _article, value);
            }
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
            get
            {
                return _barcode;
            }

            set
            {
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<Table> GetTableCommand
        {
            get
            {
                _getTableCommand = new DelegateCommand<Table>(async x => await GetTable(_tableId));
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

        public DelegateCommand ShowPaymentUserControlCommand
        {
            get
            {
                _showPaymentUserControlCommand = new DelegateCommand(ShowPaymentUserControl);
                return _showPaymentUserControlCommand;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _tableId = int.Parse(navigationContext.Parameters["id"].ToString());
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

        private async void ShowPaymentUserControl()
        {
            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "table",  _table}
            };

            _regionManager.RequestNavigate("MainRegion", "Payment",navigationParameters);
        }

    }
}
