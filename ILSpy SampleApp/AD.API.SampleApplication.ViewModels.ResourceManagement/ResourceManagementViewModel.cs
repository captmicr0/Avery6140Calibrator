using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AD.API.Core.Exceptions;
using AD.API.Resources;
using AD.API.SampleApplication.Commands;
using AD.API.SampleApplication.Controls;
using AD.API.SampleApplication.Models;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.Tools.MessageManagement;
using AD.API.Win8Desktop.Resources;
using Microsoft.Win32;

namespace AD.API.SampleApplication.ViewModels.ResourceManagement;

internal class ResourceManagementViewModel : BaseViewModel
{
	private Command addNewResourseCommand;

	private ResourceType CurrentResourceType;

	public List<string> GetFontList => ApplicationContext.ResourceManager.GetResourceList(ResourceType.Font);

	public List<string> GetGraphicList => ApplicationContext.ResourceManager.GetResourceList(ResourceType.Graphic);

	public List<string> GetLntList => ApplicationContext.ResourceManager.GetResourceList(ResourceType.Lnt);

	public List<string> GetAllList => ApplicationContext.ResourceManager.GetResourceList(ResourceType.All);

	public string ResourcePath
	{
		get
		{
			return ApplicationContext.ResourceManager.ResourcePath;
		}
		set
		{
			if (!string.Equals(ApplicationContext.ResourceManager.ResourcePath, value, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					ApplicationContext.ResourceManager.InitializeResourcePath(value);
					OnPropertyChanged(string.Empty);
				}
				catch (ApiResourceException ex)
				{
					Messenger.ShowErrorMessage(ex);
				}
			}
		}
	}

	public Command AddNewResourseCommand => addNewResourseCommand;

	public ResourceManagementViewModel()
	{
		addNewResourseCommand = new Command(AddNewResourseMethod);
	}

	private ResourceType? CheckResourceType(string alias)
	{
		try
		{
			if (ApplicationContext.ResourceManager.CheckResource(ResourceType.Font, alias))
			{
				return ResourceType.Font;
			}
			if (ApplicationContext.ResourceManager.CheckResource(ResourceType.Graphic, alias))
			{
				return ResourceType.Graphic;
			}
			if (ApplicationContext.ResourceManager.CheckResource(ResourceType.Lnt, alias))
			{
				return ResourceType.Lnt;
			}
		}
		catch (ApiResourceException ex)
		{
			Messenger.ShowErrorMessage(ex);
		}
		return null;
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

	public void ResourceRowSelected(string alias)
	{
		if (CheckResourceType(alias).HasValue)
		{
			ResourceType currentResourceType = CheckResourceType(alias).Value;
			if (currentResourceType == ResourceType.Lnt)
			{
				LntAction lntAction = new LntAction("The resource " + alias + " is present in the system.\nDo you want to print it or delete it?");
				lntAction.ShowDialog();
				switch (lntAction.CurrentAction)
				{
				case ResourceActions.Print:
				{
					List<byte[]> args = new List<byte[]>();
					if (ApplicationContext.CurrentDevice != null)
					{
						try
						{
							ApplicationContext.CurrentDevice.Printer.Print(alias, 0, args);
						}
						catch (ApiPrinterException ex2)
						{
							Messenger.ShowErrorMessage(ex2);
						}
					}
					else
					{
						Messenger.ShowMessage("No device connected. \r\nPlease connect first.", "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
					break;
				}
				case ResourceActions.Delete:
					try
					{
						ApplicationContext.ResourceManager.UnregisterResource(currentResourceType, alias);
					}
					catch (ApiPrinterException ex)
					{
						Messenger.ShowErrorMessage(ex);
					}
					break;
				}
			}
			else
			{
				ResourceAction resourceAction = new ResourceAction("Resource " + alias + " is present in the system.\nDo you want to delete it?");
				resourceAction.ShowDialog();
				if (resourceAction.CurrentAction == ResourceActions.Delete)
				{
					try
					{
						ApplicationContext.ResourceManager.UnregisterResource(CheckResourceType(alias).Value, alias);
					}
					catch (ApiResourceException ex3)
					{
						Messenger.ShowErrorMessage(ex3);
					}
				}
			}
		}
		OnPropertyChanged(string.Empty);
	}

	private void AddNewResourseMethod()
	{
		OpenFileDialog dlg = new OpenFileDialog();
		if (dlg.ShowDialog() != true)
		{
			return;
		}
		string filename = dlg.FileName;
		switch ((Path.GetExtension(filename) ?? string.Empty).ToLowerInvariant())
		{
		case ".jpg":
		case ".jpeg":
		case ".png":
		case ".bmp":
		case ".tiff":
		case ".gif":
		case ".tif":
			CurrentResourceType = ResourceType.Graphic;
			break;
		case ".job":
		case ".xml":
		case ".lnt":
			CurrentResourceType = ResourceType.Lnt;
			break;
		case ".ttf":
		case ".fon":
		case ".fnt":
			CurrentResourceType = ResourceType.Font;
			break;
		default:
			MessageBox.Show("You have selected the improper file.This file " + filename + "\nis not recognized as a proper resource file.");
			return;
		}
		_ = CurrentResourceType;
		AliasDialog aliasDlg = new AliasDialog();
		if (!aliasDlg.ShowDialog().Value)
		{
			return;
		}
		try
		{
			if (GetAllList.FirstOrDefault((string s) => string.Equals(s, aliasDlg.ResourceAlias, StringComparison.InvariantCulture)) != null)
			{
				Messenger.ShowMessage("Resource with such alias name already exists. Please try again to add the resource and ender different alias name", "Alias error", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			ApplicationContext.ResourceManager.TryRegisterResource(CurrentResourceType, aliasDlg.ResourceAlias, Path.GetDirectoryName(filename), Path.GetFileName(filename));
			OnPropertyChanged(string.Empty);
		}
		catch (ApiResourceException ex)
		{
			Messenger.ShowErrorMessage(ex);
		}
	}
}
