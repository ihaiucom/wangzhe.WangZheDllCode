using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Apollo
{
	internal class ApolloQuickLoginService : ApolloObject, IApolloQuickLoginService, IApolloServiceBase
	{
		private delegate void ApolloQuickLoginBaseDelegate();

		public static readonly ApolloQuickLoginService Instance = new ApolloQuickLoginService();

		private static ApolloQuickLoginNotify m_callback = null;

		private ApolloQuickLoginService()
		{
		}

		public void SetQuickLoginNotify(ApolloQuickLoginNotify callback)
		{
			ADebug.Log("C# ApolloQuickLoginService::SetCallback");
			ApolloQuickLoginService.m_callback = callback;
			ApolloQuickLoginService.apollo_account_SetQuickLoginBaseCallback(new ApolloQuickLoginService.ApolloQuickLoginBaseDelegate(ApolloQuickLoginService.QuickLoginOccur));
		}

		[MonoPInvokeCallback(typeof(ApolloQuickLoginService.ApolloQuickLoginBaseDelegate))]
		private static void QuickLoginOccur()
		{
			ADebug.Log("C# Apollo pulled up by quicklogin");
			ApolloWakeupInfo wakeupInfo = new ApolloWakeupInfo();
			bool flag = ApolloQuickLoginService.s_GetWakeupInfo(out wakeupInfo);
			if (ApolloQuickLoginService.m_callback != null)
			{
				ApolloQuickLoginService.m_callback(wakeupInfo);
			}
			else
			{
				ADebug.Log("QuickLoginOccur m_callback is null");
			}
		}

		public void SwitchUser(bool useExternalAccount)
		{
			ApolloQuickLoginService.apollo_account_SwitchUser(useExternalAccount);
		}

		private static bool s_GetWakeupInfo(out ApolloWakeupInfo wakeupInfo)
		{
			wakeupInfo = null;
			StringBuilder stringBuilder = new StringBuilder(20480);
			bool flag = ApolloQuickLoginService.apollo_account_GetWakeupInfo(stringBuilder, 20480);
			ADebug.Log("s_GetWakeupInfo : " + flag);
			if (flag)
			{
				string text = stringBuilder.ToString();
				ADebug.Log("s_GetWakeupInfo: " + text);
				if (text != null && text.get_Length() > 0)
				{
					ApolloStringParser apolloStringParser = new ApolloStringParser(text);
					if (apolloStringParser != null)
					{
						wakeupInfo = apolloStringParser.GetObject<ApolloWakeupInfo>("WakeupInfo");
						if (wakeupInfo != null)
						{
							ADebug.Log("s_GetWakeupInfo parser.GetObject success");
							return true;
						}
						ADebug.Log("s_GetWakeupInfo parser.GetObject error");
					}
					else
					{
						ADebug.Log("GetWakeupInfo parser.GetObjec error");
					}
				}
			}
			return false;
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_SwitchUser(bool useExternalAccount);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_account_GetWakeupInfo([MarshalAs(20)] StringBuilder pAccountInfo, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_SetQuickLoginBaseCallback([MarshalAs(38)] ApolloQuickLoginService.ApolloQuickLoginBaseDelegate callback);
	}
}
