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
using AD.API.SampleApplication.ViewModels.Printer;

namespace AD.API.SampleApplication.Views.Printer;

public partial class PrinterSettingsView : Page, IComponentConnector
{
	public PrinterSettingsView()
	{
		InitializeComponent();
		base.DataContext = new PrinterSettingsViewModel();
		((BaseViewModel)base.DataContext).ConnectionLostHandledEvent += delegate(object sender, ConnectionLostHandledEventArgs args)
		{
			if (!args.IsValid)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					try
					{
						if (((BaseViewModel)base.DataContext).DeviceConnected)
						{
							Messenger.ShowSelectedDeviceConnectionLost("Printer View");
							NavigationController.GetInstance.NavigateTo<PrinterView>();
						}
						else
						{
							Messenger.ShowSelectedDeviceConnectionLost("Main Menu");
							NavigationController.GetInstance.NavigateTo<ConfigView>();
						}
					}
					finally
					{
						((BaseViewModel)base.DataContext).Finalization();
						base.DataContext = null;
					}
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
			Messenger.ShowMessage(ex2.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		if (!NavigationController.GetInstance.NavigateTo<PrinterView>())
		{
			Messenger.ShowMessage("Unable to load the view. \r\nRedirecting to Main Screen", "Navigation error", MessageBoxButton.OK, MessageBoxImage.Hand);
			NavigationController.GetInstance.NavigateTo<ConfigView>();
		}
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
