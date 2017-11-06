using System;
using UnityEngine;

public class BuglyInit : MonoBehaviour
{
	private const string BuglyAppID = "YOUR APP ID GOES HERE";

	private void Awake()
	{
		BuglyAgent.ConfigDebugMode(false);
		BuglyAgent.ConfigDefault(null, null, null, 0L);
		BuglyAgent.ConfigAutoReportLogLevel(LogSeverity.LogError);
		BuglyAgent.ConfigAutoQuitApplication(false);
		BuglyAgent.RegisterLogCallback(null);
		BuglyAgent.InitWithAppId("YOUR APP ID GOES HERE");
		Object.Destroy(this);
	}
}
