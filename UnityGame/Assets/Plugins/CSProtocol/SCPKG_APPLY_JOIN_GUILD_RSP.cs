using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_APPLY_JOIN_GUILD_RSP : ProtocolObject
	{
		public byte bResult;

		public COMDT_GUILD_BRIEF_INFO stGuildBriefInfo;

		public uint dwApplyTime;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1323;

		public SCPKG_APPLY_JOIN_GUILD_RSP()
		{
			this.stGuildBriefInfo = (COMDT_GUILD_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_BRIEF_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_APPLY_JOIN_GUILD_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_APPLY_JOIN_GUILD_RSP.CURRVERSION;
			}
			if (SCPKG_APPLY_JOIN_GUILD_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildBriefInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwApplyTime);
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
			if (cutVer == 0u || SCPKG_APPLY_JOIN_GUILD_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_APPLY_JOIN_GUILD_RSP.CURRVERSION;
			}
			if (SCPKG_APPLY_JOIN_GUILD_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildBriefInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwApplyTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_APPLY_JOIN_GUILD_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bResult = 0;
			if (this.stGuildBriefInfo != null)
			{
				this.stGuildBriefInfo.Release();
				this.stGuildBriefInfo = null;
			}
			this.dwApplyTime = 0u;
		}

		public override void OnUse()
		{
			this.stGuildBriefInfo = (COMDT_GUILD_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_BRIEF_INFO.CLASS_ID);
		}
	}
}
