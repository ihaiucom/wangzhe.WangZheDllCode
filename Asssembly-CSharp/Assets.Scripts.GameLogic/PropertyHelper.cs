using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PropertyHelper
	{
		private CrypticInt32 _level = 1;

		private CrypticInt32 _curExp = 0;

		private CrypticInt32 _maxExp = 0;

		private CrypticInt32 _star = 1;

		private CrypticInt32 _quality = 1;

		private CrypticInt32 _subQuality = 0;

		private int _epRecFrequency;

		private CrypticInt32 _soulLevel = 1;

		private EnergyType _energyType = EnergyType.NoneResource;

		private ValueDataInfo[] mActorValue = new ValueDataInfo[37];

		public ActorMeta m_theActorMeta;

		public static int[] s_symbolPropValAddArr = new int[37];

		public int SoulLevel
		{
			get
			{
				return this._soulLevel;
			}
			set
			{
				this._soulLevel = value;
			}
		}

		public EnergyType EnergyType
		{
			get
			{
				return this._energyType;
			}
			set
			{
				this._energyType = value;
			}
		}

		public int actorLvl
		{
			get
			{
				return this._level;
			}
			set
			{
				ResHeroLvlUpInfo dataByKey = GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint)value);
				if (dataByKey != null)
				{
					this._level = value;
					this._maxExp = (int)dataByKey.dwExp;
				}
			}
		}

		public int actorStar
		{
			get
			{
				return this._star;
			}
			set
			{
				this._star = value;
				if (value > 1)
				{
					IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
					this.m_theActorMeta = new ActorMeta
					{
						ConfigId = this.m_theActorMeta.ConfigId
					};
					ActorPerStarLvData actorPerStarLvData = default(ActorPerStarLvData);
					if (actorDataProvider.GetActorStaticPerStarLvData(ref this.m_theActorMeta, (ActorStarLv)value, ref actorPerStarLvData))
					{
						this.mActorValue[5].growValue = actorPerStarLvData.PerLvHp;
						this.mActorValue[1].growValue = actorPerStarLvData.PerLvAd;
						this.mActorValue[2].growValue = actorPerStarLvData.PerLvAp;
						this.mActorValue[3].growValue = actorPerStarLvData.PerLvDef;
						this.mActorValue[4].growValue = actorPerStarLvData.PerLvRes;
					}
				}
			}
		}

		public int actorQuality
		{
			get
			{
				return this._quality;
			}
			set
			{
				this._quality = value;
				this._subQuality = 0;
			}
		}

		public int actorSubQuality
		{
			get
			{
				return this._subQuality;
			}
			set
			{
				this._subQuality = value;
			}
		}

		public int actorMaxExp
		{
			get
			{
				return this._maxExp;
			}
			set
			{
				this._maxExp = value;
			}
		}

		public int actorExp
		{
			get
			{
				return this._curExp;
			}
			set
			{
				this._curExp = value;
			}
		}

		public ValueDataInfo this[RES_FUNCEFT_TYPE key]
		{
			get
			{
				return this.mActorValue[(int)key];
			}
		}

		public PropertyHelper()
		{
			this._level = 1;
			this._curExp = 0;
			this._maxExp = 0;
			this._star = 1;
			this._quality = 1;
			this._subQuality = 0;
			this._soulLevel = 1;
		}

		public void OnHeroSoulLvlUp(PoolObjHandle<ActorRoot> hero, int level)
		{
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			if (hero && hero == captain)
			{
				this.SoulLevel = level;
			}
		}

		public int GenericCalculator(ValueDataInfo vd, int baseValue)
		{
			int num = baseValue + (this.SoulLevel - 1) * vd.growValue / 10000;
			long num2 = (long)(num + vd.addValue - vd.decValue) * (long)(10000 + vd.addRatio - vd.decRatio) / 10000L + (long)vd.addValueOffRatio;
			if (vd.maxLimitValue > 0)
			{
				num2 = ((num2 > (long)vd.maxLimitValue) ? ((long)vd.maxLimitValue) : num2);
			}
			if (vd.minLimitValue != 0)
			{
				num2 = ((num2 < (long)vd.minLimitValue) ? ((long)vd.minLimitValue) : num2);
			}
			return (int)num2;
		}

		public int GenericBaseCalculator(ValueDataInfo vd, int baseValue)
		{
			return baseValue + (this.SoulLevel - 1) * vd.growValue / 10000;
		}

		public int GrowCalculator(ValueDataInfo vd, ValueDataType type)
		{
			if (type == ValueDataType.TYPE_TOTAL)
			{
				return this.GenericCalculator(vd, vd.baseValue);
			}
			return this.GenericBaseCalculator(vd, vd.baseValue);
		}

		public int EpNumericalCalculator(ValueDataInfo vd, int baseValue)
		{
			int num = baseValue + (this.SoulLevel - 1) * vd.growValue;
			long num2 = (long)(num + vd.addValue - vd.decValue) * (long)(10000 + vd.addRatio - vd.decRatio) / 10000L + (long)vd.addValueOffRatio;
			return (int)num2;
		}

		public int EpBaseNumericalCalculator(ValueDataInfo vd, int baseValue)
		{
			return baseValue + (this.SoulLevel - 1) * vd.growValue;
		}

		public int EpProportionCalculator(ValueDataInfo vd, int baseValue)
		{
			int num = baseValue / 10000 + (this.SoulLevel - 1) * vd.growValue / 10000;
			long num2 = (long)(num + vd.addValue - vd.decValue) * (long)(10000 + vd.addRatio - vd.decRatio) / 10000L + (long)vd.addValueOffRatio;
			return (int)num2;
		}

		public int EpBaseProportionCalculator(ValueDataInfo vd, int baseValue)
		{
			return baseValue / 10000 + (this.SoulLevel - 1) * vd.growValue / 10000;
		}

		public int EpGrowCalculator(ValueDataInfo vd, ValueDataType type)
		{
			if (type == ValueDataType.TYPE_TOTAL)
			{
				return this.EpNumericalCalculator(vd, vd.baseValue);
			}
			return this.EpBaseNumericalCalculator(vd, vd.baseValue);
		}

		public int EpRecCalculator(ValueDataInfo vd, ValueDataType type)
		{
			if (type == ValueDataType.TYPE_TOTAL)
			{
				return this.EpProportionCalculator(vd, vd.baseValue);
			}
			return this.EpBaseProportionCalculator(vd, vd.baseValue);
		}

		public int DynamicAdjustor(ValueDataInfo vd, ValueDataType type)
		{
			if (type == ValueDataType.TYPE_TOTAL)
			{
				return this.GenericCalculator(vd, DynamicProperty.Adjustor(vd));
			}
			return this.GenericBaseCalculator(vd, DynamicProperty.Adjustor(vd));
		}

		public int DynamicAdjustorForMgcEffect(ValueDataInfo vd, ValueDataType type)
		{
			if (type == ValueDataType.TYPE_TOTAL)
			{
				int num = this.GenericCalculator(vd, DynamicProperty.Adjustor(vd));
				return num + vd.totalEftValueByMgc;
			}
			return this.GenericBaseCalculator(vd, DynamicProperty.Adjustor(vd));
		}

		public void Init(ref ActorMeta actorMeta)
		{
			this.InitValueDataArr(ref actorMeta, false);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorServerData actorServerData = default(ActorServerData);
			if (actorDataProvider.GetActorServerData(ref actorMeta, ref actorServerData))
			{
				this.actorLvl = (int)actorServerData.Level;
				this.actorExp = (int)actorServerData.Exp;
				this.actorStar = (int)actorServerData.Star;
				this.actorQuality = (int)actorServerData.TheQualityInfo.Quality;
				this.actorSubQuality = (int)actorServerData.TheQualityInfo.SubQuality;
			}
			else
			{
				if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					IGameActorDataProvider actorDataProvider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
					ActorStaticData actorStaticData = default(ActorStaticData);
					this.actorLvl = (actorDataProvider2.GetActorStaticData(ref actorMeta, ref actorStaticData) ? actorStaticData.TheMonsterOnlyInfo.MonsterBaseLevel : 1);
				}
				else
				{
					this.actorLvl = 1;
				}
				this.actorExp = 0;
				this.actorStar = 1;
				this.actorQuality = 1;
				this.actorSubQuality = 0;
			}
		}

		public void Init(COMDT_HEROINFO svrInfo)
		{
			ActorMeta actorMeta = default(ActorMeta);
			ActorMeta actorMeta2 = actorMeta;
			actorMeta2.ConfigId = (int)svrInfo.stCommonInfo.dwHeroID;
			actorMeta = actorMeta2;
			this.InitValueDataArr(ref actorMeta, true);
			this.actorLvl = (int)svrInfo.stCommonInfo.wLevel;
			this.actorExp = (int)svrInfo.stCommonInfo.dwExp;
			this.actorStar = (int)svrInfo.stCommonInfo.wStar;
			this.actorQuality = (int)svrInfo.stCommonInfo.stQuality.wQuality;
			this.actorSubQuality = (int)svrInfo.stCommonInfo.stQuality.wSubQuality;
			this.SetSkinProp(svrInfo.stCommonInfo.dwHeroID, (uint)svrInfo.stCommonInfo.wSkinID, true);
		}

		public void InitValueDataArr(ref ActorMeta theActorMeta, bool bLobby)
		{
			IGameActorDataProvider actorDataProvider;
			if (bLobby)
			{
				actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
			}
			else
			{
				actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			}
			ActorStaticData actorStaticData = default(ActorStaticData);
			actorDataProvider.GetActorStaticData(ref theActorMeta, ref actorStaticData);
			this.m_theActorMeta = theActorMeta;
			this.EnergyType = (EnergyType)actorStaticData.TheBaseAttribute.EpType;
			ResHeroEnergyInfo dataByKey = GameDataMgr.heroEnergyDatabin.GetDataByKey(actorStaticData.TheBaseAttribute.EpType);
			int nMaxLimitValue = (dataByKey != null) ? dataByKey.iEnergyMax : 0;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			bool bPvpMode = true;
			if (curLvelContext != null)
			{
				bPvpMode = (bLobby || curLvelContext.IsMobaMode());
			}
			this.mActorValue[5] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, actorStaticData.TheBaseAttribute.BaseHp, actorStaticData.TheBaseAttribute.PerLvHp, new ValueCalculator(this.DynamicAdjustorForMgcEffect), (int)actorStaticData.TheBaseAttribute.DynamicProperty, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, bPvpMode));
			DebugHelper.Assert(this.mActorValue[5].totalValue > 0, "Initialize maxhp <= 0");
			this.mActorValue[1] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT, actorStaticData.TheBaseAttribute.BaseAd, actorStaticData.TheBaseAttribute.PerLvAd, new ValueCalculator(this.DynamicAdjustorForMgcEffect), (int)actorStaticData.TheBaseAttribute.DynamicProperty, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT, bPvpMode));
			this.mActorValue[2] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT, actorStaticData.TheBaseAttribute.BaseAp, actorStaticData.TheBaseAttribute.PerLvAp, new ValueCalculator(this.DynamicAdjustor), (int)actorStaticData.TheBaseAttribute.DynamicProperty, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT, bPvpMode));
			this.mActorValue[3] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT, actorStaticData.TheBaseAttribute.BaseDef, actorStaticData.TheBaseAttribute.PerLvDef, new ValueCalculator(this.DynamicAdjustor), (int)actorStaticData.TheBaseAttribute.DynamicProperty, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT, bPvpMode));
			this.mActorValue[4] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT, actorStaticData.TheBaseAttribute.BaseRes, actorStaticData.TheBaseAttribute.PerLvRes, new ValueCalculator(this.DynamicAdjustor), (int)actorStaticData.TheBaseAttribute.DynamicProperty, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT, bPvpMode));
			this.mActorValue[32] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP, actorStaticData.TheBaseAttribute.BaseEp, actorStaticData.TheBaseAttribute.EpGrowth, new ValueCalculator(this.EpGrowCalculator), 0, nMaxLimitValue, this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP, bPvpMode));
			this.mActorValue[33] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER, actorStaticData.TheBaseAttribute.BaseEpRecover, actorStaticData.TheBaseAttribute.PerLvEpRecover, new ValueCalculator(this.EpRecCalculator), 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER, bPvpMode));
			this.mActorValue[34] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE, bPvpMode));
			this.mActorValue[35] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_MGCARMORHURT_RATE, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_MGCARMORHURT_RATE, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_MGCARMORHURT_RATE, bPvpMode));
			this.mActorValue[21] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_SightArea, actorStaticData.TheBaseAttribute.Sight, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_SightArea, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_SightArea, bPvpMode));
			this.mActorValue[15] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, actorStaticData.TheBaseAttribute.MoveSpeed, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, bPvpMode));
			this.mActorValue[16] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_HPRECOVER, actorStaticData.TheBaseAttribute.BaseHpRecover, actorStaticData.TheBaseAttribute.PerLvHpRecover, new ValueCalculator(this.GrowCalculator), 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_HPRECOVER, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_HPRECOVER, bPvpMode));
			this.mActorValue[18] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD, actorStaticData.TheBaseAttribute.BaseAtkSpeed, actorStaticData.TheBaseAttribute.PerLvAtkSpeed, new ValueCalculator(this.GrowCalculator), 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD, bPvpMode));
			this.mActorValue[6] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITRATE, actorStaticData.TheBaseAttribute.CriticalChance, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITRATE, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITRATE, bPvpMode));
			this.mActorValue[12] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITEFT, actorStaticData.TheBaseAttribute.CriticalDamage, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITEFT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITEFT, bPvpMode));
			this.mActorValue[11] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_ANTICRIT, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_ANTICRIT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_ANTICRIT, bPvpMode));
			this.mActorValue[22] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_HitRate, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_HitRate, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_HitRate, bPvpMode));
			this.mActorValue[23] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_HitRateAvoid, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_HitRateAvoid, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_HitRateAvoid, bPvpMode));
			this.mActorValue[13] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURT, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURT, bPvpMode));
			this.mActorValue[14] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURTLESS, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURTLESS, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURTLESS, bPvpMode));
			this.mActorValue[7] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYARMORHURT, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYARMORHURT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYARMORHURT, bPvpMode));
			this.mActorValue[8] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCARMORHURT, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCARMORHURT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCARMORHURT, bPvpMode));
			this.mActorValue[19] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_BASEHURTADD, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_BASEHURTADD, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_BASEHURTADD, bPvpMode));
			this.mActorValue[9] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYVAMP, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYVAMP, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYVAMP, bPvpMode));
			this.mActorValue[10] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCVAMP, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCVAMP, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCVAMP, bPvpMode));
			this.mActorValue[17] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_CTRLREDUCE, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CTRLREDUCE, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CTRLREDUCE, bPvpMode));
			this.mActorValue[20] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_FUNCEFT_CDREDUCE, 0, 0, null, 0, this.GetPropMaxValueLimitCdReduce(bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_FUNCEFT_CDREDUCE, bPvpMode));
			this.mActorValue[24] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_CRITICAL, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_CRITICAL, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_CRITICAL, bPvpMode));
			this.mActorValue[25] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_REDUCECRITICAL, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_REDUCECRITICAL, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_REDUCECRITICAL, bPvpMode));
			this.mActorValue[26] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_PHYSICSHEM, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_PHYSICSHEM, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_PHYSICSHEM, bPvpMode));
			this.mActorValue[27] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_MAGICHEM, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_MAGICHEM, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_MAGICHEM, bPvpMode));
			this.mActorValue[28] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_ATTACKSPEED, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_ATTACKSPEED, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_ATTACKSPEED, bPvpMode));
			this.mActorValue[29] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_TENACITY, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_TENACITY, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_TENACITY, bPvpMode));
			this.mActorValue[30] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE, bPvpMode));
			this.mActorValue[31] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE, bPvpMode));
			this.mActorValue[36] = new ValueDataInfo(RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT, 0, 0, null, 0, this.GetPropMaxValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT, bPvpMode), this.GetPropMinValueLimit(RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT, bPvpMode));
		}

		private int GetPropMaxValueLimitCdReduce(bool bPvpMode)
		{
			if (bPvpMode)
			{
				int result = 0;
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.m_cooldownReduceUpperLimit > 0u)
				{
					result = (int)curLvelContext.m_cooldownReduceUpperLimit;
				}
				else
				{
					ResPropertyValueInfo dataByKey = GameDataMgr.propertyValInfo.GetDataByKey(20u);
					if (dataByKey != null)
					{
						result = dataByKey.iMaxLimitValue;
					}
				}
				return result;
			}
			return 0;
		}

		private int GetPropMaxValueLimit(RES_FUNCEFT_TYPE funcType, bool bPvpMode)
		{
			if (bPvpMode)
			{
				ResPropertyValueInfo dataByKey = GameDataMgr.propertyValInfo.GetDataByKey((uint)((byte)funcType));
				return (dataByKey != null) ? dataByKey.iMaxLimitValue : 0;
			}
			return 0;
		}

		private int GetPropMinValueLimit(RES_FUNCEFT_TYPE funcType, bool bPvpMode)
		{
			ResPropertyValueInfo dataByKey = GameDataMgr.propertyValInfo.GetDataByKey((uint)((byte)funcType));
			return (dataByKey != null) ? dataByKey.iMinLimitValue : 0;
		}

		public void SetChangeEvent(RES_FUNCEFT_TYPE key, ValueChangeDelegate func)
		{
			if (this.mActorValue[(int)key] != null)
			{
				this.mActorValue[(int)key].ChangeEvent += func;
				func();
			}
		}

		public void ChangeFuncEft(RES_FUNCEFT_TYPE key, RES_VALUE_TYPE type, int val, bool bOffRatio = false)
		{
			if (this.mActorValue[(int)key] != null)
			{
				ValueDataInfo.ChangeValueData(ref this.mActorValue[(int)key], type, val, bOffRatio);
			}
		}

		public void SetSkinProp(uint heroId, uint skinId, bool bWear)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			DebugHelper.Assert(heroSkin != null, "Skin==null");
			for (int i = 0; i < 15; i++)
			{
				ushort wType = heroSkin.astAttr[i].wType;
				byte bValType = heroSkin.astAttr[i].bValType;
				int iValue = heroSkin.astAttr[i].iValue;
				if (wType != 0 && iValue != 0)
				{
					if (bWear)
					{
						this.ChangeFuncEft((RES_FUNCEFT_TYPE)wType, (RES_VALUE_TYPE)bValType, iValue, true);
					}
					else
					{
						this.ChangeFuncEft((RES_FUNCEFT_TYPE)wType, (RES_VALUE_TYPE)bValType, -iValue, true);
					}
				}
			}
		}

		public void AddSymbolPageAttToProp(ref ActorMeta meta, bool bPVPLevel)
		{
			for (int i = 0; i < 37; i++)
			{
				PropertyHelper.s_symbolPropValAddArr[i] = 0;
			}
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.GetInstance().GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorServerRuneData actorServerRuneData = default(ActorServerRuneData);
			for (int j = 0; j < 30; j++)
			{
				if (actorDataProvider.GetActorServerRuneData(ref meta, (ActorRunelSlot)j, ref actorServerRuneData))
				{
					ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(actorServerRuneData.RuneId);
					if (dataByKey != null)
					{
						if (bPVPLevel)
						{
							for (int k = 0; k < dataByKey.astFuncEftList.Length; k++)
							{
								int wType = (int)dataByKey.astFuncEftList[k].wType;
								int bValType = (int)dataByKey.astFuncEftList[k].bValType;
								int iValue = dataByKey.astFuncEftList[k].iValue;
								if (wType != 0 && wType < 37 && iValue != 0)
								{
									if (bValType == 0)
									{
										PropertyHelper.s_symbolPropValAddArr[wType] += iValue;
									}
									else if (bValType == 1)
									{
										this.ChangeFuncEft((RES_FUNCEFT_TYPE)wType, (RES_VALUE_TYPE)bValType, iValue, true);
									}
								}
							}
						}
						else
						{
							for (int l = 0; l < dataByKey.astPveEftList.Length; l++)
							{
								int wType2 = (int)dataByKey.astPveEftList[l].wType;
								int bValType2 = (int)dataByKey.astPveEftList[l].bValType;
								int iValue2 = dataByKey.astPveEftList[l].iValue;
								if (wType2 != 0 && wType2 < 37 && iValue2 != 0)
								{
									if (bValType2 == 0)
									{
										PropertyHelper.s_symbolPropValAddArr[wType2] += iValue2;
									}
									else if (bValType2 == 1)
									{
										this.ChangeFuncEft((RES_FUNCEFT_TYPE)wType2, (RES_VALUE_TYPE)bValType2, iValue2, true);
									}
								}
							}
						}
					}
				}
			}
			for (int m = 0; m < 37; m++)
			{
				int num = PropertyHelper.s_symbolPropValAddArr[m] / 100;
				if (num > 0)
				{
					this.ChangeFuncEft((RES_FUNCEFT_TYPE)m, RES_VALUE_TYPE.TYPE_VALUE, num, true);
				}
			}
		}

		public ValueDataInfo[] GetActorValue()
		{
			return this.mActorValue;
		}
	}
}
