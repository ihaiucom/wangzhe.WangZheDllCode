using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class Skill : BaseSkill
	{
		public ResSkillCfgInfo cfgData;

		public SkillRangeAppointType AppointType;

		public float SkillCD;

		public int SkillCost;

		public string GuidePrefabName;

		public string GuideWarnPrefabName;

		public string EffectPrefabName;

		public string EffectWarnPrefabName;

		public string FixedPrefabName;

		public string FixedWarnPrefabName;

		public string IconName;

		public SkillAbort skillAbort;

		public bool bProtectAbortSkill;

		public bool bDelayAbortSkill;

		private ResBattleParam battleParam;

		public SkillSlotType SlotType
		{
			get;
			set;
		}

		public bool bExposing
		{
			get
			{
				return this.cfgData.bSkillNonExposing == 0;
			}
		}

		public Skill(int id)
		{
			this.SkillID = id;
			this.skillAbort = new SkillAbort();
			this.skillAbort.InitAbort(false);
			this.bDelayAbortSkill = false;
			this.bProtectAbortSkill = false;
			this.cfgData = GameDataMgr.skillDatabin.GetDataByKey((long)id);
			if (this.cfgData != null)
			{
				this.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szPrefab);
				DebugHelper.Assert(this.ActionName != null, "Action name is null in skill databin id = {0}", new object[]
				{
					id
				});
				this.GuidePrefabName = StringHelper.UTF8BytesToString(ref this.cfgData.szGuidePrefab);
				this.GuideWarnPrefabName = StringHelper.UTF8BytesToString(ref this.cfgData.szGuideWarnPrefab);
				this.EffectPrefabName = StringHelper.UTF8BytesToString(ref this.cfgData.szEffectPrefab);
				this.EffectWarnPrefabName = StringHelper.UTF8BytesToString(ref this.cfgData.szEffectWarnPrefab);
				this.FixedPrefabName = StringHelper.UTF8BytesToString(ref this.cfgData.szFixedPrefab);
				this.FixedWarnPrefabName = StringHelper.UTF8BytesToString(ref this.cfgData.szFixedWarnPrefab);
				this.IconName = StringHelper.UTF8BytesToString(ref this.cfgData.szIconPath);
				this.SkillCD = 5f;
				this.AppointType = (SkillRangeAppointType)this.cfgData.bRangeAppointType;
				this.bAgeImmeExcute = (this.cfgData.bAgeImmeExcute == 1);
			}
			this.battleParam = GameDataMgr.battleParam.GetAnyData();
		}

		public bool canAbort(SkillAbortType _type)
		{
			return this.skillAbort.Abort(_type);
		}

		private void SetSkillSpeed(PoolObjHandle<ActorRoot> _user)
		{
			int num = 0;
			if (this.curAction)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = _user;
				MonsterWrapper monsterWrapper = _user.handle.ActorControl as MonsterWrapper;
				if (monsterWrapper != null && monsterWrapper.isCalledMonster && monsterWrapper.UseHostValueProperty)
				{
					poolObjHandle = monsterWrapper.hostActor;
				}
				ValueDataInfo valueDataInfo = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_ATTACKSPEED];
				int totalValue = valueDataInfo.totalValue;
				int num2 = totalValue + poolObjHandle.handle.ValueComponent.mActorValue.actorLvl * (int)this.battleParam.dwM_AttackSpeed + (int)this.battleParam.dwN_AttackSpeed;
				if (num2 != 0)
				{
					num = totalValue * 10000 / num2;
				}
				num += poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD].totalValue;
				if (this.cfgData != null && this.cfgData.bNoInfluenceAnim == 1)
				{
					num = 0;
				}
				VFactor playSpeed = new VFactor((long)(10000 + num), 10000L);
				this.curAction.handle.SetPlaySpeed(playSpeed);
			}
		}

		public override bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
		{
			param.SetOriginator(user);
			param.Instigator = user;
			this.skillAbort.InitAbort(false);
			this.bDelayAbortSkill = false;
			this.bProtectAbortSkill = false;
			param.bExposing = this.bExposing;
			if (base.Use(user, ref param))
			{
				if (param.SlotType == SkillSlotType.SLOT_SKILL_0)
				{
					this.SetSkillSpeed(user);
				}
				return true;
			}
			return false;
		}

		public int SkillEnergyCost(PoolObjHandle<ActorRoot> Actor, int CurSkillLevel)
		{
			if (this.cfgData == null)
			{
				return 0;
			}
			if (this.cfgData.bEnergyCostType != 1)
			{
				if (this.cfgData.bEnergyCostCalcType == 0)
				{
					return this.cfgData.iEnergyCost + (CurSkillLevel - 1) * this.cfgData.iEnergyCostGrowth;
				}
				if (this.cfgData.bEnergyCostCalcType == 1)
				{
					int actorEpTotal = Actor.handle.ValueComponent.actorEpTotal;
					long num = (long)(this.cfgData.iEnergyCost + (CurSkillLevel - 1) * this.cfgData.iEnergyCostGrowth) * (long)actorEpTotal / 10000L;
					return (int)num;
				}
				if (this.cfgData.bEnergyCostCalcType == 2)
				{
					int actorEp = Actor.handle.ValueComponent.actorEp;
					long num2 = (long)(this.cfgData.iEnergyCost + (CurSkillLevel - 1) * this.cfgData.iEnergyCostGrowth) * (long)actorEp / 10000L;
					return (int)num2;
				}
			}
			return 0;
		}

		public int GetMaxSearchDistance(int level = 0)
		{
			if (this.cfgData.iMaxSearchDistanceGrowthValue != 0)
			{
				int num = level - 1;
				if (num < 0)
				{
					num = 0;
				}
				return this.cfgData.iMaxSearchDistance + this.cfgData.iMaxSearchDistanceGrowthValue * num;
			}
			return this.cfgData.iMaxSearchDistance;
		}
	}
}
