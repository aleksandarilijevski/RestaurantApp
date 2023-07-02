namespace EntityFramework.Models
{
    public class Bill
    {
        public int ID { get; set; }

        public List<Article> BoughtArticles {  get; set; }

        public decimal TotalPrice { get; set; }
    }
}
