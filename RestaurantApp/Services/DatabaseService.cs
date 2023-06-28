using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        private EFContext _efContext { get; set; }

        public DatabaseService(EFContext efContext)
        {
            _efContext = efContext;
        }

        public List<Artical> GetAllArticals()
        {
            List<Artical> articals = _efContext.Articals.Select(x => x).ToList();
            return articals;
        }

        public Artical GetArticalByID(int id)
        {
            Artical artical = _efContext.Articals.FirstOrDefault(x => x.ID == id);
            return artical;
        }

        public int AddArtical(Artical artical)
        {
            _efContext.Articals.Add(artical);
            _efContext.SaveChanges();
            return artical.ID;
        }

        public void UpdateArtical(Artical artical)
        {
            _efContext.Entry(artical).State = EntityState.Modified;
            _efContext.SaveChanges();
        }

        public void DeleteArtical(Artical artical)
        {
            _efContext.Articals.Remove(artical);
            _efContext.SaveChanges();
        }
    }
}
