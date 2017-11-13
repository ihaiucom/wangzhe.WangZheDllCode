using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.DistancePassiveEvent)]
	public class PassiveDistanceEvent : PassiveEvent
	{
		private int curDistance;

		private int _curTriggerCount;

		private bool bDashingState;

		private int triggerDistance;

		private int maxTriggerCount;

		private int minMoveSpeed;

		private int layerEffectID;

		private int groupEffectID;

		private int curTriggerCount
		{
			get
			{
				return this._curTriggerCount;
			}
			set
			{
				if (value >= 0 && value <= this.maxTriggerCount)
				{
					this._curTriggerCount = value;
					if (this.sourceActor && this.sourceActor.handle.ValueComponent != null && this.sourceActor.handle.ValueComponent.IsEnergyType(EnergyType.SpeedResource))
					{
						this.sourceActor.handle.ValueComponent.actorEp = this._curTriggerCount;
					}
				}
			}
		}

		public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			base.Init(_actor, _skill);
			this.curDistance = 0;
			this.curTriggerCount = 0;
			this.bDashingState = false;
			this.triggerDistance = this.localParams[0];
			this.maxTriggerCount = this.localParams[1];
			this.minMoveSpeed = this.localParams[2];
			this.layerEffectID = this.localParams[3];
			this.groupEffectID = this.localParams[4];
		}

		private bool IsReduceSpeedState()
		{
			return this.sourceActor.handle.BuffHolderComp != null && this.sourceActor.handle.BuffHolderComp.IsExistSkillFuncType(7);
		}

		private bool IsLastMove()
		{
			return this.sourceActor.handle.MovementComponent.isExcuteMoving && !this.sourceActor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move) && (!this.IsReduceSpeedState() || this.sourceActor.handle.ValueComponent.actorMoveSpeed >= this.minMoveSpeed);
		}

		private void UpdateLastMove(int _delta)
		{
			if (!this.sourceActor)
			{
				return;
			}
			if (!this.bDashingState)
			{
				if (this.IsLastMove())
				{
					this.triggerDistance = this.localParams[0];
					int actorMoveSpeed = this.sourceActor.handle.ValueComponent.actorMoveSpeed;
					int num = _delta * actorMoveSpeed / 1000;
					this.curDistance += num;
					if (this.curDistance >= this.triggerDistance && this.curTriggerCount < this.maxTriggerCount)
					{
						this.curTriggerCount++;
						this.curDistance -= this.triggerDistance;
						if (this.curTriggerCount < this.maxTriggerCount)
						{
							this.SpawnSkillEffect(this.layerEffectID);
						}
						else if (this.curTriggerCount == this.maxTriggerCount)
						{
							this.SpawnSkillEffect(this.layerEffectID);
							this.bDashingState = true;
							base.Reset();
							base.Trigger();
						}
					}
				}
				else
				{
					this.AbortLastMove();
				}
			}
			else if (!this.IsLastMove() || base.Fit())
			{
				this.AbortLastMove();
				this.AbortDashingState();
			}
		}

		private void AbortDashingState()
		{
			if (this.bDashingState)
			{
				this.bDashingState = false;
				if (this.passiveSkill != null && this.passiveSkill.CurAction)
				{
					this.passiveSkill.CurAction.handle.Stop(false);
				}
				this.RemoveSkillEffectGroup(this.groupEffectID);
			}
		}

		private void AbortLastMove()
		{
			if (this.curTriggerCount > 0)
			{
				this.curDistance = 0;
				this.curTriggerCount = 0;
				this.RemoveSkillEffect(this.layerEffectID);
			}
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

		private void RemoveSkillEffectGroup(int _groupID)
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.BuffHolderComp.RemoveSkillEffectGroup(_groupID);
			}
		}

		private void RemoveSkillEffect(int _skillCombineID)
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.BuffHolderComp.RemoveBuff(_skillCombineID);
			}
		}

		public override void UpdateLogic(int _delta)
		{
			base.UpdateLogic(_delta);
			this.UpdateLastMove(_delta);
		}
	}
}
