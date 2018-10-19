using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACTIVITY_DETAIL : ProtocolObject
	{
		public uint dwActivityID;

		public uint dwAccPlayedCount;

		public byte bOpenType;

		public COMDT_ACTIVITY_OPEN_UNION stOpenData;

		public ushort wSubActivityCnt;

		public COMDT_SUBACTIVITY_DETAIL[] astSubActivityDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 147;

		public COMDT_ACTIVITY_DETAIL()
		{
			this.stOpenData = (COMDT_ACTIVITY_OPEN_UNION)ProtocolObjectPool.Get(COMDT_ACTIVITY_OPEN_UNION.CLASS_ID);
			this.astSubActivityDetail = new COMDT_SUBACTIVITY_DETAIL[20];
			for (int i = 0; i < 20; i++)
			{
				this.astSubActivityDetail[i] = (COMDT_SUBACTIVITY_DETAIL)ProtocolObjectPool.Get(COMDT_SUBACTIVITY_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACTIVITY_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACTIVITY_DETAIL.CURRVERSION;
			}
			if (COMDT_ACTIVITY_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwActivityID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwAccPlayedCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bOpenType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bOpenType;
			errorType = this.stOpenData.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wSubActivityCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.wSubActivityCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSubActivityDetail.Length < (int)this.wSubActivityCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wSubActivityCnt; i++)
			{
				errorType = this.astSubActivityDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACTIVITY_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACTIVITY_DETAIL.CURRVERSION;
			}
			if (COMDT_ACTIVITY_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwActivityID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAccPlayedCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOpenType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bOpenType;
			errorType = this.stOpenData.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wSubActivityCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.wSubActivityCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wSubActivityCnt; i++)
			{
				errorType = this.astSubActivityDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACTIVITY_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwActivityID = 0u;
			this.dwAccPlayedCount = 0u;
			this.bOpenType = 0;
			if (this.stOpenData != null)
			{
				this.stOpenData.Release();
				this.stOpenData = null;
			}
			this.wSubActivityCnt = 0;
			if (this.astSubActivityDetail != null)
			{
				for (int i = 0; i < this.astSubActivityDetail.Length; i++)
				{
					if (this.astSubActivityDetail[i] != null)
					{
						this.astSubActivityDetail[i].Release();
						this.astSubActivityDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			this.stOpenData = (COMDT_ACTIVITY_OPEN_UNION)ProtocolObjectPool.Get(COMDT_ACTIVITY_OPEN_UNION.CLASS_ID);
			if (this.astSubActivityDetail != null)
			{
				for (int i = 0; i < this.astSubActivityDetail.Length; i++)
				{
					this.astSubActivityDetail[i] = (COMDT_SUBACTIVITY_DETAIL)ProtocolObjectPool.Get(COMDT_SUBACTIVITY_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
