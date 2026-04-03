using AD.API.Barcodes.OneDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.OneDimentional;

internal class D2Of5ViewModel : BaseBarcodeViewModel
{
	private NonInterleavedCode25 barcodeNonInterleavedCode25;

	public bool D2Of5Enabled
	{
		get
		{
			return barcodeNonInterleavedCode25.Enabled;
		}
		set
		{
			if (barcodeNonInterleavedCode25.Enabled != value)
			{
				barcodeNonInterleavedCode25.Enabled = value;
				OnPropertyChanged("D2Of5Enabled");
			}
		}
	}

	public double Length1
	{
		get
		{
			return barcodeNonInterleavedCode25.Length1;
		}
		set
		{
			barcodeNonInterleavedCode25.Length1 = (int)value;
			OnPropertyChanged("Length1");
		}
	}

	public double Length2
	{
		get
		{
			return barcodeNonInterleavedCode25.Length2;
		}
		set
		{
			barcodeNonInterleavedCode25.Length2 = (int)value;
			OnPropertyChanged("Length2");
		}
	}

	public D2Of5ViewModel()
	{
		barcodeNonInterleavedCode25 = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeD2Of5;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeNonInterleavedCode25 = ((ScannerSe4500)device.Scanner).BarcodeD2Of5;
	}
}
