using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MOST_USED_HERO_INFO : ProtocolObject
	{
		public uint dwHeroID;

		public uint dwSkinID;

		public uint dwGameWinNum;

		public uint dwGameLoseNum;

		public uint dwProficiencyLv;

		public uint dwProficiency;

		public COMDT_HERO_STATISTIC_DETAIL stStatisticDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_dwSkinID = 180u;

		public static readonly uint VERSION_stStatisticDetail = 136u;

		public static readonly int CLASS_ID = 117;

		public COMDT_MOST_USED_HERO_INFO()
		{
			this.stStatisticDetail = (COMDT_HERO_STATISTIC_DETAIL)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_MOST_USED_HERO_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MOST_USED_HERO_INFO.CURRVERSION;
			}
			if (COMDT_MOST_USED_HERO_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_MOST_USED_HERO_INFO.VERSION_dwSkinID <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSkinID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwGameWinNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameLoseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwProficiencyLv);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwProficiency);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_MOST_USED_HERO_INFO.VERSION_stStatisticDetail <= cutVer)
			{
				errorType = this.stStatisticDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_MOST_USED_HERO_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MOST_USED_HERO_INFO.CURRVERSION;
			}
			if (COMDT_MOST_USED_HERO_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_MOST_USED_HERO_INFO.VERSION_dwSkinID <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwSkinID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwSkinID = 0u;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameWinNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameLoseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwProficiencyLv);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwProficiency);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_MOST_USED_HERO_INFO.VERSION_stStatisticDetail <= cutVer)
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
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MOST_USED_HERO_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroID = 0u;
			this.dwSkinID = 0u;
			this.dwGameWinNum = 0u;
			this.dwGameLoseNum = 0u;
			this.dwProficiencyLv = 0u;
			this.dwProficiency = 0u;
			if (this.stStatisticDetail != null)
			{
				this.stStatisticDetail.Release();
				this.stStatisticDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stStatisticDetail = (COMDT_HERO_STATISTIC_DETAIL)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_DETAIL.CLASS_ID);
		}
	}
}
