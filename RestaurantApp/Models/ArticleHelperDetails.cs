using EntityFramework.Models;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;

namespace RestaurantApp.Models
{
    public class ArticleHelperDetails
    {
        public TableArticleQuantity TableArticleQuantity { get; set; }

        public List<ArticleDetails> ArticleDetails { get; set; }

        public int Quantity { get; set; }

        public EFContext EFContext { get; set; }

        public IDatabaseService DatabaseService { get; set; }

        public int BillID { get;set; }
    }
}
