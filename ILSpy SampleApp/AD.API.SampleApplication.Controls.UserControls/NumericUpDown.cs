using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AD.API.SampleApplication.Controls.UserControls;

public partial class NumericUpDown : UserControl, INotifyPropertyChanged, IComponentConnector
{
	public static readonly DependencyProperty IndicatorValueProperty = DependencyProperty.Register("IndicatorValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));

	public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("MinimumValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));

	public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register("MaximumValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));

	public int IndicatorValue
	{
		get
		{
			return (int)GetValue(IndicatorValueProperty);
		}
		set
		{
			SetValue(IndicatorValueProperty, value);
		}
	}

	public int MinimumValue
	{
		get
		{
			return (int)GetValue(MinimumValueProperty);
		}
		set
		{
			SetValue(MinimumValueProperty, value);
		}
	}

	public int MaximumValue
	{
		get
		{
			return (int)GetValue(MaximumValueProperty);
		}
		set
		{
			SetValue(MaximumValueProperty, value);
		}
	}

	public bool MaxReached => IndicatorValue < MaximumValue;

	public bool MinReached => IndicatorValue > MinimumValue;

	public event PropertyChangedEventHandler PropertyChanged;

	public NumericUpDown()
	{
		InitializeComponent();
		IndicatorLabel.DataContext = this;
		DecreaseButton.DataContext = this;
		IncreaseButton.DataContext = this;
	}

	private void DecreaseButton_OnClick(object sender, RoutedEventArgs e)
	{
		if (IndicatorValue > MinimumValue)
		{
			IndicatorValue--;
		}
		OnPropertyChanged(string.Empty);
	}

	private void IncreaseButton_OnClick(object sender, RoutedEventArgs e)
	{
		if (IndicatorValue < MaximumValue)
		{
			IndicatorValue++;
		}
		OnPropertyChanged(string.Empty);
	}

	protected void OnPropertyChanged(string property)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}
}
