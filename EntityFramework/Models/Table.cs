using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Table
    {
        [Key]
        public int ID { get; set; }

        public int Places { get; set; }

        public bool Available { get; set; }

        public List<Article> Articles { get; set; }

        public int? BillID { get; set; }

        public Bill? Bill { get; set; }
    }
}
