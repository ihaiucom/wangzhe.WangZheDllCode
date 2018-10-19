using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class GameInfoBase
	{
		protected GameContextBase GameContext;

		public GameContextBase gameContext
		{
			get
			{
				return this.GameContext;
			}
		}

		public virtual bool Initialize(GameContextBase InGameContext)
		{
			DebugHelper.Assert(InGameContext != null);
			this.GameContext = InGameContext;
			return this.GameContext != null;
		}

		public virtual void PreBeginPlay()
		{
		}

		public virtual void PostBeginPlay()
		{
			Singleton<BattleLogic>.GetInstance().PrepareFight();
			if (!Singleton<LobbyLogic>.instance.inMultiGame)
			{
				Singleton<FrameSynchr>.GetInstance().ResetSynchr();
				bool flag = false;
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (curLvelContext != null && curLvelContext.m_preDialogId > 0 && hostPlayer != null && hostPlayer.Captain)
				{
					flag = true;
					MonoSingleton<DialogueProcessor>.instance.PlayDrama(curLvelContext.m_preDialogId, hostPlayer.Captain.handle.gameObject, hostPlayer.Captain.handle.gameObject, true);
				}
				if (!flag)
				{
					Singleton<BattleLogic>.GetInstance().DoBattleStart();
				}
				else
				{
					Singleton<BattleLogic>.GetInstance().BindFightPrepareFinListener();
				}
			}
			else if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<LobbyLogic>.GetInstance().ReqStartMultiGame();
			}
			SoldierRegion.bFirstSpawnEvent = true;
		}

		public virtual void StartFight()
		{
		}

		public virtual void ReduceDamage(ref HurtDataInfo HurtInfo)
		{
		}

		public virtual void EndGame()
		{
		}

		public virtual void OnLoadingProgress(float Progress)
		{
		}

		protected virtual void LoadAllTeamActors()
		{
			List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetAllPlayers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					ReadonlyContext<uint> allHeroIds = enumerator.Current.GetAllHeroIds();
					for (int i = 0; i < allHeroIds.Count; i++)
					{
						ActorMeta actorMeta = default(ActorMeta);
						actorMeta.ActorCamp = enumerator.Current.PlayerCamp;
						actorMeta.ConfigId = (int)allHeroIds[i];
						actorMeta.PlayerId = enumerator.Current.PlayerId;
						MonoSingleton<GameLoader>.instance.AddActor(ref actorMeta);
					}
				}
			}
		}
	}
}
