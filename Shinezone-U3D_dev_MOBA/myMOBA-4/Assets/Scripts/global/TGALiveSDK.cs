using System;
using UnityEngine;

public class TGALiveSDK
{
	private static string className = "com.tencent.tga.livesdk.TGAPluginManager";

	private static TGALiveSDK sTGALiveSDK;

	private AndroidJavaClass pluginManagerClass;

	private TGALiveSDK()
	{
		this.pluginManagerClass = new AndroidJavaClass(TGALiveSDK.className);
	}

	private static TGALiveSDK instance()
	{
		if (TGALiveSDK.sTGALiveSDK == null)
		{
			TGALiveSDK.sTGALiveSDK = new TGALiveSDK();
		}
		return TGALiveSDK.sTGALiveSDK;
	}

	public static void start(string token, int postion)
	{
		Debug.Log("pre click");
		TGALiveSDK.instance().pluginManagerClass.CallStatic("firePlugin", new object[]
		{
			token,
			postion
		});
		Debug.Log("post click");
	}

	public static bool available()
	{
		Debug.Log("pre available");
		return TGALiveSDK.instance().pluginManagerClass.CallStatic<bool>("available", new object[0]);
	}

	public static void init(string jsonString)
	{
		Debug.Log("pre init");
		string value = ",\"unityVersion\":\"" + Application.unityVersion + "\"";
		jsonString = jsonString.Insert(jsonString.Length - 1, value);
		TGALiveSDK.instance().pluginManagerClass.CallStatic("init", new object[]
		{
			jsonString
		});
		Debug.Log(jsonString);
	}

	public static void battleInvitation(string jsonString)
	{
		Debug.Log("battleInvitation");
		TGALiveSDK.instance().pluginManagerClass.CallStatic("battleInvitation", new object[]
		{
			jsonString
		});
	}
}
