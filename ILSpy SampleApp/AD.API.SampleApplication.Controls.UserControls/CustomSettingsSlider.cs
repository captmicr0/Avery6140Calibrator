using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AD.API.SampleApplication.Controls.UserControls;

public partial class CustomSettingsSlider : UserControl, IComponentConnector
{
	public static readonly DependencyProperty HorizontalSliderValueProperty = DependencyProperty.Register("HorizontalSliderValue", typeof(double), typeof(CustomSettingsSlider), new UIPropertyMetadata(0.0));

	public double HorizontalSliderValue
	{
		get
		{
			return (double)GetValue(HorizontalSliderValueProperty);
		}
		set
		{
			SetValue(HorizontalSliderValueProperty, value);
		}
	}

	public int MinimumValue { get; set; }

	public int MaximumValue { get; set; }

	public CustomSettingsSlider()
	{
		InitializeComponent();
		LayoutGrid.DataContext = this;
	}
}
