using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class NewbieWeakGuideImpl
{
	public const string FormWeakGuidePath = "UGUI/Form/System/Dialog/Form_WeakGuide";

	private static readonly string WEAK_EFFECT_PATH = "UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab";

	private static readonly string WEAK_BUBBLE_PATH = "UGUI/Form/System/Dialog/WeakGuideBubble.prefab";

	private CUIFormScript m_formWeakGuide;

	private GameObject m_guideTextStatic;

	private NewbieGuideWeakConf m_conf;

	private GameObject m_parentObj;

	private GameObject m_guideTextObj;

	private CUIFormScript m_originalForm;

	public void Init()
	{
	}

	public void UnInit()
	{
	}

	public void Update()
	{
		if (this.m_conf == null || this.m_parentObj == null || this.m_guideTextObj == null || this.m_originalForm == null)
		{
			if (this.m_guideTextStatic != null || this.m_guideTextObj != null)
			{
				this.ClearEffectText();
			}
			return;
		}
		NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)this.m_conf.wSpecialTip);
		if (specialTipConfig != null && specialTipConfig.bGuideTextPos > 0)
		{
			this.m_guideTextObj.CustomSetActive(this.m_parentObj.activeInHierarchy && !this.m_originalForm.IsHided());
			NewbieGuideBaseScript.UpdateGuideTextPos(specialTipConfig, this.m_parentObj, this.m_formWeakGuide, this.m_originalForm, this.m_guideTextObj);
		}
	}

	private void AddEffectText(NewbieGuideWeakConf conf, GameObject inParentObj, CUIFormScript inOriginalForm)
	{
		this.ClearEffectText();
		if (conf != null && conf.wSpecialTip > 0)
		{
			NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)conf.wSpecialTip);
			if (specialTipConfig != null)
			{
				if (specialTipConfig.bGuideTextPos == 0)
				{
					if (this.m_guideTextStatic != null)
					{
						this.m_guideTextStatic.CustomSetActive(true);
						Transform transform = this.m_guideTextStatic.transform.FindChild("RightSpecial/Text");
						if (transform != null)
						{
							Text component = transform.GetComponent<Text>();
							if (component != null)
							{
								component.set_text(StringHelper.UTF8BytesToString(ref specialTipConfig.szTipText));
							}
						}
					}
				}
				else
				{
					this.m_guideTextObj = NewbieGuideBaseScript.InstantiateGuideText(specialTipConfig, inParentObj, this.m_formWeakGuide, inOriginalForm);
				}
			}
		}
		this.m_conf = conf;
		this.m_parentObj = inParentObj;
		this.m_originalForm = inOriginalForm;
		if (this.m_formWeakGuide != null)
		{
			this.m_formWeakGuide.SetPriority(this.m_originalForm.m_priority + 1);
		}
	}

	public void ClearEffectText()
	{
		this.m_guideTextStatic.CustomSetActive(false);
		if (this.m_guideTextObj != null)
		{
			this.m_guideTextObj.CustomSetActive(false);
			this.m_guideTextObj = null;
		}
	}

	public void RemoveEffectText(NewbieGuideWeakConf conf)
	{
		if (conf == null)
		{
			return;
		}
		this.ClearEffectText();
	}

	public void OpenGuideForm()
	{
		if (this.m_formWeakGuide == null)
		{
			this.m_formWeakGuide = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Dialog/Form_WeakGuide", true, true);
			if (this.m_formWeakGuide != null)
			{
				Transform transform = this.m_formWeakGuide.transform.FindChild("GuideTextStatic");
				if (transform != null)
				{
					this.m_guideTextStatic = transform.gameObject;
					if (this.m_guideTextStatic != null)
					{
						this.m_guideTextStatic.CustomSetActive(false);
					}
				}
			}
		}
	}

	public void CloseGuideForm()
	{
		if (this.m_formWeakGuide != null)
		{
			this.m_guideTextStatic.CustomSetActive(false);
			this.m_guideTextStatic = null;
			Singleton<CUIManager>.GetInstance().CloseForm(this.m_formWeakGuide);
			this.m_formWeakGuide = null;
		}
		this.m_guideTextStatic = null;
		this.m_parentObj = null;
		this.m_guideTextObj = null;
		this.m_originalForm = null;
	}

	public bool AddEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, out GameObject highligter)
	{
		highligter = null;
		switch (conf.dwType)
		{
		case 1u:
		{
			GameObject x;
			highligter = (x = this.ShowPvPEffect(conf, inControl));
			return !(x == null);
		}
		case 2u:
		{
			GameObject x2;
			highligter = (x2 = this.ShowPvEEffect(conf, inControl));
			return !(x2 == null);
		}
		case 3u:
		{
			GameObject x3;
			highligter = (x3 = this.ShowFullHeroPanelEffect(conf, inControl));
			return !(x3 == null);
		}
		case 4u:
		{
			GameObject x4;
			highligter = (x4 = this.ShowHeroSelConfirmEffect(conf, inControl));
			return !(x4 == null);
		}
		case 5u:
		{
			GameObject x5;
			highligter = (x5 = this.ShowHumanMatch33Effect(conf, inControl));
			return !(x5 == null);
		}
		case 6u:
		{
			GameObject x6;
			highligter = (x6 = this.ShowBattleHeroSelEffect(conf, inControl));
			return !(x6 == null);
		}
		case 7u:
		{
			GameObject x7;
			highligter = (x7 = this.ShowClickPVPBtnEffect(conf, inControl));
			return !(x7 == null);
		}
		case 8u:
		{
			GameObject x8;
			highligter = (x8 = this.ShowClickMatch55(conf, inControl));
			return !(x8 == null);
		}
		case 9u:
		{
			GameObject x9;
			highligter = (x9 = this.ShowClickStartMatch55(conf, inControl));
			return !(x9 == null);
		}
		case 10u:
		{
			GameObject x10;
			highligter = (x10 = this.ShowClickWinShare(conf, inControl));
			return !(x10 == null);
		}
		case 11u:
		{
			if (conf.Param[0] == 0u || conf.Param[1] == 0u)
			{
				DebugHelper.Assert(false, "newbieguide Invalide config -- {0}", new object[]
				{
					conf.dwType
				});
				return false;
			}
			int num = Random.Range(0, 2);
			uint nextStep = conf.Param[num];
			inControl.Complete(conf.dwID, nextStep);
			return true;
		}
		case 12u:
		{
			GameObject x11;
			highligter = (x11 = this.ShowClickRankBtn(conf, inControl));
			return !(x11 == null);
		}
		case 13u:
		{
			GameObject x12;
			highligter = (x12 = this.ShowClickTrainBtn(conf, inControl));
			return !(x12 == null);
		}
		case 14u:
		{
			GameObject x13;
			highligter = (x13 = this.ShowClickTrainWheelDisc(conf, inControl));
			return !(x13 == null);
		}
		case 15u:
		{
			GameObject x14;
			highligter = (x14 = this.ShowClickMatch55Melee(conf, inControl));
			return !(x14 == null);
		}
		case 16u:
		{
			GameObject x15;
			highligter = (x15 = this.ShowClickMatchingConfirmBoxConfirm(conf, inControl));
			return !(x15 == null);
		}
		case 17u:
		{
			GameObject x16;
			highligter = (x16 = this.ShowClickVictoryTipsBtn(conf, inControl));
			return !(x16 == null);
		}
		case 18u:
		{
			GameObject x17;
			highligter = (x17 = this.ShowClickSaveReplayKit(conf, inControl));
			return !(x17 == null);
		}
		case 21u:
		{
			GameObject x18;
			highligter = (x18 = this.ShowClickSymbolDraw(conf, inControl));
			return !(x18 == null);
		}
		case 22u:
		{
			GameObject x19;
			highligter = (x19 = this.ShowClickCommomEquip(conf, inControl));
			return !(x19 == null);
		}
		case 23u:
			this.ShowBubbleTipText(conf, inControl);
			return true;
		case 24u:
		{
			GameObject x20;
			highligter = (x20 = this.ShowBackHomeTip(conf, inControl));
			return !(x20 == null);
		}
		}
		DebugHelper.Assert(false, "Invalide NewbieGuideWeakGuideType -- {0}", new object[]
		{
			conf.dwType
		});
		return false;
	}

	private GameObject ShowPvPEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("BtnCon/PvpBtn");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowPvEEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("BtnCon/PveBtn");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowFullHeroPanelEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("PanelLeft/ListHostHeroInfo/btnOpenFullHeroPanel");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowHeroSelConfirmEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("PanelRight/btnConfirm");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowHumanMatch33Effect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("panelGroup3/btnGroup/Button3");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowBattleHeroSelEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
		if (form == null)
		{
			return null;
		}
		uint num = conf.Param[0];
		string name = string.Format("PanelLeft/ListHostHeroInfo/ScrollRect/Content/ListElement_{0}/heroItemCell", num);
		Transform transform = form.gameObject.transform.Find(name);
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickPVPBtnEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.FindChild("panelGroup1/btnGroup/Button1");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickMatch55(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.FindChild("panelGroup2/btnGroup/Button4");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickStartMatch55(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.FindChild("Panel_Main/Btn_Matching");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickWinShare(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.FindChild("Panel/ButtonGrid/ButtonShare");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickRankBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.FindChild("BtnCon/LadderBtn");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickTrainBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("panelGroupBottom/ButtonTrain");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickTrainWheelDisc(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("panelGroup4/btnGroup/Button2");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickMatch55Melee(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("panelGroup2/btnGroup/Button3");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickMatchingConfirmBoxConfirm(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickVictoryTipsBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.GetWidget(24).transform;
		GameObject gameObject = transform.FindChild("Btn").gameObject;
		PlayerKDA playerKDA = null;
		if (Singleton<BattleLogic>.GetInstance().battleStat != null && Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null)
		{
			playerKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
		}
		string text = string.Empty;
		if (playerKDA != null)
		{
			ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
			uint key = 0u;
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					key = (uint)current.HeroId;
					break;
				}
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(key);
			if (dataByKey != null)
			{
				text = dataByKey.szName;
			}
			else
			{
				text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
			}
		}
		else
		{
			text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
		}
		transform.FindChild("Panel_Guide").gameObject.CustomSetActive(true);
		transform.FindChild("Panel_Guide/Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", new string[]
		{
			text
		}));
		return this.AddEffectInternal(gameObject, conf, inControl, form);
	}

	private GameObject ShowClickSaveReplayKit(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.transform.FindChild("Panel/ButtonGrid/BtnSaveReplay");
		if (transform == null)
		{
			return null;
		}
		return this.AddEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickSymbolDraw(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.transform.FindChild("pnlBodyBg/Panel_SymbolMake/Panel_SymbolDraw/btnJump");
		if (transform == null)
		{
			return null;
		}
		return this.AddBubbleEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private GameObject ShowClickCommomEquip(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CEquipSystem.s_CustomRecommendEquipPath);
		if (form == null)
		{
			return null;
		}
		Transform transform = form.transform.FindChild("Panel_Main/Panel_EquipCustom/Panel_EquipCustomContent/godEquipButton");
		if (transform == null)
		{
			return null;
		}
		return this.AddBubbleEffectInternal(transform.gameObject, conf, inControl, form);
	}

	private void ShowBubbleTipText(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		this.AddBubbleText(conf, inControl);
	}

	private GameObject ShowBackHomeTip(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		CSkillButtonManager cSkillButtonManager = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
		if (cSkillButtonManager != null)
		{
			SkillButton button = cSkillButtonManager.GetButton(SkillSlotType.SLOT_SKILL_6);
			return this.AddBubbleEffectInternal(button.m_button, conf, inControl, Singleton<CBattleSystem>.GetInstance().FightFormScript);
		}
		return null;
	}

	private GameObject AddEffectInternal(GameObject effectParent, NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, CUIFormScript inOriginalForm)
	{
		GameObject original = Singleton<CResourceManager>.GetInstance().GetResource(NewbieWeakGuideImpl.WEAK_EFFECT_PATH, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		gameObject.transform.SetParent(effectParent.transform);
		Transform transform = effectParent.transform.FindChild("Panel");
		if (transform != null && transform.gameObject.activeInHierarchy)
		{
			gameObject.transform.SetParent(transform);
		}
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.position = effectParent.transform.position;
		(gameObject.transform as RectTransform).anchoredPosition = new Vector2((float)conf.iOffsetHighLightX, (float)conf.iOffsetHighLightY);
		if (conf.bNotShowArrow != 0)
		{
			gameObject.transform.FindChild("Panel/ImageFinger").gameObject.CustomSetActive(false);
		}
		this.AddEffectText(conf, effectParent, inOriginalForm);
		return gameObject;
	}

	private GameObject AddBubbleEffectInternal(GameObject effectParent, NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, CUIFormScript inOriginalForm)
	{
		if (effectParent == null)
		{
			return null;
		}
		if (effectParent.transform.FindChild("WeakGuideBubble(Clone)") != null)
		{
			return null;
		}
		GameObject original = Singleton<CResourceManager>.GetInstance().GetResource(NewbieWeakGuideImpl.WEAK_BUBBLE_PATH, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		gameObject.transform.SetParent(effectParent.transform);
		RectTransform rectTransform = gameObject.transform.FindChild("Image") as RectTransform;
		RectTransform rectTransform2 = gameObject.transform as RectTransform;
		NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)conf.wSpecialTip);
		if (specialTipConfig != null)
		{
			CUICommonSystem.SetTextContent(rectTransform2.FindChild("Text"), specialTipConfig.szTipText);
		}
		Vector2 anchorMin = default(Vector2);
		Vector2 anchorMax = default(Vector2);
		Vector2 pivot = default(Vector2);
		Vector3 one = Vector3.one;
		Vector2 anchoredPosition = default(Vector2);
		switch (specialTipConfig.bSpecialTipPos)
		{
		case 0:
			anchorMin.x = 0f;
			anchorMin.y = 1f;
			anchorMax.x = 0f;
			anchorMax.y = 1f;
			one.y = -1f;
			pivot.x = 0f;
			pivot.y = 0f;
			anchoredPosition.x = 10f;
			anchoredPosition.y = 15f;
			break;
		case 1:
			anchorMin.x = 0f;
			anchorMin.y = 0f;
			anchorMax.x = 0f;
			anchorMax.y = 0f;
			pivot.x = 0f;
			pivot.y = 0f;
			anchoredPosition.x = 10f;
			anchoredPosition.y = -15f;
			break;
		case 2:
			anchorMin.x = 1f;
			anchorMin.y = 1f;
			anchorMax.x = 1f;
			anchorMax.y = 1f;
			one.y = -1f;
			pivot.x = 1f;
			pivot.y = 0f;
			anchoredPosition.x = -10f;
			anchoredPosition.y = 15f;
			break;
		case 3:
			anchorMin.x = 1f;
			anchorMin.y = 0f;
			anchorMax.x = 1f;
			anchorMax.y = 0f;
			pivot.x = 1f;
			pivot.y = 0f;
			anchoredPosition.x = -10f;
			anchoredPosition.y = -15f;
			break;
		}
		rectTransform2.position = effectParent.transform.position;
		rectTransform2.anchoredPosition = Vector2.zero;
		Vector2 anchoredPosition2 = rectTransform2.anchoredPosition;
		anchoredPosition2.x += (float)specialTipConfig.iOffsetX;
		anchoredPosition2.y += (float)specialTipConfig.iOffsetY;
		rectTransform2.anchoredPosition = anchoredPosition2;
		rectTransform2.localScale = Vector3.one;
		rectTransform.localScale = one;
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
		rectTransform.pivot = pivot;
		rectTransform.anchoredPosition = anchoredPosition;
		CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(gameObject, "Timer");
		if (componetInChild != null)
		{
			if (conf.Param[0] != 0u)
			{
				componetInChild.SetTotalTime(conf.Param[0]);
			}
			componetInChild.StartTimer();
		}
		return gameObject;
	}

	private void AddBubbleText(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
	{
		GameObject gameObject = this.m_formWeakGuide.transform.FindChild("GuideText").gameObject;
		gameObject.CustomSetActive(true);
		NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)conf.wSpecialTip);
		CUICommonSystem.SetTextContent(gameObject.transform.FindChild("RightSpecial/Text"), specialTipConfig.szTipText);
		RectTransform rectTransform = gameObject.transform as RectTransform;
		rectTransform.anchoredPosition = new Vector2
		{
			x = (float)specialTipConfig.iOffsetX,
			y = (float)specialTipConfig.iOffsetY
		};
		CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(gameObject, "Timer");
		if (conf.Param[0] != 0u)
		{
			componetInChild.SetTotalTime(conf.Param[0]);
		}
		componetInChild.StartTimer();
	}
}
