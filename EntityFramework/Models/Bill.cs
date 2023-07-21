using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Bill : BaseEntity
    {
        public List<Article> BoughtArticles {  get; set; }

        public decimal TotalPrice { get; set; }
    }
}
