using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ForbidAbilityDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int attackerId;

		public bool forbidMove;

		public bool forbidMoveRotate;

		public bool delaySkillAbort;

		public bool ImmuneNegative;

		public bool ImmuneControl;

		public bool forbidEnergyRecover;

		public bool forbidSkill;

		public bool forceForbidding;

		public bool forbidFilterSkill0;

		public bool forbidFilterSkill1;

		public bool forbidFilterSkill2;

		public bool forbidFilterSkill3;

		public bool forbidFilterSkill4;

		public bool forbidFilterSkill5;

		public bool forbidFilterSkill6;

		public bool forbidFilterSkill7;

		public bool forbidFilterSkill9;

		public bool forbidSkillAbort = true;

		public bool abortFilterSkill0;

		public bool abortFilterSkill1;

		public bool abortFilterSkill2;

		public bool abortFilterSkill3;

		public bool abortFilterSkill4;

		public bool abortFilterSkill5;

		public bool abortFilterSkill7;

		public bool abortFilterSkill9;

		public bool abortFilterMove;

		public bool abortFilterDamage;

		public bool forbidCollisionDetection;

		private PoolObjHandle<ActorRoot> actor_;

		public override void OnUse()
		{
			base.OnUse();
			this.attackerId = 0;
			this.forbidMove = false;
			this.forbidMoveRotate = false;
			this.delaySkillAbort = false;
			this.ImmuneNegative = false;
			this.ImmuneControl = false;
			this.forbidEnergyRecover = false;
			this.forbidSkill = false;
			this.forceForbidding = false;
			this.forbidFilterSkill0 = false;
			this.forbidFilterSkill1 = false;
			this.forbidFilterSkill2 = false;
			this.forbidFilterSkill3 = false;
			this.forbidFilterSkill4 = false;
			this.forbidFilterSkill5 = false;
			this.forbidFilterSkill6 = false;
			this.forbidFilterSkill7 = false;
			this.forbidFilterSkill9 = false;
			this.forbidSkillAbort = true;
			this.abortFilterSkill0 = false;
			this.abortFilterSkill1 = false;
			this.abortFilterSkill2 = false;
			this.abortFilterSkill3 = false;
			this.abortFilterSkill4 = false;
			this.abortFilterSkill5 = false;
			this.abortFilterSkill7 = false;
			this.abortFilterSkill9 = false;
			this.abortFilterMove = false;
			this.abortFilterDamage = false;
			this.forbidCollisionDetection = false;
			this.actor_.Release();
		}

		public override BaseEvent Clone()
		{
			ForbidAbilityDuration forbidAbilityDuration = ClassObjPool<ForbidAbilityDuration>.Get();
			forbidAbilityDuration.CopyData(this);
			return forbidAbilityDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ForbidAbilityDuration forbidAbilityDuration = src as ForbidAbilityDuration;
			this.attackerId = forbidAbilityDuration.attackerId;
			this.forbidMove = forbidAbilityDuration.forbidMove;
			this.forbidMoveRotate = forbidAbilityDuration.forbidMoveRotate;
			this.delaySkillAbort = forbidAbilityDuration.delaySkillAbort;
			this.ImmuneNegative = forbidAbilityDuration.ImmuneNegative;
			this.ImmuneControl = forbidAbilityDuration.ImmuneControl;
			this.forbidEnergyRecover = forbidAbilityDuration.forbidEnergyRecover;
			this.forbidSkill = forbidAbilityDuration.forbidSkill;
			this.forceForbidding = forbidAbilityDuration.forceForbidding;
			this.forbidFilterSkill0 = forbidAbilityDuration.forbidFilterSkill0;
			this.forbidFilterSkill1 = forbidAbilityDuration.forbidFilterSkill1;
			this.forbidFilterSkill2 = forbidAbilityDuration.forbidFilterSkill2;
			this.forbidFilterSkill3 = forbidAbilityDuration.forbidFilterSkill3;
			this.forbidFilterSkill4 = forbidAbilityDuration.forbidFilterSkill4;
			this.forbidFilterSkill5 = forbidAbilityDuration.forbidFilterSkill5;
			this.forbidFilterSkill6 = forbidAbilityDuration.forbidFilterSkill6;
			this.forbidFilterSkill7 = forbidAbilityDuration.forbidFilterSkill7;
			this.forbidFilterSkill9 = forbidAbilityDuration.forbidFilterSkill9;
			this.forbidSkillAbort = forbidAbilityDuration.forbidSkillAbort;
			this.abortFilterSkill0 = forbidAbilityDuration.abortFilterSkill0;
			this.abortFilterSkill1 = forbidAbilityDuration.abortFilterSkill1;
			this.abortFilterSkill2 = forbidAbilityDuration.abortFilterSkill2;
			this.abortFilterSkill3 = forbidAbilityDuration.abortFilterSkill3;
			this.abortFilterSkill4 = forbidAbilityDuration.abortFilterSkill4;
			this.abortFilterSkill5 = forbidAbilityDuration.abortFilterSkill5;
			this.abortFilterSkill7 = forbidAbilityDuration.abortFilterSkill7;
			this.abortFilterSkill9 = forbidAbilityDuration.abortFilterSkill9;
			this.abortFilterMove = forbidAbilityDuration.abortFilterMove;
			this.abortFilterDamage = forbidAbilityDuration.abortFilterDamage;
			this.forbidCollisionDetection = forbidAbilityDuration.forbidCollisionDetection;
			this.actor_ = forbidAbilityDuration.actor_;
		}

		private void ClearForbidSkill()
		{
			if (this.forbidSkill)
			{
				if (!this.forbidFilterSkill0)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_0, this.forceForbidding);
				}
				if (!this.forbidFilterSkill1)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_1, this.forceForbidding);
				}
				if (!this.forbidFilterSkill2)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_2, this.forceForbidding);
				}
				if (!this.forbidFilterSkill3)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_3, this.forceForbidding);
				}
				if (!this.forbidFilterSkill4)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_4, this.forceForbidding);
				}
				if (!this.forbidFilterSkill5)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_5, this.forceForbidding);
				}
				if (!this.forbidFilterSkill6)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_6, this.forceForbidding);
				}
				if (!this.forbidFilterSkill7)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_7, this.forceForbidding);
				}
				if (!this.forbidFilterSkill9)
				{
					this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_9, this.forceForbidding);
				}
			}
		}

		private void ForbidSkill()
		{
			if (this.forbidSkill)
			{
				if (!this.forbidFilterSkill0)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_0, this.forceForbidding);
				}
				if (!this.forbidFilterSkill1)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_1, this.forceForbidding);
				}
				if (!this.forbidFilterSkill2)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_2, this.forceForbidding);
				}
				if (!this.forbidFilterSkill3)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_3, this.forceForbidding);
				}
				if (!this.forbidFilterSkill4)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_4, this.forceForbidding);
				}
				if (!this.forbidFilterSkill5)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_5, this.forceForbidding);
				}
				if (!this.forbidFilterSkill6)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_6, this.forceForbidding);
				}
				if (!this.forbidFilterSkill7)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_7, this.forceForbidding);
				}
				if (!this.forbidFilterSkill9)
				{
					this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_9, this.forceForbidding);
				}
			}
		}

		private void ForbidSkillAbort()
		{
			if (this.forbidSkillAbort)
			{
				if (!this.actor_)
				{
					return;
				}
				if (this.actor_.handle.SkillControl.CurUseSkill != null)
				{
					this.actor_.handle.SkillControl.CurUseSkill.skillAbort.InitAbort(false);
					if (this.abortFilterSkill0)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_0);
					}
					if (this.abortFilterSkill1)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_1);
					}
					if (this.abortFilterSkill2)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_2);
					}
					if (this.abortFilterSkill3)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_3);
					}
					if (this.abortFilterSkill4)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_4);
					}
					if (this.abortFilterSkill5)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_5);
					}
					if (this.abortFilterSkill7)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_7);
					}
					if (this.abortFilterSkill9)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_9);
					}
					if (this.abortFilterMove)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_MOVE);
					}
					if (this.abortFilterDamage)
					{
						this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_DAMAGE);
					}
				}
			}
		}

		private void ImmuneAbility(bool bRemove)
		{
			if (this.ImmuneNegative)
			{
				if (!bRemove)
				{
					this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
				}
				else
				{
					this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
				}
			}
			if (this.ImmuneControl)
			{
				if (!this.actor_)
				{
					return;
				}
				if (!bRemove)
				{
					this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
				}
				else
				{
					this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
				}
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			this.actor_ = _action.GetActorHandle(this.attackerId);
			if (this.actor_)
			{
				if (this.forbidMove)
				{
					this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
				}
				if (this.delaySkillAbort && this.actor_.handle.SkillControl.CurUseSkill != null)
				{
					this.actor_.handle.SkillControl.CurUseSkill.bProtectAbortSkill = true;
				}
				if (this.forbidMoveRotate)
				{
					this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
				}
				if (this.forbidEnergyRecover)
				{
					this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_RecoverEnergy);
				}
				if (this.forbidCollisionDetection)
				{
					this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbiliity_CollisionDetection);
				}
				this.ForbidSkill();
				this.ForbidSkillAbort();
				this.ImmuneAbility(false);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.actor_)
			{
				if (this.forbidMove)
				{
					this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
					if (!this.actor_.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
					{
						DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actor_, this.actor_);
						Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref defaultGameEventParam);
					}
				}
				if (this.forbidSkillAbort && this.actor_.handle.SkillControl.CurUseSkill != null)
				{
					this.actor_.handle.SkillControl.CurUseSkill.skillAbort.InitAbort(true);
				}
				if (this.forbidMoveRotate)
				{
					this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
				}
				if (this.delaySkillAbort && this.actor_.handle.SkillControl.CurUseSkill != null)
				{
					this.actor_.handle.SkillControl.CurUseSkill.bProtectAbortSkill = false;
					if (this.actor_.handle.SkillControl.CurUseSkill.bDelayAbortSkill)
					{
						this.actor_.handle.SkillControl.CurUseSkill.bDelayAbortSkill = false;
						this.actor_.handle.SkillControl.ForceAbortCurUseSkill();
					}
				}
				if (this.forbidEnergyRecover)
				{
					this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_RecoverEnergy);
				}
				if (this.forbidCollisionDetection)
				{
					this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbiliity_CollisionDetection);
				}
				this.ImmuneAbility(true);
				this.ClearForbidSkill();
			}
		}
	}
}
