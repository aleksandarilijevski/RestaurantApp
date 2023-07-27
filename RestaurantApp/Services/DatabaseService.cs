using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            Table table = await _efContext.Tables.Include(x => x.Articles).ThenInclude(x => x.ArticleDetails).FirstOrDefaultAsync(x => x.ID == id);
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

        public async Task<List<Table>> GetAllTables()
        {
            List<Table> tables = await _efContext.Tables.Select(x => x).Include(x => x.Articles).ToListAsync();
            return tables;
        }

        public async Task<ArticleDetails> GetArticleDetailsByArticleID(int id)
        {
            ArticleDetails articleDetails = await _efContext.ArticleDetails.FirstOrDefaultAsync(x => x.Article.ID == id);
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
    }
}
