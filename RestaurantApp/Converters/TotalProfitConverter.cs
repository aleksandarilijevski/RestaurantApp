using EntityFramework.Models;
using Example;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TotalProfitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int billId = (int)values[0];

            List<TableArticleQuantity> tableArticleQuantities = new List<TableArticleQuantity>();


            if (values[1] != DependencyProperty.UnsetValue)
            {
                List<TableArticleQuantity> tables = (List<TableArticleQuantity>)values[1];
                tableArticleQuantities.AddRange(tables);
            }

            if (values[2] != DependencyProperty.UnsetValue)
            {
                List<TableArticleQuantity> onlineOrders = (List<TableArticleQuantity>)values[2];
                tableArticleQuantities.AddRange(onlineOrders);
            }

            decimal totalPrice = (decimal)values[3];

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
