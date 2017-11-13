using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	internal class ActorStaticDataProviderBase : ActorDataProviderBase
	{
		protected delegate bool ActorDataBuilder(ref ActorMeta actorMeta, ref ActorStaticData actorInfo);

		protected delegate bool ActorSkillDataBuilder(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillInfo);

		protected delegate bool ActorPerLvDataBuilder(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData);

		private readonly DictionaryView<uint, ActorStaticDataProviderBase.ActorDataBuilder> _actorDataBuilder = new DictionaryView<uint, ActorStaticDataProviderBase.ActorDataBuilder>();

		private readonly DictionaryView<uint, ActorStaticDataProviderBase.ActorSkillDataBuilder> _actorSkillDataBuilder = new DictionaryView<uint, ActorStaticDataProviderBase.ActorSkillDataBuilder>();

		private readonly DictionaryView<uint, ActorStaticDataProviderBase.ActorPerLvDataBuilder> _actorPerStarLvDataBuilder = new DictionaryView<uint, ActorStaticDataProviderBase.ActorPerLvDataBuilder>();

		public ActorStaticDataProviderBase()
		{
			this._actorDataBuilder.Add(0u, new ActorStaticDataProviderBase.ActorDataBuilder(this.BuildHeroData));
			this._actorDataBuilder.Add(1u, new ActorStaticDataProviderBase.ActorDataBuilder(this.BuildMonsterData));
			this._actorDataBuilder.Add(2u, new ActorStaticDataProviderBase.ActorDataBuilder(this.BuildOrganData));
			this._actorDataBuilder.Add(3u, new ActorStaticDataProviderBase.ActorDataBuilder(this.BuildMonsterData));
			this._actorDataBuilder.Add(4u, new ActorStaticDataProviderBase.ActorDataBuilder(this.BuildCallActorData));
			this._actorSkillDataBuilder.Add(0u, new ActorStaticDataProviderBase.ActorSkillDataBuilder(this.BuildHeroSkillData));
			this._actorSkillDataBuilder.Add(1u, new ActorStaticDataProviderBase.ActorSkillDataBuilder(this.BuildMonsterSkillData));
			this._actorSkillDataBuilder.Add(2u, new ActorStaticDataProviderBase.ActorSkillDataBuilder(this.BuildOrganSkillData));
			this._actorSkillDataBuilder.Add(3u, new ActorStaticDataProviderBase.ActorSkillDataBuilder(this.BuildMonsterSkillData));
			this._actorSkillDataBuilder.Add(4u, new ActorStaticDataProviderBase.ActorSkillDataBuilder(this.BuildHeroSkillData));
			this._actorPerStarLvDataBuilder.Add(0u, new ActorStaticDataProviderBase.ActorPerLvDataBuilder(this.BuildHeroPerStarLvData));
			this._actorPerStarLvDataBuilder.Add(1u, new ActorStaticDataProviderBase.ActorPerLvDataBuilder(this.BuildMonsterPerStarLvData));
			this._actorPerStarLvDataBuilder.Add(2u, new ActorStaticDataProviderBase.ActorPerLvDataBuilder(this.BuildOrganPerStarLvData));
			this._actorPerStarLvDataBuilder.Add(3u, new ActorStaticDataProviderBase.ActorPerLvDataBuilder(this.BuildMonsterPerStarLvData));
		}

		public override bool GetActorStaticData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			actorData.TheActorMeta = actorMeta;
			ActorStaticDataProviderBase.ActorDataBuilder actorDataBuilder = null;
			this._actorDataBuilder.TryGetValue((uint)actorMeta.ActorType, out actorDataBuilder);
			return actorDataBuilder != null && actorDataBuilder(ref actorMeta, ref actorData);
		}

		public override bool GetActorStaticSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
		{
			skillData.TheActorMeta = actorMeta;
			skillData.SkillSlot = skillSlot;
			ActorStaticDataProviderBase.ActorSkillDataBuilder actorSkillDataBuilder = null;
			this._actorSkillDataBuilder.TryGetValue((uint)actorMeta.ActorType, out actorSkillDataBuilder);
			return actorSkillDataBuilder != null && actorSkillDataBuilder(ref actorMeta, skillSlot, ref skillData);
		}

		public override bool GetActorStaticPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
		{
			perStarLvData.TheActorMeta = actorMeta;
			perStarLvData.StarLv = starLv;
			ActorStaticDataProviderBase.ActorPerLvDataBuilder actorPerLvDataBuilder = null;
			this._actorPerStarLvDataBuilder.TryGetValue((uint)actorMeta.ActorType, out actorPerLvDataBuilder);
			return actorPerLvDataBuilder != null && actorPerLvDataBuilder(ref actorMeta, starLv, ref perStarLvData);
		}

		protected virtual bool BuildHeroData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			return false;
		}

		protected virtual bool BuildCallActorData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			return false;
		}

		protected virtual bool BuildMonsterData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			return false;
		}

		protected virtual bool BuildOrganData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			return false;
		}

		protected virtual bool BuildHeroSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)skillData.TheActorMeta.ConfigId);
			if (dataByKey == null)
			{
				base.ErrorMissingHeroConfig((uint)skillData.TheActorMeta.ConfigId);
				return false;
			}
			if (skillSlot >= (ActorSkillSlot)dataByKey.astSkill.Length)
			{
				return false;
			}
			skillData.PassiveSkillId = dataByKey.astSkill[(int)skillSlot].iPassiveSkillID;
			skillData.SkillId = dataByKey.astSkill[(int)skillSlot].iSkillID;
			return skillData.SkillId > 0;
		}

		protected virtual bool BuildMonsterSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
		{
			ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo(skillData.TheActorMeta.ConfigId, (int)skillData.TheActorMeta.Difficuty);
			if (dataCfgInfo == null)
			{
				dataCfgInfo = MonsterDataHelper.GetDataCfgInfo(skillData.TheActorMeta.ConfigId, 1);
			}
			if (dataCfgInfo == null)
			{
				base.ErrorMissingMonsterConfig((uint)skillData.TheActorMeta.ConfigId);
				return false;
			}
			if (skillSlot >= (ActorSkillSlot)dataCfgInfo.SkillIDs.Length)
			{
				return false;
			}
			skillData.SkillId = dataCfgInfo.SkillIDs[(int)skillSlot];
			skillData.PassiveSkillId = dataCfgInfo.PassiveSkillID[(int)skillSlot];
			return skillData.SkillId > 0 || skillData.PassiveSkillId > 0;
		}

		protected virtual bool BuildOrganSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
		{
			ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(skillData.TheActorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				base.ErrorMissingOrganConfig((uint)skillData.TheActorMeta.ConfigId);
				return false;
			}
			if (skillSlot >= (ActorSkillSlot)dataCfgInfoByCurLevelDiff.SkillIDs.Length)
			{
				return false;
			}
			skillData.SkillId = dataCfgInfoByCurLevelDiff.SkillIDs[(int)skillSlot];
			skillData.PassiveSkillId = 0;
			return skillData.SkillId > 0;
		}

		protected virtual bool BuildHeroPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)perStarLvData.TheActorMeta.ConfigId);
			if (dataByKey == null)
			{
				base.ErrorMissingHeroConfig((uint)perStarLvData.TheActorMeta.ConfigId);
				return false;
			}
			perStarLvData.PerLvHp = dataByKey.iHpGrowth;
			perStarLvData.PerLvAd = dataByKey.iAtkGrowth;
			perStarLvData.PerLvAp = dataByKey.iSpellGrowth;
			perStarLvData.PerLvDef = dataByKey.iDefGrowth;
			perStarLvData.PerLvRes = dataByKey.iResistGrowth;
			return true;
		}

		protected virtual bool BuildMonsterPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
		{
			return false;
		}

		protected virtual bool BuildOrganPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
		{
			ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(perStarLvData.TheActorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				base.ErrorMissingOrganConfig((uint)perStarLvData.TheActorMeta.ConfigId);
				return false;
			}
			perStarLvData.PerLvHp = dataCfgInfoByCurLevelDiff.iHPLvlup;
			perStarLvData.PerLvAd = dataCfgInfoByCurLevelDiff.iATTLvlup;
			perStarLvData.PerLvAp = dataCfgInfoByCurLevelDiff.iINTLvlup;
			perStarLvData.PerLvDef = dataCfgInfoByCurLevelDiff.iDEFLvlup;
			perStarLvData.PerLvRes = dataCfgInfoByCurLevelDiff.iRESLvlup;
			return true;
		}
	}
}
