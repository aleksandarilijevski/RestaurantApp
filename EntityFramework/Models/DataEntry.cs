using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class DataEntry : BaseEntity
    {
        public int DataEntryNumber { get; set; }

        public List<Article> Articles { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
