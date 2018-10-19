using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_FIGHT_HISTORY : ProtocolObject
	{
		public byte bPos;

		public COMDT_ARENA_FIGHT_RECORD[] astRecord;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 548;

		public COMDT_ARENA_FIGHT_HISTORY()
		{
			this.astRecord = new COMDT_ARENA_FIGHT_RECORD[10];
			for (int i = 0; i < 10; i++)
			{
				this.astRecord[i] = (COMDT_ARENA_FIGHT_RECORD)ProtocolObjectPool.Get(COMDT_ARENA_FIGHT_RECORD.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.bPos = 0;
			for (int i = 0; i < 10; i++)
			{
				errorType = this.astRecord[i].construct();
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
			if (cutVer == 0u || COMDT_ARENA_FIGHT_HISTORY.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHT_HISTORY.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHT_HISTORY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bPos);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 10; i++)
			{
				errorType = this.astRecord[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ARENA_FIGHT_HISTORY.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHT_HISTORY.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHT_HISTORY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bPos);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 10; i++)
			{
				errorType = this.astRecord[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ARENA_FIGHT_HISTORY.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bPos = 0;
			if (this.astRecord != null)
			{
				for (int i = 0; i < this.astRecord.Length; i++)
				{
					if (this.astRecord[i] != null)
					{
						this.astRecord[i].Release();
						this.astRecord[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRecord != null)
			{
				for (int i = 0; i < this.astRecord.Length; i++)
				{
					this.astRecord[i] = (COMDT_ARENA_FIGHT_RECORD)ProtocolObjectPool.Get(COMDT_ARENA_FIGHT_RECORD.CLASS_ID);
				}
			}
		}
	}
}
