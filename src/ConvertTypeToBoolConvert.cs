﻿using System.Globalization;
using System.Windows.Data;

namespace RaceResultConverter;

public class ConvertTypeToBoolConvert : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null && value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null && value.Equals(true) ? parameter : ConvertType.ZonToZRound;
    }
}