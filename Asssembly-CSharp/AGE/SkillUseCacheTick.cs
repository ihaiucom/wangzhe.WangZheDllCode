using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class SkillUseCacheTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillUseCacheTick skillUseCacheTick = src as SkillUseCacheTick;
			this.targetId = skillUseCacheTick.targetId;
		}

		public override BaseEvent Clone()
		{
			SkillUseCacheTick skillUseCacheTick = ClassObjPool<SkillUseCacheTick>.Get();
			skillUseCacheTick.CopyData(this);
			return skillUseCacheTick;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			SkillComponent skillControl = actorHandle.handle.SkillControl;
			if (skillControl == null)
			{
				return;
			}
			if (skillControl.SkillUseCache != null)
			{
				skillControl.SkillUseCache.UseSkillCache(actorHandle);
			}
		}
	}
}
