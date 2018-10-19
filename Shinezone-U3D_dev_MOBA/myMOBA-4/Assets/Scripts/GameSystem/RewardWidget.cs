using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class RewardWidget : ActivityWidget
	{
		public class RewardListItem
		{
			public const int REWARD_ITEM_COUNT = 4;

			public GameObject root;

			public GameObject[] cellList;

			public GameObject getBtn;

			public Text getBtnTxt;

			public Text tips;

			public Image flag;

			public RewardWidget ownerWgt;

			public ActivityPhase actvPhase;

			public RewardListItem(GameObject node, RewardWidget ownWgt, ActivityPhase ap)
			{
				this.root = node;
				this.ownerWgt = ownWgt;
				this.actvPhase = ap;
				this.cellList = new GameObject[4];
				for (int i = 0; i < 4; i++)
				{
					this.cellList[i] = Utility.FindChild(node, "Items/Item" + i);
				}
				this.getBtn = Utility.FindChild(node, "GetAward");
				this.getBtnTxt = Utility.GetComponetInChild<Text>(this.getBtn, "Text");
				this.tips = Utility.GetComponetInChild<Text>(node, "Tips");
				this.flag = Utility.GetComponetInChild<Image>(node, "Flag");
				this.actvPhase.OnMaskStateChange += new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
				this.actvPhase.OnTimeStateChange += new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
			}

			public void Clear()
			{
				this.actvPhase.OnMaskStateChange -= new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
				this.actvPhase.OnTimeStateChange -= new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
			}

			private void OnStateChange(ActivityPhase ap)
			{
				this.Validate();
			}

			public void Validate()
			{
				if (this.actvPhase.Target > 0)
				{
					this.tips.gameObject.SetActive(true);
					this.tips.text = this.actvPhase.Tips;
				}
				else
				{
					this.tips.gameObject.SetActive(false);
				}
				if (this.actvPhase.Marked)
				{
					this.flag.gameObject.SetActive(true);
					this.getBtn.SetActive(false);
				}
				else
				{
					this.flag.gameObject.SetActive(false);
					this.getBtn.SetActive(true);
					if (this.actvPhase.ReadyForGet)
					{
						this.getBtn.GetComponent<CUIEventScript>().enabled = true;
						this.getBtn.GetComponent<Button>().interactable = true;
						this.getBtn.GetComponent<Image>().SetSprite((this.ownerWgt.view.form as CampaignForm).GetDynamicImage(CampaignForm.DynamicAssets.ButtonYellowImage));
						this.getBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText("get");
						this.getBtnTxt.color = Color.white;
					}
					else
					{
						this.getBtn.GetComponent<Image>().SetSprite((this.ownerWgt.view.form as CampaignForm).GetDynamicImage(CampaignForm.DynamicAssets.ButtonBlueImage));
						if (this.actvPhase.AchieveStateValid)
						{
							this.getBtn.GetComponent<CUIEventScript>().enabled = true;
							this.getBtn.GetComponent<Button>().interactable = true;
							this.getBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText("gotoFinish");
							this.getBtnTxt.color = Color.white;
						}
						else
						{
							this.getBtn.GetComponent<CUIEventScript>().enabled = false;
							this.getBtn.GetComponent<Button>().interactable = false;
							this.getBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText((this.actvPhase.timeState != ActivityPhase.TimeState.Closed) ? "notInTime" : "outOfTime");
							this.getBtnTxt.color = Color.gray;
						}
					}
				}
				int num = 0;
				for (int i = 0; i < this.cellList.Length; i++)
				{
					GameObject gameObject = this.cellList[i];
					CUseable cUseable = this.actvPhase.GetUseable(i);
					if (cUseable != null)
					{
						gameObject.CustomSetActive(true);
						cUseable.m_stackMulti = (int)this.actvPhase.MultipleTimes;
						if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
						{
							CItem cItem = cUseable as CItem;
							if (cItem != null && cItem.m_itemData.bIsView > 0)
							{
								CUICommonSystem.SetItemCell(this.ownerWgt.view.form.formScript, gameObject, cUseable, true, false, false, true);
							}
							else
							{
								CUICommonSystem.SetItemCell(this.ownerWgt.view.form.formScript, gameObject, cUseable, true, false, false, false);
								if (gameObject != null)
								{
									CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
									if (component != null)
									{
										component.SetUIEvent(enUIEventType.Click, enUIEventID.None);
									}
								}
							}
						}
						else
						{
							CUICommonSystem.SetItemCell(this.ownerWgt.view.form.formScript, gameObject, cUseable, true, false, false, false);
							if (gameObject != null)
							{
								CUIEventScript component2 = gameObject.GetComponent<CUIEventScript>();
								if (component2 != null)
								{
									component2.SetUIEvent(enUIEventType.Click, enUIEventID.None);
								}
							}
						}
						num++;
					}
					else
					{
						cUseable = this.actvPhase.GetExtraUseable(i - num);
						if (cUseable != null)
						{
							gameObject.CustomSetActive(true);
							CUICommonSystem.SetItemCell(this.ownerWgt.view.form.formScript, gameObject, cUseable, true, false, false, false);
						}
						else
						{
							gameObject.CustomSetActive(false);
						}
					}
					if (cUseable != null)
					{
						GameObject gameObject2 = Utility.FindChild(gameObject, "flag");
						if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
						{
							gameObject2.CustomSetActive(true);
							Utility.GetComponetInChild<Text>(gameObject2, "Text").text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
						}
						else if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
						{
							gameObject2.CustomSetActive(true);
							Utility.GetComponetInChild<Text>(gameObject2, "Text").text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
						}
						else
						{
							gameObject2.CustomSetActive(false);
						}
					}
				}
			}
		}

		private const float SPACING_Y = 5f;

		private ListView<RewardWidget.RewardListItem> _rewardList;

		private GameObject _elementTmpl;

		public RewardWidget(GameObject node, ActivityView view) : base(node, view)
		{
			this._elementTmpl = Utility.FindChild(node, "Template");
			float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
			ListView<ActivityPhase> listView = new ListView<ActivityPhase>(view.activity.PhaseList.Count);
			listView.AddRange(view.activity.PhaseList);
			listView.Sort(new Comparison<ActivityPhase>(RewardWidget.APSort));
			this._rewardList = new ListView<RewardWidget.RewardListItem>();
			for (int i = 0; i < listView.Count; i++)
			{
				GameObject gameObject;
				if (i > 0)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(this._elementTmpl);
					gameObject.transform.SetParent(this._elementTmpl.transform.parent);
					gameObject.transform.localPosition = this._rewardList[i - 1].root.transform.localPosition + new Vector3(0f, -(height + 5f), 0f);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.identity;
				}
				else
				{
					this._elementTmpl.SetActive(true);
					gameObject = this._elementTmpl;
				}
				RewardWidget.RewardListItem item = new RewardWidget.RewardListItem(gameObject, this, listView[i]);
				this._rewardList.Add(item);
			}
			if (this._rewardList.Count == 0)
			{
				this._elementTmpl.SetActive(false);
			}
			node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, (this._rewardList.Count <= 0) ? 0f : (height * (float)this._rewardList.Count + (float)(this._rewardList.Count - 1) * 5f));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ClickGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
			this.Validate();
		}

		private static int APSort(ActivityPhase x, ActivityPhase y)
		{
			bool readyForGet = x.ReadyForGet;
			bool readyForGet2 = y.ReadyForGet;
			if (readyForGet == readyForGet2)
			{
				return (int)((x.Marked != y.Marked) ? ((!x.Marked) ? 4294967295u : 1u) : (x.ID - y.ID));
			}
			return (!readyForGet) ? 1 : -1;
		}

		public override void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ClickGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
			for (int i = 0; i < this._rewardList.Count; i++)
			{
				this._rewardList[i].Clear();
				if (i > 0)
				{
					CUICommonSystem.DestoryObj(this._rewardList[i].root, 0.1f);
				}
			}
			this._rewardList = null;
			this._elementTmpl = null;
		}

		private void OnClickGet(CUIEvent uiEvent)
		{
			for (int i = 0; i < this._rewardList.Count; i++)
			{
				RewardWidget.RewardListItem rewardListItem = this._rewardList[i];
				if (rewardListItem.getBtn == uiEvent.m_srcWidget)
				{
					if (rewardListItem.actvPhase.ReadyForGet)
					{
						rewardListItem.actvPhase.DrawReward();
					}
					else
					{
						base.view.form.Close();
						rewardListItem.actvPhase.AchieveJump();
					}
					break;
				}
			}
		}

		public override void Validate()
		{
			for (int i = 0; i < this._rewardList.Count; i++)
			{
				RewardWidget.RewardListItem rewardListItem = this._rewardList[i];
				rewardListItem.Validate();
			}
		}
	}
}
