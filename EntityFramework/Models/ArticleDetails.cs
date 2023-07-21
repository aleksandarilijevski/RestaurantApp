namespace EntityFramework.Models
{
    public class ArticleDetails : BaseEntity
    {
        public Article Article { get; set; }

        public int Quantity { get; set; }

        public decimal EntryPrice { get; set; }
    }
}
