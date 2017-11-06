using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_GUILD_EXT_INFO : ProtocolObject
	{
		public uint dwLastCreateGuildTime;

		public uint dwLastQuitGuildTime;

		public byte bApplyJoinGuildNum;

		public uint dwClearApplyJoinGuildNumTime;

		public byte bSendGuildMailCnt;

		public uint dwClearSendGuildMailCntTime;

		public uint dwGetSeasonRewardTime;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 150u;

		public static readonly uint VERSION_bSendGuildMailCnt = 120u;

		public static readonly uint VERSION_dwClearSendGuildMailCntTime = 120u;

		public static readonly uint VERSION_dwGetSeasonRewardTime = 150u;

		public static readonly int CLASS_ID = 357;

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
			if (cutVer == 0u || COMDT_ACNT_GUILD_EXT_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_GUILD_EXT_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastCreateGuildTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastQuitGuildTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bApplyJoinGuildNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwClearApplyJoinGuildNumTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.VERSION_bSendGuildMailCnt <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bSendGuildMailCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.VERSION_dwClearSendGuildMailCntTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwClearSendGuildMailCntTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.VERSION_dwGetSeasonRewardTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGetSeasonRewardTime);
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
			if (cutVer == 0u || COMDT_ACNT_GUILD_EXT_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_GUILD_EXT_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastCreateGuildTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastQuitGuildTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bApplyJoinGuildNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwClearApplyJoinGuildNumTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.VERSION_bSendGuildMailCnt <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bSendGuildMailCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bSendGuildMailCnt = 0;
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.VERSION_dwClearSendGuildMailCntTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwClearSendGuildMailCntTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwClearSendGuildMailCntTime = 0u;
			}
			if (COMDT_ACNT_GUILD_EXT_INFO.VERSION_dwGetSeasonRewardTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGetSeasonRewardTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGetSeasonRewardTime = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_GUILD_EXT_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastCreateGuildTime = 0u;
			this.dwLastQuitGuildTime = 0u;
			this.bApplyJoinGuildNum = 0;
			this.dwClearApplyJoinGuildNumTime = 0u;
			this.bSendGuildMailCnt = 0;
			this.dwClearSendGuildMailCntTime = 0u;
			this.dwGetSeasonRewardTime = 0u;
		}
	}
}
