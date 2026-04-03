using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AD.API.Devices.Management;

namespace AD.API.SampleApplication.Controls.UserControls;

public partial class DeviceConnectionItem : UserControl, IComponentConnector
{
	private const string CONNECT = "Connect";

	private const string DISCONNECT = "Disconnect";

	private DeviceConnectionInfo _connectionInfo;

	private bool _isConnected;

	public DeviceConnectionInfo ConnectionInfo => _connectionInfo;

	public bool IsConnected
	{
		get
		{
			return _isConnected;
		}
		set
		{
			if (value != _isConnected)
			{
				_isConnected = value;
				ChangeConnectionButtonText();
			}
		}
	}

	private event EventHandler _actionButtonClick;

	public event EventHandler ActionButtonClick
	{
		add
		{
			_actionButtonClick += value;
		}
		remove
		{
			_actionButtonClick -= value;
		}
	}

	public DeviceConnectionItem()
	{
		InitializeComponent();
	}

	public DeviceConnectionItem(DeviceConnectionInfo connectionInfo)
	{
		InitializeComponent();
		_connectionInfo = connectionInfo;
		DeviceNameTextBlock.Text = connectionInfo.Name;
		IsConnected = false;
		ChangeConnectionButtonText();
	}

	private void ChangeConnectionButtonText()
	{
		BtnConnect.Content = (_isConnected ? "Disconnect" : "Connect");
	}

	private void btnConnect_Click(object sender, RoutedEventArgs e)
	{
		if (this._actionButtonClick != null)
		{
			this._actionButtonClick(this, new EventArgs());
		}
	}
}
