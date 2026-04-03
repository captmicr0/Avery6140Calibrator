using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
using AD.API.Core.Imports;
using AD.API.Devices.Management;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Controls.UserControls;
using AD.API.SampleApplication.Helpers;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;

namespace AD.API.SampleApplication.ViewModels.Connection;

internal class ConnectionViewModel : BaseViewModel
{
	private Command discoveryCommand;

    private Command resetAndBlackMarkCalCommand;

    private Command blackMarkCalibrationCommand;

	private Command resetCommand;

    private ObservableCollection<DeviceConnectionItem> availableDevicesConnectionItems;

	private ConfigurationHandler configHandler = new ConfigurationHandler();

    private bool only6140 = true;

    public bool Only6140
    {
        get => only6140;
        set
        {
            only6140 = value;
            OnPropertyChanged(nameof(Only6140));
        }
    }

    public Command DiscoveryCommand => discoveryCommand;

    public Command ResetAndBlackMarkCalCommand => resetAndBlackMarkCalCommand;

    public Command BlackMarkCalibrationCommand => blackMarkCalibrationCommand;

	public Command ResetCommand => resetCommand;

    public bool DeviceNotConnected => !base.DeviceConnected;

	public ObservableCollection<DeviceConnectionItem> AvailableDevicesConnectionItems => availableDevicesConnectionItems ?? (availableDevicesConnectionItems = new ObservableCollection<DeviceConnectionItem>());

	public bool BluetoothConnection => ApplicationContext.DeviceManager.ConnectedDevices.FirstOrDefault((IDevice x) => x.Connection.Type == ConnectionType.Bluetooth && x.Connection.State == ConnectionState.Open) != null;

	public ConnectionViewModel()
    {
        Only6140 = true;
        OnPropertyChanged(nameof(Only6140));
        discoveryCommand = new Command(DiscoverDevicesMethod);
        resetAndBlackMarkCalCommand = new Command(ResetAndBlackMarkCal);
        blackMarkCalibrationCommand = new Command(CalibrateBlackMarkSensor);
        resetCommand = new Command(ResetDevicesMethod);
        AddConnectedDevicesToElementCollection();
		DiscoverDevicesMethod();
	}

	public override void ValidateSelf()
	{
		base.ValidateSelf();
		Application.Current.Dispatcher.BeginInvoke((Action)delegate
		{
			foreach (DeviceConnectionItem current in AvailableDevicesConnectionItems)
			{
				IDevice device = ApplicationContext.DeviceManager.ConnectedDevices[current.ConnectionInfo.Address];
				current.IsConnected = device != null && device.Connection.State == ConnectionState.Open;
			}
		});
		OnPropertyChanged(string.Empty);
	}

	private void DiscoverDevicesMethod()
	{
		base.IsActionAvailable = false;
		if (availableDevicesConnectionItems != null)
		{
			availableDevicesConnectionItems.Clear();
		}
		DeviceConnectionInfosCollection availableDevices = null;
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate
		{
			try
			{
				AddConnectedDevicesToElementCollection();
				availableDevices = ApplicationContext.DeviceManager.GetAvailableDevices(ConnectionType.Bluetooth, new TimeSpan(5000L));
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
				if (availableDevices != null)
				{
					foreach (DeviceConnectionInfo current in availableDevices)
					{
                        if (!this.Only6140 || current.Name.StartsWith("6140"))
                        {
                            AddDeviceConnectionItem(current);
                        }
					}
				}
				SetConnectionActionItemsState();
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

	private void AddDeviceConnectionItem(DeviceConnectionInfo availableDevicesDeviceConnectionInfo)
	{
		if (!TestForDuplicates(availableDevicesDeviceConnectionInfo.Address))
		{
			DeviceConnectionItem deviceConnectionItem = new DeviceConnectionItem(availableDevicesDeviceConnectionInfo)
			{
				IsConnected = (ApplicationContext.DeviceManager.ConnectedDevices[availableDevicesDeviceConnectionInfo.Address] != null)
			};
			deviceConnectionItem.ActionButtonClick += OnActionButtonClick;
			AvailableDevicesConnectionItems.Add(deviceConnectionItem);
		}
	}

	private void AddConnectedDevicesToElementCollection()
	{
		Application.Current.Dispatcher.BeginInvoke((Action)delegate
		{
			foreach (IDevice current in ApplicationContext.DeviceManager.ConnectedDevices)
			{
				AddDeviceConnectionItem(new DeviceConnectionInfo($"{current.Serial}:({current.Connection.Address})", current.Connection.Type));
			}
		});
	}

	private bool TestForDuplicates(string deviceAddress)
	{
		foreach (DeviceConnectionItem availableDevicesConnectionItem in AvailableDevicesConnectionItems)
		{
			DeviceConnectionItem deviceItem;
			if ((deviceItem = availableDevicesConnectionItem) != null && string.Equals(deviceItem.ConnectionInfo.Address, deviceAddress, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private void OnActionButtonClick(object sender, EventArgs e)
	{
		base.IsActionAvailable = false;
		DeviceConnectionItem control = sender as DeviceConnectionItem;
		if (control == null)
		{
			return;
		}
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate
		{
			try
			{
				if (control != null)
				{
					IDevice device = ApplicationContext.DeviceManager.ConnectedDevices[control.ConnectionInfo.Address];
					if (device != null && device.Connection.State == ConnectionState.Open)
					{
						ApplicationContext.Disconnect(device);
						Application.Current.Dispatcher.Invoke(delegate
						{
							control.IsConnected = false;
						});
					}
					else if (ApplicationContext.DeviceManager.ConnectedDevices.Count < ApplicationContext.maxDeviceCount)
					{
						ApplicationContext.Connect(control.ConnectionInfo);
						Application.Current.Dispatcher.Invoke(delegate
						{
							control.IsConnected = true;
						});
					}
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
			SetConnectionActionItemsState();
			base.IsActionAvailable = true;
			OnPropertyChanged(string.Empty);
		};
		backgroundWorker.RunWorkerAsync();
	}

	private void SetConnectionActionItemsState()
	{
		bool fEnabled = true;
		fEnabled = ((ApplicationContext.DeviceManager.ConnectedDevices.Count < ApplicationContext.maxDeviceCount) ? true : false);
		foreach (DeviceConnectionItem control in AvailableDevicesConnectionItems)
		{
			DeviceConnectionItem deviceItem = control;
			if (control != null && !control.IsConnected)
			{
				deviceItem.IsEnabled = fEnabled;
			}
		}
    }

	public override void Finalization()
	{
		base.Finalization();
		base.IsActionAvailable = false;
		OnPropertyChanged(string.Empty);
		Console.WriteLine("Applying Device configurations to connected devices");
		foreach (IDevice connectedDevice in ApplicationContext.DeviceManager.ConnectedDevices)
		{
			configHandler.applyPrinterConfig(ApplicationContext.ConfigData, connectedDevice);
			configHandler.applyScannerConfig(ApplicationContext.ConfigData, connectedDevice);
		}
		base.IsActionAvailable = true;
		OnPropertyChanged(string.Empty);
	}

	private void BlackMarkCalAndDisconnect()
	{
        base.IsActionAvailable = false;
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += delegate
        {
            try
            {
                if (ApplicationContext.DeviceManager.ConnectedDevices.Count < 1)
                {
                    throw new Exception("No devices conneted.");
                }

                Console.WriteLine("Calibrating black mark sensor on all connected devices");

                foreach (IDevice current in ApplicationContext.DeviceManager.ConnectedDevices)
                {
                    current.Printer.CalibrateBlackMark();
                    current.Printer.FeedLabel();
                    ApplicationContext.Disconnect(current);
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
                ValidateSelf();
                SetConnectionActionItemsState();
                base.IsActionAvailable = true;
                OnPropertyChanged(string.Empty);
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

    private void ResetAndBlackMarkCal()
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

                Console.WriteLine("Calibrating black mark sensor on all connected devices");

                foreach (IDevice current in ApplicationContext.DeviceManager.ConnectedDevices)
                {
                    current.Printer.CalibrateBlackMark();
                    current.Printer.FeedLabel();
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


    private void CalibrateBlackMarkSensor()
    {
        base.IsActionAvailable = false;
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += delegate
        {
            try
            {
                if (ApplicationContext.DeviceManager.ConnectedDevices.Count < 1)
                {
                    throw new Exception("No devices conneted.");
                }

                Console.WriteLine("Calibrating black mark sensor on all connected devices");

                foreach (IDevice current in ApplicationContext.DeviceManager.ConnectedDevices)
                {
                    current.Printer.CalibrateBlackMark();
                    current.Printer.FeedLabel();
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
}
