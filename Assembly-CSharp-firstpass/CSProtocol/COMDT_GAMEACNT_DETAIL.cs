using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GAMEACNT_DETAIL : ProtocolObject
	{
		public COMDT_ACNT_MATCH_BRIEF_INFO stMatchInfo;

		public COMDT_GAMEACNT_SYMBOLPAGE stSymbolPage;

		public COMDT_FREEHERO_INACNT stFreeHeroRcd;

		public COMDT_ACNT_BANTIME stBanTime;

		public COMDT_ACNT_EXTRAINFO stBriefInfo;

		public COMDT_GAME_VIP_CLIENT stGameVip;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 121u;

		public static readonly int CLASS_ID = 211;

		public COMDT_GAMEACNT_DETAIL()
		{
			this.stMatchInfo = (COMDT_ACNT_MATCH_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_ACNT_MATCH_BRIEF_INFO.CLASS_ID);
			this.stSymbolPage = (COMDT_GAMEACNT_SYMBOLPAGE)ProtocolObjectPool.Get(COMDT_GAMEACNT_SYMBOLPAGE.CLASS_ID);
			this.stFreeHeroRcd = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
			this.stBanTime = (COMDT_ACNT_BANTIME)ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID);
			this.stBriefInfo = (COMDT_ACNT_EXTRAINFO)ProtocolObjectPool.Get(COMDT_ACNT_EXTRAINFO.CLASS_ID);
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_GAMEACNT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GAMEACNT_DETAIL.CURRVERSION;
			}
			if (COMDT_GAMEACNT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stMatchInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolPage.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroRcd.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBanTime.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBriefInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGameVip.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_GAMEACNT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GAMEACNT_DETAIL.CURRVERSION;
			}
			if (COMDT_GAMEACNT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stMatchInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolPage.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroRcd.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBanTime.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBriefInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGameVip.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GAMEACNT_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stMatchInfo != null)
			{
				this.stMatchInfo.Release();
				this.stMatchInfo = null;
			}
			if (this.stSymbolPage != null)
			{
				this.stSymbolPage.Release();
				this.stSymbolPage = null;
			}
			if (this.stFreeHeroRcd != null)
			{
				this.stFreeHeroRcd.Release();
				this.stFreeHeroRcd = null;
			}
			if (this.stBanTime != null)
			{
				this.stBanTime.Release();
				this.stBanTime = null;
			}
			if (this.stBriefInfo != null)
			{
				this.stBriefInfo.Release();
				this.stBriefInfo = null;
			}
			if (this.stGameVip != null)
			{
				this.stGameVip.Release();
				this.stGameVip = null;
			}
		}

		public override void OnUse()
		{
			this.stMatchInfo = (COMDT_ACNT_MATCH_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_ACNT_MATCH_BRIEF_INFO.CLASS_ID);
			this.stSymbolPage = (COMDT_GAMEACNT_SYMBOLPAGE)ProtocolObjectPool.Get(COMDT_GAMEACNT_SYMBOLPAGE.CLASS_ID);
			this.stFreeHeroRcd = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
			this.stBanTime = (COMDT_ACNT_BANTIME)ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID);
			this.stBriefInfo = (COMDT_ACNT_EXTRAINFO)ProtocolObjectPool.Get(COMDT_ACNT_EXTRAINFO.CLASS_ID);
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
		}
	}
}
