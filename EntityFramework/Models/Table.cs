namespace EntityFramework.Models
{
    public class Table
    {
        public int ID { get; set; }

        public bool Available { get; set; }

        public List<Article> Articles { get; set; }

        public int? BillID { get; set; }

        public Bill? Bill { get; set; }
    }
}
