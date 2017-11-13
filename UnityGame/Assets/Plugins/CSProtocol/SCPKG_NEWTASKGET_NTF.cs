using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_NEWTASKGET_NTF : ProtocolObject
	{
		public uint dwTaskCnt;

		public SCDT_NEWTASKGET[] astNewTask;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1109;

		public SCPKG_NEWTASKGET_NTF()
		{
			this.astNewTask = new SCDT_NEWTASKGET[85];
			for (int i = 0; i < 85; i++)
			{
				this.astNewTask[i] = (SCDT_NEWTASKGET)ProtocolObjectPool.Get(SCDT_NEWTASKGET.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_NEWTASKGET_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NEWTASKGET_NTF.CURRVERSION;
			}
			if (SCPKG_NEWTASKGET_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwTaskCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astNewTask.Length < (long)((ulong)this.dwTaskCnt))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwTaskCnt))
			{
				errorType = this.astNewTask[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_NEWTASKGET_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NEWTASKGET_NTF.CURRVERSION;
			}
			if (SCPKG_NEWTASKGET_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTaskCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwTaskCnt))
			{
				errorType = this.astNewTask[num].unpack(ref srcBuf, cutVer);
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
			return SCPKG_NEWTASKGET_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwTaskCnt = 0u;
			if (this.astNewTask != null)
			{
				for (int i = 0; i < this.astNewTask.Length; i++)
				{
					if (this.astNewTask[i] != null)
					{
						this.astNewTask[i].Release();
						this.astNewTask[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astNewTask != null)
			{
				for (int i = 0; i < this.astNewTask.Length; i++)
				{
					this.astNewTask[i] = (SCDT_NEWTASKGET)ProtocolObjectPool.Get(SCDT_NEWTASKGET.CLASS_ID);
				}
			}
		}
	}
}
