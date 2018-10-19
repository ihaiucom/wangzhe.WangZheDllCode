using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GameSystem
{
	public class UI3DEventMgr
	{
		public enum EventComType
		{
			Hero,
			Tower,
			Eye,
			Solider
		}

		private const float SkillEventSizeScale = 3f;

		private ListView<UI3DEventCom> m_evtComsHeros = new ListView<UI3DEventCom>();

		private ListView<UI3DEventCom> m_evtComsTowers = new ListView<UI3DEventCom>();

		private ListView<UI3DEventCom> m_evtComsEyes = new ListView<UI3DEventCom>();

		private ListView<UI3DEventCom> m_evtComsSoliders = new ListView<UI3DEventCom>();

		public ListView<UI3DEventCom> TowerList
		{
			get
			{
				return this.m_evtComsTowers;
			}
		}

		public ListView<UI3DEventCom> EyesList
		{
			get
			{
				return this.m_evtComsEyes;
			}
		}

		public int HeroComCount
		{
			get
			{
				return this.m_evtComsHeros.Count;
			}
		}

		public int TowerComCount
		{
			get
			{
				return this.m_evtComsTowers.Count;
			}
		}

		public int EyeComCount
		{
			get
			{
				return this.m_evtComsEyes.Count;
			}
		}

		public void Init()
		{
		}

		public void Clear()
		{
			this.clearList(this.m_evtComsHeros);
			this.clearList(this.m_evtComsTowers);
			this.clearList(this.m_evtComsEyes);
			this.clearList(this.m_evtComsSoliders);
		}

		private void clearList(ListView<UI3DEventCom> list)
		{
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				UI3DEventCom uI3DEventCom = list[i];
				if (uI3DEventCom != null)
				{
					uI3DEventCom.Clear();
				}
			}
			list.Clear();
		}

		public void Register(UI3DEventCom com, bool bHero)
		{
			if (com == null)
			{
				return;
			}
			com.isDead = false;
			if (bHero)
			{
				if (!this.m_evtComsHeros.Contains(com))
				{
					this.m_evtComsHeros.Add(com);
				}
			}
			else if (!this.m_evtComsTowers.Contains(com))
			{
				this.m_evtComsTowers.Add(com);
			}
		}

		public void Register(UI3DEventCom com, UI3DEventMgr.EventComType comType)
		{
			if (com == null)
			{
				return;
			}
			com.isDead = false;
			switch (comType)
			{
			case UI3DEventMgr.EventComType.Hero:
				if (!this.m_evtComsHeros.Contains(com))
				{
					this.m_evtComsHeros.Add(com);
				}
				break;
			case UI3DEventMgr.EventComType.Tower:
				if (!this.m_evtComsTowers.Contains(com))
				{
					this.m_evtComsTowers.Add(com);
				}
				break;
			case UI3DEventMgr.EventComType.Eye:
				if (!this.m_evtComsEyes.Contains(com))
				{
					this.m_evtComsEyes.Add(com);
				}
				break;
			case UI3DEventMgr.EventComType.Solider:
				if (!this.m_evtComsSoliders.Contains(com))
				{
					this.m_evtComsSoliders.Add(com);
				}
				break;
			}
		}

		public void UnRegister(UI3DEventCom com)
		{
			this.m_evtComsHeros.Remove(com);
			this.m_evtComsTowers.Remove(com);
			this.m_evtComsEyes.Remove(com);
			this.m_evtComsSoliders.Remove(com);
		}

		public bool HandleClickEvent(PointerEventData pointerEventData)
		{
			for (int i = 0; i < this.m_evtComsHeros.Count; i++)
			{
				UI3DEventCom uI3DEventCom = this.m_evtComsHeros[i];
				if (uI3DEventCom != null && !uI3DEventCom.isDead && uI3DEventCom.m_screenSize.Contains(pointerEventData.pressPosition))
				{
					this.DispatchEvent(uI3DEventCom, pointerEventData);
					return true;
				}
			}
			for (int j = 0; j < this.m_evtComsTowers.Count; j++)
			{
				UI3DEventCom uI3DEventCom2 = this.m_evtComsTowers[j];
				if (uI3DEventCom2 != null && !uI3DEventCom2.isDead && uI3DEventCom2.m_screenSize.Contains(pointerEventData.pressPosition))
				{
					this.DispatchEvent(uI3DEventCom2, pointerEventData);
					return true;
				}
			}
			return false;
		}

		private void GetNearestTarget(PointerEventData pointerEventData, ListView<UI3DEventCom> targetsComs, ref float minDistance, ref UI3DEventCom targetCom)
		{
			for (int i = 0; i < targetsComs.Count; i++)
			{
				UI3DEventCom uI3DEventCom = targetsComs[i];
				Rect screenSize = uI3DEventCom.m_screenSize;
				screenSize.size *= 3f;
				float num = (screenSize.size.x <= screenSize.size.y) ? screenSize.size.y : screenSize.size.x;
				num *= num;
				if (uI3DEventCom != null)
				{
					float sqrMagnitude = (uI3DEventCom.m_screenSize.center - pointerEventData.pressPosition).sqrMagnitude;
					if (num >= sqrMagnitude && sqrMagnitude < minDistance)
					{
						minDistance = sqrMagnitude;
						targetCom = uI3DEventCom;
					}
				}
			}
		}

		public bool HandleSkillClickEvent(PointerEventData pointerEventData)
		{
			float num = 3.40282347E+38f;
			UI3DEventCom uI3DEventCom = null;
			this.GetNearestTarget(pointerEventData, this.m_evtComsTowers, ref num, ref uI3DEventCom);
			this.GetNearestTarget(pointerEventData, this.m_evtComsEyes, ref num, ref uI3DEventCom);
			this.GetNearestTarget(pointerEventData, this.m_evtComsSoliders, ref num, ref uI3DEventCom);
			if (uI3DEventCom != null)
			{
				this.DispatchEvent(uI3DEventCom, pointerEventData);
				return true;
			}
			return false;
		}

		private void DispatchEvent(UI3DEventCom eventScript, PointerEventData pointerEventData)
		{
			if (eventScript == null || pointerEventData == null)
			{
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = eventScript.m_eventID;
			uIEvent.m_eventParams = eventScript.m_eventParams;
			uIEvent.m_pointerEventData = pointerEventData;
			if (Singleton<CUIEventManager>.GetInstance() != null)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
			}
		}
	}
}
