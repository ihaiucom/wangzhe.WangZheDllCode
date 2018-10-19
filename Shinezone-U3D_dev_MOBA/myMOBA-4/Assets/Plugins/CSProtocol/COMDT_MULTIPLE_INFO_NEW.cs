using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MULTIPLE_INFO_NEW : ProtocolObject
	{
		public uint dwMultipleNum;

		public COMDT_MULTIPLE_DATA[] astMultipleData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 236;

		public COMDT_MULTIPLE_INFO_NEW()
		{
			this.astMultipleData = new COMDT_MULTIPLE_DATA[15];
			for (int i = 0; i < 15; i++)
			{
				this.astMultipleData[i] = (COMDT_MULTIPLE_DATA)ProtocolObjectPool.Get(COMDT_MULTIPLE_DATA.CLASS_ID);
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
			if (cutVer == 0u || COMDT_MULTIPLE_INFO_NEW.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTIPLE_INFO_NEW.CURRVERSION;
			}
			if (COMDT_MULTIPLE_INFO_NEW.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwMultipleNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15u < this.dwMultipleNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astMultipleData.Length < (long)((ulong)this.dwMultipleNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwMultipleNum))
			{
				errorType = this.astMultipleData[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || COMDT_MULTIPLE_INFO_NEW.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTIPLE_INFO_NEW.CURRVERSION;
			}
			if (COMDT_MULTIPLE_INFO_NEW.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwMultipleNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15u < this.dwMultipleNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwMultipleNum))
			{
				errorType = this.astMultipleData[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MULTIPLE_INFO_NEW.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwMultipleNum = 0u;
			if (this.astMultipleData != null)
			{
				for (int i = 0; i < this.astMultipleData.Length; i++)
				{
					if (this.astMultipleData[i] != null)
					{
						this.astMultipleData[i].Release();
						this.astMultipleData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMultipleData != null)
			{
				for (int i = 0; i < this.astMultipleData.Length; i++)
				{
					this.astMultipleData[i] = (COMDT_MULTIPLE_DATA)ProtocolObjectPool.Get(COMDT_MULTIPLE_DATA.CLASS_ID);
				}
			}
		}
	}
}
