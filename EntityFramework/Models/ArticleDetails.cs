namespace EntityFramework.Models
{
    public class ArticleDetails : BaseEntity
    {
        public int ID { get; set; }

        public Article Article { get; set; }

        public int Quantity { get; set; }

        public decimal EntryPrice { get; set; }
    }
}
