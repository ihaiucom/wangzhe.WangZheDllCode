using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_CMD_CHEATCMD : ProtocolObject
	{
		public int iCheatCmd;

		public CSDT_CHEATCMD_DETAIL stCheatCmdDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly int CLASS_ID = 860;

		public CSPKG_CMD_CHEATCMD()
		{
			this.stCheatCmdDetail = (CSDT_CHEATCMD_DETAIL)ProtocolObjectPool.Get(CSDT_CHEATCMD_DETAIL.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.iCheatCmd = 0;
			long selector = (long)this.iCheatCmd;
			TdrError.ErrorType errorType = this.stCheatCmdDetail.construct(selector);
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
			if (cutVer == 0u || CSPKG_CMD_CHEATCMD.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_CHEATCMD.CURRVERSION;
			}
			if (CSPKG_CMD_CHEATCMD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iCheatCmd);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iCheatCmd;
			errorType = this.stCheatCmdDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPKG_CMD_CHEATCMD.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_CHEATCMD.CURRVERSION;
			}
			if (CSPKG_CMD_CHEATCMD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCheatCmd);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iCheatCmd;
			errorType = this.stCheatCmdDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_CMD_CHEATCMD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iCheatCmd = 0;
			if (this.stCheatCmdDetail != null)
			{
				this.stCheatCmdDetail.Release();
				this.stCheatCmdDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stCheatCmdDetail = (CSDT_CHEATCMD_DETAIL)ProtocolObjectPool.Get(CSDT_CHEATCMD_DETAIL.CLASS_ID);
		}
	}
}
