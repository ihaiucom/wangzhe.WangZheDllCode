using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class SevenDayCheckSystem : Singleton<SevenDayCheckSystem>
	{
		private CUIFormScript _form;

		public readonly string FormName = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/Form_SevenDayCheck.prefab");

		public bool IsShowingLoginOpen;

		private CheckInActivity _curActivity;

		private CheckInPhase _availablePhase;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSevenDayCheckForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_Request, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_LoginOpen, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSevenDayCheckForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_Request, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_LoginOpen, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
		}

		protected void OnLoginOpen(CUIEvent uiEvent)
		{
			if (this._form == null)
			{
				bool flag = false;
				ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList((Activity actv) => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_WEAL);
				if (activityList != null && activityList.Count > 0)
				{
					this._curActivity = (CheckInActivity)activityList[0];
					ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
					for (int i = 0; i < phaseList.Count; i++)
					{
						if (phaseList[i].ReadyForGet)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						this._curActivity.OnMaskStateChange += new Activity.ActivityEvent(this.ActivityEvent);
						this._curActivity.OnTimeStateChange += new Activity.ActivityEvent(this.ActivityEvent);
						this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
						this.UpdateCheckView();
					}
					else
					{
						this._curActivity = null;
					}
				}
				if (!flag)
				{
					Singleton<Day14CheckSystem>.GetInstance().OnLoginOpen(null);
				}
			}
		}

		protected void OnOpenSevenDayCheckForm(CUIEvent uiEvent)
		{
			if (this._form == null)
			{
				ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList((Activity actv) => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_WEAL);
				if (activityList != null && activityList.Count > 0)
				{
					this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
					this._curActivity = (CheckInActivity)activityList[0];
					this._curActivity.OnMaskStateChange += new Activity.ActivityEvent(this.ActivityEvent);
					this._curActivity.OnTimeStateChange += new Activity.ActivityEvent(this.ActivityEvent);
					this.UpdateCheckView();
				}
			}
		}

		protected void ActivityEvent(Activity acty)
		{
			this.UpdateCheckView();
		}

		private void UpdateCheckView()
		{
			if (this._curActivity != null)
			{
				ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
				Transform transform = this._form.gameObject.transform.FindChild("Panel/ItemContainer");
				bool flag = false;
				for (int i = 0; i < phaseList.Count; i++)
				{
					CheckInPhase checkInPhase = phaseList[i] as CheckInPhase;
					bool marked = checkInPhase.Marked;
					bool readyForGet = checkInPhase.ReadyForGet;
					if (readyForGet)
					{
						this._availablePhase = checkInPhase;
					}
					uint gameVipDoubleLv = checkInPhase.GetGameVipDoubleLv();
					CUseable useable = checkInPhase.GetUseable(0);
					if (useable != null)
					{
						GameObject gameObject = transform.FindChild(string.Format("itemCell{0}", i + 1)).gameObject;
						if (gameObject != null)
						{
							this.SetItem(useable, gameObject, marked, readyForGet, gameVipDoubleLv);
						}
					}
					if (!flag && readyForGet)
					{
						flag = true;
					}
				}
				Transform transform2 = this._form.gameObject.transform.FindChild("Panel/BtnCheck");
				CUICommonSystem.SetButtonEnable(transform2.GetComponent<Button>(), flag, flag, true);
				Transform transform3 = this._form.gameObject.transform.FindChild("Panel/MeinvPic");
				MonoSingleton<BannerImageSys>.GetInstance().TrySetCheckInImage(transform3.gameObject.GetComponent<Image>());
				Transform transform4 = this._form.gameObject.transform.FindChild("Panel/Title/Text");
				transform4.gameObject.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Title");
				Text component = this._form.gameObject.transform.FindChild("Panel/Desc").gameObject.GetComponent<Text>();
				component.text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Desc");
				if (Singleton<ApolloHelper>.instance.IsWXGameCenter)
				{
					this._form.gameObject.transform.FindChild("Panel/WeChatPrivilege").gameObject.CustomSetActive(true);
					Text component2 = this._form.gameObject.transform.FindChild("Panel/WeChatPrivilege/Desc").gameObject.GetComponent<Text>();
					component2.text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_WeChatPrivilege_Desc");
				}
				else
				{
					this._form.gameObject.transform.FindChild("Panel/WeChatPrivilege").gameObject.CustomSetActive(false);
				}
				DateTime dateTime = Utility.ToUtcTime2Local(this._curActivity.StartTime);
				DateTime dateTime2 = Utility.ToUtcTime2Local(this._curActivity.CloseTime);
				string text = string.Format("{0}.{1}.{2}", dateTime.Year, dateTime.Month, dateTime.Day);
				string text2 = string.Format("{0}.{1}.{2}", dateTime2.Year, dateTime2.Month, dateTime2.Day);
				string text3 = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Date", new string[]
				{
					text,
					text2
				});
				Text component3 = this._form.gameObject.transform.FindChild("Panel/Date").gameObject.GetComponent<Text>();
				component3.text = text3;
			}
		}

		private void SetItem(CUseable usable, GameObject uiNode, bool received, bool ready, uint vipLv)
		{
			SevenDayCheckHelper component = uiNode.GetComponent<SevenDayCheckHelper>();
			Image component2 = component.Icon.GetComponent<Image>();
			CUIUtility.SetImageSprite(component2, usable.GetIconPath(), this._form, true, false, false, false);
			component.ItemName.GetComponent<Text>().text = usable.m_name;
			if (vipLv > 0u)
			{
				component.NobeRoot.CustomSetActive(true);
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.Nobe.GetComponent<Image>(), (int)vipLv, false, true, 0uL);
			}
			else
			{
				component.NobeRoot.CustomSetActive(false);
			}
			if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(usable.m_baseID)) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
			{
				component.IconBg.CustomSetActive(true);
			}
			else
			{
				component.IconBg.CustomSetActive(false);
			}
			Transform transform = component.Tiyan.transform;
			if (transform != null)
			{
				if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(usable.m_baseID))
				{
					transform.gameObject.CustomSetActive(true);
					transform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
				}
				else if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(usable.m_baseID))
				{
					transform.gameObject.CustomSetActive(true);
					transform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
				}
				else
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
			Transform transform2 = component.ItemNumText.transform;
			if (transform2 != null)
			{
				Text component3 = transform2.GetComponent<Text>();
				if (usable.m_stackCount < 10000)
				{
					component3.text = usable.m_stackCount.ToString();
				}
				else
				{
					component3.text = usable.m_stackCount / 10000 + "万";
				}
				CUICommonSystem.AppendMultipleText(component3, usable.m_stackMulti);
				if (usable.m_stackCount <= 1)
				{
					component3.gameObject.CustomSetActive(false);
					component.ItemNum.CustomSetActive(false);
				}
				else
				{
					component.ItemNum.CustomSetActive(true);
					component.ItemNumText.CustomSetActive(true);
				}
				if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					if (((CSymbolItem)usable).IsGuildSymbol())
					{
						component3.text = string.Empty;
					}
					else
					{
						component3.text = usable.GetSalableCount().ToString();
					}
				}
			}
			if (received)
			{
				component.GrayMask.CustomSetActive(true);
			}
			else
			{
				component.GrayMask.CustomSetActive(false);
			}
			if (ready)
			{
				component.Effect.CustomSetActive(true);
			}
			else
			{
				component.Effect.CustomSetActive(false);
			}
			CUIEventScript component4 = uiNode.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = new stUIEventParams
			{
				iconUseable = usable
			};
			component4.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
			component4.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
			component4.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
			component4.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
		}

		protected void OnRequeset(CUIEvent uiEvent)
		{
			if (this._form != null && this._curActivity != null && this._availablePhase != null)
			{
				this._availablePhase.DrawReward();
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SevenCheck_CloseForm);
			}
		}

		protected void OnCloseSevenDayCheckForm(CUIEvent uiEvent)
		{
			if (this._curActivity != null)
			{
				this._curActivity.OnMaskStateChange -= new Activity.ActivityEvent(this.ActivityEvent);
				this._curActivity.OnTimeStateChange -= new Activity.ActivityEvent(this.ActivityEvent);
				this._curActivity = null;
			}
			if (this._form != null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(this.FormName);
				this._form = null;
			}
			Singleton<Day14CheckSystem>.GetInstance().OnLoginOpen(null);
		}

		internal void Clear()
		{
		}
	}
}
