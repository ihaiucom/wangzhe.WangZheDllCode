using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_STATISTIC_INFO : ProtocolObject
	{
		public byte bGameType;

		public byte bMapAcntNum;

		public uint dwWinNum;

		public uint dwLoseNum;

		public ulong ullKDAPct;

		public ulong ullTotalHurt;

		public ulong ullTotalHurtHero;

		public ulong ullTotalHurtOrgan;

		public ulong ullTotalBeHurt;

		public ulong ullTotalBeHurtHero;

		public uint dwGPM;

		public uint dwXPM;

		public uint dwKillMonster;

		public uint dwKillSoldier;

		public uint dwKillLittleDragon;

		public uint dwKillBigDragon;

		public uint dwDestroyTower;

		public uint dwDoubleKill;

		public uint dwTripleKill;

		public uint dwUltraKill;

		public uint dwRampage;

		public uint dwGodLike;

		public uint dwMvp;

		public uint dwLoseSoul;

		public uint dwKill;

		public uint dwCampKill;

		public uint dwDead;

		public uint dwAssist;

		public ulong ullInGameCoin;

		public uint dwMinutes;

		public uint dwHurtPM;

		public uint dwBattleRatioPct;

		public uint dwFirstBloodNum;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_dwKill = 147u;

		public static readonly uint VERSION_dwCampKill = 147u;

		public static readonly uint VERSION_dwDead = 147u;

		public static readonly uint VERSION_dwAssist = 147u;

		public static readonly uint VERSION_ullInGameCoin = 147u;

		public static readonly uint VERSION_dwMinutes = 152u;

		public static readonly uint VERSION_dwHurtPM = 152u;

		public static readonly uint VERSION_dwBattleRatioPct = 160u;

		public static readonly uint VERSION_dwFirstBloodNum = 208u;

		public static readonly int CLASS_ID = 109;

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
			if (cutVer == 0u || COMDT_HERO_STATISTIC_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_STATISTIC_INFO.CURRVERSION;
			}
			if (COMDT_HERO_STATISTIC_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bGameType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMapAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwWinNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLoseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullKDAPct);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullTotalHurt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullTotalHurtHero);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullTotalHurtOrgan);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullTotalBeHurt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullTotalBeHurtHero);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGPM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwXPM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwKillMonster);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwKillSoldier);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwKillLittleDragon);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwKillBigDragon);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDestroyTower);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDoubleKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTripleKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwUltraKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRampage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGodLike);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMvp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLoseSoul);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwKill <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwKill);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwCampKill <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwCampKill);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwDead <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDead);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwAssist <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwAssist);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_ullInGameCoin <= cutVer)
			{
				errorType = destBuf.writeUInt64(this.ullInGameCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwMinutes <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMinutes);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwHurtPM <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwHurtPM);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwBattleRatioPct <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwBattleRatioPct);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwFirstBloodNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwFirstBloodNum);
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
			if (cutVer == 0u || COMDT_HERO_STATISTIC_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_STATISTIC_INFO.CURRVERSION;
			}
			if (COMDT_HERO_STATISTIC_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bGameType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMapAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWinNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLoseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullKDAPct);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullTotalHurt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullTotalHurtHero);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullTotalHurtOrgan);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullTotalBeHurt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullTotalBeHurtHero);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGPM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwXPM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwKillMonster);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwKillSoldier);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwKillLittleDragon);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwKillBigDragon);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDestroyTower);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDoubleKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTripleKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUltraKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRampage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGodLike);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMvp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLoseSoul);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwKill <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwKill);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwKill = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwCampKill <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwCampKill);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwCampKill = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwDead <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDead);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDead = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwAssist <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwAssist);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwAssist = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_ullInGameCoin <= cutVer)
			{
				errorType = srcBuf.readUInt64(ref this.ullInGameCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.ullInGameCoin = 0uL;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwMinutes <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMinutes);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMinutes = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwHurtPM <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwHurtPM);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwHurtPM = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwBattleRatioPct <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwBattleRatioPct);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwBattleRatioPct = 0u;
			}
			if (COMDT_HERO_STATISTIC_INFO.VERSION_dwFirstBloodNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwFirstBloodNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwFirstBloodNum = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_HERO_STATISTIC_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bGameType = 0;
			this.bMapAcntNum = 0;
			this.dwWinNum = 0u;
			this.dwLoseNum = 0u;
			this.ullKDAPct = 0uL;
			this.ullTotalHurt = 0uL;
			this.ullTotalHurtHero = 0uL;
			this.ullTotalHurtOrgan = 0uL;
			this.ullTotalBeHurt = 0uL;
			this.ullTotalBeHurtHero = 0uL;
			this.dwGPM = 0u;
			this.dwXPM = 0u;
			this.dwKillMonster = 0u;
			this.dwKillSoldier = 0u;
			this.dwKillLittleDragon = 0u;
			this.dwKillBigDragon = 0u;
			this.dwDestroyTower = 0u;
			this.dwDoubleKill = 0u;
			this.dwTripleKill = 0u;
			this.dwUltraKill = 0u;
			this.dwRampage = 0u;
			this.dwGodLike = 0u;
			this.dwMvp = 0u;
			this.dwLoseSoul = 0u;
			this.dwKill = 0u;
			this.dwCampKill = 0u;
			this.dwDead = 0u;
			this.dwAssist = 0u;
			this.ullInGameCoin = 0uL;
			this.dwMinutes = 0u;
			this.dwHurtPM = 0u;
			this.dwBattleRatioPct = 0u;
			this.dwFirstBloodNum = 0u;
		}
	}
}
