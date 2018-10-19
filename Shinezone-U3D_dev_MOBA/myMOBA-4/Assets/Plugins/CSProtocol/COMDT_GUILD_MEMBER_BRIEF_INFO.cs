using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GUILD_MEMBER_BRIEF_INFO : ProtocolObject
	{
		public ulong ullUid;

		public int iLogicWorldID;

		public byte[] szName;

		public uint dwLevel;

		public uint dwAbility;

		public uint dwHeadID;

		public byte[] szHeadUrl;

		public uint dwGameEntity;

		public uint dwReserveGC;

		public COMDT_GAME_VIP_CLIENT stVip;

		public uint dwClassOfRank;

		public uint dwScoreOfRank;

		public byte bGender;

		public uint dwMasterLevel;

		public COMDT_RANKGRADE stRankShowGrade;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly uint VERSION_stVip = 45u;

		public static readonly uint VERSION_dwClassOfRank = 130u;

		public static readonly uint VERSION_dwScoreOfRank = 132u;

		public static readonly uint VERSION_bGender = 153u;

		public static readonly uint VERSION_dwMasterLevel = 205u;

		public static readonly uint VERSION_stRankShowGrade = 234u;

		public static readonly uint LENGTH_szName = 64u;

		public static readonly uint LENGTH_szHeadUrl = 256u;

		public static readonly int CLASS_ID = 362;

		public COMDT_GUILD_MEMBER_BRIEF_INFO()
		{
			this.szName = new byte[64];
			this.szHeadUrl = new byte[256];
			this.stVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stRankShowGrade = (COMDT_RANKGRADE)ProtocolObjectPool.Get(COMDT_RANKGRADE.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GUILD_MEMBER_BRIEF_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_MEMBER_BRIEF_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLogicWorldID);
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
			int num = TdrTypeUtil.cstrlen(this.szName);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szName, num);
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
			errorType = destBuf.writeUInt32(this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwAbility);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwHeadID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize3 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize4 = destBuf.getUsedSize();
			int num2 = TdrTypeUtil.cstrlen(this.szHeadUrl);
			if (num2 >= 256)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szHeadUrl, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src2 = destBuf.getUsedSize() - usedSize4;
			errorType = destBuf.writeUInt32((uint)src2, usedSize3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwReserveGC);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_stVip <= cutVer)
			{
				errorType = this.stVip.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_dwClassOfRank <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_dwScoreOfRank <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwScoreOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_bGender <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGender);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_dwMasterLevel <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMasterLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_stRankShowGrade <= cutVer)
			{
				errorType = this.stRankShowGrade.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_GUILD_MEMBER_BRIEF_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_MEMBER_BRIEF_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLogicWorldID);
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
			if (num > (uint)this.szName.GetLength(0))
			{
				if ((long)num > (long)((ulong)COMDT_GUILD_MEMBER_BRIEF_INFO.LENGTH_szName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szName = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szName, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szName[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szName) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAbility);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeadID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szHeadUrl.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)COMDT_GUILD_MEMBER_BRIEF_INFO.LENGTH_szHeadUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeadUrl = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeadUrl, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeadUrl[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szHeadUrl) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwReserveGC);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_stVip <= cutVer)
			{
				errorType = this.stVip.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stVip.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_dwClassOfRank <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwClassOfRank = 0u;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_dwScoreOfRank <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwScoreOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwScoreOfRank = 0u;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_bGender <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bGender);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bGender = 0;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_dwMasterLevel <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMasterLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMasterLevel = 0u;
			}
			if (COMDT_GUILD_MEMBER_BRIEF_INFO.VERSION_stRankShowGrade <= cutVer)
			{
				errorType = this.stRankShowGrade.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stRankShowGrade.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullUid = 0uL;
			this.iLogicWorldID = 0;
			this.dwLevel = 0u;
			this.dwAbility = 0u;
			this.dwHeadID = 0u;
			this.dwGameEntity = 0u;
			this.dwReserveGC = 0u;
			if (this.stVip != null)
			{
				this.stVip.Release();
				this.stVip = null;
			}
			this.dwClassOfRank = 0u;
			this.dwScoreOfRank = 0u;
			this.bGender = 0;
			this.dwMasterLevel = 0u;
			if (this.stRankShowGrade != null)
			{
				this.stRankShowGrade.Release();
				this.stRankShowGrade = null;
			}
		}

		public override void OnUse()
		{
			this.stVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stRankShowGrade = (COMDT_RANKGRADE)ProtocolObjectPool.Get(COMDT_RANKGRADE.CLASS_ID);
		}
	}
}
