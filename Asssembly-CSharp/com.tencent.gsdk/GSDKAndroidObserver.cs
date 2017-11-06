using System;
using UnityEngine;

namespace com.tencent.gsdk
{
	internal class GSDKAndroidObserver : AndroidJavaProxy
	{
		private static GSDKAndroidObserver instance = new GSDKAndroidObserver();

		public static GSDKAndroidObserver Instance
		{
			get
			{
				return GSDKAndroidObserver.instance;
			}
		}

		private GSDKAndroidObserver() : base("com.tencent.gsdk.GSDKObserver")
		{
		}

		public void OnStartSpeedNotify(int type, int flag, string desc)
		{
			StartSpeedRet ret = new StartSpeedRet(type, flag, desc);
			GSDK.notify(ret);
		}

		public void OnQueryKartinNotify(string _tag, int _flag, string _desc, int _jump_network, int _jump_signal, int _jump_router, int _router_status, string _router_desc, int _jump_export, int _export_status, string _export_desc, int _jump_terminal, int _terminal_status, string _terminal_desc, int _jump_proxy, int _jump_edge, string _signal_desc, int _signal_status)
		{
			KartinRet ret = new KartinRet(_tag, _flag, _desc, _jump_network, _jump_signal, _jump_router, _router_status, _router_desc, _jump_export, _export_status, _export_desc, _jump_terminal, _terminal_status, _terminal_desc, _jump_proxy, _jump_edge, _signal_desc, _signal_status);
			GSDK.notify(ret);
		}

		public void OnQueryKartinNotify2(string _tag, int _flag, string _desc, int _jump_network, int _jump_signal, int _jump_router, int _router_status, string _router_desc, int _jump_export, int _export_status, string _export_desc, int _jump_terminal, int _terminal_status, string _terminal_desc, int _jump_proxy, int _jump_edge, string _signal_desc, int _signal_status, int _jump_direct, int _direct_status, string _direct_desc)
		{
		}
	}
}
