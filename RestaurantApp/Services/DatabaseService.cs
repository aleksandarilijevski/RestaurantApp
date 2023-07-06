using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            List<Article> articles = await _efContext.Articles.ToListAsync();
            return new ObservableCollection<Article>(articles);
        }

        public async Task<Article> GetArticleByID(int id)
        {
            Article Article = await _efContext.Articles.FirstOrDefaultAsync(x => x.ID == id);
            return Article;
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
    }
}
