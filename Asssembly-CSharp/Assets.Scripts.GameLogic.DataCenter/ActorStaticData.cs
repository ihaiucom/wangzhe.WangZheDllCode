using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorStaticData
	{
		public struct BaseAttribute
		{
			public int Sight;

			public int MoveSpeed;

			public int BaseAtkSpeed;

			public int PerLvAtkSpeed;

			public int BaseHp;

			public int PerLvHp;

			public int BaseHpRecover;

			public int PerLvHpRecover;

			public uint EpType;

			public int BaseEp;

			public int EpGrowth;

			public int BaseEpRecover;

			public int PerLvEpRecover;

			public int BaseAd;

			public int PerLvAd;

			public int BaseAp;

			public int PerLvAp;

			public int BaseDef;

			public int PerLvDef;

			public int BaseRes;

			public int PerLvRes;

			public int CriticalChance;

			public int CriticalDamage;

			public int SoulExpGained;

			public int GoldCoinInBattleGained;

			public int GoldCoinInBattleGainedFloatRange;

			public uint DynamicProperty;

			public uint ClashMark;

			public int RandomPassiveSkillRule;

			public int PassiveSkillID1;

			public int PassiveSkillID2;

			public bool DeadControl;
		}

		public struct HeroOnlyInfo
		{
			public int HeroCapability;

			public int HeroAttackType;

			public int HeroDamageType;

			public int InitialStar;

			public int RecommendStandPos;

			public int AttackDistanceType;

			public string HeroNamePinYin;
		}

		public struct MonsterOnlyInfo
		{
			public int Reserved;

			public int MonsterBaseLevel;

			public byte SoldierType;
		}

		public struct OrganOnlyInfo
		{
			public int OrganType;

			public bool ShowInMinimap;

			public int PhyArmorHurtRate;

			public int AttackRouteID;

			public int DeadEnemySoldier;

			public int NoEnemyAddPhyDef;

			public int NoEnemyAddMgcDef;

			public int HorizonRadius;
		}

		public struct ResInfo
		{
			public string Name;

			public string ResPath;
		}

		public ActorMeta TheActorMeta;

		public ActorStaticData.BaseAttribute TheBaseAttribute;

		public ActorStaticData.HeroOnlyInfo TheHeroOnlyInfo;

		public ActorStaticData.MonsterOnlyInfo TheMonsterOnlyInfo;

		public ActorStaticData.OrganOnlyInfo TheOrganOnlyInfo;

		public ActorStaticData.ResInfo TheResInfo;

		public GameActorDataProviderType ProviderType;
	}
}
