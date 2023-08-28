using EntityFramework.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TableOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Table)
            {
                Table table = (Table)value;

                if (table.InUse)
                {
                    return "Green";
                }
            }

            return "black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
