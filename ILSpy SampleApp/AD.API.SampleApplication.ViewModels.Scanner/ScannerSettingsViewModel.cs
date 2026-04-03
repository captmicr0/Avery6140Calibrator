using System;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Scanner;

internal class ScannerSettingsViewModel : BaseViewModel
{
	private string enteredDeviceAddress;

	private Command resetToDefaultCommand;

	public Command ResetToDefaultCommand => resetToDefaultCommand;

	public double ScannerTimeout
	{
		get
		{
			return ApplicationContext.CurrentDevice.Scanner.DecodeSessionTimeout;
		}
		set
		{
			ApplicationContext.CurrentDevice.Scanner.DecodeSessionTimeout = (int)value;
			OnPropertyChanged("ScannerTimeout");
		}
	}

	public double ScannerAimDuration
	{
		get
		{
			return ApplicationContext.CurrentDevice.Scanner.AimDuration;
		}
		set
		{
			ApplicationContext.CurrentDevice.Scanner.AimDuration = (int)value;
			OnPropertyChanged("ScannerAimDuration");
		}
	}

	public double ScannerBiDirRedun
	{
		get
		{
			return ApplicationContext.CurrentDevice.Scanner.BiDirectionalRedundancy;
		}
		set
		{
			ApplicationContext.CurrentDevice.Scanner.BiDirectionalRedundancy = (int)value;
			OnPropertyChanged("ScannerBiDirRedun");
		}
	}

	public ScanMode ScannerMode
	{
		get
		{
			return ApplicationContext.CurrentDevice.Scanner.Mode;
		}
		set
		{
			ApplicationContext.CurrentDevice.Scanner.Mode = value;
			OnPropertyChanged("ScannerMode");
		}
	}

	public ScannerLinearSecurityLevel ScannerLinearSecurity
	{
		get
		{
			return ApplicationContext.CurrentDevice.Scanner.LinearSecurity;
		}
		set
		{
			ApplicationContext.CurrentDevice.Scanner.LinearSecurity = value;
			OnPropertyChanged("ScannerLinearSecurity");
		}
	}

	public int SelectiveScanOnOff
	{
		get
		{
			if (!((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).PickListModeEnabled)
			{
				return 1;
			}
			return 0;
		}
		set
		{
			((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).PickListModeEnabled = value == 0;
			OnPropertyChanged("SelectiveScanOnOff");
		}
	}

	public ScannerSettingsViewModel()
	{
		resetToDefaultCommand = new Command(ResetToDefaultMethod);
		enteredDeviceAddress = ApplicationContext.CurrentDeviceAddress;
		DeviceInitilization(ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress]);
	}

	public sealed override void DeviceInitilization(IDevice device)
	{
		base.DeviceInitilization(device);
		if (device != null && device.Connection.Address.Equals(enteredDeviceAddress))
		{
			device.Scanner.LoadBaseSettings();
			device.Scanner.BeginSetSession();
		}
	}

	public override void Finalization()
	{
		base.Finalization();
		if (ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress] != null)
		{
			ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress].Scanner.EndSetSession();
			ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress].Scanner.UnloadSettings();
		}
	}

	public override void ValidateSelf()
	{
		base.ValidateSelf();
		bool valid = ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress] != null;
		OnConnectionLostHandled(valid);
		if (valid)
		{
			OnPropertyChanged(string.Empty);
		}
	}

	private void ResetToDefaultMethod()
	{
		if (Messenger.ShowMessage("Are you sure you want to reset all scanner settings?", string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, base.CurrentDeviceSerial, MessageSynchronizer.RESET_PARAMETERS_TITLE), MessageBoxButton.YesNo, MessageBoxImage.Question))
		{
			try
			{
				ApplicationContext.CurrentDevice.Scanner.ResetConfiguration();
				OnPropertyChanged(string.Empty);
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
