using System.Windows;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;

namespace AD.API.SampleApplication.Controls;

public partial class DeviceErrorDialog : Window, IComponentConnector
{
	public string ErrorCaption { get; set; }

	public string ErrorMessage { get; set; }

	public DeviceErrorResult ErrorDialogResult { get; private set; }

	public DeviceErrorDialog(string caption, string message, bool isError450Thrown)
	{
		InitializeComponent();
		if (isError450Thrown)
		{
			AbortErrorButton.Content = "Abort error & Resync";
		}
		else
		{
			AbortErrorButton.Content = "Abort error";
		}
		base.Owner = Application.Current.MainWindow;
		ErrorDialogResult = DeviceErrorResult.IgnoreError;
		ErrorCaption = caption;
		ErrorMessage = message;
		base.DataContext = this;
	}

	private void IgnoreErrorButton_Click(object sender, RoutedEventArgs e)
	{
		ErrorDialogResult = DeviceErrorResult.IgnoreError;
		Close();
	}

	private void AbortErrorButton_Click(object sender, RoutedEventArgs e)
	{
		ErrorDialogResult = DeviceErrorResult.AbortError;
		Close();
	}

	private void ClearErrorButton_Click(object sender, RoutedEventArgs e)
	{
		ErrorDialogResult = DeviceErrorResult.ClearErorr;
		Close();
	}
}
