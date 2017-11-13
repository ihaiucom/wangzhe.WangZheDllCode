using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GAMEACNT_SYMBOLPAGE : ProtocolObject
	{
		public byte bValidPageCnt;

		public COMDT_GAMEACNT_PAGEINFO[] astPageList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 208;

		public COMDT_GAMEACNT_SYMBOLPAGE()
		{
			this.astPageList = new COMDT_GAMEACNT_PAGEINFO[50];
			for (int i = 0; i < 50; i++)
			{
				this.astPageList[i] = (COMDT_GAMEACNT_PAGEINFO)ProtocolObjectPool.Get(COMDT_GAMEACNT_PAGEINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GAMEACNT_SYMBOLPAGE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GAMEACNT_SYMBOLPAGE.CURRVERSION;
			}
			if (COMDT_GAMEACNT_SYMBOLPAGE.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_GAMEACNT_SYMBOLPAGE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GAMEACNT_SYMBOLPAGE.CURRVERSION;
			}
			if (COMDT_GAMEACNT_SYMBOLPAGE.BASEVERSION > cutVer)
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
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GAMEACNT_SYMBOLPAGE.CLASS_ID;
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
		}

		public override void OnUse()
		{
			if (this.astPageList != null)
			{
				for (int i = 0; i < this.astPageList.Length; i++)
				{
					this.astPageList[i] = (COMDT_GAMEACNT_PAGEINFO)ProtocolObjectPool.Get(COMDT_GAMEACNT_PAGEINFO.CLASS_ID);
				}
			}
		}
	}
}
