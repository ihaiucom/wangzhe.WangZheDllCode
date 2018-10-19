using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SoldierWave
	{
		private int curTick;

		private int preSpawnTick;

		private int firstTick = -1;

		public int repeatCount = 1;

		public SoldierSelector Selector = new SoldierSelector();

		private bool bInIdleState;

		private int idleTick;

		private bool isCannonNotified;

		private bool isNotifiedBigDragonSoldier;

		public static uint ms_updatedFrameNum;

		public SoldierRegion Region
		{
			get;
			protected set;
		}

		public ResSoldierWaveInfo WaveInfo
		{
			get;
			protected set;
		}

		public int Index
		{
			get;
			protected set;
		}

		public bool IsInIdle
		{
			get
			{
				return this.bInIdleState;
			}
		}

		public SoldierWave(SoldierRegion InRegion, ResSoldierWaveInfo InWaveInfo, int InIndex)
		{
			this.Region = InRegion;
			this.WaveInfo = InWaveInfo;
			this.Index = InIndex;
			DebugHelper.Assert(this.Region != null && InWaveInfo != null);
			this.isCannonNotified = false;
			this.isNotifiedBigDragonSoldier = false;
			this.Reset();
		}

		public SoldierWave(ResSoldierWaveInfo InWaveInfo)
		{
			this.Region = null;
			this.WaveInfo = InWaveInfo;
			this.Index = 0;
			DebugHelper.Assert(InWaveInfo != null);
			this.isCannonNotified = false;
			this.isNotifiedBigDragonSoldier = false;
			this.Reset();
		}

		public void Reset()
		{
			this.curTick = (this.preSpawnTick = 0);
			this.firstTick = -1;
			this.repeatCount = 1;
			this.bInIdleState = false;
			this.idleTick = 0;
			this.Selector.Reset(this.WaveInfo);
		}

		public void ContinueState(SoldierWave sw)
		{
			this.curTick = sw.curTick;
			this.preSpawnTick = sw.preSpawnTick;
			this.firstTick = sw.firstTick;
			this.idleTick = sw.idleTick;
			this.bInIdleState = sw.bInIdleState;
			this.repeatCount = 0;
		}

		public SoldierSpawnResult Update(int delta)
		{
			this.firstTick = ((this.firstTick != -1) ? this.firstTick : this.curTick);
			this.curTick += delta;
			if (this.bInIdleState)
			{
				if ((long)(this.curTick - this.idleTick) < (long)((ulong)this.WaveInfo.dwIntervalTick))
				{
					return SoldierSpawnResult.ShouldWaitInterval;
				}
				this.bInIdleState = false;
				this.Selector.Reset(this.WaveInfo);
				this.repeatCount++;
			}
			if ((long)this.curTick < (long)((ulong)this.WaveInfo.dwStartWatiTick))
			{
				return SoldierSpawnResult.ShouldWaitStart;
			}
			if ((long)(this.curTick - this.firstTick) >= (long)((ulong)this.WaveInfo.dwRepeatTimeTick) && this.WaveInfo.dwRepeatTimeTick > 0u && (!this.Region.bForceCompleteSpawn || (this.Region.bForceCompleteSpawn && this.Selector.isFinished)))
			{
				return SoldierSpawnResult.Finish;
			}
			if (this.curTick - this.preSpawnTick < MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval)
			{
				return SoldierSpawnResult.ShouldWaitSoldierInterval;
			}
			if (!this.Selector.isFinished)
			{
				if (SoldierWave.ms_updatedFrameNum < Singleton<FrameSynchr>.instance.CurFrameNum)
				{
					uint num = this.Selector.NextSoldierID();
					DebugHelper.Assert(num != 0u);
					this.SpawnSoldier(num);
					this.preSpawnTick = this.curTick;
					SoldierWave.ms_updatedFrameNum = Singleton<FrameSynchr>.instance.CurFrameNum;
					if (this.Region != null && this.Selector.TotalCount == 1)
					{
						SpawnSoldierParam spawnSoldierParam = new SpawnSoldierParam(this.Region);
						Singleton<GameEventSys>.instance.SendEvent<SpawnSoldierParam>(GameEventDef.Event_SpawnSoldier, ref spawnSoldierParam);
					}
				}
				return SoldierSpawnResult.ShouldWaitSoldierInterval;
			}
			this.bInIdleState = true;
			this.idleTick = this.curTick;
			if (this.WaveInfo.dwRepeatNum == 0u || (long)this.repeatCount < (long)((ulong)this.WaveInfo.dwRepeatNum))
			{
				return SoldierSpawnResult.ShouldWaitInterval;
			}
			return SoldierSpawnResult.Finish;
		}

		private void SpawnSoldier(uint SoldierID)
		{
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int)SoldierID);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return;
			}
			string path = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo);
			CActorInfo actorInfo = CActorInfo.GetActorInfo(path, enResourceType.BattleScene);
			if (actorInfo)
			{
				Transform transform = this.Region.transform;
				COM_PLAYERCAMP campType = this.Region.CampType;
				ActorMeta actorMeta = default(ActorMeta);
				ActorMeta actorMeta2 = actorMeta;
				actorMeta2.ConfigId = (int)SoldierID;
				actorMeta2.ActorType = ActorTypeDef.Actor_Type_Monster;
				actorMeta2.ActorCamp = campType;
				actorMeta = actorMeta2;
				VInt3 vInt = (VInt3)transform.position;
				VInt3 vInt2 = (VInt3)transform.forward;
				PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
				if (!Singleton<GameObjMgr>.GetInstance().TryGetFromCache(ref poolObjHandle, ref actorMeta))
				{
					poolObjHandle = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, vInt, vInt2, false, true);
					if (poolObjHandle)
					{
						poolObjHandle.handle.InitActor();
						poolObjHandle.handle.PrepareFight();
						Singleton<GameObjMgr>.instance.AddActor(poolObjHandle);
						poolObjHandle.handle.StartFight();
					}
				}
				else
				{
					ActorRoot handle = poolObjHandle.handle;
					handle.TheActorMeta.ActorCamp = actorMeta.ActorCamp;
					handle.ReactiveActor(vInt, vInt2);
				}
				if (poolObjHandle)
				{
					if (this.Region.AttackRoute != null)
					{
						poolObjHandle.handle.ActorControl.AttackAlongRoute(this.Region.AttackRoute.GetComponent<WaypointsHolder>());
					}
					else if (this.Region.finalTarget != null)
					{
						FrameCommand<AttackPositionCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
						frameCommand.cmdId = 1u;
						frameCommand.cmdData.WorldPos = new VInt3(this.Region.finalTarget.transform.position);
						poolObjHandle.handle.ActorControl.CmdAttackMoveToDest(frameCommand, frameCommand.cmdData.WorldPos);
					}
					if (!this.isCannonNotified && this.WaveInfo.bType == 1 && this.Region != null && (this.Region.RouteType == RES_SOLDIER_ROUTE_TYPE.RES_SOLDIER_ROUTE_MID || this.Region.RouteType == RES_SOLDIER_ROUTE_TYPE.RES_SOLDIER_ROUTE_NONE))
					{
						KillNotify theKillNotify = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
						Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
						if (theKillNotify != null && hostPlayer != null)
						{
							bool flag = hostPlayer.PlayerCamp == poolObjHandle.handle.TheActorMeta.ActorCamp;
							if (flag)
							{
								KillInfo killInfo = new KillInfo((hostPlayer.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? KillNotify.red_cannon_icon : KillNotify.blue_cannon_icon, null, KillDetailInfoType.Info_Type_Cannon_Spawned, flag, false, ActorTypeDef.Invalid, false);
								theKillNotify.AddKillInfo(ref killInfo);
								this.isCannonNotified = true;
							}
						}
					}
					if (this.WaveInfo.bType == 2)
					{
						if (!this.isNotifiedBigDragonSoldier && this.Region != null && (this.Region.RouteType == RES_SOLDIER_ROUTE_TYPE.RES_SOLDIER_ROUTE_MID || this.Region.RouteType == RES_SOLDIER_ROUTE_TYPE.RES_SOLDIER_ROUTE_NONE))
						{
							KillNotify theKillNotify2 = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
							Player hostPlayer2 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
							if (theKillNotify2 != null && hostPlayer2 != null)
							{
								bool bSrcAllies = hostPlayer2.PlayerCamp == poolObjHandle.handle.TheActorMeta.ActorCamp;
								KillInfo killInfo2 = new KillInfo(KillNotify.soldier_bigdragon_icon, null, KillDetailInfoType.Info_Type_Soldier_BigDragon, bSrcAllies, false, ActorTypeDef.Invalid, false);
								theKillNotify2.AddKillInfo(ref killInfo2);
								this.isNotifiedBigDragonSoldier = true;
							}
						}
					}
					else if (this.isNotifiedBigDragonSoldier)
					{
						this.isNotifiedBigDragonSoldier = false;
					}
				}
			}
			SoldierWaveParam soldierWaveParam = new SoldierWaveParam(this.Index, this.repeatCount, this.Region.GetNextRepeatTime(false), this);
			Singleton<GameEventSys>.instance.SendEvent<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, ref soldierWaveParam);
		}
	}
}
