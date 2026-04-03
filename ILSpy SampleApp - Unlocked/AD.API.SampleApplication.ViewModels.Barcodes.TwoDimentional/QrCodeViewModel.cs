using System;
using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.TwoDimentional;

internal class QrCodeViewModel : BaseBarcodeViewModel
{
	private QrCode barcodeQrCode;

	public bool QrCodeEnabled
	{
		get
		{
			return barcodeQrCode.Enabled;
		}
		set
		{
			if (barcodeQrCode.Enabled != value)
			{
				barcodeQrCode.Enabled = value;
				OnPropertyChanged("QrCodeEnabled");
			}
		}
	}

	public bool MicroEnabled
	{
		get
		{
			return barcodeQrCode.MicroEnabled;
		}
		set
		{
			if (barcodeQrCode.MicroEnabled != value)
			{
				barcodeQrCode.MicroEnabled = value;
				OnPropertyChanged("MicroEnabled");
			}
		}
	}

	public Array InverseScanningModes => Enum.GetValues(typeof(InverseScanningMode));

	public InverseScanningMode InverseScanning
	{
		get
		{
			return barcodeQrCode.InverseMode;
		}
		set
		{
			if (barcodeQrCode.InverseMode != value)
			{
				barcodeQrCode.InverseMode = value;
				OnPropertyChanged("InverseScanning");
			}
		}
	}

	public QrCodeViewModel()
	{
		barcodeQrCode = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeQrCode;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeQrCode = ((ScannerSe4500)device.Scanner).BarcodeQrCode;
	}
}
