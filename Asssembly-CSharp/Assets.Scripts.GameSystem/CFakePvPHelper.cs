using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
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
	internal class CFakePvPHelper
	{
		private class FakePlayerConfirm
		{
			public bool bConfirmed;

			public int confirmWaitTime;

			public COMDT_FAKEACNT_DETAIL FakePlayer;
		}

		private class FakeHeroSelect
		{
			public bool bConfirmed;

			public int changeHeroCount;

			public int maxChangeHeroCount;

			public uint selectedHero;

			public int selectedSkin;

			public uint selectedPlayerSkill;

			public int nextActionSec;

			public int idleSec;

			public COMDT_FAKEACNT_DETAIL FakePlayer;
		}

		public const int MAX_CONFIRM_TIME = 20;

		public const int CONFIRM_TIMEOUT = 30;

		public const int SELECT_HERO_TIMER_COUNT = 60;

		public const int MAX_CHANGE_HERO_TIME = 3;

		public const int MAX_LUANDOU_CHANGE_HERO_TIME = 2;

		public const int MAX_ROOKIE_LEVEL = 6;

		public static readonly int[] FAKE_CONFIRM_MAP_3V3 = new int[]
		{
			2,
			default(int),
			1
		};

		public static readonly int[] FAKE_CONFIRM_MAP_5V5 = new int[]
		{
			3,
			1,
			0,
			4,
			3
		};

		private static int MapPlayerNum;

		private static int RealPlayerConfirmNum;

		private static int FakePlayerConfirmNum;

		private static int CurrentSelectTime;

		private static int ConfirmedFakePlayerNum;

		private static ListView<CFakePvPHelper.FakePlayerConfirm> FakePlayerList = new ListView<CFakePvPHelper.FakePlayerConfirm>();

		private static Dictionary<ulong, uint> ChosenHeroes = new Dictionary<ulong, uint>();

		private static List<uint> EnemyChosenHeroes = new List<uint>();

		private static int HeroConfirmedPlayerNum;

		private static ListView<CFakePvPHelper.FakeHeroSelect> FakeHeroSelectList = new ListView<CFakePvPHelper.FakeHeroSelect>();

		public static bool bInFakeConfirm
		{
			get;
			private set;
		}

		public static bool bInFakeSelect
		{
			get;
			private set;
		}

		private static void DatabinCheck()
		{
			DebugHelper.Assert(GameDataMgr.robotRookieHeroSkinDatabin.Count() > 5, "Not Enough Hero");
			DebugHelper.Assert(GameDataMgr.robotVeteranHeroSkinDatabin.Count() > 5, "Not Enough Hero");
			int num = GameDataMgr.robotPlayerSkillDatabin.Count();
			for (int i = 0; i < num; i++)
			{
				ResFakeAcntSkill dataByIndex = GameDataMgr.robotPlayerSkillDatabin.GetDataByIndex(i);
				uint num2 = 0u;
				for (int j = 0; j < dataByIndex.SkillId.Length; j++)
				{
					num2 += dataByIndex.SkillId[j];
				}
				DebugHelper.Assert(num2 > 0u, "Invalid PlayerSkill Databin");
			}
		}

		private static void RemoveAllFakeTimer()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.FakeConfirmLater));
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeConfirm));
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnConfirmTimout));
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeSelectHero));
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnSelectHeroTimeout));
		}

		public static void SetConfirmFakeData()
		{
			CFakePvPHelper.RemoveAllFakeTimer();
			CFakePvPHelper.DatabinCheck();
			CFakePvPHelper.RealPlayerConfirmNum = 0;
			CFakePvPHelper.FakePlayerConfirmNum = 0;
			CFakePvPHelper.FakePlayerList.Clear();
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			DebugHelper.Assert(roomInfo != null);
			DebugHelper.Assert(roomInfo.roomAttrib.bWarmBattle);
			for (COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1; cOM_PLAYERCAMP < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; cOM_PLAYERCAMP++)
			{
				ListView<MemberInfo> listView = roomInfo[cOM_PLAYERCAMP];
				for (int i = 0; i < listView.Count; i++)
				{
					MemberInfo memberInfo = listView[i];
					if (memberInfo.RoomMemberType == 2u)
					{
						CFakePvPHelper.FakePlayerConfirm fakePlayerConfirm = new CFakePvPHelper.FakePlayerConfirm();
						fakePlayerConfirm.FakePlayer = memberInfo.WarmNpc;
						fakePlayerConfirm.confirmWaitTime = Random.Range(2, 11);
						CFakePvPHelper.FakePlayerList.Add(fakePlayerConfirm);
					}
				}
			}
		}

		public static void StartFakeConfirm()
		{
			CFakePvPHelper.bInFakeConfirm = true;
			CFakePvPHelper.CurrentSelectTime = 0;
			CFakePvPHelper.ConfirmedFakePlayerNum = 0;
			Singleton<CTimerManager>.GetInstance().AddTimer(1000, 20, new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeConfirm));
			Singleton<CTimerManager>.GetInstance().AddTimer(30000, 1, new CTimer.OnTimeUpHandler(CFakePvPHelper.OnConfirmTimout));
		}

		private static void OnConfirmTimout(int seq)
		{
			CFakePvPHelper.bInFakeConfirm = false;
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			if (roomInfo != null)
			{
				roomInfo.roomAttrib.bWarmBattle = false;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
			if (form != null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(form);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Err_NM_Cancel"), false, 1.5f, null, new object[0]);
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1058u);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		private static List<CFakePvPHelper.FakePlayerConfirm> SelectConfirmPlayer(int time)
		{
			List<CFakePvPHelper.FakePlayerConfirm> list = new List<CFakePvPHelper.FakePlayerConfirm>();
			for (int i = 0; i < CFakePvPHelper.FakePlayerList.Count; i++)
			{
				if (!CFakePvPHelper.FakePlayerList[i].bConfirmed && CFakePvPHelper.FakePlayerList[i].confirmWaitTime == time)
				{
					list.Add(CFakePvPHelper.FakePlayerList[i]);
				}
			}
			return list;
		}

		private static void UpdateFakeConfirm(int seq)
		{
			List<CFakePvPHelper.FakePlayerConfirm> list = CFakePvPHelper.SelectConfirmPlayer(CFakePvPHelper.CurrentSelectTime++);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
			if (form != null)
			{
				for (int i = 0; i < list.get_Count(); i++)
				{
					CFakePvPHelper.FakePlayerConfirm fakePlayerConfirm = list.get_Item(i);
					fakePlayerConfirm.bConfirmed = true;
					CFakePvPHelper.ConfirmedFakePlayerNum++;
					Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum++;
					CMatchingView.UpdateConfirmBox(form.gameObject, fakePlayerConfirm.FakePlayer.ullUid);
					if (CFakePvPHelper.ConfirmedFakePlayerNum == CFakePvPHelper.FakePlayerList.Count)
					{
						Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeConfirm));
					}
					if (Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum == Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum)
					{
						CFakePvPHelper.GotoHeroSelectPage();
					}
				}
			}
		}

		public static void OnSelfConfirmed(GameObject root, int PlayerNum)
		{
			Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum++;
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			DebugHelper.Assert(roomInfo != null);
			if (roomInfo != null)
			{
				CMatchingView.UpdateConfirmBox(root, roomInfo.selfInfo.ullUid);
				if (Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum == Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum)
				{
					CFakePvPHelper.GotoHeroSelectPage();
				}
			}
		}

		private static void GotoHeroSelectPage()
		{
			CFakePvPHelper.bInFakeConfirm = false;
			CFakePvPHelper.RealPlayerConfirmNum = 0;
			CFakePvPHelper.FakePlayerConfirmNum = 0;
			Singleton<LobbyLogic>.GetInstance().inMultiRoom = false;
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(roomInfo.roomAttrib.bMapType, roomInfo.roomAttrib.dwMapId);
			Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enPVE_Computer, pvpMapCommonInfo.bHeroNum, roomInfo.roomAttrib.dwMapId, roomInfo.roomAttrib.bMapType, (int)pvpMapCommonInfo.bIsAllowHeroDup);
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnConfirmTimout));
		}

		public static void UpdateConfirmBox(GameObject root, int PlayerNum)
		{
			DebugHelper.Assert(CFakePvPHelper.FakePlayerList.Count <= 5, string.Format("FakePlayerList Count Error!! Count: {0}", CFakePvPHelper.FakePlayerList.Count));
			CFakePvPHelper.MapPlayerNum = PlayerNum;
			CFakePvPHelper.RealPlayerConfirmNum++;
			if (CFakePvPHelper.RealPlayerConfirmNum == PlayerNum / 2)
			{
				for (int i = 0; i < CFakePvPHelper.FakePlayerList.Count; i++)
				{
					CFakePvPHelper.FakePlayerConfirm fakePlayerConfirm = CFakePvPHelper.FakePlayerList[i];
					if (!fakePlayerConfirm.bConfirmed)
					{
						fakePlayerConfirm.bConfirmed = true;
						CFakePvPHelper.FakePlayerConfirmNum++;
						Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum++;
						CMatchingView.UpdateConfirmBox(root, fakePlayerConfirm.FakePlayer.ullUid);
					}
				}
				Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.FakeConfirmLater));
			}
			else
			{
				Singleton<CTimerManager>.GetInstance().AddTimer(1000, 1, new CTimer.OnTimeUpHandler(CFakePvPHelper.FakeConfirmLater));
			}
		}

		private static void FakeConfirmLater(int seq)
		{
			if (CFakePvPHelper.FakePlayerConfirmNum < CFakePvPHelper.MapPlayerNum / 2)
			{
				int[] array = (CFakePvPHelper.MapPlayerNum == 6) ? CFakePvPHelper.FAKE_CONFIRM_MAP_3V3 : CFakePvPHelper.FAKE_CONFIRM_MAP_5V5;
				CFakePvPHelper.FakePlayerConfirm fakePlayerConfirm = CFakePvPHelper.FakePlayerList[array[CFakePvPHelper.FakePlayerConfirmNum]];
				fakePlayerConfirm.bConfirmed = true;
				CFakePvPHelper.FakePlayerConfirmNum++;
				Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum++;
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
				if (form != null)
				{
					CMatchingView.UpdateConfirmBox(form.gameObject, fakePlayerConfirm.FakePlayer.ullUid);
				}
			}
		}

		private static bool IsInSelectHero()
		{
			return !(Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath) == null);
		}

		public static void BeginFakeSelectHero()
		{
			CFakePvPHelper.HeroConfirmedPlayerNum = 0;
			CFakePvPHelper.FakeHeroSelectList.Clear();
			CFakePvPHelper.ChosenHeroes.Clear();
			CFakePvPHelper.EnemyChosenHeroes.Clear();
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			DebugHelper.Assert(roomInfo != null);
			DebugHelper.Assert(roomInfo.roomAttrib.bWarmBattle);
			COM_PLAYERCAMP selfCamp = roomInfo.GetSelfCamp();
			ListView<MemberInfo> listView = roomInfo[selfCamp];
			for (int i = 0; i < listView.Count; i++)
			{
				MemberInfo memberInfo = listView[i];
				if (memberInfo.RoomMemberType == 2u)
				{
					CFakePvPHelper.FakeHeroSelect fakeHeroSelect = new CFakePvPHelper.FakeHeroSelect();
					fakeHeroSelect.FakePlayer = memberInfo.WarmNpc;
					if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
					{
						fakeHeroSelect.maxChangeHeroCount = Random.Range(0, 2);
						CFakePvPHelper.DoSelectAction(ref fakeHeroSelect);
					}
					else
					{
						fakeHeroSelect.maxChangeHeroCount = Random.Range(1, 4);
					}
					fakeHeroSelect.nextActionSec = Random.Range(3, 6);
					CFakePvPHelper.FakeHeroSelectList.Add(fakeHeroSelect);
				}
			}
			if (CFakePvPHelper.FakeHeroSelectList.Count > 0)
			{
				Singleton<CTimerManager>.GetInstance().AddTimer(1000, 60, new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeSelectHero));
				CFakePvPHelper.bInFakeSelect = true;
			}
			Singleton<CTimerManager>.GetInstance().AddTimer(60000, 1, new CTimer.OnTimeUpHandler(CFakePvPHelper.OnSelectHeroTimeout));
		}

		private static void UpdateFakeSelectHero(int seq)
		{
			int num = 0;
			for (int i = 0; i < CFakePvPHelper.FakeHeroSelectList.Count; i++)
			{
				CFakePvPHelper.FakeHeroSelect fakeHeroSelect = CFakePvPHelper.FakeHeroSelectList[i];
				if (fakeHeroSelect.bConfirmed)
				{
					num++;
				}
				else if (fakeHeroSelect.idleSec == fakeHeroSelect.nextActionSec)
				{
					CFakePvPHelper.DoSelectAction(ref fakeHeroSelect);
					fakeHeroSelect.idleSec = 0;
					fakeHeroSelect.nextActionSec = Random.Range(3, 6);
				}
				else
				{
					fakeHeroSelect.idleSec++;
				}
			}
			if (num == CFakePvPHelper.FakeHeroSelectList.Count)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeSelectHero));
			}
		}

		private static void DoSelectAction(ref CFakePvPHelper.FakeHeroSelect fakeSelect)
		{
			if (!CFakePvPHelper.IsInSelectHero())
			{
				return;
			}
			if (fakeSelect.changeHeroCount < fakeSelect.maxChangeHeroCount)
			{
				uint num;
				if (fakeSelect.FakePlayer.dwAcntPvpLevel <= 6u)
				{
					int id = Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
					ResFakeAcntHero dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
					if (dataByIndex == null)
					{
						return;
					}
					num = dataByIndex.dwHeroID;
					while (CFakePvPHelper.ChosenHeroes.ContainsValue(dataByIndex.dwHeroID))
					{
						id = Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
						dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
						if (dataByIndex == null)
						{
							return;
						}
						num = dataByIndex.dwHeroID;
					}
				}
				else
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					int num2 = GameDataMgr.robotVeteranHeroSkinDatabin.Count();
					int max = num2 + ((masterRoleInfo != null) ? masterRoleInfo.freeHeroList.get_Count() : 0);
					int num3 = Random.Range(0, max);
					if (num3 < num2)
					{
						ResFakeAcntHero dataByIndex2 = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(num3);
						if (dataByIndex2 == null)
						{
							return;
						}
						num = dataByIndex2.dwHeroID;
					}
					else
					{
						num = masterRoleInfo.freeHeroList.get_Item(num3 - num2).dwFreeHeroID;
					}
					while (CFakePvPHelper.ChosenHeroes.ContainsValue(num))
					{
						num3 = Random.Range(0, max);
						if (num3 < num2)
						{
							ResFakeAcntHero dataByIndex3 = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(num3);
							if (dataByIndex3 == null)
							{
								return;
							}
							num = dataByIndex3.dwHeroID;
						}
						else
						{
							num = masterRoleInfo.freeHeroList.get_Item(num3 - num2).dwFreeHeroID;
						}
					}
				}
				CFakePvPHelper.ChosenHeroes.set_Item(fakeSelect.FakePlayer.ullUid, num);
				fakeSelect.selectedHero = num;
				fakeSelect.changeHeroCount++;
				RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
				if (roomInfo == null)
				{
					return;
				}
				MemberInfo memberInfo = roomInfo.GetMemberInfo(fakeSelect.FakePlayer.ullUid);
				if (memberInfo != null && memberInfo.ChoiceHero[0] != null)
				{
					memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = num;
					Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
				}
			}
			else
			{
				if (fakeSelect.FakePlayer.dwAcntPvpLevel <= 6u)
				{
					ResFakeAcntHero dataByKey = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByKey(fakeSelect.selectedHero);
					if (dataByKey != null && dataByKey.dwSkinID != 0u)
					{
						int num4 = Random.Range(0, 10000);
						if ((long)num4 < (long)((ulong)dataByKey.dwSkinRate))
						{
							fakeSelect.selectedSkin = (int)dataByKey.dwSkinID;
						}
					}
				}
				else
				{
					ResFakeAcntHero dataByKey2 = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByKey(fakeSelect.selectedHero);
					if (dataByKey2 != null && dataByKey2.dwSkinID != 0u)
					{
						int num5 = Random.Range(0, 10000);
						if ((long)num5 < (long)((ulong)dataByKey2.dwSkinRate))
						{
							fakeSelect.selectedSkin = (int)dataByKey2.dwSkinID;
						}
					}
				}
				ResFakeAcntSkill dataByKey3 = GameDataMgr.robotPlayerSkillDatabin.GetDataByKey(fakeSelect.FakePlayer.dwAcntPvpLevel);
				if (dataByKey3 != null)
				{
					int num6 = Random.Range(0, dataByKey3.SkillId.Length);
					while (dataByKey3.SkillId[num6] == 0u)
					{
						num6 = Random.Range(0, dataByKey3.SkillId.Length);
					}
					uint selectedPlayerSkill = dataByKey3.SkillId[num6];
					fakeSelect.selectedPlayerSkill = selectedPlayerSkill;
				}
				fakeSelect.bConfirmed = true;
				CFakePvPHelper.HeroConfirmedPlayerNum++;
				RoomInfo roomInfo2 = Singleton<CRoomSystem>.GetInstance().roomInfo;
				MemberInfo memberInfo2 = roomInfo2.GetMemberInfo(fakeSelect.FakePlayer.ullUid);
				if (memberInfo2 != null && memberInfo2.ChoiceHero[0] != null)
				{
					memberInfo2.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = fakeSelect.selectedPlayerSkill;
					memberInfo2.isPrepare = true;
					Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
				}
				if (CFakePvPHelper.HeroConfirmedPlayerNum == CFakePvPHelper.FakeHeroSelectList.Count + 1)
				{
					CFakePvPHelper.ReqStartSingleWarmBattle();
				}
			}
		}

		public static void OnSelfSelectHero(ulong selfUid, uint heroID)
		{
			CFakePvPHelper.ChosenHeroes.set_Item(selfUid, heroID);
		}

		public static void OnSelfHeroConfirmed()
		{
			CFakePvPHelper.HeroConfirmedPlayerNum++;
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			MemberInfo memberInfo = roomInfo.GetMemberInfo(roomInfo.selfInfo.ullUid);
			memberInfo.isPrepare = true;
			if (CFakePvPHelper.HeroConfirmedPlayerNum == CFakePvPHelper.FakeHeroSelectList.Count + 1)
			{
				CFakePvPHelper.ReqStartSingleWarmBattle();
			}
		}

		private static void ReqStartSingleWarmBattle()
		{
			CFakePvPHelper.bInFakeSelect = false;
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnSelectHeroTimeout));
			CFakePvPHelper.RemoveAllFakeTimer();
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1050u);
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			if (roomInfo == null)
			{
				DebugHelper.Assert(roomInfo != null, "RoomInfo Should not be NULL!!!");
				return;
			}
			if (roomInfo != null)
			{
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.bGameType = 1;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfCombat = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfCombat;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = 1;
				if (cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfCombat == null)
				{
					return;
				}
				MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
				if (masterMemberInfo == null)
				{
					DebugHelper.Assert(roomInfo != null, "selfInfo Should not be NULL!!!");
					return;
				}
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList[0] = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
				ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(roomInfo.roomAttrib.bMapType, roomInfo.roomAttrib.dwMapId);
				if (pvpMapCommonInfo != null)
				{
					COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
					int dwHeroID = 0;
					for (COM_PLAYERCAMP cOM_PLAYERCAMP2 = COM_PLAYERCAMP.COM_PLAYERCAMP_1; cOM_PLAYERCAMP2 < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; cOM_PLAYERCAMP2++)
					{
						ListView<MemberInfo> listView = roomInfo[cOM_PLAYERCAMP2];
						for (int i = 0; i < listView.Count; i++)
						{
							if (listView[i].ullUid == roomInfo.selfInfo.ullUid)
							{
								cOM_PLAYERCAMP = listView[i].camp;
								dwHeroID = (int)listView[i].ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
								break;
							}
						}
					}
					CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer = cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer;
					int num = 0;
					for (COM_PLAYERCAMP cOM_PLAYERCAMP3 = COM_PLAYERCAMP.COM_PLAYERCAMP_1; cOM_PLAYERCAMP3 < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; cOM_PLAYERCAMP3++)
					{
						ListView<MemberInfo> listView2 = roomInfo[cOM_PLAYERCAMP3];
						for (int j = 0; j < listView2.Count; j++)
						{
							MemberInfo memberInfo = listView2[j];
							if (memberInfo != null)
							{
								if (memberInfo.RoomMemberType == 2u)
								{
									stBattlePlayer.astFighter[num].bObjType = 2;
									stBattlePlayer.astFighter[num].bPosOfCamp = (byte)j;
									stBattlePlayer.astFighter[num].bObjCamp = (byte)cOM_PLAYERCAMP3;
									stBattlePlayer.astFighter[num].dwLevel = memberInfo.dwMemberPvpLevel;
									CFakePvPHelper.FakeHeroSelect fakeHeroSelect = CFakePvPHelper.GetFakeHeroSelect(memberInfo.ullUid);
									if (fakeHeroSelect != null)
									{
										stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = fakeHeroSelect.selectedHero;
										stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = fakeHeroSelect.selectedPlayerSkill;
										stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID = (ushort)fakeHeroSelect.selectedSkin;
										stBattlePlayer.astFighter[num].szName = fakeHeroSelect.FakePlayer.szUserName;
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc = new COMDT_PLAYERINFO_OF_NPC();
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc.ullFakeUid = fakeHeroSelect.FakePlayer.ullUid;
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc.dwFakePvpLevel = fakeHeroSelect.FakePlayer.dwAcntPvpLevel;
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc.dwFakeLogicWorldID = fakeHeroSelect.FakePlayer.dwLogicWorldId;
									}
									else
									{
										CFakePvPHelper.SelectHeroForEnemyPlayer(ref stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo, stBattlePlayer.astFighter[num].dwLevel);
										stBattlePlayer.astFighter[num].szName = memberInfo.WarmNpc.szUserName;
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc = new COMDT_PLAYERINFO_OF_NPC();
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc.ullFakeUid = memberInfo.WarmNpc.ullUid;
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc.dwFakePvpLevel = memberInfo.WarmNpc.dwAcntPvpLevel;
										stBattlePlayer.astFighter[num].stDetail.stPlayerOfNpc.dwFakeLogicWorldID = memberInfo.WarmNpc.dwLogicWorldId;
									}
								}
								else if (memberInfo.RoomMemberType == 1u)
								{
									stBattlePlayer.astFighter[num].bObjType = 1;
									stBattlePlayer.astFighter[num].bPosOfCamp = (byte)j;
									stBattlePlayer.astFighter[num].bObjCamp = (byte)cOM_PLAYERCAMP;
									for (int k = 0; k < (int)pvpMapCommonInfo.bHeroNum; k++)
									{
										stBattlePlayer.astFighter[num].astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID = (uint)dwHeroID;
									}
								}
								num++;
							}
						}
					}
					stBattlePlayer.bNum = (byte)num;
					roomInfo.roomAttrib.bWarmBattle = false;
					roomInfo.roomAttrib.npcAILevel = 2;
					Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
					Singleton<WatchController>.GetInstance().Stop();
				}
			}
		}

		private static void SelectHeroForEnemyPlayer(ref COMDT_HEROINFO heroInfo, uint playerLevel)
		{
			uint heroID = 0u;
			ushort wSkinID = 0;
			uint dwSelSkillID = 0u;
			if (playerLevel <= 6u)
			{
				int id = Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
				ResFakeAcntHero dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
				if (dataByIndex == null)
				{
					return;
				}
				heroID = dataByIndex.dwHeroID;
				while (CFakePvPHelper.EnemyChosenHeroes.FindIndex((uint x) => x == heroID) != -1)
				{
					id = Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
					dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
					if (dataByIndex == null)
					{
						return;
					}
					heroID = dataByIndex.dwHeroID;
				}
				if (dataByIndex != null && dataByIndex.dwSkinID != 0u)
				{
					int num = Random.Range(0, 10000);
					if ((long)num < (long)((ulong)dataByIndex.dwSkinRate))
					{
						wSkinID = (ushort)dataByIndex.dwSkinID;
					}
				}
			}
			else
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return;
				}
				int num2 = GameDataMgr.robotVeteranHeroSkinDatabin.Count();
				int max = num2 + ((masterRoleInfo != null) ? masterRoleInfo.freeHeroList.get_Count() : 0);
				int num3 = Random.Range(0, max);
				if (num3 < num2)
				{
					ResFakeAcntHero dataByIndex2 = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(num3);
					if (dataByIndex2 == null)
					{
						return;
					}
					heroID = dataByIndex2.dwHeroID;
				}
				else
				{
					heroID = masterRoleInfo.freeHeroList.get_Item(num3 - num2).dwFreeHeroID;
				}
				while (CFakePvPHelper.EnemyChosenHeroes.FindIndex((uint x) => x == heroID) != -1)
				{
					num3 = Random.Range(0, max);
					if (num3 < num2)
					{
						ResFakeAcntHero dataByIndex3 = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(num3);
						if (dataByIndex3 == null)
						{
							return;
						}
						heroID = dataByIndex3.dwHeroID;
					}
					else
					{
						heroID = masterRoleInfo.freeHeroList.get_Item(num3 - num2).dwFreeHeroID;
					}
				}
			}
			ResFakeAcntSkill dataByKey = GameDataMgr.robotPlayerSkillDatabin.GetDataByKey(playerLevel);
			if (dataByKey != null)
			{
				int num4 = Random.Range(0, dataByKey.SkillId.Length);
				while (dataByKey.SkillId[num4] == 0u)
				{
					num4 = Random.Range(0, dataByKey.SkillId.Length);
				}
				dwSelSkillID = dataByKey.SkillId[num4];
			}
			CFakePvPHelper.EnemyChosenHeroes.Add(heroID);
			heroInfo.stCommonInfo.dwHeroID = heroID;
			heroInfo.stCommonInfo.stSkill.dwSelSkillID = dwSelSkillID;
			heroInfo.stCommonInfo.wSkinID = wSkinID;
		}

		private static CFakePvPHelper.FakeHeroSelect GetFakeHeroSelect(ulong playerUid)
		{
			CFakePvPHelper.FakeHeroSelect result = null;
			for (int i = 0; i < CFakePvPHelper.FakeHeroSelectList.Count; i++)
			{
				if (CFakePvPHelper.FakeHeroSelectList[i].FakePlayer.ullUid == playerUid)
				{
					result = CFakePvPHelper.FakeHeroSelectList[i];
					break;
				}
			}
			return result;
		}

		private static void OnSelectHeroTimeout(int seq)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
			if (form != null)
			{
				RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
				if (roomInfo != null && roomInfo.GetMasterMemberInfo() != null)
				{
					ListView<IHeroData> pvPHeroList = CHeroDataFactory.GetPvPHeroList(CMallSortHelper.HeroViewSortType.Name);
					DebugHelper.Assert(pvPHeroList.Count > CFakePvPHelper.ChosenHeroes.get_Count(), "May have not enough Candidate Heroes!!!");
					int index = Random.Range(0, pvPHeroList.Count);
					IHeroData heroData = pvPHeroList[index];
					while (CFakePvPHelper.ChosenHeroes.ContainsValue(heroData.cfgID))
					{
						index = Random.Range(0, pvPHeroList.Count);
						heroData = pvPHeroList[index];
					}
					MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
					masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = heroData.cfgID;
					CFakePvPHelper.ReqStartSingleWarmBattle();
				}
			}
		}

		public static void FakeSendChat(string content)
		{
			try
			{
				RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
				DebugHelper.Assert(roomInfo != null);
				CChatEntity cChatEntity = new CChatEntity();
				cChatEntity.ullUid = roomInfo.selfInfo.ullUid;
				cChatEntity.iLogicWorldID = (uint)roomInfo.selfInfo.iLogicWorldId;
				cChatEntity.type = EChaterType.Self;
				cChatEntity.text = content;
				CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
				Singleton<CChatController>.instance.model.channelMgr.Add_ChatEntity(cChatEntity, EChatChannel.Select_Hero, 0uL, 0u);
				Singleton<EventRouter>.instance.BroadCastEvent("Chat_HeorSelectChatData_Change");
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Fake Send Chat, {0}", new object[]
				{
					ex.get_Message()
				});
			}
		}

		public static void FakeSendChatTemplate(int index)
		{
			CFakePvPHelper.FakeSendChat(Singleton<CChatController>.instance.model.Get_HeroSelect_ChatTemplate(index).GetTemplateString());
		}

		public static void FakeLoadProcess(float progress)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUILoadingSystem.PVP_PATH_LOADING);
			if (form == null)
			{
				return;
			}
			List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.get_Current().Computer)
				{
					int num = enumerator.get_Current().CampPos + 1;
					GameObject gameObject = (enumerator.get_Current().PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? form.gameObject.transform.FindChild("UpPanel").FindChild(string.Format("Up_Player{0}", num)).gameObject : form.gameObject.transform.FindChild("DownPanel").FindChild(string.Format("Down_Player{0}", num)).gameObject;
					if (gameObject != null)
					{
						GameObject gameObject2 = gameObject.transform.Find("Txt_LoadingPct").gameObject;
						if (gameObject2)
						{
							Text component = gameObject2.GetComponent<Text>();
							string text = component.get_text();
							if (!(text == "100%"))
							{
								int max = Random.Range(1, 30);
								int num2 = Math.Min(100, Convert.ToInt32(progress * 100f + (float)Random.Range(0, max)));
								int num3;
								int.TryParse(text.Substring(0, 2), ref num3);
								if (num2 > num3)
								{
									component.set_text(string.Format("{0}%", num2));
								}
							}
						}
					}
				}
			}
		}
	}
}
