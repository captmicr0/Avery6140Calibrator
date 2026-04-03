using AD.API.SampleApplication.Models;
using NLog;

namespace AD.API.SampleApplication.Helpers;

internal static class Metrics
{
	public static void LogMetricsEvent(Logger logger, string eventType, string transactionTime, string TBA, string sortZone, string routeCode, string errorCode, string message)
	{
		LogEventInfo metricsEvent = new LogEventInfo(LogLevel.Info, "", "");
		metricsEvent.LoggerName = logger.Name;
		metricsEvent.Properties.Add("action-type", eventType);
		metricsEvent.Properties.Add("captured-duration", transactionTime);
		metricsEvent.Properties.Add("tba", TBA);
		metricsEvent.Properties.Add("sort-zone", sortZone);
		metricsEvent.Properties.Add("route-code", routeCode);
		metricsEvent.Properties.Add("error-code", errorCode);
		metricsEvent.Properties.Add("message", message);
		logger.Log(metricsEvent);
	}

	public static void LogMetricsScan(Logger logger, string TBA)
	{
		LogMetricsEvent(logger, "Scan", "", TBA, "", "", "", "");
	}

	public static void LogMetricsPrint(Logger logger, string transactionTime, string TBA, string sortZone, string routeCode)
	{
		LogMetricsEvent(logger, "Print", transactionTime, TBA, sortZone, routeCode, "", "");
	}

	public static void LogMetricsInfo(Logger logger, string message)
	{
		if (ApplicationContext.infoLoggingEnable)
		{
			LogMetricsEvent(logger, "Info", "", "", "", "", "", message);
		}
	}

	public static void LogMetricsServerInfo(Logger logger, string message)
	{
		if (ApplicationContext.serverLoggingEnable)
		{
			LogMetricsEvent(logger, "Server Info", "", "", "", "", "", message);
		}
	}

	public static void LogMetricsError(Logger logger, int errorCode, string message)
	{
		LogMetricsEvent(logger, "Error", "", "", "", "", errorCode.ToString(), message);
	}

	public static void LogMetricsServerError(Logger logger, int errorCode, string message)
	{
		LogMetricsEvent(logger, "Server Error", "", "", "", "", errorCode.ToString(), message);
	}

	public static void LogMetricsErrorHandled(Logger logger, int errorCode, string message)
	{
		LogMetricsEvent(logger, "Error Handled", "", "", "", "", errorCode.ToString(), message);
	}
}
