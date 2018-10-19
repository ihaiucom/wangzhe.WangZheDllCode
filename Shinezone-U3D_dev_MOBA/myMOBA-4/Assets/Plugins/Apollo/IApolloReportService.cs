using System;
using System.Collections.Generic;

namespace Apollo
{
	public interface IApolloReportService : IApolloServiceBase
	{
		bool ApolloRepoertEvent(string eventName, List<KeyValuePair<string, string>> events, bool isReal);

		void ApolloEnableCrashReport(bool rdm, bool mta);

		void ApolloReportInit(bool rdm = true, bool mta = true);

		[Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
		void EnableBuglyLog(bool enable);

		void SetUserId(string userId);

		[Obsolete("Obsolete since 1.1.16, use ReportException instead")]
		void HandleException(Exception e);

		[Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
		void SetGameObjectForCallback(string gameObject);

		void EnableExceptionHandler(LogSeverity level = LogSeverity.LogException);

		void SetAutoQuitApplication(bool autoExit);

		void RegisterLogCallback(ApolloBuglyLogDelegate handler);

		void ReportException(Exception e);

		void ReportException(Exception e, string message);

		void ReportException(string name, string message, string stackTrace);
	}
}
