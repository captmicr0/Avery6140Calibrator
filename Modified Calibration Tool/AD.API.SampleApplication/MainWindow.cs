using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using AD.API.Resources;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.SampleApplication.Views.Connection;
using AD.API.SampleApplication.Views.Scenarios;
using AD.API.Win8Desktop.Resources;
using AD.Win8Desktop.Configuration;

namespace AD.API.SampleApplication;

public partial class MainWindow : Window, IComponentConnector
{
	public MainWindow()
	{
		InitializeComponent();
		MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
		try
		{
			NavigationController.GetInstance.MainWindow = this;
			NavigationController.GetInstance.NavigateTo<ConnectionView>();
		}
		catch (Exception ex)
		{
			if (ex.InnerException.InnerException is UnauthorizedAccessException)
			{
				MessageBox.Show("Application cannot extract resources, please change the permission to the folder with files or \"Run as administrator\"", "Error - Not enough permissions!", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			Application.Current.Shutdown();
		}
	}

	private static void RegisterResources()
	{
		try
		{
			string resourcesDirectory = Path.Combine(ConfigurationSettings.AssemblyDirectory, "Resources");
			ApplicationContext.ResourceManager.InitializeResourcePath(resourcesDirectory);
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Font, "Arial", resourcesDirectory, "Arial.ttf");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Font, "Roboto Bold", resourcesDirectory, "Roboto-Bold.ttf");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Font, "Helvetica Neue Bold", resourcesDirectory, "HelveticaNeueBold.ttf");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Font, "Helvetica Neue", resourcesDirectory, "HelveticaNeue.ttf");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Graphic, "Avery Logo", resourcesDirectory, "AveryDennisonLogo.png");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "Sample Print LNT", resourcesDirectory, "SamplePrint.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "Calibration Template LNT", resourcesDirectory, "CalibrationTemplate.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "Quantity Test LNT", resourcesDirectory, "QuantityTest.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "Bold Italic Underline LNT", resourcesDirectory, "BoldItalicUnderline.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "Bold Italic Underline MIX LNT", resourcesDirectory, "BoldItalicUnderlineMixed.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "Bold Italic Underline New Line", resourcesDirectory, "BoldItalicUnderlineNewLine.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "AmazonLabel", resourcesDirectory, "AmazonLabel.LNT");
			ApplicationContext.ResourceManager.TryRegisterResource(ResourceType.Lnt, "newAmazonLabel", resourcesDirectory, "newAmazonLabel(V4).LNT");
		}
		catch (ApiResourceException ex)
		{
			ApiResourceException ex2 = ex;
			ApiResourceException ex3 = ex2;
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				Messenger.ShowMessage($"Failed to register resources: {ex3.Message}", "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
			});
		}
		catch (Exception ex4)
		{
			Exception ex5 = ex4;
			Exception e = ex5;
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				Messenger.ShowMessage(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
			});
		}
	}

	private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	{
		string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		while (appVersion.EndsWith(".0"))
		{
			appVersion = appVersion.Remove(appVersion.Length - 2);
		}
		base.Title = base.Title + " Version: " + appVersion;
		RegisterResources();
	}
}
