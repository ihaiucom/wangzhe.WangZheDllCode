using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_COMMON_INFO : ProtocolObject
	{
		public uint dwHeroID;

		public uint dwMaskBits;

		public ushort wLevel;

		public ushort wStar;

		public COMDT_ACNTHERO_QUALITY stQuality;

		public uint dwExp;

		public COMDT_SKILLARRAY stSkill;

		public COMDT_HERO_PROFICIENCY stProficiency;

		public byte bSymbolPageWear;

		public ushort wSkinID;

		public uint dwGameWinNum;

		public uint dwGameLoseNum;

		public COMDT_TALENTARRAY stTalent;

		public uint dwRankGameTotalFightCnt;

		public uint dwRankGameTotalWinCnt;

		public uint dwDeadLine;

		public COMDT_HERO_STATISTIC_DETAIL stStatisticDetail;

		public uint dwLastMasterGameSec;

		public uint dwMasterTotalFightCnt;

		public uint dwMasterTotalWinCnt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_dwGameWinNum = 22u;

		public static readonly uint VERSION_dwGameLoseNum = 22u;

		public static readonly uint VERSION_stTalent = 26u;

		public static readonly uint VERSION_dwRankGameTotalFightCnt = 33u;

		public static readonly uint VERSION_dwRankGameTotalWinCnt = 33u;

		public static readonly uint VERSION_dwDeadLine = 51u;

		public static readonly uint VERSION_stStatisticDetail = 136u;

		public static readonly uint VERSION_dwLastMasterGameSec = 138u;

		public static readonly uint VERSION_dwMasterTotalFightCnt = 138u;

		public static readonly uint VERSION_dwMasterTotalWinCnt = 138u;

		public static readonly int CLASS_ID = 111;

		public COMDT_HERO_COMMON_INFO()
		{
			this.stQuality = (COMDT_ACNTHERO_QUALITY)ProtocolObjectPool.Get(COMDT_ACNTHERO_QUALITY.CLASS_ID);
			this.stSkill = (COMDT_SKILLARRAY)ProtocolObjectPool.Get(COMDT_SKILLARRAY.CLASS_ID);
			this.stProficiency = (COMDT_HERO_PROFICIENCY)ProtocolObjectPool.Get(COMDT_HERO_PROFICIENCY.CLASS_ID);
			this.stTalent = (COMDT_TALENTARRAY)ProtocolObjectPool.Get(COMDT_TALENTARRAY.CLASS_ID);
			this.stStatisticDetail = (COMDT_HERO_STATISTIC_DETAIL)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_DETAIL.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.dwHeroID = 0u;
			this.dwMaskBits = 0u;
			this.wLevel = 1;
			this.wStar = 0;
			TdrError.ErrorType errorType = this.stQuality.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.dwExp = 0u;
			errorType = this.stSkill.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stProficiency.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.bSymbolPageWear = 0;
			this.wSkinID = 0;
			this.dwGameWinNum = 0u;
			this.dwGameLoseNum = 0u;
			errorType = this.stTalent.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.dwRankGameTotalFightCnt = 0u;
			this.dwRankGameTotalWinCnt = 0u;
			this.dwDeadLine = 0u;
			errorType = this.stStatisticDetail.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.dwLastMasterGameSec = 0u;
			this.dwMasterTotalFightCnt = 0u;
			this.dwMasterTotalWinCnt = 0u;
			return errorType;
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
			if (cutVer == 0u || COMDT_HERO_COMMON_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_COMMON_INFO.CURRVERSION;
			}
			if (COMDT_HERO_COMMON_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMaskBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wStar);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stQuality.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSkill.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stProficiency.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bSymbolPageWear);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wSkinID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwGameWinNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGameWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwGameLoseNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGameLoseNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_stTalent <= cutVer)
			{
				errorType = this.stTalent.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwRankGameTotalFightCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwRankGameTotalFightCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwRankGameTotalWinCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwRankGameTotalWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwDeadLine <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDeadLine);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_stStatisticDetail <= cutVer)
			{
				errorType = this.stStatisticDetail.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwLastMasterGameSec <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastMasterGameSec);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwMasterTotalFightCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMasterTotalFightCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwMasterTotalWinCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMasterTotalWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || COMDT_HERO_COMMON_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_COMMON_INFO.CURRVERSION;
			}
			if (COMDT_HERO_COMMON_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMaskBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wStar);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stQuality.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSkill.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stProficiency.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSymbolPageWear);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wSkinID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwGameWinNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGameWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGameWinNum = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwGameLoseNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGameLoseNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGameLoseNum = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_stTalent <= cutVer)
			{
				errorType = this.stTalent.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stTalent.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwRankGameTotalFightCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwRankGameTotalFightCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwRankGameTotalFightCnt = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwRankGameTotalWinCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwRankGameTotalWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwRankGameTotalWinCnt = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwDeadLine <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDeadLine);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDeadLine = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_stStatisticDetail <= cutVer)
			{
				errorType = this.stStatisticDetail.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stStatisticDetail.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwLastMasterGameSec <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastMasterGameSec);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastMasterGameSec = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwMasterTotalFightCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMasterTotalFightCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMasterTotalFightCnt = 0u;
			}
			if (COMDT_HERO_COMMON_INFO.VERSION_dwMasterTotalWinCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMasterTotalWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMasterTotalWinCnt = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_HERO_COMMON_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroID = 0u;
			this.dwMaskBits = 0u;
			this.wLevel = 0;
			this.wStar = 0;
			if (this.stQuality != null)
			{
				this.stQuality.Release();
				this.stQuality = null;
			}
			this.dwExp = 0u;
			if (this.stSkill != null)
			{
				this.stSkill.Release();
				this.stSkill = null;
			}
			if (this.stProficiency != null)
			{
				this.stProficiency.Release();
				this.stProficiency = null;
			}
			this.bSymbolPageWear = 0;
			this.wSkinID = 0;
			this.dwGameWinNum = 0u;
			this.dwGameLoseNum = 0u;
			if (this.stTalent != null)
			{
				this.stTalent.Release();
				this.stTalent = null;
			}
			this.dwRankGameTotalFightCnt = 0u;
			this.dwRankGameTotalWinCnt = 0u;
			this.dwDeadLine = 0u;
			if (this.stStatisticDetail != null)
			{
				this.stStatisticDetail.Release();
				this.stStatisticDetail = null;
			}
			this.dwLastMasterGameSec = 0u;
			this.dwMasterTotalFightCnt = 0u;
			this.dwMasterTotalWinCnt = 0u;
		}

		public override void OnUse()
		{
			this.stQuality = (COMDT_ACNTHERO_QUALITY)ProtocolObjectPool.Get(COMDT_ACNTHERO_QUALITY.CLASS_ID);
			this.stSkill = (COMDT_SKILLARRAY)ProtocolObjectPool.Get(COMDT_SKILLARRAY.CLASS_ID);
			this.stProficiency = (COMDT_HERO_PROFICIENCY)ProtocolObjectPool.Get(COMDT_HERO_PROFICIENCY.CLASS_ID);
			this.stTalent = (COMDT_TALENTARRAY)ProtocolObjectPool.Get(COMDT_TALENTARRAY.CLASS_ID);
			this.stStatisticDetail = (COMDT_HERO_STATISTIC_DETAIL)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_DETAIL.CLASS_ID);
		}
	}
}
