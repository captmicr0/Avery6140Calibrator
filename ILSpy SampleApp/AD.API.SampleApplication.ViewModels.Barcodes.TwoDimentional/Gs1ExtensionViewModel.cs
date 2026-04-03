using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.TwoDimentional;

internal class Gs1ExtensionViewModel : BaseBarcodeViewModel
{
	private Gs1Extension barcodeGs1Extension;

	public bool CompositeCcAbEnabled
	{
		get
		{
			return barcodeGs1Extension.CompositeCcAbEnabled;
		}
		set
		{
			if (barcodeGs1Extension.CompositeCcAbEnabled != value)
			{
				barcodeGs1Extension.CompositeCcAbEnabled = value;
				OnPropertyChanged("CompositeCcAbEnabled");
			}
		}
	}

	public bool CompositeCccEnabled
	{
		get
		{
			return barcodeGs1Extension.CompositeCccEnabled;
		}
		set
		{
			if (barcodeGs1Extension.CompositeCccEnabled != value)
			{
				barcodeGs1Extension.CompositeCccEnabled = value;
				OnPropertyChanged("CompositeCccEnabled");
			}
		}
	}

	public bool ExpandedEnabled
	{
		get
		{
			return barcodeGs1Extension.ExpandedEnabled;
		}
		set
		{
			if (barcodeGs1Extension.ExpandedEnabled != value)
			{
				barcodeGs1Extension.ExpandedEnabled = value;
				OnPropertyChanged("ExpandedEnabled");
			}
		}
	}

	public bool LimitedEnabled
	{
		get
		{
			return barcodeGs1Extension.LimitedEnabled;
		}
		set
		{
			if (barcodeGs1Extension.LimitedEnabled != value)
			{
				barcodeGs1Extension.LimitedEnabled = value;
				OnPropertyChanged("LimitedEnabled");
			}
		}
	}

	public Gs1ExtensionViewModel()
	{
		barcodeGs1Extension = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeGs1Extension;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeGs1Extension = ((ScannerSe4500)device.Scanner).BarcodeGs1Extension;
	}
}
