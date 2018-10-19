using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class TriggerBulletTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public int triggerRadius;

		private PoolObjHandle<ActorRoot> targetActor;

		public override BaseEvent Clone()
		{
			TriggerBulletTick triggerBulletTick = ClassObjPool<TriggerBulletTick>.Get();
			triggerBulletTick.CopyData(this);
			return triggerBulletTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerBulletTick triggerBulletTick = src as TriggerBulletTick;
			this.targetId = triggerBulletTick.targetId;
			this.triggerRadius = triggerBulletTick.triggerRadius;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
		}

		public override void OnRelease()
		{
			base.OnRelease();
			this.targetActor.Release();
		}

		private void TriggerBullet(VInt3 _bulletPos, Action _bulletAction)
		{
			long sqrMagnitudeLong2D = (this.targetActor.handle.location - _bulletPos).sqrMagnitudeLong2D;
			if (sqrMagnitudeLong2D <= (long)this.triggerRadius * (long)this.triggerRadius)
			{
				_bulletAction.refParams.SetRefParam("_TriggerBullet", true);
			}
		}

		public override void Process(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.targetId);
			if (!this.targetActor)
			{
				return;
			}
			VInt3 zero = VInt3.zero;
			int count = this.targetActor.handle.SkillControl.SpawnedBullets.Count;
			for (int i = 0; i < count; i++)
			{
				BulletSkill bulletSkill = this.targetActor.handle.SkillControl.SpawnedBullets[i];
				if (bulletSkill != null && bulletSkill.CurAction)
				{
					bulletSkill.CurAction.handle.refParams.GetRefParam("_BulletPos", ref zero);
					this.TriggerBullet(zero, bulletSkill.CurAction);
				}
			}
		}
	}
}
