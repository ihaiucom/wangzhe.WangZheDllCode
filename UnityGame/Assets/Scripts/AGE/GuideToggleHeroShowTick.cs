using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class GuideToggleHeroShowTick : DurationEvent
	{
		public bool bShow;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			GuideToggleHeroShowTick guideToggleHeroShowTick = src as GuideToggleHeroShowTick;
			this.bShow = guideToggleHeroShowTick.bShow;
		}

		public override BaseEvent Clone()
		{
			GuideToggleHeroShowTick guideToggleHeroShowTick = ClassObjPool<GuideToggleHeroShowTick>.Get();
			guideToggleHeroShowTick.CopyData(this);
			return guideToggleHeroShowTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bShow = false;
		}

		public override void Enter(Action _action, Track _track)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain)
			{
				DebugHelper.Assert(hostPlayer.Captain, "Captain Hero is NULL!!");
				if (this.bShow)
				{
					hostPlayer.Captain.handle.ActorMesh.SetLayer("Actor", false);
					hostPlayer.Captain.handle.HudControl.SetComVisible(true && hostPlayer.Captain.handle.InCamera);
				}
				else
				{
					hostPlayer.Captain.handle.ActorMesh.SetLayer("Hide", false);
				}
			}
		}

		public override void Leave(Action _action, Track _track)
		{
		}
	}
}
