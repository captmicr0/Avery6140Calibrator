using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AD.API.SampleApplication.Controls.CustomControls;

public class CustomThumb : Thumb
{
	private TouchDevice currentDevice;

	static CustomThumb()
	{
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomThumb), new FrameworkPropertyMetadata(typeof(CustomThumb)));
	}

	protected override void OnPreviewTouchDown(TouchEventArgs e)
	{
		ReleaseCurrentDevice();
		CaptureCurrentDevice(e);
	}

	protected override void OnPreviewTouchUp(TouchEventArgs e)
	{
		ReleaseCurrentDevice();
	}

	protected override void OnLostTouchCapture(TouchEventArgs e)
	{
		if (currentDevice != null)
		{
			CaptureCurrentDevice(e);
		}
	}

	private void ReleaseCurrentDevice()
	{
		if (currentDevice != null)
		{
			TouchDevice temp = currentDevice;
			currentDevice = null;
			ReleaseTouchCapture(temp);
		}
	}

	private void CaptureCurrentDevice(TouchEventArgs e)
	{
		if (CaptureTouch(e.TouchDevice))
		{
			currentDevice = e.TouchDevice;
		}
	}
}
