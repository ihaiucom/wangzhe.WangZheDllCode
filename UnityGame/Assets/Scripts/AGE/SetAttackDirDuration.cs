using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetAttackDirDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int attackerId;

		private bool done_;

		private int rotTime_;

		private bool bRotate;

		private Quaternion fromRot = Quaternion.identity;

		private Quaternion toRot = Quaternion.identity;

		private PoolObjHandle<ActorRoot> actor_;

		public override void OnUse()
		{
			base.OnUse();
			this.attackerId = 0;
			this.done_ = false;
			this.rotTime_ = 0;
			this.bRotate = false;
			this.fromRot = Quaternion.identity;
			this.toRot = Quaternion.identity;
			this.actor_.Release();
		}

		public override BaseEvent Clone()
		{
			SetAttackDirDuration setAttackDirDuration = ClassObjPool<SetAttackDirDuration>.Get();
			setAttackDirDuration.CopyData(this);
			return setAttackDirDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetAttackDirDuration setAttackDirDuration = src as SetAttackDirDuration;
			this.attackerId = setAttackDirDuration.attackerId;
			this.done_ = setAttackDirDuration.done_;
			this.rotTime_ = setAttackDirDuration.rotTime_;
			this.fromRot = setAttackDirDuration.fromRot;
			this.toRot = setAttackDirDuration.toRot;
			this.actor_.Release();
		}

		private void Init(Action _action, Track _track)
		{
			this.actor_ = _action.GetActorHandle(this.attackerId);
			if (!this.actor_)
			{
				this.done_ = true;
				return;
			}
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject == null)
			{
				this.done_ = true;
				return;
			}
			VInt3 vInt;
			if (!refParamObject.CalcAttackerDir(out vInt, this.actor_))
			{
				this.done_ = true;
				return;
			}
			if (vInt == VInt3.zero)
			{
				return;
			}
			if (this.actor_.handle.MovementComponent == null)
			{
				return;
			}
			this.bRotate = true;
			this.actor_.handle.MovementComponent.SetRotate(vInt, true);
			this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
			this.fromRot = this.actor_.handle.rotation;
			this.toRot = Quaternion.LookRotation((Vector3)vInt);
			if (this.length <= 30)
			{
				this.actor_.handle.rotation = this.toRot;
				this.done_ = true;
				return;
			}
			float num = Quaternion.Angle(this.fromRot, this.toRot);
			if (num > 180.1f)
			{
				DebugHelper.Assert(num <= 180.1f);
			}
			this.rotTime_ = Mathf.FloorToInt(num * (float)this.length / 180f);
			DebugHelper.Assert(this.rotTime_ <= this.length);
		}

		public override void Enter(Action _action, Track _track)
		{
			this.done_ = false;
			this.Init(_action, _track);
			base.Enter(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.done_ || !this.actor_)
			{
				return;
			}
			if (_localTime >= this.rotTime_)
			{
				this.actor_.handle.rotation = this.toRot;
				this.done_ = true;
			}
			else
			{
				this.actor_.handle.rotation = Quaternion.Slerp(this.fromRot, this.toRot, (float)_localTime / (float)this.rotTime_);
			}
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.actor_ && !this.done_)
			{
				this.actor_.handle.rotation = this.toRot;
			}
			if (this.actor_ && this.bRotate)
			{
				this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
			}
			this.done_ = true;
			base.Leave(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.done_;
		}
	}
}
