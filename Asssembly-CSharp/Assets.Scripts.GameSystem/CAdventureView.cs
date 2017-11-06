using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CAdventureView
	{
		public static Color[] s_Adv_Difficult_Color = new Color[]
		{
			new Color(1f, 1f, 1f),
			new Color(0.117647059f, 1f, 1f),
			new Color(1f, 0f, 0.1254902f),
			new Color(1f, 0f, 0.5568628f),
			new Color(1f, 1f, 1f),
			new Color(0.258823544f, 0.784313738f, 1f),
			new Color(1f, 0f, 0.1254902f),
			new Color(1f, 0f, 0.5568628f)
		};

		public static Color[] s_Adv_Difficult_Circle_Color = new Color[]
		{
			new Color(0.368627459f, 1f, 1f),
			new Color(0.368627459f, 0.68235296f, 1f),
			new Color(1f, 0f, 0f),
			new Color(1f, 0f, 0.917647064f)
		};

		public static Color[] s_Adv_Difficult_Bg_Color = new Color[]
		{
			new Color(0.117647059f, 1f, 1f),
			new Color(0f, 0.784313738f, 1f),
			new Color(1f, 0.2784314f, 0.2784314f),
			new Color(1f, 0f, 0.5568628f)
		};

		public static Color[] s_Adv_Chaptper_Colors = new Color[]
		{
			new Color(0.3019608f, 1f, 1f),
			new Color(0.894117653f, 0.894117653f, 0.894117653f),
			new Color(0.3529412f, 0.3529412f, 0.3529412f)
		};

		public static Color[] s_Adv_Level_Colors = new Color[]
		{
			new Color(1f, 1f, 1f),
			new Color(0.894117653f, 0.894117653f, 0.894117653f),
			new Color(0.5019608f, 0.5254902f, 0.5882353f)
		};

		public static Color s_Adv_Difficulty_Gray_Color = new Color(0.5882353f, 0.5882353f, 0.5882353f);

		public static string MAT_NORMAL_LEVEL = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Bule_Normal.mat";

		public static string MAT_NORMAL_LEVEL_HIGHLIGHT = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Bule_Open.mat";

		public static string MAT_NORMAL_LEVEL_LOCK = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Bule_Lock.mat";

		public static string MAT_ELITE_LEVEL = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Golden_Normal.mat";

		public static string MAT_ELITE_LEVEL_HIGHLIGHT = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Golden_Open.mat";

		public static string MAT_ELITE_LEVEL_LOCK = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Golden_Lock.mat";

		public static string MAT_BOSS_LEVEL = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Red_Normal.mat";

		public static string MAT_BOSS_LEVEL_HIGHLIGHT = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Red_Open.mat";

		public static string MAT_BOSS_LEVEL_LOCK = "UGUI/Form/System/PvE/Adv/Material/PVE_Adv_Red_Lock.mat";

		private static SCDT_SWEEP_REWARD rewardRef;

		private static int CurrentRewardPos;

		private static int CurrentItemPos;

		private static bool bLevelUp;

		private static uint oldLevel;

		private static uint newLevel;

		public static void InitChapterForm(CUIFormScript formScript, int currentChapter, int levelNo, int difficulty)
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return;
			}
			CAdventureView.InitChapterList(formScript, currentChapter, levelNo, difficulty);
			CAdventureView.InitLevelList(formScript, currentChapter, levelNo, difficulty);
			CAdventureView.InitDifficultList(formScript, currentChapter, levelNo, difficulty);
			CAdventureView.InitChapterElement(formScript, currentChapter, levelNo, difficulty);
		}

		public static void InitChapterList(CUIFormScript formScript, int currentChapter, int levelNo, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUIListScript component = formScript.transform.FindChild("ChapterList").GetComponent<CUIListScript>();
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			string prefabPath = string.Empty;
			string text = string.Empty;
			component.SetElementAmount(CAdventureSys.CHAPTER_NUM);
			for (int i = 0; i < CAdventureSys.CHAPTER_NUM; i++)
			{
				ResChapterInfo dataByIndex = GameDataMgr.chapterInfoDatabin.GetDataByIndex(i);
				DebugHelper.Assert(dataByIndex != null);
				bool flag = Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock(dataByIndex.dwChapterId);
				bool bActive = i == (int)(Singleton<CAdventureSys>.instance.bNewChapterId - 1);
				PVE_CHAPTER_COMPLETE_INFO chapterInfo = pVE_ADV_COMPLETE_INFO.ChapterDetailList[i];
				CUIListElementScript elemenet = component.GetElemenet(i);
				int chapterTotalStar = CAdventureSys.GetChapterTotalStar(chapterInfo);
				CAdventureView.SetRewardItem(elemenet.gameObject, chapterInfo, chapterTotalStar, i);
				text = CAdventureView.GetChapterName(i + 1);
				elemenet.transform.FindChild("ChapterNameText").GetComponent<Text>().set_text(text);
				if (currentChapter == i + 1 && flag)
				{
					elemenet.transform.FindChild("ChapterNameText").GetComponent<Text>().set_color(CAdventureView.s_Adv_Chaptper_Colors[0]);
				}
				else if (flag)
				{
					elemenet.transform.FindChild("ChapterNameText").GetComponent<Text>().set_color(CAdventureView.s_Adv_Chaptper_Colors[1]);
				}
				else
				{
					elemenet.transform.FindChild("ChapterNameText").GetComponent<Text>().set_color(CAdventureView.s_Adv_Chaptper_Colors[2]);
				}
				elemenet.GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Adv_SelectChapter;
				elemenet.GetComponent<CUIEventScript>().m_onClickEventParams.tag = i + 1;
				elemenet.transform.FindChild("Lock").gameObject.CustomSetActive(!flag);
				elemenet.transform.FindChild("Unlock").gameObject.CustomSetActive(flag);
				prefabPath = CAdventureView.GetChapterBgPath(i + 1);
				elemenet.transform.FindChild("BackgroundImg").GetComponent<Image>().SetSprite(prefabPath, component.m_belongedFormScript, true, false, false, false);
				elemenet.transform.FindChild("Lock/SelectedImg").GetComponent<Image>().SetSprite(prefabPath, component.m_belongedFormScript, true, false, false, false);
				elemenet.transform.FindChild("Lock/LockText").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByIndex.szLockedTip));
				elemenet.transform.FindChild("New").gameObject.CustomSetActive(bActive);
			}
			component.SelectElement(currentChapter - 1, true);
			component.MoveElementInScrollArea(currentChapter - 1, true);
		}

		public static void InitLevelList(CUIFormScript form, int currentChapter, int levelNo, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey((long)currentChapter);
			DebugHelper.Assert(dataByKey != null);
			bool flag = Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock(dataByKey.dwChapterId);
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[currentChapter - 1];
			PVE_LEVEL_COMPLETE_INFO[] levelDetailList = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList;
			CUIListScript component = form.transform.FindChild("LevelList").GetComponent<CUIListScript>();
			component.SetElementAmount(levelDetailList.Length);
			Sprite sprite = CUIUtility.GetSpritePrefeb(CAdventureView.GetLevelFramePath(difficulty), false, false).GetComponent<SpriteRenderer>().sprite;
			GameObject spritePrefeb = CUIUtility.GetSpritePrefeb(CAdventureView.GetLevelSelectFramePath(difficulty), false, false);
			for (int i = 0; i < levelDetailList.Length; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				ResLevelCfgInfo dataByKey2 = GameDataMgr.levelDatabin.GetDataByKey((long)levelDetailList[i].iLevelID);
				DebugHelper.Assert(dataByKey2 != null, "Can't find LevelConfig = {0}", new object[]
				{
					levelDetailList[i].iLevelID
				});
				bool flag2 = levelDetailList[i].levelStatus == 0 || !flag;
				bool bActive = levelDetailList[i].levelStatus == 1 && flag;
				int starNum = CAdventureSys.GetStarNum(levelDetailList[i].bStarBits);
				elemenet.transform.FindChild("Unlock/star1").GetComponent<Image>().set_color((starNum >= 1) ? Color.white : CUIUtility.s_Color_GrayShader);
				elemenet.transform.FindChild("Unlock/star2").GetComponent<Image>().set_color((starNum >= 2) ? Color.white : CUIUtility.s_Color_GrayShader);
				elemenet.transform.FindChild("Unlock/star3").GetComponent<Image>().set_color((starNum >= 3) ? Color.white : CUIUtility.s_Color_GrayShader);
				elemenet.transform.FindChild("TxtLevelNameText").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey2.szName));
				if (levelNo == i + 1 && !flag2)
				{
					elemenet.transform.FindChild("TxtLevelNameText").GetComponent<Text>().set_color(CAdventureView.s_Adv_Level_Colors[0]);
				}
				else if (!flag2)
				{
					elemenet.transform.FindChild("TxtLevelNameText").GetComponent<Text>().set_color(CAdventureView.s_Adv_Level_Colors[1]);
				}
				else
				{
					elemenet.transform.FindChild("TxtLevelNameText").GetComponent<Text>().set_color(CAdventureView.s_Adv_Level_Colors[2]);
				}
				elemenet.transform.FindChild("SelectedFrame").GetComponent<Image>().set_color(CAdventureView.s_Adv_Difficult_Color[difficulty - 1]);
				elemenet.transform.FindChild("SelectedFrame/Image1").GetComponent<Image>().set_color(CAdventureView.s_Adv_Difficult_Color[CAdventureView.s_Adv_Difficult_Color.Length / 2 + difficulty - 1]);
				elemenet.transform.FindChild("SelectedFrame/Image2").GetComponent<Image>().set_color(CAdventureView.s_Adv_Difficult_Color[CAdventureView.s_Adv_Difficult_Color.Length / 2 + difficulty - 1]);
				elemenet.transform.FindChild("SelectedFrame/SelectedFrame").GetComponent<Image>().SetSprite(spritePrefeb, false);
				elemenet.transform.FindChild("New").gameObject.CustomSetActive(bActive);
				elemenet.GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Adv_SelectLevel;
				elemenet.GetComponent<CUIEventScript>().m_onClickEventParams.tag = i + 1;
				elemenet.transform.FindChild("Unlock").gameObject.CustomSetActive(!flag2);
				elemenet.transform.FindChild("Lock").gameObject.CustomSetActive(flag2);
				elemenet.m_selectedSprite = sprite;
				elemenet.GetComponent<Image>().SetSprite((levelNo - 1 == i) ? sprite : elemenet.m_defaultSprite, elemenet.m_selectedLayout);
			}
			component.SelectElement(levelNo - 1, true);
		}

		public static void InitDifficultList(CUIFormScript form, int currentChapter, int levelNo, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUIListScript component = form.transform.FindChild("DifficultList").GetComponent<CUIListScript>();
			string text = string.Empty;
			string prefabPath = string.Empty;
			int lEVEL_DIFFICULT_OPENED = CAdventureSys.LEVEL_DIFFICULT_OPENED;
			component.SetElementAmount(lEVEL_DIFFICULT_OPENED);
			for (int i = 0; i < lEVEL_DIFFICULT_OPENED; i++)
			{
				bool flag = CAdventureSys.IsDifOpen(currentChapter, i + 1);
				prefabPath = CAdventureView.GetDifficultIcon(i + 1);
				CUIListElementScript elemenet = component.GetElemenet(i);
				Image component2 = elemenet.transform.FindChild("DifficultImg").GetComponent<Image>();
				component2.SetSprite(prefabPath, form, true, false, false, false);
				component2.set_color(flag ? Color.white : CAdventureView.s_Adv_Difficulty_Gray_Color);
				elemenet.transform.FindChild("SelectedFrame").GetComponent<Image>().SetSprite(prefabPath, form, true, false, false, false);
				text = Singleton<CTextManager>.instance.GetText(string.Format("Adventure_Level_{0}", i + 1));
				elemenet.transform.FindChild("DifficultImg/DifficultText").GetComponent<Text>().set_text(text);
				elemenet.transform.FindChild("SelectedFrame/DifficultText").GetComponent<Text>().set_text(text);
				elemenet.GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Adv_SelectDifficult;
				elemenet.GetComponent<CUIEventScript>().m_onClickEventParams.tag = i + 1;
				elemenet.transform.FindChild("SelectedFrame/Frame_circle").GetComponent<Image>().set_color(CAdventureView.s_Adv_Difficult_Circle_Color[i]);
				PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[i];
				PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[currentChapter - 1];
				int chapterTotalStar = CAdventureSys.GetChapterTotalStar(pVE_CHAPTER_COMPLETE_INFO);
				elemenet.transform.FindChild("SelectedFrame/RewardBox").gameObject.CustomSetActive(chapterTotalStar == CAdventureSys.LEVEL_PER_CHAPTER * CAdventureSys.STAR_PER_LEVEL && pVE_CHAPTER_COMPLETE_INFO.bIsGetBonus == 0);
				elemenet.transform.FindChild("Lock").gameObject.CustomSetActive(!flag);
			}
			component.SelectElement(difficulty - 1, true);
		}

		public static void InitChapterElement(CUIFormScript formScript, int currentChapter, int levelNo, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[currentChapter - 1];
			PVE_LEVEL_COMPLETE_INFO[] levelDetailList = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList;
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)levelDetailList[levelNo - 1].iLevelID);
			if (dataByKey == null)
			{
				return;
			}
			formScript.transform.FindChild("ChapterElement/ChapterImg").GetComponent<Image>().SetSprite(CAdventureView.GetLevelBgPath(currentChapter, levelNo, difficulty), formScript, true, false, false, false);
			formScript.transform.FindChild("ChapterElement/ChapterNameText").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szName));
			formScript.transform.FindChild("ChapterElement/ChapterDEscText").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szLevelDesc));
			formScript.transform.FindChild("ChapterElement/RecPlayerLvlText").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Level_Recommend_Tips_1", new string[]
			{
				dataByKey.RecommendLevel[difficulty - 1].ToString()
			}));
			formScript.transform.FindChild("Bg").GetComponent<Image>().set_color(CAdventureView.s_Adv_Difficult_Bg_Color[difficulty - 1]);
		}

		public static void InitLevelForm(CUIFormScript formScript, int chapterNo, int LevelNo, int difficulty)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			PVE_ADV_COMPLETE_INFO pVE_ADV_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[difficulty - 1];
			PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = pVE_ADV_COMPLETE_INFO.ChapterDetailList[chapterNo - 1].LevelDetailList[LevelNo - 1];
			GameObject gameObject = formScript.gameObject;
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)pVE_LEVEL_COMPLETE_INFO.iLevelID);
			if (dataByKey != null)
			{
				string text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
				formScript.transform.Find("PanelLeft/DifficultText").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(string.Format("Adventure_Level_{0}", difficulty)));
				formScript.transform.Find("Panel_Main/ImgMapNameBg/MapNameText").GetComponent<Text>().set_text(text);
				formScript.transform.Find("PanelLeft/MapNameText").GetComponent<Text>().set_text(text);
				formScript.transform.Find("PanelLeft/MapDescText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref dataByKey.szLevelDesc));
				formScript.transform.Find("PanelLeft/RecPlayerLvlText").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Level_Recommend_Tips_1", new string[]
				{
					dataByKey.RecommendLevel[difficulty - 1].ToString()
				}));
				formScript.transform.Find("PanelLeft/ChapterImg").GetComponent<Image>().SetSprite(CAdventureView.GetLevelBgPath(chapterNo, LevelNo, difficulty), formScript, true, false, false, false);
				formScript.transform.Find("PanelLeft/DifficultImg").GetComponent<Image>().SetSprite(CAdventureView.GetDifficultIcon(difficulty), formScript, true, false, false, false);
				for (int i = 1; i <= CAdventureSys.STAR_PER_LEVEL; i++)
				{
					GameObject gameObject2 = gameObject.transform.Find("PanelRight/WinCondition" + i).gameObject;
					CAdventureView.SetStarConditionDesc(formScript, gameObject2, (uint)dataByKey.astStarDetail[i - 1].iParam, CAdventureSys.IsStarGained(pVE_LEVEL_COMPLETE_INFO.bStarBits, i));
				}
				GameObject gameObject3 = gameObject.transform.Find("PanelRight/itemCell").gameObject;
				CAdventureView.SetReward(formScript, gameObject3, dataByKey, difficulty);
				GameObject gameObject4 = gameObject.transform.Find("PanelRight/HeroList").gameObject;
				int num = 0;
				List<uint> heroListForBattleListID = Singleton<CHeroSelectBaseSystem>.instance.GetHeroListForBattleListID(dataByKey.dwBattleListID);
				CAdventureView.SetTeamHeroList(gameObject4, heroListForBattleListID, out num);
				GameObject gameObject5 = gameObject.transform.Find("BtnStart").gameObject;
				CAdventureView.SetStartBtnEnable(gameObject5, heroListForBattleListID);
				formScript.gameObject.transform.FindChild("Bg").gameObject.GetComponent<Image>().set_color(CAdventureView.s_Adv_Difficult_Bg_Color[difficulty - 1]);
			}
			else
			{
				DebugHelper.Assert(false, "Can't find level info -- id: {0}", new object[]
				{
					pVE_LEVEL_COMPLETE_INFO.iLevelID
				});
			}
		}

		private static string GetLevelBgPath(int ChapterId, int LevelNo, int difficult)
		{
			return string.Format("{0}{1}_{2}", CUIUtility.s_Sprite_Dynamic_Adventure_Dir, ChapterId.ToString("D2"), LevelNo.ToString("D2"));
		}

		private static string GetChapterBgPath(int ChapterId)
		{
			return string.Format("{0}Chapter_{1}", CUIUtility.s_Sprite_Dynamic_Adventure_Dir, ChapterId);
		}

		private static string GetLevelFramePath(int difficult)
		{
			return string.Format("{0}Adventure_Level_Frame_{1}", CUIUtility.s_Sprite_Dynamic_Adventure_Dir, difficult);
		}

		private static string GetLevelSelectFramePath(int difficult)
		{
			return string.Format("{0}Adventure_Level_Selected_Frame_{1}", CUIUtility.s_Sprite_Dynamic_Adventure_Dir, difficult);
		}

		private static string GetDifficultIcon(int difficult)
		{
			return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Adventure_Dir, ((RES_LEVEL_DIFFICULTY_TYPE)difficult).ToString());
		}

		public static void SetMopupLevelUp(uint oldLvl, uint newLvl)
		{
			CAdventureView.bLevelUp = true;
			CAdventureView.oldLevel = oldLvl;
			CAdventureView.newLevel = newLvl;
		}

		public static void CheckMopupLevelUp()
		{
			if (CAdventureView.bLevelUp)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = (int)CAdventureView.oldLevel;
				cUIEvent.m_eventParams.tag2 = (int)CAdventureView.newLevel;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				CAdventureView.bLevelUp = false;
				CAdventureView.oldLevel = (CAdventureView.newLevel = 0u);
			}
		}

		public static void OpenChapterRewardPanel(CUIFormScript formScript, GameObject root, int ChapterId, int difficulty, bool bCanGet)
		{
			GameObject gameObject = root.transform.Find("ChapterRewardPanel").gameObject;
			gameObject.CustomSetActive(true);
			ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey((uint)ChapterId);
			DebugHelper.Assert(dataByKey != null, "Can't find chapter config with ID: {0}", new object[]
			{
				ChapterId
			});
			ResDT_ChapterRewardInfo[] array = dataByKey.astNormalRewardDetail;
			if (difficulty == 1)
			{
				array = dataByKey.astNormalRewardDetail;
			}
			else if (difficulty == 2)
			{
				array = dataByKey.astEliteRewardDetail;
			}
			else if (difficulty == 3)
			{
				array = dataByKey.astMasterRewardDetail;
			}
			else if (difficulty == 4)
			{
				array = dataByKey.astAbyssRewardDetail;
			}
			DebugHelper.Assert(array != null, "Chapter RewardArr is NULL! -- ID: {0}, Difficulty: {1}", new object[]
			{
				ChapterId,
				difficulty
			});
			for (int i = 0; i < array.Length; i++)
			{
				ResDT_ChapterRewardInfo resDT_ChapterRewardInfo = array[i];
				GameObject gameObject2 = gameObject.transform.Find(string.Format("RewardCon/itemCell{0}", i + 1)).gameObject;
				if (resDT_ChapterRewardInfo.bType != 0)
				{
					gameObject2.CustomSetActive(true);
					CUseable itemUseable = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resDT_ChapterRewardInfo.bType, (int)resDT_ChapterRewardInfo.dwNum, resDT_ChapterRewardInfo.dwID);
					CUICommonSystem.SetItemCell(formScript, gameObject2, itemUseable, true, false, false, false);
				}
				else
				{
					gameObject2.CustomSetActive(false);
				}
			}
			GameObject gameObject3 = gameObject.transform.Find("BtnGetReward").gameObject;
			if (bCanGet)
			{
				gameObject3.GetComponentInChildren<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Can_Get_The_Box"));
				gameObject3.GetComponent<Button>().set_interactable(true);
			}
			else
			{
				gameObject3.GetComponentInChildren<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Not_Enough_Starts"));
				gameObject3.GetComponent<Button>().set_interactable(false);
			}
		}

		public static void CloseChapterRewardPanel(GameObject root)
		{
			GameObject gameObject = root.transform.Find("ChapterRewardPanel").gameObject;
			gameObject.CustomSetActive(false);
		}

		private static void SetStarConditionDesc(CUIFormScript formScript, GameObject descCon, uint conditionId, bool bAchieved)
		{
			Text component = descCon.transform.Find("TxtWinCondtion").gameObject.GetComponent<Text>();
			ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey(conditionId);
			DebugHelper.Assert(dataByKey != null, "Can't find star condition config with ID: {0}", new object[]
			{
				conditionId
			});
			component.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szCondDesc));
			component.set_color(bAchieved ? Color.green : Color.white);
			Image component2 = descCon.transform.Find("ImgStar").gameObject.GetComponent<Image>();
			component2.SetSprite("UGUI/Sprite/System/" + (bAchieved ? "Adventure/big_star" : "Adventure/empty_big_star"), formScript, true, false, false, false);
		}

		private static void SetReward(CUIFormScript formScript, GameObject itemCell, ResLevelCfgInfo resLevelInfo, int difficulty)
		{
			ResDT_PveRewardShowInfo resDT_PveRewardShowInfo = resLevelInfo.astRewardShowDetail[difficulty - 1];
			CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)resDT_PveRewardShowInfo.bRewardType, resDT_PveRewardShowInfo.dwRewardID, 0);
			if (cUseable != null)
			{
				CUICommonSystem.SetItemCell(formScript, itemCell, cUseable, true, false, false, false);
			}
		}

		private static void SetTeamHeroList(GameObject list, List<uint> heroIds, out int teamPower)
		{
			CUIListScript component = list.GetComponent<CUIListScript>();
			teamPower = 0;
			component.SetElementAmount(heroIds.get_Count());
			for (int i = 0; i < heroIds.get_Count(); i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				GameObject gameObject = elemenet.gameObject.transform.Find("heroItemCell").gameObject;
				if (heroIds.get_Item(i) > 0u)
				{
					IHeroData heroData = CHeroDataFactory.CreateHeroData(heroIds.get_Item(i));
					teamPower += heroData.combatEft;
					CUICommonSystem.SetHeroItemData(elemenet.m_belongedFormScript, gameObject, heroData, enHeroHeadType.enIcon, false, true);
					elemenet.gameObject.CustomSetActive(true);
					gameObject.gameObject.CustomSetActive(true);
				}
				else
				{
					elemenet.gameObject.CustomSetActive(false);
					gameObject.gameObject.CustomSetActive(false);
				}
			}
		}

		private static void SetTeamHeroList(GameObject list, COMDT_BATTLELIST_LIST battleList, uint battleListID, out int teamPower)
		{
			CUIListScript component = list.GetComponent<CUIListScript>();
			teamPower = 0;
			component.SetElementAmount(0);
			if (battleList == null || battleList.dwListNum == 0u)
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)battleList.dwListNum))
			{
				if (battleList.astBattleList[num].dwBattleListID == battleListID)
				{
					if (battleList.astBattleList[num].stBattleList.wHeroCnt == 0)
					{
						return;
					}
					component.SetElementAmount((int)battleList.astBattleList[num].stBattleList.wHeroCnt);
					int num2 = 0;
					for (int i = 0; i < (int)battleList.astBattleList[num].stBattleList.wHeroCnt; i++)
					{
						CUIListElementScript elemenet = component.GetElemenet(i);
						GameObject gameObject = elemenet.gameObject.transform.Find("heroItemCell").gameObject;
						if (battleList.astBattleList[num].stBattleList.BattleHeroList[i] > 0u)
						{
							IHeroData heroData = CHeroDataFactory.CreateHeroData(battleList.astBattleList[num].stBattleList.BattleHeroList[i]);
							teamPower += heroData.combatEft;
							CUICommonSystem.SetHeroItemData(elemenet.m_belongedFormScript, gameObject, heroData, enHeroHeadType.enIcon, false, true);
							elemenet.gameObject.CustomSetActive(true);
							gameObject.gameObject.CustomSetActive(true);
							num2++;
						}
						else
						{
							elemenet.gameObject.CustomSetActive(false);
							gameObject.gameObject.CustomSetActive(false);
						}
					}
					break;
				}
				else
				{
					num++;
				}
			}
		}

		private static void SetMopupEnable(GameObject Mopup, byte StarBitsMask, RES_LEVEL_DIFFICULTY_TYPE difficulty, int LeftPlayNum)
		{
			Button component = Mopup.GetComponent<Button>();
			bool isEnable;
			if (difficulty == RES_LEVEL_DIFFICULTY_TYPE.RES_LEVEL_DIFFICULTY_TYPE_NORMAL)
			{
				isEnable = (CAdventureSys.GetStarNum(StarBitsMask) == CAdventureSys.STAR_PER_LEVEL);
			}
			else
			{
				if (difficulty != RES_LEVEL_DIFFICULTY_TYPE.RES_LEVEL_DIFFICULTY_TYPE_NIGHTMARE)
				{
					DebugHelper.Assert(false, "Invalid difficulty -- {0}", new object[]
					{
						difficulty
					});
					return;
				}
				isEnable = (CAdventureSys.GetStarNum(StarBitsMask) == CAdventureSys.STAR_PER_LEVEL && LeftPlayNum > 0);
			}
			CUICommonSystem.SetButtonEnable(component, isEnable, true, true);
		}

		private static void SetMopupTenEnable(GameObject Mopup, byte StarBitsMask, RES_LEVEL_DIFFICULTY_TYPE difficulty, int LeftPlayNum)
		{
			Button component = Mopup.GetComponent<Button>();
			CUIEventScript component2 = component.gameObject.GetComponent<CUIEventScript>();
			stUIEventParams onClickEventParams = default(stUIEventParams);
			bool isEnable;
			if (difficulty == RES_LEVEL_DIFFICULTY_TYPE.RES_LEVEL_DIFFICULTY_TYPE_NORMAL)
			{
				Text componetInChild = Utility.GetComponetInChild<Text>(Mopup, "Text");
				componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Sweep_Number", new string[]
				{
					"10"
				}));
				isEnable = (CAdventureSys.GetStarNum(StarBitsMask) == CAdventureSys.STAR_PER_LEVEL && CAdventureView.isVip());
				onClickEventParams.tag = 10;
				component2.m_onClickEventParams = onClickEventParams;
			}
			else
			{
				if (difficulty != RES_LEVEL_DIFFICULTY_TYPE.RES_LEVEL_DIFFICULTY_TYPE_NIGHTMARE)
				{
					DebugHelper.Assert(false, "Invalid difficulty -- {0}", new object[]
					{
						difficulty
					});
					return;
				}
				Text componetInChild2 = Utility.GetComponetInChild<Text>(Mopup, "Text");
				if (LeftPlayNum > 0)
				{
					componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Sweep_Number", new string[]
					{
						LeftPlayNum.ToString()
					}));
				}
				else
				{
					componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Can_Not_Sweep"));
				}
				isEnable = (CAdventureSys.GetStarNum(StarBitsMask) == CAdventureSys.STAR_PER_LEVEL && CAdventureView.isVip() && LeftPlayNum > 0);
				onClickEventParams.tag = LeftPlayNum;
				component2.m_onClickEventParams = onClickEventParams;
			}
			CUICommonSystem.SetButtonEnable(component, isEnable, true, true);
		}

		private static void SetStartBtnEnable(GameObject Start, List<uint> heros)
		{
			Button component = Start.GetComponent<Button>();
			bool flag = heros != null && heros.get_Count() > 0;
			CUICommonSystem.SetButtonEnable(component, flag, flag, true);
			component.set_interactable(flag);
		}

		private static bool isVip()
		{
			return true;
		}

		public static string GetChapterName(int ChapterId)
		{
			ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey((uint)ChapterId);
			if (dataByKey != null)
			{
				return StringHelper.UTF8BytesToString(ref dataByKey.szChapterName);
			}
			return string.Empty;
		}

		private static string GetChapterIcon(int ChapterId)
		{
			ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey((uint)ChapterId);
			if (dataByKey != null)
			{
				return StringHelper.UTF8BytesToString(ref dataByKey.szChapterIcon);
			}
			return string.Empty;
		}

		private static void SetRewardItem(GameObject item, PVE_CHAPTER_COMPLETE_INFO chapterInfo, int stars, int chapterNo)
		{
			item.CustomSetActive(true);
			int num = CAdventureSys.LEVEL_PER_CHAPTER * CAdventureSys.STAR_PER_LEVEL;
			if (stars == num)
			{
				if (chapterInfo.bIsGetBonus > 0)
				{
					item.transform.FindChild("Unlock/RewardBox").GetComponent<Image>().set_color(Color.gray);
					item.transform.FindChild("Unlock/StarText").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("hasGot"));
				}
				else
				{
					item.transform.FindChild("Unlock/RewardBox").GetComponent<Image>().set_color(Color.white);
					item.transform.FindChild("Unlock/StarText").GetComponent<Text>().set_text(string.Format("{0}/{1}", stars, num));
				}
			}
			else
			{
				item.transform.FindChild("Unlock/RewardBox").GetComponent<Image>().set_color(Color.gray);
				item.transform.FindChild("Unlock/StarText").GetComponent<Text>().set_text(string.Format("{0}/{1}", stars, num));
			}
			item.transform.FindChild("Unlock/RewardBox").GetComponent<CUIEventScript>().m_onClickEventParams.tag = chapterNo + 1;
		}
	}
}
