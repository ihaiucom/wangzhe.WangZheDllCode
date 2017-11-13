using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_CUSTOM_EQUIP_DETAIL : ProtocolObject
	{
		public uint dwUsedNum;

		public COMDT_HERO_CUSTOM_EQUIP_INFO[] astDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 170u;

		public static readonly int CLASS_ID = 592;

		public COMDT_HERO_CUSTOM_EQUIP_DETAIL()
		{
			this.astDetail = new COMDT_HERO_CUSTOM_EQUIP_INFO[1000];
			for (int i = 0; i < 1000; i++)
			{
				this.astDetail[i] = (COMDT_HERO_CUSTOM_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_HERO_CUSTOM_EQUIP_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_HERO_CUSTOM_EQUIP_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_CUSTOM_EQUIP_DETAIL.CURRVERSION;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwUsedNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (1000u < this.dwUsedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astDetail.Length < (long)((ulong)this.dwUsedNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwUsedNum))
			{
				errorType = this.astDetail[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_HERO_CUSTOM_EQUIP_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_CUSTOM_EQUIP_DETAIL.CURRVERSION;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwUsedNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (1000u < this.dwUsedNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwUsedNum))
			{
				errorType = this.astDetail[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_HERO_CUSTOM_EQUIP_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwUsedNum = 0u;
			if (this.astDetail != null)
			{
				for (int i = 0; i < this.astDetail.Length; i++)
				{
					if (this.astDetail[i] != null)
					{
						this.astDetail[i].Release();
						this.astDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astDetail != null)
			{
				for (int i = 0; i < this.astDetail.Length; i++)
				{
					this.astDetail[i] = (COMDT_HERO_CUSTOM_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_HERO_CUSTOM_EQUIP_INFO.CLASS_ID);
				}
			}
		}
	}
}
