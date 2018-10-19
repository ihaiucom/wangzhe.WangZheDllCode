using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Apollo
{
	internal class ApolloAccountService : ApolloObject, IApolloAccountService, IApolloServiceBase
	{
		public static readonly ApolloAccountService Instance = new ApolloAccountService();

		public event AccountInitializeHandle InitializeEvent;

		public event AccountLoginHandle LoginEvent;

		public event AccountLogoutHandle LogoutEvent;

		public event RefreshAccessTokenHandler RefreshAtkEvent;

		public event RealNameAuthHandler RealNameAutEvent;

		private ApolloAccountService()
		{
		}

		public bool Initialize(ApolloBufferBase initInfo)
		{
			if (initInfo != null)
			{
				byte[] array;
				initInfo.Encode(out array);
				if (array != null)
				{
					return ApolloAccountService.apollo_account_initialize(array, array.Length);
				}
				ADebug.LogError("Account Initialize Encode Error");
			}
			else
			{
				ADebug.LogError("Account Initialize param is null");
			}
			return false;
		}

		[Obsolete("Obsolete since 1.1.6, use Initialize instead")]
		public void SetPermission(uint permission)
		{
			throw new Exception("Obsolete since 1.1.6, use Initialize instead");
		}

		public void Reset()
		{
			ApolloAccountService.apollo_account_reset();
		}

		public void Login(ApolloPlatform platform)
		{
			ADebug.Log("Login");
			ApolloAccountService.apollo_account_login(base.ObjectId, platform);
		}

		public void Logout()
		{
			ApolloAccountService.apollo_account_logout(base.ObjectId);
		}

		public void RefreshAccessToken()
		{
			ApolloAccountService.apollo_account_refreshAtk(base.ObjectId);
		}

		public ApolloResult GetRecord(ref ApolloAccountInfo pAccountInfo)
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			ApolloResult apolloResult = ApolloAccountService.apollo_account_getRecord(base.ObjectId, stringBuilder, 4096);
			string text = stringBuilder.ToString();
			ADebug.Log(string.Concat(new object[]
			{
				"GetRecord:",
				apolloResult,
				", ",
				text
			}));
			if (text.Length > 0)
			{
				pAccountInfo.FromString(text);
			}
			return apolloResult;
		}

		public bool IsPlatformInstalled(ApolloPlatform platform)
		{
			return ApolloAccountService.apollo_account_IsPlatformInstalled(platform);
		}

		public bool IsPlatformSupportApi(ApolloPlatform platform)
		{
			return ApolloAccountService.apollo_account_IsPlatformSupportApi(platform);
		}

		public void RealNameAuth(ApolloRealNameAuthInfo info)
		{
			if (info != null)
			{
				byte[] array;
				info.Encode(out array);
				if (array != null)
				{
					ApolloAccountService.apollo_account_realname_auth(array, array.Length);
				}
				else
				{
					ADebug.LogError("RealNameAuth Encode Error");
				}
			}
			else
			{
				ADebug.LogError("RealNameAuth param is null");
			}
		}

		private void OnAccountInitializeProc(int ret, byte[] buf)
		{
			ADebug.Log("OnAccountInitializeProc result:" + (ApolloResult)ret);
			if (this.InitializeEvent != null)
			{
				try
				{
					this.InitializeEvent((ApolloResult)ret, null);
				}
				catch (Exception arg)
				{
					ADebug.LogError("OnAccountInitializeProc:" + arg);
				}
			}
		}

		private void onLoginProc(string msg)
		{
			BugLocateLogSys.Log("ApolloAccountService onLoginProc:" + msg);
			if (!string.IsNullOrEmpty(msg))
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloAccountInfo apolloAccountInfo = null;
				ApolloResult @int = (ApolloResult)apolloStringParser.GetInt("Result");
				BugLocateLogSys.Log("ApolloAccountService onLoginProc: result" + @int);
				if (@int == ApolloResult.Success)
				{
					apolloAccountInfo = apolloStringParser.GetObject<ApolloAccountInfo>("AccountInfo");
					if (apolloAccountInfo != null && apolloAccountInfo.TokenList != null)
					{
						BugLocateLogSys.Log(string.Concat(new object[]
						{
							"C# onLoginProc|",
							@int,
							" platform:",
							apolloAccountInfo.Platform,
							" openid:",
							apolloAccountInfo.OpenId,
							" tokensize:",
							apolloAccountInfo.TokenList.Count,
							" pf:",
							apolloAccountInfo.Pf,
							" pfkey:",
							apolloAccountInfo.PfKey
						}));
					}
					else
					{
						BugLocateLogSys.Log("parser.GetObject<ApolloAccountInfo>() return null");
						Debug.LogError("parser.GetObject<ApolloAccountInfo>() return null");
					}
				}
				else
				{
					BugLocateLogSys.Log("C# onLoginProc error:" + @int);
					DebugHelper.Assert(false, "C# onLoginProc error:" + @int);
				}
				Debug.LogWarning(string.Format("LoginEvent:{0}", this.LoginEvent));
				if (this.LoginEvent != null)
				{
					try
					{
						this.LoginEvent(@int, apolloAccountInfo);
					}
					catch (Exception ex)
					{
						DebugHelper.Assert(false, "onLoginProc:" + ex);
						BugLocateLogSys.Log("onLoginProc catch exception :" + ex.Message + "|" + ex.ToString());
					}
				}
			}
		}

		private void OnAccountLogoutProc(int ret)
		{
			ADebug.Log("OnAccountLogoutProc result:" + (ApolloResult)ret);
			if (this.LogoutEvent != null)
			{
				try
				{
					this.LogoutEvent((ApolloResult)ret);
				}
				catch (Exception arg)
				{
					ADebug.LogError("OnAccountLogoutProc:" + arg);
				}
			}
		}

		private void onAccessTokenRefresedProc(string msg)
		{
			ADebug.Log("onAccessTokenRefresedProc: " + msg);
			ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
			ListView<ApolloToken> listView = null;
			ApolloResult @int = (ApolloResult)apolloStringParser.GetInt("Result");
			if (@int == ApolloResult.Success)
			{
				string text = apolloStringParser.GetString("tokens");
				if (text != null)
				{
					text = ApolloStringParser.ReplaceApolloString(text);
					ADebug.Log("onAccessTokenRefresedProc tokens:" + text);
					if (text != null && text.Length > 0)
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						listView = new ListView<ApolloToken>();
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string src = array2[i];
							string text2 = ApolloStringParser.ReplaceApolloString(src);
							text2 = ApolloStringParser.ReplaceApolloString(text2);
							ApolloToken apolloToken = new ApolloToken();
							apolloToken.FromString(text2);
							ADebug.Log(string.Format("onAccessTokenRefresedProc str:{0} |||||| {1}   |||||{2}", text2, apolloToken.Type, apolloToken.Value));
							listView.Add(apolloToken);
						}
					}
				}
			}
			if (this.RefreshAtkEvent != null)
			{
				this.RefreshAtkEvent(@int, listView);
			}
		}

		private void OnRealNameAuthProc(byte[] data)
		{
			ADebug.Log("OnRealNameAuthProc!");
			ApolloRealNameAuthResult apolloRealNameAuthResult = new ApolloRealNameAuthResult();
			if (this.RealNameAutEvent != null && data.Length > 0)
			{
				if (!apolloRealNameAuthResult.Decode(data))
				{
					ADebug.LogError("OnRealNameAuthProc Decode failed");
				}
				this.RealNameAutEvent(apolloRealNameAuthResult);
			}
			else
			{
				ADebug.Log("RealNameAutEvent is null");
			}
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_account_initialize([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_login(ulong objId, ApolloPlatform platform);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_logout(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_reset();

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern ApolloResult apollo_account_getRecord(ulong objId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder pAccountInfo, int size);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern bool apollo_account_IsPlatformInstalled(ApolloPlatform platformType);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern bool apollo_account_IsPlatformSupportApi(ApolloPlatform platformType);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_refreshAtk(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_account_realname_auth([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
	}
}
