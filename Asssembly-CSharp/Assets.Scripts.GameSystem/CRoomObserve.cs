using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public static class CRoomObserve
	{
		public static void RegisterEvents()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Fold, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickFold));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Seat, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSeat));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Kick, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickKick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Swap, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSwap));
		}

		public static void UnRegisterEvents()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Fold, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickFold));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Seat, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSeat));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Kick, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickKick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Swap, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSwap));
		}

		public static void SetObservers(GameObject root, int maxNum, ListView<MemberInfo> memberList, MemberInfo masterMember)
		{
			if (maxNum > 0)
			{
				root.CustomSetActive(true);
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(root, "SlotList");
				componetInChild.SetElementAmount(maxNum);
				MemberInfo[] array = new MemberInfo[maxNum];
				for (int i = 0; i < memberList.Count; i++)
				{
					MemberInfo memberInfo = memberList[i];
					if (memberInfo != null && memberInfo.dwPosOfCamp < (uint)maxNum)
					{
						array[(int)((uint)((UIntPtr)memberInfo.dwPosOfCamp))] = memberInfo;
					}
				}
				int num = 0;
				for (int j = 0; j < maxNum; j++)
				{
					MemberInfo memberInfo2 = array[j];
					GameObject gameObject = componetInChild.GetElemenet(j).gameObject;
					bool flag = null != memberInfo2;
					CUIHttpImageScript componetInChild2 = Utility.GetComponetInChild<CUIHttpImageScript>(gameObject, "Icon");
					Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject, "Name");
					CUIEventScript componetInChild4 = Utility.GetComponetInChild<CUIEventScript>(gameObject, "SitDown");
					CUIEventScript componetInChild5 = Utility.GetComponetInChild<CUIEventScript>(gameObject, "KickOut");
					componetInChild2.gameObject.CustomSetActive(flag);
					componetInChild3.gameObject.CustomSetActive(flag);
					componetInChild4.gameObject.CustomSetActive(!flag);
					componetInChild5.gameObject.CustomSetActive(flag && memberInfo2 != masterMember && Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner);
					if (flag)
					{
						componetInChild3.set_text(memberInfo2.MemberName);
						componetInChild2.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(memberInfo2.MemberHeadUrl));
						num++;
					}
					if (componetInChild4.gameObject.activeSelf)
					{
						componetInChild4.m_onClickEventID = enUIEventID.Room_Observe_Seat;
						componetInChild4.m_onClickEventParams.tag = j;
					}
					if (componetInChild5.gameObject.activeSelf)
					{
						componetInChild5.m_onClickEventID = enUIEventID.Room_Observe_Kick;
						componetInChild5.m_onClickEventParams.tag = j;
					}
				}
				Utility.GetComponetInChild<Text>(root, "PersonCount").set_text(num + "/" + maxNum);
			}
			else
			{
				root.CustomSetActive(false);
			}
		}

		private static void OnClickSeat(CUIEvent evt)
		{
			CRoomSystem.ReqChangeCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, evt.m_eventParams.tag, COM_CHGROOMPOS_TYPE.COM_CHGROOMPOS_EMPTY);
		}

		private static void OnClickSwap(CUIEvent evt)
		{
			CRoomSystem.ReqChangeCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, evt.m_eventParams.tag, COM_CHGROOMPOS_TYPE.COM_CHGROOMPOS_PLAYER);
		}

		private static void OnClickKick(CUIEvent evt)
		{
			CRoomSystem.ReqKickPlayer(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, evt.m_eventParams.tag);
		}

		private static void OnClickFold(CUIEvent evt)
		{
			Animator componentInParent = evt.m_srcWidget.GetComponentInParent<Animator>();
			if (componentInParent)
			{
				CUIEventScript component = evt.m_srcWidget.GetComponent<CUIEventScript>();
				if (component.m_onClickEventParams.tag == 0)
				{
					componentInParent.Play("Open");
					component.m_onClickEventParams.tag = 1;
				}
				else
				{
					componentInParent.Play("Close");
					component.m_onClickEventParams.tag = 0;
				}
			}
		}
	}
}
