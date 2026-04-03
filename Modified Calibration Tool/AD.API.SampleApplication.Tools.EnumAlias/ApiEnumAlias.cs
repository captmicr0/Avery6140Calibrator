using System.Collections.Generic;
using AD.API.Barcodes.OneDimensional;
using AD.API.Barcodes.TwoDimensional;
using AD.API.Core;

namespace AD.API.SampleApplication.Tools.EnumAlias;

internal class ApiEnumAlias
{
	private static Dictionary<ScannerLinearSecurityLevel, string> scannerLiearSecurityLevelAliases;

	private static Dictionary<MirrorDecodingMode, string> mirrorDecodingModeAliases;

	private static Dictionary<InverseScanningMode, string> inverseScanningModeAliases;

	private static Dictionary<UpcPreambleTransmissionMode, string> upcPreambleTransmissionModeAliases;

	public static Dictionary<ScannerLinearSecurityLevel, string> ScannerLiearSecurityLevelAliases => scannerLiearSecurityLevelAliases;

	public static Dictionary<MirrorDecodingMode, string> MirrorDecodingModeAliases => mirrorDecodingModeAliases;

	public static Dictionary<InverseScanningMode, string> InverseScanningModeAliases => inverseScanningModeAliases;

	public static Dictionary<UpcPreambleTransmissionMode, string> UpcPreambleTransmissionModeAliases => upcPreambleTransmissionModeAliases;

	static ApiEnumAlias()
	{
		scannerLiearSecurityLevelAliases = new Dictionary<ScannerLinearSecurityLevel, string>
		{
			{
				ScannerLinearSecurityLevel.Low,
				"Low"
			},
			{
				ScannerLinearSecurityLevel.Medium,
				"Medium"
			},
			{
				ScannerLinearSecurityLevel.High,
				"High"
			},
			{
				ScannerLinearSecurityLevel.VeryHigh,
				"Very High"
			}
		};
		mirrorDecodingModeAliases = new Dictionary<MirrorDecodingMode, string>
		{
			{
				MirrorDecodingMode.Never,
				"Never"
			},
			{
				MirrorDecodingMode.Always,
				"Always"
			},
			{
				MirrorDecodingMode.Autodetect,
				"Auto-Detect"
			}
		};
		inverseScanningModeAliases = new Dictionary<InverseScanningMode, string>
		{
			{
				InverseScanningMode.RegularOnly,
				"Regular Only"
			},
			{
				InverseScanningMode.InversedOnly,
				"Inversed Only"
			},
			{
				InverseScanningMode.RegularAndInversed,
				"Regular And Inversed"
			}
		};
		upcPreambleTransmissionModeAliases = new Dictionary<UpcPreambleTransmissionMode, string>
		{
			{
				UpcPreambleTransmissionMode.None,
				"None"
			},
			{
				UpcPreambleTransmissionMode.SystemCharacter,
				"All"
			},
			{
				UpcPreambleTransmissionMode.CountryCodeAndSysChars,
				"Country Code"
			}
		};
	}
}
