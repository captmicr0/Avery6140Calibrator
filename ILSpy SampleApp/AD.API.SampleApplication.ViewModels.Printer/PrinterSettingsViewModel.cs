using System;
using System.Collections.Generic;
using System.Windows;
using AD.API.Core;
using AD.API.Core.BaseImplementation;
using AD.API.Core.Exceptions;
using AD.API.Core.Imports;
using AD.API.Printers.AveryDennison.Pr6140;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;

namespace AD.API.SampleApplication.ViewModels.Printer;

internal class PrinterSettingsViewModel : BaseViewModel
{
	private string enteredDeviceAddress;

	private List<int> availableSpeedValuesList;

	private int selectedSpeedIndex;

	private Command resetToDefaultCommand;

	private Command decreaseSpeedCommand;

	private Command increaseSpeedCommand;

	public Command ResetToDefaultCommand => resetToDefaultCommand;

	public Command DecreaseSpeedCommand => decreaseSpeedCommand;

	public Command IncreaseSpeedCommand => increaseSpeedCommand;

	public string PrinterVersion => ApplicationContext.CurrentDevice.Printer.ModelVersion;

	public double PrinterContrast
	{
		get
		{
			return ApplicationContext.CurrentDevice.Printer.Contrast;
		}
		set
		{
			ApplicationContext.CurrentDevice.Printer.Contrast = (int)value;
			OnPropertyChanged("PrinterContrast");
		}
	}

	public double PrinterSupplyPossition
	{
		get
		{
			return ApplicationContext.CurrentDevice.Printer.SupplyPosition;
		}
		set
		{
			ApplicationContext.CurrentDevice.Printer.SupplyPosition = (int)value;
			OnPropertyChanged("PrinterSupplyPossition");
		}
	}

	public double PrinterMaxLength
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).MaximumLabelLength;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).MaximumLabelLength = (int)value;
			OnPropertyChanged("PrinterMaxLength");
		}
	}

	public double PrinterVerticalAdjust
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).VerticalAdjust;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).VerticalAdjust = (int)value;
			OnPropertyChanged("PrinterVerticalAdjust");
		}
	}

	public double PrinterHorizontalAdjust
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).HorizontalAdjust;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).HorizontalAdjust = (int)value;
			OnPropertyChanged("PrinterHorizontalAdjust");
		}
	}

	public double PrinterBackDistance
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).BackFeedDistance;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).BackFeedDistance = (int)value;
			OnPropertyChanged("PrinterBackDistance");
		}
	}

	public double PrinterOverFeedDistance
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).OverFeedDistance;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).OverFeedDistance = (int)value;
			OnPropertyChanged("PrinterOverFeedDistance");
		}
	}

	public string PrinterStatus => Convert.ToString((int)ApplicationContext.CurrentDevice.Printer.Status);

	public SupplyType PrinterSupplyType
	{
		get
		{
			return ApplicationContext.CurrentDevice.Printer.SupplyType;
		}
		set
		{
			ApplicationContext.CurrentDevice.Printer.SupplyType = value;
			OnPropertyChanged("PrinterSupplyType");
		}
	}

	public EnergyType PrinterEnergyType
	{
		get
		{
			return ApplicationContext.CurrentDevice.Printer.EnergyType;
		}
		set
		{
			ApplicationContext.CurrentDevice.Printer.EnergyType = value;
			OnPropertyChanged("PrinterEnergyType");
		}
	}

	public bool PrinterBackFeedMode
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).BackFeedEnabled;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).BackFeedEnabled = value;
			OnPropertyChanged("PrinterBackFeedMode");
		}
	}

	public bool PrinterNoneIndexLineMode
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).NoneIndexLineModeEnabled;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).NoneIndexLineModeEnabled = value;
			OnPropertyChanged("PrinterNoneIndexLineMode");
		}
	}

	public bool PrinterOnBlackMark => ((Printer6140)ApplicationContext.CurrentDevice.Printer).OnBlackMark;

	public bool PrinterOverFeed
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).OverFeedEnabled;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).OverFeedEnabled = value;
			OnPropertyChanged("PrinterOverFeed");
		}
	}

	public int PrinterSpeed
	{
		get
		{
			return ((Printer6140)ApplicationContext.CurrentDevice.Printer).Speed;
		}
		set
		{
			((Printer6140)ApplicationContext.CurrentDevice.Printer).Speed = value;
			OnPropertyChanged("PrinterSpeed");
		}
	}

	public PrinterSettingsViewModel()
	{
		resetToDefaultCommand = new Command(ResetToDefaultMethod);
		decreaseSpeedCommand = new Command(DecreaseSpeedMethod);
		increaseSpeedCommand = new Command(IncreaseSpeedMethod);
		enteredDeviceAddress = ApplicationContext.CurrentDeviceAddress;
		DeviceInitilization(ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress]);
		availableSpeedValuesList = new List<int> { 0, 15, 20, 30, 40, 50 };
		selectedSpeedIndex = availableSpeedValuesList.IndexOf(((Printer6140)ApplicationContext.CurrentDevice.Printer).Speed);
		SetIncreaseDecreaseCanExecute();
	}

	public sealed override void DeviceInitilization(IDevice device)
	{
		base.DeviceInitilization(device);
		if (device != null && device.Connection.Address.Equals(enteredDeviceAddress))
		{
			device.Printer.LoadSettings();
			device.Printer.BeginSetSession();
		}
	}

	public override void Finalization()
	{
		base.Finalization();
		if (ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress] != null)
		{
			try
			{
				ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress].Printer.EndSetSession();
			}
			catch (ApiException ex)
			{
				UnloadPritnerSettings();
				Messenger.ShowErrorMessage(ex);
			}
			catch (Exception ex2)
			{
				UnloadPritnerSettings();
				Messenger.ShowMessage(ex2.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
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
		if (Messenger.ShowMessage("Are you sure you want to reset all printer settings?", string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, base.CurrentDeviceSerial, MessageSynchronizer.RESET_PARAMETERS_TITLE), MessageBoxButton.YesNo, MessageBoxImage.Question))
		{
			try
			{
				PrinterContrast = 0.0;
				PrinterEnergyType = EnergyType.Standard;
				PrinterSupplyPossition = 0.0;
				PrinterMaxLength = 8.0;
				PrinterNoneIndexLineMode = false;
				PrinterHorizontalAdjust = 0.0;
				PrinterVerticalAdjust = 0.0;
				PrinterSpeed = 0;
				PrinterSupplyType = SupplyType.BlackMark;
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

	private void DecreaseSpeedMethod()
	{
		if (selectedSpeedIndex > 0)
		{
			selectedSpeedIndex--;
			PrinterSpeed = availableSpeedValuesList[selectedSpeedIndex];
		}
		SetIncreaseDecreaseCanExecute();
	}

	private void IncreaseSpeedMethod()
	{
		if (selectedSpeedIndex < availableSpeedValuesList.Count - 1)
		{
			selectedSpeedIndex++;
			PrinterSpeed = availableSpeedValuesList[selectedSpeedIndex];
		}
		SetIncreaseDecreaseCanExecute();
	}

	private void SetIncreaseDecreaseCanExecute()
	{
		decreaseSpeedCommand.CanExecute = selectedSpeedIndex > 0;
		increaseSpeedCommand.CanExecute = selectedSpeedIndex < availableSpeedValuesList.Count - 1;
	}

	private void UnloadPritnerSettings()
	{
		if (ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress] != null)
		{
			((BasePrinter)ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress].Printer).UnloadSettings();
		}
	}
}
