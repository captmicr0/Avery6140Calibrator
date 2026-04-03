using AD.API.Core;
using AD.API.SampleApplication.Models;

namespace AD.API.SampleApplication.ViewModels.Barcodes;

internal class BaseBarcodeViewModel : BaseViewModel
{
	protected string EnteredDeviceAddress;

	public BaseBarcodeViewModel()
	{
		EnteredDeviceAddress = ApplicationContext.CurrentDeviceAddress;
	}

	public sealed override void DeviceInitilization(IDevice device)
	{
		base.DeviceInitilization(device);
		if (device != null && device.Connection.Address.Equals(EnteredDeviceAddress))
		{
			device.Scanner.LoadSettings();
			device.Scanner.BeginSetSession();
			InitializeBarcode(device);
		}
	}

	public override void ValidateSelf()
	{
		base.ValidateSelf();
		bool valid = ApplicationContext.DeviceManager.ConnectedDevices[EnteredDeviceAddress] != null;
		OnConnectionLostHandled(valid);
		if (valid)
		{
			OnPropertyChanged(string.Empty);
		}
	}

	protected virtual void InitializeBarcode(IDevice device)
	{
	}
}
