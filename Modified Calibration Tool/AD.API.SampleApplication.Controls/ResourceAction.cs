using System.Windows;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;

namespace AD.API.SampleApplication.Controls;

public partial class ResourceAction : Window, IComponentConnector
{
	private string messageText;

	public ResourceActions CurrentAction;

	public string MessageText { get; set; }

	public ResourceAction(string message)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		MessageText = message;
		base.DataContext = this;
	}

	private void DeleteButton_Click(object sender, RoutedEventArgs e)
	{
		CurrentAction = ResourceActions.Delete;
		Close();
	}

	private void IgnoreButton_Click(object sender, RoutedEventArgs e)
	{
		CurrentAction = ResourceActions.Ignore;
		Close();
	}
}
