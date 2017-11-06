using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class PauseGameDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId;

		public bool bEffectTimeScale = true;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.srcId = 0;
			this.bEffectTimeScale = true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PauseGameDuration pauseGameDuration = src as PauseGameDuration;
			this.srcId = pauseGameDuration.srcId;
			this.bEffectTimeScale = pauseGameDuration.bEffectTimeScale;
		}

		public override BaseEvent Clone()
		{
			PauseGameDuration pauseGameDuration = ClassObjPool<PauseGameDuration>.Get();
			pauseGameDuration.CopyData(this);
			return pauseGameDuration;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, this.bEffectTimeScale);
		}

		public override void Leave(Action _action, Track _track)
		{
			Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
			base.Leave(_action, _track);
		}
	}
}
