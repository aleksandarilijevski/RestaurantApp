﻿using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Article: BaseEntity
    {
        public long Barcode { get;set; }

        public string Name { get; set; }

        public List<ArticleDetails> ArticleDetails { get; set; }

        public List<Table> Tables { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
