using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class ArticleDetails : BaseEntity
    {
        public Article Article { get; set; }

        public int OriginalQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal EntryPrice { get; set; }

        public List<TableArticleQuantity> TableArticleQuantities { get; set; }  
    }
}
