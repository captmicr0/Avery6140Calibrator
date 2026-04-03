using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AD.API.SampleApplication.Tools;

public class TabSizeConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		TabControl tabControl = values[0] as TabControl;
		double width = tabControl.ActualWidth / (double)tabControl.Items.Count;
		return (width <= 1.0) ? 0.0 : (width - 1.0);
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
