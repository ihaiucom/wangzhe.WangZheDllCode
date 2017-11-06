using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_REWARDMATCH_INFO_RSP : ProtocolObject
	{
		public COMDT_REWARDMATCH_DATA stRewardMatchInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1538;

		public SCPKG_GET_REWARDMATCH_INFO_RSP()
		{
			this.stRewardMatchInfo = (COMDT_REWARDMATCH_DATA)ProtocolObjectPool.Get(COMDT_REWARDMATCH_DATA.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GET_REWARDMATCH_INFO_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_REWARDMATCH_INFO_RSP.CURRVERSION;
			}
			if (SCPKG_GET_REWARDMATCH_INFO_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stRewardMatchInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GET_REWARDMATCH_INFO_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_REWARDMATCH_INFO_RSP.CURRVERSION;
			}
			if (SCPKG_GET_REWARDMATCH_INFO_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stRewardMatchInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_REWARDMATCH_INFO_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stRewardMatchInfo != null)
			{
				this.stRewardMatchInfo.Release();
				this.stRewardMatchInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stRewardMatchInfo = (COMDT_REWARDMATCH_DATA)ProtocolObjectPool.Get(COMDT_REWARDMATCH_DATA.CLASS_ID);
		}
	}
}
