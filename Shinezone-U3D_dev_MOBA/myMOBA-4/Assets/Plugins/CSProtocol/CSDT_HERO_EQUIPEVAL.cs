using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_HERO_EQUIPEVAL : ProtocolObject
	{
		public uint dwHeroId;

		public COMDT_HERO_EQUIP_INFO stHeroEquipList;

		public COMDT_EQUIPEVAL_PERACNT stEvalInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1609;

		public CSDT_HERO_EQUIPEVAL()
		{
			this.stHeroEquipList = (COMDT_HERO_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_HERO_EQUIP_INFO.CLASS_ID);
			this.stEvalInfo = (COMDT_EQUIPEVAL_PERACNT)ProtocolObjectPool.Get(COMDT_EQUIPEVAL_PERACNT.CLASS_ID);
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
			if (cutVer == 0u || CSDT_HERO_EQUIPEVAL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_HERO_EQUIPEVAL.CURRVERSION;
			}
			if (CSDT_HERO_EQUIPEVAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroEquipList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEvalInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || CSDT_HERO_EQUIPEVAL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_HERO_EQUIPEVAL.CURRVERSION;
			}
			if (CSDT_HERO_EQUIPEVAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroEquipList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEvalInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_HERO_EQUIPEVAL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroId = 0u;
			if (this.stHeroEquipList != null)
			{
				this.stHeroEquipList.Release();
				this.stHeroEquipList = null;
			}
			if (this.stEvalInfo != null)
			{
				this.stEvalInfo.Release();
				this.stEvalInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stHeroEquipList = (COMDT_HERO_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_HERO_EQUIP_INFO.CLASS_ID);
			this.stEvalInfo = (COMDT_EQUIPEVAL_PERACNT)ProtocolObjectPool.Get(COMDT_EQUIPEVAL_PERACNT.CLASS_ID);
		}
	}
}
