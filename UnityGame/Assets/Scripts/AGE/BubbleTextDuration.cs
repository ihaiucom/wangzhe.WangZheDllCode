using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class BubbleTextDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public bool bPlayer1;

		public bool bPlayer2;

		public bool bPlayer3;

		public bool bTeammate1;

		public bool bTeammate2;

		public bool bTeammate3;

		public COM_PLAYERCAMP PlayerCamp;

		public int BubbleTextId;

		public int Offset_x;

		public int Offset_y;

		private List<PoolObjHandle<ActorRoot>> actorRootList;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.srcId = 0;
			this.targetId = 0;
			this.bPlayer1 = false;
			this.bPlayer2 = false;
			this.bPlayer3 = false;
			this.bTeammate1 = false;
			this.bTeammate2 = false;
			this.bTeammate3 = false;
			this.PlayerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
			this.BubbleTextId = 0;
			this.Offset_x = 0;
			this.Offset_y = 0;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BubbleTextDuration bubbleTextDuration = src as BubbleTextDuration;
			this.srcId = bubbleTextDuration.srcId;
			this.targetId = bubbleTextDuration.targetId;
			this.bPlayer1 = bubbleTextDuration.bPlayer1;
			this.bPlayer2 = bubbleTextDuration.bPlayer2;
			this.bPlayer3 = bubbleTextDuration.bPlayer3;
			this.bTeammate1 = bubbleTextDuration.bTeammate1;
			this.bTeammate2 = bubbleTextDuration.bTeammate2;
			this.bTeammate3 = bubbleTextDuration.bTeammate3;
			this.PlayerCamp = bubbleTextDuration.PlayerCamp;
			this.BubbleTextId = bubbleTextDuration.BubbleTextId;
			this.Offset_x = bubbleTextDuration.Offset_x;
			this.Offset_y = bubbleTextDuration.Offset_y;
		}

		public override BaseEvent Clone()
		{
			BubbleTextDuration bubbleTextDuration = ClassObjPool<BubbleTextDuration>.Get();
			bubbleTextDuration.CopyData(this);
			return bubbleTextDuration;
		}

		private void SetHudText(string inText, PoolObjHandle<ActorRoot> inActor)
		{
			if (inActor && inActor.handle.HudControl != null)
			{
				inActor.handle.HudControl.SetTextHud(inText, this.Offset_x, this.Offset_y, true);
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			if (this.BubbleTextId <= 0)
			{
				return;
			}
			this.actorRootList = new List<PoolObjHandle<ActorRoot>>();
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.srcId);
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.targetId);
			if (actorHandle)
			{
				this.actorRootList.Add(actorHandle);
			}
			if (actorHandle2)
			{
				this.actorRootList.Add(actorHandle2);
			}
			if (this.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
				this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
			else
			{
				this.AddActorRootList(this.PlayerCamp);
			}
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = this.actorRootList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.SetHudText(Utility.GetBubbleText((uint)this.BubbleTextId), enumerator.get_Current());
			}
		}

		private void AddActorRootList(COM_PLAYERCAMP inPlayerCamp)
		{
			List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(inPlayerCamp);
			if (!this.bPlayer3 && allCampPlayers.get_Count() >= 3)
			{
				allCampPlayers.RemoveAt(2);
			}
			if (!this.bPlayer2 && allCampPlayers.get_Count() >= 2)
			{
				allCampPlayers.RemoveAt(1);
			}
			if (!this.bPlayer1 && allCampPlayers.get_Count() >= 1)
			{
				allCampPlayers.RemoveAt(0);
			}
			List<Player>.Enumerator enumerator = allCampPlayers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = enumerator.get_Current().GetAllHeroes();
				List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>(allHeroes.Count);
				for (int i = 0; i < allHeroes.Count; i++)
				{
					list.Add(allHeroes[i]);
				}
				if (!this.bTeammate3 && list.get_Count() >= 3)
				{
					list.RemoveAt(2);
				}
				if (!this.bTeammate2 && list.get_Count() >= 2)
				{
					list.RemoveAt(1);
				}
				if (!this.bTeammate1 && list.get_Count() >= 1)
				{
					list.RemoveAt(0);
				}
				this.actorRootList.AddRange(list);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.actorRootList != null)
			{
				List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = this.actorRootList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					this.SetHudText(string.Empty, enumerator.get_Current());
				}
				this.actorRootList.Clear();
				this.actorRootList = null;
			}
			base.Leave(_action, _track);
		}
	}
}
