using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class MiniMapSysUT
	{
		public const int ATK = 2;

		public const int DEF = 3;

		public const int PROTECT = 5;

		public static string Map_HeroSelf_prefab = "UI3D/Battle/MiniMap/Map_HeroSelf.prefab";

		public static string Map_HeroAlice_prefab = "UI3D/Battle/MiniMap/Map_HeroAlice.prefab";

		public static string Map_HeroEnemy_prefab = "UI3D/Battle/MiniMap/Map_HeroEnemy.prefab";

		public static string Map_EyeAlice_prefab = "UI3D/Battle/MiniMap/Map_EyeAlice.prefab";

		public static string Map_EyeEnemy_prefab = "UI3D/Battle/MiniMap/Map_EyeEnemy.prefab";

		public static string Map_RedBuff_prefab = "UI3D/Battle/MiniMap/Map_RedBuff.prefab";

		public static string Map_BlueBuff_prefab = "UI3D/Battle/MiniMap/Map_BlueBuff.prefab";

		public static string Map_Jungle_prefab = "UI3D/Battle/MiniMap/Map_Jungle.prefab";

		public static string Map_SoilderAlice_prefab = "UI3D/Battle/MiniMap/Map_SoilderAlice.prefab";

		public static string Map_SoilderEnemy_prefab = "UI3D/Battle/MiniMap/Map_SoilderEnemy.prefab";

		public static string Map_Vanguard_prefab = "UI3D/Battle/MiniMap/Map_Vanguard.prefab";

		public static string Map_OrganAlice_prefab = "UI3D/Battle/MiniMap/Map_OrganAlice.prefab";

		public static string Map_OrganEnemy_prefab = "UI3D/Battle/MiniMap/Map_OrganEnemy.prefab";

		public static string Map_BaseAlice_prefab = "UI3D/Battle/MiniMap/Map_BaseAlice.prefab";

		public static string Map_BaseEnemy_prefab = "UI3D/Battle/MiniMap/Map_BaseEnemy.prefab";

		public static string Map_Signal_prefab = "UI3D/Battle/MiniMap/Map_Signal.prefab";

		public static string Map_Track_prefab = "UI3D/Battle/MiniMap/Map_Track.prefab";

		public static int UI3D_Depth = 30;

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddMesh(MiniMapSysUT.Map_HeroSelf_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_HeroAlice_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_HeroEnemy_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_EyeAlice_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_EyeEnemy_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_RedBuff_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_BlueBuff_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_Jungle_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_SoilderAlice_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_SoilderEnemy_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_Vanguard_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_OrganAlice_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_OrganEnemy_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_BaseAlice_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_BaseEnemy_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_Signal_prefab);
			preloadTab.AddMesh(MiniMapSysUT.Map_Track_prefab);
		}

		public static void NativeSizeLize(GameObject minimap3DUI)
		{
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return;
			}
			Sprite3D[] componentsInChildren = minimap3DUI.GetComponentsInChildren<Sprite3D>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Sprite3D sprite3D = componentsInChildren[i];
				if (sprite3D != null)
				{
					sprite3D.SetNativeSize(currentCamera, (float)MiniMapSysUT.UI3D_Depth);
				}
			}
		}

		public static void NativeSizeLize(GameObject minimap3DUI, float screenWidth, float screenHeight)
		{
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return;
			}
			Sprite3D[] componentsInChildren = minimap3DUI.GetComponentsInChildren<Sprite3D>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Sprite3D sprite3D = componentsInChildren[i];
				if (sprite3D != null)
				{
					sprite3D.SetNativeSize(currentCamera, (float)MiniMapSysUT.UI3D_Depth, screenWidth, screenHeight);
				}
			}
		}

		public static GameObject GetSignalGameObject(bool bMiniMap)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return null;
			}
			GameObject gameObject = (!bMiniMap) ? theMinimapSys.bmpcSignal : theMinimapSys.mmpcSignal;
			GameObject gameObject2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(MiniMapSysUT.Map_Signal_prefab, enResourceType.BattleScene);
			if (gameObject2 != null)
			{
				if (gameObject != null)
				{
					gameObject2.transform.SetParent(gameObject.transform, true);
				}
				float num = (!bMiniMap) ? 1f : 0.5f;
				gameObject2.transform.localScale = new Vector3(num, num, 1f);
				gameObject2.transform.localRotation = Quaternion.identity;
				MiniMapSysUT.NativeSizeLize(gameObject2);
			}
			return gameObject2;
		}

		public static GameObject GetMapTrackObj(string prefabPath, bool bMiniMap)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return null;
			}
			GameObject gameObject = (!bMiniMap) ? theMinimapSys.bmpcTrack : theMinimapSys.mmpcTrack;
			GameObject gameObject2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabPath, enResourceType.BattleScene);
			if (gameObject2 != null)
			{
				if (gameObject != null)
				{
					gameObject2.transform.SetParent(gameObject.transform, true);
				}
				float num = (!bMiniMap) ? 1f : 0.7f;
				gameObject2.transform.localScale = new Vector3(num, num, 1f);
				gameObject2.transform.localRotation = Quaternion.identity;
				MiniMapSysUT.NativeSizeLize(gameObject2);
			}
			return gameObject2;
		}

		public static GameObject GetHeroIconObj(string path, bool bMiniMap, bool bSelf, bool bSameCamp)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return null;
			}
			GameObject gameObject;
			if (bMiniMap)
			{
				if (bSelf)
				{
					gameObject = theMinimapSys.mmHeroIconNode_Self;
				}
				else
				{
					gameObject = ((!bSameCamp) ? theMinimapSys.mmHeroIconNode_Enemy : theMinimapSys.mmHeroIconNode_Friend);
				}
			}
			else if (bSelf)
			{
				gameObject = theMinimapSys.bmHeroIconNode_Self;
			}
			else
			{
				gameObject = ((!bSameCamp) ? theMinimapSys.bmHeroIconNode_Enemy : theMinimapSys.bmHeroIconNode_Friend);
			}
			GameObject gameObject2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(path, enResourceType.UI3DImage);
			if (gameObject2 != null)
			{
				if (gameObject != null)
				{
					gameObject2.transform.SetParent(gameObject.transform, true);
				}
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localRotation = Quaternion.identity;
				float num = (!bMiniMap) ? (40f * Sprite3D.Ratio()) : (20f * Sprite3D.Ratio());
				MiniMapSysUT.NativeSizeLize(gameObject2, num, num);
				Singleton<Camera_UI3D>.instance.GetCurrentCanvas().RefreshLayout(null);
			}
			return gameObject2;
		}

		public static void RecycleMapGameObject(GameObject element)
		{
			Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(element);
		}

		public static void RecycleMapGameObject(Transform ts)
		{
			if (ts != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(ts.gameObject);
			}
		}

		public static void UnRegisterEventCom(UI3DEventCom evtCom)
		{
			if (evtCom == null)
			{
				return;
			}
			evtCom.isDead = true;
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys != null)
			{
				UI3DEventMgr uI3DEventMgr = theMinimapSys.UI3DEventMgr;
				if (uI3DEventMgr != null)
				{
					uI3DEventMgr.UnRegister(evtCom);
				}
			}
		}

		public static GameObject GetMapGameObject(ActorRoot actor, bool bMiniMap, out UI3DEventCom evtCom)
		{
			evtCom = null;
			float num = 1f;
			GameObject gameObject = null;
			string text = string.Empty;
			bool flag = (!Singleton<WatchController>.GetInstance().IsWatching) ? actor.IsHostCamp() : (actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return null;
			}
			if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
			{
				if (!flag && actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
				{
					CallActorWrapper callActorWrapper = actor.ActorControl as CallActorWrapper;
					if (callActorWrapper != null && !callActorWrapper.IsTrueType)
					{
						flag = true;
					}
				}
				text = ((!flag) ? MiniMapSysUT.Map_HeroEnemy_prefab : MiniMapSysUT.Map_HeroAlice_prefab);
				bool flag2 = actor.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId;
				if (flag2)
				{
					text = MiniMapSysUT.Map_HeroSelf_prefab;
				}
				if (bMiniMap)
				{
					gameObject = ((!flag) ? theMinimapSys.mmpcHeroBgFrame_Enemy : theMinimapSys.mmpcHeroBgFrame_Friend);
				}
				else
				{
					gameObject = ((!flag) ? theMinimapSys.bmpcHeroBgFrame_Enemy : theMinimapSys.bmpcHeroBgFrame_Friend);
				}
				if (!bMiniMap)
				{
					evtCom = new UI3DEventCom();
					MiniMapSysUT.SetEventComScreenSize(evtCom, 40f, 40f, 1f);
					MiniMapSysUT.SetMapElement_EventParam(evtCom, actor.IsHostCamp(), MinimapSys.ElementType.Hero, actor.ObjID, (uint)actor.TheActorMeta.ConfigId);
					theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Hero);
				}
			}
			else if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
			{
				text = ((!flag) ? MiniMapSysUT.Map_EyeEnemy_prefab : MiniMapSysUT.Map_EyeAlice_prefab);
				gameObject = ((!bMiniMap) ? theMinimapSys.bmpcEye : theMinimapSys.mmpcEye);
				num = ((!bMiniMap) ? 1f : 0.5f);
				if (!bMiniMap)
				{
					evtCom = new UI3DEventCom();
					MiniMapSysUT.SetEventComScreenSize(evtCom, 30f, 18f, 1f);
					MiniMapSysUT.SetMapElement_EventParam(evtCom, actor.IsHostCamp(), MinimapSys.ElementType.Eye, actor.ObjID, (uint)actor.TheActorMeta.ConfigId);
					theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Eye);
				}
			}
			else if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				byte actorSubType = actor.ActorControl.GetActorSubType();
				if (actorSubType == 2)
				{
					byte actorSubSoliderType = actor.ActorControl.GetActorSubSoliderType();
					if (actorSubSoliderType == 8 || actorSubSoliderType == 9 || actorSubSoliderType == 13 || actorSubSoliderType == 7)
					{
						return null;
					}
					if (actorSubSoliderType == 11)
					{
						text = MiniMapSysUT.Map_RedBuff_prefab;
						gameObject = ((!bMiniMap) ? theMinimapSys.bmpcRedBuff : theMinimapSys.mmpcRedBuff);
						num = ((!bMiniMap) ? 1f : 0.6f);
					}
					else if (actorSubSoliderType == 10)
					{
						text = MiniMapSysUT.Map_BlueBuff_prefab;
						gameObject = ((!bMiniMap) ? theMinimapSys.bmpcBlueBuff : theMinimapSys.mmpcBlueBuff);
						num = ((!bMiniMap) ? 1f : 0.6f);
					}
					else
					{
						text = MiniMapSysUT.Map_Jungle_prefab;
						gameObject = ((!bMiniMap) ? theMinimapSys.bmpcJungle : theMinimapSys.mmpcJungle);
						num = ((!bMiniMap) ? 1.3f : 1f);
					}
				}
				else
				{
					if (bMiniMap)
					{
						gameObject = ((!flag) ? theMinimapSys.mmpcEnemy : theMinimapSys.mmpcAlies);
					}
					else
					{
						gameObject = ((!flag) ? theMinimapSys.bmpcEnemy : theMinimapSys.bmpcAlies);
					}
					text = ((!flag) ? MiniMapSysUT.Map_SoilderEnemy_prefab : MiniMapSysUT.Map_SoilderAlice_prefab);
					num = ((!bMiniMap) ? 0.6f : 0.3f);
					byte actorSubSoliderType2 = actor.ActorControl.GetActorSubSoliderType();
					if (actorSubSoliderType2 == 16)
					{
						text = MiniMapSysUT.Map_Vanguard_prefab;
						num = ((!bMiniMap) ? 1.5f : 1f);
					}
					if (!bMiniMap)
					{
						evtCom = new UI3DEventCom();
						MiniMapSysUT.SetEventComScreenSize(evtCom, 30f, 18f, 1f);
						MiniMapSysUT.SetMapElement_EventParam(evtCom, actor.IsHostCamp(), MinimapSys.ElementType.Solider, actor.ObjID, (uint)actor.TheActorMeta.ConfigId);
						theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Solider);
					}
				}
			}
			else if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				if (actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4)
				{
					text = ((!flag) ? MiniMapSysUT.Map_OrganEnemy_prefab : MiniMapSysUT.Map_OrganAlice_prefab);
					gameObject = ((!bMiniMap) ? theMinimapSys.bmpcOrgan : theMinimapSys.mmpcOrgan);
					num = ((!bMiniMap) ? 1f : 0.5f);
					if (!bMiniMap)
					{
						evtCom = new UI3DEventCom();
						MiniMapSysUT.SetEventComScreenSize(evtCom, 30f, 32f, 1f);
						MiniMapSysUT.SetMapElement_EventParam(evtCom, flag, MinimapSys.ElementType.Tower, actor.ObjID, 0u);
						theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Tower);
						if (flag)
						{
							Singleton<CBattleSystem>.GetInstance().TowerHitMgr.Register(actor.ObjID, (RES_ORGAN_TYPE)actor.TheStaticData.TheOrganOnlyInfo.OrganType);
						}
					}
				}
				else
				{
					if (actor.TheStaticData.TheOrganOnlyInfo.OrganType != 2)
					{
						return null;
					}
					text = ((!flag) ? MiniMapSysUT.Map_BaseEnemy_prefab : MiniMapSysUT.Map_BaseAlice_prefab);
					gameObject = ((!bMiniMap) ? theMinimapSys.bmpcOrgan : theMinimapSys.mmpcOrgan);
					num = ((!bMiniMap) ? 1f : 0.5f);
					if (!bMiniMap)
					{
						evtCom = new UI3DEventCom();
						MiniMapSysUT.SetEventComScreenSize(evtCom, 30f, 32f, 1f);
						MiniMapSysUT.SetMapElement_EventParam(evtCom, flag, MinimapSys.ElementType.Base, actor.ObjID, 0u);
						theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Tower);
						if (flag)
						{
							Singleton<CBattleSystem>.GetInstance().TowerHitMgr.Register(actor.ObjID, (RES_ORGAN_TYPE)actor.TheStaticData.TheOrganOnlyInfo.OrganType);
						}
					}
				}
			}
			if (string.IsNullOrEmpty(text) || gameObject == null)
			{
				return null;
			}
			GameObject gameObject2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(text, enResourceType.BattleScene);
			if (gameObject2 != null)
			{
				gameObject2.transform.SetParent(gameObject.transform, true);
				gameObject2.transform.localScale = new Vector3(num, num, 1f);
				gameObject2.transform.localRotation = Quaternion.identity;
				MiniMapSysUT.NativeSizeLize(gameObject2);
			}
			if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
			{
				float num2 = (!bMiniMap) ? (65f * Sprite3D.Ratio()) : (34f * Sprite3D.Ratio());
				MiniMapSysUT.NativeSizeLize(gameObject2, num2, num2);
			}
			return gameObject2;
		}

		public static void Set3DUIWorldPos_ByScreenPosition(GameObject uiObj_3dui, float screenX, float screenY)
		{
			if (uiObj_3dui == null)
			{
				return;
			}
			if (Singleton<CBattleSystem>.GetInstance().TheMinimapSys == null)
			{
				return;
			}
			Vector3 position = new Vector3(screenX, screenY, (float)MiniMapSysUT.UI3D_Depth);
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera != null)
			{
				uiObj_3dui.transform.position = currentCamera.ScreenToWorldPoint(position);
			}
		}

		public static Vector3 Set3DUIWorldPos_ByScreenPoint(ref Vector3 worldPos, bool bMiniMap, out float finalScreenX, out float finalScreenY)
		{
			finalScreenX = (finalScreenY = 0f);
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return Vector3.zero;
			}
			Vector2 vector = (!bMiniMap) ? Singleton<CBattleSystem>.instance.world_UI_Factor_Big : Singleton<CBattleSystem>.instance.world_UI_Factor_Small;
			Vector2 vector2 = new Vector2(worldPos.x * vector.x, worldPos.z * vector.y);
			vector2.x *= Sprite3D.Ratio();
			vector2.y *= Sprite3D.Ratio();
			Vector2 vector3 = (!bMiniMap) ? theMinimapSys.GetBMFianlScreenPos() : theMinimapSys.GetMMFianlScreenPos();
			vector2.x += vector3.x;
			vector2.y += vector3.y;
			Vector3 position = new Vector3(vector2.x, vector2.y, (float)MiniMapSysUT.UI3D_Depth);
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return Vector3.zero;
			}
			finalScreenX = position.x;
			finalScreenY = position.y;
			return currentCamera.ScreenToWorldPoint(position);
		}

		public static Vector2 CalcScreenPosInMapByWorldPos(ref Vector3 worldPos, bool bMiniMap)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return Vector3.zero;
			}
			Vector2 vector = (!bMiniMap) ? Singleton<CBattleSystem>.instance.world_UI_Factor_Big : Singleton<CBattleSystem>.instance.world_UI_Factor_Small;
			Vector2 result = new Vector2(worldPos.x * vector.x, worldPos.z * vector.y);
			result.x *= Sprite3D.Ratio();
			result.y *= Sprite3D.Ratio();
			Vector2 vector2 = (!bMiniMap) ? theMinimapSys.GetBMFianlScreenPos() : theMinimapSys.GetMMFianlScreenPos();
			result.x += vector2.x;
			result.y += vector2.y;
			return result;
		}

		public static Vector3 Set3DUIWorldPos_ByScreenPoint(ref Vector3 worldPos, bool bMiniMap)
		{
			float num;
			float num2;
			return MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldPos, bMiniMap, out num, out num2);
		}

		public static Vector2 Get3DCameraGameObject_ScreenPos(GameObject obj)
		{
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null || obj == null)
			{
				return Vector2.zero;
			}
			return currentCamera.WorldToScreenPoint(obj.transform.position);
		}

		private static void SetEventComScreenSize(UI3DEventCom evtCom, float width, float height, float scale = 1f)
		{
			if (evtCom != null)
			{
				evtCom.m_screenSize.width = width * scale * Sprite3D.Ratio();
				evtCom.m_screenSize.height = height * scale * Sprite3D.Ratio();
			}
		}

		public static void SetMapElement_EventParam(GameObject obj, bool bAlien, MinimapSys.ElementType type, uint objID = 0u, uint targetHeroID = 0u)
		{
			if (obj == null || type == MinimapSys.ElementType.None)
			{
				return;
			}
			CUIEventScript component = obj.GetComponent<CUIEventScript>();
			if (component != null)
			{
				MiniMapSysUT.SetMapElement_EventParam(component, bAlien, type, objID, targetHeroID);
			}
		}

		public static void SetMapElement_EventParam(CUIEventScript evtCom, bool bAlien, MinimapSys.ElementType type, uint objID = 0u, uint targetHeroID = 0u)
		{
			if (evtCom == null || type == MinimapSys.ElementType.None)
			{
				return;
			}
			stUIEventParams onClickEventParams = default(stUIEventParams);
			onClickEventParams.tag = ((!bAlien) ? 0 : 1);
			onClickEventParams.tag2 = (int)type;
			onClickEventParams.tagUInt = objID;
			onClickEventParams.heroId = targetHeroID;
			if (type == MinimapSys.ElementType.Dragon_3 || type == MinimapSys.ElementType.Dragon_5_big || type == MinimapSys.ElementType.Dragon_5_small)
			{
				onClickEventParams.tag3 = 2;
			}
			else if (type == MinimapSys.ElementType.Tower || type == MinimapSys.ElementType.Base)
			{
				onClickEventParams.tag3 = ((!bAlien) ? 2 : 3);
			}
			else if (type == MinimapSys.ElementType.Hero)
			{
				onClickEventParams.tag3 = ((!bAlien) ? 2 : 5);
			}
			evtCom.m_onClickEventParams = onClickEventParams;
		}

		public static void SetMapElement_EventParam(UI3DEventCom evtCom, bool bAlien, MinimapSys.ElementType type, uint objID = 0u, uint targetHeroID = 0u)
		{
			if (evtCom == null || type == MinimapSys.ElementType.None)
			{
				return;
			}
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.tag = ((!bAlien) ? 0 : 1);
			eventParams.tag2 = (int)type;
			eventParams.tagUInt = objID;
			eventParams.heroId = targetHeroID;
			if (type == MinimapSys.ElementType.Dragon_3 || type == MinimapSys.ElementType.Dragon_5_big || type == MinimapSys.ElementType.Dragon_5_small)
			{
				eventParams.tag3 = 2;
			}
			else if (type == MinimapSys.ElementType.Tower || type == MinimapSys.ElementType.Base)
			{
				eventParams.tag3 = ((!bAlien) ? 2 : 3);
			}
			else if (type == MinimapSys.ElementType.Hero)
			{
				eventParams.tag3 = ((!bAlien) ? 2 : 5);
			}
			evtCom.bHostSameCamp = bAlien;
			evtCom.m_eventID = MiniMapSysUT.GetEventId(type);
			evtCom.m_eventParams = eventParams;
		}

		public static enUIEventID GetEventId(MinimapSys.ElementType type)
		{
			if (type == MinimapSys.ElementType.Dragon_3)
			{
				return enUIEventID.BigMap_Click_3_long;
			}
			if (type == MinimapSys.ElementType.Dragon_5_big)
			{
				return enUIEventID.BigMap_Click_5_Dalong;
			}
			if (type == MinimapSys.ElementType.Dragon_5_small)
			{
				return enUIEventID.BigMap_Click_5_Xiaolong;
			}
			if (type == MinimapSys.ElementType.Tower)
			{
				return enUIEventID.BigMap_Click_Organ;
			}
			if (type == MinimapSys.ElementType.Base)
			{
				return enUIEventID.BigMap_Click_Organ;
			}
			if (type == MinimapSys.ElementType.Hero)
			{
				return enUIEventID.BigMap_Click_Hero;
			}
			if (type == MinimapSys.ElementType.Eye)
			{
				return enUIEventID.BigMap_Click_Eye;
			}
			if (type == MinimapSys.ElementType.Solider)
			{
				return enUIEventID.BigMap_Click_Solider;
			}
			return enUIEventID.None;
		}

		public static void RefreshMapPointerBig(GameObject go)
		{
			CUIFormScript formScript = Singleton<CBattleSystem>.GetInstance().FormScript;
			if (formScript != null && formScript.m_sgameGraphicRaycaster != null && go != null)
			{
				formScript.m_sgameGraphicRaycaster.RefreshGameObject(go);
			}
		}
	}
}
