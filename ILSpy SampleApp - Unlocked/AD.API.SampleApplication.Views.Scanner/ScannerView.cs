using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.Scanner;

namespace AD.API.SampleApplication.Views.Scanner;

public partial class ScannerView : Page, IComponentConnector
{
	public ScannerView()
	{
		InitializeComponent();
		base.DataContext = new ScannerViewModel();
		((BaseViewModel)base.DataContext).ConnectionLostHandledEvent += delegate(object sender, ConnectionLostHandledEventArgs args)
		{
			if (!args.IsValid)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					Messenger.ShowNoDeviceAvailable();
					((BaseViewModel)base.DataContext).Finalization();
					base.DataContext = null;
					NavigationController.GetInstance.NavigateTo<ConfigView>();
				});
			}
		};
	}

	private void BtnBack_OnClick(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<ConfigView>();
	}

	private void ScannerSettingsButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			((BaseViewModel)base.DataContext).Finalization();
			NavigationController.GetInstance.NavigateTo<ScannerSettingsView>();
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

	private void ScannedItemsTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
	{
		if (ScannedItemsTextBox.LineCount != -1)
		{
			ScannedItemsTextBox.ScrollToLine(ScannedItemsTextBox.LineCount - 1);
		}
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
