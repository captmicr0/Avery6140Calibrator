using System;

namespace AD.API.SampleApplication.Tools;

public class ConnectionLostHandledEventArgs : EventArgs
{
	public bool IsValid { get; private set; }

	public ConnectionLostHandledEventArgs(bool isValid)
	{
		IsValid = isValid;
	}
}
