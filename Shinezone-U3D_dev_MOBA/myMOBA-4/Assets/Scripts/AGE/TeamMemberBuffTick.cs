using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class TeamMemberBuffTick : TickEvent
	{
		public bool bPlayer1;

		public bool bPlayer2;

		public bool bPlayer3;

		public bool bPlayer4;

		public bool bPlayer5;

		public bool bTeammate1;

		public bool bTeammate2;

		public bool bTeammate3;

		public COM_PLAYERCAMP PlayerCamp;

		[AssetReference(AssetRefType.SkillCombine)]
		public int BuffID;

		public bool bSkipDead = true;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bPlayer1 = false;
			this.bPlayer2 = false;
			this.bPlayer3 = false;
			this.bPlayer4 = false;
			this.bPlayer5 = false;
			this.bTeammate1 = false;
			this.bTeammate2 = false;
			this.bTeammate3 = false;
			this.PlayerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
			this.BuffID = 0;
			this.bSkipDead = true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TeamMemberBuffTick teamMemberBuffTick = src as TeamMemberBuffTick;
			this.bPlayer1 = teamMemberBuffTick.bPlayer1;
			this.bPlayer2 = teamMemberBuffTick.bPlayer2;
			this.bPlayer3 = teamMemberBuffTick.bPlayer3;
			this.bPlayer4 = teamMemberBuffTick.bPlayer4;
			this.bPlayer5 = teamMemberBuffTick.bPlayer5;
			this.bTeammate1 = teamMemberBuffTick.bTeammate1;
			this.bTeammate2 = teamMemberBuffTick.bTeammate2;
			this.bTeammate3 = teamMemberBuffTick.bTeammate3;
			this.PlayerCamp = teamMemberBuffTick.PlayerCamp;
			this.BuffID = teamMemberBuffTick.BuffID;
			this.bSkipDead = teamMemberBuffTick.bSkipDead;
		}

		public override BaseEvent Clone()
		{
			TeamMemberBuffTick teamMemberBuffTick = ClassObjPool<TeamMemberBuffTick>.Get();
			teamMemberBuffTick.CopyData(this);
			return teamMemberBuffTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			if (this.BuffID <= 0)
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
			if (this.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ref list);
				this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ref list);
			}
			else
			{
				this.AddActorRootList(this.PlayerCamp, ref list);
			}
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BufConsumer bufConsumer = new BufConsumer(this.BuffID, enumerator.Current, enumerator.Current);
				bufConsumer.Use();
			}
		}

		private void AddActorRootList(COM_PLAYERCAMP inPlayerCamp, ref List<PoolObjHandle<ActorRoot>> actorRootList)
		{
			List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(inPlayerCamp);
			if (!this.bPlayer5 && allCampPlayers.Count >= 5)
			{
				allCampPlayers.RemoveAt(4);
			}
			if (!this.bPlayer4 && allCampPlayers.Count >= 4)
			{
				allCampPlayers.RemoveAt(3);
			}
			if (!this.bPlayer3 && allCampPlayers.Count >= 3)
			{
				allCampPlayers.RemoveAt(2);
			}
			if (!this.bPlayer2 && allCampPlayers.Count >= 2)
			{
				allCampPlayers.RemoveAt(1);
			}
			if (!this.bPlayer1 && allCampPlayers.Count >= 1)
			{
				allCampPlayers.RemoveAt(0);
			}
			List<Player>.Enumerator enumerator = allCampPlayers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = enumerator.Current.GetAllHeroes();
				List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>(allHeroes.Count);
				for (int i = 0; i < allHeroes.Count; i++)
				{
					list.Add(allHeroes[i]);
				}
				if (!this.bTeammate3 && list.Count >= 3)
				{
					list.RemoveAt(2);
				}
				if (!this.bTeammate2 && list.Count >= 2)
				{
					list.RemoveAt(1);
				}
				if (!this.bTeammate1 && list.Count >= 1)
				{
					list.RemoveAt(0);
				}
				if (!this.bSkipDead)
				{
					actorRootList.AddRange(allHeroes);
				}
				else
				{
					int count = allHeroes.Count;
					for (int j = 0; j < count; j++)
					{
						PoolObjHandle<ActorRoot> poolObjHandle = allHeroes[j];
						if (poolObjHandle)
						{
							if (!poolObjHandle.handle.ActorControl.IsDeadState)
							{
								actorRootList.Add(poolObjHandle);
							}
						}
					}
				}
			}
		}
	}
}
