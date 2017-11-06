using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CLadderView
	{
		public const int MAX_RECENT_GAME_SHOW_NUM = 10;

		public const int MAX_MOST_USED_HERO_NUM = 4;

		public static string RANK_ICON_PATH = "UGUI/Sprite/Dynamic/Ladder/";

		public static string RANK_SMALL_ICON_PATH = "UGUI/Sprite/Dynamic/Ladder_Small/";

		public static string LADDER_IMG_PATH = "UGUI/Sprite/System/Ladder/";

		public static string LADDER_IMG_STAR = CLadderView.LADDER_IMG_PATH + "Img_CompetitiveRace_Staropen.prefab";

		public static string LADDER_IMG_STAR_EMPTY = CLadderView.LADDER_IMG_PATH + "Img_CompetitiveRace_Staroff.prefab";

		public static Color s_Bg_Color_Shrink = new Color(0.137254909f, 0.180392161f, 0.282352954f, 0.5f);

		public static Color s_Bg_Color_Expand = new Color(0.31764707f, 0.380392164f, 0.56078434f, 0.5f);

		public static void InitLadderEntry(CUIFormScript form, ref COMDT_RANKDETAIL data, bool isQualified)
		{
			Transform transform = form.transform.Find("MainPanel/BtnGroup/SingleStart");
			Transform transform2 = form.transform.Find("MainPanel/BtnGroup/DoubleStart");
			Transform transform3 = form.transform.Find("MainPanel/BtnGroup/FiveStart");
			Button button = null;
			Button button2 = null;
			Button button3 = null;
			if (transform)
			{
				button = transform.GetComponent<Button>();
			}
			if (transform2)
			{
				button2 = transform2.GetComponent<Button>();
			}
			if (transform3)
			{
				button3 = transform3.GetComponent<Button>();
			}
			GameObject widget = form.GetWidget(11);
			widget.CustomSetActive(isQualified);
			if (isQualified)
			{
				form.transform.Find("ReqPanel").gameObject.CustomSetActive(false);
				form.transform.Find("MainPanel/ImgLock").gameObject.CustomSetActive(false);
				form.transform.Find("MainPanel/RankCon").gameObject.CustomSetActive(true);
				GameObject gameObject = form.transform.Find("MainPanel/RankCon").gameObject;
				if (data != null)
				{
					if (button)
					{
						CUICommonSystem.SetButtonEnableWithShader(button, data.bState == 1, true);
					}
					if (button2)
					{
						CUICommonSystem.SetButtonEnableWithShader(button2, data.bState == 1, true);
						if (CLadderSystem.MultiLadderMaxTeamerNum() == 2)
						{
							CUICommonSystem.SetTextContent(transform2.FindChild("Text"), Singleton<CTextManager>.GetInstance().GetText("Ladder_EntryBtn_Text1"));
						}
						else
						{
							CUICommonSystem.SetTextContent(transform2.FindChild("Text"), Singleton<CTextManager>.GetInstance().GetText("Ladder_EntryBtn_Text2"));
						}
					}
					if (button3)
					{
						CUICommonSystem.SetButtonEnableWithShader(button3, data.bState == 1, true);
					}
					CLadderView.ShowRankDetail(gameObject, ref data, false);
					CUIParticleScript component = form.GetWidget(16).GetComponent<CUIParticleScript>();
					component.LoadRes(CLadderView.GetGradeParticleBgResName());
				}
				else
				{
					if (button)
					{
						CUICommonSystem.SetButtonEnableWithShader(button, false, true);
					}
					if (button2)
					{
						CUICommonSystem.SetButtonEnableWithShader(button2, false, true);
					}
					if (button3)
					{
						CUICommonSystem.SetButtonEnableWithShader(button3, false, true);
					}
				}
			}
			else
			{
				if (button)
				{
					CUICommonSystem.SetButtonEnableWithShader(button, false, true);
				}
				if (button2)
				{
					CUICommonSystem.SetButtonEnableWithShader(button2, false, true);
				}
				if (button3)
				{
					CUICommonSystem.SetButtonEnableWithShader(button3, false, true);
				}
				form.transform.Find("ReqPanel").gameObject.CustomSetActive(true);
				form.transform.Find("MainPanel/ImgLock").gameObject.CustomSetActive(true);
				form.transform.Find("MainPanel/RankCon").gameObject.CustomSetActive(false);
				Text component2 = form.transform.Find("ReqPanel/txtHeroNum").GetComponent<Text>();
				Text component3 = form.transform.Find("ReqPanel/txtReqHeroNum").GetComponent<Text>();
				int num = 0;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					num = masterRoleInfo.GetHaveHeroCount(false);
				}
				component2.set_text(string.Format("{0}/{1}", num, CLadderSystem.REQ_HERO_NUM));
				component3.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Ladder_Req_Hero_Num"), CLadderSystem.REQ_HERO_NUM.ToString()));
			}
			CLadderView.ShowBraveScorePanel(form, data, isQualified);
			CLadderView.ShowRewardPanel(form, data);
			CLadderView.ShowBpModePanel(form);
			CLadderView.ShowSuperKingRankPanel(form);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				form.transform.FindChild("MainPanel/pnlRankingBtn").gameObject.CustomSetActive(false);
			}
		}

		private static void ShowBraveScorePanel(CUIFormScript form, COMDT_RANKDETAIL data, bool isShow)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			if (!isShow)
			{
				widget.CustomSetActive(false);
				return;
			}
			widget.CustomSetActive(true);
			if (data == null)
			{
				return;
			}
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankGrade);
			Image component = form.GetWidget(1).GetComponent<Image>();
			Text component2 = form.GetWidget(2).GetComponent<Text>();
			Text component3 = form.GetWidget(3).GetComponent<Text>();
			Transform transform = form.transform.FindChild("BottomPanel/BravePanel/imgKeDu");
			GameObject widget2 = form.GetWidget(15);
			uint dwAddScoreOfConWinCnt = data.dwAddScoreOfConWinCnt;
			uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
			component.set_fillAmount(CLadderView.GetProcessCircleFillAmount((int)dwAddScoreOfConWinCnt, (int)selfBraveScoreMax));
			component2.set_text(dwAddScoreOfConWinCnt + "/" + selfBraveScoreMax);
			component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", new string[]
			{
				selfBraveScoreMax.ToString()
			}));
			transform.rotation = CLadderView.GetImgKeDuRotation(gradeDataByShowGrade.dwProtectGradeScore, selfBraveScoreMax);
			Text component4 = form.transform.FindChild("BottomPanel/BravePanel/txtBaoJi").GetComponent<Text>();
			if (dwAddScoreOfConWinCnt >= gradeDataByShowGrade.dwProtectGradeScore)
			{
				component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_KeepGrade_Txt2"));
				component.set_color(CUIUtility.s_Color_BraveScore_BaojiKedu_On);
			}
			else
			{
				component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_KeepGrade_Txt1", new string[]
				{
					(gradeDataByShowGrade.dwProtectGradeScore - dwAddScoreOfConWinCnt).ToString()
				}));
				component.set_color(CUIUtility.s_Color_BraveScore_BaojiKedu_Off);
			}
			if (data.dwContinuousWin > 0u)
			{
				widget2.CustomSetActive(true);
				Text component5 = widget2.GetComponent<Text>();
				component5.set_text(data.dwContinuousWin + Singleton<CTextManager>.GetInstance().GetText("Common_Continues_Win"));
			}
			else
			{
				widget2.CustomSetActive(false);
			}
		}

		public static Quaternion GetImgKeDuRotation(float baojiScore, float MaxScore)
		{
			Quaternion result = default(Quaternion);
			float num = MaxScore / 2f;
			float z = (num - baojiScore) / num * 142f;
			result.eulerAngles = new Vector3(0f, 0f, z);
			return result;
		}

		private static void ShowRewardPanel(CUIFormScript form, COMDT_RANKDETAIL data)
		{
			if (data == null)
			{
				return;
			}
			Text component = form.GetWidget(5).GetComponent<Text>();
			uint num;
			if (data.bMaxSeasonGrade > 0)
			{
				num = (uint)data.bMaxSeasonGrade;
			}
			else
			{
				num = (uint)CLadderSystem.GetGradeDataByLogicGrade(1).bGrade;
			}
			component.set_text(Singleton<CLadderSystem>.GetInstance().GetRewardDesc((byte)num));
			GameObject widget = form.GetWidget(12);
			CUseable skinRewardUseable = Singleton<CLadderSystem>.GetInstance().GetSkinRewardUseable();
			CUICommonSystem.SetItemCell(form, widget, skinRewardUseable, true, false, false, false);
			GameObject widget2 = form.GetWidget(14);
			widget2.CustomSetActive(Singleton<CLadderSystem>.GetInstance().IsGotSkinReward());
			CLadderView.ShowSeasonEndRewardPanel(form);
		}

		public static float GetProcessCircleFillAmount(int target, int total)
		{
			if (total == 0)
			{
				return 0f;
			}
			double num = 0.085;
			double num2 = 1.0 - num * 2.0;
			return (float)(num + (double)((float)target / (float)total) * num2);
		}

		private static void ShowBpModePanel(CUIFormScript form)
		{
			GameObject widget = form.GetWidget(7);
			GameObject widget2 = form.GetWidget(8);
			GameObject widget3 = form.GetWidget(9);
			GameObject widget4 = form.GetWidget(6);
			if (Singleton<CLadderSystem>.GetInstance().IsUseBpMode())
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(true);
				widget4.CustomSetActive(true);
			}
			else
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget4.CustomSetActive(false);
			}
		}

		private static void ShowSuperKingRankPanel(CUIFormScript form)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			bool flag = CLadderView.IsSuperKing(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass);
			GameObject widget = form.GetWidget(10);
			widget.CustomSetActive(flag);
			if (flag)
			{
				CUICommonSystem.SetRankDisplay(masterRoleInfo.m_rankClass, widget.transform);
			}
		}

		public static void SetGameInfoRecentPanel(CUIFormScript form, COMDT_RANKDETAIL rankDetail, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
		{
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(14);
			if (rankDetail != null && dataList != null && dataList.get_Count() > 0 && rankDetail.dwSeasonIdx == dataList.get_Item(0).dwSeasonId)
			{
				Text component = form.GetWidget(7).GetComponent<Text>();
				Text component2 = form.GetWidget(8).GetComponent<Text>();
				COMDT_RANK_CURSEASON_FIGHT_RECORD cOMDT_RANK_CURSEASON_FIGHT_RECORD = dataList.get_Item(0);
				CLadderView.SetWinLose(component, ref cOMDT_RANK_CURSEASON_FIGHT_RECORD);
				component2.set_text(CLadderView.GetGameTime(ref cOMDT_RANK_CURSEASON_FIGHT_RECORD));
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(cOMDT_RANK_CURSEASON_FIGHT_RECORD.dwHeroId);
				if (dataByKey != null)
				{
					Image component3 = form.GetWidget(3).GetComponent<Image>();
					component3.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false, false);
					form.GetWidget(6).CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 5);
					form.GetWidget(16).CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 4);
					form.GetWidget(17).CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 3);
					form.GetWidget(18).CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 2);
					GameObject gameObject = form.GetWidget(4).gameObject;
					gameObject.CustomSetActive(Convert.ToBoolean(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bIsBanPick));
				}
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
			}
			else
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
			}
		}

		public static void SetGameInfoSeasonPanel(CUIFormScript form, COMDT_RANKDETAIL rankdetail)
		{
			if (rankdetail == null)
			{
				return;
			}
			Text component = form.GetWidget(10).GetComponent<Text>();
			Text component2 = form.GetWidget(9).GetComponent<Text>();
			Text component3 = form.GetWidget(11).GetComponent<Text>();
			component.set_text(rankdetail.dwTotalFightCnt.ToString());
			component2.set_text(rankdetail.dwTotalWinCnt.ToString());
			component3.set_text(rankdetail.dwMaxContinuousWinCnt.ToString());
		}

		public static void SetGameInfoSeasonTimePanel(CUIFormScript form, COMDT_RANKDETAIL rankdetail)
		{
			Text component = form.GetWidget(15).GetComponent<Text>();
			Text text = component;
			text.set_text(text.get_text() + " " + CLadderView.GetSeasonNameWithBracket((ulong)rankdetail.dwSeasonStartTime));
			Text component2 = form.GetWidget(12).GetComponent<Text>();
			component2.set_text(CLadderView.GetSeasonText(ref rankdetail));
		}

		public static void InitKingForm(CUIFormScript form, ref COMDT_RANKDETAIL data)
		{
			GameObject gameObject = form.transform.Find("RankCon").gameObject;
			CLadderView.ShowRankDetail(gameObject, ref data, false);
		}

		public static void InitRewardForm(CUIFormScript form, ref COMDT_RANKDETAIL data)
		{
			GameObject gameObject = form.transform.Find("RankCon").gameObject;
			CLadderView.ShowRankDetail(gameObject, ref data, true);
		}

		private static void ShowSeasonEndRewardPanel(CUIFormScript form)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			uint key;
			if (masterRoleInfo.m_rankSeasonHighestGrade > 0)
			{
				key = (uint)masterRoleInfo.m_rankSeasonHighestGrade;
			}
			else
			{
				key = (uint)CLadderSystem.GetGradeDataByLogicGrade(1).bGrade;
			}
			ResRankRewardConf dataByKey = GameDataMgr.rankRewardDatabin.GetDataByKey(key);
			if (dataByKey != null)
			{
				ListView<CUseable> listView = new ListView<CUseable>();
				for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
				{
					ResDT_ChapterRewardInfo resDT_ChapterRewardInfo = dataByKey.astRewardDetail[i];
					if (resDT_ChapterRewardInfo.bType != 0)
					{
						CUseable cUseable = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resDT_ChapterRewardInfo.bType, (int)resDT_ChapterRewardInfo.dwNum, resDT_ChapterRewardInfo.dwID);
						if (cUseable != null)
						{
							listView.Add(cUseable);
						}
					}
				}
				if (listView.Count > 0)
				{
					GameObject widget = form.GetWidget(13);
					CUICommonSystem.SetItemCell(form, widget, listView[0], true, false, false, false);
				}
			}
		}

		public static void ShowSeasonEndGetRewardForm(byte grade)
		{
			ResRankRewardConf dataByKey = GameDataMgr.rankRewardDatabin.GetDataByKey((uint)grade);
			if (dataByKey != null)
			{
				ListView<CUseable> listView = new ListView<CUseable>();
				for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
				{
					ResDT_ChapterRewardInfo resDT_ChapterRewardInfo = dataByKey.astRewardDetail[i];
					if (resDT_ChapterRewardInfo.bType != 0)
					{
						CUseable cUseable = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resDT_ChapterRewardInfo.bType, (int)resDT_ChapterRewardInfo.dwNum, resDT_ChapterRewardInfo.dwID);
						if (cUseable != null)
						{
							listView.Add(cUseable);
						}
					}
				}
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(listView), Singleton<CTextManager>.GetInstance().GetText("Ladder_Season_Reward"), false, enUIEventID.Ladder_ReqGetSeasonReward, false, false, "Form_Award");
			}
		}

		public static void InitLadderGameInfo(CUIFormScript gameInfoForm, COMDT_RANKDETAIL rankDetail, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
		{
			CLadderView.SetGameInfoRecentPanel(gameInfoForm, rankDetail, dataList);
			CLadderView.SetGameInfoSeasonPanel(gameInfoForm, rankDetail);
			CLadderView.SetGameInfoSeasonTimePanel(gameInfoForm, rankDetail);
		}

		public static void InitLadderHistory(CUIFormScript form, List<COMDT_RANK_PASTSEASON_FIGHT_RECORD> dataList)
		{
			CUIListScript component = form.transform.Find("ExpandList").GetComponent<CUIListScript>();
			if (dataList == null)
			{
				component.SetElementAmount(0);
			}
			else
			{
				component.SetElementAmount(dataList.get_Count());
				for (int i = 0; i < dataList.get_Count(); i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					COMDT_RANK_PASTSEASON_FIGHT_RECORD cOMDT_RANK_PASTSEASON_FIGHT_RECORD = dataList.get_Item(i);
					Text component2 = elemenet.transform.Find("Title/txtLeagueTime").GetComponent<Text>();
					Text component3 = elemenet.transform.Find("Title/txtRankTitle").GetComponent<Text>();
					Text component4 = elemenet.transform.Find("Title/txtHeroes").GetComponent<Text>();
					Text component5 = elemenet.transform.Find("Expand/txtGameNum").GetComponent<Text>();
					Text component6 = elemenet.transform.Find("Expand/txtWinNum").GetComponent<Text>();
					Text component7 = elemenet.transform.Find("Expand/txtWinRate").GetComponent<Text>();
					Text component8 = elemenet.transform.Find("Expand/txtContiWinNum").GetComponent<Text>();
					component2.set_text(CLadderView.GetSeasonNameWithBracket((ulong)cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwSeaStartTime) + " " + CLadderView.GetSeasonText(ref cOMDT_RANK_PASTSEASON_FIGHT_RECORD));
					component3.set_text(CLadderView.GetRankName(ref cOMDT_RANK_PASTSEASON_FIGHT_RECORD));
					List<COMDT_RANK_COMMON_USED_HERO> list = new List<COMDT_RANK_COMMON_USED_HERO>();
					component4.set_text(CLadderView.GetTopUseHeroNames(ref cOMDT_RANK_PASTSEASON_FIGHT_RECORD, out list));
					component5.set_text(cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwTotalFightCnt.ToString());
					component6.set_text(cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwTotalWinCnt.ToString());
					component7.set_text((cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwTotalFightCnt > 0u) ? string.Format("{0}%", (cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwTotalWinCnt * 100f / cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwTotalFightCnt).ToString("0.00")) : "0.00%");
					component8.set_text(cOMDT_RANK_PASTSEASON_FIGHT_RECORD.dwMaxContinuousWinCnt.ToString());
					int num = (list.get_Count() > 4) ? 4 : list.get_Count();
					for (int j = 0; j < num; j++)
					{
						Transform transform = elemenet.transform.Find(string.Format("Expand/Hero{0}", j + 1));
						transform.gameObject.CustomSetActive(true);
						COMDT_RANK_COMMON_USED_HERO cOMDT_RANK_COMMON_USED_HERO = list.get_Item(j);
						CLadderView.SetMostUsedHero(transform, ref cOMDT_RANK_COMMON_USED_HERO, form);
					}
					for (int k = num; k < 4; k++)
					{
						Transform transform2 = elemenet.transform.Find(string.Format("Expand/Hero{0}", k + 1));
						transform2.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public static void InitLadderRecent(CUIFormScript form, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
		{
			CUIListScript component = form.transform.Find("Root/List").GetComponent<CUIListScript>();
			if (dataList == null)
			{
				component.SetElementAmount(0);
			}
			else
			{
				int num = (dataList.get_Count() < 10) ? dataList.get_Count() : 10;
				component.SetElementAmount(num);
				for (int i = 0; i < num; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					COMDT_RANK_CURSEASON_FIGHT_RECORD cOMDT_RANK_CURSEASON_FIGHT_RECORD = dataList.get_Item(i);
					DebugHelper.Assert(cOMDT_RANK_CURSEASON_FIGHT_RECORD != null);
					Image component2 = elemenet.transform.Find("imageIcon").GetComponent<Image>();
					Text component3 = elemenet.transform.Find("txtResult").GetComponent<Text>();
					Text component4 = elemenet.transform.Find("txtBraveScore").GetComponent<Text>();
					Text component5 = elemenet.transform.Find("txtTime").GetComponent<Text>();
					Text component6 = elemenet.transform.Find("txtKDA").GetComponent<Text>();
					CLadderView.SetWinLose(component3, ref cOMDT_RANK_CURSEASON_FIGHT_RECORD);
					component4.set_text(cOMDT_RANK_CURSEASON_FIGHT_RECORD.dwAddStarScore.ToString());
					component5.set_text(CLadderView.GetGameTime(ref cOMDT_RANK_CURSEASON_FIGHT_RECORD));
					component6.set_text(string.Format("{0} / {1} / {2}", cOMDT_RANK_CURSEASON_FIGHT_RECORD.dwKillNum, cOMDT_RANK_CURSEASON_FIGHT_RECORD.dwDeadNum, cOMDT_RANK_CURSEASON_FIGHT_RECORD.dwAssistNum));
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(cOMDT_RANK_CURSEASON_FIGHT_RECORD.dwHeroId);
					if (dataByKey != null)
					{
						component2.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false, false);
						Utility.FindChild(component2.gameObject, "FiveFriend").CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 5);
						Utility.FindChild(component2.gameObject, "FourFriend").CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 4);
						Utility.FindChild(component2.gameObject, "ThreeFriend").CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 3);
						Utility.FindChild(component2.gameObject, "TwoFriend").CustomSetActive(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bTeamerNum == 2);
						Utility.FindChild(component2.gameObject, "Bp").CustomSetActive(Convert.ToBoolean(cOMDT_RANK_CURSEASON_FIGHT_RECORD.bIsBanPick));
					}
					for (int j = 0; j < 6; j++)
					{
						COMDT_INGAME_EQUIP_INFO cOMDT_INGAME_EQUIP_INFO = null;
						if (j < (int)cOMDT_RANK_CURSEASON_FIGHT_RECORD.bEquipNum)
						{
							cOMDT_INGAME_EQUIP_INFO = cOMDT_RANK_CURSEASON_FIGHT_RECORD.astEquipDetail[j];
						}
						Image component7 = elemenet.transform.FindChild(string.Format("TianFu/TianFuIcon{0}", (j + 1).ToString())).GetComponent<Image>();
						if (cOMDT_INGAME_EQUIP_INFO == null || cOMDT_INGAME_EQUIP_INFO.dwEquipID == 0u)
						{
							component7.gameObject.CustomSetActive(false);
						}
						else
						{
							component7.gameObject.CustomSetActive(true);
							CUICommonSystem.SetEquipIcon((ushort)cOMDT_INGAME_EQUIP_INFO.dwEquipID, component7.gameObject, form);
						}
					}
				}
			}
		}

		public static void OnHistoryItemChange(GameObject go, bool bExpand)
		{
			Image component = go.transform.Find("Bg").GetComponent<Image>();
			if (component)
			{
				component.set_color(bExpand ? CLadderView.s_Bg_Color_Expand : CLadderView.s_Bg_Color_Shrink);
			}
			Transform transform = go.transform.Find("Title/Button");
			if (transform)
			{
				if (bExpand)
				{
					(transform as RectTransform).rotation = Quaternion.Euler(0f, 0f, 180f);
				}
				else
				{
					(transform as RectTransform).rotation = Quaternion.Euler(0f, 0f, 0f);
				}
			}
		}

		public static void ShowRankDetail(GameObject go, ref COMDT_RANKDETAIL rankDetail, bool isShowSeasonHighestGrade = false)
		{
			DebugHelper.Assert(rankDetail != null, "Rank Data must not be null!!!");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && rankDetail != null)
			{
				if (isShowSeasonHighestGrade)
				{
					if (!Singleton<CLadderSystem>.GetInstance().IsValidGrade((int)masterRoleInfo.m_rankSeasonHighestGrade))
					{
						throw new Exception("Not valid rank highest season grade: " + masterRoleInfo.m_rankSeasonHighestGrade);
					}
					CLadderView.ShowRankDetail(go, masterRoleInfo.m_rankSeasonHighestGrade, masterRoleInfo.m_rankSeasonHighestClass, rankDetail.dwScore, true, false, true, false, true);
				}
				else
				{
					CLadderView.ShowRankDetail(go, masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass, rankDetail.dwScore, true, false, false, false, true);
				}
			}
		}

		public static void ShowRankDetail(GameObject go, byte rankGrade, uint rankClass, uint score, bool bShowScore = true, bool useSmall = false, bool isLadderRewardForm = false, bool isUseSpecialColorWhenSuperKing = false, bool isImgSamll = true)
		{
			DebugHelper.Assert(go != null, "GameObject is NULL!!!");
			Image image = go.transform.Find("ImgRank") ? go.transform.Find("ImgRank").GetComponent<Image>() : null;
			Image image2 = go.transform.Find("ImgSubRank") ? go.transform.Find("ImgSubRank").GetComponent<Image>() : null;
			Text text = go.transform.Find("txtRankName") ? go.transform.Find("txtRankName").GetComponent<Text>() : null;
			Text text2 = go.transform.Find("txtTopGroupScore") ? go.transform.Find("txtTopGroupScore").GetComponent<Text>() : null;
			byte bLogicGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade).bLogicGrade;
			if (image)
			{
				string rankIconPath = CLadderView.GetRankIconPath(rankGrade, rankClass);
				string text3 = "{0}";
				if (isImgSamll)
				{
					text3 += "_small";
				}
				image.SetSprite((!useSmall) ? rankIconPath : string.Format(text3, rankIconPath), null, true, false, false, false);
				image.gameObject.CustomSetActive(true);
			}
			if (image2)
			{
				if (isLadderRewardForm && (int)bLogicGrade >= CLadderSystem.MAX_RANK_LEVEL)
				{
					image2.gameObject.CustomSetActive(false);
				}
				else
				{
					image2.SetSprite(CLadderView.GetSubRankIconPath(rankGrade, rankClass), null, true, false, false, false);
					image2.gameObject.CustomSetActive(true);
				}
			}
			if (text)
			{
				text.set_text(CLadderView.GetRankName(rankGrade, rankClass));
				if (isUseSpecialColorWhenSuperKing && CLadderView.IsSuperKing(rankGrade, rankClass))
				{
					text.set_text("<color=#feba29>" + text.get_text() + "</color>");
				}
			}
			if (text2)
			{
				if ((int)bLogicGrade >= CLadderSystem.MAX_RANK_LEVEL)
				{
					text2.set_text(string.Format("x{0}", score));
				}
				text2.gameObject.CustomSetActive((int)bLogicGrade >= CLadderSystem.MAX_RANK_LEVEL);
			}
			Transform transform = go.transform.Find("ScoreCon");
			if (!transform)
			{
				return;
			}
			if ((int)bLogicGrade >= CLadderSystem.MAX_RANK_LEVEL || !bShowScore)
			{
				transform.gameObject.CustomSetActive(false);
			}
			else
			{
				transform.Find("Con3Star").gameObject.CustomSetActive(false);
				transform.Find("Con4Star").gameObject.CustomSetActive(false);
				transform.Find("Con5Star").gameObject.CustomSetActive(false);
				ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
				if (gradeDataByShowGrade != null)
				{
					Transform transform2 = transform.Find(string.Format("Con{0}Star", gradeDataByShowGrade.dwGradeUpNeedScore));
					if (transform2)
					{
						transform2.gameObject.CustomSetActive(true);
						int num = 1;
						while ((long)num <= (long)((ulong)gradeDataByShowGrade.dwGradeUpNeedScore))
						{
							Image component = transform2.Find(string.Format("ImgScore{0}", num)).GetComponent<Image>();
							string prefabPath = ((ulong)score >= (ulong)((long)num)) ? CLadderView.LADDER_IMG_STAR : CLadderView.LADDER_IMG_STAR_EMPTY;
							if (component)
							{
								component.SetSprite(prefabPath, null, true, false, false, false);
							}
							num++;
						}
					}
				}
				transform.gameObject.CustomSetActive(true);
			}
		}

		public static void ShowRankDetail(GameObject go, byte rankGrade, uint rankClass, bool isGrey = false, bool isUseSmall = false)
		{
			DebugHelper.Assert(go != null, "GameObject is NULL!!!");
			DebugHelper.Assert(rankGrade > 0, "grade must be above 0!!!");
			Image image = go.transform.Find("ImgRank") ? go.transform.Find("ImgRank").GetComponent<Image>() : null;
			Image image2 = go.transform.Find("ImgSubRank") ? go.transform.Find("ImgSubRank").GetComponent<Image>() : null;
			Text text = go.transform.Find("txtRankName") ? go.transform.Find("txtRankName").GetComponent<Text>() : null;
			if (image)
			{
				string rankIconPath = CLadderView.GetRankIconPath(rankGrade, rankClass);
				image.SetSprite((!isUseSmall) ? rankIconPath : string.Format("{0}_small", rankIconPath), null, true, false, false, false);
				image.gameObject.CustomSetActive(true);
				CUIUtility.SetImageGrey(image, isGrey);
			}
			if (image2)
			{
				image2.SetSprite(CLadderView.GetSubRankIconPath(rankGrade, rankClass), null, true, false, false, false);
				image2.gameObject.CustomSetActive(true);
				CUIUtility.SetImageGrey(image2, isGrey);
			}
			if (text)
			{
				text.set_text(CLadderView.GetRankName(rankGrade, rankClass));
				CUIUtility.SetImageGrey(text, isGrey);
			}
		}

		public static bool IsSuperMaster(byte rankGrade, uint rankClass)
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(205u);
			return dataByKey != null && (int)CLadderSystem.GetGradeDataByShowGrade((int)rankGrade).bLogicGrade == CLadderSystem.MAX_RANK_LEVEL && (rankClass == 0u || rankClass > dataByKey.dwConfValue);
		}

		public static bool IsSuperKing(byte rankGrade, uint rankClass)
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(205u);
			return dataByKey != null && (int)CLadderSystem.GetGradeDataByShowGrade((int)rankGrade).bLogicGrade == CLadderSystem.MAX_RANK_LEVEL && 0u < rankClass && rankClass <= dataByKey.dwConfValue;
		}

		public static string GetGradeParticleBgResName()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankGrade);
				if (gradeDataByShowGrade != null)
				{
					if (CLadderView.IsSuperKing(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass))
					{
						return gradeDataByShowGrade.szGradeParticleBgSuperMaster;
					}
					return gradeDataByShowGrade.szGradeParticleBg;
				}
			}
			return string.Empty;
		}

		private static void SetMostUsedHero(Transform item, ref COMDT_RANK_COMMON_USED_HERO data, CUIFormScript formScript)
		{
			Text component = item.Find("txtGameNum").GetComponent<Text>();
			Text component2 = item.Find("txtWinNum").GetComponent<Text>();
			component.set_text(data.dwFightCnt.ToString());
			component2.set_text(data.dwWinCnt.ToString());
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
			if (dataByKey != null)
			{
				Image component3 = item.Find("heroItemCell/imageIcon").GetComponent<Image>();
				component3.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), formScript, true, false, false, false);
			}
		}

		private static string GetSeasonText(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data)
		{
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)data.dwSeaStartTime));
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)data.dwSeaEndTime));
			string text = Singleton<CTextManager>.GetInstance().GetText("ladder_season_duration");
			return string.Format(text, new object[]
			{
				dateTime.get_Year(),
				dateTime.get_Month(),
				dateTime2.get_Year(),
				dateTime2.get_Month()
			});
		}

		private static string GetSeasonText(ref COMDT_RANKDETAIL data)
		{
			if (data.bState == 1)
			{
				DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)data.dwSeasonStartTime));
				DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)data.dwSeasonEndTime));
				string text = Singleton<CTextManager>.GetInstance().GetText("ladder_season_duration");
				return string.Format(text, new object[]
				{
					dateTime.get_Year(),
					dateTime.get_Month(),
					dateTime2.get_Year(),
					dateTime2.get_Month()
				});
			}
			return "赛季还未开始";
		}

		private static string GetSeasonNameWithBracket(ulong time)
		{
			string ladderSeasonName = Singleton<CLadderSystem>.GetInstance().GetLadderSeasonName(time);
			return string.IsNullOrEmpty(ladderSeasonName) ? string.Empty : ("(" + ladderSeasonName + ")");
		}

		private static string GetRankName(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data)
		{
			return CLadderView.GetRankName(data.bGrade, data.dwClassOfRank);
		}

		public static string GetRankName(byte rankGrade, uint rankClass)
		{
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
			if (gradeDataByShowGrade == null)
			{
				return string.Empty;
			}
			if (CLadderView.IsSuperMaster(rankGrade, rankClass))
			{
				return Singleton<CTextManager>.GetInstance().GetText("Ladder_Super_Master");
			}
			return StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradeDesc);
		}

		public static string GetRankIconPath(byte rankGrade, uint rankClass)
		{
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
			if (gradeDataByShowGrade == null)
			{
				return string.Empty;
			}
			if (CLadderView.IsSuperMaster(rankGrade, rankClass))
			{
				return CLadderView.RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradePicturePathSuperMaster);
			}
			return CLadderView.RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradePicturePath);
		}

		public static string GetRankSmallIconPath(byte rankGrade, uint rankClass)
		{
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
			if (gradeDataByShowGrade == null)
			{
				return string.Empty;
			}
			if (CLadderView.IsSuperMaster(rankGrade, rankClass))
			{
				return CLadderView.RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradePicturePathSuperMaster);
			}
			return CLadderView.RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradePicturePath);
		}

		public static string GetSubRankIconPath(byte rankGrade, uint rankClass)
		{
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
			if (gradeDataByShowGrade == null)
			{
				return string.Empty;
			}
			if (CLadderView.IsSuperMaster(rankGrade, rankClass))
			{
				return CLadderView.RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradeSmallPicPathSuperMaster);
			}
			return CLadderView.RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradeSmallPicPath);
		}

		public static string GetSubRankSmallIconPath(byte rankGrade, uint rankClass)
		{
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
			if (gradeDataByShowGrade == null)
			{
				return string.Empty;
			}
			if (CLadderView.IsSuperMaster(rankGrade, rankClass))
			{
				return CLadderView.RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradeSmallPicPathSuperMaster);
			}
			return CLadderView.RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradeSmallPicPath);
		}

		public static string GetRankFrameIconPath(byte rankGrade, uint rankClass)
		{
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankGrade);
			if (gradeDataByShowGrade == null || string.IsNullOrEmpty(gradeDataByShowGrade.szGradeFramePicPath))
			{
				return string.Empty;
			}
			if (CLadderView.IsSuperMaster(rankGrade, rankClass))
			{
				return CLadderView.RANK_ICON_PATH + gradeDataByShowGrade.szGradeFramePicPathSuperMaster;
			}
			return CLadderView.RANK_ICON_PATH + gradeDataByShowGrade.szGradeFramePicPath;
		}

		public static string GetRankIconPath()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				return CLadderView.GetRankIconPath(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass);
			}
			return string.Empty;
		}

		public static string GetRankBigGradeName(byte rankBigGrade)
		{
			ResRankGradeConf resRankGradeConf = GameDataMgr.rankGradeDatabin.FindIf((ResRankGradeConf x) => x.bBelongBigGrade == rankBigGrade);
			if (resRankGradeConf == null)
			{
				return string.Empty;
			}
			if (rankBigGrade == 6)
			{
				return Singleton<CTextManager>.GetInstance().GetText("Ladder_Super_Master");
			}
			return resRankGradeConf.szBigGradeName;
		}

		private static string GetTopUseHeroNames(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data, out List<COMDT_RANK_COMMON_USED_HERO> heroList)
		{
			heroList = new List<COMDT_RANK_COMMON_USED_HERO>();
			int num = 0;
			while ((long)num < (long)((ulong)data.dwCommonUsedHeroNum))
			{
				if (data.astCommonUsedHeroInfo[num].dwHeroId != 0u)
				{
					heroList.Add(data.astCommonUsedHeroInfo[num]);
				}
				num++;
			}
			heroList.Sort(new Comparison<COMDT_RANK_COMMON_USED_HERO>(CLadderView.ComparisonHeroData));
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = (heroList.get_Count() > 4) ? 4 : heroList.get_Count();
			for (int i = 0; i < num2; i++)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroList.get_Item(i).dwHeroId);
				if (dataByKey != null)
				{
					stringBuilder.Append(StringHelper.UTF8BytesToString(ref dataByKey.szName));
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		private static void SetWinLose(Text Result, ref COMDT_RANK_CURSEASON_FIGHT_RECORD data)
		{
			if (data.dwGameResult == 1u)
			{
				Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Win"));
			}
			else if (data.dwGameResult == 2u)
			{
				Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Lose"));
			}
			else if (data.dwGameResult == 3u)
			{
				Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Draw"));
			}
			else
			{
				Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Null"));
			}
		}

		private static string GetGameTime(ref COMDT_RANK_CURSEASON_FIGHT_RECORD data)
		{
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)data.dwFightTime));
			string text = Singleton<CTextManager>.GetInstance().GetText("GameTime_Template");
			return string.Format(text, new object[]
			{
				dateTime.get_Month().ToString("00"),
				dateTime.get_Day().ToString("00"),
				dateTime.get_Hour().ToString("00"),
				dateTime.get_Minute().ToString("00")
			});
		}

		private static int ComparisonHeroData(COMDT_RANK_COMMON_USED_HERO a, COMDT_RANK_COMMON_USED_HERO b)
		{
			if (a.dwFightCnt > b.dwFightCnt)
			{
				return -1;
			}
			if (a.dwFightCnt < b.dwFightCnt)
			{
				return 1;
			}
			return 0;
		}

		public static void ShowRankButtonIn5(CUIFormScript form, bool show)
		{
			if (form)
			{
				Transform transform = form.transform.Find("MainPanel/BtnGroup/FiveStart");
				if (transform)
				{
					transform.gameObject.CustomSetActive(show);
				}
			}
		}
	}
}
