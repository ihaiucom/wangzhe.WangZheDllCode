using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_MULTGAME_GAMEOVER : ProtocolObject
	{
		public byte bAcntNum;

		public CSDT_MULTIGAMEOVER_INFO[] astAcntInfo;

		public COMDT_CLIENT_RECORD_DATA stClientRecordData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 785;

		public CSPKG_MULTGAME_GAMEOVER()
		{
			this.astAcntInfo = new CSDT_MULTIGAMEOVER_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astAcntInfo[i] = (CSDT_MULTIGAMEOVER_INFO)ProtocolObjectPool.Get(CSDT_MULTIGAMEOVER_INFO.CLASS_ID);
			}
			this.stClientRecordData = (COMDT_CLIENT_RECORD_DATA)ProtocolObjectPool.Get(COMDT_CLIENT_RECORD_DATA.CLASS_ID);
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
			if (cutVer == 0u || CSPKG_MULTGAME_GAMEOVER.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_MULTGAME_GAMEOVER.CURRVERSION;
			}
			if (CSPKG_MULTGAME_GAMEOVER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bAcntNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astAcntInfo.Length < (int)this.bAcntNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bAcntNum; i++)
			{
				errorType = this.astAcntInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stClientRecordData.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPKG_MULTGAME_GAMEOVER.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_MULTGAME_GAMEOVER.CURRVERSION;
			}
			if (CSPKG_MULTGAME_GAMEOVER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bAcntNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bAcntNum; i++)
			{
				errorType = this.astAcntInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stClientRecordData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_MULTGAME_GAMEOVER.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bAcntNum = 0;
			if (this.astAcntInfo != null)
			{
				for (int i = 0; i < this.astAcntInfo.Length; i++)
				{
					if (this.astAcntInfo[i] != null)
					{
						this.astAcntInfo[i].Release();
						this.astAcntInfo[i] = null;
					}
				}
			}
			if (this.stClientRecordData != null)
			{
				this.stClientRecordData.Release();
				this.stClientRecordData = null;
			}
		}

		public override void OnUse()
		{
			if (this.astAcntInfo != null)
			{
				for (int i = 0; i < this.astAcntInfo.Length; i++)
				{
					this.astAcntInfo[i] = (CSDT_MULTIGAMEOVER_INFO)ProtocolObjectPool.Get(CSDT_MULTIGAMEOVER_INFO.CLASS_ID);
				}
			}
			this.stClientRecordData = (COMDT_CLIENT_RECORD_DATA)ProtocolObjectPool.Get(COMDT_CLIENT_RECORD_DATA.CLASS_ID);
		}
	}
}
