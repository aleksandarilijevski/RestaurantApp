using EntityFramework.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class IDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bill bill = (Bill)value;

            if (bill.Table is not null)
            {
                return bill.TableID.ToString();
            }

            if (bill.OnlineOrder is not null)
            {
                return bill.OnlineOrderID.ToString();
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
