using System.Management;

namespace AD.API.SampleApplication.Tools;

public static class BluetoothInfo
{
	public static bool CheckBluetooth()
	{
		bool adapterTypeContainsBTH = false;
		bool captionContainsBTH = false;
		bool nameContainsBTH = false;
		int isBthOn = 0;
		using (ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapter"))
		{
			using ManagementObjectCollection objectCollection = managementClass.GetInstances();
			foreach (ManagementBaseObject managementObject in objectCollection)
			{
				if (managementObject["AdapterType"] != null)
				{
					adapterTypeContainsBTH = CheckNameForBluetooth(managementObject["AdapterType"].ToString());
				}
				if (managementObject["Name"] != null)
				{
					nameContainsBTH = CheckNameForBluetooth(managementObject["Name"].ToString());
				}
				if (managementObject["Caption"] != null)
				{
					captionContainsBTH = CheckNameForBluetooth(managementObject["Caption"].ToString());
				}
				if ((adapterTypeContainsBTH || nameContainsBTH || captionContainsBTH) && managementObject["NetConnectionStatus"] != null && (double)(int)(ushort)managementObject["NetConnectionStatus"] == 7.0)
				{
					isBthOn++;
				}
				managementObject.Dispose();
			}
		}
		if (isBthOn >= 1)
		{
			return true;
		}
		return false;
	}

	private static bool CheckNameForBluetooth(string adapterName)
	{
		if (adapterName.ToLower().Contains("bluetooth"))
		{
			return true;
		}
		return false;
	}
}
