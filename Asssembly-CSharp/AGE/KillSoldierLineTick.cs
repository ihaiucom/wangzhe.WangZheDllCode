using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class KillSoldierLineTick : TickEvent
	{
		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
		}

		public override BaseEvent Clone()
		{
			KillSoldierLineTick killSoldierLineTick = ClassObjPool<KillSoldierLineTick>.Get();
			killSoldierLineTick.CopyData(this);
			return killSoldierLineTick;
		}

		public override void Process(Action _action, Track _track)
		{
			Singleton<GameObjMgr>.GetInstance().KillSoldiers();
		}
	}
}
