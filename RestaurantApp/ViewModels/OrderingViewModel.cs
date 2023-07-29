using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Table> _temporarySales = new ObservableCollection<Table>();

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

        public ObservableCollection<Table> TemporarySales
        {
            get
            {
                return _temporarySales;
            }

            set
            {
                _temporarySales = value;
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
                await EditTable(_table);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(TemporarySales));
        }

        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            int quantity = GetAvailableQuantity(article.ArticleDetails);

            if (article.Quantity < quantity)
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
                article.Quantity++;
            }
            else
            {
                _table.Available = false;
                article.Quantity = 1;
                _table.Articles.Add(article);
            }

            _temporarySales.Add(_table);
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
                _temporarySales.Add(_table);
            }


            _temporarySales = new ObservableCollection<Table>(await _databaseService.GetAllTables());
        }

        private async void DeleteArticleFromTable(Article article)
        {
            article.Quantity = 0;
            _table.Articles.Remove(article);
            //_temporarySales.Remove(_table.Articles);
            await EditTable(_table);

            if (_table.Articles.Count == 0)
            {
                _table.Available = true;
                await EditTable(_table);
            }

            RaisePropertyChanged(nameof(TemporarySales));
        }

        private async Task EditTable(Table table)
        {
            await _databaseService.EditTable(table);
        }

        private async void ShowPaymentUserControl()
        {
            if (TemporarySales.Count == 0)
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
