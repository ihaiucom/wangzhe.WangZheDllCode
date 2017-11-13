using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class NewbieGuideMainLineConf : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public byte bOldPlayerGuide;

		public byte bIndependentNet;

		public uint dwPriority;

		public int iSavePoint;

		public ushort wTriggerLevelUpperLimit;

		public ushort wTriggerLevelLowerLimit;

		public NewbieGuideTriggerTimeItem[] astTriggerTime;

		public NewbieGuideTriggerConditionItem[] astTriggerCondition;

		public NewbieGuideSkipConditionItem[] astSkipCondition;

		public byte[] szRemark_ByteArray;

		public NewbieGuideCompleteIdItem[] astSetCompleteId;

		public string szRemark;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szRemark = 60u;

		public NewbieGuideMainLineConf()
		{
			this.astTriggerTime = new NewbieGuideTriggerTimeItem[3];
			for (int i = 0; i < 3; i++)
			{
				this.astTriggerTime[i] = new NewbieGuideTriggerTimeItem();
			}
			this.astTriggerCondition = new NewbieGuideTriggerConditionItem[3];
			for (int j = 0; j < 3; j++)
			{
				this.astTriggerCondition[j] = new NewbieGuideTriggerConditionItem();
			}
			this.astSkipCondition = new NewbieGuideSkipConditionItem[3];
			for (int k = 0; k < 3; k++)
			{
				this.astSkipCondition[k] = new NewbieGuideSkipConditionItem();
			}
			this.szRemark_ByteArray = new byte[1];
			this.astSetCompleteId = new NewbieGuideCompleteIdItem[3];
			for (int l = 0; l < 3; l++)
			{
				this.astSetCompleteId[l] = new NewbieGuideCompleteIdItem();
			}
			this.szRemark = string.Empty;
		}

		private void TransferData()
		{
			this.szRemark = StringHelper.UTF8BytesToString(ref this.szRemark_ByteArray);
			this.szRemark_ByteArray = null;
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
			if (cutVer == 0u || NewbieGuideMainLineConf.CURRVERSION < cutVer)
			{
				cutVer = NewbieGuideMainLineConf.CURRVERSION;
			}
			if (NewbieGuideMainLineConf.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOldPlayerGuide);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIndependentNet);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPriority);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSavePoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wTriggerLevelUpperLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wTriggerLevelLowerLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astTriggerTime[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				errorType = this.astTriggerCondition[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astSkipCondition[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (num > (uint)this.szRemark_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)NewbieGuideMainLineConf.LENGTH_szRemark)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRemark_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRemark_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRemark_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szRemark_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int l = 0; l < 3; l++)
			{
				errorType = this.astSetCompleteId[l].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || NewbieGuideMainLineConf.CURRVERSION < cutVer)
			{
				cutVer = NewbieGuideMainLineConf.CURRVERSION;
			}
			if (NewbieGuideMainLineConf.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOldPlayerGuide);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIndependentNet);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPriority);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSavePoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wTriggerLevelUpperLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wTriggerLevelLowerLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astTriggerTime[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				errorType = this.astTriggerCondition[j].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astSkipCondition[k].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num = 60;
			if (this.szRemark_ByteArray.GetLength(0) < num)
			{
				this.szRemark_ByteArray = new byte[NewbieGuideMainLineConf.LENGTH_szRemark];
			}
			errorType = srcBuf.readCString(ref this.szRemark_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int l = 0; l < 3; l++)
			{
				errorType = this.astSetCompleteId[l].load(ref srcBuf, cutVer);
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
