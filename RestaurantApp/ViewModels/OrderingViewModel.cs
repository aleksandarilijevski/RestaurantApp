using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private DelegateCommand<string> _addArticleToTableCommand;
        private int _tableId;
        private Table _table = new Table();
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
                _deleteArticleFromTableCommand = new DelegateCommand<Article>(DeleteArticleFromTable);
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
                bool isAvailable = await CheckIfQuantityIsAvailable(article);

                if (isAvailable)
                {
                    await CheckIfArticleExistsOnTable(article);
                    await EditTable(_table);
                }
            }

            Barcode = string.Empty;
        }

        private async Task CheckIfArticleExistsOnTable(Article article)
        {

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


        private void Test(Article article)
        {
            List<ArticleDetails> articleDetails = article.ArticleDetails.OrderByDescending(x => x.CreatedDateTime).ToList();

            DateTime oldest = article.ArticleDetails.Min(x => x.CreatedDateTime);

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (oldest == articleDetail.CreatedDateTime)
                {
                    if (article.Quantity == articleDetail.Quantity)
                    {
                        MessageBox.Show("", $"Quantity {articleDetail.Quantity} Entry Price {articleDetail.EntryPrice} Created Date {articleDetail.CreatedDateTime.ToString()}");
                    }
                }
            }
        }

        private async Task<bool> CheckIfQuantityIsAvailable(Article article)
        {
            int quantity = GetAvailableQuantity(article.ArticleDetails);

            if (article.Quantity < quantity)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        private int GetAvailableQuantity(List<ArticleDetails> articleDetails)
        {
            int quantity = 0;

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                quantity += articleDetail.Quantity;
            }

            return quantity;
        }

        private async Task GetTable(int id)
        {
            _table = await _databaseService.GetTableByID(id);

            if (_table is null)
            {
                Table table = new Table { Available = true };
                _table = table;
                await _databaseService.AddTable(table);
            }

            if (_table.Articles is null)
            {
                _table.Articles = new List<Article>();
            }

            if (_table.Articles.Count > 0)
            {
                Articles = _table.Articles;
            }
        }

        private async void DeleteArticleFromTable(Article article)
        {
            article.Quantity = 0;
            _table.Articles.Remove(article);
            await EditTable(_table);

            if (_table.Articles.Count == 0)
            {
                _table.Available = true;
                await EditTable(_table);
            }

            RaisePropertyChanged(nameof(Articles));
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

            _regionManager.RequestNavigate("MainRegion", "Payment", navigationParameters);
        }

    }
}
