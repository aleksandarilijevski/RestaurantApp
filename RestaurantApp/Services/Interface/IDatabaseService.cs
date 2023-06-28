using EntityFramework.Models;
using System.Collections.Generic;

namespace RestaurantApp.Services.Interface
{
    public interface IDatabaseService
    {
        public List<Artical> GetAllArticals();

        public Artical GetArticalByID(int id);

        public int AddArtical(Artical artical);

        public void UpdateArtical(Artical artical);

        public void DeleteArtical(Artical artical);
    }
}
