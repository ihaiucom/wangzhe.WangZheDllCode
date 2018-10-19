using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Apollo
{
	internal class ApolloCommonService : ApolloObject, IApolloCommonService, IApolloServiceBase
	{
		public static readonly ApolloCommonService Instance = new ApolloCommonService();

		public event OnFeedbackNotifyHandle onFeedbackEvent;

		public event OnCrashExtMessageNotifyHandle onCrashExtMessageEvent;

		public event OnReceivedPushNotifyHandle onReceivedPushEvent;

		private ApolloCommonService()
		{
		}

		public void PushInit()
		{
			ApolloCommonService.apollo_common_PushInit();
		}

		public void OpenUrl(string openUrl)
		{
			ApolloCommonService.apollo_common_OpenUrl(openUrl, APOLLO_SCREENDIR.APO_SCREENDIR_SENSOR);
		}

		public void OpenUrl(string openUrl, APOLLO_SCREENDIR screendir)
		{
			ApolloCommonService.apollo_common_OpenUrl(openUrl, screendir);
		}

		public bool OpenAmsCenter(string param)
		{
			return ApolloCommonService.apollo_common_OpenAmsCenter(param);
		}

		public string GetEncodeUrl(string openUrl)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			ApolloCommonService.apollo_common_GetEncodeUrl(openUrl, stringBuilder, 1024);
			return stringBuilder.ToString();
		}

		public void Feedback(string body)
		{
			ApolloCommonService.apollo_common_Feedback(body);
		}

		public string GetChannelId()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			ApolloCommonService.apollo_common_GetChannelId(stringBuilder, 128);
			return stringBuilder.ToString();
		}

		public string GetRegisterChannelId()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			ApolloCommonService.apollo_common_GetRegisterChannelId(stringBuilder, 128);
			return stringBuilder.ToString();
		}

		public void OpenWeiXinDeeplink(string link)
		{
			ApolloCommonService.apollo_common_OpenWeiXinDeeplink(link);
		}

		private void OnFeedbackNotify(string msg)
		{
			ADebug.Log("onFeedbackEvent:" + msg);
			if (this.onFeedbackEvent != null)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				int @int = apolloStringParser.GetInt("Flag");
				string @string = apolloStringParser.GetString("Desc");
				try
				{
					this.onFeedbackEvent(@int, @string);
				}
				catch (Exception arg)
				{
					ADebug.Log("onFeedbackEvent:" + arg);
				}
			}
		}

		private void OnCrashExtMessageNotify(string msg)
		{
			try
			{
				if (this.onCrashExtMessageEvent != null)
				{
					this.onCrashExtMessageEvent();
				}
			}
			catch (Exception arg)
			{
				ADebug.LogError("OnCrashExtMessageNotify" + arg);
			}
		}

		private void OnReceivedPushNotify(string msg)
		{
			ADebug.Log("OnReceivedPushNotify:" + msg);
			if (this.onReceivedPushEvent != null)
			{
				try
				{
					this.onReceivedPushEvent(msg);
				}
				catch (Exception arg)
				{
					ADebug.Log("onReceivedPushEvent:" + arg);
				}
			}
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_OpenUrl([MarshalAs(UnmanagedType.LPStr)] string openUrl, APOLLO_SCREENDIR screendir);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_common_OpenAmsCenter([MarshalAs(UnmanagedType.LPStr)] string param);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_Feedback([MarshalAs(UnmanagedType.LPStr)] string body);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_GetChannelId([MarshalAs(UnmanagedType.LPStr)] StringBuilder ChannelId, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_GetRegisterChannelId([MarshalAs(UnmanagedType.LPStr)] StringBuilder RegisterChannelId, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_PushInit();

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_OpenWeiXinDeeplink([MarshalAs(UnmanagedType.LPStr)] string link);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_GetEncodeUrl([MarshalAs(UnmanagedType.LPStr)] string openUrl, [MarshalAs(UnmanagedType.LPStr)] StringBuilder EncodeUrl, int size);
	}
}
