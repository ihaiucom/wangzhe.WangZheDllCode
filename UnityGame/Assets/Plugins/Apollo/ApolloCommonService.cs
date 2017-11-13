using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Apollo
{
	internal class ApolloCommonService : ApolloObject, IApolloCommonService, IApolloServiceBase
	{
		public static readonly ApolloCommonService Instance = new ApolloCommonService();

		public event OnFeedbackNotifyHandle onFeedbackEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onFeedbackEvent = (OnFeedbackNotifyHandle)Delegate.Combine(this.onFeedbackEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onFeedbackEvent = (OnFeedbackNotifyHandle)Delegate.Remove(this.onFeedbackEvent, value);
			}
		}

		public event OnCrashExtMessageNotifyHandle onCrashExtMessageEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onCrashExtMessageEvent = (OnCrashExtMessageNotifyHandle)Delegate.Combine(this.onCrashExtMessageEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onCrashExtMessageEvent = (OnCrashExtMessageNotifyHandle)Delegate.Remove(this.onCrashExtMessageEvent, value);
			}
		}

		public event OnReceivedPushNotifyHandle onReceivedPushEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onReceivedPushEvent = (OnReceivedPushNotifyHandle)Delegate.Combine(this.onReceivedPushEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onReceivedPushEvent = (OnReceivedPushNotifyHandle)Delegate.Remove(this.onReceivedPushEvent, value);
			}
		}

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
				catch (Exception ex)
				{
					ADebug.Log("onFeedbackEvent:" + ex);
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
			catch (Exception ex)
			{
				ADebug.LogError("OnCrashExtMessageNotify" + ex);
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
				catch (Exception ex)
				{
					ADebug.Log("onReceivedPushEvent:" + ex);
				}
			}
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_OpenUrl([MarshalAs(20)] string openUrl, APOLLO_SCREENDIR screendir);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_common_OpenAmsCenter([MarshalAs(20)] string param);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_Feedback([MarshalAs(20)] string body);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_GetChannelId([MarshalAs(20)] StringBuilder ChannelId, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_GetRegisterChannelId([MarshalAs(20)] StringBuilder RegisterChannelId, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_PushInit();

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_OpenWeiXinDeeplink([MarshalAs(20)] string link);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_common_GetEncodeUrl([MarshalAs(20)] string openUrl, [MarshalAs(20)] StringBuilder EncodeUrl, int size);
	}
}
