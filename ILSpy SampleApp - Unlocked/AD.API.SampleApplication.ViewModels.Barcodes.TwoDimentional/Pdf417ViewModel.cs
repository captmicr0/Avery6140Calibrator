using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.TwoDimentional;

internal class Pdf417ViewModel : BaseBarcodeViewModel
{
	private Pdf417 barcodePdf417;

	public bool Pdf417Enabled
	{
		get
		{
			return barcodePdf417.Enabled;
		}
		set
		{
			if (barcodePdf417.Enabled != value)
			{
				barcodePdf417.Enabled = value;
				OnPropertyChanged("Pdf417Enabled");
			}
		}
	}

	public bool MicroEnabled
	{
		get
		{
			return barcodePdf417.MicroEnabled;
		}
		set
		{
			if (barcodePdf417.MicroEnabled != value)
			{
				barcodePdf417.MicroEnabled = value;
				OnPropertyChanged("MicroEnabled");
			}
		}
	}

	public bool Code128Enabled
	{
		get
		{
			return barcodePdf417.Code128EmulationEnabled;
		}
		set
		{
			if (barcodePdf417.Code128EmulationEnabled != value)
			{
				barcodePdf417.Code128EmulationEnabled = value;
				OnPropertyChanged("Code128Enabled");
			}
		}
	}

	public Pdf417ViewModel()
	{
		barcodePdf417 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodePdf417;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodePdf417 = ((ScannerSe4500)device.Scanner).BarcodePdf417;
	}
}
