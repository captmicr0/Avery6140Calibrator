using System.Windows;
using System.Windows.Controls;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.Views.Scanner;

namespace AD.API.SampleApplication.Views.Barcodes;

public class BaseBarcodeView : Page
{
	protected void SubscribeHandlePostConnectionLost(BaseViewModel baseViewModel)
	{
		if (baseViewModel == null)
		{
			return;
		}
		baseViewModel.ConnectionLostHandledEvent += delegate(object sender, ConnectionLostHandledEventArgs args)
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
							if (!NavigationController.GetInstance.NavigateTo<ScannerView>())
							{
								Messenger.ShowMessage("Unable to load the view. \r\nRedirecting to Main Screen", "Navigation error", MessageBoxButton.OK, MessageBoxImage.Hand);
								NavigationController.GetInstance.NavigateTo<MainView>();
							}
						}
						else
						{
							Messenger.ShowSelectedDeviceConnectionLost("Main Menu");
							NavigationController.GetInstance.NavigateTo<MainView>();
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
}
