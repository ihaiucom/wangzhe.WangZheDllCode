using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CSkillButtonManager
	{
		private const float c_skillIndicatorRespondMinRadius = 15f;

		private const float c_skillIndicatorMoveRadius = 30f;

		private const float c_directionalSkillIndicatorRespondMinRadius = 30f;

		private const float c_skillICancleRadius = 270f;

		private const float c_skillIndicatorRadius = 110f;

		private const string c_skillJoystickTargetNormalBorderPath = "UGUI/Sprite/Battle/Battle_skillFrameBg_new.prefab";

		private const string c_skillJoystickTargetSelectedBorderPath = "UGUI/Sprite/Battle/Battle_ComboCD.prefab";

		private const string c_skillBtnFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_SkillBtn.prefab";

		private const string m_strKillNotifyBlueRingPath = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Blue_ring";

		private const string m_strKillNotifyRedRingPath = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Red_ring";

		private static enSkillButtonFormWidget[] s_skillButtons = new enSkillButtonFormWidget[]
		{
			enSkillButtonFormWidget.Button_Attack,
			enSkillButtonFormWidget.Button_Skill_1,
			enSkillButtonFormWidget.Button_Skill_2,
			enSkillButtonFormWidget.Button_Skill_3,
			enSkillButtonFormWidget.Button_Recover,
			enSkillButtonFormWidget.Button_Talent,
			enSkillButtonFormWidget.Button_Skill_6,
			enSkillButtonFormWidget.Button_Skill_Ornament,
			enSkillButtonFormWidget.Button_Skill_9,
			enSkillButtonFormWidget.Button_Skill_10,
			enSkillButtonFormWidget.None
		};

		private static enSkillButtonFormWidget[] s_skillCDTexts = new enSkillButtonFormWidget[]
		{
			enSkillButtonFormWidget.None,
			enSkillButtonFormWidget.Text_Skill_1_CD,
			enSkillButtonFormWidget.Text_Skill_2_CD,
			enSkillButtonFormWidget.Text_Skill_3_CD,
			enSkillButtonFormWidget.Text_Skill_4_CD,
			enSkillButtonFormWidget.Text_Skill_5_CD,
			enSkillButtonFormWidget.Text_Skill_6_CD,
			enSkillButtonFormWidget.Text_Skill_Ornament_CD,
			enSkillButtonFormWidget.Text_Skill_9_CD,
			enSkillButtonFormWidget.Text_Skill_10_CD,
			enSkillButtonFormWidget.None
		};

		private static enSkillButtonFormWidget[] s_skillBeanTexts = new enSkillButtonFormWidget[]
		{
			enSkillButtonFormWidget.None,
			enSkillButtonFormWidget.Text_Skill_1_Bean,
			enSkillButtonFormWidget.Text_Skill_2_Bean,
			enSkillButtonFormWidget.Text_Skill_3_Bean,
			enSkillButtonFormWidget.Text_Skill_4_Bean,
			enSkillButtonFormWidget.Text_Skill_5_Bean,
			enSkillButtonFormWidget.Text_Skill_6_Bean,
			enSkillButtonFormWidget.Text_Skill_7_Bean,
			enSkillButtonFormWidget.Text_Skill_9_Bean,
			enSkillButtonFormWidget.Text_Skill_10_Bean,
			enSkillButtonFormWidget.None
		};

		private SkillButton[] _skillButtons;

		private enSkillIndicatorMode m_skillIndicatorMode = enSkillIndicatorMode.FixedPosition;

		private bool m_currentSkillIndicatorResponed;

		private bool m_currentSkillTipsResponed;

		private bool m_commonAtkSlide;

		private int m_currentSkillJoystickSelectedIndex = -1;

		private stCampHeroInfo[] m_campHeroInfos = new stCampHeroInfo[0];

		private stCampHeroInfo[] m_targetInfos = new stCampHeroInfo[0];

		private uint m_CurTargtNum;

		private stCampHeroInfo[] m_EnemyHeroInfos = new stCampHeroInfo[0];

		private stCampHeroInfo[] m_EnemyTargetInfos = new stCampHeroInfo[0];

		private uint m_CurEnemyTargtNum;

		private static int MaxTargetNum = 4;

		private static int MaxTargetNum5 = 5;

		private static Color s_skillCursorBGColorBlue = new Color(0.168627456f, 0.7607843f, 1f, 1f);

		private static Color s_skillCursorBGColorRed = new Color(0.972549f, 0.184313729f, 0.184313729f, 1f);

		private GameObject attackOrganBtnEffect;

		private Player hostPlayer;

		private SkillSlotType m_currentSkillSlotType = SkillSlotType.SLOT_SKILL_COUNT;

		private Vector2 m_currentSkillDownScreenPosition = Vector2.zero;

		private bool m_currentSkillIndicatorEnabled;

		private bool m_currentSkillIndicatorJoystickEnabled;

		private enSkillJoystickMode m_currentSkillJoystickMode;

		private bool m_currentSkillIndicatorInCancelArea;

		private Vector2 m_currentSkillIndicatorScreenPosition = Vector2.zero;

		private static byte s_maxButtonCount = 10;

		public bool CurrentSkillTipsResponed
		{
			get
			{
				return this.m_currentSkillTipsResponed;
			}
		}

		public bool CurrentSkillIndicatorResponed
		{
			get
			{
				return this.m_currentSkillIndicatorResponed;
			}
		}

		public CSkillButtonManager()
		{
			this._skillButtons = new SkillButton[(int)CSkillButtonManager.s_maxButtonCount];
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
		}

		~CSkillButtonManager()
		{
		}

		public void Init()
		{
			CUIFormScript cUIFormScript = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
			if (cUIFormScript != null)
			{
				GameObject widget = cUIFormScript.GetWidget(33);
				this.attackOrganBtnEffect = Utility.FindChild(widget, "Light");
				if (this.attackOrganBtnEffect != null)
				{
					Animation component = this.attackOrganBtnEffect.GetComponent<Animation>();
					if (component != null)
					{
						AnimationState animationState = component["ApplyAttactOrgan"];
						if (animationState != null)
						{
							animationState.wrapMode = WrapMode.Loop;
						}
					}
				}
			}
			this.hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
		}

		public void Uninit()
		{
			this.attackOrganBtnEffect = null;
			this.hostPlayer = null;
		}

		public void Initialise(PoolObjHandle<ActorRoot> actor, bool bInitSpecifiedButton = false, SkillSlotType specifiedType = SkillSlotType.SLOT_SKILL_COUNT)
		{
			if (!actor || actor.handle.SkillControl == null || actor.handle.SkillControl.SkillSlotArray == null)
			{
				return;
			}
			SkillSlot[] skillSlotArray = actor.handle.SkillControl.SkillSlotArray;
			for (int i = 0; i < (int)CSkillButtonManager.s_maxButtonCount; i++)
			{
				SkillSlotType skillSlotType = (SkillSlotType)i;
				if (!bInitSpecifiedButton || skillSlotType == specifiedType)
				{
					SkillButton button = this.GetButton(skillSlotType);
					SkillSlot skillSlot = skillSlotArray[i];
					if (skillSlotType >= SkillSlotType.SLOT_SKILL_1 && skillSlotType < SkillSlotType.SLOT_SKILL_4 && !Singleton<WatchController>.GetInstance().IsWatching)
					{
						if (skillSlot == null)
						{
							goto IL_8D4;
						}
						ResSkillCfgInfo cfgData = skillSlot.SkillObj.cfgData;
						if (cfgData != null)
						{
							MinimapSkillIndicator_3DUI.AddIndicatorData(skillSlotType, cfgData.szMapIndicatorNormalPrefab, cfgData.szMapIndicatorRedPrefab, (float)cfgData.iSmallMapIndicatorHeight, (float)cfgData.iBigMapIndicatorHeight);
						}
					}
					DebugHelper.Assert(button != null);
					if (button != null)
					{
						button.bDisableFlag = (skillSlot != null && !skillSlot.EnableButtonFlag);
						if (actor.handle.SkillControl.IsIngnoreDisableSkill((SkillSlotType)i) && actor.handle.SkillControl.ForceDisableSkill[i] == 0)
						{
							button.bLimitedFlag = false;
						}
						else
						{
							button.bLimitedFlag = (actor.handle.SkillControl.DisableSkill[i] >= 1);
						}
						SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
						if (skillSlotType == SkillSlotType.SLOT_SKILL_6)
						{
							if (curLvelContext.IsGameTypeGuide())
							{
								if (!CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID) || skillSlot == null || skillSlot.SkillObj == null || skillSlot.SkillObj.cfgData == null)
								{
									button.m_button.CustomSetActive(false);
									goto IL_8D4;
								}
							}
							else if (!curLvelContext.IsMobaModeWithOutGuide() || curLvelContext.m_pvpPlayerNum != 10 || skillSlot == null || skillSlot.SkillObj == null || skillSlot.SkillObj.cfgData == null)
							{
								button.m_button.CustomSetActive(false);
								goto IL_8D4;
							}
						}
						if (skillSlotType == SkillSlotType.SLOT_SKILL_7 && !curLvelContext.m_bEnableOrnamentSlot)
						{
							button.m_button.CustomSetActive(false);
							button.m_cdText.CustomSetActive(false);
						}
						else
						{
							if (Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeAdventure())
							{
								if ((Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena()) && (skillSlotType == SkillSlotType.SLOT_SKILL_4 || skillSlotType == SkillSlotType.SLOT_SKILL_5))
								{
									if (button.m_button != null && button.m_cdText != null)
									{
										button.m_button.CustomSetActive(false);
										button.m_cdText.CustomSetActive(false);
									}
									goto IL_8D4;
								}
								if (skillSlotType >= SkillSlotType.SLOT_SKILL_1 && skillSlotType <= SkillSlotType.SLOT_SKILL_3)
								{
									if (button.m_button != null)
									{
										GameObject skillLvlFrameImg = button.GetSkillLvlFrameImg(true);
										if (skillLvlFrameImg != null)
										{
											skillLvlFrameImg.CustomSetActive(false);
										}
										GameObject skillLvlFrameImg2 = button.GetSkillLvlFrameImg(false);
										if (skillLvlFrameImg2 != null)
										{
											skillLvlFrameImg2.CustomSetActive(false);
										}
										GameObject skillFrameImg = button.GetSkillFrameImg();
										if (skillFrameImg != null)
										{
											skillFrameImg.CustomSetActive(true);
										}
									}
									if (skillSlot != null)
									{
										int skillLevel = 0;
										if (skillSlotType == SkillSlotType.SLOT_SKILL_1 || skillSlotType == SkillSlotType.SLOT_SKILL_2)
										{
											ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(142u);
											skillLevel = (int)dataByKey.dwConfValue;
										}
										else if (skillSlotType == SkillSlotType.SLOT_SKILL_3)
										{
											ResGlobalInfo dataByKey2 = GameDataMgr.globalInfoDatabin.GetDataByKey(143u);
											skillLevel = (int)dataByKey2.dwConfValue;
										}
										skillSlot.SetSkillLevel(skillLevel);
									}
								}
							}
							if (!(button.m_button == null))
							{
								CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
								CUIEventScript component2 = button.GetDisableButton().GetComponent<CUIEventScript>();
								if (skillSlot != null)
								{
									component.enabled = true;
									component2.enabled = true;
									if (skillSlotType == SkillSlotType.SLOT_SKILL_1 || skillSlotType == SkillSlotType.SLOT_SKILL_2 || skillSlotType == SkillSlotType.SLOT_SKILL_3)
									{
										if (skillSlot.EnableButtonFlag)
										{
											component.enabled = true;
										}
										else
										{
											component.enabled = false;
										}
									}
									if (button.GetDisableButton() != null)
									{
										if (skillSlot.GetSkillLevel() > 0)
										{
											this.SetEnableButton(skillSlotType);
											if ((actor.handle.ActorControl != null && actor.handle.ActorControl.IsDeadState) || (skillSlot.SlotType != SkillSlotType.SLOT_SKILL_0 && !skillSlot.IsCDReady))
											{
												this.SetDisableButton(skillSlotType);
											}
										}
										else
										{
											this.SetDisableButton(skillSlotType);
										}
									}
									if ((skillSlotType != SkillSlotType.SLOT_SKILL_2 && skillSlotType != SkillSlotType.SLOT_SKILL_3) || (Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() != MinimapSys.EMapType.Skill && (Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() != MinimapSys.EMapType.Big || GameSettings.MiniMapPosMode != MiniMapPosType.MiniMapRightMode)))
									{
										if (button.m_button != null)
										{
											button.m_button.CustomSetActive(true);
										}
										if (button.m_cdText != null)
										{
											button.m_cdText.CustomSetActive(true);
										}
									}
									if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
									{
										Image component3 = button.m_button.transform.Find("Present/SkillImg").GetComponent<Image>();
										component3.gameObject.CustomSetActive(true);
										component3.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skillSlot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false, false);
										if (skillSlotType == SkillSlotType.SLOT_SKILL_4 || skillSlotType == SkillSlotType.SLOT_SKILL_5 || skillSlotType == SkillSlotType.SLOT_SKILL_6 || skillSlotType == SkillSlotType.SLOT_SKILL_7)
										{
											Transform transform = button.m_button.transform.Find("lblName");
											if (transform != null)
											{
												if (skillSlot.SkillObj.cfgData != null)
												{
													transform.GetComponent<Text>().set_text(skillSlot.SkillObj.cfgData.szSkillName);
												}
												transform.gameObject.CustomSetActive(true);
											}
										}
										if (actor.handle.SkillControl.IsDisableSkillSlot(skillSlotType))
										{
											this.SetLimitButton(skillSlotType);
										}
										else if (skillSlot.GetSkillLevel() > 0 && skillSlot.IsEnergyEnough)
										{
											this.CancelLimitButton(skillSlotType);
										}
										if (skillSlot.GetSkillLevel() > 0)
										{
											this.UpdateButtonCD(skillSlotType, skillSlot.CurSkillCD);
										}
										else if (button.m_cdText)
										{
											button.m_cdText.CustomSetActive(false);
										}
									}
									component.m_onDownEventParams.m_skillSlotType = skillSlotType;
									component.m_onUpEventParams.m_skillSlotType = skillSlotType;
									component.m_onHoldEventParams.m_skillSlotType = skillSlotType;
									component.m_onHoldEndEventParams.m_skillSlotType = skillSlotType;
									component.m_onDragStartEventParams.m_skillSlotType = skillSlotType;
									component.m_onDragEventParams.m_skillSlotType = skillSlotType;
									component2.m_onClickEventParams.m_skillSlotType = skillSlotType;
									if (skillSlot.skillChangeEvent.IsDisplayUI())
									{
										this.SetComboEffect(skillSlotType, skillSlot.skillChangeEvent.GetEffectTIme(), skillSlot.skillChangeEvent.GetEffectMaxTime());
									}
									else if (!curLvelContext.IsMobaMode())
									{
										this.SetComboEffect(skillSlotType, skillSlot.skillChangeEvent.GetEffectTIme(), skillSlot.skillChangeEvent.GetEffectMaxTime());
									}
								}
								else
								{
									component.enabled = false;
									component2.enabled = false;
									if (button.GetDisableButton() != null)
									{
										this.SetDisableButton(skillSlotType);
									}
									if (button.m_cdText != null)
									{
										button.m_cdText.CustomSetActive(false);
									}
									if (skillSlotType == SkillSlotType.SLOT_SKILL_7)
									{
										Transform transform2 = button.m_button.transform.Find("Present/SkillImg");
										if (transform2 != null)
										{
											transform2.gameObject.CustomSetActive(false);
										}
										Transform transform3 = button.m_button.transform.Find("lblName");
										if (transform3 != null)
										{
											transform3.gameObject.CustomSetActive(false);
										}
									}
								}
								if (button.m_beanText != null)
								{
									if (skillSlotType >= SkillSlotType.SLOT_SKILL_1 && skillSlotType <= SkillSlotType.SLOT_SKILL_3 && skillSlot != null && skillSlot.IsUnLock() && skillSlot.bConsumeBean)
									{
										Text component4 = button.m_beanText.GetComponent<Text>();
										component4.set_text(skillSlot.skillBeanAmount.ToString());
										button.m_beanText.CustomSetActive(true);
									}
									else
									{
										button.m_beanText.CustomSetActive(false);
									}
									if (button.m_button != null)
									{
										button.m_beanText.transform.position = button.m_button.transform.position;
									}
								}
								if ((skillSlotType == SkillSlotType.SLOT_SKILL_9 || skillSlotType == SkillSlotType.SLOT_SKILL_10) && !bInitSpecifiedButton)
								{
									button.m_button.CustomSetActive(false);
									button.m_cdText.CustomSetActive(false);
								}
							}
						}
					}
				}
				IL_8D4:;
			}
		}

		public void Clear()
		{
			if (this._skillButtons != null)
			{
				for (int i = 0; i < this._skillButtons.Length; i++)
				{
					if (this._skillButtons[i] != null)
					{
						this._skillButtons[i].Clear();
					}
				}
			}
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
		}

		public void InitializeCampHeroInfo(CUIFormScript battleFormScript, bool bIsEnemyCamp = false)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player == null)
			{
				return;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			int num = 0;
			if (curLvelContext.IsMobaMode())
			{
				COM_PLAYERCAMP camp = bIsEnemyCamp ? ((player.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_2 : COM_PLAYERCAMP.COM_PLAYERCAMP_1) : player.PlayerCamp;
				List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(camp);
				if (allCampPlayers != null)
				{
					if (bIsEnemyCamp)
					{
						this.m_EnemyHeroInfos = new stCampHeroInfo[allCampPlayers.get_Count()];
					}
					else
					{
						this.m_campHeroInfos = new stCampHeroInfo[allCampPlayers.get_Count() - 1];
					}
					for (int i = 0; i < allCampPlayers.get_Count(); i++)
					{
						if (allCampPlayers.get_Item(i) != player)
						{
							if (bIsEnemyCamp)
							{
								this.m_EnemyHeroInfos[num].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint)allCampPlayers.get_Item(i).Captain.handle.TheActorMeta.ConfigId, 0u);
								this.m_EnemyHeroInfos[num].m_objectID = allCampPlayers.get_Item(i).Captain.handle.ObjID;
							}
							else
							{
								this.m_campHeroInfos[num].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint)allCampPlayers.get_Item(i).Captain.handle.TheActorMeta.ConfigId, 0u);
								this.m_campHeroInfos[num].m_objectID = allCampPlayers.get_Item(i).Captain.handle.ObjID;
							}
							num++;
						}
					}
				}
			}
			else if (!bIsEnemyCamp)
			{
				ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = player.GetAllHeroes();
				int count = allHeroes.Count;
				if (count > 0)
				{
					this.m_campHeroInfos = new stCampHeroInfo[count - 1];
					for (int j = 0; j < count; j++)
					{
						if (!(allHeroes[j] == player.Captain))
						{
							this.m_campHeroInfos[num].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint)allHeroes[j].handle.TheActorMeta.ConfigId, 0u);
							this.m_campHeroInfos[num].m_objectID = allHeroes[j].handle.ObjID;
							num++;
						}
					}
				}
			}
			this.m_currentSkillJoystickSelectedIndex = -1;
			if (battleFormScript == null)
			{
				return;
			}
			GameObject skillCursor;
			if (bIsEnemyCamp)
			{
				skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
			}
			else
			{
				skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
			}
			CUIFormScript skillCursorFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript();
			if (skillCursorFormScript == null)
			{
				return;
			}
			if (skillCursor != null)
			{
				CUIComponent component = skillCursor.GetComponent<CUIComponent>();
				if (component != null && component.m_widgets != null)
				{
					if (bIsEnemyCamp)
					{
						CSkillButtonManager.MaxTargetNum5 = component.m_widgets.Length;
						this.m_EnemyTargetInfos = new stCampHeroInfo[CSkillButtonManager.MaxTargetNum5];
					}
					else
					{
						CSkillButtonManager.MaxTargetNum = component.m_widgets.Length;
						this.m_targetInfos = new stCampHeroInfo[CSkillButtonManager.MaxTargetNum];
					}
					for (int k = 0; k < component.m_widgets.Length; k++)
					{
						GameObject gameObject = component.m_widgets[k];
						if (gameObject != null)
						{
							gameObject.CustomSetActive(true);
							gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
							if (bIsEnemyCamp)
							{
								CSkillButtonManager.GetJoystickHeadAreaInScreen(ref this.m_EnemyTargetInfos[k].m_headAreaInScreen, battleFormScript, skillCursor.transform as RectTransform, gameObject.transform as RectTransform);
								CSkillButtonManager.GetJoystickHeadAreaFan(ref this.m_EnemyTargetInfos[k].m_headAreaFan, gameObject, (k == 0) ? null : component.m_widgets[k - 1], (k == component.m_widgets.Length - 1) ? null : component.m_widgets[k + 1]);
							}
							else
							{
								CSkillButtonManager.GetJoystickHeadAreaInScreen(ref this.m_targetInfos[k].m_headAreaInScreen, battleFormScript, skillCursor.transform as RectTransform, gameObject.transform as RectTransform);
								CSkillButtonManager.GetJoystickHeadAreaFan(ref this.m_targetInfos[k].m_headAreaFan, gameObject, (k == 0) ? null : component.m_widgets[k - 1], (k == component.m_widgets.Length - 1) ? null : component.m_widgets[k + 1]);
							}
						}
					}
				}
				skillCursor.CustomSetActive(false);
			}
		}

		public void ResetSkillTargetJoyStickHeadToCampHero()
		{
			GameObject gameObject = null;
			bool flag = false;
			if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
			{
				gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
			}
			else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
			{
				flag = true;
				gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
			}
			CUIFormScript skillCursorFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript();
			if (skillCursorFormScript == null)
			{
				return;
			}
			if (gameObject != null)
			{
				CUIComponent component = gameObject.GetComponent<CUIComponent>();
				DebugHelper.Assert(flag ? (CSkillButtonManager.MaxTargetNum5 >= this.m_EnemyHeroInfos.Length) : (CSkillButtonManager.MaxTargetNum >= this.m_campHeroInfos.Length), "目标数大于轮盘支持的最大英雄数!");
				if (flag)
				{
					this.m_CurTargtNum = (uint)Mathf.Min(CSkillButtonManager.MaxTargetNum5, this.m_EnemyHeroInfos.Length);
				}
				else
				{
					this.m_CurTargtNum = (uint)Mathf.Min(CSkillButtonManager.MaxTargetNum, this.m_campHeroInfos.Length);
				}
				if (component != null && component.m_widgets != null)
				{
					for (int i = 0; i < component.m_widgets.Length; i++)
					{
						GameObject gameObject2 = component.m_widgets[i];
						if (gameObject2 != null)
						{
							if (flag)
							{
								if (i >= this.m_EnemyHeroInfos.Length)
								{
									this.m_EnemyTargetInfos[i].m_objectID = 0u;
									gameObject2.CustomSetActive(false);
									goto IL_29F;
								}
								this.m_EnemyTargetInfos[i].m_objectID = this.m_EnemyHeroInfos[i].m_objectID;
							}
							else
							{
								if (i >= this.m_campHeroInfos.Length)
								{
									this.m_targetInfos[i].m_objectID = 0u;
									gameObject2.CustomSetActive(false);
									goto IL_29F;
								}
								this.m_targetInfos[i].m_objectID = this.m_campHeroInfos[i].m_objectID;
							}
							gameObject2.CustomSetActive(true);
							gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
							CUIComponent component2 = gameObject2.GetComponent<CUIComponent>();
							if (component2 != null && component2.m_widgets != null && component2.m_widgets.Length >= 2)
							{
								GameObject gameObject3 = component2.m_widgets[0];
								if (gameObject3 != null)
								{
									Image component3 = gameObject3.GetComponent<Image>();
									if (component3 != null)
									{
										if (flag)
										{
											component3.SetSprite(this.m_EnemyHeroInfos[i].m_headIconPath, skillCursorFormScript, true, false, false, false);
										}
										else
										{
											component3.SetSprite(this.m_campHeroInfos[i].m_headIconPath, skillCursorFormScript, true, false, false, false);
										}
									}
								}
							}
						}
						IL_29F:;
					}
				}
			}
		}

		public void ResetSkillTargetJoyStickHeadToTargets(ListValueView<uint> targetIDs)
		{
			bool flag = false;
			GameObject gameObject = null;
			if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
			{
				gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
			}
			else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
			{
				flag = true;
				gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
			}
			CUIFormScript skillCursorFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript();
			if (skillCursorFormScript == null)
			{
				return;
			}
			if (gameObject != null)
			{
				CUIComponent component = gameObject.GetComponent<CUIComponent>();
				int num = flag ? CSkillButtonManager.MaxTargetNum5 : CSkillButtonManager.MaxTargetNum;
				DebugHelper.Assert(num >= targetIDs.Count, "目标数大于轮盘支持的最大英雄数!");
				this.m_CurTargtNum = (uint)Mathf.Min(num, targetIDs.Count);
				if (component != null && component.m_widgets != null)
				{
					for (int i = 0; i < component.m_widgets.Length; i++)
					{
						GameObject gameObject2 = component.m_widgets[i];
						if (gameObject2 != null)
						{
							if (flag)
							{
								if (i >= targetIDs.Count)
								{
									this.m_EnemyTargetInfos[i].m_objectID = 0u;
									gameObject2.CustomSetActive(false);
									goto IL_252;
								}
								this.m_EnemyTargetInfos[i].m_objectID = targetIDs[i];
							}
							else
							{
								if (i >= targetIDs.Count)
								{
									this.m_targetInfos[i].m_objectID = 0u;
									gameObject2.CustomSetActive(false);
									goto IL_252;
								}
								this.m_targetInfos[i].m_objectID = targetIDs[i];
							}
							gameObject2.CustomSetActive(true);
							gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
							CUIComponent component2 = gameObject2.GetComponent<CUIComponent>();
							if (component2 != null && component2.m_widgets != null && component2.m_widgets.Length >= 2)
							{
								PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(targetIDs[i]);
								string prefabPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint)actor.handle.TheActorMeta.ConfigId, 0u);
								GameObject gameObject3 = component2.m_widgets[0];
								if (gameObject3 != null)
								{
									Image component3 = gameObject3.GetComponent<Image>();
									if (component3 != null)
									{
										component3.SetSprite(prefabPath, skillCursorFormScript, true, false, false, false);
									}
								}
							}
						}
						IL_252:;
					}
				}
			}
		}

		public static Color GetCursorBGColor(bool cancel)
		{
			return cancel ? CSkillButtonManager.s_skillCursorBGColorRed : CSkillButtonManager.s_skillCursorBGColorBlue;
		}

		private static void GetJoystickHeadAreaInScreen(ref Rect targetArea, CUIFormScript formScript, RectTransform joystickRectTransform, RectTransform targetRectTransform)
		{
			if (joystickRectTransform == null || targetRectTransform == null)
			{
				targetArea = new Rect(0f, 0f, 0f, 0f);
				return;
			}
			Vector2 vector = new Vector2(formScript.ChangeFormValueToScreen(targetRectTransform.anchoredPosition.x), formScript.ChangeFormValueToScreen(targetRectTransform.anchoredPosition.y));
			float num = formScript.ChangeFormValueToScreen(targetRectTransform.rect.width);
			float num2 = formScript.ChangeFormValueToScreen(targetRectTransform.rect.height);
			float left = vector.x - num / 2f;
			float top = vector.y - num2 / 2f;
			targetArea = new Rect(left, top, num, num2);
		}

		private static void GetJoystickHeadAreaFan(ref stFan headAreaFan, GameObject head, GameObject preHead, GameObject backHead)
		{
			if (preHead == null && backHead == null)
			{
				headAreaFan.m_minRadian = 0f;
				headAreaFan.m_maxRadian = 0f;
			}
			float radian = CSkillButtonManager.GetRadian((head.transform as RectTransform).anchoredPosition);
			if (preHead != null)
			{
				headAreaFan.m_minRadian = CSkillButtonManager.GetRadian(((head.transform as RectTransform).anchoredPosition + (preHead.transform as RectTransform).anchoredPosition) / 2f);
				if (backHead == null)
				{
					headAreaFan.m_maxRadian = radian + (radian - headAreaFan.m_minRadian);
					return;
				}
			}
			if (backHead != null)
			{
				headAreaFan.m_maxRadian = CSkillButtonManager.GetRadian(((head.transform as RectTransform).anchoredPosition + (backHead.transform as RectTransform).anchoredPosition) / 2f);
				if (preHead == null)
				{
					headAreaFan.m_minRadian = radian - (headAreaFan.m_maxRadian - radian);
					return;
				}
			}
		}

		private static float GetRadian(Vector2 point)
		{
			float num = Mathf.Atan2(point.y, point.x);
			if (num < 0f)
			{
				num += 6.28318548f;
			}
			return num;
		}

		public static void Preload(ref ActorPreloadTab result)
		{
		}

		public SkillButton GetButton(SkillSlotType skillSlotType)
		{
			if (skillSlotType < SkillSlotType.SLOT_SKILL_0 || skillSlotType >= (SkillSlotType)this._skillButtons.Length)
			{
				return null;
			}
			SkillButton skillButton = this._skillButtons[(int)skillSlotType];
			if (skillButton == null)
			{
				FightForm fightForm = Singleton<CBattleSystem>.GetInstance().FightForm;
				if (fightForm != null)
				{
					CUIFormScript skillBtnFormScript = fightForm.GetSkillBtnFormScript();
					skillButton = new SkillButton();
					skillButton.m_button = ((skillBtnFormScript == null) ? null : skillBtnFormScript.GetWidget((int)CSkillButtonManager.s_skillButtons[(int)skillSlotType]));
					skillButton.m_cdText = ((skillBtnFormScript == null) ? null : skillBtnFormScript.GetWidget((int)CSkillButtonManager.s_skillCDTexts[(int)skillSlotType]));
					skillButton.m_beanText = ((skillBtnFormScript == null) ? null : skillBtnFormScript.GetWidget((int)CSkillButtonManager.s_skillBeanTexts[(int)skillSlotType]));
					this._skillButtons[(int)skillSlotType] = skillButton;
				}
				if (skillButton.m_button != null)
				{
					Transform transform = skillButton.m_button.transform.FindChild("IndicatorPosition");
					if (transform != null)
					{
						skillButton.m_skillIndicatorFixedPosition = transform.position;
					}
				}
			}
			return skillButton;
		}

		public void SetLimitButton(SkillSlotType skillSlotType)
		{
			CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
			if (fightFormScript == null)
			{
				return;
			}
			if (this.m_currentSkillSlotType == skillSlotType)
			{
				this.SkillButtonUp(fightFormScript, skillSlotType, false, default(Vector2));
			}
			SkillButton button = this.GetButton(skillSlotType);
			DebugHelper.Assert(button != null);
			if (button != null)
			{
				button.bLimitedFlag = true;
				if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
				{
					if (button.m_button != null)
					{
						CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
						if (component)
						{
							if (component.ClearInputStatus())
							{
								Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
							}
							component.enabled = false;
						}
					}
					GameObject gameObject = button.GetAnimationPresent().transform.Find("disableFrame").gameObject;
					DebugHelper.Assert(gameObject != null);
					if (gameObject != null)
					{
						gameObject.CustomSetActive(true);
					}
					GameObject disableButton = button.GetDisableButton();
					if (disableButton)
					{
						disableButton.CustomSetActive(true);
					}
					GameObject animationPresent = button.GetAnimationPresent();
					CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.disable.ToString(), false);
				}
			}
		}

		public void CancelLimitButton(SkillSlotType skillSlotType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			SkillButton button = this.GetButton(skillSlotType);
			DebugHelper.Assert(button != null);
			if (button != null)
			{
				button.bLimitedFlag = false;
				if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
				{
					if (!button.bDisableFlag)
					{
						GameObject animationPresent = button.GetAnimationPresent();
						if (animationPresent != null)
						{
							Image component = animationPresent.GetComponent<Image>();
							if (component != null)
							{
								component.set_color(CUIUtility.s_Color_Full);
							}
						}
						GameObject disableButton = button.GetDisableButton();
						if (disableButton)
						{
							disableButton.CustomSetActive(false);
						}
						this.SetSelectTargetBtnState(false);
					}
				}
				else
				{
					if (button.m_button != null)
					{
						CUIEventScript component2 = button.m_button.GetComponent<CUIEventScript>();
						SkillSlot skillSlot;
						if (component2 && player != null && player.Captain && player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot))
						{
							if (skillSlot.EnableButtonFlag)
							{
								component2.enabled = true;
							}
							else
							{
								component2.enabled = false;
							}
						}
					}
					GameObject gameObject = button.GetAnimationPresent().transform.Find("disableFrame").gameObject;
					DebugHelper.Assert(gameObject != null);
					if (gameObject != null)
					{
						gameObject.CustomSetActive(false);
					}
					if (!button.bDisableFlag)
					{
						GameObject disableButton2 = button.GetDisableButton();
						if (disableButton2)
						{
							CUIEventScript component3 = disableButton2.GetComponent<CUIEventScript>();
							if (component3 && component3.ClearInputStatus())
							{
								Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
							}
							disableButton2.CustomSetActive(false);
						}
						GameObject animationPresent2 = button.GetAnimationPresent();
						CUICommonSystem.PlayAnimation(animationPresent2, enSkillButtonAnimationName.normal.ToString(), false);
					}
				}
			}
		}

		public void SetDisableButton(SkillSlotType skillSlotType)
		{
			CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
			if (fightFormScript == null)
			{
				return;
			}
			if (this.m_currentSkillSlotType == skillSlotType)
			{
				this.SkillButtonUp(fightFormScript, skillSlotType, false, default(Vector2));
			}
			SkillButton button = this.GetButton(skillSlotType);
			if (button != null)
			{
				if (button.m_button != null)
				{
					CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
					if (component)
					{
						if (component.ClearInputStatus())
						{
							Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
						}
						component.enabled = false;
					}
				}
				button.bDisableFlag = true;
				if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
				{
					GameObject animationPresent = button.GetAnimationPresent();
					CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.disable.ToString(), false);
				}
				else
				{
					GameObject animationPresent2 = button.GetAnimationPresent();
					if (animationPresent2 != null)
					{
						Image component2 = animationPresent2.GetComponent<Image>();
						if (component2 != null)
						{
							component2.set_color(CUIUtility.s_Color_DisableGray);
						}
					}
					GameObject disableButton = button.GetDisableButton();
					if (disableButton)
					{
						disableButton.CustomSetActive(true);
					}
					this.SetSelectTargetBtnState(true);
					this.EnableSpecialAttackBtn(false, enSkillButtonFormWidget.LastHitBtn);
					this.EnableSpecialAttackBtn(false, enSkillButtonFormWidget.AttackOrganBtn);
				}
				GameObject disableButton2 = button.GetDisableButton();
				if (disableButton2 != null)
				{
					disableButton2.CustomSetActive(true);
				}
			}
		}

		public void SetEnergyDisableButton(SkillSlotType skillSlotType)
		{
			CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
			if (fightFormScript == null)
			{
				return;
			}
			SkillButton button = this.GetButton(skillSlotType);
			if (button != null)
			{
				if (button.m_button != null)
				{
					CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
					if (component)
					{
						component.enabled = false;
					}
				}
				button.bDisableFlag = true;
				GameObject animationPresent = button.GetAnimationPresent();
				CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.disable.ToString(), false);
				GameObject disableButton = button.GetDisableButton();
				if (disableButton != null)
				{
					disableButton.CustomSetActive(true);
				}
			}
		}

		public bool SetEnableButton(SkillSlotType skillSlotType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				SkillSlot skillSlot;
				if (!player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot))
				{
					return false;
				}
				if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
				{
					if (player.Captain.handle.ActorControl.IsDeadState)
					{
						return false;
					}
				}
				else if (!skillSlot.EnableButtonFlag)
				{
					return false;
				}
			}
			SkillButton button = this.GetButton(skillSlotType);
			if (button != null)
			{
				button.bDisableFlag = false;
				if (button.bLimitedFlag)
				{
					return false;
				}
				if (button.m_button != null)
				{
					CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
					if (component)
					{
						if (component.ClearInputStatus())
						{
							Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
						}
						component.enabled = true;
					}
				}
				if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
				{
					GameObject animationPresent = button.GetAnimationPresent();
					CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.normal.ToString(), false);
				}
				else if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
				{
					GameObject animationPresent2 = button.GetAnimationPresent();
					if (animationPresent2 != null)
					{
						Image component2 = animationPresent2.GetComponent<Image>();
						if (component2 != null)
						{
							component2.set_color(CUIUtility.s_Color_Full);
						}
					}
					this.SetSelectTargetBtnState(false);
					this.EnableSpecialAttackBtn(true, enSkillButtonFormWidget.LastHitBtn);
					this.EnableSpecialAttackBtn(true, enSkillButtonFormWidget.AttackOrganBtn);
				}
				GameObject disableButton = button.GetDisableButton();
				if (disableButton != null)
				{
					CUIEventScript component3 = disableButton.GetComponent<CUIEventScript>();
					if (component3)
					{
						if (component3.ClearInputStatus())
						{
							Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
						}
						component3.enabled = true;
					}
					disableButton.CustomSetActive(false);
				}
			}
			return true;
		}

		public bool ClearButtonInput(SkillSlotType CurSlotType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				for (int i = 0; i < 10; i++)
				{
					if (CurSlotType != (SkillSlotType)i)
					{
						SkillSlot skillSlot;
						if (!player.Captain.handle.SkillControl.TryGetSkillSlot((SkillSlotType)i, out skillSlot))
						{
							return false;
						}
						SkillButton button = this.GetButton((SkillSlotType)i);
						if (button != null)
						{
							if (button.m_button != null)
							{
								CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
								if (component)
								{
									component.ClearInputStatus();
								}
							}
							GameObject disableButton = button.GetDisableButton();
							if (disableButton != null)
							{
								CUIEventScript component2 = disableButton.GetComponent<CUIEventScript>();
								if (component2)
								{
									component2.ClearInputStatus();
								}
							}
						}
					}
				}
			}
			return true;
		}

		public void LastHitButtonDown(CUIFormScript formScript)
		{
			this.SendUseCommonAttack(2, 0u);
		}

		public void LastHitButtonUp(CUIFormScript formScript)
		{
			this.SendUseCommonAttack(0, 0u);
		}

		public void AttackOrganButtonDown(CUIFormScript formScript)
		{
			this.SendUseCommonAttack(3, 0u);
		}

		public void AttackOrganButtonUp(CUIFormScript formScript)
		{
			this.SendUseCommonAttack(0, 0u);
		}

		public void SetLastHitBtnState(bool bEnable)
		{
			CUIFormScript cUIFormScript = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(25);
			if (widget != null)
			{
				widget.CustomSetActive(bEnable);
			}
		}

		public void SetAttackOrganBtnState(bool bEnable)
		{
			CUIFormScript cUIFormScript = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(33);
			if (widget != null)
			{
				widget.CustomSetActive(bEnable);
			}
		}

		public void SetAdvancedAttackBtnState(CommonAttactType attactType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player == null)
			{
				return;
			}
			LastHitMode useLastHitMode = player.useLastHitMode;
			AttackOrganMode curAttackOrganMode = player.curAttackOrganMode;
			if (attactType == CommonAttactType.Type1)
			{
				this.SetLastHitBtnState(useLastHitMode == LastHitMode.LastHit);
				this.SetAttackOrganBtnState(curAttackOrganMode == AttackOrganMode.AttackOrgan);
			}
			else
			{
				this.SetLastHitBtnState(false);
				this.SetAttackOrganBtnState(false);
			}
		}

		public void SkillButtonDown(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 downScreenPosition)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				int num = 0;
				if (player.Captain.handle.SkillControl != null && skillSlotType >= SkillSlotType.SLOT_SKILL_0 && skillSlotType < SkillSlotType.SLOT_SKILL_COUNT && player.Captain.handle.SkillControl.SkillSlotArray[(int)skillSlotType] != null)
				{
					num = player.Captain.handle.SkillControl.SkillSlotArray[(int)skillSlotType].GetSkillLevel();
				}
				if (num <= 0)
				{
					return;
				}
			}
			if (this.m_currentSkillSlotType != SkillSlotType.SLOT_SKILL_COUNT)
			{
				this.SkillButtonUp(formScript, this.m_currentSkillSlotType, false, default(Vector2));
			}
			this.SetSkillBtnDragFlag(skillSlotType, false);
			this.m_currentSkillSlotType = skillSlotType;
			this.m_currentSkillDownScreenPosition = downScreenPosition;
			this.m_currentSkillIndicatorEnabled = false;
			this.m_currentSkillIndicatorJoystickEnabled = false;
			this.m_currentSkillIndicatorInCancelArea = false;
			this.m_currentSkillJoystickMode = this.GetSkillJoystickMode(skillSlotType);
			this.m_commonAtkSlide = false;
			GameObject animationPresent = this.GetButton(skillSlotType).GetAnimationPresent();
			if (player == null)
			{
				return;
			}
			if (Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Skill && this.m_currentSkillJoystickMode == enSkillJoystickMode.MapSelect)
			{
				Singleton<CBattleSystem>.instance.TheMinimapSys.ClearMapSkillStatus();
				return;
			}
			if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
			{
				this.SendUseCommonAttack(1, 0u);
				Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, animationPresent, formScript, default(Quaternion?));
			}
			else
			{
				if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
				{
					CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.pressDown.ToString(), false);
				}
				this.ReadyUseSkillSlot(skillSlotType, !Singleton<GameInput>.GetInstance().IsSmartUse() && skillSlotType == SkillSlotType.SLOT_SKILL_7 && this.m_skillIndicatorMode == enSkillIndicatorMode.FixedPosition && this.m_currentSkillJoystickMode == enSkillJoystickMode.General);
				this.EnableSkillCursor(formScript, ref downScreenPosition, this.IsUseSkillCursorJoystick(skillSlotType), skillSlotType, player.Captain.handle.SkillControl.SkillSlotArray[(int)skillSlotType], skillSlotType != SkillSlotType.SLOT_SKILL_0);
				if ((skillSlotType == SkillSlotType.SLOT_SKILL_3 || skillSlotType == SkillSlotType.SLOT_SKILL_2) && player != null && player.Captain && player.Captain.handle.SkillControl != null)
				{
					SkillSlot skillSlot = null;
					player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot);
					GameObject effectPrefab = skillSlot.skillIndicator.effectPrefab;
					if (effectPrefab != null && !string.IsNullOrEmpty(skillSlot.SkillObj.cfgData.szMapIndicatorNormalPrefab))
					{
						Vector3 forward = effectPrefab.transform.forward;
						MinimapSkillIndicator_3DUI.SetIndicator(skillSlotType, ref forward);
						MinimapSkillIndicator_3DUI.SetIndicatorColor(!this.m_currentSkillIndicatorInCancelArea);
					}
				}
			}
		}

		public void SkillButtonUp(CUIFormScript formScript, SkillSlotType skillSlotType, bool isTriggeredActively, Vector2 screenPosition = default(Vector2))
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player == null || this.m_currentSkillSlotType != skillSlotType || formScript == null)
			{
				return;
			}
			if (player.Captain)
			{
				int num = 0;
				if (player.Captain.handle.SkillControl != null && skillSlotType >= SkillSlotType.SLOT_SKILL_0 && skillSlotType < SkillSlotType.SLOT_SKILL_COUNT && player.Captain.handle.SkillControl.SkillSlotArray[(int)skillSlotType] != null)
				{
					num = player.Captain.handle.SkillControl.SkillSlotArray[(int)skillSlotType].GetSkillLevel();
				}
				if (num <= 0)
				{
					return;
				}
			}
			SkillButton button = this.GetButton(skillSlotType);
			if (button == null)
			{
				return;
			}
			GameObject animationPresent = button.GetAnimationPresent();
			if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
			{
				if (this.m_commonAtkSlide)
				{
					this.CommonAtkSlide(formScript, screenPosition);
					this.m_commonAtkSlide = false;
				}
				this.SendUseCommonAttack(0, 0u);
			}
			else
			{
				if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
				{
					CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.pressUp.ToString(), false);
				}
				if (isTriggeredActively && !this.m_currentSkillIndicatorInCancelArea)
				{
					if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
					{
						enSkillJoystickMode skillJoystickMode = this.GetSkillJoystickMode(skillSlotType);
						if (skillJoystickMode != enSkillJoystickMode.MapSelect && skillJoystickMode != enSkillJoystickMode.MapSelectOther)
						{
							uint num2 = 0u;
							SkillSlot skillSlot;
							if (GameSettings.TheCastType == CastType.LunPanCast && GameSettings.ShowEnemyHeroHeadBtnMode && GameSettings.LunPanLockEnemyHeroMode && player.Captain && player.Captain.handle.SkillControl != null && player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot))
							{
								Skill skill = (skillSlot.NextSkillObj != null) ? skillSlot.NextSkillObj : skillSlot.SkillObj;
								if (skill != null && skill.cfgData != null && skill.cfgData.bRangeAppointType == 1 && skillSlot.CanUseSkillWithEnemyHeroSelectMode() && skillJoystickMode == enSkillJoystickMode.General && (num2 = Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.GetLockedEnemyHeroObjId()) > 0u)
								{
									Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.OnSkillBtnUp(skillSlotType, this.m_currentSkillJoystickMode);
								}
							}
							if (num2 == 0u)
							{
								if (this.m_currentSkillJoystickSelectedIndex != -1)
								{
									if (skillJoystickMode == enSkillJoystickMode.SelectTarget)
									{
										num2 = this.m_targetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
									}
									else if (skillJoystickMode == enSkillJoystickMode.SelectTarget5)
									{
										num2 = this.m_EnemyTargetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
									}
								}
								this.RequestUseSkillSlot(skillSlotType, this.m_currentSkillJoystickMode, num2);
							}
						}
					}
				}
				else
				{
					this.CancelUseSkillSlot(skillSlotType, this.m_currentSkillJoystickMode);
				}
				if (this.m_currentSkillIndicatorEnabled)
				{
					this.DisableSkillCursor(formScript, skillSlotType);
				}
				if (skillSlotType == SkillSlotType.SLOT_SKILL_3 || skillSlotType == SkillSlotType.SLOT_SKILL_2)
				{
					MinimapSkillIndicator_3DUI.CancelIndicator();
				}
			}
			if (CSkillButtonManager.IsSelectedTargetJoyStick(this.m_currentSkillJoystickMode) && this.m_currentSkillJoystickSelectedIndex >= 0)
			{
				this.m_currentSkillJoystickSelectedIndex = -1;
				this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
			}
			this.m_currentSkillSlotType = SkillSlotType.SLOT_SKILL_COUNT;
			this.m_currentSkillDownScreenPosition = Vector2.zero;
		}

		public void SkillButtonUp(CUIFormScript formScript)
		{
			if (this.m_currentSkillSlotType == SkillSlotType.SLOT_SKILL_COUNT || formScript == null)
			{
				return;
			}
			this.SkillButtonUp(formScript, this.m_currentSkillSlotType, false, default(Vector2));
		}

		public void SelectedMapTarget(SkillSlotType skillSlotType, uint targetObjId)
		{
			this.RequestUseSkillSlot(skillSlotType, enSkillJoystickMode.MapSelect, targetObjId);
		}

		public void SelectedMapTarget(uint targetObjId)
		{
			SkillSlotType skillSlotType;
			if (this.HasMapSlectTargetSkill(out skillSlotType))
			{
				this.RequestUseSkillSlot(skillSlotType, enSkillJoystickMode.MapSelect, targetObjId);
			}
		}

		public void CommonAtkSlide(CUIFormScript battleFormScript, Vector2 screenPosition)
		{
			if (GameSettings.TheCommonAttackType == CommonAttactType.Type2)
			{
				SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
				CUIFormScript skillBtnFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
				if (button == null || button.bDisableFlag || skillBtnFormScript == null)
				{
					return;
				}
				GameObject widget = skillBtnFormScript.GetWidget(24);
				GameObject widget2 = skillBtnFormScript.GetWidget(9);
				if (this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, widget))
				{
					Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_HERO);
					Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, widget, battleFormScript, default(Quaternion?));
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_hero", null);
				}
				else if (this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, widget2))
				{
					Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_SOLDIER);
					Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, widget2, battleFormScript, default(Quaternion?));
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_xiaobing", null);
				}
			}
		}

		public void DragSkillButton(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 dragScreenPosition)
		{
			if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
			{
				this.m_commonAtkSlide = true;
			}
			if (this.m_currentSkillSlotType != skillSlotType || !this.m_currentSkillIndicatorEnabled)
			{
				return;
			}
			bool currentSkillIndicatorInCancelArea = this.m_currentSkillIndicatorInCancelArea;
			if (formScript == null)
			{
				return;
			}
			this.SetSkillBtnDragFlag(skillSlotType, true);
			GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(this.m_currentSkillJoystickMode);
			Vector2 vector = this.MoveSkillCursor(formScript, skillCursor, skillSlotType, this.m_currentSkillJoystickMode, ref dragScreenPosition, out this.m_currentSkillIndicatorInCancelArea, true);
			if (currentSkillIndicatorInCancelArea != this.m_currentSkillIndicatorInCancelArea)
			{
				this.ChangeSkillCursorBGSprite(formScript, skillCursor, this.m_currentSkillIndicatorInCancelArea);
			}
			if (this.m_currentSkillJoystickMode == enSkillJoystickMode.General)
			{
				if (GameSettings.TheCastType == CastType.LunPanCast && GameSettings.ShowEnemyHeroHeadBtnMode && GameSettings.LunPanLockEnemyHeroMode)
				{
					Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
					SkillSlot skillSlot;
					if (player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot))
					{
						Skill skill = (skillSlot.NextSkillObj != null) ? skillSlot.NextSkillObj : skillSlot.SkillObj;
						if (skill != null && skill.cfgData != null && skill.cfgData.bRangeAppointType == 1 && skillSlot.CanUseSkillWithEnemyHeroSelectMode() && Singleton<CBattleSystem>.instance.FightForm != null && Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn != null)
						{
							Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.OnSkillBtnDrag(formScript, skillSlotType, dragScreenPosition, this.m_currentSkillIndicatorInCancelArea);
							return;
						}
					}
				}
				this.MoveSkillCursorInScene(skillSlotType, ref vector, this.m_currentSkillIndicatorInCancelArea, true);
				if (skillSlotType == SkillSlotType.SLOT_SKILL_3 || skillSlotType == SkillSlotType.SLOT_SKILL_2)
				{
					MinimapSkillIndicator_3DUI.UpdateIndicator(skillSlotType, ref vector);
					MinimapSkillIndicator_3DUI.SetIndicatorColor(!this.m_currentSkillIndicatorInCancelArea);
				}
			}
			else if ((this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget || this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5) && this.m_currentSkillJoystickSelectedIndex != -1)
			{
				uint objID = 0u;
				if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
				{
					objID = this.m_targetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
				}
				else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
				{
					objID = this.m_EnemyTargetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
				}
				PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
				if (actor)
				{
					MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(actor);
				}
			}
		}

		public void SendUseCommonAttack(sbyte Start, uint uiRealObjID = 0u)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				if (player.Captain.handle.ActorControl.IsDeadState)
				{
					return;
				}
				FrameCommand<UseCommonAttackCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseCommonAttackCommand>();
				frameCommand.cmdData.Start = Start;
				frameCommand.cmdData.uiRealObjID = uiRealObjID;
				frameCommand.Send();
			}
		}

		public void ReadyUseSkillSlot(SkillSlotType skillSlotType, bool bForceSkillUseInDefaultPosition = false)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				player.Captain.handle.SkillControl.ReadyUseSkillSlot(skillSlotType, bForceSkillUseInDefaultPosition);
			}
		}

		public void OnBattleSkillDisableAlert(SkillSlotType skillSlotType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			SkillSlot skillSlot;
			if (player != null && player.Captain && player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot) && skillSlot.IsUnLock())
			{
				if (!skillSlot.IsCDReady)
				{
					skillSlot.SendSkillCooldownEvent();
				}
				else if (!skillSlot.IsSkillBeanEnough)
				{
					skillSlot.SendSkillBeanShortageEvent();
				}
				else if (!skillSlot.IsEnergyEnough)
				{
					skillSlot.SendSkillShortageEvent();
				}
			}
		}

		public void RequestUseSkillSlot(SkillSlotType skillSlotType, enSkillJoystickMode mode = enSkillJoystickMode.General, uint objID = 0u)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				player.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, mode, objID);
			}
		}

		public void CancelUseSkillSlot(SkillSlotType skillSlotType, enSkillJoystickMode mode = enSkillJoystickMode.General)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				player.Captain.handle.SkillControl.HostCancelUseSkillSlot(skillSlotType, mode);
			}
			if ((mode == enSkillJoystickMode.MapSelect || mode == enSkillJoystickMode.MapSelectOther) && Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Skill)
			{
				Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Mini);
			}
		}

		public bool IsUseSkillCursorJoystick(SkillSlotType skillSlotType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			return player != null && player.Captain && player.Captain.handle.SkillControl.IsUseSkillJoystick(skillSlotType);
		}

		public enSkillJoystickMode GetSkillJoystickMode(SkillSlotType skillSlotType)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				SkillSlot skillSlot = null;
				if (player.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot) && skillSlot != null)
				{
					Skill skill = (skillSlot.NextSkillObj != null) ? skillSlot.NextSkillObj : skillSlot.SkillObj;
					if (skill != null && skill.cfgData != null)
					{
						return (enSkillJoystickMode)skill.cfgData.bWheelType;
					}
				}
			}
			return enSkillJoystickMode.General;
		}

		public void EnableSkillCursor(CUIFormScript battleFormScript, ref Vector2 screenPosition, bool enableSkillCursorJoystick, SkillSlotType skillSlotType, SkillSlot useSlot, bool isSkillCanBeCancled)
		{
			this.m_currentSkillIndicatorEnabled = true;
			this.m_currentSkillIndicatorResponed = false;
			this.m_currentSkillTipsResponed = false;
			if (enableSkillCursorJoystick)
			{
				this.m_currentSkillIndicatorJoystickEnabled = true;
				if (battleFormScript != null)
				{
					if (this.m_currentSkillJoystickMode == enSkillJoystickMode.General)
					{
						this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
						this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget5);
						GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.General);
						if (skillCursor != null)
						{
							skillCursor.CustomSetActive(true);
							Vector3 skillIndicatorFixedPosition = this.GetButton(skillSlotType).m_skillIndicatorFixedPosition;
							if (this.m_skillIndicatorMode == enSkillIndicatorMode.General || skillIndicatorFixedPosition == Vector3.zero)
							{
								skillCursor.transform.position = CUIUtility.ScreenToWorldPoint(battleFormScript.GetCamera(), screenPosition, skillCursor.transform.position.z);
								this.m_currentSkillIndicatorScreenPosition = screenPosition;
							}
							else
							{
								skillCursor.transform.position = skillIndicatorFixedPosition;
								this.m_currentSkillIndicatorScreenPosition = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), skillIndicatorFixedPosition);
								if (skillSlotType == SkillSlotType.SLOT_SKILL_7)
								{
									this.m_currentSkillDownScreenPosition = this.m_currentSkillIndicatorScreenPosition;
									Vector2 vector = this.MoveSkillCursor(Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript(), skillCursor, skillSlotType, this.m_currentSkillJoystickMode, ref screenPosition, out this.m_currentSkillIndicatorInCancelArea, false);
									this.MoveSkillCursorInScene(skillSlotType, ref vector, this.m_currentSkillIndicatorInCancelArea, false);
								}
							}
						}
						this.ChangeSkillCursorBGSprite(battleFormScript, skillCursor, this.m_currentSkillIndicatorInCancelArea);
					}
					else if (CSkillButtonManager.IsSelectedTargetJoyStick(this.m_currentSkillJoystickMode))
					{
						this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
						if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
						{
							this.ResetSkillTargetJoyStickHeadToCampHero();
						}
						else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectNextSkillTarget || this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
						{
							this.ResetSkillTargetJoyStickHeadToTargets(useSlot.NextSkillTargetIDs);
						}
						GameObject skillCursor2;
						if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
						{
							skillCursor2 = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
						}
						else
						{
							skillCursor2 = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
						}
						if (skillCursor2 != null)
						{
							skillCursor2.CustomSetActive(true);
							skillCursor2.transform.position = this.GetButton(skillSlotType).m_button.transform.position;
							this.m_currentSkillIndicatorScreenPosition = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), skillCursor2.transform.position);
							CUIAnimationScript component = skillCursor2.GetComponent<CUIAnimationScript>();
							if (component != null)
							{
								if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
								{
									component.PlayAnimation("Head_In3", true);
								}
								else
								{
									component.PlayAnimation("Head_In2", true);
								}
							}
							this.ResetSkillJoystickSelectedTarget(battleFormScript);
							this.ChangeSkillCursorBGSprite(battleFormScript, skillCursor2, this.m_currentSkillIndicatorInCancelArea);
						}
					}
				}
			}
			if (this.m_currentSkillJoystickMode == enSkillJoystickMode.MapSelect)
			{
				this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
				this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
				this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget5);
				Player player = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (player != null && player.Captain && player.Captain.handle.SkillControl != null && player.Captain.handle.SkillControl.CanUseSkill(skillSlotType))
				{
					Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Skill, skillSlotType);
				}
			}
			else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.MapSelectOther)
			{
				this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
				this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
				this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget5);
				if (Singleton<TeleportTargetSelector>.GetInstance().m_ClickDownStatus)
				{
					Singleton<TeleportTargetSelector>.GetInstance().m_bConfirmed = true;
				}
				else
				{
					Player player2 = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
					if (player2 != null && player2.Captain && player2.Captain.handle.SkillControl != null && player2.Captain.handle.SkillControl.CanUseSkill(skillSlotType))
					{
						Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Skill, skillSlotType);
					}
				}
			}
			if (battleFormScript != null && GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
			{
				GameObject gameObject;
				if (skillSlotType != SkillSlotType.SLOT_SKILL_9)
				{
					gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCancleArea();
				}
				else
				{
					gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetEquipSkillCancleArea();
				}
				if (gameObject != null)
				{
					gameObject.CustomSetActive(isSkillCanBeCancled);
				}
			}
		}

		public void DisableSkillCursor(CUIFormScript battleFormScript, SkillSlotType skillSlotType)
		{
			this.m_currentSkillIndicatorEnabled = false;
			this.m_currentSkillIndicatorJoystickEnabled = false;
			this.m_currentSkillIndicatorResponed = false;
			this.m_currentSkillTipsResponed = false;
			this.m_currentSkillIndicatorInCancelArea = false;
			DebugHelper.Assert(battleFormScript != null);
			if (battleFormScript != null)
			{
				this.DisableSkillJoystick(battleFormScript, this.m_currentSkillJoystickMode);
				if (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
				{
					GameObject gameObject;
					if (skillSlotType == SkillSlotType.SLOT_SKILL_9)
					{
						gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetEquipSkillCancleArea();
					}
					else
					{
						gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCancleArea();
					}
					if (gameObject != null)
					{
						gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public Vector2 MoveSkillCursor(CUIFormScript battleFormScript, GameObject skillJoystick, SkillSlotType skillSlotType, enSkillJoystickMode skillJoystickMode, ref Vector2 screenPosition, out bool skillCanceled, bool isControlMove = true)
		{
			skillCanceled = this.IsSkillCursorInCanceledArea(battleFormScript, ref screenPosition, skillSlotType);
			if (!this.m_currentSkillIndicatorJoystickEnabled)
			{
				return Vector2.zero;
			}
			if (!this.m_currentSkillIndicatorResponed && battleFormScript.ChangeScreenValueToForm((screenPosition - this.m_currentSkillDownScreenPosition).magnitude) > 15f)
			{
				this.m_currentSkillIndicatorResponed = true;
			}
			if (!this.m_currentSkillTipsResponed && isControlMove && battleFormScript.ChangeScreenValueToForm((screenPosition - this.m_currentSkillDownScreenPosition).magnitude) > 30f)
			{
				this.m_currentSkillTipsResponed = true;
			}
			if (!this.m_currentSkillIndicatorResponed)
			{
				return Vector2.zero;
			}
			Vector2 vector = screenPosition - this.m_currentSkillIndicatorScreenPosition;
			Vector2 vector2 = vector;
			float magnitude = vector.magnitude;
			float num = battleFormScript.ChangeScreenValueToForm(magnitude);
			vector2.x = battleFormScript.ChangeScreenValueToForm(vector.x);
			vector2.y = battleFormScript.ChangeScreenValueToForm(vector.y);
			if (num > 110f)
			{
				vector2 = vector2.normalized * 110f;
			}
			if (skillJoystick != null)
			{
				Transform transform = skillJoystick.transform.FindChild("Cursor");
				if (transform != null)
				{
					(transform as RectTransform).anchoredPosition = vector2;
				}
			}
			if (skillJoystickMode == enSkillJoystickMode.General)
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (player != null && player.Captain && player.Captain.handle.SkillControl.SkillSlotArray[(int)skillSlotType].SkillObj.cfgData.bRangeAppointType == 3 && num < 30f)
				{
					return Vector2.zero;
				}
			}
			else if (CSkillButtonManager.IsSelectedTargetJoyStick(skillJoystickMode))
			{
				int selectedIndex = this.SkillJoystickSelectTarget(battleFormScript, skillJoystick, ref screenPosition);
				this.ChangeSkillJoystickSelectedTarget(battleFormScript, skillJoystick, selectedIndex);
			}
			return vector2 / 110f;
		}

		private void DisableSkillJoystick(CUIFormScript battleFormScript, enSkillJoystickMode skillJoystickMode)
		{
			if (battleFormScript == null)
			{
				return;
			}
			if (skillJoystickMode == enSkillJoystickMode.General)
			{
				GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.General);
				if (skillCursor != null && skillCursor.activeSelf)
				{
					skillCursor.CustomSetActive(false);
					RectTransform rectTransform = skillCursor.transform.FindChild("Cursor") as RectTransform;
					if (rectTransform != null)
					{
						rectTransform.anchoredPosition = Vector2.zero;
					}
				}
			}
			else if (CSkillButtonManager.IsSelectedTargetJoyStick(skillJoystickMode))
			{
				GameObject skillCursor2;
				if (skillJoystickMode == enSkillJoystickMode.SelectTarget5)
				{
					skillCursor2 = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
				}
				else
				{
					skillCursor2 = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
				}
				if (skillCursor2 != null && skillCursor2.activeSelf)
				{
					skillCursor2.CustomSetActive(false);
					RectTransform rectTransform2 = skillCursor2.transform.FindChild("Cursor") as RectTransform;
					if (rectTransform2 != null)
					{
						rectTransform2.anchoredPosition = Vector2.zero;
					}
				}
			}
		}

		private bool IsSkillCursorInCanceledArea(CUIFormScript battleFormScript, ref Vector2 screenPosition, SkillSlotType skillSlotType)
		{
			if (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
			{
				GameObject targetObj;
				if (skillSlotType != SkillSlotType.SLOT_SKILL_9)
				{
					targetObj = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCancleArea();
				}
				else
				{
					targetObj = Singleton<CBattleSystem>.GetInstance().FightForm.GetEquipSkillCancleArea();
				}
				return this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, targetObj);
			}
			return battleFormScript.ChangeScreenValueToForm((screenPosition - this.m_currentSkillDownScreenPosition).magnitude) > 270f;
		}

		public bool IsSkillCursorInTargetArea(CUIFormScript battleFormScript, ref Vector2 screenPosition, GameObject targetObj)
		{
			DebugHelper.Assert(battleFormScript != null, "battleFormScript!=null");
			if (battleFormScript != null)
			{
				DebugHelper.Assert(targetObj != null && targetObj.transform is RectTransform, "targetObj != null && targetObj.transform is RectTransform");
				if (targetObj != null && targetObj.activeInHierarchy && targetObj.transform is RectTransform)
				{
					Vector2 vector = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), targetObj.transform.position);
					float num = battleFormScript.ChangeFormValueToScreen((targetObj.transform as RectTransform).sizeDelta.x);
					float num2 = battleFormScript.ChangeFormValueToScreen((targetObj.transform as RectTransform).sizeDelta.y);
					Rect rect = new Rect(vector.x - num / 2f, vector.y - num2 / 2f, num, num2);
					return rect.Contains(screenPosition);
				}
			}
			return false;
		}

		private int SkillJoystickSelectTarget(CUIFormScript battleFormScript, GameObject skillJoystick, ref Vector2 screenPosition)
		{
			Vector2 point = screenPosition - this.m_currentSkillIndicatorScreenPosition;
			if (battleFormScript == null || battleFormScript.ChangeScreenValueToForm(point.magnitude) > 270f)
			{
				return -1;
			}
			float radian = CSkillButtonManager.GetRadian(point);
			if (battleFormScript != null && skillJoystick != null)
			{
				CUIComponent component = skillJoystick.GetComponent<CUIComponent>();
				if (component != null && component.m_widgets != null && component.m_widgets.Length >= this.m_targetInfos.Length)
				{
					stCampHeroInfo[] array = null;
					if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
					{
						array = this.m_targetInfos;
					}
					else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
					{
						array = this.m_EnemyTargetInfos;
					}
					if (array == null)
					{
						return -1;
					}
					for (int i = 0; i < array.Length; i++)
					{
						GameObject gameObject = component.m_widgets[i];
						if (gameObject != null && gameObject.activeSelf && (false || (radian >= array[i].m_headAreaFan.m_minRadian && radian <= array[i].m_headAreaFan.m_maxRadian)))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		private int GetCampHeroInfosIndexByObjId(uint uiObjId)
		{
			int result = -1;
			for (int i = 0; i < this.m_campHeroInfos.Length; i++)
			{
				if (this.m_campHeroInfos[i].m_objectID == uiObjId)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public void MoveSkillCursorInScene(SkillSlotType skillSlotType, ref Vector2 cursor, bool isSkillCursorInCancelArea, bool isControlMove = true)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				player.Captain.handle.SkillControl.SelectSkillTarget(skillSlotType, cursor, isSkillCursorInCancelArea, isControlMove);
			}
		}

		private void ChangeSkillCursorBGSprite(CUIFormScript battleFormScript, GameObject skillJoystick, bool isSkillCursorInCancelArea)
		{
			if (skillJoystick != null)
			{
				Image component = skillJoystick.GetComponent<Image>();
				if (component != null)
				{
					component.set_color(CSkillButtonManager.GetCursorBGColor(isSkillCursorInCancelArea));
				}
			}
		}

		private void ResetSkillJoystickSelectedTarget(CUIFormScript battleFormScript)
		{
			this.m_currentSkillJoystickSelectedIndex = -1;
			this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
			if (battleFormScript == null)
			{
				return;
			}
			bool flag = false;
			GameObject gameObject = null;
			if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
			{
				gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
			}
			else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget5)
			{
				flag = true;
				gameObject = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
			}
			if (gameObject != null)
			{
				CUIComponent component = gameObject.GetComponent<CUIComponent>();
				if (component != null && component.m_widgets != null)
				{
					int num = 0;
					while ((long)num < (long)((ulong)this.m_CurTargtNum))
					{
						this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[num], false);
						uint objID = flag ? this.m_EnemyTargetInfos[num].m_objectID : this.m_targetInfos[num].m_objectID;
						PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
						if (actor && actor.handle.ValueComponent != null)
						{
							float fHpRate = (float)actor.handle.ValueComponent.actorHp / (float)actor.handle.ValueComponent.actorHpTotal;
							this.SetJoystickHeroHpFill(component.m_widgets[num], fHpRate);
						}
						num++;
					}
				}
				Transform transform = gameObject.transform.FindChild("HighLight");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		private void ChangeSkillJoystickSelectedTarget(CUIFormScript battleFormScript, GameObject skillJoystick, int selectedIndex)
		{
			if (this.m_currentSkillJoystickSelectedIndex == selectedIndex)
			{
				return;
			}
			int currentSkillJoystickSelectedIndex = this.m_currentSkillJoystickSelectedIndex;
			this.m_currentSkillJoystickSelectedIndex = selectedIndex;
			this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
			if (battleFormScript == null || skillJoystick == null)
			{
				return;
			}
			CUIComponent component = skillJoystick.GetComponent<CUIComponent>();
			if (component != null && component.m_widgets != null)
			{
				if (this.m_currentSkillJoystickSelectedIndex >= 0 && (long)this.m_currentSkillJoystickSelectedIndex < (long)((ulong)this.m_CurTargtNum))
				{
					this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[this.m_currentSkillJoystickSelectedIndex], true);
					Transform transform = skillJoystick.transform.FindChild("HighLight");
					if (transform != null)
					{
						transform.gameObject.CustomSetActive(true);
						transform.eulerAngles = new Vector3(0f, 0f, (float)(45 * this.m_currentSkillJoystickSelectedIndex));
					}
				}
				else
				{
					Transform transform2 = skillJoystick.transform.FindChild("HighLight");
					if (transform2 != null)
					{
						transform2.gameObject.CustomSetActive(false);
					}
				}
				if (currentSkillJoystickSelectedIndex >= 0 && (long)currentSkillJoystickSelectedIndex < (long)((ulong)this.m_CurTargtNum))
				{
					this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[currentSkillJoystickSelectedIndex], false);
				}
			}
		}

		private void SetSkillJoystickTargetHead(CUIFormScript battleFormScript, GameObject head, bool selected)
		{
			if (head != null)
			{
				head.transform.localScale = new Vector3(selected ? 1.3f : 1f, selected ? 1.3f : 1f, selected ? 1.3f : 1f);
			}
		}

		private void SetSkillIndicatorSelectedTarget(int index)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				stCampHeroInfo[] array;
				if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
				{
					array = this.m_targetInfos;
				}
				else
				{
					if (this.m_currentSkillJoystickMode != enSkillJoystickMode.SelectTarget5)
					{
						return;
					}
					array = this.m_EnemyTargetInfos;
				}
				SkillSlot skillSlot = player.Captain.handle.SkillControl.GetSkillSlot(this.m_currentSkillSlotType);
				if (skillSlot != null && skillSlot.skillIndicator != null)
				{
					if (index < 0 || index >= array.Length || array[index].m_objectID == 0u)
					{
						skillSlot.skillIndicator.SetUseSkillTarget(null);
					}
					else
					{
						PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(array[this.m_currentSkillJoystickSelectedIndex].m_objectID);
						if (actor)
						{
							skillSlot.skillIndicator.SetUseSkillTarget(actor.handle);
						}
						else
						{
							skillSlot.skillIndicator.SetUseSkillTarget(null);
						}
					}
				}
			}
		}

		private void SetSkillBtnDragFlag(SkillSlotType slotType, bool bDrag)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (player != null && player.Captain)
			{
				SkillSlot skillSlot = player.Captain.handle.SkillControl.GetSkillSlot(slotType);
				if (skillSlot != null && skillSlot.skillIndicator != null)
				{
					skillSlot.skillIndicator.SetSkillBtnDrag(bDrag);
				}
			}
		}

		public void ChangeSkill(SkillSlotType skillSlotType, ref ChangeSkillEventParam skillParam)
		{
			if (skillSlotType > SkillSlotType.SLOT_SKILL_0 && skillParam.skillID > 0)
			{
				SkillButton skillButton = this._skillButtons[(int)skillSlotType];
				if (skillButton != null)
				{
					skillButton.ChangeSkillIcon(skillParam.skillID);
				}
				if (skillParam.bIsUseCombo)
				{
					this.SetComboEffect(skillSlotType, skillParam.changeTime, skillParam.changeTime);
				}
			}
		}

		public void RecoverSkill(SkillSlotType skillSlotType, ref DefaultSkillEventParam skillParam)
		{
			if (skillSlotType > SkillSlotType.SLOT_SKILL_0 && skillParam.param > 0)
			{
				SkillButton skillButton = this._skillButtons[(int)skillSlotType];
				if (skillButton != null)
				{
					skillButton.ChangeSkillIcon(skillParam.param);
				}
				this.SetComboEffect(skillSlotType, 0, 0);
			}
		}

		private void SetComboEffect(SkillSlotType skillSlotType, int leftTime, int totalTime)
		{
			SkillButton button = this.GetButton(skillSlotType);
			if (button == null || null == button.m_button)
			{
				return;
			}
			button.effectTimeTotal = totalTime;
			button.effectTimeLeft = leftTime;
			GameObject gameObject = Utility.FindChildSafe(button.m_button, "Present/comboCD");
			if (gameObject)
			{
				if (button.effectTimeLeft > 0 && button.effectTimeTotal > 0)
				{
					gameObject.CustomSetActive(true);
					button.effectTimeImage = gameObject.GetComponent<Image>();
				}
				else
				{
					gameObject.CustomSetActive(false);
					button.effectTimeImage = null;
				}
			}
		}

		public void SetButtonCDStart(SkillSlotType skillSlotType)
		{
			if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
			{
				return;
			}
			this.SetDisableButton(skillSlotType);
			SkillButton button = this.GetButton(skillSlotType);
			GameObject target = (button != null) ? button.GetAnimationCD() : null;
			CUICommonSystem.PlayAnimation(target, enSkillButtonAnimationName.CD_Star.ToString(), false);
		}

		public void SetButtonCDOver(SkillSlotType skillSlotType, bool isPlayMusic = true)
		{
			if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
			{
				return;
			}
			if (!this.SetEnableButton(skillSlotType))
			{
				return;
			}
			SkillButton button = this.GetButton(skillSlotType);
			GameObject target = (button != null) ? button.GetAnimationCD() : null;
			CUICommonSystem.PlayAnimation(target, enSkillButtonAnimationName.CD_End.ToString(), false);
			if (isPlayMusic)
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("UI_prompt_jineng");
			}
		}

		public void UpdateButtonBeanNum(SkillSlotType skillSlotType, int value)
		{
			SkillButton button = this.GetButton(skillSlotType);
			if (value > 0)
			{
				this.SetEnableButton(skillSlotType);
			}
			else
			{
				this.SetEnergyDisableButton(skillSlotType);
			}
			if (button.m_beanText != null)
			{
				Text component = button.m_beanText.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(SimpleNumericString.GetNumeric(value));
				}
				button.m_beanText.CustomSetActive(true);
			}
		}

		public void UpdateButtonCD(SkillSlotType skillSlotType, int cd)
		{
			SkillButton button = this.GetButton(skillSlotType);
			if (cd <= 0)
			{
				this.SetEnableButton(skillSlotType);
			}
			this.UpdateButtonCDText((button != null) ? button.m_button : null, (button != null) ? button.m_cdText : null, cd);
		}

		private void UpdateButtonCDText(GameObject button, GameObject cdText, int cd)
		{
			if (cdText != null)
			{
				if (cd <= 0)
				{
					cdText.CustomSetActive(false);
				}
				else
				{
					if (button && button.activeSelf)
					{
						cdText.CustomSetActive(true);
					}
					Text component = cdText.GetComponent<Text>();
					if (component != null)
					{
						component.set_text(SimpleNumericString.GetNumeric(Mathf.CeilToInt((float)(cd / 1000)) + 1));
					}
				}
			}
			if (button != null && cdText != null)
			{
				cdText.transform.position = button.transform.position;
			}
		}

		public void SetButtonHighLight(SkillSlotType skillSlotType, bool highLight)
		{
			SkillButton button = this.GetButton(skillSlotType);
			if (button != null && button.m_button != null)
			{
				this.SetButtonHighLight(button.m_button, highLight);
			}
		}

		public void SetButtonHighLight(GameObject button, bool highLight)
		{
			Transform transform = button.transform.FindChild("Present/highlighter");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(highLight);
			}
		}

		public void SetlearnBtnHighLight(GameObject learnBtn, bool highLight)
		{
			Transform transform = learnBtn.transform.FindChild("highlighter");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(highLight);
			}
		}

		public void SetButtonFlowLight(GameObject button, bool highLight)
		{
			Transform transform = button.transform.FindChild("Present/highlighter");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(highLight);
			}
		}

		public SkillSlotType GetCurSkillSlotType()
		{
			return this.m_currentSkillSlotType;
		}

		public void SetSkillIndicatorMode(enSkillIndicatorMode indicaMode)
		{
			this.m_skillIndicatorMode = indicaMode;
		}

		public void UpdateLogic(int delta)
		{
			for (int i = 0; i < this._skillButtons.Length; i++)
			{
				SkillButton skillButton = this._skillButtons[i];
				if (skillButton != null && null != skillButton.effectTimeImage)
				{
					skillButton.effectTimeLeft -= delta;
					if (skillButton.effectTimeLeft < 0)
					{
						skillButton.effectTimeLeft = 0;
					}
					skillButton.effectTimeImage.CustomFillAmount((float)skillButton.effectTimeLeft / (float)skillButton.effectTimeTotal);
					if (skillButton.effectTimeLeft <= 0)
					{
						skillButton.effectTimeTotal = 0;
						skillButton.effectTimeImage.gameObject.CustomSetActive(false);
						skillButton.effectTimeImage = null;
					}
				}
			}
		}

		public bool IsIndicatorInCancelArea()
		{
			return this.m_currentSkillIndicatorInCancelArea;
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			if (captain == prm.src && (!captain.handle.TheStaticData.TheBaseAttribute.DeadControl || captain.handle.ActorControl.IsEnableReviveContext()))
			{
				for (int i = 0; i < 10; i++)
				{
					this.SetDisableButton((SkillSlotType)i);
				}
			}
		}

		private void onActorRevive(ref DefaultGameEventParam prm)
		{
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			if (captain == prm.src && (!captain.handle.TheStaticData.TheBaseAttribute.DeadControl || captain.handle.ActorControl.IsEnableReviveContext()))
			{
				for (int i = 0; i < 10; i++)
				{
					this.SetEnableButton((SkillSlotType)i);
				}
			}
		}

		private void OnCaptainSwitched(ref DefaultGameEventParam prm)
		{
			Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(prm.src, false, SkillSlotType.SLOT_SKILL_COUNT);
			this.ResetPickHeroInfo(prm.src);
		}

		private void ResetPickHeroInfo(PoolObjHandle<ActorRoot> actor)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (player == null || curLvelContext == null)
			{
				return;
			}
			if (!curLvelContext.IsMobaMode())
			{
				ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = player.GetAllHeroes();
				int count = allHeroes.Count;
				int num = 0;
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						if (!(allHeroes[i] == actor))
						{
							this.m_campHeroInfos[num].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint)allHeroes[i].handle.TheActorMeta.ConfigId, 0u);
							this.m_campHeroInfos[num].m_objectID = allHeroes[i].handle.ObjID;
							num++;
						}
					}
				}
			}
			this.m_currentSkillJoystickSelectedIndex = -1;
			this.ResetSkillTargetJoyStickHeadToCampHero();
		}

		public void SetCommonAtkBtnState(CommonAttactType byAtkType)
		{
			CUIFormScript cUIFormScript = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(24);
			GameObject widget2 = cUIFormScript.GetWidget(9);
			if (widget == null || widget2 == null)
			{
				return;
			}
			CUIEventScript component = widget2.GetComponent<CUIEventScript>();
			if (component == null)
			{
				return;
			}
			if (byAtkType == CommonAttactType.Type1)
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
			}
			else if (byAtkType == CommonAttactType.Type2)
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(true);
				bool selectTargetBtnState = false;
				SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
				if (button != null)
				{
					GameObject disableButton = button.GetDisableButton();
					if (disableButton != null)
					{
						selectTargetBtnState = disableButton.activeSelf;
					}
				}
				this.SetSelectTargetBtnState(selectTargetBtnState);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("CommonAttack_Type_Changed");
		}

		private void SetSelectTargetBtnState(bool bActive)
		{
			if (GameSettings.TheCommonAttackType != CommonAttactType.Type2)
			{
				return;
			}
			CUIFormScript cUIFormScript = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(24);
			GameObject widget2 = cUIFormScript.GetWidget(9);
			if (widget == null || widget2 == null)
			{
				return;
			}
			Color color = CUIUtility.s_Color_Full;
			if (bActive)
			{
				color = CUIUtility.s_Color_DisableGray;
			}
			GameObject gameObject = widget2.transform.FindChild("disable").gameObject;
			if (gameObject)
			{
				gameObject.CustomSetActive(bActive);
			}
			GameObject gameObject2 = widget2.transform.FindChild("Present").gameObject;
			if (gameObject2 != null)
			{
				Image component = gameObject2.GetComponent<Image>();
				if (component != null)
				{
					component.set_color(color);
				}
			}
			gameObject = widget.transform.FindChild("disable").gameObject;
			if (gameObject)
			{
				gameObject.CustomSetActive(bActive);
			}
			gameObject2 = widget.transform.FindChild("Present").gameObject;
			if (gameObject2 != null)
			{
				Image component2 = gameObject2.GetComponent<Image>();
				if (component2 != null)
				{
					component2.set_color(color);
				}
			}
		}

		private void EnableSpecialAttackBtn(bool _bEnable, enSkillButtonFormWidget enSkillbtn)
		{
			CUIFormScript skillBtnFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
			if (skillBtnFormScript == null)
			{
				return;
			}
			GameObject widget = skillBtnFormScript.GetWidget((int)enSkillbtn);
			if (widget == null)
			{
				return;
			}
			Color color = _bEnable ? CUIUtility.s_Color_Full : CUIUtility.s_Color_DisableGray;
			GameObject gameObject = Utility.FindChild(widget, "disable");
			if (gameObject != null)
			{
				gameObject.CustomSetActive(!_bEnable);
			}
			GameObject gameObject2 = Utility.FindChild(widget, "Present");
			if (gameObject2 != null)
			{
				Image component = gameObject2.GetComponent<Image>();
				if (component != null)
				{
					component.set_color(color);
				}
			}
		}

		public bool HasMapSlectTargetSkill(out SkillSlotType slotType)
		{
			for (int i = 0; i < (int)CSkillButtonManager.s_maxButtonCount; i++)
			{
				SkillSlotType skillSlotType = (SkillSlotType)i;
				enSkillJoystickMode skillJoystickMode = this.GetSkillJoystickMode(skillSlotType);
				if (skillJoystickMode == enSkillJoystickMode.MapSelect || skillJoystickMode == enSkillJoystickMode.MapSelectOther)
				{
					slotType = skillSlotType;
					return true;
				}
			}
			slotType = SkillSlotType.SLOT_SKILL_COUNT;
			return false;
		}

		private void SetJoystickHeroHpFill(GameObject head, float fHpRate)
		{
			if (head != null)
			{
				CUIComponent component = head.GetComponent<CUIComponent>();
				if (component != null && component.m_widgets != null && component.m_widgets.Length >= 2)
				{
					GameObject gameObject = component.m_widgets[0];
					GameObject gameObject2 = component.m_widgets[1];
					if (gameObject2 != null && gameObject != null)
					{
						Image component2 = gameObject.GetComponent<Image>();
						Image component3 = gameObject2.GetComponent<Image>();
						if (component3 != null)
						{
							float fillAmount = component3.get_fillAmount();
							if ((double)fHpRate < 0.3 && (double)fillAmount >= 0.3)
							{
								component3.SetSprite("UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Red_ring", Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
								if (component2)
								{
									component2.set_color(CUIUtility.s_Color_EnemyHero_Button_PINK);
								}
							}
							else if ((double)fHpRate >= 0.3 && (double)fillAmount < 0.3)
							{
								component3.SetSprite("UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Blue_ring", Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
								if (component2)
								{
									component2.set_color(CUIUtility.s_Color_White);
								}
							}
							else if (fHpRate <= 0f && fillAmount > 0f && component2)
							{
								component2.set_color(CUIUtility.s_Color_DisableGray);
							}
							component3.CustomFillAmount(fHpRate);
						}
					}
				}
			}
		}

		private void SetJoystickHeroHpFill(PoolObjHandle<ActorRoot> hero, float fHpRate)
		{
			if (!hero)
			{
				return;
			}
			stCampHeroInfo[] array;
			GameObject skillCursor;
			if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
			{
				array = this.m_targetInfos;
				skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
			}
			else
			{
				if (this.m_currentSkillJoystickMode != enSkillJoystickMode.SelectTarget5)
				{
					return;
				}
				array = this.m_EnemyTargetInfos;
				skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget5);
			}
			if (skillCursor != null)
			{
				CUIComponent component = skillCursor.GetComponent<CUIComponent>();
				if (component != null && component.m_widgets != null && component.m_widgets.Length >= array.Length)
				{
					for (int i = 0; i < component.m_widgets.Length; i++)
					{
						if (i >= array.Length)
						{
							break;
						}
						if (array[i].m_objectID == hero.handle.ObjID)
						{
							this.SetJoystickHeroHpFill(component.m_widgets[i], fHpRate);
							break;
						}
					}
				}
			}
		}

		private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
		{
			if (!this.m_currentSkillIndicatorJoystickEnabled)
			{
				return;
			}
			if (!CSkillButtonManager.IsSelectedTargetJoyStick(this.m_currentSkillJoystickMode))
			{
				return;
			}
			if (!hero || !ActorHelper.IsCaptainActor(ref hero))
			{
				return;
			}
			float fHpRate = (float)iCurVal / (float)iMaxVal;
			this.SetJoystickHeroHpFill(hero, fHpRate);
		}

		private static bool IsSelectedTargetJoyStick(enSkillJoystickMode mode)
		{
			return mode == enSkillJoystickMode.SelectTarget || mode == enSkillJoystickMode.SelectNextSkillTarget || mode == enSkillJoystickMode.SelectTarget5;
		}

		public void SetSkillButtuonActive(SkillSlotType skillSlotType, bool active)
		{
			SkillButton skillButton = this._skillButtons[(int)skillSlotType];
			if (skillButton != null)
			{
				skillButton.m_button.CustomSetActive(active);
				skillButton.m_cdText.GetComponent<Text>().enabled = active;
			}
		}

		public void SetSkillButtuonSelectActive(SkillSlotType skillSlotType, bool active)
		{
			SkillButton skillButton = this._skillButtons[(int)skillSlotType];
			if (skillButton != null)
			{
				Transform transform = skillButton.m_button.transform.FindChild("select");
				if (transform)
				{
					transform.gameObject.CustomSetActive(active);
				}
			}
		}

		public void LateUpdate()
		{
			if (GameSettings.TheAttackOrganMode == AttackOrganMode.AttackOrgan)
			{
				bool bActive = this.IsEnemyOrganInSearchScope();
				this.attackOrganBtnEffect.CustomSetActive(bActive);
			}
		}

		private bool IsEnemyOrganInSearchScope()
		{
			if (this.hostPlayer == null)
			{
				this.hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			}
			if (this.hostPlayer != null)
			{
				PoolObjHandle<ActorRoot> captain = this.hostPlayer.Captain;
				if (!captain || captain.handle.ActorControl.IsDeadState)
				{
					return false;
				}
				int searchRange = captain.handle.ActorControl.SearchRange;
				ulong num = (ulong)((long)searchRange * (long)searchRange);
				List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
				int count = organActors.get_Count();
				for (int i = 0; i < count; i++)
				{
					ActorRoot handle = organActors.get_Item(i).handle;
					if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && captain.handle.CanAttack(handle))
					{
						ulong sqrMagnitudeLong2D = (ulong)(handle.location - captain.handle.location).sqrMagnitudeLong2D;
						if (sqrMagnitudeLong2D < num)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
