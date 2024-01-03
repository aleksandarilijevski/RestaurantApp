using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class ArticleDetails : BaseEntity, ICloneable
    {
        public int ArticleID { get; set; }

        public Article Article { get; set; }

        public int OriginalQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int DataEntryQuantity { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal EntryPrice { get; set; }

        public List<TableArticleQuantity> TableArticleQuantities { get; set; }

        public object Clone()
        {
            return new ArticleDetails
            {
                ID = this.ID,
                CreatedDateTime = this.CreatedDateTime,
                ModifiedDateTime = this.ModifiedDateTime,
                ArticleID = this.ArticleID,
                Article = this.Article,
                OriginalQuantity = this.OriginalQuantity,
                ReservedQuantity = this.ReservedQuantity,
                DataEntryQuantity = this.DataEntryQuantity,
                EntryPrice = this.EntryPrice,
                TableArticleQuantities = this.TableArticleQuantities
            };
        }
    }
}
