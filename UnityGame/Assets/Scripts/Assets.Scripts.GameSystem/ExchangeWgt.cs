using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class ExchangeWgt : ActivityWidget
	{
		private class ExchangeElement
		{
			public ExchangePhase phase;

			public GameObject uiItem;

			public ExchangeWgt owner;

			public int index;

			public ExchangeElement(ExchangePhase phase, GameObject uiItem, ExchangeWgt owner, int index)
			{
				this.phase = phase;
				this.uiItem = uiItem;
				this.owner = owner;
				this.index = index;
				this.Validate();
			}

			public void Validate()
			{
				if (this.phase == null || this.uiItem == null)
				{
					return;
				}
				this.uiItem.CustomSetActive(true);
				ResDT_Item_Info resDT_Item_Info = null;
				ResDT_Item_Info resDT_Item_Info2 = null;
				ResDT_Item_Info stResItemInfo = this.phase.Config.stResItemInfo;
				if (this.phase.Config.bColItemCnt > 0)
				{
					resDT_Item_Info = this.phase.Config.astColItemInfo[0];
				}
				if (this.phase.Config.bColItemCnt > 1)
				{
					resDT_Item_Info2 = this.phase.Config.astColItemInfo[1];
				}
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
				if (useableContainer == null)
				{
					return;
				}
				int arg_CB_0 = (resDT_Item_Info == null) ? 0 : useableContainer.GetUseableStackCount((COM_ITEM_TYPE)resDT_Item_Info.wItemType, resDT_Item_Info.dwItemID);
				int arg_EC_0 = (resDT_Item_Info2 == null) ? 0 : useableContainer.GetUseableStackCount((COM_ITEM_TYPE)resDT_Item_Info2.wItemType, resDT_Item_Info2.dwItemID);
				if (stResItemInfo != null)
				{
					GameObject gameObject = this.uiItem.transform.FindChild("DuihuanBtn").gameObject;
					gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint)this.index;
					bool isEnable = this.owner.view.activity.timeState == Activity.TimeState.Going;
					CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), isEnable, true);
					if (resDT_Item_Info != null)
					{
						CUseable itemUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)resDT_Item_Info.wItemType, resDT_Item_Info.dwItemID, 1);
						GameObject gameObject2 = this.uiItem.transform.FindChild("Panel/ItemCell1").gameObject;
						CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject2, itemUseable, true, false, false, false);
						int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE)resDT_Item_Info.wItemType, resDT_Item_Info.dwItemID);
						ushort wItemCnt = resDT_Item_Info.wItemCnt;
						Text component = this.uiItem.transform.FindChild("Panel/ItemCell1/ItemCount").gameObject.GetComponent<Text>();
						if (useableStackCount < (int)wItemCnt)
						{
							component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), useableStackCount, wItemCnt));
							CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), false, true);
						}
						else
						{
							component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), useableStackCount, wItemCnt));
						}
					}
					if (resDT_Item_Info2 != null)
					{
						CUseable itemUseable2 = CUseableManager.CreateUseable((COM_ITEM_TYPE)resDT_Item_Info2.wItemType, resDT_Item_Info2.dwItemID, 1);
						GameObject gameObject3 = this.uiItem.transform.FindChild("Panel/ItemCell2").gameObject;
						gameObject3.CustomSetActive(true);
						CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject3, itemUseable2, true, false, false, false);
						int useableStackCount2 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE)resDT_Item_Info2.wItemType, resDT_Item_Info2.dwItemID);
						ushort wItemCnt2 = resDT_Item_Info2.wItemCnt;
						Text component2 = this.uiItem.transform.FindChild("Panel/ItemCell2/ItemCount").gameObject.GetComponent<Text>();
						if (useableStackCount2 < (int)wItemCnt2)
						{
							component2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), useableStackCount2, wItemCnt2));
							CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), false, true);
						}
						else
						{
							component2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), useableStackCount2, wItemCnt2));
						}
					}
					else
					{
						GameObject gameObject4 = this.uiItem.transform.FindChild("Panel/ItemCell2").gameObject;
						gameObject4.CustomSetActive(false);
						GameObject gameObject5 = this.uiItem.transform.FindChild("Panel/Add").gameObject;
						gameObject5.CustomSetActive(false);
					}
					CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)stResItemInfo.wItemType, stResItemInfo.dwItemID, (int)stResItemInfo.wItemCnt);
					GameObject gameObject6 = this.uiItem.transform.FindChild("Panel/GetItemCell").gameObject;
					bool bActive = false;
					if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
					{
						CHeroItem cHeroItem = (CHeroItem)cUseable;
						if (cHeroItem != null)
						{
							CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo2 != null)
							{
								bActive = masterRoleInfo2.IsHaveHero(cHeroItem.m_baseID, true);
							}
						}
					}
					else if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
					{
						CHeroSkin cHeroSkin = (CHeroSkin)cUseable;
						if (cHeroSkin != null)
						{
							CRoleInfo masterRoleInfo3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo3 != null)
							{
								bActive = masterRoleInfo3.IsHaveHeroSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, false);
							}
						}
					}
					Transform transform = this.uiItem.transform.FindChild("Panel/GetItemCell/HaveItemFlag");
					if (transform != null && transform.gameObject != null)
					{
						transform.gameObject.CustomSetActive(bActive);
					}
					if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						CItem cItem = cUseable as CItem;
						if (cItem != null && cItem.m_itemData.bIsView > 0)
						{
							CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject6, cUseable, true, false, false, true);
						}
						else
						{
							CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject6, cUseable, true, false, false, false);
							if (gameObject6 != null)
							{
								CUIEventScript component3 = gameObject6.GetComponent<CUIEventScript>();
								if (component3 != null)
								{
									component3.SetUIEvent(enUIEventType.Click, enUIEventID.None);
								}
							}
						}
					}
					else
					{
						CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject6, cUseable, true, false, false, false);
						if (gameObject6 != null)
						{
							CUIEventScript component4 = gameObject6.GetComponent<CUIEventScript>();
							if (component4 != null)
							{
								component4.SetUIEvent(enUIEventType.Click, enUIEventID.None);
							}
						}
					}
					ExchangeActivity exchangeActivity = this.owner.view.activity as ExchangeActivity;
					if (exchangeActivity != null)
					{
						GameObject gameObject7 = this.uiItem.transform.FindChild("ExchangeCount").gameObject;
						uint maxExchangeCount = exchangeActivity.GetMaxExchangeCount((int)this.phase.Config.bIdx);
						uint exchangeCount = exchangeActivity.GetExchangeCount((int)this.phase.Config.bIdx);
						if (maxExchangeCount > 0u)
						{
							gameObject7.CustomSetActive(true);
							gameObject7.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_TimeLimit"), exchangeCount, maxExchangeCount));
							if (exchangeCount >= maxExchangeCount)
							{
								CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), false, true);
							}
						}
						else
						{
							gameObject7.CustomSetActive(false);
						}
					}
				}
			}
		}

		private const float SpacingY = 5f;

		private GameObject _elementTmpl;

		private ListView<ExchangeWgt.ExchangeElement> _elementList;

		public ExchangeWgt(GameObject node, ActivityView view) : base(node, view)
		{
			ListView<ActivityPhase> phaseList = view.activity.PhaseList;
			this._elementList = new ListView<ExchangeWgt.ExchangeElement>();
			this._elementTmpl = Utility.FindChild(node, "Template");
			float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
			for (int i = 0; i < phaseList.Count; i++)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(this._elementTmpl);
				if (gameObject != null)
				{
					gameObject.GetComponent<RectTransform>().sizeDelta = this._elementTmpl.GetComponent<RectTransform>().sizeDelta;
					gameObject.transform.SetParent(this._elementTmpl.transform.parent);
					gameObject.transform.localPosition = this._elementTmpl.transform.localPosition + new Vector3(0f, -(height + 5f) * (float)i, 0f);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.identity;
					this._elementList.Add(new ExchangeWgt.ExchangeElement(phaseList[phaseList.Count - i - 1] as ExchangePhase, gameObject, this, i));
				}
			}
			node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, height * (float)phaseList.Count + (float)(phaseList.Count - 1) * 5f);
			this._elementTmpl.CustomSetActive(false);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Exchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeHeroSkinConfirm, new CUIEventManager.OnUIEventHandler(this.OnExchangeHeroSkinConfirm));
			view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
		}

		private void OnStateChange(Activity acty)
		{
			this.Validate();
		}

		public override void Clear()
		{
			base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Exchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeHeroSkinConfirm, new CUIEventManager.OnUIEventHandler(this.OnExchangeHeroSkinConfirm));
			if (this._elementList != null)
			{
				for (int i = 0; i < this._elementList.Count; i++)
				{
					if (this._elementList[i].uiItem)
					{
						CUICommonSystem.DestoryObj(this._elementList[i].uiItem, 0.1f);
					}
				}
				this._elementList = null;
			}
			this._elementTmpl = null;
		}

		public override void Validate()
		{
			if (this._elementList == null)
			{
				return;
			}
			for (int i = 0; i < this._elementList.Count; i++)
			{
				this._elementList[i].Validate();
			}
		}

		private void OnExchangeHeroSkinConfirm(CUIEvent uiEvent)
		{
			if (this._elementList == null)
			{
				return;
			}
			int commonUInt32Param = (int)uiEvent.m_eventParams.commonUInt32Param1;
			if (commonUInt32Param >= 0 && commonUInt32Param < this._elementList.Count)
			{
				ResDT_Item_Info stResItemInfo = this._elementList[commonUInt32Param].phase.Config.stResItemInfo;
				CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
				int maxExchangeCount = this._elementList[commonUInt32Param].phase.GetMaxExchangeCount();
				if (maxExchangeCount > 1)
				{
					stUIEventParams par = default(stUIEventParams);
					par.commonUInt16Param1 = (ushort)commonUInt32Param;
					Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(cUseable, maxExchangeCount, enUIEventID.Activity_ExchangeCountReady, par, 0u, 0u);
				}
				else
				{
					string arg_C2_0 = (cUseable == null) ? string.Empty : cUseable.m_name;
					stUIEventParams par2 = default(stUIEventParams);
					par2.commonUInt16Param1 = (ushort)commonUInt32Param;
					this._elementList[commonUInt32Param].phase.SetExchangeCountOnce(1);
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_ExchangeConfirm, par2);
				}
			}
		}

		private void OnClickExchange(CUIEvent uiEvent)
		{
			if (this._elementList == null)
			{
				return;
			}
			int commonUInt32Param = (int)uiEvent.m_eventParams.commonUInt32Param1;
			if (commonUInt32Param >= 0 && commonUInt32Param < this._elementList.Count)
			{
				ResDT_Item_Info stResItemInfo = this._elementList[commonUInt32Param].phase.Config.stResItemInfo;
				CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
				bool flag = false;
				int num = 0;
				if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
				{
					CHeroItem cHeroItem = (CHeroItem)cUseable;
					if (cHeroItem != null)
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo != null)
						{
							flag = masterRoleInfo.IsHaveHero(cHeroItem.m_baseID, true);
						}
					}
				}
				else if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
				{
					CHeroSkin cHeroSkin = (CHeroSkin)cUseable;
					if (cHeroSkin != null)
					{
						CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo2 != null)
						{
							flag = masterRoleInfo2.IsHaveHeroSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, false);
						}
					}
				}
				else if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = (CItem)cUseable;
					if (cItem != null)
					{
						CRoleInfo masterRoleInfo3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (cItem.m_itemData != null && masterRoleInfo3 != null && cItem.m_itemData.bType == 4)
						{
							int num2 = (int)cItem.m_itemData.EftParam[0];
							if (num2 > 0)
							{
								ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey((long)num2);
								if (dataByKey != null)
								{
									for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
									{
										if (dataByKey.astRewardDetail[i].bItemType == 0 || dataByKey.astRewardDetail[i].bItemType >= 18)
										{
											break;
										}
										if (dataByKey.astRewardDetail[i].bItemType == 4)
										{
											if (!masterRoleInfo3.IsHaveHero(dataByKey.astRewardDetail[i].dwItemID, true))
											{
												num = 0;
												break;
											}
											num = 1;
										}
										else if (dataByKey.astRewardDetail[i].bItemType == 11)
										{
											if (!masterRoleInfo3.IsHaveHeroSkin(dataByKey.astRewardDetail[i].dwItemID, true))
											{
												num = 0;
												break;
											}
											num = 1;
										}
										else if (dataByKey.astRewardDetail[i].bItemType > 0 && dataByKey.astRewardDetail[i].bItemType < 18)
										{
											num = 0;
										}
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeWgt_Hero_Tips"), cUseable.m_name);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Activity_ExchangeHeroSkinConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
					return;
				}
				if (num == 1)
				{
					string text = Singleton<CTextManager>.GetInstance().GetText("ExchangeWgt_Have_AllGift");
					uiEvent.m_eventParams.taskId = 1u;
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Activity_ExchangeHeroSkinConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
					return;
				}
				int maxExchangeCount = this._elementList[commonUInt32Param].phase.GetMaxExchangeCount();
				if (maxExchangeCount > 1)
				{
					stUIEventParams par = default(stUIEventParams);
					par.commonUInt16Param1 = (ushort)commonUInt32Param;
					Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(cUseable, maxExchangeCount, enUIEventID.Activity_ExchangeCountReady, par, 0u, 0u);
				}
				else
				{
					string text2 = (cUseable == null) ? string.Empty : cUseable.m_name;
					stUIEventParams par2 = default(stUIEventParams);
					par2.commonUInt16Param1 = (ushort)commonUInt32Param;
					this._elementList[commonUInt32Param].phase.SetExchangeCountOnce(1);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), maxExchangeCount, text2), enUIEventID.Activity_ExchangeConfirm, enUIEventID.None, par2, false);
				}
			}
		}

		private void OnClickExchangeConfirm(CUIEvent uiEvent)
		{
			if (this._elementList == null)
			{
				return;
			}
			int commonUInt16Param = (int)uiEvent.m_eventParams.commonUInt16Param1;
			if (commonUInt16Param >= 0 && commonUInt16Param < this._elementList.Count)
			{
				this._elementList[commonUInt16Param].phase.DrawReward();
			}
		}

		private void OnClickExchangeCountSelectReady(CUIEvent uiEvent)
		{
			if (this._elementList != null)
			{
				uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
				int commonUInt16Param = (int)uiEvent.m_eventParams.commonUInt16Param1;
				if (commonUInt16Param >= 0 && commonUInt16Param < this._elementList.Count)
				{
					ResDT_Item_Info stResItemInfo = this._elementList[commonUInt16Param].phase.Config.stResItemInfo;
					CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
					string text = (cUseable == null) ? string.Empty : cUseable.m_name;
					stUIEventParams par = default(stUIEventParams);
					par.commonUInt16Param1 = (ushort)commonUInt16Param;
					this._elementList[commonUInt16Param].phase.SetExchangeCountOnce((int)commonUInt32Param);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), commonUInt32Param, text), enUIEventID.Activity_ExchangeConfirm, enUIEventID.None, par, false);
				}
			}
		}
	}
}
