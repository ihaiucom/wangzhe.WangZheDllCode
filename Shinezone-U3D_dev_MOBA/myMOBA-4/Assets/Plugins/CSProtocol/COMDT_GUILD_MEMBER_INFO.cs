using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GUILD_MEMBER_INFO : ProtocolObject
	{
		public COMDT_GUILD_MEMBER_BRIEF_INFO stBriefInfo;

		public byte bPosition;

		public uint dwConstruct;

		public byte bFireCnt;

		public uint dwJoinTime;

		public COMDT_GUILD_MEMBER_RANK_INFO stRankInfo;

		public uint dwLastLoginTime;

		public COMDT_MEMBER_GUILD_MATCH_INFO stGuildMatchInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly uint VERSION_dwLastLoginTime = 82u;

		public static readonly uint VERSION_stGuildMatchInfo = 145u;

		public static readonly int CLASS_ID = 371;

		public COMDT_GUILD_MEMBER_INFO()
		{
			this.stBriefInfo = (COMDT_GUILD_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
			this.stRankInfo = (COMDT_GUILD_MEMBER_RANK_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_RANK_INFO.CLASS_ID);
			this.stGuildMatchInfo = (COMDT_MEMBER_GUILD_MATCH_INFO)ProtocolObjectPool.Get(COMDT_MEMBER_GUILD_MATCH_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GUILD_MEMBER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_MEMBER_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_MEMBER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBriefInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPosition);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwConstruct);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bFireCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwJoinTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRankInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_GUILD_MEMBER_INFO.VERSION_dwLastLoginTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastLoginTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_MEMBER_INFO.VERSION_stGuildMatchInfo <= cutVer)
			{
				errorType = this.stGuildMatchInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_GUILD_MEMBER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_MEMBER_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_MEMBER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBriefInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPosition);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwConstruct);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFireCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwJoinTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRankInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_GUILD_MEMBER_INFO.VERSION_dwLastLoginTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastLoginTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastLoginTime = 0u;
			}
			if (COMDT_GUILD_MEMBER_INFO.VERSION_stGuildMatchInfo <= cutVer)
			{
				errorType = this.stGuildMatchInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stGuildMatchInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GUILD_MEMBER_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stBriefInfo != null)
			{
				this.stBriefInfo.Release();
				this.stBriefInfo = null;
			}
			this.bPosition = 0;
			this.dwConstruct = 0u;
			this.bFireCnt = 0;
			this.dwJoinTime = 0u;
			if (this.stRankInfo != null)
			{
				this.stRankInfo.Release();
				this.stRankInfo = null;
			}
			this.dwLastLoginTime = 0u;
			if (this.stGuildMatchInfo != null)
			{
				this.stGuildMatchInfo.Release();
				this.stGuildMatchInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stBriefInfo = (COMDT_GUILD_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
			this.stRankInfo = (COMDT_GUILD_MEMBER_RANK_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_RANK_INFO.CLASS_ID);
			this.stGuildMatchInfo = (COMDT_MEMBER_GUILD_MATCH_INFO)ProtocolObjectPool.Get(COMDT_MEMBER_GUILD_MATCH_INFO.CLASS_ID);
		}
	}
}
