﻿using EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RestaurantApp.Converters
{
    public class TotalQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<ArticleDetails> articleDetails)
            {
                int sumOfOriginalQuantity = articleDetails.Sum(x => x.OriginalQuantity);
                return sumOfOriginalQuantity;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
