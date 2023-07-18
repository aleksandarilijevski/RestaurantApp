using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class DispatchNote : BaseEntity
    {
        [Key]
        public int ID { get; set; }

        public int DispatchNoteNumber { get; set; }

        public List<Article> Articles { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
