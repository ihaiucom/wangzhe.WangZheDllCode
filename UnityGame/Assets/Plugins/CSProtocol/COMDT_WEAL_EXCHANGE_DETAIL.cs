using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WEAL_EXCHANGE_DETAIL : ProtocolObject
	{
		public uint dwLastRefreshTime;

		public byte bWealCnt;

		public COMDT_WEAL_EXCHANGE_OBJ[] astWealList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 538;

		public COMDT_WEAL_EXCHANGE_DETAIL()
		{
			this.astWealList = new COMDT_WEAL_EXCHANGE_OBJ[15];
			for (int i = 0; i < 15; i++)
			{
				this.astWealList[i] = (COMDT_WEAL_EXCHANGE_OBJ)ProtocolObjectPool.Get(COMDT_WEAL_EXCHANGE_OBJ.CLASS_ID);
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
			if (cutVer == 0u || COMDT_WEAL_EXCHANGE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_EXCHANGE_DETAIL.CURRVERSION;
			}
			if (COMDT_WEAL_EXCHANGE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bWealCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15 < this.bWealCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astWealList.Length < (int)this.bWealCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bWealCnt; i++)
			{
				errorType = this.astWealList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_WEAL_EXCHANGE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_EXCHANGE_DETAIL.CURRVERSION;
			}
			if (COMDT_WEAL_EXCHANGE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bWealCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15 < this.bWealCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bWealCnt; i++)
			{
				errorType = this.astWealList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_WEAL_EXCHANGE_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastRefreshTime = 0u;
			this.bWealCnt = 0;
			if (this.astWealList != null)
			{
				for (int i = 0; i < this.astWealList.Length; i++)
				{
					if (this.astWealList[i] != null)
					{
						this.astWealList[i].Release();
						this.astWealList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astWealList != null)
			{
				for (int i = 0; i < this.astWealList.Length; i++)
				{
					this.astWealList[i] = (COMDT_WEAL_EXCHANGE_OBJ)ProtocolObjectPool.Get(COMDT_WEAL_EXCHANGE_OBJ.CLASS_ID);
				}
			}
		}
	}
}
