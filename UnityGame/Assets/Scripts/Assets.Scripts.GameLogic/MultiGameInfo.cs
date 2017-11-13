using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class MultiGameInfo : GameInfoBase
	{
		public override void PreBeginPlay()
		{
			base.PreBeginPlay();
			this.PreparePlayer();
			this.ResetSynchrConfig();
			this.LoadAllTeamActors();
		}

		protected virtual void PreparePlayer()
		{
			MultiGameContext multiGameContext = this.GameContext as MultiGameContext;
			DebugHelper.Assert(multiGameContext != null);
			if (multiGameContext == null)
			{
				return;
			}
			Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
			uint num = 0u;
			string text = string.Empty;
			for (int i = 0; i < 2; i++)
			{
				text = text + "camp" + i.ToString() + "|";
				uint num2 = 0u;
				while (num2 < multiGameContext.MessageRef.astCampInfo[i].dwPlayerNum)
				{
					CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[(int)((UIntPtr)num2)];
					if (cSDT_CAMPPLAYERINFO == null)
					{
						num2 += 1u;
					}
					else
					{
						uint dwObjId = cSDT_CAMPPLAYERINFO.stPlayerInfo.dwObjId;
						Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(dwObjId);
						DebugHelper.Assert(player == null, "你tm肯定在逗我");
						if (num == 0u && i == 0)
						{
							num = dwObjId;
						}
						bool flag = cSDT_CAMPPLAYERINFO.stPlayerInfo.bObjType == 2;
						if (player == null)
						{
							string openId = string.Empty;
							uint vipLv = 0u;
							int honorId = 0;
							int honorLevel = 0;
							uint wangZheCnt = 0u;
							ulong num3 = 0uL;
							uint num4 = 0u;
							uint level = cSDT_CAMPPLAYERINFO.stPlayerInfo.dwLevel;
							if (flag)
							{
								if (Convert.ToBoolean(multiGameContext.MessageRef.stDeskInfo.bIsWarmBattle))
								{
									num3 = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
									num4 = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
									level = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
								}
							}
							else
							{
								num3 = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
								num4 = (uint)cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
								openId = Utility.UTF8Convert(cSDT_CAMPPLAYERINFO.szOpenID);
								vipLv = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
								honorId = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorID;
								honorLevel = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorLevel;
								wangZheCnt = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.dwWangZheCnt;
							}
							GameIntimacyData intimacyData = null;
							if (!flag)
							{
								string text2 = StringHelper.UTF8BytesToString(ref cSDT_CAMPPLAYERINFO.stPlayerInfo.szName);
								byte bIntimacyRelationPrior = cSDT_CAMPPLAYERINFO.stIntimacyRelation.bIntimacyRelationPrior;
								COMDT_INTIMACY_CARD_INFO stIntimacyRelationData = cSDT_CAMPPLAYERINFO.stIntimacyRelation.stIntimacyRelationData;
								LoadEnt loadEnt = new LoadEnt();
								for (int j = 0; j < (int)stIntimacyRelationData.bIntimacyNum; j++)
								{
									COMDT_INTIMACY_CARD_DATA cOMDT_INTIMACY_CARD_DATA = stIntimacyRelationData.astIntimacyData[j];
									if (cOMDT_INTIMACY_CARD_DATA != null)
									{
										for (int k = 0; k < 2; k++)
										{
											int num5 = 0;
											while ((long)num5 < (long)((ulong)multiGameContext.MessageRef.astCampInfo[k].dwPlayerNum))
											{
												CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO2 = multiGameContext.MessageRef.astCampInfo[k].astCampPlayerInfo[num5];
												if (multiGameContext.MessageRef.astCampInfo[k].astCampPlayerInfo[num5].stPlayerInfo.bObjType == 2)
												{
													num5++;
												}
												else
												{
													ulong ullUid = cSDT_CAMPPLAYERINFO2.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
													int iLogicWorldID = cSDT_CAMPPLAYERINFO2.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
													if (cSDT_CAMPPLAYERINFO2 != null && (ullUid != num3 || (long)iLogicWorldID != (long)((ulong)num4)) && (int)cOMDT_INTIMACY_CARD_DATA.wIntimacyValue >= IntimacyRelationViewUT.Getlevel2_maxValue() && cOMDT_INTIMACY_CARD_DATA.stUin.ullUid == ullUid && (ulong)cOMDT_INTIMACY_CARD_DATA.stUin.dwLogicWorldId == (ulong)((long)iLogicWorldID) && IntimacyRelationViewUT.IsRelationPriorityHigher(cOMDT_INTIMACY_CARD_DATA.bIntimacyState, loadEnt.state, bIntimacyRelationPrior))
													{
														string text3 = StringHelper.UTF8BytesToString(ref cSDT_CAMPPLAYERINFO2.stPlayerInfo.szName);
														string text4 = string.Format("----FRData Loading s1: 玩家:{0} ----对方名字:{1}, 新关系:{2}替换之前关系:{3}", new object[]
														{
															text2,
															text3,
															(COM_INTIMACY_STATE)cOMDT_INTIMACY_CARD_DATA.bIntimacyState,
															loadEnt.state
														});
														loadEnt.intimacyValue = (int)cOMDT_INTIMACY_CARD_DATA.wIntimacyValue;
														loadEnt.otherSideName = text3;
														loadEnt.state = (COM_INTIMACY_STATE)cOMDT_INTIMACY_CARD_DATA.bIntimacyState;
														loadEnt.otherSideUlluid = ullUid;
														loadEnt.otherSideWorldId = iLogicWorldID;
													}
													num5++;
												}
											}
										}
									}
								}
								if (loadEnt.state != COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
								{
									string relationInLoadingMenu = IntimacyRelationViewUT.GetRelationInLoadingMenu((byte)loadEnt.state, loadEnt.otherSideName);
									intimacyData = new GameIntimacyData(loadEnt.intimacyValue, loadEnt.state, loadEnt.otherSideUlluid, loadEnt.otherSideWorldId, relationInLoadingMenu);
								}
							}
							player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(dwObjId, i + COM_PLAYERCAMP.COM_PLAYERCAMP_1, (int)cSDT_CAMPPLAYERINFO.stPlayerInfo.bPosOfCamp, level, flag, Utility.UTF8Convert(cSDT_CAMPPLAYERINFO.stPlayerInfo.szName), 0, (int)num4, num3, vipLv, openId, cSDT_CAMPPLAYERINFO.dwShowGradeOfRank, cSDT_CAMPPLAYERINFO.dwClassOfRank, wangZheCnt, honorId, honorLevel, intimacyData, cSDT_CAMPPLAYERINFO.ullUserPrivacyBits);
							DebugHelper.Assert(player != null, "创建player失败");
							if (player != null)
							{
								player.isGM = (cSDT_CAMPPLAYERINFO.bIsGM > 0);
							}
						}
						for (int l = 0; l < cSDT_CAMPPLAYERINFO.stPlayerInfo.astChoiceHero.Length; l++)
						{
							COMDT_CHOICEHERO cOMDT_CHOICEHERO = cSDT_CAMPPLAYERINFO.stPlayerInfo.astChoiceHero[l];
							int dwHeroID = (int)cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID;
							player.AddHero((uint)dwHeroID);
						}
						if (player != null)
						{
							string text5 = string.Format("{0}:{1}|", player.OpenId, player.LogicWrold);
							text += text5;
						}
						num2 += 1u;
					}
				}
			}
			MonoSingleton<TGPSDKSys>.GetInstance().GameStart(text);
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<WatchController>.GetInstance().TargetUID);
				if (playerByUid != null)
				{
					Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerByUid.PlayerId);
				}
				else
				{
					Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(num);
				}
			}
			else
			{
				Player playerByUid2 = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<CRoleInfoManager>.instance.masterUUID);
				DebugHelper.Assert(playerByUid2 != null, "load multi game but hostPlayer is null");
				Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerByUid2.PlayerId);
			}
			multiGameContext.levelContext.m_isWarmBattle = Convert.ToBoolean(multiGameContext.MessageRef.stDeskInfo.bIsWarmBattle);
			multiGameContext.SaveServerData();
		}

		protected virtual void ResetSynchrConfig()
		{
			MultiGameContext multiGameContext = this.GameContext as MultiGameContext;
			DebugHelper.Assert(multiGameContext != null);
			Singleton<FrameSynchr>.GetInstance().SetSynchrConfig(multiGameContext.MessageRef.dwKFrapsFreqMs, (uint)multiGameContext.MessageRef.bKFrapsLater, (uint)multiGameContext.MessageRef.bPreActFrap, multiGameContext.MessageRef.dwRandomSeed);
		}

		public override void OnLoadingProgress(float Progress)
		{
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1083u);
				cSPkg.stPkgData.stMultGameLoadProcessReq.wProcess = Convert.ToUInt16(Progress * 100f);
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			}
			CUILoadingSystem.OnSelfLoadProcess(Progress);
		}

		public override void StartFight()
		{
			base.StartFight();
		}

		public override void EndGame()
		{
			base.EndGame();
		}
	}
}
