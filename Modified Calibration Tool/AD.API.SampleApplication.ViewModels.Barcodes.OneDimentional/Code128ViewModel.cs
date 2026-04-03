using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class Code128ViewModel : BaseBarcodeViewModel
{
	private Code128 barcodeCode128;

	public bool Code128Enabled
	{
		get
		{
			return barcodeCode128.Enabled;
		}
		set
		{
			if (barcodeCode128.Enabled != value)
			{
				barcodeCode128.Enabled = value;
				OnPropertyChanged("Code128Enabled");
			}
		}
	}

	public bool IsbtEnabled
	{
		get
		{
			return barcodeCode128.Isbt128Enabled;
		}
		set
		{
			if (barcodeCode128.Isbt128Enabled != value)
			{
				barcodeCode128.Isbt128Enabled = value;
				OnPropertyChanged("IsbtEnabled");
			}
		}
	}

	public bool UccEanEnabled
	{
		get
		{
			return barcodeCode128.UccEan128Enabled;
		}
		set
		{
			if (barcodeCode128.UccEan128Enabled != value)
			{
				barcodeCode128.UccEan128Enabled = value;
				OnPropertyChanged("UccEanEnabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeCode128.Length1;
		}
		set
		{
			barcodeCode128.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeCode128.Length2;
		}
		set
		{
			barcodeCode128.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public Code128ViewModel()
	{
		barcodeCode128 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeCode128;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeCode128 = ((ScannerSe4500)device.Scanner).BarcodeCode128;
	}
}
