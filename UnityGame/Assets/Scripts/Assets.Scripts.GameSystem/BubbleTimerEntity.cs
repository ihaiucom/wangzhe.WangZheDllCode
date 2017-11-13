using System;

namespace Assets.Scripts.GameSystem
{
	public class BubbleTimerEntity : CDEntity
	{
		public ulong playerid;

		public uint heroid;

		public BubbleTimerEntity(ulong playerid, uint heroid, int cd_time) : base(cd_time, 0)
		{
			this.playerid = playerid;
			this.heroid = heroid;
		}

		public override void On_Timer_End(int timer)
		{
			base.On_Timer_End(timer);
			InBattleMsgUT.ShowBubble(this.playerid, this.heroid, string.Empty);
		}
	}
}
