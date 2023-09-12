using EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace RestaurantApp.Converters
{
    public class TableOrderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ObservableCollection<Table> && values[1] is string)
            {
                string tableIdString = (string)values[1];
                int tableId = int.Parse(tableIdString);

                ObservableCollection<Table> tablesObservableCollection = (ObservableCollection<Table>)values[0];
                List<Table> tables = tablesObservableCollection.ToList();

                Table table = tables.FirstOrDefault(x => x.ID == tableId);

                if (table is null)
                {
                    return Brushes.Gray;
                }

                if (table.InUse)
                {
                    return Brushes.LimeGreen;
                }

                if (!table.InUse)
                {
                    return Brushes.Gray;
                }
            }

            return Brushes.Gray;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
