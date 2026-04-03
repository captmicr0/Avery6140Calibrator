using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace AD.API.SampleApplication.Controls;

public partial class CommonMessageDialog : Window, IComponentConnector
{
	private const string OkButtonText = "OK";

	private const string CancelButtonText = "Cancel";

	private const string YesButtonText = "Yes";

	private const string NoButtonText = "No";

	public string ErrorCaption { get; set; }

	public string ErrorMessage { get; set; }

	public string FirstButtonText { get; set; }

	public string SecondButtonText { get; set; }

	public Visibility SecondButtonVisible { get; private set; }

	public CommonMessageDialog(string message, string caption, MessageBoxButton buttons, MessageBoxImage messageBoxImage)
	{
		InitializeComponent();
        var owner = Application.Current?.MainWindow;
        if (owner != null && owner.IsLoaded && owner.IsVisible)
        {
            this.Owner = owner;
        }
        ErrorMessage = Regex.Replace(message, "^\\s+$[\\r\\n]*", "", RegexOptions.Multiline);
		ErrorCaption = caption;
		switch (buttons)
		{
		case MessageBoxButton.OK:
			SecondButtonVisible = Visibility.Collapsed;
			FirstButtonText = "OK";
			break;
		case MessageBoxButton.OKCancel:
			SecondButtonVisible = Visibility.Visible;
			FirstButtonText = "OK";
			SecondButtonText = "Cancel";
			break;
		case MessageBoxButton.YesNo:
			SecondButtonVisible = Visibility.Visible;
			FirstButtonText = "Yes";
			SecondButtonText = "No";
			break;
		}
		switch (messageBoxImage)
		{
		case MessageBoxImage.Hand:
			MessageImage.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			break;
		case MessageBoxImage.Asterisk:
			MessageImage.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			break;
		case MessageBoxImage.Question:
			MessageImage.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			break;
		case MessageBoxImage.Exclamation:
			MessageImage.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			break;
		}
		base.DataContext = this;
	}

	private void FirstButton_OnClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
		Close();
	}

	private void SecondButton_OnClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
		Close();
	}
}
