using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class SignalPanel
	{
		private class CPlayerSignalCooldown
		{
			public ulong m_lastSignalExecuteTimestamp;

			public uint m_cooldownTime;

			public CPlayerSignalCooldown(ulong lastSignalExecuteTimestamp, uint cooldownTime)
			{
				this.m_lastSignalExecuteTimestamp = lastSignalExecuteTimestamp;
				this.m_cooldownTime = cooldownTime;
			}
		}

		private const float c_latestSignalTipsMaxDuringTime = 5f;

		private const float speed = 0.15f;

		private static int[][] s_signalButtonInfos = new int[][]
		{
			new int[]
			{
				1,
				12
			},
			new int[]
			{
				2,
				13
			},
			new int[]
			{
				3,
				14
			},
			new int[]
			{
				4,
				15
			}
		};

		private CUIFormScript m_formScript;

		public static string s_signalTipsFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_SignalTips.prefab";

		private bool m_useSignalButton;

		private GameObject m_miniMap;

		private Vector2 m_miniMapScreenPosition;

		private CUIContainerScript m_signalSrcHeroNameContainer;

		private CUIListScript m_signalTipsList;

		private CanvasGroup m_signalTipsListCanvasGroup;

		private CSignalButton[] m_signalButtons;

		private int m_selectedSignalID = -1;

		private Plane m_battleSceneGroundPlane;

		private ListView<CSignal> m_signals;

		private ListView<CSignalTipsElement> m_signalTipses;

		private float m_latestSignalTipsDuringTime;

		private Dictionary<uint, SignalPanel.CPlayerSignalCooldown> m_playerSignalCooldowns;

		public void Init(CUIFormScript formScript, GameObject minimapGameObject, GameObject signalSrcHeroNameContainer, bool useSignalButton)
		{
			if (formScript == null)
			{
				return;
			}
			this.m_formScript = formScript;
			this.m_miniMap = minimapGameObject;
			if (this.m_miniMap != null)
			{
				this.m_miniMapScreenPosition = CUIUtility.WorldToScreenPoint(formScript.GetCamera(), this.m_miniMap.transform.position);
			}
			this.m_signalSrcHeroNameContainer = ((!(signalSrcHeroNameContainer == null)) ? signalSrcHeroNameContainer.GetComponent<CUIContainerScript>() : null);
			this.m_signalTipsList = SignalPanel.GetSignalTipsListScript();
			if (this.m_signalTipsList != null)
			{
				this.m_signalTipsListCanvasGroup = this.m_signalTipsList.gameObject.GetComponent<CanvasGroup>();
				if (this.m_signalTipsListCanvasGroup == null)
				{
					this.m_signalTipsListCanvasGroup = this.m_signalTipsList.gameObject.AddComponent<CanvasGroup>();
				}
				this.m_signalTipsListCanvasGroup.alpha = 0f;
				this.m_signalTipsListCanvasGroup.blocksRaycasts = false;
			}
			this.m_useSignalButton = useSignalButton;
			this.m_signalButtons = new CSignalButton[SignalPanel.s_signalButtonInfos.Length];
			for (int i = 0; i < this.m_signalButtons.Length; i++)
			{
				this.m_signalButtons[i] = new CSignalButton(this.m_formScript.GetWidget(SignalPanel.s_signalButtonInfos[i][1]), SignalPanel.s_signalButtonInfos[i][0]);
				this.m_signalButtons[i].Initialize(this.m_formScript);
				if (!useSignalButton)
				{
					this.m_signalButtons[i].Disable();
				}
			}
			if (this.m_useSignalButton)
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ClickMiniMap, new CUIEventManager.OnUIEventHandler(this.OnClickMiniMap));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSignalButtonClicked, new CUIEventManager.OnUIEventHandler(this.OnSignalButtonClicked));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSignalTipsListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnSignalListElementEnabled));
				this.m_battleSceneGroundPlane = new Plane(new Vector3(0f, 1f, 0f), 0.15f);
				this.m_signals = new ListView<CSignal>();
				this.m_playerSignalCooldowns = new Dictionary<uint, SignalPanel.CPlayerSignalCooldown>();
				this.m_signalTipses = new ListView<CSignalTipsElement>();
			}
			Singleton<GameEventSys>.instance.AddEventHandler<SpawnSoldierParam>(GameEventDef.Event_SpawnSoldier, new RefAction<SpawnSoldierParam>(this.OnSpawnSoldier));
		}

		public void Clear()
		{
			if (this.m_useSignalButton)
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ClickMiniMap, new CUIEventManager.OnUIEventHandler(this.OnClickMiniMap));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSignalButtonClicked, new CUIEventManager.OnUIEventHandler(this.OnSignalButtonClicked));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSignalTipsListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnSignalListElementEnabled));
				this.m_signals = null;
				this.m_playerSignalCooldowns = null;
				this.m_signalTipses = null;
			}
			Singleton<GameEventSys>.instance.RmvEventHandler<SpawnSoldierParam>(GameEventDef.Event_SpawnSoldier, new RefAction<SpawnSoldierParam>(this.OnSpawnSoldier));
			this.m_signalButtons = null;
			this.m_formScript = null;
			this.m_miniMap = null;
			this.m_signalSrcHeroNameContainer = null;
			this.m_signalTipsList = null;
			Singleton<CUIManager>.GetInstance().CloseForm(SignalPanel.s_signalTipsFormPath);
		}

		private void OnSpawnSoldier(ref SpawnSoldierParam inParam)
		{
			if (inParam.SourceRegion != null && inParam.SourceRegion.IsInEmergency && inParam.SourceRegion.RouteType == RES_SOLDIER_ROUTE_TYPE.RES_SOLDIER_ROUTE_MID)
			{
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
				if (theMinimapSys == null)
				{
					return;
				}
				Vector2 localPosition = new Vector2(0f, 0f);
				Quaternion localRotation = Quaternion.identity;
				float num = 13f;
				if (inParam.SourceRegion.IsMyCamp())
				{
					localPosition = new Vector2(theMinimapSys.GetMMFianlScreenPos().x - theMinimapSys.mmFinalScreenSize.x / 2f + num, theMinimapSys.GetMMFianlScreenPos().y - theMinimapSys.mmFinalScreenSize.y / 2f + num);
				}
				else
				{
					localPosition = new Vector2(theMinimapSys.GetMMFianlScreenPos().x + theMinimapSys.mmFinalScreenSize.x / 2f - num, theMinimapSys.GetMMFianlScreenPos().y + theMinimapSys.mmFinalScreenSize.y / 2f - num);
					localRotation = Quaternion.Euler(0f, 0f, 180f);
				}
				this.ExecCommand(6, localPosition, localRotation);
			}
		}

		public static CUIListScript GetSignalTipsListScript()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm(SignalPanel.s_signalTipsFormPath);
			if (cUIFormScript == null)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(SignalPanel.s_signalTipsFormPath, true, true);
			}
			return Utility.GetComponetInChild<CUIListScript>(cUIFormScript.gameObject, "List_SignalTips");
		}

		public void Update()
		{
			if (!this.m_useSignalButton)
			{
				return;
			}
			if (this.m_signalButtons != null)
			{
				for (int i = 0; i < this.m_signalButtons.Length; i++)
				{
					if (this.m_signalButtons[i] != null)
					{
						this.m_signalButtons[i].UpdateCooldown();
					}
				}
			}
			if (this.m_signals != null)
			{
				int j = 0;
				while (j < this.m_signals.Count)
				{
					if (this.m_signals[j].IsNeedDisposed())
					{
						this.m_signals[j].Dispose();
						this.m_signals.RemoveAt(j);
					}
					else
					{
						this.m_signals[j].Update(this.m_formScript, Time.deltaTime);
						j++;
					}
				}
			}
			this.UpdateSignalTipses();
		}

		private void OnSignalButtonClicked(CUIEvent uiEvent)
		{
			if (Singleton<CBattleGuideManager>.instance.bPauseGame)
			{
				return;
			}
			if (!this.m_useSignalButton)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag;
			if (tag == 2)
			{
				CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_2);
			}
			else if (tag == 3)
			{
				CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_3);
			}
			else if (tag == 4)
			{
				CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_4);
			}
			CSignalButton singleButton = this.GetSingleButton(tag);
			if (singleButton != null && !singleButton.IsInCooldown())
			{
				if (singleButton.m_signalInfo.bSignalType == 0)
				{
					if (this.m_selectedSignalID != tag)
					{
						if (this.m_selectedSignalID >= 0)
						{
							CSignalButton singleButton2 = this.GetSingleButton(this.m_selectedSignalID);
							if (singleButton2 != null)
							{
								singleButton2.SetHighLight(false);
							}
						}
						this.m_selectedSignalID = tag;
						singleButton.SetHighLight(true);
					}
				}
				else
				{
					this.SendCommand_SignalBtn_Position((byte)tag, VInt3.zero);
				}
			}
		}

		private void OnClickBattleScene(CUIEvent uievent)
		{
			if (!this.m_useSignalButton || this.m_selectedSignalID < 0)
			{
				Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Mini);
				Singleton<InBattleMsgMgr>.instance.HideView();
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(uievent.m_pointerEventData.position);
			float distance;
			if (this.m_battleSceneGroundPlane.Raycast(ray, out distance))
			{
				VInt3 worldPos = (VInt3)ray.GetPoint(distance);
				this.SendCommand_SignalBtn_Position((byte)this.m_selectedSignalID, worldPos);
			}
		}

		public bool IsUseSingalButton()
		{
			return !(this.m_miniMap == null) && this.m_useSignalButton && this.m_selectedSignalID >= 0;
		}

		public void OnClickMiniMap(CUIEvent uiEvent)
		{
			if (!this.m_useSignalButton || this.m_selectedSignalID < 0 || this.m_miniMap == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			ActorRoot actorRoot = (hostPlayer != null) ? hostPlayer.Captain.handle : null;
			this.m_miniMapScreenPosition = CUIUtility.WorldToScreenPoint(uiEvent.m_srcFormScript.GetCamera(), this.m_miniMap.transform.position);
			Vector3 zero = Vector3.zero;
			zero.x = (uiEvent.m_pointerEventData.position.x - this.m_miniMapScreenPosition.x) * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Small.x;
			zero.y = ((actorRoot == null) ? 0.15f : ((Vector3)actorRoot.location).y);
			zero.z = (uiEvent.m_pointerEventData.position.y - this.m_miniMapScreenPosition.y) * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Small.y;
			VInt vInt;
			PathfindingUtility.GetGroundY((VInt3)zero, out vInt);
			zero.y = vInt.scalar;
			this.SendCommand_SignalMiniMap_Position((byte)this.m_selectedSignalID, (VInt3)zero, MinimapSys.ElementType.None);
		}

		public CSignalButton GetSingleButton(int signalID)
		{
			if (this.m_signalButtons == null)
			{
				return null;
			}
			for (int i = 0; i < this.m_signalButtons.Length; i++)
			{
				if (this.m_signalButtons[i] != null && this.m_signalButtons[i].m_signalID == signalID)
				{
					return this.m_signalButtons[i];
				}
			}
			return null;
		}

		public void CancelSelectedSignalButton()
		{
			if (!this.m_useSignalButton || this.m_selectedSignalID < 0)
			{
				return;
			}
			CSignalButton singleButton = this.GetSingleButton(this.m_selectedSignalID);
			if (singleButton != null)
			{
				singleButton.SetHighLight(false);
			}
			this.m_selectedSignalID = -1;
		}

		public void ExecCommand_SignalBtn_Position(uint senderPlayerID, byte signalID, ref VInt3 worldPos)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle == null)
			{
				return;
			}
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(senderPlayerID);
			int configId = player.Captain.handle.TheActorMeta.ConfigId;
			this.ExecCommand(senderPlayerID, (uint)configId, (int)signalID, worldPos.x, worldPos.y, worldPos.z, 0, 0, 0u);
		}

		public void ExecCommand_SignalMiniMap_Position(uint senderPlayerID, byte signalID, ref VInt3 worldPos, byte elementType)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle == null)
			{
				return;
			}
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(senderPlayerID);
			bool flag = hostPlayer.PlayerCamp == player.PlayerCamp;
			int configId = player.Captain.handle.TheActorMeta.ConfigId;
			if (signalID < 200)
			{
                this.ExecCommand(senderPlayerID, (uint)configId, (int)signalID, worldPos.x, worldPos.y, worldPos.z, (!flag) ? (byte)0 : (byte)1, elementType, 0u);
			}
			else
			{
				this.ExecCommand_4_SignalPanel_Sign(player.Captain, senderPlayerID, (uint)configId, (int)signalID, worldPos.x, worldPos.y, worldPos.z);
			}
		}

		public void ExecCommand_SignalMiniMap_Target(uint senderPlayerID, byte signalID, byte type, uint targetObjID)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actor = this.GetActor(type, targetObjID);
			if (!actor || actor.handle.ObjID == 0u)
			{
				return;
			}
			if (!actor.handle.HorizonMarker.IsVisibleFor(hostPlayer.PlayerCamp))
			{
				return;
			}
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(senderPlayerID);
			bool flag = hostPlayer.PlayerCamp == actor.handle.TheActorMeta.ActorCamp;
			int configId = actor.handle.TheActorMeta.ConfigId;
			int configId2 = player.Captain.handle.TheActorMeta.ConfigId;
            this.ExecCommand(actor, senderPlayerID, (uint)configId2, (int)signalID, (!flag) ? (byte)0 : (byte)1, type, 0u, (uint)configId);
		}

		private PoolObjHandle<ActorRoot> GetActor(byte type, uint targetObjID)
		{
			List<PoolObjHandle<ActorRoot>> list = null;
			if (type == 6 || type == 4 || type == 5)
			{
				list = Singleton<GameObjMgr>.instance.GameActors;
			}
			else if (type == 3)
			{
				list = Singleton<GameObjMgr>.instance.HeroActors;
			}
			else if (type == 1)
			{
				list = Singleton<GameObjMgr>.instance.TowerActors;
			}
			else if (type == 2)
			{
				list = Singleton<GameObjMgr>.instance.OrganActors;
			}
			for (int i = 0; i < list.Count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = list[i];
				if (poolObjHandle && poolObjHandle.handle.ObjID == targetObjID)
				{
					return poolObjHandle;
				}
			}
			return default(PoolObjHandle<ActorRoot>);
		}

		private uint GetCDTime(ResSignalInfo signalInfo)
		{
			uint result = (uint)signalInfo.bCooldownTime * 1000u;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
			{
				result = 2000u;
			}
			return result;
		}

		public void ExecCommand(uint senderPlayerID, uint heroID, int signalID, int worldPositionX, int worldPositionY, int worldPositionZ, byte bAlice = 0, byte elementType = 0, uint targetHeroID = 0u)
		{
			if (!this.m_useSignalButton || this.m_formScript == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(senderPlayerID);
			if (hostPlayer == null || player == null)
			{
				return;
			}
			if (hostPlayer.PlayerCamp != player.PlayerCamp)
			{
				return;
			}
			bool flag = hostPlayer == player;
			ResSignalInfo dataByKey = GameDataMgr.signalDatabin.GetDataByKey((long)signalID);
			if (dataByKey == null)
			{
				DebugHelper.Assert(dataByKey != null, "ExecCommand signalInfo is null, check out...");
				return;
			}
			uint cDTime = this.GetCDTime(dataByKey);
			ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			SignalPanel.CPlayerSignalCooldown cPlayerSignalCooldown = null;
			this.m_playerSignalCooldowns.TryGetValue(senderPlayerID, out cPlayerSignalCooldown);
			if (cPlayerSignalCooldown != null)
			{
				if ((uint)(logicFrameTick - cPlayerSignalCooldown.m_lastSignalExecuteTimestamp) < cPlayerSignalCooldown.m_cooldownTime)
				{
					return;
				}
				cPlayerSignalCooldown.m_lastSignalExecuteTimestamp = logicFrameTick;
				cPlayerSignalCooldown.m_cooldownTime = cDTime;
			}
			else
			{
				cPlayerSignalCooldown = new SignalPanel.CPlayerSignalCooldown(logicFrameTick, cDTime);
				this.m_playerSignalCooldowns.Add(senderPlayerID, cPlayerSignalCooldown);
			}
			if (flag && this.m_signalButtons != null)
			{
				for (int i = 0; i < this.m_signalButtons.Length; i++)
				{
					if (this.m_signalButtons[i] != null)
					{
						this.m_signalButtons[i].StartCooldown(cDTime);
					}
				}
			}
			bool bSmall = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
			this.PlaySignalTipsSound(elementType, bAlice, targetHeroID);
			bool bUseCfgSound = elementType == 0;
			senderPlayerID = ((elementType == 0) ? senderPlayerID : 0u);
			CSignal cSignal = new CSignal(senderPlayerID, signalID, worldPositionX, worldPositionY, worldPositionZ, true, bSmall, bUseCfgSound);
			cSignal.Initialize(this.m_formScript, dataByKey);
			this.m_signals.Add(cSignal);
			CSignalTips obj = new CSignalTips(signalID, heroID, flag, bAlice, elementType, targetHeroID);
			this.Add_SignalTip(obj);
		}

		public void ExecCommand_4_SignalPanel_Sign(PoolObjHandle<ActorRoot> sender, uint senderPlayerID, uint heroID, int signalID, int worldPositionX, int worldPositionY, int worldPositionZ)
		{
			if (!this.m_useSignalButton || this.m_formScript == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(senderPlayerID);
			if (hostPlayer == null || player == null)
			{
				return;
			}
			if (hostPlayer.PlayerCamp != player.PlayerCamp)
			{
				return;
			}
			bool flag = hostPlayer == player;
			ResSignalInfo dataByKey = GameDataMgr.signalDatabin.GetDataByKey((long)signalID);
			if (dataByKey == null)
			{
				DebugHelper.Assert(dataByKey != null, "ExecCommand signalInfo is null, check out...");
				return;
			}
			uint cDTime = this.GetCDTime(dataByKey);
			ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			SignalPanel.CPlayerSignalCooldown cPlayerSignalCooldown = null;
			this.m_playerSignalCooldowns.TryGetValue(senderPlayerID, out cPlayerSignalCooldown);
			if (cPlayerSignalCooldown != null)
			{
				if ((uint)(logicFrameTick - cPlayerSignalCooldown.m_lastSignalExecuteTimestamp) < cPlayerSignalCooldown.m_cooldownTime)
				{
					return;
				}
				cPlayerSignalCooldown.m_lastSignalExecuteTimestamp = logicFrameTick;
				cPlayerSignalCooldown.m_cooldownTime = cDTime;
			}
			else
			{
				cPlayerSignalCooldown = new SignalPanel.CPlayerSignalCooldown(logicFrameTick, cDTime);
				this.m_playerSignalCooldowns.Add(senderPlayerID, cPlayerSignalCooldown);
			}
			if (flag && this.m_signalButtons != null)
			{
				for (int i = 0; i < this.m_signalButtons.Length; i++)
				{
					if (this.m_signalButtons[i] != null)
					{
						this.m_signalButtons[i].StartCooldown(cDTime);
					}
				}
			}
			Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(dataByKey.szSound);
			MiniMapEffectModule miniMapEffectModule = Singleton<CBattleSystem>.instance.TheMinimapSys.miniMapEffectModule;
			if (miniMapEffectModule != null)
			{
				Vector3 vector = new Vector3((float)worldPositionX, (float)worldPositionY, (float)worldPositionZ);
				GameObject gameObject = miniMapEffectModule.PlaySceneEffect(dataByKey.szSceneEffect, (int)dataByKey.bTime * 1000, vector);
				if (signalID == 202 && gameObject != null)
				{
					TextMesh component = gameObject.transform.FindChild("text").GetComponent<TextMesh>();
					if (component != null)
					{
						ResHeroCfgInfo dataByKey2 = GameDataMgr.heroDatabin.GetDataByKey(heroID);
						if (dataByKey2 == null)
						{
							return;
						}
						component.text = dataByKey2.szName;
						component.gameObject.CustomSetActive(true);
					}
					Transform transform = gameObject.transform.FindChild("JianTou");
					Vector3 forward = sender.handle.gameObject.transform.position - vector;
					forward.Normalize();
					transform.forward = forward;
					if (Singleton<BattleLogic>.instance.m_LevelContext.m_isCameraFlip)
					{
						component.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
					}
				}
				if (signalID == 203 && Singleton<BattleLogic>.instance.m_LevelContext.m_isCameraFlip)
				{
					gameObject.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
				}
				if (!string.IsNullOrEmpty(dataByKey.szRealEffect))
				{
					bool bMiniMap = false;
					MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
					if (theMinimapSys != null)
					{
						bMiniMap = (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini);
					}
					Vector2 screenPos = MiniMapSysUT.CalcScreenPosInMapByWorldPos(ref vector, bMiniMap);
					miniMapEffectModule.PlayScreenPosEffect(dataByKey.szRealEffect, (float)((int)dataByKey.bTime * 1000), screenPos);
				}
			}
		}

		public void ExecCommand(PoolObjHandle<ActorRoot> followActor, uint senderPlayerID, uint heroID, int signalID, byte bAlice = 0, byte elementType = 0, uint targetObjID = 0u, uint targetHeroID = 0u)
		{
			if (!this.m_useSignalButton || this.m_formScript == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(senderPlayerID);
			if (hostPlayer == null || player == null)
			{
				return;
			}
			if (hostPlayer.PlayerCamp != player.PlayerCamp)
			{
				return;
			}
			bool flag = hostPlayer == player;
			ResSignalInfo dataByKey = GameDataMgr.signalDatabin.GetDataByKey((long)signalID);
			if (dataByKey == null)
			{
				DebugHelper.Assert(dataByKey != null, "ExecCommand signalInfo is null, check out...");
				return;
			}
			uint cDTime = this.GetCDTime(dataByKey);
			ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			SignalPanel.CPlayerSignalCooldown cPlayerSignalCooldown = null;
			this.m_playerSignalCooldowns.TryGetValue(senderPlayerID, out cPlayerSignalCooldown);
			if (cPlayerSignalCooldown != null)
			{
				if ((uint)(logicFrameTick - cPlayerSignalCooldown.m_lastSignalExecuteTimestamp) < cPlayerSignalCooldown.m_cooldownTime)
				{
					return;
				}
				cPlayerSignalCooldown.m_lastSignalExecuteTimestamp = logicFrameTick;
				cPlayerSignalCooldown.m_cooldownTime = cDTime;
			}
			else
			{
				cPlayerSignalCooldown = new SignalPanel.CPlayerSignalCooldown(logicFrameTick, cDTime);
				this.m_playerSignalCooldowns.Add(senderPlayerID, cPlayerSignalCooldown);
			}
			if (flag && this.m_signalButtons != null)
			{
				for (int i = 0; i < this.m_signalButtons.Length; i++)
				{
					if (this.m_signalButtons[i] != null)
					{
						this.m_signalButtons[i].StartCooldown(cDTime);
					}
				}
			}
			bool bSmall = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
			this.PlaySignalTipsSound(elementType, bAlice, targetHeroID);
			bool bUseCfgSound = elementType == 0;
			if (followActor && followActor.handle.Visible)
			{
				CSignal cSignal = new CSignal(followActor, signalID, true, bSmall, bUseCfgSound);
				cSignal.Initialize(this.m_formScript, dataByKey);
				this.m_signals.Add(cSignal);
			}
			CSignalTips obj = new CSignalTips(signalID, heroID, flag, bAlice, elementType, targetHeroID);
			this.Add_SignalTip(obj);
		}

		public void ExecCommand(int signalID, Vector2 localPosition, Quaternion localRotation)
		{
			if (this.m_formScript == null)
			{
				return;
			}
			ResSignalInfo dataByKey = GameDataMgr.signalDatabin.GetDataByKey((long)signalID);
			bool bSmall = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
			CSignal cSignal = new CSignal(signalID, localPosition, true, bSmall, true, true, localRotation);
			cSignal.Initialize(this.m_formScript, dataByKey);
			this.m_signals.Add(cSignal);
		}

		private void PlaySignalTipsSound(byte elementType, byte bAlice, uint targetHeroID)
		{
			string text = string.Empty;
			switch (elementType)
			{
			case 1:
				text = ((bAlice != 1) ? "Play_notice_map_1" : "Play_notice_map_2");
				break;
			case 2:
				text = ((bAlice != 1) ? "Play_sys_bobao_jihe_6" : "Play_sys_bobao_jihe_7");
				break;
			case 3:
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(targetHeroID);
				if (dataByKey != null && !string.IsNullOrEmpty(dataByKey.szHeroSound))
				{
					Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(dataByKey.szHeroSound);
				}
				text = ((bAlice != 1) ? "Play_Call_Attack" : "Play_Call_Guard");
				break;
			}
			case 4:
				text = "Play_sys_bobao_jihe_3";
				break;
			case 5:
				text = "Play_sys_bobao_jihe_4";
				break;
			case 6:
				text = "Play_sys_bobao_jihe_4";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(text);
			}
		}

		public void Add_SignalTip(CSignalTipsElement obj)
		{
			if (obj != null)
			{
				this.m_signalTipses.Add(obj);
			}
			this.RefreshSignalTipsList();
		}

		public void SendCommand_SignalBtn_Position(byte signalID, VInt3 worldPos)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle == null)
			{
				return;
			}
			FrameCommand<SignalBtnPosition> frameCommand = FrameCommandFactory.CreateFrameCommand<SignalBtnPosition>();
			frameCommand.cmdData.m_signalID = signalID;
			frameCommand.cmdData.m_worldPos = worldPos;
			frameCommand.Send();
		}

		public void SendCommand_SignalMiniMap_Position(byte signalID, VInt3 worldPos, MinimapSys.ElementType elementType)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle == null)
			{
				return;
			}
			FrameCommand<SignalMiniMapPosition> frameCommand = FrameCommandFactory.CreateFrameCommand<SignalMiniMapPosition>();
			frameCommand.cmdData.m_signalID = signalID;
			frameCommand.cmdData.m_worldPos = worldPos;
			frameCommand.cmdData.m_elementType = (byte)elementType;
			frameCommand.Send();
		}

		public void SendCommand_SignalMiniMap_Target(byte signalID, byte type, uint targetObjID)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle == null)
			{
				return;
			}
			FrameCommand<SignalMiniMapTarget> frameCommand = FrameCommandFactory.CreateFrameCommand<SignalMiniMapTarget>();
			frameCommand.cmdData.m_signalID = signalID;
			frameCommand.cmdData.m_type = type;
			frameCommand.cmdData.m_targetObjID = targetObjID;
			frameCommand.Send();
		}

		private void RefreshSignalTipsList()
		{
			this.m_latestSignalTipsDuringTime = 5f;
			if (this.m_signalTipsList != null)
			{
				this.m_signalTipsList.SetElementAmount(this.m_signalTipses.Count);
				this.m_signalTipsList.MoveElementInScrollArea(this.m_signalTipses.Count - 1, false);
			}
		}

		private void UpdateSignalTipses()
		{
			if (this.m_signalTipsListCanvasGroup != null)
			{
				if (this.m_latestSignalTipsDuringTime > 0f)
				{
					if (this.m_signalTipsListCanvasGroup.alpha < 1f)
					{
						this.m_signalTipsListCanvasGroup.alpha += 0.15f;
						if (this.m_signalTipsListCanvasGroup.alpha > 1f)
						{
							this.m_signalTipsListCanvasGroup.alpha = 1f;
						}
					}
				}
				else if (this.m_signalTipsListCanvasGroup.alpha > 0f)
				{
					this.m_signalTipsListCanvasGroup.alpha -= 0.15f;
					if (this.m_signalTipsListCanvasGroup.alpha < 0f)
					{
						this.m_signalTipsListCanvasGroup.alpha = 0f;
					}
				}
				else
				{
					this.ClearSignalTipses();
				}
			}
			if (this.m_latestSignalTipsDuringTime > 0f && this.m_signalTipsList != null && this.m_signalTipsList.IsElementInScrollArea(this.m_signalTipsList.GetElementAmount() - 1))
			{
				this.m_latestSignalTipsDuringTime -= Time.deltaTime;
				if (this.m_latestSignalTipsDuringTime < 0f)
				{
					this.m_latestSignalTipsDuringTime = 0f;
				}
			}
		}

		private void ClearSignalTipses()
		{
			if (this.m_signalTipses != null && this.m_signalTipses.Count > 0)
			{
				this.m_signalTipses.Clear();
				if (this.m_signalTipsList != null)
				{
					this.m_signalTipsList.SetElementAmount(0);
					this.m_signalTipsList.ResetContentPosition();
				}
			}
		}

		private void OnSignalListElementEnabled(CUIEvent uiEvent)
		{
			CUIListElementScript cUIListElementScript = (CUIListElementScript)uiEvent.m_srcWidgetScript;
			int index = cUIListElementScript.m_index;
			if (index < 0 && index >= this.m_signalTipses.Count)
			{
				return;
			}
			CSignalTipShower component = cUIListElementScript.GetComponent<CSignalTipShower>();
			if (component != null)
			{
				component.Set(this.m_signalTipses[index], uiEvent.m_srcFormScript);
			}
		}

		public bool CheckSignalPositionValid(Vector2 sigScreenPos)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			return theMinimapSys != null && sigScreenPos.x >= theMinimapSys.GetMMFianlScreenPos().x - theMinimapSys.mmFinalScreenSize.x / 2f && sigScreenPos.x <= theMinimapSys.GetMMFianlScreenPos().x + theMinimapSys.mmFinalScreenSize.x / 2f && sigScreenPos.y >= theMinimapSys.GetMMFianlScreenPos().y - theMinimapSys.mmFinalScreenSize.y / 2f && sigScreenPos.y <= theMinimapSys.GetMMFianlScreenPos().y + theMinimapSys.mmFinalScreenSize.y / 2f;
		}
	}
}
