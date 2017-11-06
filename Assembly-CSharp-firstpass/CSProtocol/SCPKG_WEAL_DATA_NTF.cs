using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_WEAL_DATA_NTF : ProtocolObject
	{
		public COMDT_WEAL_LIST stWealList;

		public COMDT_WEAL_CON_DATA stWealConData;

		public COMDT_WEAL_POINT_DATA stWealPointData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 127u;

		public static readonly int CLASS_ID = 1410;

		public SCPKG_WEAL_DATA_NTF()
		{
			this.stWealList = (COMDT_WEAL_LIST)ProtocolObjectPool.Get(COMDT_WEAL_LIST.CLASS_ID);
			this.stWealConData = (COMDT_WEAL_CON_DATA)ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA.CLASS_ID);
			this.stWealPointData = (COMDT_WEAL_POINT_DATA)ProtocolObjectPool.Get(COMDT_WEAL_POINT_DATA.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_WEAL_DATA_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_WEAL_DATA_NTF.CURRVERSION;
			}
			if (SCPKG_WEAL_DATA_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stWealList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stWealConData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stWealPointData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || SCPKG_WEAL_DATA_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_WEAL_DATA_NTF.CURRVERSION;
			}
			if (SCPKG_WEAL_DATA_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stWealList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stWealConData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stWealPointData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_WEAL_DATA_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stWealList != null)
			{
				this.stWealList.Release();
				this.stWealList = null;
			}
			if (this.stWealConData != null)
			{
				this.stWealConData.Release();
				this.stWealConData = null;
			}
			if (this.stWealPointData != null)
			{
				this.stWealPointData.Release();
				this.stWealPointData = null;
			}
		}

		public override void OnUse()
		{
			this.stWealList = (COMDT_WEAL_LIST)ProtocolObjectPool.Get(COMDT_WEAL_LIST.CLASS_ID);
			this.stWealConData = (COMDT_WEAL_CON_DATA)ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA.CLASS_ID);
			this.stWealPointData = (COMDT_WEAL_POINT_DATA)ProtocolObjectPool.Get(COMDT_WEAL_POINT_DATA.CLASS_ID);
		}
	}
}
