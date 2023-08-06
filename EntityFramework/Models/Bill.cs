using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Bill : BaseEntity
    {
        public int TableID { get; set; }

        public Table Table { get; set; }  

        [Column(TypeName = "decimal(18,2 )")]
        public decimal TotalPrice { get; set; }
    }
}
