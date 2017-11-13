using com.tencent.pandora.MiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
	public class MidasUtil : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
		}

		public static bool AndroidPay(string jsonParams)
		{
			try
			{
				string env = string.Empty;
				string offerId = string.Empty;
				string openId = string.Empty;
				string openKey = string.Empty;
				string sessionId = string.Empty;
				string sessionType = string.Empty;
				string zoneId = string.Empty;
				string pf = string.Empty;
				string pfKey = string.Empty;
				string goodsTokenUrl = string.Empty;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary = (Json.Deserialize(jsonParams) as Dictionary<string, object>);
				if (dictionary.ContainsKey("env"))
				{
					env = (dictionary.get_Item("env") as string);
				}
				if (dictionary.ContainsKey("offerId"))
				{
					offerId = (dictionary.get_Item("offerId") as string);
				}
				if (dictionary.ContainsKey("openId"))
				{
					openId = (dictionary.get_Item("openId") as string);
				}
				if (dictionary.ContainsKey("openKey"))
				{
					openKey = (dictionary.get_Item("openKey") as string);
				}
				if (dictionary.ContainsKey("sessionId"))
				{
					sessionId = (dictionary.get_Item("sessionId") as string);
				}
				if (dictionary.ContainsKey("sessionType"))
				{
					sessionType = (dictionary.get_Item("sessionType") as string);
				}
				if (dictionary.ContainsKey("zoneId"))
				{
					zoneId = (dictionary.get_Item("zoneId") as string);
				}
				if (dictionary.ContainsKey("pf"))
				{
					pf = (dictionary.get_Item("pf") as string);
				}
				if (dictionary.ContainsKey("pfKey"))
				{
					pfKey = (dictionary.get_Item("pfKey") as string);
				}
				if (dictionary.ContainsKey("goodsTokenUrl"))
				{
					goodsTokenUrl = (dictionary.get_Item("goodsTokenUrl") as string);
				}
				AndroidJavaClass midasWrap = new AndroidJavaClass("com.tencent.pandora.MidasWrap");
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				activity.Call("runOnUiThread", new object[]
				{
					delegate
					{
						midasWrap.CallStatic("launchPay", new object[]
						{
							activity,
							env,
							offerId,
							openId,
							openKey,
							sessionId,
							sessionType,
							zoneId,
							pf,
							pfKey,
							goodsTokenUrl,
							"Pandora GameObject",
							"PayCallback"
						});
					}
				});
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_Message());
				return false;
			}
			return true;
		}

		public void PayCallback(string jsonResult)
		{
			CSharpInterface.NotifyAndroidPayFinish(jsonResult);
		}
	}
}
