using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_APPROVE_JOIN_GUILD_APPLY : ProtocolObject
	{
		public byte bAgree;

		public COMDT_GUILD_MEMBER_BRIEF_INFO stApplyInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1326;

		public CSPKG_APPROVE_JOIN_GUILD_APPLY()
		{
			this.stApplyInfo = (COMDT_GUILD_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
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
			if (cutVer == 0u || CSPKG_APPROVE_JOIN_GUILD_APPLY.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_APPROVE_JOIN_GUILD_APPLY.CURRVERSION;
			}
			if (CSPKG_APPROVE_JOIN_GUILD_APPLY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bAgree);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stApplyInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPKG_APPROVE_JOIN_GUILD_APPLY.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_APPROVE_JOIN_GUILD_APPLY.CURRVERSION;
			}
			if (CSPKG_APPROVE_JOIN_GUILD_APPLY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bAgree);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stApplyInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_APPROVE_JOIN_GUILD_APPLY.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bAgree = 0;
			if (this.stApplyInfo != null)
			{
				this.stApplyInfo.Release();
				this.stApplyInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stApplyInfo = (COMDT_GUILD_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
		}
	}
}
