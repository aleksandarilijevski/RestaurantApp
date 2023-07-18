﻿using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Bill : CreatedOn
    {
        [Key]
        public int ID { get; set; }

        public List<Article> BoughtArticles {  get; set; }

        public decimal TotalPrice { get; set; }
    }
}
