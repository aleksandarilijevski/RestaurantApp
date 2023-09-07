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
        //Private fields
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

        //Constructor
        public OrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        //Public properties
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

        //Delegate commands
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
            using EFContext efContext = new EFContext();
            //Gets article from database by barcode.
            long.TryParse(barcode, out long barcodeLong);


            //It's adding articleDetails too! because efContext instance remains same.
            Article article = await _databaseService.GetArticleByBarcodeContext(barcodeLong,efContext);

            //If article is null display error message.
            if (article is null)
            {
                MessageBox.Show("Article with entered barcode doesn't exist in the system!", "Ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                Barcode = string.Empty;
                return;
            }

            //Check if quantity is available.
            bool isQuantityAvailable = await IfQuantityIsAvailable(article);

            if (isQuantityAvailable)
            {
                //Table property InUse is changed to true, table is becoming active.
                Table.InUse = true;

                //Gets all articleDetails by articleId
                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleIDContext(article.ID, efContext);

                //Creating tableArticleQuantity object and providing required data.
                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    //There was ArticleID = article.ID, before disposing EFContext
                    Article = article,
                    TableID = Table.ID,
                    Quantity = 1,
                    ArticleDetails = articleDetails
                };

                //We are increasing reserved quantity of articleDetails
                await IncreaseReservedQuantity(articleDetails, tableArticleQuantity.Quantity);

                //Adds tableArticleQuantity to table.
                Table.TableArticleQuantities.Add(tableArticleQuantity);
                //This is removed due tests with disposing EFContexts

                //Adds tableArticleQuantity to observableCollection
                TableArticleQuantities.Add(tableArticleQuantity);

                //Edits table
                //await _databaseService.EditTableContext(Table,efContext);
                //This is removed due tests with disposing EFContexts

                //Added due tests.
                //It says articleDetails is already being tracked.
                await _databaseService.AddTableArticleQuantityContext(tableArticleQuantity,efContext);
            }

            //Clearing barcode textBox and updating Table.
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
                    break;

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, quantityToBeReserved);

                articleDetail.ReservedQuantity += quantityToReserve;

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
            //Sums all reservedQuantity from provided articleDetails
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
                    break;

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, reservedQuantityToBeDecreased);

                if (articleDetail.OriginalQuantity == articleDetail.ReservedQuantity)
                {
                    quantityToReserve = reservedQuantityToBeDecreased;
                }

                articleDetail.ReservedQuantity -= quantityToReserve;

                reservedQuantityToBeDecreased += quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail);
            }
        }

        /// <summary>
        /// Calculating if quantity is available
        /// </summary>
        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            //Added due tests and tracking
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID);

            //article.ArticleDetails was parametar before change
            int quantity = GetAvailableQuantity(articleDetails);

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

            TableArticleQuantities.Remove(tableArticleQuantity);

            //Should be deleted from same instance
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
