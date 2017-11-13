using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_USUTASKLIST : ProtocolObject
	{
		public byte bNewUsualTaskCnt;

		public DT_USUTASKINFO[] astNewUsualTask;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1095;

		public SCDT_USUTASKLIST()
		{
			this.astNewUsualTask = new DT_USUTASKINFO[85];
			for (int i = 0; i < 85; i++)
			{
				this.astNewUsualTask[i] = (DT_USUTASKINFO)ProtocolObjectPool.Get(DT_USUTASKINFO.CLASS_ID);
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
			if (cutVer == 0u || SCDT_USUTASKLIST.CURRVERSION < cutVer)
			{
				cutVer = SCDT_USUTASKLIST.CURRVERSION;
			}
			if (SCDT_USUTASKLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bNewUsualTaskCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85 < this.bNewUsualTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astNewUsualTask.Length < (int)this.bNewUsualTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bNewUsualTaskCnt; i++)
			{
				errorType = this.astNewUsualTask[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCDT_USUTASKLIST.CURRVERSION < cutVer)
			{
				cutVer = SCDT_USUTASKLIST.CURRVERSION;
			}
			if (SCDT_USUTASKLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bNewUsualTaskCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85 < this.bNewUsualTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bNewUsualTaskCnt; i++)
			{
				errorType = this.astNewUsualTask[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCDT_USUTASKLIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bNewUsualTaskCnt = 0;
			if (this.astNewUsualTask != null)
			{
				for (int i = 0; i < this.astNewUsualTask.Length; i++)
				{
					if (this.astNewUsualTask[i] != null)
					{
						this.astNewUsualTask[i].Release();
						this.astNewUsualTask[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astNewUsualTask != null)
			{
				for (int i = 0; i < this.astNewUsualTask.Length; i++)
				{
					this.astNewUsualTask[i] = (DT_USUTASKINFO)ProtocolObjectPool.Get(DT_USUTASKINFO.CLASS_ID);
				}
			}
		}
	}
}
