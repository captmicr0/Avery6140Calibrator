using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AD.API.Barcodes.OneDimensional;
using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;
using AD.API.Core.Imports;
using AD.API.Printers.AveryDennison.Pr6140;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.Scanners.Motorola.Se4500;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AD.API.SampleApplication.Helpers;

internal class ConfigurationHandler
{
	private Dictionary<string, uint> barcode_type = new Dictionary<string, uint>
	{
		{ "Code 39", 1u },
		{ "Codabar", 2u },
		{ "Code 128", 3u },
		{ "D25", 4u },
		{ "IATA", 5u },
		{ "ITF", 6u },
		{ "Code 93", 7u },
		{ "UPCA", 8u },
		{ "UPCE", 9u },
		{ "EAN-8", 10u },
		{ "EAN-13", 11u },
		{ "Code 11", 12u },
		{ "MSI", 14u },
		{ "EAN-128", 15u },
		{ "UPCE1", 16u },
		{ "PDF-417", 17u },
		{ "Code 39 Full ASCII", 19u },
		{ "Trioptic", 21u },
		{ "Bookland", 22u },
		{ "Coupon Code", 23u },
		{ "ISBT-128", 25u },
		{ "Micro PDF", 26u },
		{ "Data Matrix", 27u },
		{ "QR Code", 28u },
		{ "Postal (US)", 30u },
		{ "Planet (US)", 31u },
		{ "Code 32", 32u },
		{ "ISBT-128 Concat.", 33u },
		{ "Postal (Japan)", 34u },
		{ "Postal (Australia)", 35u },
		{ "Postal (Dutch)", 36u },
		{ "Maxicode", 37u },
		{ "Postal (CA)", 38u },
		{ "Postal (UK)", 39u },
		{ "Macro PDF-417", 40u },
		{ "RSS-14", 48u },
		{ "RSS Limited", 49u },
		{ "RSS Expanded", 50u },
		{ "Parameter (FNC3)", 51u },
		{ "Scanlet Webcode", 55u },
		{ "Cue CAT Code", 56u },
		{ "UPCA + 2", 72u },
		{ "UPCE + 2", 73u },
		{ "EAN-8 + 2", 74u },
		{ "EAN-13 + 2", 75u },
		{ "UPCE1 + 2", 80u },
		{ "CC-A + EAN-128", 81u },
		{ "CC-A + EAN-13", 82u },
		{ "CC-A + EAN-8", 83u },
		{ "CC-A + RSS Expanded", 84u },
		{ "CC-A + RSS Limited", 85u },
		{ "CC-A + RSS-14", 86u },
		{ "CC-A + UPC-A", 87u },
		{ "CC-A + UPC-E", 88u },
		{ "CC-C + EAN-128", 89u },
		{ "TLC-39", 90u },
		{ "CC-B + EAN-128", 97u },
		{ "CC-B + EAN-13", 98u },
		{ "CC-B + EAN-8", 99u },
		{ "CC-B + RSS Expanded", 100u },
		{ "CC-B + RSS Limited", 101u },
		{ "CC-B + RSS-14", 102u },
		{ "CC-B + UPC-A", 103u },
		{ "CC-B + UPC-E", 104u },
		{ "UPCA + 5", 136u },
		{ "UPCE + 5", 137u },
		{ "EAN-8 + 5", 138u },
		{ "EAN-13 + 5", 139u },
		{ "UPCE1 + 5", 144u },
		{ "Multipacket Format", 153u },
		{ "Macro Micro PDF", 154u }
	};

	public dynamic parseConfigFile(string configPath)
	{
		if (!Directory.Exists(configPath))
		{
			return null;
		}
		if (File.Exists(configPath + "\\config.json"))
		{
			return JsonConvert.DeserializeObject(File.ReadAllText(configPath + "config.json"));
		}
		string[] files = Directory.GetFiles(configPath, "config.*.json");
		if (files.Length >= 1)
		{
			return JsonConvert.DeserializeObject(File.ReadAllText(files[0]));
		}
		return null;
	}

	public bool isValidTrackingFormat(dynamic configObject, string data)
	{
		bool defaultValue = false;
		if (configObject == null)
		{
			return defaultValue;
		}
		JArray trackingInfo = getTrackingInfo(configObject);
		uint dataLength = (uint)data.Length;
		uint index = 0u;
		if (trackingInfo == null)
		{
			Console.WriteLine("Tracking Formats are not available in this config object.");
			return false;
		}
		foreach (dynamic infoNugget in trackingInfo)
		{
			foreach (dynamic filter in infoNugget.filters)
			{
				string prefix = null;
				uint minimumLength = 0u;
				uint maximumLength = 0u;
				try
				{
					prefix = filter.prefix;
				}
				catch (Exception)
				{
					Console.WriteLine("Tracking Format {0} Prefix is not available in the config object.", index);
				}
				try
				{
					minimumLength = filter.minimumLength;
				}
				catch (Exception)
				{
					Console.WriteLine("Tracking Format {0} Minimum Length not available in the config object.", index);
				}
				try
				{
					maximumLength = filter.maximumLength;
				}
				catch (Exception)
				{
					Console.WriteLine("Tracking Format {0} Maximum Length not available in the config object.", index);
				}
				if ((prefix == null || data.StartsWith(prefix)) && dataLength >= minimumLength && dataLength <= maximumLength)
				{
					Console.WriteLine("Tracking Format {0} is a match!", index);
					return !defaultValue;
				}
				index++;
			}
		}
		return defaultValue;
	}

	private JArray getTrackingInfo(dynamic configObject)
	{
		JArray defaultValue = null;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.tracking;
		}
		catch (Exception)
		{
			Console.WriteLine("Tracking Formats are not available in this config object.");
			return defaultValue;
		}
	}

	public string getTrackingPrefix(dynamic configObject)
	{
		string defaultValue = null;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.tracking.prefix;
		}
		catch (Exception)
		{
			Console.WriteLine("Tracking Prefix is not available in the config object.");
			return defaultValue;
		}
	}

	public uint getTrackingMaximumLength(dynamic configObject, string symbology)
	{
		uint defaultValue = 0u;
		JArray trackingInfo = getTrackingInfo(configObject);
		if (configObject == null || trackingInfo == null || symbology == null)
		{
			return defaultValue;
		}
		uint maximumLength = defaultValue;
		uint index = 0u;
		foreach (dynamic infoNugget in trackingInfo)
		{
			if (!((infoNugget.symbology == symbology) ? true : false))
			{
				continue;
			}
			foreach (dynamic filter in infoNugget.filters)
			{
				try
				{
					if (filter.maximumLength >= maximumLength)
					{
						maximumLength = filter.maximumLength;
					}
					index++;
				}
				catch (Exception)
				{
					Console.WriteLine("Tracking {0} Maximum Length is not available in the config object.", index);
				}
			}
		}
		return maximumLength;
	}

	public uint getTrackingMinimumLength(dynamic configObject, string symbology)
	{
		uint defaultValue = 0u;
		JArray trackingInfo = getTrackingInfo(configObject);
		if (configObject == null || trackingInfo == null || symbology == null)
		{
			return defaultValue;
		}
		uint minimumLength = defaultValue;
		uint index = 0u;
		foreach (dynamic infoNugget in trackingInfo)
		{
			if (!((infoNugget.symbology == symbology) ? true : false))
			{
				continue;
			}
			foreach (dynamic filter in infoNugget.filters)
			{
				try
				{
					if (filter.minimumLength >= minimumLength)
					{
						minimumLength = filter.minimumLength;
					}
					index++;
				}
				catch (Exception)
				{
					Console.WriteLine("Tracking {0} Minimum Length is not available in the config object.", index);
				}
			}
		}
		return minimumLength;
	}

	public int getContrast(dynamic configObject)
	{
		int defaultValue = 0;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.contrast;
		}
		catch (Exception)
		{
			Console.WriteLine("Contrast is not available in the config object.");
			return defaultValue;
		}
	}

	public EnergyType getEnergyType(dynamic configObject)
	{
		EnergyType defaultValue = EnergyType.Standard;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			EnergyType energyType = default(EnergyType);
			if (Enum.TryParse(configObject.printerSettings.energyType, out energyType))
			{
				return energyType;
			}
			return defaultValue;
		}
		catch (Exception)
		{
			Console.WriteLine("Energy Type is not available in the config object.");
			return defaultValue;
		}
	}

	public int getSupplyPosition(dynamic configObject)
	{
		int defaultValue = 0;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.supplyPosition;
		}
		catch (Exception)
		{
			Console.WriteLine("Supply Position is not available in the config object.");
			return defaultValue;
		}
	}

	public int getMaximumLabelLength(dynamic configObject)
	{
		int defaultValue = 8;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.maximumLabelLength;
		}
		catch (Exception)
		{
			Console.WriteLine("Maximum Label Length is not available in the config object.");
			return defaultValue;
		}
	}

	public bool getNonIndexLineModeEnabled(dynamic configObject)
	{
		bool defaultValue = false;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.nonIndexLineModeEnabled;
		}
		catch (Exception)
		{
			Console.WriteLine("Non Inex Line Mode Enabled is not available in the config object.");
			return defaultValue;
		}
	}

	public int getHorizontalAdjust(dynamic configObject)
	{
		int defaultValue = 0;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.horizontalAdjust;
		}
		catch (Exception)
		{
			Console.WriteLine("Horizontal Adjust is not available in the config object.");
			return defaultValue;
		}
	}

	public int getVerticalAdjust(dynamic configObject)
	{
		int defaultValue = 0;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.varticalAdjust;
		}
		catch (Exception)
		{
			Console.WriteLine("Vertical Adjust is not available in the config object.");
			return defaultValue;
		}
	}

	public int getPrintSpeed(dynamic configObject)
	{
		int defaultValue = 0;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			return configObject.printerSettings.printSpeed;
		}
		catch (Exception)
		{
			Console.WriteLine("Print Speed is not available in the config object.");
			return defaultValue;
		}
	}

	public SupplyType getSupplyType(dynamic configObject)
	{
		SupplyType defaultValue = SupplyType.BlackMark;
		if (configObject == null)
		{
			return defaultValue;
		}
		try
		{
			SupplyType supplytype = default(SupplyType);
			if (Enum.TryParse(configObject.printerSettings.supplyType, out supplytype))
			{
				return supplytype;
			}
			return defaultValue;
		}
		catch (Exception)
		{
			Console.WriteLine("Supply Type is not available in the config object.");
			return defaultValue;
		}
	}

	public string[] getTrackingSymbologies(dynamic configObject)
	{
		List<string> symbologies = new List<string>();
		if (configObject == null)
		{
			return null;
		}
		try
		{
			foreach (dynamic trackingItem in configObject.tracking)
			{
				symbologies.Add(trackingItem.symbology.ToString());
			}
			return symbologies.ToArray();
		}
		catch (Exception)
		{
			Console.WriteLine("Tracking Information or Symbologies are not available in the config object.");
			return null;
		}
	}

	public string getRouteZoneLocation(dynamic configObject)
	{
		try
		{
			return configObject.dataLocations.routeZone;
		}
		catch (Exception)
		{
			Console.WriteLine("Route Zone location is not available in the config object.");
			return "^FB400,2,0,C";
		}
	}

	public string getSortZoneLocation(dynamic configObject)
	{
		try
		{
			return configObject.dataLocations.sortZone;
		}
		catch (Exception)
		{
			Console.WriteLine("Sort zone location is not available in the config object.");
			return "^FO390,50";
		}
	}

	public string getTrackingIDLocation(dynamic configObject)
	{
		try
		{
			return configObject.dataLocations.trackingID;
		}
		catch (Exception)
		{
			Console.WriteLine("Tracking ID location is not available in the config object.");
			return "^FO20,280";
		}
	}

	public string getErrorCodeLocation(dynamic configObject)
	{
		try
		{
			return configObject.dataLocations.errorCode;
		}
		catch (Exception)
		{
			Console.WriteLine("Error Code location is not available in the config object.");
			return "^FO390,150";
		}
	}

	public int getMaxDeviceCount(dynamic configObject)
	{
		try
		{
			return configObject.applicationSettings.maxPrinterCount;
		}
		catch (Exception)
		{
			Console.WriteLine("Max printer count is not available in the config object.");
			return 1;
		}
	}

	public bool getResetButtonEnable(dynamic configObject)
	{
		try
		{
			return configObject.applicationSettings.resetButtonEnable;
		}
		catch (Exception)
		{
			Console.WriteLine("Reset Button Enable is not available in the config object.");
			return false;
		}
	}

	public bool getServerLoggingEnable(dynamic configObject)
	{
		try
		{
			return configObject.applicationSettings.serverLoggingEnable;
		}
		catch (Exception)
		{
			Console.WriteLine("Server Logging Enable is not available in the config object.");
			return false;
		}
	}

	public bool getInfoLoggingEnable(dynamic configObject)
	{
		try
		{
			return configObject.applicationSettings.serverLoggingEnable;
		}
		catch (Exception)
		{
			Console.WriteLine("Info Logging Enable is not available in the config object.");
			return false;
		}
	}

	private BarcodeType[] parseSymbologyList(dynamic configObject)
	{
		if (configObject == null)
		{
			return null;
		}
		List<BarcodeType> symbologyList = new List<BarcodeType>();
		string[] symbologies = getTrackingSymbologies(configObject);
		if (symbologies == null)
		{
			return null;
		}
		string[] array = symbologies;
		foreach (string symbology in array)
		{
			if (Enum.TryParse<BarcodeType>(symbology, out var barcodeType))
			{
				symbologyList.Add(barcodeType);
			}
			else if (symbology == "GS1")
			{
				symbologyList.Add(BarcodeType.Gs1Cca_Upca);
			}
		}
		if (symbologyList.Count > 0)
		{
			return Enumerable.ToArray(symbologyList);
		}
		return null;
	}

	public void applyScannerConfig(dynamic configObject, IDevice device)
	{
		BarcodeType[] symbologies = parseSymbologyList(configObject);
		if (symbologies == null)
		{
			return;
		}
		try
		{
			device.Scanner.LoadSettings();
			device.Scanner.DisableAllBarcodes();
			device.Scanner.BeginSetSession();
			BarcodeType[] array = symbologies;
			for (int i = 0; i < array.Length; i++)
			{
				BarcodeType symbology = array[i];
				uint trackingMimimumLength = getTrackingMinimumLength(configObject, symbology.ToString());
				uint trackingMaximumLength = getTrackingMaximumLength(configObject, symbology.ToString());
				setBarcodeSettings(symbology, trackingMimimumLength, trackingMaximumLength, device);
			}
			device.Scanner.EndSetSession();
		}
		catch (Exception ex)
		{
			Messenger.ShowMessage(ex.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	public void applyPrinterConfig(dynamic configObject, IDevice device)
	{
		try
		{
			device.Printer.LoadSettings();
			device.Printer.BeginSetSession();
			device.Printer.Contrast = getContrast(configObject);
			device.Printer.EnergyType = getEnergyType(configObject);
			device.Printer.SupplyPosition = getSupplyPosition(configObject);
			((Printer6140)device.Printer).MaximumLabelLength = getMaximumLabelLength(configObject);
			((Printer6140)device.Printer).NoneIndexLineModeEnabled = getNonIndexLineModeEnabled(configObject);
			((Printer6140)device.Printer).HorizontalAdjust = getHorizontalAdjust(configObject);
			((Printer6140)device.Printer).VerticalAdjust = getVerticalAdjust(configObject);
			((Printer6140)device.Printer).Speed = getPrintSpeed(configObject);
			device.Printer.SupplyType = getSupplyType(configObject);
			device.Printer.EndSetSession();
		}
		catch (Exception ex)
		{
			Messenger.ShowMessage(ex.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	public void setBarcodeSettings(BarcodeType barcode, uint minLength, uint maxLength, IDevice device)
	{
		switch (barcode)
		{
		case BarcodeType.Codabar:
		{
			Codabar barcodeCodabar = ((ScannerSe4500)device.Scanner).BarcodeCodabar;
			barcodeCodabar.Enabled = true;
			barcodeCodabar.Length1 = (int)minLength;
			barcodeCodabar.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.Code128:
		{
			Code128 barcodeCode3 = ((ScannerSe4500)device.Scanner).BarcodeCode128;
			barcodeCode3.Enabled = true;
			barcodeCode3.Isbt128Enabled = true;
			barcodeCode3.UccEan128Enabled = true;
			barcodeCode3.Length1 = (int)minLength;
			barcodeCode3.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.Code39:
		case BarcodeType.Code39Trioptic:
		{
			Code39 barcodeCode2 = ((ScannerSe4500)device.Scanner).BarcodeCode39;
			barcodeCode2.Enabled = true;
			barcodeCode2.TriopticEnabled = true;
			barcodeCode2.Length1 = (int)minLength;
			barcodeCode2.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.Code93:
		{
			Code93 barcodeCode = ((ScannerSe4500)device.Scanner).BarcodeCode93;
			barcodeCode.Enabled = true;
			barcodeCode.Length1 = (int)minLength;
			barcodeCode.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.CodeD2of5:
		{
			NonInterleavedCode25 barcodeD2Of = ((ScannerSe4500)device.Scanner).BarcodeD2Of5;
			barcodeD2Of.Enabled = true;
			barcodeD2Of.Length1 = (int)minLength;
			barcodeD2Of.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.DataMatrix:
		{
			DataMatrix barcodeDataMatrix = ((ScannerSe4500)device.Scanner).BarcodeDataMatrix;
			barcodeDataMatrix.Enabled = true;
			barcodeDataMatrix.InverseScanning = InverseScanningMode.RegularAndInversed;
			barcodeDataMatrix.MirrorDecoding = MirrorDecodingMode.Autodetect;
			break;
		}
		case BarcodeType.Gs1Cca_Ean128:
		case BarcodeType.Gs1Cca_Ean13:
		case BarcodeType.Gs1Cca_Ean8:
		case BarcodeType.Gs1Cca_RssExpanded:
		case BarcodeType.Gs1Cca_RssLimited:
		case BarcodeType.Gs1Cca_Rss14:
		case BarcodeType.Gs1Cca_Upca:
		case BarcodeType.Gs1Cca_Upce:
		case BarcodeType.Gs1Ccc_Ean128:
		case BarcodeType.Gs1Ccb_Ean128:
		case BarcodeType.Gs1Ccb_Ean13:
		case BarcodeType.Gs1Ccb_Ean8:
		case BarcodeType.Gs1Ccb_RssExpanded:
		case BarcodeType.Gs1Ccb_RssLimited:
		case BarcodeType.Gs1Ccb_Rss14:
		case BarcodeType.Gs1Ccb_Upca:
		case BarcodeType.Gs1Ccb_Upce:
		{
			Gs1 barcodeGs = ((ScannerSe4500)device.Scanner).BarcodeGs1;
			Gs1Extension gs1Ext = ((ScannerSe4500)device.Scanner).BarcodeGs1Extension;
			barcodeGs.Enabled = true;
			barcodeGs.UpcEanConversionEnabled = true;
			gs1Ext.CompositeCcAbEnabled = true;
			gs1Ext.CompositeCccEnabled = true;
			gs1Ext.ExpandedEnabled = true;
			gs1Ext.LimitedEnabled = true;
			gs1Ext.UpcEanConversionEnabled = true;
			break;
		}
		case BarcodeType.CodeIATA2of5:
		case BarcodeType.CodeInterleaved2of5:
		{
			InterleavedCode25 barcodeI2Of = ((ScannerSe4500)device.Scanner).BarcodeI2Of5;
			barcodeI2Of.Enabled = true;
			barcodeI2Of.Length1 = (int)minLength;
			barcodeI2Of.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.MaxiCode:
			((ScannerSe4500)device.Scanner).BarcodeMaxiCode.Enabled = true;
			break;
		case BarcodeType.Msi:
		{
			Msi barcodeMsi = ((ScannerSe4500)device.Scanner).BarcodeMsi;
			barcodeMsi.Enabled = true;
			barcodeMsi.Mod10Mod11Enabled = true;
			barcodeMsi.TwoChecksumsEnabled = true;
			barcodeMsi.Length1 = (int)minLength;
			barcodeMsi.Length2 = (int)maxLength;
			break;
		}
		case BarcodeType.Pdf417:
		{
			Pdf417 barcodePdf = ((ScannerSe4500)device.Scanner).BarcodePdf417;
			barcodePdf.Enabled = true;
			barcodePdf.MicroEnabled = true;
			break;
		}
		case BarcodeType.Qr:
		{
			QrCode barcodeQrCode = ((ScannerSe4500)device.Scanner).BarcodeQrCode;
			barcodeQrCode.Enabled = true;
			barcodeQrCode.MicroEnabled = false;
			barcodeQrCode.InverseMode = InverseScanningMode.RegularOnly;
			break;
		}
		case BarcodeType.Upca:
		case BarcodeType.Upce:
		case BarcodeType.Ean8:
		case BarcodeType.Ean13:
		case BarcodeType.Ean128:
		case BarcodeType.Upce1:
		case BarcodeType.EanBookland:
		case BarcodeType.CouponCode:
		case BarcodeType.Upca_2:
		case BarcodeType.Upce_2:
		case BarcodeType.Ean8_2:
		case BarcodeType.Ean13_2:
		case BarcodeType.Upce1_2:
		case BarcodeType.Upca_5:
		case BarcodeType.Upce_5:
		case BarcodeType.Ean8_5:
		case BarcodeType.Ean13_5:
		case BarcodeType.Upce1_5:
		{
			Upc barcodeUpc = ((ScannerSe4500)device.Scanner).BarcodeUpc;
			barcodeUpc.Enabled = true;
			barcodeUpc.CouponCodeEnabled = true;
			barcodeUpc.Ean13Enabled = true;
			barcodeUpc.Ean8Enabled = true;
			barcodeUpc.Ean8ZerosExtensionEnabled = true;
			barcodeUpc.EanBooklandEnabled = true;
			barcodeUpc.Upce1Enabled = true;
			barcodeUpc.UpceEnabled = true;
			break;
		}
		}
	}
}
