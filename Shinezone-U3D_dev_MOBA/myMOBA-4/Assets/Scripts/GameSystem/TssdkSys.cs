using Apollo;
using Assets.Scripts.Framework;
using CSProtocol;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class TssdkSys : MonoSingleton<TssdkSys>
	{
		private bool bInit;

		private bool bEnableTSS = true;

		private string m_OpenID = string.Empty;

		private float m_LastTime;

		protected override void Init()
		{
			base.Init();
			try
			{
				this.bEnableTSS = (PlayerPrefs.GetInt("EnableTSS", (!this.bEnableTSS) ? 0 : 1) != 0);
				if (this.bEnableTSS)
				{
					TssSdk.TssSdkInit(2577u);
					this.bInit = true;
					this.m_LastTime = Time.time;
					Debug.Log("TssdkSys init");
				}
			}
			catch (Exception)
			{
				this.bInit = false;
			}
		}

		public void OnAccountLogin()
		{
			if (this.bEnableTSS)
			{
				this.InitTssSdk();
				Debug.Log("TssdkSys OnAccountLogin");
			}
		}

		private void InitTssSdk()
		{
			if (this.bEnableTSS)
			{
				int nPlatform = 1;
				if (ApolloConfig.platform == ApolloPlatform.Wechat)
				{
					nPlatform = 2;
				}
				ApolloAccountInfo apolloAccountInfo = new ApolloAccountInfo();
				IApollo.Instance.GetAccountService().GetRecord(ref apolloAccountInfo);
				this.m_OpenID = apolloAccountInfo.OpenId;
				this.CreateTssSDKSys(this.m_OpenID, nPlatform);
			}
		}

		public void CreateTssSDKSys(string openId, int nPlatform)
		{
			Debug.Log("TssdkSys CreateTssSDKSys, nplatf = " + nPlatform);
			if (openId != null)
			{
				if (nPlatform == 2)
				{
					TssSdk.TssSdkSetUserInfo((TssSdk.EENTRYID)nPlatform, openId, ApolloConfig.WXAppID);
				}
				else
				{
					TssSdk.TssSdkSetUserInfo((TssSdk.EENTRYID)nPlatform, openId, ApolloConfig.QQAppID);
				}
			}
		}

		private void OnApplicationPause(bool pause)
		{
			if (!this.bInit || !this.bEnableTSS)
			{
				return;
			}
			if (pause)
			{
				TssSdk.TssSdkSetGameStatus(TssSdk.EGAMESTATUS.GAME_STATUS_BACKEND);
			}
			else
			{
				TssSdk.TssSdkSetGameStatus(TssSdk.EGAMESTATUS.GAME_STATUS_FRONTEND);
			}
			Singleton<BeaconHelper>.GetInstance().Event_ApplicationPause(pause);
		}

		[MessageHandler(3001)]
		public static void On_GetAntiData(CSPkg msg)
		{
			if (!MonoSingleton<TssdkSys>.instance.bEnableTSS)
			{
				return;
			}
			TssSdk.TssSdkRcvAntiData(msg.stPkgData.stAntiDataSyn.szBuff, msg.stPkgData.stAntiDataSyn.wLen);
		}

		public void Update()
		{
			if (!this.bInit || !this.bEnableTSS)
			{
				return;
			}
			if (Time.time - this.m_LastTime > 20f)
			{
				IntPtr intPtr = TssSdk.tss_get_report_data();
				if (intPtr != IntPtr.Zero)
				{
					TssSdk.AntiDataInfo antiDataInfo = new TssSdk.AntiDataInfo();
					if (TssSdk.Is64bit())
					{
						short num = Marshal.ReadInt16(intPtr, 0);
						long value = Marshal.ReadInt64(intPtr, 2);
						antiDataInfo.anti_data_len = (ushort)num;
						antiDataInfo.anti_data = new IntPtr(value);
					}
					else if (TssSdk.Is32bit())
					{
						short num2 = Marshal.ReadInt16(intPtr, 0);
						int value2 = Marshal.ReadInt32(intPtr, 2);
						antiDataInfo.anti_data_len = (ushort)num2;
						antiDataInfo.anti_data = new IntPtr(value2);
					}
					if (antiDataInfo.anti_data != IntPtr.Zero)
					{
						CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(3000u);
						cSPkg.stPkgData.stAntiDataReq.wLen = antiDataInfo.anti_data_len;
						Marshal.Copy(antiDataInfo.anti_data, cSPkg.stPkgData.stAntiDataReq.szBuff, 0, (int)antiDataInfo.anti_data_len);
						Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
					}
					TssSdk.tss_del_report_data(intPtr);
				}
				this.m_LastTime = Time.time;
			}
		}
	}
}
