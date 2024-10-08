﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Models;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
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
        private string _barcode;

        private IDatabaseService _databaseService;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private DelegateCommand<string> _addArticleToTableCommand;

        private DelegateCommand<Table> _getTableCommand;

        private DelegateCommand _showPaymentUserControlCommand;

        private DelegateCommand<TableArticleQuantity> _deleteArticleFromTableCommand;

        private DelegateCommand _navigateToTablesCommand;

        private DelegateCommand _showInvoiceHistoryDialogCommand;

        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities;

        private TableArticleQuantity _selectedTableArticleQuantity;

        private Table _table;

        public OrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        public int TableID { get; set; }

        public User User { get; set; }

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
                if (_selectedTableArticleQuantity != value)
                {
                    if (_selectedTableArticleQuantity != null)
                    {
                        _selectedTableArticleQuantity.PropertyChanged -= OnQuantityPropertyChanged;
                    }

                    _selectedTableArticleQuantity = value;

                    if (_selectedTableArticleQuantity != null)
                    {
                        _selectedTableArticleQuantity.PropertyChanged += OnQuantityPropertyChanged;
                    }
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

        public DelegateCommand ShowInvoiceHistoryDialogCommand
        {
            get
            {
                _showInvoiceHistoryDialogCommand = new DelegateCommand(ShowInvoiceHistoryDialog);
                return _showInvoiceHistoryDialogCommand;
            }
        }

        private async void ShowInvoiceHistoryDialog()
        {
            List<Bill> originalBills = await _databaseService.GetAllBills();
            List<Bill> billsFilteredByDate = originalBills.Where(x => x.CreatedDateTime?.Date == DateTime.Today && x.TableID == Table.ID).ToList();

            DialogParameters dialogParameters = new DialogParameters
            {
                { "bills", billsFilteredByDate},
                { "tableID", Table.ID},
            };

            _dialogService.ShowDialog(ViewConstants.TableInvoiceHistoryDialogViewName, dialogParameters, r => { });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            TableID = int.Parse(navigationContext.Parameters["id"].ToString());
        }

        private async void OnQuantityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableArticleQuantity.Quantity))
            {
                await IsQuantityAvailableForArticleOnTable(_selectedTableArticleQuantity);
            }
        }

        private bool UserLogin()
        {
            bool isResultGood = false;

            _dialogService.ShowDialog(ViewConstants.UserLoginDialogViewName, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    isResultGood = true;
                    User = r.Parameters.GetValue<User>("user");
                }
                else
                {
                    isResultGood = false;
                }
            });

            return isResultGood;
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

            List<TableArticleQuantity> filtered = Table.TableArticleQuantities.Where(x => x is not (SoldTableArticleQuantity)).ToList();

            bool dialogResult = UserLogin();

            if (dialogResult)
            {
                if (Table.User is null || filtered.Count == 0)
                {
                    Table.UserID = User.ID;
                    Table.User = await _databaseService.GetUserByID(User.ID, efContext);

                    User.IsActive = true;
                    await _databaseService.EditUser(Table.User, efContext);
                }

                if (User.Barcode != Table.User.Barcode)
                {
                    MessageBox.Show(MessageBoxConstants.TableIsAlreadyInUseByAnotherWaiter, MessageBoxConstants.OrderingTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.TableOrderViewName);
                    return;
                }

                await _databaseService.EditTable(Table, efContext);
            }
            else
            {
                _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.TableOrderViewName);
                return;
            }

            if (Table.TableArticleQuantities is null)
            {
                Table.UserID = User.ID;
                Table.TableArticleQuantities = new List<TableArticleQuantity>();
                await _databaseService.EditTable(Table, efContext);
            }

            TableArticleQuantities = new ObservableCollection<TableArticleQuantity>(_table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)));
            RaisePropertyChanged(nameof(Table));
        }

        private async void AddArticleToTable(string barcode)
        {
            using EFContext efContext = new EFContext();

            long.TryParse(barcode, out long barcodeLong);
            Article article = await _databaseService.GetArticleByBarcode(barcodeLong, efContext);

            if (article is null)
            {
                MessageBox.Show(MessageBoxConstants.ArticleWithEnteredBarcodeDoesntExistInTheSystem, MessageBoxConstants.OrderingTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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

                ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                {
                    TableArticleQuantity = tableArticleQuantity,
                    ArticleDetails = articleDetails,
                    Quantity = tableArticleQuantity.Quantity,
                    DatabaseService = _databaseService,
                    EFContext = efContext
                };

                await QuantityLogicHelper.IncreaseReservedQuantity(articleHelperDetails);
            }

            await _databaseService.EditTable(Table, efContext);

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(Table));
        }

        private async Task IsQuantityAvailableForArticleOnTable(TableArticleQuantity selectedTableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            TableArticleQuantity tableArticleQuantity = await _databaseService.GetTableArticleQuantityByID(selectedTableArticleQuantity.ID, efContext);
            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(selectedTableArticleQuantity.ArticleID, efContext);

            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(selectedTableArticleQuantity.ArticleID, efContext);
            int availableReservedQuantity = QuantityLogicHelper.GetAvailableQuantity(articleDetails) + tableArticleQuantity.Quantity;

            if (selectedTableArticleQuantity.Quantity < 1)
            {
                SelectedTableArticleQuantity.Quantity = tableArticleQuantity.Quantity;
                MessageBox.Show(MessageBoxConstants.QuantityCantBeLowerThanOne, MessageBoxConstants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                RaisePropertyChanged(nameof(TableArticleQuantities));
                return;
            }

            if (availableReservedQuantity >= selectedTableArticleQuantity.Quantity)
            {
                int oldQuantity = tableArticleQuantity.Quantity;

                tableArticleQuantity.Quantity = selectedTableArticleQuantity.Quantity;
                await _databaseService.EditTableArticleQuantity(tableArticleQuantity, efContext);

                if (selectedTableArticleQuantity.Quantity > oldQuantity)
                {
                    int quantityToIncrease = selectedTableArticleQuantity.Quantity - oldQuantity;

                    ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                    {
                        TableArticleQuantity = selectedTableArticleQuantity,
                        ArticleDetails = articleDetails,
                        Quantity = quantityToIncrease,
                        DatabaseService = _databaseService,
                        EFContext = efContext
                    };

                    await QuantityLogicHelper.IncreaseReservedQuantity(articleHelperDetails);
                }
                else
                {
                    int quantityToDecrease = Math.Abs(selectedTableArticleQuantity.Quantity - oldQuantity);

                    ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
                    {
                        TableArticleQuantity = selectedTableArticleQuantity,
                        ArticleDetails = articleDetails,
                        Quantity = quantityToDecrease,
                        DatabaseService = _databaseService,
                        EFContext = efContext
                    };

                    await QuantityLogicHelper.DecreaseQuantityFromCell(articleHelperDetails);
                }
            }
            else
            {
                selectedTableArticleQuantity.Quantity = tableArticleQuantity.Quantity;
                MessageBox.Show(MessageBoxConstants.ArticleIsNotInStock, MessageBoxConstants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RaisePropertyChanged(nameof(TableArticleQuantities));
        }

        private async Task<bool> IfQuantityIsAvailable(Article article)
        {
            using EFContext efContext = new EFContext();

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID, efContext);

            int quantity = QuantityLogicHelper.GetAvailableQuantity(articleDetails);

            if (quantity != 0)
            {
                return true;
            }
            else
            {
                MessageBox.Show(MessageBoxConstants.ArticleIsNotInStock, MessageBoxConstants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void DeleteArticleFromTable(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            int quantityToDecrease = SelectedTableArticleQuantity.Quantity;

            List<ArticleDetails> articleDetails = await _databaseService.GetArticleDetailsByArticleID(tableArticleQuantity.ArticleID, efContext);

            ArticleHelperDetails articleHelperDetails = new ArticleHelperDetails
            {
                TableArticleQuantity = tableArticleQuantity,
                ArticleDetails = articleDetails,
                DatabaseService = _databaseService,
                EFContext = efContext
            };

            await QuantityLogicHelper.DecreaseReservedQuantity(articleHelperDetails);

            TableArticleQuantity tableArticleQuantityLoad = await _databaseService.GetTableArticleQuantityByID(tableArticleQuantity.ID, efContext);

            TableArticleQuantities.Remove(tableArticleQuantity);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantityLoad, efContext);

            if (TableArticleQuantities.Count == 0)
            {
                Table.InUse = false;
                Table.UserID = null;
                await _databaseService.EditTable(Table, efContext);
                _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.TableOrderViewName);
            }

            RaisePropertyChanged(nameof(Table));
        }

        private void ShowPaymentUserControl()
        {
            if (TableArticleQuantities.Count == 0)
            {
                MessageBox.Show(MessageBoxConstants.ThereAreNoArticlesToBePaid, MessageBoxConstants.OrderingTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "table",  Table},
                { "tableArticleQuantities",  TableArticleQuantities.ToList()}
            };

            _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.PaymentViewName, navigationParameters);
        }

        private void NavigateToTables()
        {
            _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.TableOrderViewName);
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
