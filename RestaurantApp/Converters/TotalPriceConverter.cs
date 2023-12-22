using System;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TotalPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || values[0] == null || values[1] == null)
            {
                return 0;
            }

            if (!(values[0] is int quantity) || !(values[1] is decimal price))
            {
                return 0;
            }

            decimal totalPrice = quantity * price;
            return totalPrice.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}