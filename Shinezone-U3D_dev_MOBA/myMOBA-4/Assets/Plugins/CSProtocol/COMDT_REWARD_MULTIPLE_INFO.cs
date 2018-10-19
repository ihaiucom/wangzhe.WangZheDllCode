using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_REWARD_MULTIPLE_INFO : ProtocolObject
	{
		public ushort wRewardType;

		public uint dwRewardTypeParam;

		public COMDT_MULTIPLE_INFO stMultipleInfo;

		public COMDT_MULTIPLE_INFO_NEW stNewMultipleInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 175u;

		public static readonly uint VERSION_stNewMultipleInfo = 175u;

		public static readonly int CLASS_ID = 237;

		public COMDT_REWARD_MULTIPLE_INFO()
		{
			this.stMultipleInfo = (COMDT_MULTIPLE_INFO)ProtocolObjectPool.Get(COMDT_MULTIPLE_INFO.CLASS_ID);
			this.stNewMultipleInfo = (COMDT_MULTIPLE_INFO_NEW)ProtocolObjectPool.Get(COMDT_MULTIPLE_INFO_NEW.CLASS_ID);
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
			if (cutVer == 0u || COMDT_REWARD_MULTIPLE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_MULTIPLE_INFO.CURRVERSION;
			}
			if (COMDT_REWARD_MULTIPLE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wRewardType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRewardTypeParam);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_REWARD_MULTIPLE_INFO.VERSION_stNewMultipleInfo <= cutVer)
			{
				errorType = this.stNewMultipleInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_REWARD_MULTIPLE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_MULTIPLE_INFO.CURRVERSION;
			}
			if (COMDT_REWARD_MULTIPLE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wRewardType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRewardTypeParam);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_REWARD_MULTIPLE_INFO.VERSION_stNewMultipleInfo <= cutVer)
			{
				errorType = this.stNewMultipleInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stNewMultipleInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_REWARD_MULTIPLE_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wRewardType = 0;
			this.dwRewardTypeParam = 0u;
			if (this.stMultipleInfo != null)
			{
				this.stMultipleInfo.Release();
				this.stMultipleInfo = null;
			}
			if (this.stNewMultipleInfo != null)
			{
				this.stNewMultipleInfo.Release();
				this.stNewMultipleInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stMultipleInfo = (COMDT_MULTIPLE_INFO)ProtocolObjectPool.Get(COMDT_MULTIPLE_INFO.CLASS_ID);
			this.stNewMultipleInfo = (COMDT_MULTIPLE_INFO_NEW)ProtocolObjectPool.Get(COMDT_MULTIPLE_INFO_NEW.CLASS_ID);
		}
	}
}
