using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Models
{
    public class ArticleQuantity
    {
        public int ID { get; set; }

        public int ArticleID { get; set; }

        public int Quantity { get; set; }
    }
}
