using System.Windows.Controls;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.Views.Scenarios;

namespace AD.API.SampleApplication.Views;

public partial class MainView : Page, IComponentConnector
{
	public MainView()
	{
		base.DataContext = new MainViewModel();
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<ScenariosView>();
	}
}
