using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class ShareStatusList : tsf4g_csharp_interface, IPackable, IUnpackable
	{
		public int iCount;

		public ShareStatus[] astShareStatus;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public ShareStatusList()
		{
			this.astShareStatus = new ShareStatus[50];
			for (int i = 0; i < 50; i++)
			{
				this.astShareStatus[i] = new ShareStatus();
			}
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = new TdrWriteBuf(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			return errorType;
		}

		public TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || ShareStatusList.CURRVERSION < cutVer)
			{
				cutVer = ShareStatusList.CURRVERSION;
			}
			if (ShareStatusList.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iCount)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (50 < this.iCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astShareStatus.Length < this.iCount)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < this.iCount; i++)
			{
				errorType = this.astShareStatus[i].pack(ref destBuf, cutVer);
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
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ShareStatusList.CURRVERSION < cutVer)
			{
				cutVer = ShareStatusList.CURRVERSION;
			}
			if (ShareStatusList.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iCount)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (50 < this.iCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.astShareStatus = new ShareStatus[this.iCount];
			for (int i = 0; i < this.iCount; i++)
			{
				this.astShareStatus[i] = new ShareStatus();
			}
			for (int j = 0; j < this.iCount; j++)
			{
				errorType = this.astShareStatus[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}
	}
}
