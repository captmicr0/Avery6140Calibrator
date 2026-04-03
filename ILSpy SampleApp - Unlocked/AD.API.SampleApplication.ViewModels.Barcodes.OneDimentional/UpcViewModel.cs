using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class UpcViewModel : BaseBarcodeViewModel
{
	private Upc barcodeUpc;

	public bool UpcEnabled
	{
		get
		{
			return barcodeUpc.Enabled;
		}
		set
		{
			if (barcodeUpc.Enabled != value)
			{
				barcodeUpc.Enabled = value;
				OnPropertyChanged("UpcEnabled");
			}
		}
	}

	public bool UpceEnabled
	{
		get
		{
			return barcodeUpc.UpceEnabled;
		}
		set
		{
			if (barcodeUpc.UpceEnabled != value)
			{
				barcodeUpc.UpceEnabled = value;
				OnPropertyChanged("UpceEnabled");
			}
		}
	}

	public bool Upce1Enabled
	{
		get
		{
			return barcodeUpc.Upce1Enabled;
		}
		set
		{
			if (barcodeUpc.Upce1Enabled != value)
			{
				barcodeUpc.Upce1Enabled = value;
				OnPropertyChanged("Upce1Enabled");
			}
		}
	}

	public bool Ean8Enabled
	{
		get
		{
			return barcodeUpc.Ean8Enabled;
		}
		set
		{
			if (barcodeUpc.Ean8Enabled != value)
			{
				barcodeUpc.Ean8Enabled = value;
				OnPropertyChanged("Ean8Enabled");
			}
		}
	}

	public bool Ean13Enabled
	{
		get
		{
			return barcodeUpc.Ean13Enabled;
		}
		set
		{
			if (barcodeUpc.Ean13Enabled != value)
			{
				barcodeUpc.Ean13Enabled = value;
				OnPropertyChanged("Ean13Enabled");
			}
		}
	}

	public bool EanBooklandEnabled
	{
		get
		{
			return barcodeUpc.EanBooklandEnabled;
		}
		set
		{
			if (barcodeUpc.EanBooklandEnabled != value)
			{
				barcodeUpc.EanBooklandEnabled = value;
				OnPropertyChanged("EanBooklandEnabled");
			}
		}
	}

	public double SupplementalMode
	{
		get
		{
			return barcodeUpc.SupplementalMode;
		}
		set
		{
			barcodeUpc.SupplementalMode = (int)value;
			OnPropertyChanged("SupplementalMode");
		}
	}

	public double SupplementalRedundancy
	{
		get
		{
			return barcodeUpc.SupplementalRedundancy;
		}
		set
		{
			barcodeUpc.SupplementalRedundancy = (int)value;
			OnPropertyChanged("SupplementalRedundancy");
		}
	}

	public bool UpcaDigitsTransmissionEnabled
	{
		get
		{
			return barcodeUpc.UpcaCheckDigitsTransmissionEnabled;
		}
		set
		{
			if (barcodeUpc.UpcaCheckDigitsTransmissionEnabled != value)
			{
				barcodeUpc.UpcaCheckDigitsTransmissionEnabled = value;
				OnPropertyChanged("UpcaDigitsTransmissionEnabled");
			}
		}
	}

	public bool UpceDigitsTransmissionEnabled
	{
		get
		{
			return barcodeUpc.UpceCheckDigitsTransmissionEnabled;
		}
		set
		{
			if (barcodeUpc.UpceCheckDigitsTransmissionEnabled != value)
			{
				barcodeUpc.UpceCheckDigitsTransmissionEnabled = value;
				OnPropertyChanged("UpceDigitsTransmissionEnabled");
			}
		}
	}

	public bool Upce1DigitsTransmissionEnabled
	{
		get
		{
			return barcodeUpc.Upce1CheckDigitsTransmissionEnabled;
		}
		set
		{
			if (barcodeUpc.Upce1CheckDigitsTransmissionEnabled != value)
			{
				barcodeUpc.Upce1CheckDigitsTransmissionEnabled = value;
				OnPropertyChanged("Upce1DigitsTransmissionEnabled");
			}
		}
	}

	public bool CouponCodeEnabled
	{
		get
		{
			return barcodeUpc.CouponCodeEnabled;
		}
		set
		{
			if (barcodeUpc.CouponCodeEnabled != value)
			{
				barcodeUpc.CouponCodeEnabled = value;
				OnPropertyChanged("CouponCodeEnabled");
			}
		}
	}

	public bool UpceToUpcaEnabled
	{
		get
		{
			return barcodeUpc.UpceToUpcaConversionEnabled;
		}
		set
		{
			if (barcodeUpc.UpceToUpcaConversionEnabled != value)
			{
				barcodeUpc.UpceToUpcaConversionEnabled = value;
				OnPropertyChanged("UpceToUpcaEnabled");
			}
		}
	}

	public bool Upce1ToUpcaEnabled
	{
		get
		{
			return barcodeUpc.Upce1ToUpcaConversionEnabled;
		}
		set
		{
			if (barcodeUpc.Upce1ToUpcaConversionEnabled != value)
			{
				barcodeUpc.Upce1ToUpcaConversionEnabled = value;
				OnPropertyChanged("Upce1ToUpcaEnabled");
			}
		}
	}

	public bool Ean8ZerosEnabled
	{
		get
		{
			return barcodeUpc.Ean8ZerosExtensionEnabled;
		}
		set
		{
			if (barcodeUpc.Ean8ZerosExtensionEnabled != value)
			{
				barcodeUpc.Ean8ZerosExtensionEnabled = value;
				OnPropertyChanged("Ean8ZerosEnabled");
			}
		}
	}

	public bool Ean8ToEan13Enabled
	{
		get
		{
			return barcodeUpc.Ean8ToEan13ConversionEnabled;
		}
		set
		{
			if (barcodeUpc.Ean8ToEan13ConversionEnabled != value)
			{
				barcodeUpc.Ean8ToEan13ConversionEnabled = value;
				OnPropertyChanged("Ean8ToEan13Enabled");
			}
		}
	}

	public UpcPreambleTransmissionMode UpcaPreamble
	{
		get
		{
			return barcodeUpc.UpcaPreambleMode;
		}
		set
		{
			if (barcodeUpc.UpcaPreambleMode != value)
			{
				barcodeUpc.UpcaPreambleMode = value;
				OnPropertyChanged("UpcaPreamble");
			}
		}
	}

	public UpcPreambleTransmissionMode UpcePreamble
	{
		get
		{
			return barcodeUpc.UpcePreambleMode;
		}
		set
		{
			if (barcodeUpc.UpcePreambleMode != value)
			{
				barcodeUpc.UpcePreambleMode = value;
				OnPropertyChanged("UpcePreamble");
			}
		}
	}

	public UpcPreambleTransmissionMode Upce1Preamble
	{
		get
		{
			return barcodeUpc.Upce1PreambleMode;
		}
		set
		{
			if (barcodeUpc.Upce1PreambleMode != value)
			{
				barcodeUpc.Upce1PreambleMode = value;
				OnPropertyChanged("Upce1Preamble");
			}
		}
	}

	public UpcViewModel()
	{
		barcodeUpc = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeUpc;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeUpc = ((ScannerSe4500)device.Scanner).BarcodeUpc;
	}
}
