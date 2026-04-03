using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace AD.API.SampleApplication.Controls.UserControls;

public partial class DeviceNavigationControl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty SelectedDeviceSerialProperty = DependencyProperty.Register("SelectedDeviceSerialValue", typeof(string), typeof(DeviceNavigationControl), new UIPropertyMetadata((object)null));

	public static DependencyProperty CommandPreviousProperty = DependencyProperty.Register("CommandPrevious", typeof(ICommand), typeof(DeviceNavigationControl), new PropertyMetadata(null));

	public static DependencyProperty CommandNextProperty = DependencyProperty.Register("CommandNext", typeof(ICommand), typeof(DeviceNavigationControl), new PropertyMetadata(null));

	public ICommand CommandPrevious
	{
		get
		{
			return (ICommand)GetValue(CommandPreviousProperty);
		}
		set
		{
			SetValue(CommandPreviousProperty, value);
		}
	}

	public ICommand CommandNext
	{
		get
		{
			return (ICommand)GetValue(CommandNextProperty);
		}
		set
		{
			SetValue(CommandNextProperty, value);
		}
	}

	public string SelectedDeviceSerialValue
	{
		get
		{
			return (string)GetValue(SelectedDeviceSerialProperty);
		}
		set
		{
			SetValue(SelectedDeviceSerialProperty, value);
		}
	}

	public DeviceNavigationControl()
	{
		InitializeComponent();
		LayoutGrid.DataContext = this;
	}
}
