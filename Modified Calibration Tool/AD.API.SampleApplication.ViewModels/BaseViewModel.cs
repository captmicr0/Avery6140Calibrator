using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using AD.API.Core;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools;
using AD.API.Win8Desktop.Devices.Management;

namespace AD.API.SampleApplication.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
	public delegate void ConnectionLostHandled(object sender, ConnectionLostHandledEventArgs args);

	private bool isActionAvailable = true;

	private DeviceManager deviceManager;

	private Command navigatePreviousDeviceCommand;

	private Command navigateNextDeviceCommand;

	public bool IsActionAvailable
	{
		get
		{
			return isActionAvailable;
		}
		set
		{
			isActionAvailable = value;
			OnPropertyChanged("IsActionAvailable");
		}
	}

	public bool DeviceConnected => deviceManager.ConnectedDevices.FirstOrDefault((IDevice x) => x.Connection.State == ConnectionState.Open) != null;

	public string CurrentDeviceSerial
	{
		get
		{
			if (ApplicationContext.CurrentDevice != null)
			{
				return ApplicationContext.CurrentDevice.Serial;
			}
			return "Not Connected";
		}
	}

	public Command NavigatePreviousDeviceCommand => navigatePreviousDeviceCommand;

	public Command NavigateNextDeviceCommand => navigateNextDeviceCommand;

	public event PropertyChangedEventHandler PropertyChanged;

	protected event ConnectionLostHandled connectionLostHandledEvent;

	public event ConnectionLostHandled ConnectionLostHandledEvent
	{
		add
		{
			connectionLostHandledEvent += value;
		}
		remove
		{
			connectionLostHandledEvent -= value;
		}
	}

	protected BaseViewModel()
	{
		navigatePreviousDeviceCommand = new Command(NavigatePreviousDeviceMethod);
		navigateNextDeviceCommand = new Command(NavigateNextDeviceMethod);
		deviceManager = ApplicationContext.DeviceManager;
		HandleNavigationEnable();
	}

	public virtual void Finalization()
	{
	}

	public virtual void DeviceInitilization(IDevice device)
	{
	}

	public virtual void ValidateSelf()
	{
		if (ApplicationContext.CurrentDevice == null && ApplicationContext.DeviceManager.ConnectedDevices.Count <= 1)
		{
			MoveToNextDevice();
		}
		HandleNavigationEnable();
	}

	protected void OnPropertyChanged(string property)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}

	private void NavigatePreviousDeviceMethod()
	{
		MoveToPreviousDevice();
		HandleNavigationEnable();
		OnPropertyChanged(string.Empty);
	}

	private void MoveToPreviousDevice()
	{
		if (deviceManager == null || deviceManager.ConnectedDevices.Count <= 0)
		{
			return;
		}
		if (deviceManager.ConnectedDevices.Count == 1)
		{
			ApplicationContext.CurrentDeviceAddress = deviceManager.ConnectedDevices.First().Connection.Address;
			return;
		}
		int currentDeviceIndex = deviceManager.ConnectedDevices.IndexOf(ApplicationContext.CurrentDevice);
		if (currentDeviceIndex >= 1)
		{
			ApplicationContext.CurrentDeviceAddress = deviceManager.ConnectedDevices.ElementAt(currentDeviceIndex - 1).Connection.Address;
		}
		else if (currentDeviceIndex == -1)
		{
			ApplicationContext.CurrentDeviceAddress = deviceManager.ConnectedDevices.First().Connection.Address;
		}
	}

	private void NavigateNextDeviceMethod()
	{
		MoveToNextDevice();
		HandleNavigationEnable();
		OnPropertyChanged(string.Empty);
	}

	private void MoveToNextDevice()
	{
		if (deviceManager != null && deviceManager.ConnectedDevices.Count > 0)
		{
			if (deviceManager.ConnectedDevices.Count == 1)
			{
				ApplicationContext.CurrentDeviceAddress = deviceManager.ConnectedDevices.First().Connection.Address;
				return;
			}
			int currentDeviceIndex = deviceManager.ConnectedDevices.IndexOf(ApplicationContext.CurrentDevice);
			if (currentDeviceIndex < deviceManager.ConnectedDevices.Count - 1)
			{
				ApplicationContext.CurrentDeviceAddress = deviceManager.ConnectedDevices.ElementAt(currentDeviceIndex + 1).Connection.Address;
			}
			else if (currentDeviceIndex == -1)
			{
				ApplicationContext.CurrentDeviceAddress = deviceManager.ConnectedDevices.First().Connection.Address;
			}
		}
		else
		{
			ApplicationContext.CurrentDeviceAddress = string.Empty;
		}
	}

	private void HandleNavigationEnable()
	{
		Application.Current.Dispatcher.BeginInvoke((Action)delegate
		{
			navigateNextDeviceCommand.CanExecute = deviceManager != null && deviceManager.ConnectedDevices.Count > 1;
			navigatePreviousDeviceCommand.CanExecute = deviceManager != null && deviceManager.ConnectedDevices.Count != 0 && deviceManager.ConnectedDevices.IndexOf(ApplicationContext.CurrentDevice) != 0;
		});
	}

	protected void OnConnectionLostHandled(bool isViewModelValid)
	{
		if (this.connectionLostHandledEvent != null)
		{
			this.connectionLostHandledEvent(this, new ConnectionLostHandledEventArgs(isViewModelValid));
		}
	}
}
