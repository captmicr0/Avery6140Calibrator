using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class Code39ViewModel : BaseBarcodeViewModel
{
	private Code39 barcodeCode39;

	public bool Code39Enabled
	{
		get
		{
			return barcodeCode39.Enabled;
		}
		set
		{
			if (barcodeCode39.Enabled != value)
			{
				barcodeCode39.Enabled = value;
				OnPropertyChanged("Code39Enabled");
			}
		}
	}

	public bool PrefixAEnabled
	{
		get
		{
			return barcodeCode39.PrefixAEnabled;
		}
		set
		{
			if (barcodeCode39.PrefixAEnabled != value)
			{
				barcodeCode39.PrefixAEnabled = value;
				OnPropertyChanged("PrefixAEnabled");
			}
		}
	}

	public bool FullAsciiEnabled
	{
		get
		{
			return barcodeCode39.FullAsciiEnabled;
		}
		set
		{
			if (barcodeCode39.FullAsciiEnabled != value)
			{
				barcodeCode39.FullAsciiEnabled = value;
				OnPropertyChanged("FullAsciiEnabled");
			}
		}
	}

	public bool TrioptricEnabled
	{
		get
		{
			return barcodeCode39.TriopticEnabled;
		}
		set
		{
			if (barcodeCode39.TriopticEnabled != value)
			{
				barcodeCode39.TriopticEnabled = value;
				OnPropertyChanged("TrioptricEnabled");
			}
		}
	}

	public bool IntegrityEnabled
	{
		get
		{
			return barcodeCode39.IntegrityCheckEnabled;
		}
		set
		{
			if (barcodeCode39.IntegrityCheckEnabled != value)
			{
				barcodeCode39.IntegrityCheckEnabled = value;
				OnPropertyChanged("IntegrityEnabled");
			}
		}
	}

	public bool ToCode32Enabled
	{
		get
		{
			return barcodeCode39.ConversionToCode32Enabled;
		}
		set
		{
			if (barcodeCode39.ConversionToCode32Enabled != value)
			{
				barcodeCode39.ConversionToCode32Enabled = value;
				OnPropertyChanged("ToCode32Enabled");
			}
		}
	}

	public bool DigitTransmissionEnabled
	{
		get
		{
			return barcodeCode39.CheckDigitsTransmissionEnabled;
		}
		set
		{
			if (barcodeCode39.CheckDigitsTransmissionEnabled != value)
			{
				barcodeCode39.CheckDigitsTransmissionEnabled = value;
				OnPropertyChanged("DigitTransmissionEnabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeCode39.Length1;
		}
		set
		{
			barcodeCode39.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeCode39.Length2;
		}
		set
		{
			barcodeCode39.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public Code39ViewModel()
	{
		barcodeCode39 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeCode39;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeCode39 = ((ScannerSe4500)device.Scanner).BarcodeCode39;
	}
}
