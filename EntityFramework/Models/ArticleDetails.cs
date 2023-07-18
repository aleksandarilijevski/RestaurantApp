using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Models
{
    public class ArticleDetails
    {
        public int ID { get; set; }

        public Article Article { get; set; }

        public int Quantity { get; set; }

        public decimal EntryPrice { get; set; }
    }
}
