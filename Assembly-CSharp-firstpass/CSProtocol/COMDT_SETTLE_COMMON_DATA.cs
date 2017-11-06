using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SETTLE_COMMON_DATA : ProtocolObject
	{
		public COMDT_STATISTIC_DATA stStatisticData;

		public COMDT_SETTLE_HERO_DETAIL stHeroData;

		public COMDT_SETTLE_GAME_GENERAL_INFO stGeneralData;

		public COMDT_NONHERO_DETAIL stNonHeroData;

		public COMDT_SETTLE_OTHERINFO stOtherData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 174;

		public COMDT_SETTLE_COMMON_DATA()
		{
			this.stStatisticData = (COMDT_STATISTIC_DATA)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA.CLASS_ID);
			this.stHeroData = (COMDT_SETTLE_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_SETTLE_HERO_DETAIL.CLASS_ID);
			this.stGeneralData = (COMDT_SETTLE_GAME_GENERAL_INFO)ProtocolObjectPool.Get(COMDT_SETTLE_GAME_GENERAL_INFO.CLASS_ID);
			this.stNonHeroData = (COMDT_NONHERO_DETAIL)ProtocolObjectPool.Get(COMDT_NONHERO_DETAIL.CLASS_ID);
			this.stOtherData = (COMDT_SETTLE_OTHERINFO)ProtocolObjectPool.Get(COMDT_SETTLE_OTHERINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SETTLE_COMMON_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_COMMON_DATA.CURRVERSION;
			}
			if (COMDT_SETTLE_COMMON_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stStatisticData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGeneralData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNonHeroData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stOtherData.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_SETTLE_COMMON_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_COMMON_DATA.CURRVERSION;
			}
			if (COMDT_SETTLE_COMMON_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stStatisticData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGeneralData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNonHeroData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stOtherData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SETTLE_COMMON_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stStatisticData != null)
			{
				this.stStatisticData.Release();
				this.stStatisticData = null;
			}
			if (this.stHeroData != null)
			{
				this.stHeroData.Release();
				this.stHeroData = null;
			}
			if (this.stGeneralData != null)
			{
				this.stGeneralData.Release();
				this.stGeneralData = null;
			}
			if (this.stNonHeroData != null)
			{
				this.stNonHeroData.Release();
				this.stNonHeroData = null;
			}
			if (this.stOtherData != null)
			{
				this.stOtherData.Release();
				this.stOtherData = null;
			}
		}

		public override void OnUse()
		{
			this.stStatisticData = (COMDT_STATISTIC_DATA)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA.CLASS_ID);
			this.stHeroData = (COMDT_SETTLE_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_SETTLE_HERO_DETAIL.CLASS_ID);
			this.stGeneralData = (COMDT_SETTLE_GAME_GENERAL_INFO)ProtocolObjectPool.Get(COMDT_SETTLE_GAME_GENERAL_INFO.CLASS_ID);
			this.stNonHeroData = (COMDT_NONHERO_DETAIL)ProtocolObjectPool.Get(COMDT_NONHERO_DETAIL.CLASS_ID);
			this.stOtherData = (COMDT_SETTLE_OTHERINFO)ProtocolObjectPool.Get(COMDT_SETTLE_OTHERINFO.CLASS_ID);
		}
	}
}
