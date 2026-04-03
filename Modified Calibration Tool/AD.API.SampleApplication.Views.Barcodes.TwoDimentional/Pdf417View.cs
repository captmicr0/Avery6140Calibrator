using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.Barcodes.TwoDimentional;

namespace AD.API.SampleApplication.Views.Barcodes.TwoDimentional;

public partial class Pdf417View : BaseBarcodeView, IComponentConnector
{
	public Pdf417View()
	{
		InitializeComponent();
		base.DataContext = new Pdf417ViewModel();
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
