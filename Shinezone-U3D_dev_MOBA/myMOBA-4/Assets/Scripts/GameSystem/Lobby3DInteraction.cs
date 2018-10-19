using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	internal class Lobby3DInteraction
	{
		public const float CAMERA_MIN = 0.8f;

		public const float CAMERA_MAX = 7.5f;

		private const string NAME_PVP = "PVP";

		private const string NAME_PVE = "PVE";

		private const string NAME_PROBE = "Probe";

		private const string NAME_LOTTERY = "Lottery";

		private const string NAME_ARENA = "Arena";

		private const string NAME_SOCIAL = "Social";

		private const string NAME_SHOP = "Shop";

		public static string PATH_3DINTERACTION_FORM = "UGUI/Form/System/Lobby/Form_LobbyInteractable.prefab";

		private int InteractLayerMask = 1 << LayerMask.NameToLayer("Lobby_Interactable");

		private Transform mouseDownObj;

		private CUIEvent uiEvt;

		public void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseDown, new CUIEventManager.OnUIEventHandler(this.onMouseDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseUp, new CUIEventManager.OnUIEventHandler(this.onMouseUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseClick, new CUIEventManager.OnUIEventHandler(this.onMouseClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragStart, new CUIEventManager.OnUIEventHandler(this.onDragStart));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragging, new CUIEventManager.OnUIEventHandler(this.onDragging));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragEnd, new CUIEventManager.OnUIEventHandler(this.onDragEnd));
			this.uiEvt = new CUIEvent();
		}

		public void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseDown, new CUIEventManager.OnUIEventHandler(this.onMouseDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseUp, new CUIEventManager.OnUIEventHandler(this.onMouseUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseClick, new CUIEventManager.OnUIEventHandler(this.onMouseClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragStart, new CUIEventManager.OnUIEventHandler(this.onDragStart));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragging, new CUIEventManager.OnUIEventHandler(this.onDragging));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragEnd, new CUIEventManager.OnUIEventHandler(this.onDragEnd));
			this.uiEvt = null;
		}

		public void OpenForm()
		{
			Singleton<CUIManager>.GetInstance().OpenForm(Lobby3DInteraction.PATH_3DINTERACTION_FORM, false, true);
		}

		private void onMouseDown(CUIEvent uiEvent)
		{
			Ray ray = Camera.main.ScreenPointToRay(uiEvent.m_pointerEventData.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, this.InteractLayerMask))
			{
				this.mouseDownObj = raycastHit.collider.gameObject.transform;
				this.mouseDownObj.position = new Vector3(this.mouseDownObj.position.x, this.mouseDownObj.position.y + 0.2f, this.mouseDownObj.position.z);
			}
		}

		private void onMouseUp(CUIEvent uiEvent)
		{
			if (this.mouseDownObj != null)
			{
				this.mouseDownObj.position = new Vector3(this.mouseDownObj.position.x, this.mouseDownObj.position.y - 0.2f, this.mouseDownObj.position.z);
				this.mouseDownObj = null;
			}
		}

		private void onMouseClick(CUIEvent uiEvent)
		{
			Ray ray = Camera.main.ScreenPointToRay(uiEvent.m_pointerEventData.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, this.InteractLayerMask))
			{
				Transform transform = raycastHit.collider.gameObject.transform;
				string name = transform.gameObject.name;
				string text = name;
				switch (text)
				{
				case "PVP":
					this.uiEvt.m_eventID = enUIEventID.Matching_OpenEntry;
					goto IL_1AE;
				case "PVE":
					this.uiEvt.m_eventID = enUIEventID.Adv_OpenChapterForm;
					goto IL_1AE;
				case "Probe":
					this.uiEvt.m_eventID = enUIEventID.Explore_OpenForm;
					goto IL_1AE;
				case "Lottery":
					this.uiEvt.m_eventID = enUIEventID.Lottery_OpenForm;
					goto IL_1AE;
				case "Arena":
					this.uiEvt.m_eventID = enUIEventID.Arena_OpenForm;
					goto IL_1AE;
				case "Social":
					this.uiEvt.m_eventID = enUIEventID.Guild_OpenForm;
					goto IL_1AE;
				case "Shop":
					this.uiEvt.m_eventID = enUIEventID.Shop_OpenForm;
					goto IL_1AE;
				}
				this.uiEvt.m_eventID = enUIEventID.None;
				IL_1AE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.uiEvt);
			}
		}

		private void onDragStart(CUIEvent uiEvent)
		{
		}

		private void onDragging(CUIEvent uiEvent)
		{
			Transform transform = Camera.main.transform;
			transform.position = new Vector3(transform.position.x - uiEvent.m_pointerEventData.delta.x / 100f, transform.position.y, transform.position.z);
			if (transform.position.x < 0.8f)
			{
				transform.position = new Vector3(0.8f, transform.position.y, transform.position.z);
			}
			if (transform.position.x > 7.5f)
			{
				transform.position = new Vector3(7.5f, transform.position.y, transform.position.z);
			}
		}

		private void onDragEnd(CUIEvent uiEvent)
		{
		}
	}
}
