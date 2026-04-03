using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AD.API.SampleApplication.Models;
using NLog;

namespace AD.API.SampleApplication.Helpers;

public sealed class AveryWebServer : IWebServer
{
	private enum logLevel
	{
		debug,
		info,
		error
	}

	internal const string ClassId = "24a6311e-5a90-48ca-a988-9da2983b4f07";

	internal const string InterfaceId = "99278cea-a9ab-4bae-a22d-f63352d0186f";

	private static Logger logger = LogManager.GetLogger("MetricsLogger");

	private const uint BufferSize = 8192u;

	private TcpListener tcpListener;

	private CancellationTokenSource cancellation;

	public event EventHandler<string> RequestRecieved;

	public event EventHandler<string> DataRecieved;

	public AveryWebServer()
	{
		Log("New AveryWebServer instance created!", logLevel.info);
	}

	public void Start(int port)
	{
		Log("Starting TCPListener on port " + port, logLevel.info);
		IPAddress localAddr = IPAddress.Parse("127.0.0.1");
		if (cancellation == null)
		{
			cancellation = new CancellationTokenSource();
		}
		if (tcpListener == null)
		{
			tcpListener = new TcpListener(localAddr, port);
		}
		try
		{
			tcpListener.Start();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Exception Starting Port Listener: ");
			Console.WriteLine(ex.Message);
			Log("Exception Starting Port Listener:" + ex.Message, logLevel.error);
			return;
		}
		Log("TCPListener started", logLevel.info);
		ListenAsync(cancellation.Token);
	}

	public void Stop()
	{
		if (tcpListener != null && cancellation != null)
		{
			Log("Stopping the TCPListener...", logLevel.info);
			cancellation.Cancel();
			tcpListener.Stop();
			Log("TCPListener stopped", logLevel.info);
		}
	}

	private async Task ListenAsync(CancellationToken token)
	{
		Log("Begining to listen for incomming connections", logLevel.info);
		while (!token.IsCancellationRequested)
		{
			Log("Waiting for connection[s]....", logLevel.info);
			TcpClient client = await tcpListener.AcceptTcpClientAsync();
			Log("Connected!", logLevel.info);
			await Task.Run(async delegate
			{
				while (client.Connected && !token.IsCancellationRequested)
				{
					StringBuilder request = new StringBuilder();
					int contentLength = 0;
					NetworkStream networkStream = client.GetStream();
					StreamReader streamReader = new StreamReader(networkStream);
					Log("Waiting for Data from COMP", logLevel.info);
					if (streamReader.EndOfStream)
					{
						Log("No Data recieved from COMP.", logLevel.error);
						Log("Closing Connection after not recieving a request...", logLevel.info);
						client.Client.Disconnect(reuseSocket: false);
						client.Close();
						Log("Connection closed", logLevel.info);
						break;
					}
					string tempString = await streamReader.ReadLineAsync();
					while (tempString != "")
					{
						Log("Line Read from COMP: " + tempString, logLevel.info);
						if (tempString.StartsWith("Content-Length: "))
						{
							contentLength = Convert.ToInt32(tempString.Substring("Content-Length: ".Length));
						}
						request.AppendLine(tempString);
						tempString = await streamReader.ReadLineAsync();
					}
					request.AppendLine(tempString);
					char[] content = new char[contentLength];
					await streamReader.ReadBlockAsync(content, 0, contentLength);
					request.AppendLine(new string(content));
					Log("Request recieved: " + request, logLevel.info);
					string[] array = request.ToString().Split('\n')[0].Split(' ');
					string header = "";
					if (array[0] == "POST")
					{
						string dateTimeString = DateTime.UtcNow.ToString("ddd, d MMMM yyyy HH:mm:ss 'GMT'");
						header = string.Format("HTTP/1.1 200 OK\r\nAccess-Control-Allow-Methods: POST, OPTIONS\r\nAccess-Control-Allow-Origin: *\r\nAllow: POST, OPTIONS\r\nDate: " + dateTimeString + "\r\nServer: Microsoft-HTTPAPI/2.0\r\nTransfer-Encoding: chunked\r\n");
					}
					Log("Sending Response: " + header, logLevel.info);
					byte[] headerArray = Encoding.UTF8.GetBytes(header);
					await networkStream.WriteAsync(headerArray, 0, headerArray.Length);
					await networkStream.FlushAsync();
					string[] requestLines = request.ToString().Split('\n');
					contentLength = -1;
					uint i = 0u;
					while (contentLength < 0)
					{
						string[] headerLine = requestLines[i].Split(' ');
						if (headerLine[0] == "Content-Length:")
						{
							contentLength = Convert.ToInt32(headerLine[1]);
						}
						else
						{
							i++;
						}
					}
					if (contentLength > 0)
					{
						string data = request.ToString();
						data = data.Substring(data.IndexOf("\r\n\r\n") + "\r\n\r\n".Length);
						FireDataRecievedEvent(data);
					}
					else
					{
						FireDataRecievedEvent("");
					}
					Log("Closing Connection after processing the request...", logLevel.info);
					client.Client.Disconnect(reuseSocket: false);
					client.Close();
					Log("Connection closed", logLevel.info);
				}
			}, token);
		}
	}

	private void FireRequestRecievedEvent(string request)
	{
		Log("Firing Request Revieced Event: " + request, logLevel.info);
		this.RequestRecieved(this, request);
	}

	private void FireDataRecievedEvent(string data)
	{
		Log("Firing Data Recieved Event: " + data, logLevel.info);
		this.DataRecieved(this, data);
	}

	private void Log(string logMessage, logLevel logLevel)
	{
		Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + " : " + logMessage);
		if (ApplicationContext.serverLoggingEnable)
		{
			switch (logLevel)
			{
			default:
				Metrics.LogMetricsServerInfo(logger, logMessage);
				break;
			case logLevel.info:
				Metrics.LogMetricsServerInfo(logger, logMessage);
				break;
			case logLevel.error:
				Metrics.LogMetricsServerError(logger, -2, logMessage);
				break;
			}
		}
	}
}
