using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResDT_PrerequisiteInTask : IUnpackable, tsf4g_csharp_interface
	{
		public uint dwPrerequisiteType;

		public byte[] szPrerequisiteDesc_ByteArray;

		public ResDT_IntParamArrayNode[] astPrerequisiteParam;

		public string szPrerequisiteDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szPrerequisiteDesc = 128u;

		public ResDT_PrerequisiteInTask()
		{
			this.szPrerequisiteDesc_ByteArray = new byte[1];
			this.astPrerequisiteParam = new ResDT_IntParamArrayNode[15];
			for (int i = 0; i < 15; i++)
			{
				this.astPrerequisiteParam[i] = new ResDT_IntParamArrayNode();
			}
			this.szPrerequisiteDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szPrerequisiteDesc = StringHelper.UTF8BytesToString(ref this.szPrerequisiteDesc_ByteArray);
			this.szPrerequisiteDesc_ByteArray = null;
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ResDT_PrerequisiteInTask.CURRVERSION < cutVer)
			{
				cutVer = ResDT_PrerequisiteInTask.CURRVERSION;
			}
			if (ResDT_PrerequisiteInTask.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwPrerequisiteType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szPrerequisiteDesc_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResDT_PrerequisiteInTask.LENGTH_szPrerequisiteDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szPrerequisiteDesc_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szPrerequisiteDesc_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szPrerequisiteDesc_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szPrerequisiteDesc_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astPrerequisiteParam[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.TransferData();
			return errorType;
		}

		public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.load(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
		{
			srcBuf.disableEndian();
			if (cutVer == 0u || ResDT_PrerequisiteInTask.CURRVERSION < cutVer)
			{
				cutVer = ResDT_PrerequisiteInTask.CURRVERSION;
			}
			if (ResDT_PrerequisiteInTask.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwPrerequisiteType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szPrerequisiteDesc_ByteArray.GetLength(0) < num)
			{
				this.szPrerequisiteDesc_ByteArray = new byte[ResDT_PrerequisiteInTask.LENGTH_szPrerequisiteDesc];
			}
			errorType = srcBuf.readCString(ref this.szPrerequisiteDesc_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astPrerequisiteParam[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.TransferData();
			return errorType;
		}
	}
}
