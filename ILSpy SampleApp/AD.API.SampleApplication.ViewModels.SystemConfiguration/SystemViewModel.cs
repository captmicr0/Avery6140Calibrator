using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
using AD.API.Core.Imports;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;

namespace AD.API.SampleApplication.ViewModels.SystemConfiguration;

internal class SystemViewModel : BaseViewModel
{
	public enum AvailableSounds
	{
		ScanGood,
		Scan_Bad,
		LookPass,
		LookFail
	}

	private string enteredDeviceAddress;

	private Command playSoundCommand;

	private Command resetToDefaultCommand;

	private AvailableSounds selectedSound;

	private int selectedRepeatCount;

	public Command PlaySoundCommand => playSoundCommand;

	public Command ResetToDefaultCommand => resetToDefaultCommand;

	public double DeviceVolume
	{
		get
		{
			return ApplicationContext.CurrentDevice.SoundVolume;
		}
		set
		{
			try
			{
				ApplicationContext.CurrentDevice.SoundVolume = Convert.ToInt32(value);
				OnPropertyChanged("DeviceVolume");
			}
			catch (ApiException ex)
			{
				ApiException ex2 = ex;
				ApiException apiException = ex2;
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					Messenger.ShowErrorMessage(apiException);
				});
				throw new Exception();
			}
			catch (Exception ex3)
			{
				Exception ex4 = ex3;
				Exception exception = ex4;
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					Messenger.ShowMessage(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				});
				throw new Exception();
			}
		}
	}

	public string DeviceModel => ApplicationContext.CurrentDevice.ModelName;

	public string DeviceStatus => ((int)ApplicationContext.CurrentDevice.Status).ToString(CultureInfo.InvariantCulture);

	public int DeviceBatteryLevel => ApplicationContext.CurrentDevice.BatteryLevel;

	public double DeviceBatteryVoltage => (double)ApplicationContext.CurrentDevice.BatteryVoltage / 100.0;

	public string DeviceFirmwareVersion => ApplicationContext.CurrentDevice.ModelVersion;

	public string CdilVersion => ApplicationContext.CurrentDevice.CdilVersion;

	public string DeviceComponents => ApplicationContext.CurrentDevice.Components;

	public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	public int FeedButtonOnOff
	{
		get
		{
			if (ApplicationContext.CurrentDevice.FeedMode != FeedButtonMode.Forward)
			{
				return 1;
			}
			return 0;
		}
		set
		{
			try
			{
				ApplicationContext.CurrentDevice.FeedMode = ((value == 0) ? FeedButtonMode.Forward : FeedButtonMode.Feed);
			}
			catch (ApiException ex)
			{
				ApiException ex2 = ex;
				ApiException apiException = ex2;
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					Messenger.ShowErrorMessage(apiException);
				});
				throw new Exception();
			}
			catch (Exception ex3)
			{
				Exception ex4 = ex3;
				Exception exception = ex4;
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					Messenger.ShowMessage(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				});
				throw new Exception();
			}
		}
	}

	public Array TriggerModeValues => Enum.GetValues(typeof(TriggerMode));

	public TriggerMode SelectedTriggerMode
	{
		get
		{
			return ApplicationContext.CurrentDevice.TriggerMode;
		}
		set
		{
			try
			{
				ApplicationContext.CurrentDevice.TriggerMode = value;
			}
			catch (ApiException ex)
			{
				ApiException ex2 = ex;
				ApiException apiException = ex2;
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					Messenger.ShowErrorMessage(apiException);
				});
				throw new Exception();
			}
			catch (Exception ex3)
			{
				Exception ex4 = ex3;
				Exception exception = ex4;
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					Messenger.ShowMessage(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				});
				throw new Exception();
			}
		}
	}

	public Array AvailableSoundsArray => Enum.GetValues(typeof(AvailableSounds));

	public AvailableSounds SelectedAvailableSound
	{
		get
		{
			return selectedSound;
		}
		set
		{
			selectedSound = value;
		}
	}

	public int SelectedRepeatCount
	{
		private get
		{
			return selectedRepeatCount;
		}
		set
		{
			selectedRepeatCount = value;
			OnPropertyChanged("SelectedRepeatCount");
		}
	}

	public SystemViewModel()
	{
		playSoundCommand = new Command(PlaySoundCommandMethod);
		resetToDefaultCommand = new Command(ResetToDefaultMethod);
		selectedSound = AvailableSounds.ScanGood;
		selectedRepeatCount = 1;
		enteredDeviceAddress = ApplicationContext.CurrentDeviceAddress;
		DeviceInitilization(ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress]);
	}

	private void PlaySoundCommandMethod()
	{
		try
		{
			ApplicationContext.CurrentDevice.PlaySound(SelectedAvailableSound.ToString(), (uint)SelectedRepeatCount);
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

	private void ResetToDefaultMethod()
	{
		if (Messenger.ShowMessage("Are you sure you want to reset all device settings?", string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, base.CurrentDeviceSerial, MessageSynchronizer.RESET_PARAMETERS_TITLE), MessageBoxButton.YesNo, MessageBoxImage.Question))
		{
			try
			{
				ApplicationContext.CurrentDevice.ResetConfiguration();
				OnPropertyChanged(string.Empty);
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

	public sealed override void DeviceInitilization(IDevice device)
	{
		base.DeviceInitilization(device);
		if (device != null && device.Connection.Address.Equals(enteredDeviceAddress))
		{
			device.LoadSettings();
			device.TriggerPressed += DeviceOnTriggerPressed;
		}
	}

	public override void Finalization()
	{
		base.Finalization();
		if (ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress] != null)
		{
			try
			{
				ApplicationContext.DeviceManager.ConnectedDevices[enteredDeviceAddress].TriggerPressed -= DeviceOnTriggerPressed;
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

	private void DeviceOnTriggerPressed(object sender, ApiButtonEventArgs args)
	{
		IDevice senderDevice = sender as IDevice;
		if (senderDevice != null)
		{
			new Thread((ThreadStart)delegate
			{
				string format = (args.StatePressed ? "{0} is pressed." : "{0} is released.");
				string text = ((args.Button == 0) ? MessageSynchronizer.TRIGGER_BUTTON_TITLE : ((args.Button != 1) ? MessageSynchronizer.UNKNOWN_BUTTON_TITLE : MessageSynchronizer.FEED_BUTTON_TITLE));
				string format2 = string.Format(format, text);
				Messenger.ShowMessage(caption: string.Format(MessageSynchronizer.MESSAGE_FORMAT_STRING, senderDevice.Serial, text), message: string.Format(format2), buttons: MessageBoxButton.OK, image: MessageBoxImage.Exclamation);
			}).Start();
		}
	}
}
