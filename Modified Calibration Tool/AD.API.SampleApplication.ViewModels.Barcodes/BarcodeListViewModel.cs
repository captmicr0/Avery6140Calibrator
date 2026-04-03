using System;
using System.Windows;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;

namespace AD.API.SampleApplication.ViewModels.Barcodes;

internal class BarcodeListViewModel : BaseBarcodeViewModel
{
	private Command disableBarcodesCommand;

	public Command DisableBarcodesCommand => disableBarcodesCommand;

	public BarcodeListViewModel()
	{
		disableBarcodesCommand = new Command(DisableBarcodesMethod);
		DeviceInitilization(ApplicationContext.DeviceManager.ConnectedDevices[EnteredDeviceAddress]);
	}

	public void EndScannerSetSession()
	{
		if (ApplicationContext.DeviceManager.ConnectedDevices[EnteredDeviceAddress] != null)
		{
			ApplicationContext.DeviceManager.ConnectedDevices[EnteredDeviceAddress].Scanner.EndSetSession();
		}
	}

	private void DisableBarcodesMethod()
	{
		if (Messenger.ShowMessage("Are you sure you want to disable all barcodes for the devices?", string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, base.CurrentDeviceSerial, MessageSynchronizer.RESET_PARAMETERS_TITLE), MessageBoxButton.YesNo, MessageBoxImage.Question))
		{
			try
			{
				ApplicationContext.CurrentDevice.Scanner.DisableAllBarcodes();
			}
			catch (ApiException ex)
			{
				Messenger.ShowErrorMessage(ex);
			}
			catch (Exception ex2)
			{
				Messenger.ShowMessage(ex2.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
	}
}
