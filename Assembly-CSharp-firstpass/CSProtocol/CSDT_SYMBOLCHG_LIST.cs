using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_SYMBOLCHG_LIST : ProtocolObject
	{
		public byte bChgCnt;

		public CSDT_SYMBOLCHG_OBJ[] astChgInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 974;

		public CSDT_SYMBOLCHG_LIST()
		{
			this.astChgInfo = new CSDT_SYMBOLCHG_OBJ[30];
			for (int i = 0; i < 30; i++)
			{
				this.astChgInfo[i] = (CSDT_SYMBOLCHG_OBJ)ProtocolObjectPool.Get(CSDT_SYMBOLCHG_OBJ.CLASS_ID);
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
			if (cutVer == 0u || CSDT_SYMBOLCHG_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SYMBOLCHG_LIST.CURRVERSION;
			}
			if (CSDT_SYMBOLCHG_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bChgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.bChgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astChgInfo.Length < (int)this.bChgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bChgCnt; i++)
			{
				errorType = this.astChgInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_SYMBOLCHG_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SYMBOLCHG_LIST.CURRVERSION;
			}
			if (CSDT_SYMBOLCHG_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bChgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.bChgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bChgCnt; i++)
			{
				errorType = this.astChgInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_SYMBOLCHG_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bChgCnt = 0;
			if (this.astChgInfo != null)
			{
				for (int i = 0; i < this.astChgInfo.Length; i++)
				{
					if (this.astChgInfo[i] != null)
					{
						this.astChgInfo[i].Release();
						this.astChgInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astChgInfo != null)
			{
				for (int i = 0; i < this.astChgInfo.Length; i++)
				{
					this.astChgInfo[i] = (CSDT_SYMBOLCHG_OBJ)ProtocolObjectPool.Get(CSDT_SYMBOLCHG_OBJ.CLASS_ID);
				}
			}
		}
	}
}
