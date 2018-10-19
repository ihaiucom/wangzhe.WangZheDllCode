using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CHeroSelectBaseSystem : Singleton<CHeroSelectBaseSystem>
	{
		public bool m_isInHeroSelectState;

		private enSelectGameType m_gameType = enSelectGameType.enNull;

		private enSelectType m_selectType = enSelectType.enNull;

		private enUIType m_uiType = enUIType.enNull;

		public CMallSortHelper.HeroViewSortType m_sortType = CMallSortHelper.HeroViewSortType.Name;

		public uint m_mapId;

		public byte m_mapType;

		private int m_isAllowDupHero;

		private byte m_mapMaxHeroCount;

		public ResDT_UnUseSkill m_mapUnUseSkill;

		public uint m_chatID;

		public ListView<IHeroData> m_expCardHeroList = new ListView<IHeroData>();

		public List<uint> m_notUseHeroList = new List<uint>();

		public ListView<IHeroData> m_canUseHeroList = new ListView<IHeroData>();

		public List<uint> m_selectHeroIDList = new List<uint>();

		public byte m_selectHeroCount;

		public bool m_isSelectConfirm;

		private bool m_isMobaMode;

		private bool m_isMultiMode;

		public CSDT_SINGLE_GAME_OF_ADVENTURE m_stGameOfAdventure;

		public CSDT_SINGLE_GAME_OF_COMBAT m_stGameOfCombat;

		public CSDT_SINGLE_GAME_OF_BURNING m_stGameOfBurnning;

		public CSDT_SINGLE_GAME_OF_ARENA m_stGameOfArena;

		public static COMDT_BATTLELIST_LIST s_defaultBattleListInfo = null;

		public uint m_battleListID;

		public static int[] s_propArr = new int[37];

		public static int[] s_propPctArr = new int[37];

		public static string[] s_propImgArr = new string[37];

		public float m_fOpenHeroSelectForm;

		public int m_UseRandSelCount;

		public enBanPickStep m_banPickStep;

		public int m_banHeroTeamMaxCount;

		private List<uint>[] campBanList;

		public enSwapHeroState m_swapState;

		public SCPKG_NTF_SWAP_HERO m_swapInfo = new SCPKG_NTF_SWAP_HERO();

		public SCPKG_NTF_CUR_BAN_PICK_INFO m_curBanPickInfo = new SCPKG_NTF_CUR_BAN_PICK_INFO();

		public bool m_isAllowDupHeroAllCamp;

		public bool m_isAllowCancelConfirmHero;

		public bool m_isAllowShowTeamTips;

		public bool m_isAllowShowBattleHistory;

		public enSelectGameType gameType
		{
			get
			{
				return this.m_gameType;
			}
		}

		public enSelectType selectType
		{
			get
			{
				return this.m_selectType;
			}
		}

		public enUIType uiType
		{
			get
			{
				return this.m_uiType;
			}
		}

		public RoomInfo roomInfo
		{
			get
			{
				return Singleton<CRoomSystem>.GetInstance().roomInfo;
			}
		}

		private void InitMobaMode()
		{
			if (this.gameType != enSelectGameType.enNull && this.gameType != enSelectGameType.enPVE_Adventure && this.gameType != enSelectGameType.enBurning && this.gameType != enSelectGameType.enArena && this.gameType != enSelectGameType.enGuide && this.gameType != enSelectGameType.enArenaDefTeamConfig)
			{
				this.m_isMobaMode = true;
			}
			else
			{
				this.m_isMobaMode = false;
			}
		}

		private void InitMultiMode()
		{
			if (this.gameType != enSelectGameType.enNull && this.gameType != enSelectGameType.enPVE_Adventure && this.gameType != enSelectGameType.enBurning && this.gameType != enSelectGameType.enArena && this.gameType != enSelectGameType.enGuide && this.gameType != enSelectGameType.enPVE_Computer && this.gameType != enSelectGameType.enArenaDefTeamConfig)
			{
				this.m_isMultiMode = true;
			}
			else
			{
				this.m_isMultiMode = false;
			}
		}

		private void InitSelectType()
		{
			enSelectType selectType = enSelectType.enNormal;
			if (this.gameType == enSelectGameType.enPVE_Adventure || this.gameType == enSelectGameType.enBurning || this.gameType == enSelectGameType.enArena || this.gameType == enSelectGameType.enGuide || this.gameType == enSelectGameType.enArenaDefTeamConfig)
			{
				selectType = enSelectType.enMutile;
			}
			else
			{
				ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.m_mapType, this.m_mapId);
				if (pvpMapCommonInfo != null)
				{
					selectType = (enSelectType)pvpMapCommonInfo.stPickRuleInfo.bPickType;
					this.m_isAllowDupHeroAllCamp = (pvpMapCommonInfo.stPickRuleInfo.Param[1] == 0u);
					this.m_isAllowDupHero = (int)pvpMapCommonInfo.bIsAllowHeroDup;
					this.m_mapUnUseSkill = pvpMapCommonInfo.stUnUseSkillInfo;
					this.m_chatID = pvpMapCommonInfo.dwChatID;
					this.m_isAllowCancelConfirmHero = (pvpMapCommonInfo.bForceTimeoutPick > 0);
					this.m_isAllowShowTeamTips = (pvpMapCommonInfo.bShowTeamHint > 0);
					this.m_isAllowShowBattleHistory = (pvpMapCommonInfo.bShowWinRatio > 0);
				}
			}
			this.m_selectType = selectType;
		}

		private void InitUIType()
		{
			if (this.m_selectType == enSelectType.enMutile || this.m_selectType == enSelectType.enNormal || this.m_selectType == enSelectType.enRandom || this.m_selectType == enSelectType.enClone)
			{
				this.m_uiType = enUIType.enNormal;
			}
			else if (this.m_selectType == enSelectType.enBanPick)
			{
				this.m_uiType = enUIType.enBanPick;
			}
		}

		private void InitSystem()
		{
			this.InitMobaMode();
			this.InitMultiMode();
			this.InitSelectType();
			this.InitUIType();
		}

		private void InitCanUseHeroData()
		{
			Singleton<CHeroSelectNormalSystem>.instance.m_showHeroID = 0u;
			this.m_sortType = GameSettings.HeroSelectHeroViewSortType;
			if (this.gameType == enSelectGameType.enArenaDefTeamConfig)
			{
				this.LoadArenaDefHeroList(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList);
			}
			else if (!this.IsMobaMode())
			{
				this.LoadPveDefaultHeroList();
			}
			if (this.IsMultilMode() || this.IsSingleWarmBattle())
			{
				if (this.selectType == enSelectType.enBanPick)
				{
					this.LoadServerCanUseHeroData();
				}
				else if ((this.gameType == enSelectGameType.enLadder && !Singleton<CLadderSystem>.GetInstance().IsNoHeroLimit()) || this.gameType == enSelectGameType.enGuildMatch)
				{
					this.m_canUseHeroList = CHeroDataFactory.GetHostHeroList(false, this.m_sortType);
				}
				else
				{
					this.m_canUseHeroList = CHeroDataFactory.GetPvPHeroList(this.m_sortType);
					this.m_expCardHeroList = CBagSystem.GetExpCardHeroList();
					this.m_canUseHeroList.AddRange(this.m_expCardHeroList);
				}
			}
			else if (this.IsSpecTraingMode())
			{
				this.m_canUseHeroList = CHeroDataFactory.GetTrainingHeroList(this.m_sortType);
			}
			else
			{
				this.m_canUseHeroList = CHeroDataFactory.GetPvPHeroList(this.m_sortType);
				this.m_expCardHeroList = CBagSystem.GetExpCardHeroList();
				this.m_canUseHeroList.AddRange(this.m_expCardHeroList);
			}
			if (this.IsMobaMode())
			{
				this.m_notUseHeroList.Clear();
				ResBanHeroConf dataByKey = GameDataMgr.banHeroBin.GetDataByKey(GameDataMgr.GetDoubleKey((uint)this.m_mapType, this.m_mapId));
				if (dataByKey != null)
				{
					for (int i = 0; i < dataByKey.BanHero.Length; i++)
					{
						if (dataByKey.BanHero[i] != 0u)
						{
							this.m_notUseHeroList.Add(dataByKey.BanHero[i]);
						}
					}
				}
			}
		}

		public void InitSelectHeroIDList(byte heroMaxCount)
		{
			this.m_selectHeroIDList.Clear();
			for (int i = 0; i < (int)heroMaxCount; i++)
			{
				this.m_selectHeroIDList.Add(0u);
			}
			this.m_selectHeroCount = 0;
			this.m_mapMaxHeroCount = heroMaxCount;
		}

		public override void Init()
		{
			this.campBanList = new List<uint>[3];
			for (int i = 0; i < this.campBanList.Length; i++)
			{
				this.campBanList[i] = new List<uint>();
			}
		}

		public void OpenForm(enSelectGameType gameType, byte mapHeroMaxCount, uint mapID = 0u, byte mapType = 0, int isAllowDupHero = 0)
		{
			this.m_fOpenHeroSelectForm = Time.time;
			Singleton<CRoomSystem>.GetInstance().CloseRoom();
			Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
			Singleton<CMatchingSystem>.GetInstance().CloseMatchingConfirm();
			this.CloseForm();
			this.m_isInHeroSelectState = true;
			this.m_gameType = gameType;
			this.m_selectType = this.selectType;
			this.m_mapId = mapID;
			this.m_mapType = mapType;
			this.m_isAllowDupHero = isAllowDupHero;
			this.ClearBanHero();
			this.InitSystem();
			this.InitSelectHeroIDList(mapHeroMaxCount);
			this.InitCanUseHeroData();
			if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || this.IsSingleWarmBattle())
			{
				Singleton<CUIManager>.instance.CloseAllFormExceptLobby(true);
			}
			if (this.m_uiType == enUIType.enNormal)
			{
				Singleton<CHeroSelectNormalSystem>.instance.OpenForm();
			}
			else if (this.m_uiType == enUIType.enBanPick)
			{
				Singleton<CHeroSelectBanPickSystem>.instance.OpenForm();
			}
			if ((Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || this.IsSingleWarmBattle()) && !Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<CChatController>.instance.model.SetCurGroupTemplateInfo(this.m_chatID);
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_Hero_Select_OpenForm);
			}
			MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
		}

		public void CloseForm()
		{
			Singleton<CHeroSelectNormalSystem>.instance.CloseForm();
			Singleton<CHeroSelectBanPickSystem>.instance.CloseForm();
		}

		public void Clear()
		{
			this.m_isInHeroSelectState = false;
			this.m_gameType = enSelectGameType.enNull;
			this.m_selectType = enSelectType.enNull;
			this.m_uiType = enUIType.enNull;
			this.m_mapId = 0u;
			this.m_mapType = 0;
			this.m_isAllowDupHero = 0;
			this.m_mapMaxHeroCount = 0;
			this.m_mapUnUseSkill = null;
			this.m_chatID = 0u;
			this.m_expCardHeroList.Clear();
			this.m_notUseHeroList.Clear();
			this.m_canUseHeroList.Clear();
			this.m_selectHeroIDList.Clear();
			this.m_selectHeroCount = 0;
			this.m_isSelectConfirm = false;
			this.m_isMobaMode = false;
			this.m_isMultiMode = false;
			this.m_stGameOfAdventure = null;
			this.m_stGameOfCombat = null;
			this.m_stGameOfBurnning = null;
			this.m_stGameOfArena = null;
			this.m_battleListID = 0u;
			CHeroSelectBaseSystem.s_propArr = new int[37];
			CHeroSelectBaseSystem.s_propPctArr = new int[37];
			this.m_UseRandSelCount = 0;
			this.m_banPickStep = enBanPickStep.enBan;
			this.m_banHeroTeamMaxCount = 0;
			this.ClearBanHero();
			this.m_swapState = enSwapHeroState.enIdle;
			this.m_swapInfo = new SCPKG_NTF_SWAP_HERO();
			this.m_curBanPickInfo = new SCPKG_NTF_CUR_BAN_PICK_INFO();
			this.m_isAllowDupHeroAllCamp = false;
			this.m_isAllowCancelConfirmHero = false;
			this.m_isAllowShowTeamTips = false;
			this.m_isAllowShowBattleHistory = false;
			MonoSingleton<VoiceSys>.GetInstance().HeroSelectTobattle();
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_Hero_Select_CloseForm);
			Singleton<CUIManager>.instance.CloseForm(CHeroSkinBuyManager.s_buyHeroSkin3DFormPath);
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
		}

		private void LoadServerCanUseHeroData()
		{
			MemberInfo masterMemberInfo = this.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo != null)
			{
				for (int i = 0; i < masterMemberInfo.canUseHero.Length; i++)
				{
					uint num = masterMemberInfo.canUseHero[i];
					IHeroData heroData = CHeroDataFactory.CreateHeroData(masterMemberInfo.canUseHero[i]);
					if (heroData != null)
					{
						this.m_canUseHeroList.Add(heroData);
					}
				}
			}
			CHeroOverviewSystem.SortHeroList(ref this.m_canUseHeroList, this.m_sortType, false);
		}

		public void SetPvpHeroSelect(uint heroID)
		{
			Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0] = heroID;
			Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount = 1;
		}

		public bool IsMobaMode()
		{
			return this.m_isMobaMode;
		}

		public bool IsMultilMode()
		{
			return this.m_isMultiMode;
		}

		public bool IsSingleWarmBattle()
		{
			return this.gameType == enSelectGameType.enPVE_Computer && this.roomInfo != null && this.roomInfo.roomAttrib.bWarmBattle;
		}

		public bool IsSpecTraingMode()
		{
			return this.gameType == enSelectGameType.enGuide && this.m_stGameOfAdventure != null && (long)this.m_stGameOfAdventure.iLevelID == (long)((ulong)GameDataMgr.globalInfoDatabin.GetDataByKey(120u).dwConfValue);
		}

		public bool IsHeroExist(uint heroID)
		{
			bool result = true;
			if (this.IsMobaMode())
			{
				if (this.m_notUseHeroList.Contains(heroID))
				{
					return result;
				}
				if (this.selectType != enSelectType.enBanPick)
				{
					if (this.m_isAllowDupHero == 0)
					{
						MemberInfo masterMemberInfo = this.roomInfo.GetMasterMemberInfo();
						if (masterMemberInfo == null)
						{
							string text = this.roomInfo.selfObjID.ToString();
						}
						if (masterMemberInfo != null)
						{
							result = this.roomInfo.IsHeroExistWithCamp(masterMemberInfo.camp, heroID);
						}
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = (!this.m_isAllowDupHeroAllCamp && (this.roomInfo.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1, heroID) || this.roomInfo.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2, heroID)));
				}
			}
			else
			{
				result = (this.m_selectHeroIDList == null || ((int)this.m_selectHeroCount >= this.m_selectHeroIDList.Count && this.m_selectHeroIDList.Count != 1) || this.m_selectHeroIDList.Contains(heroID));
			}
			return result;
		}

		public bool IsExpCard(uint heroID)
		{
			bool result = false;
			for (int i = 0; i < this.m_expCardHeroList.Count; i++)
			{
				if (this.m_expCardHeroList[i].cfgID == heroID)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool IsBanByHeroID(uint heroID)
		{
			for (int i = 0; i < this.campBanList.Length; i++)
			{
				if (this.campBanList[i].Contains(heroID))
				{
					return true;
				}
			}
			return false;
		}

		public int GetCanUseHeroIndex(uint heroID)
		{
			int result = 0;
			for (int i = 0; i < this.m_canUseHeroList.Count; i++)
			{
				if (this.m_canUseHeroList[i].cfgID == heroID)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public List<uint> GetPveTeamHeroIDList()
		{
			List<uint> list = this.m_selectHeroIDList;
			if (this.gameType == enSelectGameType.enPVE_Computer)
			{
				list = new List<uint>();
				MemberInfo masterMemberInfo = this.roomInfo.GetMasterMemberInfo();
				list.Add(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
			}
			return list;
		}

		public List<uint> GetTeamHeroList(COM_PLAYERCAMP tarCamp)
		{
			List<uint> list = new List<uint>();
			if (this.IsMobaMode())
			{
				if (this.roomInfo != null)
				{
					ListView<MemberInfo> listView = this.roomInfo[tarCamp];
					for (int i = 0; i < listView.Count; i++)
					{
						list.Add(listView[i].ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
					}
				}
			}
			else
			{
				list = this.m_selectHeroIDList;
			}
			return list;
		}

		public int GetTeamPlayerIndex(ulong playerUID, COM_PLAYERCAMP tarCamp)
		{
			int result = 0;
			if (this.IsMobaMode() && this.roomInfo != null)
			{
				ListView<MemberInfo> listView = this.roomInfo[tarCamp];
				for (int i = 0; i < listView.Count; i++)
				{
					if (listView[i].ullUid == playerUID)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public Transform GetTeamPlayerElement(ulong playerUID, COM_PLAYERCAMP tarCamp)
		{
			Transform result = null;
			if (this.uiType == enUIType.enNormal)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
				if (form == null)
				{
					return null;
				}
				Transform transform = form.transform.Find("PanelRight/ListTeamHeroInfo");
				if (transform == null)
				{
					return null;
				}
				CUIListScript component = transform.gameObject.GetComponent<CUIListScript>();
				if (component == null)
				{
					return null;
				}
				int teamPlayerIndex = this.GetTeamPlayerIndex(playerUID, tarCamp);
				if (teamPlayerIndex >= 0 && teamPlayerIndex < component.GetElementAmount())
				{
					result = component.GetElemenet(teamPlayerIndex).transform;
				}
			}
			else if (this.uiType == enUIType.enBanPick)
			{
				CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
				if (form2 == null)
				{
					return null;
				}
				MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
				Transform transform2;
				if ((masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID && tarCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) || (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && tarCamp == masterMemberInfo.camp))
				{
					transform2 = form2.transform.Find("PanelLeft/TeamHeroInfo");
				}
				else
				{
					transform2 = form2.transform.Find("PanelRight/TeamHeroInfo");
				}
				if (transform2 == null)
				{
					return null;
				}
				CUIListScript component2 = transform2.gameObject.GetComponent<CUIListScript>();
				if (component2 == null)
				{
					return null;
				}
				int teamPlayerIndex2 = this.GetTeamPlayerIndex(playerUID, tarCamp);
				if (teamPlayerIndex2 >= 0 && teamPlayerIndex2 < component2.GetElementAmount())
				{
					result = component2.GetElemenet(teamPlayerIndex2).transform;
				}
			}
			return result;
		}

		public void ResetRandHeroLeftCount(int inLeftCount)
		{
			this.m_UseRandSelCount = inLeftCount;
		}

		public void OnHeroSkinWearSuc(uint heroId, uint skinId)
		{
			if (this.uiType == enUIType.enNormal)
			{
				Singleton<CHeroSelectNormalSystem>.instance.OnHeroSkinWearSuc(heroId, skinId);
			}
			else if (this.uiType == enUIType.enBanPick)
			{
				Singleton<CHeroSelectBanPickSystem>.instance.OnHeroSkinWearSuc(heroId, skinId);
			}
		}

		private void ClearBanHero()
		{
			for (int i = 0; i < this.campBanList.Length; i++)
			{
				this.campBanList[i].Clear();
			}
		}

		public void AddBanHero(COM_PLAYERCAMP camp, uint heroId)
		{
			if (camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_MID && camp < (COM_PLAYERCAMP)this.campBanList.Length)
			{
				this.campBanList[(int)camp].Add(heroId);
			}
		}

		public void AddBanHero(COM_PLAYERCAMP camp, uint[] heroIdArr)
		{
			if (camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_MID && camp < (COM_PLAYERCAMP)this.campBanList.Length)
			{
				this.campBanList[(int)camp].AddRange(heroIdArr);
			}
		}

		public List<uint> GetBanHeroList(COM_PLAYERCAMP camp)
		{
			if (camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_MID && camp < (COM_PLAYERCAMP)this.campBanList.Length)
			{
				return this.campBanList[(int)camp];
			}
			return null;
		}

		public bool IsCurBanOrPickMember(MemberInfo mInfo)
		{
			return this.IsOpMemberWithServerInfo(mInfo, this.m_curBanPickInfo.stCurState);
		}

		public bool IsNextBanOrPickMember(MemberInfo mInfo)
		{
			return this.IsOpMemberWithServerInfo(mInfo, this.m_curBanPickInfo.stNextState);
		}

		public bool IsPickedMember(MemberInfo mInfo)
		{
			return mInfo.isPrepare;
		}

		public bool IsOpMemberWithServerInfo(MemberInfo mInfo, CSDT_BAN_PICK_STATE_INFO stateInfo)
		{
			bool result = false;
			for (int i = 0; i < (int)stateInfo.bPosNum; i++)
			{
				if ((byte)mInfo.camp == stateInfo.bCamp && (byte)mInfo.dwPosOfCamp == stateInfo.szPosList[i])
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool IsCurOpByCamp(MemberInfo info)
		{
			return this.m_curBanPickInfo.stCurState.bCamp == (byte)info.camp;
		}

		public void SetPVEDataWithAdventure(uint battleListID, CSDT_SINGLE_GAME_OF_ADVENTURE reportInfo, string levleName = "")
		{
			this.m_battleListID = battleListID;
			this.m_stGameOfAdventure = reportInfo;
			this.m_gameType = enSelectGameType.enPVE_Adventure;
		}

		public void SetPVEDataWithCombat(uint battleListID, CSDT_SINGLE_GAME_OF_COMBAT reportInfo, string levleName = "")
		{
			this.m_battleListID = battleListID;
			this.m_stGameOfCombat = reportInfo;
		}

		public void SetPVEDataWithBurnExpedition(uint battleListID, CSDT_SINGLE_GAME_OF_BURNING reportInfo, string levleName = "")
		{
			this.m_battleListID = battleListID;
			this.m_stGameOfBurnning = reportInfo;
		}

		public void SetPveDataWithArena(uint battleListID, CSDT_SINGLE_GAME_OF_ARENA reportInfo = null, string levleName = "")
		{
			this.m_battleListID = battleListID;
			this.m_stGameOfArena = reportInfo;
		}

		public void LoadPveDefaultHeroList()
		{
			if (CHeroSelectBaseSystem.s_defaultBattleListInfo != null)
			{
				int num = 0;
				while ((long)num < (long)((ulong)CHeroSelectBaseSystem.s_defaultBattleListInfo.dwListNum))
				{
					if (CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num].dwBattleListID == this.m_battleListID)
					{
						COMDT_BATTLEHERO stBattleList = CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num].stBattleList;
						for (int i = 0; i < stBattleList.BattleHeroList.Length; i++)
						{
							if (i < this.m_selectHeroIDList.Count)
							{
								uint num2 = stBattleList.BattleHeroList[i];
								if (this.gameType == enSelectGameType.enBurning && Singleton<BurnExpeditionController>.instance.model.IsHeroInRecord(num2))
								{
									int num3 = Singleton<BurnExpeditionController>.instance.model.Get_HeroHP(num2);
									if (num3 <= 0)
									{
										num2 = 0u;
									}
								}
								if (!CHeroDataFactory.IsHeroCanUse(num2))
								{
									num2 = 0u;
								}
								if (num2 != 0u)
								{
									this.m_selectHeroIDList[(int)this.m_selectHeroCount] = num2;
									this.m_selectHeroCount += 1;
									Singleton<CHeroSelectNormalSystem>.instance.m_showHeroID = num2;
								}
							}
						}
					}
					num++;
				}
			}
			else
			{
				CHeroSelectBaseSystem.s_defaultBattleListInfo = new COMDT_BATTLELIST_LIST();
				CHeroSelectBaseSystem.s_defaultBattleListInfo.dwListNum = 0u;
			}
		}

		public List<uint> GetHeroListForBattleListID(uint battleListID)
		{
			List<uint> list = new List<uint>();
			if (CHeroSelectBaseSystem.s_defaultBattleListInfo != null)
			{
				int num = 0;
				while ((long)num < (long)((ulong)CHeroSelectBaseSystem.s_defaultBattleListInfo.dwListNum))
				{
					if (CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num].dwBattleListID == battleListID)
					{
						COMDT_BATTLEHERO stBattleList = CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num].stBattleList;
						for (int i = 0; i < stBattleList.BattleHeroList.Length; i++)
						{
							uint num2 = stBattleList.BattleHeroList[i];
							if (!CHeroDataFactory.IsHeroCanUse(num2))
							{
								num2 = 0u;
							}
							if (num2 != 0u)
							{
								list.Add(num2);
							}
						}
					}
					num++;
				}
			}
			return list;
		}

		public void SavePveDefaultHeroList()
		{
			if (CHeroSelectBaseSystem.s_defaultBattleListInfo != null)
			{
				bool flag = false;
				int num = 0;
				while ((long)num < (long)((ulong)CHeroSelectBaseSystem.s_defaultBattleListInfo.dwListNum))
				{
					if (CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num].dwBattleListID == this.m_battleListID)
					{
						COMDT_BATTLEHERO stBattleList = CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num].stBattleList;
						stBattleList.wHeroCnt = (ushort)this.m_selectHeroIDList.Count;
						stBattleList.BattleHeroList = this.m_selectHeroIDList.ToArray();
						flag = true;
					}
					num++;
				}
				if (!flag)
				{
					ListLinqView<COMDT_BATTLELIST> listLinqView = new ListLinqView<COMDT_BATTLELIST>();
					int num2 = 0;
					while ((long)num2 < (long)((ulong)CHeroSelectBaseSystem.s_defaultBattleListInfo.dwListNum))
					{
						listLinqView.Add(CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList[num2]);
						num2++;
					}
					listLinqView.Add(new COMDT_BATTLELIST
					{
						dwBattleListID = this.m_battleListID,
						stBattleList = new COMDT_BATTLEHERO()
						{
							wHeroCnt = (ushort)this.m_selectHeroIDList.Count,
							BattleHeroList = this.m_selectHeroIDList.ToArray()
						}
					});
					CHeroSelectBaseSystem.s_defaultBattleListInfo.dwListNum = (uint)listLinqView.Count;
					CHeroSelectBaseSystem.s_defaultBattleListInfo.astBattleList = listLinqView.ToArray();
				}
			}
		}

		public void LoadArenaDefHeroList(List<uint> defaultHeroList)
		{
			for (int i = 0; i < defaultHeroList.Count; i++)
			{
				uint num = defaultHeroList[i];
				if (!CHeroDataFactory.IsHeroCanUse(num))
				{
					num = 0u;
				}
				if (num != 0u)
				{
					this.m_selectHeroIDList[(int)this.m_selectHeroCount] = num;
					this.m_selectHeroCount += 1;
					Singleton<CHeroSelectNormalSystem>.instance.m_showHeroID = num;
				}
			}
		}

		public static void SendQuitSingleGameReq()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1058u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendSinglePrepareToBattleMsg(RoomInfo roomInfo, uint battleListID, enSelectGameType selectGameType, byte selectHeroCount, List<uint> selectHeroIDList)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1805u);
			cSPkg.stPkgData.stBattleListReq.stBattleList.dwBattleListID = battleListID;
			if (selectGameType == enSelectGameType.enPVE_Computer)
			{
				cSPkg.stPkgData.stBattleListReq.stBattleList.stBattleList.wHeroCnt = 1;
				MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
				if (masterMemberInfo != null)
				{
					cSPkg.stPkgData.stBattleListReq.stBattleList.stBattleList.BattleHeroList[0] = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
				}
			}
			else
			{
				cSPkg.stPkgData.stBattleListReq.stBattleList.stBattleList.wHeroCnt = (ushort)selectHeroCount;
				for (int i = 0; i < (int)cSPkg.stPkgData.stBattleListReq.stBattleList.stBattleList.wHeroCnt; i++)
				{
					cSPkg.stPkgData.stBattleListReq.stBattleList.stBattleList.BattleHeroList[i] = selectHeroIDList[i];
				}
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendSingleGameStartMsgSkipHeroSelect(int iLevelID, int iDifficult)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)iLevelID);
			DebugHelper.Assert(dataByKey != null);
			CSDT_SINGLE_GAME_OF_ADVENTURE cSDT_SINGLE_GAME_OF_ADVENTURE = new CSDT_SINGLE_GAME_OF_ADVENTURE();
			cSDT_SINGLE_GAME_OF_ADVENTURE.iLevelID = iLevelID;
			cSDT_SINGLE_GAME_OF_ADVENTURE.bChapterNo = (byte)dataByKey.iChapterId;
			cSDT_SINGLE_GAME_OF_ADVENTURE.bLevelNo = dataByKey.bLevelNo;
			cSDT_SINGLE_GAME_OF_ADVENTURE.bDifficultType = (byte)iDifficult;
			byte heroMaxCount = (byte)dataByKey.iHeroNum;
			uint dwBattleListID = dataByKey.dwBattleListID;
			Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithAdventure(dwBattleListID, cSDT_SINGLE_GAME_OF_ADVENTURE, string.Empty);
			Singleton<CHeroSelectBaseSystem>.instance.InitSelectHeroIDList(heroMaxCount);
			Singleton<CHeroSelectBaseSystem>.instance.LoadPveDefaultHeroList();
			CHeroSelectBaseSystem.SendSinglePrepareToBattleMsg(Singleton<CHeroSelectBaseSystem>.instance.roomInfo, Singleton<CHeroSelectBaseSystem>.instance.m_battleListID, Singleton<CHeroSelectBaseSystem>.instance.gameType, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList);
		}

		public static void SendSingleGameStartMsg(RoomInfo roomInfo, uint battleListID, enSelectGameType selectGameType, byte selectHeroCount, List<uint> selectHeroIDList)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1050u);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.bGameType = (byte)selectGameType;
			cSPkg.stPkgData.stStartSingleGameReq.stBattleList.dwBattleListID = battleListID;
			if (selectGameType == enSelectGameType.enPVE_Adventure)
			{
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfAdventure = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfAdventure;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = (ushort)selectHeroCount;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
				masterRoleInfo.battleHeroList = selectHeroIDList;
				Singleton<CHeroSelectBaseSystem>.instance.SavePveDefaultHeroList();
				CHeroSelectBaseSystem.PostAdventureSingleGame(cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer);
			}
			else if (selectGameType == enSelectGameType.enPVE_Computer)
			{
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfCombat = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfCombat;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = 1;
				MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList[0] = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
				CHeroSelectBaseSystem.PostCombatSingleGame(cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer);
			}
			else if (selectGameType == enSelectGameType.enBurning)
			{
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfBurning = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfBurnning;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = (ushort)selectHeroCount;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
				Singleton<CHeroSelectBaseSystem>.instance.SavePveDefaultHeroList();
				CHeroSelectBaseSystem.PostBurningSingleGame(cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer);
			}
			else if (selectGameType == enSelectGameType.enArena)
			{
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfArena = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfArena;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = (ushort)selectHeroCount;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
				Singleton<CHeroSelectBaseSystem>.instance.SavePveDefaultHeroList();
				CHeroSelectBaseSystem.PostArenaSingleGame(cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer);
			}
			else if (selectGameType == enSelectGameType.enGuide)
			{
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = (ushort)selectHeroCount;
				cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.construct(2L);
				cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfGuide.iLevelID = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfAdventure.iLevelID;
				masterRoleInfo.battleHeroList = selectHeroIDList;
				CHeroSelectBaseSystem.PostAdventureSingleGame(cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer);
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			Singleton<WatchController>.GetInstance().Stop();
		}

		public static void PostAdventureSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)Singleton<CAdventureSys>.instance.currentLevelId);
			if (dataByKey != null && (dataByKey.iLevelType == 0 || (dataByKey.iLevelType == 4 && Singleton<CBattleGuideManager>.instance.bTrainingAdv)))
			{
				uint dwAIPlayerLevel = dataByKey.dwAIPlayerLevel;
				uint[] aIHeroID = dataByKey.AIHeroID;
				stBattlePlayer.astFighter[0].bObjType = 1;
				stBattlePlayer.astFighter[0].bPosOfCamp = 0;
				stBattlePlayer.astFighter[0].bObjCamp = 1;
				for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count; i++)
				{
					stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i];
				}
				int num = 1;
				for (int j = 0; j < dataByKey.SelfCampAIHeroID.Length; j++)
				{
					if (dataByKey.SelfCampAIHeroID[j] != 0u)
					{
						stBattlePlayer.astFighter[num].bPosOfCamp = (byte)(j + 1);
						stBattlePlayer.astFighter[num].bObjType = 2;
						stBattlePlayer.astFighter[num].bObjCamp = 1;
						stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.SelfCampAIHeroID[j];
						num++;
					}
				}
				for (int k = 0; k < dataByKey.AIHeroID.Length; k++)
				{
					if (dataByKey.AIHeroID[k] != 0u)
					{
						stBattlePlayer.astFighter[num].bPosOfCamp = (byte)k;
						stBattlePlayer.astFighter[num].bObjType = 2;
						stBattlePlayer.astFighter[num].bObjCamp = 2;
						stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.AIHeroID[k];
						num++;
					}
				}
				stBattlePlayer.bNum = (byte)num;
			}
		}

		public static void PostCombatSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
		{
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
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
			List<uint>[] array = new List<uint>[2];
			if (Singleton<CHeroSelectBaseSystem>.instance.m_isAllowDupHero == 0)
			{
				array[0] = new List<uint>();
				array[1] = new List<uint>();
				array[(cOM_PLAYERCAMP != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? 1 : 0] = Singleton<CHeroSelectBaseSystem>.instance.GetPveTeamHeroIDList();
			}
			int num = 0;
			int canPickHeroNum = 0;
			GameDataMgr.heroDatabin.Accept(delegate(ResHeroCfgInfo heroCfg)
			{
				if (heroCfg != null && GameDataMgr.IsHeroCanBePickByComputer(heroCfg.dwCfgID))
				{
					canPickHeroNum++;
				}
			});
			DebugHelper.Assert(canPickHeroNum >= 3, "Not Enough Hero To Pick!!!");
			for (COM_PLAYERCAMP cOM_PLAYERCAMP3 = COM_PLAYERCAMP.COM_PLAYERCAMP_1; cOM_PLAYERCAMP3 < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; cOM_PLAYERCAMP3++)
			{
				ListView<MemberInfo> listView2 = roomInfo[cOM_PLAYERCAMP3];
				for (int j = 0; j < listView2.Count; j++)
				{
					MemberInfo memberInfo = listView2[j];
					if (memberInfo.RoomMemberType == 2u)
					{
						stBattlePlayer.astFighter[num].bObjType = 2;
						stBattlePlayer.astFighter[num].bPosOfCamp = (byte)j;
						stBattlePlayer.astFighter[num].bObjCamp = (byte)cOM_PLAYERCAMP3;
						stBattlePlayer.astFighter[num].dwLevel = 1u;
						for (int k = 0; k < (int)Singleton<CHeroSelectBaseSystem>.instance.m_mapMaxHeroCount; k++)
						{
							int id = UnityEngine.Random.Range(0, GameDataMgr.heroDatabin.Count());
							ResHeroCfgInfo heroCfg = GameDataMgr.heroDatabin.GetDataByIndex(id);
							bool flag = GameDataMgr.IsHeroCanBePickByComputer(heroCfg.dwCfgID);
							while (!flag)
							{
								id = UnityEngine.Random.Range(0, GameDataMgr.heroDatabin.Count());
								heroCfg = GameDataMgr.heroDatabin.GetDataByIndex(id);
								flag = GameDataMgr.IsHeroCanBePickByComputer(heroCfg.dwCfgID);
							}
							if (Singleton<CHeroSelectBaseSystem>.instance.m_isAllowDupHero == 0)
							{
								while (array[cOM_PLAYERCAMP3 - COM_PLAYERCAMP.COM_PLAYERCAMP_1].FindIndex((uint x) => x == heroCfg.dwCfgID) != -1 || !flag || (CSysDynamicBlock.bLobbyEntryBlocked && heroCfg.bIOSHide == 1))
								{
									id = UnityEngine.Random.Range(0, GameDataMgr.heroDatabin.Count());
									heroCfg = GameDataMgr.heroDatabin.GetDataByIndex(id);
									flag = GameDataMgr.IsHeroCanBePickByComputer(heroCfg.dwCfgID);
								}
								array[cOM_PLAYERCAMP3 - COM_PLAYERCAMP.COM_PLAYERCAMP_1].Add(heroCfg.dwCfgID);
							}
							stBattlePlayer.astFighter[num].astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID = heroCfg.dwCfgID;
						}
					}
					else if (memberInfo.RoomMemberType == 1u)
					{
						stBattlePlayer.astFighter[num].bObjType = 1;
						stBattlePlayer.astFighter[num].bPosOfCamp = (byte)j;
						stBattlePlayer.astFighter[num].bObjCamp = (byte)cOM_PLAYERCAMP;
						for (int l = 0; l < (int)Singleton<CHeroSelectBaseSystem>.instance.m_mapMaxHeroCount; l++)
						{
							stBattlePlayer.astFighter[num].astChoiceHero[l].stBaseInfo.stCommonInfo.dwHeroID = (uint)dwHeroID;
						}
					}
					num++;
				}
			}
			stBattlePlayer.bNum = (byte)num;
		}

		public static void PostBurningSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
		{
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.instance.model;
			List<uint> enemy_HeroIDS = model.Get_Enemy_HeroIDS();
			COMDT_PLAYERINFO current_Enemy_PlayerInfo = model.Get_Current_Enemy_PlayerInfo();
			stBattlePlayer.bNum = 2;
			stBattlePlayer.astFighter[0].bObjType = 1;
			stBattlePlayer.astFighter[0].bPosOfCamp = 0;
			stBattlePlayer.astFighter[0].bObjCamp = 1;
			for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count; i++)
			{
				stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i];
			}
			stBattlePlayer.astFighter[1].bObjType = 2;
			stBattlePlayer.astFighter[1].bPosOfCamp = 0;
			stBattlePlayer.astFighter[1].dwLevel = current_Enemy_PlayerInfo.dwLevel;
			stBattlePlayer.astFighter[1].bObjCamp = 2;
			current_Enemy_PlayerInfo.szName.CopyTo(stBattlePlayer.astFighter[1].szName, 0);
			for (int j = 0; j < enemy_HeroIDS.Count; j++)
			{
				for (int k = 0; k < current_Enemy_PlayerInfo.astChoiceHero.Length; k++)
				{
					if (current_Enemy_PlayerInfo.astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID == enemy_HeroIDS[j])
					{
						if (current_Enemy_PlayerInfo.astChoiceHero[k].stBurningInfo.bIsDead == 0)
						{
							stBattlePlayer.astFighter[1].astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID = enemy_HeroIDS[j];
						}
						break;
					}
				}
			}
		}

		public static void PostArenaSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
		{
			stBattlePlayer.bNum = 2;
			stBattlePlayer.astFighter[0].bObjType = 1;
			stBattlePlayer.astFighter[0].bPosOfCamp = 0;
			stBattlePlayer.astFighter[0].bObjCamp = 1;
			for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count; i++)
			{
				stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i];
			}
			COMDT_ARENA_MEMBER_OF_ACNT tarInfo = Singleton<CArenaSystem>.GetInstance().m_tarInfo;
			stBattlePlayer.astFighter[1].bObjType = 2;
			stBattlePlayer.astFighter[1].bPosOfCamp = 0;
			stBattlePlayer.astFighter[1].dwLevel = tarInfo.dwPVPLevel;
			stBattlePlayer.astFighter[1].bObjCamp = 2;
			for (int j = 0; j < tarInfo.stBattleHero.astHero.Length; j++)
			{
				stBattlePlayer.astFighter[1].astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID = tarInfo.stBattleHero.astHero[j].dwHeroId;
			}
		}

		public static void ImpCalc9SlotHeroStandingPosition(ref Calc9SlotHeroData[] heroes)
		{
			List<int> list = CHeroSelectBaseSystem.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_FRONT);
			switch (list.Count)
			{
			case 1:
			{
				for (int i = 0; i < 3; i++)
				{
					if (heroes[i].RecommendPos == 0)
					{
						heroes[i].selected = true;
						heroes[i].BornIndex = 1;
						break;
					}
				}
				int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				if (heroes[num].RecommendPos == 1)
				{
					heroes[num].BornIndex = 3;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = ((heroes[num].RecommendPos != 1) ? 8 : 5);
				}
				else
				{
					heroes[num].BornIndex = 8;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = ((heroes[num].RecommendPos != 1) ? 6 : 3);
				}
				break;
			}
			case 2:
			{
				for (int j = 0; j < 3; j++)
				{
					if (heroes[j].RecommendPos == 1)
					{
						heroes[j].selected = true;
						heroes[j].BornIndex = 3;
						break;
					}
					if (heroes[j].RecommendPos == 2)
					{
						heroes[j].selected = true;
						heroes[j].BornIndex = 6;
						break;
					}
				}
				int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				heroes[num].BornIndex = 1;
				num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				heroes[num].BornIndex = 0;
				break;
			}
			case 3:
			{
				int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				heroes[num].BornIndex = 1;
				num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				heroes[num].BornIndex = 0;
				num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				heroes[num].BornIndex = 2;
				break;
			}
			default:
				list = CHeroSelectBaseSystem.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_CENTER);
				switch (list.Count)
				{
				case 1:
				{
					for (int k = 0; k < 3; k++)
					{
						if (heroes[k].RecommendPos == 1)
						{
							heroes[k].selected = true;
							heroes[k].BornIndex = 1;
							break;
						}
					}
					int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 8;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 6;
					break;
				}
				case 2:
				{
					for (int l = 0; l < 3; l++)
					{
						if (heroes[l].RecommendPos == 2)
						{
							heroes[l].selected = true;
							heroes[l].BornIndex = 3;
							break;
						}
					}
					int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 1;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 0;
					break;
				}
				case 3:
				{
					int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 1;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 0;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 2;
					break;
				}
				default:
				{
					int num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 4;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 3;
					num = CHeroSelectBaseSystem.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = 5;
					break;
				}
				}
				break;
			}
		}

		public static List<int> HasPositionHero(ref Calc9SlotHeroData[] heroes, RES_HERO_RECOMMEND_POSITION pos)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				if (heroes[i].RecommendPos == (int)pos)
				{
					list.Add(i);
				}
			}
			return list;
		}

		public static int WhoIsBestHero(ref Calc9SlotHeroData[] heroes)
		{
			if (CHeroSelectBaseSystem.IsBetterHero(ref heroes[0], ref heroes[1]) && CHeroSelectBaseSystem.IsBetterHero(ref heroes[0], ref heroes[2]))
			{
				return 0;
			}
			if (CHeroSelectBaseSystem.IsBetterHero(ref heroes[1], ref heroes[0]) && CHeroSelectBaseSystem.IsBetterHero(ref heroes[1], ref heroes[2]))
			{
				return 1;
			}
			return 2;
		}

		public static bool IsBetterHero(ref Calc9SlotHeroData heroe1, ref Calc9SlotHeroData heroe2)
		{
			return heroe1.ConfigId > 0u && !heroe1.selected && (heroe2.ConfigId == 0u || heroe2.selected || heroe1.Ability > heroe2.Ability || (heroe1.Ability == heroe2.Ability && heroe1.Level > heroe2.Level) || (heroe1.Ability == heroe2.Ability && heroe1.Level == heroe2.Level && heroe1.Quality >= heroe2.Quality));
		}

		[MessageHandler(2012)]
		public static void ReciveSingleChooseHeroBegin(CSPkg msg)
		{
			CSDT_SINGLE_GAME_OF_COMBAT cSDT_SINGLE_GAME_OF_COMBAT = new CSDT_SINGLE_GAME_OF_COMBAT();
			if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo != null)
			{
				cSDT_SINGLE_GAME_OF_COMBAT.bRoomType = (byte)Singleton<CRoomSystem>.instance.RoomType;
				cSDT_SINGLE_GAME_OF_COMBAT.dwMapId = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.dwMapId;
				cSDT_SINGLE_GAME_OF_COMBAT.bMapType = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.bMapType;
				cSDT_SINGLE_GAME_OF_COMBAT.bAILevel = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.npcAILevel;
			}
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(cSDT_SINGLE_GAME_OF_COMBAT.bMapType, cSDT_SINGLE_GAME_OF_COMBAT.dwMapId);
			Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithCombat(pvpMapCommonInfo.dwHeroFormId, cSDT_SINGLE_GAME_OF_COMBAT, "Room Type");
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			DebugHelper.Assert(roomInfo != null);
			if (roomInfo != null && roomInfo.roomAttrib.bWarmBattle)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Matching_OpenConfirmBox;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				CFakePvPHelper.SetConfirmFakeData();
				CFakePvPHelper.StartFakeConfirm();
			}
			else
			{
				Singleton<LobbyLogic>.GetInstance().inMultiRoom = false;
				Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enPVE_Computer, 1, cSDT_SINGLE_GAME_OF_COMBAT.dwMapId, cSDT_SINGLE_GAME_OF_COMBAT.bMapType, 0);
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		[MessageHandler(1807)]
		public static void ReciveBATTLELIST_NTY(CSPkg msg)
		{
		}

		[MessageHandler(1806)]
		public static void ReciveBATTLELIST_RSP(CSPkg msg)
		{
			CHeroSelectBaseSystem.SendSingleGameStartMsg(Singleton<CHeroSelectBaseSystem>.instance.roomInfo, Singleton<CHeroSelectBaseSystem>.instance.m_battleListID, Singleton<CHeroSelectBaseSystem>.instance.gameType, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList);
		}

		[MessageHandler(1059)]
		public static void ReciveQuitSingleGame(CSPkg msg)
		{
			if (msg.stPkgData.stQuitSingleGameRsp.bErrCode == 0)
			{
				Singleton<CHeroSelectNormalSystem>.instance.CloseForm();
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1059, (int)msg.stPkgData.stQuitSingleGameRsp.bErrCode), false, 1.5f, null, new object[0]);
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.ReplayKit_Pause_Recording;
			cUIEvent.m_eventParams.tag2 = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		public static void SendHeroSelectMsg(byte operType, byte operPos, uint heroID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1240u);
			cSPkg.stPkgData.stOperHeroReq.bOperType = operType;
			cSPkg.stPkgData.stOperHeroReq.stOperDetail = new CSDT_OPER_HERO();
			if (operType == 0)
			{
				cSPkg.stPkgData.stOperHeroReq.stOperDetail.stSetHero = new CSDT_SETHERO();
				cSPkg.stPkgData.stOperHeroReq.stOperDetail.stSetHero.bHeroPos = operPos;
				cSPkg.stPkgData.stOperHeroReq.stOperDetail.stSetHero.dwHeroId = heroID;
			}
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendMuliPrepareToBattleMsg()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1242u);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendCancelConfirmHero()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1245u);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendShowBattleHistory()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1247u);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendHeroSelectSymbolPage(uint heroId, int selIndex, bool bSendGame = false)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1137u);
			cSPkg.stPkgData.stSymbolPageChgReq = new CSPKG_CMD_SYMBOLPAGESEL();
			cSPkg.stPkgData.stSymbolPageChgReq.dwHeroID = heroId;
			cSPkg.stPkgData.stSymbolPageChgReq.bPageIdx = (byte)selIndex;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendBanHeroMsg(uint heroID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5212u);
			cSPkg.stPkgData.stBanHeroReq.dwHeroID = heroID;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendSwapHeroMsg(uint passiveObjID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5214u);
			cSPkg.stPkgData.stSwapHeroReq.dwPassiveObjID = passiveObjID;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendSwapAcceptHeroMsg(byte bAccept)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5216u);
			cSPkg.stPkgData.stConfirmSwapHeroReq.bAccept = bAccept;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void SendCanelSwapHeroMsg()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5226u);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		public static void StartPvpHeroSelectSystem(COMDT_DESKINFO deskInfo, CSDT_CAMPINFO[] campInfo, COMDT_FREEHERO freeHero, COMDT_FREEHERO_INACNT freeHeroSymbol)
		{
			CHeroSelectBaseSystem.InitRoomData(deskInfo, campInfo, freeHero, freeHeroSymbol);
			enSelectGameType gameType = enSelectGameType.enNull;
			if (deskInfo.bMapType == 1)
			{
				gameType = enSelectGameType.enPVP;
			}
			else if (deskInfo.bMapType == 3)
			{
				gameType = enSelectGameType.enLadder;
			}
			else if (deskInfo.bMapType == 4)
			{
				gameType = enSelectGameType.enLuanDou;
			}
			else if (deskInfo.bMapType == 5)
			{
				gameType = enSelectGameType.enUnion;
			}
			else if (deskInfo.bMapType == 6)
			{
				gameType = enSelectGameType.enGuildMatch;
			}
			uint dwMapId = deskInfo.dwMapId;
			byte bMapType = deskInfo.bMapType;
			Singleton<CHeroSelectBaseSystem>.instance.OpenForm(gameType, 1, dwMapId, bMapType, 0);
		}

		public static void InitRoomData(COMDT_DESKINFO deskInfo, CSDT_CAMPINFO[] campInfo, COMDT_FREEHERO freeHero, COMDT_FREEHERO_INACNT freeHeroSymbol)
		{
			if (deskInfo.bMapType != 3 && deskInfo.bMapType != 6)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				masterRoleInfo.SetFreeHeroInfo(freeHero);
				masterRoleInfo.SetFreeHeroSymbol(freeHeroSymbol);
			}
			Singleton<CRoomSystem>.GetInstance().UpdateRoomInfo(deskInfo, campInfo);
			Singleton<LobbyLogic>.GetInstance().inMultiRoom = true;
		}

		[MessageHandler(1069)]
		public static void ReciveMultiBanBegin(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
			{
				CHeroSelectBaseSystem.StartPvpHeroSelectSystem(msg.stPkgData.stMultGameBeginBan.stDeskInfo, msg.stPkgData.stMultGameBeginBan.astCampInfo, msg.stPkgData.stMultGameBeginBan.stFreeHero, msg.stPkgData.stMultGameBeginBan.stFreeHeroSymbol);
			}
			Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = (int)msg.stPkgData.stMultGameBeginBan.bBanPosNum;
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshTop();
			Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
			Singleton<CSoundManager>.GetInstance().PostEvent("Play_Music_BanPick", null);
		}

		[MessageHandler(1070)]
		public static void ReciveMultiChooseHeroBegin(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
			{
				CHeroSelectBaseSystem.StartPvpHeroSelectSystem(msg.stPkgData.stMultGameBeginPick.stDeskInfo, msg.stPkgData.stMultGameBeginPick.astCampInfo, msg.stPkgData.stMultGameBeginPick.stFreeHero, msg.stPkgData.stMultGameBeginPick.stFreeHeroSymbol);
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enBanPick)
			{
				Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enPick;
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
				Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(true);
				Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment1", null);
			}
		}

		[MessageHandler(1071)]
		public static void ReciveMultiAjustHeroBegin(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enSwap;
			int dwTimeout = (int)msg.stPkgData.stMultGameBeginAdjust.dwTimeout;
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enClone)
				{
					Singleton<CHeroSelectNormalSystem>.instance.StartEndTimer(dwTimeout);
					Singleton<CUIManager>.instance.OpenTips("Clone_Swap_Tips", true, 1.5f, null, new object[0]);
				}
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
			{
				Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
				Singleton<CHeroSelectBanPickSystem>.instance.StartEndTimer(dwTimeout);
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_BanPickEnd", null);
			}
		}

		[MessageHandler(1241)]
		public static void ReciveHeroSelect(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stOperHeroRsp.bErrCode != 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("HeroIsSelectByOther", true, 1.5f, null, new object[0]);
				return;
			}
			if (!Singleton<GameDataValidator>.instance.ValidateGameData())
			{
				return;
			}
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			MemberInfo memberInfo = roomInfo.GetMemberInfo(msg.stPkgData.stOperHeroRsp.stChoiceHero.dwObjId);
			bool flag = false;
			if (memberInfo == null)
			{
				return;
			}
			memberInfo.ChoiceHero = msg.stPkgData.stOperHeroRsp.stChoiceHero.astChoiceHero;
			memberInfo.selectHeroInfo = memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stStatisticDetail;
			if (memberInfo.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.roomInfo.selfObjID)
			{
				Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
				flag = true;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				if (flag)
				{
					Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
					if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
					{
						Singleton<CHeroSelectNormalSystem>.instance.RefreshSkinPanel(null);
					}
				}
				Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(flag, flag);
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
			{
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
				if (flag)
				{
					Singleton<CHeroSelectBanPickSystem>.instance.RefreshBottom();
				}
			}
		}

		[MessageHandler(1243)]
		public static void RecivePlayerConfirm(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!Singleton<GameDataValidator>.instance.ValidateGameData())
			{
				return;
			}
			SCPKG_CONFIRM_HERO_NTF stConfirmHeroNtf = msg.stPkgData.stConfirmHeroNtf;
			bool flag = false;
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			MemberInfo memberInfo = roomInfo.GetMemberInfo(stConfirmHeroNtf.dwObjId);
			if (memberInfo == null)
			{
				return;
			}
			memberInfo.isPrepare = true;
			if (memberInfo.dwObjId == roomInfo.selfObjID)
			{
				Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
				flag = true;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				if (memberInfo.dwObjId == roomInfo.selfObjID && Singleton<CHeroSelectBaseSystem>.instance.selectType != enSelectType.enRandom)
				{
					Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect(1);
				}
				Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, memberInfo.dwObjId == roomInfo.selfObjID);
				if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enClone)
				{
					MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
					if (masterMemberInfo == null)
					{
						return;
					}
					if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsAllConfirmHeroByTeam(masterMemberInfo.camp))
					{
						Singleton<CUIManager>.instance.OpenTips("Clone_Confirm_Tips", true, 1.5f, null, new object[0]);
					}
				}
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
			{
				if (flag)
				{
					Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
				}
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
			}
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_Select_Hero", null);
		}

		[MessageHandler(1246)]
		public static void RecivePlayerCancelConfirm(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!Singleton<GameDataValidator>.instance.ValidateGameData())
			{
				return;
			}
			SCPKG_HERO_CANCEL_CONFIRM_NTF stHeroCancelConfirmNtf = msg.stPkgData.stHeroCancelConfirmNtf;
			bool flag = false;
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			MemberInfo memberInfo = roomInfo.GetMemberInfo(stHeroCancelConfirmNtf.dwObjId);
			if (memberInfo == null)
			{
				return;
			}
			memberInfo.isPrepare = false;
			if (memberInfo.dwObjId == roomInfo.selfObjID)
			{
				Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = false;
				flag = true;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				if (flag && Singleton<CHeroSelectBaseSystem>.instance.selectType != enSelectType.enRandom)
				{
					Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect(0);
				}
				Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, memberInfo.dwObjId == roomInfo.selfObjID);
			}
		}

		[MessageHandler(1138)]
		public static void ReciveHeroSymbolPageSel(CSPkg msg)
		{
			uint dwHeroID = msg.stPkgData.stSymbolPageChgRsp.dwHeroID;
			int bPageIdx = (int)msg.stPkgData.stSymbolPageChgRsp.bPageIdx;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			masterRoleInfo.SetHeroSymbolPageIdx(dwHeroID, bPageIdx);
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				Singleton<CHeroSelectNormalSystem>.GetInstance().OnSymbolPageChange();
				CSymbolWearController.PlayPageSelectAnim(enSymbolPageOpenSrc.enHeroSelectNormal);
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
			{
				Singleton<CHeroSelectBanPickSystem>.GetInstance().OnSymbolPageChange();
				CSymbolWearController.PlayPageSelectAnim(enSymbolPageOpenSrc.enHeroSelectBanPic);
			}
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
		}

		[MessageHandler(1167)]
		public static void ReciveAddedSkillSel(CSPkg msg)
		{
			if (msg.stPkgData.stUnlockSkillSelRsp.iResult != 0)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			bool flag = false;
			if (masterRoleInfo != null && masterRoleInfo.playerUllUID == msg.stPkgData.stUnlockSkillSelRsp.ullAcntUid)
			{
				masterRoleInfo.SetHeroSelSkillID(msg.stPkgData.stUnlockSkillSelRsp.dwHeroID, msg.stPkgData.stUnlockSkillSelRsp.dwSkillID);
				flag = true;
			}
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo != null)
			{
				MemberInfo memberInfo = roomInfo.GetMemberInfo(msg.stPkgData.stUnlockSkillSelRsp.ullAcntUid);
				if (memberInfo != null && memberInfo.ChoiceHero[0] != null)
				{
					memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = msg.stPkgData.stUnlockSkillSelRsp.dwSkillID;
				}
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, true);
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
			{
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
				if (flag)
				{
					Singleton<CHeroSelectBanPickSystem>.instance.RefreshAddedSkillItem();
				}
			}
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
		}

		[MessageHandler(1248)]
		public static void ReciveShowBattleHistory(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!Singleton<GameDataValidator>.instance.ValidateGameData())
			{
				return;
			}
			SCPKG_SHOW_HERO_WIN_RATIO_NTF stShowHeroWinRatioNtf = msg.stPkgData.stShowHeroWinRatioNtf;
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			MemberInfo memberInfo = roomInfo.GetMemberInfo(stShowHeroWinRatioNtf.dwObjId);
			if (memberInfo == null)
			{
				return;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				Singleton<CHeroSelectNormalSystem>.instance.ShowHeroInfoWithChatSys(memberInfo);
			}
		}

		[MessageHandler(1244)]
		public static void ReciveDefaultSelectHeroes(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!Singleton<GameDataValidator>.instance.ValidateGameData())
			{
				return;
			}
			SCPKG_DEFAULT_HERO_NTF stDefaultHeroNtf = msg.stPkgData.stDefaultHeroNtf;
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			bool flag = false;
			for (int i = 0; i < (int)stDefaultHeroNtf.bAcntNum; i++)
			{
				COMDT_PLAYERINFO cOMDT_PLAYERINFO = stDefaultHeroNtf.astDefaultHeroGrp[i];
				MemberInfo memberInfo = roomInfo.GetMemberInfo((COM_PLAYERCAMP)cOMDT_PLAYERINFO.bObjCamp, (int)cOMDT_PLAYERINFO.bPosOfCamp);
				if (memberInfo != null)
				{
					memberInfo.ChoiceHero = cOMDT_PLAYERINFO.astChoiceHero;
					if (memberInfo.dwObjId == roomInfo.selfObjID)
					{
						Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
						flag = true;
					}
				}
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				if (flag)
				{
					Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
				}
				Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
			}
		}

		[MessageHandler(5211)]
		public static void RecivePlayerBanOrPick(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_NTF_CUR_BAN_PICK_INFO stNtfCurBanPickInfo = msg.stPkgData.stNtfCurBanPickInfo;
			Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo = stNtfCurBanPickInfo;
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
			Singleton<CHeroSelectBanPickSystem>.instance.PlayCurrentBgAnimation();
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
				{
					Utility.VibrateHelper();
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_3", null);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.IsCurOpByCamp(masterMemberInfo))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_2", null);
				}
				else if (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_1", null);
				}
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
				{
					Utility.VibrateHelper();
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_4", null);
					Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment2", null);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.IsCurOpByCamp(masterMemberInfo))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_7", null);
				}
				else if (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_6", null);
				}
			}
		}

		[MessageHandler(5213)]
		public static void RecivePlayerBanRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_BAN_HERO_RSP stBanHeroRsp = msg.stPkgData.stBanHeroRsp;
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			if (roomInfo.GetMasterMemberInfo() == null)
			{
				return;
			}
			Singleton<CHeroSelectBaseSystem>.instance.AddBanHero((COM_PLAYERCAMP)stBanHeroRsp.bCamp, stBanHeroRsp.dwHeroID);
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshTop();
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_Ban_Button", null);
		}

		[MessageHandler(5215)]
		public static void RecivePlayerSwapReqRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_NTF_SWAP_HERO stNtfSwapHero = msg.stPkgData.stNtfSwapHero;
			RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
			if (roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			if (stNtfSwapHero.iErrCode == 0)
			{
				if (stNtfSwapHero.dwActiveObjID == masterMemberInfo.dwObjId)
				{
					Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enReqing;
					Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo = stNtfSwapHero;
				}
				else if (stNtfSwapHero.dwPassiveObjID == masterMemberInfo.dwObjId)
				{
					Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enSwapAllow;
					Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo = stNtfSwapHero;
				}
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshSwapPanel();
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_Exchange_Hero", null);
			}
			else if (stNtfSwapHero.iErrCode == 156)
			{
				Singleton<CUIManager>.instance.OpenTips("BP_ChangeHero_Busy", true, 1.5f, null, new object[0]);
			}
			else if (stNtfSwapHero.iErrCode == 157)
			{
				Singleton<CUIManager>.instance.OpenTips("BP_ChangeHero_Busy", true, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(5217)]
		public static void RecivePlayerSwapConfirmRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_NTF_CONFIRM_SWAP_HERO stNtfConfirmSwapHero = msg.stPkgData.stNtfConfirmSwapHero;
			Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enIdle;
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
			Singleton<CHeroSelectBanPickSystem>.instance.RefreshSwapPanel();
			if (stNtfConfirmSwapHero.bAccept == 1)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_Select_Hero", null);
			}
		}

		[MessageHandler(5227)]
		public static void RecivePlayerSwapReqCancelRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CANCEL_SWAP_HERO_RSP stCancelSwapHeroRsp = msg.stPkgData.stCancelSwapHeroRsp;
			if (stCancelSwapHeroRsp.iErrCode == 0)
			{
				Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enIdle;
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshSwapPanel();
			}
		}
	}
}
