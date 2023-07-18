using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Bill : BaseEntity
    {
        [Key]
        public int ID { get; set; }

        public List<Article> BoughtArticles {  get; set; }

        public decimal TotalPrice { get; set; }
    }
}
