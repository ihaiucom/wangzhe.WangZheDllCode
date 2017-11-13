using apollo_tss;
using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class TssService : ApolloObject, IApolloServiceBase, ITssService
	{
		private uint gameId;

		private ApolloTalker talker;

		private TssTransferBase transfer;

		public static readonly TssService Instance = new TssService();

		private float intervalBetweenCollections = 20f;

		private bool connected
		{
			get
			{
				if (this.talker != null)
				{
					ApolloConnector apolloConnector = this.talker.connector as ApolloConnector;
					if (apolloConnector != null)
					{
						return apolloConnector.Connected;
					}
				}
				else if (this.transfer != null)
				{
					return this.transfer.IsConnected();
				}
				return false;
			}
		}

		private TssService() : base(true, true)
		{
		}

		public void Intialize(uint gameId)
		{
			this.gameId = gameId;
			TssSdk.TssSdkInit(gameId);
		}

		public void ReportUserInfo()
		{
			this.ReportUserInfo(0u, string.Empty);
		}

		public void ReportUserInfo(uint wordId, string roleId)
		{
			IApolloAccountService accountService = IApollo.Instance.GetAccountService();
			ApolloAccountInfo accountInfo = new ApolloAccountInfo();
			if (accountService.GetRecord(ref accountInfo) == ApolloResult.Success)
			{
				this.setUserInfo(accountInfo, wordId, roleId);
			}
		}

		private void setUserInfo(ApolloAccountInfo accountInfo, uint worldId, string roleId)
		{
			if (accountInfo == null)
			{
				ADebug.Log("TssService account info is null");
				return;
			}
			TssSdk.EENTRYID entryId = TssSdk.EENTRYID.ENTRY_ID_OTHERS;
			string openId = accountInfo.OpenId;
			string appId = null;
			if (accountInfo != null)
			{
				if (accountInfo.Platform == ApolloPlatform.Wechat)
				{
					entryId = TssSdk.EENTRYID.ENTRY_ID_MM;
					appId = ApolloCommon.ApolloInfo.WXAppId;
				}
				else
				{
					entryId = TssSdk.EENTRYID.ENTRY_ID_QQ;
					appId = ApolloCommon.ApolloInfo.QQAppId;
				}
			}
			TssSdk.TssSdkSetUserInfoEx(entryId, openId, appId, worldId, roleId);
		}

		public void StartWithTalker(IApolloTalker talker, float intervalBetweenCollections = 2f)
		{
			this.intervalBetweenCollections = intervalBetweenCollections;
			this.ResetTimeInterval();
			this.talker = (talker as ApolloTalker);
			if (this.talker == null)
			{
				throw new Exception("Talker must not be null !");
			}
			this.talker.Register<ApolloTSS>(TalkerCommand.CommandDomain.TSS, delegate(ApolloTSS resp)
			{
				if (resp != null)
				{
					ADebug.Log("tss recv data len:" + resp.wLen);
					TssSdk.TssSdkRcvAntiData(resp.szData, resp.wLen);
				}
				else
				{
					ADebug.Log("Tss resp  is null");
				}
			});
		}

		public void StartWithTransfer(TssTransferBase transfer, float intervalBetweenCollections = 2f)
		{
			this.intervalBetweenCollections = intervalBetweenCollections;
			this.ResetTimeInterval();
			this.transfer = transfer;
			if (this.transfer == null)
			{
				throw new Exception("Transfer must not be null !");
			}
			this.transfer.tssService = this;
		}

		internal void SetAntiData(byte[] data, int len = 0)
		{
			if (data != null)
			{
				if (len == 0)
				{
					len = data.Length;
				}
				TssSdk.TssSdkRcvAntiData(data, (ushort)len);
			}
		}

		protected override void OnTimeOut()
		{
			if (this.talker == null && this.transfer == null)
			{
				throw new Exception("Talker or Transfer must not be null !");
			}
			if (this.connected)
			{
				this.OnTssCollected();
			}
			this.ResetTimeInterval();
		}

		private void OnTssCollected()
		{
			IntPtr intPtr = TssSdk.tss_get_report_data();
			if (intPtr != IntPtr.Zero)
			{
				TssSdk.AntiDataInfo antiDataInfo = new TssSdk.AntiDataInfo();
				if (TssSdk.Is64bit())
				{
					short num = Marshal.ReadInt16(intPtr, 0);
					long num2 = Marshal.ReadInt64(intPtr, 2);
					antiDataInfo.anti_data_len = (ushort)num;
					antiDataInfo.anti_data = new IntPtr(num2);
				}
				else if (TssSdk.Is32bit())
				{
					short num3 = Marshal.ReadInt16(intPtr, 0);
					long num4 = (long)Marshal.ReadInt32(intPtr, 2);
					antiDataInfo.anti_data_len = (ushort)num3;
					antiDataInfo.anti_data = new IntPtr(num4);
				}
				if (antiDataInfo.anti_data == IntPtr.Zero)
				{
					ADebug.Log("OnTssCollected aniti data is null");
					return;
				}
				ApolloTSS apolloTSS = new ApolloTSS();
				apolloTSS.wLen = antiDataInfo.anti_data_len;
				apolloTSS.szData = new byte[(int)apolloTSS.wLen];
				Marshal.Copy(antiDataInfo.anti_data, apolloTSS.szData, 0, (int)apolloTSS.wLen);
				ADebug.Log("begin send tss data len:" + apolloTSS.wLen);
				if (this.talker != null)
				{
					this.talker.Send(TalkerCommand.CommandDomain.TSS, apolloTSS);
				}
				else if (this.transfer != null)
				{
					this.transfer.OnTssDataCollected(apolloTSS.szData);
				}
				TssSdk.tss_del_report_data(intPtr);
			}
			else
			{
				ADebug.Log("tss tss_get_report_data addr is null");
			}
		}

		private void ResetTimeInterval()
		{
			base.UpdateTimeLeft = this.intervalBetweenCollections;
		}

		public override void OnApplicationPause(bool pauseStatus)
		{
		}
	}
}
