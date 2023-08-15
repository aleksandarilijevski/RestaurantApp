using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase, INavigationAware, INotifyPropertyChanged
    {
        private IDatabaseService _databaseService;
        private IRegionManager _regionManager;
        private DelegateCommand<string> _addArticleToTableCommand;
        private int _tableId;
        private Table _table;
        private string _barcode;
        private DelegateCommand<Table> _getTableCommand;
        private DelegateCommand _showPaymentUserControlCommand;
        private DelegateCommand<TableArticleQuantity> _deleteArticleFromTableCommand;
        private ObservableCollection<TableArticleQuantity> _tableArticleQuantities;

        public OrderingViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        //public event PropertyChangedEventHandler PropertyChanged;

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

        private TableArticleQuantity _tableArticleQuantity;

        public TableArticleQuantity TableArticleQuantity
        {
            get
            {
                return _tableArticleQuantity;
            }

            set
            {
                _tableArticleQuantity = value;
                RaisePropertyChanged();

                if (_tableArticleQuantity != null)
                {
                    _tableArticleQuantity.PropertyChanged += OnPaymentPropertyChanged;
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
                //NotifyPropertyChanged("TableArticleQuantities");
                RaisePropertyChanged();
            }
        }

        private async Task CheckIfQuantityIsAvailableUI()
        {
            bool available = await IfQuantityIsAvailable(TableArticleQuantity.Article);

            if (!available)
            {
                TableArticleQuantity.Quantity = 1;
                TableArticleQuantities.FirstOrDefault(x => x.ID == TableArticleQuantity.ID).Quantity = 1;
            }

            await _databaseService.EditTableArticleQuantity(TableArticleQuantity);
        }

        private async void OnPaymentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableArticleQuantity.Quantity))
            {
                await CheckIfQuantityIsAvailableUI();
            }
        }

        //protected async Task NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    if (propertyName.Equals("Quantity"))
        //    {
        //        await CheckIfQuantityIsAvailableUI();
        //    }

        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

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

        /// <summary>
        /// While loading usercontrol it's getting table by id from the database
        /// </summary>
        private async Task GetTable(int id)
        {
            //Gets table from database
            Table = await _databaseService.GetTableByID(id);

            //Check if table is null
            //If table is null, create new providing id and availability, add to database
            if (Table is null)
            {
                Table table = new Table { ID = id, Available = true, TableArticleQuantities = new List<TableArticleQuantity>() };
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
                TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
                {
                    ArticleID = article.ID,
                    TableID = Table.ID,
                    Quantity = 1
                };

                Table.TableArticleQuantities.Add(tableArticleQuantity);
                TableArticleQuantities.Add(tableArticleQuantity);
            }

            await EditTable(Table);
            Barcode = string.Empty;
            RaisePropertyChanged(nameof(Table));
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

            if (articleDetails != null)
            {
                foreach (ArticleDetails articleDetail in articleDetails)
                {
                    quantity += articleDetail.Quantity;
                }
            }

            return quantity;
        }

        private async void DeleteArticleFromTable(TableArticleQuantity tableArticleQuantity)
        {
            Table.TableArticleQuantities.Remove(tableArticleQuantity);
            TableArticleQuantities.Remove(tableArticleQuantity);
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
