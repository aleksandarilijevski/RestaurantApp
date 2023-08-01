using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Bill : BaseEntity
    {
        public List<Article> BoughtArticles { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal TotalPrice { get; set; }
    }
}
