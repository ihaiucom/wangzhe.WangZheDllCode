using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_INTIMACY_CARD_INFO : ProtocolObject
	{
		public byte bIntimacyNum;

		public COMDT_INTIMACY_CARD_DATA[] astIntimacyData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 567;

		public COMDT_INTIMACY_CARD_INFO()
		{
			this.astIntimacyData = new COMDT_INTIMACY_CARD_DATA[4];
			for (int i = 0; i < 4; i++)
			{
				this.astIntimacyData[i] = (COMDT_INTIMACY_CARD_DATA)ProtocolObjectPool.Get(COMDT_INTIMACY_CARD_DATA.CLASS_ID);
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
			if (cutVer == 0u || COMDT_INTIMACY_CARD_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INTIMACY_CARD_INFO.CURRVERSION;
			}
			if (COMDT_INTIMACY_CARD_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bIntimacyNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bIntimacyNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astIntimacyData.Length < (int)this.bIntimacyNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bIntimacyNum; i++)
			{
				errorType = this.astIntimacyData[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_INTIMACY_CARD_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INTIMACY_CARD_INFO.CURRVERSION;
			}
			if (COMDT_INTIMACY_CARD_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bIntimacyNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bIntimacyNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bIntimacyNum; i++)
			{
				errorType = this.astIntimacyData[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_INTIMACY_CARD_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bIntimacyNum = 0;
			if (this.astIntimacyData != null)
			{
				for (int i = 0; i < this.astIntimacyData.Length; i++)
				{
					if (this.astIntimacyData[i] != null)
					{
						this.astIntimacyData[i].Release();
						this.astIntimacyData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astIntimacyData != null)
			{
				for (int i = 0; i < this.astIntimacyData.Length; i++)
				{
					this.astIntimacyData[i] = (COMDT_INTIMACY_CARD_DATA)ProtocolObjectPool.Get(COMDT_INTIMACY_CARD_DATA.CLASS_ID);
				}
			}
		}
	}
}
