using EntityFramework.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RestaurantApp.Services.Interface
{
    public interface IDatabaseService
    {
        public Task<ObservableCollection<Article>> GetAllArticles();

        public Task<Article> GetArticleByID(int id);

        public Task<int> AddArticle(Article Article);

        public Task EditArticle(Article Article);

        public Task DeleteArticle(Article Article);

        public Task<ObservableCollection<Waiter>> GetAllWaiters();

        public Task EditWaiter(Waiter waiter);

        public Task DeleteWaiter(Waiter waiter);
    }
}
