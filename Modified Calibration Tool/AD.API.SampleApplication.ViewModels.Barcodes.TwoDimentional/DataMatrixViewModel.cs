using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.Scanners.Motorola.Se4500;

namespace AD.API.SampleApplication.ViewModels.Barcodes.TwoDimentional;

internal class DataMatrixViewModel : BaseBarcodeViewModel
{
	private DataMatrix barcodeDataMatrix;

	public bool DataMatrixEnabled
	{
		get
		{
			return barcodeDataMatrix.Enabled;
		}
		set
		{
			if (barcodeDataMatrix.Enabled != value)
			{
				barcodeDataMatrix.Enabled = value;
				OnPropertyChanged("DataMatrixEnabled");
			}
		}
	}

	public InverseScanningMode InverseScanning
	{
		get
		{
			return barcodeDataMatrix.InverseScanning;
		}
		set
		{
			if (barcodeDataMatrix.InverseScanning != value)
			{
				barcodeDataMatrix.InverseScanning = value;
				OnPropertyChanged("InverseScanning");
			}
		}
	}

	public MirrorDecodingMode MirrorDecoding
	{
		get
		{
			return barcodeDataMatrix.MirrorDecoding;
		}
		set
		{
			if (barcodeDataMatrix.MirrorDecoding != value)
			{
				barcodeDataMatrix.MirrorDecoding = value;
				OnPropertyChanged("MirrorDecoding");
			}
		}
	}

	public DataMatrixViewModel()
	{
		barcodeDataMatrix = ((ScannerSe4500)ApplicationContext.CurrentDevice.Scanner).BarcodeDataMatrix;
	}

	protected sealed override void InitializeBarcode(IDevice device)
	{
		base.InitializeBarcode(device);
		barcodeDataMatrix = ((ScannerSe4500)device.Scanner).BarcodeDataMatrix;
	}
}
