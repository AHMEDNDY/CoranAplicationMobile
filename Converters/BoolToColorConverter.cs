using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoranWarshSynchroniser.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Color.FromArgb("#0F4C5C")  : Colors.Transparent;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => false;
    }
}
