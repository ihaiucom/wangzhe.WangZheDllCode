using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PUNISH_CREDIT : ProtocolObject
	{
		public int iCreditChgValue;

		public uint dwCreditChgType;

		public uint dwLastDeskId;

		public uint dwLastDeskSeq;

		public uint dwPunishNum;

		public uint dwLastPunishTime;

		public uint dwTypeNum;

		public int[] SumDelCredie;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 174u;

		public static readonly uint VERSION_dwCreditChgType = 174u;

		public static readonly uint VERSION_dwPunishNum = 154u;

		public static readonly uint VERSION_dwLastPunishTime = 154u;

		public static readonly uint VERSION_dwTypeNum = 174u;

		public static readonly uint VERSION_SumDelCredie = 174u;

		public static readonly int CLASS_ID = 621;

		public COMDT_PUNISH_CREDIT()
		{
			this.SumDelCredie = new int[13];
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
			if (cutVer == 0u || COMDT_PUNISH_CREDIT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PUNISH_CREDIT.CURRVERSION;
			}
			if (COMDT_PUNISH_CREDIT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iCreditChgValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwCreditChgType <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwCreditChgType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwLastDeskId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastDeskSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwPunishNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwPunishNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwLastPunishTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastPunishTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwTypeNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwTypeNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PUNISH_CREDIT.VERSION_SumDelCredie <= cutVer)
			{
				if (13u < this.dwTypeNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if ((long)this.SumDelCredie.Length < (long)((ulong)this.dwTypeNum))
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				int num = 0;
				while ((long)num < (long)((ulong)this.dwTypeNum))
				{
					errorType = destBuf.writeInt32(this.SumDelCredie[num]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					num++;
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
			if (cutVer == 0u || COMDT_PUNISH_CREDIT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PUNISH_CREDIT.CURRVERSION;
			}
			if (COMDT_PUNISH_CREDIT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCreditChgValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwCreditChgType <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwCreditChgType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwCreditChgType = 0u;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastDeskId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastDeskSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwPunishNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwPunishNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwPunishNum = 0u;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwLastPunishTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastPunishTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastPunishTime = 0u;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_dwTypeNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwTypeNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwTypeNum = 0u;
			}
			if (COMDT_PUNISH_CREDIT.VERSION_SumDelCredie <= cutVer)
			{
				if (13u < this.dwTypeNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				this.SumDelCredie = new int[this.dwTypeNum];
				int num = 0;
				while ((long)num < (long)((ulong)this.dwTypeNum))
				{
					errorType = srcBuf.readInt32(ref this.SumDelCredie[num]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					num++;
				}
			}
			else if (13u < this.dwTypeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PUNISH_CREDIT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iCreditChgValue = 0;
			this.dwCreditChgType = 0u;
			this.dwLastDeskId = 0u;
			this.dwLastDeskSeq = 0u;
			this.dwPunishNum = 0u;
			this.dwLastPunishTime = 0u;
			this.dwTypeNum = 0u;
		}
	}
}
