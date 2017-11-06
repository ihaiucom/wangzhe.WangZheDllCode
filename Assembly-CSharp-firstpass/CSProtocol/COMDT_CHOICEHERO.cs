using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_CHOICEHERO : ProtocolObject
	{
		public COMDT_HEROINFO stBaseInfo;

		public uint[] SymbolID;

		public COMDT_HERO_BURNING_INFO stBurningInfo;

		public COMDT_HEROEXTRALINFO stHeroExtral;

		public uint[] HeroEquipList;

		public COMDT_HERO_EQUIPLIST_NEW stNewHeroEquipList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_HeroEquipList = 81u;

		public static readonly uint VERSION_stNewHeroEquipList = 207u;

		public static readonly int CLASS_ID = 129;

		public COMDT_CHOICEHERO()
		{
			this.stBaseInfo = (COMDT_HEROINFO)ProtocolObjectPool.Get(COMDT_HEROINFO.CLASS_ID);
			this.SymbolID = new uint[30];
			this.stBurningInfo = (COMDT_HERO_BURNING_INFO)ProtocolObjectPool.Get(COMDT_HERO_BURNING_INFO.CLASS_ID);
			this.stHeroExtral = (COMDT_HEROEXTRALINFO)ProtocolObjectPool.Get(COMDT_HEROEXTRALINFO.CLASS_ID);
			this.HeroEquipList = new uint[6];
			this.stNewHeroEquipList = (COMDT_HERO_EQUIPLIST_NEW)ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST_NEW.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stBaseInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBurningInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroExtral.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNewHeroEquipList.construct();
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
			if (cutVer == 0u || COMDT_CHOICEHERO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CHOICEHERO.CURRVERSION;
			}
			if (COMDT_CHOICEHERO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBaseInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 30; i++)
			{
				errorType = destBuf.writeUInt32(this.SymbolID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stBurningInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroExtral.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_CHOICEHERO.VERSION_HeroEquipList <= cutVer)
			{
				for (int j = 0; j < 6; j++)
				{
					errorType = destBuf.writeUInt32(this.HeroEquipList[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_CHOICEHERO.VERSION_stNewHeroEquipList <= cutVer)
			{
				errorType = this.stNewHeroEquipList.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_CHOICEHERO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CHOICEHERO.CURRVERSION;
			}
			if (COMDT_CHOICEHERO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBaseInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 30; i++)
			{
				errorType = srcBuf.readUInt32(ref this.SymbolID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stBurningInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroExtral.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_CHOICEHERO.VERSION_HeroEquipList <= cutVer)
			{
				for (int j = 0; j < 6; j++)
				{
					errorType = srcBuf.readUInt32(ref this.HeroEquipList[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_CHOICEHERO.VERSION_stNewHeroEquipList <= cutVer)
			{
				errorType = this.stNewHeroEquipList.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stNewHeroEquipList.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_CHOICEHERO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stBaseInfo != null)
			{
				this.stBaseInfo.Release();
				this.stBaseInfo = null;
			}
			if (this.stBurningInfo != null)
			{
				this.stBurningInfo.Release();
				this.stBurningInfo = null;
			}
			if (this.stHeroExtral != null)
			{
				this.stHeroExtral.Release();
				this.stHeroExtral = null;
			}
			if (this.stNewHeroEquipList != null)
			{
				this.stNewHeroEquipList.Release();
				this.stNewHeroEquipList = null;
			}
		}

		public override void OnUse()
		{
			this.stBaseInfo = (COMDT_HEROINFO)ProtocolObjectPool.Get(COMDT_HEROINFO.CLASS_ID);
			this.stBurningInfo = (COMDT_HERO_BURNING_INFO)ProtocolObjectPool.Get(COMDT_HERO_BURNING_INFO.CLASS_ID);
			this.stHeroExtral = (COMDT_HEROEXTRALINFO)ProtocolObjectPool.Get(COMDT_HEROEXTRALINFO.CLASS_ID);
			this.stNewHeroEquipList = (COMDT_HERO_EQUIPLIST_NEW)ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST_NEW.CLASS_ID);
		}
	}
}
