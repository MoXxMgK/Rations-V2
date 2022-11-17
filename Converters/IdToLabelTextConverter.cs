using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Rations_V2.Converters
{
    public class IdToLabelTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = (int)value;

            int[] milk = new[] { 1, 2 };

            if (milk.Contains(id))
                return "Průměrná denní dojivost, l/den";
            else
                return "Průměrný denní přírůstek, kg/den";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
