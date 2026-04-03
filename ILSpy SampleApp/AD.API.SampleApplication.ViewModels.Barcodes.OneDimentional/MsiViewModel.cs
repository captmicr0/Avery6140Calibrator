using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class MsiViewModel : BaseBarcodeViewModel
{
	private Msi barcodeMsi;

	public bool MsiEnabled
	{
		get
		{
			return barcodeMsi.Enabled;
		}
		set
		{
			if (barcodeMsi.Enabled != value)
			{
				barcodeMsi.Enabled = value;
				OnPropertyChanged("MsiEnabled");
			}
		}
	}

	public bool TwoChecksumsEnabled
	{
		get
		{
			return barcodeMsi.TwoChecksumsEnabled;
		}
		set
		{
			if (barcodeMsi.TwoChecksumsEnabled != value)
			{
				barcodeMsi.TwoChecksumsEnabled = value;
				OnPropertyChanged("TwoChecksumsEnabled");
			}
		}
	}

	public bool Mod10Mod11Enabled
	{
		get
		{
			return barcodeMsi.Mod10Mod11Enabled;
		}
		set
		{
			if (barcodeMsi.Mod10Mod11Enabled != value)
			{
				barcodeMsi.Mod10Mod11Enabled = value;
				OnPropertyChanged("Mod10Mod11Enabled");
			}
		}
	}

	public bool DigitsTransmissionEnabled
	{
		get
		{
			return barcodeMsi.CheckDigitsTransmissionEnabled;
		}
		set
		{
			if (barcodeMsi.CheckDigitsTransmissionEnabled != value)
			{
				barcodeMsi.CheckDigitsTransmissionEnabled = value;
				OnPropertyChanged("DigitsTransmissionEnabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeMsi.Length1;
		}
		set
		{
			barcodeMsi.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeMsi.Length2;
		}
		set
		{
			barcodeMsi.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public MsiViewModel()
	{
		barcodeMsi = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeMsi;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeMsi = ((ScannerSe4500)device.Scanner).BarcodeMsi;
	}
}
