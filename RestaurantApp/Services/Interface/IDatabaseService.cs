using EntityFramework.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RestaurantApp.Services.Interface
{
    public interface IDatabaseService
    {
        public  Task EditArticleDetailsContext(ArticleDetails articleDetails, EFContext efContext);

        public Task<int> CreateBillContext(Bill bill, EFContext efContext);

        public Task ModifyTableArticlesContext(List<TableArticleQuantity> tableArticleQuantities, List<SoldTableArticleQuantity> soldTableArticleQuantities, EFContext efContext);
        
        public Task EditTableContext(Table table,EFContext efContext);

        public Task AddDataEntryContext(DataEntry dataEntry, EFContext efContext);

        public Task<Article> GetArticleByIDContext(int id, EFContext efContext);

        public Task<ObservableCollection<Article>> GetAllArticles();

        public Task<Article> GetArticleByID(int id);

        public Task<Article> GetArticleByBarcode(long barcode);

        public Task<int> AddArticle(Article Article);

        public Task EditArticle(Article Article);

        public Task DeleteArticle(Article Article);

        public Task<ObservableCollection<Waiter>> GetAllWaiters();

        public Task<int> AddWaiter(Waiter waiter);

        public Task EditWaiter(Waiter waiter);

        public Task DeleteWaiter(Waiter waiter);

        public Task<Table> GetTableByID(int id);

        public Task<int> AddTable(Table table);

        //Table table
        public Task ModifyTableArticles(List<TableArticleQuantity> tableArticleQuantities, List<SoldTableArticleQuantity> soldTableArticleQuantities);

        public Task ModifyTableArticlesOnlineOrder(OnlineOrder onlineOrder, List<SoldTableArticleQuantity> soldTableArticleQuantities);

        public Task EditTable(Table table);

        public Task<Article> GetArticleByBarcodeContext(long barcode, EFContext efContext);

        public Task EditOnlineOrder(OnlineOrder onlineOrder);

        public Task<List<Table>> GetAllTables();

        public Task<ArticleDetails> GetArticleDetailByArticleID(int id);

        public Task<List<ArticleDetails>> GetArticleDetailsByArticleID(int articleId);

        public Task AddTableArticleQuantityContext(TableArticleQuantity tableArticleQuantity, EFContext efContext);

        public Task<List<ArticleDetails>> GetArticleDetailsByArticleIDContext(int articleId, EFContext efContext);

        public Task<List<TableArticleQuantity>> GetTableArticleQuantitiesExceptProvidedID(Table table);

        public Task<int> AddArticleDetails(ArticleDetails articleDetails);

        public Task EditArticleDetails(ArticleDetails articleDetails);

        public Task DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity);

        public Task AddDataEntry(DataEntry dataEntry);

        public Task<DataEntry> GetDataEntryByNumber(int dataEntryNumber);

        public Task<ArticleDetails> GetArticleDetailsByID(int id);

        public Task DeleteArticleDetails(ArticleDetails articleDetails);

        public Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity);

        public Task<List<TableArticleQuantity>> GetTableArticleQuantities(int articleID, int tableID);

        public Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity);

        public Task<int> GetTableArticleTotalQuantity(int articleID);

        public Task<int> CreateBill(Bill bill);

        public Task<int> AddSoldTableArticleQuantity(SoldTableArticleQuantity soldTableArticleQuantity);

        public Task<int> CreateConfiguration(Configuration configuration);

        public Task<Configuration> GetConfiguration();

        public Task EditConfiguration(Configuration configuration);

        public Task<List<Bill>> GetAllBills();
    }
}
