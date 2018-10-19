using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class RotateActorDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int rotateSpeed;

		public int RotateToId;

		public bool bUseRotateToActor;

		public bool bRotateToCallActor;

		private int lastTime;

		private PoolObjHandle<ActorRoot> actorTarget;

		private VInt3 destDir = VInt3.zero;

		private bool bNeedRotate;

		private int curRotateSpd;

		private PoolObjHandle<ActorRoot> actorRotateTo;

		public override BaseEvent Clone()
		{
			RotateActorDuration rotateActorDuration = ClassObjPool<RotateActorDuration>.Get();
			rotateActorDuration.CopyData(this);
			return rotateActorDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			RotateActorDuration rotateActorDuration = src as RotateActorDuration;
			this.targetId = rotateActorDuration.targetId;
			this.rotateSpeed = rotateActorDuration.rotateSpeed;
			this.lastTime = rotateActorDuration.lastTime;
			this.actorTarget = rotateActorDuration.actorTarget;
			this.destDir = rotateActorDuration.destDir;
			this.bNeedRotate = rotateActorDuration.bNeedRotate;
			this.curRotateSpd = rotateActorDuration.curRotateSpd;
			this.RotateToId = rotateActorDuration.RotateToId;
			this.bRotateToCallActor = rotateActorDuration.bRotateToCallActor;
			this.bUseRotateToActor = rotateActorDuration.bUseRotateToActor;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.rotateSpeed = 0;
			this.lastTime = 0;
			this.actorTarget.Release();
			this.destDir = VInt3.zero;
			this.bNeedRotate = false;
			this.curRotateSpd = 0;
			this.RotateToId = 0;
			this.bRotateToCallActor = false;
			this.bUseRotateToActor = false;
			this.actorRotateTo.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorTarget = _action.GetActorHandle(this.targetId);
			this.lastTime = 0;
			if (!this.actorTarget)
			{
				return;
			}
			this.bNeedRotate = false;
			this.curRotateSpd = 0;
			if (!this.bUseRotateToActor)
			{
				this.actorTarget.handle.ObjLinker.AddCustomRotateLerp(new CustomRotateLerpFunc(this.ActionRotateLerp));
			}
			this.actorRotateTo = _action.GetActorHandle(this.RotateToId);
			if (this.bRotateToCallActor && this.actorRotateTo && this.actorRotateTo.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				HeroWrapper heroWrapper = this.actorRotateTo.handle.ActorControl as HeroWrapper;
				if (heroWrapper != null)
				{
					this.actorRotateTo = heroWrapper.GetCallActor();
				}
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (!this.actorTarget)
			{
				return;
			}
			int num = _localTime - this.lastTime;
			this.lastTime = _localTime;
			if (!this.bUseRotateToActor)
			{
				if (this.actorTarget.handle.ActorControl.curMoveCommand != null)
				{
					FrameCommand<MoveDirectionCommand> frameCommand = (FrameCommand<MoveDirectionCommand>)this.actorTarget.handle.ActorControl.curMoveCommand;
					VInt3 vInt = this.actorTarget.handle.forward;
					this.destDir = VInt3.right.RotateY((int)frameCommand.cmdData.Degree);
					if (this.destDir != vInt)
					{
						this.bNeedRotate = true;
						this.curRotateSpd = this.rotateSpeed;
						int num2 = this.destDir.x * vInt.z - vInt.x * this.destDir.z;
						if (num2 == 0)
						{
							int num3 = VInt3.Dot(this.destDir, vInt);
							if (num3 >= 0)
							{
								return;
							}
							this.curRotateSpd = this.rotateSpeed;
						}
						else if (num2 < 0)
						{
							this.curRotateSpd = -this.rotateSpeed;
						}
						VFactor a = VInt3.AngleInt(this.destDir, vInt);
						VFactor b = VFactor.pi * (long)num * (long)this.curRotateSpd / 180L / 1000L;
						if (a <= b)
						{
							vInt = vInt.RotateY(ref a);
							this.bNeedRotate = false;
						}
						else
						{
							vInt = vInt.RotateY(ref b);
						}
						this.actorTarget.handle.MovementComponent.SetRotate(vInt, true);
					}
				}
				else
				{
					this.destDir = this.actorTarget.handle.forward;
					this.bNeedRotate = false;
					this.curRotateSpd = 0;
				}
			}
			else
			{
				if (!this.actorRotateTo)
				{
					return;
				}
				if (this.actorTarget.handle.MovementComponent != null)
				{
					VInt3 inDirection = this.actorRotateTo.handle.location - this.actorTarget.handle.location;
					this.actorTarget.handle.MovementComponent.SetRotate(inDirection, true);
				}
			}
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.actorTarget)
			{
				return;
			}
			if (!this.bUseRotateToActor)
			{
				this.actorTarget.handle.ObjLinker.RmvCustomRotateLerp(new CustomRotateLerpFunc(this.ActionRotateLerp));
			}
			this.curRotateSpd = 0;
			this.bNeedRotate = false;
		}

		public void ActionRotateLerp(ActorRoot actor, uint nDelta)
		{
			if (actor == null || !this.bNeedRotate || this.curRotateSpd == 0)
			{
				return;
			}
			int degree = (int)(nDelta * (uint)this.curRotateSpd * 10u / 1000u);
			Quaternion to = Quaternion.LookRotation((Vector3)actor.forward.RotateY(degree));
			actor.myTransform.rotation = Quaternion.RotateTowards(actor.myTransform.rotation, to, (float)((long)Mathf.Abs(this.curRotateSpd) * (long)((ulong)nDelta)) * 0.001f);
		}
	}
}
