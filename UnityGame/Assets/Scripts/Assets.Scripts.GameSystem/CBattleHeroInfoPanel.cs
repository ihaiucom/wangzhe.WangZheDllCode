using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CBattleHeroInfoPanel : Singleton<CBattleHeroInfoPanel>
	{
		public static string s_battleHeroInfoForm = "UGUI/Form/Battle/Form_Battle_HeroInfo.prefab";

		private static string s_propPanel = "Panel_Prop";

		public static readonly string valForm1 = "<color=#60bd67>{0}</color>({1}+<color=#60bd67>{2}</color>)";

		public static readonly string valForm2 = "<color=#60bd67>{0}</color>";

		public static readonly string valForm3 = "<color=#FF3030>{0}</color>({1}<color=#FF3030>{2}</color>)";

		private static uint PropertyMaxAmount = 18u;

		public CUIFormScript m_FormScript;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleHeroInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleHeroInfoTabChange));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleHeroInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleHeroInfoTabChange));
		}

		public bool IsFormOpened()
		{
			return this.m_FormScript != null;
		}

		public void Show()
		{
			this.m_FormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBattleHeroInfoPanel.s_battleHeroInfoForm, false, true);
			if (this.m_FormScript == null)
			{
				return;
			}
			if (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer() == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain;
			if (captain)
			{
				GameObject gameObject = this.m_FormScript.m_formWidgets[3];
				if (gameObject == null)
				{
					return;
				}
				string[] titleList = new string[]
				{
					Singleton<CTextManager>.GetInstance().GetText("BattleHeroInfo_Property"),
					Singleton<CTextManager>.GetInstance().GetText("BattleHeroInfo_Skill")
				};
				CUICommonSystem.InitMenuPanel(gameObject, titleList, 0, true);
				this.ShowHero(captain);
				this.RefreshPropPanel(this.m_FormScript.gameObject, ref captain);
				this.OnSkillTipsShow();
				this.m_FormScript.m_formWidgets[2].CustomSetActive(true);
				this.m_FormScript.m_formWidgets[1].CustomSetActive(false);
			}
			Singleton<CUIParticleSystem>.instance.Hide(null);
		}

		private void ShowHero(PoolObjHandle<ActorRoot> actor)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long)actor.handle.TheActorMeta.ConfigId);
			GameObject gameObject = this.m_FormScript.m_formWidgets[4];
			if (gameObject != null)
			{
				string s_Sprite_Dynamic_BustHero_Dir = CUIUtility.s_Sprite_Dynamic_BustHero_Dir;
				string heroSkinPic = CSkinInfo.GetHeroSkinPic((uint)actor.handle.TheActorMeta.ConfigId, 0u);
				Image component = gameObject.transform.GetComponent<Image>();
				if (component != null)
				{
					component.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + heroSkinPic, Singleton<CUIManager>.GetInstance().GetForm(CBattleHeroInfoPanel.s_battleHeroInfoForm), true, false, false, false);
				}
			}
			GameObject gameObject2 = this.m_FormScript.m_formWidgets[5];
			if (gameObject2 != null)
			{
				Text component2 = gameObject2.transform.GetComponent<Text>();
				if (component2 != null)
				{
					component2.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
				}
			}
			GameObject gameObject3 = this.m_FormScript.m_formWidgets[6];
			if (gameObject3 != null)
			{
				Text component3 = gameObject3.transform.GetComponent<Text>();
				if (component3 != null)
				{
					component3.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szHeroTips));
				}
			}
		}

		public void Hide()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CBattleHeroInfoPanel.s_battleHeroInfoForm);
			this.m_FormScript = null;
			Singleton<CUIParticleSystem>.instance.Show(null);
		}

		protected void OnBttleHeroInfoTabChange(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			int selectedIndex = component.GetSelectedIndex();
			if (this.m_FormScript != null)
			{
				if (selectedIndex == 0)
				{
					this.m_FormScript.m_formWidgets[2].CustomSetActive(true);
					this.m_FormScript.m_formWidgets[1].CustomSetActive(false);
				}
				else
				{
					this.m_FormScript.m_formWidgets[2].CustomSetActive(false);
					this.m_FormScript.m_formWidgets[1].CustomSetActive(true);
				}
			}
		}

		public void SwitchPanel()
		{
		}

		private void RefreshPropPanel(GameObject root, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!actor || root == null)
			{
				return;
			}
			GameObject gameObject = root.transform.Find(CBattleHeroInfoPanel.s_propPanel).gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel_HeroProp").gameObject;
			this.RefreshHeroPropPanel(gameObject2, ref actor);
		}

		private void RefreshHeroPropPanel(GameObject root, ref PoolObjHandle<ActorRoot> actor)
		{
			if (actor.handle == null || actor.handle.ValueComponent == null)
			{
				return;
			}
			ValueDataInfo[] actorValue = actor.handle.ValueComponent.mActorValue.GetActorValue();
			int soulLevel = actor.handle.ValueComponent.mActorValue.SoulLevel;
			uint configId = (uint)actor.handle.TheActorMeta.ConfigId;
			int actorMoveSpeed = actor.handle.ValueComponent.actorMoveSpeed;
			uint energyType = (uint)actor.handle.ValueComponent.mActorValue.EnergyType;
			Transform transform = root.transform;
			Text[] array = new Text[CBattleHeroInfoPanel.PropertyMaxAmount + 1u];
			Text[] array2 = new Text[CBattleHeroInfoPanel.PropertyMaxAmount + 1u];
			int num = 1;
			while ((long)num <= (long)((ulong)CBattleHeroInfoPanel.PropertyMaxAmount))
			{
				array[num] = transform.Find(string.Format("TextL{0}", num)).GetComponent<Text>();
				array2[num] = transform.Find(string.Format("TextR{0}", num)).GetComponent<Text>();
				num++;
			}
			ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
			array[1].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyAtkPt"));
			array2[1].set_text(CBattleHeroInfoPanel.GetFormStr((float)actorValue[1].basePropertyValue, (float)actorValue[1].extraPropertyValue));
			array[2].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcAtkPt"));
			array2[2].set_text(CBattleHeroInfoPanel.GetFormStr((float)actorValue[2].basePropertyValue, (float)actorValue[2].extraPropertyValue));
			array[3].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MaxHp"));
			array2[3].set_text(CBattleHeroInfoPanel.GetFormStr((float)actorValue[5].basePropertyValue, (float)actorValue[5].extraPropertyValue));
			array[4].set_text(Singleton<CTextManager>.GetInstance().GetText(EnergyCommon.GetEnergyShowText(energyType, EnergyShowType.MaxValue)));
			array2[4].set_text(CBattleHeroInfoPanel.GetFormStr((float)actorValue[32].basePropertyValue, (float)actorValue[32].extraPropertyValue));
			int totalValue = actorValue[3].totalValue;
			int percent = totalValue * 10000 / (totalValue + soulLevel * (int)anyData.dwM_PhysicsDefend + (int)anyData.dwN_PhysicsDefend);
			array[5].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyDefPt"));
			array2[5].set_text(string.Format("{0}|{1}", CBattleHeroInfoPanel.GetFormStr((float)actorValue[3].basePropertyValue, (float)actorValue[3].extraPropertyValue), CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[3].extraPropertyValue > 0)));
			totalValue = actorValue[4].totalValue;
			percent = totalValue * 10000 / (totalValue + soulLevel * (int)anyData.dwM_MagicDefend + (int)anyData.dwN_MagicDefend);
			array[6].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcDefPt"));
			array2[6].set_text(string.Format("{0}|{1}", CBattleHeroInfoPanel.GetFormStr((float)actorValue[4].basePropertyValue, (float)actorValue[4].extraPropertyValue), CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[4].extraPropertyValue > 0)));
			totalValue = actorValue[28].totalValue;
			percent = 10000 * totalValue / (totalValue + soulLevel * (int)anyData.dwM_AttackSpeed + (int)anyData.dwN_AttackSpeed) + actorValue[18].totalValue;
			array[7].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_AtkSpdLvl"));
			array2[7].set_text(CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[18].extraPropertyValue > 0));
			percent = actorValue[20].totalValue;
			array[8].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CdReduce"));
			array2[8].set_text(CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[20].extraPropertyValue > 0));
			totalValue = actorValue[24].totalValue;
			percent = 10000 * totalValue / (totalValue + soulLevel * (int)anyData.dwM_Critical + (int)anyData.dwN_Critical) + actorValue[6].totalValue;
			array[9].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CritLvl"));
			array2[9].set_text(CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[6].extraPropertyValue > 0));
			array[10].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MoveSpd"));
			array2[10].set_text(CBattleHeroInfoPanel.GetFormStr((float)(actorValue[15].basePropertyValue / 10), (float)((actorMoveSpeed - actorValue[15].basePropertyValue) / 10)));
			array[11].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover"));
			totalValue = actorValue[16].totalValue;
			string text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover_Desc"), totalValue);
			array2[11].set_text(CBattleHeroInfoPanel.GetFormStr((float)actorValue[16].basePropertyValue, (float)actorValue[16].extraPropertyValue));
			array[12].set_text(Singleton<CTextManager>.GetInstance().GetText(EnergyCommon.GetEnergyShowText(energyType, EnergyShowType.RecoverValue)));
			totalValue = actorValue[33].totalValue;
			string text2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_EpRecover_Desc"), totalValue);
			array2[12].set_text(CBattleHeroInfoPanel.GetFormStr((float)actorValue[33].basePropertyValue, (float)actorValue[33].extraPropertyValue));
			array[13].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyArmorHurt"));
			array2[13].set_text(string.Format("{0}|{1}", CBattleHeroInfoPanel.GetFormStr((float)actorValue[7].baseValue, (float)actorValue[7].extraPropertyValue), CBattleHeroInfoPanel.GetFormPercentStr(actorValue[34].totalValue, actorValue[34].extraPropertyValue > 0)));
			array[14].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcArmorHurt"));
			array2[14].set_text(string.Format("{0}|{1}", CBattleHeroInfoPanel.GetFormStr((float)actorValue[8].baseValue, (float)actorValue[8].extraPropertyValue), CBattleHeroInfoPanel.GetFormPercentStr(actorValue[35].totalValue, actorValue[35].extraPropertyValue > 0)));
			totalValue = actorValue[26].totalValue;
			percent = 10000 * totalValue / (totalValue + soulLevel * (int)anyData.dwM_PhysicsHemophagia + (int)anyData.dwN_PhysicsHemophagia) + actorValue[9].totalValue;
			array[15].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyVampLvl"));
			array2[15].set_text(CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[9].extraPropertyValue > 0));
			totalValue = actorValue[27].totalValue;
			percent = 10000 * totalValue / (totalValue + soulLevel * (int)anyData.dwM_MagicHemophagia + (int)anyData.dwN_MagicHemophagia) + actorValue[10].totalValue;
			array[16].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcVampLvl"));
			array2[16].set_text(CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[10].extraPropertyValue > 0));
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(configId);
			if (dataByKey != null)
			{
				array[17].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_BaseAtkRange"));
				array2[17].set_text(Utility.UTF8Convert(dataByKey.szAttackRangeDesc));
			}
			else
			{
				array[17].set_text(string.Empty);
				array2[17].set_text(string.Empty);
			}
			totalValue = actorValue[29].totalValue;
			percent = 10000 * totalValue / (totalValue + soulLevel * (int)anyData.dwM_Tenacity + (int)anyData.dwN_Tenacity) + actorValue[17].totalValue;
			array[18].set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CtrlReduceLvl"));
			array2[18].set_text(CBattleHeroInfoPanel.GetFormPercentStr(percent, actorValue[17].extraPropertyValue > 0));
		}

		private static string GetFormStr(float baseValue, float growValue)
		{
			if (growValue > 0f)
			{
				return string.Format(CBattleHeroInfoPanel.valForm1, baseValue + growValue, baseValue, growValue);
			}
			if (growValue < 0f)
			{
				float num = baseValue + growValue;
				if (num < 0f)
				{
					num = 0f;
					growValue = -baseValue;
				}
				return string.Format(CBattleHeroInfoPanel.valForm3, num, baseValue, growValue);
			}
			return baseValue.ToString();
		}

		private static string GetFormPercentStr(int percent, bool isExtra)
		{
			if (isExtra)
			{
				return string.Format(CBattleHeroInfoPanel.valForm2, CUICommonSystem.GetValuePercent(percent));
			}
			return CUICommonSystem.GetValuePercent(percent);
		}

		public void OnSkillTipsShow()
		{
			if (this.m_FormScript == null)
			{
				return;
			}
			GameObject gameObject = this.m_FormScript.transform.Find("SkillTipsBg").gameObject;
			if (!gameObject.activeSelf)
			{
				gameObject.CustomSetActive(true);
			}
			SkillSlotType[] array = new SkillSlotType[]
			{
				SkillSlotType.SLOT_SKILL_0,
				SkillSlotType.SLOT_SKILL_1,
				SkillSlotType.SLOT_SKILL_2,
				SkillSlotType.SLOT_SKILL_3
			};
			GameObject[] array2 = new GameObject[array.Length];
			array2[0] = gameObject.transform.Find("Panel0").gameObject;
			array2[1] = gameObject.transform.Find("Panel1").gameObject;
			array2[2] = gameObject.transform.Find("Panel2").gameObject;
			array2[3] = gameObject.transform.Find("Panel3").gameObject;
			if (Singleton<GamePlayerCenter>.instance.GetHostPlayer() == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			if (!captain)
			{
				return;
			}
			IHeroData heroData = CHeroDataFactory.CreateHeroData((uint)captain.handle.TheActorMeta.ConfigId);
			SkillSlot[] skillSlotArray = captain.handle.SkillControl.SkillSlotArray;
			for (int i = 0; i < array.Length; i++)
			{
				SkillSlot skillSlot = skillSlotArray[(int)array[i]];
				array2[i].CustomSetActive(true);
				Skill skill;
				if (skillSlot != null)
				{
					skill = skillSlot.SkillObj;
				}
				else if (i < array.Length - 1 && i > 0)
				{
					skill = new Skill(captain.handle.TheActorMeta.ConfigId * 100 + i * 10);
				}
				else
				{
					skill = null;
				}
				if (skill != null)
				{
					Image component = array2[i].transform.Find("SkillMask/SkillImg").GetComponent<Image>();
					if (component != null && !string.IsNullOrEmpty(skill.IconName))
					{
						component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skill.IconName, Singleton<CUIManager>.GetInstance().GetForm(CBattleHeroInfoPanel.s_battleHeroInfoForm), true, false, false, false);
					}
					Text component2 = array2[i].transform.Find("Text_Tittle").GetComponent<Text>();
					if (component2 != null && skill.cfgData.szSkillName.get_Length() > 0)
					{
						component2.set_text(StringHelper.UTF8BytesToString(ref skill.cfgData.szSkillName));
					}
					Text component3 = array2[i].transform.Find("Text_CD").GetComponent<Text>();
					int millsecond = 0;
					if (skillSlot != null)
					{
						millsecond = skillSlot.GetSkillCDMax();
					}
					component3.set_text((i == 0) ? Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5") : Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[]
					{
						CUICommonSystem.ConvertMillisecondToSecondWithOneDecimal(millsecond)
					}));
					if (i < array.Length && i > 0)
					{
						Text component4 = array2[i].transform.Find("Text_EnergyCost").GetComponent<Text>();
						if (skillSlot == null)
						{
							component4.set_text((skill.cfgData.bEnergyCostType == 6) ? string.Empty : Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText((uint)skill.cfgData.bEnergyCostType, EnergyShowType.CostValue), new string[]
							{
								skill.cfgData.iEnergyCost.ToString()
							}));
						}
						else
						{
							component4.set_text((skill.cfgData.bEnergyCostType == 6) ? string.Empty : Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText((uint)skill.cfgData.bEnergyCostType, EnergyShowType.CostValue), new string[]
							{
								skillSlot.NextSkillEnergyCostTotal().ToString()
							}));
						}
					}
					ushort[] skillEffectType = skill.cfgData.SkillEffectType;
					for (int j = 1; j <= 2; j++)
					{
						GameObject gameObject2 = array2[i].transform.Find(string.Format("EffectNode{0}", j)).gameObject;
						if (j <= skillEffectType.Length && skillEffectType[j - 1] != 0)
						{
							gameObject2.CustomSetActive(true);
							gameObject2.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType)skillEffectType[j - 1]), this.m_FormScript, true, false, false, false);
							gameObject2.transform.Find("Text").GetComponent<Text>().set_text(CSkillData.GetEffectDesc((SkillEffectType)skillEffectType[j - 1]));
						}
						else
						{
							gameObject2.CustomSetActive(false);
						}
					}
					Text component5 = array2[i].transform.Find("Text_Detail").GetComponent<Text>();
					ValueDataInfo[] actorValue = captain.handle.ValueComponent.mActorValue.GetActorValue();
					if (component5 != null && skill.cfgData.szSkillDesc.get_Length() > 0)
					{
						component5.set_text(CUICommonSystem.GetSkillDesc(skill.cfgData.szSkillDesc, actorValue, skillSlot.GetSkillLevel(), captain.handle.ValueComponent.actorSoulLevel, (uint)captain.handle.TheActorMeta.ConfigId));
					}
				}
				else if (i == array.Length - 1)
				{
					Text component6 = array2[i].transform.Find("Text_Detail").GetComponent<Text>();
					if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
					{
						component6.set_text(Singleton<CTextManager>.GetInstance().GetText("Skill_Text_Lock_PVP"));
					}
					else
					{
						component6.set_text(Singleton<CTextManager>.GetInstance().GetText("Skill_Text_Lock_PVE"));
					}
				}
			}
		}
	}
}
