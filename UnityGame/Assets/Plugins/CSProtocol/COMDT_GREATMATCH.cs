using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GREATMATCH : ProtocolObject
	{
		public COMDT_OB_DESK stDesk;

		public uint dwStartTime;

		public uint dwObserveNum;

		public ulong ullFeature;

		public uint dwLabelNum;

		public COMDT_HEROLABEL[] astLabel;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 607;

		public COMDT_GREATMATCH()
		{
			this.stDesk = (COMDT_OB_DESK)ProtocolObjectPool.Get(COMDT_OB_DESK.CLASS_ID);
			this.astLabel = new COMDT_HEROLABEL[2];
			for (int i = 0; i < 2; i++)
			{
				this.astLabel[i] = (COMDT_HEROLABEL)ProtocolObjectPool.Get(COMDT_HEROLABEL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GREATMATCH.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GREATMATCH.CURRVERSION;
			}
			if (COMDT_GREATMATCH.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stDesk.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwObserveNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullFeature);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLabelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (2u < this.dwLabelNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astLabel.Length < (long)((ulong)this.dwLabelNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwLabelNum))
			{
				errorType = this.astLabel[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_GREATMATCH.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GREATMATCH.CURRVERSION;
			}
			if (COMDT_GREATMATCH.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stDesk.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwObserveNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullFeature);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLabelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (2u < this.dwLabelNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwLabelNum))
			{
				errorType = this.astLabel[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_GREATMATCH.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stDesk != null)
			{
				this.stDesk.Release();
				this.stDesk = null;
			}
			this.dwStartTime = 0u;
			this.dwObserveNum = 0u;
			this.ullFeature = 0uL;
			this.dwLabelNum = 0u;
			if (this.astLabel != null)
			{
				for (int i = 0; i < this.astLabel.Length; i++)
				{
					if (this.astLabel[i] != null)
					{
						this.astLabel[i].Release();
						this.astLabel[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			this.stDesk = (COMDT_OB_DESK)ProtocolObjectPool.Get(COMDT_OB_DESK.CLASS_ID);
			if (this.astLabel != null)
			{
				for (int i = 0; i < this.astLabel.Length; i++)
				{
					this.astLabel[i] = (COMDT_HEROLABEL)ProtocolObjectPool.Get(COMDT_HEROLABEL.CLASS_ID);
				}
			}
		}
	}
}
