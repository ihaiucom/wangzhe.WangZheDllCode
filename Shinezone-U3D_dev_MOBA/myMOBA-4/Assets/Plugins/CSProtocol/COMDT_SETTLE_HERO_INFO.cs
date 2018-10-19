using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SETTLE_HERO_INFO : ProtocolObject
	{
		public uint dwHeroConfID;

		public uint dwBloodTTH;

		public uint dwGhostLevel;

		public COMDT_SETTLE_TALENT_INFO[] astTalentDetail;

		public COMDT_HERO_BASE_INFO stHeroDetailInfo;

		public COMDT_HERO_BATTLE_STATISTIC_INFO stHeroBattleInfo;

		public COMDT_SKILL_STATISTIC_INFO[] astSkillStatisticInfo;

		public byte bInBattleEquipNum;

		public COMDT_SETTLE_INBATTLE_EQUIP_INFO[] astInBattleEquipInfo;

		public byte bUsedEquipIndex;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 163;

		public COMDT_SETTLE_HERO_INFO()
		{
			this.astTalentDetail = new COMDT_SETTLE_TALENT_INFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astTalentDetail[i] = (COMDT_SETTLE_TALENT_INFO)ProtocolObjectPool.Get(COMDT_SETTLE_TALENT_INFO.CLASS_ID);
			}
			this.stHeroDetailInfo = (COMDT_HERO_BASE_INFO)ProtocolObjectPool.Get(COMDT_HERO_BASE_INFO.CLASS_ID);
			this.stHeroBattleInfo = (COMDT_HERO_BATTLE_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_BATTLE_STATISTIC_INFO.CLASS_ID);
			this.astSkillStatisticInfo = new COMDT_SKILL_STATISTIC_INFO[5];
			for (int j = 0; j < 5; j++)
			{
				this.astSkillStatisticInfo[j] = (COMDT_SKILL_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_SKILL_STATISTIC_INFO.CLASS_ID);
			}
			this.astInBattleEquipInfo = new COMDT_SETTLE_INBATTLE_EQUIP_INFO[30];
			for (int k = 0; k < 30; k++)
			{
				this.astInBattleEquipInfo[k] = (COMDT_SETTLE_INBATTLE_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_SETTLE_INBATTLE_EQUIP_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SETTLE_HERO_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_HERO_INFO.CURRVERSION;
			}
			if (COMDT_SETTLE_HERO_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroConfID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwBloodTTH);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGhostLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astTalentDetail[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stHeroDetailInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroBattleInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astSkillStatisticInfo[j].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bInBattleEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.bInBattleEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astInBattleEquipInfo.Length < (int)this.bInBattleEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int k = 0; k < (int)this.bInBattleEquipNum; k++)
			{
				errorType = this.astInBattleEquipInfo[k].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bUsedEquipIndex);
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
			if (cutVer == 0u || COMDT_SETTLE_HERO_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_HERO_INFO.CURRVERSION;
			}
			if (COMDT_SETTLE_HERO_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroConfID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBloodTTH);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGhostLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astTalentDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stHeroDetailInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroBattleInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astSkillStatisticInfo[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bInBattleEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.bInBattleEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int k = 0; k < (int)this.bInBattleEquipNum; k++)
			{
				errorType = this.astInBattleEquipInfo[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bUsedEquipIndex);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SETTLE_HERO_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroConfID = 0u;
			this.dwBloodTTH = 0u;
			this.dwGhostLevel = 0u;
			if (this.astTalentDetail != null)
			{
				for (int i = 0; i < this.astTalentDetail.Length; i++)
				{
					if (this.astTalentDetail[i] != null)
					{
						this.astTalentDetail[i].Release();
						this.astTalentDetail[i] = null;
					}
				}
			}
			if (this.stHeroDetailInfo != null)
			{
				this.stHeroDetailInfo.Release();
				this.stHeroDetailInfo = null;
			}
			if (this.stHeroBattleInfo != null)
			{
				this.stHeroBattleInfo.Release();
				this.stHeroBattleInfo = null;
			}
			if (this.astSkillStatisticInfo != null)
			{
				for (int j = 0; j < this.astSkillStatisticInfo.Length; j++)
				{
					if (this.astSkillStatisticInfo[j] != null)
					{
						this.astSkillStatisticInfo[j].Release();
						this.astSkillStatisticInfo[j] = null;
					}
				}
			}
			this.bInBattleEquipNum = 0;
			if (this.astInBattleEquipInfo != null)
			{
				for (int k = 0; k < this.astInBattleEquipInfo.Length; k++)
				{
					if (this.astInBattleEquipInfo[k] != null)
					{
						this.astInBattleEquipInfo[k].Release();
						this.astInBattleEquipInfo[k] = null;
					}
				}
			}
			this.bUsedEquipIndex = 0;
		}

		public override void OnUse()
		{
			if (this.astTalentDetail != null)
			{
				for (int i = 0; i < this.astTalentDetail.Length; i++)
				{
					this.astTalentDetail[i] = (COMDT_SETTLE_TALENT_INFO)ProtocolObjectPool.Get(COMDT_SETTLE_TALENT_INFO.CLASS_ID);
				}
			}
			this.stHeroDetailInfo = (COMDT_HERO_BASE_INFO)ProtocolObjectPool.Get(COMDT_HERO_BASE_INFO.CLASS_ID);
			this.stHeroBattleInfo = (COMDT_HERO_BATTLE_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_BATTLE_STATISTIC_INFO.CLASS_ID);
			if (this.astSkillStatisticInfo != null)
			{
				for (int j = 0; j < this.astSkillStatisticInfo.Length; j++)
				{
					this.astSkillStatisticInfo[j] = (COMDT_SKILL_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_SKILL_STATISTIC_INFO.CLASS_ID);
				}
			}
			if (this.astInBattleEquipInfo != null)
			{
				for (int k = 0; k < this.astInBattleEquipInfo.Length; k++)
				{
					this.astInBattleEquipInfo[k] = (COMDT_SETTLE_INBATTLE_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_SETTLE_INBATTLE_EQUIP_INFO.CLASS_ID);
				}
			}
		}
	}
}
