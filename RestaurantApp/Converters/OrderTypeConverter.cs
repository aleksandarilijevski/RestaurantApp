using EntityFramework.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class OrderTypeConverter : IValueConverter
    {
        public const string TableOrderDefaultValue = "Table order";

        public const string OnlineOrderDefaultValue = "Online order";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bill bill = (Bill)value;

            if (bill.Table is not null)
            {
                return TableOrderDefaultValue;
            }

            if (bill.OnlineOrder is not null)
            {
                return OnlineOrderDefaultValue;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
