using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.ViewModels.Connection;
using AD.API.SampleApplication.Views.Scenarios;

namespace AD.API.SampleApplication.Views.Connection;

public partial class ConnectionView : Page, IComponentConnector
{
	public ConnectionView()
	{
		InitializeComponent();
		base.DataContext = new ConnectionViewModel();
	}

	private void BtnBack_Click(object sender, RoutedEventArgs e)
	{
		((ConnectionViewModel)base.DataContext).IsActionAvailable = false;
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate(object o, DoWorkEventArgs args)
		{
			((ConnectionViewModel)args.Argument).Finalization();
		};
		backgroundWorker.RunWorkerCompleted += delegate
		{
			NavigationController.GetInstance.NavigateTo<ScenariosView>();
		};
		backgroundWorker.RunWorkerAsync(base.DataContext);
	}
}
