using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResRollingMsg : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwRollingMsgID;

		public byte[] szRollingMsgText_ByteArray;

		public uint[] TriggerCondition;

		public uint[] RollingMsgParam;

		public string szRollingMsgText;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szRollingMsgText = 256u;

		public ResRollingMsg()
		{
			this.szRollingMsgText_ByteArray = new byte[1];
			this.TriggerCondition = new uint[15];
			this.RollingMsgParam = new uint[2];
			this.szRollingMsgText = string.Empty;
		}

		private void TransferData()
		{
			this.szRollingMsgText = StringHelper.UTF8BytesToString(ref this.szRollingMsgText_ByteArray);
			this.szRollingMsgText_ByteArray = null;
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
			if (cutVer == 0u || ResRollingMsg.CURRVERSION < cutVer)
			{
				cutVer = ResRollingMsg.CURRVERSION;
			}
			if (ResRollingMsg.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwRollingMsgID);
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
			if (num > (uint)this.szRollingMsgText_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResRollingMsg.LENGTH_szRollingMsgText)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRollingMsgText_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRollingMsgText_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRollingMsgText_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szRollingMsgText_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = srcBuf.readUInt32(ref this.TriggerCondition[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 2; j++)
			{
				errorType = srcBuf.readUInt32(ref this.RollingMsgParam[j]);
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
			if (cutVer == 0u || ResRollingMsg.CURRVERSION < cutVer)
			{
				cutVer = ResRollingMsg.CURRVERSION;
			}
			if (ResRollingMsg.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwRollingMsgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 256;
			if (this.szRollingMsgText_ByteArray.GetLength(0) < num)
			{
				this.szRollingMsgText_ByteArray = new byte[ResRollingMsg.LENGTH_szRollingMsgText];
			}
			errorType = srcBuf.readCString(ref this.szRollingMsgText_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = srcBuf.readUInt32(ref this.TriggerCondition[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 2; j++)
			{
				errorType = srcBuf.readUInt32(ref this.RollingMsgParam[j]);
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
