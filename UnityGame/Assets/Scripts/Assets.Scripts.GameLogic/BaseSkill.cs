using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public abstract class BaseSkill : PooledClassObject
	{
		public int SkillID;

		public string ActionName = string.Empty;

		protected PoolObjHandle<Action> curAction = default(PoolObjHandle<Action>);

		public SkillUseContext skillContext = new SkillUseContext();

		public bool bAgeImmeExcute;

		private ActionStopDelegate OnActionStopDelegate;

		public bool isFinish
		{
			get
			{
				return !this.curAction;
			}
		}

		public PoolObjHandle<Action> CurAction
		{
			get
			{
				return this.curAction;
			}
		}

		public virtual bool isBullet
		{
			get
			{
				return false;
			}
		}

		public virtual bool isBuff
		{
			get
			{
				return false;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.SkillID = 0;
			this.ActionName = string.Empty;
			this.curAction.Release();
			this.skillContext.Reset();
			this.bAgeImmeExcute = false;
			this.OnActionStopDelegate = new ActionStopDelegate(this.OnActionStoped);
		}

		public override void OnRelease()
		{
			this.SkillID = 0;
			this.ActionName = string.Empty;
			this.curAction.Release();
			this.OnActionStopDelegate = null;
			base.OnRelease();
		}

		public virtual void Stop()
		{
			if (this.curAction)
			{
				this.curAction.handle.Stop(false);
				this.curAction.Release();
			}
		}

		private bool UseImpl(PoolObjHandle<ActorRoot> user)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			VInt3 value = VInt3.forward;
			switch (this.skillContext.AppointType)
			{
			case SkillRangeAppointType.Auto:
			case SkillRangeAppointType.Target:
				flag = true;
				break;
			case SkillRangeAppointType.Pos:
				flag2 = true;
				break;
			case SkillRangeAppointType.Directional:
				flag3 = true;
				value = this.skillContext.UseVector;
				if (this.skillContext.TargetID != 0u)
				{
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.skillContext.TargetID);
					if (actor)
					{
						VInt3 vInt = actor.handle.location - user.handle.location;
						vInt.y = 0;
						vInt.Normalize();
						value = vInt;
					}
				}
				break;
			case SkillRangeAppointType.Track:
				flag2 = true;
				flag3 = true;
				value = this.skillContext.EndVector - this.skillContext.UseVector;
				if (value.sqrMagnitudeLong < 1L)
				{
					value = VInt3.forward;
				}
				break;
			}
			if (flag && !this.skillContext.TargetActor)
			{
				return false;
			}
			if (flag)
			{
				this.curAction = new PoolObjHandle<Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, new GameObject[]
				{
					user.handle.gameObject,
					this.skillContext.TargetActor.handle.gameObject
				}));
			}
			else
			{
				this.curAction = new PoolObjHandle<Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, new GameObject[]
				{
					user.handle.gameObject
				}));
			}
			if (!this.curAction)
			{
				return false;
			}
			this.curAction.handle.onActionStop += this.OnActionStopDelegate;
			this.curAction.handle.refParams.AddRefParam("SkillObj", this);
			this.curAction.handle.refParams.AddRefParam("SkillContext", this.skillContext);
			if (flag)
			{
				this.curAction.handle.refParams.AddRefParam("TargetActor", this.skillContext.TargetActor);
			}
			if (flag2)
			{
				this.curAction.handle.refParams.SetRefParam("_TargetPos", this.skillContext.UseVector);
			}
			if (flag3)
			{
				this.curAction.handle.refParams.SetRefParam("_TargetDir", value);
			}
			this.curAction.handle.refParams.SetRefParam("_BulletPos", this.skillContext.BulletPos);
			this.curAction.handle.refParams.SetRefParam("_BulletUseDir", user.handle.forward);
			if (this.bAgeImmeExcute)
			{
				this.curAction.handle.UpdateLogic((int)Singleton<FrameSynchr>.GetInstance().FrameDelta);
			}
			return true;
		}

		public virtual bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
		{
			if (!user || this.skillContext == null || string.IsNullOrEmpty(this.ActionName))
			{
				return false;
			}
			this.skillContext.Copy(ref param);
			return this.UseImpl(user);
		}

		public virtual bool Use(PoolObjHandle<ActorRoot> user)
		{
			return user && this.skillContext != null && !string.IsNullOrEmpty(this.ActionName) && this.UseImpl(user);
		}

		public virtual void OnActionStoped(ref PoolObjHandle<Action> action)
		{
			action.handle.onActionStop -= this.OnActionStopDelegate;
			if (!this.curAction)
			{
				return;
			}
			if (action == this.curAction)
			{
				this.curAction.Release();
			}
		}

		public PoolObjHandle<ActorRoot> GetTargetActor()
		{
			SkillUseContext skillUseContext = this.GetSkillUseContext();
			if (skillUseContext == null)
			{
				return new PoolObjHandle<ActorRoot>(null);
			}
			return skillUseContext.TargetActor;
		}

		public SkillUseContext GetSkillUseContext()
		{
			if (!this.curAction)
			{
				return null;
			}
			return this.curAction.handle.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
		}
	}
}
