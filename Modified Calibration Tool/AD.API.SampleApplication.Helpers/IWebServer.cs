using System;

namespace AD.API.SampleApplication.Helpers;

public interface IWebServer
{
	event EventHandler<string> RequestRecieved;

	event EventHandler<string> DataRecieved;

	void Start(int port);

	void Stop();
}
