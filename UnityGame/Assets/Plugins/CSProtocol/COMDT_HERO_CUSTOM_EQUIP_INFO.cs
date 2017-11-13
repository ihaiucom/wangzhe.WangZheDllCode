using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_CUSTOM_EQUIP_INFO : ProtocolObject
	{
		public byte bEquipNum;

		public uint[] EquipID;

		public ulong ullUsedWinNum;

		public uint dwTimeStamp;

		public ulong ullUsedTotalNum;

		public COMDT_EQUIPEVAL stEquipEvalInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 170u;

		public static readonly uint VERSION_ullUsedTotalNum = 143u;

		public static readonly uint VERSION_stEquipEvalInfo = 166u;

		public static readonly int CLASS_ID = 591;

		public COMDT_HERO_CUSTOM_EQUIP_INFO()
		{
			this.EquipID = new uint[6];
			this.stEquipEvalInfo = (COMDT_EQUIPEVAL)ProtocolObjectPool.Get(COMDT_EQUIPEVAL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_HERO_CUSTOM_EQUIP_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_CUSTOM_EQUIP_INFO.CURRVERSION;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.EquipID.Length < (int)this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bEquipNum; i++)
			{
				errorType = destBuf.writeUInt32(this.EquipID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt64(this.ullUsedWinNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTimeStamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_INFO.VERSION_ullUsedTotalNum <= cutVer)
			{
				errorType = destBuf.writeUInt64(this.ullUsedTotalNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_CUSTOM_EQUIP_INFO.VERSION_stEquipEvalInfo <= cutVer)
			{
				errorType = this.stEquipEvalInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_HERO_CUSTOM_EQUIP_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_CUSTOM_EQUIP_INFO.CURRVERSION;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.EquipID = new uint[(int)this.bEquipNum];
			for (int i = 0; i < (int)this.bEquipNum; i++)
			{
				errorType = srcBuf.readUInt32(ref this.EquipID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt64(ref this.ullUsedWinNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeStamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_INFO.VERSION_ullUsedTotalNum <= cutVer)
			{
				errorType = srcBuf.readUInt64(ref this.ullUsedTotalNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.ullUsedTotalNum = 0uL;
			}
			if (COMDT_HERO_CUSTOM_EQUIP_INFO.VERSION_stEquipEvalInfo <= cutVer)
			{
				errorType = this.stEquipEvalInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stEquipEvalInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_HERO_CUSTOM_EQUIP_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bEquipNum = 0;
			this.ullUsedWinNum = 0uL;
			this.dwTimeStamp = 0u;
			this.ullUsedTotalNum = 0uL;
			if (this.stEquipEvalInfo != null)
			{
				this.stEquipEvalInfo.Release();
				this.stEquipEvalInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stEquipEvalInfo = (COMDT_EQUIPEVAL)ProtocolObjectPool.Get(COMDT_EQUIPEVAL.CLASS_ID);
		}
	}
}
