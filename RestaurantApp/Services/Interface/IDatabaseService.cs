using EntityFramework.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RestaurantApp.Services.Interface
{
    public interface IDatabaseService
    {
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

        public Task EditTable(Table table);

        public Task<List<Table>> GetAllTables();

        public Task<ArticleDetails> GetArticleDetailsByArticleID(int id);

        public Task<int> AddArticleDetails(ArticleDetails articleDetails);

        public Task EditArticleDetails(ArticleDetails articleDetails);

        public Task AddDataEntry(DataEntry dataEntry);

        public Task<ArticleDetails> GetArticleDetailsByID(int id);

        public Task DeleteArticleDetails(ArticleDetails articleDetails);

        public Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity);

        public Task<List<TableArticleQuantity>> GetTableArticleQuantities(int articleID, int tableID);

        public Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity);

        public Task<int> GetTableArticleTotalQuantity(int articleID);

        public Task<int> CreateBill(Bill bill);

        public Task<int> AddSoldTableArticleQuantity(SoldTableArticleQuantity soldTableArticleQuantity);
    }
}
