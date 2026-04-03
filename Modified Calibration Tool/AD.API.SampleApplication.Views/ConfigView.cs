using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.Views.Printer;
using AD.API.SampleApplication.Views.ResourceManagement;
using AD.API.SampleApplication.Views.Scanner;
using AD.API.SampleApplication.Views.Scenarios;
using AD.API.SampleApplication.Views.SystemConfiguration;

namespace AD.API.SampleApplication.Views;

public partial class ConfigView : Page, IComponentConnector
{
	public ConfigView()
	{
		InitializeComponent();
		base.DataContext = new ConfigViewModel();
	}

	private void BtnPrinter_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<PrinterView>();
	}

	private void BtnScanner_Click(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<ScannerView>();
	}

	private void BtnSystem_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			((BaseViewModel)base.DataContext).Finalization();
			NavigationController.GetInstance.NavigateTo<SystemView>();
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

	private void BtnResourceManagement_Click(object sender, RoutedEventArgs e)
	{
		NavigationController.GetInstance.NavigateTo<ResourceManagementView>();
	}

	private void BtnBack_OnClick(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<ScenariosView>();
	}
}
