using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class MultiGameContext : GameContextBase
	{
		public SCPKG_MULTGAME_BEGINLOAD MessageRef;

		public MultiGameContext(SCPKG_MULTGAME_BEGINLOAD InMessage)
		{
			this.MessageRef = InMessage;
			if (CheatCommandReplayEntry.heroPerformanceTest)
			{
				this.InitPerformanceTest();
			}
			uint dwMapId = this.MessageRef.stDeskInfo.dwMapId;
			byte bMapType = this.MessageRef.stDeskInfo.bMapType;
			this.LevelContext = CLevelCfgLogicManager.CreatePvpLevelContext(bMapType, dwMapId, (COM_GAME_TYPE)InMessage.bGameType, 1);
			this.LevelContext.m_isWarmBattle = Convert.ToBoolean(InMessage.stDeskInfo.bIsWarmBattle);
			this.LevelContext.SetWarmHeroAiDiff(InMessage.stDeskInfo.bAILevel);
			this.LevelContext.SetCampExtInfo(InMessage.astCampInfo);
			base.levelContext.dwDeskId = InMessage.dwDeskId;
			base.levelContext.dwDeskSeq = InMessage.dwDeskSeq;
			Singleton<GameLogic>.GetInstance().HashCheckFreq = InMessage.dwHaskChkFreq;
			Singleton<GameLogic>.GetInstance().SnakeTraceMasks = InMessage.dwCltLogMask;
			Singleton<GameLogic>.GetInstance().SnakeTraceSize = InMessage.dwCltLogSize;
		}

		private void InitPerformanceTest()
		{
			for (int i = 0; i < this.MessageRef.astCampInfo.Length; i++)
			{
				CSDT_CAMPINFO cSDT_CAMPINFO = this.MessageRef.astCampInfo[i];
				COMDT_HERO_COMMON_INFO stCommonInfo = cSDT_CAMPINFO.astCampPlayerInfo[0].stPlayerInfo.astChoiceHero[0].stBaseInfo.stCommonInfo;
				uint dwHeroID = stCommonInfo.dwHeroID;
				ushort wSkinID = stCommonInfo.wSkinID;
				int num = 0;
				while ((long)num < (long)((ulong)cSDT_CAMPINFO.dwPlayerNum))
				{
					CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO = cSDT_CAMPINFO.astCampPlayerInfo[num];
					if (cSDT_CAMPPLAYERINFO != null)
					{
						for (int j = 0; j < cSDT_CAMPPLAYERINFO.stPlayerInfo.astChoiceHero.Length; j++)
						{
							COMDT_CHOICEHERO cOMDT_CHOICEHERO = cSDT_CAMPPLAYERINFO.stPlayerInfo.astChoiceHero[j];
							if (cOMDT_CHOICEHERO != null)
							{
								cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID = dwHeroID;
								cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.wSkinID = wSkinID;
							}
						}
					}
					num++;
				}
			}
		}

		public override GameInfoBase CreateGameInfo()
		{
			PVPMobaGame pVPMobaGame = new PVPMobaGame();
			pVPMobaGame.Initialize(this);
			return pVPMobaGame;
		}

		public void SaveServerData()
		{
			Singleton<ActorDataCenter>.instance.ClearHeroServerData();
			if (this.MessageRef == null)
			{
				return;
			}
			for (int i = 0; i < this.MessageRef.astCampInfo.Length; i++)
			{
				CSDT_CAMPINFO cSDT_CAMPINFO = this.MessageRef.astCampInfo[i];
				int num = Mathf.Min(cSDT_CAMPINFO.astCampPlayerInfo.Length, (int)cSDT_CAMPINFO.dwPlayerNum);
				for (int j = 0; j < num; j++)
				{
					COMDT_PLAYERINFO stPlayerInfo = cSDT_CAMPINFO.astCampPlayerInfo[j].stPlayerInfo;
					Singleton<ActorDataCenter>.instance.AddHeroesServerData(stPlayerInfo.dwObjId, stPlayerInfo.astChoiceHero);
				}
			}
		}
	}
}
