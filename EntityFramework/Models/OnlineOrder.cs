namespace EntityFramework.Models
{
    public class OnlineOrder : BaseEntity
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }    

        public string Address { get; set; }

        public long PhoneNumber { get; set; }

        public int Floor { get; set; }

        public int ApartmentNumber { get;set; }

        public string Comment { get; set; }

        public List<TableArticleQuantity> TableArticleQuantities { get;set; } = new List<TableArticleQuantity>();
    }
}
