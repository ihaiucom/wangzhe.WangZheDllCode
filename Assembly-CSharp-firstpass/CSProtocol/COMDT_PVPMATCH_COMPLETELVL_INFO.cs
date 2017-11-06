using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PVPMATCH_COMPLETELVL_INFO : ProtocolObject
	{
		public uint dwLevelNum;

		public COMDT_LEVEL_PVPMATCH_INFO[] astLevelDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 550;

		public COMDT_PVPMATCH_COMPLETELVL_INFO()
		{
			this.astLevelDetail = new COMDT_LEVEL_PVPMATCH_INFO[50];
			for (int i = 0; i < 50; i++)
			{
				this.astLevelDetail[i] = (COMDT_LEVEL_PVPMATCH_INFO)ProtocolObjectPool.Get(COMDT_LEVEL_PVPMATCH_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PVPMATCH_COMPLETELVL_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVPMATCH_COMPLETELVL_INFO.CURRVERSION;
			}
			if (COMDT_PVPMATCH_COMPLETELVL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLevelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50u < this.dwLevelNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astLevelDetail.Length < (long)((ulong)this.dwLevelNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwLevelNum))
			{
				errorType = this.astLevelDetail[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_PVPMATCH_COMPLETELVL_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVPMATCH_COMPLETELVL_INFO.CURRVERSION;
			}
			if (COMDT_PVPMATCH_COMPLETELVL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLevelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50u < this.dwLevelNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwLevelNum))
			{
				errorType = this.astLevelDetail[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_PVPMATCH_COMPLETELVL_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLevelNum = 0u;
			if (this.astLevelDetail != null)
			{
				for (int i = 0; i < this.astLevelDetail.Length; i++)
				{
					if (this.astLevelDetail[i] != null)
					{
						this.astLevelDetail[i].Release();
						this.astLevelDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astLevelDetail != null)
			{
				for (int i = 0; i < this.astLevelDetail.Length; i++)
				{
					this.astLevelDetail[i] = (COMDT_LEVEL_PVPMATCH_INFO)ProtocolObjectPool.Get(COMDT_LEVEL_PVPMATCH_INFO.CLASS_ID);
				}
			}
		}
	}
}
