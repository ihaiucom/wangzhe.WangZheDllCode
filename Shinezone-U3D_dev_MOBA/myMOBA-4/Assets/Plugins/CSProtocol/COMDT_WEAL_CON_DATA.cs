using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WEAL_CON_DATA : ProtocolObject
	{
		public ushort wWealNum;

		public COMDT_WEAL_CON_DATA_DETAIL[] astWealDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 24u;

		public static readonly uint VERSION_wWealNum = 21u;

		public static readonly uint VERSION_astWealDetail = 21u;

		public static readonly int CLASS_ID = 505;

		public COMDT_WEAL_CON_DATA()
		{
			this.astWealDetail = new COMDT_WEAL_CON_DATA_DETAIL[20];
			for (int i = 0; i < 20; i++)
			{
				this.astWealDetail[i] = (COMDT_WEAL_CON_DATA_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_DETAIL.CLASS_ID);
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_WEAL_CON_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_CON_DATA.CURRVERSION;
			}
			if (COMDT_WEAL_CON_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			if (COMDT_WEAL_CON_DATA.VERSION_wWealNum <= cutVer)
			{
				errorType = destBuf.writeUInt16(this.wWealNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_WEAL_CON_DATA.VERSION_astWealDetail <= cutVer)
			{
				if (20 < this.wWealNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if (this.astWealDetail.Length < (int)this.wWealNum)
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				for (int i = 0; i < (int)this.wWealNum; i++)
				{
					errorType = this.astWealDetail[i].pack(ref destBuf, cutVer);
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_WEAL_CON_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_CON_DATA.CURRVERSION;
			}
			if (COMDT_WEAL_CON_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			if (COMDT_WEAL_CON_DATA.VERSION_wWealNum <= cutVer)
			{
				errorType = srcBuf.readUInt16(ref this.wWealNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.wWealNum = 0;
			}
			if (COMDT_WEAL_CON_DATA.VERSION_astWealDetail <= cutVer)
			{
				if (20 < this.wWealNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				for (int i = 0; i < (int)this.wWealNum; i++)
				{
					errorType = this.astWealDetail[i].unpack(ref srcBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			else
			{
				if (20 < this.wWealNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				for (int j = 0; j < (int)this.wWealNum; j++)
				{
					errorType = this.astWealDetail[j].construct();
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
			return COMDT_WEAL_CON_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wWealNum = 0;
			if (this.astWealDetail != null)
			{
				for (int i = 0; i < this.astWealDetail.Length; i++)
				{
					if (this.astWealDetail[i] != null)
					{
						this.astWealDetail[i].Release();
						this.astWealDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astWealDetail != null)
			{
				for (int i = 0; i < this.astWealDetail.Length; i++)
				{
					this.astWealDetail[i] = (COMDT_WEAL_CON_DATA_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
