using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
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
	[MessageHandlerClass]
	internal class CAdventureSys : Singleton<CAdventureSys>
	{
		private const string s_str_chapter = "Sgame_Chapter";

		private const string s_str_difficulty = "Sgame_Difficulty";

		private const string s_str_Level = "Sgame_Level";

		public static string ADVENTURE_SELECT_FORM = "UGUI/Form/System/PvE/Adv/Form_Adv_Select.prefab";

		public static string ADVENTURE_LEVEL_FORM = "UGUI/Form/System/PvE/Adv/Form_Adv_Level.prefab";

		public static string ADVENTURE_MOB_FORM = "UGUI/Form/System/PvE/Adv/Form_Mopup.prefab";

		public static string EXLPORE_FORM_PATH = "UGUI/Form/System/PvE/Adv/Form_Explore_Select.prefab";

		public static int CHAPTER_NUM = 10;

		public static int LEVEL_PER_CHAPTER = 4;

		public static int STAR_PER_LEVEL = 3;

		public static uint MOPUP_TICKET_ID;

		public static uint MOPUP_TICKET_NUM_PER_LEVEL = 1u;

		public static uint MOPUP_TICKET_PRICE_BY_DIAMOND = 1u;

		public static uint CHALLENGE_BUYTIME_LIMIT = 1u;

		public static int LEVEL_DIFFICULT_OPENED = 4;

		public int currentDifficulty;

		public int currentChapter;

		public int currentLevelId;

		public int currentLevelSeq;

		private byte[] m_chapterStatus;

		private byte[] m_difficultyStatus;

		public byte bNewChapterId;

		public override void Init()
		{
			base.Init();
			CAdventureSys.CHAPTER_NUM = GameDataMgr.chapterInfoDatabin.Count();
			CAdventureSys.MOPUP_TICKET_ID = GameDataMgr.globalInfoDatabin.GetDataByKey(33u).dwConfValue;
			CAdventureSys.MOPUP_TICKET_NUM_PER_LEVEL = GameDataMgr.globalInfoDatabin.GetDataByKey(34u).dwConfValue;
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(CAdventureSys.MOPUP_TICKET_ID);
			DebugHelper.Assert(dataByKey != null, "Can't find Mopup ticket config -- ID: {0}", new object[]
			{
				CAdventureSys.MOPUP_TICKET_ID
			});
			CAdventureSys.MOPUP_TICKET_PRICE_BY_DIAMOND = dataByKey.dwCouponsBuy;
			CAdventureSys.CHALLENGE_BUYTIME_LIMIT = GameDataMgr.globalInfoDatabin.GetDataByKey(42u).dwConfValue;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_CloseChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectLevel, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectLevel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectDifficult, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectDifficult));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectChapter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenLevelForm, new CUIEventManager.OnUIEventHandler(this.OnLevelDetail_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectPreChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectPreChapter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectNextChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectNextChapter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenChooseHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChooseHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ChooseHeroReady, new CUIEventManager.OnUIEventHandler(this.OnChooseHeroReady));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_Mopup, new CUIEventManager.OnUIEventHandler(this.OnMopup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_MopupTenTimes, new CUIEventManager.OnUIEventHandler(this.OnMopupTenTimes));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmDiamondMopup, new CUIEventManager.OnUIEventHandler(this.OnConfirmDiamondMopup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.OpenChapterRewardPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_CloseChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.CloseChapterRewardPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_GetChapterReward, new CUIEventManager.OnUIEventHandler(this.GetChapterReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_BuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnBuyPlayTime));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmBuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyPlayTime));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmItemFullMopup, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullMopup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmItemFullAdv, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullAdv));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_CloseSettleForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSettleForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmTeamPowerCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamPower));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmTeamNumCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamNum));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Explore_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenExplore));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ExploreScroll, new CUIEventManager.OnUIEventHandler(this.OnExploreListScroll));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ExploreSelect, new CUIEventManager.OnUIEventHandler(this.OnExploreSelect));
			Singleton<EventRouter>.instance.AddEventHandler<CSPkg>("ShopBuyLvlChallengeTime", new Action<CSPkg>(this.OnBuyPlayTimeRsp));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.SINGLEGAME_ERR_FREEHERO, new Action(this.OnFreeHeroChanged));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(CAdventureSys.ResetElitePlayNum));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_OpenForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_CloseChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_CloseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectLevel, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectLevel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectDifficult, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectDifficult));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectChapter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectPreChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectPreChapter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectNextChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectNextChapter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenLevelForm, new CUIEventManager.OnUIEventHandler(this.OnLevelDetail_OpenForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenChooseHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChooseHero));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ChooseHeroReady, new CUIEventManager.OnUIEventHandler(this.OnChooseHeroReady));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_Mopup, new CUIEventManager.OnUIEventHandler(this.OnMopup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_MopupTenTimes, new CUIEventManager.OnUIEventHandler(this.OnMopupTenTimes));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmDiamondMopup, new CUIEventManager.OnUIEventHandler(this.OnConfirmDiamondMopup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.OpenChapterRewardPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_CloseChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.CloseChapterRewardPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_GetChapterReward, new CUIEventManager.OnUIEventHandler(this.GetChapterReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_BuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnBuyPlayTime));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmBuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyPlayTime));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmItemFullMopup, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullMopup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmItemFullAdv, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullAdv));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_CloseSettleForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSettleForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmTeamPowerCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamPower));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmTeamNumCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamNum));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Explore_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenExplore));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ExploreScroll, new CUIEventManager.OnUIEventHandler(this.OnExploreListScroll));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ExploreSelect, new CUIEventManager.OnUIEventHandler(this.OnExploreSelect));
			Singleton<EventRouter>.instance.RemoveEventHandler<CSPkg>("ShopBuyLvlChallengeTime", new Action<CSPkg>(this.OnBuyPlayTimeRsp));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.SINGLEGAME_ERR_FREEHERO, new Action(this.OnFreeHeroChanged));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(CAdventureSys.ResetElitePlayNum));
		}

		public void Clear()
		{
			this.currentChapter = 0;
			this.currentDifficulty = 0;
			this.currentLevelSeq = 0;
		}

		public void OpenAdvForm(int chapterId, int LevelNo, int difficulty)
		{
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(CAdventureSys.ADVENTURE_SELECT_FORM, true, true);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				this.currentChapter = chapterId;
				this.currentDifficulty = difficulty;
				this.currentLevelSeq = LevelNo;
				if (!this.IsLevelVaild(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
				{
					DebugHelper.Assert(false, string.Format("The Level is Invaild , chapterNo[{0}],levelNo[{1}],difficulty[{2}]", this.currentChapter, this.currentLevelSeq, this.currentDifficulty));
					this.currentChapter = 1;
					this.currentDifficulty = 1;
					this.currentLevelSeq = 1;
				}
				ResLevelCfgInfo levelCfg = CAdventureSys.GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				if (levelCfg == null)
				{
					return;
				}
				this.currentLevelId = levelCfg.iCfgID;
				this.CheckUnlockTips();
				CAdventureView.InitChapterForm(formScript, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
		}

		private void CheckUnlockTips()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				if (this.m_chapterStatus == null || this.m_difficultyStatus == null)
				{
					this.m_chapterStatus = new byte[CAdventureSys.CHAPTER_NUM];
					this.m_difficultyStatus = new byte[CAdventureSys.CHAPTER_NUM * CAdventureSys.LEVEL_DIFFICULT_OPENED];
					for (int i = 0; i < CAdventureSys.CHAPTER_NUM; i++)
					{
						this.m_chapterStatus[i] = (Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint)(i + 1)) ? 1 : 0);
					}
					for (int j = 0; j < CAdventureSys.LEVEL_DIFFICULT_OPENED; j++)
					{
						for (int k = 0; k < CAdventureSys.CHAPTER_NUM; k++)
						{
							if (this.m_chapterStatus[k] == 1 && masterRoleInfo.pveLevelDetail[j].ChapterDetailList[k].LevelDetailList[0].levelStatus != 0)
							{
								this.m_difficultyStatus[j * CAdventureSys.LEVEL_DIFFICULT_OPENED + k] = 1;
							}
						}
					}
				}
				else
				{
					int num = -1;
					int num2 = 0;
					for (int l = 0; l < CAdventureSys.CHAPTER_NUM; l++)
					{
						if (this.m_chapterStatus[l] == 0 && Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint)(l + 1)))
						{
							this.m_chapterStatus[l] = 1;
							num = l;
							this.bNewChapterId = (byte)(l + 1);
						}
					}
					for (int m = 0; m < CAdventureSys.LEVEL_DIFFICULT_OPENED; m++)
					{
						for (int n = 0; n < CAdventureSys.CHAPTER_NUM; n++)
						{
							if (this.m_chapterStatus[n] == 1 && this.m_difficultyStatus[m * CAdventureSys.LEVEL_DIFFICULT_OPENED + n] == 0 && masterRoleInfo.pveLevelDetail[m].ChapterDetailList[n].LevelDetailList[0].levelStatus != 0)
							{
								num2 = m;
								num = n;
								this.m_difficultyStatus[m * CAdventureSys.LEVEL_DIFFICULT_OPENED + n] = 1;
							}
						}
					}
					if (num >= 0)
					{
						ResChapterInfo dataByIndex = GameDataMgr.chapterInfoDatabin.GetDataByIndex(num);
						if (dataByIndex != null)
						{
							Singleton<CUIManager>.instance.OpenTips("Adventure_Unlock_Tips", true, 1f, null, new object[]
							{
								Utility.UTF8Convert(dataByIndex.szChapterName),
								Singleton<CTextManager>.instance.GetText(string.Format("Adventure_Level_{0}", num2 + 1))
							});
						}
						return;
					}
				}
			}
		}

		private void OnAdv_OpenForm(CUIEvent uiEvent)
		{
			if (!Singleton<SCModuleControl>.instance.GetActiveModule(COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_ADVENTURE))
			{
				Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
				return;
			}
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(CAdventureSys.ADVENTURE_SELECT_FORM, true, true);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				Singleton<CBattleGuideManager>.instance.bTrainingAdv = false;
				if (!this.IsLevelVaild(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
				{
					byte[] cacheLevel = this.GetCacheLevel();
					this.currentChapter = (int)cacheLevel[0];
					this.currentDifficulty = (int)cacheLevel[2];
					this.currentLevelSeq = (int)cacheLevel[1];
				}
				if (!this.IsLevelVaild(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
				{
					this.currentChapter = 1;
					this.currentDifficulty = 1;
					this.currentLevelSeq = 1;
				}
				ResLevelCfgInfo levelCfg = CAdventureSys.GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				if (levelCfg == null)
				{
					return;
				}
				this.currentLevelId = levelCfg.iCfgID;
				this.CheckUnlockTips();
				CAdventureView.InitChapterForm(formScript, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.enterPveEntry, new uint[0]);
			}
		}

		private void OnAdv_CloseForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			Singleton<CResourceManager>.instance.UnloadUnusedAssets();
		}

		private void OnAdv_SelectChapter(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			if (tag == this.currentChapter)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null)
			{
				this.currentChapter = tag;
				this.currentDifficulty = CAdventureSys.GetLastDifficulty(this.currentChapter);
				this.currentLevelSeq = CAdventureSys.GetLastLevel(this.currentChapter, this.currentDifficulty);
				ResLevelCfgInfo levelCfg = CAdventureSys.GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				if (levelCfg == null)
				{
					return;
				}
				this.currentLevelId = levelCfg.iCfgID;
				if (this.currentChapter == (int)this.bNewChapterId)
				{
					this.bNewChapterId = 0;
				}
				CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
			this.SetCacheLevel((byte)this.currentChapter, (byte)this.currentLevelSeq, (byte)this.currentDifficulty);
		}

		private void OnAdv_SelectPreChapter(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null && this.currentChapter - 1 > 0)
			{
				this.currentChapter--;
				if (this.currentChapter == (int)this.bNewChapterId)
				{
					this.bNewChapterId = 0;
				}
				CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
		}

		private void OnAdv_SelectNextChapter(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null && this.currentChapter + 1 <= CAdventureSys.CHAPTER_NUM)
			{
				this.currentChapter++;
				if (this.currentChapter == (int)this.bNewChapterId)
				{
					this.bNewChapterId = 0;
				}
				CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
		}

		private void OnAdv_SelectLevel(CUIEvent uiEvent)
		{
			if (this.currentLevelSeq == uiEvent.m_eventParams.tag)
			{
				return;
			}
			this.currentLevelSeq = uiEvent.m_eventParams.tag;
			ResLevelCfgInfo levelCfg = CAdventureSys.GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			if (levelCfg == null)
			{
				return;
			}
			this.currentLevelId = levelCfg.iCfgID;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null)
			{
				CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
			this.SetCacheLevel((byte)this.currentChapter, (byte)this.currentLevelSeq, (byte)this.currentDifficulty);
		}

		private void OnAdv_SelectDifficult(CUIEvent uiEvent)
		{
			if (this.currentDifficulty == uiEvent.m_eventParams.tag)
			{
				return;
			}
			this.currentDifficulty = uiEvent.m_eventParams.tag;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null)
			{
				this.currentLevelSeq = Math.Min(CAdventureSys.GetLastLevel(this.currentChapter, this.currentDifficulty), this.currentLevelSeq);
				ResLevelCfgInfo levelCfg = CAdventureSys.GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				if (levelCfg == null)
				{
					return;
				}
				this.currentLevelId = levelCfg.iCfgID;
				CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
			this.SetCacheLevel((byte)this.currentChapter, (byte)this.currentLevelSeq, (byte)this.currentDifficulty);
		}

		private void OnLevelDetail_OpenForm(CUIEvent uiEvent)
		{
			ResLevelCfgInfo levelCfg = CAdventureSys.GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			if (levelCfg == null)
			{
				return;
			}
			DebugHelper.Assert(levelCfg != null);
			EnterAdvError enterAdvError = this.CanEnterLevel(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			if (enterAdvError == EnterAdvError.Locked)
			{
				if (this.currentDifficulty == 1)
				{
					Singleton<CUIManager>.instance.OpenTips("Level_Error_Tips_1", true, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.instance.OpenTips("Level_Error_Tips_3", true, 1.5f, null, new object[0]);
				}
			}
			else if (enterAdvError == EnterAdvError.Other)
			{
				Singleton<CUIManager>.instance.OpenTips("Level_Error_Tips_2", true, 1.5f, null, new object[0]);
			}
			else if (enterAdvError == EnterAdvError.None)
			{
				CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(CAdventureSys.ADVENTURE_LEVEL_FORM, false, true);
				CAdventureView.InitLevelForm(formScript, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				if (this.currentChapter == 1 && this.currentLevelSeq == 1 && this.currentDifficulty == 1)
				{
					Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.PVE_1_1_1_Enter;
					MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(12, true, false);
				}
			}
		}

		private void OnCloseSettleForm(CUIEvent uiEvent)
		{
			CAdventureView.CheckMopupLevelUp();
		}

		private void OnOpenChooseHero(CUIEvent uiEvent)
		{
			Button component = uiEvent.m_srcWidget.GetComponent<Button>();
			if (component.get_interactable())
			{
				ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)this.currentLevelId);
				DebugHelper.Assert(dataByKey != null, "Can't find level config -- ID: {0}", new object[]
				{
					this.currentLevelId
				});
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				DebugHelper.Assert(masterRoleInfo != null, "Master role info is NULL!");
				if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
					return;
				}
				ResLevelCfgInfo dataByKey2 = GameDataMgr.levelDatabin.GetDataByKey((long)Singleton<CAdventureSys>.GetInstance().currentLevelId);
				CSDT_SINGLE_GAME_OF_ADVENTURE cSDT_SINGLE_GAME_OF_ADVENTURE = new CSDT_SINGLE_GAME_OF_ADVENTURE();
				cSDT_SINGLE_GAME_OF_ADVENTURE.iLevelID = dataByKey2.iCfgID;
				cSDT_SINGLE_GAME_OF_ADVENTURE.bChapterNo = (byte)dataByKey2.iChapterId;
				cSDT_SINGLE_GAME_OF_ADVENTURE.bLevelNo = dataByKey2.bLevelNo;
				cSDT_SINGLE_GAME_OF_ADVENTURE.bDifficultType = (byte)this.currentDifficulty;
				Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithAdventure(dataByKey2.dwBattleListID, cSDT_SINGLE_GAME_OF_ADVENTURE, StringHelper.UTF8BytesToString(ref dataByKey2.szName));
				Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enPVE_Adventure, (byte)dataByKey2.iHeroNum, 0u, 0, 0);
			}
		}

		private void OnChooseHeroReady(CUIEvent uiEvent)
		{
			Button component = uiEvent.m_srcWidget.GetComponent<Button>();
			if (component.get_interactable())
			{
				DebugHelper.Assert(this.currentLevelId != 0);
				DebugHelper.Assert(this.currentChapter != 0);
				DebugHelper.Assert(this.currentLevelSeq != 0);
				ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)this.currentLevelId);
				DebugHelper.Assert(dataByKey != null);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				DebugHelper.Assert(masterRoleInfo != null);
				if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
					return;
				}
				this.ConfirmCheckTeamPower(null);
			}
		}

		private void ConfirmCheckTeamPower(CUIEvent uiEvent = null)
		{
			if (this.currentChapter > 1 && !this.CheckTeamNum())
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Adv_TeamNum_Confirm"), enUIEventID.Adv_ConfirmTeamNumCheck, enUIEventID.None, false);
				return;
			}
			this.ConfirmCheckTeamNum(null);
		}

		private void ConfirmCheckTeamNum(CUIEvent uiEvent = null)
		{
			string empty = string.Empty;
			if (!this.CheckFullItem(out empty))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Package_Item_Max_Tip_Level", new string[]
				{
					empty
				}), enUIEventID.Adv_ConfirmItemFullAdv, enUIEventID.None, false);
				return;
			}
			this.ConfirmItemFullAdv(null);
		}

		private void ConfirmItemFullAdv(CUIEvent uiEvent = null)
		{
			CHeroSelectBaseSystem.SendSingleGameStartMsgSkipHeroSelect(this.currentLevelId, this.currentDifficulty);
		}

		private void OnMopup(CUIEvent uiEvent)
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SAODANG))
			{
				Button component = uiEvent.m_srcWidget.GetComponent<Button>();
				if (component.get_interactable())
				{
					if (this.CheckMopUpCondition(1))
					{
						this.ReqMopup(1);
					}
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Mopup_Condition", true, 1.5f, null, new object[0]);
				}
			}
			else
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(7u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			}
		}

		private void OnMopupTenTimes(CUIEvent uiEvent)
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SAODANG))
			{
				Button component = uiEvent.m_srcWidget.GetComponent<Button>();
				if (component.get_interactable())
				{
					byte b = (byte)uiEvent.m_eventParams.tag;
					if (this.CheckMopUpCondition((int)b))
					{
						this.ReqMopup(b);
					}
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Mopup10_Condition", true, 1.5f, null, new object[0]);
				}
			}
			else
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(7u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			}
		}

		private void ConfirmItemFullMopup(CUIEvent uiEvent)
		{
			byte sweepCount = (byte)uiEvent.m_eventParams.tag;
			this.ReqMopup(sweepCount);
		}

		private void OnConfirmDiamondMopup(CUIEvent uiEvent)
		{
			string empty = string.Empty;
			byte b = (byte)uiEvent.m_eventParams.tag;
			if (!this.CheckFullItem(out empty))
			{
				stUIEventParams par = default(stUIEventParams);
				par.tag = (int)b;
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Package_Item_Max_Tip_Sweep", new string[]
				{
					empty
				}), enUIEventID.Adv_ConfirmItemFullMopup, enUIEventID.None, par, false);
			}
			else
			{
				this.ReqMopup(b);
			}
		}

		private void OpenChapterRewardPanel(CUIEvent uiEvent)
		{
			bool bCanGet = false;
			int tag = uiEvent.m_eventParams.tag;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null)
			{
				this.currentChapter = tag;
				this.currentLevelSeq = CAdventureSys.GetLastLevel(this.currentChapter, this.currentDifficulty);
				CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
				if (this.CheckOpenChapterRewardCondition(out bCanGet, tag))
				{
					CAdventureView.OpenChapterRewardPanel(form, form.gameObject, tag, this.currentDifficulty, bCanGet);
				}
			}
		}

		private void CloseChapterRewardPanel(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
			if (form != null)
			{
				CAdventureView.CloseChapterRewardPanel(form.gameObject);
			}
		}

		private void GetChapterReward(CUIEvent uiEvent)
		{
			Button component = uiEvent.m_srcWidget.GetComponent<Button>();
			if (component.get_interactable() && this.CheckChapterRewardCondition())
			{
				this.ReqGetChapterReward();
			}
		}

		private void OnBuyPlayTime(CUIEvent uiEvent)
		{
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
			DebugHelper.Assert(pVE_ADV_COMPLETE_INFO != null);
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[this.currentChapter - 1];
			DebugHelper.Assert(pVE_CHAPTER_COMPLETE_INFO != null);
			PVE_LEVEL_COMPLETE_INFO levelInfo = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList[this.currentLevelSeq - 1];
			byte b = 0;
			uint num = 0u;
			string empty = string.Empty;
			int num2 = this.CanBuyPlayTime(levelInfo, out b, out num, out empty);
			if (num2 == 0)
			{
				string text = (b == 2) ? Singleton<CTextManager>.GetInstance().GetText("Money_Type_DianQuan") : Singleton<CTextManager>.GetInstance().GetText("Money_Type_GoldCoin");
				string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Level_Refresh_Confirm", new string[]
				{
					num.ToString(),
					text
				}), new object[0]);
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Adv_ConfirmBuyPlayTime, enUIEventID.None, false);
			}
			else if (num2 == -1)
			{
				CUICommonSystem.OpenDianQuanNotEnoughTip();
			}
			else if (num2 == -2)
			{
				CUICommonSystem.OpenGoldCoinNotEnoughTip();
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Can_Not_Refresh_Level") + empty, false, 1.5f, null, new object[0]);
			}
		}

		private void OnConfirmBuyPlayTime(CUIEvent uiEvent)
		{
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
			DebugHelper.Assert(pVE_ADV_COMPLETE_INFO != null);
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[this.currentChapter - 1];
			DebugHelper.Assert(pVE_CHAPTER_COMPLETE_INFO != null);
			PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList[this.currentLevelSeq - 1];
			this.ReqBuyPlayTime(pVE_LEVEL_COMPLETE_INFO.PlayLimit + 1);
		}

		private void OnFreeHeroChanged()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_LEVEL_FORM);
			if (form != null)
			{
				CAdventureView.InitLevelForm(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
		}

		private bool CheckChapterRewardCondition()
		{
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
			if (pVE_ADV_COMPLETE_INFO == null)
			{
				return false;
			}
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[this.currentChapter - 1];
			return pVE_CHAPTER_COMPLETE_INFO != null && pVE_CHAPTER_COMPLETE_INFO.bIsGetBonus == 0 && CAdventureSys.GetChapterTotalStar(pVE_CHAPTER_COMPLETE_INFO) == CAdventureSys.STAR_PER_LEVEL * CAdventureSys.LEVEL_PER_CHAPTER;
		}

		private bool CheckOpenChapterRewardCondition(out bool bCanGetReward, int chapterId)
		{
			bCanGetReward = false;
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
			if (pVE_ADV_COMPLETE_INFO == null)
			{
				return true;
			}
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterId - 1];
			if (pVE_CHAPTER_COMPLETE_INFO == null)
			{
				return true;
			}
			if (pVE_CHAPTER_COMPLETE_INFO.bIsGetBonus == 0)
			{
				if (CAdventureSys.GetChapterTotalStar(pVE_CHAPTER_COMPLETE_INFO) == CAdventureSys.STAR_PER_LEVEL * CAdventureSys.LEVEL_PER_CHAPTER)
				{
					bCanGetReward = true;
				}
				return true;
			}
			return false;
		}

		private bool CheckMopUpCondition(int time)
		{
			if (!this.HasEnoughMopupTicket(time))
			{
				int num;
				if (this.HasEnoughDiamondForTicket(time, out num))
				{
					stUIEventParams par = default(stUIEventParams);
					par.tag = time;
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Buy_Sweep_Ticket_Confirm", new string[]
					{
						num.ToString()
					}), enUIEventID.Adv_ConfirmDiamondMopup, enUIEventID.None, par, false);
					return false;
				}
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Not_Enough_Sweep_Ticket"), false);
				return false;
			}
			else
			{
				string empty = string.Empty;
				if (!this.CheckFullItem(out empty))
				{
					stUIEventParams par2 = default(stUIEventParams);
					par2.tag = time;
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Package_Item_Max_Tip_Sweep", new string[]
					{
						empty.ToString()
					}), enUIEventID.Adv_ConfirmItemFullMopup, enUIEventID.None, par2, false);
					return false;
				}
				return true;
			}
		}

		private void ReqMopup(byte SweepCount)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1054u);
			cSPkg.stPkgData.stSweepSingleGameReq.iLevelID = this.currentLevelId;
			cSPkg.stPkgData.stSweepSingleGameReq.bGameType = 0;
			cSPkg.stPkgData.stSweepSingleGameReq.dwSweepCnt = (uint)SweepCount;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqGetChapterReward()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1056u);
			cSPkg.stPkgData.stGetChapterRewardReq.bChapterNo = (byte)this.currentChapter;
			cSPkg.stPkgData.stGetChapterRewardReq.bDifficultType = (byte)this.currentDifficulty;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqBuyPlayTime(int BuyTime)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1113u);
			cSPkg.stPkgData.stShopBuyReq.stExtraInfo.dwLevelID = (uint)this.currentLevelId;
			cSPkg.stPkgData.stShopBuyReq.iBuyType = 6;
			cSPkg.stPkgData.stShopBuyReq.iBuySubType = BuyTime;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1057)]
		public static void OnGetChapterReward(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stGetChapterRewardRsp.iErrorCode == 0)
			{
				PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[(int)(msg.stPkgData.stGetChapterRewardRsp.bDifficultType - 1)];
				DebugHelper.Assert(pVE_ADV_COMPLETE_INFO != null, "PVE info is NULL!!! -- Difficulty", new object[]
				{
					msg.stPkgData.stGetChapterRewardRsp.bDifficultType
				});
				pVE_ADV_COMPLETE_INFO.ChapterDetailList[(int)(msg.stPkgData.stGetChapterRewardRsp.bChapterNo - 1)].bIsGetBonus = 1;
				ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(msg.stPkgData.stGetChapterRewardRsp.stRewardDetail);
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), null, false, enUIEventID.None, false, false, "Form_Award");
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
				if (form != null)
				{
					CAdventureView.InitChapterForm(form, (int)msg.stPkgData.stGetChapterRewardRsp.bChapterNo, Singleton<CAdventureSys>.instance.currentLevelSeq, (int)msg.stPkgData.stGetChapterRewardRsp.bDifficultType);
					CAdventureView.CloseChapterRewardPanel(form.gameObject);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_All_Chapter_Starts_Error") + Utility.ProtErrCodeToStr(1057, msg.stPkgData.stGetChapterRewardRsp.iErrorCode), false, 1.5f, null, new object[0]);
			}
		}

		public void OnBuyPlayTimeRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)((ulong)msg.stPkgData.stShopBuyRsp.stExtraInfo.dwLevelID));
			DebugHelper.Assert(dataByKey != null);
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[Singleton<CAdventureSys>.GetInstance().currentDifficulty - 1];
			DebugHelper.Assert(pVE_ADV_COMPLETE_INFO != null);
			PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[dataByKey.iChapterId - 1].LevelDetailList[(int)(dataByKey.bLevelNo - 1)];
			pVE_LEVEL_COMPLETE_INFO.PlayLimit = msg.stPkgData.stShopBuyRsp.iBuySubType;
			pVE_LEVEL_COMPLETE_INFO.PlayNum = 0u;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_LEVEL_FORM);
			if (form != null)
			{
				CAdventureView.InitLevelForm(form, dataByKey.iChapterId, (int)dataByKey.bLevelNo, this.currentDifficulty);
			}
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
		}

		public static void ResetElitePlayNum()
		{
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[1];
			if (pVE_ADV_COMPLETE_INFO == null)
			{
				return;
			}
			for (int i = 0; i < pVE_ADV_COMPLETE_INFO.ChapterDetailList.Length; i++)
			{
				if (pVE_ADV_COMPLETE_INFO.ChapterDetailList[i] != null)
				{
					PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[i];
					for (int j = 0; j < pVE_CHAPTER_COMPLETE_INFO.LevelDetailList.Length; j++)
					{
						if (pVE_CHAPTER_COMPLETE_INFO.LevelDetailList[j] != null)
						{
							PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList[j];
							pVE_LEVEL_COMPLETE_INFO.PlayNum = 0u;
							pVE_LEVEL_COMPLETE_INFO.PlayLimit = 0;
						}
					}
				}
			}
			Singleton<CAdventureSys>.GetInstance().RefreshLevelForm();
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
		}

		private void RefreshLevelForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_LEVEL_FORM);
			if (form != null)
			{
				CAdventureView.InitLevelForm(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
			}
		}

		public bool UpdateAdvProgress(bool bWin)
		{
			PVE_ADV_COMPLETE_INFO[] pveLevelDetail = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail;
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = pveLevelDetail[this.currentDifficulty - 1];
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null)
			{
				int chapterNo = curLvelContext.m_chapterNo;
				int levelNo = (int)curLvelContext.m_levelNo;
				bool result = false;
				if (bWin)
				{
					if (this.IsLevelFinished(pVE_ADV_COMPLETE_INFO, chapterNo, levelNo))
					{
						pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].levelStatus = 2;
						pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].PlayNum += 1u;
						byte starBits = Singleton<StarSystem>.GetInstance().GetStarBits();
						if (this.HasMoreStar(starBits, pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].bStarBits))
						{
							pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].bStarBits = starBits;
						}
					}
					else
					{
						result = true;
						if (pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1] != null)
						{
							pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].levelStatus = 2;
							pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].PlayNum += 1u;
							pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].bStarBits = Singleton<StarSystem>.GetInstance().GetStarBits();
						}
						if (levelNo < 4)
						{
							if (pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1] != null)
							{
								pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo].levelStatus = 1;
								this.currentLevelSeq = levelNo + 1;
							}
						}
						else if (this.currentDifficulty < CAdventureSys.LEVEL_DIFFICULT_OPENED)
						{
							PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO2 = pveLevelDetail[this.currentDifficulty];
							if (pVE_ADV_COMPLETE_INFO2.ChapterDetailList[chapterNo - 1] != null)
							{
								pVE_ADV_COMPLETE_INFO2.ChapterDetailList[chapterNo - 1].LevelDetailList[0].levelStatus = 1;
								this.currentDifficulty++;
								this.currentLevelSeq = 1;
							}
						}
					}
				}
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
				return result;
			}
			return false;
		}

		public static ResLevelCfgInfo GetLevelCfg(int ChapterId, int Seq, int Difficulty)
		{
			return GameDataMgr.levelDatabin.FindIf((ResLevelCfgInfo x) => x.iChapterId == ChapterId && (int)x.bLevelNo == Seq);
		}

		public static int GetLevelId(int ChapterId, int Seq, int Difficulty)
		{
			ResLevelCfgInfo resLevelCfgInfo = GameDataMgr.levelDatabin.FindIf((ResLevelCfgInfo x) => x.iChapterId == ChapterId && (int)x.bLevelNo == Seq && (int)x.bLevelDifficulty == Difficulty);
			return (resLevelCfgInfo != null) ? resLevelCfgInfo.iCfgID : 0;
		}

		public static int GetNextLevelId(int ChapterId, int Seq, int Difficulty)
		{
			int levelId;
			if (Seq < CAdventureSys.LEVEL_PER_CHAPTER)
			{
				levelId = CAdventureSys.GetLevelId(ChapterId, ++Seq, Difficulty);
			}
			else
			{
				levelId = CAdventureSys.GetLevelId(++ChapterId, 1, Difficulty);
			}
			return levelId;
		}

		public static int GetNextChapterId(int ChapterId, int Seq)
		{
			int result;
			if (Seq < CAdventureSys.LEVEL_PER_CHAPTER)
			{
				result = ChapterId;
			}
			else
			{
				result = ChapterId + 1;
			}
			return result;
		}

		public static int GetStarNum(byte starMask)
		{
			return (((starMask & 1) > 0) ? 1 : 0) + (((starMask & 2) > 0) ? 1 : 0) + (((starMask & 4) > 0) ? 1 : 0);
		}

		public static bool IsStarGained(byte starMask, int Pos)
		{
			int num = (int)Math.Pow(2.0, (double)(Pos - 1));
			return ((int)starMask & num) > 0;
		}

		public static bool IsLevelFullStar(int levelId)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)levelId);
			DebugHelper.Assert(dataByKey != null);
			DebugHelper.Assert(dataByKey.bLevelDifficulty > 0, "LevelDifficulty must > 0");
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[(int)(dataByKey.bLevelDifficulty - 1)];
			if (pVE_ADV_COMPLETE_INFO == null)
			{
				return false;
			}
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[dataByKey.iChapterId - 1];
			if (pVE_CHAPTER_COMPLETE_INFO == null)
			{
				return false;
			}
			PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList[(int)(dataByKey.bLevelNo - 1)];
			return pVE_LEVEL_COMPLETE_INFO != null && CAdventureSys.GetStarNum(pVE_LEVEL_COMPLETE_INFO.bStarBits) == CAdventureSys.STAR_PER_LEVEL;
		}

		public static bool IsChapterFullStar(int chapterNum, int difficulty)
		{
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[difficulty - 1];
			if (pVE_ADV_COMPLETE_INFO == null)
			{
				return false;
			}
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNum - 1];
			return pVE_CHAPTER_COMPLETE_INFO != null && CAdventureSys.GetChapterTotalStar(pVE_CHAPTER_COMPLETE_INFO) == CAdventureSys.STAR_PER_LEVEL * CAdventureSys.LEVEL_PER_CHAPTER;
		}

		public static int GetChapterTotalStar(PVE_CHAPTER_COMPLETE_INFO chapterInfo)
		{
			int num = 0;
			for (int i = 0; i < chapterInfo.LevelDetailList.Length; i++)
			{
				if (chapterInfo.LevelDetailList[i] != null)
				{
					num += CAdventureSys.GetStarNum(chapterInfo.LevelDetailList[i].bStarBits);
				}
			}
			return num;
		}

		public bool IsLevelOpen(int LevelId)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)LevelId);
			DebugHelper.Assert(dataByKey != null, "Can't find level config with ID: {0}", new object[]
			{
				LevelId
			});
			DebugHelper.Assert(dataByKey.bLevelDifficulty > 0, "LevelDifficulty must > 0");
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[(int)(dataByKey.bLevelDifficulty - 1)];
			return pVE_ADV_COMPLETE_INFO != null && Singleton<CFunctionUnlockSys>.GetInstance().ChapterIsUnlock((uint)dataByKey.iChapterId) && this.IsLevelOpened(pVE_ADV_COMPLETE_INFO, dataByKey.iChapterId, (int)dataByKey.bLevelNo);
		}

		private bool IsLevelOpened(PVE_ADV_COMPLETE_INFO pveInfo, int ChapterNum, int LevelSeq)
		{
			return pveInfo.ChapterDetailList[ChapterNum - 1].LevelDetailList[LevelSeq - 1].levelStatus != 0;
		}

		public bool IsLevelFinished(int inLevelId)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)inLevelId);
			DebugHelper.Assert(dataByKey != null, "Can't find level config with ID: {0}", new object[]
			{
				inLevelId
			});
			DebugHelper.Assert(dataByKey.bLevelDifficulty > 0, "LevelDifficulty must > 0");
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[(int)(dataByKey.bLevelDifficulty - 1)];
			return pVE_ADV_COMPLETE_INFO != null && this.IsLevelFinished(pVE_ADV_COMPLETE_INFO, dataByKey.iChapterId, (int)dataByKey.bLevelNo);
		}

		private bool IsLevelFinished(PVE_ADV_COMPLETE_INFO pveInfo, int ChapterNum, int LevelSeq)
		{
			return this.IsLevelOpened(pveInfo, ChapterNum, LevelSeq) && pveInfo.ChapterDetailList[ChapterNum - 1].LevelDetailList[LevelSeq - 1].levelStatus == 2;
		}

		private bool HasMoreStar(byte newBits, byte oldBits)
		{
			return CAdventureSys.GetStarNum(newBits) >= CAdventureSys.GetStarNum(oldBits);
		}

		private bool HasEnoughAPMopup(int count)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)this.currentLevelId);
			DebugHelper.Assert(dataByKey != null);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null);
			return (ulong)masterRoleInfo.CurActionPoint >= (ulong)(dataByKey.dwEnterConsumeAP + dataByKey.dwFinishConsumeAP) * (ulong)((long)count);
		}

		private bool HasEnoughMopupTicket(int count)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, CAdventureSys.MOPUP_TICKET_ID);
			return (long)useableStackCount >= (long)count * (long)((ulong)CAdventureSys.MOPUP_TICKET_NUM_PER_LEVEL);
		}

		private bool HasEnoughDiamondForTicket(int count, out int ReqDiamondNum)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, CAdventureSys.MOPUP_TICKET_ID);
			DebugHelper.Assert((long)useableStackCount < (long)count * (long)((ulong)CAdventureSys.MOPUP_TICKET_NUM_PER_LEVEL));
			ReqDiamondNum = (int)(((long)count * (long)((ulong)CAdventureSys.MOPUP_TICKET_NUM_PER_LEVEL) - (long)useableStackCount) * (long)((ulong)CAdventureSys.MOPUP_TICKET_PRICE_BY_DIAMOND));
			return Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().DianQuan >= (ulong)((long)ReqDiamondNum);
		}

		private int CanBuyPlayTime(PVE_LEVEL_COMPLETE_INFO levelInfo, out byte CoinType, out uint Price, out string reason)
		{
			reason = string.Empty;
			int num = levelInfo.PlayLimit + 1;
			if ((long)num <= (long)((ulong)CAdventureSys.CHALLENGE_BUYTIME_LIMIT))
			{
				Dictionary<long, object>.Enumerator enumerator = GameDataMgr.resShopInfoDatabin.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.get_Current();
					ResShopInfo resShopInfo = (ResShopInfo)current.get_Value();
					if (resShopInfo.iType == 6 && resShopInfo.iSubType == num)
					{
						CoinType = resShopInfo.bCoinType;
						Price = resShopInfo.dwCoinPrice;
						if (resShopInfo.bCoinType == 2)
						{
							if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().DianQuan >= (ulong)resShopInfo.dwCoinPrice)
							{
								return 0;
							}
							reason = Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Not_Enough");
							return -1;
						}
						else
						{
							if (resShopInfo.bCoinType != 4)
							{
								DebugHelper.Assert(false, "Invalid coin type: {0}", new object[]
								{
									resShopInfo.bCoinType
								});
								reason = Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Money_Type_Error");
								return -3;
							}
							if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GoldCoin >= resShopInfo.dwCoinPrice)
							{
								return 0;
							}
							reason = Singleton<CTextManager>.GetInstance().GetText("Common_GoldCoin_Not_Enough");
							return -2;
						}
					}
				}
				CoinType = 0;
				Price = 0u;
				reason = Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Can_Not_Find_Config");
				return -4;
			}
			CoinType = 0;
			Price = 0u;
			reason = Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Level_Refresh_Max");
			return -5;
		}

		private bool CheckFullItem(out string itemName)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)this.currentLevelId);
			DebugHelper.Assert(dataByKey != null, "level config is null with level ID: {0}", new object[]
			{
				this.currentLevelId
			});
			itemName = string.Empty;
			for (int i = 0; i < 5; i++)
			{
				if (dataByKey.astRewardShowDetail[i].dwRewardID != 0u)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
					if (dataByKey.astRewardShowDetail[i].bRewardType == 2)
					{
						int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dataByKey.astRewardShowDetail[i].dwRewardID);
						ResPropInfo dataByKey2 = GameDataMgr.itemDatabin.GetDataByKey(dataByKey.astRewardShowDetail[i].dwRewardID);
						DebugHelper.Assert(dataByKey2 != null, "item is null with ID: {0}", new object[]
						{
							dataByKey.astRewardShowDetail[i].dwRewardID
						});
						itemName = StringHelper.UTF8BytesToString(ref dataByKey2.szName);
						if (useableStackCount == dataByKey2.iOverLimit)
						{
							return false;
						}
					}
					else if (dataByKey.astRewardShowDetail[i].bRewardType == 3)
					{
						int useableStackCount2 = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, dataByKey.astRewardShowDetail[i].dwRewardID);
						ResEquipInfo dataByKey3 = GameDataMgr.equipInfoDatabin.GetDataByKey(dataByKey.astRewardShowDetail[i].dwRewardID);
						DebugHelper.Assert(dataByKey3 != null, "equip is null with ID: {0}", new object[]
						{
							dataByKey.astRewardShowDetail[i].dwRewardID
						});
						itemName = StringHelper.UTF8BytesToString(ref dataByKey3.szName);
						if (useableStackCount2 == dataByKey3.iOverLimit)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool CheckTeamNum()
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)this.currentLevelId);
			if (dataByKey == null)
			{
				DebugHelper.Assert(false, "Can't find Level Config -- LevelID: ", new object[]
				{
					this.currentLevelId
				});
				return false;
			}
			COMDT_BATTLELIST_LIST s_defaultBattleListInfo = CHeroSelectBaseSystem.s_defaultBattleListInfo;
			int num = 0;
			if (s_defaultBattleListInfo == null || s_defaultBattleListInfo.dwListNum == 0u)
			{
				return false;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)s_defaultBattleListInfo.dwListNum))
			{
				if (s_defaultBattleListInfo.astBattleList[num2].dwBattleListID == dataByKey.dwBattleListID)
				{
					if (s_defaultBattleListInfo.astBattleList[num2].stBattleList.wHeroCnt == 0)
					{
						return false;
					}
					for (int i = 0; i < (int)s_defaultBattleListInfo.astBattleList[num2].stBattleList.wHeroCnt; i++)
					{
						if (s_defaultBattleListInfo.astBattleList[num2].stBattleList.BattleHeroList[i] > 0u)
						{
							num++;
						}
					}
					break;
				}
				else
				{
					num2++;
				}
			}
			return num == dataByKey.iHeroNum;
		}

		public void setDifficult(int indata)
		{
			this.currentDifficulty = indata;
		}

		private void OnOpenExplore(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
			if (CUICommonSystem.IsInMatchingWithAlert())
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CAdventureSys.EXLPORE_FORM_PATH, true, true);
			if (cUIFormScript != null)
			{
				CExploreView.InitExloreList(cUIFormScript);
			}
		}

		private void OnCloseExplore(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(CAdventureSys.EXLPORE_FORM_PATH);
		}

		private void OnDragStart(CUIEvent uiEvent)
		{
			Singleton<CSoundManager>.instance.PostEvent("UI_Add_Button", null);
		}

		private void OnExploreSelect(CUIEvent uiEvent)
		{
			Singleton<CSoundManager>.instance.PostEvent("UI_Add_Button", null);
		}

		private void OnExploreListScroll(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CAdventureSys.EXLPORE_FORM_PATH);
			if (form != null)
			{
				CExploreView.OnExploreListScroll(form.gameObject);
			}
		}

		private EnterAdvError CanEnterLevel(int chapterId, int levelSeq, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return EnterAdvError.Other;
			}
			ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey((long)chapterId);
			DebugHelper.Assert(dataByKey != null, string.Format("chapterId[{0}]", chapterId));
			if (dataByKey == null)
			{
				return EnterAdvError.Other;
			}
			bool flag = Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock(dataByKey.dwChapterId);
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterId - 1];
			PVE_LEVEL_COMPLETE_INFO[] levelDetailList = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList;
			if (levelDetailList[levelSeq - 1].levelStatus == 0 || !flag)
			{
				return EnterAdvError.Locked;
			}
			return EnterAdvError.None;
		}

		private bool IsLevelVaild(int chapterId, int levelSeq, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			return masterRoleInfo != null && chapterId > 0 && levelSeq > 0 && difficulty > 0 && CAdventureSys.LEVEL_DIFFICULT_OPENED >= difficulty && masterRoleInfo.pveLevelDetail[difficulty - 1] != null && CAdventureSys.CHAPTER_NUM >= chapterId && masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[chapterId - 1] != null && masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[levelSeq - 1].LevelDetailList.Length >= levelSeq && masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[chapterId - 1].LevelDetailList[levelSeq - 1] != null;
		}

		public static int GetLastLevel(int chapterId, int difficulty)
		{
			int result = 1;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return result;
			}
			if (chapterId <= 0 || difficulty <= 0)
			{
				return result;
			}
			if (CAdventureSys.LEVEL_DIFFICULT_OPENED < difficulty || masterRoleInfo.pveLevelDetail[difficulty - 1] == null)
			{
				return result;
			}
			if (CAdventureSys.CHAPTER_NUM < chapterId || masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[chapterId - 1] == null)
			{
				return result;
			}
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterId - 1];
			PVE_LEVEL_COMPLETE_INFO[] levelDetailList = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList;
			for (int i = 0; i < levelDetailList.Length; i++)
			{
				if (levelDetailList[i].levelStatus != 0)
				{
					result = i + 1;
				}
			}
			return result;
		}

		public static int GetLastChapter(int difficulty = 1)
		{
			int result = 1;
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return result;
			}
			for (int i = 0; i < CAdventureSys.CHAPTER_NUM; i++)
			{
				if (!Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint)(i + 1)))
				{
					break;
				}
				result = i + 1;
			}
			return result;
		}

		public static int GetLastDifficulty(int chapterId)
		{
			int result = 1;
			if (!Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint)chapterId))
			{
				return result;
			}
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return result;
			}
			int lEVEL_DIFFICULT_OPENED = CAdventureSys.LEVEL_DIFFICULT_OPENED;
			for (int i = 0; i < lEVEL_DIFFICULT_OPENED; i++)
			{
				if (CAdventureSys.IsDifOpen(chapterId, i + 1))
				{
					result = i + 1;
				}
			}
			return result;
		}

		public static bool IsDifOpen(int chapterId, int difficulty)
		{
			if (!Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint)chapterId))
			{
				return false;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return false;
			}
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterId - 1];
			PVE_LEVEL_COMPLETE_INFO[] levelDetailList = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList;
			return levelDetailList[0].levelStatus != 0;
		}

		private byte[] GetCacheLevel()
		{
			byte[] array = new byte[3];
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				array[0] = (byte)PlayerPrefs.GetInt(string.Format("{0}_{1}", "Sgame_Chapter", masterRoleInfo.playerUllUID));
				array[1] = (byte)PlayerPrefs.GetInt(string.Format("{0}_{1}", "Sgame_Level", masterRoleInfo.playerUllUID));
				array[2] = (byte)PlayerPrefs.GetInt(string.Format("{0}_{1}", "Sgame_Difficulty", masterRoleInfo.playerUllUID));
			}
			return array;
		}

		private void SetCacheLevel(byte chapterNo, byte levelNo, byte difficultyNo)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				PlayerPrefs.SetInt(string.Format("{0}_{1}", "Sgame_Chapter", masterRoleInfo.playerUllUID), (int)chapterNo);
				PlayerPrefs.SetInt(string.Format("{0}_{1}", "Sgame_Level", masterRoleInfo.playerUllUID), (int)levelNo);
				PlayerPrefs.SetInt(string.Format("{0}_{1}", "Sgame_Difficulty", masterRoleInfo.playerUllUID), (int)difficultyNo);
			}
		}
	}
}
