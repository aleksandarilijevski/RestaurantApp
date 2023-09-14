using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        private Table _table;
        private TableArticleQuantity _selectedTableArticleQuantity;
        private string _barcode;
        private DelegateCommand<Table> _getTableCommand;
        private DelegateCommand _showPaymentUserControlCommand;
        private DelegateCommand<TableArticleQuantity> _deleteArticleFromTableCommand;
        private DelegateCommand _navigateToTablesCommand;
        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities;

        public OrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public int TableID { get; set; }

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

        public TableArticleQuantity SelectedTableArticleQuantity
        {
            get
            {
                return _selectedTableArticleQuantity;
            }

            set
            {
                _selectedTableArticleQuantity = value;
                RaisePropertyChanged();

                if (_selectedTableArticleQuantity != null)
                {
                    _selectedTableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
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
                _getTableCommand = new DelegateCommand<Table>(async x => await GetTable(TableID));
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            TableID = int.Parse(navigationContext.Parameters["id"].ToString());
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void OnQuantityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableArticleQuantity.Quantity))
            {
                _ = IsQuantityAvailableForArticleOnTable(SelectedTableArticleQuantity);
            }
        }

        private async Task GetTable(int id)
        {
            using EFContext efContext = new EFContext();

            Table = await _databaseService.GetTableByID(id, efContext);

            if (Table is null)
            {
                Table table = new Table { ID = id, InUse = false, TableArticleQuantities = new List<TableArticleQuantity>() };
                Table = table;
                await _databaseService.AddTable(table, efContext);
            }

            if (Table.TableArticleQuantities is null)
            {
                Table.TableArticleQuantities = new List<TableArticleQuantity>();
            }

            TableArticleQuantities = new ObservableCollection<TableArticleQuantity>(_table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)));

            RaisePropertyChanged(nameof(Table));
        }

        private async void AddArticleToTable(string barcode)
        {
            using EFContext efContext = new EFContext();

            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcodeContext(barcodeLong, efContext);

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

                List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID, efContext);

                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    Article = article,
                    TableID = Table.ID,
                    Quantity = 1,
                    ArticleDetails = articleDetails
                };

                Table.TableArticleQuantities.Add(tableArticleQuantity);
                TableArticleQuantities.Add(tableArticleQuantity);

                await _databaseService.AddTableArticleQuantity(tableArticleQuantity, efContext);
                await IncreaseQuantity(tableArticleQuantity, articleDetails, tableArticleQuantity.Quantity, efContext);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(Table));
        }

        private async Task IsQuantityAvailableForArticleOnTable(TableArticleQuantity selectedTableArticleQuantity)
        {
            _selectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;

            using EFContext efContext = new EFContext();

            Debug.WriteLine("Trigger method");

            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantityByID(selectedTableArticleQuantity.ID, efContext);
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(selectedTableArticleQuantity.ArticleID, efContext);

            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(selectedTableArticleQuantity.ArticleID, efContext);
            int availableReservedQuantity = GetAvailableQuantity(articleDetails) + tableArticleQuantity.Quantity;

            if (availableReservedQuantity >= selectedTableArticleQuantity.Quantity)
            {
                int oldQuantity = tableArticleQuantity.Quantity;

                tableArticleQuantity.Quantity = selectedTableArticleQuantity.Quantity;
                await _databaseService.EditTableArticleQuantity(tableArticleQuantity, efContext);

                if (selectedTableArticleQuantity.Quantity > oldQuantity)
                {
                    int calculateHowMuchToIncrease = selectedTableArticleQuantity.Quantity - oldQuantity;
                    await IncreaseQuantity(tableArticleQuantity, articleDetails, calculateHowMuchToIncrease, efContext);
                }
                else
                {
                    int calculateHowMuchToDecrease = Math.Abs(selectedTableArticleQuantity.Quantity - oldQuantity);
                    await DecreaseQuantityFromCell(selectedTableArticleQuantity, articleDetails, calculateHowMuchToDecrease, efContext);
                }
            }
            else
            {
                SelectedTableArticleQuantity.Quantity = tableArticleQuantity.Quantity;
                MessageBox.Show("Article is not in stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RaisePropertyChanged(nameof(TableArticleQuantities));
        }

        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            using EFContext efContext = new EFContext();

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID, efContext);

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
        private async Task IncreaseQuantity(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, int quantity, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (quantity <= 0)
                {
                    break;
                }

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, quantity);

                articleDetail.ReservedQuantity += quantityToReserve;
                quantity -= quantityToReserve;

                await _databaseService.EditArticleDetails(articleDetail, efContext);
            }
        }

        private async Task DecreaseQuantityFromCell(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, int quantityToBeRemoved, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.ReservedQuantity != 0)
                {
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (quantityToBeRemoved == 1)
                    {
                        articleDetail.ReservedQuantity--; ;
                        await _databaseService.EditArticleDetails(articleDetail, efContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        private async Task DecreaseQuantity(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            int quantityToBeRemoved = tableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.ReservedQuantity != 0)
                {
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await _databaseService.EditArticleDetails(articleDetail, efContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (quantityToBeRemoved == 1)
                    {
                        articleDetail.ReservedQuantity--; ;
                        await _databaseService.EditArticleDetails(articleDetail, efContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        private int GetAvailableQuantity(List<ArticleDetails> articleDetails)
        {
            int quantity = 0;

            if (articleDetails != null)
            {
                quantity = articleDetails.Sum(x => x.OriginalQuantity - x.ReservedQuantity);
            }

            return quantity;
        }

        private async void DeleteArticleFromTable(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            int quantityToRemove = SelectedTableArticleQuantity.Quantity;

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);
            await DecreaseQuantity(tableArticleQuantity, articleDetails, efContext);

            TableArticleQuantities.Remove(tableArticleQuantity);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity, new EFContext());

            List<TableArticleQuantity> tableArticleQuantities = Table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)).ToList();

            if (tableArticleQuantities.Count == 0)
            {
                Table.InUse = false;
                await _databaseService.EditTable(Table, efContext);
            }

            RaisePropertyChanged(nameof(Table));
        }

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

        private void NavigateToTables()
        {
            _regionManager.RequestNavigate("MainRegion", "TableOrder");
        }
    }
}
