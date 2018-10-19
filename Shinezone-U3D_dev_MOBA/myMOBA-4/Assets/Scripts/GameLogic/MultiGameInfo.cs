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
                // check team1 and team2 players
				text = text + "camp" + i.ToString() + "|";
				uint num3 = 0;
				while (num3 < multiGameContext.MessageRef.astCampInfo[i].dwPlayerNum)
				{
                    CSDT_CAMPPLAYERINFO playerInfo = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3];
                    if (playerInfo == null)
                    {
                        num3++;
                        continue;
                    }

                    uint dwObjId = playerInfo.stPlayerInfo.dwObjId;
					Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(dwObjId);
					DebugHelper.Assert(player == null, "你tm肯定在逗我");
					if (num == 0u && i == 0)
					{
						num = dwObjId;
					}
					bool bIsNPC = playerInfo.stPlayerInfo.bObjType == 2;
					if (player == null)
					{
                        string openId = string.Empty;
                        uint vipLv = 0u;
                        int honorId = 0;
                        int honorLevel = 0;
                        uint wangZheCnt = 0u;
                        ulong uid = 0uL;
                        uint logicWrold = 0u;
                        uint level = playerInfo.stPlayerInfo.dwLevel;
                        if (bIsNPC)
                        {
                            if (Convert.ToBoolean(multiGameContext.MessageRef.stDeskInfo.bIsWarmBattle))
                            {
                                uid = playerInfo.stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
                                logicWrold = playerInfo.stPlayerInfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
                                level = playerInfo.stPlayerInfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
                            }
                        }
                        else
                        {
                            uid = playerInfo.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                            logicWrold = (uint)playerInfo.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
                            openId = Utility.UTF8Convert(playerInfo.szOpenID);
                            vipLv = playerInfo.stPlayerInfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
                            honorId = playerInfo.stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorID;
                            honorLevel = playerInfo.stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorLevel;
                            wangZheCnt = playerInfo.stPlayerInfo.stDetail.stPlayerOfAcnt.dwWangZheCnt;
                        }

                        GameIntimacyData intimacyData = null;
                        if (!bIsNPC)
                        {
                            string playerName = StringHelper.UTF8BytesToString(ref playerInfo.stPlayerInfo.szName);
                            byte bIntimacyRelationPrior = playerInfo.stIntimacyRelation.bIntimacyRelationPrior;
                            COMDT_INTIMACY_CARD_INFO stIntimacyRelationData = playerInfo.stIntimacyRelation.stIntimacyRelationData;
                            LoadEnt loadEnt = new LoadEnt();
                            for (int j = 0; j < (int)stIntimacyRelationData.bIntimacyNum; j++)
                            {
                                COMDT_INTIMACY_CARD_DATA cardData = stIntimacyRelationData.astIntimacyData[j];
                                if (cardData == null)
                                {
                                    continue;
                                }

                                for (int k = 0; k < 2; k++)
                                {
                                    int num4 = 0;
                                    while ((long)num4 < (long)((ulong)multiGameContext.MessageRef.astCampInfo[k].dwPlayerNum))
                                    {
                                        CSDT_CAMPPLAYERINFO playerInfo2 = multiGameContext.MessageRef.astCampInfo[k].astCampPlayerInfo[num4];
                                        if (multiGameContext.MessageRef.astCampInfo[k].astCampPlayerInfo[num4].stPlayerInfo.bObjType == 2)
                                        {
                                            num4++;
                                            continue;
                                        }

                                        ulong ullUid2 = playerInfo2.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                                        int iLogicWorldID2 = playerInfo2.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
                                        if (playerInfo2 != null && (ullUid2 != uid || iLogicWorldID2 != logicWrold))
                                        {
                                            if ((int)cardData.wIntimacyValue >= IntimacyRelationViewUT.Getlevel2_maxValue() &&
                                                cardData.stUin.ullUid == ullUid2 &&
                                                (ulong)cardData.stUin.dwLogicWorldId == (ulong)((long)iLogicWorldID2) &&
                                                IntimacyRelationViewUT.IsRelationPriorityHigher(cardData.bIntimacyState, loadEnt.state, bIntimacyRelationPrior))
                                            {
                                                string text3 = StringHelper.UTF8BytesToString(ref playerInfo2.stPlayerInfo.szName);
                                                string text4 = string.Format("----FRData Loading s1: 玩家:{0} ----对方名字:{1}, 新关系:{2}替换之前关系:{3}", new object[]
                                                {
                                                    playerName,
                                                    text3,
                                                    (COM_INTIMACY_STATE)cardData.bIntimacyState,
                                                    loadEnt.state
                                                });
                                                loadEnt.intimacyValue = (int)cardData.wIntimacyValue;
                                                loadEnt.otherSideName = text3;
                                                loadEnt.state = (COM_INTIMACY_STATE)cardData.bIntimacyState;
                                                loadEnt.otherSideUlluid = ullUid2;
                                                loadEnt.otherSideWorldId = iLogicWorldID2;
                                            }
                                        }

                                        num4++;
                                    }
                                }
                            }
                            if (loadEnt.state != COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
                            {
                                string relationInLoadingMenu = IntimacyRelationViewUT.GetRelationInLoadingMenu((byte)loadEnt.state, loadEnt.otherSideName);
                                intimacyData = new GameIntimacyData(loadEnt.intimacyValue, loadEnt.state, loadEnt.otherSideUlluid, loadEnt.otherSideWorldId, relationInLoadingMenu);
                            }
                        }

                        player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(dwObjId, 
                            i + COM_PLAYERCAMP.COM_PLAYERCAMP_1,
                            playerInfo.stPlayerInfo.bPosOfCamp, 
                            level, 
                            bIsNPC, 
                            Utility.UTF8Convert(playerInfo.stPlayerInfo.szName), 
                            0, 
                            (int)logicWrold, 
                            uid, 
                            vipLv, 
                            openId,
                            playerInfo.dwShowGradeOfRank,
                            playerInfo.dwClassOfRank, 
                            wangZheCnt, 
                            honorId, 
                            honorLevel, 
                            intimacyData,
                            playerInfo.ullUserPrivacyBits);
                        DebugHelper.Assert(player != null, "创建player失败");
                        if (player != null)
                        {
                            player.isGM = (playerInfo.bIsGM > 0);
                        }
                    }

					for (int l = 0; l < playerInfo.stPlayerInfo.astChoiceHero.Length; l++)
					{
						COMDT_CHOICEHERO hero = playerInfo.stPlayerInfo.astChoiceHero[l];
						int dwHeroID = (int)hero.stBaseInfo.stCommonInfo.dwHeroID;
                        player.AddHero((uint)dwHeroID);
					}

					if (player != null)
					{
						string str = string.Format("{0}:{1}|", player.OpenId, player.LogicWrold);
						text += str;
					}

					num3++;
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
				Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<CRoleInfoManager>.instance.masterUUID);
				DebugHelper.Assert(hostPlayer != null, "load multi game but hostPlayer is null");
				Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(hostPlayer.PlayerId);
			}
			multiGameContext.levelContext.m_isWarmBattle = Convert.ToBoolean(multiGameContext.MessageRef.stDeskInfo.bIsWarmBattle);
			multiGameContext.SaveServerData();
		}

		protected virtual void ResetSynchrConfig()
		{
			MultiGameContext multiGameContext = this.GameContext as MultiGameContext;
			DebugHelper.Assert(multiGameContext != null);
			Singleton<FrameSynchr>.GetInstance().SetSynchrConfig(
                multiGameContext.MessageRef.dwKFrapsFreqMs, 
                (uint)multiGameContext.MessageRef.bKFrapsLater, 
                (uint)multiGameContext.MessageRef.bPreActFrap, 
                multiGameContext.MessageRef.dwRandomSeed);
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
