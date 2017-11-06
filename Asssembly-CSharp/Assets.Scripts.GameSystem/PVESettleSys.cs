using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class PVESettleSys : Singleton<PVESettleSys>
	{
		public static readonly string PATH_STAR = "UGUI/Form/System/PvE/Settle/Form_PVEWinSettlement.prefab";

		public static readonly string PATH_EXP = "UGUI/Form/System/PvE/Settle/Form_PVEExpSettlement.prefab";

		public static readonly string PATH_ITEM = "UGUI/Form/System/PvE/Settle/Form_PVEAward.prefab";

		public static readonly string PATH_LOSE = "UGUI/Form/System/PvE/Adv/Form_AdventureLose.prefab";

		public static readonly string PATH_LEVELUP = "UGUI/Form/System/PvE/Settle/Form_PlayerLevelUp.prefab";

		private StarCondition[] m_WinConditions = new StarCondition[3];

		private COMDT_SETTLE_RESULT_DETAIL m_SettleData;

		private int _lastlvTarget = 1;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_EscapeAnim, new CUIEventManager.OnUIEventHandler(this.onEscapeAnim));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_ShowExpForm, new CUIEventManager.OnUIEventHandler(this.ShowExpForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_ShowRewardForm, new CUIEventManager.OnUIEventHandler(this.ShowRewardForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_BattleAgain, new CUIEventManager.OnUIEventHandler(this.BattleAgain));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_NextLevel, new CUIEventManager.OnUIEventHandler(this.GotoNextLevel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_BackToLobby, new CUIEventManager.OnUIEventHandler(this.BackToLobby));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_CloseLvlUp, new CUIEventManager.OnUIEventHandler(this.OnCloseLvlUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_OpenLvlUp, new CUIEventManager.OnUIEventHandler(this.OnOpenLvlUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_Back, new CUIEventManager.OnUIEventHandler(this.OnCloseLoseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_OnGameEnd, new CUIEventManager.OnUIEventHandler(this.OnGameEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_AnimEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_ClickItemDetailEnd, new CUIEventManager.OnUIEventHandler(this.OnClickItemDetailEnd));
			base.Init();
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_EscapeAnim, new CUIEventManager.OnUIEventHandler(this.onEscapeAnim));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_ShowExpForm, new CUIEventManager.OnUIEventHandler(this.ShowExpForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_ShowRewardForm, new CUIEventManager.OnUIEventHandler(this.ShowRewardForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_BattleAgain, new CUIEventManager.OnUIEventHandler(this.BattleAgain));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_NextLevel, new CUIEventManager.OnUIEventHandler(this.GotoNextLevel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_BackToLobby, new CUIEventManager.OnUIEventHandler(this.BackToLobby));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_CloseLvlUp, new CUIEventManager.OnUIEventHandler(this.OnCloseLvlUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_OpenLvlUp, new CUIEventManager.OnUIEventHandler(this.OnOpenLvlUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_Back, new CUIEventManager.OnUIEventHandler(this.OnCloseLoseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_OnGameEnd, new CUIEventManager.OnUIEventHandler(this.OnGameEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_AnimEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_ClickItemDetailEnd, new CUIEventManager.OnUIEventHandler(this.OnClickItemDetailEnd));
			base.UnInit();
		}

		private void onEscapeAnim(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_STAR);
			if (form != null)
			{
				PVESettleView.StopStarAnim(form);
			}
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_EXP);
			if (form2 != null)
			{
				PVESettleView.StopExpAnim(form2);
			}
			CUIFormScript form3 = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_ITEM);
			if (form3 != null)
			{
				PVESettleView.StopRewardAnim(form3);
			}
		}

		private void ShowExpForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(PVESettleSys.PATH_STAR);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PVESettleSys.PATH_EXP, false, true);
			PVESettleView.SetExpFormData(form, this.m_SettleData);
		}

		private void ShowRewardForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(PVESettleSys.PATH_STAR);
			if (this.m_SettleData != null)
			{
				if (this.m_SettleData.stReward != null)
				{
					COMDT_REWARD_DETAIL stReward = this.m_SettleData.stReward;
					ListView<COMDT_REWARD_INFO> listView = new ListView<COMDT_REWARD_INFO>();
					for (int i = 0; i < (int)stReward.bNum; i++)
					{
						COMDT_REWARD_INFO cOMDT_REWARD_INFO = stReward.astRewardDetail[i];
						byte bType = cOMDT_REWARD_INFO.bType;
						if (bType == 6)
						{
							listView.Add(cOMDT_REWARD_INFO);
						}
					}
					if (listView.Count > 0 && listView[0].bType == 6)
					{
						CSymbolItem useable = (CSymbolItem)CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0uL, listView[0].stRewardInfo.stSymbol.dwSymbolID, (int)listView[0].stRewardInfo.stSymbol.dwCnt, 0);
						CUseableContainer cUseableContainer = new CUseableContainer(enCONTAINER_TYPE.ITEM);
						cUseableContainer.Add(useable);
						CUICommonSystem.ShowSymbol(cUseableContainer, enUIEventID.Settle_ClickItemDetailEnd);
						MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.getSymbolReward, new uint[0]);
						return;
					}
				}
				this.ShowPveExp();
			}
		}

		private void OnGameEnd(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript != null)
			{
				srcFormScript.transform.Find("ClickBack").gameObject.CustomSetActive(true);
				srcFormScript.transform.Find("WaitNote").gameObject.CustomSetActive(false);
			}
			Singleton<GameBuilder>.instance.EndGame();
		}

		private void OnCloseLoseForm(CUIEvent uiEvent)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			Singleton<CUIManager>.GetInstance().CloseForm(PVESettleSys.PATH_LOSE);
			if (!Singleton<CBattleGuideManager>.instance.bTrainingAdv && curLvelContext != null && curLvelContext.IsGameTypeAdventure())
			{
				Singleton<CAdventureSys>.instance.OpenAdvForm(curLvelContext.m_chapterNo, (int)curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
			}
		}

		private void OnAnimEnd(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_STAR);
			if (form != null)
			{
				if (uiEvent.m_eventParams.tagStr == "Win_Show")
				{
					PVESettleView.OnStarWinAnimEnd(form, ref this.m_WinConditions);
				}
				else if (!uiEvent.m_eventParams.tagStr.Contains("Done"))
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settle_EscapeAnim);
				}
			}
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_ITEM);
			if (form2 != null && uiEvent.m_eventParams.tagStr == "Box_Show_2")
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settle_EscapeAnim);
			}
		}

		private void BattleAgain(CUIEvent uiEvent)
		{
			this.CloseItemForm();
			if (!Singleton<CBattleGuideManager>.instance.bTrainingAdv)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.IsGameTypeAdventure())
				{
					CUIEvent cUIEvent = new CUIEvent();
					Singleton<CAdventureSys>.instance.OpenAdvForm(curLvelContext.m_chapterNo, (int)curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
					cUIEvent.m_eventID = enUIEventID.Adv_OpenLevelForm;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				}
			}
			else
			{
				CMatchingSystem.ReqStartTrainingLevel();
			}
		}

		private void GotoNextLevel(CUIEvent uiEvent)
		{
			this.CloseItemForm();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsGameTypeAdventure())
			{
				int nextLevelId = CAdventureSys.GetNextLevelId(curLvelContext.m_chapterNo, (int)curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
				if (nextLevelId != 0)
				{
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.Adv_OpenLevelForm;
					cUIEvent.m_eventParams.tag = nextLevelId;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				}
			}
			else if (curLvelContext != null && curLvelContext.IsGameTypeActivity())
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Explore_OpenForm);
			}
		}

		private void BackToLobby(CUIEvent uiEvent)
		{
			this.CloseItemForm();
			if (!Singleton<CBattleGuideManager>.instance.bTrainingAdv)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.IsGameTypeAdventure())
				{
					Singleton<CAdventureSys>.instance.OpenAdvForm(Singleton<CAdventureSys>.instance.currentChapter, Singleton<CAdventureSys>.instance.currentLevelSeq, Singleton<CAdventureSys>.instance.currentDifficulty);
				}
			}
		}

		private void OnCloseLvlUp(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(PVESettleSys.PATH_LEVELUP);
		}

		private void OnOpenLvlUp(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			int tag2 = uiEvent.m_eventParams.tag2;
			if (tag2 > this._lastlvTarget)
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(PVESettleSys.PATH_LEVELUP, false, true);
				if (cUIFormScript != null)
				{
					PVESettleView.ShowPlayerLevelUp(cUIFormScript, tag, tag2);
					uint key = 0u;
					if (CAddSkillView.NewPlayerLevelUnlockAddSkill(tag2, tag, out key))
					{
						Transform transform = cUIFormScript.transform.FindChild("PlayerLvlUp/Panel/groupPanel/SkillPanel");
						if (transform != null)
						{
							ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(key);
							if (dataByKey != null)
							{
								transform.gameObject.CustomSetActive(true);
								Text component = transform.FindChild("Skill/SkillName").GetComponent<Text>();
								if (component != null)
								{
									component.set_text(Utility.UTF8Convert(dataByKey.szSkillName));
								}
								Image component2 = transform.FindChild("Skill/Icon").GetComponent<Image>();
								if (component2 != null)
								{
									string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
									component2.SetSprite(prefabPath, cUIFormScript, true, false, false, false);
								}
							}
						}
					}
					else
					{
						Transform transform2 = cUIFormScript.transform.FindChild("PlayerLvlUp/Panel/groupPanel/SkillPanel");
						if (transform2 != null)
						{
							transform2.gameObject.CustomSetActive(false);
						}
					}
				}
				this._lastlvTarget = tag2;
			}
		}

		private void CloseItemForm()
		{
			Singleton<CShopSystem>.GetInstance().OpenMysteryShopActiveTip();
			PVESettleView.DoCoinTweenEnd();
			Singleton<CUIManager>.GetInstance().CloseForm(PVESettleSys.PATH_ITEM);
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
			Singleton<GameBuilder>.instance.EndGame();
		}

		public void StartSettle(COMDT_SETTLE_RESULT_DETAIL settleData = null, bool bFirstPass = true)
		{
			if (settleData == null || settleData.stGameInfo.bGameResult == 2)
			{
				this.openFormLose();
			}
			else if (settleData.stGameInfo.bGameResult == 1)
			{
				this.m_SettleData = settleData;
				CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PVESettleSys.PATH_STAR, false, true);
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_Victor", null);
				this.calcStarConditions();
				PVESettleView.SetStarFormData(form, settleData, ref this.m_WinConditions);
			}
		}

		public void OnStarWinAnimEnd()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_STAR);
			if (form != null)
			{
				PVESettleView.OnStarWinAnimEnd(form, ref this.m_WinConditions);
			}
		}

		public StarCondition[] GetCondition()
		{
			return this.m_WinConditions;
		}

		private void openFormLose()
		{
			Singleton<CUIManager>.GetInstance().OpenForm(PVESettleSys.PATH_LOSE, false, true);
			Singleton<CSoundManager>.GetInstance().PostEvent("Set_Defeat", null);
		}

		private void calcStarConditions()
		{
			ListView<IStarEvaluation>.Enumerator enumerator = Singleton<StarSystem>.GetInstance().GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				this.m_WinConditions[num].ConditionName = enumerator.Current.description;
				this.m_WinConditions[num].bCompelete = enumerator.Current.isSuccess;
				num++;
			}
		}

		private void OnClickItemDetailEnd(CUIEvent uiEvent)
		{
			this.ShowPveExp();
		}

		protected void ShowPveExp()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PVESettleSys.PATH_ITEM, false, true);
			PVESettleView.SetRewardFormData(form, this.m_SettleData);
			if (this.m_SettleData != null)
			{
				uint iLevelID = (uint)this.m_SettleData.stGameInfo.iLevelID;
				uint bGameResult = (uint)this.m_SettleData.stGameInfo.bGameResult;
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.battleFin, new uint[]
				{
					iLevelID,
					bGameResult,
					1u,
					1u
				});
			}
		}

		public void OnAwardDisplayEnd()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_ITEM);
			if (form != null)
			{
			}
		}

		protected void CheckLevelUp()
		{
			if (this.m_SettleData.stAcntInfo.dwPvpExp < this.m_SettleData.stAcntInfo.dwPvpSettleExp)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = (int)(this.m_SettleData.stAcntInfo.dwPvpLv - 1u);
				cUIEvent.m_eventParams.tag2 = (int)this.m_SettleData.stAcntInfo.dwPvpLv;
				CUIEvent uiEvent = cUIEvent;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
		}
	}
}
