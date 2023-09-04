using EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TotalProfitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int billId = (int)values[0];

            List<TableArticleQuantity> tableArticleQuantities = (List<TableArticleQuantity>)values[1];
            decimal totalPrice = (decimal)values[2];

            List<TableArticleQuantity> filtered = tableArticleQuantities.Where(x => x.BillID == billId).ToList();

            decimal profit = 0;

            foreach (TableArticleQuantity tableArticleQuantity in filtered)
            {
                foreach (ArticleDetails articleDetails in tableArticleQuantity.ArticleDetails)
                {
                    int soldQuantity = filtered.Sum(x => x.Quantity);
                    profit += totalPrice - (articleDetails.EntryPrice * soldQuantity);
                }

                return profit.ToString();
            }

            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
