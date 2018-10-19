using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
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
	public class CMallRecommendController : Singleton<CMallRecommendController>
	{
		public enum Tab
		{
			None = -1,
			Hero,
			Skin
		}

		public enum ListTab
		{
			None = -1,
			Hero,
			Skin
		}

		private ResRandDrawInfo[] m_DefaultLotteryCtrl;

		public ResRandDrawInfo[] m_CurrentLottryCtrl;

		private ListView<ResRewardPoolInfo>[] m_CurrentPoolList;

		private ListView<ResSaleRecommend> m_CurrentRecommendProductList;

		private ListView<ResHeroCfgInfo> m_CurrentRecommendHeros;

		private ListView<ResHeroSkin> m_CurrentRecommendSkins;

		private Dictionary<long, uint> m_CurrentRecommendProductMap;

		private int[] m_MostRecentCtrlRefreshTime;

		private int m_MostRecentRecommendRefreshTime;

		private int[] m_RefreshLotteryCtrlTimerSeq;

		private int m_RefreshRecommendProductTimerSeq;

		private ResHeroCfgInfo m_FirstHero;

		private ResHeroSkin m_FirstSkin;

		private string m_heroModelPath;

		private int m_HasHeroAndSkin;

		private CMallRecommendController.Tab m_CurTab;

		private CMallRecommendController.ListTab m_CurListTab;

		public static string sMallHeroExchangeListPath = "UGUI/Form/System/Mall/Form_Recommend_Exchange_List.prefab";

		public static string[] s_exchangePurposeNameKeys = new string[]
		{
			"ExchangePurpose_Exchange"
		};

		public static string[] s_exchangeTypeNameKeys = new string[]
		{
			"ExchangeType_NotSupport",
			"ExchangeType_Hero_Exchange_Coupons",
			"ExchangeType_Skin_Exchange_Coupons"
		};

		public CMallRecommendController.Tab CurTab
		{
			get
			{
				return this.m_CurTab;
			}
			set
			{
				this.m_CurTab = value;
			}
		}

		public CMallRecommendController.ListTab CurListTab
		{
			get
			{
				return this.m_CurListTab;
			}
			set
			{
				this.m_CurListTab = value;
			}
		}

		public override void Init()
		{
			base.Init();
			this.m_DefaultLotteryCtrl = new ResRandDrawInfo[3];
			this.m_CurrentLottryCtrl = new ResRandDrawInfo[3];
			this.m_CurrentPoolList = new ListView<ResRewardPoolInfo>[3];
			this.m_CurrentRecommendProductList = new ListView<ResSaleRecommend>();
			this.m_CurrentRecommendHeros = new ListView<ResHeroCfgInfo>();
			this.m_CurrentRecommendSkins = new ListView<ResHeroSkin>();
			this.m_CurrentRecommendProductMap = new Dictionary<long, uint>();
			this.m_MostRecentCtrlRefreshTime = new int[3];
			this.m_MostRecentRecommendRefreshTime = 0;
			this.m_RefreshLotteryCtrlTimerSeq = new int[3];
			this.m_RefreshRecommendProductTimerSeq = -1;
			this.m_FirstHero = null;
			this.m_FirstSkin = null;
			this.CurTab = CMallRecommendController.Tab.None;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Exchange_More, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeMore));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendListTabChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Hero_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListHeroEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Skin_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListSkinEnable));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BAG_ITEMS_UPDATE, new Action(this.OnItemAdd));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Recommend_Recommend_Data_Refresh, new Action(this.RefreshRecommendView));
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_DefaultLotteryCtrl = null;
			this.m_CurrentLottryCtrl = null;
			this.m_CurrentPoolList = null;
			this.m_CurrentRecommendProductList = null;
			this.m_CurrentRecommendProductMap = null;
			this.m_CurrentRecommendHeros = null;
			this.m_CurrentRecommendSkins = null;
			this.m_MostRecentRecommendRefreshTime = 0;
			for (byte b = 0; b < 3; b += 1)
			{
				this.m_MostRecentCtrlRefreshTime[(int)b] = 0;
				Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_RefreshLotteryCtrlTimerSeq[(int)b]);
				this.m_RefreshLotteryCtrlTimerSeq[(int)b] = -1;
			}
			this.m_RefreshRecommendProductTimerSeq = -1;
			this.m_FirstHero = null;
			this.m_FirstSkin = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendTabChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Exchange_More, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeMore));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendListTabChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListHeroEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Skin_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListSkinEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnRecommendTimerEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BAG_ITEMS_UPDATE, new Action(this.OnItemAdd));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Recommend_Recommend_Data_Refresh, new Action(this.RefreshRecommendView));
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Recommend", "pnlRecommend", form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), "pnlRecommend");
			return !(x == null);
		}

		public void Draw(CUIFormScript form)
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendTabChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Buy, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnRecommendTimerEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
			this.InitElements();
			if (!this.RefreshPoolData(0u))
			{
				Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			}
			if (!this.RefreshRecommendData())
			{
				Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			}
		}

		public void InitElements()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend").CustomSetActive(true);
			Utility.FindChild(mallForm.gameObject, "UIScene_Recommend_HeroInfo").CustomSetActive(true);
		}

		private bool SetCurrentLotteryCtrl(uint drawID = 0u)
		{
			byte b = 3;
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				this.m_CurrentLottryCtrl[(int)b2] = null;
				this.m_MostRecentCtrlRefreshTime[(int)b2] = 2147483647;
			}
			ResRandDrawInfo resRandDrawInfo = new ResRandDrawInfo();
			if (drawID != 0u && GameDataMgr.recommendLotteryCtrlDict.TryGetValue(drawID, out resRandDrawInfo) && resRandDrawInfo.bDrawType >= 1 && resRandDrawInfo.bDrawType < 3)
			{
				this.m_CurrentLottryCtrl[(int)resRandDrawInfo.bDrawType] = resRandDrawInfo;
			}
			DictionaryView<uint, ResRandDrawInfo>.Enumerator enumerator = GameDataMgr.recommendLotteryCtrlDict.GetEnumerator();
			int num = 1;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResRandDrawInfo> current = enumerator.Current;
				ResRandDrawInfo value = current.Value;
				byte bDrawType = value.bDrawType;
				if (bDrawType < 1 || value.bDrawType > 3)
				{
					DebugHelper.Assert(false, string.Format("recommend random draw config err, draw type{0} invalid!", bDrawType));
					return false;
				}
				if (value.dwEndTimeGen == 2147483647u && value.dwStartTimeGen == 0u)
				{
					this.m_DefaultLotteryCtrl[(int)bDrawType] = value;
					num++;
				}
				else
				{
					uint num2 = value.dwStartTimeGen;
					uint num3 = value.dwEndTimeGen;
					if (num2 > num3)
					{
						num2 ^= num3;
						num3 ^= num2;
						num2 = (num3 ^ num2);
					}
					if (num2 < currentUTCTime && currentUTCTime < num3)
					{
						if (this.m_CurrentLottryCtrl[(int)bDrawType] == null)
						{
							value.dwStartTimeGen = num2;
							value.dwEndTimeGen = num3;
							this.m_CurrentLottryCtrl[(int)bDrawType] = value;
						}
					}
					else if (num2 > currentUTCTime && (ulong)num2 < (ulong)((long)this.m_MostRecentCtrlRefreshTime[(int)bDrawType]))
					{
						this.m_MostRecentCtrlRefreshTime[(int)bDrawType] = (int)num2;
					}
					num++;
				}
			}
			for (int i = 1; i < (int)b; i++)
			{
				if (this.m_DefaultLotteryCtrl[i] == null || this.m_DefaultLotteryCtrl[i].dwDrawID == 0u)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Recommend Lottery Ctrl Cfg Err", false, 1.5f, null, new object[0]);
					return false;
				}
				if (this.m_CurrentLottryCtrl[i] == null || this.m_CurrentLottryCtrl[i].dwDrawID == 0u)
				{
					this.m_CurrentLottryCtrl[i] = this.m_DefaultLotteryCtrl[i];
				}
				Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_RefreshLotteryCtrlTimerSeq[i]);
				if (this.m_MostRecentCtrlRefreshTime[i] == 2147483647 && this.m_CurrentLottryCtrl[i].dwDrawID != this.m_DefaultLotteryCtrl[i].dwDrawID)
				{
					this.m_MostRecentCtrlRefreshTime[i] = (int)this.m_CurrentLottryCtrl[i].dwEndTimeGen;
				}
				long num4 = ((long)this.m_MostRecentCtrlRefreshTime[i] - (long)((ulong)currentUTCTime)) * 1000L;
				if (num4 > 0L && num4 < 2147483647L)
				{
					this.m_RefreshLotteryCtrlTimerSeq[i] = Singleton<CTimerManager>.GetInstance().AddTimer((int)num4, 1, new CTimer.OnTimeUpHandler(this.RefreshPoolDataTimerHandler));
				}
			}
			return true;
		}

		private bool SetCurrentPoolData()
		{
			byte b = 3;
			for (byte b2 = 1; b2 < b; b2 += 1)
			{
				if (this.m_CurrentPoolList[(int)b2] == null)
				{
					this.m_CurrentPoolList[(int)b2] = new ListView<ResRewardPoolInfo>();
				}
				else
				{
					this.m_CurrentPoolList[(int)b2].Clear();
				}
				DictionaryView<long, ResRewardPoolInfo>.Enumerator enumerator = GameDataMgr.recommendRewardDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, ResRewardPoolInfo> current = enumerator.Current;
					ResRewardPoolInfo value = current.Value;
					if (value.dwPoolID == this.m_CurrentLottryCtrl[(int)b2].dwRewardPoolID && value.stRewardInfo.bIsShow > 0)
					{
						this.m_CurrentPoolList[(int)b2].Add(value);
					}
				}
				if (this.m_CurrentPoolList[(int)b2].Count == 0)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Recommend Lottery Reward Cfg Err", false, 1.5f, null, new object[0]);
					return false;
				}
			}
			return true;
		}

		private bool SetCurrentRecommendData()
		{
			this.m_CurrentRecommendProductList.Clear();
			this.m_MostRecentRecommendRefreshTime = 2147483647;
			DictionaryView<uint, ResSaleRecommend>.Enumerator enumerator = GameDataMgr.recommendProductDict.GetEnumerator();
			int num = 1;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResSaleRecommend> current = enumerator.Current;
				ResSaleRecommend value = current.Value;
				if (value.dwOffTimeGen == 2147483647u)
				{
					value.dwOnTimeGen = 0u;
					value.dwOffTimeGen = 2147483647u;
				}
				uint num2 = value.dwOnTimeGen;
				uint num3 = value.dwOffTimeGen;
				if (num2 > num3)
				{
					num2 ^= num3;
					num3 ^= num2;
					num2 = (num3 ^ num2);
				}
				if (num2 < currentUTCTime && currentUTCTime < num3)
				{
					value.dwOnTimeGen = num2;
					value.dwOffTimeGen = num3;
					if ((long)this.m_MostRecentRecommendRefreshTime > (long)((ulong)value.dwOffTimeGen))
					{
						this.m_MostRecentRecommendRefreshTime = (int)value.dwOffTimeGen;
					}
					this.m_CurrentRecommendProductList.Add(value);
				}
				else if (num2 > currentUTCTime && (ulong)num2 < (ulong)((long)this.m_MostRecentRecommendRefreshTime))
				{
					this.m_MostRecentRecommendRefreshTime = (int)num2;
				}
				num++;
			}
			if (this.m_CurrentRecommendProductList.Count == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Recommend Products Ctrl Cfg Err", false, 1.5f, null, new object[0]);
				return false;
			}
			this.m_CurrentRecommendProductList.Sort(delegate(ResSaleRecommend a, ResSaleRecommend b)
			{
				if (a == null && b == null)
				{
					return 0;
				}
				if (a == null && b != null)
				{
					return 1;
				}
				if (b == null && a != null)
				{
					return -1;
				}
				if (a.iRecmdStar < b.iRecmdStar)
				{
					return 1;
				}
				if (a.iRecmdStar == b.iRecmdStar)
				{
					return 0;
				}
				if (a.iRecmdStar > b.iRecmdStar)
				{
					return -1;
				}
				return -1;
			});
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_RefreshRecommendProductTimerSeq);
			long num4 = ((long)this.m_MostRecentRecommendRefreshTime - (long)((ulong)currentUTCTime)) * 1000L;
			if (num4 > 0L && num4 < 2147483647L)
			{
				this.m_RefreshRecommendProductTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int)num4, 1, new CTimer.OnTimeUpHandler(this.RefreshRecommendDataTimerHandler));
			}
			return true;
		}

		private void RefreshPoolDataTimerHandler(int seq)
		{
			this.RefreshPoolData(0u);
		}

		private void RefreshRecommendDataTimerHandler(int seq)
		{
			this.RefreshRecommendData();
		}

		public bool RefreshPoolData(uint drawID = 0u)
		{
			if (!this.SetCurrentLotteryCtrl(drawID))
			{
				return false;
			}
			if (!this.SetCurrentPoolData())
			{
				return false;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Recommend_Pool_Data_Refresh);
			return true;
		}

		public bool RefreshRecommendData()
		{
			if (!this.SetCurrentRecommendData())
			{
				return false;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Recommend_Recommend_Data_Refresh);
			return true;
		}

		public void Clear()
		{
			for (byte b = 0; b < 3; b += 1)
			{
				if (this.m_CurrentPoolList[(int)b] != null)
				{
					this.m_CurrentPoolList[(int)b].Clear();
				}
			}
			this.m_CurrentRecommendProductList.Clear();
			this.m_CurrentRecommendProductMap.Clear();
			this.m_CurrentRecommendHeros.Clear();
			this.m_CurrentRecommendSkins.Clear();
			this.m_HasHeroAndSkin = 0;
			this.m_FirstHero = null;
			this.m_FirstSkin = null;
		}

		private void RefreshPoolView()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/rewardBody/List");
			if (gameObject == null)
			{
				return;
			}
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			ListView<ResRewardPoolInfo> listView = new ListView<ResRewardPoolInfo>();
			ResRandDrawInfo resRandDrawInfo = new ResRandDrawInfo();
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					listView = this.m_CurrentPoolList[2];
					resRandDrawInfo = this.m_CurrentLottryCtrl[2];
				}
			}
			else
			{
				listView = this.m_CurrentPoolList[1];
				resRandDrawInfo = this.m_CurrentLottryCtrl[1];
			}
			int count = listView.Count;
			component.SetElementAmount(count);
			for (int i = 0; i < count; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)listView[i].stRewardInfo.wItemType, (int)listView[i].stRewardInfo.wItemCnt, listView[i].stRewardInfo.dwItemID);
				if (elemenet != null && cUseable != null)
				{
					GameObject gameObject2 = Utility.FindChild(elemenet.gameObject, "itemCell");
					if (gameObject2 != null)
					{
						CUICommonSystem.SetItemCell(mallForm, gameObject2, cUseable, false, false, false, false);
						if (cUseable.m_stackCount == 1)
						{
							Utility.FindChild(gameObject2, "cntBg").CustomSetActive(false);
							Utility.FindChild(gameObject2, "lblIconCount").CustomSetActive(false);
						}
					}
				}
			}
			stPayInfo stPayInfo = default(stPayInfo);
			stPayInfo.m_payType = CMallSystem.ResBuyTypeToPayType((int)resRandDrawInfo.bCostType);
			stPayInfo.m_payValue = resRandDrawInfo.dwCostPrice;
			Transform transform = mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/BuyButton");
			stUIEventParams stUIEventParams = default(stUIEventParams);
			CMallSystem.SetPayButton(mallForm, transform as RectTransform, stPayInfo.m_payType, stPayInfo.m_payValue, enUIEventID.Mall_Recommend_On_Lottery_Buy, ref stUIEventParams);
		}

		private void RefreshRecommendView()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is NUll");
				return;
			}
			this.m_CurrentRecommendHeros.Clear();
			this.m_CurrentRecommendSkins.Clear();
			for (int i = 0; i < this.m_CurrentRecommendProductList.Count; i++)
			{
				ResSaleRecommend resSaleRecommend = this.m_CurrentRecommendProductList[i];
				COM_ITEM_TYPE wSaleType = (COM_ITEM_TYPE)resSaleRecommend.wSaleType;
				if (wSaleType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resSaleRecommend.dwSaleID);
					DebugHelper.Assert(dataByKey != null, string.Format("heroCfg databin error, Hero ID{0}", resSaleRecommend.dwSaleID));
					if (dataByKey != null)
					{
						if ((this.m_HasHeroAndSkin & 1) == 0)
						{
							this.m_FirstHero = dataByKey;
							this.m_HasHeroAndSkin |= 1;
						}
						IHeroData heroData = CHeroDataFactory.CreateHeroData(this.m_FirstHero.dwCfgID);
						IHeroData heroData2 = CHeroDataFactory.CreateHeroData(dataByKey.dwCfgID);
						if (heroData.bPlayerOwn && !heroData2.bPlayerOwn)
						{
							this.m_FirstHero = dataByKey;
						}
						this.m_CurrentRecommendHeros.Add(dataByKey);
						long doubleKey = GameDataMgr.GetDoubleKey((uint)resSaleRecommend.wSaleType, resSaleRecommend.dwSaleID);
						if (!this.m_CurrentRecommendProductMap.ContainsKey(doubleKey))
						{
							this.m_CurrentRecommendProductMap.Add(doubleKey, resSaleRecommend.dwID);
						}
					}
				}
				else if (wSaleType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
				{
					uint heroId = 0u;
					uint skinId = 0u;
					CSkinInfo.ResolveHeroSkin(resSaleRecommend.dwSaleID, out heroId, out skinId);
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
					DebugHelper.Assert(heroSkin != null, string.Format("skinCfg databin error, Skin ID{0}", resSaleRecommend.dwSaleID));
					if (heroSkin != null)
					{
						if ((this.m_HasHeroAndSkin & 2) == 0)
						{
							this.m_FirstSkin = heroSkin;
							this.m_HasHeroAndSkin |= 2;
						}
						if (masterRoleInfo.IsHaveHeroSkin(this.m_FirstSkin.dwHeroID, this.m_FirstSkin.dwSkinID, false) && !masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
						{
							this.m_FirstSkin = heroSkin;
						}
						this.m_CurrentRecommendSkins.Add(heroSkin);
						long doubleKey2 = GameDataMgr.GetDoubleKey((uint)resSaleRecommend.wSaleType, resSaleRecommend.dwSaleID);
						if (!this.m_CurrentRecommendProductMap.ContainsKey(doubleKey2))
						{
							this.m_CurrentRecommendProductMap.Add(doubleKey2, resSaleRecommend.dwID);
						}
					}
				}
			}
			this.InitRecommendTab();
		}

		private void InitRecommendTab()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			CMallRecommendController.Tab[] array = (CMallRecommendController.Tab[])Enum.GetValues(typeof(CMallRecommendController.Tab));
			List<CMallRecommendController.Tab> list = new List<CMallRecommendController.Tab>();
			byte b = 0;
			while ((int)b < array.Length)
			{
				CMallRecommendController.Tab tab = array[(int)b];
				switch (tab + 1)
				{
				case CMallRecommendController.Tab.Skin:
					if ((this.m_HasHeroAndSkin & 1) > 0)
					{
						list.Add(array[(int)b]);
					}
					break;
				case (CMallRecommendController.Tab)2:
					if ((this.m_HasHeroAndSkin & 2) > 0)
					{
						list.Add(array[(int)b]);
					}
					break;
				}
				b += 1;
			}
			string[] array2 = new string[list.Count];
			byte b2 = 0;
			while ((int)b2 < array2.Length)
			{
				CMallRecommendController.Tab tab = list[(int)b2];
				if (tab != CMallRecommendController.Tab.Hero)
				{
					if (tab == CMallRecommendController.Tab.Skin)
					{
						array2[(int)b2] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewSkin");
					}
				}
				else
				{
					array2[(int)b2] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewHero");
				}
				b2 += 1;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlRecommend/Tab");
			if (componetInChild == null)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("list err", false, 1.5f, null, new object[0]);
				Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
				return;
			}
			componetInChild.SetElementAmount(array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				CUIListElementScript elemenet = componetInChild.GetElemenet(i);
				if (elemenet != null)
				{
					Text component = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
					component.text = array2[i];
				}
			}
			componetInChild.m_alwaysDispatchSelectedChangeEvent = true;
			if (this.CurTab == CMallRecommendController.Tab.None || this.CurTab < CMallRecommendController.Tab.Hero || this.CurTab >= (CMallRecommendController.Tab)componetInChild.GetElementAmount())
			{
				componetInChild.SelectElement(0, true);
			}
			else
			{
				componetInChild.SelectElement((int)this.CurTab, true);
			}
		}

		private void InitListTab()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRecommendController.sMallHeroExchangeListPath);
			if (form == null)
			{
				return;
			}
			CMallRecommendController.ListTab[] array = (CMallRecommendController.ListTab[])Enum.GetValues(typeof(CMallRecommendController.ListTab));
			List<CMallRecommendController.ListTab> list = new List<CMallRecommendController.ListTab>();
			byte b = 0;
			while ((int)b < array.Length)
			{
				CMallRecommendController.ListTab listTab = array[(int)b];
				switch (listTab + 1)
				{
				case CMallRecommendController.ListTab.Skin:
					if ((this.m_HasHeroAndSkin & 1) > 0)
					{
						list.Add(array[(int)b]);
					}
					break;
				case (CMallRecommendController.ListTab)2:
					if ((this.m_HasHeroAndSkin & 2) > 0)
					{
						list.Add(array[(int)b]);
					}
					break;
				}
				b += 1;
			}
			string[] array2 = new string[list.Count];
			byte b2 = 0;
			while ((int)b2 < array2.Length)
			{
				CMallRecommendController.ListTab listTab = list[(int)b2];
				if (listTab != CMallRecommendController.ListTab.Hero)
				{
					if (listTab == CMallRecommendController.ListTab.Skin)
					{
						array2[(int)b2] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewSkin");
					}
				}
				else
				{
					array2[(int)b2] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewHero");
				}
				b2 += 1;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "Panel/Tab");
			if (componetInChild == null)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("list err", false, 1.5f, null, new object[0]);
				Singleton<CUIManager>.GetInstance().CloseForm(CMallRecommendController.sMallHeroExchangeListPath);
				return;
			}
			componetInChild.SetElementAmount(array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				CUIListElementScript elemenet = componetInChild.GetElemenet(i);
				if (elemenet != null)
				{
					Text component = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
					component.text = array2[i];
				}
			}
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab != CMallRecommendController.Tab.Skin)
				{
					this.CurListTab = CMallRecommendController.ListTab.Hero;
				}
				else
				{
					this.CurListTab = CMallRecommendController.ListTab.Skin;
				}
			}
			else
			{
				this.CurListTab = CMallRecommendController.ListTab.Hero;
			}
			componetInChild.SelectElement((int)this.CurListTab, true);
		}

		private void UpdateHeroView()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_recommendHeroInfoBgPath, mallForm);
			if (this.m_FirstHero != null)
			{
				this.Refresh3DModel(this.m_FirstHero.dwCfgID, 0);
				this.RefreshHeroBaseInfo(this.m_FirstHero);
				this.RefreshHeroPricePnl(this.m_FirstHero);
				this.RefreshSkillPanel(this.m_FirstHero);
			}
		}

		private void UpdateSkinView()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_recommendHeroInfoBgPath, mallForm);
			this.Refresh3DModel(this.m_FirstSkin.dwHeroID, (int)this.m_FirstSkin.dwSkinID);
			this.RefreshSkinBaseInfo(this.m_FirstSkin);
			this.RefreshSkinPricePnl(this.m_FirstSkin);
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/SkillList");
			obj.CustomSetActive(false);
		}

		private void UpdateExchangeCouponsInfo(CUIFormScript form, Transform container, enExchangeType exchangeType)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is NUll");
				return;
			}
			Image componetInChild = Utility.GetComponetInChild<Image>(container.gameObject, "Image");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(container.gameObject, "Cnt");
			if (componetInChild == null || componetInChild2 == null)
			{
				return;
			}
			componetInChild.SetSprite(this.GetExchangeTypeIconPath(exchangeType), form, true, false, false, false);
			componetInChild2.text = CMallRecommendController.GetCurrencyValueFromRoleInfo(masterRoleInfo, exchangeType).ToString();
		}

		private void RefreshHeroBaseInfo(ResHeroCfgInfo heroCfg)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/heroInfoPanel");
			if (gameObject == null)
			{
				return;
			}
			Text component = gameObject.transform.Find("heroNameText").GetComponent<Text>();
			GameObject obj = Utility.FindChild(gameObject, "heroTitleText");
			obj.CustomSetActive(false);
			GameObject gameObject2 = gameObject.transform.Find("jobImage").gameObject;
			IHeroData heroData = CHeroDataFactory.CreateHeroData(heroCfg.dwCfgID);
			if (gameObject2 != null)
			{
				CUICommonSystem.SetHeroJob(mallForm, gameObject2, (enHeroJobType)heroData.heroType);
			}
			if (component != null)
			{
				component.text = heroData.heroName;
			}
		}

		private void RefreshSkinBaseInfo(ResHeroSkin skinCfg)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/heroInfoPanel");
			if (gameObject == null)
			{
				return;
			}
			Text component = gameObject.transform.Find("heroNameText").GetComponent<Text>();
			GameObject obj = Utility.FindChild(gameObject, "heroTitleText");
			obj.CustomSetActive(false);
			GameObject gameObject2 = gameObject.transform.Find("jobImage").gameObject;
			IHeroData heroData = CHeroDataFactory.CreateHeroData(skinCfg.dwHeroID);
			if (gameObject2 != null)
			{
				CUICommonSystem.SetHeroJob(mallForm, gameObject2, (enHeroJobType)heroData.heroType);
			}
			if (component != null)
			{
				component.text = string.Format("{0} {1}", heroData.heroName, StringHelper.UTF8BytesToString(ref skinCfg.szSkinName));
			}
		}

		private void RefreshSkillPanel(ResHeroCfgInfo heroCfg)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/SkillList");
			gameObject.CustomSetActive(true);
			CUIListScript cUIListScript = null;
			if (gameObject != null)
			{
				cUIListScript = gameObject.GetComponent<CUIListScript>();
			}
			DebugHelper.Assert(cUIListScript != null, "skill list is null");
			if (cUIListScript == null)
			{
				gameObject.CustomSetActive(false);
				return;
			}
			IHeroData heroData = CHeroDataFactory.CreateHeroData(heroCfg.dwCfgID);
			if (heroData == null)
			{
				return;
			}
			ResDT_SkillInfo[] skillArr = heroData.skillArr;
			cUIListScript.SetElementAmount(skillArr.Length - 1);
			for (int i = 0; i < skillArr.Length - 1; i++)
			{
				CUIListElementScript elemenet = cUIListScript.GetElemenet(i);
				if (elemenet == null)
				{
					DebugHelper.Assert(elemenet != null, "list element is null");
					break;
				}
				GameObject gameObject2 = cUIListScript.GetElemenet(i).transform.Find("heroSkillItemCell").gameObject;
				if (!(gameObject2 == null))
				{
					ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(skillArr[i].iSkillID);
					CUIEventScript component = gameObject2.GetComponent<CUIEventScript>();
					if (skillCfgInfo == null)
					{
						return;
					}
					GameObject gameObject3 = Utility.FindChild(gameObject2, "skillMask/skillIcon");
					if (gameObject3 == null)
					{
						return;
					}
					Image component2 = gameObject3.GetComponent<Image>();
					if (component2 == null)
					{
						return;
					}
					string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, StringHelper.UTF8BytesToString(ref skillCfgInfo.szIconPath));
					CUIUtility.SetImageSprite(component2, prefabPath, elemenet.m_belongedFormScript, false, true, true, false);
					gameObject3.CustomSetActive(true);
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.skillTipParam.skillName = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillName);
					eventParams.skillTipParam.strTipText = CUICommonSystem.GetSkillDescLobby(skillCfgInfo.szSkillDesc, heroCfg.dwCfgID);
					eventParams.skillTipParam.skillCoolDown = ((i != 0) ? Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[]
					{
						CUICommonSystem.ConvertMillisecondToSecondWithOneDecimal(skillCfgInfo.iCoolDown)
					}) : Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5"));
					eventParams.skillTipParam.skillEnergyCost = ((i != 0 && heroCfg.dwEnergyType != 6u) ? Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText(heroCfg.dwEnergyType, EnergyShowType.CostValue), new string[]
					{
						skillCfgInfo.iEnergyCost.ToString()
					}) : string.Empty);
					eventParams.skillPropertyDesc = CUICommonSystem.ParseSkillLevelUpProperty(ref skillCfgInfo.astSkillPropertyDescInfo, heroCfg.dwCfgID);
					eventParams.skillSlotId = i;
					eventParams.skillTipParam.skillEffect = skillCfgInfo.SkillEffectType;
					if (component != null)
					{
						component.SetUIEvent(enUIEventType.Down, enUIEventID.Mall_Recommend_Hero_Skill_Down, eventParams);
						component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Mall_Recommend_Hero_Skill_Up, eventParams);
						component.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Recommend_Hero_Skill_Up, eventParams);
						component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Mall_Recommend_Hero_Skill_Up, eventParams);
					}
				}
			}
		}

		private void RefreshHeroPricePnl(ResHeroCfgInfo heroCfg)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/coinBuy");
			GameObject gameObject2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/diamondBuy");
			GameObject gameObject3 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/couponsBuy");
			GameObject gameObject4 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/exchangeBuy");
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/linkBtnContainer");
			GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/owned");
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			gameObject3.CustomSetActive(false);
			gameObject4.CustomSetActive(false);
			obj.CustomSetActive(false);
			obj2.CustomSetActive(false);
			IHeroData heroData = CHeroDataFactory.CreateHeroData(heroCfg.dwCfgID);
			if (heroData.bPlayerOwn)
			{
				obj2.CustomSetActive(true);
				return;
			}
			ResHeroPromotion resPromotion = heroData.promotion();
			stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroCfg, resPromotion);
			for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
			{
				GameObject gameObject5 = null;
				stPayInfo stPayInfo = payInfoSetOfGood.m_payInfos[i];
				switch (stPayInfo.m_payType)
				{
				case enPayType.GoldCoin:
					gameObject.CustomSetActive(true);
					gameObject5 = gameObject;
					break;
				case enPayType.DianQuan:
					gameObject3.CustomSetActive(true);
					gameObject5 = gameObject3;
					break;
				case enPayType.Diamond:
				case enPayType.DiamondAndDianQuan:
					gameObject2.CustomSetActive(true);
					gameObject5 = gameObject2;
					break;
				}
				if (gameObject5 != null)
				{
					Transform transform = gameObject5.transform.Find("BuyButton");
					if (transform != null)
					{
						stUIEventParams stUIEventParams = default(stUIEventParams);
						stUIEventParams.tag = (int)stPayInfo.m_payType;
						stUIEventParams.commonUInt32Param1 = stPayInfo.m_payValue;
						stUIEventParams.heroSkinParam.heroId = heroCfg.dwCfgID;
						CMallSystem.SetPayButton(mallForm, transform as RectTransform, stPayInfo.m_payType, stPayInfo.m_payValue, enUIEventID.Mall_Recommend_On_Buy, ref stUIEventParams);
					}
				}
			}
			if (gameObject4 != null)
			{
				ResHeroShop resHeroShop = null;
				GameDataMgr.heroShopInfoDict.TryGetValue(heroCfg.dwCfgID, out resHeroShop);
				if (resHeroShop != null)
				{
					uint num = (resHeroShop.bIsBuyItem <= 0) ? 0u : resHeroShop.dwBuyItemCnt;
					Transform transform2 = gameObject4.transform.Find("BuyButton");
					stUIEventParams stUIEventParams2 = default(stUIEventParams);
					stUIEventParams2.tag = 1;
					stUIEventParams2.commonUInt32Param1 = num;
					stUIEventParams2.heroSkinParam.heroId = heroCfg.dwCfgID;
					if (num > 0u)
					{
						gameObject4.CustomSetActive(true);
						this.SetExchangeButton(mallForm, transform2 as RectTransform, enExchangeType.HeroExchangeCoupons, num, enUIEventID.Mall_Recommend_On_Exchange, ref stUIEventParams2);
					}
				}
			}
		}

		private void RefreshSkinPricePnl(ResHeroSkin skinCfg)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is NUll");
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/coinBuy");
			GameObject gameObject2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/diamondBuy");
			GameObject gameObject3 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/couponsBuy");
			GameObject gameObject4 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/exchangeBuy");
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/owned");
			GameObject gameObject5 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/linkBtnContainer");
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			gameObject3.CustomSetActive(false);
			gameObject4.CustomSetActive(false);
			gameObject5.CustomSetActive(false);
			obj.CustomSetActive(false);
			if (masterRoleInfo.IsHaveHeroSkin(skinCfg.dwHeroID, skinCfg.dwSkinID, false))
			{
				obj.CustomSetActive(true);
				return;
			}
			if (masterRoleInfo.IsCanBuySkinButNotHaveHero(skinCfg.dwHeroID, skinCfg.dwSkinID))
			{
				gameObject5.CustomSetActive(true);
				if (gameObject5 != null)
				{
					Text componetInChild = Utility.GetComponetInChild<Text>(gameObject5, "linkBtn/Text");
					CUIEventScript componetInChild2 = Utility.GetComponetInChild<CUIEventScript>(gameObject5, "linkBtn");
					if (componetInChild == null || componetInChild2 == null)
					{
						return;
					}
					componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.openHeroFormPar.heroId = skinCfg.dwHeroID;
					eventParams.openHeroFormPar.skinId = skinCfg.dwSkinID;
					eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
					componetInChild2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
				}
				return;
			}
			ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(skinCfg.dwID);
			stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(skinCfg, skinPromotion);
			for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
			{
				GameObject gameObject6 = null;
				stPayInfo stPayInfo = payInfoSetOfGood.m_payInfos[i];
				switch (stPayInfo.m_payType)
				{
				case enPayType.GoldCoin:
					gameObject.CustomSetActive(true);
					gameObject6 = gameObject;
					break;
				case enPayType.DianQuan:
					gameObject3.CustomSetActive(true);
					gameObject6 = gameObject3;
					break;
				case enPayType.Diamond:
				case enPayType.DiamondAndDianQuan:
					gameObject2.CustomSetActive(true);
					gameObject6 = gameObject2;
					break;
				}
				if (gameObject6 != null)
				{
					Transform transform = gameObject6.transform.Find("BuyButton");
					if (transform != null)
					{
						stUIEventParams stUIEventParams = default(stUIEventParams);
						stUIEventParams.tag = (int)stPayInfo.m_payType;
						stUIEventParams.commonUInt32Param1 = stPayInfo.m_payValue;
						stUIEventParams.heroSkinParam.heroId = skinCfg.dwHeroID;
						stUIEventParams.heroSkinParam.skinId = skinCfg.dwSkinID;
						CMallSystem.SetPayButton(mallForm, transform as RectTransform, stPayInfo.m_payType, stPayInfo.m_payValue, enUIEventID.Mall_Recommend_On_Buy, ref stUIEventParams);
					}
				}
			}
			if (gameObject4 != null)
			{
				ResHeroSkinShop resHeroSkinShop = null;
				GameDataMgr.skinShopInfoDict.TryGetValue(skinCfg.dwID, out resHeroSkinShop);
				if (resHeroSkinShop != null)
				{
					uint num = (resHeroSkinShop.bIsBuyItem <= 0) ? 0u : resHeroSkinShop.dwBuyItemCnt;
					Transform transform2 = gameObject4.transform.Find("BuyButton");
					stUIEventParams stUIEventParams2 = default(stUIEventParams);
					stUIEventParams2.tag = 2;
					stUIEventParams2.commonUInt32Param1 = num;
					stUIEventParams2.heroSkinParam.heroId = skinCfg.dwHeroID;
					stUIEventParams2.heroSkinParam.skinId = skinCfg.dwSkinID;
					if (num > 0u)
					{
						gameObject4.CustomSetActive(true);
						this.SetExchangeButton(mallForm, transform2 as RectTransform, enExchangeType.SkinExchangeCoupons, num, enUIEventID.Mall_Recommend_On_Exchange, ref stUIEventParams2);
					}
				}
			}
		}

		public void SetExchangeButton(CUIFormScript formScript, RectTransform buttonTransform, enExchangeType exchangeType, uint exchangeValue, enUIEventID eventID, ref stUIEventParams eventParams)
		{
			if (formScript == null || buttonTransform == null)
			{
				return;
			}
			if (exchangeValue == 0u)
			{
				return;
			}
			Transform transform = buttonTransform.FindChild("Image");
			if (transform != null)
			{
				Image component = transform.gameObject.GetComponent<Image>();
				if (component != null)
				{
					component.SetSprite(this.GetExchangeTypeIconPath(exchangeType), formScript, true, false, false, false);
				}
			}
			Transform transform2 = buttonTransform.FindChild("Text");
			if (transform2 != null)
			{
				Text component2 = transform2.gameObject.GetComponent<Text>();
				if (component2 != null)
				{
					component2.text = exchangeValue.ToString();
				}
			}
			CUIEventScript component3 = buttonTransform.gameObject.GetComponent<CUIEventScript>();
			if (component3 != null)
			{
				component3.SetUIEvent(enUIEventType.Click, eventID, eventParams);
			}
		}

		public string GetExchangeTypeIconPath(enExchangeType type)
		{
			if (type != enExchangeType.HeroExchangeCoupons)
			{
				if (type == enExchangeType.SkinExchangeCoupons)
				{
					ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(135u);
					DebugHelper.Assert(dataByKey != null, "global cfg databin err: skin exchange id doesnt exist");
					if (dataByKey == null)
					{
						return null;
					}
					uint dwConfValue = dataByKey.dwConfValue;
					CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue, 0);
					if (cUseable != null)
					{
						return cUseable.GetIconPath();
					}
				}
			}
			else
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(134u);
				DebugHelper.Assert(dataByKey != null, "global cfg databin err: hero exchange id doesnt exist");
				if (dataByKey == null)
				{
					return null;
				}
				uint dwConfValue = dataByKey.dwConfValue;
				CUseable cUseable2 = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue, 0);
				if (cUseable2 != null)
				{
					return cUseable2.GetIconPath();
				}
			}
			return null;
		}

		private void Refresh3DModel(uint heroID, int skinId)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/3DImage");
			DebugHelper.Assert(gameObject != null);
			if (gameObject == null)
			{
				return;
			}
			CUI3DImageScript component = gameObject.GetComponent<CUI3DImageScript>();
			ObjNameData heroPrefabPath = CUICommonSystem.GetHeroPrefabPath(heroID, skinId, true);
			string objectName = heroPrefabPath.ObjectName;
			if (!string.IsNullOrEmpty(this.m_heroModelPath))
			{
				component.RemoveGameObject(this.m_heroModelPath);
			}
			this.m_heroModelPath = objectName;
			GameObject gameObject2 = component.AddGameObject(this.m_heroModelPath, false, false);
			if (gameObject2 == null)
			{
				return;
			}
			gameObject2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			if (heroPrefabPath.ActorInfo != null)
			{
				gameObject2.transform.localScale = new Vector3(heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale);
			}
			CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
			instance.Set3DModel(gameObject2);
			instance.InitAnimatList();
			instance.InitAnimatSoundList(heroID, (uint)skinId);
			instance.OnModePlayAnima("Come");
		}

		private void RefreshTimerView(Transform pnlRefreshTrans)
		{
			if (pnlRefreshTrans == null)
			{
				return;
			}
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(pnlRefreshTrans.gameObject, "refreshTimer");
			int num = (int)((long)this.m_MostRecentRecommendRefreshTime - (long)((ulong)currentUTCTime));
			if (num > 0 && componetInChild != null)
			{
				pnlRefreshTrans.gameObject.CustomSetActive(true);
				componetInChild.SetTotalTime((float)num);
				componetInChild.ReStartTimer();
			}
			else
			{
				pnlRefreshTrans.gameObject.CustomSetActive(false);
			}
		}

		public static void TryToExchange(enExchangePurpose exchangePurpose, string goodName, enExchangeType exchangeType, uint exchangeValue, enUIEventID confirmEventID, ref stUIEventParams confirmEventParams, enUIEventID cancelEventID, bool needConfirm, bool guideToAchieveDianQuan)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			uint currencyValueFromRoleInfo = CMallRecommendController.GetCurrencyValueFromRoleInfo(masterRoleInfo, exchangeType);
			if (currencyValueFromRoleInfo >= exchangeValue)
			{
				string empty = string.Empty;
				if (needConfirm)
				{
					string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPay", new string[]
					{
						exchangeValue.ToString(),
						Singleton<CTextManager>.GetInstance().GetText(CMallRecommendController.s_exchangeTypeNameKeys[(int)exchangeType]),
						Singleton<CTextManager>.GetInstance().GetText(CMallRecommendController.s_exchangePurposeNameKeys[(int)exchangePurpose]),
						goodName,
						empty
					}), new object[0]);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, confirmEventID, cancelEventID, confirmEventParams, false);
				}
				else
				{
					CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
					uIEvent.m_eventID = confirmEventID;
					uIEvent.m_eventParams = confirmEventParams;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
				}
			}
			else
			{
				string strContent2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("CurrencyNotEnough"), Singleton<CTextManager>.GetInstance().GetText(CMallRecommendController.s_exchangeTypeNameKeys[(int)exchangeType]));
				Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent2, cancelEventID, false);
			}
		}

		public static uint GetCurrencyValueFromRoleInfo(CRoleInfo roleInfo, enExchangeType exchangeType)
		{
			if (exchangeType != enExchangeType.HeroExchangeCoupons)
			{
				if (exchangeType != enExchangeType.SkinExchangeCoupons)
				{
					return 0u;
				}
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(135u);
				DebugHelper.Assert(dataByKey != null, "global cfg databin err: skin exchange id doesnt exist");
				if (dataByKey == null)
				{
					return 0u;
				}
				uint dwConfValue = dataByKey.dwConfValue;
				CUseableContainer useableContainer = roleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
				return (uint)useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue);
			}
			else
			{
				ResGlobalInfo dataByKey2 = GameDataMgr.globalInfoDatabin.GetDataByKey(134u);
				DebugHelper.Assert(dataByKey2 != null, "global cfg databin err: hero exchange id doesnt exist");
				if (dataByKey2 == null)
				{
					return 0u;
				}
				uint dwConfValue2 = dataByKey2.dwConfValue;
				CUseableContainer useableContainer2 = roleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
				return (uint)useableContainer2.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue2);
			}
		}

		private void OnNtyAddSkin(uint heroID, uint skinID, uint addReason)
		{
			if (addReason == 4u)
			{
				CUICommonSystem.ShowNewHeroOrSkin(heroID, skinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
			}
			this.RefreshRecommendListPnl();
		}

		private void OnNtyAddHero(uint heroID)
		{
			this.RefreshRecommendListPnl();
		}

		private void OnItemAdd()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm != null && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				CMallRecommendController.Tab curTab = this.CurTab;
				if (curTab != CMallRecommendController.Tab.Hero)
				{
					if (curTab == CMallRecommendController.Tab.Skin)
					{
						this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.SkinExchangeCoupons);
					}
				}
				else
				{
					this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.HeroExchangeCoupons);
				}
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRecommendController.sMallHeroExchangeListPath);
			if (form != null)
			{
				CMallRecommendController.ListTab curListTab = this.CurListTab;
				if (curListTab != CMallRecommendController.ListTab.Hero)
				{
					if (curListTab == CMallRecommendController.ListTab.Skin)
					{
						this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.SkinExchangeCoupons);
					}
				}
				else
				{
					this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.HeroExchangeCoupons);
				}
			}
		}

		private void RefreshRecommendList(enExchangeType type)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRecommendController.sMallHeroExchangeListPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "Panel/HeroList");
			GameObject gameObject2 = Utility.FindChild(form.gameObject, "Panel/SkinList");
			if (gameObject == null || gameObject2 == null)
			{
				return;
			}
			if (type != enExchangeType.HeroExchangeCoupons)
			{
				if (type == enExchangeType.SkinExchangeCoupons)
				{
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
					CUIListScript component = gameObject2.GetComponent<CUIListScript>();
					if (component == null)
					{
						return;
					}
					component.SetElementAmount(this.m_CurrentRecommendSkins.Count);
				}
			}
			else
			{
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
				CUIListScript component2 = gameObject.GetComponent<CUIListScript>();
				if (component2 == null)
				{
					return;
				}
				component2.SetElementAmount(this.m_CurrentRecommendHeros.Count);
			}
		}

		public void RefreshRecommendListPnl()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRecommendController.sMallHeroExchangeListPath);
			if (form == null)
			{
				return;
			}
			CRoleInfo role = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(role != null, "master roleInfo is null");
			if (role == null)
			{
				return;
			}
			CMallRecommendController.ListTab curListTab = this.CurListTab;
			if (curListTab != CMallRecommendController.ListTab.Hero)
			{
				if (curListTab == CMallRecommendController.ListTab.Skin)
				{
					this.m_CurrentRecommendSkins.Sort(delegate(ResHeroSkin l, ResHeroSkin r)
					{
						bool flag = false;
						bool flag2 = false;
						if (role.IsHaveHeroSkin(l.dwHeroID, l.dwSkinID, false))
						{
							flag = true;
						}
						if (role.IsHaveHeroSkin(r.dwHeroID, r.dwSkinID, false))
						{
							flag2 = true;
						}
						if (flag && flag2)
						{
							return -1;
						}
						if (flag)
						{
							return 1;
						}
						if (flag2)
						{
							return -1;
						}
						return -1;
					});
					this.RefreshRecommendList(enExchangeType.SkinExchangeCoupons);
					this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.SkinExchangeCoupons);
				}
			}
			else
			{
				this.m_CurrentRecommendHeros.Sort(delegate(ResHeroCfgInfo l, ResHeroCfgInfo r)
				{
					bool flag = false;
					bool flag2 = false;
					if (role.IsHaveHero(l.dwCfgID, false))
					{
						flag = true;
					}
					if (role.IsHaveHero(r.dwCfgID, false))
					{
						flag2 = true;
					}
					if (flag && flag2)
					{
						return -1;
					}
					if (flag)
					{
						return 1;
					}
					if (flag2)
					{
						return -1;
					}
					return -1;
				});
				this.RefreshRecommendList(enExchangeType.HeroExchangeCoupons);
				this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.HeroExchangeCoupons);
			}
		}

		private void OnMallAppear(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcFormScript == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen || Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Recommend)
			{
				return;
			}
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					this.UpdateSkinView();
				}
			}
			else
			{
				this.UpdateHeroView();
			}
		}

		private void OnRecommendTabChange(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseTips();
			CUICommonSystem.CloseCommonTips();
			CUICommonSystem.CloseUseableTips();
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Recommend_Hero_Skill_Up);
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.CurTab = (CMallRecommendController.Tab)selectedIndex;
			CMallRecommendController.Tab curTab = this.m_CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					this.UpdateSkinView();
					this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.SkinExchangeCoupons);
					this.RefreshPoolView();
				}
			}
			else
			{
				this.UpdateHeroView();
				this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.HeroExchangeCoupons);
				this.RefreshPoolView();
			}
		}

		private void OnRecommendListTabChange(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseTips();
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRecommendController.sMallHeroExchangeListPath);
			if (form == null)
			{
				return;
			}
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.CurListTab = (CMallRecommendController.ListTab)selectedIndex;
			this.RefreshRecommendListPnl();
		}

		private void OnRecommendListHeroEnable(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcFormScript == null || srcWidget == null || srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_CurrentRecommendHeros.Count)
			{
				return;
			}
			ResHeroCfgInfo resHeroCfgInfo = this.m_CurrentRecommendHeros[srcWidgetIndexInBelongedList];
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(resHeroCfgInfo.dwCfgID, out resHeroShop);
			if (resHeroShop != null && (resHeroShop.bIsBuyItem == 0 || resHeroShop.dwBuyItemCnt == 0u))
			{
				Debug.LogError(string.Format("hero {0} exchange not supported", resHeroCfgInfo.dwCfgID));
				return;
			}
			uint key = 0u;
			long doubleKey = GameDataMgr.GetDoubleKey(4u, resHeroCfgInfo.dwCfgID);
			if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out key))
			{
				Debug.LogError("recommend product not found");
				return;
			}
			ResSaleRecommend resSaleRecommend = new ResSaleRecommend();
			if (!GameDataMgr.recommendProductDict.TryGetValue(key, out resSaleRecommend))
			{
				Debug.LogError("recommend product not found");
				return;
			}
			Transform transform = srcWidget.transform;
			Transform transform2 = transform.Find("heroItem");
			GameObject gameObject = transform2.Find("profession").gameObject;
			CUICommonSystem.SetHeroItemImage(srcFormScript, transform2.gameObject, StringHelper.UTF8BytesToString(ref this.m_CurrentRecommendHeros[srcWidgetIndexInBelongedList].szImagePath), enHeroHeadType.enBust, false, true);
			CUICommonSystem.SetHeroJob(srcFormScript, gameObject, (enHeroJobType)resHeroCfgInfo.bMainJob);
			Text component = transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
			component.text = StringHelper.UTF8BytesToString(ref resHeroCfgInfo.szName);
			CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = resHeroCfgInfo.dwCfgID;
			eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
			component2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
			component2.m_closeFormWhenClicked = true;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
				return;
			}
			GameObject gameObject2 = transform.Find("imgExperienceMark").gameObject;
			GameObject gameObject3 = Utility.FindChild(transform2.gameObject, "heroDataPanel/heroPricePanel");
			if (gameObject3 == null)
			{
				DebugHelper.Assert(gameObject3 != null, "price panel is null");
				return;
			}
			gameObject3.CustomSetActive(false);
			Transform transform3 = transform.Find("ButtonGroup/BuyBtn");
			Button component3 = transform3.GetComponent<Button>();
			Text component4 = transform3.Find("Text").GetComponent<Text>();
			CUIEventScript component5 = transform3.GetComponent<CUIEventScript>();
			transform3.gameObject.CustomSetActive(false);
			component5.enabled = false;
			component3.enabled = false;
			if (masterRoleInfo.IsHaveHero(resHeroCfgInfo.dwCfgID, false))
			{
				transform3.gameObject.CustomSetActive(true);
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own");
				gameObject2.CustomSetActive(false);
				CUICommonSystem.PlayAnimator(srcWidget, "OnlyName");
			}
			else
			{
				gameObject2.CustomSetActive(masterRoleInfo.IsValidExperienceHero(resHeroCfgInfo.dwCfgID));
				gameObject3.CustomSetActive(true);
				Image componetInChild = Utility.GetComponetInChild<Image>(gameObject3, "pnlExchange/costImage");
				Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject3, "pnlExchange/costText");
				if (componetInChild == null || componetInChild2 == null)
				{
					return;
				}
				componetInChild.SetSprite(this.GetExchangeTypeIconPath(enExchangeType.HeroExchangeCoupons), srcFormScript, true, false, false, false);
				if (resHeroShop != null)
				{
					componetInChild2.text = resHeroShop.dwBuyItemCnt.ToString();
				}
				transform3.gameObject.CustomSetActive(true);
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Exchange_Btn");
				component5.enabled = true;
				component3.enabled = true;
				stUIEventParams eventParams2 = default(stUIEventParams);
				eventParams2.tag = 1;
				if (resHeroShop != null)
				{
					eventParams2.commonUInt32Param1 = resHeroShop.dwBuyItemCnt;
				}
				eventParams2.heroSkinParam.heroId = resHeroCfgInfo.dwCfgID;
				component5.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Recommend_On_Exchange, eventParams2);
			}
		}

		private void OnRecommendListSkinEnable(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcFormScript == null || srcWidget == null || srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_CurrentRecommendSkins.Count)
			{
				return;
			}
			ResHeroSkin resHeroSkin = this.m_CurrentRecommendSkins[srcWidgetIndexInBelongedList];
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(resHeroSkin.dwID, out resHeroSkinShop);
			if (resHeroSkinShop != null && (resHeroSkinShop.bIsBuyItem == 0 || resHeroSkinShop.dwBuyItemCnt == 0u))
			{
				Debug.LogError(string.Format("skin {0} exchange not supported", resHeroSkin.dwID));
				return;
			}
			uint key = 0u;
			long doubleKey = GameDataMgr.GetDoubleKey(7u, resHeroSkin.dwID);
			if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out key))
			{
				Debug.LogError("recommend product not found");
				return;
			}
			ResSaleRecommend resSaleRecommend = new ResSaleRecommend();
			if (!GameDataMgr.recommendProductDict.TryGetValue(key, out resSaleRecommend))
			{
				Debug.LogError("recommend product not found");
				return;
			}
			Transform transform = srcWidget.transform;
			Transform transform2 = transform.Find("heroItem");
			Text component = transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
			Text component2 = transform2.Find("heroDataPanel/heroNamePanel/heroSkinText").GetComponent<Text>();
			CUICommonSystem.SetHeroItemImage(srcFormScript, transform2.gameObject, resHeroSkin.szSkinPicID, enHeroHeadType.enBust, false, true);
			Transform transform3 = transform2.Find("skinLabelImage");
			CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, transform3.gameObject, resHeroSkin.dwHeroID, resHeroSkin.dwSkinID);
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resHeroSkin.dwHeroID);
			if (dataByKey != null)
			{
				component.text = dataByKey.szName;
			}
			component2.text = resHeroSkin.szSkinName;
			CUIEventScript component3 = transform2.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = resHeroSkin.dwHeroID;
			eventParams.openHeroFormPar.skinId = resHeroSkin.dwSkinID;
			eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
			component3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
			component3.m_closeFormWhenClicked = true;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
				return;
			}
			Transform transform4 = transform.Find("ButtonGroup/BuyBtn");
			Button component4 = transform4.GetComponent<Button>();
			Text component5 = transform4.Find("Text").GetComponent<Text>();
			CUIEventScript component6 = transform4.GetComponent<CUIEventScript>();
			transform4.gameObject.CustomSetActive(false);
			component6.enabled = false;
			component4.enabled = false;
			GameObject gameObject = Utility.FindChild(transform2.gameObject, "heroDataPanel/heroPricePanel");
			if (gameObject == null)
			{
				DebugHelper.Assert(gameObject != null, "price panel is null");
				return;
			}
			gameObject.CustomSetActive(false);
			GameObject gameObject2 = transform.Find("imgExperienceMark").gameObject;
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			if (masterRoleInfo.IsHaveHeroSkin(resHeroSkin.dwHeroID, resHeroSkin.dwSkinID, false))
			{
				transform4.gameObject.CustomSetActive(true);
				component5.text = instance.GetText("Mall_Skin_State_Own");
				gameObject2.CustomSetActive(false);
			}
			else
			{
				gameObject2.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(resHeroSkin.dwHeroID, resHeroSkin.dwSkinID));
				if (masterRoleInfo.IsCanBuySkinButNotHaveHero(resHeroSkin.dwHeroID, resHeroSkin.dwSkinID))
				{
					gameObject.CustomSetActive(false);
					component6.enabled = true;
					transform4.gameObject.CustomSetActive(true);
					component5.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
					component4.enabled = true;
					component6.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
					component6.m_closeFormWhenClicked = true;
					return;
				}
				gameObject.CustomSetActive(true);
				Image componetInChild = Utility.GetComponetInChild<Image>(gameObject, "pnlExchange/costImage");
				Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "pnlExchange/costText");
				if (componetInChild == null || componetInChild2 == null)
				{
					return;
				}
				componetInChild.SetSprite(this.GetExchangeTypeIconPath(enExchangeType.SkinExchangeCoupons), srcFormScript, true, false, false, false);
				if (resHeroSkinShop != null)
				{
					componetInChild2.text = resHeroSkinShop.dwBuyItemCnt.ToString();
				}
				component6.enabled = true;
				transform4.gameObject.CustomSetActive(true);
				component5.text = Singleton<CTextManager>.GetInstance().GetText("Exchange_Btn");
				component4.enabled = true;
				stUIEventParams eventParams2 = default(stUIEventParams);
				eventParams2.tag = 2;
				if (resHeroSkinShop != null)
				{
					eventParams2.commonUInt32Param1 = resHeroSkinShop.dwBuyItemCnt;
				}
				eventParams2.heroSkinParam.heroId = resHeroSkin.dwHeroID;
				eventParams2.heroSkinParam.skinId = resHeroSkin.dwSkinID;
				component6.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Recommend_On_Exchange, eventParams2);
			}
		}

		private void OnRecommendTimerEnd(CUIEvent uiEvent)
		{
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidget != null)
			{
				Transform parent = srcWidget.transform.parent;
				if (parent != null)
				{
					parent.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnRecommendBuy(CUIEvent uiEvent)
		{
			enPayType tag = (enPayType)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
					DebugHelper.Assert(heroSkin != null);
					CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref heroSkin.szSkinName), tag, commonUInt32Param, enUIEventID.Mall_Recommend_On_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
				}
			}
			else
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
				DebugHelper.Assert(dataByKey != null);
				CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, commonUInt32Param, enUIEventID.Mall_Recommend_On_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHero;
			uIEvent.m_eventParams.heroId = heroId;
		}

		private void OnRecommendExchange(CUIEvent uiEvent)
		{
			enExchangeType tag = (enExchangeType)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			enExchangeType enExchangeType = tag;
			if (enExchangeType != enExchangeType.HeroExchangeCoupons)
			{
				if (enExchangeType == enExchangeType.SkinExchangeCoupons)
				{
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
					DebugHelper.Assert(heroSkin != null);
					CMallRecommendController.TryToExchange(enExchangePurpose.Exchange, StringHelper.UTF8BytesToString(ref heroSkin.szSkinName), tag, commonUInt32Param, enUIEventID.Mall_Recommend_On_Exchange_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, false);
				}
			}
			else
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
				DebugHelper.Assert(dataByKey != null);
				CMallRecommendController.TryToExchange(enExchangePurpose.Exchange, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, commonUInt32Param, enUIEventID.Mall_Recommend_On_Exchange_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, false);
			}
		}

		private void OnRecommendBuyConfirm(CUIEvent uiEvent)
		{
			enPayType tag = (enPayType)uiEvent.m_eventParams.tag;
			CS_SALERECMD_BUYTYPE buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_COUPONS;
			switch (tag)
			{
			case enPayType.GoldCoin:
				buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_COIN;
				break;
			case enPayType.DianQuan:
				buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_COUPONS;
				break;
			case enPayType.Diamond:
			case enPayType.DiamondAndDianQuan:
				buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_DIAMOND;
				break;
			}
			uint recommendProductID = 0u;
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					uint skinCfgId = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
					long doubleKey = GameDataMgr.GetDoubleKey(7u, skinCfgId);
					if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out recommendProductID))
					{
						return;
					}
				}
			}
			else
			{
				long doubleKey = GameDataMgr.GetDoubleKey(4u, uiEvent.m_eventParams.heroSkinParam.heroId);
				if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out recommendProductID))
				{
					return;
				}
			}
			this.ReqBuy(recommendProductID, buyType);
		}

		private void OnRecommendExchangeMore(CUIEvent uiEvent)
		{
			if (!this.RefreshPoolData(0u))
			{
				Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
				return;
			}
			if (!this.RefreshRecommendData())
			{
				Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenForm(CMallRecommendController.sMallHeroExchangeListPath, false, true);
			this.InitListTab();
		}

		private void OnRecommendExchangeConfirm(CUIEvent uiEvent)
		{
			enExchangeType tag = (enExchangeType)uiEvent.m_eventParams.tag;
			uint recommendProductID = 0u;
			CS_SALERECMD_BUYTYPE buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_EXCHANGE;
			enExchangeType enExchangeType = tag;
			if (enExchangeType != enExchangeType.HeroExchangeCoupons)
			{
				if (enExchangeType == enExchangeType.SkinExchangeCoupons)
				{
					uint skinCfgId = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
					long doubleKey = GameDataMgr.GetDoubleKey(7u, skinCfgId);
					if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out recommendProductID))
					{
						return;
					}
				}
			}
			else
			{
				long doubleKey = GameDataMgr.GetDoubleKey(4u, uiEvent.m_eventParams.heroSkinParam.heroId);
				if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out recommendProductID))
				{
					return;
				}
			}
			this.ReqBuy(recommendProductID, buyType);
		}

		private void OnLotteryBuy(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					CMallSystem.TryToPay(enPayPurpose.RecommendLottery, string.Empty, CMallSystem.ResBuyTypeToPayType((int)this.m_CurrentLottryCtrl[2].bCostType), this.m_CurrentLottryCtrl[2].dwCostPrice, enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, false, true, false);
				}
			}
			else
			{
				CMallSystem.TryToPay(enPayPurpose.RecommendLottery, string.Empty, CMallSystem.ResBuyTypeToPayType((int)this.m_CurrentLottryCtrl[1].bCostType), this.m_CurrentLottryCtrl[1].dwCostPrice, enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, false, true, false);
			}
		}

		private void OnLotteryBuyConfirm(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1177u);
			CSPKG_CMD_RANDDRAW_REQ cSPKG_CMD_RANDDRAW_REQ = new CSPKG_CMD_RANDDRAW_REQ();
			CMallRecommendController.Tab curTab = this.CurTab;
			if (curTab != CMallRecommendController.Tab.Hero)
			{
				if (curTab == CMallRecommendController.Tab.Skin)
				{
					cSPKG_CMD_RANDDRAW_REQ.dwDrawID = this.m_CurrentLottryCtrl[2].dwDrawID;
				}
			}
			else
			{
				cSPKG_CMD_RANDDRAW_REQ.dwDrawID = this.m_CurrentLottryCtrl[1].dwDrawID;
			}
			cSPkg.stPkgData.stRandDrawReq = cSPKG_CMD_RANDDRAW_REQ;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void HeroSelect_Skill_Down(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/Panel_SkillTip");
			if (gameObject == null)
			{
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "skillNameText");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "SkillDescribeText");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject, "SkillCDText");
			Text componetInChild4 = Utility.GetComponetInChild<Text>(gameObject, "SkillCDText/SkillEnergyCostText");
			GameObject skillPropertyInfoPanel = Utility.FindChild(gameObject, "SkillPropertyInfo");
			CUICommonSystem.RefreshSkillLevelUpProperty(skillPropertyInfoPanel, ref uiEvent.m_eventParams.skillPropertyDesc, uiEvent.m_eventParams.skillSlotId);
			componetInChild.text = uiEvent.m_eventParams.skillTipParam.skillName;
			componetInChild2.text = uiEvent.m_eventParams.skillTipParam.strTipText;
			componetInChild3.text = uiEvent.m_eventParams.skillTipParam.skillCoolDown;
			componetInChild4.text = uiEvent.m_eventParams.skillTipParam.skillEnergyCost;
			ushort[] skillEffect = uiEvent.m_eventParams.skillTipParam.skillEffect;
			if (skillEffect == null)
			{
				return;
			}
			for (int i = 1; i <= 2; i++)
			{
				GameObject gameObject2 = Utility.FindChild(gameObject, string.Format("skillNameText/EffectNode{0}", i));
				if (!(gameObject2 == null))
				{
					if (i <= skillEffect.Length && skillEffect[i - 1] != 0)
					{
						gameObject2.CustomSetActive(true);
						gameObject2.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType)skillEffect[i - 1]), uiEvent.m_srcFormScript, true, false, false, false);
						gameObject2.transform.Find("Text").GetComponent<Text>().text = CSkillData.GetEffectDesc((SkillEffectType)skillEffect[i - 1]);
					}
					else
					{
						gameObject2.CustomSetActive(false);
					}
				}
			}
			gameObject.CustomSetActive(true);
		}

		private void HeroSelect_Skill_Up(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/Panel_SkillTip");
			obj.CustomSetActive(false);
		}

		private void OnMallTabChange()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Recommend_Hero_Skill_Up);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendTabChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnRecommendTimerEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
			this.Clear();
			Utility.FindChild(mallForm.gameObject, "UIScene_Recommend_HeroInfo").CustomSetActive(false);
		}

		private void ReqBuy(uint recommendProductID, CS_SALERECMD_BUYTYPE buyType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1175u);
			cSPkg.stPkgData.stSaleRecmdBuyReq.bBuyType = (byte)buyType;
			cSPkg.stPkgData.stSaleRecmdBuyReq.dwRcmdID = recommendProductID;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1176)]
		public static void ReceiveRecommendBuyRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stSaleRecmdBuyRsp.iResult != 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1176, msg.stPkgData.stSaleRecmdBuyRsp.iResult), false, 1.5f, null, new object[0]);
				return;
			}
			uint dwRcmdID = msg.stPkgData.stSaleRecmdBuyRsp.dwRcmdID;
			ResSaleRecommend resSaleRecommend = new ResSaleRecommend();
			if (GameDataMgr.recommendProductDict.TryGetValue(dwRcmdID, out resSaleRecommend))
			{
				COM_ITEM_TYPE wSaleType = (COM_ITEM_TYPE)resSaleRecommend.wSaleType;
				if (wSaleType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resSaleRecommend.dwSaleID);
					DebugHelper.Assert(dataByKey != null, string.Format("heroCfg databin error, Hero ID{0}", resSaleRecommend.dwSaleID));
					if (dataByKey != null)
					{
						CUICommonSystem.ShowNewHeroOrSkin(dataByKey.dwCfgID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, 0);
					}
					else
					{
						Singleton<CUIManager>.GetInstance().OpenTips(string.Format("heroCfg databin error, Hero ID:{0} not found", resSaleRecommend.dwSaleID), false, 1.5f, null, new object[0]);
					}
				}
			}
		}

		[MessageHandler(1179)]
		public static void ReceiveRecommendSyncIDNTF(CSPkg msg)
		{
			CMallRecommendController instance = Singleton<CMallRecommendController>.GetInstance();
			instance.RefreshPoolData(msg.stPkgData.stSyncRandDraw.dwDrawID);
		}

		[MessageHandler(1178)]
		public static void ReceiveLotteryRsp(CSPkg msg)
		{
			Singleton<CMallSystem>.GetInstance().ToggleActionMask(false, 30f);
			if (msg.stPkgData.stRandDrawRsp.iResult != 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1178, msg.stPkgData.stRandDrawRsp.iResult), false, 1.5f, null, new object[0]);
				return;
			}
			CMallRecommendController instance = Singleton<CMallRecommendController>.GetInstance();
			ResRandDrawInfo resRandDrawInfo = new ResRandDrawInfo();
			if (!GameDataMgr.recommendLotteryCtrlDict.TryGetValue(msg.stPkgData.stRandDrawRsp.dwDrawID, out resRandDrawInfo))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Mall_Recommend_Config_Err_Tips", true, 1f, null, new object[]
				{
					"ctr",
					msg.stPkgData.stRandDrawRsp.dwDrawID
				});
				return;
			}
			uint dwRewardPoolID = resRandDrawInfo.dwRewardPoolID;
			long doubleKey = GameDataMgr.GetDoubleKey(dwRewardPoolID, (uint)msg.stPkgData.stRandDrawRsp.bPoolIdx);
			ResRewardPoolInfo resRewardPoolInfo = new ResRewardPoolInfo();
			if (GameDataMgr.recommendRewardDict.TryGetValue(doubleKey, out resRewardPoolInfo))
			{
				ListView<CUseable> listView = new ListView<CUseable>();
				CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resRewardPoolInfo.stRewardInfo.wItemType, (int)resRewardPoolInfo.stRewardInfo.wItemCnt, resRewardPoolInfo.stRewardInfo.dwItemID);
				if (cUseable != null)
				{
					listView.Add(cUseable);
					CUseable[] array = new CUseable[listView.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = listView[i];
					}
					Singleton<CUIManager>.GetInstance().OpenAwardTip(array, Singleton<CTextManager>.GetInstance().GetText("gotAward"), false, enUIEventID.None, false, false, "Form_Award");
					Singleton<CMallRouletteController>.GetInstance().ShowHeroSkin(listView);
				}
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenTips("Mall_Recommend_Config_Err_Tips", true, 1f, null, new object[]
			{
				"pool",
				doubleKey
			});
		}
	}
}
