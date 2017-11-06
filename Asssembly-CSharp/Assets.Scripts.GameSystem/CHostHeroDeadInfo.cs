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
	public class CHostHeroDeadInfo
	{
		private const string m_strHeroDeadInfoForm = "UGUI/Form/Battle/Part/Form_Battle_Part_HeroDeadInfo.prefab";

		public static string m_strDragonBig = CUIUtility.s_Sprite_Dynamic_Signal_Dir + "Dragon_big";

		public static string m_dragon_icon_3v3 = "UGUI/Sprite/Battle/kn_dragon_3v3";

		private static string[] m_Skill_HurtType_Bg_ImgName = new string[]
		{
			"Common_Bg_Physicalbg",
			"Common_Bg_Spellbg",
			"Common_Bg_Realbg",
			"Common_Bg_blend"
		};

		private static string[] m_Skill_HurtValue_Bg_ImgName = new string[]
		{
			"ImgDamge_physicalbg",
			"ImgDamge_spellbg",
			"ImgDamge_realbg",
			"ImgDamge_blend"
		};

		private string[] m_Skill_HurtType_Name = new string[]
		{
			"physical",
			"magic",
			"real",
			"blend"
		};

		private bool m_bIsMobaMode;

		private PoolObjHandle<ActorRoot> m_hostActor;

		private CUIFormScript m_heroDeadInfoForm;

		private string m_strHeroPassiveSkillName;

		private string m_strHeroEquipSkillName;

		private string m_strAtkSkill0Name;

		public void Init()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaMode())
			{
				this.m_bIsMobaMode = true;
				this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
				for (int i = 0; i < 4; i++)
				{
					this.m_Skill_HurtType_Name[i] = Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_" + (i + 1));
				}
				this.m_strHeroPassiveSkillName = Singleton<CTextManager>.instance.GetText("HeroDeadInfo_PassiveSkill_Name");
				this.m_strHeroEquipSkillName = Singleton<CTextManager>.instance.GetText("HeroDeadInfo_EquipSkill_Name");
				this.m_strAtkSkill0Name = Singleton<CTextManager>.instance.GetText("HeroDeadInfo_Skill0_Name");
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_DeadInfo_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormOpen));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_DeadInfoForm_Close_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormClose));
			}
		}

		public void UnInit()
		{
			if (this.m_bIsMobaMode)
			{
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_DeadInfo_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormOpen));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_DeadInfoForm_Close_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormClose));
				if (this.m_hostActor)
				{
					this.m_hostActor.Release();
				}
				this.OnDeadInfoFormClose(null);
			}
		}

		public void OnActorRevive(ref DefaultGameEventParam prm)
		{
			uint hostPlayerId = Singleton<GamePlayerCenter>.instance.HostPlayerId;
			if (prm.src && prm.src.handle.TheActorMeta.PlayerId == hostPlayerId)
			{
				PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(hostPlayerId);
				if (playerKDA == null)
				{
					return;
				}
				ListView<CHostHeroDamage>.Enumerator enumerator = playerKDA.m_hostHeroDamage.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						enumerator.Current.OnActorRevive(ref prm);
					}
				}
				if (prm.src == this.m_hostActor)
				{
					this.OnDeadInfoFormClose(null);
				}
			}
		}

		public void OnDeadInfoFormClose(CUIEvent uiEvent)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (Singleton<CBattleSystem>.instance.FightForm == null)
			{
				return;
			}
			if (this.m_heroDeadInfoForm)
			{
				Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Battle/Part/Form_Battle_Part_HeroDeadInfo.prefab");
				this.m_heroDeadInfoForm = null;
			}
		}

		public void OnDeadInfoFormOpen(CUIEvent uiEvent)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (Singleton<CBattleSystem>.instance.FightForm == null)
			{
				return;
			}
			this.m_heroDeadInfoForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Battle/Part/Form_Battle_Part_HeroDeadInfo.prefab", false, true);
			if (this.m_heroDeadInfoForm)
			{
				this.InitHeroDeadInfoForm();
			}
		}

		private void InitHeroPanelHeroInfo(uint uiObjId, Transform trHeroPanel, CHostHeroDamage objHostHeroDamage)
		{
			if (trHeroPanel == null || objHostHeroDamage == null)
			{
				return;
			}
			Transform transform = trHeroPanel.FindChild("PanelTop");
			if (transform)
			{
				int num = 0;
				string text = null;
				string text2 = null;
				byte b = 0;
				byte b2 = 0;
				ActorTypeDef actorTypeDef = ActorTypeDef.Invalid;
				if (objHostHeroDamage.GetDamageActorInfo(uiObjId, ref text, ref text2, ref actorTypeDef, ref num, ref b, ref b2))
				{
					Transform transform2 = transform.FindChild("Imghead");
					if (transform2)
					{
						Transform transform3 = transform2.FindChild("head");
						if (transform3)
						{
							Image component = transform3.GetComponent<Image>();
							if (component)
							{
								string prefabPath;
								if (actorTypeDef == ActorTypeDef.Actor_Type_Hero)
								{
									string heroSkinPic = CSkinInfo.GetHeroSkinPic((uint)num, 0u);
									prefabPath = CUIUtility.s_Sprite_Dynamic_Icon_Dir + heroSkinPic;
								}
								else if (actorTypeDef == ActorTypeDef.Actor_Type_Organ)
								{
									prefabPath = KillNotifyUT.Convert_DetailInfo_KillInfo(new KillDetailInfo
									{
										Killer = Singleton<GameObjMgr>.instance.GetActor(uiObjId)
									}).KillerImgSrc;
								}
								else if (actorTypeDef == ActorTypeDef.Actor_Type_Monster)
								{
									ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(num);
									if (dataCfgInfoByCurLevelDiff != null && !string.IsNullOrEmpty(dataCfgInfoByCurLevelDiff.szBossIcon))
									{
										prefabPath = dataCfgInfoByCurLevelDiff.szBossIcon;
									}
									else if (b2 == 1)
									{
										prefabPath = KillNotify.monster_icon;
									}
									else if (b == 7)
									{
										prefabPath = CHostHeroDeadInfo.m_dragon_icon_3v3;
									}
									else if (b == 9)
									{
										prefabPath = KillNotify.dragon_icon;
									}
									else if (b == 8)
									{
										prefabPath = CHostHeroDeadInfo.m_strDragonBig;
									}
									else
									{
										prefabPath = KillNotify.yeguai_icon;
									}
								}
								else
								{
									prefabPath = KillNotify.monster_icon;
								}
								component.SetSprite(prefabPath, this.m_heroDeadInfoForm, true, false, false, false);
							}
						}
					}
					Transform transform4 = transform.FindChild("heroName");
					if (transform4)
					{
						Text component2 = transform4.GetComponent<Text>();
						if (component2)
						{
							string text3 = text;
							if (!string.IsNullOrEmpty(text3))
							{
								int num2 = text3.IndexOf('(');
								string text4 = text3.Substring(num2 + 1, text3.get_Length() - num2 - 2);
								component2.set_text(text4);
							}
						}
					}
					Transform transform5 = transform.FindChild("playerName");
					if (transform5)
					{
						Text component3 = transform5.GetComponent<Text>();
						if (component3)
						{
							if (!string.IsNullOrEmpty(text2))
							{
								component3.set_text(text2);
								transform5.gameObject.CustomSetActive(true);
							}
							else
							{
								transform5.gameObject.CustomSetActive(false);
							}
						}
					}
				}
			}
		}

		private void InitHeroPanelSkillInfo(uint uiObjId, Transform trHeroPanel, uint uiTotalDamge, CHostHeroDamage objHostHeroDamage)
		{
			int num = 0;
			HURT_INFO[] array = new HURT_INFO[12];
			objHostHeroDamage.GetActorDamage(uiObjId, ref array, ref num);
			for (int i = 0; i < 3; i++)
			{
				string name = "PanelSkill/Skill" + i;
				Transform transform = trHeroPanel.FindChild(name);
				if (transform)
				{
					if (array[i].iValue <= 0)
					{
						transform.gameObject.CustomSetActive(false);
					}
					else
					{
						transform.gameObject.CustomSetActive(true);
						PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(uiObjId);
						Transform transform2 = transform.FindChild("ImgSkill");
						if (transform2)
						{
							Image component = transform2.GetComponent<Image>();
							if (component)
							{
								if (array[i].strIconName != null)
								{
									if (array[i].SkillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL || array[i].SkillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_AREATRIGGER)
									{
										component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + array[i].strIconName, this.m_heroDeadInfoForm, true, false, false, false);
									}
									else if (array[i].SkillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_EQUIP)
									{
										component.SetSprite(CUIUtility.s_Sprite_System_BattleEquip_Dir + array[i].strIconName, this.m_heroDeadInfoForm, true, false, false, false);
									}
								}
								else if (array[i].SkillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_EQUIP)
								{
									component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + "1106", this.m_heroDeadInfoForm, true, false, false, false);
								}
								else
								{
									component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + "1001", this.m_heroDeadInfoForm, true, false, false, false);
								}
							}
						}
						HurtTypeDef hurtType = array[i].hurtType;
						if (hurtType >= HurtTypeDef.PhysHurt && hurtType < HurtTypeDef.Max)
						{
							Transform transform3 = transform.FindChild("TxtSkillTypeBg");
							if (transform3)
							{
								Image component2 = transform3.GetComponent<Image>();
								if (component2)
								{
									component2.SetSprite("UGUI/Sprite/Common/" + CHostHeroDeadInfo.m_Skill_HurtType_Bg_ImgName[(int)hurtType], this.m_heroDeadInfoForm, true, false, false, false);
								}
							}
							Transform transform4 = transform.FindChild("TxtSkillType");
							if (transform4)
							{
								Text component3 = transform4.GetComponent<Text>();
								if (component3)
								{
									component3.set_text(this.m_Skill_HurtType_Name[(int)hurtType]);
									component3.set_color(CUIUtility.s_Text_Skill_HurtType_Color[(int)hurtType]);
								}
							}
							Transform transform5 = transform.FindChild("TxtSkillName");
							if (transform5)
							{
								Text component4 = transform5.GetComponent<Text>();
								if (component4)
								{
									if (array[i].strName != null)
									{
										component4.set_text(array[i].strName);
									}
									else
									{
										ActorTypeDef actorTypeDef = ActorTypeDef.Invalid;
										if (!actor)
										{
											int num2 = 0;
											string text = null;
											string text2 = null;
											ActorTypeDef actorTypeDef2 = ActorTypeDef.Invalid;
											byte b = 0;
											byte b2 = 0;
											if (objHostHeroDamage.GetDamageActorInfo(uiObjId, ref text, ref text2, ref actorTypeDef, ref num2, ref b, ref b2))
											{
												actorTypeDef = actorTypeDef2;
											}
										}
										else
										{
											actorTypeDef = actor.handle.TheActorMeta.ActorType;
										}
										if (actorTypeDef == ActorTypeDef.Actor_Type_Hero)
										{
											if (array[i].SkillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL)
											{
												component4.set_text(this.m_strAtkSkill0Name);
											}
											else if (array[i].SkillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_EQUIP)
											{
												component4.set_text(this.m_strHeroEquipSkillName);
											}
											else
											{
												component4.set_text(this.m_strHeroPassiveSkillName);
											}
										}
										else
										{
											component4.set_text(this.m_strAtkSkill0Name);
										}
									}
									component4.set_color(CUIUtility.s_Text_SkillName_And_HurtValue_Color[(int)hurtType]);
								}
							}
							float value = (uiTotalDamge == 0u) ? 1f : ((float)array[i].iValue / uiTotalDamge);
							Transform transform6 = transform.FindChild("Damage");
							if (transform6)
							{
								for (int j = 0; j < 4; j++)
								{
									Transform transform7 = transform6.FindChild(CHostHeroDeadInfo.m_Skill_HurtValue_Bg_ImgName[j]);
									if (transform7)
									{
										transform7.gameObject.CustomSetActive(j == (int)hurtType);
										if (j == (int)hurtType)
										{
											Image component5 = transform7.GetComponent<Image>();
											if (component5)
											{
												component5.CustomFillAmount(value);
											}
										}
									}
								}
							}
							Transform transform8 = transform.FindChild("TxtDamageValue");
							if (transform8)
							{
								Text component6 = transform8.GetComponent<Text>();
								if (component6)
								{
									string text3 = string.Concat(new object[]
									{
										array[i].iValue,
										"(",
										value.ToString("P0"),
										")"
									});
									component6.set_text(text3);
									component6.set_color(CUIUtility.s_Text_SkillName_And_HurtValue_Color[(int)hurtType]);
								}
							}
						}
					}
				}
			}
		}

		private void InitHeroPanelTotalDamage(Transform trHeroPanel, int iHeroTotalDamage, float fTotalDamageRate, bool bIsMaxTotalDamage)
		{
			Transform transform = trHeroPanel.FindChild("PanelBottom");
			if (transform)
			{
				Transform transform2 = transform.FindChild("TxtTotalDamage");
				if (transform2)
				{
					Text component = transform2.GetComponent<Text>();
					if (component)
					{
						component.set_color(CUIUtility.s_Text_Total_Damage_Text_Color[bIsMaxTotalDamage ? 0 : 1]);
					}
					Outline component2 = transform2.GetComponent<Outline>();
					if (component2)
					{
						component2.set_effectColor(CUIUtility.s_Text_Total_Damage_Text_Outline_Color[bIsMaxTotalDamage ? 0 : 1]);
					}
					Transform transform3 = transform2.FindChild("TxtValue");
					if (transform3)
					{
						Text component3 = transform3.GetComponent<Text>();
						if (component3)
						{
							component3.set_color(CUIUtility.s_Text_Total_Damage_Value_Color[bIsMaxTotalDamage ? 0 : 1]);
							string text = string.Concat(new object[]
							{
								iHeroTotalDamage,
								"(",
								fTotalDamageRate.ToString("P0"),
								")"
							});
							component3.set_text(text);
						}
					}
					Transform transform4 = transform2.FindChild("Tag");
					if (transform4)
					{
						transform4.gameObject.CustomSetActive(bIsMaxTotalDamage);
					}
				}
			}
		}

		private void InitHeroPanelInfo(uint uiObjId, Transform trHeroPanel, int iHeroTotalDamage, uint uiTotalDamage, bool bIsMaxTotalDamage, CHostHeroDamage objHostHeroDamage)
		{
			if (trHeroPanel == null || uiObjId == 0u)
			{
				return;
			}
			this.InitHeroPanelHeroInfo(uiObjId, trHeroPanel, objHostHeroDamage);
			this.InitHeroPanelSkillInfo(uiObjId, trHeroPanel, uiTotalDamage, objHostHeroDamage);
			this.InitHeroPanelTotalDamage(trHeroPanel, iHeroTotalDamage, (uiTotalDamage == 0u) ? 1f : ((float)iHeroTotalDamage / uiTotalDamage), bIsMaxTotalDamage);
		}

		private void InitHeroDeadInfoForm()
		{
			this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			if (this.m_hostActor)
			{
				PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(Singleton<GamePlayerCenter>.instance.HostPlayerId);
				if (playerKDA == null)
				{
					return;
				}
				ListView<CHostHeroDamage>.Enumerator enumerator = playerKDA.m_hostHeroDamage.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null && this.m_hostActor.handle.ObjID == enumerator.Current.GetHostHeroObjId())
					{
						uint[] arrDiffTypeHurtValue = new uint[4];
						uint[] array = new uint[2];
						uint uiTotalDamage = 0u;
						int allActorsTotalDamageAndTopActorId = enumerator.Current.GetAllActorsTotalDamageAndTopActorId(ref array, 2, ref uiTotalDamage, ref arrDiffTypeHurtValue);
						uint uiObjId = 0u;
						ActorTypeDef actorTypeDef = ActorTypeDef.Invalid;
						enumerator.Current.GetKillerObjId(ref uiObjId, ref actorTypeDef);
						HURT_INFO[] array2 = new HURT_INFO[12];
						int[] array3 = new int[3];
						for (int i = 0; i < 1 + allActorsTotalDamageAndTopActorId; i++)
						{
							int num = 0;
							if (i == 0)
							{
								enumerator.Current.GetActorDamage(uiObjId, ref array2, ref num);
							}
							else
							{
								enumerator.Current.GetActorDamage(array[i - 1], ref array2, ref num);
							}
							array3[i] = num;
						}
						int num2 = (array3[0] > array3[1]) ? array3[0] : array3[1];
						num2 = ((num2 > array3[2]) ? num2 : array3[2]);
						for (int j = 0; j < 1 + allActorsTotalDamageAndTopActorId; j++)
						{
							if (j == 0)
							{
								Transform trHeroPanel = this.m_heroDeadInfoForm.transform.FindChild("PanelDeadInfo/KillerGounp/PanelKiller");
								this.InitHeroPanelInfo(uiObjId, trHeroPanel, array3[j], uiTotalDamage, array3[j] == num2, enumerator.Current);
							}
							else if (allActorsTotalDamageAndTopActorId == 2)
							{
								Transform trHeroPanel2 = this.m_heroDeadInfoForm.transform.FindChild("PanelDeadInfo/KillerGounp/PanelAssister" + (j - 1));
								this.InitHeroPanelInfo(array[j - 1], trHeroPanel2, array3[j], uiTotalDamage, array3[j] == num2, enumerator.Current);
							}
							else
							{
								Transform trHeroPanel3 = this.m_heroDeadInfoForm.transform.FindChild("PanelDeadInfo/KillerGounp/PanelAssister" + j);
								this.InitHeroPanelInfo(array[j - 1], trHeroPanel3, array3[j], uiTotalDamage, array3[j] == num2, enumerator.Current);
							}
						}
						int num3 = 0;
						ulong hostHeroDeadTime = enumerator.Current.GetHostHeroDeadTime();
						if (hostHeroDeadTime == (ulong)Singleton<BattleStatistic>.instance.m_battleDeadStat.m_uiFBTime)
						{
							num3 |= 2;
						}
						if (actorTypeDef == ActorTypeDef.Actor_Type_Organ)
						{
							num3 |= 4;
						}
						this.InitDeadInfoPanelBottomText(hostHeroDeadTime, arrDiffTypeHurtValue, uiTotalDamage, num3);
						this.ResetFormSize(1 + allActorsTotalDamageAndTopActorId);
						break;
					}
				}
			}
		}

		private void ResetFormSize(int iHeroPanelCount)
		{
			if (iHeroPanelCount >= 1 && iHeroPanelCount <= 3)
			{
				Animation component = this.m_heroDeadInfoForm.transform.GetComponent<Animation>();
				if (component)
				{
					float length = component["HeroDeadInfo_Bg_Anim"].length;
					component.Stop("HeroDeadInfo_Bg_Anim");
					float time = 0f;
					if (iHeroPanelCount != 3)
					{
						time = (float)(3 - iHeroPanelCount + 1) * length / 3f;
					}
					component["HeroDeadInfo_Bg_Anim"].time = time;
					component["HeroDeadInfo_Bg_Anim"].speed = 0f;
					component.Play("HeroDeadInfo_Bg_Anim");
				}
			}
		}

		private void InitDeadInfoPanelBottomText(ulong ulDeadTime, uint[] arrDiffTypeHurtValue, uint uiTotalDamage, int iSpecailType)
		{
			int count = GameDataMgr.deadInfoConditionDatabin.count;
			int i;
			for (i = 2; i <= count; i++)
			{
				ResDeadInfoCondition dataByKey = GameDataMgr.deadInfoConditionDatabin.GetDataByKey((long)i);
				if (this.MatchRule(dataByKey, ulDeadTime, arrDiffTypeHurtValue, uiTotalDamage, iSpecailType))
				{
					break;
				}
			}
			if (i > count)
			{
				i = 1;
			}
			if (i <= count)
			{
				int condtionTextCount = this.GetCondtionTextCount(i);
				if (condtionTextCount > 0)
				{
					int iIndex = (int)(Singleton<FrameSynchr>.instance.LogicFrameTick % (ulong)((long)condtionTextCount));
					string condtionText = this.GetCondtionText(i, iIndex);
					if (condtionText != null)
					{
						Transform transform = this.m_heroDeadInfoForm.transform.FindChild("PanelDeadInfo/TxtBottom");
						if (transform)
						{
							Text component = transform.GetComponent<Text>();
							if (component)
							{
								component.set_text(condtionText);
							}
						}
					}
				}
			}
		}

		private bool MatchRule(ResDeadInfoCondition resCond, ulong ulDeadTime, uint[] arrDiffTypeHurtValue, uint uiTotalDamage, int iSpecailType)
		{
			if (resCond == null)
			{
				return false;
			}
			if (resCond.ullEndTime != 0uL && (ulDeadTime < resCond.ullStartTime || ulDeadTime > resCond.ullEndTime))
			{
				return false;
			}
			if (resCond.bHurtType != 0)
			{
				if (resCond.bHurtType == 1 && resCond.bHurtType == 2)
				{
					return false;
				}
				if (resCond.dwHurtValue < arrDiffTypeHurtValue[(int)(resCond.bHurtType - 1)])
				{
					return false;
				}
				uint num = (uiTotalDamage == 0u) ? 100u : (arrDiffTypeHurtValue[(int)(resCond.bHurtType - 1)] * 100u / uiTotalDamage);
				if (num < resCond.dwHurtRate)
				{
					return false;
				}
			}
			return resCond.bSpecailType == 0 || (iSpecailType & 1 << (int)resCond.bSpecailType) != 0;
		}

		private int GetCondtionTextCount(int iCondId)
		{
			int num = 0;
			int count = GameDataMgr.deadInfoTextDatabin.count;
			for (int i = 0; i < count; i++)
			{
				ResDeadInfoText dataByKey = GameDataMgr.deadInfoTextDatabin.GetDataByKey((long)i);
				if ((int)dataByKey.bConditionId == iCondId)
				{
					num++;
				}
			}
			return num;
		}

		private string GetCondtionText(int iCondId, int iIndex = 0)
		{
			int num = 0;
			int count = GameDataMgr.deadInfoTextDatabin.count;
			for (int i = 0; i < count; i++)
			{
				ResDeadInfoText dataByKey = GameDataMgr.deadInfoTextDatabin.GetDataByKey((long)i);
				if ((int)dataByKey.bConditionId == iCondId)
				{
					if (num == iIndex)
					{
						return dataByKey.szText;
					}
					num++;
				}
			}
			return null;
		}
	}
}
