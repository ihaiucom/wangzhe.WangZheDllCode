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
	[MessageHandlerClass]
	public class CArenaSystem : Singleton<CArenaSystem>
	{
		public const int s_arenaRankMaxNum = 50;

		public static string s_arenaFormPath = "UGUI/Form/System/Arena/Form_Arena.prefab";

		public static string s_arenaPlayerInfoFormPath = "UGUI/Form/System/Arena/Form_Arena_PlayerInfo.prefab";

		public static string s_arenaFightRecordFormPath = "UGUI/Form/System/Arena/Form_Arena_FightRecord.prefab";

		public static string s_arenaRankInfoFormPath = "UGUI/Form/System/Arena/Form_Arena_RankInfo.prefab";

		public static string s_arenaFightResultFormPath = "UGUI/Form/System/Arena/Form_Arena_Result.prefab";

		public static string s_arenaRankChangeFormPath = "UGUI/Form/System/Arena/Form_Arena_RankChange.prefab";

		public static int s_Arena_RULE_ID = 5;

		public static int s_mapKey = 30601;

		public ulong m_tarObjID;

		public byte m_tarIndex;

		public COMDT_ARENA_MEMBER_OF_ACNT m_tarInfo;

		public uint m_tarRank;

		public byte m_tarType;

		public int m_nextCanFightTimes;

		public SCPKG_CHGARENAFIGHTERRSP m_fightHeroInfoList;

		public COMDT_ARENA_FIGHTER_INFO m_rankInfoList;

		public ListView<COMDT_ARENA_FIGHT_RECORD> m_recordList = new ListView<COMDT_ARENA_FIGHT_RECORD>();

		public CSDT_ACNT_ARENADATA m_serverInfo;

		public uint m_lastRankRequestTime;

		public uint m_lastRecordRequestTime;

		public uint m_lastFighterInfoRequestTime;

		private bool m_openArenaForm;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_TeamConfig, new CUIEventManager.OnUIEventHandler(this.Arena_TeamConfig));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ChangeHeroList, new CUIEventManager.OnUIEventHandler(this.Arena_ChangeHeroList));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_BuyChangeTimes, new CUIEventManager.OnUIEventHandler(this.Arena_BuyChangeTimes));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ResetCD, new CUIEventManager.OnUIEventHandler(this.Arena_ResetCD));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ConfirmBuyChangeTimes, new CUIEventManager.OnUIEventHandler(this.Arena_ConfirmBuyChangeTimes));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ConfirmBuyResetCD, new CUIEventManager.OnUIEventHandler(this.Arena_ConfirmResetCD));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_CDTimeEnd, new CUIEventManager.OnUIEventHandler(this.Arena_CDTimeEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenPlayerInfoForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenPlayerInfoForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenRankInfoForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenRankInfoForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_RankElementEnable, new CUIEventManager.OnUIEventHandler(CArenaSystem.Arena_RankElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenRecordForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenRecordForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_RecordElementEnable, new CUIEventManager.OnUIEventHandler(this.Arena_RecordlementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenShopForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenShopForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_StartFight, new CUIEventManager.OnUIEventHandler(this.Arena_StartFight));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_RankInfoMenuClick, new CUIEventManager.OnUIEventHandler(this.Arena_RankInfoMenuClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ReciveDefTeamInfo, new CUIEventManager.OnUIEventHandler(this.Arena_ReciveDefTeamInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ResetByNewDay, new CUIEventManager.OnUIEventHandler(this.Arena_ResetByNewDay));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ConfirmResutlForm, new CUIEventManager.OnUIEventHandler(this.Arena_ConfirmResutlForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenTips, new CUIEventManager.OnUIEventHandler(this.Arena_OpenTips));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_CloseTps, new CUIEventManager.OnUIEventHandler(this.Arena_CloseTps));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OnClose, new CUIEventManager.OnUIEventHandler(this.Arena_OnCloseForm));
			CArenaSystem.s_mapKey = GameDataMgr.arenaLevelDatabin.GetAnyData().iCfgID;
		}

		public int GetLessFightTimes()
		{
			return (int)this.m_serverInfo.bChallengeLimitCnt - (int)this.m_serverInfo.chAlreadyFightCnt;
		}

		public void InitServerData(CSDT_ACNT_ARENADATA serverData)
		{
			this.m_serverInfo = serverData;
			this.m_nextCanFightTimes = (int)((long)CRoleInfo.GetElapseSecondsSinceLogin() + (long)((ulong)this.m_serverInfo.dwLeftFightCoolTime));
		}

		public void Reset()
		{
			this.m_tarObjID = 0uL;
			this.m_fightHeroInfoList = null;
			this.m_rankInfoList = null;
		}

		private void Arena_OpenForm(CUIEvent uiEvent)
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA))
			{
				if (!Singleton<SCModuleControl>.instance.GetActiveModule(COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_ARENA))
				{
					Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
					return;
				}
				if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
					return;
				}
				if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Count == 0)
				{
					CArenaSystem.SendJoinArenaMSG();
					return;
				}
				MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(15, true, false);
				this.Reset();
				CArenaSystem.SendGetFightHeroListMSG(true);
				this.m_openArenaForm = true;
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterArena, new uint[0]);
			}
			else
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(9u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			}
		}

		public void RefreshArenaForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaFormPath);
			if (form == null)
			{
				return;
			}
			CUITimerScript component = form.gameObject.transform.Find("Timer").gameObject.GetComponent<CUITimerScript>();
			component.SetTotalTime((float)((ulong)this.m_serverInfo.dwNextRefreshLeftTime - (ulong)((long)CRoleInfo.GetElapseSecondsSinceLogin())));
			component.StartTimer();
			CUIListScript component2 = form.gameObject.transform.Find("Root/panelTop/List").gameObject.GetComponent<CUIListScript>();
			Text component3 = form.gameObject.transform.Find("Root/panelTop/lblFightValue").gameObject.GetComponent<Text>();
			Text component4 = form.gameObject.transform.Find("Root/panelRight/lblTitle").gameObject.GetComponent<Text>();
			Text component5 = form.gameObject.transform.Find("Root/panelCenter/lblTitle").gameObject.GetComponent<Text>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			List<uint> arenaDefHeroList = masterRoleInfo.m_arenaDefHeroList;
			component3.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", new string[]
			{
				masterRoleInfo.PvpLevel.ToString()
			});
			if (this.m_fightHeroInfoList.stArenaInfo.dwSelfRank > 0u)
			{
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Arena_My_Rank", new string[]
				{
					this.m_fightHeroInfoList.stArenaInfo.dwSelfRank.ToString()
				});
			}
			else
			{
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Not_In_Rank");
			}
			if (this.m_fightHeroInfoList.stArenaInfo.dwSelfRank == 1u)
			{
				component5.gameObject.CustomSetActive(true);
			}
			component2.SetElementAmount(arenaDefHeroList.Count);
			for (int i = 0; i < arenaDefHeroList.Count; i++)
			{
				CUIListElementScript elemenet = component2.GetElemenet(i);
				GameObject gameObject = elemenet.gameObject.transform.Find("heroItemCell").gameObject;
				IHeroData data = CHeroDataFactory.CreateHeroData(arenaDefHeroList[i]);
				CUICommonSystem.SetHeroItemData(form, gameObject, data, enHeroHeadType.enIcon, false, true);
			}
			component2 = form.gameObject.transform.Find("Root/panelCenter/List").gameObject.GetComponent<CUIListScript>();
			component2.SetElementAmount((int)this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.bFigterNum);
			for (int j = 0; j < (int)this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.bFigterNum; j++)
			{
				CUIListElementScript elemenet2 = component2.GetElemenet(j);
				GameObject gameObject2 = elemenet2.gameObject.transform.Find("heroItemCell").gameObject;
				Text component6 = elemenet2.gameObject.transform.Find("lblName").gameObject.GetComponent<Text>();
				Text component7 = elemenet2.gameObject.transform.Find("lblRank").gameObject.GetComponent<Text>();
				Text component8 = elemenet2.gameObject.transform.Find("lblFight").gameObject.GetComponent<Text>();
				Text component9 = elemenet2.gameObject.transform.Find("lblLevel").gameObject.GetComponent<Text>();
				COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = CArenaSystem.GetFighterInfo(this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[j].stFigterData);
				int bMemberType = (int)this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[j].stFigterData.bMemberType;
				component6.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
				component7.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", new string[]
				{
					this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[j].dwRank.ToString()
				});
				component8.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", new string[]
				{
					fighterInfo.dwPVPLevel.ToString()
				});
				component9.text = fighterInfo.dwPVPLevel.ToString();
				component9.gameObject.CustomSetActive(false);
				Image component10 = gameObject2.transform.Find("NobeIcon").GetComponent<Image>();
				Image component11 = gameObject2.transform.Find("NobeImag").GetComponent<Image>();
				if (component10)
				{
					MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component10, (int)fighterInfo.stVip.dwCurLevel, false, true, 0uL);
				}
				if (component11)
				{
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component11, (int)fighterInfo.stVip.dwHeadIconId);
				}
				this.SetPlayerHead(gameObject2, StringHelper.UTF8BytesToString(ref fighterInfo.szHeadUrl), bMemberType, form);
			}
			Text component12 = form.gameObject.transform.Find("Root/panelLeft/lblTitle").gameObject.GetComponent<Text>();
			Button component13 = form.gameObject.transform.Find("Root/panelLeft/btnChangeHeroList").gameObject.GetComponent<Button>();
			Button component14 = form.gameObject.transform.Find("Root/panelLeft/btnBuyTimes").gameObject.GetComponent<Button>();
			GameObject gameObject3 = form.gameObject.transform.Find("Root/panelLeft/panelCD").gameObject;
			CUITimerScript component15 = form.gameObject.transform.Find("Root/panelLeft/panelCD/TimerCD").gameObject.GetComponent<CUITimerScript>();
			component12.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Today_Challenge_Count", new string[]
			{
				((int)this.m_serverInfo.bChallengeLimitCnt - (int)this.m_serverInfo.chAlreadyFightCnt).ToString(),
				this.m_serverInfo.bChallengeLimitCnt.ToString()
			});
			int num = this.m_nextCanFightTimes - CRoleInfo.GetElapseSecondsSinceLogin();
			if (num <= 0)
			{
				gameObject3.CustomSetActive(false);
				component15.EndTimer();
			}
			else
			{
				gameObject3.CustomSetActive(true);
				component15.SetTotalTime((float)num);
				component15.StartTimer();
			}
			if ((int)this.m_serverInfo.chAlreadyFightCnt >= (int)this.m_serverInfo.bChallengeLimitCnt)
			{
				component13.gameObject.CustomSetActive(false);
				uint key = (uint)(9 + this.m_serverInfo.bBuyChallengeCnt + 1);
				ResShopRefreshCost dataByKey = GameDataMgr.shopRefreshCostDatabin.GetDataByKey(key);
				if (dataByKey != null)
				{
					component14.gameObject.CustomSetActive(true);
				}
			}
			else
			{
				component13.gameObject.CustomSetActive(true);
				component14.gameObject.CustomSetActive(false);
			}
		}

		private void SetPlayerHead(GameObject cell, string headPath, int memberType, CUIFormScript formScript)
		{
			if (CSysDynamicBlock.bSocialBlocked)
			{
				return;
			}
			CUIHttpImageScript component = cell.transform.Find("httpIcon").GetComponent<CUIHttpImageScript>();
			Image component2 = cell.transform.Find("npcIcon").GetComponent<Image>();
			if (memberType == 2)
			{
				component.gameObject.CustomSetActive(false);
				component2.gameObject.CustomSetActive(true);
				component2.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + headPath, formScript, true, false, false, false);
			}
			else
			{
				component.gameObject.CustomSetActive(true);
				component2.gameObject.CustomSetActive(false);
				component.SetImageUrl(headPath);
			}
		}

		private void Arena_TeamConfig(CUIEvent uiEvent)
		{
			Singleton<CHeroSelectBaseSystem>.instance.SetPveDataWithArena(0u, null, Singleton<CTextManager>.GetInstance().GetText("Arena_Arena_Defensive_Team"));
			Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enArenaDefTeamConfig, 3, 0u, 0, 0);
		}

		private int GetDefTeamBattleValue()
		{
			return 0;
		}

		private void Arena_ReciveDefTeamInfo(CUIEvent uiEvent)
		{
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Clear();
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.AddRange(uiEvent.m_eventParams.tagList);
			CArenaSystem.SendSetDefTeamConfigMSG(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList);
		}

		private void Arena_ResetByNewDay(CUIEvent uiEvent)
		{
			this.m_serverInfo.bChallengeLimitCnt = (byte)GameDataMgr.globalInfoDatabin.GetDataByKey(48u).dwConfValue;
			this.m_serverInfo.bBuyChallengeCnt = 0;
			this.m_serverInfo.dwNextRefreshLeftTime = (uint)(CRoleInfo.GetElapseSecondsSinceLogin() + (int)Utility.s_daySecond);
			this.m_serverInfo.dwLeftFightCoolTime = 0u;
			this.m_serverInfo.chAlreadyFightCnt = 0;
			this.m_nextCanFightTimes = CRoleInfo.GetElapseSecondsSinceLogin();
			this.RefreshArenaForm();
		}

		public void BattleReturn(bool isWin)
		{
			if (!isWin)
			{
				Singleton<CArenaSystem>.GetInstance().m_nextCanFightTimes = (int)((long)CRoleInfo.GetElapseSecondsSinceLogin() + (long)((ulong)GameDataMgr.globalInfoDatabin.GetDataByKey(49u).dwConfValue));
			}
			CSDT_ACNT_ARENADATA expr_34 = Singleton<CArenaSystem>.GetInstance().m_serverInfo;
			expr_34.chAlreadyFightCnt += 1;
		}

		public void ShowBattleResult(SCPKG_SINGLEGAMEFINRSP settleData)
		{
			if (settleData.stDetail.stGameInfo.bGameResult == 1)
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaFightResultFormPath, false, true);
				GameObject gameObject = cUIFormScript.gameObject.transform.Find("Root/resultCell1").gameObject;
				GameObject gameObject2 = cUIFormScript.gameObject.transform.Find("Root/resultCell2").gameObject;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				uint pvpLevel = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel;
				this.SetResultCell(gameObject, masterRoleInfo.Name, pvpLevel, this.GetDefTeamBattleValue(), this.m_tarRank, 1, masterRoleInfo.HeadUrl, cUIFormScript);
				this.SetResultCell(gameObject2, StringHelper.UTF8BytesToString(ref this.m_tarInfo.szName), this.m_tarInfo.dwPVPLevel, (int)this.m_tarInfo.dwForceValue, this.m_fightHeroInfoList.stArenaInfo.dwSelfRank, (int)this.m_tarType, StringHelper.UTF8BytesToString(ref this.m_tarInfo.szHeadUrl), cUIFormScript);
			}
		}

		private void SetResultCell(GameObject root, string name, uint pvpLevel, int battleValue, uint rank, int memberType, string headPath, CUIFormScript form)
		{
			Text component = root.transform.Find("lblName").GetComponent<Text>();
			Text component2 = root.transform.Find("lblFight").GetComponent<Text>();
			Text component3 = root.transform.Find("lblRank").GetComponent<Text>();
			GameObject gameObject = root.transform.Find("heroItemCell").gameObject;
			component.text = name;
			component2.text = pvpLevel.ToString();
			component3.text = rank.ToString();
			this.SetPlayerHead(gameObject, headPath, memberType, form);
		}

		private void Arena_ConfirmResutlForm(CUIEvent uiEvent)
		{
			if (this.m_tarRank < this.m_serverInfo.dwTopRank || this.m_serverInfo.dwTopRank == 0u)
			{
				long num = (long)Math.Abs((int)(this.m_serverInfo.dwTopRank - this.m_tarRank));
				this.m_serverInfo.dwTopRank = this.m_tarRank;
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaRankChangeFormPath, false, true);
				Text component = cUIFormScript.gameObject.transform.Find("resultCell/lblRank").GetComponent<Text>();
				Text component2 = cUIFormScript.gameObject.transform.Find("resultCell/lblRankChange").GetComponent<Text>();
				Transform[] array = new Transform[]
				{
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell1"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell2"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell3"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell4"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell5"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell6"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell7"),
					cUIFormScript.gameObject.transform.Find("Award/ListAward/itemCell8")
				};
				component.text = this.m_tarRank.ToString();
				component2.text = num.ToString();
				ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(Singleton<SingleGameSettleMgr>.GetInstance().m_settleData.stDetail.stReward);
				for (int i = 0; i < array.Length; i++)
				{
					if (i < useableListFromReward.Count)
					{
						useableListFromReward[i].SetMultiple(ref Singleton<SingleGameSettleMgr>.GetInstance().m_settleData.stDetail.stMultipleDetail, true);
						CUICommonSystem.SetItemCell(cUIFormScript, array[i].gameObject, useableListFromReward[i], true, false, false, false);
					}
					else
					{
						array[i].gameObject.CustomSetActive(false);
					}
				}
			}
			this.m_fightHeroInfoList.stArenaInfo.dwSelfRank = this.m_tarRank;
		}

		private void Arena_OnCloseForm(CUIEvent uiEvent)
		{
			CExploreView.RefreshExploreList();
		}

		private void Arena_ChangeHeroList(CUIEvent uiEvent)
		{
			CArenaSystem.SendGetFightHeroListMSG(true);
		}

		private void Arena_BuyChangeTimes(CUIEvent uiEvent)
		{
			uint shopInfoCfgId = CPurchaseSys.GetShopInfoCfgId(RES_SHOPBUY_TYPE.RES_BUYTYPE_ARENACHALLENGECNT, (int)(this.m_serverInfo.bBuyChallengeCnt + 1));
			ResShopInfo dataByKey = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint)((ushort)shopInfoCfgId));
			if (dataByKey == null)
			{
				return;
			}
			uint dwCoinPrice = dataByKey.dwCoinPrice;
			int dwValue = (int)dataByKey.dwValue;
			enPayType payType = CMallSystem.ResBuyTypeToPayType((int)dataByKey.bCoinType);
			CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Arena_Arena_Count_Buy", new string[]
			{
				dwValue.ToString(),
				((int)(this.m_serverInfo.bBuyChallengeCnt + 1)).ToString()
			}), payType, dwCoinPrice, enUIEventID.Arena_ConfirmBuyChangeTimes, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
		}

		private void Arena_ConfirmBuyChangeTimes(CUIEvent uiEvent)
		{
			CArenaSystem.SendBuyTimesMSG();
		}

		public void ResetFightTimes(int haveBuyTimes, int maxTimes)
		{
			this.m_serverInfo.bBuyChallengeCnt = (byte)haveBuyTimes;
			this.m_serverInfo.bChallengeLimitCnt = (byte)maxTimes;
			this.m_serverInfo.chAlreadyFightCnt = 0;
			this.m_serverInfo.dwLeftFightCoolTime = 0u;
			this.RefreshArenaForm();
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		private void Arena_ResetCD(CUIEvent uiEvent)
		{
			int num = this.m_nextCanFightTimes - CRoleInfo.GetElapseSecondsSinceLogin();
			if (num <= 0)
			{
				return;
			}
			ResClrCD dataByKey = GameDataMgr.cdDatabin.GetDataByKey(1u);
			uint num2 = (uint)((long)num / (long)((ulong)dataByKey.dwConsumeUnit) * (long)((ulong)dataByKey.dwUnitPrice));
			if (num2 == 0u)
			{
				num2 = dataByKey.dwUnitPrice;
			}
			enPayType payType = CMallSystem.ResBuyTypeToPayType((int)dataByKey.dwConsumeType);
			CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Arena_Cd_End"), payType, num2, enUIEventID.Arena_ConfirmBuyResetCD, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
		}

		private void Arena_ConfirmResetCD(CUIEvent uiEvent)
		{
			CArenaSystem.SendResetCDMSG();
		}

		private void Arena_CDTimeEnd(CUIEvent uiEvent)
		{
			this.RefreshArenaForm();
		}

		private void Arena_OpenTips(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenInfoForm(CArenaSystem.s_Arena_RULE_ID);
		}

		private void Arena_CloseTps(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject.transform.Find("panelTips").gameObject;
			gameObject.CustomSetActive(false);
		}

		private void Arena_OpenPlayerInfoForm(CUIEvent uiEvent)
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Count == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Arena_ARENAADDMEM_ERR_BATTLELISTISNULL"), false, 1.5f, null, new object[0]);
				Singleton<CUIManager>.GetInstance().CloseForm(CArenaSystem.s_arenaPlayerInfoFormPath);
				return;
			}
			int num = this.m_nextCanFightTimes - CRoleInfo.GetElapseSecondsSinceLogin();
			if (num > 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_LIMIT_CD"), false, 1.5f, null, new object[0]);
				Singleton<CUIManager>.GetInstance().CloseForm(CArenaSystem.s_arenaPlayerInfoFormPath);
				return;
			}
			if ((long)this.m_serverInfo.chAlreadyFightCnt >= (long)((ulong)GameDataMgr.globalInfoDatabin.GetDataByKey(48u).dwConfValue))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_LIMIT_CNT"), false, 1.5f, null, new object[0]);
				Singleton<CUIManager>.GetInstance().CloseForm(CArenaSystem.s_arenaPlayerInfoFormPath);
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = CArenaSystem.GetFighterInfo(this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData);
			this.m_tarObjID = fighterInfo.ullUid;
			this.m_tarIndex = (byte)srcWidgetIndexInBelongedList;
			this.m_tarInfo = fighterInfo;
			this.m_tarType = this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData.bMemberType;
			this.m_tarRank = this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[srcWidgetIndexInBelongedList].dwRank;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaPlayerInfoFormPath, false, true);
			GameObject gameObject = cUIFormScript.gameObject.transform.Find("Root/heroItemCell").gameObject;
			Text component = cUIFormScript.gameObject.transform.Find("Root/lblName").gameObject.GetComponent<Text>();
			Text component2 = cUIFormScript.gameObject.transform.Find("Root/lblFight").gameObject.GetComponent<Text>();
			Text component3 = cUIFormScript.gameObject.transform.Find("Root/lblRank").gameObject.GetComponent<Text>();
			CUIListScript component4 = cUIFormScript.gameObject.transform.Find("Root/List").gameObject.GetComponent<CUIListScript>();
			component.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
			component2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", new string[]
			{
				fighterInfo.dwPVPLevel.ToString()
			});
			component3.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", new string[]
			{
				this.m_tarRank.ToString()
			});
			this.SetPlayerHead(gameObject, StringHelper.UTF8BytesToString(ref fighterInfo.szHeadUrl), (int)this.m_tarType, cUIFormScript);
			Image component5 = gameObject.transform.Find("NobeIcon").GetComponent<Image>();
			Image component6 = gameObject.transform.Find("NobeImag").GetComponent<Image>();
			if (component5)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component5, (int)fighterInfo.stVip.dwCurLevel, false, true, 0uL);
			}
			if (component6)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component6, (int)fighterInfo.stVip.dwHeadIconId);
			}
			component4.SetElementAmount(fighterInfo.stBattleHero.astHero.Length);
			for (int i = 0; i < component4.GetElementAmount(); i++)
			{
				CUIListElementScript elemenet = component4.GetElemenet(i);
				GameObject gameObject2 = elemenet.gameObject.transform.Find("heroItemCell").gameObject;
				COMDT_ARENA_HERODETAIL cOMDT_ARENA_HERODETAIL = fighterInfo.stBattleHero.astHero[i];
				if (cOMDT_ARENA_HERODETAIL.dwHeroId != 0u)
				{
					CCustomHeroData cCustomHeroData = CHeroDataFactory.CreateCustomHeroData(cOMDT_ARENA_HERODETAIL.dwHeroId) as CCustomHeroData;
					cCustomHeroData.m_star = (int)cOMDT_ARENA_HERODETAIL.wHeroStar;
					cCustomHeroData.m_level = (int)cOMDT_ARENA_HERODETAIL.wHeroLevel;
					cCustomHeroData.m_quaility = (int)cOMDT_ARENA_HERODETAIL.stHeroQuality.wQuality;
					cCustomHeroData.m_subQualility = (int)cOMDT_ARENA_HERODETAIL.stHeroQuality.wSubQuality;
					CUICommonSystem.SetHeroItemData(cUIFormScript, gameObject2, cCustomHeroData, enHeroHeadType.enIcon, false, true);
				}
				else
				{
					gameObject2.CustomSetActive(false);
				}
			}
		}

		private void Arena_StartFight(CUIEvent uiEvent)
		{
			CSDT_SINGLE_GAME_OF_ARENA cSDT_SINGLE_GAME_OF_ARENA = new CSDT_SINGLE_GAME_OF_ARENA();
			cSDT_SINGLE_GAME_OF_ARENA.iLevelID = CArenaSystem.s_mapKey;
			cSDT_SINGLE_GAME_OF_ARENA.bTargetIndex = this.m_tarIndex;
			ResLevelCfgInfo dataByKey = GameDataMgr.arenaLevelDatabin.GetDataByKey((long)CArenaSystem.s_mapKey);
			DebugHelper.Assert(dataByKey != null);
			Singleton<CHeroSelectBaseSystem>.instance.SetPveDataWithArena(dataByKey.dwBattleListID, cSDT_SINGLE_GAME_OF_ARENA, Singleton<CTextManager>.GetInstance().GetText("Arena_Arena"));
			Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enArena, (byte)dataByKey.iHeroNum, 0u, 0, 0);
		}

		private void Arena_OpenRankInfoForm(CUIEvent uiEvent)
		{
			CArenaSystem.SendGetRankListMSG(true);
		}

		private void RefreshRankForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaRankInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject.transform.Find("Root/panelRank").gameObject;
			GameObject gameObject2 = form.gameObject.transform.Find("Root/panelAward").gameObject;
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			GameObject gameObject3 = form.gameObject.transform.Find("Root/ListMenu").gameObject;
			CUIListScript component = gameObject3.GetComponent<CUIListScript>();
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Arena_Ranking"),
				Singleton<CTextManager>.GetInstance().GetText("Arena_Ranking_Award")
			};
			component.SetElementAmount(array.Length);
			for (int i = 0; i < component.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
			component.SelectElement(0, true);
		}

		private void Arena_RankInfoMenuClick(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			GameObject gameObject = srcFormScript.gameObject.transform.Find("Root/panelRank").gameObject;
			GameObject gameObject2 = srcFormScript.gameObject.transform.Find("Root/panelAward").gameObject;
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			if (selectedIndex == 0)
			{
				gameObject.CustomSetActive(true);
				CUIListScript component = gameObject.transform.Find("List").gameObject.GetComponent<CUIListScript>();
				component.SetElementAmount(Math.Min((int)this.m_rankInfoList.bFigterNum, 50));
			}
			else if (selectedIndex == 1)
			{
				gameObject2.CustomSetActive(true);
				CUIListScript component2 = gameObject2.transform.Find("List").gameObject.GetComponent<CUIListScript>();
				component2.SetElementAmount(10);
				for (int i = 0; i < component2.GetElementAmount(); i++)
				{
					CUIListElementScript elemenet = component2.GetElemenet(i);
					Image component3 = elemenet.gameObject.transform.Find("img1").GetComponent<Image>();
					Image component4 = elemenet.gameObject.transform.Find("img2").GetComponent<Image>();
					Image component5 = elemenet.gameObject.transform.Find("img3").GetComponent<Image>();
					Text component6 = elemenet.gameObject.transform.Find("lbTop").GetComponent<Text>();
					GameObject[] array = new GameObject[]
					{
						elemenet.gameObject.transform.Find("ListAward/itemCell1").gameObject,
						elemenet.gameObject.transform.Find("ListAward/itemCell2").gameObject,
						elemenet.gameObject.transform.Find("ListAward/itemCell3").gameObject,
						elemenet.gameObject.transform.Find("ListAward/itemCell4").gameObject
					};
					component3.gameObject.CustomSetActive(false);
					component4.gameObject.CustomSetActive(false);
					component5.gameObject.CustomSetActive(false);
					component6.gameObject.CustomSetActive(false);
					if (i == 0)
					{
						component3.gameObject.CustomSetActive(true);
					}
					else if (i == 1)
					{
						component4.gameObject.CustomSetActive(true);
					}
					else if (i == 2)
					{
						component5.gameObject.CustomSetActive(true);
					}
					else
					{
						component6.gameObject.CustomSetActive(true);
					}
					component6.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", new string[]
					{
						(i + 1).ToString()
					});
					ListView<CUseable> rewardUseableList = this.GetRewardUseableList(i + 1);
					for (int j = 0; j < array.Length; j++)
					{
						if (j > rewardUseableList.Count - 1)
						{
							array[j].CustomSetActive(false);
						}
						else
						{
							CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, array[j], rewardUseableList[j], true, false, false, false);
							if (rewardUseableList[j].m_stackCount == 0)
							{
								array[j].CustomSetActive(false);
							}
							else
							{
								array[j].CustomSetActive(true);
							}
						}
					}
				}
				Text component7 = gameObject2.transform.Find("panelSelfInfo/lblRank").GetComponent<Text>();
				GameObject[] array2 = new GameObject[]
				{
					gameObject2.transform.Find("panelSelfInfo/ListAward/itemCell1").gameObject,
					gameObject2.transform.Find("panelSelfInfo/ListAward/itemCell2").gameObject,
					gameObject2.transform.Find("panelSelfInfo/ListAward/itemCell3").gameObject,
					gameObject2.transform.Find("panelSelfInfo/ListAward/itemCell4").gameObject
				};
				component7.text = Singleton<CTextManager>.GetInstance().GetText("Arena_My_Rank", new string[]
				{
					this.m_fightHeroInfoList.stArenaInfo.dwSelfRank.ToString()
				});
				ListView<CUseable> rewardUseableList2 = this.GetRewardUseableList((int)this.m_fightHeroInfoList.stArenaInfo.dwSelfRank);
				for (int k = 0; k < array2.Length; k++)
				{
					if (k > rewardUseableList2.Count - 1)
					{
						array2[k].CustomSetActive(false);
					}
					else
					{
						CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, array2[k], rewardUseableList2[k], true, false, false, false);
						if (rewardUseableList2[k].m_stackCount == 0)
						{
							array2[k].CustomSetActive(false);
						}
						else
						{
							array2[k].CustomSetActive(true);
						}
					}
				}
			}
		}

		public ListView<CUseable> GetRewardUseableList(int rank)
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			ResArenaOneDayReward resArenaOneDayReward = GameDataMgr.arenaRewardDatabin.FindIf((ResArenaOneDayReward x) => (long)rank >= (long)((ulong)x.dwRankStart) && (long)rank <= (long)((ulong)x.dwRankEnd));
			if (resArenaOneDayReward != null)
			{
				listView.Add(CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int)resArenaOneDayReward.dwRewardDiamond));
				listView.Add(CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int)resArenaOneDayReward.dwRewardCoin));
				listView.Add(CUseableManager.CreateVirtualUseable(enVirtualItemType.enArenaCoin, (int)resArenaOneDayReward.dwRewardArenaCoin));
				listView.Add(CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, resArenaOneDayReward.dwRewardPropId, (int)resArenaOneDayReward.dwRewardPropCnt));
			}
			return listView;
		}

		public static void Arena_RankElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			Text component = srcWidget.transform.Find("lbTop").GetComponent<Text>();
			Text component2 = srcWidget.transform.Find("lblTitle").GetComponent<Text>();
			Text component3 = srcWidget.transform.Find("lblFight").GetComponent<Text>();
			GameObject[] array = new GameObject[]
			{
				srcWidget.transform.Find("listHero/heroItemCell1").gameObject,
				srcWidget.transform.Find("listHero/heroItemCell2").gameObject,
				srcWidget.transform.Find("listHero/heroItemCell3").gameObject
			};
			Image component4 = srcWidget.transform.Find("img1").GetComponent<Image>();
			Image component5 = srcWidget.transform.Find("img2").GetComponent<Image>();
			Image component6 = srcWidget.transform.Find("img3").GetComponent<Image>();
			COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = CArenaSystem.GetFighterInfo(Singleton<CArenaSystem>.instance.m_rankInfoList.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData);
			component4.gameObject.CustomSetActive(false);
			component5.gameObject.CustomSetActive(false);
			component6.gameObject.CustomSetActive(false);
			component.gameObject.CustomSetActive(false);
			if (srcWidgetIndexInBelongedList == 0)
			{
				component4.gameObject.CustomSetActive(true);
			}
			else if (srcWidgetIndexInBelongedList == 1)
			{
				component5.gameObject.CustomSetActive(true);
			}
			else if (srcWidgetIndexInBelongedList == 2)
			{
				component6.gameObject.CustomSetActive(true);
			}
			else
			{
				component.gameObject.CustomSetActive(true);
			}
			component.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", new string[]
			{
				(srcWidgetIndexInBelongedList + 1).ToString()
			});
			component2.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
			component3.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", new string[]
			{
				fighterInfo.dwPVPLevel.ToString()
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (i < fighterInfo.stBattleHero.astHero.Length)
				{
					COMDT_ARENA_HERODETAIL cOMDT_ARENA_HERODETAIL = fighterInfo.stBattleHero.astHero[i];
					if (cOMDT_ARENA_HERODETAIL.dwHeroId != 0u)
					{
						array[i].CustomSetActive(true);
						CCustomHeroData cCustomHeroData = CHeroDataFactory.CreateCustomHeroData(cOMDT_ARENA_HERODETAIL.dwHeroId) as CCustomHeroData;
						cCustomHeroData.m_star = (int)cOMDT_ARENA_HERODETAIL.wHeroStar;
						cCustomHeroData.m_level = (int)cOMDT_ARENA_HERODETAIL.wHeroLevel;
						cCustomHeroData.m_quaility = (int)cOMDT_ARENA_HERODETAIL.stHeroQuality.wQuality;
						cCustomHeroData.m_subQualility = (int)cOMDT_ARENA_HERODETAIL.stHeroQuality.wSubQuality;
						CUICommonSystem.SetHeroItemData(uiEvent.m_srcFormScript, array[i], cCustomHeroData, enHeroHeadType.enIcon, false, true);
					}
					else
					{
						array[i].CustomSetActive(false);
					}
				}
				else
				{
					array[i].CustomSetActive(false);
				}
			}
		}

		private void Arena_OpenRecordForm(CUIEvent uiEvent)
		{
			if ((long)CRoleInfo.GetElapseSecondsSinceLogin() - (long)((ulong)this.m_lastRecordRequestTime) > 60L || this.m_lastRecordRequestTime == 0u)
			{
				CArenaSystem.SendGetRecordMSG(true);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaFightRecordFormPath, false, true);
				Singleton<CArenaSystem>.GetInstance().RefreshRecordForm();
			}
		}

		public void RefreshRecordForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaFightRecordFormPath);
			if (form == null)
			{
				return;
			}
			CUIListScript component = form.gameObject.transform.Find("Root/List").gameObject.GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_recordList.Count);
		}

		private void Arena_RecordlementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			Text component = srcWidget.transform.Find("lblTitle").GetComponent<Text>();
			Text component2 = srcWidget.transform.Find("lblFight").GetComponent<Text>();
			Text component3 = srcWidget.transform.Find("lblRankChange").GetComponent<Text>();
			Image component4 = srcWidget.transform.Find("imgUp").GetComponent<Image>();
			Image component5 = srcWidget.transform.Find("imgDown").GetComponent<Image>();
			Image component6 = srcWidget.transform.Find("imgWin").GetComponent<Image>();
			Image component7 = srcWidget.transform.Find("imgLose").GetComponent<Image>();
			Image component8 = srcWidget.transform.Find("RedFlag").GetComponent<Image>();
			Image component9 = srcWidget.transform.Find("BlueFlag").GetComponent<Image>();
			component3.text = "---";
			component4.gameObject.CustomSetActive(false);
			component5.gameObject.CustomSetActive(false);
			component6.gameObject.CustomSetActive(false);
			component7.gameObject.CustomSetActive(false);
			component8.gameObject.CustomSetActive(false);
			component9.gameObject.CustomSetActive(false);
			GameObject[] array = new GameObject[]
			{
				srcWidget.transform.Find("listHero/heroItemCell1").gameObject,
				srcWidget.transform.Find("listHero/heroItemCell2").gameObject,
				srcWidget.transform.Find("listHero/heroItemCell3").gameObject
			};
			COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = CArenaSystem.GetFighterInfo(this.m_recordList[srcWidgetIndexInBelongedList].stTargetInfo);
			component.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
			component2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", new string[]
			{
				fighterInfo.dwPVPLevel.ToString()
			});
			component6.gameObject.CustomSetActive(false);
			component7.gameObject.CustomSetActive(false);
			if (this.m_recordList[srcWidgetIndexInBelongedList].ullAtkerUid == playerUllUID)
			{
				component8.gameObject.CustomSetActive(true);
				if (this.m_recordList[srcWidgetIndexInBelongedList].bResult == 1)
				{
					component3.text = Math.Abs((int)(this.m_recordList[srcWidgetIndexInBelongedList].dwAtkerRank - this.m_recordList[srcWidgetIndexInBelongedList].dwTargetRank)).ToString();
					component6.gameObject.CustomSetActive(true);
					component4.gameObject.CustomSetActive(true);
				}
				else
				{
					component7.gameObject.CustomSetActive(true);
				}
			}
			else
			{
				component9.gameObject.CustomSetActive(true);
				if (this.m_recordList[srcWidgetIndexInBelongedList].bResult == 1)
				{
					component3.text = Math.Abs((int)(this.m_recordList[srcWidgetIndexInBelongedList].dwAtkerRank - this.m_recordList[srcWidgetIndexInBelongedList].dwTargetRank)).ToString();
					component7.gameObject.CustomSetActive(true);
					component5.gameObject.CustomSetActive(true);
				}
				else
				{
					component6.gameObject.CustomSetActive(true);
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (i < fighterInfo.stBattleHero.astHero.Length)
				{
					COMDT_ARENA_HERODETAIL cOMDT_ARENA_HERODETAIL = fighterInfo.stBattleHero.astHero[i];
					if (cOMDT_ARENA_HERODETAIL.dwHeroId != 0u)
					{
						CCustomHeroData cCustomHeroData = CHeroDataFactory.CreateCustomHeroData(cOMDT_ARENA_HERODETAIL.dwHeroId) as CCustomHeroData;
						cCustomHeroData.m_star = (int)cOMDT_ARENA_HERODETAIL.wHeroStar;
						cCustomHeroData.m_level = (int)cOMDT_ARENA_HERODETAIL.wHeroLevel;
						cCustomHeroData.m_quaility = (int)cOMDT_ARENA_HERODETAIL.stHeroQuality.wQuality;
						cCustomHeroData.m_subQualility = (int)cOMDT_ARENA_HERODETAIL.stHeroQuality.wSubQuality;
						CUICommonSystem.SetHeroItemData(uiEvent.m_srcFormScript, array[i], cCustomHeroData, enHeroHeadType.enIcon, false, true);
					}
					else
					{
						array[i].CustomSetActive(false);
					}
				}
				else
				{
					array[i].CustomSetActive(false);
				}
			}
		}

		public static int SortCompareRecord(COMDT_ARENA_FIGHT_RECORD info1, COMDT_ARENA_FIGHT_RECORD info2)
		{
			if (info2.dwFightTime > info1.dwFightTime)
			{
				return 1;
			}
			if (info2.dwFightTime == info1.dwFightTime)
			{
				return 0;
			}
			return -1;
		}

		private void Arena_OpenShopForm(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_OpenArenaShop);
		}

		public static void SendJoinArenaMSG()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2903u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendSetDefTeamConfigMSG(List<uint> heroList)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2900u);
			cSPkg.stPkgData.stSetBattleListOfArenaReq.stBattleList.wHeroCnt = (ushort)heroList.Count;
			cSPkg.stPkgData.stSetBattleListOfArenaReq.stBattleList.BattleHeroList = heroList.ToArray();
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendGetFightHeroListMSG(bool bForceSend = true)
		{
			if (bForceSend || (long)CRoleInfo.GetElapseSecondsSinceLogin() - (long)((ulong)Singleton<CArenaSystem>.GetInstance().m_lastFighterInfoRequestTime) > 60L || Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList == null)
			{
				Singleton<CArenaSystem>.GetInstance().m_lastFighterInfoRequestTime = (uint)CRoleInfo.GetElapseSecondsSinceLogin();
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2907u);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public static void SendGetRankListMSG(bool bForceSend = true)
		{
			if (bForceSend || (long)CRoleInfo.GetElapseSecondsSinceLogin() - (long)((ulong)Singleton<CArenaSystem>.GetInstance().m_lastRankRequestTime) > 60L || Singleton<CArenaSystem>.GetInstance().m_rankInfoList == null)
			{
				Singleton<CArenaSystem>.GetInstance().m_lastRankRequestTime = (uint)CRoleInfo.GetElapseSecondsSinceLogin();
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2909u);
				cSPkg.stPkgData.stGetTopFighterOfArenaReq.bTopNum = 100;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public static void SendGetRecordMSG(bool bForceSend = true)
		{
			if (bForceSend || (long)CRoleInfo.GetElapseSecondsSinceLogin() - (long)((ulong)Singleton<CArenaSystem>.GetInstance().m_lastRecordRequestTime) > 60L || Singleton<CArenaSystem>.GetInstance().m_recordList == null)
			{
				Singleton<CArenaSystem>.GetInstance().m_lastRecordRequestTime = (uint)CRoleInfo.GetElapseSecondsSinceLogin();
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2911u);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public static void SendBuyTimesMSG()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1113u);
			cSPkg.stPkgData.stShopBuyReq = new CSPKG_CMD_SHOPBUY();
			cSPkg.stPkgData.stShopBuyReq.iBuyType = 9;
			cSPkg.stPkgData.stShopBuyReq.iBuySubType = (int)(Singleton<CArenaSystem>.GetInstance().m_serverInfo.bBuyChallengeCnt + 1);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendResetCDMSG()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1262u);
			cSPkg.stPkgData.stClrCdLimitReq.dwCdType = 1u;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static COMDT_ARENA_MEMBER_OF_ACNT GetFighterInfo(COMDT_ARENA_BLOCKDATA rankInfo)
		{
			COMDT_ARENA_MEMBER_OF_ACNT cOMDT_ARENA_MEMBER_OF_ACNT = new COMDT_ARENA_MEMBER_OF_ACNT();
			if (rankInfo.bMemberType == 1)
			{
				cOMDT_ARENA_MEMBER_OF_ACNT = rankInfo.stMember.stAcnt;
			}
			else
			{
				cOMDT_ARENA_MEMBER_OF_ACNT.ullUid = (ulong)rankInfo.stMember.stNpc.dwObjId;
				ResNpcOfArena dataByKey = GameDataMgr.npcOfArena.GetDataByKey((uint)cOMDT_ARENA_MEMBER_OF_ACNT.ullUid);
				ResRobotBattleList dataByKey2 = GameDataMgr.robotBattleListInfo.GetDataByKey(dataByKey.dwBattleList);
				ListView<COMDT_ARENA_HERODETAIL> listView = new ListView<COMDT_ARENA_HERODETAIL>();
				uint[] array = new uint[]
				{
					(uint)(dataByKey.dwForceValue * 0.33),
					(uint)(dataByKey.dwForceValue * 0.33),
					(uint)(dataByKey.dwForceValue * 0.34)
				};
				uint num = 0u;
				for (int i = 0; i < array.Length; i++)
				{
					ResRobotPower robotHeroInfo = CArenaSystem.GetRobotHeroInfo(array[i]);
					COMDT_ARENA_HERODETAIL cOMDT_ARENA_HERODETAIL = new COMDT_ARENA_HERODETAIL();
					cOMDT_ARENA_HERODETAIL.dwHeroId = dataByKey2.HeroList[i];
					cOMDT_ARENA_HERODETAIL.stHeroQuality.wQuality = (ushort)robotHeroInfo.iQuality;
					cOMDT_ARENA_HERODETAIL.stHeroQuality.wSubQuality = (ushort)robotHeroInfo.iSubQuality;
					cOMDT_ARENA_HERODETAIL.wHeroStar = (ushort)robotHeroInfo.iStar;
					cOMDT_ARENA_HERODETAIL.wHeroLevel = robotHeroInfo.wLevel;
					if ((uint)cOMDT_ARENA_HERODETAIL.wHeroLevel > dataByKey.dwNpcLevel)
					{
						cOMDT_ARENA_HERODETAIL.wHeroLevel = (ushort)dataByKey.dwNpcLevel;
					}
					num += robotHeroInfo.dwPower;
					listView.Add(cOMDT_ARENA_HERODETAIL);
				}
				StringHelper.StringToUTF8Bytes(dataByKey.szNpcName, ref cOMDT_ARENA_MEMBER_OF_ACNT.szName);
				cOMDT_ARENA_MEMBER_OF_ACNT.dwPVPLevel = dataByKey.dwNpcLevel;
				cOMDT_ARENA_MEMBER_OF_ACNT.dwForceValue = num;
				StringHelper.StringToUTF8Bytes(dataByKey.dwNpcHeadId.ToString(), ref cOMDT_ARENA_MEMBER_OF_ACNT.szHeadUrl);
				cOMDT_ARENA_MEMBER_OF_ACNT.stBattleHero = new COMDT_ARENA_HEROINFO();
				cOMDT_ARENA_MEMBER_OF_ACNT.stBattleHero.astHero = LinqS.ToArray<COMDT_ARENA_HERODETAIL>(listView);
			}
			return cOMDT_ARENA_MEMBER_OF_ACNT;
		}

		public static ResRobotPower GetRobotHeroInfo(uint forceValue)
		{
			ResRobotPower res = null;
			GameDataMgr.robotHeroInfo.Accept(delegate(ResRobotPower x)
			{
				if (x.dwPower > forceValue && (res == null || res.dwPower > x.dwPower))
				{
					res = x;
				}
			});
			if (res == null)
			{
				GameDataMgr.robotHeroInfo.Accept(delegate(ResRobotPower x)
				{
					if (x.dwPower < forceValue && (res == null || res.dwPower < x.dwPower))
					{
						res = x;
					}
				});
			}
			return res;
		}

		[MessageHandler(2904)]
		public static void ReciveJoinArenaInfo(CSPkg msg)
		{
			SCPKG_JOINARENA_RSP stJoinArenaRsp = msg.stPkgData.stJoinArenaRsp;
			if (stJoinArenaRsp.stResult.bErrCode == 0)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_OpenForm);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(2904, (int)stJoinArenaRsp.stResult.bErrCode), false, 1.5f, null, new object[0]);
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		[MessageHandler(2901)]
		public static void ReciveDefTeamListInfo(CSPkg msg)
		{
			SCPKG_SETBATTLELISTOFARENA_RSP stSetBattleListOfArenaRsp = msg.stPkgData.stSetBattleListOfArenaRsp;
			if (stSetBattleListOfArenaRsp.bErrCode == 0)
			{
				Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Clear();
				Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.AddRange(stSetBattleListOfArenaRsp.stResult.stSucc.stBattleList.BattleHeroList);
				Singleton<CArenaSystem>.GetInstance().RefreshArenaForm();
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(2901, (int)stSetBattleListOfArenaRsp.bErrCode), false, 1.5f, null, new object[0]);
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		[MessageHandler(2908)]
		public static void ReciveFightHeroListInfo(CSPkg msg)
		{
			SCPKG_CHGARENAFIGHTERRSP stChgArenaFighterRsp = msg.stPkgData.stChgArenaFighterRsp;
			Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList = stChgArenaFighterRsp;
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (Singleton<CArenaSystem>.instance.m_openArenaForm)
			{
				Singleton<CArenaSystem>.instance.m_openArenaForm = false;
				if (Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaFormPath) == null)
				{
					Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaFormPath, false, true);
				}
			}
			if (Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaFormPath) != null)
			{
				Singleton<CArenaSystem>.GetInstance().RefreshArenaForm();
			}
			Singleton<EventRouter>.instance.BroadCastEvent("Arena_Fighter_Changed");
		}

		[MessageHandler(2910)]
		public static void ReciveRankListInfo(CSPkg msg)
		{
			SCPKG_GETTOPFIGHTEROFARENA_RSP stGetTopFighterOfArenaRsp = msg.stPkgData.stGetTopFighterOfArenaRsp;
			Singleton<CArenaSystem>.GetInstance().m_rankInfoList = stGetTopFighterOfArenaRsp.stTopFighter;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			for (int i = 0; i < (int)stGetTopFighterOfArenaRsp.stTopFighter.bFigterNum; i++)
			{
				if (stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.bMemberType == 1 && masterRoleInfo.playerUllUID == stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.stMember.stAcnt.ullUid)
				{
					stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.stMember.stAcnt.stBattleHero.astHero = new COMDT_ARENA_HERODETAIL[3];
					for (int j = 0; j < masterRoleInfo.m_arenaDefHeroList.Count; j++)
					{
						COMDT_ARENA_HERODETAIL cOMDT_ARENA_HERODETAIL = new COMDT_ARENA_HERODETAIL();
						cOMDT_ARENA_HERODETAIL.dwHeroId = masterRoleInfo.m_arenaDefHeroList[j];
						stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.stMember.stAcnt.stBattleHero.astHero[j] = cOMDT_ARENA_HERODETAIL;
					}
					break;
				}
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaFormPath) != null)
			{
				Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaRankInfoFormPath, false, true);
				Singleton<CArenaSystem>.GetInstance().RefreshRankForm();
			}
			Singleton<EventRouter>.instance.BroadCastEvent("Rank_Arena_List");
		}

		[MessageHandler(1263)]
		public static void ReciveResetCD(CSPkg msg)
		{
			SCPKG_CLRCDLIMIT_RSP stClrCdLimitRsp = msg.stPkgData.stClrCdLimitRsp;
			if (stClrCdLimitRsp.bResult == 0)
			{
				Singleton<CArenaSystem>.GetInstance().m_nextCanFightTimes = CRoleInfo.GetElapseSecondsSinceLogin();
				Singleton<CArenaSystem>.GetInstance().RefreshArenaForm();
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Reset CD Fail", false, 1.5f, null, new object[0]);
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		[MessageHandler(2912)]
		public static void ReciveRecordListInfo(CSPkg msg)
		{
			SCPKG_GETARENAFIGHTHISTORY_RSP stGetArenaFightHistoryRsp = msg.stPkgData.stGetArenaFightHistoryRsp;
			Singleton<CArenaSystem>.GetInstance().m_recordList.Clear();
			for (int i = 0; i < (int)stGetArenaFightHistoryRsp.bNum; i++)
			{
				Singleton<CArenaSystem>.GetInstance().m_recordList.Add(stGetArenaFightHistoryRsp.astRecord[i]);
			}
			Singleton<CArenaSystem>.GetInstance().m_recordList.Sort(new Comparison<COMDT_ARENA_FIGHT_RECORD>(CArenaSystem.SortCompareRecord));
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (Singleton<CUIManager>.GetInstance().GetForm(CArenaSystem.s_arenaFormPath) != null)
			{
				Singleton<CUIManager>.GetInstance().OpenForm(CArenaSystem.s_arenaFightRecordFormPath, false, true);
				Singleton<CArenaSystem>.GetInstance().RefreshRecordForm();
			}
			Singleton<EventRouter>.instance.BroadCastEvent("Arena_Record_List");
		}

		public void Clear()
		{
			this.m_lastRankRequestTime = 0u;
			this.m_lastRecordRequestTime = 0u;
			this.m_lastFighterInfoRequestTime = 0u;
		}
	}
}
