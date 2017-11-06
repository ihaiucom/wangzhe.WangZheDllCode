using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class MapWrapper : MonoBehaviour, IUpdateLogic
	{
		public struct ObjTriggerKeyValuePair
		{
			public GameObject obj;

			public ListView<AreaEventTrigger> triggers;
		}

		[FriendlyName("刷兵总数限制上限")]
		public int SoldierOverNumUpper;

		[FriendlyName("刷兵总数限制下限")]
		public int SoldierOverNumLower;

		private ListView<ReviveRegion> reviveAreas = new ListView<ReviveRegion>();

		private ListView<SoldierRegion> soldierAreas = new ListView<SoldierRegion>();

		private ListView<CommonSpawnGroup> commonSpawnGroups = new ListView<CommonSpawnGroup>();

		private ListView<SpawnGroup> spawnGroups = new ListView<SpawnGroup>();

		private ListView<WaypointsHolder> waypointsList = new ListView<WaypointsHolder>();

		private ListView<AreaEventTrigger> triggerList = new ListView<AreaEventTrigger>();

		private ListView<DynamicChannel> channelList = new ListView<DynamicChannel>();

		public List<MapWrapper.ObjTriggerKeyValuePair> objTriggerMultiMap = new List<MapWrapper.ObjTriggerKeyValuePair>();

		private bool m_bSoldierOverNum;

		private List<PoolObjHandle<ActorRoot>> TrueMen;

		private int SoldierActivateDelay;

		private int SoldierActivateCountDelay1;

		private int SoldierActivateCountDelay2;

		private int SoldierActivateDelaySeq = -1;

		private int SoldierActivateCountDelay1Seq = -1;

		private int SoldierActivateCountDelay2Seq = -1;

		private int WelcomeDelaySeq = -1;

		public ActionHelper ActionHelper
		{
			get
			{
				return base.GetComponent<ActionHelper>();
			}
		}

		private void Awake()
		{
			Singleton<BattleLogic>.GetInstance().SetupMap(this);
		}

		private void Start()
		{
			FuncRegion[] componentsInChildren = base.GetComponentsInChildren<FuncRegion>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				FuncRegion funcRegion = componentsInChildren[i];
				if (!(funcRegion == null) && funcRegion.enabled && funcRegion.gameObject.activeInHierarchy && funcRegion.gameObject.activeSelf)
				{
					if (funcRegion is ReviveRegion)
					{
						this.reviveAreas.Add(funcRegion as ReviveRegion);
					}
					else if (funcRegion is SoldierRegion)
					{
						this.soldierAreas.Add(funcRegion as SoldierRegion);
					}
					else if (funcRegion is CommonSpawnGroup)
					{
						this.commonSpawnGroups.Add(funcRegion as CommonSpawnGroup);
					}
					else if (funcRegion is SpawnGroup)
					{
						this.spawnGroups.Add(funcRegion as SpawnGroup);
					}
					else if (funcRegion is WaypointsHolder)
					{
						this.waypointsList.Add(funcRegion as WaypointsHolder);
					}
					else if (funcRegion is AreaEventTrigger)
					{
						AreaEventTrigger item = funcRegion as AreaEventTrigger;
						this.triggerList.Add(item);
						GameObject gameObject = funcRegion.gameObject;
						ListView<AreaEventTrigger> listView = null;
						for (int j = this.objTriggerMultiMap.get_Count() - 1; j >= 0; j--)
						{
							MapWrapper.ObjTriggerKeyValuePair objTriggerKeyValuePair = this.objTriggerMultiMap.get_Item(j);
							if (objTriggerKeyValuePair.obj == gameObject)
							{
								listView = objTriggerKeyValuePair.triggers;
								break;
							}
						}
						if (listView == null)
						{
							listView = new ListView<AreaEventTrigger>();
							this.objTriggerMultiMap.Add(new MapWrapper.ObjTriggerKeyValuePair
							{
								obj = gameObject,
								triggers = listView
							});
						}
						listView.Add(item);
					}
					else if (funcRegion is DynamicChannel)
					{
						this.channelList.Add(funcRegion as DynamicChannel);
					}
				}
			}
			this.TrueMen = new List<PoolObjHandle<ActorRoot>>(Singleton<GameObjMgr>.GetInstance().SoldierActors);
		}

		private void OnDestroy()
		{
		}

		public void Startup()
		{
			if (MTileHandlerHelper.Instance != null)
			{
				MTileHandlerHelper.Instance.UpdateLogic();
			}
			SoldierWave.ms_updatedFrameNum = 0u;
			ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Startup();
			}
			ListView<CommonSpawnGroup>.Enumerator enumerator2 = this.commonSpawnGroups.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				enumerator2.Current.Startup();
			}
			ListView<SpawnGroup>.Enumerator enumerator3 = this.spawnGroups.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				enumerator3.Current.Startup();
			}
			ListView<AreaEventTrigger>.Enumerator enumerator4 = this.triggerList.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				enumerator4.Current.Startup();
			}
			ListView<DynamicChannel>.Enumerator enumerator5 = this.channelList.GetEnumerator();
			while (enumerator5.MoveNext())
			{
				enumerator5.Current.Startup();
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			this.ClearSoldierActivateTimers();
			if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
			{
				this.SoldierActivateDelay = curLvelContext.m_soldierActivateDelay;
				this.SoldierActivateCountDelay1 = curLvelContext.m_soldierActivateCountDelay1;
				this.SoldierActivateCountDelay2 = curLvelContext.m_soldierActivateCountDelay2;
				this.WelcomeDelaySeq = Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnWelcomeDelay), true);
			}
			if (this.SoldierActivateDelay > 0)
			{
				this.SoldierActivateDelaySeq = Singleton<CTimerManager>.instance.AddTimer(this.SoldierActivateDelay, 1, new CTimer.OnTimeUpHandler(this.OnSoldierActivateDelay), true);
			}
			if (this.SoldierActivateCountDelay1 > 0)
			{
				this.SoldierActivateCountDelay1Seq = Singleton<CTimerManager>.instance.AddTimer(this.SoldierActivateCountDelay1, 1, new CTimer.OnTimeUpHandler(this.OnSoldierActivateCountDelay1), true);
			}
			if (this.SoldierActivateCountDelay2 > 0)
			{
				this.SoldierActivateCountDelay2Seq = Singleton<CTimerManager>.instance.AddTimer(this.SoldierActivateCountDelay2, 1, new CTimer.OnTimeUpHandler(this.OnSoldierActivateCountDelay2), true);
			}
		}

		public ListView<SpawnGroup> GetSpawnGroups()
		{
			return this.spawnGroups;
		}

		private void OnWelcomeDelay(int inTimeSeq)
		{
			KillDetailInfo killDetailInfo = new KillDetailInfo();
			killDetailInfo.Type = KillDetailInfoType.Info_Type_Game_Start_Wel;
			Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
			Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
			this.WelcomeDelaySeq = -1;
		}

		private void OnSoldierActivateDelay(int inTimeSeq)
		{
			KillDetailInfo killDetailInfo = new KillDetailInfo();
			killDetailInfo.Type = KillDetailInfoType.Info_Type_Soldier_Activate;
			Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
			Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
			this.SoldierActivateDelaySeq = -1;
		}

		private void OnSoldierActivateCountDelay1(int inTimeSeq)
		{
			KillDetailInfo killDetailInfo = new KillDetailInfo();
			killDetailInfo.Type = KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3;
			Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
			Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
			this.SoldierActivateCountDelay1Seq = -1;
		}

		private void OnSoldierActivateCountDelay2(int inTimeSeq)
		{
			KillDetailInfo killDetailInfo = new KillDetailInfo();
			killDetailInfo.Type = KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5;
			Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
			Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
			this.SoldierActivateCountDelay2Seq = -1;
		}

		private void ClearSoldierActivateTimers()
		{
			this.SoldierActivateDelay = 0;
			this.SoldierActivateCountDelay1 = 0;
			this.SoldierActivateCountDelay2 = 0;
			if (this.SoldierActivateDelaySeq >= 0)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.SoldierActivateDelaySeq);
				this.SoldierActivateDelaySeq = -1;
			}
			if (this.SoldierActivateCountDelay1Seq >= 0)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.SoldierActivateCountDelay1Seq);
				this.SoldierActivateCountDelay1Seq = -1;
			}
			if (this.SoldierActivateCountDelay2Seq >= 0)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.SoldierActivateCountDelay2Seq);
				this.SoldierActivateCountDelay2Seq = -1;
			}
			if (this.WelcomeDelaySeq >= 0)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.WelcomeDelaySeq);
				this.WelcomeDelaySeq = -1;
			}
		}

		public void Reset()
		{
			this.ClearSoldierActivateTimers();
			ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Stop();
			}
			ListView<CommonSpawnGroup>.Enumerator enumerator2 = this.commonSpawnGroups.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				enumerator2.Current.Stop();
			}
			ListView<SpawnGroup>.Enumerator enumerator3 = this.spawnGroups.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				enumerator3.Current.Stop();
			}
			ListView<AreaEventTrigger>.Enumerator enumerator4 = this.triggerList.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				enumerator4.Current.Stop();
			}
		}

		private void CheckSoldierOverNumUpper()
		{
			if (this.SoldierOverNumUpper <= 0 || this.SoldierOverNumLower < 0)
			{
				return;
			}
			DebugHelper.Assert(!this.m_bSoldierOverNum);
			int count = this.TrueMen.get_Count();
			if (count >= this.SoldierOverNumUpper)
			{
				this.m_bSoldierOverNum = true;
			}
		}

		private void CheckSoldierOverNumLower()
		{
			if (this.SoldierOverNumUpper <= 0 || this.SoldierOverNumLower < 0)
			{
				return;
			}
			DebugHelper.Assert(this.m_bSoldierOverNum);
			int count = this.TrueMen.get_Count();
			if (count <= this.SoldierOverNumLower)
			{
				this.m_bSoldierOverNum = false;
			}
		}

		public bool DoesSoldierOverNum()
		{
			return this.m_bSoldierOverNum;
		}

		public void UpdateLogic(int delta)
		{
			Singleton<SceneManagement>.GetInstance().UpdateDirtyNodes();
			if (!this.m_bSoldierOverNum)
			{
				bool flag = true;
				int count = this.soldierAreas.Count;
				for (int i = 0; i < count; i++)
				{
					SoldierRegion soldierRegion = this.soldierAreas[i];
					if (soldierRegion.isStartup)
					{
						SoldierSpawnResult soldierSpawnResult = soldierRegion.UpdateLogicSpec(delta);
						flag &= (soldierSpawnResult != SoldierSpawnResult.ShouldWaitSoldierInterval);
					}
				}
				if (flag)
				{
					this.CheckSoldierOverNumUpper();
				}
			}
			else
			{
				this.CheckSoldierOverNumLower();
			}
			int count2 = this.commonSpawnGroups.Count;
			for (int j = 0; j < count2; j++)
			{
				CommonSpawnGroup commonSpawnGroup = this.commonSpawnGroups[j];
				if (commonSpawnGroup.isStartup)
				{
					commonSpawnGroup.UpdateLogic(delta);
				}
			}
			for (int k = 0; k < this.spawnGroups.Count; k++)
			{
				SpawnGroup spawnGroup = this.spawnGroups[k];
				if (spawnGroup.isStartup)
				{
					spawnGroup.UpdateLogic(delta);
				}
			}
			int count3 = this.objTriggerMultiMap.get_Count();
			for (int l = 0; l < count3; l++)
			{
				MapWrapper.ObjTriggerKeyValuePair objTriggerKeyValuePair = this.objTriggerMultiMap.get_Item(l);
				GameObject obj = objTriggerKeyValuePair.obj;
				ListView<AreaEventTrigger> triggers = objTriggerKeyValuePair.triggers;
				if (obj != null && obj.activeSelf && (ulong)(Singleton<FrameSynchr>.instance.CurFrameNum % 4u) == (ulong)((long)(l % 4)))
				{
					bool flag2 = false;
					int count4 = triggers.Count;
					for (int m = 0; m < count4; m++)
					{
						AreaEventTrigger areaEventTrigger = triggers[m];
						if (areaEventTrigger != null && areaEventTrigger.isStartup)
						{
							areaEventTrigger.UpdateLogic(delta * 4);
							flag2 |= areaEventTrigger.bDoDeactivating;
						}
					}
					if (flag2)
					{
						for (int n = 0; n < count4; n++)
						{
							AreaEventTrigger areaEventTrigger2 = triggers[n];
							if (areaEventTrigger2 != null)
							{
								areaEventTrigger2.DoSelfDeactivating();
							}
						}
					}
				}
			}
		}

		public bool GetRevivePosDir(ref ActorMeta actorMeta, bool bGiveBirth, out VInt3 outPosWorld, out VInt3 outDirWorld)
		{
			outPosWorld = VInt3.zero;
			outDirWorld = VInt3.forward;
			if (Singleton<GamePlayerCenter>.instance.GetPlayer(actorMeta.PlayerId) == null)
			{
				return false;
			}
			ListView<ReviveRegion>.Enumerator enumerator = this.reviveAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current == null) && actorMeta.ActorCamp == enumerator.Current.CampType)
				{
					IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
					int num = actorDataProvider.Fast_GetActorServerDataBornIndex(ref actorMeta);
					Transform transform = enumerator.Current.transform;
					if (!bGiveBirth)
					{
						uint num2 = (uint)enumerator.Current.SubRevivePlaces.Length;
						if (!enumerator.Current.OnlyBirth)
						{
							num2 += 1u;
							int num3 = (int)FrameRandom.Random(num2);
							if (0 < num3 && enumerator.Current.SubRevivePlaces[num3 - 1] != null)
							{
								transform = enumerator.Current.SubRevivePlaces[num3 - 1].transform;
							}
						}
						else if (num2 >= 1u)
						{
							int num4 = (int)FrameRandom.Random(num2);
							if (enumerator.Current.SubRevivePlaces[num4] != null)
							{
								transform = enumerator.Current.SubRevivePlaces[num4].transform;
							}
						}
					}
					Transform transform2 = null;
					if (transform != null)
					{
						if (num < transform.childCount)
						{
							transform2 = transform.GetChild(num);
						}
						else
						{
							transform2 = transform;
						}
					}
					if (transform2 != null)
					{
						outPosWorld = (VInt3)transform2.position;
						outDirWorld = (VInt3)transform2.forward;
						return true;
					}
				}
			}
			return false;
		}

		public ListView<WaypointsHolder> GetWaypointsList(COM_PLAYERCAMP inCampType)
		{
			ListView<WaypointsHolder> listView = new ListView<WaypointsHolder>();
			ListView<WaypointsHolder>.Enumerator enumerator = this.waypointsList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CampType == inCampType)
				{
					listView.Add(enumerator.Current);
				}
			}
			return listView;
		}

		public SoldierRegion GetSoldirRegion()
		{
			MapWrapperAdd component = base.GetComponent<MapWrapperAdd>();
			if (component != null)
			{
				return component.CareSoldierRegion;
			}
			return null;
		}

		public void EnableSoldierRegion(bool bEnable)
		{
			ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current == null))
				{
					if (bEnable)
					{
						enumerator.Current.Startup();
					}
					else
					{
						enumerator.Current.Stop();
					}
				}
			}
		}

		public void EnableSoldierRegion(bool bEnable, int inWaveId)
		{
			if (inWaveId <= 0)
			{
				this.EnableSoldierRegion(bEnable);
			}
			else
			{
				ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null && enumerator.Current.WaveID == inWaveId)
					{
						if (bEnable)
						{
							enumerator.Current.Startup();
						}
						else
						{
							enumerator.Current.Stop();
						}
					}
				}
			}
		}

		public void EnableSoldierRegion(bool bEnable, SoldierRegion inSoldierRegion)
		{
			if (inSoldierRegion == null || !this.soldierAreas.Contains(inSoldierRegion))
			{
				return;
			}
			if (bEnable)
			{
				inSoldierRegion.Startup();
			}
			else
			{
				inSoldierRegion.Stop();
			}
		}

		public void ResetSoldierRegion()
		{
			ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.ResetRegion();
			}
		}

		public SoldierRegion GetSoldierRegionByRoute(COM_PLAYERCAMP camp, int routeID)
		{
			ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SoldierRegion current = enumerator.Current;
				if (current.CampType == camp && current.RouteID == routeID)
				{
					return current;
				}
			}
			return null;
		}

		public ListView<SoldierRegion> GetSoldierRegionsByCamp(COM_PLAYERCAMP camp)
		{
			ListView<SoldierRegion> listView = new ListView<SoldierRegion>();
			ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SoldierRegion current = enumerator.Current;
				if (current.CampType == camp)
				{
					listView.Add(current);
				}
			}
			return listView;
		}
	}
}
