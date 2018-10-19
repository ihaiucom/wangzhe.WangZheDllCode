using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CBattleEquipSystem
	{
		private struct stSelectedEquipItem
		{
			public CEquipInfo m_equipInfo;

			public Transform m_equipItemTransform;

			public int m_positionInBag;
		}

		private enum enSelectedEquipOrigin
		{
			None = -1,
			EquipLibaray,
			EquipTree,
			EquipBag,
			Count
		}

		private enum enEquipChangeFlag
		{
			GoldCoinInBattleChanged = 1,
			EquipInBattleChanged
		}

		public class CEquipBuyPrice
		{
			public struct stSwappedPreEquipInfo
			{
				public ushort m_equipID;

				public uint m_swappedAmount;
			}

			public CrypticInt32 m_buyPrice;

			public CBattleEquipSystem.CEquipBuyPrice.stSwappedPreEquipInfo[] m_swappedPreEquipInfos;

			public int m_swappedPreEquipInfoCount;

			public bool m_used;

			public CEquipBuyPrice(uint buyPrice)
			{
				this.m_buyPrice = (int)buyPrice;
				this.m_swappedPreEquipInfos = new CBattleEquipSystem.CEquipBuyPrice.stSwappedPreEquipInfo[6];
				this.m_swappedPreEquipInfoCount = 0;
				this.m_used = true;
			}

			public void Clear()
			{
				this.m_buyPrice = 0;
				for (int i = 0; i < 6; i++)
				{
					this.m_swappedPreEquipInfos[i].m_equipID = 0;
					this.m_swappedPreEquipInfos[i].m_swappedAmount = 0u;
				}
				this.m_swappedPreEquipInfoCount = 0;
			}

			public void Clone(CBattleEquipSystem.CEquipBuyPrice equipBuyPrice)
			{
				if (equipBuyPrice == null)
				{
					return;
				}
				this.m_buyPrice = equipBuyPrice.m_buyPrice;
				for (int i = 0; i < 6; i++)
				{
					this.m_swappedPreEquipInfos[i] = equipBuyPrice.m_swappedPreEquipInfos[i];
				}
				this.m_swappedPreEquipInfoCount = equipBuyPrice.m_swappedPreEquipInfoCount;
			}

			public void AddSwappedPreEquipInfo(ushort equipID)
			{
				for (int i = 0; i < this.m_swappedPreEquipInfoCount; i++)
				{
					if (this.m_swappedPreEquipInfos[i].m_equipID == equipID)
					{
						CBattleEquipSystem.CEquipBuyPrice.stSwappedPreEquipInfo[] expr_2A_cp_0 = this.m_swappedPreEquipInfos;
						int expr_2A_cp_1 = i;
						expr_2A_cp_0[expr_2A_cp_1].m_swappedAmount = expr_2A_cp_0[expr_2A_cp_1].m_swappedAmount + 1u;
						return;
					}
				}
				if (this.m_swappedPreEquipInfoCount < 6)
				{
					this.m_swappedPreEquipInfos[this.m_swappedPreEquipInfoCount].m_equipID = equipID;
					this.m_swappedPreEquipInfos[this.m_swappedPreEquipInfoCount].m_swappedAmount = 1u;
					this.m_swappedPreEquipInfoCount++;
				}
			}
		}

		public const int c_equipLevelMaxCount = 3;

		public const int c_maxEquipCntPerLevel = 12;

		public const uint c_quicklyBuyEquipCount = 2u;

		public const int c_fadeTime = 10000;

		public const int c_animationTime = 2000;

		public static string s_equipFormPath = "UGUI/Form/Battle/Form_Battle_Equip.prefab";

		private bool m_isEnabled;

		private bool m_isInEquipLimitedLevel;

		private PoolObjHandle<ActorRoot> m_hostCtrlHero;

		private bool m_hostCtrlHeroPermitedToBuyEquip;

		private CUIFormScript m_battleFormScript;

		private static int s_equipUsageAmount = Enum.GetValues(typeof(enEquipUsage)).Length;

		private List<ushort>[][] m_equipList;

		private Dictionary<ushort, CEquipInfo> m_equipInfoDictionary;

		private CEquipRelationPath m_equipRelationPath;

		private CUIFormScript m_equipFormScript;

		private CUIListScript m_backEquipListScript;

		private enEquipUsage m_curEquipUsage;

		private CBattleEquipSystem.enSelectedEquipOrigin m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.None;

		private CBattleEquipSystem.stSelectedEquipItem[] m_selectedEquipItems = new CBattleEquipSystem.stSelectedEquipItem[3];

		private bool m_isEquipTreeOpened;

		private CEquipInfo m_rootEquipInTree;

		private stEquipTree m_equipTree = new stEquipTree(3, 2, 20);

		private ListView<Transform> m_equipItemList = new ListView<Transform>();

		private ListView<Transform> m_bagEquipItemList = new ListView<Transform>();

		private uint m_equipChangedFlags;

		private ushort[] m_tempQuicklyBuyEquipIDs = new ushort[2];

		private ListView<CEquipInfo> m_tempRelatedRecommondEquips = new ListView<CEquipInfo>();

		private ushort[] m_hostPlayerQuicklyBuyEquipIDs = new ushort[2];

		private Dictionary<ushort, CBattleEquipSystem.CEquipBuyPrice> m_hostPlayerCachedEquipBuyPrice = new Dictionary<ushort, CBattleEquipSystem.CEquipBuyPrice>();

		private ListView<CExistEquipInfoSet> m_existEquipInfoSetPool = new ListView<CExistEquipInfoSet>(5);

		private ListView<CBattleEquipSystem.CEquipBuyPrice> m_equipBuyPricePool = new ListView<CBattleEquipSystem.CEquipBuyPrice>(5);

		private float m_uiEquipItemHeight;

		private float m_uiEquipItemContentDefaultHeight;

		private int m_tickFadeTime;

		private int m_tickAnimationTime;

		private bool m_bPlayAnimation;

		private GameObject[] m_objEquipActiveTimerTxt = new GameObject[6];

		private int[] m_arrEquipActiveSkillCd = new int[6];

		private static int s_horizon_Pos = 6;

		public CBattleEquipSystem()
		{
			this.m_equipInfoDictionary = Singleton<CEquipSystem>.GetInstance().GetEquipInfoDictionary();
			this.m_equipList = Singleton<CEquipSystem>.GetInstance().GetEquipList();
			this.m_equipRelationPath = new CEquipRelationPath();
			Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<ushort, CBattleEquipSystem.CEquipBuyPrice> arg_112_0 = this.m_hostPlayerCachedEquipBuyPrice;
				KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
				ushort arg_112_1 = current.Key;
				KeyValuePair<ushort, CEquipInfo> current2 = enumerator.Current;
				arg_112_0.Add(arg_112_1, new CBattleEquipSystem.CEquipBuyPrice(current2.Value.m_resEquipInBattle.dwBuyPrice));
			}
		}

		public void Initialize(CUIFormScript battleFormScript, PoolObjHandle<ActorRoot> hostCtrlHero, bool enableEquipSystem, bool isInEquipLimitedLevel)
		{
			this.Clear();
			this.m_battleFormScript = battleFormScript;
			this.m_hostCtrlHero = hostCtrlHero;
			DebugHelper.Assert(this.m_hostCtrlHero, "Initialize EquipSystem with null host ctrl hero.");
			this.m_isEnabled = enableEquipSystem;
			this.m_isInEquipLimitedLevel = isInEquipLimitedLevel;
			this.m_hostCtrlHeroPermitedToBuyEquip = false;
			this.m_isEquipTreeOpened = false;
			this.m_rootEquipInTree = null;
			EquipComponent.s_equipEffectSequence = 0u;
			if (!this.m_isEnabled)
			{
				return;
			}
			this.RefreshHostPlayerCachedEquipBuyPrice();
			this.OnEquipFormOpen(null);
			Singleton<CUIManager>.GetInstance().CloseForm(CBattleEquipSystem.s_equipFormPath);
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this.OnHeroEquipInBattleChanged));
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("HeroRecommendEquipInit", new Action<uint>(this.OnHeroRecommendEquipInit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_ShowOrHideInBattle, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipShowOrHideInBattleBtnClicked));
		}

		public void Clear()
		{
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this.OnHeroEquipInBattleChanged));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroRecommendEquipInit", new Action<uint>(this.OnHeroRecommendEquipInit));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_ShowOrHideInBattle, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipShowOrHideInBattleBtnClicked));
			this.m_battleFormScript = null;
			this.m_hostCtrlHero.Release();
			this.m_isEnabled = false;
			this.m_hostCtrlHeroPermitedToBuyEquip = false;
			this.m_curEquipUsage = enEquipUsage.Recommend;
			this.ClearEquipList(enEquipUsage.Recommend);
			this.ClearHostPlayerEquipInfo();
			this.m_tickFadeTime = 0;
			this.m_tickAnimationTime = 0;
			this.m_bPlayAnimation = false;
			for (int i = 0; i < 6; i++)
			{
				this.m_objEquipActiveTimerTxt[i] = null;
				this.m_arrEquipActiveSkillCd[i] = 0;
			}
			Singleton<CUIManager>.GetInstance().CloseForm(CBattleEquipSystem.s_equipFormPath);
		}

		public void Update()
		{
			if (!this.m_isEnabled || Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			if (this.HasEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.EquipInBattleChanged) || this.HasEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.GoldCoinInBattleChanged))
			{
				if (this.m_hostCtrlHeroPermitedToBuyEquip)
				{
					this.RefreshHostPlayerQuicklyBuyEquipList();
				}
				if (this.m_equipFormScript != null)
				{
					if (this.m_isEquipTreeOpened)
					{
						this.RefreshEquipTreePanel(true);
					}
					else
					{
						this.RefreshEquipLibraryPanel(true);
					}
					this.RefreshEquipFormRightPanel(true);
					if (this.HasEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.EquipInBattleChanged))
					{
						this.RefreshEquipBagPanel();
					}
					if (this.HasEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.GoldCoinInBattleChanged))
					{
						this.RefreshGoldCoin();
					}
				}
			}
			this.ClearEquipChangeFlag();
			if (this.m_hostCtrlHero)
			{
				bool flag = this.m_hostCtrlHero.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel);
				if (this.m_hostCtrlHeroPermitedToBuyEquip != flag)
				{
					this.m_hostCtrlHeroPermitedToBuyEquip = flag;
					if (this.m_hostCtrlHeroPermitedToBuyEquip)
					{
						this.RefreshHostPlayerQuicklyBuyEquipList();
					}
					this.RefreshHostPlayerQuicklyBuyEquipPanel(this.m_hostCtrlHeroPermitedToBuyEquip);
					if (this.m_equipFormScript != null && this.m_selectedEquipOrigin != CBattleEquipSystem.enSelectedEquipOrigin.None && this.m_selectedEquipOrigin != CBattleEquipSystem.enSelectedEquipOrigin.EquipBag)
					{
						GameObject widget = this.m_equipFormScript.GetWidget(6);
						this.RefreshEquipBuyPanel(this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo, widget.transform);
					}
				}
			}
			if (this.m_equipFormScript != null)
			{
				this.UpdateEquipActiveSkillCd(false);
			}
		}

		public void UpdateLogic(int delta)
		{
			if (this.m_tickFadeTime > 0)
			{
				this.m_tickFadeTime -= delta;
				if (this.m_tickFadeTime <= 0)
				{
					this.m_bPlayAnimation = true;
					this.m_tickAnimationTime = 2000;
				}
			}
			if (this.m_bPlayAnimation)
			{
				this.m_tickAnimationTime -= delta;
				if (!GameSettings.ShowEquipInfo)
				{
					this.SetQuicklyBuyInfoPanelAlpha(0f);
				}
				else
				{
					this.SetQuicklyBuyInfoPanelAlpha((float)this.m_tickAnimationTime / 2000f);
				}
				this.m_bPlayAnimation = (this.m_tickAnimationTime > 0);
			}
		}

		public bool IsInEquipLimitedLevel()
		{
			return this.m_isInEquipLimitedLevel;
		}

		public List<ushort> GetUsageLevelEquipList(enEquipUsage equipUsage, int level)
		{
			if (equipUsage <= enEquipUsage.Recommend || equipUsage > enEquipUsage.Horizon || level <= 0 || level > 3)
			{
				return null;
			}
			return this.m_equipList[(int)equipUsage][level - 1];
		}

        public bool IsEquipCanBought(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref CEquipBuyPrice equipBuyPrice)
        {
            if ((!this.m_isEnabled || (equipID == 0)) || (actor == null))
            {
                return false;
            }
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
            if (((dataByKey == null) || (dataByKey.bInvalid > 0)) || (dataByKey.bIsAttachEquip > 0))
            {
                return false;
            }
            if ((dataByKey.bNeedPunish > 0) && !actor.handle.SkillControl.HasPunishSkill())
            {
                return false;
            }
            if ((dataByKey.wGroup > 0) && actor.handle.EquipComponent.HasEquipInGroup(dataByKey.wGroup))
            {
                return false;
            }
            ushort[] requiredEquipIDs = this.GetRequiredEquipIDs(equipID);
            if ((requiredEquipIDs != null) && (requiredEquipIDs.Length > 0))
            {
                bool flag = false;
                for (int i = 0; i < requiredEquipIDs.Length; i++)
                {
                    if (actor.handle.EquipComponent.HasEquip(requiredEquipIDs[i], 1))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            if (actor == this.m_hostCtrlHero)
            {
                equipBuyPrice.Clone(this.GetHostPlayerCachedEquipBuyPrice(equipID));
            }
            else
            {
                this.GetEquipBuyPrice(equipID, ref actor, ref equipBuyPrice);
            }
            if (((ulong)actor.handle.ValueComponent.GetGoldCoinInBattle()) < ((ulong)(int)equipBuyPrice.m_buyPrice))
            {
                return false;
            }
            if (!actor.handle.EquipComponent.IsEquipCanAddedToGrid(equipID, ref equipBuyPrice))
            {
                return false;
            }
            return true;
        }


		public bool IsHorizonEquipCanBought(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref int price)
		{
			if (!this.m_isEnabled || equipID == 0 || !actor)
			{
				return false;
			}
			ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
			if (dataByKey == null || dataByKey.bInvalid > 0 || dataByKey.bIsAttachEquip > 0)
			{
				return false;
			}
			price = dataByKey.dwBuyPrice;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_bEnableOrnamentSlot)
			{
				SkillSlot skillSlot = actor.handle.SkillControl.SkillSlotArray[7];
				if (dataByKey.dwActiveSkillID > 0u && (skillSlot == null || (ulong)dataByKey.dwActiveSkillID != (ulong)((long)skillSlot.SkillObj.SkillID)) && actor.handle.ValueComponent.GetGoldCoinInBattle() >= (int)dataByKey.dwBuyPrice)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsHorizonEquipOwn(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!this.m_isEnabled || equipID == 0 || !actor)
			{
				return false;
			}
			ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
			if (dataByKey == null || dataByKey.bInvalid > 0)
			{
				return false;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_bEnableOrnamentSlot)
			{
				SkillSlot skillSlot = actor.handle.SkillControl.SkillSlotArray[7];
				if (skillSlot != null && skillSlot.SkillObj != null)
				{
					return (ulong)dataByKey.dwActiveSkillID == (ulong)((long)skillSlot.SkillObj.SkillID);
				}
			}
			return false;
		}

		public void GetEquipBuyPrice(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref CBattleEquipSystem.CEquipBuyPrice equipBuyPrice)
		{
			equipBuyPrice.Clear();
			if (!this.m_isEnabled || equipID == 0 || !actor)
			{
				return;
			}
			ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
			if (dataByKey == null)
			{
				return;
			}
			uint num = dataByKey.dwBuyPrice;
			CExistEquipInfoSet freeExistEquipInfoSet = this.GetFreeExistEquipInfoSet();
			freeExistEquipInfoSet.Clone(actor.handle.EquipComponent.GetExistEquipInfoSet());
			freeExistEquipInfoSet.ResetCalculateAmount();
			uint preEquipSwappedPrice = this.GetPreEquipSwappedPrice(equipID, ref freeExistEquipInfoSet, ref equipBuyPrice);
			freeExistEquipInfoSet.m_used = false;
			if (num >= preEquipSwappedPrice)
			{
				num -= preEquipSwappedPrice;
			}
			else
			{
				num = 0u;
			}
			equipBuyPrice.m_buyPrice = (int)num;
		}

		public ushort[] GetQuicklyBuyEquipList(ref PoolObjHandle<ActorRoot> actor)
		{
			int num = 0;
			while ((long)num < 2L)
			{
				this.m_tempQuicklyBuyEquipIDs[num] = 0;
				num++;
			}
			if (!actor || actor.handle.EquipComponent == null)
			{
				return this.m_tempQuicklyBuyEquipIDs;
			}
			stRecommendEquipInfo[] recommendEquipInfos = actor.handle.EquipComponent.GetRecommendEquipInfos();
			Dictionary<ushort, uint> equipBoughtHistory = actor.handle.EquipComponent.GetEquipBoughtHistory();
			bool flag = false;
			List<ushort> usageLevelEquipList = this.GetUsageLevelEquipList(enEquipUsage.Move, 2);
			if (usageLevelEquipList != null)
			{
				for (int i = 0; i < usageLevelEquipList.Count; i++)
				{
					if (equipBoughtHistory.ContainsKey(usageLevelEquipList[i]))
					{
						flag = true;
						break;
					}
				}
			}
			CExistEquipInfoSet freeExistEquipInfoSet = this.GetFreeExistEquipInfoSet();
			freeExistEquipInfoSet.Clone(actor.handle.EquipComponent.GetExistEquipInfoSet());
			int num2 = 0;
			for (int j = 0; j < recommendEquipInfos.Length; j++)
			{
				if (recommendEquipInfos[j].m_equipID == 0)
				{
					break;
				}
				if (!recommendEquipInfos[j].m_hasBeenBought)
				{
					if (recommendEquipInfos[j].m_resEquipInBattle.bUsage != 4 || !flag)
					{
						this.m_tempRelatedRecommondEquips.Clear();
						freeExistEquipInfoSet.ResetCalculateAmount();
						this.GetRelatedRecommondEquips(recommendEquipInfos[j].m_equipID, true, ref actor, ref freeExistEquipInfoSet, ref this.m_tempRelatedRecommondEquips);
						for (int k = 0; k < num2; k++)
						{
							for (int l = 0; l < this.m_tempRelatedRecommondEquips.Count; l++)
							{
								if (this.m_tempQuicklyBuyEquipIDs[k] == this.m_tempRelatedRecommondEquips[l].m_equipID)
								{
									this.m_tempRelatedRecommondEquips.RemoveAt(l);
									break;
								}
							}
						}
						if (this.m_tempRelatedRecommondEquips.Count == 0)
						{
							break;
						}
						this.m_tempRelatedRecommondEquips.Sort();
						int num3 = Math.Min(this.m_tempRelatedRecommondEquips.Count, 2 - num2);
						for (int m = 0; m < num3; m++)
						{
							this.m_tempQuicklyBuyEquipIDs[num2] = this.m_tempRelatedRecommondEquips[m].m_equipID;
							num2++;
						}
						if ((long)num2 >= 2L)
						{
							break;
						}
					}
				}
			}
			freeExistEquipInfoSet.m_used = false;
			return this.m_tempQuicklyBuyEquipIDs;
		}

		public bool IsHostCtrlHeroPermitedToBuyEquip()
		{
			return this.m_hostCtrlHeroPermitedToBuyEquip;
		}

		public ushort[] GetHostCtrlHeroQuicklyBuyEquipList()
		{
			return this.m_hostPlayerQuicklyBuyEquipIDs;
		}

		private void OnHeroEquipInBattleChanged(uint actorObjectID, stEquipInfo[] equips, bool bIsAdd, int iEquipSlotIndex)
		{
			if (!this.m_isEnabled)
			{
				return;
			}
			if (this.m_hostCtrlHero && actorObjectID == this.m_hostCtrlHero.handle.ObjID)
			{
				this.RefreshHostPlayerCachedEquipBuyPrice();
				this.SetEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.EquipInBattleChanged);
			}
			this.HandleEquipActiveSkillWhenEquipChange(actorObjectID, bIsAdd, iEquipSlotIndex);
		}

		private void OnHeroRecommendEquipInit(uint actorObjectID)
		{
			if (!this.m_isEnabled)
			{
				return;
			}
			if (this.m_hostCtrlHero && actorObjectID == this.m_hostCtrlHero.handle.ObjID)
			{
				this.InitializeRecommendEquipList();
			}
		}

		public void SendBuyEquipFrameCommand(ushort equipID, bool isQuicklyBuy)
		{
			FrameCommand<PlayerBuyEquipCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerBuyEquipCommand>();
			frameCommand.cmdData.m_equipID = equipID;
			frameCommand.Send();
			if (isQuicklyBuy)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_junei_ani_goumai", null);
				if (this.m_hostCtrlHero)
				{
					this.m_hostCtrlHero.handle.EquipComponent.m_iFastBuyEquipCount++;
				}
			}
			else if (this.m_hostCtrlHero)
			{
				this.m_hostCtrlHero.handle.EquipComponent.m_iBuyEquipCount++;
			}
		}

		public void SendSellEquipFrameCommand(int equipIndexInGrid)
		{
			FrameCommand<PlayerSellEquipCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerSellEquipCommand>();
			frameCommand.cmdData.m_equipIndex = equipIndexInGrid;
			frameCommand.Send();
		}

		public void SendBuyHorizonEquipFrameCommand(ushort equipID)
		{
			FrameCommand<PlayerBuyHorizonEquipCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerBuyHorizonEquipCommand>();
			frameCommand.cmdData.m_equipID = equipID;
			frameCommand.Send();
		}

		public void SendInOutEquipShopFrameCommand(byte inOut)
		{
			FrameCommand<PlayerInOutEquipShopCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerInOutEquipShopCommand>();
			frameCommand.cmdData.m_inOut = inOut;
			frameCommand.Send();
		}

		public void SendChangeUsedRecommendEquipGroupCommand(byte group)
		{
			FrameCommand<PlayerChangeUsedRecommendEquipGroupCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerChangeUsedRecommendEquipGroupCommand>();
			frameCommand.cmdData.m_group = group;
			frameCommand.Send();
		}

		public void ExecuteBuyEquipFrameCommand(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
		{
			this.BuyEquip(equipID, ref actor);
		}

		public void ExecuteSellEquipFrameCommand(int equipIndex, ref PoolObjHandle<ActorRoot> actor)
		{
			this.SellEquip(equipIndex, ref actor);
		}

		public void ExecuteBuyHorizonEquipFrameCommand(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
		{
			this.BuyHorizonEquip(equipID, ref actor);
		}

		public void ExecuteInOutEquipShopFrameCommand(byte inOut, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!Singleton<WatchController>.GetInstance().IsWatching && !actor.handle.IsHostCamp())
			{
				return;
			}
			if (inOut > 0)
			{
				actor.handle.HudControl.AddInEquipShopHud();
			}
			else
			{
				actor.handle.HudControl.RemoveInEquipShopHud();
			}
		}

		public void ExecuteChangeUsedRecommendEquipGroup(byte group, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!this.m_isEnabled)
			{
				return;
			}
			if (actor.handle.EquipComponent != null)
			{
				actor.handle.EquipComponent.SetUsedRecommendEquipGroup((uint)group);
			}
			if (!Singleton<WatchController>.GetInstance().IsWatching && actor == this.m_hostCtrlHero)
			{
				this.InitializeRecommendEquipList();
				this.RefreshRcmdEquipPlanPanel();
				uint configId = (uint)this.m_hostCtrlHero.handle.TheStaticData.TheActorMeta.ConfigId;
				stRcmdEquipListInfo stRcmdEquipListInfo = this.m_hostCtrlHero.handle.EquipComponent.ConvertRcmdEquipListInfo();
				CEquipSystem.RefreshSelfEquipPlanForm(configId, ref stRcmdEquipListInfo, enUIEventID.BattleEquip_ChangeSelfRcmEuipPlan, false, true);
				this.OnRcmdEquipPlanChangeSuccess();
				if (this.m_equipFormScript != null && this.m_curEquipUsage == enEquipUsage.Recommend)
				{
					this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray);
					if (this.m_equipRelationPath != null)
					{
						this.m_equipRelationPath.Reset();
					}
					if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray)
					{
						this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.None;
						this.EnableOpenEquipTreeButton(false);
						this.RefreshEquipFormRightPanel(false);
					}
					if (!this.m_isEquipTreeOpened)
					{
						this.RefreshEquipLibraryPanel(false);
					}
				}
				this.RefreshHostPlayerQuicklyBuyEquipList();
			}
		}

		private void InitializeRecommendEquipList()
		{
			this.ClearEquipList(enEquipUsage.Recommend);
			DebugHelper.Assert(this.m_hostCtrlHero, "InitializeEquipList m_hostCtrlHero is null");
			if (this.m_hostCtrlHero)
			{
				stRecommendEquipInfo[] recommendEquipInfos = this.m_hostCtrlHero.handle.EquipComponent.GetRecommendEquipInfos();
				if (recommendEquipInfos != null)
				{
					for (int i = 0; i < recommendEquipInfos.Length; i++)
					{
						if (recommendEquipInfos[i].m_equipID == 0)
						{
							break;
						}
						this.AddRecommendPreEquip(recommendEquipInfos[i].m_equipID, true);
					}
				}
			}
		}

		private void AddRecommendPreEquip(ushort equipId, bool bRootRecommend)
		{
			ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipId);
			if (dataByKey == null || dataByKey.bInvalid > 0 || dataByKey.bIsAttachEquip > 0)
			{
				return;
			}
			if (this.m_equipList[0][(int)(dataByKey.bLevel - 1)].Count >= 12)
			{
				return;
			}
			if (bRootRecommend || !this.m_equipList[0][(int)(dataByKey.bLevel - 1)].Contains(equipId))
			{
				this.m_equipList[0][(int)(dataByKey.bLevel - 1)].Add(equipId);
			}
			for (int i = 0; i < dataByKey.PreEquipID.Length; i++)
			{
				this.AddRecommendPreEquip(dataByKey.PreEquipID[i], false);
			}
		}

		private void ClearEquipList(enEquipUsage equipUsage)
		{
			for (int i = 0; i < 3; i++)
			{
				this.m_equipList[(int)equipUsage][i].Clear();
			}
		}

		private void ClearHostPlayerEquipInfo()
		{
			for (int i = 0; i < this.m_hostPlayerQuicklyBuyEquipIDs.Length; i++)
			{
				this.m_hostPlayerQuicklyBuyEquipIDs[i] = 0;
			}
			this.m_hostPlayerCachedEquipBuyPrice.Clear();
		}

		private void RefreshHostPlayerCachedEquipBuyPrice()
		{
			Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
				ushort key = current.Key;
				CBattleEquipSystem.CEquipBuyPrice value = null;
				if (this.m_hostPlayerCachedEquipBuyPrice.TryGetValue(key, out value))
				{
					this.GetEquipBuyPrice(key, ref this.m_hostCtrlHero, ref value);
				}
				else
				{
					value = new CBattleEquipSystem.CEquipBuyPrice(0u);
					this.GetEquipBuyPrice(key, ref this.m_hostCtrlHero, ref value);
					this.m_hostPlayerCachedEquipBuyPrice.Add(key, value);
				}
			}
		}

		private CBattleEquipSystem.CEquipBuyPrice GetHostPlayerCachedEquipBuyPrice(ushort equipID)
		{
			CBattleEquipSystem.CEquipBuyPrice result = null;
			if (this.m_hostPlayerCachedEquipBuyPrice.TryGetValue(equipID, out result))
			{
				return result;
			}
			return null;
		}

		private void RefreshHostPlayerQuicklyBuyEquipList()
		{
			if (!this.m_hostCtrlHero)
			{
				return;
			}
			ushort[] quicklyBuyEquipList = this.GetQuicklyBuyEquipList(ref this.m_hostCtrlHero);
			bool flag = false;
			int num = 0;
			while ((long)num < 2L)
			{
				if (this.m_hostPlayerQuicklyBuyEquipIDs[num] != quicklyBuyEquipList[num])
				{
					this.m_hostPlayerQuicklyBuyEquipIDs[num] = quicklyBuyEquipList[num];
					if (!flag)
					{
						flag = true;
					}
				}
				num++;
			}
			if (flag)
			{
				this.RefreshQuicklyBuyPanel(flag);
			}
		}

		public void GetRelatedRecommondEquips(ushort equipID, bool isRootEquip, ref PoolObjHandle<ActorRoot> actor, ref CExistEquipInfoSet existEquipInfoSet, ref ListView<CEquipInfo> relatedRecommondEquips)
		{
			CEquipInfo cEquipInfo = null;
			if (!this.m_equipInfoDictionary.TryGetValue(equipID, out cEquipInfo))
			{
				return;
			}
			if (!isRootEquip)
			{
				for (int i = 0; i < existEquipInfoSet.m_existEquipInfoCount; i++)
				{
					if (existEquipInfoSet.m_existEquipInfos[i].m_equipID == equipID && existEquipInfoSet.m_existEquipInfos[i].m_calculateAmount > 0u)
					{
						stExistEquipInfo[] expr_63_cp_0 = existEquipInfoSet.m_existEquipInfos;
						int expr_63_cp_1 = i;
						expr_63_cp_0[expr_63_cp_1].m_calculateAmount = expr_63_cp_0[expr_63_cp_1].m_calculateAmount - 1u;
						return;
					}
				}
			}
			CBattleEquipSystem.CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
			if (this.IsEquipCanBought(equipID, ref actor, ref freeEquipBuyPrice))
			{
				if (!relatedRecommondEquips.Contains(cEquipInfo))
				{
					relatedRecommondEquips.Add(cEquipInfo);
				}
				freeEquipBuyPrice.m_used = false;
				return;
			}
			freeEquipBuyPrice.m_used = false;
			for (int j = 0; j < cEquipInfo.m_resEquipInBattle.PreEquipID.Length; j++)
			{
				this.GetRelatedRecommondEquips(cEquipInfo.m_resEquipInBattle.PreEquipID[j], false, ref actor, ref existEquipInfoSet, ref relatedRecommondEquips);
			}
		}

		private void BuyEquip(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!this.m_isEnabled || !actor || !actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
			{
				return;
			}
			CBattleEquipSystem.CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
			if (!this.m_isEnabled || !this.IsEquipCanBought(equipID, ref actor, ref freeEquipBuyPrice))
			{
				freeEquipBuyPrice.m_used = false;
				return;
			}
			actor.handle.ValueComponent.ChangeGoldCoinInBattle(freeEquipBuyPrice.m_buyPrice * -1, false, false, default(Vector3), false, default(PoolObjHandle<ActorRoot>));
			for (int i = 0; i < freeEquipBuyPrice.m_swappedPreEquipInfoCount; i++)
			{
				if (freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_equipID > 0 && freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount > 0u)
				{
					while (freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount > 0u)
					{
						actor.handle.EquipComponent.RemoveEquip(freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_equipID);
						CBattleEquipSystem.CEquipBuyPrice.stSwappedPreEquipInfo[] expr_FB_cp_0 = freeEquipBuyPrice.m_swappedPreEquipInfos;
						int expr_FB_cp_1 = i;
						expr_FB_cp_0[expr_FB_cp_1].m_swappedAmount = expr_FB_cp_0[expr_FB_cp_1].m_swappedAmount - 1u;
					}
				}
			}
			ushort[] requiredEquipIDs = this.GetRequiredEquipIDs(equipID);
			if (requiredEquipIDs != null && requiredEquipIDs.Length > 0)
			{
				for (int j = 0; j < requiredEquipIDs.Length; j++)
				{
					if (actor.handle.EquipComponent.HasEquip(requiredEquipIDs[j], 1u))
					{
						actor.handle.EquipComponent.RemoveEquip(requiredEquipIDs[j]);
						break;
					}
				}
			}
			actor.handle.EquipComponent.AddEquip(equipID);
			actor.handle.EquipComponent.UpdateEquipEffect();
			freeEquipBuyPrice.m_used = false;
		}

		private void SellEquip(int equipIndexInGrid, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!this.m_isEnabled || equipIndexInGrid < 0 || equipIndexInGrid >= 6 || !actor || !actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
			{
				return;
			}
			stEquipInfo[] equips = actor.handle.EquipComponent.GetEquips();
			if (equips[equipIndexInGrid].m_equipID > 0 && equips[equipIndexInGrid].m_amount > 0u)
			{
				ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equips[equipIndexInGrid].m_equipID);
				if (dataByKey != null)
				{
					actor.handle.EquipComponent.RemoveEquip(equipIndexInGrid);
					actor.handle.EquipComponent.UpdateEquipEffect();
					actor.handle.ValueComponent.ChangeGoldCoinInBattle(dataByKey.dwSalePrice, false, false, default(Vector3), false, default(PoolObjHandle<ActorRoot>));
				}
			}
		}

		private void BuyHorizonEquip(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
		{
			if (!this.m_isEnabled || !actor || !actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
			{
				return;
			}
			CEquipInfo equipInfo = this.GetEquipInfo(equipID);
			if (equipInfo == null)
			{
				DebugHelper.Assert(equipInfo != null, "BuyHorizonEquip GetEquipInfo is null equipId = " + equipID);
				return;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_bEnableOrnamentSlot)
			{
				SkillComponent skillControl = actor.handle.SkillControl;
				if (actor.handle.BuffHolderComp != null && skillControl != null)
				{
					SkillSlot skillSlot = skillControl.SkillSlotArray[7];
					if (skillSlot == null || skillSlot.SkillObj == null)
					{
						skillControl.InitSkillSlot(7, (int)equipInfo.m_resEquipInBattle.dwActiveSkillID, 0);
						if (actor.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_7, out skillSlot))
						{
							skillSlot.SetSkillLevel(1);
							if (actor == this.m_hostCtrlHero && Singleton<CBattleSystem>.GetInstance().FightForm != null)
							{
								Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(actor, true, SkillSlotType.SLOT_SKILL_7);
							}
						}
					}
					else
					{
						BuffChangeSkillRule changeSkillRule = actor.handle.BuffHolderComp.changeSkillRule;
						if (changeSkillRule != null)
						{
							changeSkillRule.ChangeSkillSlot(SkillSlotType.SLOT_SKILL_7, (int)equipInfo.m_resEquipInBattle.dwActiveSkillID, skillSlot.SkillObj.SkillID);
						}
					}
					actor.handle.EquipComponent.m_horizonEquipId = equipID;
				}
				if (this.m_hostCtrlHero && actor == this.m_hostCtrlHero)
				{
					this.SetEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.EquipInBattleChanged);
				}
			}
		}

		private CEquipInfo GetEquipInfo(ushort equipID)
		{
			CEquipInfo result = null;
			if (this.m_equipInfoDictionary.TryGetValue(equipID, out result))
			{
				return result;
			}
			return null;
		}

		private ushort[] GetRequiredEquipIDs(ushort equipID)
		{
			CEquipInfo cEquipInfo = null;
			if (this.m_equipInfoDictionary.TryGetValue(equipID, out cEquipInfo))
			{
				return cEquipInfo.m_requiredEquipIDs;
			}
			return null;
		}

		private uint GetPreEquipSwappedPrice(ushort equipID, ref CExistEquipInfoSet existEquipInfoSet, ref CBattleEquipSystem.CEquipBuyPrice equipBuyPrice)
		{
			if (equipID == 0)
			{
				return 0u;
			}
			ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
			if (dataByKey == null)
			{
				return 0u;
			}
			uint num = 0u;
			for (int i = 0; i < dataByKey.PreEquipID.Length; i++)
			{
				if (dataByKey.PreEquipID[i] > 0)
				{
					bool flag = false;
					for (int j = 0; j < existEquipInfoSet.m_existEquipInfoCount; j++)
					{
						if (existEquipInfoSet.m_existEquipInfos[j].m_equipID == dataByKey.PreEquipID[i] && existEquipInfoSet.m_existEquipInfos[j].m_calculateAmount > 0u)
						{
							flag = true;
							num += (uint)existEquipInfoSet.m_existEquipInfos[j].m_unitBuyPrice;
							stExistEquipInfo[] expr_A6_cp_0 = existEquipInfoSet.m_existEquipInfos;
							int expr_A6_cp_1 = j;
							expr_A6_cp_0[expr_A6_cp_1].m_calculateAmount = expr_A6_cp_0[expr_A6_cp_1].m_calculateAmount - 1u;
							equipBuyPrice.AddSwappedPreEquipInfo(dataByKey.PreEquipID[i]);
							break;
						}
					}
					if (!flag)
					{
						num += this.GetPreEquipSwappedPrice(dataByKey.PreEquipID[i], ref existEquipInfoSet, ref equipBuyPrice);
					}
				}
			}
			return num;
		}

		private void SetEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag flag)
		{
			this.m_equipChangedFlags |= (uint)flag;
		}

		private bool HasEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag flag)
		{
			return (this.m_equipChangedFlags & (uint)flag) > 0u;
		}

		private void ClearEquipChangeFlag()
		{
			this.m_equipChangedFlags = 0u;
		}

		private CExistEquipInfoSet GetFreeExistEquipInfoSet()
		{
			for (int i = 0; i < this.m_existEquipInfoSetPool.Count; i++)
			{
				if (!this.m_existEquipInfoSetPool[i].m_used)
				{
					this.m_existEquipInfoSetPool[i].Clear();
					this.m_existEquipInfoSetPool[i].m_used = true;
					return this.m_existEquipInfoSetPool[i];
				}
			}
			CExistEquipInfoSet cExistEquipInfoSet = new CExistEquipInfoSet();
			this.m_existEquipInfoSetPool.Add(cExistEquipInfoSet);
			cExistEquipInfoSet.m_used = true;
			return cExistEquipInfoSet;
		}

		private CBattleEquipSystem.CEquipBuyPrice GetFreeEquipBuyPrice()
		{
			for (int i = 0; i < this.m_equipBuyPricePool.Count; i++)
			{
				if (!this.m_equipBuyPricePool[i].m_used)
				{
					this.m_equipBuyPricePool[i].Clear();
					this.m_equipBuyPricePool[i].m_used = true;
					return this.m_equipBuyPricePool[i];
				}
			}
			CBattleEquipSystem.CEquipBuyPrice cEquipBuyPrice = new CBattleEquipSystem.CEquipBuyPrice(0u);
			this.m_equipBuyPricePool.Add(cEquipBuyPrice);
			cEquipBuyPrice.m_used = true;
			return cEquipBuyPrice;
		}

		private void SetEquipItemSelectFlag(Transform equipItemObj, bool bSelect, CBattleEquipSystem.enSelectedEquipOrigin equipSelectedOrigin, bool bIsActiveSKillEquip)
		{
			if (equipItemObj != null)
			{
				if (equipSelectedOrigin != CBattleEquipSystem.enSelectedEquipOrigin.EquipBag)
				{
					Transform transform = equipItemObj.Find("selectImg");
					if (transform != null)
					{
						transform.gameObject.CustomSetActive(bSelect);
					}
				}
				else
				{
					Transform transform3;
					if (bIsActiveSKillEquip)
					{
						Transform transform2 = equipItemObj.FindChild("Beingused");
						if (transform2.gameObject.activeSelf)
						{
							transform3 = equipItemObj.Find("selectImg");
						}
						else
						{
							transform3 = equipItemObj.Find("SelectImg_InBattle");
							Transform transform4 = equipItemObj.FindChild("BtnShowInBattle");
							if (transform4 && transform4.gameObject.activeSelf != bSelect)
							{
								transform4.gameObject.CustomSetActive(bSelect);
							}
						}
					}
					else
					{
						transform3 = equipItemObj.Find("selectImg");
					}
					if (transform3 != null)
					{
						transform3.gameObject.CustomSetActive(bSelect);
					}
				}
			}
		}

		public void RefreshQuicklyBuyPanel(bool isQuicklyBuyEquipListChanged)
		{
			if (null == this.m_battleFormScript)
			{
				return;
			}
			for (int i = 0; i < this.m_hostPlayerQuicklyBuyEquipIDs.Length; i++)
			{
				ushort equipID = this.m_hostPlayerQuicklyBuyEquipIDs[i];
				GameObject widget = this.m_battleFormScript.GetWidget(47 + i);
				if (widget != null)
				{
					CEquipInfo equipInfo = this.GetEquipInfo(equipID);
					if (equipInfo != null)
					{
						widget.CustomSetActive(true);
						Transform transform = widget.transform;
						Image component = transform.Find("imgIcon").GetComponent<Image>();
						component.SetSprite(equipInfo.m_equipIconPath, this.m_battleFormScript, true, false, false, false);
						Text component2 = transform.Find("Panel_Info/descText").GetComponent<Text>();
						component2.text = string.Format("<color=#ffa500>{0}</color>\n{1}", equipInfo.m_equipName, equipInfo.m_equipDesc);
						Text component3 = transform.Find("moneyText").GetComponent<Text>();
						component3.text = ((uint)this.GetHostPlayerCachedEquipBuyPrice(equipInfo.m_equipID).m_buyPrice).ToString();
						CUIMiniEventScript component4 = widget.GetComponent<CUIMiniEventScript>();
						stUIEventParams eventParams = default(stUIEventParams);
						eventParams.battleEquipPar.equipInfo = equipInfo;
						eventParams.battleEquipPar.m_indexInQuicklyBuyList = i;
						component4.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_RecommendEquip_Buy, eventParams);
					}
					else
					{
						widget.CustomSetActive(false);
					}
				}
			}
			if (isQuicklyBuyEquipListChanged)
			{
				if (!GameSettings.ShowEquipInfo)
				{
					this.SetQuicklyBuyInfoPanelAlpha(0f);
				}
				else
				{
					this.m_tickFadeTime = 10000;
					this.m_bPlayAnimation = false;
					this.SetQuicklyBuyInfoPanelAlpha(1f);
				}
			}
		}

		private void SetQuicklyBuyInfoPanelAlpha(float alphaValue)
		{
			if (null == this.m_battleFormScript)
			{
				return;
			}
			int num = 0;
			while ((long)num < 2L)
			{
				GameObject widget = this.m_battleFormScript.GetWidget(47 + num);
				if (widget != null && widget.activeSelf)
				{
					Transform transform = widget.transform.Find("Panel_Info");
					if (transform != null)
					{
						CanvasGroup component = transform.GetComponent<CanvasGroup>();
						if (component != null)
						{
							component.alpha = alphaValue;
						}
					}
				}
				num++;
			}
		}

		public void CloseEquipFormRightPanel()
		{
			if (null == this.m_equipFormScript)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(6);
			if (widget != null)
			{
				widget.CustomSetActive(false);
			}
			GameObject widget2 = this.m_equipFormScript.GetWidget(7);
			if (widget2 != null)
			{
				widget2.CustomSetActive(false);
			}
			GameObject widget3 = this.m_equipFormScript.GetWidget(5);
			if (widget3 != null)
			{
				widget3.CustomSetActive(false);
			}
		}

		private void RefreshEquipLevelPanel(Transform equipPanel, int level, bool onlyRefreshPriceAndOwned)
		{
			if (null == equipPanel)
			{
				return;
			}
			List<ushort> list = this.m_equipList[(int)this.m_curEquipUsage][level - 1];
			int count = list.Count;
			for (int i = 0; i < 12; i++)
			{
				Transform equipItem = this.GetEquipItem(level, i);
				if (!(equipItem == null))
				{
					if (i < count)
					{
						if (!onlyRefreshPriceAndOwned)
						{
							equipItem.gameObject.CustomSetActive(true);
						}
						this.RefreshEquipItem(equipItem, list[i], onlyRefreshPriceAndOwned);
					}
					else if (!onlyRefreshPriceAndOwned)
					{
						equipItem.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		private void RefreshEquipItem(Transform equipItem, ushort equipID, bool onlyRefreshPriceAndOwned)
		{
			if (null == equipItem)
			{
				return;
			}
			CEquipInfo equipInfo = this.GetEquipInfo(equipID);
			if (equipInfo == null)
			{
				DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = {0}", new object[]
				{
					equipID
				});
				return;
			}
			if (!this.m_hostCtrlHero)
			{
				DebugHelper.Assert(this.m_hostCtrlHero, " RefreshEquipItem m_hostCtrlHero is null");
				return;
			}
			ResEquipInBattle resEquipInBattle = equipInfo.m_resEquipInBattle;
			if (resEquipInBattle != null)
			{
				Image component = equipItem.Find("imgIcon").GetComponent<Image>();
				if (!onlyRefreshPriceAndOwned)
				{
					component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
				}
				CBattleEquipSystem.CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
				Text component2 = equipItem.Find("priceText").GetComponent<Text>();
				bool flag;
				bool flag2;
				if (equipInfo.m_resEquipInBattle.bUsage == 6)
				{
					int num = 0;
					flag = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref num);
					flag2 = this.IsHorizonEquipOwn(equipInfo.m_equipID, ref this.m_hostCtrlHero);
					component2.text = num.ToString();
				}
				else
				{
					flag = this.IsEquipCanBought(equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
					flag2 = this.m_hostCtrlHero.handle.EquipComponent.HasEquip(equipInfo.m_equipID, 1u);
					component2.text = freeEquipBuyPrice.m_buyPrice.ToUintString();
				}
				component.color = ((!flag && !flag2) ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White);
				Transform transform = equipItem.Find("imgOwn");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(flag2);
				}
				if (!onlyRefreshPriceAndOwned)
				{
					Text component3 = equipItem.Find("nameText").GetComponent<Text>();
					component3.text = equipInfo.m_equipName;
					CUIMiniEventWithDragScript component4 = equipItem.GetComponent<CUIMiniEventWithDragScript>();
					if (component4 != null)
					{
						component4.m_onClickEventID = enUIEventID.BattleEquip_Item_Select;
						component4.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
					}
				}
				Transform transform2 = equipItem.FindChild("imgActiveEquip");
				Transform transform3 = equipItem.FindChild("imgEyeEquip");
				if (transform3 && transform2)
				{
					if (equipInfo.m_resEquipInBattle.dwActiveSkillID > 0u)
					{
						if (equipInfo.m_resEquipInBattle.bUsage == 6)
						{
							transform3.gameObject.CustomSetActive(true);
						}
						else
						{
							transform2.gameObject.CustomSetActive(true);
						}
					}
					else
					{
						transform2.gameObject.CustomSetActive(false);
						transform3.gameObject.CustomSetActive(false);
					}
				}
				freeEquipBuyPrice.m_used = false;
			}
		}

		private void RefreshEquipInfoPanel(CEquipInfo equipInfo, Transform infoPanel)
		{
			if (null == infoPanel || equipInfo == null)
			{
				return;
			}
			Text component = infoPanel.Find("equipNameText").GetComponent<Text>();
			component.text = equipInfo.m_equipName;
			Text component2 = infoPanel.Find("Panel_equipPropertyDesc/equipPropertyDescText").GetComponent<Text>();
			component2.text = equipInfo.m_equipPropertyDesc;
			RectTransform rectTransform = component2.transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(0f, 0f);
		}

		private void RefreshEquipBuyPanel(CEquipInfo equipInfo, Transform buyPanel)
		{
			if (null == buyPanel || equipInfo == null)
			{
				return;
			}
			CBattleEquipSystem.CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
			Text component = buyPanel.Find("buyPriceText").GetComponent<Text>();
			bool flag;
			if (equipInfo.m_resEquipInBattle.bUsage == 6)
			{
				int num = 0;
				flag = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref num);
				component.text = num.ToString();
			}
			else
			{
				flag = this.IsEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
				component.text = freeEquipBuyPrice.m_buyPrice.ToString();
			}
			Button component2 = buyPanel.Find("buyBtn").GetComponent<Button>();
			CUICommonSystem.SetButtonEnableWithShader(component2, flag && this.m_hostCtrlHeroPermitedToBuyEquip, true);
			CUIEventScript component3 = buyPanel.Find("buyBtn").GetComponent<CUIEventScript>();
			if (flag && this.m_hostCtrlHeroPermitedToBuyEquip)
			{
				component3.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_BuyBtn_Click, new stUIEventParams
				{
					commonUInt16Param1 = equipInfo.m_equipID
				});
			}
			GameObject gameObject = component2.transform.FindChild("Text").gameObject;
			GameObject gameObject2 = component2.transform.FindChild("CantBuyText").gameObject;
			gameObject.CustomSetActive(this.m_hostCtrlHeroPermitedToBuyEquip);
			gameObject2.CustomSetActive(!this.m_hostCtrlHeroPermitedToBuyEquip);
			freeEquipBuyPrice.m_used = false;
		}

		private void RefreshEquipSalePanel(CEquipInfo equipInfo, int positionInBag, Transform salePanel)
		{
			if (null == salePanel || equipInfo == null)
			{
				return;
			}
			Text component = salePanel.Find("salePriceText").GetComponent<Text>();
			component.text = equipInfo.m_resEquipInBattle.dwSalePrice.ToString();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.selectIndex = positionInBag;
			Button component2 = salePanel.Find("saleBtn").GetComponent<Button>();
			CUICommonSystem.SetButtonEnableWithShader(component2, positionInBag != CBattleEquipSystem.s_horizon_Pos && this.m_hostCtrlHeroPermitedToBuyEquip, true);
			if (positionInBag != CBattleEquipSystem.s_horizon_Pos && this.m_hostCtrlHeroPermitedToBuyEquip)
			{
				CUIEventScript component3 = salePanel.Find("saleBtn").GetComponent<CUIEventScript>();
				component3.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_SaleBtn_Click, eventParams);
			}
		}

		private void InitEquipLevelPanel()
		{
			this.m_equipItemList.Clear();
			for (int i = 0; i < 3; i++)
			{
				GameObject widget = this.m_equipFormScript.GetWidget(1 + i);
				if (widget != null)
				{
					Transform transform = widget.transform;
					for (int j = 0; j < 12; j++)
					{
						Transform transform2 = transform.Find(string.Format("equipItem{0}", j));
						this.m_equipItemList.Add(transform2);
						if (transform2 != null)
						{
							transform2.gameObject.CustomSetActive(true);
						}
					}
				}
			}
		}

		private Transform GetEquipItem(int level, int index)
		{
			index = (level - 1) * 12 + index;
			if (index >= 0 && index < this.m_equipItemList.Count)
			{
				return this.m_equipItemList[index];
			}
			return null;
		}

		private void InitBagEquipItemList()
		{
			this.m_bagEquipItemList.Clear();
			GameObject widget = this.m_equipFormScript.GetWidget(4);
			if (null == widget)
			{
				return;
			}
			Transform transform = widget.transform;
			for (int i = 0; i < 6; i++)
			{
				Transform item = transform.Find(string.Format("equipItem{0}", i));
				this.m_bagEquipItemList.Add(item);
			}
		}

		private Transform GetBagEquipItem(int index)
		{
			if (index >= 0 && index < this.m_bagEquipItemList.Count)
			{
				return this.m_bagEquipItemList[index];
			}
			return null;
		}

		private void InitEquipPathLine()
		{
			if (this.m_equipFormScript != null)
			{
				this.m_equipRelationPath.Clear();
				GameObject widget = this.m_equipFormScript.GetWidget(8);
				Transform transform = widget.transform;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 11; j++)
					{
						Transform transform2 = transform.Find(string.Format("imgLine_{0}_{1}", i, j));
						this.m_equipRelationPath.InitializeVerticalLine(i, j, j + 1, transform2.gameObject);
					}
				}
				GameObject widget2 = this.m_equipFormScript.GetWidget(1);
				this.InitEquipItemHorizontalLine(widget2.transform, 1);
				GameObject widget3 = this.m_equipFormScript.GetWidget(2);
				this.InitEquipItemHorizontalLine(widget3.transform, 2);
				GameObject widget4 = this.m_equipFormScript.GetWidget(3);
				this.InitEquipItemHorizontalLine(widget4.transform, 3);
			}
		}

		private void InitEquipItemHorizontalLine(Transform equipPanel, int level)
		{
			if (null == equipPanel)
			{
				return;
			}
			for (int i = 0; i < 12; i++)
			{
				Transform equipItem = this.GetEquipItem(level, i);
				if (!(null == equipItem))
				{
					Transform transform = equipItem.Find("imgLineFront");
					if (level > 1)
					{
						int index = (level <= 2) ? 0 : 1;
						this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Right, transform.gameObject);
					}
					Transform transform2 = equipItem.Find("imgLineBack");
					if (level < 3)
					{
						int index = (level >= 2) ? 1 : 0;
						this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Left, transform2.gameObject);
					}
				}
			}
		}

		public void OnEquipFormOpen(CUIEvent uiEvent)
		{
			this.m_equipFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBattleEquipSystem.s_equipFormPath, true, true);
			this.SendInOutEquipShopFrameCommand(1);
			if (this.m_equipFormScript != null)
			{
				if (this.m_uiEquipItemHeight == 0f)
				{
					GameObject widget = this.m_equipFormScript.GetWidget(10);
					if (widget != null)
					{
						this.m_uiEquipItemHeight = (widget.transform as RectTransform).rect.height;
					}
				}
				if (this.m_uiEquipItemContentDefaultHeight == 0f)
				{
					GameObject widget2 = this.m_equipFormScript.GetWidget(11);
					if (widget2 != null)
					{
						this.m_uiEquipItemContentDefaultHeight = (widget2.transform as RectTransform).rect.height;
					}
				}
				GameObject widget3 = this.m_equipFormScript.GetWidget(15);
				if (widget3 != null)
				{
					this.m_backEquipListScript = widget3.GetComponent<CUIListScript>();
				}
				this.InitEquipLevelPanel();
				this.InitBagEquipItemList();
				this.InitEquipPathLine();
				this.RefreshGoldCoin();
				CBattleEquipSystem.stSelectedEquipItem[] array;
				CBattleEquipSystem.enSelectedEquipOrigin enSelectedEquipOrigin;
				this.GetEquipSelectData(out array, out enSelectedEquipOrigin);
				bool isEquipTreeOpened = this.m_isEquipTreeOpened;
				CEquipInfo rootEquipInTree = this.m_rootEquipInTree;
				this.RefreshRcmdEquipPlanPanel();
				GameObject widget4 = this.m_equipFormScript.GetWidget(0);
				CTextManager instance = Singleton<CTextManager>.GetInstance();
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext.m_bEnableShopHorizonTab)
				{
					string[] titleList = new string[]
					{
						instance.GetText("Equip_Usage_Recommend"),
						instance.GetText("Equip_Usage_PhyAttack"),
						instance.GetText("Equip_Usage_MagicAttack"),
						instance.GetText("Equip_Usage_Defence"),
						instance.GetText("Equip_Usage_Move"),
						instance.GetText("Equip_Usage_Jungle"),
						instance.GetText("Equip_Usage_Horizon")
					};
					CUICommonSystem.InitMenuPanel(widget4, titleList, (int)this.m_curEquipUsage, true);
				}
				else
				{
					string[] titleList2 = new string[]
					{
						instance.GetText("Equip_Usage_Recommend"),
						instance.GetText("Equip_Usage_PhyAttack"),
						instance.GetText("Equip_Usage_MagicAttack"),
						instance.GetText("Equip_Usage_Defence"),
						instance.GetText("Equip_Usage_Move"),
						instance.GetText("Equip_Usage_Jungle")
					};
					CUICommonSystem.InitMenuPanel(widget4, titleList2, (int)this.m_curEquipUsage, true);
				}
				this.m_isEquipTreeOpened = isEquipTreeOpened;
				this.m_rootEquipInTree = rootEquipInTree;
				this.SetEquipSelectData(ref array, ref enSelectedEquipOrigin);
				if (!this.m_isEquipTreeOpened)
				{
					this.CloseEquipTreePanel();
				}
				else if (this.m_isEquipTreeOpened && this.m_rootEquipInTree != null)
				{
					this.OpenEquipTreePanel(this.m_rootEquipInTree, false);
				}
				this.RefreshEquipBagPanel();
				if (this.m_selectedEquipOrigin != CBattleEquipSystem.enSelectedEquipOrigin.None)
				{
					CBattleEquipSystem.stSelectedEquipItem stSelectedEquipItem = this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin];
					if (stSelectedEquipItem.m_equipInfo != null && stSelectedEquipItem.m_equipItemTransform != null)
					{
						this.SetSelectedEquipItem(this.m_selectedEquipOrigin, stSelectedEquipItem.m_equipInfo, stSelectedEquipItem.m_equipItemTransform, stSelectedEquipItem.m_positionInBag, true);
						if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipTree && this.m_selectedEquipItems[0].m_equipInfo != null && this.m_selectedEquipItems[0].m_equipItemTransform != null)
						{
							this.SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray, this.m_selectedEquipItems[0].m_equipInfo, this.m_selectedEquipItems[0].m_equipItemTransform, -1, true);
						}
						this.SetFocusEquipInEquipLibrary(stSelectedEquipItem.m_equipInfo, this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipBag);
					}
					bool enabled = this.m_selectedEquipItems[2].m_equipInfo != null || this.m_selectedEquipItems[0].m_equipInfo != null;
					this.EnableOpenEquipTreeButton(enabled);
				}
				this.RefreshEquipFormRightPanel(false);
				this.InitEquipActiveSkillTimerTxt();
				this.UpdateEquipActiveSkillCd(true);
			}
			Singleton<CUIParticleSystem>.instance.Hide(null);
			if (uiEvent != null)
			{
				Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(this.m_equipFormScript.GetWidget(20), enNewFlagKey.New_Battle_CustomEquip_V14, enNewFlagPos.enTopRight, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
			}
		}

		private void GetEquipSelectData(out CBattleEquipSystem.stSelectedEquipItem[] selEquipItem, out CBattleEquipSystem.enSelectedEquipOrigin selEquipOrigin)
		{
			selEquipItem = new CBattleEquipSystem.stSelectedEquipItem[3];
			for (int i = 0; i < 3; i++)
			{
				selEquipItem[i] = this.m_selectedEquipItems[i];
			}
			selEquipOrigin = this.m_selectedEquipOrigin;
		}

		private void SetEquipSelectData(ref CBattleEquipSystem.stSelectedEquipItem[] selEquipItem, ref CBattleEquipSystem.enSelectedEquipOrigin selEquipOrigin)
		{
			if (selEquipItem == null)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				this.m_selectedEquipItems[i] = selEquipItem[i];
			}
			this.m_selectedEquipOrigin = selEquipOrigin;
		}

		private void RefreshRcmdEquipPlanPanel()
		{
			if (this.m_equipFormScript == null || !this.m_hostCtrlHero)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(20);
			if (widget != null)
			{
				widget.transform.Find("Text").GetComponent<Text>().text = this.m_hostCtrlHero.handle.EquipComponent.GetCurRcmdEquipPlanName();
			}
		}

		public void OnRcmdEquipPlanChangeSuccess()
		{
			if (this.m_equipFormScript == null)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(20);
			if (widget != null)
			{
				CUICommonSystem.PlayAnimator(widget, "EquipChange_Anim");
			}
		}

		public void OnEquipFormClose(CUIEvent uiEvent)
		{
			this.SendInOutEquipShopFrameCommand(0);
			this.EnableOpenEquipTreeButton(false);
			this.m_equipFormScript = null;
			this.m_backEquipListScript = null;
			Singleton<CUIParticleSystem>.instance.Show(null);
		}

		public void OnEquipTypeListSelect(CUIEvent uiEvent)
		{
			if (this.m_isEquipTreeOpened)
			{
				this.CloseEquipTreePanel();
			}
			this.ClearSelectedEquipItem();
			this.CloseEquipFormRightPanel();
			CUIListScript cUIListScript = (CUIListScript)uiEvent.m_srcWidgetScript;
			if (cUIListScript != null)
			{
				this.m_curEquipUsage = (enEquipUsage)cUIListScript.GetSelectedIndex();
				this.RefreshEquipLibraryPanel(false);
			}
			if (this.m_equipFormScript != null)
			{
				this.m_equipFormScript.GetWidget(18).CustomSetActive(this.m_curEquipUsage == enEquipUsage.Horizon);
			}
		}

		public void OnEquipItemSelect(CUIEvent uiEvent)
		{
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo == uiEvent.m_eventParams.battleEquipPar.equipInfo && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipItemTransform == uiEvent.m_srcWidget.transform)
			{
				return;
			}
			this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree);
			this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipBag);
			this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray;
			this.SetFocusEquipInEquipLibrary(uiEvent.m_eventParams.battleEquipPar.equipInfo, false);
			this.SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray, uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_srcWidget.transform, -1, false);
			this.RefreshEquipFormRightPanel(false);
			this.EnableOpenEquipTreeButton(true);
		}

		public void OnEquipBagItemSelect(CUIEvent uiEvent)
		{
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipBag && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo != null && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo.m_equipID == uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipItemTransform == uiEvent.m_srcWidget.transform)
			{
				return;
			}
			this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray);
			this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree);
			this.SetFocusEquipInEquipLibrary(uiEvent.m_eventParams.battleEquipPar.equipInfo, true);
			this.SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipBag, uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_srcWidget.transform, uiEvent.m_eventParams.battleEquipPar.pos, false);
			this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.EquipBag;
			if (this.m_isEquipTreeOpened)
			{
				this.m_equipTree.Create(uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID, this.m_equipInfoDictionary);
				this.m_rootEquipInTree = uiEvent.m_eventParams.battleEquipPar.equipInfo;
				this.RefreshEquipTreePanel(false);
			}
			this.RefreshEquipFormRightPanel(false);
			this.EnableOpenEquipTreeButton(true);
		}

		public void OnEquipBuyBtnClick(CUIEvent uiEvent)
		{
			CEquipInfo equipInfo = this.GetEquipInfo(uiEvent.m_eventParams.commonUInt16Param1);
			if (equipInfo != null)
			{
				if (equipInfo.m_resEquipInBattle.bUsage == 6)
				{
					this.SendBuyHorizonEquipFrameCommand(equipInfo.m_equipID);
				}
				else
				{
					this.SendBuyEquipFrameCommand(equipInfo.m_equipID, false);
				}
			}
			this.ClearSelectedEquipItem();
			this.CloseEquipFormRightPanel();
		}

		public void OnEquipSaleBtnClick(CUIEvent uiEvent)
		{
			this.SendSellEquipFrameCommand(uiEvent.m_eventParams.selectIndex);
			this.ClearSelectedEquipItem();
			this.CloseEquipFormRightPanel();
		}

		public void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
		{
			if (uiEvent.m_eventParams.battleEquipPar.equipInfo != null)
			{
				this.SendBuyEquipFrameCommand(uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID, true);
			}
		}

		public void OnBattleEquipOpenEquipTree(CUIEvent uiEvent)
		{
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray || this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipBag)
			{
				CEquipInfo equipInfo = this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo;
				if (equipInfo != null)
				{
					this.OpenEquipTreePanel(equipInfo, this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray);
				}
			}
		}

		public void OnBattleEquipCloseEquipTree(CUIEvent uiEvent)
		{
			this.CloseEquipTreePanel();
			this.RefreshEquipLibraryPanel(false);
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipTree || this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.None)
			{
				this.ClearSelectedEquipItem();
				this.CloseEquipFormRightPanel();
			}
			else
			{
				this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree);
				this.RefreshEquipFormRightPanel(false);
				this.SetFocusEquipInEquipLibrary(this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo, false);
			}
		}

		public void OnBattleEquipSelectItemInEquipTree(CUIEvent uiEvent)
		{
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipBag)
			{
				this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipBag);
			}
			else if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipTree)
			{
				CEquipInfo equipInfo = this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo;
				if (equipInfo != null && equipInfo.m_equipID == uiEvent.m_eventParams.commonUInt16Param1 && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipItemTransform == uiEvent.m_srcWidget.transform)
				{
					return;
				}
			}
			this.SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree, this.GetEquipInfo(uiEvent.m_eventParams.commonUInt16Param1), uiEvent.m_srcWidget.transform, -1, false);
			this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.EquipTree;
			this.RefreshBackEquipList(false);
			this.RefreshEquipFormRightPanel(false);
		}

		public void OnBattleEquipBackEquipListSelectedChanged(CUIEvent uiEvent)
		{
			List<ushort> list = null;
			int num = 0;
			if (this.m_selectedEquipOrigin != CBattleEquipSystem.enSelectedEquipOrigin.None && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo != null)
			{
				list = this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo.m_backEquipIDs;
				num = ((list != null) ? list.Count : 0);
			}
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript != null)
			{
				int selectedIndex = cUIListScript.GetSelectedIndex();
				if (selectedIndex >= 0 && selectedIndex < num)
				{
					ushort num2 = list[selectedIndex];
					this.m_equipTree.Create(num2, this.m_equipInfoDictionary);
					this.m_rootEquipInTree = this.GetEquipInfo(num2);
					GameObject widget = uiEvent.m_srcFormScript.GetWidget(13);
					Transform equipItemTransform = (!(widget != null)) ? null : widget.transform.Find("rootEquipItem");
					this.SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree, this.GetEquipInfo(num2), equipItemTransform, -1, false);
					this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.EquipTree;
					this.RefreshEquipTreePanel(false);
					this.RefreshEquipFormRightPanel(false);
				}
			}
		}

		public void OnBattleEquipBackEquipListScrollChanged(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript != null)
			{
				Vector2 contentSize = cUIListScript.GetContentSize();
				Vector2 scrollAreaSize = cUIListScript.GetScrollAreaSize();
				Vector2 contentPosition = cUIListScript.GetContentPosition();
				GameObject widget = uiEvent.m_srcFormScript.GetWidget(16);
				GameObject widget2 = uiEvent.m_srcFormScript.GetWidget(17);
				if (cUIListScript.IsNeedScroll())
				{
					if (contentPosition.x >= 0f)
					{
						widget.CustomSetActive(false);
						widget2.CustomSetActive(true);
					}
					else if (contentPosition.x + contentSize.x <= scrollAreaSize.x)
					{
						widget.CustomSetActive(true);
						widget2.CustomSetActive(false);
					}
					else
					{
						widget.CustomSetActive(true);
						widget2.CustomSetActive(true);
					}
				}
				else
				{
					widget.CustomSetActive(false);
					widget2.CustomSetActive(false);
				}
			}
		}

		public void OnBattleEquipBackEquipListMoreLeftClicked(CUIEvent uiEvent)
		{
			if (this.m_backEquipListScript != null)
			{
				this.m_backEquipListScript.MoveContent(new Vector2(this.m_backEquipListScript.GetEelementDefaultSize().x, 0f));
			}
		}

		public void OnBattleEquipOpenSelfRcmEuipPlanForm(CUIEvent uiEvent)
		{
			if (!this.m_hostCtrlHero)
			{
				return;
			}
			uint configId = (uint)this.m_hostCtrlHero.handle.TheStaticData.TheActorMeta.ConfigId;
			stRcmdEquipListInfo stRcmdEquipListInfo = this.m_hostCtrlHero.handle.EquipComponent.ConvertRcmdEquipListInfo();
			CEquipSystem.OpenSelfEquipPlanForm(configId, ref stRcmdEquipListInfo, enUIEventID.BattleEquip_ChangeSelfRcmEuipPlan, false, false);
			Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.m_equipFormScript.GetWidget(20), enNewFlagKey.New_Battle_CustomEquip_V14, true);
		}

		public void OnBattleEquipChangeSelfRcmEuipPlan(CUIEvent uiEvent)
		{
			this.SendChangeUsedRecommendEquipGroupCommand((byte)uiEvent.m_eventParams.tagUInt);
		}

		public void OnBattleEquipBackEquipListMoreRightClicked(CUIEvent uiEvent)
		{
			if (this.m_backEquipListScript != null)
			{
				this.m_backEquipListScript.MoveContent(new Vector2(this.m_backEquipListScript.GetEelementDefaultSize().x * -1f, 0f));
			}
		}

		public void OnActorGoldChange(int changeValue, int currentValue)
		{
			this.SetEquipChangeFlag(CBattleEquipSystem.enEquipChangeFlag.GoldCoinInBattleChanged);
		}

		private void RefreshHostPlayerQuicklyBuyEquipPanel(bool hostCtrlHeroPermitedToBuyEquip)
		{
			CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
			if (fightFormScript == null)
			{
				return;
			}
			GameObject widget = fightFormScript.GetWidget(58);
			if (widget != null)
			{
				Image component = widget.GetComponent<Image>();
				if (component != null)
				{
					component.color = new Color((float)((!hostCtrlHeroPermitedToBuyEquip) ? 0 : 1), 1f, 1f, 1f);
				}
			}
			GameObject widget2 = fightFormScript.GetWidget(59);
			if (widget2 != null)
			{
				widget2.CustomSetActive(hostCtrlHeroPermitedToBuyEquip);
			}
		}

		private void SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin equipSelectedOrigin, CEquipInfo equipInfo, Transform equipItemTransform, int positionInBag, bool bForceSelect = false)
		{
			Transform equipItemTransform2 = this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipItemTransform;
			bool bIsActiveSKillEquip = false;
			if (this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo != null && this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo.m_resEquipInBattle != null && this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo.m_resEquipInBattle.dwActiveSkillID > 0u)
			{
				bIsActiveSKillEquip = true;
			}
			this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo = equipInfo;
			this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipItemTransform = equipItemTransform;
			this.m_selectedEquipItems[(int)equipSelectedOrigin].m_positionInBag = positionInBag;
			bool bIsActiveSKillEquip2 = false;
			if (equipInfo != null)
			{
				bIsActiveSKillEquip2 = this.IsActiveEquipButNotHorizon(equipInfo.m_equipID);
			}
			if (bForceSelect || equipItemTransform2 != equipItemTransform)
			{
				if (equipItemTransform2 != null)
				{
					this.SetEquipItemSelectFlag(equipItemTransform2, false, equipSelectedOrigin, bIsActiveSKillEquip);
				}
				if (equipItemTransform != null)
				{
					this.SetEquipItemSelectFlag(equipItemTransform, true, equipSelectedOrigin, bIsActiveSKillEquip2);
				}
			}
		}

		private bool IsActiveEquipButNotHorizon(ushort equipId)
		{
			CEquipInfo equipInfo = this.GetEquipInfo(equipId);
			return equipInfo != null && equipInfo.m_resEquipInBattle.dwActiveSkillID > 0u && equipInfo.m_resEquipInBattle.bUsage != 6;
		}

		private void ClearSelectedEquipItem()
		{
			for (int i = 0; i < 3; i++)
			{
				this.ClearSelectedEquipItem((CBattleEquipSystem.enSelectedEquipOrigin)i);
			}
			this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.None;
			if (this.m_equipRelationPath != null)
			{
				this.m_equipRelationPath.Reset();
			}
			this.EnableOpenEquipTreeButton(false);
		}

		private void ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin equipSelectedOrigin)
		{
			Transform equipItemTransform = this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipItemTransform;
			bool bIsActiveSKillEquip = false;
			if (this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo != null && this.IsActiveEquipButNotHorizon(this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo.m_equipID))
			{
				bIsActiveSKillEquip = true;
			}
			this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipInfo = null;
			this.m_selectedEquipItems[(int)equipSelectedOrigin].m_equipItemTransform = null;
			this.m_selectedEquipItems[(int)equipSelectedOrigin].m_positionInBag = -1;
			if (equipItemTransform != null)
			{
				this.SetEquipItemSelectFlag(equipItemTransform, false, equipSelectedOrigin, bIsActiveSKillEquip);
			}
		}

		private void SetFocusEquipInEquipLibrary(CEquipInfo equipInfo, bool switchUsage)
		{
			if (equipInfo == null || this.m_equipFormScript == null)
			{
				return;
			}
			if (switchUsage && (enEquipUsage)equipInfo.m_resEquipInBattle.bUsage != this.m_curEquipUsage)
			{
				GameObject widget = this.m_equipFormScript.GetWidget(0);
				if (widget != null)
				{
					CUIListScript component = widget.GetComponent<CUIListScript>();
					if (component != null)
					{
						component.SelectElement((int)equipInfo.m_resEquipInBattle.bUsage, true);
					}
				}
			}
			this.m_equipRelationPath.Display(equipInfo.m_equipID, this.m_equipList[(int)this.m_curEquipUsage], this.m_equipInfoDictionary);
		}

		private void OpenEquipTreePanel(CEquipInfo rootEquipInfo, bool selectRootItem)
		{
			if (this.m_equipFormScript == null)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(12);
			if (widget != null)
			{
				widget.CustomSetActive(false);
			}
			GameObject widget2 = this.m_equipFormScript.GetWidget(13);
			if (widget2 != null)
			{
				widget2.CustomSetActive(true);
			}
			this.m_isEquipTreeOpened = true;
			this.m_rootEquipInTree = rootEquipInfo;
			this.m_equipTree.Create(rootEquipInfo.m_equipID, this.m_equipInfoDictionary);
			if (selectRootItem)
			{
				Transform equipItemTransform = (!(widget2 != null)) ? null : widget2.transform.Find("rootEquipItem");
				this.SetSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree, rootEquipInfo, equipItemTransform, -1, false);
				this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.EquipTree;
			}
			this.RefreshEquipTreePanel(false);
			this.RefreshEquipFormRightPanel(false);
		}

		private void CloseEquipTreePanel()
		{
			if (this.m_equipFormScript == null)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(12);
			if (widget != null)
			{
				widget.CustomSetActive(true);
			}
			GameObject widget2 = this.m_equipFormScript.GetWidget(13);
			if (widget2 != null)
			{
				widget2.CustomSetActive(false);
			}
			this.m_isEquipTreeOpened = false;
			this.m_rootEquipInTree = null;
			this.m_equipTree.Clear();
			this.ClearSelectedEquipItem(CBattleEquipSystem.enSelectedEquipOrigin.EquipTree);
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipTree)
			{
				if (this.m_selectedEquipItems[0].m_equipInfo != null)
				{
					this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.EquipLibaray;
				}
				else
				{
					this.m_selectedEquipOrigin = CBattleEquipSystem.enSelectedEquipOrigin.None;
				}
			}
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.None)
			{
				this.CloseEquipFormRightPanel();
			}
			else
			{
				this.RefreshEquipFormRightPanel(false);
			}
		}

		private void RefreshEquipFormRightPanel(bool onlyRefreshPrice)
		{
			if (this.m_equipFormScript == null)
			{
				return;
			}
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.None || this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo == null)
			{
				this.CloseEquipFormRightPanel();
				return;
			}
			CEquipInfo equipInfo = this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo;
			if (!onlyRefreshPrice)
			{
				GameObject widget = this.m_equipFormScript.GetWidget(5);
				widget.CustomSetActive(true);
				this.RefreshEquipInfoPanel(equipInfo, widget.transform);
			}
			if (this.m_selectedEquipOrigin == CBattleEquipSystem.enSelectedEquipOrigin.EquipBag)
			{
				GameObject widget2 = this.m_equipFormScript.GetWidget(6);
				widget2.CustomSetActive(false);
				GameObject widget3 = this.m_equipFormScript.GetWidget(7);
				widget3.CustomSetActive(true);
				if (!onlyRefreshPrice)
				{
					this.RefreshEquipSalePanel(equipInfo, this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_positionInBag, widget3.transform);
				}
			}
			else
			{
				GameObject widget4 = this.m_equipFormScript.GetWidget(7);
				widget4.CustomSetActive(false);
				GameObject widget5 = this.m_equipFormScript.GetWidget(6);
				widget5.CustomSetActive(true);
				this.RefreshEquipBuyPanel(equipInfo, widget5.transform);
			}
		}

		private void RefreshEquipBagPanel()
		{
			GameObject widget = this.m_equipFormScript.GetWidget(4);
			if (widget != null && this.m_hostCtrlHero)
			{
				stEquipInfo[] equips = this.m_hostCtrlHero.handle.EquipComponent.GetEquips();
				for (int i = 0; i < 6; i++)
				{
					Transform bagEquipItem = this.GetBagEquipItem(i);
					if (!(null == bagEquipItem))
					{
						Image component = bagEquipItem.Find("imgIcon").GetComponent<Image>();
						CUIMiniEventScript component2 = bagEquipItem.GetComponent<CUIMiniEventScript>();
						if (equips[i].m_amount >= 1u)
						{
							component2.enabled = true;
							component.gameObject.CustomSetActive(true);
							CEquipInfo equipInfo = this.GetEquipInfo(equips[i].m_equipID);
							if (equipInfo != null)
							{
								component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
								if (this.IsActiveEquipButNotHorizon(equipInfo.m_equipID))
								{
									Transform transform = bagEquipItem.FindChild("imgActiveEquip");
									if (transform && !transform.gameObject.activeSelf)
									{
										transform.gameObject.CustomSetActive(true);
									}
								}
								component2.m_onClickEventID = enUIEventID.BattleEquip_BagItem_Select;
								component2.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
								component2.m_onClickEventParams.battleEquipPar.pos = i;
							}
						}
						else
						{
							component2.enabled = false;
							component.gameObject.CustomSetActive(false);
							Transform transform2 = bagEquipItem.FindChild("imgActiveEquip");
							if (transform2 && transform2.gameObject.activeSelf)
							{
								transform2.gameObject.CustomSetActive(false);
							}
						}
					}
				}
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				GameObject widget2 = this.m_equipFormScript.GetWidget(19);
				if (curLvelContext != null && widget2 != null)
				{
					if (curLvelContext.m_bEnableOrnamentSlot && curLvelContext.m_bEnableShopHorizonTab)
					{
						widget2.CustomSetActive(true);
						this.RefreshHorizonEquipGrid(widget2);
					}
					else
					{
						widget2.CustomSetActive(false);
					}
				}
			}
		}

		private void RefreshHorizonEquipGrid(GameObject horizonEquipGrid)
		{
			if (null == horizonEquipGrid || !this.m_hostCtrlHero)
			{
				return;
			}
			Transform transform = horizonEquipGrid.transform;
			Image component = transform.Find("imgIcon").GetComponent<Image>();
			CUIMiniEventScript component2 = transform.GetComponent<CUIMiniEventScript>();
			CEquipInfo equipInfo = this.GetEquipInfo(this.m_hostCtrlHero.handle.EquipComponent.m_horizonEquipId);
			if (equipInfo != null)
			{
				component.gameObject.CustomSetActive(true);
				component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
				component2.enabled = true;
				component2.m_onClickEventID = enUIEventID.BattleEquip_BagItem_Select;
				component2.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
				component2.m_onClickEventParams.battleEquipPar.pos = CBattleEquipSystem.s_horizon_Pos;
			}
			else
			{
				component.gameObject.CustomSetActive(false);
				component2.enabled = false;
			}
		}

		private void RefreshEquipLibraryPanel(bool onlyRefreshPriceAndOwned)
		{
			if (this.m_equipFormScript == null || !this.m_hostCtrlHero)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(1);
			if (widget != null)
			{
				this.RefreshEquipLevelPanel(widget.transform, 1, onlyRefreshPriceAndOwned);
			}
			GameObject widget2 = this.m_equipFormScript.GetWidget(2);
			if (widget2 != null)
			{
				this.RefreshEquipLevelPanel(widget2.transform, 2, onlyRefreshPriceAndOwned);
			}
			GameObject widget3 = this.m_equipFormScript.GetWidget(3);
			if (widget3 != null)
			{
				this.RefreshEquipLevelPanel(widget3.transform, 3, onlyRefreshPriceAndOwned);
			}
			if (!onlyRefreshPriceAndOwned && this.m_equipList != null)
			{
				int num = 0;
				List<ushort>[] array = this.m_equipList[(int)this.m_curEquipUsage];
				if (array == null)
				{
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Count > num)
					{
						num = array[i].Count;
					}
				}
				float num2 = this.m_uiEquipItemContentDefaultHeight - (float)(12 - num) * this.m_uiEquipItemHeight;
				GameObject widget4 = this.m_equipFormScript.GetWidget(11);
				if (widget4 != null)
				{
					RectTransform rectTransform = widget4.transform as RectTransform;
					rectTransform.offsetMin = new Vector2(0f, -num2);
					rectTransform.offsetMax = new Vector2(0f, 0f);
				}
			}
		}

		private void RefreshGoldCoin()
		{
			if (this.m_equipFormScript == null || !this.m_hostCtrlHero)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(9);
			if (widget != null)
			{
				widget.GetComponent<Text>().text = this.m_hostCtrlHero.handle.ValueComponent.GetGoldCoinInBattle().ToString();
			}
		}

		private void RefreshEquipTreePanel(bool onlyRefreshPriceAndOwned)
		{
			if (!this.m_isEquipTreeOpened || this.m_equipFormScript == null)
			{
				return;
			}
			GameObject widget = this.m_equipFormScript.GetWidget(13);
			if (widget == null)
			{
				return;
			}
			Transform equipItemTransform = widget.transform.Find("rootEquipItem");
			this.RefreshEquipTreeItem(equipItemTransform, this.m_equipTree.m_rootEquipID, onlyRefreshPriceAndOwned);
			Transform lineGroupPanel = widget.transform.Find("lineGroupPanel");
			this.RefreshLineGroupPanel(lineGroupPanel, 3, (int)this.m_equipTree.m_2ndEquipCount);
			Transform transform = widget.transform.Find("preEquipGroupPanel");
			if (null == transform)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				ushort num = this.m_equipTree.m_2ndEquipIDs[i];
				Transform transform2 = transform.Find("preEquipGroup" + i);
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(num > 0);
					if (num > 0)
					{
						Transform equipItemTransform2 = transform2.Find("2ndEquipItem");
						this.RefreshEquipTreeItem(equipItemTransform2, num, onlyRefreshPriceAndOwned);
						lineGroupPanel = transform2.transform.Find("lineGroupPanel");
						this.RefreshLineGroupPanel(lineGroupPanel, 2, (int)this.m_equipTree.m_3rdEquipCounts[i]);
						for (int j = 0; j < 2; j++)
						{
							ushort num2 = this.m_equipTree.m_3rdEquipIDs[i][j];
							Transform transform3 = transform2.Find("preEquipPanel/3rdEquipItem" + j);
							transform3.gameObject.CustomSetActive(num2 > 0);
							this.RefreshEquipTreeItem(transform3, num2, onlyRefreshPriceAndOwned);
						}
					}
				}
			}
			this.RefreshBackEquipList(onlyRefreshPriceAndOwned);
		}

		private void RefreshBackEquipList(bool onlyRefreshPriceAndOwned)
		{
			if (!this.m_isEquipTreeOpened || this.m_equipFormScript == null)
			{
				return;
			}
			List<ushort> list = null;
			int num = 0;
			if (this.m_selectedEquipOrigin != CBattleEquipSystem.enSelectedEquipOrigin.None && this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo != null)
			{
				list = this.m_selectedEquipItems[(int)this.m_selectedEquipOrigin].m_equipInfo.m_backEquipIDs;
				num = ((list != null) ? list.Count : 0);
			}
			if (this.m_backEquipListScript != null)
			{
				if (!onlyRefreshPriceAndOwned || this.m_backEquipListScript.GetElementAmount() != num)
				{
					this.m_backEquipListScript.SetElementAmount(num);
					this.m_backEquipListScript.SelectElement(-1, false);
					this.m_backEquipListScript.ResetContentPosition();
					SGameGraphicRaycaster component = this.m_backEquipListScript.m_belongedFormScript.GetComponent<SGameGraphicRaycaster>();
					if (component != null)
					{
						component.RefreshGameObject(this.m_backEquipListScript.gameObject);
					}
					GameObject widget = this.m_equipFormScript.GetWidget(16);
					GameObject widget2 = this.m_equipFormScript.GetWidget(17);
					if (!this.m_backEquipListScript.IsNeedScroll())
					{
						widget.CustomSetActive(false);
						widget2.CustomSetActive(false);
					}
					else
					{
						widget.CustomSetActive(false);
						widget2.CustomSetActive(true);
					}
				}
				for (int i = 0; i < num; i++)
				{
					this.RefreshEquipTreeItem(this.m_backEquipListScript.GetElemenet(i).transform, list[i], onlyRefreshPriceAndOwned);
				}
			}
		}

		private void RefreshEquipTreeItem(Transform equipItemTransform, ushort equipID, bool onlyRefreshPriceAndOwned)
		{
			if (equipItemTransform == null || equipID == 0)
			{
				return;
			}
			CEquipInfo equipInfo = this.GetEquipInfo(equipID);
			if (equipInfo == null || equipInfo.m_resEquipInBattle == null)
			{
				DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + equipID);
				return;
			}
			Transform transform = equipItemTransform.Find("imgIcon");
			Image image = (!(transform != null)) ? null : transform.gameObject.GetComponent<Image>();
			if (!onlyRefreshPriceAndOwned && image != null)
			{
				image.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
			}
			int num = 0;
			bool flag;
			bool flag2;
			if (equipInfo.m_resEquipInBattle.bUsage == 6)
			{
				flag = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref num);
				flag2 = this.IsHorizonEquipOwn(equipInfo.m_equipID, ref this.m_hostCtrlHero);
			}
			else
			{
				CBattleEquipSystem.CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
				flag = this.IsEquipCanBought(equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
				flag2 = this.m_hostCtrlHero.handle.EquipComponent.HasEquip(equipInfo.m_equipID, 1u);
				num = freeEquipBuyPrice.m_buyPrice;
				freeEquipBuyPrice.m_used = false;
			}
			Transform transform2 = equipItemTransform.Find("Price");
			if (transform2 != null)
			{
				Text component = transform2.gameObject.GetComponent<Text>();
				if (component != null)
				{
					component.text = num.ToString();
				}
			}
			if (image != null)
			{
				image.color = ((!flag && !flag2) ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White);
			}
			Transform transform3 = equipItemTransform.Find("imgOwn");
			if (transform3 != null)
			{
				transform3.gameObject.CustomSetActive(flag2);
			}
			if (!onlyRefreshPriceAndOwned)
			{
				CUIMiniEventScript component2 = equipItemTransform.gameObject.GetComponent<CUIMiniEventScript>();
				if (component2 != null)
				{
					component2.m_onClickEventParams.commonUInt16Param1 = equipID;
				}
			}
			Transform transform4 = equipItemTransform.FindChild("imgActiveEquip");
			Transform transform5 = equipItemTransform.FindChild("imgEyeEquip");
			if (transform5 && transform4)
			{
				if (equipInfo.m_resEquipInBattle.dwActiveSkillID > 0u)
				{
					if (equipInfo.m_resEquipInBattle.bUsage == 6)
					{
						transform5.gameObject.CustomSetActive(true);
					}
					else
					{
						transform4.gameObject.CustomSetActive(true);
					}
				}
				else
				{
					transform4.gameObject.CustomSetActive(false);
					transform5.gameObject.CustomSetActive(false);
				}
			}
		}

		private void RefreshLineGroupPanel(Transform lineGroupPanel, int maxLineCnt, int curLineCnt)
		{
			if (null == lineGroupPanel)
			{
				return;
			}
			for (int i = 0; i < maxLineCnt; i++)
			{
				Transform transform = lineGroupPanel.Find("linePanel" + i);
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(i + 1 == curLineCnt);
				}
			}
		}

		private void EnableOpenEquipTreeButton(bool enabled)
		{
			if (this.m_equipFormScript != null)
			{
				GameObject widget = this.m_equipFormScript.GetWidget(14);
				if (widget != null)
				{
					widget.CustomSetActive(enabled);
				}
			}
		}

		public void ExecChooseEquipSkillCmd(int iEquipSlotIndex, ref PoolObjHandle<ActorRoot> actor)
		{
			if (iEquipSlotIndex < 0 || iEquipSlotIndex >= 6)
			{
				return;
			}
			if (actor && actor.handle.EquipComponent != null)
			{
				ENUM_EQUIP_ACTIVESKILL_STATUS equipActiveSkillSlotInfo = actor.handle.EquipComponent.GetEquipActiveSkillSlotInfo(iEquipSlotIndex);
				if (equipActiveSkillSlotInfo == ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW)
				{
					SkillSlotType showEquipActiveSkillSlot = actor.handle.EquipComponent.GetShowEquipActiveSkillSlot();
					int equipSlotBySkillSlot = actor.handle.EquipComponent.GetEquipSlotBySkillSlot(showEquipActiveSkillSlot);
					if (equipSlotBySkillSlot >= 0 && equipSlotBySkillSlot < 6)
					{
						SkillSlot skillSlot;
						if (actor.handle.SkillControl != null && actor.handle.SkillControl.TryGetSkillSlot(showEquipActiveSkillSlot, out skillSlot))
						{
							int cd = skillSlot.CurSkillCD;
							actor.handle.EquipComponent.AddEquipActiveSkillCdInfo(equipSlotBySkillSlot, cd);
						}
						actor.handle.EquipComponent.SetEquipActiveSkillSlotInfo(equipSlotBySkillSlot, ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW);
						if (actor == this.m_hostCtrlHero)
						{
							this.SetEquipSkillBeUsingBtnState(false, equipSlotBySkillSlot);
						}
					}
					if (actor.handle.EquipComponent.ChangeEquipSkillSlot(showEquipActiveSkillSlot, iEquipSlotIndex))
					{
						actor.handle.EquipComponent.SetEquipActiveSkillSlotInfo(iEquipSlotIndex, ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_ISSHOWING);
						if (actor == this.m_hostCtrlHero)
						{
							if (actor.handle.EquipComponent.GetEquipActiveSkillInitFlag(showEquipActiveSkillSlot) && Singleton<CBattleSystem>.GetInstance().FightForm != null)
							{
								Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(actor, true, showEquipActiveSkillSlot);
							}
							this.SetEquipSkillBtnState(true, showEquipActiveSkillSlot);
							this.SetEquipSkillBeUsingBtnState(true, iEquipSlotIndex);
							this.SetEquipSkillShowBtnState(false, iEquipSlotIndex);
						}
					}
				}
			}
		}

		private void SetEquipSkillShowBtnState(bool bIsBtnShow, int iEquipSlotIndex)
		{
			if (this.m_bagEquipItemList[iEquipSlotIndex])
			{
				Transform transform = this.m_bagEquipItemList[iEquipSlotIndex].FindChild("SelectImg_InBattle");
				if (transform && transform.gameObject.activeSelf != bIsBtnShow)
				{
					transform.gameObject.CustomSetActive(bIsBtnShow);
				}
				Transform transform2 = this.m_bagEquipItemList[iEquipSlotIndex].FindChild("BtnShowInBattle");
				if (transform2 && transform2.gameObject.activeSelf != bIsBtnShow)
				{
					transform2.gameObject.CustomSetActive(bIsBtnShow);
				}
			}
		}

		private void SetEquipSkillBeUsingBtnState(bool bIsBtnShow, int iEquipSlotIndex)
		{
			if (this.m_bagEquipItemList[iEquipSlotIndex])
			{
				Transform transform = this.m_bagEquipItemList[iEquipSlotIndex].FindChild("Beingused");
				if (transform && transform.gameObject.activeSelf != bIsBtnShow)
				{
					transform.gameObject.CustomSetActive(bIsBtnShow);
				}
			}
		}

		private void SetEquipSkillBtnState(bool bIsShow, SkillSlotType slot)
		{
			if (Singleton<CBattleSystem>.GetInstance() == null || Singleton<CBattleSystem>.GetInstance().FightForm == null)
			{
				return;
			}
			CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
			if (skillButtonManager != null)
			{
				SkillButton button = skillButtonManager.GetButton(slot);
				if (button != null && button.m_button != null && button.m_button.activeSelf != bIsShow)
				{
					button.m_button.CustomSetActive(bIsShow);
					if (!bIsShow && button.m_cdText && button.m_cdText.activeSelf != bIsShow)
					{
						button.m_cdText.CustomSetActive(bIsShow);
					}
				}
			}
		}

		private void OnBattleEquipShowOrHideInBattleBtnClicked(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidget != null && uiEvent.m_srcWidget.transform != null && uiEvent.m_srcWidget.transform.parent != null)
			{
				string name = uiEvent.m_srcWidget.transform.parent.name;
				int num = int.Parse(name.Substring(name.IndexOf("equipItem") + 9));
				if (num < 0 || num >= 6)
				{
					return;
				}
				FrameCommand<PlayerChooseEquipSkillCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerChooseEquipSkillCommand>();
				frameCommand.cmdData.m_iEquipSlotIndex = num;
				frameCommand.Send();
			}
		}

		private void HandleEquipActiveSkillWhenEquipChange(uint actorObjectID, bool bIsAdd, int iEquipSlotIndex)
		{
			if (this.m_hostCtrlHero && actorObjectID == this.m_hostCtrlHero.handle.ObjID && this.m_hostCtrlHero.handle.EquipComponent != null)
			{
				ENUM_EQUIP_ACTIVESKILL_STATUS equipActiveSkillSlotInfo = this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillSlotInfo(iEquipSlotIndex);
				SkillSlotType showingSkillSlotByEquipSlot = this.m_hostCtrlHero.handle.EquipComponent.GetShowingSkillSlotByEquipSlot(iEquipSlotIndex);
				switch (equipActiveSkillSlotInfo)
				{
				case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_WHITHOUTACTIVESKILL:
					this.SetEquipSkillShowBtnState(false, iEquipSlotIndex);
					this.SetEquipSkillBeUsingBtnState(false, iEquipSlotIndex);
					if (!bIsAdd)
					{
						this.SetEquipActiveSkillCdState(iEquipSlotIndex, -1);
						for (int i = 0; i < 1; i++)
						{
							if (this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillShowFlag(i + SkillSlotType.SLOT_SKILL_9))
							{
								int equipSlotBySkillSlot = this.m_hostCtrlHero.handle.EquipComponent.GetEquipSlotBySkillSlot(i + SkillSlotType.SLOT_SKILL_9);
								this.SetEquipSkillBeUsingBtnState(true, equipSlotBySkillSlot);
							}
						}
					}
					break;
				case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW:
					this.UpdateEquipActiveSkillCd(iEquipSlotIndex, false);
					this.SetEquipSkillBeUsingBtnState(false, iEquipSlotIndex);
					break;
				case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_ISSHOWING:
					if (this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillInitFlag(showingSkillSlotByEquipSlot))
					{
						if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
						{
							Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(this.m_hostCtrlHero, true, showingSkillSlotByEquipSlot);
						}
					}
					else
					{
						this.SetEquipSkillBtnState(true, showingSkillSlotByEquipSlot);
					}
					this.UpdateEquipActiveSkillCd(iEquipSlotIndex, false);
					this.SetEquipSkillBeUsingBtnState(true, iEquipSlotIndex);
					break;
				case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_WHITHOUTACTIVESKILL_REMOVECURSKILL:
					this.SetEquipActiveSkillCdState(iEquipSlotIndex, -1);
					for (int j = 0; j < 1; j++)
					{
						if (!this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillShowFlag(j + SkillSlotType.SLOT_SKILL_9))
						{
							this.SetEquipSkillBtnState(false, j + SkillSlotType.SLOT_SKILL_9);
						}
					}
					this.SetEquipSkillShowBtnState(false, iEquipSlotIndex);
					this.SetEquipSkillBeUsingBtnState(false, iEquipSlotIndex);
					break;
				}
			}
		}

		private void InitEquipActiveSkillTimerTxt()
		{
			for (int i = 0; i < 6; i++)
			{
				if (this.m_objEquipActiveTimerTxt[i] == null)
				{
					Transform bagEquipItem = this.GetBagEquipItem(i);
					if (bagEquipItem)
					{
						Transform transform = bagEquipItem.FindChild("TimerTxt");
						if (transform)
						{
							this.m_objEquipActiveTimerTxt[i] = transform.FindChild("TimerTxt").gameObject;
						}
					}
					this.m_arrEquipActiveSkillCd[i] = 0;
				}
			}
		}

		private void SetEquipActiveSkillCdState(int iEquipSlotIndex, int icd)
		{
			if (iEquipSlotIndex < 0 || iEquipSlotIndex > 6)
			{
				return;
			}
			if (this.m_objEquipActiveTimerTxt[iEquipSlotIndex] != null)
			{
				if (icd > 0)
				{
					Text component = this.m_objEquipActiveTimerTxt[iEquipSlotIndex].GetComponent<Text>();
					if (component != null)
					{
						component.text = SimpleNumericString.GetNumeric(Mathf.CeilToInt((float)(icd / 1000)) + 1);
					}
					if (!this.m_objEquipActiveTimerTxt[iEquipSlotIndex].transform.parent.gameObject.activeSelf)
					{
						this.m_objEquipActiveTimerTxt[iEquipSlotIndex].transform.parent.gameObject.CustomSetActive(true);
					}
				}
				else if (this.m_objEquipActiveTimerTxt[iEquipSlotIndex].transform.parent.gameObject.activeSelf)
				{
					this.m_objEquipActiveTimerTxt[iEquipSlotIndex].transform.parent.gameObject.CustomSetActive(false);
				}
			}
		}

		private void UpdateEquipActiveSkillCd(int iEquipSlotIndex, bool bIsOpenForm = false)
		{
			if (iEquipSlotIndex < 0 || iEquipSlotIndex > 6)
			{
				return;
			}
			EquipComponent equipComponent = this.m_hostCtrlHero.handle.EquipComponent;
			stEquipInfo[] equips = equipComponent.GetEquips();
			if (equips == null)
			{
				return;
			}
			if (equips[iEquipSlotIndex].m_equipID > 0)
			{
				ENUM_EQUIP_ACTIVESKILL_STATUS equipActiveSkillSlotInfo = equipComponent.GetEquipActiveSkillSlotInfo(iEquipSlotIndex);
				if (equipActiveSkillSlotInfo == ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_ISSHOWING)
				{
					SkillSlotType showingSkillSlotByEquipSlot = equipComponent.GetShowingSkillSlotByEquipSlot(iEquipSlotIndex);
					SkillSlot skillSlot;
					if (this.m_hostCtrlHero.handle.SkillControl != null && this.m_hostCtrlHero.handle.SkillControl.TryGetSkillSlot(showingSkillSlotByEquipSlot, out skillSlot))
					{
						int num = skillSlot.CurSkillCD;
						if (num != this.m_arrEquipActiveSkillCd[iEquipSlotIndex] || bIsOpenForm)
						{
							this.m_arrEquipActiveSkillCd[iEquipSlotIndex] = num;
							this.SetEquipActiveSkillCdState(iEquipSlotIndex, num);
							equipComponent.AddEquipActiveSkillCdInfo(iEquipSlotIndex, num);
						}
					}
				}
				else if (equipActiveSkillSlotInfo == ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW)
				{
					int num2 = 0;
					equipComponent.GetEquipActiveSkillCdInfo(iEquipSlotIndex, out num2);
					if (num2 != this.m_arrEquipActiveSkillCd[iEquipSlotIndex] || bIsOpenForm)
					{
						this.m_arrEquipActiveSkillCd[iEquipSlotIndex] = num2;
						this.SetEquipActiveSkillCdState(iEquipSlotIndex, num2);
					}
				}
			}
		}

		private void UpdateEquipActiveSkillCd(bool bIsOpenForm = false)
		{
			if (!this.m_hostCtrlHero)
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				this.UpdateEquipActiveSkillCd(i, bIsOpenForm);
			}
		}
	}
}
