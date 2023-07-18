using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class BaseEntity
    {
        [Key]
        public int ID { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }
    }
}
