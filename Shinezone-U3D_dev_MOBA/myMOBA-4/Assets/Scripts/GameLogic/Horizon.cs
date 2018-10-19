using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class Horizon : IUpdateLogic
	{
		public enum EnableMethod
		{
			DisableAll,
			EnableAll,
			EnableMarkNoMist,
			INVALID
		}

		public const byte UPDATE_CYCLE = 8;

		private bool _fighting;

		private bool _enabled;

		private static int GlobalSightSqr_;

		private static int _globalSight;

		private static int exposeRadius;

		private static int exposeDurationNormal;

		private static int exposeDurationHero;

		private static int showmarkDurationHero;

		private static int fakeSightRadius;

		private static int soldierSightRadius;

		private static int fowTowerSightRadius;

		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				if (value != this._enabled)
				{
					this._enabled = value;
					List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
					int count = gameActors.Count;
					for (int i = 0; i < count; i++)
					{
						gameActors[i].handle.HorizonMarker.SetEnabled(this._enabled);
					}
				}
			}
		}

		public Horizon()
		{
			this._fighting = false;
			this._enabled = false;
		}

		public static int QueryGlobalSight()
		{
			if (Horizon._globalSight == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(56u);
				if (dataByKey != null)
				{
					Horizon._globalSight = (int)dataByKey.dwConfValue;
					Horizon.GlobalSightSqr_ = Horizon._globalSight * Horizon._globalSight;
				}
			}
			return Horizon._globalSight;
		}

		public static int QueryExposeRadius()
		{
			if (Horizon.exposeRadius == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(222u);
				if (dataByKey != null)
				{
					Horizon.exposeRadius = (int)dataByKey.dwConfValue;
				}
			}
			return Horizon.exposeRadius;
		}

		public static int QueryExposeDurationNormal()
		{
			if (Horizon.exposeDurationNormal == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(223u);
				if (dataByKey != null)
				{
					Horizon.exposeDurationNormal = (int)dataByKey.dwConfValue;
				}
			}
			return Horizon.exposeDurationNormal;
		}

		public static int QueryExposeDurationHero()
		{
			if (Horizon.exposeDurationHero == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(224u);
				if (dataByKey != null)
				{
					Horizon.exposeDurationHero = (int)dataByKey.dwConfValue;
				}
			}
			return Horizon.exposeDurationHero;
		}

		public static int QueryAttackShowMarkDuration()
		{
			if (Horizon.showmarkDurationHero == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(267u);
				if (dataByKey != null)
				{
					Horizon.showmarkDurationHero = (int)dataByKey.dwConfValue;
				}
			}
			return Horizon.showmarkDurationHero;
		}

		public static int QueryMainActorFakeSightRadius()
		{
			Horizon.fakeSightRadius = 0;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null)
			{
				Horizon.fakeSightRadius = curLvelContext.m_fakeSightRange;
			}
			return Horizon.fakeSightRadius;
		}

		public static int QuerySoldierSightRadius()
		{
			if (Horizon.soldierSightRadius == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(257u);
				if (dataByKey != null)
				{
					Horizon.soldierSightRadius = (int)dataByKey.dwConfValue;
				}
			}
			return Horizon.soldierSightRadius;
		}

		public static int QueryFowTowerSightRadius()
		{
			if (Horizon.fowTowerSightRadius == 0 && GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(258u);
				if (dataByKey != null)
				{
					Horizon.fowTowerSightRadius = (int)dataByKey.dwConfValue;
				}
			}
			return Horizon.fowTowerSightRadius;
		}

		public void FightStart()
		{
			this._fighting = true;
			Horizon.QueryGlobalSight();
			this._enabled = (Singleton<BattleLogic>.instance.GetCurLvelContext().m_horizonEnableMethod == Horizon.EnableMethod.EnableAll);
		}

		public void FightOver()
		{
			this.Enabled = false;
			Horizon._globalSight = 0;
			Horizon.GlobalSightSqr_ = 0;
		}

		public void UpdateLogic(int delta)
		{
			if (this._enabled && this._fighting)
			{
				uint num = Singleton<FrameSynchr>.instance.CurFrameNum % 8u;
				GameObjMgr instance = Singleton<GameObjMgr>.GetInstance();
				List<PoolObjHandle<ActorRoot>> gameActors = instance.GameActors;
				int count = gameActors.Count;
				for (int i = 0; i < count; i++)
				{
					if (gameActors[i])
					{
						ActorRoot handle = gameActors[i].handle;
						if (handle.ObjID % 8u == num && (!handle.ActorControl.IsDeadState || handle.TheStaticData.TheBaseAttribute.DeadControl))
						{
							for (int j = 0; j < 3; j++)
							{
								if (j != (int)handle.TheActorMeta.ActorCamp)
								{
									COM_PLAYERCAMP actorCamp = handle.TheActorMeta.ActorCamp;
									List<PoolObjHandle<ActorRoot>> campActors = instance.GetCampActors((COM_PLAYERCAMP)j);
									int count2 = campActors.Count;
									for (int k = 0; k < count2; k++)
									{
										if (campActors[k])
										{
											ActorRoot handle2 = campActors[k].handle;
											if (!handle2.HorizonMarker.IsSightVisited(actorCamp))
											{
												long num2 = (long)Horizon.GlobalSightSqr_;
												if (handle.HorizonMarker.SightRadius != 0)
												{
													num2 = (long)handle.HorizonMarker.SightRadius;
													num2 *= num2;
												}
												if ((handle2.location - handle.location).sqrMagnitudeLong2D < num2)
												{
													handle2.HorizonMarker.VisitSight(actorCamp);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
