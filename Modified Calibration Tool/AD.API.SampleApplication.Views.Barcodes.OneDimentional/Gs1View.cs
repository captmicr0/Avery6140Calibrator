using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

namespace AD.API.SampleApplication.Views.Barcodes.OneDimentional;

public partial class Gs1View : BaseBarcodeView, IComponentConnector
{
	public Gs1View()
	{
		InitializeComponent();
		base.DataContext = new Gs1ViewModel();
		SubscribeHandlePostConnectionLost(base.DataContext as BaseViewModel);
	}

	private void BackButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<BarcodeListView>();
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
