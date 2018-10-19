using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	internal class CReplayKitSys : Singleton<CReplayKitSys>
	{
		public enum Status
		{
			Recording,
			Paused,
			Transition
		}

		public enum StorageStatus
		{
			Warning = -2,
			Disable,
			Ok
		}

		private bool m_enable;

		private bool m_capable;

		private bool m_needDiscard;

		private uint MIN_SPACE_LIMIT = 200u;

		private uint WARNING_SPACE_LIMIT = 500u;

		public bool Cap
		{
			get
			{
				return this.m_capable;
			}
		}

		public bool Enable
		{
			get
			{
				return this.m_enable;
			}
		}

		public override void Init()
		{
			base.Init();
			this.m_capable = false;
			this.m_needDiscard = false;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Start_Recording, new CUIEventManager.OnUIEventHandler(this.OnRecord));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Pause_Recording, new CUIEventManager.OnUIEventHandler(this.OnPause));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Preview_Record, new CUIEventManager.OnUIEventHandler(this.OnPreview));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Discard_Record, new CUIEventManager.OnUIEventHandler(this.OnDiscard));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY, new Action(this.SetReplayKitGlobalCfg));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Start_Recording, new CUIEventManager.OnUIEventHandler(this.OnRecord));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Pause_Recording, new CUIEventManager.OnUIEventHandler(this.OnPause));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Preview_Record, new CUIEventManager.OnUIEventHandler(this.OnPreview));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Discard_Record, new CUIEventManager.OnUIEventHandler(this.OnDiscard));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY, new Action(this.SetReplayKitGlobalCfg));
		}

		private void SetReplayKitGlobalCfg()
		{
			if (GameDataMgr.svr2CltCfgDict != null)
			{
				if (GameDataMgr.svr2CltCfgDict.ContainsKey(6u))
				{
					ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
					if (GameDataMgr.svr2CltCfgDict.TryGetValue(6u, out resGlobalInfo))
					{
						this.m_enable = (resGlobalInfo.dwConfValue > 0u);
					}
				}
				if (GameDataMgr.svr2CltCfgDict.ContainsKey(7u))
				{
					ResGlobalInfo resGlobalInfo2 = new ResGlobalInfo();
					if (GameDataMgr.svr2CltCfgDict.TryGetValue(7u, out resGlobalInfo2))
					{
						this.MIN_SPACE_LIMIT = resGlobalInfo2.dwConfValue;
					}
				}
				if (GameDataMgr.svr2CltCfgDict.ContainsKey(8u))
				{
					ResGlobalInfo resGlobalInfo3 = new ResGlobalInfo();
					if (GameDataMgr.svr2CltCfgDict.TryGetValue(8u, out resGlobalInfo3))
					{
						this.WARNING_SPACE_LIMIT = resGlobalInfo3.dwConfValue;
					}
				}
			}
		}

		public CReplayKitSys.StorageStatus CheckStorage(bool showTips = true)
		{
			return CReplayKitSys.StorageStatus.Ok;
		}

		public void InitReplayKit(Transform container, bool autoRecord = false, bool autoPreview = false)
		{
			if (container == null)
			{
				return;
			}
			if (!this.Cap)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			if (!GameSettings.EnableReplayKit)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			CReplayKitSys.StorageStatus storageStatus = this.CheckStorage(false);
			CReplayKitSys.StorageStatus storageStatus2 = storageStatus;
			if (storageStatus2 == CReplayKitSys.StorageStatus.Disable)
			{
				container.gameObject.CustomSetActive(false);
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.ReplayKit_Pause_Recording;
				cUIEvent.m_eventParams.tag2 = 1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				return;
			}
			if (Singleton<BattleLogic>.GetInstance().isRuning)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && (curLvelContext.IsGameTypeGuide() || !curLvelContext.IsMobaMode()))
				{
					container.gameObject.CustomSetActive(false);
					CUIEvent cUIEvent2 = new CUIEvent();
					cUIEvent2.m_eventID = enUIEventID.ReplayKit_Pause_Recording;
					cUIEvent2.m_eventParams.tag2 = 1;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
					return;
				}
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState && (Singleton<CHeroSelectBaseSystem>.GetInstance().IsSpecTraingMode() || !Singleton<CHeroSelectBaseSystem>.GetInstance().IsMobaMode()))
			{
				container.gameObject.CustomSetActive(false);
				CUIEvent cUIEvent3 = new CUIEvent();
				cUIEvent3.m_eventID = enUIEventID.ReplayKit_Pause_Recording;
				cUIEvent3.m_eventParams.tag2 = 1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent3);
				return;
			}
			Transform transform = container.transform.Find("Recording");
			Transform transform2 = container.transform.Find("Paused");
			Transform transform3 = container.transform.Find("Transition");
			if (transform == null || transform2 == null)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			CUIEventScript component = transform.GetComponent<CUIEventScript>();
			CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
			if (component != null)
			{
				component.m_onClickEventID = enUIEventID.ReplayKit_Pause_Recording;
				if (autoPreview)
				{
					component.m_onClickEventParams.tag = 1;
				}
			}
			if (component2 != null)
			{
				component2.m_onClickEventID = enUIEventID.ReplayKit_Start_Recording;
			}
			if (autoRecord)
			{
				if (GameSettings.EnableReplayKitAutoMode)
				{
					transform.gameObject.CustomSetActive(true);
					transform2.gameObject.CustomSetActive(false);
					transform3.gameObject.CustomSetActive(false);
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.ReplayKit_Start_Recording);
				}
				else if (this.IsRecording())
				{
					transform.gameObject.CustomSetActive(true);
					transform2.gameObject.CustomSetActive(false);
					transform3.gameObject.CustomSetActive(false);
				}
				else
				{
					transform.gameObject.CustomSetActive(false);
					transform2.gameObject.CustomSetActive(true);
					transform3.gameObject.CustomSetActive(false);
				}
			}
			else if (this.IsRecording())
			{
				transform.gameObject.CustomSetActive(true);
				transform2.gameObject.CustomSetActive(false);
				transform3.gameObject.CustomSetActive(false);
			}
			else
			{
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(true);
				transform3.gameObject.CustomSetActive(false);
			}
		}

		public bool IsRecording()
		{
			return (!this.Cap || !GameSettings.EnableReplayKit) && false;
		}

		public void InitReplayKitRecordBtn(Transform container)
		{
			if (container == null)
			{
				return;
			}
			if (!this.Cap || !GameSettings.EnableReplayKit)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(container.gameObject, "Record");
			if (componetInChild != null)
			{
				componetInChild.m_onClickEventID = enUIEventID.ReplayKit_Preview_Record;
			}
			Transform transform = container.transform.Find("Extra/BtnGroup/BtnYes");
			if (transform != null)
			{
				CUIEventScript component = transform.GetComponent<CUIEventScript>();
				if (component != null)
				{
					component.m_onClickEventID = enUIEventID.ReplayKit_Preview_Record;
				}
			}
			Transform transform2 = container.transform.Find("Extra/BtnGroup/BtnNo");
			if (transform2 != null)
			{
				CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
				if (component2 != null)
				{
					component2.m_onClickEventID = enUIEventID.ReplayKit_Discard_Record;
				}
			}
			container.gameObject.CustomSetActive(false);
			Singleton<CTimerManager>.GetInstance().AddTimer(1000, 1, delegate(int sequence)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.ReplayKit_Pause_Recording);
			});
		}

		public void ChangeReplayKitStatus(Transform container, CReplayKitSys.Status status)
		{
			if (container == null)
			{
				return;
			}
			if (!this.Cap)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			if (!GameSettings.EnableReplayKit)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			if (!container.gameObject.activeSelf)
			{
				return;
			}
			Transform transform = container.transform.Find("Recording");
			Transform transform2 = container.transform.Find("Paused");
			Transform transform3 = container.transform.Find("Transition");
			if (transform == null || transform2 == null)
			{
				container.gameObject.CustomSetActive(false);
				return;
			}
			switch (status)
			{
			case CReplayKitSys.Status.Recording:
				transform.gameObject.CustomSetActive(true);
				transform2.gameObject.CustomSetActive(false);
				transform3.gameObject.CustomSetActive(false);
				break;
			case CReplayKitSys.Status.Paused:
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(true);
				transform3.gameObject.CustomSetActive(false);
				break;
			case CReplayKitSys.Status.Transition:
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(false);
				transform3.gameObject.CustomSetActive(true);
				break;
			}
		}

		private void OnRecord(CUIEvent uiEvent)
		{
			if (!this.m_capable || !GameSettings.EnableReplayKit)
			{
				return;
			}
		}

		private void OnPause(CUIEvent uiEvent)
		{
			if (!this.m_capable || !GameSettings.EnableReplayKit)
			{
				return;
			}
			int arg_2D_0 = (uiEvent.m_eventParams.tag == 0) ? 0 : 1;
			this.m_needDiscard = (uiEvent.m_eventParams.tag2 != 0);
		}

		private void OnPreview(CUIEvent uiEvent)
		{
			if (!this.m_capable || !GameSettings.EnableReplayKit)
			{
				return;
			}
		}

		private void OnDiscard(CUIEvent uiEvent)
		{
			if (!this.m_capable || !GameSettings.EnableReplayKit)
			{
				return;
			}
		}
	}
}
