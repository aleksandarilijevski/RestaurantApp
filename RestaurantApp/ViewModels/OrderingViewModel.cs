using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private Table _table;
        private TableArticleQuantity _tableArticleQuantity;
        private string _barcode;
        private DelegateCommand<Table> _getTableCommand;
        private DelegateCommand _showPaymentUserControlCommand;
        private DelegateCommand<TableArticleQuantity> _deleteArticleFromTableCommand;
        private DelegateCommand _navigateToTablesCommand;
        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities;
        private int _quantityValueBeforeChange = 0;

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
                _barcode = value;
                RaisePropertyChanged();
            }
        }

        public TableArticleQuantity TableArticleQuantity
        {
            get
            {
                return _tableArticleQuantity;
            }

            set
            {
                if (_tableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _tableArticleQuantity.Quantity;

                    _tableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
                }

                _tableArticleQuantity = value;
                RaisePropertyChanged();

                if (_tableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _tableArticleQuantity.Quantity;

                    _tableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                }
            }
        }

        public ObservableCollection<TableArticleQuantity> TableArticleQuantities
        {
            get
            {
                return _tableArticleQuantities;
            }

            set
            {
                _tableArticleQuantities = value;
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

        public DelegateCommand<TableArticleQuantity> DeleteArticleFromTableCommand
        {
            get
            {
                _deleteArticleFromTableCommand = new DelegateCommand<TableArticleQuantity>(DeleteArticleFromTable);
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

        public DelegateCommand NavigateToTablesCommand
        {
            get
            {
                _navigateToTablesCommand = new DelegateCommand(NavigateToTables);
                return _navigateToTablesCommand;
            }
        }

        /// <summary>
        /// This functions is triggers every time Quantity cell in dataGrid is changed.
        /// </summary>
        private void OnQuantityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableArticleQuantity.Quantity))
            {
                Task isQuantityAvailableForArticleOnTable = Task.Run(() => IsQuantityAvailableForArticleOnTable(TableArticleQuantity.Article));
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
                Table table = new Table { ID = id, InUse = false, TableArticleQuantities = new List<TableArticleQuantity>() };
                Table = table;
                await _databaseService.AddTable(table);
            }

            if (Table.TableArticleQuantities is null)
            {
                Table.TableArticleQuantities = new List<TableArticleQuantity>();
            }

            TableArticleQuantities = new ObservableCollection<TableArticleQuantity>(_table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)));

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
                Barcode = string.Empty;
                return;
            }

            bool isQuantityAvailable = await IfQuantityIsAvailable(article);

            if (isQuantityAvailable)
            {
                Table.InUse = true;

                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID);

                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    //ArticleID = article.ID,
                    Article = article,
                    //TableID = Table.ID,
                    Table = Table,
                    Quantity = 1,
                    ArticleDetails = articleDetails
                };

                await IncreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity);

                Table.TableArticleQuantities.Add(tableArticleQuantity);
                TableArticleQuantities.Add(tableArticleQuantity);

                //await _databaseService.AddTableArticleQuantity(tableArticleQuantity);
                await EditTable(Table);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(Table));
        }

        /// <summary>
        /// Increasing reserved quantity
        /// </summary>
        private async Task IncreaseReservedQuantity(List<ArticleDetails> articleDetails, int quantityToBeReserved)
        {
            if (quantityToBeReserved != 1)
            {
                quantityToBeReserved = Math.Abs(_quantityValueBeforeChange - quantityToBeReserved);
            }

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (quantityToBeReserved <= 0)
                    break; // No more quantity to sell, exit the loop

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, quantityToBeReserved);

                // Reserve quantityToReserve for the current articleDetail
                articleDetail.ReservedQuantity += quantityToReserve;

                // Reduce remainingQuantityToSell
                quantityToBeReserved -= quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail);
            }
        }

        /// <summary>
        /// Gets all reserved quantities from each articleDetails.
        /// </summary>
        public int GetReservedQuantity(List<ArticleDetails> articleDetails)
        {
            int reservedQuantity = 0;
            reservedQuantity += articleDetails.Sum(x => x.ReservedQuantity);
            return reservedQuantity;
        }

        /// <summary>
        /// Checking if quantity is available for article on table.
        /// This function is called when quantity is changed from dataGrid cell.
        /// </summary>
        private async Task IsQuantityAvailableForArticleOnTable(Article article)
        {
            TableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
            int availableQuantity = GetAvailableQuantity(article.ArticleDetails);

            if (_quantityValueBeforeChange < TableArticleQuantity.Quantity)
            {
                if (availableQuantity >= TableArticleQuantity.Quantity - _quantityValueBeforeChange)
                {
                    await IncreaseReservedQuantity(article.ArticleDetails, TableArticleQuantity.Quantity);
                    await _databaseService.EditTableArticleQuantity(TableArticleQuantity);
                    TableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                }
                else
                {
                    TableArticleQuantity.Quantity = _quantityValueBeforeChange;
                    MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                int quantityToRemove = Math.Abs(TableArticleQuantity.Quantity - _quantityValueBeforeChange);
                await DecreaseReservedQuantity(article.ArticleDetails, quantityToRemove);
            }

            RaisePropertyChanged(nameof(TableArticleQuantities));
        }

        /// <summary>
        /// Decreasing reserved quantity.
        /// </summary>
        private async Task DecreaseReservedQuantity(List<ArticleDetails> articleDetails, int reservedQuantityToBeDecreased)
        {
            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (reservedQuantityToBeDecreased <= 0)
                    break; // No more quantity to sell, exit the loop

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, reservedQuantityToBeDecreased);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = reservedQuantityToBeDecreased;
                }

                // Reserve quantityToReserve for the current articleDetail
                articleDetail.ReservedQuantity -= quantityToReserve;

                // Reduce remainingQuantityToSell
                reservedQuantityToBeDecreased += quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail);
            }
        }

        /// <summary>
        /// Calculating if quantity is available
        /// </summary>
        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            int quantity = GetAvailableQuantity(article.ArticleDetails);

            if (quantity != 0)
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

            if (articleDetails != null)
            {
                quantity = articleDetails.Sum(x => x.OriginalQuantity - x.ReservedQuantity);
            }

            return quantity;
        }

        /// <summary>
        /// Deleting articles from table and restoring reserved quantity
        /// </summary>
        private async void DeleteArticleFromTable(TableArticleQuantity tableArticleQuantity)
        {
            List<ArticleDetails>? articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID);

            int quantityToRemove = TableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.Article.IsDeleted == false && articleDetail.Article.ID == tableArticleQuantity.Article.ID)
                {

                    if (articleDetail.ReservedQuantity != 0)
                    {
                        if (articleDetail.ReservedQuantity < TableArticleQuantity.Quantity)
                        {
                            int reservedDelete = Math.Min(articleDetail.ReservedQuantity, quantityToRemove);
                            articleDetail.ReservedQuantity -= reservedDelete;
                            quantityToRemove -= reservedDelete;

                            if (quantityToRemove != 0)
                            {
                                continue;
                            }

                        }
                        else
                        {
                            articleDetail.ReservedQuantity -= TableArticleQuantity.Quantity;

                        }
                    }
                    else
                    {
                        continue;
                    }

                    await _databaseService.EditArticleDetails(articleDetail);
                    break;
                }
            }

            //Table.TableArticleQuantities.Remove(tableArticleQuantity);
            TableArticleQuantities.Remove(tableArticleQuantity);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity);

            List<TableArticleQuantity> tableArticleQuantities = Table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)).ToList();

            if (tableArticleQuantities.Count == 0)
            {
                Table.InUse = false;
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
        /// Function for navigating to tables
        /// </summary>
        private void NavigateToTables()
        {
            _regionManager.RequestNavigate("MainRegion", "TableOrder");
        }

        /// <summary>
        /// Showing payment usercontrol
        /// If there are no articles on table we will get error message
        /// </summary>
        private void ShowPaymentUserControl()
        {
            if (TableArticleQuantities.Count == 0)
            {
                MessageBox.Show("There are no articles to be paid!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "table",  Table},
                { "tableArticleQuantities",  TableArticleQuantities.ToList()}
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
