using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TEAMMEMBER : ProtocolObject
	{
		public uint dwMemNum;

		public COMDT_TEAMMEMBER_INFO[] astMemInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 92u;

		public static readonly int CLASS_ID = 325;

		public COMDT_TEAMMEMBER()
		{
			this.astMemInfo = new COMDT_TEAMMEMBER_INFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astMemInfo[i] = (COMDT_TEAMMEMBER_INFO)ProtocolObjectPool.Get(COMDT_TEAMMEMBER_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TEAMMEMBER.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAMMEMBER.CURRVERSION;
			}
			if (COMDT_TEAMMEMBER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwMemNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwMemNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astMemInfo.Length < (long)((ulong)this.dwMemNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwMemNum))
			{
				errorType = this.astMemInfo[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_TEAMMEMBER.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAMMEMBER.CURRVERSION;
			}
			if (COMDT_TEAMMEMBER.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwMemNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwMemNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwMemNum))
			{
				errorType = this.astMemInfo[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_TEAMMEMBER.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwMemNum = 0u;
			if (this.astMemInfo != null)
			{
				for (int i = 0; i < this.astMemInfo.Length; i++)
				{
					if (this.astMemInfo[i] != null)
					{
						this.astMemInfo[i].Release();
						this.astMemInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMemInfo != null)
			{
				for (int i = 0; i < this.astMemInfo.Length; i++)
				{
					this.astMemInfo[i] = (COMDT_TEAMMEMBER_INFO)ProtocolObjectPool.Get(COMDT_TEAMMEMBER_INFO.CLASS_ID);
				}
			}
		}
	}
}
