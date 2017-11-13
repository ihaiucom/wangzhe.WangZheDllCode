using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class EnableSoldierLineTick : TickEvent
	{
		public bool bEnable;

		public int SoldierWaveId;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			EnableSoldierLineTick enableSoldierLineTick = src as EnableSoldierLineTick;
			this.bEnable = enableSoldierLineTick.bEnable;
		}

		public override BaseEvent Clone()
		{
			EnableSoldierLineTick enableSoldierLineTick = ClassObjPool<EnableSoldierLineTick>.Get();
			enableSoldierLineTick.CopyData(this);
			return enableSoldierLineTick;
		}

		public override void Process(Action _action, Track _track)
		{
			if (Singleton<BattleLogic>.GetInstance().mapLogic != null)
			{
				Singleton<BattleLogic>.GetInstance().mapLogic.EnableSoldierRegion(this.bEnable, this.SoldierWaveId);
			}
		}
	}
}
