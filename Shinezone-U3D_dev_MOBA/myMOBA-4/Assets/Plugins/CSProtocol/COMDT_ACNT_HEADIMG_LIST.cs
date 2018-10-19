using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_HEADIMG_LIST : ProtocolObject
	{
		public ushort wHeadImgCnt;

		public COMDT_ACNT_HEADIMG_INFO[] astHeadImgInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 595;

		public COMDT_ACNT_HEADIMG_LIST()
		{
			this.astHeadImgInfo = new COMDT_ACNT_HEADIMG_INFO[300];
			for (int i = 0; i < 300; i++)
			{
				this.astHeadImgInfo[i] = (COMDT_ACNT_HEADIMG_INFO)ProtocolObjectPool.Get(COMDT_ACNT_HEADIMG_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_HEADIMG_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_HEADIMG_LIST.CURRVERSION;
			}
			if (COMDT_ACNT_HEADIMG_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wHeadImgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (300 < this.wHeadImgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astHeadImgInfo.Length < (int)this.wHeadImgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wHeadImgCnt; i++)
			{
				errorType = this.astHeadImgInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_HEADIMG_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_HEADIMG_LIST.CURRVERSION;
			}
			if (COMDT_ACNT_HEADIMG_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wHeadImgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (300 < this.wHeadImgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wHeadImgCnt; i++)
			{
				errorType = this.astHeadImgInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_HEADIMG_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wHeadImgCnt = 0;
			if (this.astHeadImgInfo != null)
			{
				for (int i = 0; i < this.astHeadImgInfo.Length; i++)
				{
					if (this.astHeadImgInfo[i] != null)
					{
						this.astHeadImgInfo[i].Release();
						this.astHeadImgInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHeadImgInfo != null)
			{
				for (int i = 0; i < this.astHeadImgInfo.Length; i++)
				{
					this.astHeadImgInfo[i] = (COMDT_ACNT_HEADIMG_INFO)ProtocolObjectPool.Get(COMDT_ACNT_HEADIMG_INFO.CLASS_ID);
				}
			}
		}
	}
}
