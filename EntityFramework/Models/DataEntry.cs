using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class DataEntry : BaseEntity
    {
        public int DataEntryNumber { get; set; }

        public List<ArticleDetails> ArticleDetails { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal TotalAmount { get; set; }
    }
}
