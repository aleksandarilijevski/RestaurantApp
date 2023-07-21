using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class DispatchNote : BaseEntity
    {
        public int DispatchNoteNumber { get; set; }

        public List<Article> Articles { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
