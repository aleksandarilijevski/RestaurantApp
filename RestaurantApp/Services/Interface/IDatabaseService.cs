using EntityFramework.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantApp.Services.Interface
{
    public interface IDatabaseService
    {
        public Task<List<Artical>> GetAllArticals();

        public Task<Artical> GetArticalByID(int id);

        public Task<int> AddArtical(Artical artical);

        public Task UpdateArtical(Artical artical);

        public Task DeleteArtical(Artical artical);
    }
}
