using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_CLTUPLOADDATA_RSP : ProtocolObject
	{
		public byte bMemberNum;

		public CSDT_MULTIGAMEDATA[] astMemberData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 891;

		public CSPKG_CLTUPLOADDATA_RSP()
		{
			this.astMemberData = new CSDT_MULTIGAMEDATA[10];
			for (int i = 0; i < 10; i++)
			{
				this.astMemberData[i] = (CSDT_MULTIGAMEDATA)ProtocolObjectPool.Get(CSDT_MULTIGAMEDATA.CLASS_ID);
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
			if (cutVer == 0u || CSPKG_CLTUPLOADDATA_RSP.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CLTUPLOADDATA_RSP.CURRVERSION;
			}
			if (CSPKG_CLTUPLOADDATA_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMemberNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMemberNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMemberData.Length < (int)this.bMemberNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMemberNum; i++)
			{
				errorType = this.astMemberData[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPKG_CLTUPLOADDATA_RSP.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CLTUPLOADDATA_RSP.CURRVERSION;
			}
			if (CSPKG_CLTUPLOADDATA_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMemberNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMemberNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMemberNum; i++)
			{
				errorType = this.astMemberData[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_CLTUPLOADDATA_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMemberNum = 0;
			if (this.astMemberData != null)
			{
				for (int i = 0; i < this.astMemberData.Length; i++)
				{
					if (this.astMemberData[i] != null)
					{
						this.astMemberData[i].Release();
						this.astMemberData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMemberData != null)
			{
				for (int i = 0; i < this.astMemberData.Length; i++)
				{
					this.astMemberData[i] = (CSDT_MULTIGAMEDATA)ProtocolObjectPool.Get(CSDT_MULTIGAMEDATA.CLASS_ID);
				}
			}
		}
	}
}
