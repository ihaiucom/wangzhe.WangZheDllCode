using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class ResetSoldierLineTick : TickEvent
	{
		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
		}

		public override BaseEvent Clone()
		{
			return ClassObjPool<ResetSoldierLineTick>.Get();
		}

		public override void Process(Action _action, Track _track)
		{
			if (Singleton<BattleLogic>.GetInstance().mapLogic != null)
			{
				Singleton<BattleLogic>.GetInstance().mapLogic.ResetSoldierRegion();
			}
		}
	}
}
