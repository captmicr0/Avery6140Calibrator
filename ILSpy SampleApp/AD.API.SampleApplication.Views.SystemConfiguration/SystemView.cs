using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.SystemConfiguration;

namespace AD.API.SampleApplication.Views.SystemConfiguration;

public partial class SystemView : Page, IComponentConnector
{
	public SystemView()
	{
		InitializeComponent();
		base.DataContext = new SystemViewModel();
		((BaseViewModel)base.DataContext).ConnectionLostHandledEvent += delegate(object sender, ConnectionLostHandledEventArgs args)
		{
			if (!args.IsValid)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					Messenger.ShowSelectedDeviceConnectionLost("Main Menu");
					((BaseViewModel)base.DataContext).Finalization();
					base.DataContext = null;
					NavigationController.GetInstance.NavigateTo<ConfigView>();
				});
			}
		};
	}

	private void BtnGoToBlack_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			((BaseViewModel)base.DataContext).Finalization();
		}
		catch (ApiException ex)
		{
			Messenger.ShowErrorMessage(ex);
		}
		catch (Exception ex2)
		{
			Messenger.ShowMessage(ex2.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		NavigationController.GetInstance.NavigateTo<ConfigView>();
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
