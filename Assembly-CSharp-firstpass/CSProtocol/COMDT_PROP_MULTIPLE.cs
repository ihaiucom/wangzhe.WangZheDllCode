using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PROP_MULTIPLE : ProtocolObject
	{
		public ushort wCnt;

		public COMDT_PROP_MULTIPLE_INFO[] astMultipleInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 554;

		public COMDT_PROP_MULTIPLE()
		{
			this.astMultipleInfo = new COMDT_PROP_MULTIPLE_INFO[4];
			for (int i = 0; i < 4; i++)
			{
				this.astMultipleInfo[i] = (COMDT_PROP_MULTIPLE_INFO)ProtocolObjectPool.Get(COMDT_PROP_MULTIPLE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PROP_MULTIPLE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PROP_MULTIPLE.CURRVERSION;
			}
			if (COMDT_PROP_MULTIPLE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.wCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMultipleInfo.Length < (int)this.wCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wCnt; i++)
			{
				errorType = this.astMultipleInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_PROP_MULTIPLE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PROP_MULTIPLE.CURRVERSION;
			}
			if (COMDT_PROP_MULTIPLE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.wCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wCnt; i++)
			{
				errorType = this.astMultipleInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PROP_MULTIPLE.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wCnt = 0;
			if (this.astMultipleInfo != null)
			{
				for (int i = 0; i < this.astMultipleInfo.Length; i++)
				{
					if (this.astMultipleInfo[i] != null)
					{
						this.astMultipleInfo[i].Release();
						this.astMultipleInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMultipleInfo != null)
			{
				for (int i = 0; i < this.astMultipleInfo.Length; i++)
				{
					this.astMultipleInfo[i] = (COMDT_PROP_MULTIPLE_INFO)ProtocolObjectPool.Get(COMDT_PROP_MULTIPLE_INFO.CLASS_ID);
				}
			}
		}
	}
}
