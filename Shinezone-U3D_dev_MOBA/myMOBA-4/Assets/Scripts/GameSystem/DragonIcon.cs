using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.Sound;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class DragonIcon
	{
		private class DragonNode
		{
			public GameObject mmDragonNode_ugui;

			public GameObject bmDragonNode_ugui;

			public GameObject mmDragonNode_3dui;

			public GameObject dragon_live_icon_smallMap;

			public GameObject dragon_dead_icon_smallMap;

			public GameObject bmDragonNode_3dui;

			public GameObject dragon_live_icon_bigMap;

			public GameObject dragon_dead_icon_bigMap;

			public Text cdTxtInMini;

			public Text cdTxtInBig;

			public uint objid;

			public byte type;

			public byte optType;

			public SpawnGroup spawnGroup;

			private Vector3 cachePos = Vector3.zero;

			public DragonNode(GameObject mmNode, GameObject bmNode, GameObject mmNode_3dui, GameObject bmNode_3dui, string path, byte type, byte optType = 0)
			{
				this.spawnGroup = null;
				this._init(mmNode.transform.Find(path).gameObject, bmNode.transform.Find(path).gameObject, mmNode_3dui.transform.Find(path).gameObject, bmNode_3dui.transform.Find(path).gameObject, type, optType);
			}

			public void _init(GameObject mmDNode_ugui, GameObject bmDNode_ugui, GameObject mmDNode_3dui, GameObject bmDNode_3dui, byte type, byte optType)
			{
				this.mmDragonNode_ugui = mmDNode_ugui;
				this.bmDragonNode_ugui = bmDNode_ugui;
				this.type = type;
				this.optType = optType;
				this.mmDragonNode_3dui = mmDNode_3dui;
				this.dragon_live_icon_smallMap = this.mmDragonNode_3dui.transform.Find("live").gameObject;
				this.dragon_dead_icon_smallMap = this.mmDragonNode_3dui.transform.Find("dead").gameObject;
				this.cdTxtInMini = Utility.GetComponetInChild<Text>(mmDNode_ugui, "cdTxt");
				this.bmDragonNode_3dui = bmDNode_3dui;
				this.dragon_live_icon_bigMap = this.bmDragonNode_3dui.transform.Find("live").gameObject;
				this.dragon_dead_icon_bigMap = this.bmDragonNode_3dui.transform.Find("dead").gameObject;
				this.cdTxtInBig = Utility.GetComponetInChild<Text>(bmDNode_ugui, "cdTxt");
				if (type == 7)
				{
					MiniMapSysUT.SetMapElement_EventParam(bmDNode_ugui, false, MinimapSys.ElementType.Dragon_3, 0u, 0u);
				}
				else if (type == 8)
				{
					MiniMapSysUT.SetMapElement_EventParam(bmDNode_ugui, false, MinimapSys.ElementType.Dragon_5_big, 0u, 0u);
				}
				else if (type == 9)
				{
					MiniMapSysUT.SetMapElement_EventParam(bmDNode_ugui, false, MinimapSys.ElementType.Dragon_5_small, 0u, 0u);
				}
			}

			public void Clear()
			{
				this.spawnGroup = null;
				this.mmDragonNode_ugui = (this.bmDragonNode_ugui = null);
				this.dragon_live_icon_smallMap = null;
				this.dragon_dead_icon_smallMap = null;
				this.mmDragonNode_3dui = null;
				this.cdTxtInMini = null;
				this.dragon_live_icon_bigMap = null;
				this.dragon_dead_icon_bigMap = null;
				this.bmDragonNode_3dui = null;
				this.cdTxtInBig = null;
				this.objid = 0u;
				this.type = 0;
				this.optType = 0;
				this.cachePos = Vector3.zero;
			}

			public void SetData(Vector3 worldpos, int type, uint id, bool b5v5 = false, bool bUseCache = true, bool bRefreshCache = false, bool bRefreshMM = true)
			{
				if (this.mmDragonNode_3dui != null && bRefreshMM)
				{
					if (this.mmDragonNode_ugui != null)
					{
						RectTransform rectTransform = this.mmDragonNode_ugui.transform as RectTransform;
						rectTransform.anchoredPosition = new Vector2(worldpos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, worldpos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y);
					}
					Vector3 position = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldpos, true);
					this.mmDragonNode_3dui.transform.position = position;
				}
				if (this.bmDragonNode_3dui != null)
				{
					if (this.bmDragonNode_ugui != null)
					{
						RectTransform rectTransform2 = this.bmDragonNode_ugui.transform as RectTransform;
						rectTransform2.anchoredPosition = new Vector2(worldpos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.x, worldpos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.y);
					}
					if (bUseCache)
					{
						if (this.cachePos == Vector3.zero || bRefreshCache)
						{
							this.cachePos = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldpos, false);
						}
						this.bmDragonNode_3dui.transform.position = this.cachePos;
					}
					else
					{
						this.bmDragonNode_3dui.transform.position = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldpos, false);
					}
				}
				this.objid = id;
			}

			public void ShowDead(bool bDead)
			{
				this.mmDragonNode_3dui.CustomSetActive(true);
				this.bmDragonNode_3dui.CustomSetActive(true);
				this.mmDragonNode_ugui.CustomSetActive(true);
				this.bmDragonNode_ugui.CustomSetActive(true);
				this.dragon_live_icon_smallMap.CustomSetActive(!bDead);
				this.dragon_live_icon_bigMap.CustomSetActive(!bDead);
				this.dragon_dead_icon_smallMap.CustomSetActive(bDead);
				this.dragon_dead_icon_bigMap.CustomSetActive(bDead);
			}

			public void Recycle()
			{
				this.mmDragonNode_3dui.CustomSetActive(false);
				this.bmDragonNode_3dui.CustomSetActive(false);
				this.mmDragonNode_ugui.CustomSetActive(false);
				this.bmDragonNode_ugui.CustomSetActive(false);
			}

			public bool IsType(byte type)
			{
				return this.type == type || this.optType == type;
			}

			public void ValidateCD()
			{
				if (null == this.spawnGroup)
				{
					return;
				}
				if (null == this.cdTxtInMini || null == this.cdTxtInBig)
				{
					return;
				}
				bool flag = this.spawnGroup.IsCountingDown();
				bool isWatching = Singleton<WatchController>.GetInstance().IsWatching;
				bool flag2 = flag && (isWatching || (!isWatching && this.spawnGroup.GetSpawnTimer() <= 60000));
				if (flag2)
				{
					this.cdTxtInMini.gameObject.CustomSetActive(true);
					this.cdTxtInBig.gameObject.CustomSetActive(true);
					string text = (this.spawnGroup.GetSpawnTimer() / 1000).ToString();
					this.cdTxtInMini.text = text;
					this.cdTxtInBig.text = text;
				}
				else
				{
					this.cdTxtInMini.gameObject.CustomSetActive(false);
					this.cdTxtInBig.gameObject.CustomSetActive(false);
				}
			}
		}

		public const string Dragon_born = "Dragon_born";

		public const string Dragon_dead = "Dragon_dead";

		private ListView<DragonIcon.DragonNode> node_ary = new ListView<DragonIcon.DragonNode>();

		private bool m_b5v5;

		private int m_cdTimer;

		private static string dragonBornEffect = "Prefab_Skill_Effects/tongyong_effects/Indicator/blin_01_c.prefab";

		public static void Check_Dragon_Born_Evt(ActorRoot actor, bool bThrow_Born_Evt)
		{
			if (actor == null)
			{
				return;
			}
			byte actorSubSoliderType = actor.ActorControl.GetActorSubSoliderType();
			if (actorSubSoliderType != 8 && actorSubSoliderType != 9 && actorSubSoliderType != 7 && actorSubSoliderType != 13)
			{
				return;
			}
			if (bThrow_Born_Evt)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ActorRoot>("Dragon_born", actor);
			}
			else
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ActorRoot>("Dragon_dead", actor);
			}
		}

		public void Init(GameObject mmNode_ugui, GameObject bmNode_ugui, GameObject mmNode_3dui, GameObject bmNode_3dui, bool b5V5)
		{
			this.m_b5v5 = b5V5;
			Singleton<EventRouter>.GetInstance().AddEventHandler<ActorRoot>("Dragon_born", new Action<ActorRoot>(this.onDragon_Born));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ActorRoot>("Dragon_dead", new Action<ActorRoot>(this.onDragon_Dead));
			Singleton<GameEventSys>.instance.AddEventHandler<NextSpawnGroupsParam>(GameEventDef.Event_NextSpawnGroups, new RefAction<NextSpawnGroupsParam>(this.OnNextSpawnGroups));
			for (int i = 0; i < mmNode_ugui.transform.childCount; i++)
			{
				mmNode_ugui.transform.GetChild(i).gameObject.CustomSetActive(false);
			}
			for (int j = 0; j < bmNode_ugui.transform.childCount; j++)
			{
				bmNode_ugui.transform.GetChild(j).gameObject.CustomSetActive(false);
			}
			for (int k = 0; k < mmNode_3dui.transform.childCount; k++)
			{
				mmNode_3dui.transform.GetChild(k).gameObject.CustomSetActive(false);
			}
			for (int l = 0; l < bmNode_3dui.transform.childCount; l++)
			{
				bmNode_3dui.transform.GetChild(l).gameObject.CustomSetActive(false);
			}
			this.node_ary.Add(new DragonIcon.DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_3", 7, 0));
			this.node_ary.Add(new DragonIcon.DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_5_big", 8, 0));
			this.node_ary.Add(new DragonIcon.DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_5_small_1", 9, 13));
			this.node_ary.Add(new DragonIcon.DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_5_small_2", 9, 13));
			ListView<SpawnGroup> spawnGroups = Singleton<BattleLogic>.instance.mapLogic.GetSpawnGroups();
			if (spawnGroups != null)
			{
				for (int m = 0; m < spawnGroups.Count; m++)
				{
					SpawnGroup spawnGroup = spawnGroups[m];
					if (spawnGroup != null)
					{
						ActorMeta actorMeta = spawnGroup.TheActorsMeta[0];
						ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(actorMeta.ConfigId);
						if (dataCfgInfoByCurLevelDiff != null)
						{
							bool flag = dataCfgInfoByCurLevelDiff.bSoldierType == 8 || dataCfgInfoByCurLevelDiff.bSoldierType == 9 || dataCfgInfoByCurLevelDiff.bSoldierType == 7 || dataCfgInfoByCurLevelDiff.bSoldierType == 13;
							if (flag && !spawnGroup.bTriggerSpawn)
							{
								DragonIcon.DragonNode dragonNode = this.getDragonNode(dataCfgInfoByCurLevelDiff.bSoldierType);
								if (dragonNode != null)
								{
									dragonNode.spawnGroup = spawnGroup;
									dragonNode.SetData(spawnGroup.gameObject.transform.position, (int)dataCfgInfoByCurLevelDiff.bSoldierType, 0u, this.m_b5v5, true, true, true);
									dragonNode.ShowDead(true);
									MiniMapSysUT.RefreshMapPointerBig(dragonNode.bmDragonNode_3dui);
								}
							}
						}
					}
				}
			}
			this.m_cdTimer = Singleton<CTimerManager>.GetInstance().AddTimer(1000, 0, new CTimer.OnTimeUpHandler(this.OnCDTimer));
		}

		private void OnNextSpawnGroups(ref NextSpawnGroupsParam prm)
		{
			if (prm.OldGroup == null)
			{
				return;
			}
			for (int i = 0; i < this.node_ary.Count; i++)
			{
				DragonIcon.DragonNode dragonNode = this.node_ary[i];
				if (dragonNode != null && dragonNode.spawnGroup == prm.OldGroup)
				{
					dragonNode.spawnGroup = prm.NextGroups[0];
				}
			}
			ActorMeta actorMeta = prm.OldGroup.TheActorsMeta[0];
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(actorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return;
			}
			if (dataCfgInfoByCurLevelDiff.bSoldierType == 9)
			{
				KillDetailInfo killDetailInfo = new KillDetailInfo();
				killDetailInfo.bSelfCamp = true;
				killDetailInfo.Type = KillDetailInfoType.Info_Type_5V5SmallDragon_Suicide;
				Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
				KillDetailInfo killDetailInfo2 = new KillDetailInfo();
				killDetailInfo2.bSelfCamp = true;
				killDetailInfo2.Type = KillDetailInfoType.Info_Type_5V5SmallDragon_Enter;
				Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo2);
			}
		}

		public void Clear()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_cdTimer);
			for (int i = 0; i < this.node_ary.Count; i++)
			{
				DragonIcon.DragonNode dragonNode = this.node_ary[i];
				if (dragonNode != null)
				{
					dragonNode.Clear();
				}
			}
			this.node_ary.Clear();
			this.node_ary = null;
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<ActorRoot>("Dragon_born", new Action<ActorRoot>(this.onDragon_Born));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<ActorRoot>("Dragon_dead", new Action<ActorRoot>(this.onDragon_Dead));
			Singleton<GameEventSys>.instance.RmvEventHandler<NextSpawnGroupsParam>(GameEventDef.Event_NextSpawnGroups, new RefAction<NextSpawnGroupsParam>(this.OnNextSpawnGroups));
			this.m_b5v5 = false;
		}

		private void OnCDTimer(int seq)
		{
			if (this.node_ary == null)
			{
				return;
			}
			for (int i = 0; i < this.node_ary.Count; i++)
			{
				this.node_ary[i].ValidateCD();
			}
		}

		private void onDragon_Dead(ActorRoot actor)
		{
			DragonIcon.DragonNode dragonNode = this.getDragonNode(actor.ObjID, actor.ActorControl.GetActorSubSoliderType());
			if (dragonNode != null)
			{
				dragonNode.ShowDead(actor.ActorControl.IsDeadState);
				dragonNode.objid = 0u;
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.IsFireHolePlayMode())
				{
					dragonNode.Recycle();
				}
			}
		}

		private void onDragon_Born(ActorRoot actor)
		{
			DragonIcon.DragonNode dragonNode = this.getDragonNode(actor.ObjID, actor.ActorControl.GetActorSubSoliderType());
			DebugHelper.Assert(dragonNode != null, "onDragon_Born mmDNode_ugui == null, check out...");
			if (dragonNode == null)
			{
				return;
			}
			dragonNode.SetData(actor.myTransform.position, (int)actor.ActorControl.GetActorSubSoliderType(), actor.ObjID, this.m_b5v5, true, true, true);
			dragonNode.ShowDead(actor.ActorControl.IsDeadState);
			byte actorSubSoliderType = actor.ActorControl.GetActorSubSoliderType();
			if (actorSubSoliderType == 8 || actorSubSoliderType == 9 || actorSubSoliderType == 13)
			{
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
				bool flag = theMinimapSys != null && theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
				if (flag)
				{
					Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
					if (currentCamera != null)
					{
						Vector3 v = currentCamera.WorldToScreenPoint((!flag) ? dragonNode.bmDragonNode_3dui.transform.position : dragonNode.mmDragonNode_3dui.transform.position);
						Singleton<CUIParticleSystem>.instance.AddParticle(DragonIcon.dragonBornEffect, 3f, v, null);
					}
				}
			}
			if (actorSubSoliderType == 8)
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_DaLong_VO_Refresh");
			}
			else if (actorSubSoliderType == 9)
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_XiaoLong_VO_Refresh");
			}
			else
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_BaoJun_VO_Anger");
			}
		}

		private DragonIcon.DragonNode getDragonNode(byte type = 0)
		{
			for (int i = 0; i < this.node_ary.Count; i++)
			{
				DragonIcon.DragonNode dragonNode = this.node_ary[i];
				if (dragonNode != null && dragonNode.IsType(type))
				{
					return dragonNode;
				}
			}
			return null;
		}

		private DragonIcon.DragonNode getDragonNode(uint objid, byte type)
		{
			for (int i = 0; i < this.node_ary.Count; i++)
			{
				DragonIcon.DragonNode dragonNode = this.node_ary[i];
				if (dragonNode != null && dragonNode.IsType(type) && dragonNode.objid == objid)
				{
					return dragonNode;
				}
			}
			for (int j = 0; j < this.node_ary.Count; j++)
			{
				DragonIcon.DragonNode dragonNode2 = this.node_ary[j];
				if (dragonNode2 != null && dragonNode2.IsType(type) && dragonNode2.objid == 0u)
				{
					dragonNode2.objid = objid;
					return dragonNode2;
				}
			}
			return null;
		}

		public void RefreshDragNode(bool bUseCache, bool bRefreshMM = false)
		{
			ListView<SpawnGroup> spawnGroups = Singleton<BattleLogic>.instance.mapLogic.GetSpawnGroups();
			if (spawnGroups != null)
			{
				for (int i = 0; i < spawnGroups.Count; i++)
				{
					SpawnGroup spawnGroup = spawnGroups[i];
					if (spawnGroup != null && !spawnGroup.bTriggerSpawn)
					{
						ActorMeta actorMeta = spawnGroup.TheActorsMeta[0];
						ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(actorMeta.ConfigId);
						if (dataCfgInfoByCurLevelDiff != null)
						{
							bool flag = dataCfgInfoByCurLevelDiff.bSoldierType == 8 || dataCfgInfoByCurLevelDiff.bSoldierType == 9 || dataCfgInfoByCurLevelDiff.bSoldierType == 7 || dataCfgInfoByCurLevelDiff.bSoldierType == 13;
							if (flag)
							{
								DragonIcon.DragonNode dragonNode = this.getDragonNode(dataCfgInfoByCurLevelDiff.bSoldierType);
								if (dragonNode != null)
								{
									dragonNode.SetData(spawnGroup.gameObject.transform.position, (int)dataCfgInfoByCurLevelDiff.bSoldierType, dragonNode.objid, this.m_b5v5, bUseCache, false, bRefreshMM);
									if (dragonNode.objid != 0u)
									{
										PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(dragonNode.objid);
										if (actor)
										{
											dragonNode.ShowDead(actor.handle.ActorControl.IsDeadState);
										}
										else
										{
											dragonNode.ShowDead(true);
										}
									}
									else
									{
										dragonNode.ShowDead(true);
									}
									MiniMapSysUT.RefreshMapPointerBig(dragonNode.bmDragonNode_3dui);
								}
							}
						}
					}
				}
			}
		}
	}
}
