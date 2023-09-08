using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using RestaurantApp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        public async Task<ObservableCollection<Article>> GetAllArticles()
        {
            using EFContext efContext = new EFContext();
            List<Article> articles = await efContext.Articles.Include(x => x.ArticleDetails).Where(x => x.IsDeleted == false).ToListAsync();
            return new ObservableCollection<Article>(articles);
        }

        public async Task<Article> GetArticleByID(int id)
        {

            using EFContext efContext = new EFContext();
            Article article = await efContext.Articles.FirstOrDefaultAsync(x => x.ID == id);
            return article;
        }

        public async Task<Article> GetArticleByIDContext(int id,EFContext efContext)
        {
            Article article = await efContext.Articles.FirstOrDefaultAsync(x => x.ID == id);
            return article;
        }

        public async Task<OnlineOrder> GetLastOnlineOrder()
        {
            using EFContext efContext = new EFContext();

            OnlineOrder onlineOrder = null;

            if (efContext.OnlineOrders.Any())
            {
                int id = efContext.OnlineOrders.Max(x => x.ID);
                onlineOrder = await efContext.OnlineOrders.Include(x => x.TableArticleQuantities).ThenInclude(x => x.Article).FirstOrDefaultAsync(x => x.ID == id);
            }

            return onlineOrder;
        }

        public async Task<Article> GetArticleByBarcode(long barcode)
        {

            using EFContext efContext = new EFContext();

            //.Include(x => x.ArticleDetails) Removed due tests and tracking.
            Article article = await efContext.Articles.FirstOrDefaultAsync(x => x.Barcode == barcode && x.IsDeleted == false);
            return article;
        }

        public async Task<int> AddOnlineOrderContext(OnlineOrder onlineOrder, EFContext efContext)
        {
            efContext.OnlineOrders.Add(onlineOrder);
            await efContext.SaveChangesAsync();
            return onlineOrder.ID;
        }

        public async Task<int> AddOnlineOrder(OnlineOrder onlineOrder)
        {
            using EFContext efContext = new EFContext();
            efContext.OnlineOrders.Add(onlineOrder);
            await efContext.SaveChangesAsync();
            return onlineOrder.ID;
        }

        public async Task<Article> GetArticleByBarcodeContext(long barcode,EFContext efContext)
        {
            Article article = await efContext.Articles.FirstOrDefaultAsync(x => x.Barcode == barcode && x.IsDeleted == false);
            return article;
        }

        public async Task<int> AddArticle(Article article)
        {
            using EFContext efContext = new EFContext();
            efContext.Articles.Add(article);
            await efContext.SaveChangesAsync();
            return article.ID;
        }

        public async Task EditArticle(Article article)
        {
            using EFContext efContext = new EFContext();
            efContext.Entry(article).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task DeleteArticle(Article article)
        {
            using EFContext efContext = new EFContext();

            efContext.Articles.Remove(article);
            await efContext.SaveChangesAsync();
        }

        public async Task<ObservableCollection<Waiter>> GetAllWaiters()
        {
            using EFContext efContext = new EFContext();
            List<Waiter> waiters = await efContext.Waiters.ToListAsync();
            return new ObservableCollection<Waiter>(waiters);
        }

        public async Task<int> AddWaiter(Waiter waiter)
        {
            using EFContext efContext = new EFContext();
            efContext.Waiters.Add(waiter);
            await efContext.SaveChangesAsync();
            return waiter.ID;
        }

        public async Task EditWaiter(Waiter waiter)
        {
            using EFContext efContext = new EFContext();
            efContext.Entry(waiter).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task DeleteWaiter(Waiter waiter)
        {
            using EFContext efContext = new EFContext();
            efContext.Waiters.Remove(waiter);
            await efContext.SaveChangesAsync();
        }

        public async Task<Table> GetTableByID(int id)
        {
            using EFContext efContext = new EFContext();
            //NotIncluding Article due tests and tracking
            //NotInluciding ArticleDetails due tests and tracking
            Table table = await efContext.Tables.Include(x => x.TableArticleQuantities).ThenInclude(x => x.ArticleDetails).ThenInclude(x => x.Article).FirstOrDefaultAsync(x => x.ID == id);
            return table;
        }

        public async Task<int> AddTable(Table table)
        {
            using EFContext efContext = new EFContext();
            efContext.Tables.Add(table);
            await efContext.SaveChangesAsync();
            return table.ID;
        }

        public async Task EditTable(Table table)
        {
            using EFContext efContext = new EFContext();
            efContext.Entry(table).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task EditTableContext(Table table,EFContext efContext)
        {
            //Added due tests and tracking
            efContext.Attach(table);
            efContext.Entry(table).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }


        //Table table
        public async Task ModifyTableArticles(List<TableArticleQuantity> tableArticleQuantities, List<SoldTableArticleQuantity> soldTableArticleQuantities)
        {
            using EFContext efContext = new EFContext();

            List<TableArticleQuantity> quantitiesToRemove = tableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)).ToList();

            foreach (TableArticleQuantity tableArticleQuantity in quantitiesToRemove.ToList())
            {
                tableArticleQuantities.Remove(tableArticleQuantity);
                efContext.TableArticleQuantities.Remove(tableArticleQuantity);
            }

           // efContext.Entry(table).State = EntityState.Modified;

            efContext.TableArticleQuantities.AddRange(soldTableArticleQuantities);
            await efContext.SaveChangesAsync();
        }

        //Table table
        public async Task ModifyTableArticlesContext(List<TableArticleQuantity> tableArticleQuantities, List<SoldTableArticleQuantity> soldTableArticleQuantities,EFContext efContext)
        {
            List<TableArticleQuantity> quantitiesToRemove = tableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)).ToList();

            foreach (var taq in tableArticleQuantities)
            {
                efContext.Entry(taq).State = EntityState.Detached;
            }

            foreach (TableArticleQuantity tableArticleQuantity in quantitiesToRemove.ToList())
            {
                tableArticleQuantities.Remove(tableArticleQuantity);
                efContext.TableArticleQuantities.Remove(tableArticleQuantity);
            }

            // efContext.Entry(table).State = EntityState.Modified;

            efContext.TableArticleQuantities.AddRange(soldTableArticleQuantities);
            await efContext.SaveChangesAsync();
        }


        public async Task ModifyTableArticlesOnlineOrder(OnlineOrder onlineOrder, List<SoldTableArticleQuantity> soldTableArticleQuantities)
        {
            using EFContext efContext = new EFContext();

            efContext.SoldTableArticleQuantities.AddRange(soldTableArticleQuantities);
            await efContext.SaveChangesAsync();
        }

        public async Task EditOnlineOrderContext(OnlineOrder onlineOrder,EFContext efContext)
        {
            efContext.Entry(onlineOrder).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task<List<Table>> GetAllTables()
        {

            using EFContext efContext = new EFContext();
            List<Table> tables = await efContext.Tables.ToListAsync();
            return tables;
        }

        public async Task<ArticleDetails> GetArticleDetailByArticleID(int id)
        {
            using EFContext efContext = new EFContext();
            ArticleDetails articleDetails = await efContext.ArticleDetails.Include(x => x.Article).FirstOrDefaultAsync(x => x.Article.ID == id);
            return articleDetails;
        }

        public async Task<List<ArticleDetails>> GetArticleDetailsByArticleID(int articleId)
        {
            using EFContext efContext = new EFContext();
            //.Include(x => x.Article) not including article due tests and tracking.
            List<ArticleDetails> articleDetails = await efContext.ArticleDetails.Include(x => x.Article).Where(x => x.Article.ID == articleId).ToListAsync();
            return articleDetails;
        }

        public async Task<List<ArticleDetails>> GetArticleDetailsByArticleIDContext(int articleId,EFContext efContext)
        {
            //.Include(x => x.Article) not including article due tests and tracking.
            List<ArticleDetails> articleDetails = await efContext.ArticleDetails.Where(x => x.ArticleID == articleId).ToListAsync();
            return articleDetails;
        }

        public async Task<int> AddArticleDetails(ArticleDetails articleDetails)
        {

            using EFContext efContext = new EFContext();
            efContext.ArticleDetails.Add(articleDetails);
            await efContext.SaveChangesAsync();
            return articleDetails.ID;
        }

        public async Task EditArticleDetails(ArticleDetails articleDetails)
        {
            using EFContext efContext = new EFContext();
            efContext.Entry(articleDetails).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task EditArticleDetailsContext(ArticleDetails articleDetails,EFContext efContext)
        {
            efContext.Entry(articleDetails).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task AddDataEntry(DataEntry dataEntry)
        {
            using EFContext efContext = new EFContext();
            efContext.DataEntries.Add(dataEntry);
            await efContext.SaveChangesAsync();
        }

        public async Task AddDataEntryContext(DataEntry dataEntry,EFContext efContext)
        {
            efContext.DataEntries.Add(dataEntry);
            await efContext.SaveChangesAsync();
        }

        public async Task<DataEntry> GetDataEntryByNumber(int dataEntryNumber)
        {

            using EFContext efContext = new EFContext();
            DataEntry dataEntry = await efContext.DataEntries.FirstOrDefaultAsync(x => x.DataEntryNumber == dataEntryNumber);
            return dataEntry;
        }

        public async Task<ArticleDetails> GetArticleDetailsByID(int id)
        {

            using EFContext efContext = new EFContext();
            ArticleDetails articleDetails = await efContext.ArticleDetails.FirstOrDefaultAsync(x => x.ID == id);
            return articleDetails;
        }

        public async Task DeleteArticleDetails(ArticleDetails articleDetails)
        {

            using EFContext efContext = new EFContext();
            efContext.ArticleDetails.Remove(articleDetails);
            await efContext.SaveChangesAsync();
        }

        public async Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();
            efContext.TableArticleQuantities.Add(tableArticleQuantity);
            await efContext.SaveChangesAsync();
        }

        public async Task AddTableArticleQuantityContext(TableArticleQuantity tableArticleQuantity,EFContext efContext)
        {
            efContext.TableArticleQuantities.Add(tableArticleQuantity);
            await efContext.SaveChangesAsync();
        }


        public async Task<List<TableArticleQuantity>> GetTableArticleQuantities(int articleID, int tableID)
        {

            using EFContext efContext = new EFContext();
            List<TableArticleQuantity> tableArticleQuantity = await efContext.TableArticleQuantities.Select(x => x).Where(x => x.TableID == tableID && x.ArticleID == articleID).ToListAsync();
            return tableArticleQuantity;
        }

        public async Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();
            efContext.Entry(tableArticleQuantity).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            efContext.TableArticleQuantities.Remove(tableArticleQuantity);
            await efContext.SaveChangesAsync();
        }

        public async Task DeleteTableArticleQuantityContext(TableArticleQuantity tableArticleQuantity,EFContext efContext)
        {
            efContext.TableArticleQuantities.Remove(tableArticleQuantity);
            await efContext.SaveChangesAsync();
        }


        public async Task<int> GetTableArticleTotalQuantity(int articleID)
        {
            int totalQuantity = 0;

            using EFContext efContext = new EFContext();

            List<TableArticleQuantity> tableArticleQuantities = await efContext.TableArticleQuantities.Select(x => x)
                .Where(x => x.ArticleID == articleID && !(x is SoldTableArticleQuantity))
                .ToListAsync();

            totalQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            return totalQuantity;
        }

        public async Task<List<TableArticleQuantity>> GetTableArticleQuantitiesExceptProvidedID(Table table)
        {
            using EFContext efContext = new EFContext();

            List<TableArticleQuantity> tableArticleQuantities = await efContext.TableArticleQuantities.Select(x => x).Where(x => x.Table.ID != table.ID).ToListAsync();
            return tableArticleQuantities;
        }

        public async Task<int> CreateBill(Bill bill)
        {

            using EFContext efContext = new EFContext();
            efContext.Bills.Add(bill);
            await efContext.SaveChangesAsync();
            return bill.ID;
        }

        public async Task<int> CreateBillContext(Bill bill,EFContext efContext)
        {
            efContext.Bills.Add(bill);
            await efContext.SaveChangesAsync();
            return bill.ID;
        }

        public async Task<int> AddSoldTableArticleQuantity(SoldTableArticleQuantity soldTableArticleQuantity)
        {
            using EFContext efContext = new EFContext();

            efContext.SoldTableArticleQuantities.Add(soldTableArticleQuantity);
            await efContext.SaveChangesAsync();
            return soldTableArticleQuantity.ID;
        }

        public async Task<int> CreateConfiguration(Configuration configuration)
        {
            using EFContext efContext = new EFContext();


            efContext.Configurations.Add(configuration);
            await efContext.SaveChangesAsync();
            return configuration.ID;
        }

        public async Task<Configuration> GetConfiguration()
        {
            using EFContext efContext = new EFContext();

            Configuration configuration = await efContext.Configurations.FirstOrDefaultAsync();
            return configuration;
        }

        public async Task EditConfiguration(Configuration configuration)
        {
            using EFContext efContext = new EFContext();

            efContext.Entry(configuration).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task<List<Bill>> GetAllBills()
        {
            using EFContext efContext = new EFContext();

            List<Bill> bills = await efContext.Bills.Select(x => x)
                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.Article)
                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.Bill)
                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.ArticleDetails)
                .Include(x => x.OnlineOrder)
                .ThenInclude(x => x.TableArticleQuantities)
                .ToListAsync();
            return bills;
        }
    }
}
