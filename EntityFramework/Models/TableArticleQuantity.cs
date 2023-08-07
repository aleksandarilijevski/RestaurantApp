﻿namespace EntityFramework.Models
{
    public class TableArticleQuantity
    {
        public int ID { get; set; }

        public int TableID { get; set; }

        public Table Table { get; set; }

        public int ArticleID { get; set; }

        public Article Article { get; set; }

        public int Quantity { get; set; }
    }
}