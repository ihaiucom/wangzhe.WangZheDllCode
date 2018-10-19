using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	internal class TeleportTargetSelector : Singleton<TeleportTargetSelector>
	{
		public const string TeleportNotePrefabName = "Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_tishi";

		private Ray screenRay;

		private Plane curPlane;

		private List<PoolObjHandle<ActorRoot>> m_CanTeleportActorList;

		private Dictionary<PoolObjHandle<ActorRoot>, GameObject> m_CachedTeleportGameObjects;

		private bool m_TargetSkillEnable;

		public bool m_ClickDownStatus;

		private bool m_NeedConfirm;

		public bool m_bConfirmed = true;

		public override void Init()
		{
			this.curPlane = new Plane(new Vector3(0f, 1f, 0f), 0f);
			this.m_CanTeleportActorList = new List<PoolObjHandle<ActorRoot>>();
            this.m_CachedTeleportGameObjects = new Dictionary<PoolObjHandle<ActorRoot>, GameObject>();
            this.m_TargetSkillEnable = false;
			this.m_ClickDownStatus = false;
			this.m_NeedConfirm = false;
			this.m_bConfirmed = false;
			this.RegEvent();
		}

		public override void UnInit()
		{
			this.m_CanTeleportActorList.Clear();
			this.m_CachedTeleportGameObjects.Clear();
			this.UnregEvent();
		}

		private void RegEvent()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraClickDown, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraClickUp, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
		}

		private void UnregEvent()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraClickDown, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraClickUp, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
		}

		public void Update()
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			this.RefreshSkillEnableState();
			this.UpdateTargetAfterClickDown();
		}

		private void RefreshSkillEnableState()
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm == null)
			{
				return;
			}
			CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
			SkillSlotType skillSlotType;
			if (!skillButtonManager.HasMapSlectTargetSkill(out skillSlotType))
			{
				return;
			}
			SkillSlot skillSlot = null;
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			if (!hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot))
			{
				return;
			}
			this.m_NeedConfirm = (skillButtonManager.GetSkillJoystickMode(skillSlotType) == enSkillJoystickMode.MapSelectOther);
			if (this.m_TargetSkillEnable != skillSlot.IsEnableSkillSlot())
			{
				this.m_TargetSkillEnable = !this.m_TargetSkillEnable;
				if (this.m_TargetSkillEnable)
				{
					this.OnPlayerSkillEnable();
				}
				else
				{
					this.OnPlayerSkillDisable();
				}
			}
		}

		private void UpdateTargetAfterClickDown()
		{
			if (this.m_ClickDownStatus)
			{
				this.UpdateTeleportTargetList();
				this.RefreshTeleportTargetEffect();
			}
		}

		public void OnClickBattleScene(Vector2 _screenPos)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (this.m_NeedConfirm && !this.m_bConfirmed)
			{
				return;
			}
			if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera && !this.m_ClickDownStatus)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle.ActorControl == null || hostPlayer.Captain.handle.ActorControl.IsDeadState)
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(_screenPos);
			float distance;
			if (this.curPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
					SkillSlotType skillSlotType;
					if (skillButtonManager != null && skillButtonManager.HasMapSlectTargetSkill(out skillSlotType))
					{
						uint num = Singleton<TeleportTargetSearcher>.GetInstance().SearchNearestCanTeleportTarget(ref hostPlayer.Captain, (VInt3)point, 3000);
						if (num != 0u)
						{
							skillButtonManager.SelectedMapTarget(num);
						}
					}
				}
			}
		}

		private void OnMiniMap_Click_Down(CUIEvent uievent)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			this.m_ClickDownStatus = true;
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
				SkillSlotType skillSlotType;
				if (skillButtonManager != null && skillButtonManager.HasMapSlectTargetSkill(out skillSlotType))
				{
					CUIJoystickScript joystick = Singleton<CBattleSystem>.GetInstance().FightForm.GetJoystick();
					if (joystick != null)
					{
						joystick.ChangeJoystickResponseArea(true);
					}
				}
			}
		}

		private void OnMiniMap_Click_Up(CUIEvent uievent)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			this.m_ClickDownStatus = false;
			this.ClearTeleportTarget();
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
				SkillSlotType skillSlotType;
				if (skillButtonManager != null && skillButtonManager.HasMapSlectTargetSkill(out skillSlotType))
				{
					CUIJoystickScript joystick = Singleton<CBattleSystem>.GetInstance().FightForm.GetJoystick();
					if (joystick != null)
					{
						joystick.ChangeJoystickResponseArea(false);
					}
				}
			}
		}

		private void OnDragMiniMap(CUIEvent uievent)
		{
			this.m_ClickDownStatus = true;
		}

		private void OnDragMiniMapEnd(CUIEvent uievent)
		{
			this.ClearTeleportTarget();
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (!ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				return;
			}
			this.ClearTeleportTarget();
		}

		private void OnPlayerSkillEnable()
		{
			if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
			{
				return;
			}
			this.UpdateTeleportTargetList();
			this.RefreshTeleportTargetEffect();
		}

		private void OnPlayerSkillDisable()
		{
			this.ClearTeleportTarget();
		}

		private void UpdateTeleportTargetList()
		{
			this.m_CanTeleportActorList.Clear();
			CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
			SkillSlotType type;
			if (!skillButtonManager.HasMapSlectTargetSkill(out type))
			{
				return;
			}
			SkillSlot skillSlot = null;
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			if (!hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(type, out skillSlot))
			{
				return;
			}
			if (!skillSlot.IsEnableSkillSlot())
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.instance.GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = gameActors[i];
				if (poolObjHandle && !(poolObjHandle == hostPlayer.Captain))
				{
					ActorRoot handle = poolObjHandle.handle;
					if (!handle.ActorControl.IsDeadState && handle.IsHostCamp() && handle.InCamera)
					{
						uint dwSkillTargetFilter = skillSlot.SkillObj.cfgData.dwSkillTargetFilter;
						if (((ulong)dwSkillTargetFilter & (ulong)(1L << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31))) <= 0uL)
						{
							this.m_CanTeleportActorList.Add(poolObjHandle);
						}
					}
				}
			}
		}

		private void RefreshTeleportTargetEffect()
		{
			List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
			Dictionary<PoolObjHandle<ActorRoot>, GameObject>.Enumerator enumerator = this.m_CachedTeleportGameObjects.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<PoolObjHandle<ActorRoot>, GameObject> current = enumerator.Current;
				if (!this.m_CanTeleportActorList.Contains(current.Key))
				{
					if (current.Value != null)
					{
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(current.Value);
					}
					list.Add(current.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				PoolObjHandle<ActorRoot> key = list[i];
				if (this.m_CachedTeleportGameObjects.ContainsKey(key))
				{
					this.m_CachedTeleportGameObjects.Remove(key);
				}
			}
			if (this.m_NeedConfirm)
			{
				if (this.m_bConfirmed)
				{
					for (int j = 0; j < this.m_CanTeleportActorList.Count; j++)
					{
						PoolObjHandle<ActorRoot> poolObjHandle = this.m_CanTeleportActorList[j];
						if (!this.m_CachedTeleportGameObjects.ContainsKey(poolObjHandle) && this.IsShowTeleportNoteEffect(poolObjHandle))
						{
							this.ShowTeleportNoteEffect(poolObjHandle);
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < this.m_CanTeleportActorList.Count; k++)
				{
					PoolObjHandle<ActorRoot> poolObjHandle2 = this.m_CanTeleportActorList[k];
					if (!this.m_CachedTeleportGameObjects.ContainsKey(poolObjHandle2) && this.IsShowTeleportNoteEffect(poolObjHandle2))
					{
						this.ShowTeleportNoteEffect(poolObjHandle2);
					}
				}
			}
			if (this.m_NeedConfirm)
			{
				this.ShowSkillButtonFlowEffect(this.m_CanTeleportActorList.Count > 0 && !this.m_bConfirmed && this.m_ClickDownStatus && this.m_TargetSkillEnable);
			}
		}

		private bool IsShowTeleportNoteEffect(PoolObjHandle<ActorRoot> actor)
		{
			return actor && (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ || actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE || (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && actor.handle.ActorControl.GetActorSubType() == 1));
		}

		private void ShowSkillButtonFlowEffect(bool show)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm == null || Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager == null)
			{
				return;
			}
			CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
			SkillSlotType skillSlotType;
			if (skillButtonManager.HasMapSlectTargetSkill(out skillSlotType))
			{
				SkillButton button = skillButtonManager.GetButton(skillSlotType);
				if (button != null && button.m_button != null)
				{
					skillButtonManager.SetButtonFlowLight(button.m_button, show);
				}
			}
		}

		private void ClearTeleportTarget()
		{
			this.m_CanTeleportActorList = new List<PoolObjHandle<ActorRoot>>();
			this.m_bConfirmed = false;
			this.RefreshTeleportTargetEffect();
		}

		public void ShowTeleportNoteEffect(PoolObjHandle<ActorRoot> actorRoot)
		{
			if (!this.m_CachedTeleportGameObjects.ContainsKey(actorRoot) && (actorRoot.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ || actorRoot.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE))
			{
				bool flag = false;
				GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_tishi", false, SceneObjType.ActionRes, Vector3.zero, Quaternion.identity, out flag);
				if (pooledGameObjLOD != null)
				{
					pooledGameObjLOD.transform.SetParent(actorRoot.handle.myTransform);
					pooledGameObjLOD.transform.localPosition = Vector3.zero;
					pooledGameObjLOD.transform.localRotation = Quaternion.identity;
				}
				this.m_CachedTeleportGameObjects[actorRoot] = pooledGameObjLOD;
			}
		}
	}
}
