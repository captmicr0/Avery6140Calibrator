using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using AD.API.Core;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.Scenarios;
using AD.API.SampleApplication.Views.Connection;

namespace AD.API.SampleApplication.Views.Scenarios;

public partial class ScenariosView : Page, IComponentConnector
{
	public static RoutedCommand navigateToConfigurationCommand = new RoutedCommand();

	public ScenariosView()
	{
		InitializeComponent();
		base.DataContext = new ScenariosViewModel();
		((BaseViewModel)base.DataContext).ConnectionLostHandledEvent += delegate(object sender, ConnectionLostHandledEventArgs args)
		{
			if (!args.IsValid)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					Messenger.ShowNoDeviceAvailable();
					((BaseViewModel)base.DataContext).Finalization();
					SpinnerOverlay.Visibility = Visibility.Collapsed;
                    BtnResetAndBlackMarkCal.IsEnabled = false;
                    BtnBlackMarkCalibration.IsEnabled = false;
					BtnReset.IsEnabled = false;
					base.DataContext = null;
				});
			}
		};
		navigateToConfigurationCommand.InputGestures.Add(new KeyGesture(Key.F12, ModifierKeys.Control));
		if (!ApplicationContext.resetButtonEnable)
		{
			BtnReset.Visibility = Visibility.Collapsed;
			BtnReset.IsEnabled = false;
		}
		else if (ApplicationContext.DeviceManager.ConnectedDevices.Count > 0)
        {
            BtnResetAndBlackMarkCal.IsEnabled = true;
            BtnReset.IsEnabled = true;
			BtnBlackMarkCalibration.IsEnabled = true;
		}
		else
        {
            BtnResetAndBlackMarkCal.IsEnabled = false;
            BtnReset.IsEnabled = false;
			BtnBlackMarkCalibration.IsEnabled = false;
        }
	}

	private void ScanAndPrintTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
	{
		if (ScanAndPrintTextBox.LineCount > 0)
		{
			ScanAndPrintTextBox.ScrollToLine(ScanAndPrintTextBox.LineCount - 1);
		}
	}

	private void ActiveDevice_OnTextChanged(object sender, TextChangedEventArgs e)
	{
	}

	private void BtnConnection_OnClick(object sender, RoutedEventArgs e)
	{
		if (base.DataContext != null)
		{
			((BaseViewModel)base.DataContext).Finalization();
		}
		DisconnectAllConnectedDevices();
		NavigationController.GetInstance.NavigateTo<ConnectionView>();
	}

	private void BtnConfiguration_OnClick(object sender, RoutedEventArgs e)
	{
		if (base.DataContext != null)
		{
			((BaseViewModel)base.DataContext).Finalization();
		}
		NavigationController.GetInstance.NavigateTo<ConfigView>();
	}

	public void navigateToConfigurationCommandExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		if (base.DataContext != null)
		{
			((BaseViewModel)base.DataContext).Finalization();
		}
		NavigationController.GetInstance.NavigateTo<ConfigView>();
	}

	private void DisconnectAllConnectedDevices()
	{
		foreach (IDevice connectedDevice in ApplicationContext.DeviceManager.ConnectedDevices)
		{
			ApplicationContext.Disconnect(connectedDevice);
		}
	}
}
