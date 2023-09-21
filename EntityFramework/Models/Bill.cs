using RestaurantApp.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Bill : BaseEntity
    {
        public int? TableID { get; set; }

        public Table? Table { get; set; }

        public int? OnlineOrderID { get; set; }

        public OnlineOrder? OnlineOrder { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal Cash { get; set; }

        [Column(TypeName = "decimal(18,2 )")]
        public decimal Change { get; set; }

        public PaymentType PaymentType { get; set; }

        public string RegistrationNumber { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }
    }
}
