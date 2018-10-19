using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
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
	internal class CUILoadingSystem : Singleton<CUILoadingSystem>
	{
		public enum SinglePlayerLoadingFormWidget
		{
			None = -1,
			Reserve,
			Tips,
			LoadingProgress,
			LoadingProgressBar,
			LoadingNotice
		}

		public enum enLoadingFormWidget
		{
			TipsLeft,
			TipsRight,
			GuildMatchPanel,
			LeftGuildHead,
			LeftGuildName,
			RightGuildHead,
			RightGuildName,
			TipsLeftText
		}

		private const uint MAX_PLAYER_NUM = 5u;

		public static string PVE_PATH_LOADING = "UGUI/Form/System/PvE/Adv/Form_Adv_Loading.prefab";

		public static string PVP_PATH_LOADING = "UGUI/Form/System/PvP/Loading/Form_Loading.prefab";

		private static CUIFormScript _singlePlayerLoading;

		private static int _pvpLoadingIndex = 1;

		private static int SoloRandomNum = 1;

		public override void Init()
		{
			base.Init();
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onGamePrepareFight));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onGameStartFight));
			Singleton<GameEventSys>.instance.AddEventHandler(GameEventDef.Event_MultiRecoverFin, new Action(this.onGameRecoverFin));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.ADVANCE_STOP_LOADING, new Action(this.HideLoading));
			Singleton<GameEventSys>.instance.AddEventHandler<PreDialogStartedEventParam>(GameEventDef.Event_PreDialogStarted, new RefAction<PreDialogStartedEventParam>(this.OnPreDialogStarted));
		}

		private void OnPreDialogStarted(ref PreDialogStartedEventParam eventParam)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_preDialogId > 0 && curLvelContext.m_preDialogId == eventParam.PreDialogId)
			{
				this.HideLoading();
			}
		}

		private void onGamePrepareFight(ref DefaultGameEventParam prm)
		{
			if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
			{
				return;
			}
		}

		private void onGameStartFight(ref DefaultGameEventParam prm)
		{
			if (!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
			{
				this.HideLoading();
			}
		}

		private void onGameRecoverFin()
		{
			this.HideLoading();
		}

		public void ShowLoading()
		{
			Singleton<BurnExpeditionController>.instance.Clear();
			MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
			Singleton<CUIManager>.GetInstance().CloseAllForm(null, true, true);
			Singleton<CUIManager>.GetInstance().ClearEventGraphicsData();
			if (Singleton<LobbyLogic>.instance.inMultiGame)
			{
				CUILoadingSystem.ShowMultiLoading();
			}
			else
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null)
				{
					if (curLvelContext.IsMobaModeWithOutGuide())
					{
						CUILoadingSystem.ShowMultiLoading();
					}
					else
					{
						this.ShowPveLoading();
					}
				}
			}
		}

		private void ShowPveLoading()
		{
			CUILoadingSystem._singlePlayerLoading = Singleton<CUIManager>.GetInstance().OpenForm(CUILoadingSystem.PVE_PATH_LOADING, false, true);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			int num = 0;
			if (curLvelContext != null)
			{
				if (curLvelContext.IsGameTypeGuide())
				{
					if (CBattleGuideManager.Is1v1GuideLevel(curLvelContext.m_mapID))
					{
						num = 1;
					}
					else if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
					{
						num = 2;
					}
					else if (curLvelContext.m_mapID == 8)
					{
						num = 3;
					}
					else
					{
						num = CUILoadingSystem.GenerateSoloRandomNum();
					}
				}
				else
				{
					num = CUILoadingSystem.GenerateSoloRandomNum();
				}
			}
			CUILoadingSystem._singlePlayerLoading.m_formWidgets[1].GetComponent<Text>().text = CUILoadingSystem.GenerateRandomPveLoadingTips(num);
			CUILoadingSystem._singlePlayerLoading.m_formWidgets[2].GetComponent<Text>().text = string.Format("{0}%", 0);
			CUILoadingSystem._singlePlayerLoading.m_formWidgets[3].GetComponent<Image>().CustomFillAmount(0f);
			Image component = CUILoadingSystem._singlePlayerLoading.m_formWidgets[4].GetComponent<Image>();
			if (num >= 4)
			{
				MonoSingleton<BannerImageSys>.GetInstance().TrySetLoadingImage((uint)num, component);
			}
			else
			{
				component.SetSprite(string.Format("{0}{1}{2}", "UGUI/Sprite/Dynamic/", "Loading/LoadingNotice", num), CUILoadingSystem._singlePlayerLoading, true, false, false, false);
			}
		}

		private void HidePveLoading()
		{
			CUILoadingSystem._singlePlayerLoading = null;
			Singleton<CUIManager>.GetInstance().CloseForm(CUILoadingSystem.PVE_PATH_LOADING);
		}

		private static void HideMultiLoading()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CUILoadingSystem.PVP_PATH_LOADING);
		}

		public void HideLoading()
		{
			Singleton<CUIManager>.GetInstance().ClearEventGraphicsData();
			if (Singleton<LobbyLogic>.instance.inMultiGame)
			{
				CUILoadingSystem.HideMultiLoading();
			}
			else
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null)
				{
					if (curLvelContext.IsMobaModeWithOutGuide())
					{
						CUILoadingSystem.HideMultiLoading();
					}
					else
					{
						this.HidePveLoading();
					}
				}
			}
		}

		[MessageHandler(1084)]
		public static void OnMultiGameLoadProcess(CSPkg msg)
		{
			GameObject memberItem = CUILoadingSystem.GetMemberItem(msg.stPkgData.stMultGameLoadProcessRsp.dwObjId);
			if (memberItem != null)
			{
				GameObject gameObject = memberItem.transform.Find("Txt_LoadingPct").gameObject;
				if (gameObject)
				{
					Text component = gameObject.GetComponent<Text>();
					component.text = string.Format("{0}%", msg.stPkgData.stMultGameLoadProcessRsp.wProcess);
				}
			}
		}

		public static void OnSelfLoadProcess(float progress)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			if (curLvelContext.IsMobaModeWithOutGuide())
			{
				uint num = 0u;
				if (Singleton<WatchController>.GetInstance().IsWatching)
				{
					Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<WatchController>.GetInstance().TargetUID);
					num = ((playerByUid == null) ? 0u : playerByUid.PlayerId);
				}
				if (num == 0u)
				{
					Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
					num = ((hostPlayer == null) ? 0u : hostPlayer.PlayerId);
				}
				GameObject memberItem = CUILoadingSystem.GetMemberItem(num);
				if (memberItem != null)
				{
					Transform transform = memberItem.transform.Find("Txt_LoadingPct");
					if (transform)
					{
						Text component = transform.GetComponent<Text>();
						component.text = string.Format("{0}%", Convert.ToUInt16(progress * 100f));
					}
				}
				if (curLvelContext.m_isWarmBattle)
				{
					CFakePvPHelper.FakeLoadProcess(progress);
				}
			}
			else
			{
				if (CUILoadingSystem._singlePlayerLoading == null)
				{
					return;
				}
				CUILoadingSystem._singlePlayerLoading.m_formWidgets[2].GetComponent<Text>().text = string.Format("{0}%", (int)(Mathf.Clamp(progress, 0f, 1f) * 100f));
				CUILoadingSystem._singlePlayerLoading.m_formWidgets[3].GetComponent<Image>().CustomFillAmount(Mathf.Clamp(progress, 0f, 1f));
			}
		}

		private static void On_IntimacyRela_LoadingClick(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcWidget.transform.parent.parent.FindChild("Txt_Qinmidu").gameObject;
			gameObject.CustomSetActive(!gameObject.activeSelf);
		}

		private static void ShowMultiLoading()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUILoadingSystem.PVP_PATH_LOADING, false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			if (!Singleton<CUIEventManager>.GetInstance().HasUIEventListener(enUIEventID.IntimacyRela_LoadingClick))
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_LoadingClick, new CUIEventManager.OnUIEventHandler(CUILoadingSystem.On_IntimacyRela_LoadingClick));
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			IGameActorDataProvider actorDataProvider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorStaticData actorStaticData = default(ActorStaticData);
			ActorMeta actorMeta = default(ActorMeta);
			ActorMeta actorMeta2 = default(ActorMeta);
			ActorServerData actorServerData = default(ActorServerData);
			actorMeta.ActorType = ActorTypeDef.Actor_Type_Hero;
			for (int i = 1; i <= 2; i++)
			{
				List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers((COM_PLAYERCAMP)i);
				if (allCampPlayers == null)
				{
					DebugHelper.Assert(false, "Loading Players is Null");
				}
				else
				{
					Transform transform = (i != 1) ? cUIFormScript.transform.FindChild("DownPanel") : cUIFormScript.transform.FindChild("UpPanel");
					int num = 1;
					while ((long)num <= 5L)
					{
						string name = (i != 1) ? string.Format("Down_Player{0}", num) : string.Format("Up_Player{0}", num);
						GameObject gameObject = transform.FindChild(name).gameObject;
						gameObject.CustomSetActive(false);
						num++;
					}
					List<Player>.Enumerator enumerator = allCampPlayers.GetEnumerator();
					Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
					COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
					while (enumerator.MoveNext())
					{
						Player current = enumerator.Current;
						if (current != null)
						{
							bool flag = current.PlayerId == hostPlayer.PlayerId;
							string name = (i != 1) ? string.Format("Down_Player{0}", current.CampPos + 1) : string.Format("Up_Player{0}", current.CampPos + 1);
							GameObject gameObject2 = transform.FindChild(name).gameObject;
							gameObject2.CustomSetActive(true);
							GameObject gameObject3 = gameObject2.transform.Find("RankFrame").gameObject;
							bool flag2 = current.PlayerCamp == playerCamp && (!Singleton<WatchController>.GetInstance().IsWatching || !Singleton<WatchController>.GetInstance().IsReplaying);
							if (gameObject3 != null)
							{
								if (flag2)
								{
									string rankFrameIconPath = CLadderView.GetRankFrameIconPath((byte)current.GradeOfRank, current.ClassOfRank);
									if (string.IsNullOrEmpty(rankFrameIconPath))
									{
										gameObject3.CustomSetActive(false);
									}
									else
									{
										Image component = gameObject3.GetComponent<Image>();
										if (component != null)
										{
											component.SetSprite(rankFrameIconPath, cUIFormScript, true, false, false, false);
										}
										gameObject3.CustomSetActive(true);
									}
								}
								else
								{
									gameObject3.CustomSetActive(false);
								}
							}
							Transform transform2 = gameObject2.transform.Find("RankClassText");
							if (transform2 != null)
							{
								GameObject gameObject4 = transform2.gameObject;
								if (flag2 && CLadderView.IsSuperKing((byte)current.GradeOfRank, current.ClassOfRank))
								{
									gameObject4.CustomSetActive(true);
									gameObject4.GetComponent<Text>().text = current.ClassOfRank.ToString();
								}
								else
								{
									gameObject4.CustomSetActive(false);
								}
							}
							Text component2 = gameObject2.transform.Find("Txt_PlayerName/Name").gameObject.GetComponent<Text>();
							component2.text = current.Name;
							Image component3 = gameObject2.transform.Find("Txt_PlayerName/NobeIcon").gameObject.GetComponent<Image>();
							if (component3)
							{
								MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component3, (int)current.VipLv, true, flag, current.privacyBits);
							}
							Text component4 = gameObject2.transform.Find("Txt_HeroName").gameObject.GetComponent<Text>();
							actorMeta.ConfigId = (int)current.CaptainId;
							component4.text = ((!actorDataProvider.GetActorStaticData(ref actorMeta, ref actorStaticData)) ? null : actorStaticData.TheResInfo.Name);
							GameObject gameObject5 = gameObject2.transform.Find("Txt_Qinmidu").gameObject;
							if (gameObject5 != null)
							{
								if (current.IntimacyData != null)
								{
									gameObject5.CustomSetActive(true);
									Text component5 = gameObject5.transform.Find("Text").gameObject.GetComponent<Text>();
									if (component5 != null)
									{
										component5.text = current.IntimacyData.title;
									}
									GameObject gameObject6 = gameObject5.transform.Find("BlueBg").gameObject;
									GameObject gameObject7 = gameObject5.transform.Find("RedBg").gameObject;
									if (current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY || current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK)
									{
										gameObject6.CustomSetActive(true);
										gameObject7.CustomSetActive(false);
									}
									else if (current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER || current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE)
									{
										gameObject6.CustomSetActive(false);
										gameObject7.CustomSetActive(true);
									}
								}
								else
								{
									gameObject5.CustomSetActive(false);
								}
							}
							GameObject gameObject8 = gameObject2.transform.Find("btns").gameObject;
							if (gameObject8 != null)
							{
								if (current.IntimacyData != null && IntimacyRelationViewUT.IsRelaState(current.IntimacyData.state))
								{
									int relaLevel = IntimacyRelationViewUT.CalcRelaLevel(current.IntimacyData.intimacyValue);
									string relaIconByRelaLevel = IntimacyRelationViewUT.GetRelaIconByRelaLevel(relaLevel, current.IntimacyData.state);
									if (!string.IsNullOrEmpty(relaIconByRelaLevel))
									{
										Image componetInChild = Utility.GetComponetInChild<Image>(gameObject8, "btnRela");
										if (componetInChild != null)
										{
											componetInChild.gameObject.CustomSetActive(true);
											componetInChild.SetSprite(relaIconByRelaLevel, cUIFormScript, true, false, false, false);
										}
									}
								}
								else
								{
									gameObject8.CustomSetActive(false);
								}
							}
							component2.color = Color.white;
							GameObject gameObject9 = gameObject2.transform.Find("Txt_PlayerName_Mine").gameObject;
							if (flag)
							{
								gameObject9.CustomSetActive(true);
							}
							else
							{
								gameObject9.CustomSetActive(false);
							}
							GameObject gameObject10 = gameObject2.transform.Find("Txt_LoadingPct").gameObject;
							if (gameObject10)
							{
								Text component6 = gameObject10.GetComponent<Text>();
								if (current.Computer)
								{
									component6.text = ((!curLvelContext.m_isWarmBattle) ? "100%" : "1%");
								}
								else
								{
									component6.text = ((!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover && !Singleton<WatchController>.GetInstance().IsWatching) ? "1%" : "100%");
								}
							}
							ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(current.CaptainId);
							if (dataByKey != null)
							{
								actorMeta2.PlayerId = current.PlayerId;
								actorMeta2.ActorCamp = (COM_PLAYERCAMP)i;
								actorMeta2.ConfigId = (int)dataByKey.dwCfgID;
								actorMeta2.ActorType = ActorTypeDef.Actor_Type_Hero;
								Image component7 = gameObject2.transform.Find("Hero").gameObject.GetComponent<Image>();
								if (actorDataProvider2.GetActorServerData(ref actorMeta2, ref actorServerData))
								{
									ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint)actorServerData.TheActorMeta.ConfigId, actorServerData.SkinId);
									if (heroSkin != null)
									{
										component7.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID), cUIFormScript, true, false, false, true);
										if (heroSkin.dwSkinID > 0u)
										{
											component4.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("LoadingSkinNameTxt"), heroSkin.szSkinName, heroSkin.szHeroName);
										}
										if (flag)
										{
											component4.color = CUIUtility.s_Text_Color_MyHeroName;
											Outline component8 = component4.gameObject.GetComponent<Outline>();
											if (component8 != null)
											{
												component8.effectColor = CUIUtility.s_Text_OutLineColor_MyHeroName;
											}
										}
									}
									bool flag3 = current.PlayerCamp == playerCamp && (!Singleton<WatchController>.GetInstance().IsWatching || !Singleton<WatchController>.GetInstance().IsReplaying) && (curLvelContext.m_isWarmBattle || !current.Computer);
									Transform transform3 = gameObject2.transform.Find("heroProficiencyBgImg");
									if (transform3 != null)
									{
										transform3.gameObject.CustomSetActive(flag3);
										if (flag3)
										{
											CUICommonSystem.SetHeroProficiencyBgImage(cUIFormScript, transform3.gameObject, (int)actorServerData.TheProficiencyInfo.Level, true);
										}
									}
									Transform transform4 = gameObject2.transform.Find("heroProficiencyImg");
									if (transform4 != null)
									{
										transform4.gameObject.CustomSetActive(flag3);
										if (flag3)
										{
											CUICommonSystem.SetHeroProficiencyIconImage(cUIFormScript, transform4.gameObject, (int)actorServerData.TheProficiencyInfo.Level);
										}
									}
								}
								else
								{
									component7.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), cUIFormScript, true, false, false, true);
								}
								uint num2 = 0u;
								if (actorDataProvider2.GetActorServerCommonSkillData(ref actorMeta2, out num2) && num2 != 0u)
								{
									ResSkillCfgInfo dataByKey2 = GameDataMgr.skillDatabin.GetDataByKey(num2);
									if (dataByKey2 != null)
									{
										gameObject2.transform.Find("SelSkill").gameObject.CustomSetActive(true);
										string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey2.szIconPath));
										Image component9 = gameObject2.transform.Find("SelSkill/Icon").GetComponent<Image>();
										component9.SetSprite(prefabPath, cUIFormScript.GetComponent<CUIFormScript>(), true, false, false, false);
									}
									else
									{
										gameObject2.transform.Find("SelSkill").gameObject.CustomSetActive(false);
									}
								}
								else
								{
									gameObject2.transform.Find("SelSkill").gameObject.CustomSetActive(false);
								}
								Transform transform5 = gameObject2.transform.Find("skinLabelImage");
								if (transform5 != null)
								{
									CUICommonSystem.SetHeroSkinLabelPic(cUIFormScript, transform5.gameObject, dataByKey.dwCfgID, actorServerData.SkinId);
								}
							}
						}
						else
						{
							DebugHelper.Assert(false, "Loading Player is Null");
						}
					}
				}
			}
			GameObject widget = cUIFormScript.GetWidget(0);
			GameObject widget2 = cUIFormScript.GetWidget(1);
			GameObject widget3 = cUIFormScript.GetWidget(2);
			if (curLvelContext.IsGameTypeGuildMatch())
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(true);
				CSDT_CAMP_EXT_GUILDMATCH[] campExtGuildMatchInfo = curLvelContext.GetCampExtGuildMatchInfo();
				if (campExtGuildMatchInfo != null && campExtGuildMatchInfo.Length == 2)
				{
					Image component10 = cUIFormScript.GetWidget(3).GetComponent<Image>();
					Text component11 = cUIFormScript.GetWidget(4).GetComponent<Text>();
					Image component12 = cUIFormScript.GetWidget(5).GetComponent<Image>();
					Text component13 = cUIFormScript.GetWidget(6).GetComponent<Text>();
					component10.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + campExtGuildMatchInfo[0].dwGuildHeadID, cUIFormScript, true, false, false, false);
					component11.text = StringHelper.UTF8BytesToString(ref campExtGuildMatchInfo[0].szGuildName);
					component12.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + campExtGuildMatchInfo[1].dwGuildHeadID, cUIFormScript, true, false, false, false);
					component13.text = StringHelper.UTF8BytesToString(ref campExtGuildMatchInfo[1].szGuildName);
				}
			}
			else
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(false);
				Text component14 = cUIFormScript.GetWidget(7).GetComponent<Text>();
				component14.text = CUILoadingSystem.GenerateRandomPvpLoadingTips(CUILoadingSystem.GenerateMultiRandomNum());
				widget2.CustomSetActive(MonoSingleton<Reconnection>.instance.isProcessingRelayRecover);
			}
		}

		private static string GenerateRandomPveLoadingTips(int randNum)
		{
			string text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVE_Tips_{0}", randNum));
			if (string.IsNullOrEmpty(text))
			{
				text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVE_Tips_{0}", 0));
			}
			return text;
		}

		private static string GenerateRandomPvpLoadingTips(int randNum)
		{
			string text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVP_Tips_{0}", randNum));
			if (string.IsNullOrEmpty(text))
			{
				text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVP_Tips_{0}", 0));
			}
			return text;
		}

		private static int GenerateSoloRandomNum()
		{
			int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(93u).dwConfValue;
			if (CUILoadingSystem.SoloRandomNum > dwConfValue || CUILoadingSystem.SoloRandomNum < 4)
			{
				CUILoadingSystem.SoloRandomNum = 4;
			}
			return CUILoadingSystem.SoloRandomNum++;
		}

		private static int GenerateMultiRandomNum()
		{
			int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(94u).dwConfValue;
			int pvpLoadingIndex = CUILoadingSystem._pvpLoadingIndex;
			CUILoadingSystem._pvpLoadingIndex++;
			if (CUILoadingSystem._pvpLoadingIndex > dwConfValue)
			{
				CUILoadingSystem._pvpLoadingIndex = 1;
			}
			return pvpLoadingIndex;
		}

		private static Player GetPlayer(COM_PLAYERCAMP Camp, int Pos)
		{
			List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(Camp).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (Pos == enumerator.Current.CampPos + 1)
				{
					return enumerator.Current;
				}
			}
			return null;
		}

		private static uint GetPlayerId(COM_PLAYERCAMP Camp, int Pos)
		{
			List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.PlayerCamp == Camp && Pos == enumerator.Current.CampPos + 1)
				{
					return enumerator.Current.PlayerId;
				}
			}
			return 0u;
		}

		private static GameObject GetMemberItem(uint ObjId)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUILoadingSystem.PVP_PATH_LOADING);
			if (form == null)
			{
				return null;
			}
			List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.PlayerId == ObjId)
				{
					int num = enumerator.Current.CampPos + 1;
					return (enumerator.Current.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? form.gameObject.transform.FindChild("DownPanel").FindChild(string.Format("Down_Player{0}", num)).gameObject : form.gameObject.transform.FindChild("UpPanel").FindChild(string.Format("Up_Player{0}", num)).gameObject;
				}
			}
			return null;
		}
	}
}
