using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.GameKernal
{
	public class GamePlayerCenter : Singleton<GamePlayerCenter>
	{
		public const uint MaxPlayerNum = 10u;

		private List<Player> _playersTempList = new List<Player>();

		private List<Player> _playersTempAllList = new List<Player>();

		public uint HostPlayerId;

		private Player _hostPlayer;

		private readonly SortedDictionary<uint, Player> _players = new SortedDictionary<uint, Player>();

		public COM_PLAYERCAMP hostPlayerCamp
		{
			get
			{
				Player hostPlayer = this.GetHostPlayer();
				return (hostPlayer == null) ? COM_PLAYERCAMP.COM_PLAYERCAMP_MID : hostPlayer.PlayerCamp;
			}
		}

		public bool isHostPlayerCaptainDead
		{
			get
			{
				Player hostPlayer = this.GetHostPlayer();
				return hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle.ActorControl == null || hostPlayer.Captain.handle.ActorControl.IsDeadState;
			}
		}

		public override void Init()
		{
		}

		public Player AddPlayer(uint playerId, COM_PLAYERCAMP camp, int campPos = 0, uint level = 1u, bool isComputer = false, string name = null, int headIconId = 0, int logicWrold = 0, ulong uid = 0uL, uint vipLv = 0u, string openId = null, uint gradeOfRank = 0u, uint classOfRank = 0u, uint wangZheCnt = 0u, int honorId = 0, int honorLevel = 0, GameIntimacyData IntimacyData = null, ulong privacyBits = 0uL)
		{
			Player result = null;
			if (playerId == 0u)
			{
				DebugHelper.Assert(false, "Try to create player by Id 0");
			}
			else if (this._players.ContainsKey(playerId))
			{
				DebugHelper.Assert(false, "Try to create player which is already existed, ID is {0}", new object[]
				{
					playerId
				});
				result = this.GetPlayer(playerId);
			}
			else
			{
				Player player = new Player();
				player.PlayerId = playerId;
				player.LogicWrold = logicWrold;
				player.PlayerUId = uid;
				player.PlayerCamp = camp;
				player.CampPos = campPos;
				player.Level = (int)level;
				player.HeadIconId = headIconId;
				player.Computer = isComputer;
				player.Name = CUIUtility.RemoveEmoji(name);
				player.isGM = false;
				player.VipLv = vipLv;
				player.OpenId = openId;
				player.GradeOfRank = gradeOfRank;
				player.ClassOfRank = classOfRank;
				player.WangZheCnt = wangZheCnt;
				player.HonorId = honorId;
				player.HonorLevel = honorLevel;
				player.IntimacyData = IntimacyData;
				player.privacyBits = privacyBits;
				this._players.Add(playerId, player);
				result = player;
			}
			DebugHelper.Assert((long)this._players.Count <= 10L, "超出Player最大数量");
			return result;
		}

		public void SetHostPlayer(uint playerId)
		{
			if (!this._players.ContainsKey(playerId))
			{
				DebugHelper.Assert(false, "try to set hostplayer which is not exists in player lists. id={0}", new object[]
				{
					playerId
				});
				return;
			}
			DebugHelper.CustomLog("SetHostPlayer id = {0}", new object[]
			{
				playerId
			});
			this.HostPlayerId = playerId;
			this._players.TryGetValue(playerId, out this._hostPlayer);
			if (!Singleton<WatchController>.instance.IsWatching)
			{
				Singleton<WatchController>.instance.InitHostCamp();
			}
		}

		public void ConnectActorRootAndPlayer(ref PoolObjHandle<ActorRoot> hero)
		{
			if (!hero)
			{
				DebugHelper.Assert(false, "Failed Connect Actor Root And Player, hero is null");
				return;
			}
			Player player = this.GetPlayer(hero.handle.TheActorMeta.PlayerId);
			if (player == null)
			{
				DebugHelper.Assert(false, "Failed Find palyer {0}, failed connect actor.", new object[]
				{
					hero.handle.TheActorMeta.PlayerId
				});
				return;
			}
			player.ConnectHeroActorRoot(ref hero);
		}

		public List<Player> GetDiffCampPlayers(COM_PLAYERCAMP camp)
		{
			this._playersTempList.Clear();
			SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Player> current = enumerator.Current;
				Player value = current.Value;
				if (value.PlayerCamp != camp)
				{
					this._playersTempList.Add(value);
				}
			}
			return this._playersTempList;
		}

		public List<Player> GetAllCampPlayers(COM_PLAYERCAMP camp)
		{
			this._playersTempList.Clear();
			SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Player> current = enumerator.Current;
				Player value = current.Value;
				if (value.PlayerCamp == camp)
				{
					this._playersTempList.Add(value);
				}
			}
			return this._playersTempList;
		}

		public List<Player> GetAllPlayers()
		{
			this._playersTempAllList.Clear();
			SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Player> current = enumerator.Current;
				Player value = current.Value;
				this._playersTempAllList.Add(value);
			}
			return this._playersTempAllList;
		}

		public Player GetPlayer(uint playerId)
		{
			Player result = null;
			this._players.TryGetValue(playerId, out result);
			return result;
		}

		public Player GetPlayerByUid(ulong uid)
		{
			SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Player> current = enumerator.Current;
				if (current.Value.PlayerUId == uid)
				{
					KeyValuePair<uint, Player> current2 = enumerator.Current;
					return current2.Value;
				}
			}
			return null;
		}

		public bool IsAtSameCamp(uint player1Id, uint player2Id)
		{
			Player player = this.GetPlayer(player1Id);
			Player player2 = this.GetPlayer(player2Id);
			return player != null && player2 != null && player.PlayerCamp == player2.PlayerCamp;
		}

		public int GetPlayerCampPosIndex(uint playerId)
		{
			int num = 0;
			Player player = this.GetPlayer(playerId);
			if (player == null)
			{
				return num;
			}
			SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Player> current = enumerator.Current;
				if (current.Value.PlayerId == playerId)
				{
					break;
				}
				KeyValuePair<uint, Player> current2 = enumerator.Current;
				if (current2.Value.PlayerCamp == player.PlayerCamp)
				{
					num++;
				}
			}
			return num;
		}

		public Player GetHostPlayer()
		{
			if (this._hostPlayer == null || this._hostPlayer.PlayerId != this.HostPlayerId)
			{
				this._hostPlayer = this.GetPlayer(this.HostPlayerId);
				DebugHelper.Assert(this._hostPlayer != null);
			}
			return this._hostPlayer;
		}

		public void ClearAllPlayers()
		{
			SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Player> current = enumerator.Current;
				Player value = current.Value;
				value.ClearHeroes();
			}
			this._playersTempList.Clear();
			this._playersTempAllList.Clear();
			this._players.Clear();
			this._hostPlayer = null;
			this.HostPlayerId = 0u;
		}

		public bool IsHostPlayerHasCpuEnemy()
		{
			bool result = false;
			Player hostPlayer = this.GetHostPlayer();
			if (hostPlayer != null)
			{
				COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
				List<Player> allPlayers = this.GetAllPlayers();
				for (int i = 0; i < allPlayers.Count; i++)
				{
					Player player = allPlayers[i];
					if (player != null && player.PlayerCamp != playerCamp && player.Computer)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
