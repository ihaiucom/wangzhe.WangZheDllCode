using System;
using System.Net;
using UnityEngine;

public class HttpDnsPolicy
{
	private static AndroidJavaObject m_dnsJo;

	private static AndroidJavaClass sGSDKPlatformClass;

	public static void Init()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		if (androidJavaClass == null)
		{
			return;
		}
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (@static == null)
		{
			return;
		}
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
		AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.tencent.msdk.dns.MSDKDnsResolver", new object[0]);
		if (androidJavaObject2 == null)
		{
			return;
		}
		HttpDnsPolicy.m_dnsJo = androidJavaObject2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		if (HttpDnsPolicy.m_dnsJo == null)
		{
			return;
		}
		HttpDnsPolicy.m_dnsJo.Call("init", new object[]
		{
			androidJavaObject
		});
	}

	public static string GetHttpDnsIP(string strUrl)
	{
		string text = string.Empty;
		if (AndroidJNI.AttachCurrentThread() != 0)
		{
			return null;
		}
		text = HttpDnsPolicy.m_dnsJo.Call<string>("getAddrByName", new object[]
		{
			strUrl
		});
		Debug.Log(text);
		if (text != null)
		{
			string[] array = text.Split(new char[]
			{
				';'
			});
			text = array[0];
		}
		return text;
	}

	public static string GetHostByName(string domain)
	{
		IPAddress iPAddress;
		if (IPAddress.TryParse(domain, out iPAddress))
		{
			return domain;
		}
		if (!MonoSingleton<TdirMgr>.GetInstance().isUseHttpDns)
		{
			return domain;
		}
		string httpDnsIP = HttpDnsPolicy.GetHttpDnsIP(domain);
		if (string.IsNullOrEmpty(httpDnsIP))
		{
			return domain;
		}
		return httpDnsIP;
	}
}
