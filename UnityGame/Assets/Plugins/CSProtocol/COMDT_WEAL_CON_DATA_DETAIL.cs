using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WEAL_CON_DATA_DETAIL : ProtocolObject
	{
		public uint dwWealID;

		public uint dwRewardMask;

		public uint dwReachMask;

		public uint dwLimitReachMask;

		public uint dwLastRefreshTime;

		public ushort wConNum;

		public COMDT_WEAL_CON_DATA_INFO[] astConData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 24u;

		public static readonly uint VERSION_dwRewardMask = 24u;

		public static readonly uint VERSION_dwReachMask = 24u;

		public static readonly uint VERSION_dwLimitReachMask = 24u;

		public static readonly uint VERSION_dwLastRefreshTime = 24u;

		public static readonly int CLASS_ID = 504;

		public COMDT_WEAL_CON_DATA_DETAIL()
		{
			this.astConData = new COMDT_WEAL_CON_DATA_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astConData[i] = (COMDT_WEAL_CON_DATA_INFO)ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_WEAL_CON_DATA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_CON_DATA_DETAIL.CURRVERSION;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwWealID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwRewardMask <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwRewardMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwReachMask <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwReachMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwLimitReachMask <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLimitReachMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwLastRefreshTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastRefreshTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt16(this.wConNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.wConNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astConData.Length < (int)this.wConNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wConNum; i++)
			{
				errorType = this.astConData[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_WEAL_CON_DATA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_CON_DATA_DETAIL.CURRVERSION;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwWealID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwRewardMask <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwRewardMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwRewardMask = 0u;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwReachMask <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwReachMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwReachMask = 0u;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwLimitReachMask <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLimitReachMask);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLimitReachMask = 0u;
			}
			if (COMDT_WEAL_CON_DATA_DETAIL.VERSION_dwLastRefreshTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastRefreshTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastRefreshTime = 0u;
			}
			errorType = srcBuf.readUInt16(ref this.wConNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.wConNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wConNum; i++)
			{
				errorType = this.astConData[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_WEAL_CON_DATA_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwWealID = 0u;
			this.dwRewardMask = 0u;
			this.dwReachMask = 0u;
			this.dwLimitReachMask = 0u;
			this.dwLastRefreshTime = 0u;
			this.wConNum = 0;
			if (this.astConData != null)
			{
				for (int i = 0; i < this.astConData.Length; i++)
				{
					if (this.astConData[i] != null)
					{
						this.astConData[i].Release();
						this.astConData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astConData != null)
			{
				for (int i = 0; i < this.astConData.Length; i++)
				{
					this.astConData[i] = (COMDT_WEAL_CON_DATA_INFO)ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_INFO.CLASS_ID);
				}
			}
		}
	}
}
