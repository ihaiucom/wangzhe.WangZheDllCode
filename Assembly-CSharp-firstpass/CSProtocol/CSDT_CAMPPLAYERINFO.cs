using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_CAMPPLAYERINFO : ProtocolObject
	{
		public COMDT_PLAYERINFO stPlayerInfo;

		public byte bIsGM;

		public byte[] szOpenID;

		public uint dwShowGradeOfRank;

		public uint dwClassOfRank;

		public uint dwRandomHeroCnt;

		public COMDT_ACNT_USABLE_HERO stUsableHero;

		public COMDT_RECENT_USED_HERO stRecentUsedHero;

		public COMDT_INTIMACY_RELATION_INFO stIntimacyRelation;

		public ulong ullUserPrivacyBits;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly uint LENGTH_szOpenID = 64u;

		public static readonly int CLASS_ID = 755;

		public CSDT_CAMPPLAYERINFO()
		{
			this.stPlayerInfo = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
			this.szOpenID = new byte[64];
			this.stUsableHero = (COMDT_ACNT_USABLE_HERO)ProtocolObjectPool.Get(COMDT_ACNT_USABLE_HERO.CLASS_ID);
			this.stRecentUsedHero = (COMDT_RECENT_USED_HERO)ProtocolObjectPool.Get(COMDT_RECENT_USED_HERO.CLASS_ID);
			this.stIntimacyRelation = (COMDT_INTIMACY_RELATION_INFO)ProtocolObjectPool.Get(COMDT_INTIMACY_RELATION_INFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stPlayerInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.bIsGM = 0;
			this.dwShowGradeOfRank = 0u;
			this.dwClassOfRank = 0u;
			this.dwRandomHeroCnt = 0u;
			errorType = this.stUsableHero.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentUsedHero.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stIntimacyRelation.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.ullUserPrivacyBits = 0uL;
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
			if (cutVer == 0u || CSDT_CAMPPLAYERINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CAMPPLAYERINFO.CURRVERSION;
			}
			if (CSDT_CAMPPLAYERINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stPlayerInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsGM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szOpenID);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szOpenID, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src = destBuf.getUsedSize() - usedSize2;
			errorType = destBuf.writeUInt32((uint)src, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwShowGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRandomHeroCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stUsableHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentUsedHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stIntimacyRelation.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullUserPrivacyBits);
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
			if (cutVer == 0u || CSDT_CAMPPLAYERINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CAMPPLAYERINFO.CURRVERSION;
			}
			if (CSDT_CAMPPLAYERINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stPlayerInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsGM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szOpenID.GetLength(0))
			{
				if ((ulong)num > (ulong)CSDT_CAMPPLAYERINFO.LENGTH_szOpenID)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szOpenID = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szOpenID, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szOpenID[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szOpenID) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRandomHeroCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stUsableHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentUsedHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stIntimacyRelation.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullUserPrivacyBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_CAMPPLAYERINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stPlayerInfo != null)
			{
				this.stPlayerInfo.Release();
				this.stPlayerInfo = null;
			}
			this.bIsGM = 0;
			this.dwShowGradeOfRank = 0u;
			this.dwClassOfRank = 0u;
			this.dwRandomHeroCnt = 0u;
			if (this.stUsableHero != null)
			{
				this.stUsableHero.Release();
				this.stUsableHero = null;
			}
			if (this.stRecentUsedHero != null)
			{
				this.stRecentUsedHero.Release();
				this.stRecentUsedHero = null;
			}
			if (this.stIntimacyRelation != null)
			{
				this.stIntimacyRelation.Release();
				this.stIntimacyRelation = null;
			}
			this.ullUserPrivacyBits = 0uL;
		}

		public override void OnUse()
		{
			this.stPlayerInfo = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
			this.stUsableHero = (COMDT_ACNT_USABLE_HERO)ProtocolObjectPool.Get(COMDT_ACNT_USABLE_HERO.CLASS_ID);
			this.stRecentUsedHero = (COMDT_RECENT_USED_HERO)ProtocolObjectPool.Get(COMDT_RECENT_USED_HERO.CLASS_ID);
			this.stIntimacyRelation = (COMDT_INTIMACY_RELATION_INFO)ProtocolObjectPool.Get(COMDT_INTIMACY_RELATION_INFO.CLASS_ID);
		}
	}
}
