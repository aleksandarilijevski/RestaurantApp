using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Table
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public bool InUse { get; set; }

        public List<TableArticleQuantity> TableArticleQuantities { get; set; }

        public int? UserID { get;set; }

        public User? User { get; set; }
    }
}
