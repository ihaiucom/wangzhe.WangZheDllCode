using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CSettlementView
	{
		public const int MAX_ACHIEVEMENT = 6;

		private const float expBarWidth = 327.6f;

		private const float proficientBarWidth = 205f;

		private const float TweenTime = 2f;

		private static LTDescr _coinLTD;

		private static LTDescr _expLTD;

		private static float _expFrom;

		private static float _expTo;

		private static float _coinFrom;

		private static float _coinTo;

		private static Text _coinTweenText;

		private static RectTransform _expTweenRect;

		private static GameObject _continueBtn;

		private static uint _lvUpGrade;

		public static void ShowData(CUIFormScript form)
		{
			GameObject gameObject = form.gameObject.transform.Find("PanelB/StatCon").gameObject;
			gameObject.CustomSetActive(true);
		}

		public static void HideData(CUIFormScript form)
		{
			GameObject gameObject = form.gameObject.transform.Find("PanelB/StatCon").gameObject;
			gameObject.CustomSetActive(false);
		}

		public static void SetTab(int index, GameObject root)
		{
			if (index == 0)
			{
				Utility.FindChild(root, "PanelA").CustomSetActive(true);
				Utility.FindChild(root, "PanelB").CustomSetActive(false);
			}
			else if (index == 1)
			{
				CSettlementView.DoCoinTweenEnd();
				CSettlementView.DoExpTweenEnd();
				Utility.FindChild(root, "PanelA").CustomSetActive(false);
				Utility.FindChild(root, "PanelB").CustomSetActive(true);
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.pvpFin, new uint[0]);
			}
		}

		private static void SetWin(GameObject root, bool bWin)
		{
			Utility.FindChild(root, "PanelA/WinOrLoseTitle/win").CustomSetActive(bWin);
			Utility.FindChild(root, "PanelA/WinOrLoseTitle/lose").CustomSetActive(!bWin);
			Utility.FindChild(root, "PanelB/WinOrLoseTitle/win").CustomSetActive(bWin);
			Utility.FindChild(root, "PanelB/WinOrLoseTitle/lose").CustomSetActive(!bWin);
		}

		private static void SetExpInfo(GameObject root, CUIFormScript formScript)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "can't find roleinfo");
			if (masterRoleInfo != null)
			{
				ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)masterRoleInfo.PvpLevel));
				DebugHelper.Assert(dataByKey != null, "can't find resexp id={0}", new object[]
				{
					masterRoleInfo.PvpLevel
				});
				if (dataByKey != null)
				{
					Text component = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpLevelTxt").GetComponent<Text>();
					component.set_text(string.Format("Lv.{0}", dataByKey.bLevel.ToString()));
					Text component2 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpTxt").GetComponent<Text>();
					Text component3 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/ExpMax").GetComponent<Text>();
					Text component4 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PlayerName").GetComponent<Text>();
					CUIHttpImageScript component5 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/HeadImage").GetComponent<CUIHttpImageScript>();
					Image component6 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/NobeIcon").GetComponent<Image>();
					Image component7 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/HeadFrame").GetComponent<Image>();
					if (!CSysDynamicBlock.bSocialBlocked)
					{
						string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
						component5.SetImageUrl(headUrl);
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component6, (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component7, (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
					}
					else
					{
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component6, 0, false, true, 0uL);
					}
					SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
					DebugHelper.Assert(curLvelContext != null, "Battle Level Context is NULL!!");
					GameObject gameObject = root.transform.Find("PanelA/Award/RankCon").gameObject;
					gameObject.CustomSetActive(false);
					if (curLvelContext.IsGameTypeLadder())
					{
						COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
						if (rankInfo != null)
						{
							gameObject.CustomSetActive(true);
							Text component8 = gameObject.transform.FindChild(string.Format("txtRankName", new object[0])).gameObject.GetComponent<Text>();
							Text component9 = gameObject.transform.FindChild(string.Format("WangZheXingTxt", new object[0])).gameObject.GetComponent<Text>();
							component8.set_text(StringHelper.UTF8BytesToString(ref CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bNowShowGrade).szGradeDesc));
							if ((int)CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bNowShowGrade).bLogicGrade == CLadderSystem.MAX_RANK_LEVEL)
							{
								Transform transform = gameObject.transform.FindChild(string.Format("XingGrid/ImgScore{0}", 1));
								if (transform != null)
								{
									transform.gameObject.CustomSetActive(true);
								}
								component9.gameObject.CustomSetActive(true);
								component9.set_text(string.Format("X{0}", rankInfo.dwNowScore));
							}
							else
							{
								component9.gameObject.CustomSetActive(false);
								int num = 1;
								while ((long)num <= (long)((ulong)rankInfo.dwNowScore))
								{
									Transform transform2 = gameObject.transform.FindChild(string.Format("XingGrid/ImgScore{0}", num));
									if (transform2 != null)
									{
										transform2.gameObject.CustomSetActive(true);
									}
									num++;
								}
							}
							root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpLevelNode").gameObject.CustomSetActive(false);
						}
					}
					Image component10 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/QQVIPIcon").GetComponent<Image>();
					MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component10);
					COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
					COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
					if (multiDetail != null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						int multiple = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseExp, ref multiDetail, 15, -1);
						if (multiple > 0)
						{
							COMDT_MULTIPLE_DATA[] array = null;
							uint multipleInfo = CUseable.GetMultipleInfo(out array, ref multiDetail, 15, -1);
							string[] array2 = new string[multipleInfo + 2u];
							string text = multiple.ToString();
							array2[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_1", new string[]
							{
								text
							});
							if (array != null)
							{
								int num2 = 0;
								while ((long)num2 < (long)((ulong)multipleInfo))
								{
									string text2 = string.Empty;
									if ((ulong)acntInfo.dwPvpSettleBaseExp * (ulong)((long)array[num2].iValue) > 0uL)
									{
										text2 = "+";
									}
									byte bOperator = array[num2].bOperator;
									if (bOperator != 0)
									{
										if (bOperator != 1)
										{
											text2 += "0";
										}
										else
										{
											text2 += (long)((ulong)acntInfo.dwPvpSettleBaseExp * (ulong)((long)array[num2].iValue) / 10000uL);
										}
									}
									else
									{
										text2 += array[num2].iValue;
									}
									switch (array[num2].iType)
									{
									case 1:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", new string[]
										{
											text2
										});
										break;
									case 2:
										if (masterRoleInfo.HasVip(16))
										{
											array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", new string[]
											{
												text2
											});
										}
										else
										{
											array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", new string[]
											{
												text2
											});
										}
										break;
									case 3:
										array2[num2 + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_4"), text2, masterRoleInfo.GetExpWinCount(), Math.Ceiling((double)((float)masterRoleInfo.GetExpExpireHours() / 24f)));
										break;
									case 4:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", new string[]
										{
											masterRoleInfo.dailyPvpCnt.ToString(),
											text2
										});
										break;
									case 5:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", new string[]
										{
											text2
										});
										break;
									case 6:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_15", new string[]
										{
											text2
										});
										break;
									case 7:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", new string[]
										{
											text2
										});
										break;
									case 8:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", new string[]
										{
											text2
										});
										break;
									case 9:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", new string[]
										{
											text2
										});
										break;
									case 10:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", new string[]
										{
											text2
										});
										break;
									case 11:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", new string[]
										{
											text2
										});
										break;
									case 12:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", new string[]
										{
											text2
										});
										break;
									case 13:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", new string[]
										{
											text2
										});
										break;
									case 14:
										array2[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", new string[]
										{
											text2
										});
										break;
									}
									num2++;
								}
							}
							stringBuilder.Append(array2[0]);
							for (int i = 1; i < array2.Length; i++)
							{
								if (!string.IsNullOrEmpty(array2[i]))
								{
									stringBuilder.Append("\n");
									stringBuilder.Append(array2[i]);
								}
							}
							GameObject gameObject2 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/DoubleExp").gameObject;
							gameObject2.CustomSetActive(true);
							gameObject2.GetComponentInChildren<Text>().set_text(string.Format("+{0}", text));
							CUICommonSystem.SetCommonTipsEvent(formScript, gameObject2, stringBuilder.ToString(), enUseableTipsPos.enLeft);
						}
						else
						{
							GameObject gameObject3 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/DoubleExp").gameObject;
							gameObject3.CustomSetActive(false);
						}
						GameObject gameObject4 = root.transform.Find("PanelA/Award/ItemAndCoin/Panel_Gold/GoldMax").gameObject;
						if (Singleton<BattleStatistic>.GetInstance().acntInfo.bReachDailyLimit > 0)
						{
							gameObject4.CustomSetActive(true);
						}
						else
						{
							gameObject4.CustomSetActive(false);
						}
						int multiple2 = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref multiDetail, 11, -1);
						if (multiple2 > 0)
						{
							COMDT_MULTIPLE_DATA[] array3 = null;
							uint multipleInfo2 = CUseable.GetMultipleInfo(out array3, ref multiDetail, 11, -1);
							string[] array4 = new string[multipleInfo2 + 2u];
							string text3 = multiple2.ToString();
							array4[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_7", new string[]
							{
								text3
							});
							if (array3 != null)
							{
								int num3 = 0;
								while ((long)num3 < (long)((ulong)multipleInfo2))
								{
									string text4 = string.Empty;
									if ((ulong)acntInfo.dwPvpSettleBaseCoin * (ulong)((long)array3[num3].iValue) > 0uL)
									{
										text4 = "+";
									}
									byte bOperator2 = array3[num3].bOperator;
									if (bOperator2 != 0)
									{
										if (bOperator2 != 1)
										{
											text4 += "0";
										}
										else
										{
											text4 += (long)((ulong)acntInfo.dwPvpSettleBaseCoin * (ulong)((long)array3[num3].iValue) / 10000uL);
										}
									}
									else
									{
										text4 += array3[num3].iValue;
									}
									switch (array3[num3].iType)
									{
									case 1:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", new string[]
										{
											text4
										});
										break;
									case 2:
										if (masterRoleInfo.HasVip(16))
										{
											array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", new string[]
											{
												text4
											});
										}
										else
										{
											array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", new string[]
											{
												text4
											});
										}
										break;
									case 3:
										array4[num3 + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_10"), text4, masterRoleInfo.GetCoinWinCount(), Math.Ceiling((double)((float)masterRoleInfo.GetCoinExpireHours() / 24f)));
										break;
									case 4:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", new string[]
										{
											masterRoleInfo.dailyPvpCnt.ToString(),
											text4
										});
										break;
									case 5:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", new string[]
										{
											text4
										});
										break;
									case 6:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_15", new string[]
										{
											text4
										});
										break;
									case 7:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", new string[]
										{
											text4
										});
										break;
									case 8:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", new string[]
										{
											text4
										});
										break;
									case 9:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", new string[]
										{
											text4
										});
										break;
									case 10:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", new string[]
										{
											text4
										});
										break;
									case 11:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", new string[]
										{
											text4
										});
										break;
									case 12:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", new string[]
										{
											text4
										});
										break;
									case 13:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", new string[]
										{
											text4
										});
										break;
									case 14:
										array4[num3 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", new string[]
										{
											text4
										});
										break;
									}
									num3++;
								}
							}
							stringBuilder.Append(array4[0]);
							for (int j = 1; j < array4.Length; j++)
							{
								if (!string.IsNullOrEmpty(array4[j]))
								{
									stringBuilder.Append("\n");
									stringBuilder.Append(array4[j]);
								}
							}
							stringBuilder.Remove(0, stringBuilder.get_Length());
							stringBuilder.Append(array4[0]);
							for (int k = 1; k < array4.Length; k++)
							{
								if (!string.IsNullOrEmpty(array4[k]))
								{
									stringBuilder.Append("\n");
									stringBuilder.Append(array4[k]);
								}
							}
							GameObject gameObject5 = root.transform.Find("PanelA/Award/ItemAndCoin/Panel_Gold/DoubleCoin").gameObject;
							gameObject5.CustomSetActive(true);
							gameObject5.GetComponentInChildren<Text>().set_text(string.Format("+{0}", text3));
							CUICommonSystem.SetCommonTipsEvent(formScript, gameObject5, stringBuilder.ToString(), enUseableTipsPos.enLeft);
						}
						else
						{
							GameObject gameObject6 = root.transform.Find("PanelA/Award/ItemAndCoin/Panel_Gold/DoubleCoin").gameObject;
							gameObject6.CustomSetActive(false);
						}
					}
					component4.set_text(masterRoleInfo.Name);
					RectTransform component11 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
					RectTransform component12 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
					if (acntInfo != null)
					{
						if (acntInfo.dwPvpSettleExp > 0u)
						{
							Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
						}
						int num4 = (int)(acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
						if (num4 < 0)
						{
							CSettlementView._lvUpGrade = acntInfo.dwPvpLv;
						}
						else
						{
							CSettlementView._lvUpGrade = 0u;
						}
						float num5 = Mathf.Max(0f, (float)num4 / dataByKey.dwNeedExp);
						float num6 = Mathf.Max(0f, ((num4 < 0) ? acntInfo.dwPvpExp : acntInfo.dwPvpSettleExp) / dataByKey.dwNeedExp);
						root.transform.FindChild("PanelA/Award/Panel_PlayerExp/PvpExpNode/AddPvpExpTxt").GetComponent<Text>().set_text((acntInfo.dwPvpSettleExp > 0u) ? string.Format("+{0}", acntInfo.dwPvpSettleExp).ToString() : string.Empty);
						if (acntInfo.dwPvpSettleExp == 0u)
						{
							root.transform.FindChild("PanelA/Award/Panel_PlayerExp/PvpExpNode/Bar2").gameObject.CustomSetActive(false);
						}
						component11.sizeDelta = new Vector2(num5 * 327.6f, component11.sizeDelta.y);
						component12.sizeDelta = new Vector2(num5 * 327.6f, component12.sizeDelta.y);
						CSettlementView._expFrom = num5;
						CSettlementView._expTo = num5 + num6;
						CSettlementView._expTweenRect = component12;
						component11.gameObject.CustomSetActive(num4 >= 0);
						component3.set_text((acntInfo.bExpDailyLimit > 0) ? Singleton<CTextManager>.GetInstance().GetText("GetExp_Limit") : string.Empty);
						component2.set_text(string.Format("{0}/{1}", acntInfo.dwPvpExp.ToString(), dataByKey.dwNeedExp.ToString()));
					}
				}
			}
		}

		public static void DoCoinTweenEnd()
		{
			if (CSettlementView._coinLTD != null && CSettlementView._coinTweenText != null)
			{
				CSettlementView._coinTweenText.set_text(string.Format("+{0}", CSettlementView._coinTo.ToString("N0")));
				COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
				if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
				{
					CUICommonSystem.AppendMultipleText(CSettlementView._coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
				}
				CSettlementView._coinLTD.cancel();
				CSettlementView._coinLTD = null;
				CSettlementView._coinTweenText = null;
			}
			if (CSettlementView._continueBtn != null)
			{
				CSettlementView._continueBtn.CustomSetActive(true);
				CSettlementView._continueBtn = null;
			}
		}

		private static void DoExpTweenEnd()
		{
			if (CSettlementView._expTweenRect != null && CSettlementView._expLTD != null)
			{
				CSettlementView._expTweenRect.sizeDelta = new Vector2(CSettlementView._expTo * 327.6f, CSettlementView._expTweenRect.sizeDelta.y);
				CSettlementView._expLTD.cancel();
				CSettlementView._expLTD = null;
				CSettlementView._expTweenRect = null;
			}
			if (CSettlementView._continueBtn != null)
			{
				CSettlementView._continueBtn.CustomSetActive(true);
				CSettlementView._continueBtn = null;
			}
			if (CSettlementView._lvUpGrade > 1u)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = (int)(CSettlementView._lvUpGrade - 1u);
				cUIEvent.m_eventParams.tag2 = (int)CSettlementView._lvUpGrade;
				CUIEvent uiEvent = cUIEvent;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
			CSettlementView._lvUpGrade = 0u;
		}

		private static void SetHeroStat_Share(CUIFormScript formScript, GameObject item, HeroKDA kda, bool bSelf, bool bMvp, bool bWin)
		{
			Text componetInChild = Utility.GetComponetInChild<Text>(item, "Txt_PlayerLevel");
			componetInChild.set_text(string.Format("Lv.{0}", kda.SoulLevel.ToString()));
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)kda.HeroId);
			DebugHelper.Assert(dataByKey != null);
			item.transform.Find("Txt_HeroName").gameObject.GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
			string text = (kda.numKill < 10) ? string.Format(" {0} ", kda.numKill.ToString()) : kda.numKill.ToString();
			string text2 = (kda.numDead < 10) ? string.Format(" {0} ", kda.numDead.ToString()) : kda.numDead.ToString();
			string text3 = (kda.numAssist < 10) ? string.Format(" {0}", kda.numAssist.ToString()) : kda.numAssist.ToString();
			item.transform.Find("Txt_KDA").gameObject.GetComponent<Text>().set_text(string.Format("{0} / {1} / {2}", text, text2, text3));
			Image component = item.transform.Find("KillerImg").gameObject.GetComponent<Image>();
			component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint)kda.HeroId, 0u)), formScript, true, false, false, false);
			GameObject gameObject = item.transform.Find("Mvp").gameObject;
			gameObject.CustomSetActive(bMvp);
			if (bMvp)
			{
				Image component2 = gameObject.GetComponent<Image>();
				if (bWin)
				{
					component2.SetSprite(CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Red_Mvp", formScript, true, false, false, false);
					component2.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
				}
				else
				{
					component2.SetSprite(CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Blue_Mvp", formScript, true, false, false, false);
					component2.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
				}
			}
			for (int i = 0; i < 5; i++)
			{
				uint dwTalentID = kda.TalentArr[i].dwTalentID;
				Image component3 = item.transform.FindChild(string.Format("TianFu/TianFuIcon{0}", (i + 1).ToString())).GetComponent<Image>();
				if (dwTalentID == 0u)
				{
					component3.gameObject.CustomSetActive(false);
				}
				else
				{
					component3.gameObject.CustomSetActive(true);
					ResTalentLib dataByKey2 = GameDataMgr.talentLib.GetDataByKey(dwTalentID);
					component3.SetSprite(CUIUtility.s_Sprite_Dynamic_Talent_Dir + dataByKey2.dwIcon, formScript, true, false, false, false);
				}
			}
			int num = 1;
			for (int j = 1; j < 13; j++)
			{
				switch (j)
				{
				case 1:
					if (kda.LegendaryNum > 0)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.Legendary, num);
						num++;
					}
					break;
				case 2:
					if (kda.PentaKillNum > 0)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.PentaKill, num);
						num++;
					}
					break;
				case 3:
					if (kda.QuataryKillNum > 0)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.QuataryKill, num);
						num++;
					}
					break;
				case 4:
					if (kda.TripleKillNum > 0)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.TripleKill, num);
						num++;
					}
					break;
				case 5:
					if (kda.DoubleKillNum > 0)
					{
					}
					break;
				case 6:
					if (kda.bKillMost)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.KillMost, num);
						num++;
					}
					break;
				case 7:
					if (kda.bHurtMost && kda.hurtToEnemy > 0)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.HurtMost, num);
						num++;
					}
					break;
				case 8:
					if (kda.bHurtTakenMost && kda.hurtTakenByEnemy > 0)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.HurtTakenMost, num);
						num++;
					}
					break;
				case 9:
					if (kda.bAsssistMost)
					{
						CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.AsssistMost, num);
						num++;
					}
					break;
				}
			}
			for (int k = num; k <= 6; k++)
			{
				CSettlementView.SetAchievementIcon(formScript, item, PvpAchievement.NULL, k);
			}
		}

		public static void SetAchievementIcon(CUIFormScript formScript, GameObject item, PvpAchievement type, int count)
		{
			if (count <= 6)
			{
				Image component = Utility.FindChild(item, string.Format("Achievement/Image{0}", count)).GetComponent<Image>();
				if (type == PvpAchievement.NULL)
				{
					component.gameObject.CustomSetActive(false);
				}
				else
				{
					string prefabPath = CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + type.ToString();
					component.gameObject.CustomSetActive(true);
					component.SetSprite(prefabPath, formScript, true, false, false, false);
				}
			}
		}
	}
}
