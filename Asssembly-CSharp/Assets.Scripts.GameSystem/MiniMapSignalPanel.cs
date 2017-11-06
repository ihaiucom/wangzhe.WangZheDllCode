using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class MiniMapSignalPanel
	{
		public class SignalEntity
		{
			public int configID;

			public GameObject area;

			public GameObject normalObj;

			public GameObject selectedObj;

			public bool bSelected;

			public SignalEntity(int configId, GameObject area)
			{
				this.configID = configId;
				this.area = area;
				this.normalObj = area.transform.parent.FindChild("bg2").gameObject;
				this.selectedObj = area.transform.parent.FindChild("bg_Select").gameObject;
			}

			public void Clear()
			{
				this.area = null;
				this.normalObj = (this.selectedObj = null);
			}

			public void ShowSelected(bool bSelected)
			{
				this.bSelected = bSelected;
				this.selectedObj.CustomSetActive(bSelected);
			}

			public void ProcessUp(float x, float y, CUIFormScript form, Vector2 screenPos)
			{
				if (this.IsInsideRect(form, this.area, ref screenPos))
				{
					MinimapSys.Send_Position_Signal(x, y, form, (byte)this.configID, MinimapSys.ElementType.None, true);
				}
			}

			public bool IsInsideRect(CUIFormScript form, GameObject obj, ref Vector2 screenPosition)
			{
				if (form == null || obj == null)
				{
					return false;
				}
				Vector2 vector = CUIUtility.WorldToScreenPoint(form.GetCamera(), obj.transform.position);
				float num = form.ChangeFormValueToScreen((obj.transform as RectTransform).sizeDelta.x);
				float num2 = form.ChangeFormValueToScreen((obj.transform as RectTransform).sizeDelta.y);
				Rect rect = new Rect(vector.x - num / 2f, vector.y - num2 / 2f, num, num2);
				return rect.Contains(screenPosition);
			}
		}

		private CUIFormScript form;

		private GameObject node;

		private ListView<MiniMapSignalPanel.SignalEntity> signalEntities = new ListView<MiniMapSignalPanel.SignalEntity>();

		private bool bIsSPModel;

		private float cacheClickX;

		private float cacheClickY;

		public void Init(CUIFormScript form)
		{
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			this.form = form;
			Transform x = form.transform.FindChild("SignalPanel");
			if (x == null)
			{
				return;
			}
			this.node = form.transform.FindChild("SignalPanel").gameObject;
			this.node.CustomSetActive(false);
			GameObject gameObject = this.node.transform.FindChild("area_0/callback").gameObject;
			GameObject gameObject2 = this.node.transform.FindChild("area_1/callback").gameObject;
			GameObject gameObject3 = this.node.transform.FindChild("area_2/callback").gameObject;
			this.signalEntities.Add(new MiniMapSignalPanel.SignalEntity(202, gameObject));
			this.signalEntities.Add(new MiniMapSignalPanel.SignalEntity(201, gameObject2));
			this.signalEntities.Add(new MiniMapSignalPanel.SignalEntity(203, gameObject3));
		}

		public void Clear()
		{
			for (int i = 0; i < this.signalEntities.Count; i++)
			{
				MiniMapSignalPanel.SignalEntity signalEntity = this.signalEntities[i];
				if (signalEntity != null)
				{
					signalEntity.Clear();
				}
			}
			this.signalEntities.Clear();
			this.node = null;
			this.form = null;
			this.bIsSPModel = false;
		}

		public void Show(CUIEvent uievent)
		{
			if (this.form == null)
			{
				return;
			}
			if (!this.bIsSPModel)
			{
				for (int i = 0; i < this.signalEntities.Count; i++)
				{
					MiniMapSignalPanel.SignalEntity signalEntity = this.signalEntities[i];
					if (signalEntity != null)
					{
						signalEntity.ShowSelected(false);
					}
				}
			}
			Vector2 pressPosition = uievent.m_pointerEventData.get_pressPosition();
			this.cacheClickX = pressPosition.x;
			this.cacheClickY = pressPosition.y;
			Vector3 position = CUIUtility.ScreenToWorldPoint(this.form.GetCamera(), pressPosition, this.form.transform.position.z);
			this.node.transform.position = position;
			this.node.CustomSetActive(true);
			this.bIsSPModel = true;
		}

		public void Hide()
		{
			if (this.form == null)
			{
				return;
			}
			this.node.CustomSetActive(false);
			this.bIsSPModel = false;
		}

		public bool IsSignalPanelModel()
		{
			return this.bIsSPModel;
		}

		public void OnHoldUp(CUIEvent uievent)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys != null)
			{
				theMinimapSys.Switch(MinimapSys.EMapType.Mini);
			}
			if (this.form == null)
			{
				return;
			}
			for (int i = 0; i < this.signalEntities.Count; i++)
			{
				MiniMapSignalPanel.SignalEntity signalEntity = this.signalEntities[i];
				if (signalEntity != null)
				{
					signalEntity.ProcessUp(this.cacheClickX, this.cacheClickY, this.form, uievent.m_pointerEventData.get_position());
				}
			}
		}

		public void OnDrag(CUIEvent uievent)
		{
			if (this.form == null)
			{
				return;
			}
			Vector2 position = uievent.m_pointerEventData.get_position();
			for (int i = 0; i < this.signalEntities.Count; i++)
			{
				MiniMapSignalPanel.SignalEntity signalEntity = this.signalEntities[i];
				if (signalEntity != null)
				{
					signalEntity.ShowSelected(signalEntity.IsInsideRect(this.form, signalEntity.area, ref position));
				}
			}
		}
	}
}
