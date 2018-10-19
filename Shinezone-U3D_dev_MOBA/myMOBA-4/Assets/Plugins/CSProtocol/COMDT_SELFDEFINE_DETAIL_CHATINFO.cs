using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SELFDEFINE_DETAIL_CHATINFO : ProtocolObject
	{
		public byte bChatType;

		public COMDT_INBATTLE_CHAT_UNION stChatInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 297;

		public COMDT_SELFDEFINE_DETAIL_CHATINFO()
		{
			this.stChatInfo = (COMDT_INBATTLE_CHAT_UNION)ProtocolObjectPool.Get(COMDT_INBATTLE_CHAT_UNION.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SELFDEFINE_DETAIL_CHATINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SELFDEFINE_DETAIL_CHATINFO.CURRVERSION;
			}
			if (COMDT_SELFDEFINE_DETAIL_CHATINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bChatType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bChatType;
			errorType = this.stChatInfo.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_SELFDEFINE_DETAIL_CHATINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SELFDEFINE_DETAIL_CHATINFO.CURRVERSION;
			}
			if (COMDT_SELFDEFINE_DETAIL_CHATINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bChatType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bChatType;
			errorType = this.stChatInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SELFDEFINE_DETAIL_CHATINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bChatType = 0;
			if (this.stChatInfo != null)
			{
				this.stChatInfo.Release();
				this.stChatInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stChatInfo = (COMDT_INBATTLE_CHAT_UNION)ProtocolObjectPool.Get(COMDT_INBATTLE_CHAT_UNION.CLASS_ID);
		}
	}
}
