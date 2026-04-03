using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
using AD.API.Core.Imports;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Helpers;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;
using Newtonsoft.Json;
using NLog;
using WindowsInput;
using WindowsInput.Native;

namespace AD.API.SampleApplication.ViewModels.Scenarios;

internal class ScenariosViewModel : BaseViewModel
{
	private AveryWebServer zplWebServer;

	private ConfigurationHandler configHandler = new ConfigurationHandler();

	private static Logger logger = LogManager.GetLogger("MetricsLogger");

	private Command blackMarkCalibrationCommand;

	private Command resetCommand;

	public Command BlackMarkCalibrationCommand => blackMarkCalibrationCommand;

	public Command ResetCommand => resetCommand;

	public string ScanData => ApplicationContext.ScanData;

	public string ActiveDevice
	{
		get
		{
			if (ApplicationContext.CurrentDevice == null)
			{
				return "";
			}
			return ApplicationContext.CurrentDevice.Serial;
		}
	}

	public bool AutoTrigger
	{
		get
		{
			return ApplicationContext.AutoTrigger;
		}
		set
		{
			ApplicationContext.AutoTrigger = value;
		}
	}

	public ScenariosViewModel()
	{
		SubscribeScanAllConnectedDevices();
		if (zplWebServer == null)
		{
			zplWebServer = new AveryWebServer();
		}
		zplWebServer.DataRecieved += IncomingData;
		zplWebServer.Start(8080);
		blackMarkCalibrationCommand = new Command(CalibrateBlackMarkSensor);
		resetCommand = new Command(ResetDevicesMethod);
		string configPath = AppDomain.CurrentDomain.BaseDirectory + "configuration\\";
		Console.WriteLine("Configuration File Path: " + configPath);
		if (ApplicationContext.ConfigData == null)
		{
			ApplicationContext.ConfigData = (object)configHandler.parseConfigFile(configPath);
			if (ApplicationContext.ConfigData == null)
			{
				Messenger.ShowMessage("There was an issue while loading the configuration file. Please validate configuration file syntax and values.", "Configuration File Load Error.", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			ApplicationContext.trackingSymbologies = configHandler.getTrackingSymbologies(ApplicationContext.ConfigData);
			ApplicationContext.routeZoneLocation = configHandler.getRouteZoneLocation(ApplicationContext.ConfigData);
			ApplicationContext.sortZoneLocation = configHandler.getSortZoneLocation(ApplicationContext.ConfigData);
			ApplicationContext.trackingIDLocation = configHandler.getTrackingIDLocation(ApplicationContext.ConfigData);
			ApplicationContext.errorCodeLocation = configHandler.getErrorCodeLocation(ApplicationContext.ConfigData);
			ApplicationContext.maxDeviceCount = configHandler.getMaxDeviceCount(ApplicationContext.ConfigData);
			ApplicationContext.resetButtonEnable = configHandler.getResetButtonEnable(ApplicationContext.ConfigData);
			ApplicationContext.serverLoggingEnable = configHandler.getServerLoggingEnable(ApplicationContext.ConfigData);
			ApplicationContext.infoLoggingEnable = configHandler.getInfoLoggingEnable(ApplicationContext.ConfigData);
		}
	}

	public override void DeviceInitilization(IDevice device)
	{
		base.DeviceInitilization(device);
		if (device != null)
		{
			device.Scanner.DecodeSessionTimeout = 99;
			device.Scanner.Mode = ScanMode.Compatible;
			device.Scanner.Scan += ScannerOnScan;
			if (device == ApplicationContext.CurrentDevice)
			{
				OnPropertyChanged("ActiveDevice");
			}
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

	public override void Finalization()
	{
		base.Finalization();
		UnsubscribeScanAllConnectedDevices();
		zplWebServer.Stop();
	}

	private void SubscribeScanAllConnectedDevices()
	{
		foreach (IDevice connectedDevice in ApplicationContext.DeviceManager.ConnectedDevices)
		{
			connectedDevice.Scanner.DecodeSessionTimeout = 99;
			connectedDevice.Scanner.Mode = ScanMode.Compatible;
			connectedDevice.Scanner.Scan += ScannerOnScan;
		}
	}

	private void UnsubscribeScanAllConnectedDevices()
	{
		foreach (IDevice connectedDevice in ApplicationContext.DeviceManager.ConnectedDevices)
		{
			connectedDevice.Scanner.Scan -= ScannerOnScan;
		}
	}

	private void CalibrateBlackMarkSensor()
	{
		dynamic wizardInfo = null;
		string wizPath = AppDomain.CurrentDomain.BaseDirectory + "wizard\\";
		Console.WriteLine("Wizard File Path: " + wizPath);
		if (!Directory.Exists(wizPath))
		{
			MessageBox.Show("Failed to load the Black Mark Calibration Wizard. Make sure that any .json files have not been edited or removed.", (string)wizardInfo.caption, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		if (File.Exists(wizPath + "\\CalibrateBM.json"))
		{
			wizardInfo = JsonConvert.DeserializeObject(File.ReadAllText(wizPath + "CalibrateBM.json"));
		}
		if (wizardInfo == null)
		{
			MessageBox.Show("Failed to load the Black Mark Calibration Wizard. Make sure that any .json files have not been edited or removed.", (string)wizardInfo.caption, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		try
		{
			foreach (dynamic step in wizardInfo.steps)
			{
				MessageBox.Show((string)step.text, (string)wizardInfo.caption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
				if (ApplicationContext.DeviceManager.ConnectedDevices.Count < 1)
				{
					throw new Exception("No devices conneted.");
				}
				if (step.calBM == true)
				{
					ApplicationContext.CurrentDevice.Printer.CalibrateBlackMark();
				}
				if (step.feed == true)
				{
					ApplicationContext.CurrentDevice.Printer.FeedLabel();
				}
			}
		}
		catch (Exception)
		{
			MessageBox.Show("An error occured. Make sure the 6140 is connected and try again.", (string)wizardInfo.caption, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void ResetDevicesMethod()
	{
		base.IsActionAvailable = false;
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate
		{
			try
			{
				Console.WriteLine("Applying Device configurations to connected devices");
				foreach (IDevice current in ApplicationContext.DeviceManager.ConnectedDevices)
				{
					if (current.Status != ErrorCode.Success)
					{
						current.Printer.AbortError();
					}
					configHandler.applyPrinterConfig(ApplicationContext.ConfigData, current);
					configHandler.applyScannerConfig(ApplicationContext.ConfigData, current);
				}
			}
			catch (ApiException ex)
			{
				Messenger.ShowErrorMessage(ex);
				base.IsActionAvailable = true;
			}
			catch (Exception ex2)
			{
				Messenger.ShowMessage(ex2.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
				base.IsActionAvailable = true;
			}
		};
		backgroundWorker.RunWorkerCompleted += delegate
		{
			try
			{
				base.IsActionAvailable = true;
			}
			catch (ApiException ex)
			{
				Messenger.ShowErrorMessage(ex);
				base.IsActionAvailable = true;
			}
			catch (Exception ex2)
			{
				Messenger.ShowMessage(ex2.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
				base.IsActionAvailable = true;
			}
		};
		backgroundWorker.RunWorkerAsync();
	}

	private void ScannerOnScan(object sender, ApiDataEventArgs args)
	{
		Metrics.LogMetricsInfo(logger, "Scan Data Recieved from 6140. Symbology: " + args.Barcode.ToString() + ", Data: " + args.Data);
		ApplicationContext.stopwatch.Stop();
		ApplicationContext.stopwatch.Reset();
		ApplicationContext.stopwatch.Start();
		IScanner scanner = (IScanner)sender;
		if (scanner == null)
		{
			Metrics.LogMetricsInfo(logger, "Exiting Scan Data function due to no scanner present.");
			return;
		}
		if (ApplicationContext.CurrentDevice == null || scanner.Device != ApplicationContext.CurrentDevice)
		{
			ApplicationContext.CurrentDeviceAddress = scanner.Device.Connection.Address;
			OnPropertyChanged("ActiveDevice");
			Metrics.LogMetricsInfo(logger, "Active Device Changed: New Device is " + ApplicationContext.CurrentDevice.Serial);
		}
		if (args.Data.StartsWith("NR"))
		{
			Metrics.LogMetricsInfo(logger, "Exiting Scan Data function with a No Read Scan.");
			return;
		}
		if ((!configHandler.isValidTrackingFormat(ApplicationContext.ConfigData, args.Data)))
		{
			scanner.Device.PlaySound("Scan_Bad", 1u);
			Thread.Sleep(250);
			if (AutoTrigger)
			{
				scanner.Start();
			}
			Metrics.LogMetricsInfo(logger, "Scan Data Did not pass the set filters.");
			return;
		}
		try
		{
			InputSimulator inputSimulator = new InputSimulator();
			inputSimulator.Keyboard.TextEntry(args.Data);
			inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
		}
		catch (Exception ex)
		{
			Messenger.ShowMessage(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		try
		{
			ApplicationContext.ScanData = "Scanned: ";
			ApplicationContext.ScanData += args.Data;
			ApplicationContext.ScanData += "\r\n";
			OnPropertyChanged("ScanData");
			Metrics.LogMetricsScan(logger, args.Data);
		}
		catch (Exception ex2)
		{
			Messenger.ShowMessage(ex2.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void IncomingData(object sender, string labelData)
	{
		Console.WriteLine("IncomingData Entry.");
		Metrics.LogMetricsInfo(logger, "Data Recieved from COMP.");
		string routeCode = "";
		string routeZone = "";
		string trackingID = "";
		string errorCode = "";
		labelData = labelData.Replace("^FS ^FO390,170 ^A0,100 ^FD", "-");
		labelData.IndexOf("^FO390,50 ^A0,120 ^FD");
		StringBuilder pattern = new StringBuilder();
		pattern.Append(Regex.Escape(ApplicationContext.sortZoneLocation));
		pattern.Append(" \\^A0,[0-9]+ \\^FD(.*?)\\^FS");
		Match match = new Regex(pattern.ToString()).Match(labelData);
		if (match.Success)
		{
			routeCode = match.Groups[1].Value;
			pattern.Clear();
			pattern.Append(Regex.Escape(ApplicationContext.routeZoneLocation));
			pattern.Append(" \\^A0,[0-9]+ \\^FD(.*?)\\^FS");
			match = new Regex(pattern.ToString()).Match(labelData);
			if (match.Success)
			{
				routeZone = match.Groups[1].Value;
				pattern.Clear();
				pattern.Append(Regex.Escape(ApplicationContext.trackingIDLocation));
				pattern.Append(" \\^A0,[0-9]+ \\^FD(.*?)\\^FS");
				match = new Regex(pattern.ToString()).Match(labelData);
				if (match.Success)
				{
					trackingID = match.Groups[1].Value;
					pattern.Clear();
					pattern.Append(Regex.Escape(ApplicationContext.errorCodeLocation));
					pattern.Append(" \\^A0,[0-9]+ \\^FD(.*?)\\^FS");
					match = new Regex(pattern.ToString()).Match(labelData);
					if (match.Success)
					{
						errorCode = match.Groups[1].Value;
					}
					string topData = "";
					string midData = "";
					string bottomData = "";
					topData = routeZone;
					midData = routeCode;
					bottomData = trackingID;
					List<byte[]> printData = new List<byte[]>
					{
						Encoding.UTF8.GetBytes(topData),
						Encoding.UTF8.GetBytes(midData),
						Encoding.UTF8.GetBytes(bottomData)
					};
					try
					{
						ApplicationContext.CurrentDevice.Printer.Print("newamazonlabel", 1, printData);
						ApplicationContext.stopwatch.Stop();
						string elapsedTime = $"{ApplicationContext.stopwatch.Elapsed.TotalMilliseconds} ms";
						ApplicationContext.ScanData += "Printed:";
						ApplicationContext.ScanData = ApplicationContext.ScanData + " Tracking ID = " + trackingID;
						ApplicationContext.ScanData = ApplicationContext.ScanData + ", Route Code = " + routeCode;
						ApplicationContext.ScanData = ApplicationContext.ScanData + ", Route Zone = " + routeZone;
						ApplicationContext.ScanData = ApplicationContext.ScanData + ", Error Code = " + errorCode;
						ApplicationContext.ScanData += "\r\nTransaction Time:";
						ApplicationContext.ScanData = ApplicationContext.ScanData + " " + elapsedTime;
						ApplicationContext.ScanData += "\r\n";
						OnPropertyChanged("ScanData");
						Metrics.LogMetricsPrint(logger, elapsedTime, trackingID, routeZone, routeCode);
						if (AutoTrigger)
						{
							ApplicationContext.CurrentDevice.Scanner.Start();
						}
						return;
					}
					catch (ApiException ex)
					{
						if (ex.ErrorCode != ErrorCode.Busy)
						{
							Messenger.ShowErrorMessage(ex);
						}
						return;
					}
					catch (Exception ex2)
					{
						Messenger.ShowMessage(ex2.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
						return;
					}
				}
				ApplicationContext.ScanData += "routeZone was not present in the print stream\r\n";
				OnPropertyChanged("ScanData");
				Metrics.LogMetricsError(logger, -1, labelData);
			}
			else
			{
				ApplicationContext.ScanData += "routeZone was not present in the print stream\r\n";
				OnPropertyChanged("ScanData");
				Metrics.LogMetricsError(logger, -1, labelData);
			}
		}
		else
		{
			ApplicationContext.ScanData += "routeCode was not present in the print stream\r\n";
			OnPropertyChanged("ScanData");
			Metrics.LogMetricsError(logger, -1, labelData);
		}
	}
}
