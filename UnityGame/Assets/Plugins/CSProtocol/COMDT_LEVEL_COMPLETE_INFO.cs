using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_LEVEL_COMPLETE_INFO : ProtocolObject
	{
		public int iLevelID;

		public byte bDiffNum;

		public COMDT_LEVEL_DIFFICULT_INFO[] astDiffDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 139;

		public COMDT_LEVEL_COMPLETE_INFO()
		{
			this.astDiffDetail = new COMDT_LEVEL_DIFFICULT_INFO[4];
			for (int i = 0; i < 4; i++)
			{
				this.astDiffDetail[i] = (COMDT_LEVEL_DIFFICULT_INFO)ProtocolObjectPool.Get(COMDT_LEVEL_DIFFICULT_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_LEVEL_COMPLETE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LEVEL_COMPLETE_INFO.CURRVERSION;
			}
			if (COMDT_LEVEL_COMPLETE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iLevelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bDiffNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astDiffDetail.Length < (int)this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bDiffNum; i++)
			{
				errorType = this.astDiffDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_LEVEL_COMPLETE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LEVEL_COMPLETE_INFO.CURRVERSION;
			}
			if (COMDT_LEVEL_COMPLETE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iLevelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bDiffNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bDiffNum; i++)
			{
				errorType = this.astDiffDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_LEVEL_COMPLETE_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iLevelID = 0;
			this.bDiffNum = 0;
			if (this.astDiffDetail != null)
			{
				for (int i = 0; i < this.astDiffDetail.Length; i++)
				{
					if (this.astDiffDetail[i] != null)
					{
						this.astDiffDetail[i].Release();
						this.astDiffDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astDiffDetail != null)
			{
				for (int i = 0; i < this.astDiffDetail.Length; i++)
				{
					this.astDiffDetail[i] = (COMDT_LEVEL_DIFFICULT_INFO)ProtocolObjectPool.Get(COMDT_LEVEL_DIFFICULT_INFO.CLASS_ID);
				}
			}
		}
	}
}
