using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public struct STriggerCondition
	{
		[FriendlyName("受害者是玩家队长")]
		public bool bCaptainSrc;

		[FriendlyName("肇事者是玩家队长")]
		public bool bCaptainAtker;

		public STriggerCondActor[] SrcActorCond;

		public STriggerCondActor[] AtkerActorCond;

		[FriendlyName("使用百分比")]
		public bool bPercent;

		[FriendlyName("百分比数")]
		public int Percent;

		[FriendlyName("技能槽位")]
		public int skillSlot;

		[FriendlyName("天赋等级")]
		public int TalentLevel;

		[FriendlyName("难度筛选")]
		public int Difficulty;

		[FriendlyName("金币数目")]
		public int GoldNum;

		[FriendlyName("全局变量")]
		public int GlobalVariable;

		private bool CheckDifficulty()
		{
			if (this.Difficulty == 0)
			{
				return true;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			return curLvelContext.m_levelDifficulty >= this.Difficulty;
		}

		public bool FilterMatch(EGlobalGameEvent inEventType, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ref SFilterMatchParam param, CTriggerMatch match, int inMatchIndex)
		{
			if (!this.CheckDifficulty())
			{
				return false;
			}
			if (this.GlobalVariable != 0 && Singleton<BattleLogic>.instance.m_globalTrigger != null && this.GlobalVariable != Singleton<BattleLogic>.instance.m_globalTrigger.CurGlobalVariable)
			{
				return false;
			}
			if (this.bCaptainSrc && (!src || src != Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain))
			{
				return false;
			}
			if (this.bCaptainAtker && (!atker || atker != Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain))
			{
				return false;
			}
			if (this.SrcActorCond != null)
			{
				STriggerCondActor[] srcActorCond = this.SrcActorCond;
				for (int i = 0; i < srcActorCond.Length; i++)
				{
					STriggerCondActor sTriggerCondActor = srcActorCond[i];
					if (!sTriggerCondActor.FilterMatch(ref src))
					{
						return false;
					}
				}
			}
			if (this.AtkerActorCond != null)
			{
				STriggerCondActor[] atkerActorCond = this.AtkerActorCond;
				for (int j = 0; j < atkerActorCond.Length; j++)
				{
					STriggerCondActor sTriggerCondActor2 = atkerActorCond[j];
					if (!sTriggerCondActor2.FilterMatch(ref atker))
					{
						return false;
					}
				}
			}
			switch (inEventType)
			{
			case EGlobalGameEvent.SpawnGroupDead:
				if (match.Originator != null)
				{
					CommonSpawnGroup component = match.Originator.GetComponent<CommonSpawnGroup>();
					SpawnGroup component2 = match.Originator.GetComponent<SpawnGroup>();
					if (component != null && component != param.csg)
					{
						return false;
					}
					if (component2 != null && component2 != param.sg)
					{
						return false;
					}
				}
				break;
			case EGlobalGameEvent.ActorDead:
			{
				PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(match.Originator);
				if (actorRoot && actorRoot != src)
				{
					return false;
				}
				break;
			}
			case EGlobalGameEvent.ActorDamage:
				if (!this.FilterMatchDamage(ref param.hurtInfo))
				{
					return false;
				}
				break;
			case EGlobalGameEvent.UseSkill:
				if (param.slot != (SkillSlotType)this.skillSlot)
				{
					return false;
				}
				break;
			case EGlobalGameEvent.TalentLevelChange:
				if (match.Condition.TalentLevel != param.intParam)
				{
					return false;
				}
				break;
			case EGlobalGameEvent.BattleGoldChange:
			{
				if (!src || !ActorHelper.IsHostCtrlActor(ref src))
				{
					return false;
				}
				int intParam = param.intParam;
				if (!this.FilterBattleGoldNum(intParam))
				{
					return false;
				}
				break;
			}
			case EGlobalGameEvent.SkillUseCanceled:
				if (param.slot != (SkillSlotType)this.skillSlot)
				{
					return false;
				}
				break;
			}
			return true;
		}

		private bool FilterMatchDamage(ref HurtEventResultInfo inHurtInfo)
		{
			if (!this.bPercent)
			{
				return true;
			}
			int actorHp = inHurtInfo.src.handle.ValueComponent.actorHp;
			int hpChanged = inHurtInfo.hpChanged;
			int num = actorHp - hpChanged;
			int num2 = this.Percent;
			if (num2 < 0)
			{
				num2 = 0;
			}
			else if (num2 > 100)
			{
				num2 = 100;
			}
			int num3 = inHurtInfo.src.handle.ValueComponent.actorHpTotal * num2 / 100;
			return num > actorHp && num >= num3 && actorHp <= num3;
		}

		public bool FilterBattleGoldNum(int currentGold)
		{
			return currentGold >= this.GoldNum;
		}
	}
}
