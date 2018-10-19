using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER : ProtocolObject
	{
		public ulong ullUid;

		public int iLogicWorldId;

		public uint dwPvpLevel;

		public byte[] szHeadUrl;

		public byte[] szPlayerName;

		public uint dwVipLevel;

		public COMDT_GAME_VIP_CLIENT stGameVip;

		public byte bPrivilege;

		public uint dwPrivilegeRewardTime;

		public COMDT_TRANK_GRADEOFRANK_INFO stRankInfo;

		public ulong ullUserPrivacyBits;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly uint VERSION_dwPvpLevel = 20u;

		public static readonly uint VERSION_dwVipLevel = 34u;

		public static readonly uint VERSION_stGameVip = 42u;

		public static readonly uint VERSION_bPrivilege = 63u;

		public static readonly uint VERSION_dwPrivilegeRewardTime = 63u;

		public static readonly uint VERSION_stRankInfo = 107u;

		public static readonly uint VERSION_ullUserPrivacyBits = 209u;

		public static readonly uint LENGTH_szHeadUrl = 256u;

		public static readonly uint LENGTH_szPlayerName = 64u;

		public static readonly int CLASS_ID = 512;

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER()
		{
			this.szHeadUrl = new byte[256];
			this.szPlayerName = new byte[64];
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stRankInfo = (COMDT_TRANK_GRADEOFRANK_INFO)ProtocolObjectPool.Get(COMDT_TRANK_GRADEOFRANK_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLogicWorldId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_dwPvpLevel <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwPvpLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int usedSize = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szHeadUrl);
			if (num >= 256)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szHeadUrl, num);
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
			int usedSize3 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize4 = destBuf.getUsedSize();
			int num2 = TdrTypeUtil.cstrlen(this.szPlayerName);
			if (num2 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szPlayerName, num2);
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
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_dwVipLevel <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwVipLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_stGameVip <= cutVer)
			{
				errorType = this.stGameVip.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_bPrivilege <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_dwPrivilegeRewardTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwPrivilegeRewardTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_stRankInfo <= cutVer)
			{
				errorType = this.stRankInfo.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_ullUserPrivacyBits <= cutVer)
			{
				errorType = destBuf.writeUInt64(this.ullUserPrivacyBits);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLogicWorldId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_dwPvpLevel <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwPvpLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwPvpLevel = 0u;
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
			if (num > (uint)this.szHeadUrl.GetLength(0))
			{
				if ((long)num > (long)((ulong)COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.LENGTH_szHeadUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeadUrl = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeadUrl, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeadUrl[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szHeadUrl) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num3 > (uint)this.szPlayerName.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.LENGTH_szPlayerName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szPlayerName = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szPlayerName, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szPlayerName[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szPlayerName) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_dwVipLevel <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwVipLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwVipLevel = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_stGameVip <= cutVer)
			{
				errorType = this.stGameVip.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stGameVip.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_bPrivilege <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bPrivilege = 0;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_dwPrivilegeRewardTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwPrivilegeRewardTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwPrivilegeRewardTime = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_stRankInfo <= cutVer)
			{
				errorType = this.stRankInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stRankInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.VERSION_ullUserPrivacyBits <= cutVer)
			{
				errorType = srcBuf.readUInt64(ref this.ullUserPrivacyBits);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.ullUserPrivacyBits = 0uL;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullUid = 0uL;
			this.iLogicWorldId = 0;
			this.dwPvpLevel = 0u;
			this.dwVipLevel = 0u;
			if (this.stGameVip != null)
			{
				this.stGameVip.Release();
				this.stGameVip = null;
			}
			this.bPrivilege = 0;
			this.dwPrivilegeRewardTime = 0u;
			if (this.stRankInfo != null)
			{
				this.stRankInfo.Release();
				this.stRankInfo = null;
			}
			this.ullUserPrivacyBits = 0uL;
		}

		public override void OnUse()
		{
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stRankInfo = (COMDT_TRANK_GRADEOFRANK_INFO)ProtocolObjectPool.Get(COMDT_TRANK_GRADEOFRANK_INFO.CLASS_ID);
		}
	}
}
