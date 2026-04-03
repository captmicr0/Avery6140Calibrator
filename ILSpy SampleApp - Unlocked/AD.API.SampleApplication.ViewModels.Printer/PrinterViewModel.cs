using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using AD.API.Core;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;

namespace AD.API.SampleApplication.ViewModels.Printer;

internal class PrinterViewModel : BaseViewModel
{
	private int printNumber;

	private Command printerCommand;

	public Command PrinterCommand => printerCommand;

	public PrinterViewModel()
	{
		printerCommand = new Command(PrinterCommandMethod);
	}

	private void PrinterCommandMethod(object commandName)
	{
		string commandToExecuteName = commandName.ToString();
		List<IDevice> deviceListToExecute = new List<IDevice>();
		if (ApplicationContext.AllDevicesSelected)
		{
			deviceListToExecute.AddRange(ApplicationContext.DeviceManager.ConnectedDevices);
		}
		else
		{
			deviceListToExecute.Add(ApplicationContext.CurrentDevice);
		}
		try
		{
			switch (commandToExecuteName)
			{
			case "Resync":
			{
				foreach (IDevice item in deviceListToExecute)
				{
					item.Printer.Resync();
				}
				break;
			}
			case "GoToBlack":
			{
				foreach (IDevice item2 in deviceListToExecute)
				{
					item2.Printer.GoToBlack();
				}
				break;
			}
			case "CalibrateBlack":
			{
				foreach (IDevice item3 in deviceListToExecute)
				{
					item3.Printer.CalibrateBlackMark();
				}
				break;
			}
			case "FeedLabel":
			{
				foreach (IDevice item4 in deviceListToExecute)
				{
					item4.Printer.FeedLabel();
				}
				break;
			}
			case "ClearError":
			{
				foreach (IDevice item5 in deviceListToExecute)
				{
					item5.Printer.ClearError();
				}
				break;
			}
			case "AbortError":
			{
				foreach (IDevice item6 in deviceListToExecute)
				{
					item6.Printer.AbortError();
				}
				break;
			}
			case "AbortAllJobs":
			{
				foreach (IDevice item7 in deviceListToExecute)
				{
					item7.Printer.AbortAllJobs();
				}
				break;
			}
			case "PrintSample":
			{
				List<byte[]> list = new List<byte[]>();
				list.Add(Encoding.ASCII.GetBytes(string.Format(CultureInfo.InvariantCulture, "Test #{0}", new object[1] { printNumber })));
				List<byte[]> args = list;
				foreach (IDevice item8 in deviceListToExecute)
				{
					item8.Printer.Print("Sample Print LNT", 1, args);
				}
				printNumber++;
				break;
			}
			case "PrintRuler":
			{
				foreach (IDevice item9 in deviceListToExecute)
				{
					item9.Printer.Print("Calibration Template LNT", 1, new List<byte[]>());
				}
				break;
			}
			}
		}
		catch (ApiException ex)
		{
			Messenger.ShowErrorMessage(ex);
		}
		catch (Exception ex2)
		{
			Messenger.ShowMessage(ex2.Message, "Exception.", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	public override void ValidateSelf()
	{
		base.ValidateSelf();
		OnConnectionLostHandled(base.DeviceConnected);
		if (base.DeviceConnected)
		{
			OnPropertyChanged(string.Empty);
		}
	}
}
