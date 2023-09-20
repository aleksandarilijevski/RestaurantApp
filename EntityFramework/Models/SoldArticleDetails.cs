namespace EntityFramework.Models
{
    public class SoldArticleDetails : BaseEntity
    {
        public decimal EntryPrice { get; set; }

        public int SoldQuantity { get; set; }

        public int BillID { get; set; }

        public Bill Bill { get; set; }
    }
}
