using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class Gs1ViewModel : BaseBarcodeViewModel
{
	private Gs1 barcodeGs1;

	public bool Gs1Enabled
	{
		get
		{
			return barcodeGs1.Enabled;
		}
		set
		{
			if (barcodeGs1.Enabled != value)
			{
				barcodeGs1.Enabled = value;
				OnPropertyChanged("Gs1Enabled");
			}
		}
	}

	public bool UpcEanConversionEnabled
	{
		get
		{
			return barcodeGs1.UpcEanConversionEnabled;
		}
		set
		{
			if (barcodeGs1.UpcEanConversionEnabled != value)
			{
				barcodeGs1.UpcEanConversionEnabled = value;
				OnPropertyChanged("UpcEanConversionEnabled");
			}
		}
	}

	public Gs1ViewModel()
	{
		barcodeGs1 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeGs1;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeGs1 = ((ScannerSe4500)device.Scanner).BarcodeGs1;
	}
}
