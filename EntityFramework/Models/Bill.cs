namespace EntityFramework.Models
{
    public class Bill
    {
        public int ID { get; set; }

        public List<Artical> BoughtArticals {  get; set; }

        public decimal TotalPrice { get; set; }
    }
}
