using EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class OrderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Bill>)
            {
                ObservableCollection<Bill> bills = (ObservableCollection<Bill>)value;

                foreach (Bill bill in bills)
                {
                    if (bill.Table is not null)
                    {
                        return "Table order";
                    }

                    if (bill.OnlineOrder is not null)
                    {
                        return "Online order";
                    }
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
