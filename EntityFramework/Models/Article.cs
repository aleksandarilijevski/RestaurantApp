using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Article: BaseEntity
    {
        public long Barcode { get;set; }

        public string Name { get; set; }

        public List<ArticleDetails> ArticleDetails { get; set; }

        public List<Table> Tables { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal Price { get; set; }
    }
}
