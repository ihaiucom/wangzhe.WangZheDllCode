using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_ACNT_DETAIL_INFO : ProtocolObject
	{
		public ulong ullUid;

		public byte[] szAcntName;

		public byte[] szOpenID;

		public int iLogicWorldId;

		public byte bIsOnline;

		public uint dwLastLoginTime;

		public byte[] szOpenUrl;

		public uint dwLevel;

		public uint dwExp;

		public uint dwPower;

		public uint dwPvpLevel;

		public uint dwPvpExp;

		public byte bGradeOfRank;

		public byte bMaxGradeOfRank;

		public uint dwWangZheCnt;

		public COMDT_GAME_VIP_CLIENT stGameVip;

		public CSDT_PVPDETAIL_INFO stStatistic;

		public COMDT_ACNT_GUILD_INFO stGuildInfo;

		public COMDT_ACNT_BANTIME stBanTime;

		public COMDT_MOST_USED_HERO_DETAIL stMostUsedHero;

		public uint dwQQVIPMask;

		public byte bPrivilege;

		public byte bGender;

		public COMDT_ACNT_HONORINFO stHonorInfo;

		public uint dwCreditValue;

		public COMDT_RANKDETAIL stRankInfo;

		public uint dwCurClassOfRank;

		public COMDT_LIKE_NUMS stLikeNum;

		public uint dwAchieveMentScore;

		public CSDT_SHOWACHIEVE_DETAIL[] astShowAchievement;

		public byte[] szSignatureInfo;

		public uint dwRefuseFriendBits;

		public COMDT_ACNT_MASTER_INFO stAcntMasterInfo;

		public int iSumDelCreditValue;

		public uint dwMostDelCreditType;

		public ulong ullUserPrivacyBits;

		public COMDT_FRIEND_CARD stFriendCard;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly uint VERSION_dwQQVIPMask = 49u;

		public static readonly uint VERSION_bPrivilege = 62u;

		public static readonly uint VERSION_bGender = 70u;

		public static readonly uint VERSION_stLikeNum = 126u;

		public static readonly uint LENGTH_szAcntName = 64u;

		public static readonly uint LENGTH_szOpenID = 64u;

		public static readonly uint LENGTH_szOpenUrl = 256u;

		public static readonly uint LENGTH_szSignatureInfo = 128u;

		public static readonly int CLASS_ID = 1238;

		public CSDT_ACNT_DETAIL_INFO()
		{
			this.szAcntName = new byte[64];
			this.szOpenID = new byte[64];
			this.szOpenUrl = new byte[256];
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stStatistic = (CSDT_PVPDETAIL_INFO)ProtocolObjectPool.Get(CSDT_PVPDETAIL_INFO.CLASS_ID);
			this.stGuildInfo = (COMDT_ACNT_GUILD_INFO)ProtocolObjectPool.Get(COMDT_ACNT_GUILD_INFO.CLASS_ID);
			this.stBanTime = (COMDT_ACNT_BANTIME)ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID);
			this.stMostUsedHero = (COMDT_MOST_USED_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_MOST_USED_HERO_DETAIL.CLASS_ID);
			this.stHonorInfo = (COMDT_ACNT_HONORINFO)ProtocolObjectPool.Get(COMDT_ACNT_HONORINFO.CLASS_ID);
			this.stRankInfo = (COMDT_RANKDETAIL)ProtocolObjectPool.Get(COMDT_RANKDETAIL.CLASS_ID);
			this.stLikeNum = (COMDT_LIKE_NUMS)ProtocolObjectPool.Get(COMDT_LIKE_NUMS.CLASS_ID);
			this.astShowAchievement = new CSDT_SHOWACHIEVE_DETAIL[3];
			for (int i = 0; i < 3; i++)
			{
				this.astShowAchievement[i] = (CSDT_SHOWACHIEVE_DETAIL)ProtocolObjectPool.Get(CSDT_SHOWACHIEVE_DETAIL.CLASS_ID);
			}
			this.szSignatureInfo = new byte[128];
			this.stAcntMasterInfo = (COMDT_ACNT_MASTER_INFO)ProtocolObjectPool.Get(COMDT_ACNT_MASTER_INFO.CLASS_ID);
			this.stFriendCard = (COMDT_FRIEND_CARD)ProtocolObjectPool.Get(COMDT_FRIEND_CARD.CLASS_ID);
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
			if (cutVer == 0u || CSDT_ACNT_DETAIL_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_ACNT_DETAIL_INFO.CURRVERSION;
			}
			if (CSDT_ACNT_DETAIL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullUid);
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
			int num = TdrTypeUtil.cstrlen(this.szAcntName);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szAcntName, num);
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
			int num2 = TdrTypeUtil.cstrlen(this.szOpenID);
			if (num2 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szOpenID, num2);
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
			errorType = destBuf.writeInt32(this.iLogicWorldId);
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
			int usedSize5 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize6 = destBuf.getUsedSize();
			int num3 = TdrTypeUtil.cstrlen(this.szOpenUrl);
			if (num3 >= 256)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szOpenUrl, num3);
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
			errorType = destBuf.writeUInt32(this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPower);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPvpLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPvpExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMaxGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwWangZheCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGameVip.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stStatistic.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBanTime.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMostUsedHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (CSDT_ACNT_DETAIL_INFO.VERSION_dwQQVIPMask <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwQQVIPMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (CSDT_ACNT_DETAIL_INFO.VERSION_bPrivilege <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (CSDT_ACNT_DETAIL_INFO.VERSION_bGender <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGender);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stHonorInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRankInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCurClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (CSDT_ACNT_DETAIL_INFO.VERSION_stLikeNum <= cutVer)
			{
				errorType = this.stLikeNum.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwAchieveMentScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astShowAchievement[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int usedSize7 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize8 = destBuf.getUsedSize();
			int num4 = TdrTypeUtil.cstrlen(this.szSignatureInfo);
			if (num4 >= 128)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szSignatureInfo, num4);
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
			errorType = destBuf.writeUInt32(this.dwRefuseFriendBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntMasterInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iSumDelCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMostDelCreditType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullUserPrivacyBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendCard.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_ACNT_DETAIL_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_ACNT_DETAIL_INFO.CURRVERSION;
			}
			if (CSDT_ACNT_DETAIL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullUid);
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
			if (num > (uint)this.szAcntName.GetLength(0))
			{
				if ((long)num > (long)((ulong)CSDT_ACNT_DETAIL_INFO.LENGTH_szAcntName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAcntName = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAcntName, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAcntName[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szAcntName) + 1;
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
			if (num3 > (uint)this.szOpenID.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)CSDT_ACNT_DETAIL_INFO.LENGTH_szOpenID))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szOpenID = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szOpenID, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szOpenID[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szOpenID) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iLogicWorldId);
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
			if (num5 > (uint)this.szOpenUrl.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)CSDT_ACNT_DETAIL_INFO.LENGTH_szOpenUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szOpenUrl = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szOpenUrl, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szOpenUrl[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szOpenUrl) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPower);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMaxGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWangZheCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGameVip.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stStatistic.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBanTime.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMostUsedHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (CSDT_ACNT_DETAIL_INFO.VERSION_dwQQVIPMask <= cutVer)
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
			if (CSDT_ACNT_DETAIL_INFO.VERSION_bPrivilege <= cutVer)
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
			if (CSDT_ACNT_DETAIL_INFO.VERSION_bGender <= cutVer)
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
			errorType = this.stHonorInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRankInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCurClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (CSDT_ACNT_DETAIL_INFO.VERSION_stLikeNum <= cutVer)
			{
				errorType = this.stLikeNum.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stLikeNum.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwAchieveMentScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astShowAchievement[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (num7 > (uint)this.szSignatureInfo.GetLength(0))
			{
				if ((long)num7 > (long)((ulong)CSDT_ACNT_DETAIL_INFO.LENGTH_szSignatureInfo))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSignatureInfo = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSignatureInfo, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSignatureInfo[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szSignatureInfo) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwRefuseFriendBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntMasterInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSumDelCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMostDelCreditType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullUserPrivacyBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendCard.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_ACNT_DETAIL_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullUid = 0uL;
			this.iLogicWorldId = 0;
			this.bIsOnline = 0;
			this.dwLastLoginTime = 0u;
			this.dwLevel = 0u;
			this.dwExp = 0u;
			this.dwPower = 0u;
			this.dwPvpLevel = 0u;
			this.dwPvpExp = 0u;
			this.bGradeOfRank = 0;
			this.bMaxGradeOfRank = 0;
			this.dwWangZheCnt = 0u;
			if (this.stGameVip != null)
			{
				this.stGameVip.Release();
				this.stGameVip = null;
			}
			if (this.stStatistic != null)
			{
				this.stStatistic.Release();
				this.stStatistic = null;
			}
			if (this.stGuildInfo != null)
			{
				this.stGuildInfo.Release();
				this.stGuildInfo = null;
			}
			if (this.stBanTime != null)
			{
				this.stBanTime.Release();
				this.stBanTime = null;
			}
			if (this.stMostUsedHero != null)
			{
				this.stMostUsedHero.Release();
				this.stMostUsedHero = null;
			}
			this.dwQQVIPMask = 0u;
			this.bPrivilege = 0;
			this.bGender = 0;
			if (this.stHonorInfo != null)
			{
				this.stHonorInfo.Release();
				this.stHonorInfo = null;
			}
			this.dwCreditValue = 0u;
			if (this.stRankInfo != null)
			{
				this.stRankInfo.Release();
				this.stRankInfo = null;
			}
			this.dwCurClassOfRank = 0u;
			if (this.stLikeNum != null)
			{
				this.stLikeNum.Release();
				this.stLikeNum = null;
			}
			this.dwAchieveMentScore = 0u;
			if (this.astShowAchievement != null)
			{
				for (int i = 0; i < this.astShowAchievement.Length; i++)
				{
					if (this.astShowAchievement[i] != null)
					{
						this.astShowAchievement[i].Release();
						this.astShowAchievement[i] = null;
					}
				}
			}
			this.dwRefuseFriendBits = 0u;
			if (this.stAcntMasterInfo != null)
			{
				this.stAcntMasterInfo.Release();
				this.stAcntMasterInfo = null;
			}
			this.iSumDelCreditValue = 0;
			this.dwMostDelCreditType = 0u;
			this.ullUserPrivacyBits = 0uL;
			if (this.stFriendCard != null)
			{
				this.stFriendCard.Release();
				this.stFriendCard = null;
			}
		}

		public override void OnUse()
		{
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stStatistic = (CSDT_PVPDETAIL_INFO)ProtocolObjectPool.Get(CSDT_PVPDETAIL_INFO.CLASS_ID);
			this.stGuildInfo = (COMDT_ACNT_GUILD_INFO)ProtocolObjectPool.Get(COMDT_ACNT_GUILD_INFO.CLASS_ID);
			this.stBanTime = (COMDT_ACNT_BANTIME)ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID);
			this.stMostUsedHero = (COMDT_MOST_USED_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_MOST_USED_HERO_DETAIL.CLASS_ID);
			this.stHonorInfo = (COMDT_ACNT_HONORINFO)ProtocolObjectPool.Get(COMDT_ACNT_HONORINFO.CLASS_ID);
			this.stRankInfo = (COMDT_RANKDETAIL)ProtocolObjectPool.Get(COMDT_RANKDETAIL.CLASS_ID);
			this.stLikeNum = (COMDT_LIKE_NUMS)ProtocolObjectPool.Get(COMDT_LIKE_NUMS.CLASS_ID);
			if (this.astShowAchievement != null)
			{
				for (int i = 0; i < this.astShowAchievement.Length; i++)
				{
					this.astShowAchievement[i] = (CSDT_SHOWACHIEVE_DETAIL)ProtocolObjectPool.Get(CSDT_SHOWACHIEVE_DETAIL.CLASS_ID);
				}
			}
			this.stAcntMasterInfo = (COMDT_ACNT_MASTER_INFO)ProtocolObjectPool.Get(COMDT_ACNT_MASTER_INFO.CLASS_ID);
			this.stFriendCard = (COMDT_FRIEND_CARD)ProtocolObjectPool.Get(COMDT_FRIEND_CARD.CLASS_ID);
		}
	}
}
