using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class OrganControl
	{
		public struct SoldierAddAttrs
		{
			public bool HadAdd;

			public int MaxHP;

			public int PhycAttack;

			public int MagcAttack;

			public int PhycDefend;

			public int MagcDefend;

			public int DropCoin;

			public int DropExp;

			public int ShapeScale;

			public void Reset()
			{
				this.HadAdd = false;
				this.MaxHP = 0;
				this.PhycAttack = 0;
				this.PhycDefend = 0;
				this.MagcAttack = 0;
				this.MagcDefend = 0;
				this.DropCoin = 0;
				this.DropExp = 0;
				this.ShapeScale = 0;
			}
		}

		private bool _supportHighTowerSoldier;

		private OrganControl.SoldierAddAttrs[] _soldierAddAttrs = new OrganControl.SoldierAddAttrs[3];

		public bool HadSoldierAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].HadAdd;
		}

		public int GetSoldierMaxHpAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].MaxHP;
		}

		public int GetSoldierPhycAttackAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].PhycAttack;
		}

		public int GetSoldierMagcAttackAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].MagcAttack;
		}

		public int GetSoldierPhycDefendAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].PhycDefend;
		}

		public int GetSoldierMagcDefendAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].MagcDefend;
		}

		public int GetSoldierDropCoinAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].DropCoin;
		}

		public int GetSoldierDropExpAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].DropExp;
		}

		public int GetSoldierShapeScaleAddByOrgan(COM_PLAYERCAMP camp)
		{
			return this._soldierAddAttrs[(int)camp].ShapeScale;
		}

		public void FightStart()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)curLvelContext.m_mapType, (uint)curLvelContext.m_mapID);
			this._supportHighTowerSoldier = (pvpMapCommonInfo != null && pvpMapCommonInfo.bSupportHighTowerSoldier != 0);
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnOrganDead));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnOrganDead));
			for (int i = 0; i < this._soldierAddAttrs.Length; i++)
			{
				this._soldierAddAttrs[i].Reset();
			}
		}

		public void FightOver()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnOrganDead));
		}

		private void OnOrganDead(ref GameDeadEventParam prm)
		{
			if (!prm.src || prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ || !prm.atker)
			{
				return;
			}
			ActorRoot handle = prm.src.handle;
			OrganWrapper organWrapper = handle.AsOrgan();
			COM_PLAYERCAMP actorCamp = prm.atker.handle.TheActorMeta.ActorCamp;
			if (handle.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
			{
				SoldierRegion soldierRegionByRoute = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldierRegionByRoute(actorCamp, handle.TheStaticData.TheOrganOnlyInfo.AttackRouteID);
				if (soldierRegionByRoute != null)
				{
					soldierRegionByRoute.SwitchWave(handle.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier, false);
				}
			}
			if (this._supportHighTowerSoldier && organWrapper.cfgInfo.bOrganType == 4)
			{
				List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
				bool flag = true;
				for (int i = 0; i < organActors.Count; i++)
				{
					PoolObjHandle<ActorRoot> ptr = organActors[i];
					if (ptr && ptr.handle.AsOrgan().cfgInfo.bOrganType == organWrapper.cfgInfo.bOrganType && ptr.handle.TheActorMeta.ActorCamp == handle.TheActorMeta.ActorCamp && !ptr.handle.ActorControl.IsDeadState)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this._soldierAddAttrs[(int)actorCamp].HadAdd = true;
					this._soldierAddAttrs[(int)actorCamp].MaxHP = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(335u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].PhycAttack = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(336u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].PhycDefend = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(338u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].MagcAttack = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(337u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].MagcDefend = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(339u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].DropCoin = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(340u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].DropExp = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(341u).dwConfValue;
					this._soldierAddAttrs[(int)actorCamp].ShapeScale = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(342u).dwConfValue;
					KillNotify theKillNotify = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
					Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
					if (theKillNotify != null && hostPlayer != null)
					{
						bool bSrcAllies = hostPlayer.PlayerCamp == actorCamp;
						KillInfo killInfo = new KillInfo(KillNotify.building_icon, null, KillDetailInfoType.Info_Type_Destroy_All_Tower, bSrcAllies, false, ActorTypeDef.Invalid, false);
						theKillNotify.AddKillInfo(ref killInfo);
					}
				}
			}
		}
	}
}
