using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_SYMBOLINFO : ProtocolObject
	{
		public byte bValidPageCnt;

		public COMDT_SYMBOLPAGE_DETAIL[] astPageList;

		public byte bBuyPageCnt;

		public COMDT_SYMBOLPAGE_EXTRA[] astPageExtra;

		public COMDT_SYMBOLPAGE_RCMD stRecommend;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 111u;

		public static readonly uint VERSION_bBuyPageCnt = 19u;

		public static readonly uint VERSION_stRecommend = 111u;

		public static readonly int CLASS_ID = 90;

		public COMDT_ACNT_SYMBOLINFO()
		{
			this.astPageList = new COMDT_SYMBOLPAGE_DETAIL[50];
			for (int i = 0; i < 50; i++)
			{
				this.astPageList[i] = (COMDT_SYMBOLPAGE_DETAIL)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_DETAIL.CLASS_ID);
			}
			this.astPageExtra = new COMDT_SYMBOLPAGE_EXTRA[30];
			for (int j = 0; j < 30; j++)
			{
				this.astPageExtra[j] = (COMDT_SYMBOLPAGE_EXTRA)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_EXTRA.CLASS_ID);
			}
			this.stRecommend = (COMDT_SYMBOLPAGE_RCMD)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_RCMD.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_SYMBOLINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_SYMBOLINFO.CURRVERSION;
			}
			if (COMDT_ACNT_SYMBOLINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bValidPageCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50 < this.bValidPageCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astPageList.Length < (int)this.bValidPageCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bValidPageCnt; i++)
			{
				errorType = this.astPageList[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_SYMBOLINFO.VERSION_bBuyPageCnt <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bBuyPageCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 30; j++)
			{
				errorType = this.astPageExtra[j].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_SYMBOLINFO.VERSION_stRecommend <= cutVer)
			{
				errorType = this.stRecommend.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_SYMBOLINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_SYMBOLINFO.CURRVERSION;
			}
			if (COMDT_ACNT_SYMBOLINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bValidPageCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50 < this.bValidPageCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bValidPageCnt; i++)
			{
				errorType = this.astPageList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_SYMBOLINFO.VERSION_bBuyPageCnt <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bBuyPageCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bBuyPageCnt = 0;
			}
			for (int j = 0; j < 30; j++)
			{
				errorType = this.astPageExtra[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_SYMBOLINFO.VERSION_stRecommend <= cutVer)
			{
				errorType = this.stRecommend.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stRecommend.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_SYMBOLINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bValidPageCnt = 0;
			if (this.astPageList != null)
			{
				for (int i = 0; i < this.astPageList.Length; i++)
				{
					if (this.astPageList[i] != null)
					{
						this.astPageList[i].Release();
						this.astPageList[i] = null;
					}
				}
			}
			this.bBuyPageCnt = 0;
			if (this.astPageExtra != null)
			{
				for (int j = 0; j < this.astPageExtra.Length; j++)
				{
					if (this.astPageExtra[j] != null)
					{
						this.astPageExtra[j].Release();
						this.astPageExtra[j] = null;
					}
				}
			}
			if (this.stRecommend != null)
			{
				this.stRecommend.Release();
				this.stRecommend = null;
			}
		}

		public override void OnUse()
		{
			if (this.astPageList != null)
			{
				for (int i = 0; i < this.astPageList.Length; i++)
				{
					this.astPageList[i] = (COMDT_SYMBOLPAGE_DETAIL)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_DETAIL.CLASS_ID);
				}
			}
			if (this.astPageExtra != null)
			{
				for (int j = 0; j < this.astPageExtra.Length; j++)
				{
					this.astPageExtra[j] = (COMDT_SYMBOLPAGE_EXTRA)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_EXTRA.CLASS_ID);
				}
			}
			this.stRecommend = (COMDT_SYMBOLPAGE_RCMD)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_RCMD.CLASS_ID);
		}
	}
}
