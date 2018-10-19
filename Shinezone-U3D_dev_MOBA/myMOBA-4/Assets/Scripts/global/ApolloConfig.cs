using Apollo;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ApolloConfig
{

	public static ApolloPlatform platform = ApolloPlatform.Wechat;

	public static string QQAppID = "1104466820";

	public static string QQAppKey = "LX7Pi1KzPZJD9LlL";

	public static string WXAppID = "wx95a3a4d7c627e07d";

	public static string WXAppKey = "5018f9d67e25213f4f33d546c74624ba";

	public static string MsdkKey = "8f190beb8c306e248ac6a26aca812e1d";

	public static string appID = "1104466820";

	public static string offerID = "1450002258";

	public static string payEnv = "release";

	public static bool payEnabled = true;

	public static string serviceID = "10056";

	public static int maxMessageBufferSize = 512000;

	public static string loginUrl = string.Empty;

	public static string loginOnlyIpOrUrl = string.Empty;

	public static string loginOnlyIp = string.Empty;

	public static ushort loginOnlyVPort = 6629;

	public static ushort echoPort;

	public static ulong Uin;

	public static string Password = string.Empty;

	public static string CustomOpenId = string.Empty;

	public static string loginOnlyIpTongCai = string.Empty;

	public static int ISPType;

	public static string loginHostName = "login.smoba.qq.com";

	public static string qq_android_port = "60612";

	public static string qq_ios_port = "60622";

	public static string serverUrlPath = "server_url.txt";

	public static string GetAppID()
	{
		if (ApolloConfig.platform == ApolloPlatform.Wechat)
		{
			return ApolloConfig.WXAppID;
		}
		return ApolloConfig.appID;
	}

	public static int IsUseCEPackage()
	{
		return 0;
	}

	public static string GetPackageName()
	{
		return "com.tencent.tmgp.sgame";
	}

	public static string GetGameUtilityString()
	{
		return "com.tencent.tmgp.sgame.SGameUtility";
	}

	public static string[] LoadLoginUrl()
	{
		CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource(ApolloConfig.serverUrlPath, typeof(TextAsset), enResourceType.Numeric, false, false).m_content as CBinaryObject;
		if (cBinaryObject == null)
		{
			Debug.LogError(string.Format("Can't find file: {0}", ApolloConfig.serverUrlPath));
			Singleton<CResourceManager>.GetInstance().RemoveCachedResource(ApolloConfig.serverUrlPath);
			return null;
		}
		string @string = Encoding.UTF8.GetString(cBinaryObject.m_data);
		Singleton<CResourceManager>.GetInstance().RemoveCachedResource(ApolloConfig.serverUrlPath);
		string[] array = @string.Split(new char[]
		{
			'\r',
			'\t',
			' ',
			'\n',
			';'
		}, StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			string[] array2 = text.Split(new char[]
			{
				'.'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length == 4)
			{
				bool flag = true;
				for (int j = 0; j < array2.Length; j++)
				{
					try
					{
						Convert.ToUInt16(array2[j]);
					}
					catch
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(text);
				}
			}
		}
		if (list.Count != 3)
		{
			Debug.LogError(string.Format("Invalid server list file: {0}", ApolloConfig.serverUrlPath));
			return null;
		}
		return list.ToArray();
	}
}
