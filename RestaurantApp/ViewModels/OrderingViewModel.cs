﻿using EntityFramework.Models;
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
        private int _quantityValueBeforeChange = 0;

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
                if (_selectedTableArticleQuantity != null)
                {
                    _quantityValueBeforeChange = _selectedTableArticleQuantity.Quantity;
                    _selectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
                }

                _selectedTableArticleQuantity = value;
                RaisePropertyChanged();

                if (_selectedTableArticleQuantity != null)
                {
                    _selectedTableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                    _quantityValueBeforeChange = _selectedTableArticleQuantity.Quantity;
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
            using EFContext efContext = new EFContext();

            Debug.WriteLine("Trigger method");

            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantityByID(selectedTableArticleQuantity.ID,efContext);
            int oldQuantity = tableArticleQuantity.Quantity;

            tableArticleQuantity.Quantity = selectedTableArticleQuantity.Quantity;
            await _databaseService.EditTableArticleQuantity(tableArticleQuantity,efContext);

            if (selectedTableArticleQuantity.Quantity > oldQuantity)
            {
                int calculateHowMuchToIncrease = selectedTableArticleQuantity.Quantity - oldQuantity;
                int quantityToAdd = calculateHowMuchToIncrease;
                await IncreaseQuantity(tableArticleQuantity, selectedTableArticleQuantity.ArticleDetails, quantityToAdd, efContext);
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

            tableArticleQuantity.Quantity = quantity;
            await _databaseService.EditTableArticleQuantity(tableArticleQuantity, efContext);

            foreach (ArticleDetails articleDetail in articleDetails)
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

        private async Task DecreaseQuantity(TableArticleQuantity tableArticleQuantity, List<ArticleDetails> articleDetails, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(tableArticleQuantity.ArticleID, efContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            int quantityToBeRemoved = tableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                if (quantityToBeRemoved <= 0)
                {
                    break;
                }

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToRemove = Math.Max(quantityToBeRemoved, availableQuantity);

                if (availableQuantity == 0)
                {
                    int reservedQuantity = 1;
                    articleDetail.ReservedQuantity -= reservedQuantity;
                    quantityToBeRemoved = Math.Abs(reservedQuantity - quantityToBeRemoved);
                    await _databaseService.EditArticleDetails(articleDetail, efContext);
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
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity, efContext);

            List<TableArticleQuantity> tableArticleQuantities = Table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)).ToList();

            if (tableArticleQuantities.Count == 0)
            {
                Table.InUse = false;
                await _databaseService.EditTable(Table, new EFContext());
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
