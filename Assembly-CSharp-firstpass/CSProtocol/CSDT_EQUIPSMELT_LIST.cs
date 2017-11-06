using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_EQUIPSMELT_LIST : ProtocolObject
	{
		public byte bEquipCnt;

		public CSDT_ITEM_DELINFO[] astEquipList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 980;

		public CSDT_EQUIPSMELT_LIST()
		{
			this.astEquipList = new CSDT_ITEM_DELINFO[400];
			for (int i = 0; i < 400; i++)
			{
				this.astEquipList[i] = (CSDT_ITEM_DELINFO)ProtocolObjectPool.Get(CSDT_ITEM_DELINFO.CLASS_ID);
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
			if (cutVer == 0u || CSDT_EQUIPSMELT_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_EQUIPSMELT_LIST.CURRVERSION;
			}
			if (CSDT_EQUIPSMELT_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bEquipCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.astEquipList.Length < (int)this.bEquipCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bEquipCnt; i++)
			{
				errorType = this.astEquipList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_EQUIPSMELT_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_EQUIPSMELT_LIST.CURRVERSION;
			}
			if (CSDT_EQUIPSMELT_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bEquipCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < (int)this.bEquipCnt; i++)
			{
				errorType = this.astEquipList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_EQUIPSMELT_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bEquipCnt = 0;
			if (this.astEquipList != null)
			{
				for (int i = 0; i < this.astEquipList.Length; i++)
				{
					if (this.astEquipList[i] != null)
					{
						this.astEquipList[i].Release();
						this.astEquipList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astEquipList != null)
			{
				for (int i = 0; i < this.astEquipList.Length; i++)
				{
					this.astEquipList[i] = (CSDT_ITEM_DELINFO)ProtocolObjectPool.Get(CSDT_ITEM_DELINFO.CLASS_ID);
				}
			}
		}
	}
}
