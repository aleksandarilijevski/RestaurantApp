using EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TotalPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string totalPrice = string.Empty;

            if (value is ObservableCollection<TableArticleQuantity>)
            {
                ObservableCollection<TableArticleQuantity> tableArticleQuantities = (ObservableCollection<TableArticleQuantity>)value;
                List<TableArticleQuantity> soldtableArticleQunatities = tableArticleQuantities.ToList();

                return soldtableArticleQunatities.Sum(x => x.Quantity * x.Article.Price);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
