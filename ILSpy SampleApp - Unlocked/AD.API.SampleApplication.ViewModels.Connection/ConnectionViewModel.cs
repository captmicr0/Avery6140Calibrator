using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
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

	private ObservableCollection<DeviceConnectionItem> availableDevicesConnectionItems;

	private ConfigurationHandler configHandler = new ConfigurationHandler();

	public Command DiscoveryCommand => discoveryCommand;

	public bool DeviceNotConnected => !base.DeviceConnected;

	public ObservableCollection<DeviceConnectionItem> AvailableDevicesConnectionItems => availableDevicesConnectionItems ?? (availableDevicesConnectionItems = new ObservableCollection<DeviceConnectionItem>());

	public bool BluetoothConnection => ApplicationContext.DeviceManager.ConnectedDevices.FirstOrDefault((IDevice x) => x.Connection.Type == ConnectionType.Bluetooth && x.Connection.State == ConnectionState.Open) != null;

	public ConnectionViewModel()
	{
		discoveryCommand = new Command(DiscoverDevicesMethod);
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
						AddDeviceConnectionItem(current);
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
}
