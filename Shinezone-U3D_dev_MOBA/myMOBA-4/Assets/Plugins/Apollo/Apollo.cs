using Apollo.Plugins.Msdk;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apollo
{
	internal class Apollo : IApollo
	{
		private event ApolloLogHandler logEvent;

		public Apollo()
		{
			Tx.Instance.Initialize();
		}

		[MonoPInvokeCallback(typeof(ApolloLogDelegate))]
		private static void OnApolloLogDelegate(ApolloLogPriority pri, IntPtr msg)
		{
			Apollo apollo = IApollo.Instance as Apollo;
			if (apollo.logEvent != null)
			{
				apollo.logEvent(pri, Marshal.PtrToStringAnsi(msg));
			}
		}

		public override void SetApolloLogger(ApolloLogPriority pri, ApolloLogHandler handler)
		{
			ADebug.Log("SetApolloLogger");
			this.logEvent = handler;
			Apollo.apollo_setApolloLogger(pri, new ApolloLogDelegate(Apollo.OnApolloLogDelegate));
		}

		~Apollo()
		{
		}

		public override ApolloResult Initialize(ApolloInfo platformInfo)
		{
			ApolloCommon.ApolloInfo = platformInfo;
			if (platformInfo == null)
			{
				throw new Exception("ApolloInfo could not be null!!");
			}
			ADebug.Log(string.Format("Apollo Init QQAppId:{0}, WXAppId:{1}, pluginName:{2}", platformInfo.QQAppId, platformInfo.WXAppId, platformInfo.PluginName));
			if (string.IsNullOrEmpty(platformInfo.PluginName))
			{
				ApolloCommon.ApolloInfo.PluginName = "MSDK";
			}
			MsdkAdapter.InnerInstall();
			return (ApolloResult)Apollo.apollo_init(platformInfo.ServiceId, platformInfo.MaxMessageBufferSize, ApolloCommon.ApolloInfo.PluginName);
		}

		public static Type GetType2(string typeName)
		{
			Type type = UtilityPlugin.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				type = assembly.GetType(typeName);
				if (type != null)
				{
					Debug.Log("GetType2 find " + typeName + "FullName= " + assembly.FullName);
					return type;
				}
			}
			return null;
		}

		public override bool SwitchPlugin(string pluginName)
		{
			return Apollo.apollo_switchplugin(pluginName);
		}

		public override IApolloConnector CreateApolloConnection(ApolloPlatform platform, string url)
		{
			return this.CreateApolloConnection(platform, 16777215u, url);
		}

		[Obsolete("Deprecated since 1.1.2, use CreateApolloConnection(ApolloPlatform platform, UInt32 permission, String svrUrl) instead")]
		public override IApolloConnector CreateApolloConnection(ApolloPlatform platform, ApolloPermission permission, string url)
		{
			return this.CreateApolloConnection(platform, (uint)permission, url);
		}

		[Obsolete("Deprecated since 1.1.13, use CreateApolloConnection(ApolloPlatform platform,  String svrUrl) instead")]
		public override IApolloConnector CreateApolloConnection(ApolloPlatform platform, uint permission, string url)
		{
			ADebug.Log("CreateApolloConnection");
			if (string.IsNullOrEmpty(url))
			{
				throw new Exception("Argument Error: url can not be null or empty");
			}
			if (ApolloCommon.ApolloInfo == null)
			{
				throw new Exception("IApollo.Instance.Initialize must be called first!");
			}
			if (platform == ApolloPlatform.None)
			{
			}
			ApolloConnector apolloConnector = new ApolloConnector();
			ApolloResult apolloResult = apolloConnector.Initialize(platform, permission, url);
			if (apolloResult != ApolloResult.Success)
			{
				throw new Exception("connector Initialize Error:" + apolloResult);
			}
			return apolloConnector;
		}

		public override void DestroyApolloConnector(IApolloConnector connector)
		{
			ADebug.Log("DestroyApolloConnector");
			if (connector == null)
			{
				return;
			}
			ApolloConnector apolloConnector = connector as ApolloConnector;
			if (apolloConnector != null)
			{
				apolloConnector.Destroy();
			}
		}

		public override IApolloServiceBase GetService(int type)
		{
			if (type == 1000)
			{
				return TssService.Instance;
			}
			if (type == 1001)
			{
				return ApolloNetworkService.Intance;
			}
			if (type == 2)
			{
				return ApolloPayService.Instance;
			}
			PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
			if (currentPlugin == null)
			{
				return null;
			}
			return currentPlugin.GetService(type);
		}

		public override IApolloAccountService GetAccountService()
		{
			return ApolloAccountService.Instance;
		}

		public override IApolloTalker CreateTalker(IApolloConnector connector)
		{
			if (ApolloCommon.ApolloInfo == null)
			{
				throw new Exception("IApollo.Instance.Initialize must be called first!");
			}
			return new ApolloTalker(connector);
		}

		public override void DestroyTalker(IApolloTalker talker)
		{
			ApolloTalker apolloTalker = talker as ApolloTalker;
			if (apolloTalker != null)
			{
				apolloTalker.Destroy();
			}
		}

		public override IApolloHttpClient CreateHttpClient()
		{
			if (ApolloCommon.ApolloInfo == null)
			{
				throw new Exception("IApollo.Instance.Initialize must be called first!");
			}
			return new ApolloHttpClient();
		}

		public override void DestoryHttpClient(IApolloHttpClient client)
		{
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_setLogLevel(ApolloLogPriority pri);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern int apollo_init(int nServiceId, int nMaxMessageBuffSize, [MarshalAs(UnmanagedType.LPStr)] string pluginName);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_switchplugin([MarshalAs(UnmanagedType.LPStr)] string pluginName);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_setApolloLogger(ApolloLogPriority pri, [MarshalAs(UnmanagedType.FunctionPtr)] ApolloLogDelegate callback);
	}
}
