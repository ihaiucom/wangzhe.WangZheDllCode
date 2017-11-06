using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_FIGHTER_INFO : ProtocolObject
	{
		public byte bFigterNum;

		public COMDT_ARENA_FIGHTER_DETAIL[] astFigterDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 510;

		public COMDT_ARENA_FIGHTER_INFO()
		{
			this.astFigterDetail = new COMDT_ARENA_FIGHTER_DETAIL[100];
			for (int i = 0; i < 100; i++)
			{
				this.astFigterDetail[i] = (COMDT_ARENA_FIGHTER_DETAIL)ProtocolObjectPool.Get(COMDT_ARENA_FIGHTER_DETAIL.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.bFigterNum = 0;
			for (int i = 0; i < 100; i++)
			{
				errorType = this.astFigterDetail[i].construct();
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
			if (cutVer == 0u || COMDT_ARENA_FIGHTER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHTER_INFO.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHTER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bFigterNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.bFigterNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astFigterDetail.Length < (int)this.bFigterNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bFigterNum; i++)
			{
				errorType = this.astFigterDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ARENA_FIGHTER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHTER_INFO.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHTER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bFigterNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.bFigterNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bFigterNum; i++)
			{
				errorType = this.astFigterDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ARENA_FIGHTER_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bFigterNum = 0;
			if (this.astFigterDetail != null)
			{
				for (int i = 0; i < this.astFigterDetail.Length; i++)
				{
					if (this.astFigterDetail[i] != null)
					{
						this.astFigterDetail[i].Release();
						this.astFigterDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astFigterDetail != null)
			{
				for (int i = 0; i < this.astFigterDetail.Length; i++)
				{
					this.astFigterDetail[i] = (COMDT_ARENA_FIGHTER_DETAIL)ProtocolObjectPool.Get(COMDT_ARENA_FIGHTER_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
