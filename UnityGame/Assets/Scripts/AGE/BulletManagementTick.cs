using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class BulletManagementTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public bool bIgnoreLimit;

		public int iLifeTime;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.bIgnoreLimit = false;
			this.iLifeTime = 0;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BulletManagementTick bulletManagementTick = src as BulletManagementTick;
			this.targetId = bulletManagementTick.targetId;
			this.bIgnoreLimit = bulletManagementTick.bIgnoreLimit;
			this.iLifeTime = bulletManagementTick.iLifeTime;
		}

		public override BaseEvent Clone()
		{
			BulletManagementTick bulletManagementTick = ClassObjPool<BulletManagementTick>.Get();
			bulletManagementTick.CopyData(this);
			return bulletManagementTick;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
			if (refParamObject != null)
			{
				BulletSkill bulletSkill = refParamObject as BulletSkill;
				if (bulletSkill != null)
				{
					if (this.bIgnoreLimit)
					{
						bulletSkill.IgnoreUpperLimit();
					}
					bulletSkill.lifeTime = this.iLifeTime;
				}
			}
		}
	}
}
