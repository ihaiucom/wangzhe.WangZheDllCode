using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.DisFromActorTOMonsterPassiveEvent)]
	public class DisFromActorTOMonsterPassiveEvent : PassiveEvent
	{
		private int[] distanceThreshold = new int[2];

		protected int[] skillCombineID = new int[3];

		private int lastInterval;

		private int curInterval;

		private bool intervalSwitch;

		private PoolObjHandle<ActorRoot> callMonster;

		public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			base.Init(_actor, _skill);
			this.distanceThreshold[0] = this.localParams[0];
			this.distanceThreshold[1] = this.localParams[1];
			this.skillCombineID[0] = this.localParams[2];
			this.skillCombineID[1] = this.localParams[3];
			this.skillCombineID[2] = this.localParams[4];
			this.lastInterval = -1;
			this.curInterval = 0;
			this.intervalSwitch = false;
			this.GetCallMonster();
		}

		private bool GetCallMonster()
		{
			if (this.sourceActor && this.sourceActor.handle.ActorControl is HeroWrapper)
			{
				HeroWrapper heroWrapper = this.sourceActor.handle.ActorControl as HeroWrapper;
				if (heroWrapper.hasCalledMonster)
				{
					this.callMonster = heroWrapper.CallMonster;
					return true;
				}
			}
			return false;
		}

		private void SpawnSkillEffect(int _skillCombineID)
		{
			if (this.sourceActor)
			{
				SkillUseParam skillUseParam = default(SkillUseParam);
				skillUseParam.Init();
				skillUseParam.SetOriginator(this.sourceActor);
				this.sourceActor.handle.SkillControl.SpawnBuff(this.sourceActor, ref skillUseParam, _skillCombineID, true);
			}
		}

		private void RemoveSkillEffect(int _skillCombineID)
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.BuffHolderComp.RemoveBuff(_skillCombineID);
			}
		}

		private void checkDistanceChange()
		{
			this.curInterval = 2;
			int magnitude2D = (this.sourceActor.handle.location - this.callMonster.handle.location).magnitude2D;
			for (int i = 0; i < this.distanceThreshold.Length; i++)
			{
				if (magnitude2D < this.distanceThreshold[i])
				{
					this.curInterval = i;
					break;
				}
			}
		}

		public override void UpdateLogic(int _delta)
		{
			base.UpdateLogic(_delta);
			if (this.deltaTime <= 0)
			{
				if (this.callMonster || this.GetCallMonster())
				{
					this.checkDistanceChange();
					if (this.curInterval != this.lastInterval)
					{
						if (this.lastInterval >= 0)
						{
							this.RemoveSkillEffect(this.skillCombineID[this.lastInterval]);
						}
						this.lastInterval = this.curInterval;
						this.intervalSwitch = true;
					}
					if (this.intervalSwitch && this.callMonster.handle.ActorControl.myBehavior == ObjBehaviMode.State_Idle && ((this.curInterval == 0 && this.callMonster.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move)) || (this.curInterval == 1 && !this.callMonster.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move)) || this.curInterval == 2))
					{
						this.SpawnSkillEffect(this.skillCombineID[this.curInterval]);
						this.intervalSwitch = false;
					}
				}
				this.deltaTime = this.cfgData.iCoolDown;
			}
		}
	}
}
