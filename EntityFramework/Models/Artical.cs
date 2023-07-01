namespace EntityFramework.Models
{
    public class Artical
    {
        public int ID { get; set; }

        public long BarCode { get;set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal EntryPrice { get; set; }

        public decimal Price { get; set; }
    }
}
