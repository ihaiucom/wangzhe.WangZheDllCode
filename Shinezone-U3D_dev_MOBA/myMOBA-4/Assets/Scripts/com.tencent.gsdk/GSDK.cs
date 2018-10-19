using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace com.tencent.gsdk
{
	public class GSDK
	{
		public delegate void GSDKObserver(StartSpeedRet ret);

		public delegate void GSDKKartinObserver(KartinRet ret);

		private static AndroidJavaClass sGSDKPlatformClass;

        private static event GSDK.GSDKObserver sSpeedNotifyEvent;

        private static event GSDK.GSDKKartinObserver sKartinNotifyEvent;

		public static void Init(string appid, bool debug, int zoneid)
		{
			try
			{
				GSDKUtils.isDebug = debug;
				GSDKUtils.Logger("gsdk mna init");
				GSDK.sGSDKPlatformClass = new AndroidJavaClass("com.tencent.gsdk.GSDKPlatform");
				AndroidJavaObject androidJavaObject = null;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				if (GSDK.sGSDKPlatformClass != null)
				{
					androidJavaObject.Call("runOnUiThread", new object[]
					{
						new AndroidJavaRunnable(delegate
						{
							GSDK.sGSDKPlatformClass.CallStatic("GSDKInit", new object[]
							{
								appid,
								debug,
								zoneid
							});
							GSDK.sGSDKPlatformClass.CallStatic("GSDKSetObserver", new object[]
							{
								GSDKAndroidObserver.Instance
							});
						})
					});
				}
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void GoBack()
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKGoBack", new object[0]);
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void GoFront()
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKGoFront", new object[0]);
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void SetUserName(int plat, string openid)
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKSetUserName", new object[]
				{
					plat,
					openid
				});
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void SetZoneId(int zoneid)
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKSetZoneId", new object[]
				{
					zoneid
				});
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void StartSpeed(string vip, int vport, int htype, string hookModules, int zoneid, int reserved)
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKStartSpeed", new object[]
				{
					vip,
					vport,
					htype,
					hookModules,
					zoneid,
					reserved
				});
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void EndSpeed(string vip, int vport)
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKEndSpeed", new object[]
				{
					vip,
					vport
				});
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static string EndSpeedWithKartinResult(string vip, int vport)
		{
			string result;
			try
			{
				string text = GSDK.sGSDKPlatformClass.CallStatic<string>("GSDKEndSpeed2", new object[]
				{
					vip,
					vport
				});
				GSDKUtils.Logger(text);
				result = text;
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
				result = string.Empty;
			}
			return result;
		}

		public static SpeedInfo GetSpeedInfo(string vip, int vport)
		{
			SpeedInfo speedInfo = new SpeedInfo();
			try
			{
				AndroidJavaObject androidJavaObject = GSDK.sGSDKPlatformClass.CallStatic<AndroidJavaObject>("GSDKGetSpeedInfo", new object[]
				{
					vip,
					vport
				});
				speedInfo.state = androidJavaObject.Get<int>("state");
				speedInfo.netType = androidJavaObject.Get<int>("netType");
				speedInfo.delay = androidJavaObject.Get<int>("delay");
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
			return speedInfo;
		}

		public static void QueryKartin(string tag)
		{
			try
			{
				GSDK.sGSDKPlatformClass.CallStatic("GSDKQueryKartin", new object[]
				{
					tag
				});
			}
			catch (Exception ex)
			{
				Debug.Log("catch ex " + ex.ToString());
			}
		}

		public static void SetObserver(GSDK.GSDKObserver d)
		{
			if (d == null)
			{
				return;
			}
			GSDK.sSpeedNotifyEvent = (GSDK.GSDKObserver)Delegate.Combine(GSDK.sSpeedNotifyEvent, d);
		}

		public static void SetKartinObserver(GSDK.GSDKKartinObserver d)
		{
			if (d == null)
			{
				return;
			}
			GSDK.sKartinNotifyEvent = (GSDK.GSDKKartinObserver)Delegate.Combine(GSDK.sKartinNotifyEvent, d);
		}

		internal static void notify(StartSpeedRet ret)
		{
			if (GSDK.sSpeedNotifyEvent != null)
			{
				GSDK.sSpeedNotifyEvent(ret);
			}
		}

		internal static void notify(KartinRet ret)
		{
			if (GSDK.sKartinNotifyEvent != null)
			{
				GSDK.sKartinNotifyEvent(ret);
			}
		}
	}
}
