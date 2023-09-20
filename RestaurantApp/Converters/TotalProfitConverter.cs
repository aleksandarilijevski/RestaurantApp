using EntityFramework.Models;
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
            List<SoldArticleDetails> soldArticleDetails = new List<SoldArticleDetails>();
            int billId = (int)values[0];
            decimal totalPrice = (decimal)values[1];
            decimal profit = 0;

            if (values[2] != DependencyProperty.UnsetValue)
            {
                soldArticleDetails = (List<SoldArticleDetails>)values[2];
            }

            List<SoldArticleDetails> filteredSoldArticleDetails = soldArticleDetails.Where(x => x.BillID == billId).ToList();

            foreach (SoldArticleDetails soldArticleDetail in filteredSoldArticleDetails)
            {
                profit += totalPrice - (soldArticleDetail.EntryPrice * soldArticleDetail.SoldQuantity);
                return profit.ToString();
            }

            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
