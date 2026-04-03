using System.Windows;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;

namespace AD.API.SampleApplication.Controls;

public partial class LntAction : Window, IComponentConnector
{
	public bool shouldPrint;

	public ResourceActions CurrentAction;

	public string MessageText { get; set; }

	public LntAction(string message)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		MessageText = message;
		shouldPrint = false;
		base.DataContext = this;
	}

	private void PrintButton_Click(object sender, RoutedEventArgs e)
	{
		CurrentAction = ResourceActions.Print;
		Close();
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
