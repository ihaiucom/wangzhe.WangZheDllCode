using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_REWARD_MATCH : ProtocolObject
	{
		public ulong ullActivePlayerNum;

		public uint dwMatchNum;

		public COMDT_ONE_REWARD_MATCH[] astMatchDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 588;

		public COMDT_REWARD_MATCH()
		{
			this.astMatchDetail = new COMDT_ONE_REWARD_MATCH[128];
			for (int i = 0; i < 128; i++)
			{
				this.astMatchDetail[i] = (COMDT_ONE_REWARD_MATCH)ProtocolObjectPool.Get(COMDT_ONE_REWARD_MATCH.CLASS_ID);
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
			if (cutVer == 0u || COMDT_REWARD_MATCH.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_MATCH.CURRVERSION;
			}
			if (COMDT_REWARD_MATCH.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullActivePlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMatchNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (128u < this.dwMatchNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astMatchDetail.Length < (long)((ulong)this.dwMatchNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwMatchNum))
			{
				errorType = this.astMatchDetail[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_REWARD_MATCH.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_MATCH.CURRVERSION;
			}
			if (COMDT_REWARD_MATCH.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullActivePlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMatchNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (128u < this.dwMatchNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwMatchNum))
			{
				errorType = this.astMatchDetail[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_REWARD_MATCH.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullActivePlayerNum = 0uL;
			this.dwMatchNum = 0u;
			if (this.astMatchDetail != null)
			{
				for (int i = 0; i < this.astMatchDetail.Length; i++)
				{
					if (this.astMatchDetail[i] != null)
					{
						this.astMatchDetail[i].Release();
						this.astMatchDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMatchDetail != null)
			{
				for (int i = 0; i < this.astMatchDetail.Length; i++)
				{
					this.astMatchDetail[i] = (COMDT_ONE_REWARD_MATCH)ProtocolObjectPool.Get(COMDT_ONE_REWARD_MATCH.CLASS_ID);
				}
			}
		}
	}
}
