using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class Code93ViewModel : BaseBarcodeViewModel
{
	private Code93 barcodeCode93;

	public bool Code93Enabled
	{
		get
		{
			return barcodeCode93.Enabled;
		}
		set
		{
			if (barcodeCode93.Enabled != value)
			{
				barcodeCode93.Enabled = value;
				OnPropertyChanged("Code93Enabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeCode93.Length1;
		}
		set
		{
			barcodeCode93.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeCode93.Length2;
		}
		set
		{
			barcodeCode93.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public Code93ViewModel()
	{
		barcodeCode93 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeCode93;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeCode93 = ((ScannerSe4500)device.Scanner).BarcodeCode93;
	}
}
