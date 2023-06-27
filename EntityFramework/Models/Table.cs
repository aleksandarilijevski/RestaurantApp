using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Models
{
    public class Table
    {
        public int ID { get; set; }

        public int Places { get; set; }

        public bool Available { get; set; }

        public int BillID { get; set; }

        public Bill Bill { get;set; }
    }
}
