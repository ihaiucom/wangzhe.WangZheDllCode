using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using System;
using System.Diagnostics;
using UnityEngine;

public static class SGameApplication
{
	public static void Quit()
	{
		try
		{
			Singleton<NetworkModule>.GetInstance().CloseAllServerConnect();
			MonoSingleton<TGPSDKSys>.GetInstance().OnApplicationQuit();
			Process.GetCurrentProcess().Kill();
		}
		catch (Exception)
		{
			Application.Quit();
		}
	}
}
