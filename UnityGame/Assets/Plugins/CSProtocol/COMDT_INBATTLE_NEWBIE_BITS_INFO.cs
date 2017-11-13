using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_INBATTLE_NEWBIE_BITS_INFO : ProtocolObject
	{
		public int iLevelID;

		public byte bReportFinished;

		public byte bFinishedNum;

		public uint[] FinishedDetail;

		public uint[] FinishedTime;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 157u;

		public static readonly uint VERSION_FinishedTime = 157u;

		public static readonly int CLASS_ID = 573;

		public COMDT_INBATTLE_NEWBIE_BITS_INFO()
		{
			this.FinishedDetail = new uint[20];
			this.FinishedTime = new uint[20];
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
			if (cutVer == 0u || COMDT_INBATTLE_NEWBIE_BITS_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INBATTLE_NEWBIE_BITS_INFO.CURRVERSION;
			}
			if (COMDT_INBATTLE_NEWBIE_BITS_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iLevelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bReportFinished);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bFinishedNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bFinishedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.FinishedDetail.Length < (int)this.bFinishedNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bFinishedNum; i++)
			{
				errorType = destBuf.writeUInt32(this.FinishedDetail[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_INBATTLE_NEWBIE_BITS_INFO.VERSION_FinishedTime <= cutVer)
			{
				if (20 < this.bFinishedNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if (this.FinishedTime.Length < (int)this.bFinishedNum)
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				for (int j = 0; j < (int)this.bFinishedNum; j++)
				{
					errorType = destBuf.writeUInt32(this.FinishedTime[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
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
			if (cutVer == 0u || COMDT_INBATTLE_NEWBIE_BITS_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INBATTLE_NEWBIE_BITS_INFO.CURRVERSION;
			}
			if (COMDT_INBATTLE_NEWBIE_BITS_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iLevelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bReportFinished);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFinishedNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bFinishedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.FinishedDetail = new uint[(int)this.bFinishedNum];
			for (int i = 0; i < (int)this.bFinishedNum; i++)
			{
				errorType = srcBuf.readUInt32(ref this.FinishedDetail[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_INBATTLE_NEWBIE_BITS_INFO.VERSION_FinishedTime <= cutVer)
			{
				if (20 < this.bFinishedNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				this.FinishedTime = new uint[(int)this.bFinishedNum];
				for (int j = 0; j < (int)this.bFinishedNum; j++)
				{
					errorType = srcBuf.readUInt32(ref this.FinishedTime[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			else if (20 < this.bFinishedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_INBATTLE_NEWBIE_BITS_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iLevelID = 0;
			this.bReportFinished = 0;
			this.bFinishedNum = 0;
		}
	}
}
