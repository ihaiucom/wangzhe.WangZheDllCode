using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Pathfinding.RVO;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class FreezeActorDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int freezeHeight;

		private float recordSpeed;

		private PoolObjHandle<ActorRoot> actorObj;

		private RVOController rovController;

		private string curAnimName;

		public bool forbidMove = true;

		public bool IsFreeze = true;

		public bool forbidSkill = true;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.freezeHeight = 0;
			this.recordSpeed = 0f;
			this.actorObj.Release();
			this.curAnimName = null;
			this.rovController = null;
			this.forbidMove = true;
			this.IsFreeze = true;
			this.forbidSkill = true;
		}

		public override BaseEvent Clone()
		{
			FreezeActorDuration freezeActorDuration = ClassObjPool<FreezeActorDuration>.Get();
			freezeActorDuration.CopyData(this);
			return freezeActorDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			FreezeActorDuration freezeActorDuration = src as FreezeActorDuration;
			this.targetId = freezeActorDuration.targetId;
			this.freezeHeight = freezeActorDuration.freezeHeight;
			this.actorObj = freezeActorDuration.actorObj;
			this.curAnimName = freezeActorDuration.curAnimName;
			this.rovController = freezeActorDuration.rovController;
			this.forbidMove = freezeActorDuration.forbidMove;
			this.forbidSkill = freezeActorDuration.forbidSkill;
			this.IsFreeze = freezeActorDuration.IsFreeze;
		}

		private void PauseAnimation()
		{
			AnimPlayComponent animControl = this.actorObj.handle.AnimControl;
			if (animControl == null)
			{
				return;
			}
			this.curAnimName = animControl.GetCurAnimName();
			if (this.actorObj.handle.ActorMesh == null || this.actorObj.handle.ActorMeshAnimation == null)
			{
				return;
			}
			AnimationState animationState = this.actorObj.handle.ActorMeshAnimation[this.curAnimName];
			if (animationState != null)
			{
				this.recordSpeed = animationState.speed;
				animationState.speed = 0f;
			}
			animControl.bPausePlay = true;
		}

		private void RecoverAnimation()
		{
			AnimPlayComponent animControl = this.actorObj.handle.AnimControl;
			if (this.actorObj.handle.ActorMesh == null || this.actorObj.handle.ActorMeshAnimation == null)
			{
				return;
			}
			AnimationState animationState = this.actorObj.handle.ActorMeshAnimation[this.curAnimName];
			if (animationState != null)
			{
				animationState.speed = this.recordSpeed;
			}
			if (animControl != null)
			{
				animControl.bPausePlay = false;
				animControl.UpdatePlay();
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			if (!this.actorObj)
			{
				return;
			}
			ObjWrapper actorControl = this.actorObj.handle.ActorControl;
			if (actorControl == null)
			{
				return;
			}
			this.PauseAnimation();
			if (this.forbidMove)
			{
				actorControl.TerminateMove();
				actorControl.ClearMoveCommand();
				this.actorObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			}
			if (this.IsFreeze)
			{
				this.actorObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Freeze);
			}
			if (this.forbidSkill)
			{
				actorControl.ForceAbortCurUseSkill();
				this.actorObj.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_COUNT, false);
			}
			if (this.freezeHeight > 0 && this.actorObj.handle.isMovable)
			{
				VInt vInt = 0;
				VInt3 location = this.actorObj.handle.location;
				PathfindingUtility.GetGroundY(location, out vInt);
				location.y = vInt.i + this.freezeHeight;
				this.actorObj.handle.location = location;
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (this.actorObj)
			{
				this.RecoverAnimation();
				if (this.forbidMove)
				{
					this.actorObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
				}
				if (this.IsFreeze)
				{
					this.actorObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Freeze);
				}
				if (this.forbidSkill)
				{
					this.actorObj.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_COUNT, false);
				}
				if (this.freezeHeight > 0 && this.actorObj.handle.isMovable)
				{
					VInt vInt = 0;
					VInt3 location = this.actorObj.handle.location;
					location.y -= this.freezeHeight;
					PathfindingUtility.GetGroundY(location, out vInt);
					if (location.y < vInt.i)
					{
						location.y = vInt.i;
					}
					this.actorObj.handle.location = location;
				}
			}
		}
	}
}
