using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.TwoDimentional;

internal class MaxiCodeViewModel : BaseBarcodeViewModel
{
	private MaxiCode barcodeMaxiCode;

	public bool MaxiCodeEnabled
	{
		get
		{
			return barcodeMaxiCode.Enabled;
		}
		set
		{
			if (barcodeMaxiCode.Enabled != value)
			{
				barcodeMaxiCode.Enabled = value;
				OnPropertyChanged("MaxiCodeEnabled");
			}
		}
	}

	public MaxiCodeViewModel()
	{
		barcodeMaxiCode = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeMaxiCode;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeMaxiCode = ((ScannerSe4500)device.Scanner).BarcodeMaxiCode;
	}
}
