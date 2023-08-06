using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.ComponentModel;
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

        public OrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public int TableID
        {
            get { return _tableId; }
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
            //Gets table from database
            Table tableCheck = await _databaseService.GetTableByID(id);

            //Check if table is null
            //If table is null, create new providing id and availability, add to database
            if (tableCheck is null)
            {
                Table table = new Table { ID = id, Available = true, TableArticleQuantities = new List<TableArticleQuantity>() };
                Table = table;
                tableCheck = table;
                await _databaseService.AddTable(table);
            }


            bool activeArticle = false;
            List<TableArticleQuantity> tableArticleQuantities = new List<TableArticleQuantity>();

            //Check if there is non deleted article in table, if article is not deleted add it to new list and set activeArticle value to true
            foreach (TableArticleQuantity tableArticleQuantity in tableCheck.TableArticleQuantities)
            {
                if (tableArticleQuantity.IsDeleted == false)
                {
                    activeArticle = true;
                    tableArticleQuantities.Add(tableArticleQuantity);
                }
            }

            //If article is active replace list with new list.
            //If not clear the list.
            if (activeArticle is true)
            {
                tableCheck.TableArticleQuantities = tableArticleQuantities;
                Table = tableCheck;
            }
            else
            {
                tableCheck.TableArticleQuantities.Clear();
                Table = tableCheck;
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
                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    ArticleID = article.ID,
                    TableID = Table.ID,
                    Quantity = 1
                };

                Table.TableArticleQuantities.Add(tableArticleQuantity);
            }

            await EditTable(Table);
            Barcode = string.Empty;
            RaisePropertyChanged(nameof(Table));
        }

        /// <summary>
        /// Checking if article is on table
        /// If article is on table do quantity++
        /// If article is not on the table, add it and set quantity to 1
        /// </summary>
        private async Task AddArticleToTable(Article article, TableArticleQuantity tableArticleQuantity)
        {
            Table.Available = false;
            tableArticleQuantity.Quantity = 1;
            Table.TableArticleQuantities.Add(tableArticleQuantity);
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
            //Table.TableArticleQuantities.Remove(article);
            //await EditTable(Table);

            //if (Table.TableArticleQuantities.Count == 0)
            //{
            //    Table.Available = true;
            //    await EditTable(Table);
            //}

            //RaisePropertyChanged(nameof(Table));
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
        private async Task<List<TableArticleQuantity>> GetTableArticleQuantities(int articleID, int tableID)
        {
            List<TableArticleQuantity> tableArticleQuantity = await _databaseService.GetTableArticleQuantities(articleID, tableID);
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
