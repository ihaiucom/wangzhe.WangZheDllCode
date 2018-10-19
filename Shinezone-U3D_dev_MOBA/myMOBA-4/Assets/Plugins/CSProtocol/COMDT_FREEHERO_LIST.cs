using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_FREEHERO_LIST : ProtocolObject
	{
		public ushort wFreeCnt;

		public COMDT_FREEHERO_DETAIL[] astFreeHeroDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 135;

		public COMDT_FREEHERO_LIST()
		{
			this.astFreeHeroDetail = new COMDT_FREEHERO_DETAIL[200];
			for (int i = 0; i < 200; i++)
			{
				this.astFreeHeroDetail[i] = (COMDT_FREEHERO_DETAIL)ProtocolObjectPool.Get(COMDT_FREEHERO_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_FREEHERO_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FREEHERO_LIST.CURRVERSION;
			}
			if (COMDT_FREEHERO_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wFreeCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200 < this.wFreeCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astFreeHeroDetail.Length < (int)this.wFreeCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wFreeCnt; i++)
			{
				errorType = this.astFreeHeroDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_FREEHERO_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FREEHERO_LIST.CURRVERSION;
			}
			if (COMDT_FREEHERO_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wFreeCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200 < this.wFreeCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wFreeCnt; i++)
			{
				errorType = this.astFreeHeroDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_FREEHERO_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wFreeCnt = 0;
			if (this.astFreeHeroDetail != null)
			{
				for (int i = 0; i < this.astFreeHeroDetail.Length; i++)
				{
					if (this.astFreeHeroDetail[i] != null)
					{
						this.astFreeHeroDetail[i].Release();
						this.astFreeHeroDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astFreeHeroDetail != null)
			{
				for (int i = 0; i < this.astFreeHeroDetail.Length; i++)
				{
					this.astFreeHeroDetail[i] = (COMDT_FREEHERO_DETAIL)ProtocolObjectPool.Get(COMDT_FREEHERO_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
