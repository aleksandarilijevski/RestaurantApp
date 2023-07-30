using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
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
        private int _quantity;

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

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                _quantity = value;
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
            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcode(barcodeLong);

            if (article is null)
            {
                MessageBox.Show("Article with entered barcode doesn't exist in the system!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isAvailable = await IfQuantityIsAvailable(article);

            if (isAvailable)
            {
                await IfArticleExistsOnTable(article);

                TableArticleQuantity tableArticleQuantity = await GetTableArticleQuantity(article.ID, _table.ID);

                if (tableArticleQuantity is null)
                {
                    tableArticleQuantity = new TableArticleQuantity
                    {
                        ArticleID = article.ID,
                        TableID = _table.ID,
                        Quantity = _quantity
                    };

                    await AddTableArticleQuantity(tableArticleQuantity);
                }

                tableArticleQuantity.Quantity = _quantity;
                await EditTableArticleQuantity(tableArticleQuantity);
                await EditTable(_table);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(Table));
        }

        private async Task<TableArticleQuantity> GetTableArticleQuantity(int articleID, int tableID)
        {
            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantity(articleID, tableID);
            return tableArticleQuantity;
        }

        private async Task<int> GetTableArticleTotalQuantity(int articleID)
        {
            int usedQuantity = await _databaseService.GetTableArticleTotalQuantity(articleID);
            return usedQuantity;
        }

        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            int quantity = GetAvailableQuantity(article.ArticleDetails);
            int usedQuantity = await GetTableArticleTotalQuantity(article.ID);

            if (usedQuantity < quantity)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async Task IfArticleExistsOnTable(Article article)
        {
            if (_table.Articles.Contains(article))
            {
                _quantity++;
            }
            else
            {
                _table.Available = false;
                _quantity = 1;
                _table.Articles.Add(article);
            }
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

            RaisePropertyChanged(nameof(Table));
        }

        private async void DeleteArticleFromTable(Article article)
        {
            _table.Articles.Remove(article);
            await EditTable(_table);

            if (_table.Articles.Count == 0)
            {
                _table.Available = true;
                await EditTable(_table);
            }

            //RaisePropertyChanged(nameof(TemporarySales));
        }

        private async Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            await _databaseService.AddTableArticleQuantity(tableArticleQuantity);
        }

        private async Task EditTable(Table table)
        {
            await _databaseService.EditTable(table);
        }

        private async Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            await _databaseService.EditTableArticleQuantity(tableArticleQuantity);
        }

        private async void ShowPaymentUserControl()
        {
            if (_table.Articles.Count == 0)
            {
                MessageBox.Show("There are no articles to be paid!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "table",  _table}
            };

            _regionManager.RequestNavigate("MainRegion", "Payment", navigationParameters);
        }

        //public void SellArticles(List<Article> articles)
        //{
        //    foreach (Article article in articles)
        //    {
        //        foreach (ArticleDetails articleDetails in article.ArticleDetails)
        //        {
        //            if (article.Quantity <= articleDetails.Quantity)
        //            {
        //                articleDetails.Quantity -= article.Quantity;
        //                article.Quantity = 0;
        //                _databaseService.EditArticleDetails(articleDetails);
        //                break;
        //            }
        //            else
        //            {
        //                article.Quantity -= articleDetails.Quantity;
        //                articleDetails.Quantity = 0;

        //                _databaseService.EditArticleDetails(articleDetails);
        //            }
        //        }
        //    }
        //}
    }
}
