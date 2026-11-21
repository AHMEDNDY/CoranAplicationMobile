using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CoranWarshSynchroniser.Converters;

// Convertit 123 en ١٢٣ pour les badges d'ayat. Sans toucher au modèle Surah.
public sealed class ArabicVerseNumberConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;
        var s = System.Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        const string digits = "٠١٢٣٤٥٦٧٨٩";
        var chars = s.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
            if (chars[i] >= '0' && chars[i] <= '9')
                chars[i] = digits[chars[i] - '0'];
        return new string(chars);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
