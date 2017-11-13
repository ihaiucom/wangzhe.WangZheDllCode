using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_CHAT_NTF : ProtocolObject
	{
		public byte bMsgCnt;

		public COMDT_CHAT_MSG[] astChatMsg;

		public uint dwTimeStamp;

		public byte bNeedSetChatFreeCnt;

		public uint dwRestChatFreeCnt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 62u;

		public static readonly uint VERSION_bNeedSetChatFreeCnt = 62u;

		public static readonly uint VERSION_dwRestChatFreeCnt = 62u;

		public static readonly int CLASS_ID = 1084;

		public SCPKG_CMD_CHAT_NTF()
		{
			this.astChatMsg = new COMDT_CHAT_MSG[100];
			for (int i = 0; i < 100; i++)
			{
				this.astChatMsg[i] = (COMDT_CHAT_MSG)ProtocolObjectPool.Get(COMDT_CHAT_MSG.CLASS_ID);
			}
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
			if (cutVer == 0u || SCPKG_CMD_CHAT_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_CHAT_NTF.CURRVERSION;
			}
			if (SCPKG_CMD_CHAT_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMsgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.bMsgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astChatMsg.Length < (int)this.bMsgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMsgCnt; i++)
			{
				errorType = this.astChatMsg[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwTimeStamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_CMD_CHAT_NTF.VERSION_bNeedSetChatFreeCnt <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bNeedSetChatFreeCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_CHAT_NTF.VERSION_dwRestChatFreeCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwRestChatFreeCnt);
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
			if (cutVer == 0u || SCPKG_CMD_CHAT_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_CHAT_NTF.CURRVERSION;
			}
			if (SCPKG_CMD_CHAT_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMsgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.bMsgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMsgCnt; i++)
			{
				errorType = this.astChatMsg[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeStamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_CMD_CHAT_NTF.VERSION_bNeedSetChatFreeCnt <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bNeedSetChatFreeCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bNeedSetChatFreeCnt = 0;
			}
			if (SCPKG_CMD_CHAT_NTF.VERSION_dwRestChatFreeCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwRestChatFreeCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwRestChatFreeCnt = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_CHAT_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMsgCnt = 0;
			if (this.astChatMsg != null)
			{
				for (int i = 0; i < this.astChatMsg.Length; i++)
				{
					if (this.astChatMsg[i] != null)
					{
						this.astChatMsg[i].Release();
						this.astChatMsg[i] = null;
					}
				}
			}
			this.dwTimeStamp = 0u;
			this.bNeedSetChatFreeCnt = 0;
			this.dwRestChatFreeCnt = 0u;
		}

		public override void OnUse()
		{
			if (this.astChatMsg != null)
			{
				for (int i = 0; i < this.astChatMsg.Length; i++)
				{
					this.astChatMsg[i] = (COMDT_CHAT_MSG)ProtocolObjectPool.Get(COMDT_CHAT_MSG.CLASS_ID);
				}
			}
		}
	}
}
