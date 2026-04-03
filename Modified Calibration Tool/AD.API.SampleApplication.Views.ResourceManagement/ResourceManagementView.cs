using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using AD.API.SampleApplication.Tools;
using AD.API.SampleApplication.ViewModels;
using AD.API.SampleApplication.ViewModels.ResourceManagement;

namespace AD.API.SampleApplication.Views.ResourceManagement;

public partial class ResourceManagementView : Page, IComponentConnector
{
	public ResourceManagementView()
	{
		InitializeComponent();
		base.DataContext = new ResourceManagementViewModel();
		((BaseViewModel)base.DataContext).ConnectionLostHandledEvent += delegate(object sender, ConnectionLostHandledEventArgs args)
		{
			if (!args.IsValid)
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					NavigationController.GetInstance.NavigateTo<ResourceManagementView>();
				});
			}
		};
	}

	private void BtnBack_OnClick(object sender, RoutedEventArgs e)
	{
		((BaseViewModel)base.DataContext).Finalization();
		NavigationController.GetInstance.NavigateTo<ConfigView>();
	}

	private void AlltGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		if (((System.Windows.Controls.ListBox)sender).SelectedItems != null && ((System.Windows.Controls.ListBox)sender).SelectedItems.Count > 0)
		{
			((ResourceManagementViewModel)base.DataContext).ResourceRowSelected(((System.Windows.Controls.ListBox)sender).SelectedItems[0].ToString());
			((System.Windows.Controls.ListBox)sender).UnselectAll();
		}
	}

	private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
	{
		FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			((ResourceManagementViewModel)base.DataContext).ResourcePath = openFileDialog.SelectedPath;
		}
	}

	/* [DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	} */
}
