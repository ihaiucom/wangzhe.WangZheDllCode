using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT : ProtocolObject
	{
		public uint dwGuildHeadID;

		public byte[] szGuildName;

		public byte bGuildLevel;

		public byte bMemberNum;

		public uint dwStar;

		public uint dwTotalRankPoint;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 87u;

		public static readonly uint VERSION_dwStar = 82u;

		public static readonly uint VERSION_dwTotalRankPoint = 87u;

		public static readonly uint LENGTH_szGuildName = 32u;

		public static readonly int CLASS_ID = 514;

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT()
		{
			this.szGuildName = new byte[32];
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.BASEVERSION > cutVer)
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
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.VERSION_dwStar <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwStar);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.VERSION_dwTotalRankPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwTotalRankPoint);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.BASEVERSION > cutVer)
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
				if ((ulong)num > (ulong)COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.LENGTH_szGuildName)
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
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.VERSION_dwStar <= cutVer)
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
			if (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.VERSION_dwTotalRankPoint <= cutVer)
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
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwGuildHeadID = 0u;
			this.bGuildLevel = 0;
			this.bMemberNum = 0;
			this.dwStar = 0u;
			this.dwTotalRankPoint = 0u;
		}
	}
}
