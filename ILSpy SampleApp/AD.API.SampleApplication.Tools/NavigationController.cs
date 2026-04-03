using System;
using System.Windows;
using System.Windows.Controls;
using AD.API.Core.Exceptions;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.ViewModels;

namespace AD.API.SampleApplication.Tools;

public class NavigationController
{
	private static NavigationController instance;

	private MainWindow mainWindow;

	public MainWindow MainWindow
	{
		set
		{
			mainWindow = value;
		}
	}

	public static NavigationController GetInstance => instance ?? (instance = new NavigationController());

	public bool NavigateTo<TTargetView>() where TTargetView : Page
	{
		return NavigateTo<TTargetView>(null);
	}

	public bool NavigateTo<TTargetView>(object data) where TTargetView : Page
	{
		try
		{
			Page viewToNavigate = (Page)Activator.CreateInstance(typeof(TTargetView));
			ApplicationContext.CurrentViewModel = (BaseViewModel)viewToNavigate.DataContext;
			mainWindow.MainFrame.Navigate(viewToNavigate);
			return true;
		}
		catch (ApiException ex)
		{
			ApiException ex2 = ex;
			ApiException apiException = ex2;
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				Messenger.ShowErrorMessage(apiException);
				Messenger.ShowMessage("Navigation will not occur.", MessageSynchronizer.ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Hand);
			});
			return false;
		}
		catch (Exception ex3)
		{
			Exception ex4 = ex3;
			Exception exception = ex4;
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				if (exception.InnerException != null)
				{
					Messenger.ShowMessage(exception.InnerException.Message + "\r\nNavigation will not occur.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				else
				{
					Messenger.ShowMessage(exception.Message + "\r\nNavigation will not occur.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
			});
			return false;
		}
	}
}
