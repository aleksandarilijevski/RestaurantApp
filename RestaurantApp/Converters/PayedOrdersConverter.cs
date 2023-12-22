using EntityFramework.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RestaurantApp.Converters
{
    public class PayedOrdersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OnlineOrder onlineOrder = (OnlineOrder)value;

            if (onlineOrder.IsPayed)
            {
                return Brushes.DarkGreen;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
