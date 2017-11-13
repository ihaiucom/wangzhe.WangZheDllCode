using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_FRIEND_DONATE_INFO : ProtocolObject
	{
		public uint dwLastDonateTime;

		public byte bDonateNum;

		public byte bGetCoinNums;

		public COMDT_DONATE_ITEM[] astDonateAcntList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 125u;

		public static readonly uint VERSION_bGetCoinNums = 125u;

		public static readonly int CLASS_ID = 565;

		public COMDT_FRIEND_DONATE_INFO()
		{
			this.astDonateAcntList = new COMDT_DONATE_ITEM[100];
			for (int i = 0; i < 100; i++)
			{
				this.astDonateAcntList[i] = (COMDT_DONATE_ITEM)ProtocolObjectPool.Get(COMDT_DONATE_ITEM.CLASS_ID);
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
			if (cutVer == 0u || COMDT_FRIEND_DONATE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FRIEND_DONATE_INFO.CURRVERSION;
			}
			if (COMDT_FRIEND_DONATE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastDonateTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bDonateNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_FRIEND_DONATE_INFO.VERSION_bGetCoinNums <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGetCoinNums);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (100 < this.bDonateNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astDonateAcntList.Length < (int)this.bDonateNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bDonateNum; i++)
			{
				errorType = this.astDonateAcntList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_FRIEND_DONATE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FRIEND_DONATE_INFO.CURRVERSION;
			}
			if (COMDT_FRIEND_DONATE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastDonateTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bDonateNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_FRIEND_DONATE_INFO.VERSION_bGetCoinNums <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bGetCoinNums);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bGetCoinNums = 0;
			}
			if (100 < this.bDonateNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bDonateNum; i++)
			{
				errorType = this.astDonateAcntList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_FRIEND_DONATE_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastDonateTime = 0u;
			this.bDonateNum = 0;
			this.bGetCoinNums = 0;
			if (this.astDonateAcntList != null)
			{
				for (int i = 0; i < this.astDonateAcntList.Length; i++)
				{
					if (this.astDonateAcntList[i] != null)
					{
						this.astDonateAcntList[i].Release();
						this.astDonateAcntList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astDonateAcntList != null)
			{
				for (int i = 0; i < this.astDonateAcntList.Length; i++)
				{
					this.astDonateAcntList[i] = (COMDT_DONATE_ITEM)ProtocolObjectPool.Get(COMDT_DONATE_ITEM.CLASS_ID);
				}
			}
		}
	}
}
