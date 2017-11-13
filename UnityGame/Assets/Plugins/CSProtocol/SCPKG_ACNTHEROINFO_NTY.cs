using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_ACNTHEROINFO_NTY : ProtocolObject
	{
		public COMDT_HEROCTRLINFO stHeroCtrlInfo;

		public COMDT_HERO_SKIN_LIST stSkinInfo;

		public COMDT_BATTLELIST_LIST stBattleListInfo;

		public COMDT_HEROINFO_LIST stHeroInfo;

		public COMDT_HERO_LIMIT_SKIN_LIST stLimitSkinInfo;

		public COMDT_SELFDEFINE_EQUIP_INFO stSelfDefineEquipInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly int CLASS_ID = 881;

		public SCPKG_ACNTHEROINFO_NTY()
		{
			this.stHeroCtrlInfo = (COMDT_HEROCTRLINFO)ProtocolObjectPool.Get(COMDT_HEROCTRLINFO.CLASS_ID);
			this.stSkinInfo = (COMDT_HERO_SKIN_LIST)ProtocolObjectPool.Get(COMDT_HERO_SKIN_LIST.CLASS_ID);
			this.stBattleListInfo = (COMDT_BATTLELIST_LIST)ProtocolObjectPool.Get(COMDT_BATTLELIST_LIST.CLASS_ID);
			this.stHeroInfo = (COMDT_HEROINFO_LIST)ProtocolObjectPool.Get(COMDT_HEROINFO_LIST.CLASS_ID);
			this.stLimitSkinInfo = (COMDT_HERO_LIMIT_SKIN_LIST)ProtocolObjectPool.Get(COMDT_HERO_LIMIT_SKIN_LIST.CLASS_ID);
			this.stSelfDefineEquipInfo = (COMDT_SELFDEFINE_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_SELFDEFINE_EQUIP_INFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stHeroCtrlInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSkinInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleListInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLimitSkinInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSelfDefineEquipInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || SCPKG_ACNTHEROINFO_NTY.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_ACNTHEROINFO_NTY.CURRVERSION;
			}
			if (SCPKG_ACNTHEROINFO_NTY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stHeroCtrlInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSkinInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleListInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLimitSkinInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSelfDefineEquipInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_ACNTHEROINFO_NTY.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_ACNTHEROINFO_NTY.CURRVERSION;
			}
			if (SCPKG_ACNTHEROINFO_NTY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stHeroCtrlInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSkinInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleListInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLimitSkinInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSelfDefineEquipInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_ACNTHEROINFO_NTY.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stHeroCtrlInfo != null)
			{
				this.stHeroCtrlInfo.Release();
				this.stHeroCtrlInfo = null;
			}
			if (this.stSkinInfo != null)
			{
				this.stSkinInfo.Release();
				this.stSkinInfo = null;
			}
			if (this.stBattleListInfo != null)
			{
				this.stBattleListInfo.Release();
				this.stBattleListInfo = null;
			}
			if (this.stHeroInfo != null)
			{
				this.stHeroInfo.Release();
				this.stHeroInfo = null;
			}
			if (this.stLimitSkinInfo != null)
			{
				this.stLimitSkinInfo.Release();
				this.stLimitSkinInfo = null;
			}
			if (this.stSelfDefineEquipInfo != null)
			{
				this.stSelfDefineEquipInfo.Release();
				this.stSelfDefineEquipInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stHeroCtrlInfo = (COMDT_HEROCTRLINFO)ProtocolObjectPool.Get(COMDT_HEROCTRLINFO.CLASS_ID);
			this.stSkinInfo = (COMDT_HERO_SKIN_LIST)ProtocolObjectPool.Get(COMDT_HERO_SKIN_LIST.CLASS_ID);
			this.stBattleListInfo = (COMDT_BATTLELIST_LIST)ProtocolObjectPool.Get(COMDT_BATTLELIST_LIST.CLASS_ID);
			this.stHeroInfo = (COMDT_HEROINFO_LIST)ProtocolObjectPool.Get(COMDT_HEROINFO_LIST.CLASS_ID);
			this.stLimitSkinInfo = (COMDT_HERO_LIMIT_SKIN_LIST)ProtocolObjectPool.Get(COMDT_HERO_LIMIT_SKIN_LIST.CLASS_ID);
			this.stSelfDefineEquipInfo = (COMDT_SELFDEFINE_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_SELFDEFINE_EQUIP_INFO.CLASS_ID);
		}
	}
}
