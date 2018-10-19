using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SkillGatherDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int triggerRadius;

		public int triggerTime;

		public bool bGatherTime;

		private int lastTime;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.lastTime = 0;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			SkillGatherDuration skillGatherDuration = ClassObjPool<SkillGatherDuration>.Get();
			skillGatherDuration.CopyData(this);
			return skillGatherDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillGatherDuration skillGatherDuration = src as SkillGatherDuration;
			this.targetId = skillGatherDuration.targetId;
			this.triggerTime = skillGatherDuration.triggerTime;
			this.triggerRadius = skillGatherDuration.triggerRadius;
			this.bGatherTime = skillGatherDuration.bGatherTime;
		}

		private void TriggerBullet()
		{
			if (!this.actorObj)
			{
				return;
			}
			int count = this.actorObj.handle.SkillControl.SpawnedBullets.Count;
			for (int i = 0; i < count; i++)
			{
				BulletSkill bulletSkill = this.actorObj.handle.SkillControl.SpawnedBullets[i];
				if (bulletSkill != null && bulletSkill.CurAction)
				{
					bulletSkill.CurAction.handle.refParams.SetRefParam("_TriggerBullet", true);
					if (this.bGatherTime)
					{
						SkillUseContext refParamObject = bulletSkill.CurAction.handle.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
						if (refParamObject != null)
						{
							refParamObject.GatherTime = this.lastTime / 1000;
							if (refParamObject.GatherTime <= 0)
							{
								refParamObject.GatherTime = 1;
							}
							bulletSkill.lifeTime = this.triggerTime * refParamObject.GatherTime;
						}
					}
					bulletSkill.CurAction.handle.refParams.AddRefParam("_BulletRealFlyingTime", bulletSkill.lifeTime);
				}
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			this.lastTime = 0;
		}

		public override void Leave(Action _action, Track _track)
		{
			this.lastTime = _action.CurrentTime;
			this.TriggerBullet();
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}
	}
}
