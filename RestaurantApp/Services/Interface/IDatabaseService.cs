using EntityFramework.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RestaurantApp.Services.Interface
{
    public interface IDatabaseService
    {
        public Task<TableArticleQuantity> GetTableArticleQuantityByID(int id, EFContext efContext);

        public Task DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity, EFContext efContext);

        public Task<int> AddOnlineOrder(OnlineOrder onlineOrder, EFContext efContext);

        public Task<List<TableArticleQuantity>> GetTableArticleQuantityByArticleID(int articleId, EFContext efContext);

        public Task<OnlineOrder> GetLastOnlineOrder(EFContext efContext);

        public Task EditArticleDetails(ArticleDetails articleDetails, EFContext efContext);

        public Task<int> CreateBill(Bill bill, EFContext efContext);

        public Task AddDataEntry(DataEntry dataEntry, EFContext efContext);

        public Task<Article> GetArticleByID(int id, EFContext efContext);

        public Task<ObservableCollection<Article>> GetAllArticles();

        public Task<int> AddArticle(Article article, EFContext efContext);

        public Task EditArticle(Article article, EFContext efContext);

        public Task<ObservableCollection<User>> GetAllUsers();

        public Task<int> AddUser(User user, EFContext efContext);

        public Task EditUser(User user, EFContext efContext);

        public Task DeleteUser(User user, EFContext efContext);

        public Task<Table> GetTableByID(int id, EFContext efContext);

        public Task<int> AddTable(Table table, EFContext efContext);

        public Task EditTable(Table table, EFContext efContext);

        public Task<Article> GetArticleByBarcodeContext(long barcode, EFContext efContext);

        public Task EditOnlineOrderContext(OnlineOrder onlineOrder, EFContext efContext);

        public Task<List<Table>> GetAllTables();

        public Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity, EFContext efContext);

        public Task<List<ArticleDetails>> GetArticleDetailsByArticleID(int articleId, EFContext efContext);

        public Task<int> AddArticleDetails(ArticleDetails articleDetails, EFContext efContext);

        public Task<DataEntry> GetDataEntryByNumber(int dataEntryNumber);

        public Task DeleteArticleDetails(ArticleDetails articleDetails, EFContext efContext);

        public Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity, EFContext efContext);

        public Task<int> CreateConfiguration(Configuration configuration, EFContext efContext);

        public Task<Configuration> GetConfiguration();

        public Task EditConfiguration(Configuration configuration, EFContext eFContext);

        public Task<List<Bill>> GetAllBills();

        public Task<List<DataEntry>> GetAllDataEntries();

        public Task AddSoldArticleDetails(SoldArticleDetails soldArticleDetails, EFContext efContext);

        public Task<List<SoldArticleDetails>> GetAllSoldArticleDetails();

        public Task<List<SoldArticleDetails>> GetSoldArticleDetailsByBillID(int billID, EFContext efContext);

        public Task<bool> CheckIfAnyUserExists();

        public Task<User> GetUserByBarcode(long barcode, EFContext efContext);

        public Task<User> GetUserByID(int id, EFContext efContext);
    }
}
