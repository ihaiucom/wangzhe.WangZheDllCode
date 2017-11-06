using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER : ProtocolObject
	{
		public uint dwGuildHeadID;

		public byte[] szGuildName;

		public byte bGuildLevel;

		public byte bMemberNum;

		public uint dwChairManHeadID;

		public byte[] szChairManHeadUrl;

		public byte[] szChairManName;

		public uint dwChairManLv;

		public byte[] szGuildNotice;

		public COMDT_GAME_VIP_CLIENT stChairManVip;

		public uint dwStar;

		public uint dwTotalRankPoint;

		public uint dwSettingMask;

		public byte bLimitLevel;

		public byte bLimitGrade;

		public int iGuildLogicWorldID;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly uint VERSION_stChairManVip = 45u;

		public static readonly uint VERSION_dwStar = 82u;

		public static readonly uint VERSION_dwTotalRankPoint = 93u;

		public static readonly uint VERSION_dwSettingMask = 149u;

		public static readonly uint VERSION_bLimitLevel = 149u;

		public static readonly uint VERSION_bLimitGrade = 149u;

		public static readonly uint VERSION_iGuildLogicWorldID = 240u;

		public static readonly uint LENGTH_szGuildName = 32u;

		public static readonly uint LENGTH_szChairManHeadUrl = 256u;

		public static readonly uint LENGTH_szChairManName = 64u;

		public static readonly uint LENGTH_szGuildNotice = 128u;

		public static readonly int CLASS_ID = 513;

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER()
		{
			this.szGuildName = new byte[32];
			this.szChairManHeadUrl = new byte[256];
			this.szChairManName = new byte[64];
			this.szGuildNotice = new byte[128];
			this.stChairManVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwGuildHeadID);
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
			int num = TdrTypeUtil.cstrlen(this.szGuildName);
			if (num >= 32)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szGuildName, num);
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
			errorType = destBuf.writeUInt8(this.bGuildLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMemberNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwChairManHeadID);
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
			int num2 = TdrTypeUtil.cstrlen(this.szChairManHeadUrl);
			if (num2 >= 256)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szChairManHeadUrl, num2);
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
			int usedSize5 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize6 = destBuf.getUsedSize();
			int num3 = TdrTypeUtil.cstrlen(this.szChairManName);
			if (num3 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szChairManName, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src3 = destBuf.getUsedSize() - usedSize6;
			errorType = destBuf.writeUInt32((uint)src3, usedSize5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwChairManLv);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize7 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize8 = destBuf.getUsedSize();
			int num4 = TdrTypeUtil.cstrlen(this.szGuildNotice);
			if (num4 >= 128)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szGuildNotice, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src4 = destBuf.getUsedSize() - usedSize8;
			errorType = destBuf.writeUInt32((uint)src4, usedSize7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_stChairManVip <= cutVer)
			{
				errorType = this.stChairManVip.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_dwStar <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwStar);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_dwTotalRankPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwTotalRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_dwSettingMask <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSettingMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_bLimitLevel <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bLimitLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_bLimitGrade <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bLimitGrade);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_iGuildLogicWorldID <= cutVer)
			{
				errorType = destBuf.writeInt32(this.iGuildLogicWorldID);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwGuildHeadID);
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
			if (num > (uint)this.szGuildName.GetLength(0))
			{
				if ((ulong)num > (ulong)COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.LENGTH_szGuildName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGuildName = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGuildName, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGuildName[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szGuildName) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bGuildLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMemberNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwChairManHeadID);
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
			if (num3 > (uint)this.szChairManHeadUrl.GetLength(0))
			{
				if ((ulong)num3 > (ulong)COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.LENGTH_szChairManHeadUrl)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szChairManHeadUrl = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szChairManHeadUrl, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szChairManHeadUrl[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szChairManHeadUrl) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num5 = 0u;
			errorType = srcBuf.readUInt32(ref num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num5 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num5 > (uint)this.szChairManName.GetLength(0))
			{
				if ((ulong)num5 > (ulong)COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.LENGTH_szChairManName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szChairManName = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szChairManName, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szChairManName[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szChairManName) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwChairManLv);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num7 = 0u;
			errorType = srcBuf.readUInt32(ref num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num7 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num7 > (uint)this.szGuildNotice.GetLength(0))
			{
				if ((ulong)num7 > (ulong)COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.LENGTH_szGuildNotice)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGuildNotice = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGuildNotice, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGuildNotice[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szGuildNotice) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_stChairManVip <= cutVer)
			{
				errorType = this.stChairManVip.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stChairManVip.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_dwStar <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwStar);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwStar = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_dwTotalRankPoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwTotalRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwTotalRankPoint = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_dwSettingMask <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwSettingMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwSettingMask = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_bLimitLevel <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bLimitLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bLimitLevel = 0;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_bLimitGrade <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bLimitGrade);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bLimitGrade = 0;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.VERSION_iGuildLogicWorldID <= cutVer)
			{
				errorType = srcBuf.readInt32(ref this.iGuildLogicWorldID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.iGuildLogicWorldID = 0;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwGuildHeadID = 0u;
			this.bGuildLevel = 0;
			this.bMemberNum = 0;
			this.dwChairManHeadID = 0u;
			this.dwChairManLv = 0u;
			if (this.stChairManVip != null)
			{
				this.stChairManVip.Release();
				this.stChairManVip = null;
			}
			this.dwStar = 0u;
			this.dwTotalRankPoint = 0u;
			this.dwSettingMask = 0u;
			this.bLimitLevel = 0;
			this.bLimitGrade = 0;
			this.iGuildLogicWorldID = 0;
		}

		public override void OnUse()
		{
			this.stChairManVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
		}
	}
}
