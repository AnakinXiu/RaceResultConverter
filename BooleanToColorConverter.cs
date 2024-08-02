using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace RaceResultConverter;

public class BooleanToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? Color.LightBlue : Color.DarkRed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Color color && color.ToArgb() == Color.LightBlue.ToArgb();
    }
}