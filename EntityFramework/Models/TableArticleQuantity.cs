using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    [Table("TableArticleQuantity")]
    public class TableArticleQuantity : INotifyPropertyChanged
    {
        private int _quantity;

        public int ID { get; set; }

        public int TableID { get; set; }

        public Table Table { get; set; }

        public int ArticleID { get; set; }

        public Article Article { get; set; }

        public int? BillID { get; set; }

        public Bill? Bill { get; set; }

        //public List<ArticleDetails> ArticleDetails { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
