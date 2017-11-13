using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_BLOCKDATA : ProtocolObject
	{
		public byte bMemberType;

		public COMDT_ARENA_MEMBER stMember;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 417;

		public COMDT_ARENA_BLOCKDATA()
		{
			this.stMember = (COMDT_ARENA_MEMBER)ProtocolObjectPool.Get(COMDT_ARENA_MEMBER.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.bMemberType = 0;
			long selector = (long)this.bMemberType;
			TdrError.ErrorType errorType = this.stMember.construct(selector);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || COMDT_ARENA_BLOCKDATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_BLOCKDATA.CURRVERSION;
			}
			if (COMDT_ARENA_BLOCKDATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMemberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bMemberType;
			errorType = this.stMember.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ARENA_BLOCKDATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_BLOCKDATA.CURRVERSION;
			}
			if (COMDT_ARENA_BLOCKDATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMemberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bMemberType;
			errorType = this.stMember.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ARENA_BLOCKDATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMemberType = 0;
			if (this.stMember != null)
			{
				this.stMember.Release();
				this.stMember = null;
			}
		}

		public override void OnUse()
		{
			this.stMember = (COMDT_ARENA_MEMBER)ProtocolObjectPool.Get(COMDT_ARENA_MEMBER.CLASS_ID);
		}
	}
}
