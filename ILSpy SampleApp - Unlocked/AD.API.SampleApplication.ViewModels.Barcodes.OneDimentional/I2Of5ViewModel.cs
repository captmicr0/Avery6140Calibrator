using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class I2Of5ViewModel : BaseBarcodeViewModel
{
	private InterleavedCode25 barcodeInterleavedCode25;

	public bool I2Of5Enabled
	{
		get
		{
			return barcodeInterleavedCode25.Enabled;
		}
		set
		{
			if (barcodeInterleavedCode25.Enabled != value)
			{
				barcodeInterleavedCode25.Enabled = value;
				OnPropertyChanged("I2Of5Enabled");
			}
		}
	}

	public bool IntergrityCheckEnabled
	{
		get
		{
			return barcodeInterleavedCode25.IntegrityCheckEnabled;
		}
		set
		{
			if (barcodeInterleavedCode25.IntegrityCheckEnabled != value)
			{
				barcodeInterleavedCode25.IntegrityCheckEnabled = value;
				OnPropertyChanged("IntergrityCheckEnabled");
			}
		}
	}

	public bool ToEan13Enabled
	{
		get
		{
			return barcodeInterleavedCode25.ConversionToEan13Enabled;
		}
		set
		{
			if (barcodeInterleavedCode25.ConversionToEan13Enabled != value)
			{
				barcodeInterleavedCode25.ConversionToEan13Enabled = value;
				OnPropertyChanged("ToEan13Enabled");
			}
		}
	}

	public bool DigitsTransmissionEnabled
	{
		get
		{
			return barcodeInterleavedCode25.CheckDigitsTransmissionEnabled;
		}
		set
		{
			if (barcodeInterleavedCode25.CheckDigitsTransmissionEnabled != value)
			{
				barcodeInterleavedCode25.CheckDigitsTransmissionEnabled = value;
				OnPropertyChanged("DigitsTransmissionEnabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeInterleavedCode25.Length1;
		}
		set
		{
			barcodeInterleavedCode25.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeInterleavedCode25.Length2;
		}
		set
		{
			barcodeInterleavedCode25.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public I2Of5ViewModel()
	{
		barcodeInterleavedCode25 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeI2Of5;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeInterleavedCode25 = ((ScannerSe4500)device.Scanner).BarcodeI2Of5;
	}
}
