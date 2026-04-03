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
using AD.API.SampleApplication.ViewModels.Scanner;
using AD.API.SampleApplication.Views.Barcodes;

namespace AD.API.SampleApplication.Views.Scanner;

public partial class ScannerSettingsView : Page, IComponentConnector
{
	public ScannerSettingsView()
	{
		InitializeComponent();
		base.DataContext = new ScannerSettingsViewModel();
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
							Messenger.ShowSelectedDeviceConnectionLost("Scanner View");
							NavigationController.GetInstance.NavigateTo<ScannerView>();
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
		if (!NavigationController.GetInstance.NavigateTo<ScannerView>())
		{
			Messenger.ShowMessage("Unable to load the view. \r\nRedirecting to Main Screen", "Navigation error", MessageBoxButton.OK, MessageBoxImage.Hand);
			NavigationController.GetInstance.NavigateTo<ConfigView>();
		}
	}

	private void BtnBarcodeSettings_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			((BaseViewModel)base.DataContext).Finalization();
			NavigationController.GetInstance.NavigateTo<BarcodeListView>();
		}
		catch (ApiException ex)
		{
			Messenger.ShowErrorMessage(ex);
		}
		catch (Exception ex2)
		{
			Messenger.ShowMessage(ex2.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
