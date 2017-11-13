using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_EQUIPEVAL_LIST : ProtocolObject
	{
		public uint dwEquipEvalNum;

		public COMDT_HERO_EQUIPEVAL[] astEquipEvalInfoList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 167u;

		public static readonly int CLASS_ID = 150;

		public COMDT_EQUIPEVAL_LIST()
		{
			this.astEquipEvalInfoList = new COMDT_HERO_EQUIPEVAL[20];
			for (int i = 0; i < 20; i++)
			{
				this.astEquipEvalInfoList[i] = (COMDT_HERO_EQUIPEVAL)ProtocolObjectPool.Get(COMDT_HERO_EQUIPEVAL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_EQUIPEVAL_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_EQUIPEVAL_LIST.CURRVERSION;
			}
			if (COMDT_EQUIPEVAL_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwEquipEvalNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwEquipEvalNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astEquipEvalInfoList.Length < (long)((ulong)this.dwEquipEvalNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwEquipEvalNum))
			{
				errorType = this.astEquipEvalInfoList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_EQUIPEVAL_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_EQUIPEVAL_LIST.CURRVERSION;
			}
			if (COMDT_EQUIPEVAL_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwEquipEvalNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwEquipEvalNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwEquipEvalNum))
			{
				errorType = this.astEquipEvalInfoList[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_EQUIPEVAL_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwEquipEvalNum = 0u;
			if (this.astEquipEvalInfoList != null)
			{
				for (int i = 0; i < this.astEquipEvalInfoList.Length; i++)
				{
					if (this.astEquipEvalInfoList[i] != null)
					{
						this.astEquipEvalInfoList[i].Release();
						this.astEquipEvalInfoList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astEquipEvalInfoList != null)
			{
				for (int i = 0; i < this.astEquipEvalInfoList.Length; i++)
				{
					this.astEquipEvalInfoList[i] = (COMDT_HERO_EQUIPEVAL)ProtocolObjectPool.Get(COMDT_HERO_EQUIPEVAL.CLASS_ID);
				}
			}
		}
	}
}
