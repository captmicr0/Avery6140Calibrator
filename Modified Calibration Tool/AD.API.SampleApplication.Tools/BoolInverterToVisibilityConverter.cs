using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AD.API.SampleApplication.Tools;

internal class BoolInverterToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
		}
		catch (InvalidCastException)
		{
			return Visibility.Collapsed;
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return ((Visibility)value != Visibility.Visible) ? true : false;
		}
		catch (InvalidCastException)
		{
			return true;
		}
	}
}
