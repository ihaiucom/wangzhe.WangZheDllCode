using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_STARTSINGLEGAMERSP : ProtocolObject
	{
		public int iErrCode;

		public uint dwRewardNum;

		public byte bGameType;

		public CSDT_SINGLEGAMETYPE_RSP stGameParam;

		public int iLevelId;

		public CSDT_SINGLEGAME_DETAIL stDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 778;

		public SCPKG_STARTSINGLEGAMERSP()
		{
			this.stGameParam = (CSDT_SINGLEGAMETYPE_RSP)ProtocolObjectPool.Get(CSDT_SINGLEGAMETYPE_RSP.CLASS_ID);
			this.stDetail = (CSDT_SINGLEGAME_DETAIL)ProtocolObjectPool.Get(CSDT_SINGLEGAME_DETAIL.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.iErrCode = 0;
			this.dwRewardNum = 0u;
			this.bGameType = 0;
			long selector = (long)this.bGameType;
			TdrError.ErrorType errorType = this.stGameParam.construct(selector);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.iLevelId = 0;
			long selector2 = (long)this.iErrCode;
			errorType = this.stDetail.construct(selector2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || SCPKG_STARTSINGLEGAMERSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_STARTSINGLEGAMERSP.CURRVERSION;
			}
			if (SCPKG_STARTSINGLEGAMERSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRewardNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGameType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bGameType;
			errorType = this.stGameParam.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLevelId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector2 = (long)this.iErrCode;
			errorType = this.stDetail.pack(selector2, ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_STARTSINGLEGAMERSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_STARTSINGLEGAMERSP.CURRVERSION;
			}
			if (SCPKG_STARTSINGLEGAMERSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRewardNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGameType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bGameType;
			errorType = this.stGameParam.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLevelId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector2 = (long)this.iErrCode;
			errorType = this.stDetail.unpack(selector2, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_STARTSINGLEGAMERSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iErrCode = 0;
			this.dwRewardNum = 0u;
			this.bGameType = 0;
			if (this.stGameParam != null)
			{
				this.stGameParam.Release();
				this.stGameParam = null;
			}
			this.iLevelId = 0;
			if (this.stDetail != null)
			{
				this.stDetail.Release();
				this.stDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stGameParam = (CSDT_SINGLEGAMETYPE_RSP)ProtocolObjectPool.Get(CSDT_SINGLEGAMETYPE_RSP.CLASS_ID);
			this.stDetail = (CSDT_SINGLEGAME_DETAIL)ProtocolObjectPool.Get(CSDT_SINGLEGAME_DETAIL.CLASS_ID);
		}
	}
}
