using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class PointsExchangeWgt : ActivityWidget
	{
		private class PointsExchangeElement
		{
			public PointsExchangePhase phase;

			public GameObject uiItem;

			public PointsExchangeWgt owner;

			public int index;

			public PointsExchangeElement(PointsExchangePhase phase, GameObject uiItem, PointsExchangeWgt owner, int index)
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
				ResDT_PointExchange config = this.phase.Config;
				PointsExchangeActivity pointsExchangeActivity = this.phase.Owner as PointsExchangeActivity;
				if (pointsExchangeActivity == null || pointsExchangeActivity.PointsConfig == null)
				{
					return;
				}
				ResWealPointExchange pointsConfig = pointsExchangeActivity.PointsConfig;
				GameObject gameObject = this.uiItem.transform.FindChild("DuihuanBtn").gameObject;
				gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint)this.index;
				uint maxExchangeCount = pointsExchangeActivity.GetMaxExchangeCount(this.index);
				uint exchangeCount = pointsExchangeActivity.GetExchangeCount(this.index);
				uint dwPointCnt = config.dwPointCnt;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				uint jiFen = masterRoleInfo.JiFen;
				bool isEnable = jiFen >= dwPointCnt && (maxExchangeCount == 0u || exchangeCount < maxExchangeCount);
				if (this.owner.view.activity.timeState != Activity.TimeState.Going)
				{
					CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), false, true);
				}
				else
				{
					CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), isEnable, true);
				}
				CUseable itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianJuanJiFen, 1);
				GameObject gameObject2 = this.uiItem.transform.FindChild("Panel/PointsCell").gameObject;
				CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject2, itemUseable, true, false, false, false);
				CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)config.wResItemType, config.dwResItemID, (int)config.wResItemCnt);
				GameObject gameObject3 = this.uiItem.transform.FindChild("Panel/GetItemCell").gameObject;
				if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = cUseable as CItem;
					if (cItem != null && cItem.m_itemData.bIsView > 0)
					{
						CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject3, cUseable, true, false, false, true);
					}
					else
					{
						CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject3, cUseable, true, false, false, false);
						if (gameObject3 != null)
						{
							CUIEventScript component = gameObject3.GetComponent<CUIEventScript>();
							if (component != null)
							{
								component.SetUIEvent(enUIEventType.Click, enUIEventID.None);
							}
						}
					}
				}
				else
				{
					CUICommonSystem.SetItemCell(this.owner.view.form.formScript, gameObject3, cUseable, true, false, false, false);
					if (gameObject3 != null)
					{
						CUIEventScript component2 = gameObject3.GetComponent<CUIEventScript>();
						if (component2 != null)
						{
							component2.SetUIEvent(enUIEventType.Click, enUIEventID.None);
						}
					}
				}
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
				Text component3 = this.uiItem.transform.FindChild("Panel/PointsCell/ItemCount").gameObject.GetComponent<Text>();
				if (jiFen < config.dwPointCnt)
				{
					component3.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), jiFen, dwPointCnt);
					CUICommonSystem.SetButtonEnableWithShader(gameObject.GetComponent<Button>(), false, true);
				}
				else
				{
					component3.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), jiFen, dwPointCnt);
				}
				GameObject gameObject4 = this.uiItem.transform.FindChild("ExchangeCount").gameObject;
				if (maxExchangeCount > 0u)
				{
					gameObject4.CustomSetActive(true);
					gameObject4.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_TimeLimit"), exchangeCount, maxExchangeCount);
				}
				else
				{
					gameObject4.CustomSetActive(false);
				}
			}
		}

		private const float SpacingY = 5f;

		private GameObject _elementTmpl;

		private ListView<PointsExchangeWgt.PointsExchangeElement> _elementList;

		public PointsExchangeWgt(GameObject node, ActivityView view) : base(node, view)
		{
			ListView<ActivityPhase> phaseList = view.activity.PhaseList;
			this._elementList = new ListView<PointsExchangeWgt.PointsExchangeElement>();
			this._elementTmpl = Utility.FindChild(node, "Template");
			float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
			for (int i = 0; i < phaseList.Count; i++)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this._elementTmpl);
				if (gameObject != null)
				{
					gameObject.GetComponent<RectTransform>().sizeDelta = this._elementTmpl.GetComponent<RectTransform>().sizeDelta;
					gameObject.transform.SetParent(this._elementTmpl.transform.parent);
					gameObject.transform.localPosition = this._elementTmpl.transform.localPosition + new Vector3(0f, -(height + 5f) * (float)i, 0f);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.identity;
					this._elementList.Add(new PointsExchangeWgt.PointsExchangeElement(phaseList[i] as PointsExchangePhase, gameObject, this, i));
				}
			}
			node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, height * (float)phaseList.Count + (float)(phaseList.Count - 1) * 5f);
			this._elementTmpl.CustomSetActive(false);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeHeroSkinConfirm, new CUIEventManager.OnUIEventHandler(this.OnExchangeHeroSkinConfirm));
			view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
		}

		private void OnStateChange(Activity acty)
		{
			this.Validate();
			ActivityView view = base.view;
			if (view != null)
			{
				ListView<ActivityWidget> widgetList = view.WidgetList;
				for (int i = 0; i < widgetList.Count; i++)
				{
					IntrodWidget introdWidget = widgetList[i] as IntrodWidget;
					if (introdWidget != null)
					{
						introdWidget.Validate();
						break;
					}
				}
			}
		}

		public override void Clear()
		{
			base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
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
				uint dwResItemID = this._elementList[commonUInt32Param].phase.Config.dwResItemID;
				CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)this._elementList[commonUInt32Param].phase.Config.wResItemType, this._elementList[commonUInt32Param].phase.Config.dwResItemID, (int)this._elementList[commonUInt32Param].phase.Config.wResItemCnt);
				int maxExchangeCount = this._elementList[commonUInt32Param].phase.GetMaxExchangeCount();
				if (maxExchangeCount > 1)
				{
					stUIEventParams par = default(stUIEventParams);
					par.commonUInt16Param1 = (ushort)commonUInt32Param;
					Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(cUseable, maxExchangeCount, enUIEventID.Activity_PtExchangeCountReady, par, this._elementList[commonUInt32Param].phase.Config.dwPointCnt, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen);
				}
				else
				{
					string arg_12A_0 = (cUseable != null) ? cUseable.m_name : string.Empty;
					stUIEventParams par2 = default(stUIEventParams);
					par2.commonUInt16Param1 = (ushort)commonUInt32Param;
					this._elementList[commonUInt32Param].phase.SetExchangeCountOnce(1);
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_PtExchangeConfirm, par2);
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
				uint dwResItemID = this._elementList[commonUInt32Param].phase.Config.dwResItemID;
				CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)this._elementList[commonUInt32Param].phase.Config.wResItemType, this._elementList[commonUInt32Param].phase.Config.dwResItemID, (int)this._elementList[commonUInt32Param].phase.Config.wResItemCnt);
				if (cUseable == null)
				{
					return;
				}
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
					uiEvent.m_eventParams.taskId = 0u;
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
					Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(cUseable, maxExchangeCount, enUIEventID.Activity_PtExchangeCountReady, par, this._elementList[commonUInt32Param].phase.Config.dwPointCnt, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen);
				}
				else
				{
					string arg = (cUseable != null) ? cUseable.m_name : string.Empty;
					stUIEventParams par2 = default(stUIEventParams);
					par2.commonUInt16Param1 = (ushort)commonUInt32Param;
					this._elementList[commonUInt32Param].phase.SetExchangeCountOnce(1);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), maxExchangeCount, arg), enUIEventID.Activity_PtExchangeConfirm, enUIEventID.None, par2, false);
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
					uint dwResItemID = this._elementList[commonUInt16Param].phase.Config.dwResItemID;
					CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)this._elementList[commonUInt16Param].phase.Config.wResItemType, this._elementList[commonUInt16Param].phase.Config.dwResItemID, 1);
					string arg = (cUseable != null) ? cUseable.m_name : string.Empty;
					stUIEventParams par = default(stUIEventParams);
					par.commonUInt16Param1 = (ushort)commonUInt16Param;
					this._elementList[commonUInt16Param].phase.SetExchangeCountOnce((int)commonUInt32Param);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), commonUInt32Param, arg), enUIEventID.Activity_PtExchangeConfirm, enUIEventID.None, par, false);
				}
			}
		}
	}
}
