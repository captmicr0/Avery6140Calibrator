using System.Windows;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Controls;

namespace AD.API.SampleApplication.Tools.MessageManagement;

internal static class Messenger
{
	public static void ShowErrorMessage(ApiException ex)
	{
		ShowMessage($"{ex.Message}\r\nError code is {(int)ex.ErrorCode}.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Hand);
	}

	public static bool ShowMessage(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
	{
		bool returnValue = false;
		Application.Current.Dispatcher.Invoke(delegate
		{
			CommonMessageDialog commonMessageDialog = new CommonMessageDialog(message, caption, buttons, image);
			returnValue = commonMessageDialog.ShowDialog().Value;
		});
		return returnValue;
	}

	public static void ShowSelectedDeviceConnectionLost(string sceenToNavigateName)
	{
		ShowMessage($"Connection to the selected device was lost. \r\nNavigating to {sceenToNavigateName}.", "No connection!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
	}

	public static void ShowNoDeviceAvailable()
	{
		ShowMessage("No device is connected. Connect first.", "No connection!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
	}
}
