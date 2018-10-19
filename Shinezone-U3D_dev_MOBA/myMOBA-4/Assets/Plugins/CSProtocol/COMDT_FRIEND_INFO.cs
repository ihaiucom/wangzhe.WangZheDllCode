using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_FRIEND_INFO : ProtocolObject
	{
		public COMDT_ACNT_UNIQ stUin;

		public byte[] szUserName;

		public uint dwLevel;

		public uint dwVipLvl;

		public byte bIsOnline;

		public uint dwLastLoginTime;

		public uint dwHeadID;

		public byte bGuildState;

		public uint dwPvpLvl;

		public uint dwRankClass;

		public uint dwRefuseFriendBits;

		public byte[] szHeadUrl;

		public uint dwQQVIPMask;

		public uint[] RankVal;

		public uint dwVipScore;

		public COMDT_GAME_VIP_CLIENT stGameVip;

		public byte[] szOpenId;

		public byte bPrivilege;

		public byte bGender;

		public uint dwMasterLvl;

		public uint dwStudentNum;

		public byte bStudentType;

		public ulong ullUserPrivacyBits;

		public COMDT_RANKGRADE stRankShowGrade;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly uint VERSION_dwRefuseFriendBits = 99u;

		public static readonly uint VERSION_szHeadUrl = 3u;

		public static readonly uint VERSION_dwQQVIPMask = 17u;

		public static readonly uint VERSION_RankVal = 32u;

		public static readonly uint VERSION_dwVipScore = 34u;

		public static readonly uint VERSION_stGameVip = 42u;

		public static readonly uint VERSION_szOpenId = 47u;

		public static readonly uint VERSION_bPrivilege = 63u;

		public static readonly uint VERSION_bGender = 70u;

		public static readonly uint VERSION_dwMasterLvl = 173u;

		public static readonly uint VERSION_dwStudentNum = 173u;

		public static readonly uint VERSION_bStudentType = 173u;

		public static readonly uint VERSION_ullUserPrivacyBits = 209u;

		public static readonly uint VERSION_stRankShowGrade = 234u;

		public static readonly uint LENGTH_szUserName = 64u;

		public static readonly uint LENGTH_szHeadUrl = 256u;

		public static readonly uint LENGTH_szOpenId = 64u;

		public static readonly int CLASS_ID = 261;

		public COMDT_FRIEND_INFO()
		{
			this.stUin = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.szUserName = new byte[64];
			this.szHeadUrl = new byte[256];
			this.RankVal = new uint[32];
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.szOpenId = new byte[64];
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
			if (cutVer == 0u || COMDT_FRIEND_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FRIEND_INFO.CURRVERSION;
			}
			if (COMDT_FRIEND_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stUin.pack(ref destBuf, cutVer);
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
			int num = TdrTypeUtil.cstrlen(this.szUserName);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szUserName, num);
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
			errorType = destBuf.writeUInt32(this.dwVipLvl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsOnline);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastLoginTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwHeadID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGuildState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPvpLvl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRankClass);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_FRIEND_INFO.VERSION_dwRefuseFriendBits <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwRefuseFriendBits);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_szHeadUrl <= cutVer)
			{
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
			}
			if (COMDT_FRIEND_INFO.VERSION_dwQQVIPMask <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwQQVIPMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_RankVal <= cutVer)
			{
				for (int i = 0; i < 32; i++)
				{
					errorType = destBuf.writeUInt32(this.RankVal[i]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_dwVipScore <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwVipScore);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_stGameVip <= cutVer)
			{
				errorType = this.stGameVip.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_szOpenId <= cutVer)
			{
				int usedSize5 = destBuf.getUsedSize();
				errorType = destBuf.reserve(4);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				int usedSize6 = destBuf.getUsedSize();
				int num3 = TdrTypeUtil.cstrlen(this.szOpenId);
				if (num3 >= 64)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				errorType = destBuf.writeCString(this.szOpenId, num3);
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
			}
			if (COMDT_FRIEND_INFO.VERSION_bPrivilege <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_bGender <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGender);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_dwMasterLvl <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMasterLvl);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_dwStudentNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwStudentNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_bStudentType <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bStudentType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_ullUserPrivacyBits <= cutVer)
			{
				errorType = destBuf.writeUInt64(this.ullUserPrivacyBits);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_stRankShowGrade <= cutVer)
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
			if (cutVer == 0u || COMDT_FRIEND_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FRIEND_INFO.CURRVERSION;
			}
			if (COMDT_FRIEND_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stUin.unpack(ref srcBuf, cutVer);
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
			if (num > (uint)this.szUserName.GetLength(0))
			{
				if ((long)num > (long)((ulong)COMDT_FRIEND_INFO.LENGTH_szUserName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUserName = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUserName, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUserName[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szUserName) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwVipLvl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOnline);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastLoginTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeadID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGuildState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpLvl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRankClass);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_FRIEND_INFO.VERSION_dwRefuseFriendBits <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwRefuseFriendBits);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwRefuseFriendBits = 0u;
			}
			if (COMDT_FRIEND_INFO.VERSION_szHeadUrl <= cutVer)
			{
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
					if ((long)num3 > (long)((ulong)COMDT_FRIEND_INFO.LENGTH_szHeadUrl))
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
			}
			if (COMDT_FRIEND_INFO.VERSION_dwQQVIPMask <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwQQVIPMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwQQVIPMask = 0u;
			}
			if (COMDT_FRIEND_INFO.VERSION_RankVal <= cutVer)
			{
				for (int i = 0; i < 32; i++)
				{
					errorType = srcBuf.readUInt32(ref this.RankVal[i]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_dwVipScore <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwVipScore);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwVipScore = 0u;
			}
			if (COMDT_FRIEND_INFO.VERSION_stGameVip <= cutVer)
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
			if (COMDT_FRIEND_INFO.VERSION_szOpenId <= cutVer)
			{
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
				if (num5 > (uint)this.szOpenId.GetLength(0))
				{
					if ((long)num5 > (long)((ulong)COMDT_FRIEND_INFO.LENGTH_szOpenId))
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					this.szOpenId = new byte[num5];
				}
				if (1u > num5)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
				}
				errorType = srcBuf.readCString(ref this.szOpenId, (int)num5);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				if (this.szOpenId[(int)(num5 - 1u)] != 0)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
				int num6 = TdrTypeUtil.cstrlen(this.szOpenId) + 1;
				if ((ulong)num5 != (ulong)((long)num6))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
			}
			if (COMDT_FRIEND_INFO.VERSION_bPrivilege <= cutVer)
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
			if (COMDT_FRIEND_INFO.VERSION_bGender <= cutVer)
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
			if (COMDT_FRIEND_INFO.VERSION_dwMasterLvl <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMasterLvl);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMasterLvl = 0u;
			}
			if (COMDT_FRIEND_INFO.VERSION_dwStudentNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwStudentNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwStudentNum = 0u;
			}
			if (COMDT_FRIEND_INFO.VERSION_bStudentType <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bStudentType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bStudentType = 0;
			}
			if (COMDT_FRIEND_INFO.VERSION_ullUserPrivacyBits <= cutVer)
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
			if (COMDT_FRIEND_INFO.VERSION_stRankShowGrade <= cutVer)
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
			return COMDT_FRIEND_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stUin != null)
			{
				this.stUin.Release();
				this.stUin = null;
			}
			this.dwLevel = 0u;
			this.dwVipLvl = 0u;
			this.bIsOnline = 0;
			this.dwLastLoginTime = 0u;
			this.dwHeadID = 0u;
			this.bGuildState = 0;
			this.dwPvpLvl = 0u;
			this.dwRankClass = 0u;
			this.dwRefuseFriendBits = 0u;
			this.dwQQVIPMask = 0u;
			this.dwVipScore = 0u;
			if (this.stGameVip != null)
			{
				this.stGameVip.Release();
				this.stGameVip = null;
			}
			this.bPrivilege = 0;
			this.bGender = 0;
			this.dwMasterLvl = 0u;
			this.dwStudentNum = 0u;
			this.bStudentType = 0;
			this.ullUserPrivacyBits = 0uL;
			if (this.stRankShowGrade != null)
			{
				this.stRankShowGrade.Release();
				this.stRankShowGrade = null;
			}
		}

		public override void OnUse()
		{
			this.stUin = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stRankShowGrade = (COMDT_RANKGRADE)ProtocolObjectPool.Get(COMDT_RANKGRADE.CLASS_ID);
		}
	}
}
