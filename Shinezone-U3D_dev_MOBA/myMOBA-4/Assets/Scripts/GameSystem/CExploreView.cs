using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CExploreView
	{
		private static float lastScrollX = 0f;

		public static readonly enUIEventID[] s_eventIDs = new enUIEventID[]
		{
			enUIEventID.Arena_OpenForm,
			enUIEventID.Adv_OpenChapterForm,
			enUIEventID.Burn_OpenForm
		};

		public static readonly RES_SPECIALFUNCUNLOCK_TYPE[] s_unlockTypes = new RES_SPECIALFUNCUNLOCK_TYPE[]
		{
			RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA,
			RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_NONE,
			RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG
		};

		public static readonly string[] s_exploreTypes = new string[]
		{
			"Explore_Common_Type_2",
			"Explore_Common_Type_1",
			"Explore_Common_Type_3"
		};

		public static readonly Color[] s_exploreColors = new Color[]
		{
			new Color(1f, 0f, 0.847058833f),
			new Color(0f, 0.627451f, 1f),
			new Color(1f, 0f, 0.0431372561f)
		};

		public static void RefreshExploreList()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CAdventureSys.EXLPORE_FORM_PATH);
			if (form != null)
			{
				CExploreView.InitExloreList(form);
			}
		}

		public static void InitExloreList(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			int num = CExploreView.s_eventIDs.Length;
			CUIStepListScript component = form.transform.Find("ExploreList").gameObject.GetComponent<CUIStepListScript>();
			component.SetElementAmount(num);
			for (int i = 0; i < num; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				CUIEventScript component2 = elemenet.GetComponent<CUIEventScript>();
				component2.m_onClickEventID = CExploreView.s_eventIDs[i];
				Text component3 = elemenet.gameObject.transform.Find("TitleBg/ExlporeNameText").GetComponent<Text>();
				component3.text = Singleton<CTextManager>.instance.GetText(CExploreView.s_exploreTypes[i]);
				Image component4 = elemenet.gameObject.transform.Find("TitleBg/Image").GetComponent<Image>();
				component4.color = CExploreView.s_exploreColors[i];
				Image component5 = elemenet.gameObject.transform.Find("Icon").gameObject.GetComponent<Image>();
				GameObject spritePrefeb = CUIUtility.GetSpritePrefeb(CUIUtility.s_Sprite_Dynamic_Adventure_Dir + (i + 1), false, false);
				if (spritePrefeb != null)
				{
					component5.SetSprite(spritePrefeb, false);
				}
				GameObject gameObject = elemenet.transform.FindChild("Lock").gameObject;
				GameObject gameObject2 = elemenet.transform.FindChild("Unlock").gameObject;
				RES_SPECIALFUNCUNLOCK_TYPE rES_SPECIALFUNCUNLOCK_TYPE = CExploreView.s_unlockTypes[i];
				if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(rES_SPECIALFUNCUNLOCK_TYPE))
				{
					component5.color = CUIUtility.s_Color_White;
					gameObject.CustomSetActive(false);
				}
				else
				{
					component5.color = CUIUtility.s_Color_GrayShader;
					gameObject.CustomSetActive(true);
					ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)rES_SPECIALFUNCUNLOCK_TYPE);
					if (dataByKey != null)
					{
						gameObject.GetComponentInChildren<Text>().text = Utility.UTF8Convert(dataByKey.szLockedTip);
					}
				}
				if (CExploreView.s_unlockTypes[i] == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_NONE)
				{
					int lastChapter = CAdventureSys.GetLastChapter(1);
					ResChapterInfo dataByKey2 = GameDataMgr.chapterInfoDatabin.GetDataByKey((long)lastChapter);
					if (dataByKey2 != null)
					{
						gameObject2.CustomSetActive(true);
						gameObject2.GetComponentInChildren<Text>().text = string.Format(Singleton<CTextManager>.instance.GetText("Adventure_Chapter_Max_Tips"), Utility.UTF8Convert(dataByKey2.szChapterName));
					}
				}
				else if (CExploreView.s_unlockTypes[i] == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA)
				{
					if (Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList == null || Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList.stArenaInfo.dwSelfRank == 0u)
					{
						gameObject2.CustomSetActive(false);
					}
					else
					{
						string text = string.Empty;
						text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExploreArenaRankText"), Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList.stArenaInfo.dwSelfRank);
						gameObject2.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = text;
						gameObject2.CustomSetActive(true);
					}
				}
				else if (CExploreView.s_unlockTypes[i] == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG)
				{
					BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
					if (model._data == null)
					{
						gameObject2.CustomSetActive(false);
					}
					else
					{
						string text2 = string.Empty;
						if (model.IsAllCompelte())
						{
							text2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExploreBurnFinishText"), new object[0]);
						}
						else
						{
							text2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExploreBurnText"), Math.Max(1, model.Get_LastUnlockLevelIndex(model.curDifficultyType) + 1));
						}
						gameObject2.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = text2;
						gameObject2.CustomSetActive(true);
					}
				}
			}
			component.SelectElementImmediately(1);
			Text component6 = form.gameObject.transform.FindChild("AwardGroup/Name1").gameObject.GetComponent<Text>();
			Text component7 = form.gameObject.transform.FindChild("AwardGroup/Name2").gameObject.GetComponent<Text>();
			Image component8 = form.gameObject.transform.FindChild("AwardGroup/Icon1").gameObject.GetComponent<Image>();
			Image component9 = form.gameObject.transform.FindChild("AwardGroup/Icon2").gameObject.GetComponent<Image>();
			component6.gameObject.CustomSetActive(false);
			component7.gameObject.CustomSetActive(false);
			component8.gameObject.CustomSetActive(false);
			component9.gameObject.CustomSetActive(false);
			uint num2 = 0u;
			string empty = string.Empty;
			try
			{
				num2 = uint.Parse(Singleton<CTextManager>.GetInstance().GetText("ArenaAwardHeroId"));
			}
			catch (Exception)
			{
			}
			if (num2 != 0u)
			{
				ResHeroCfgInfo dataByKey3 = GameDataMgr.heroDatabin.GetDataByKey(num2);
				if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsHaveHero(num2, false) && dataByKey3 != null)
				{
					component6.gameObject.CustomSetActive(true);
					component6.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ArenaAwardHero"), dataByKey3.szName);
					component8.gameObject.CustomSetActive(true);
					component8.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(num2, 0u)), form, true, false, false, false);
				}
			}
			num2 = 0u;
			try
			{
				num2 = uint.Parse(Singleton<CTextManager>.GetInstance().GetText("BurningAwardHeroId"));
			}
			catch (Exception)
			{
			}
			if (num2 != 0u)
			{
				ResHeroCfgInfo dataByKey4 = GameDataMgr.heroDatabin.GetDataByKey(num2);
				if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsHaveHero(num2, false) && dataByKey4 != null)
				{
					component7.gameObject.CustomSetActive(true);
					component7.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("BurningAwardHero"), dataByKey4.szName);
					component9.gameObject.CustomSetActive(true);
					component9.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(num2, 0u)), form, true, false, false, false);
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform = form.transform.FindChild("AwardGroup");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		public static void OnExploreListScroll(GameObject root)
		{
			CUIListScript component = root.transform.Find("ExploreList").gameObject.GetComponent<CUIListScript>();
			if (component != null)
			{
				Vector2 contentSize = component.GetContentSize();
				Vector2 scrollAreaSize = component.GetScrollAreaSize();
				Vector2 contentPosition = component.GetContentPosition();
				Vector2 zero = Vector2.zero;
				zero.x = ((contentSize.x != scrollAreaSize.x) ? (contentPosition.x / (contentSize.x - scrollAreaSize.x)) : 0f);
				float num = zero.x - CExploreView.lastScrollX;
				CExploreView.lastScrollX = zero.x;
				Transform transform = root.transform.Find("FW_MovePanel/textureFrame");
				float zAngle = (num != 0f) ? (num / (1f / (float)(CAdventureSys.CHAPTER_NUM - 1)) * 120f) : 0f;
				transform.Rotate(0f, 0f, zAngle);
			}
		}
	}
}
