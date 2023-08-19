using EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TotalPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<TableArticleQuantity> soldTableArticleQuantities = (ObservableCollection<TableArticleQuantity>)values[0];
            List<decimal> totalPrices = new List<decimal>();

            foreach (TableArticleQuantity tableArticleQuantity in soldTableArticleQuantities)
            {
                totalPrices.Add(tableArticleQuantity.Quantity * tableArticleQuantity.Article.Price);
            }

            return totalPrices;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
