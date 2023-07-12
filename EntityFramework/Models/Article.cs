﻿using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Article
    {
        [Key]
        public int ID { get; set; }

        public long Barcode { get;set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ArticleQuantity ArticleQuantity { get; set; }

        public List<Table> Tables { get; set; }

        public int Quantity { get; set; }

        public decimal EntryPrice { get; set; }

        public decimal Price { get; set; }
    }
}