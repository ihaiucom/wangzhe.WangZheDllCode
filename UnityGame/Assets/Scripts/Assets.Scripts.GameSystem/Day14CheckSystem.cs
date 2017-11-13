using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class Day14CheckSystem : Singleton<Day14CheckSystem>
	{
		private CUIFormScript _form;

		public readonly string FormName = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/Form_14DayCheck.prefab");

		public bool IsShowingLoginOpen;

		private bool m_bOpenLink;

		private int m_SelectIDx;

		private int[] m_DisplayIndex = new int[]
		{
			1,
			5,
			7,
			12
		};

		private CheckInActivity _curActivity;

		private CheckInPhase _availablePhase;

		private static string[] m_Days = new string[]
		{
			"一",
			"二",
			"三",
			"四",
			"五",
			"六",
			"七",
			"八",
			"九",
			"十",
			"十一",
			"十二",
			"十三",
			"十四"
		};

		public override void Init()
		{
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Day14Check_OnItemEnable, new CUIEventManager.OnUIEventHandler(this.OnCheckItemEnable));
			instance.AddUIEventListener(enUIEventID.Day14Check_OnRequestCheck, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
			instance.AddUIEventListener(enUIEventID.Day14Check_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
			instance.AddUIEventListener(enUIEventID.Day14Check_LeftUIItemEnable, new CUIEventManager.OnUIEventHandler(this.OnLeftUIItemEnable));
			instance.AddUIEventListener(enUIEventID.Day14Check_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
		}

		public override void UnInit()
		{
			base.UnInit();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Day14Check_OnItemEnable, new CUIEventManager.OnUIEventHandler(this.OnCheckItemEnable));
			instance.RemoveUIEventListener(enUIEventID.Day14Check_OnRequestCheck, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
			instance.RemoveUIEventListener(enUIEventID.Day14Check_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
			instance.RemoveUIEventListener(enUIEventID.Day14Check_LeftUIItemEnable, new CUIEventManager.OnUIEventHandler(this.OnLeftUIItemEnable));
			instance.RemoveUIEventListener(enUIEventID.Day14Check_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
		}

		public void OnLoginOpen(CUIEvent uiEvent)
		{
			this.m_SelectIDx = 0;
			if (uiEvent == null)
			{
				this.m_bOpenLink = false;
			}
			else
			{
				this.m_bOpenLink = true;
			}
			if (this._form == null)
			{
				bool flag = false;
				ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList((Activity actv) => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_14CHECK);
				if (activityList != null && activityList.Count > 0)
				{
					this._curActivity = (CheckInActivity)activityList[0];
					ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
					for (int i = 0; i < phaseList.Count; i++)
					{
						if (phaseList[i].ReadyForGet)
						{
							this.m_SelectIDx = i;
							flag = true;
							break;
						}
					}
					if (flag || this.m_bOpenLink)
					{
						this._curActivity.OnMaskStateChange += new Activity.ActivityEvent(this.ActivityEvent);
						this._curActivity.OnTimeStateChange += new Activity.ActivityEvent(this.ActivityEvent);
						this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
						this.InitUI();
						this.InitLeftUI();
						if (this._form != null)
						{
							Transform transform = this._form.gameObject.transform.FindChild("Panel/BtnCheck");
							CUICommonSystem.SetButtonEnable(transform.GetComponent<Button>(), flag, flag, true);
						}
					}
					else
					{
						this._curActivity = null;
					}
				}
			}
		}

		private void InitUI()
		{
			if (this._curActivity == null)
			{
				return;
			}
			ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
			Transform transform = this._form.gameObject.transform.FindChild("Panel/ItemContainer");
			if (transform != null)
			{
				CUIListScript component = transform.GetComponent<CUIListScript>();
				if (component == null)
				{
					return;
				}
				if (component != null)
				{
					component.SetElementAmount(phaseList.Count);
				}
				if (this.m_SelectIDx < phaseList.Count)
				{
					component.MoveElementInScrollArea(this.m_SelectIDx, true);
				}
			}
			DateTime dateTime = Utility.ToUtcTime2Local(this._curActivity.StartTime);
			DateTime dateTime2 = Utility.ToUtcTime2Local(this._curActivity.CloseTime);
			string text = string.Format("{0}.{1}.{2}", dateTime.get_Year(), dateTime.get_Month(), dateTime.get_Day());
			string text2 = string.Format("{0}.{1}.{2}", dateTime2.get_Year(), dateTime2.get_Month(), dateTime2.get_Day());
			string text3 = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Date", new string[]
			{
				text,
				text2
			});
			Text component2 = this._form.gameObject.transform.FindChild("Panel/Date").gameObject.GetComponent<Text>();
			component2.set_text(text3);
			Transform transform2 = this._form.gameObject.transform.FindChild("Panel/TopPic/MaskBg");
			if (transform2)
			{
				MonoSingleton<BannerImageSys>.GetInstance().TrySet14CheckInImage(transform2.GetComponent<Image>());
			}
		}

		private void GetCheckParams()
		{
			if (this._curActivity != null && GameDataMgr.resWealParamDict.ContainsKey(this._curActivity.EntranceParam))
			{
				ResWealParam resWealParam = new ResWealParam();
				bool flag = GameDataMgr.resWealParamDict.TryGetValue(this._curActivity.EntranceParam, out resWealParam);
				if (flag)
				{
					if ((long)this.m_DisplayIndex.Length != (long)((ulong)resWealParam.dwNum))
					{
						this.m_DisplayIndex = new int[resWealParam.dwNum];
					}
					int num = 0;
					while ((long)num < (long)((ulong)resWealParam.dwNum))
					{
						this.m_DisplayIndex[num] = resWealParam.Param[num];
						num++;
					}
				}
			}
		}

		private void InitLeftUI()
		{
			this.GetCheckParams();
			if (this._curActivity == null)
			{
				return;
			}
			int elementAmount = 4;
			ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
			Transform transform = this._form.gameObject.transform.FindChild("Panel/LeftContainer");
			if (transform != null)
			{
				CUIListScript component = transform.GetComponent<CUIListScript>();
				if (component != null)
				{
					component.SetElementAmount(elementAmount);
				}
			}
		}

		private void OnCheckItemEnable(CUIEvent uiEvent)
		{
			if (this._curActivity == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
			bool flag = false;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < phaseList.Count)
			{
				CheckInPhase checkInPhase = phaseList[srcWidgetIndexInBelongedList] as CheckInPhase;
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
					Transform transform = uiEvent.m_srcWidget.transform;
					if (transform != null)
					{
						this.SetItem(useable, transform, marked, readyForGet, gameVipDoubleLv, srcWidgetIndexInBelongedList);
					}
				}
				if (flag || readyForGet)
				{
				}
			}
		}

		private void OnLeftUIItemEnable(CUIEvent uiEvent)
		{
			if (this._curActivity == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
			Transform transform = uiEvent.m_srcWidget.transform;
			int num = this.m_DisplayIndex.Length;
			if (num <= srcWidgetIndexInBelongedList)
			{
				return;
			}
			int num2 = this.m_DisplayIndex[srcWidgetIndexInBelongedList];
			num2--;
			if (num2 >= 0 && num2 < phaseList.Count)
			{
				CheckInPhase checkInPhase = phaseList[num2] as CheckInPhase;
				if (checkInPhase != null)
				{
					bool marked = checkInPhase.Marked;
					bool readyForGet = checkInPhase.ReadyForGet;
					if (readyForGet)
					{
						this._availablePhase = checkInPhase;
					}
					uint gameVipDoubleLv = checkInPhase.GetGameVipDoubleLv();
					CUseable useable = checkInPhase.GetUseable(0);
					if (useable != null && transform != null)
					{
						this.SetLeftItem(useable, transform, marked, readyForGet, gameVipDoubleLv, num2);
					}
				}
			}
		}

		protected void ActivityEvent(Activity acty)
		{
		}

		private static string GetDay(int i)
		{
			int num = Day14CheckSystem.m_Days.Length;
			if (i < num)
			{
				return Day14CheckSystem.m_Days[i];
			}
			return string.Empty;
		}

		private void SetItem(CUseable usable, Transform uiNode, bool received, bool ready, uint vipLv, int elemIdx)
		{
			Transform transform = uiNode.transform.FindChild("DayBg/DayText");
			if (transform != null)
			{
				transform.GetComponent<Text>().set_text(string.Format("第{0}天", Day14CheckSystem.GetDay(elemIdx)));
			}
			Transform transform2 = uiNode.transform.FindChild("ItemIcon");
			if (transform2 != null)
			{
				CUIUtility.SetImageSprite(transform2.GetComponent<Image>(), usable.GetIconPath(), this._form, true, false, false, false);
			}
			Transform transform3 = uiNode.transform.FindChild("ItemName");
			if (transform3 != null)
			{
				transform3.GetComponent<Text>().set_text(usable.m_name);
			}
			Transform transform4 = uiNode.transform.FindChild("Bg");
			if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(usable.m_baseID)) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
			{
				if (transform4)
				{
					transform4.gameObject.CustomSetActive(true);
				}
			}
			else if (transform4)
			{
				transform4.gameObject.CustomSetActive(false);
			}
			Transform transform5 = uiNode.transform.FindChild("TiyanMask");
			if (transform5 != null)
			{
				if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(usable.m_baseID))
				{
					transform5.gameObject.CustomSetActive(true);
					transform5.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
				}
				else if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(usable.m_baseID))
				{
					transform5.gameObject.CustomSetActive(true);
					transform5.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
				}
				else
				{
					transform5.gameObject.CustomSetActive(false);
				}
			}
			Transform transform6 = uiNode.transform.FindChild("ItemNum/ItemNumText");
			if (transform6 != null)
			{
				Text component = transform6.GetComponent<Text>();
				if (usable.m_stackCount < 10000)
				{
					component.set_text(usable.m_stackCount.ToString());
				}
				else
				{
					component.set_text(usable.m_stackCount / 10000 + "万");
				}
				CUICommonSystem.AppendMultipleText(component, usable.m_stackMulti);
				if (usable.m_stackCount <= 1)
				{
					component.gameObject.CustomSetActive(false);
					uiNode.transform.FindChild("ItemNum").gameObject.CustomSetActive(false);
				}
				else
				{
					uiNode.transform.FindChild("ItemNum").gameObject.CustomSetActive(true);
					transform6.gameObject.CustomSetActive(true);
				}
				if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					if (((CSymbolItem)usable).IsGuildSymbol())
					{
						component.set_text(string.Empty);
					}
					else
					{
						component.set_text(usable.GetSalableCount().ToString());
					}
				}
			}
			Transform transform7 = uiNode.transform.FindChild("LingQuGou");
			if (transform7)
			{
				if (received)
				{
					transform7.gameObject.CustomSetActive(true);
				}
				else
				{
					transform7.gameObject.CustomSetActive(false);
				}
			}
			Transform transform8 = uiNode.transform.FindChild("XiYou");
			if (transform8)
			{
				if (ready)
				{
					transform8.gameObject.CustomSetActive(true);
					Transform transform9 = transform8.transform.FindChild("Bg/Text");
					if (transform9 != null)
					{
						transform9.GetComponent<Text>().set_text(string.Format("第{0}天", Day14CheckSystem.GetDay(elemIdx)));
					}
				}
				else
				{
					transform8.gameObject.CustomSetActive(false);
				}
			}
			CUIEventScript component2 = uiNode.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = new stUIEventParams
			{
				iconUseable = usable
			};
			component2.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
			component2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
			component2.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
			component2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
		}

		private void SetLeftItem(CUseable usable, Transform uiNode, bool received, bool ready, uint vipLv, int elemIdx)
		{
			Transform transform = uiNode.transform.FindChild("ItemIcon");
			if (transform != null)
			{
				CUIUtility.SetImageSprite(transform.GetComponent<Image>(), usable.GetIconPath(), this._form, true, false, false, false);
			}
			Transform transform2 = uiNode.transform.FindChild("GotCeck");
			if (transform2)
			{
				if (received)
				{
					transform2.gameObject.CustomSetActive(true);
				}
				else
				{
					transform2.gameObject.CustomSetActive(false);
				}
			}
			Transform transform3 = uiNode.transform.FindChild("TiyanMask");
			if (transform3 != null)
			{
				if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(usable.m_baseID))
				{
					transform3.gameObject.CustomSetActive(true);
					transform3.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
				}
				else if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(usable.m_baseID))
				{
					transform3.gameObject.CustomSetActive(true);
					transform3.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
				}
				else
				{
					transform3.gameObject.CustomSetActive(false);
				}
			}
			Transform transform4 = uiNode.transform.FindChild("Bg");
			if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(usable.m_baseID)) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
			{
				if (transform4)
				{
					transform4.gameObject.CustomSetActive(true);
				}
			}
			else if (transform4)
			{
				transform4.gameObject.CustomSetActive(false);
			}
			Transform transform5 = uiNode.transform.FindChild("Name");
			if (transform5 != null)
			{
				transform5.GetComponent<Text>().set_text(usable.m_name);
			}
			Transform transform6 = uiNode.transform.FindChild("Num");
			if (transform6)
			{
				transform6.GetComponent<Text>().set_text((elemIdx + 1).ToString());
			}
			CUIEventScript component = uiNode.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = new stUIEventParams
			{
				iconUseable = usable
			};
			component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
			component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
			component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
			component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
		}

		protected void OnRequeset(CUIEvent uiEvent)
		{
			if (this._form != null && this._curActivity != null && this._availablePhase != null)
			{
				this._availablePhase.DrawReward();
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Day14Check_CloseForm);
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
		}

		internal void Clear()
		{
		}
	}
}
