using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using AD.API.Core;
using AD.API.Core.BaseImplementation;
using AD.API.Core.Exceptions;
using AD.API.Core.Imports;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;

namespace AD.API.SampleApplication.ViewModels.Scanner;

internal class ScannerViewModel : BaseViewModel
{
	private Command scanEnableCommand;

	private Command startScanCommand;

	private Command stopScanCommand;

	public Command ScanEnableCommand => scanEnableCommand;

	public Command StartScanCommand => startScanCommand;

	public Command StopScanCommand => stopScanCommand;

	public bool IsScannerSettingsAvailable => base.DeviceConnected;

	public string ScanEngineOnOf
	{
		get
		{
			if (!ApplicationContext.CurrentDevice.Scanner.IsEnabled)
			{
				return "Enable";
			}
			return "Disable";
		}
	}

	public bool SingleDeviceSelected => ApplicationContext.CurrentDevice != null;

	public string ScannedData => ApplicationContext.ScannedData;

	public ScannerViewModel()
	{
		try
		{
			scanEnableCommand = new Command(ScanEnableMethod);
			startScanCommand = new Command(StartScanMethod);
			stopScanCommand = new Command(StopScanMethod);
			SubscribeScanAllConnectedDevices();
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

	public override void DeviceInitilization(IDevice device)
	{
		base.DeviceInitilization(device);
		if (device != null)
		{
			device.Scanner.Scan += OnDataScanned;
		}
	}

	public override void ValidateSelf()
	{
		base.ValidateSelf();
		OnConnectionLostHandled(base.DeviceConnected);
		if (base.DeviceConnected)
		{
			OnPropertyChanged(string.Empty);
		}
	}

	private void ScanEnableMethod()
	{
		if (ApplicationContext.CurrentDevice.Scanner.IsEnabled)
		{
			ApplicationContext.CurrentDevice.Scanner.DisableScan();
		}
		else
		{
			ApplicationContext.CurrentDevice.Scanner.EnableScan();
		}
		OnPropertyChanged("ScanEngineOnOf");
	}

	private void StartScanMethod()
	{
		try
		{
			List<IDevice> deviceListToExecute = new List<IDevice>();
			if (ApplicationContext.AllDevicesSelected)
			{
				deviceListToExecute.AddRange(ApplicationContext.DeviceManager.ConnectedDevices);
			}
			else
			{
				deviceListToExecute.Add(ApplicationContext.CurrentDevice);
			}
			foreach (IDevice item in deviceListToExecute)
			{
				item.Scanner.Start();
			}
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

	private void StopScanMethod()
	{
		try
		{
			List<IDevice> deviceListToExecute = new List<IDevice>();
			if (ApplicationContext.AllDevicesSelected)
			{
				deviceListToExecute.AddRange(ApplicationContext.DeviceManager.ConnectedDevices);
			}
			else if (ApplicationContext.CurrentDevice != null)
			{
				deviceListToExecute.Add(ApplicationContext.CurrentDevice);
			}
			foreach (IDevice item in deviceListToExecute)
			{
				item.Scanner.Stop();
			}
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

	public override void Finalization()
	{
		try
		{
			StopScanMethod();
			UnsubscribeScanAllConnectedDevices();
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

	private void SubscribeScanAllConnectedDevices()
	{
		foreach (IDevice connectedDevice in ApplicationContext.DeviceManager.ConnectedDevices)
		{
			connectedDevice.Scanner.Scan += OnDataScanned;
		}
	}

	private void UnsubscribeScanAllConnectedDevices()
	{
		foreach (IDevice connectedDevice in ApplicationContext.DeviceManager.ConnectedDevices)
		{
			connectedDevice.Scanner.Scan -= OnDataScanned;
		}
	}

	private void OnDataScanned(object sender, ApiDataEventArgs args)
	{
		IScanner scanner = (IScanner)sender;
		if (scanner == null || (!ApplicationContext.AllDevicesSelected && !string.Equals(scanner.Device.Connection.Address, ApplicationContext.CurrentDevice.Connection.Address)))
		{
			return;
		}
		try
		{
			string barcodeTypeString = "Unknown";
			MemberInfo[] memberInfo = typeof(BarcodeType).GetMember(args.Barcode.ToString());
			if (memberInfo != null && memberInfo.Count() > 0 && memberInfo[0] != null)
			{
				object[] attributes = memberInfo[0].GetCustomAttributes(typeof(ComponentDescriptionAttribute), inherit: false);
				if (attributes != null && attributes.Count() > 0 && attributes[0] != null && attributes[0] is ComponentDescriptionAttribute)
				{
					barcodeTypeString = ((ComponentDescriptionAttribute)attributes[0]).Description;
				}
			}
			ApplicationContext.ScannedData += string.Format("Device: {0}\r\n Scanned: {1}. Type:{2}\r\n", scanner.Device.Serial, Regex.Replace(args.Data, "\\0", string.Empty).RemoveControlCharacters(), barcodeTypeString);
			OnPropertyChanged("ScannedData");
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
}
