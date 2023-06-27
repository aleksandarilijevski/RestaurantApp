using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RestaurantApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        private EFContext _efContext { get; set; }

        public DatabaseService(EFContext efContext)
        {
            _efContext = efContext;
        }

        public async Task<List<Artical>> GetAllArticals()
        {
            List<Artical> articals = await _efContext.Articals.Select(x => x).ToListAsync();
            return articals;
        }

        public async Task<Artical> GetArticalByID(int id)
        {
            Artical artical = await _efContext.Articals.FirstOrDefaultAsync(x => x.ID == id);
            return artical;
        }

        public async Task<int> AddArtical(Artical artical)
        {
            _efContext.Articals.Add(artical);
            await _efContext.SaveChangesAsync();
            return artical.ID;
        }

        public async Task UpdateArtical(Artical artical)
        {
            _efContext.Entry(artical).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        public async Task DeleteArtical(Artical artical)
        {
            _efContext.Articals.Remove(artical);
            await _efContext.SaveChangesAsync();
        }
    }
}
