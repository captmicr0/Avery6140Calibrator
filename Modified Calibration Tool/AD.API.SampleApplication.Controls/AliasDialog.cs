using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AD.API.SampleApplication.Controls;

public partial class AliasDialog : Window, IComponentConnector
{
	private string resourceAlias;

	public string ResourceAlias
	{
		get
		{
			return resourceAlias;
		}
		set
		{
			if (!string.Equals(resourceAlias, value, StringComparison.OrdinalIgnoreCase))
			{
				resourceAlias = value;
			}
		}
	}

	public AliasDialog()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		base.DataContext = this;
	}

	private void Ok_Click(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(resourceAlias))
		{
			base.DialogResult = true;
			Close();
		}
	}

	private void Cancel_Click(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
		Close();
	}

	private void TextBox_GotFocus(object sender, RoutedEventArgs e)
	{
		((TextBox)sender).Text = string.Empty;
	}
}
