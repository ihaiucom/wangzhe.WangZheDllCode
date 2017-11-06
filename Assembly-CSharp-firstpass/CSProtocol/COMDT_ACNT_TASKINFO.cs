using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_TASKINFO : ProtocolObject
	{
		public ushort wMaxUsualTaskRefrshCnt;

		public uint dwLastFreshTime;

		public uint dwLastMainTaskFreshTime;

		public uint dwCurtaskNum;

		public COMDT_ACNT_CURTASK[] astCurtask;

		public uint[] MainTaskIDs;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 104u;

		public static readonly uint VERSION_wMaxUsualTaskRefrshCnt = 15u;

		public static readonly uint VERSION_dwLastMainTaskFreshTime = 37u;

		public static readonly uint VERSION_MainTaskIDs = 104u;

		public static readonly int CLASS_ID = 256;

		public COMDT_ACNT_TASKINFO()
		{
			this.astCurtask = new COMDT_ACNT_CURTASK[85];
			for (int i = 0; i < 85; i++)
			{
				this.astCurtask[i] = (COMDT_ACNT_CURTASK)ProtocolObjectPool.Get(COMDT_ACNT_CURTASK.CLASS_ID);
			}
			this.MainTaskIDs = new uint[10];
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
			if (cutVer == 0u || COMDT_ACNT_TASKINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_TASKINFO.CURRVERSION;
			}
			if (COMDT_ACNT_TASKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			if (COMDT_ACNT_TASKINFO.VERSION_wMaxUsualTaskRefrshCnt <= cutVer)
			{
				errorType = destBuf.writeUInt16(this.wMaxUsualTaskRefrshCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwLastFreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_TASKINFO.VERSION_dwLastMainTaskFreshTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastMainTaskFreshTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwCurtaskNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwCurtaskNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astCurtask.Length < (long)((ulong)this.dwCurtaskNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCurtaskNum))
			{
				errorType = this.astCurtask[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			if (COMDT_ACNT_TASKINFO.VERSION_MainTaskIDs <= cutVer)
			{
				for (int i = 0; i < 10; i++)
				{
					errorType = destBuf.writeUInt32(this.MainTaskIDs[i]);
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
			if (cutVer == 0u || COMDT_ACNT_TASKINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_TASKINFO.CURRVERSION;
			}
			if (COMDT_ACNT_TASKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			if (COMDT_ACNT_TASKINFO.VERSION_wMaxUsualTaskRefrshCnt <= cutVer)
			{
				errorType = srcBuf.readUInt16(ref this.wMaxUsualTaskRefrshCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.wMaxUsualTaskRefrshCnt = 0;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastFreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_TASKINFO.VERSION_dwLastMainTaskFreshTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastMainTaskFreshTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastMainTaskFreshTime = 0u;
			}
			errorType = srcBuf.readUInt32(ref this.dwCurtaskNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwCurtaskNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCurtaskNum))
			{
				errorType = this.astCurtask[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			if (COMDT_ACNT_TASKINFO.VERSION_MainTaskIDs <= cutVer)
			{
				for (int i = 0; i < 10; i++)
				{
					errorType = srcBuf.readUInt32(ref this.MainTaskIDs[i]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_TASKINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wMaxUsualTaskRefrshCnt = 0;
			this.dwLastFreshTime = 0u;
			this.dwLastMainTaskFreshTime = 0u;
			this.dwCurtaskNum = 0u;
			if (this.astCurtask != null)
			{
				for (int i = 0; i < this.astCurtask.Length; i++)
				{
					if (this.astCurtask[i] != null)
					{
						this.astCurtask[i].Release();
						this.astCurtask[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astCurtask != null)
			{
				for (int i = 0; i < this.astCurtask.Length; i++)
				{
					this.astCurtask[i] = (COMDT_ACNT_CURTASK)ProtocolObjectPool.Get(COMDT_ACNT_CURTASK.CLASS_ID);
				}
			}
		}
	}
}
