using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RestaurantApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        private EFContext _efContext { get; set; }

        public DatabaseService(EFContext efContext)
        {
            _efContext = efContext;
        }

        public async Task<ObservableCollection<Article>> GetAllArticles()
        {
            List<Article> articles = await _efContext.Articles.Include(x => x.ArticleDetails).ToListAsync();
            return new ObservableCollection<Article>(articles);
        }

        public async Task<Article> GetArticleByID(int id)
        {
            Article article = await _efContext.Articles.FirstOrDefaultAsync(x => x.ID == id);
            return article;
        }

        public async Task<Article> GetArticleByBarcode(long barcode)
        {
            Article article = await _efContext.Articles.Include(x => x.ArticleDetails).FirstOrDefaultAsync(x => x.Barcode == barcode);
            return article;
        }

        public async Task<int> AddArticle(Article Article)
        {
            _efContext.Articles.Add(Article);
            await _efContext.SaveChangesAsync();
            return Article.ID;
        }

        public async Task EditArticle(Article Article)
        {
            _efContext.Entry(Article).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task DeleteArticle(Article Article)
        {
            _efContext.Articles.Remove(Article);
            await _efContext.SaveChangesAsync();
        }

        public async Task<ObservableCollection<Waiter>> GetAllWaiters()
        {
            List<Waiter> waiters = await _efContext.Waiters.ToListAsync();
            return new ObservableCollection<Waiter>(waiters);
        }

        public async Task<int> AddWaiter(Waiter waiter)
        {
            _efContext.Waiters.Add(waiter);
            await _efContext.SaveChangesAsync();
            return waiter.ID;
        }

        public async Task EditWaiter(Waiter waiter)
        {
            _efContext.Entry(waiter).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task DeleteWaiter(Waiter waiter)
        {
            _efContext.Waiters.Remove(waiter);
            await _efContext.SaveChangesAsync();
        }

        public async Task<Table> GetTableByID(int id)
        {
            Table table = await _efContext.Tables.Include(x => x.TableArticleQuantities).ThenInclude(x => x.Article).ThenInclude(x => x.ArticleDetails).FirstOrDefaultAsync(x => x.ID == id);
            return table;
        }

        public async Task<int> AddTable(Table table)
        {
            _efContext.Tables.Add(table);
            await _efContext.SaveChangesAsync();
            return table.ID;
        }

        public async Task EditTable(Table table)
        {
            _efContext.Entry(table).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task ModifyTableArticles(int tableID, List<SoldTableArticleQuantity> soldTableArticleQuantities, List<TableArticleQuantity> tableArticleQuantities)
        {
            Table table = _efContext.Tables.Include(x => x.TableArticleQuantities).FirstOrDefault(x => x.ID == tableID);

            //List<TableArticleQuantity> quantitiesToRemove = table.TableArticleQuantities.OfType<TableArticleQuantity>().ToList();
            List<TableArticleQuantity> quantitiesToRemove = table.TableArticleQuantities.Where(x => !(x is SoldTableArticleQuantity)).ToList();

            foreach (TableArticleQuantity tableArticleQuantity in quantitiesToRemove.ToList())
            {
                table.TableArticleQuantities.Remove(tableArticleQuantity);
            }

            table.TableArticleQuantities.AddRange(soldTableArticleQuantities);

            await _efContext.SaveChangesAsync();
        }

        public async Task<List<Table>> GetAllTables()
        {
            List<Table> tables = await _efContext.Tables.Select(x => x).ToListAsync();
            return tables;
        }

        public async Task<ArticleDetails> GetArticleDetailByArticleID(int id)
        {
            ArticleDetails articleDetails = await _efContext.ArticleDetails.FirstOrDefaultAsync(x => x.Article.ID == id);
            return articleDetails;
        }

        public async Task<List<ArticleDetails>> GetArticleDetailsByArticleID(int articleId)
        {
            List<ArticleDetails> articleDetails = await _efContext.ArticleDetails.Where(x => x.Article.ID == articleId).ToListAsync();
            return articleDetails;
        }

        public async Task<int> AddArticleDetails(ArticleDetails articleDetails)
        {
            _efContext.ArticleDetails.Add(articleDetails);
            await _efContext.SaveChangesAsync();
            return articleDetails.ID;
        }

        public async Task EditArticleDetails(ArticleDetails articleDetails)
        {
            _efContext.Entry(articleDetails).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task AddDataEntry(DataEntry dataEntry)
        {
            _efContext.DataEntries.Add(dataEntry);
            await _efContext.SaveChangesAsync();
        }

        public async Task<DataEntry> GetDataEntryByNumber(int dataEntryNumber)
        {
            DataEntry dataEntry = await _efContext.DataEntries.FirstOrDefaultAsync(x => x.DataEntryNumber == dataEntryNumber);
            return dataEntry;
        }

        public async Task<ArticleDetails> GetArticleDetailsByID(int id)
        {
            ArticleDetails articleDetails = await _efContext.ArticleDetails.FirstOrDefaultAsync(x => x.ID == id);
            return articleDetails;
        }

        public async Task DeleteArticleDetails(ArticleDetails articleDetails)
        {
            _efContext.ArticleDetails.Remove(articleDetails);
            await _efContext.SaveChangesAsync();
        }

        public async Task AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            _efContext.ChangeTracker.AutoDetectChangesEnabled = false;
            _efContext.TableArticleQuantities.Add(tableArticleQuantity);
            await _efContext.SaveChangesAsync();
        }

        public async Task<List<TableArticleQuantity>> GetTableArticleQuantities(int articleID, int tableID)
        {
            List<TableArticleQuantity> tableArticleQuantity = await _efContext.TableArticleQuantities.Select(x => x).Where(x => x.TableID == tableID && x.ArticleID == articleID).ToListAsync();
            return tableArticleQuantity;
        }

        public async Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            _efContext.Entry(tableArticleQuantity).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity)
        {
            _efContext.TableArticleQuantities.Remove(tableArticleQuantity);
            await _efContext.SaveChangesAsync();
        }

        public async Task<int> GetTableArticleTotalQuantity(int articleID)
        {
            int totalQuantity = 0;

            using EFContext efContext = new EFContext();

            //List<TableArticleQuantity> tableArticleQuantities = await efContext.TableArticleQuantities.Select(x => x)
            //    .Where(x => x.ArticleID == articleID).OfType<TableArticleQuantity>()
            //    .ToListAsync(); 


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
            _efContext.Bills.Add(bill);
            await _efContext.SaveChangesAsync();
            return bill.ID;
        }

        public async Task<int> AddSoldTableArticleQuantity(SoldTableArticleQuantity soldTableArticleQuantity)
        {
            _efContext.SoldTableArticleQuantities.Add(soldTableArticleQuantity);
            await _efContext.SaveChangesAsync();
            return soldTableArticleQuantity.ID;
        }

        public async Task<int> CreateConfiguration(Configuration configuration)
        {
            _efContext.Configurations.Add(configuration);
            await _efContext.SaveChangesAsync();
            return configuration.ID;
        }

        public async Task<Configuration> GetConfiguration()
        {
            Configuration configuration = await _efContext.Configurations.FirstOrDefaultAsync();
            return configuration;
        }

        public async Task EditConfiguration(Configuration configuration)
        {
            _efContext.Entry(configuration).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task<List<Bill>> GetAllBills()
        {
            List<Bill> bills = await _efContext.Bills.Select(x => x)
                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.Article)
                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.Bill)
                .ToListAsync();
            return bills;
        }
    }
}
