using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.Barcodes;
using AD.API.SampleApplication.Views.Barcodes.OneDimentional;
using AD.API.SampleApplication.Views.Barcodes.TwoDimentional;
using AD.API.SampleApplication.Views.Scanner;

namespace AD.API.SampleApplication.Views.Barcodes;

public partial class BarcodeListView : BaseBarcodeView, IComponentConnector
{
	public BarcodeListView()
	{
		InitializeComponent();
		try
		{
			base.DataContext = new BarcodeListViewModel();
			SubscribeHandlePostConnectionLost((BaseViewModel)base.DataContext);
		}
		catch (ApiException ex)
		{
			ApiException ex2 = ex;
			ApiException apiException = ex2;
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				Messenger.ShowErrorMessage(apiException);
				Messenger.ShowMessage("Device is in error state. You will be navigated to Main Screen, please clear the device error from \"Printer Screen\"", MessageSynchronizer.ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Hand);
				NavigationController.GetInstance.NavigateTo<MainView>();
			});
		}
		catch (Exception ex3)
		{
			Exception ex4 = ex3;
			Exception exception = ex4;
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				Messenger.ShowMessage(exception.Message + "\r\nYou will be navigated to \"Main Screen\".", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				NavigationController.GetInstance.NavigateTo<MainView>();
			});
		}
	}

	private void BackButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			((BarcodeListViewModel)base.DataContext).EndScannerSetSession();
			((BaseViewModel)base.DataContext).Finalization();
		}
		catch (ApiException ex)
		{
			Messenger.ShowErrorMessage(ex);
		}
		catch (Exception ex2)
		{
			Messenger.ShowMessage(ex2.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		if (!NavigationController.GetInstance.NavigateTo<ScannerSettingsView>())
		{
			Messenger.ShowMessage("Unable to load the view. \r\nRedirecting to Main Screen", "Navigation error", MessageBoxButton.OK, MessageBoxImage.Hand);
			NavigationController.GetInstance.NavigateTo<MainView>();
		}
	}

	private void CodabarButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<CodabarView>();
	}

	private void Code93Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<Code93View>();
	}

	private void Code39Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<Code39View>();
	}

	private void Gs1Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<Gs1View>();
	}

	private void Code128Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<Code128View>();
	}

	private void I2Of5Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<I2Of5View>();
	}

	private void DataMatrixButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<DataMatrixView>();
	}

	private void Gs12DButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<Gs1ExtensionView>();
	}

	private void MsiButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<MsiView>();
	}

	private void MaxiCodeButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<MaxiCodeView>();
	}

	private void D2Of5Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<D2Of5View>();
	}

	private void Pdf417Button_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<Pdf417View>();
	}

	private void UpcButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<UpcView>();
	}

	private void QrCodeButton_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<QrCodeView>();
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
