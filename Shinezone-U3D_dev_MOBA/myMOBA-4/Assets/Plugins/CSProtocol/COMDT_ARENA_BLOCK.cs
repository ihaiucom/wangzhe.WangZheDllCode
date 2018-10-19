using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_BLOCK : ProtocolObject
	{
		public uint dwUsedNum;

		public COMDT_ARENA_BLOCKDATA[] astData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 508;

		public COMDT_ARENA_BLOCK()
		{
			this.astData = new COMDT_ARENA_BLOCKDATA[200];
			for (int i = 0; i < 200; i++)
			{
				this.astData[i] = (COMDT_ARENA_BLOCKDATA)ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.dwUsedNum = 0u;
			for (int i = 0; i < 200; i++)
			{
				errorType = this.astData[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || COMDT_ARENA_BLOCK.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_BLOCK.CURRVERSION;
			}
			if (COMDT_ARENA_BLOCK.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwUsedNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200u < this.dwUsedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astData.Length < (long)((ulong)this.dwUsedNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwUsedNum))
			{
				errorType = this.astData[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ARENA_BLOCK.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_BLOCK.CURRVERSION;
			}
			if (COMDT_ARENA_BLOCK.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwUsedNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200u < this.dwUsedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwUsedNum))
			{
				errorType = this.astData[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_ARENA_BLOCK.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwUsedNum = 0u;
			if (this.astData != null)
			{
				for (int i = 0; i < this.astData.Length; i++)
				{
					if (this.astData[i] != null)
					{
						this.astData[i].Release();
						this.astData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astData != null)
			{
				for (int i = 0; i < this.astData.Length; i++)
				{
					this.astData[i] = (COMDT_ARENA_BLOCKDATA)ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
				}
			}
		}
	}
}
