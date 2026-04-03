using System;
using System.Globalization;
using System.Windows.Data;

namespace AD.API.SampleApplication.Tools;

public class BoolInverterConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool num = (bool)value;
		bool result = bool.Parse((string)parameter);
		if (num == result)
		{
			return true;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return bool.Parse((string)parameter);
	}
}
