using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.GameKernal
{
	public class Player
	{
		public uint PlayerId;

		public int LogicWrold;

		public ulong PlayerUId;

		public COM_PLAYERCAMP PlayerCamp;

		public int CampPos;

		public bool Computer;

		public string Name;

		public int HeadIconId;

		public bool isGM;

		public uint VipLv;

		public string OpenId;

		public uint GradeOfRank;

		public uint ClassOfRank;

		public uint WangZheCnt;

		public int HonorId;

		public int HonorLevel;

		public GameIntimacyData IntimacyData;

		public ulong privacyBits;

		private CrypticInt32 _level = 1;

		public uint CaptainId;

		private readonly List<uint> _heroIds = new List<uint>();

		public PoolObjHandle<ActorRoot> Captain;

		public SelectEnemyType AttackTargetMode = SelectEnemyType.SelectLowHp;

		public LastHitMode useLastHitMode;

		public AttackOrganMode curAttackOrganMode;

		private bool bUseAdvanceCommonAttack = true;

		public bool bCommonAttackLockMode = true;

		private OperateMode useOperateMode;

		private readonly List<PoolObjHandle<ActorRoot>> _heroes = new List<PoolObjHandle<ActorRoot>>();

		public bool m_bMoved;

		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
			}
		}

		public int heroCount
		{
			get
			{
				return this._heroIds.get_Count();
			}
		}

		public Player()
		{
			this.CaptainId = 0u;
			this.Level = 1;
		}

		public void SetCallCaptain(uint configId, uint objID)
		{
			this.CaptainId = configId;
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(objID);
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
			{
				this.Captain = actor;
			}
		}

		public void SetCaptain(uint configId)
		{
			this.CaptainId = configId;
			this.Captain = this._heroes.Find((PoolObjHandle<ActorRoot> hero) => (long)hero.handle.TheActorMeta.ConfigId == (long)((ulong)configId));
			if (this.PlayerId == Singleton<GamePlayerCenter>.instance.HostPlayerId)
			{
				DebugHelper.CustomLog("Set Captain id={0}", new object[]
				{
					this.CaptainId
				});
			}
			DebugHelper.Assert(this.Captain, "Failed SetCaptain id={0}", new object[]
			{
				configId
			});
		}

		public ReadonlyContext<PoolObjHandle<ActorRoot>> GetAllHeroes()
		{
			return new ReadonlyContext<PoolObjHandle<ActorRoot>>(this._heroes);
		}

		public ReadonlyContext<uint> GetAllHeroIds()
		{
			return new ReadonlyContext<uint>(this._heroIds);
		}

		public void AddHero(uint heroCfgId)
		{
			if (heroCfgId == 0u || this._heroIds.Contains(heroCfgId))
			{
				return;
			}
			if (this._heroIds.get_Count() >= 3)
			{
				DebugHelper.Assert(false, "已经给player添加了三个英雄但是还在尝试继续添加");
				return;
			}
			this._heroIds.Add(heroCfgId);
			if (!this.Computer)
			{
				if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
				{
					DebugHelper.CustomLog("Player {0} adds Hero {1}", new object[]
					{
						this.PlayerId,
						heroCfgId
					});
				}
			}
			else if (string.IsNullOrEmpty(this.Name))
			{
				ActorStaticData actorStaticData = default(ActorStaticData);
				ActorMeta actorMeta = default(ActorMeta);
				IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
				actorMeta.PlayerId = this.PlayerId;
				actorMeta.ActorType = ActorTypeDef.Actor_Type_Hero;
				actorMeta.ConfigId = (int)heroCfgId;
				string text = actorDataProvider.GetActorStaticData(ref actorMeta, ref actorStaticData) ? actorStaticData.TheResInfo.Name : null;
				this.Name = string.Format("{0}[{1}]", text, Singleton<CTextManager>.GetInstance().GetText("PVP_NPC"));
			}
			if (this._heroIds.get_Count() != 1)
			{
				return;
			}
			this.CaptainId = heroCfgId;
			if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
			{
				DebugHelper.CustomLog("Set Captain ID {0}", new object[]
				{
					this.CaptainId
				});
			}
		}

		public void ConnectHeroActorRoot(ref PoolObjHandle<ActorRoot> hero)
		{
			if (!hero)
			{
				DebugHelper.Assert(false, "Failed ConnectHeroActorRoot, hero is null");
				return;
			}
			if (!this._heroIds.Contains((uint)hero.handle.TheActorMeta.ConfigId))
			{
				DebugHelper.Assert(false, "Failed ConnectHeroActorRoot, id is not valid = {0} Camp={1} playerid={2}", new object[]
				{
					hero.handle.TheActorMeta.ConfigId,
					this.PlayerCamp,
					this.PlayerId
				});
				return;
			}
			this._heroes.Add(hero);
			if ((long)hero.handle.TheActorMeta.ConfigId == (long)((ulong)this.CaptainId))
			{
				this.Captain = hero;
			}
		}

		public int GetHeroTeamPosIndex(uint heroCfgId)
		{
			return this._heroIds.IndexOf(heroCfgId);
		}

		public void ClearHeroes()
		{
			if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
			{
				DebugHelper.CustomLog("Clear heros for playerid={0} camp={1}", new object[]
				{
					this.PlayerId,
					this.PlayerCamp
				});
			}
			this.IntimacyData = null;
			this._heroIds.Clear();
			this._heroes.Clear();
			this.CaptainId = 0u;
			this.Captain = default(PoolObjHandle<ActorRoot>);
		}

		public bool IsAllHeroesDead()
		{
			for (int i = 0; i < this._heroes.get_Count(); i++)
			{
				if (!this._heroes.get_Item(i).handle.ActorControl.IsDeadState)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsAtMyTeam(ref ActorMeta actorMeta)
		{
			return actorMeta.PlayerId == this.PlayerId && (this._heroIds.Contains((uint)actorMeta.ConfigId) || (actorMeta.ActorType == ActorTypeDef.Actor_Type_Call && this._heroIds.Contains((uint)actorMeta.HostConfigId)));
		}

		public bool IsMyTeamOutOfBattle()
		{
			for (int i = 0; i < this._heroes.get_Count(); i++)
			{
				if (this._heroes.get_Item(i).handle.ActorControl.IsInBattle)
				{
					return false;
				}
			}
			return true;
		}

		public int GetAllHeroCombatEft()
		{
			int num = 0;
			for (int i = 0; i < this._heroes.get_Count(); i++)
			{
				num += Player.GetHeroCombatEft(this._heroes.get_Item(i));
			}
			return num;
		}

		private static int GetHeroCombatEft(PoolObjHandle<ActorRoot> actor)
		{
			if (!actor)
			{
				return 0;
			}
			int num = 0;
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorServerData actorServerData = default(ActorServerData);
			if (actorDataProvider.GetActorServerData(ref actor.handle.TheActorMeta, ref actorServerData))
			{
				num += CHeroInfo.GetCombatEftByStarLevel((int)actorServerData.Level, (int)actorServerData.Star);
				num += CSkinInfo.GetCombatEft((uint)actorServerData.TheActorMeta.ConfigId, actorServerData.SkinId);
				ActorServerRuneData actorServerRuneData = default(ActorServerRuneData);
				for (int i = 0; i < 30; i++)
				{
					if (actorDataProvider.GetActorServerRuneData(ref actor.handle.TheActorMeta, (ActorRunelSlot)i, ref actorServerRuneData))
					{
						ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(actorServerRuneData.RuneId);
						if (dataByKey != null)
						{
							num += dataByKey.iCombatEft;
						}
					}
				}
			}
			return num;
		}

		public bool IsUseAdvanceCommonAttack()
		{
			return this.bUseAdvanceCommonAttack;
		}

		public void SetUseAdvanceCommonAttack(bool bFlag)
		{
			this.bUseAdvanceCommonAttack = bFlag;
		}

		public OperateMode GetOperateMode()
		{
			return this.useOperateMode;
		}

		public void SetOperateMode(OperateMode _mode)
		{
			if (_mode == OperateMode.DefaultMode)
			{
				if (this.Captain && this.Captain.handle.LockTargetAttackModeControl != null)
				{
					this.Captain.handle.LockTargetAttackModeControl.ClearTargetID();
				}
			}
			else if (this.Captain && this.Captain.handle.DefaultAttackModeControl != null)
			{
				this.Captain.handle.DefaultAttackModeControl.ClearCommonAttackTarget();
			}
			this.useOperateMode = _mode;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			for (int i = 0; i < heroActors.get_Count(); i++)
			{
				PoolObjHandle<ActorRoot> ptr = heroActors.get_Item(i);
				if (ptr && ptr.handle.TheActorMeta.PlayerId == this.PlayerId)
				{
					HeroWrapper heroWrapper = ptr.handle.ActorControl as HeroWrapper;
					if (heroWrapper != null)
					{
						heroWrapper.CurOpMode = this.useOperateMode;
					}
				}
			}
			if (ActorHelper.IsHostCtrlActor(ref this.Captain))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, (CommonAttactType)_mode);
			}
		}

		public void SetLastHitMode(LastHitMode _mode)
		{
			this.useLastHitMode = _mode;
			if (ActorHelper.IsHostCtrlActor(ref this.Captain) && this.useOperateMode == OperateMode.DefaultMode)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<LastHitMode>(EventID.GAME_SETTING_LASTHIT_MODE_CHANGE, _mode);
			}
		}

		public void SetAttackOrganMode(AttackOrganMode _mode)
		{
			this.curAttackOrganMode = _mode;
			if (ActorHelper.IsHostCtrlActor(ref this.Captain) && this.useOperateMode == OperateMode.DefaultMode)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<AttackOrganMode>(EventID.GAME_SETTING_ATTACKORGAN_MODE_CHANGE, _mode);
			}
		}
	}
}
