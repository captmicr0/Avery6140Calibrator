using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class CodabarViewModel : BaseBarcodeViewModel
{
	private Codabar barcodeCodabar;

	public bool CodabarEnabled
	{
		get
		{
			return barcodeCodabar.Enabled;
		}
		set
		{
			if (barcodeCodabar.Enabled != value)
			{
				barcodeCodabar.Enabled = value;
				OnPropertyChanged("CodabarEnabled");
			}
		}
	}

	public bool ClsiEnabled
	{
		get
		{
			return barcodeCodabar.ClsiEditingEnabled;
		}
		set
		{
			if (barcodeCodabar.ClsiEditingEnabled != value)
			{
				barcodeCodabar.ClsiEditingEnabled = value;
				OnPropertyChanged("ClsiEnabled");
			}
		}
	}

	public bool NotisEnabled
	{
		get
		{
			return barcodeCodabar.NotisEditingEnabled;
		}
		set
		{
			if (barcodeCodabar.NotisEditingEnabled != value)
			{
				barcodeCodabar.NotisEditingEnabled = value;
				OnPropertyChanged("NotisEnabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeCodabar.Length1;
		}
		set
		{
			barcodeCodabar.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeCodabar.Length2;
		}
		set
		{
			barcodeCodabar.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public CodabarViewModel()
	{
		barcodeCodabar = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeCodabar;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeCodabar = ((ScannerSe4500)device.Scanner).BarcodeCodabar;
	}
}
