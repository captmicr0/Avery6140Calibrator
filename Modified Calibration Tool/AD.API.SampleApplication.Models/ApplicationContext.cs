using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
using AD.API.Core.Imports;
using AD.API.Devices.Management;
using AD.API.SampleApplication.Controls;
using AD.API.SampleApplication.Helpers;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.Win8Desktop.Devices.Management;
using AD.API.Win8Desktop.Resources;
using NLog;

namespace AD.API.SampleApplication.Models;

internal static class ApplicationContext
{
	public static string ScannedData;

	public static string ScanData;

	public static bool AutoTrigger;

	private static Logger metricsLogger;

	public static int lostConnectionDevices;

	public static BaseViewModel CurrentViewModel;

	private static Dictionary<string, string> addressNamePair;

	public static Stopwatch stopwatch;

	public static string CurrentDeviceAddress;

	public static ResourceManager ResourceManager { get; private set; }

	public static DeviceManager DeviceManager { get; private set; }

	public static bool AllDevicesSelected { get; set; }

	public static dynamic ConfigData { get; set; }

	public static string trackingPrefix { get; set; }

	public static uint trackingLength { get; set; }

	public static string[] trackingSymbologies { get; set; }

	public static string routeZoneLocation { get; set; }

	public static string sortZoneLocation { get; set; }

	public static string trackingIDLocation { get; set; }

	public static string errorCodeLocation { get; set; }

	public static int maxDeviceCount { get; set; }

	public static bool resetButtonEnable { get; set; }

	public static bool serverLoggingEnable { get; set; }

	public static bool infoLoggingEnable { get; set; }

	public static IDevice CurrentDevice
	{
		get
		{
			if (DeviceManager != null)
			{
				return DeviceManager.ConnectedDevices[CurrentDeviceAddress];
			}
			return null;
		}
	}

	/*static ApplicationContext()
	{
		ScannedData = string.Empty;
		ScanData = string.Empty;
		AutoTrigger = false;
		metricsLogger = LogManager.GetLogger("MetricsLogger");
		ResourceManager = new ResourceManager();
		addressNamePair = new Dictionary<string, string>();
		DeviceManager = new DeviceManager();
		stopwatch = new Stopwatch();
	}*/
	static ApplicationContext()
{
    try
    {
        ScannedData = string.Empty;
        ScanData = string.Empty;
        AutoTrigger = false;

        metricsLogger = LogManager.GetLogger("MetricsLogger");

        ResourceManager = new ResourceManager();

        addressNamePair = new Dictionary<string, string>();

        DeviceManager = new DeviceManager();

        stopwatch = new Stopwatch();
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            ex.ToString(),
            "ApplicationContext Initialization Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );

        throw;
    }
}

	internal static void Disconnect(IDevice deviceToDisconnect)
	{
		if (deviceToDisconnect != null)
		{
			if (addressNamePair.ContainsKey(deviceToDisconnect.Connection.Address))
			{
				addressNamePair.Remove(deviceToDisconnect.Connection.Address);
			}
			UnsubscribeFromErrorEvents(deviceToDisconnect);
			DeviceManager.DisconnectDevice(deviceToDisconnect);
			if ((CurrentDevice == null && !AllDevicesSelected) || DeviceManager.ConnectedDevices.Count == 1)
			{
				AllDevicesSelected = false;
				CurrentDeviceAddress = ((DeviceManager != null && DeviceManager.ConnectedDevices.FirstOrDefault() != null) ? DeviceManager.ConnectedDevices.FirstOrDefault().Connection.Address : string.Empty);
			}
		}
	}

	internal static void Connect(DeviceConnectionInfo deviceConnectionInfo)
	{
		IDevice device = DeviceManager.CreateDevice(deviceConnectionInfo);
		SubscribeToErrorEvents(device);
		if (!addressNamePair.ContainsKey(deviceConnectionInfo.Address))
		{
			addressNamePair.Add(deviceConnectionInfo.Address, deviceConnectionInfo.Name);
		}
		if (CurrentDevice == null && !AllDevicesSelected)
		{
			CurrentDeviceAddress = device.Connection.Address;
		}
	}

	public static void SubscribeToErrorEvents(IDevice device)
	{
		if (device != null && device.Connection.State == ConnectionState.Open)
		{
			device.Error += DeviceOnError;
			device.Connection.Error += ConnectionOnError;
			device.Printer.Error += PrinterOnError;
		}
	}

	public static void UnsubscribeFromErrorEvents(IDevice device)
	{
		if (device != null && device.Connection.State == ConnectionState.Open)
		{
			device.Error -= DeviceOnError;
			device.Connection.Error -= ConnectionOnError;
			device.Printer.Error -= PrinterOnError;
		}
	}

	private static void PrinterOnError(object sender, ApiErrorEventArgs args)
	{
		IPrinter printer = sender as IPrinter;
		if (printer == null)
		{
			return;
		}
		new Thread((ThreadStart)delegate
		{
			string errorText = $"{args.InnerException.Message} Error code is {(int)args.CdilErrorCode}.";
			string errorCaption = string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, printer.Device.Serial, MessageSynchronizer.PRINTER_ERROR_TITLE);
			bool isError450Thrown = false;
			while (MessageBoxKiller.CloseTargetMessage(errorCaption))
			{
			}
			if (((ApiPrinterException)args.InnerException).ErrorCode == ErrorCode.PrinterMissingBlackmark || ((ApiPrinterException)args.InnerException).ErrorCode == ErrorCode.PrinterUnexpextedBlackmark || ((ApiPrinterException)args.InnerException).ErrorCode == ErrorCode.PrinterSenseMarkIsTooLong)
			{
				isError450Thrown = true;
			}
			else
			{
				isError450Thrown = false;
			}
			Metrics.LogMetricsError(metricsLogger, (int)args.CdilErrorCode, errorCaption + " - " + args.InnerException.Message);
			switch ((int)args.CdilErrorCode)
			{
			case 412:
			case 751:
				printer.ClearError();
				Metrics.LogMetricsErrorHandled(metricsLogger, (int)args.CdilErrorCode, "Cleared");
				break;
			case 704:
			case 706:
			case 752:
			case 756:
			case 766:
			case 790:
			case 793:
				printer.AbortError();
				Metrics.LogMetricsErrorHandled(metricsLogger, (int)args.CdilErrorCode, "Aborted");
				break;
			default:
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					DeviceErrorDialog deviceErrorDialog = new DeviceErrorDialog(errorCaption, errorText, isError450Thrown);
					deviceErrorDialog.ShowDialog();
					if (deviceErrorDialog.ErrorDialogResult == DeviceErrorResult.IgnoreError)
					{
						Metrics.LogMetricsErrorHandled(metricsLogger, (int)args.CdilErrorCode, "Ignored");
					}
					else if (deviceErrorDialog.ErrorDialogResult == DeviceErrorResult.ClearErorr)
					{
						printer.ClearError();
						Metrics.LogMetricsErrorHandled(metricsLogger, (int)args.CdilErrorCode, "Cleared");
					}
					else if (deviceErrorDialog.ErrorDialogResult == DeviceErrorResult.AbortError)
					{
						printer.AbortError();
						if (isError450Thrown)
						{
							printer.Resync();
						}
						Metrics.LogMetricsErrorHandled(metricsLogger, (int)args.CdilErrorCode, "Aborted");
					}
				});
				break;
			}
		}).Start();
	}

	private static void ConnectionOnError(object sender, ApiErrorEventArgs args)
	{
		CurrentViewModel.IsActionAvailable = false;
		IConnection connection = sender as IConnection;
		if (connection == null)
		{
			return;
		}
		MessageSynchronizer.GetObject().ClearAllMessagesForDevice(addressNamePair[connection.Address]);
		new Thread((ThreadStart)delegate
		{
			Interlocked.Increment(ref lostConnectionDevices);
			IDevice device = null;
			while (Messenger.ShowMessage($"{args.InnerException.Message}\r\nPress \"Ok\" to attempt reconnection with the device. Press \"Cancel\" to ignore.", string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, addressNamePair[connection.Address], MessageSynchronizer.CONNECTION_ERROR_TITLE), MessageBoxButton.OKCancel, MessageBoxImage.Hand))
			{
				try
				{
					CurrentViewModel.IsActionAvailable = false;
					device = DeviceManager.ReconnectDevice(connection.Address);
					SubscribeToErrorEvents(device);
					CheckPrinterState(device);
					if (CurrentViewModel != null)
					{
						CurrentViewModel.DeviceInitilization(device);
					}
				}
				catch (Exception)
				{
					continue;
				}
				break;
			}
			Interlocked.Decrement(ref lostConnectionDevices);
			if (lostConnectionDevices == 0 && CurrentViewModel != null)
			{
				try
				{
					CurrentViewModel.ValidateSelf();
					CurrentViewModel.IsActionAvailable = true;
				}
				catch (ApiException ex2)
				{
					Messenger.ShowMessage($"{ex2.Message} Device: {addressNamePair[connection.Address]}", MessageSynchronizer.ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				catch (Exception ex3)
				{
					Messenger.ShowMessage(ex3.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
			}
		}).Start();
		if (DeviceManager.ConnectedDevices.Count == 0 && !BluetoothInfo.CheckBluetooth())
		{
			Messenger.ShowMessage("Bluetooth adapter is turned off or not completely started. Please enable Bluetooth.", "", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private static void DeviceOnError(object o, ApiErrorEventArgs args)
	{
		Messenger.ShowMessage($"Device error: {args.ErrorMessage}", "Device error!", MessageBoxButton.OK, MessageBoxImage.Hand);
	}

	public static void CheckPrinterState(IDevice deviceToCheck)
	{
		try
		{
			ErrorCode deviceStatus = deviceToCheck.Status;
			if (deviceStatus != ErrorCode.Success)
			{
				PrinterOnError(deviceToCheck.Printer, new ApiErrorEventArgs
				{
					CdilErrorCode = deviceStatus,
					ErrorMessage = string.Empty,
					InnerException = new ApiPrinterException(deviceStatus, deviceToCheck.Api.GetErrorText(deviceStatus))
				});
			}
		}
		catch (ApiException ex)
		{
			Messenger.ShowMessage($"Error code: {ex.ErrorCode}. \r\n{ex.Message}", string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, deviceToCheck.Serial, MessageSynchronizer.ERROR_TITLE), MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		catch (Exception ex2)
		{
			Messenger.ShowMessage(ex2.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
