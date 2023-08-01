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
        private Table _table;
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

        /// <summary>
        /// While loading usercontrol it's getting table by id from the database
        /// </summary>
        private async Task GetTable(int id)
        {
            Table = await _databaseService.GetTableByID(id);

            if (Table is null)
            {
                Table table = new Table { Available = true };
                Table = table;
                await _databaseService.AddTable(table);
            }

            RaisePropertyChanged(nameof(Table));
        }

        /// <summary>
        /// Adding article to the table
        /// </summary>
        private async void AddArticleToTable(string barcode)
        {
            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcode(barcodeLong);

            if (article is null)
            {
                MessageBox.Show("Article with entered barcode doesn't exist in the system!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isQuantityAvailable = await IfQuantityIsAvailable(article);

            if (isQuantityAvailable)
            {
                TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantity(article.ID, _table.ID);

                if (tableArticleQuantity is null)
                {
                    tableArticleQuantity = new TableArticleQuantity
                    {
                        ArticleID = article.ID,
                        TableID = Table.ID
                    };

                    await AddTableArticleQuantity(tableArticleQuantity);
                }

                await CheckIfArticleIsOnTable(article, tableArticleQuantity);
                await EditTableArticleQuantity(tableArticleQuantity);
                await EditTable(_table);
            }

            Barcode = string.Empty;
        }

        /// <summary>
        /// Checking if article is on table
        /// If article is on table do quantity++
        /// If article is not on the table, add it and set quantity to 1
        /// </summary>
        private async Task CheckIfArticleIsOnTable(Article article,TableArticleQuantity tableArticleQuantity)
        {
            if (_table.TableArticleQuantities.Count is 0)
            {
                _table.Available = false;
                tableArticleQuantity.Quantity = 1;
                _table.TableArticleQuantities.Add(tableArticleQuantity);
            }
            else
            {
                tableArticleQuantity.Quantity++;
            }
        }

        /// <summary>
        /// Calculating if quantity is available
        /// </summary>
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

        /// <summary>
        /// Calculating available quantity of the article
        /// </summary>
        private int GetAvailableQuantity(List<ArticleDetails> articleDetails)
        {
            int quantity = 0;

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                quantity += articleDetail.Quantity;
            }

            return quantity;
        }

        private async void DeleteArticleFromTable(Article article)
        {
            TableArticleQuantity tableArticleQuantity = await GetTableArticleQuantity(article.ID, _table.ID);
            Table.TableArticleQuantities.Remove(tableArticleQuantity);
            await EditTable(Table);

            if (Table.TableArticleQuantities.Count == 0)
            {
                Table.Available = true;
                await EditTable(Table);
            }

            RaisePropertyChanged(nameof(Table));
        }

        /// <summary>
        /// Function for adding table model
        /// </summary>
        private async Task EditTable(Table table)
        {
            await _databaseService.EditTable(table);
        }

        /// <summary>
        /// Function for adding table article quantity model
        /// </summary>
        private async Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            await _databaseService.AddTableArticleQuantity(tableArticleQuantity);
        }

        /// <summary>
        /// Function for editing table article quantity model
        /// </summary>
        private async Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            await _databaseService.EditTableArticleQuantity(tableArticleQuantity);
        }

        /// <summary>
        /// Getting table article quantity model by ArticleID and TableID from database.
        /// </summary>
        private async Task<TableArticleQuantity> GetTableArticleQuantity(int articleID, int tableID)
        {
            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantity(articleID, tableID);
            return tableArticleQuantity;
        }

        /// <summary>
        /// Getting table article total used quantity by ArticleID
        /// </summary>
        private async Task<int> GetTableArticleTotalQuantity(int articleID)
        {
            int usedQuantity = await _databaseService.GetTableArticleTotalQuantity(articleID);
            return usedQuantity;
        }

        /// <summary>
        /// Showing payment usercontrol
        /// If there are no articles on table we will get error message
        /// </summary>
        private async void ShowPaymentUserControl()
        {
            if (Table.TableArticleQuantities.Count == 0)
            {
                MessageBox.Show("There are no articles to be paid!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "table",  Table}
            };

            _regionManager.RequestNavigate("MainRegion", "Payment", navigationParameters);
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
    }
}
