using System;
using System.Runtime.InteropServices;

namespace AD.API.SampleApplication.Tools.MessageManagement;

public class MessageBoxKiller
{
	private const int WM_CLOSE = 16;

	private const int WM_SHOWWINDOW = 24;

	public static bool CloseTargetMessage(string messageBoxName)
	{
		IntPtr messageWindow = FindWindow(null, messageBoxName);
		if (messageWindow != IntPtr.Zero)
		{
			SendMessage(messageWindow, 16u, IntPtr.Zero, IntPtr.Zero);
			return true;
		}
		return false;
	}

	public static bool ShowWindow(string messageBoxName)
	{
		IntPtr messageWindow = FindWindow(null, messageBoxName);
		if (messageWindow != IntPtr.Zero)
		{
			SendMessage(messageWindow, 24u, IntPtr.Zero, IntPtr.Zero);
			return true;
		}
		return false;
	}

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
}
