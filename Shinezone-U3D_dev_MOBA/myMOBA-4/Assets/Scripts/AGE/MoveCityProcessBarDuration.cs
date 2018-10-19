using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class MoveCityProcessBarDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public string key = string.Empty;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.key = string.Empty;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			MoveCityProcessBarDuration moveCityProcessBarDuration = ClassObjPool<MoveCityProcessBarDuration>.Get();
			moveCityProcessBarDuration.CopyData(this);
			return moveCityProcessBarDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MoveCityProcessBarDuration moveCityProcessBarDuration = src as MoveCityProcessBarDuration;
			this.targetId = moveCityProcessBarDuration.targetId;
			this.key = moveCityProcessBarDuration.key;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			if (!this.actorObj)
			{
				return;
			}
			if (ActorHelper.IsHostCtrlActor(ref this.actorObj))
			{
				uint startTime = (uint)Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				uint length = (uint)this.length;
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					string text = (!string.IsNullOrEmpty(this.key)) ? Singleton<CTextManager>.GetInstance().GetText(this.key) : string.Empty;
					Singleton<CBattleSystem>.GetInstance().FightForm.StartGoBackProcessBar(startTime, length, text);
				}
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (ActorHelper.IsHostCtrlActor(ref this.actorObj) && Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.EndGoBackProcessBar();
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
			if (ActorHelper.IsHostCtrlActor(ref this.actorObj) && Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				uint curTime = (uint)Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				Singleton<CBattleSystem>.GetInstance().FightForm.UpdateGoBackProcessBar(curTime);
			}
		}
	}
}
