using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class NewbieShowDragonBuffTick : TickEvent
	{
		public override BaseEvent Clone()
		{
			NewbieShowDragonBuffTick newbieShowDragonBuffTick = ClassObjPool<NewbieShowDragonBuffTick>.Get();
			newbieShowDragonBuffTick.CopyData(this);
			return newbieShowDragonBuffTick;
		}

		public override void Process(Action _action, Track _track)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.SetDragonUINum(COM_PLAYERCAMP.COM_PLAYERCAMP_1, 3);
			}
		}
	}
}
