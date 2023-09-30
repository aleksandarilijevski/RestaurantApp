﻿using EntityFramework.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class OrderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bill bill = (Bill)value;

            if (bill.Table is not null)
            {
                return "Table order";
            }

            if (bill.OnlineOrder is not null)
            {
                return "Online order";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
