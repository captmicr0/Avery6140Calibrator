using System;
using System.Threading;

namespace AD.API.SampleApplication.Tools.MessageManagement;

internal class MessageSynchronizer
{
	public static readonly string ERROR_TITLE = "Error";

	public static readonly string WARNING_TITLE = "Warning";

	public static readonly string SERVER_ERROR_TITLE = "Server Error";

	public static readonly string WEB_ERROR_TITLE = "Web Exception";

	public static readonly string PRINTER_ERROR_TITLE = "Printer Error";

	public static readonly string CONNECTION_ERROR_TITLE = "Connection Error";

	public static readonly string TRIGGER_BUTTON_TITLE = "Trigger";

	public static readonly string FEED_BUTTON_TITLE = "Feed button";

	public static readonly string UNKNOWN_BUTTON_TITLE = "Unknown button";

	public static readonly string RESET_PARAMETERS_TITLE = "Reset parameters?";

	public static readonly string DISABLE_BARCODES_TITLE = "Disable all barcodes?";

	public static readonly string MESSAGE_FORMAT_STRING = "{0}: {1}";

	private static object _locker = new object();

	private static MessageSynchronizer _instance;

	protected MessageSynchronizer()
	{
	}

	public static MessageSynchronizer GetObject()
	{
		if (_instance == null)
		{
			lock (_locker)
			{
				if (_instance == null)
				{
					_instance = new MessageSynchronizer();
				}
			}
		}
		return _instance;
	}

	public void AddActionToQueue(Action action)
	{
		new Thread((ThreadStart)delegate
		{
			action();
		}).Start();
	}

	public void StopAndClearAllActions()
	{
		while (MessageBoxKiller.CloseTargetMessage(ERROR_TITLE))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(WARNING_TITLE))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(SERVER_ERROR_TITLE))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(PRINTER_ERROR_TITLE))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(WEB_ERROR_TITLE))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(RESET_PARAMETERS_TITLE))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(DISABLE_BARCODES_TITLE))
		{
		}
	}

	public void ClearAllMessagesForDevice(string deviceSerial)
	{
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, SERVER_ERROR_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, WEB_ERROR_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, WARNING_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, ERROR_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, PRINTER_ERROR_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, CONNECTION_ERROR_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, TRIGGER_BUTTON_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, FEED_BUTTON_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, UNKNOWN_BUTTON_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, RESET_PARAMETERS_TITLE)))
		{
		}
		while (MessageBoxKiller.CloseTargetMessage(string.Format(MESSAGE_FORMAT_STRING, deviceSerial, DISABLE_BARCODES_TITLE)))
		{
		}
	}
}
