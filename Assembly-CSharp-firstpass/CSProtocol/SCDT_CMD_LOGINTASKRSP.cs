using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_CMD_LOGINTASKRSP : ProtocolObject
	{
		public uint dwCurtaskNum;

		public COMDT_ACNT_CURTASK[] astCurtask;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 37u;

		public static readonly int CLASS_ID = 721;

		public SCDT_CMD_LOGINTASKRSP()
		{
			this.astCurtask = new COMDT_ACNT_CURTASK[85];
			for (int i = 0; i < 85; i++)
			{
				this.astCurtask[i] = (COMDT_ACNT_CURTASK)ProtocolObjectPool.Get(COMDT_ACNT_CURTASK.CLASS_ID);
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
			if (cutVer == 0u || SCDT_CMD_LOGINTASKRSP.CURRVERSION < cutVer)
			{
				cutVer = SCDT_CMD_LOGINTASKRSP.CURRVERSION;
			}
			if (SCDT_CMD_LOGINTASKRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwCurtaskNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwCurtaskNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astCurtask.Length < (long)((ulong)this.dwCurtaskNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCurtaskNum))
			{
				errorType = this.astCurtask[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || SCDT_CMD_LOGINTASKRSP.CURRVERSION < cutVer)
			{
				cutVer = SCDT_CMD_LOGINTASKRSP.CURRVERSION;
			}
			if (SCDT_CMD_LOGINTASKRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwCurtaskNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwCurtaskNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCurtaskNum))
			{
				errorType = this.astCurtask[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCDT_CMD_LOGINTASKRSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwCurtaskNum = 0u;
			if (this.astCurtask != null)
			{
				for (int i = 0; i < this.astCurtask.Length; i++)
				{
					if (this.astCurtask[i] != null)
					{
						this.astCurtask[i].Release();
						this.astCurtask[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astCurtask != null)
			{
				for (int i = 0; i < this.astCurtask.Length; i++)
				{
					this.astCurtask[i] = (COMDT_ACNT_CURTASK)ProtocolObjectPool.Get(COMDT_ACNT_CURTASK.CLASS_ID);
				}
			}
		}
	}
}
