using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class MiniMapMoveCityDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			MiniMapMoveCityDuration miniMapMoveCityDuration = ClassObjPool<MiniMapMoveCityDuration>.Get();
			miniMapMoveCityDuration.CopyData(this);
			return miniMapMoveCityDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MiniMapMoveCityDuration miniMapMoveCityDuration = src as MiniMapMoveCityDuration;
			this.targetId = miniMapMoveCityDuration.targetId;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			if (!this.actorObj)
			{
				return;
			}
			if (ActorHelper.IsHostCampActor(ref this.actorObj))
			{
				BackCityCom_3DUI.ShowBack2City(this.actorObj);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (ActorHelper.IsHostCampActor(ref this.actorObj))
			{
				BackCityCom_3DUI.HideBack2City(this.actorObj);
			}
		}
	}
}
