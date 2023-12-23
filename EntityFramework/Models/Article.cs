using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Article : BaseEntity, ICloneable
    {
        public long Barcode { get; set; }

        public string Name { get; set; }

        public List<ArticleDetails> ArticleDetails { get; set; }

        public List<Table> Tables { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        public object Clone()
        {
            return new Article
            {
                ID = this.ID,
                Barcode = this.Barcode,
                Name = this.Name,
                ArticleDetails = this.ArticleDetails,
                Tables = this.Tables,
                Price = this.Price,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
