using Assets.Scripts.Framework;
using Assets.Scripts.Sound;
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
	public class CMallRouletteController : Singleton<CMallRouletteController>
	{
		public enum Tab
		{
			None = -1,
			DianQuan,
			Diamond
		}

		public enum Roulette_State
		{
			NONE,
			ACCELERATE,
			UNIFORM,
			DECELERATE,
			CONTINUOUS_DRAW,
			SKIP
		}

		public const int LEAST_LOOPS = 2;

		public const int MAX_EXTERN_REWARD_LIST_CNT = 5;

		public const ushort ROULETTE_RULE_ID = 2;

		public const int MAX_LUCK_CNT = 200;

		public const byte ACCELERATE_STEPS = 4;

		public const byte DECELERATE_STEPS = 4;

		public const byte CONTINUOUS_DRAW_MIN_STEPS = 1;

		public const float NORMAL_SPEED = 0.03f;

		public const float SLOWEST_SPEED = 0.1f;

		public const float FASTEST_SPEED = 0.03f;

		private CMallRouletteController.Tab m_CurTab;

		private List<CMallRouletteController.Tab> m_UsedTabs;

		private DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>> m_RewardDic;

		private DictionaryView<uint, ListView<ResDT_LuckyDrawExternReward>> m_ExternRewardDic;

		private ListView<CUseable> m_RewardList;

		private SCPKG_LUCKYDRAW_RSP m_LuckyDrawRsp;

		public Dictionary<uint, uint> m_RewardPoolDic;

		private DictionaryView<uint, ListView<ResRareExchange>> m_CrySatlDic;

		private uint m_nCurCryStalTabID;

		private int m_nTotalCrySalTab;

		private int m_CurLoops;

		private int m_CurSpinIdx;

		private int m_CurRewardIdx;

		private int m_CurSpinCnt;

		private byte m_CurContinousDrawSteps;

		private bool m_GotAllUnusualItems;

		private bool m_IsContinousDraw;

		private bool m_IsClockwise;

		private bool m_IsLuckyBarInited;

		public static int reqSentTimerSeq = -1;

		private CMallRouletteController.Roulette_State m_CurState;

		public static string sMallFormCrystal = "UGUI/Form/System/Mall/Form_Crystal.prefab";

		private int[] m_CrystalItemID = new int[3];

		private string m_tab0ImagePath = CUIUtility.s_Sprite_Dynamic_Icon_Dir + "13020.prefab";

		private string m_tab1ImagePath = CUIUtility.s_Sprite_Dynamic_Icon_Dir + "13021.prefab";

		public CMallRouletteController.Tab CurTab
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

		public override void Init()
		{
			base.Init();
			this.m_RewardDic = new DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>>();
			this.m_ExternRewardDic = new DictionaryView<uint, ListView<ResDT_LuckyDrawExternReward>>();
			this.m_RewardPoolDic = new Dictionary<uint, uint>();
			this.m_CrySatlDic = new DictionaryView<uint, ListView<ResRareExchange>>();
			this.m_UsedTabs = new List<CMallRouletteController.Tab>();
			this.m_CurLoops = 0;
			this.m_CurSpinIdx = 0;
			this.m_CurRewardIdx = 0;
			this.m_CurSpinCnt = 0;
			this.m_CurContinousDrawSteps = 0;
			this.m_GotAllUnusualItems = false;
			this.m_CurState = CMallRouletteController.Roulette_State.NONE;
			this.m_IsContinousDraw = false;
			this.m_LuckyDrawRsp = null;
			this.m_IsLuckyBarInited = false;
			CMallRouletteController.reqSentTimerSeq = -1;
			this.InitCryStalItemID();
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_UsedTabs.Clear();
			this.m_RewardDic.Clear();
			this.m_ExternRewardDic.Clear();
			this.m_RewardPoolDic.Clear();
			this.m_CrySatlDic.Clear();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRouletteTabChange));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOne));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFive));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOneConfirm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFiveConfirm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_Change, new CUIEventManager.OnUIEventHandler(this.OnSpinInterval));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_End, new CUIEventManager.OnUIEventHandler(this.OnSpinEnd));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new CUIEventManager.OnUIEventHandler(this.OnOpenExternRewardTip));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Draw_Extern_Reward, new CUIEventManager.OnUIEventHandler(this.OnDrawExternReward));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Reset_Reward_Draw_Cnt, new CUIEventManager.OnUIEventHandler(this.OnDrawCntReset));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Reward_Item, new CUIEventManager.OnUIEventHandler(this.OnShowRewardItem));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tmp_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnTmpRewardEnable));
			instance.RemoveUIEventListener(enUIEventID.Mall_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseAwardForm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_More, new CUIEventManager.OnUIEventHandler(this.OnCrystalMore));
			instance.RemoveUIEventListener(enUIEventID.Mall_Cystal_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnCrystalListEnable));
			instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchange));
			instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchangeConfirm));
			instance.RemoveUIEventListener(enUIEventID.Mall_CryStal_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnCryStalOnTabChange));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Receive_Roulette_Data, new Action(this.RefreshExternRewards));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Roulette", "pnlRoulette", form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), "pnlRoulette");
			return !(x == null);
		}

		public void Draw(CUIFormScript form)
		{
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRouletteTabChange));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_One, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOne));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_Five, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFive));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_One_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOneConfirm));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_Five_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFiveConfirm));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Spin_Change, new CUIEventManager.OnUIEventHandler(this.OnSpinInterval));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Spin_End, new CUIEventManager.OnUIEventHandler(this.OnSpinEnd));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new CUIEventManager.OnUIEventHandler(this.OnOpenExternRewardTip));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Draw_Extern_Reward, new CUIEventManager.OnUIEventHandler(this.OnDrawExternReward));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Reset_Reward_Draw_Cnt, new CUIEventManager.OnUIEventHandler(this.OnDrawCntReset));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Show_Reward_Item, new CUIEventManager.OnUIEventHandler(this.OnShowRewardItem));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Tmp_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnTmpRewardEnable));
			instance.AddUIEventListener(enUIEventID.Mall_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseAwardForm));
			instance.AddUIEventListener(enUIEventID.Mall_Crystal_More, new CUIEventManager.OnUIEventHandler(this.OnCrystalMore));
			instance.AddUIEventListener(enUIEventID.Mall_Cystal_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnCrystalListEnable));
			instance.AddUIEventListener(enUIEventID.Mall_Crystal_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchange));
			instance.AddUIEventListener(enUIEventID.Mall_Crystal_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchangeConfirm));
			instance.AddUIEventListener(enUIEventID.Mall_CryStal_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnCryStalOnTabChange));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Receive_Roulette_Data, new Action(this.RefreshExternRewards));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
			this.InitElements();
			this.RefreshData(0u);
			this.InitTab();
		}

		public void RefreshData(uint targetPoolId = 0u)
		{
			if (this.m_RewardDic == null)
			{
				this.m_RewardDic = new DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>>();
			}
			if (this.m_ExternRewardDic == null)
			{
				this.m_ExternRewardDic = new DictionaryView<uint, ListView<ResDT_LuckyDrawExternReward>>();
			}
			this.m_RewardDic.Clear();
			this.m_ExternRewardDic.Clear();
			DictionaryView<enPayType, ResLuckyDrawPrice>.Enumerator enumerator = GameDataMgr.mallRoulettePriceDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<enPayType, ResLuckyDrawPrice> current = enumerator.Current;
				ResLuckyDrawPrice value = current.Value;
				if (value != null)
				{
					uint num = value.dwRewardPoolID;
					if (targetPoolId != 0u)
					{
						num = targetPoolId;
					}
					else
					{
						ResDT_LuckyDrawPeriod periodCfg = this.GetPeriodCfg(value);
						if (periodCfg != null)
						{
							num = periodCfg.dwRewardPoolID;
						}
					}
					DictionaryView<long, ResLuckyDrawRewardForClient>.Enumerator enumerator2 = GameDataMgr.mallRouletteRewardDict.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						KeyValuePair<long, ResLuckyDrawRewardForClient> current2 = enumerator2.Current;
						ResLuckyDrawRewardForClient value2 = current2.Value;
						if (value2 != null && value2.dwRewardPoolID == num)
						{
							if (!this.m_RewardDic.ContainsKey((uint)value.bMoneyType))
							{
								this.m_RewardDic.Add((uint)value.bMoneyType, new ListView<ResLuckyDrawRewardForClient>());
							}
							if (!this.m_RewardPoolDic.ContainsKey((uint)value.bMoneyType))
							{
								this.m_RewardPoolDic.Add((uint)value.bMoneyType, num);
							}
							else
							{
								uint num2 = 0u;
								if (this.m_RewardPoolDic.TryGetValue((uint)value.bMoneyType, out num2))
								{
									num2 = num;
								}
							}
							ListView<ResLuckyDrawRewardForClient> listView = new ListView<ResLuckyDrawRewardForClient>();
							if (this.m_RewardDic.TryGetValue((uint)value.bMoneyType, out listView))
							{
								listView.Add(value2);
							}
						}
					}
					DictionaryView<uint, ResLuckyDrawExternReward>.Enumerator enumerator3 = GameDataMgr.mallRouletteExternRewardDict.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						KeyValuePair<uint, ResLuckyDrawExternReward> current3 = enumerator3.Current;
						ResLuckyDrawExternReward value3 = current3.Value;
						if (value3 != null && value3.bMoneyType == value.bMoneyType)
						{
							if (!this.m_ExternRewardDic.ContainsKey((uint)value.bMoneyType))
							{
								this.m_ExternRewardDic.Add((uint)value.bMoneyType, new ListView<ResDT_LuckyDrawExternReward>());
							}
							ListView<ResDT_LuckyDrawExternReward> listView2 = new ListView<ResDT_LuckyDrawExternReward>();
							if (this.m_ExternRewardDic.TryGetValue((uint)value.bMoneyType, out listView2))
							{
								for (int i = 0; i < (int)value3.bExternCnt; i++)
								{
									ResDT_LuckyDrawExternReward item = value3.astReward[i];
									listView2.Add(item);
								}
								listView2.Sort(delegate(ResDT_LuckyDrawExternReward a, ResDT_LuckyDrawExternReward b)
								{
									if (a == null && b == null)
									{
										return 0;
									}
									if (a == null && b != null)
									{
										return -1;
									}
									if (b == null && a != null)
									{
										return 1;
									}
									if (a.dwDrawCnt < b.dwDrawCnt)
									{
										return -1;
									}
									if (a.dwDrawCnt == b.dwDrawCnt)
									{
										return 0;
									}
									if (a.dwDrawCnt > b.dwDrawCnt)
									{
										return 1;
									}
									return -1;
								});
							}
						}
					}
				}
			}
		}

		private ResDT_LuckyDrawPeriod GetPeriodCfg(ResLuckyDrawPrice price)
		{
			ulong num = (ulong)((long)CRoleInfo.GetCurrentUTCTime());
			for (int i = 0; i < price.astLuckyDrawPeriod.Length; i++)
			{
				ulong num2 = price.astLuckyDrawPeriod[i].ullStartDate;
				ulong num3 = price.astLuckyDrawPeriod[i].ullEndDate;
				if (num2 > num3)
				{
					num2 ^= num3;
					num3 ^= num2;
					num2 = (num3 ^ num2);
				}
				if (num2 <= num && num < num3)
				{
					return price.astLuckyDrawPeriod[i];
				}
			}
			return null;
		}

		private void InitTab()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			this.m_UsedTabs.Clear();
			CMallRouletteController.Tab[] array = (CMallRouletteController.Tab[])Enum.GetValues(typeof(CMallRouletteController.Tab));
			byte b = 0;
			while ((int)b < array.Length)
			{
				if (array[(int)b] != CMallRouletteController.Tab.None)
				{
					CMallRouletteController.Tab tab = array[(int)b];
					if (tab != CMallRouletteController.Tab.DianQuan)
					{
						if (tab == CMallRouletteController.Tab.Diamond)
						{
							if (this.m_RewardDic.ContainsKey(10u))
							{
								this.m_UsedTabs.Add(array[(int)b]);
							}
						}
					}
					else if (this.m_RewardDic.ContainsKey(2u))
					{
						this.m_UsedTabs.Add(array[(int)b]);
					}
				}
				b += 1;
			}
			DebugHelper.Assert(this.m_UsedTabs.Count != 0, "夺宝单价设定数据档不对");
			if (this.m_UsedTabs.Count == 0)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(mallForm);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mall_Roulette_Init_Error"), false, 1.5f, null, new object[0]);
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Tab");
			if (componetInChild != null)
			{
				componetInChild.SetElementAmount(this.m_UsedTabs.Count);
				for (int i = 0; i < componetInChild.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = componetInChild.GetElemenet(i);
					if (elemenet != null)
					{
						CMallRouletteController.Tab tab2 = this.m_UsedTabs[i];
						Text component = elemenet.transform.Find("Text").GetComponent<Text>();
						Image component2 = elemenet.transform.Find("Image").GetComponent<Image>();
						RES_SHOPBUY_COINTYPE coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL;
						bool flag = false;
						CMallRouletteController.Tab tab = tab2;
						if (tab != CMallRouletteController.Tab.DianQuan)
						{
							if (tab == CMallRouletteController.Tab.Diamond)
							{
								component.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Roulette_Diamond_Buy_Tab");
								if (component2 != null)
								{
									component2.SetSprite(CMallSystem.GetPayTypeIconPath(enPayType.Diamond), mallForm, true, false, false, false);
								}
								coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND;
								stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.Diamond);
								stPayInfo payInfo2 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.Diamond);
								if (payInfo.m_payValue < payInfo.m_oriValue || payInfo2.m_payValue < payInfo2.m_oriValue)
								{
									flag = true;
								}
							}
						}
						else
						{
							component.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Roulette_DianQuan_Buy_Tab");
							if (component2 != null)
							{
								component2.SetSprite(CMallSystem.GetPayTypeIconPath(enPayType.DianQuan), mallForm, true, false, false, false);
							}
							coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
							stPayInfo payInfo3 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.DianQuan);
							stPayInfo payInfo4 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.DianQuan);
							if (payInfo3.m_payValue < payInfo3.m_oriValue || payInfo4.m_payValue < payInfo4.m_oriValue)
							{
								flag = true;
							}
						}
						if ((this.IsProbabilityDoubled(coinType) || flag) && CUIRedDotSystem.IsShowRedDotByVersion(this.TabToRedID(tab2)))
						{
							CUIRedDotSystem.AddRedDot(elemenet.gameObject, enRedDotPos.enTopRight, 0, 0, 0);
						}
					}
				}
				componetInChild.m_alwaysDispatchSelectedChangeEvent = true;
				if (this.CurTab == CMallRouletteController.Tab.None || this.CurTab < CMallRouletteController.Tab.DianQuan || this.CurTab >= (CMallRouletteController.Tab)componetInChild.GetElementAmount())
				{
					componetInChild.SelectElement(0, true);
				}
				else
				{
					componetInChild.SelectElement((int)this.CurTab, true);
				}
			}
		}

		private void RefreshDrawCnt(enPayType payType, out CSDT_LUCKYDRAW_INFO drawInfo, int drawCnt = -1)
		{
			CMallSystem.luckyDrawDic.TryGetValue(payType, out drawInfo);
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject p = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/PL_Premiums");
			Text componetInChild = Utility.GetComponetInChild<Text>(p, "Top/Value");
			if (componetInChild != null)
			{
				if (drawCnt != -1)
				{
					componetInChild.text = drawCnt.ToString();
				}
				else if (drawInfo != null)
				{
					componetInChild.text = drawInfo.dwCnt.ToString();
				}
			}
		}

		private void RefreshExternRewards()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/PL_Premiums");
			ListView<ResDT_LuckyDrawExternReward> listView = new ListView<ResDT_LuckyDrawExternReward>();
			CSDT_LUCKYDRAW_INFO cSDT_LUCKYDRAW_INFO = new CSDT_LUCKYDRAW_INFO();
			if (gameObject == null)
			{
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "Top/Value");
			if (componetInChild != null)
			{
				CMallRouletteController.Tab curTab = this.CurTab;
				if (curTab != CMallRouletteController.Tab.DianQuan)
				{
					if (curTab == CMallRouletteController.Tab.Diamond)
					{
						this.RefreshDrawCnt(enPayType.Diamond, out cSDT_LUCKYDRAW_INFO, -1);
						this.RefreshLuck(enPayType.Diamond, cSDT_LUCKYDRAW_INFO);
						this.m_ExternRewardDic.TryGetValue(10u, out listView);
					}
				}
				else
				{
					this.RefreshDrawCnt(enPayType.DianQuan, out cSDT_LUCKYDRAW_INFO, -1);
					this.RefreshLuck(enPayType.DianQuan, cSDT_LUCKYDRAW_INFO);
					this.m_ExternRewardDic.TryGetValue(2u, out listView);
				}
			}
			if (listView == null || listView.Count == 0)
			{
				gameObject.CustomSetActive(false);
			}
			else
			{
				gameObject.CustomSetActive(true);
				int count = listView.Count;
				for (byte b = 0; b < 5; b += 1)
				{
					GameObject gameObject2 = Utility.FindChild(gameObject, string.Format("Award{0}", b));
					if ((int)b < listView.Count)
					{
						if (gameObject2 != null)
						{
							gameObject2.CustomSetActive(true);
							string text = Convert.ToString((long)((ulong)cSDT_LUCKYDRAW_INFO.dwReachMask), 2).PadLeft(32, '0');
							string text2 = Convert.ToString((long)((ulong)cSDT_LUCKYDRAW_INFO.dwDrawMask), 2).PadLeft(32, '0');
							byte b2 = 0;
							byte b3 = 0;
							byte.TryParse(text.Substring((int)(32 - (b + 1)), 1), out b2);
							byte.TryParse(text2.Substring((int)(32 - (b + 1)), 1), out b3);
							ResDT_LuckyDrawExternReward resDT_LuckyDrawExternReward = listView[(int)b];
							Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject2, "Value");
							Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject2, "Value/Text");
							if (componetInChild2 != null)
							{
								componetInChild2.text = resDT_LuckyDrawExternReward.dwDrawCnt.ToString();
							}
							if (componetInChild3 != null)
							{
								componetInChild3.text = "个";
							}
							if (b2 > 0 && b3 == 0)
							{
								CUICommonSystem.PlayAnimator(gameObject2, "Premiums_Normal");
							}
							else if (b2 > 0 && b3 > 0)
							{
								CUICommonSystem.PlayAnimator(gameObject2, "Premiums_Disbled");
								if (componetInChild2 != null)
								{
									componetInChild2.text = string.Empty;
								}
								if (componetInChild3 != null)
								{
									componetInChild3.text = "已领取";
								}
							}
							else
							{
								CUICommonSystem.PlayAnimator(gameObject2, "Premiums_Disbled");
							}
							CUIEventScript cUIEventScript = gameObject2.GetComponent<CUIEventScript>();
							if (cUIEventScript == null)
							{
								cUIEventScript = gameObject2.AddComponent<CUIEventScript>();
								cUIEventScript.Initialize(mallForm);
							}
							cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new stUIEventParams
							{
								tag = (int)b
							});
						}
					}
					else if (gameObject2 != null)
					{
						gameObject2.CustomSetActive(false);
					}
				}
			}
		}

		public void RefreshRewards()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			ResLuckyDrawPrice resLuckyDrawPrice = new ResLuckyDrawPrice();
			CMallRouletteController.Tab curTab = this.CurTab;
			if (curTab != CMallRouletteController.Tab.DianQuan)
			{
				if (curTab == CMallRouletteController.Tab.Diamond)
				{
					DebugHelper.Assert(this.m_RewardDic.ContainsKey(10u), "夺宝奖励池数据档错误");
					if (!this.m_RewardDic.ContainsKey(10u))
					{
						return;
					}
					GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.Diamond, out resLuckyDrawPrice);
					ListView<ResLuckyDrawRewardForClient> listView = null;
					this.m_RewardDic.TryGetValue(10u, out listView);
					if (listView == null || listView.Count == 0)
					{
						return;
					}
					this.SetRewardItemCells(listView);
				}
			}
			else
			{
				DebugHelper.Assert(this.m_RewardDic.ContainsKey(2u), "夺宝奖励池数据档错误");
				if (!this.m_RewardDic.ContainsKey(2u))
				{
					return;
				}
				GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.DianQuan, out resLuckyDrawPrice);
				ListView<ResLuckyDrawRewardForClient> listView2 = null;
				this.m_RewardDic.TryGetValue(2u, out listView2);
				if (listView2 == null || listView2.Count == 0)
				{
					return;
				}
				this.SetRewardItemCells(listView2);
			}
		}

		private void RefreshLuck(enPayType payType, CSDT_LUCKYDRAW_INFO drawInfo)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			ResLuckyDrawPrice resLuckyDrawPrice = new ResLuckyDrawPrice();
			int num = 200;
			CMallRouletteController.Tab curTab = this.CurTab;
			if (curTab != CMallRouletteController.Tab.DianQuan)
			{
				if (curTab == CMallRouletteController.Tab.Diamond)
				{
					DebugHelper.Assert(this.m_RewardDic.ContainsKey(10u), "夺宝奖励池数据档错误");
					if (!this.m_RewardDic.ContainsKey(10u))
					{
						mallForm.Close();
						return;
					}
					GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.Diamond, out resLuckyDrawPrice);
				}
			}
			else
			{
				DebugHelper.Assert(this.m_RewardDic.ContainsKey(2u), "夺宝奖励池数据档错误");
				if (!this.m_RewardDic.ContainsKey(2u))
				{
					mallForm.Close();
					return;
				}
				GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.DianQuan, out resLuckyDrawPrice);
			}
			DebugHelper.Assert(resLuckyDrawPrice != null, "商城夺宝配置档有错");
			if (resLuckyDrawPrice != null)
			{
				ResDT_LuckyDrawPeriod periodCfg = this.GetPeriodCfg(resLuckyDrawPrice);
				if (periodCfg != null)
				{
					num = (int)periodCfg.dwPreciousHighCnt;
				}
				else
				{
					num = (int)resLuckyDrawPrice.dwPreciousHighCnt;
				}
			}
			int num2 = 0;
			if (drawInfo != null)
			{
				num2 = (int)Math.Min((long)((ulong)drawInfo.dwLuckyPoint), (long)num);
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/LuckBar");
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/LuckComplete");
			if (this.m_GotAllUnusualItems)
			{
				gameObject.CustomSetActive(false);
				GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/Effect");
				obj2.CustomSetActive(false);
				obj.CustomSetActive(true);
				if (this.m_IsLuckyBarInited)
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_common_gongxi", null);
				}
			}
			else
			{
				gameObject.CustomSetActive(true);
				obj.CustomSetActive(false);
				double num3 = (double)((float)num2 / ((float)num + 40f));
				Image componetInChild = Utility.GetComponetInChild<Image>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/LuckBar/BarBg/Image");
				Image componetInChild2 = Utility.GetComponetInChild<Image>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/LuckBar/BarBg/Bar_Light");
				Text componetInChild3 = Utility.GetComponetInChild<Text>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/LuckBar/Text");
				if (componetInChild != null && componetInChild3 != null)
				{
					float fillAmount = componetInChild.fillAmount;
					componetInChild.fillAmount = (float)num3;
					componetInChild2.fillAmount = (float)num3;
					if (this.m_IsLuckyBarInited && Math.Abs(fillAmount - componetInChild.fillAmount) > 1.401298E-45f)
					{
						CUICommonSystem.PlayAnimator(gameObject, "BarLight_Anim");
					}
					componetInChild3.text = num2.ToString();
				}
			}
			this.m_IsLuckyBarInited = true;
		}

		private void SetRewardItemCells(ListView<ResLuckyDrawRewardForClient> rewardList)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			byte b = 0;
			byte b2 = 0;
			for (byte b3 = 0; b3 < 14; b3 += 1)
			{
				GameObject gameObject = Utility.FindChild(mallForm.gameObject, string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}", b3));
				if (!(gameObject == null))
				{
					ResLuckyDrawRewardForClient resLuckyDrawRewardForClient = rewardList[(int)b3];
					GameObject gameObject2 = Utility.FindChild(gameObject, "Bg");
					Image componetInChild = Utility.GetComponetInChild<Image>(gameObject, "icon");
					GameObject gameObject3 = Utility.FindChild(gameObject, "tag");
					GameObject gameObject4 = Utility.FindChild(gameObject, "XiYou");
					Image componetInChild2 = Utility.GetComponetInChild<Image>(gameObject, "tag");
					CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resLuckyDrawRewardForClient.dwItemType, (int)resLuckyDrawRewardForClient.dwItemCnt, resLuckyDrawRewardForClient.dwItemID);
					Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject, "cntBg/cnt");
					GameObject obj = Utility.FindChild(gameObject, "cntBg");
					Text componetInChild4 = Utility.GetComponetInChild<Text>(gameObject, "Name");
					GameObject gameObject5 = Utility.FindChild(gameObject, "imgExperienceMark");
					GameObject obj2 = Utility.FindChild(gameObject, "probabilityDoubled");
					CUIEventScript cUIEventScript = gameObject.GetComponent<CUIEventScript>();
					if (cUIEventScript == null)
					{
						cUIEventScript = gameObject.AddComponent<CUIEventScript>();
						cUIEventScript.Initialize(mallForm);
					}
					bool flag = false;
					if (resLuckyDrawRewardForClient.dwItemType == 4u)
					{
						stUIEventParams eventParams = default(stUIEventParams);
						eventParams.openHeroFormPar.heroId = resLuckyDrawRewardForClient.dwItemID;
						eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
						cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
						cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.None);
						cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.None);
						cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.None);
						IHeroData heroData = CHeroDataFactory.CreateHeroData(resLuckyDrawRewardForClient.dwItemID);
						if (heroData.bPlayerOwn)
						{
							flag = true;
						}
					}
					else if (resLuckyDrawRewardForClient.dwItemType == 11u)
					{
						stUIEventParams eventParams2 = default(stUIEventParams);
						uint heroId = 0u;
						uint skinId = 0u;
						CSkinInfo.ResolveHeroSkin(resLuckyDrawRewardForClient.dwItemID, out heroId, out skinId);
						eventParams2.openHeroFormPar.heroId = heroId;
						eventParams2.openHeroFormPar.skinId = skinId;
						eventParams2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
						cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams2);
						cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.None);
						cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.None);
						cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.None);
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo != null && masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
						{
							flag = true;
						}
					}
					else
					{
						stUIEventParams eventParams3 = default(stUIEventParams);
						eventParams3.iconUseable = cUseable;
						eventParams3.tag = 0;
						cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams3);
						cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams3);
						cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams3);
						cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams3);
					}
					if (gameObject2 != null)
					{
						if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN || (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(cUseable.m_baseID)) || (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(cUseable.m_baseID)))
						{
							gameObject2.CustomSetActive(true);
						}
						else
						{
							gameObject2.CustomSetActive(false);
						}
					}
					if (componetInChild != null && cUseable != null)
					{
						componetInChild.SetSprite(cUseable.GetIconPath(), mallForm, true, false, false, false);
					}
					if (componetInChild3 != null && cUseable != null)
					{
						if (cUseable.m_stackCount <= 1)
						{
							obj.CustomSetActive(false);
						}
						else
						{
							obj.CustomSetActive(true);
							componetInChild3.text = cUseable.m_stackCount.ToString();
						}
					}
					if (componetInChild4 != null && cUseable != null)
					{
						componetInChild4.text = cUseable.m_name;
					}
					if (resLuckyDrawRewardForClient.dwItemTag == 1u)
					{
						gameObject4.CustomSetActive(true);
						if (gameObject4 != null)
						{
							Transform transform = gameObject4.transform.Find("Text");
							if (transform != null)
							{
								Text component = transform.GetComponent<Text>();
								if (component != null && cUseable != null)
								{
									component.text = cUseable.m_name;
								}
							}
						}
						b += 1;
						if (flag)
						{
							b2 += 1;
						}
					}
					else
					{
						gameObject4.CustomSetActive(false);
					}
					string productTagIconPath = CMallSystem.GetProductTagIconPath((int)resLuckyDrawRewardForClient.dwItemTag, flag);
					if (productTagIconPath == null)
					{
						gameObject3.CustomSetActive(false);
					}
					else
					{
						gameObject3.CustomSetActive(true);
						componetInChild2.SetSprite(productTagIconPath, mallForm, true, false, false, false);
						Text componetInChild5 = Utility.GetComponetInChild<Text>(gameObject3, "Text");
						if (componetInChild5 != null)
						{
							string text = string.Empty;
							switch (resLuckyDrawRewardForClient.dwItemTag)
							{
							case 1u:
							case 4u:
								text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Rare");
								break;
							case 2u:
								text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
								break;
							case 3u:
								text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
								break;
							default:
								text = string.Empty;
								break;
							}
							if (flag)
							{
								componetInChild5.text = "拥有";
							}
							else
							{
								componetInChild5.text = text;
							}
						}
					}
					if (gameObject5 != null)
					{
						gameObject5.CustomSetActive(false);
						Image component2 = gameObject5.GetComponent<Image>();
						if (component2 != null)
						{
							if (CItem.IsHeroExperienceCard(resLuckyDrawRewardForClient.dwItemID))
							{
								gameObject5.CustomSetActive(true);
								component2.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
							}
							else if (CItem.IsSkinExperienceCard(resLuckyDrawRewardForClient.dwItemID))
							{
								gameObject5.CustomSetActive(true);
								component2.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
							}
						}
					}
					if (resLuckyDrawRewardForClient.bShowProbabilityDoubled > 0)
					{
						obj2.CustomSetActive(true);
					}
					else
					{
						obj2.CustomSetActive(false);
					}
				}
			}
			if (b == b2)
			{
				this.m_GotAllUnusualItems = true;
			}
			else
			{
				this.m_GotAllUnusualItems = false;
			}
		}

		private enRedID TabToRedID(CMallRouletteController.Tab tab)
		{
			if (tab == CMallRouletteController.Tab.DianQuan)
			{
				return enRedID.Mall_Roulette_Coupons_Tab;
			}
			if (tab != CMallRouletteController.Tab.Diamond)
			{
				return enRedID.Mall_Roulette_Coupons_Tab;
			}
			return enRedID.Mall_Roulette_Diamond_Tab;
		}

		private void RefreshButtonView()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			this.ToggleBtnGroup(true);
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/btnGroup/btnBuyOne");
			GameObject gameObject2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/btnGroup/btnBuyFive");
			if (gameObject == null || gameObject2 == null)
			{
				return;
			}
			stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.None);
			stPayInfo payInfo2 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.None);
			stUIEventParams stUIEventParams = default(stUIEventParams);
			ResLuckyDrawPrice resLuckyDrawPrice = new ResLuckyDrawPrice();
			CMallRouletteController.Tab curTab = this.CurTab;
			if (curTab != CMallRouletteController.Tab.DianQuan)
			{
				if (curTab == CMallRouletteController.Tab.Diamond)
				{
					GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.Diamond, out resLuckyDrawPrice);
				}
			}
			else
			{
				GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.DianQuan, out resLuckyDrawPrice);
			}
			CMallSystem.SetPayButton(mallForm, gameObject.transform as RectTransform, payInfo.m_payType, payInfo.m_payValue, payInfo.m_oriValue, enUIEventID.Mall_Roulette_Buy_One, ref stUIEventParams);
			CMallSystem.SetPayButton(mallForm, gameObject2.transform as RectTransform, payInfo.m_payType, payInfo2.m_payValue, payInfo2.m_oriValue, enUIEventID.Mall_Roulette_Buy_Five, ref stUIEventParams);
		}

		private void RefreshTimer()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			ListView<ResDT_LuckyDrawExternReward> listView = new ListView<ResDT_LuckyDrawExternReward>();
			CMallRouletteController.Tab curTab = this.CurTab;
			if (curTab != CMallRouletteController.Tab.DianQuan)
			{
				if (curTab == CMallRouletteController.Tab.Diamond)
				{
					this.m_ExternRewardDic.TryGetValue(10u, out listView);
				}
			}
			else
			{
				this.m_ExternRewardDic.TryGetValue(2u, out listView);
			}
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/pnlRefresh");
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/pnlRefresh/refreshTimer");
			if (listView == null || listView.Count == 0)
			{
				if (componetInChild != null)
				{
					componetInChild.EndTimer();
				}
				obj.CustomSetActive(false);
			}
			else
			{
				obj.CustomSetActive(true);
				if (componetInChild != null)
				{
					componetInChild.SetTotalTime((float)this.GetNextRefreshTime());
					componetInChild.StartTimer();
				}
			}
		}

		private int GetNextRefreshTime()
		{
			DateTime value = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(103u);
			int num;
			if (value.DayOfWeek - (DayOfWeek)dataByKey.dwConfValue > 0)
			{
				num = 7 - (value.DayOfWeek - (DayOfWeek)dataByKey.dwConfValue);
			}
			else
			{
				num = (int)(dataByKey.dwConfValue - (uint)value.DayOfWeek);
			}
			DateTime value2 = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, DateTimeKind.Utc);
			value2 = value2.AddDays((double)num);
			ResGlobalInfo dataByKey2 = GameDataMgr.globalInfoDatabin.GetDataByKey(104u);
			int num2 = (int)(dataByKey2.dwConfValue / 100u);
			int num3 = (int)(dataByKey2.dwConfValue % 100u);
			value2 = value2.AddSeconds((double)(num2 * 3600) + (double)(num3 * 60));
			if (value.Subtract(value2).TotalSeconds > 0.0)
			{
				value2 = value2.AddDays(7.0);
			}
			return (int)value2.Subtract(value).TotalSeconds;
		}

		public static stPayInfo GetPayInfo(RES_SHOPDRAW_SUBTYPE drawType = RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab tab = CMallRouletteController.Tab.None)
		{
			stPayInfo result = default(stPayInfo);
			result.m_payType = CMallSystem.ResBuyTypeToPayType(10);
			if (tab == CMallRouletteController.Tab.None)
			{
				tab = Singleton<CMallRouletteController>.GetInstance().CurTab;
			}
			CMallRouletteController.Tab tab2 = tab;
			if (tab2 != CMallRouletteController.Tab.DianQuan)
			{
				if (tab2 == CMallRouletteController.Tab.Diamond)
				{
					result.m_payType = CMallSystem.ResBuyTypeToPayType(10);
				}
			}
			else
			{
				result.m_payType = CMallSystem.ResBuyTypeToPayType(2);
			}
			ResLuckyDrawPrice resLuckyDrawPrice = new ResLuckyDrawPrice();
			if (drawType != RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE)
			{
				if (drawType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE)
				{
					if (GameDataMgr.mallRoulettePriceDict.TryGetValue(result.m_payType, out resLuckyDrawPrice))
					{
						result.m_payValue = resLuckyDrawPrice.dwMultiPrice;
						result.m_oriValue = resLuckyDrawPrice.dwMultiPrice;
					}
					else
					{
						result.m_payValue = 4294967295u;
						result.m_oriValue = 4294967295u;
					}
				}
			}
			else if (GameDataMgr.mallRoulettePriceDict.TryGetValue(result.m_payType, out resLuckyDrawPrice))
			{
				result.m_payValue = resLuckyDrawPrice.dwSinglePrice;
				result.m_oriValue = resLuckyDrawPrice.dwSinglePrice;
			}
			else
			{
				result.m_payValue = 4294967295u;
				result.m_oriValue = 4294967295u;
			}
			if (resLuckyDrawPrice != null)
			{
				ulong num = resLuckyDrawPrice.ullSalesStartTime;
				ulong num2 = resLuckyDrawPrice.ullSalesEndTime;
				if (num > num2)
				{
					num ^= num2;
					num2 ^= num;
					num = (num2 ^ num);
				}
				ulong num3 = (ulong)((long)CRoleInfo.GetCurrentUTCTime());
				if (num <= num3 && num3 < num2)
				{
					if (drawType != RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE)
					{
						if (drawType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE)
						{
							result.m_payValue = resLuckyDrawPrice.dwSalesMultiPrice;
						}
					}
					else
					{
						result.m_payValue = resLuckyDrawPrice.dwSalesSinglePrice;
					}
				}
			}
			return result;
		}

		public bool IsProbabilityDoubled(RES_SHOPBUY_COINTYPE coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL)
		{
			if (this.m_RewardDic == null || this.m_RewardDic.Count == 0)
			{
				this.RefreshData(0u);
			}
			if (this.m_RewardDic == null)
			{
				return false;
			}
			DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>>.Enumerator enumerator = this.m_RewardDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (coinType != RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL)
				{
					KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> current = enumerator.Current;
					if (current.Key != (uint)((byte)coinType))
					{
						continue;
					}
				}
				KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> current2 = enumerator.Current;
				if (current2.Value != null)
				{
					KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> current3 = enumerator.Current;
					if (current3.Value.Count > 0)
					{
						KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> current4 = enumerator.Current;
						ListView<ResLuckyDrawRewardForClient> value = current4.Value;
						int count = value.Count;
						for (int i = 0; i < count; i++)
						{
							if (value[i].bShowProbabilityDoubled > 0)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public void InitElements()
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette").CustomSetActive(true);
		}

		public void Spin(int idx = -1)
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("Stop_UI_buy_chou_duobao_zhuan", null);
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_duobao_zhuan", null);
			if (idx == -1)
			{
				this.m_CurSpinCnt = 0;
				this.m_CurState = CMallRouletteController.Roulette_State.ACCELERATE;
				this.m_IsClockwise = true;
			}
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			float totalTime = 10f;
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/timerSpin");
			if (componetInChild != null)
			{
				componetInChild.SetTotalTime(totalTime);
				if (idx == -1)
				{
					componetInChild.SetOnChangedIntervalTime(0.1f);
				}
				componetInChild.m_eventParams[2].tag = idx;
				componetInChild.m_eventParams[1].tag = idx;
				componetInChild.StartTimer();
			}
		}

		public void SetRewardData(SCPKG_LUCKYDRAW_RSP stLuckyDrawRsp)
		{
			this.m_CurLoops = 0;
			this.m_CurRewardIdx = 0;
			this.m_LuckyDrawRsp = stLuckyDrawRsp;
			this.m_RewardList = this.GetRewardUseables(this.m_LuckyDrawRsp);
		}

		public void DisplayTmpRewardList(bool isShow, int amount = 0)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/tmpRewardList");
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(gameObject, "List");
			if (componetInChild != null)
			{
				if (isShow)
				{
					gameObject.CustomSetActive(true);
					GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/Effect");
					obj.CustomSetActive(false);
					if (amount >= 1)
					{
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_duobao_degao", null);
						componetInChild.SetElementAmount(amount);
						componetInChild.MoveElementInScrollArea(amount - 1, false);
					}
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
			else
			{
				gameObject.CustomSetActive(false);
			}
		}

		public ListView<CUseable> GetRewardUseables(SCPKG_LUCKYDRAW_RSP stLuckyDrawRsp)
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			if (stLuckyDrawRsp == null)
			{
				return listView;
			}
			ResLuckyDrawRewardForClient resLuckyDrawRewardForClient = new ResLuckyDrawRewardForClient();
			uint dwRewardPoolID = stLuckyDrawRsp.dwRewardPoolID;
			for (int i = 0; i < (int)stLuckyDrawRsp.bRewardCnt; i++)
			{
				long doubleKey = GameDataMgr.GetDoubleKey(dwRewardPoolID, (uint)stLuckyDrawRsp.szRewardIndex[i]);
				if (GameDataMgr.mallRouletteRewardDict.TryGetValue(doubleKey, out resLuckyDrawRewardForClient))
				{
					CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resLuckyDrawRewardForClient.dwItemType, (int)resLuckyDrawRewardForClient.dwItemCnt, resLuckyDrawRewardForClient.dwItemID);
					if (cUseable != null)
					{
						switch (cUseable.m_type)
						{
						case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
						{
							IHeroData heroData = CHeroDataFactory.CreateHeroData(resLuckyDrawRewardForClient.dwItemID);
							if (heroData.bPlayerOwn)
							{
								CHeroInfoData cHeroInfoData = (CHeroInfoData)heroData;
								ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(134u);
								DebugHelper.Assert(dataByKey != null, "global cfg databin err: hero exchange id doesnt exist");
								if (dataByKey == null)
								{
									return null;
								}
								uint dwConfValue = dataByKey.dwConfValue;
								cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue, (int)((cHeroInfoData.m_info.shopCfgInfo == null) ? 1u : cHeroInfoData.m_info.shopCfgInfo.dwChgItemCnt));
								cUseable.ExtraFromType = 1;
								cUseable.ExtraFromData = (int)resLuckyDrawRewardForClient.dwItemID;
							}
							break;
						}
						case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
						{
							uint heroId = 0u;
							uint skinId = 0u;
							CSkinInfo.ResolveHeroSkin(resLuckyDrawRewardForClient.dwItemID, out heroId, out skinId);
							CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo != null && masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
							{
								ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
								ResGlobalInfo dataByKey2 = GameDataMgr.globalInfoDatabin.GetDataByKey(135u);
								DebugHelper.Assert(dataByKey2 != null, "global cfg databin err: hero skin exchange id doesnt exist");
								if (dataByKey2 == null)
								{
									return null;
								}
								ResHeroSkinShop resHeroSkinShop = null;
								GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out resHeroSkinShop);
								uint dwConfValue2 = dataByKey2.dwConfValue;
								cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue2, (int)((resHeroSkinShop == null) ? 1u : resHeroSkinShop.dwChgItemCnt));
								cUseable.ExtraFromType = 2;
								cUseable.ExtraFromData = (int)resLuckyDrawRewardForClient.dwItemID;
							}
							break;
						}
						}
						if (cUseable != null)
						{
							cUseable.m_itemSortNum = (ulong)resLuckyDrawRewardForClient.dwItemPreciousValue;
							listView.Add(cUseable);
						}
					}
				}
			}
			return listView;
		}

		public void OpenAwardTip(RES_SHOPDRAW_SUBTYPE drawType, ListView<CUseable> items, string title = null, bool playSound = false, enUIEventID eventID = enUIEventID.None, bool displayAll = false)
		{
			if (items == null)
			{
				return;
			}
			int b = 5;
			int num = Mathf.Min(items.Count, b);
			this.m_CurRewardIdx = 0;
			CUIFormScript cUIFormScript;
			if (items.Count < 5)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Roullete_Reward.prefab", false, true);
			}
			else
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Roullete_Reward_Five.prefab", false, true);
			}
			if (cUIFormScript == null)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Error get reward form failed", false, 1.5f, null, new object[0]);
				return;
			}
			CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(cUIFormScript.gameObject, "Panel/Button_Back");
			if (componetInChild != null)
			{
				componetInChild.m_onClickEventID = eventID;
			}
			CUIEventScript componetInChild2 = Utility.GetComponetInChild<CUIEventScript>(cUIFormScript.gameObject, "Panel/CloseBtn");
			if (componetInChild2 != null)
			{
				componetInChild2.m_onClickEventID = eventID;
			}
			GameObject gameObject = null;
			stPayInfo stPayInfo = default(stPayInfo);
			stUIEventParams stUIEventParams = default(stUIEventParams);
			enUIEventID eventID2 = enUIEventID.None;
			if (drawType != RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE)
			{
				if (drawType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE)
				{
					stPayInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.None);
					GameObject obj = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnBuyOne");
					obj.CustomSetActive(false);
					gameObject = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnBuyFive");
					eventID2 = enUIEventID.Mall_Roulette_Buy_Five;
				}
			}
			else
			{
				stPayInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.None);
				GameObject obj2 = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnBuyFive");
				obj2.CustomSetActive(false);
				gameObject = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnBuyOne");
				eventID2 = enUIEventID.Mall_Roulette_Buy_One;
			}
			gameObject.CustomSetActive(true);
			if (gameObject != null)
			{
				CMallSystem.SetPayButton(cUIFormScript, gameObject.transform as RectTransform, stPayInfo.m_payType, stPayInfo.m_payValue, stPayInfo.m_oriValue, eventID2, ref stUIEventParams);
			}
			if (title != null)
			{
				Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "Panel/bg/Title").text = title;
			}
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject2 = cUIFormScript.transform.FindChild(string.Format("Panel/Centent/itemCell{0}", i + 1)).gameObject;
				gameObject2.CustomSetActive(false);
			}
			Transform transform = cUIFormScript.transform.Find("showRewardTimer");
			CUITimerScript cUITimerScript = null;
			if (transform != null)
			{
				cUITimerScript = transform.gameObject.GetComponent<CUITimerScript>();
			}
			if (cUITimerScript == null)
			{
				for (int j = 0; j < num; j++)
				{
					GameObject gameObject3 = cUIFormScript.transform.FindChild(string.Format("Panel/Centent/itemCell{0}", j + 1)).gameObject;
					CUICommonSystem.SetItemCell(cUIFormScript, gameObject3, items[j], true, displayAll, false, false);
					if (items[j].m_itemSortNum > 0uL)
					{
						GameObject obj3 = Utility.FindChild(gameObject3, "Effect_glow");
						obj3.CustomSetActive(true);
					}
					gameObject3.CustomSetActive(true);
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_get_01", null);
					if (playSound)
					{
						COM_REWARDS_TYPE mapRewardType = items[j].MapRewardType;
						if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN)
						{
							if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP)
							{
								if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND)
								{
									Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_diamond", null);
								}
							}
							else
							{
								Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_physical_power", null);
							}
						}
						else
						{
							Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_coin", null);
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < num; k++)
				{
					GameObject gameObject4 = cUIFormScript.transform.FindChild(string.Format("Panel/Centent/itemCell{0}", k + 1)).gameObject;
					CUICommonSystem.SetItemCell(cUIFormScript, gameObject4, items[k], true, displayAll, false, false);
					if (items[k].m_itemSortNum > 0uL)
					{
						GameObject obj4 = Utility.FindChild(gameObject4, "Effect_glow");
						obj4.CustomSetActive(true);
					}
					gameObject4.CustomSetActive(false);
				}
				cUITimerScript.SetTotalTime(100f);
				cUITimerScript.StartTimer();
			}
		}

		public void ShowHeroSkin(ListView<CUseable> items)
		{
			int count = items.Count;
			if (count == 0)
			{
				return;
			}
			uint heroId = 0u;
			uint skinId = 0u;
			int i = 0;
			while (i < count)
			{
				switch (items[i].m_type)
				{
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
					if (items[i].ExtraFromType == 1)
					{
						CUICommonSystem.ShowNewHeroOrSkin((uint)items[i].ExtraFromData, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority4, (uint)items[i].m_stackCount, 0);
					}
					else if (items[i].ExtraFromType == 2)
					{
						CSkinInfo.ResolveHeroSkin((uint)items[i].ExtraFromData, out heroId, out skinId);
						CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority4, (uint)items[i].m_stackCount, 0);
					}
					break;
				case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
					CUICommonSystem.ShowNewHeroOrSkin(items[i].m_baseID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority4, 0u, 0);
					break;
				case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
					CSkinInfo.ResolveHeroSkin(items[i].m_baseID, out heroId, out skinId);
					CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority4, 0u, 0);
					break;
				}
				IL_11E:
				i++;
				continue;
				goto IL_11E;
			}
		}

		public void ToggleBtnGroup(bool active)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/btnGroup");
			GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRoulette/Luck/Effect");
			if (active)
			{
				obj.CustomSetActive(true);
				obj2.CustomSetActive(false);
			}
			else
			{
				obj.CustomSetActive(false);
				if (!this.m_GotAllUnusualItems)
				{
					obj2.CustomSetActive(true);
				}
			}
		}

		public void RequestTimeOut(int seq)
		{
			this.ToggleBtnGroup(true);
		}

		private void OnNtyAddHero(uint id)
		{
			this.RefreshRewards();
		}

		private void OnNtyAddSkin(uint heroId, uint skinId, uint addReason)
		{
			this.RefreshRewards();
		}

		private void OnMallTabChange()
		{
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRouletteTabChange));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOne));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFive));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOneConfirm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFiveConfirm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_Change, new CUIEventManager.OnUIEventHandler(this.OnSpinInterval));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_End, new CUIEventManager.OnUIEventHandler(this.OnSpinEnd));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new CUIEventManager.OnUIEventHandler(this.OnOpenExternRewardTip));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Draw_Extern_Reward, new CUIEventManager.OnUIEventHandler(this.OnDrawExternReward));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Reset_Reward_Draw_Cnt, new CUIEventManager.OnUIEventHandler(this.OnDrawCntReset));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Reward_Item, new CUIEventManager.OnUIEventHandler(this.OnShowRewardItem));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tmp_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnTmpRewardEnable));
			instance.RemoveUIEventListener(enUIEventID.Mall_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
			instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseAwardForm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_More, new CUIEventManager.OnUIEventHandler(this.OnCrystalMore));
			instance.RemoveUIEventListener(enUIEventID.Mall_Cystal_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnCrystalListEnable));
			instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchange));
			instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchangeConfirm));
			instance.RemoveUIEventListener(enUIEventID.Mall_CryStal_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnCryStalOnTabChange));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Receive_Roulette_Data, new Action(this.RefreshExternRewards));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
		}

		private void OnRouletteTabChange(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseTips();
			CUICommonSystem.CloseCommonTips();
			CUICommonSystem.CloseUseableTips();
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			if (selectedIndex > this.m_UsedTabs.Count)
			{
				return;
			}
			int lastSelectedIndex = component.GetLastSelectedIndex();
			if (lastSelectedIndex >= 0 && lastSelectedIndex < this.m_UsedTabs.Count)
			{
				CMallRouletteController.Tab tab = this.m_UsedTabs[lastSelectedIndex];
				RES_SHOPBUY_COINTYPE coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL;
				bool flag = false;
				CMallRouletteController.Tab tab2 = tab;
				if (tab2 != CMallRouletteController.Tab.DianQuan)
				{
					if (tab2 == CMallRouletteController.Tab.Diamond)
					{
						coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND;
						stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.Diamond);
						stPayInfo payInfo2 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.Diamond);
						if (payInfo.m_payValue < payInfo.m_oriValue || payInfo2.m_payValue < payInfo2.m_oriValue)
						{
							flag = true;
						}
					}
				}
				else
				{
					coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
					stPayInfo payInfo3 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.DianQuan);
					stPayInfo payInfo4 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.DianQuan);
					if (payInfo3.m_payValue < payInfo3.m_oriValue || payInfo4.m_payValue < payInfo4.m_oriValue)
					{
						flag = true;
					}
				}
				if (this.IsProbabilityDoubled(coinType) || flag)
				{
					CUIRedDotSystem.SetRedDotViewByVersion(this.TabToRedID(tab));
					CUIListElementScript lastSelectedElement = component.GetLastSelectedElement();
					if (lastSelectedElement != null)
					{
						CUICommonSystem.DelRedDot(lastSelectedElement.gameObject);
					}
				}
			}
			this.CurTab = this.m_UsedTabs[selectedIndex];
			this.RefreshRewards();
			this.m_IsLuckyBarInited = false;
			this.RefreshExternRewards();
			this.DisplayTmpRewardList(false, 0);
			this.RefreshButtonView();
			this.RefreshTimer();
		}

		private void OnRouletteBuyOne(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				GameObject widget = Singleton<CMallSystem>.GetInstance().m_MallForm.GetWidget(3);
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1f, widget, new object[0]);
				return;
			}
			stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.None);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.Mall_Roulette_Buy_One_Confirm;
			if (payInfo.m_payType == enPayType.Diamond)
			{
				CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, enPayType.DiamondAndDianQuan, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
			}
			else
			{
				CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, payInfo.m_payType, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
			}
		}

		private void OnRouletteBuyFive(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				GameObject widget = Singleton<CMallSystem>.GetInstance().m_MallForm.GetWidget(3);
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1f, widget, new object[0]);
				return;
			}
			stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.None);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.Mall_Roulette_Buy_Five_Confirm;
			if (payInfo.m_payType == enPayType.Diamond)
			{
				CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, enPayType.DiamondAndDianQuan, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
			}
			else
			{
				CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, payInfo.m_payType, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
			}
		}

		private void OnRouletteBuyOneConfirm(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
			int num = this.m_CurSpinIdx % 14;
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", num));
			if (gameObject != null)
			{
				CUICommonSystem.PlayAnimator(gameObject, "Roulette_Halo_Disappear");
			}
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_duobao_btu", null);
			this.m_IsContinousDraw = false;
			this.Spin(-1);
			this.ToggleBtnGroup(false);
			this.SendLotteryMsg(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
		}

		private void OnRouletteBuyFiveConfirm(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
			int num = this.m_CurSpinIdx % 14;
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", num));
			if (gameObject != null)
			{
				CUICommonSystem.PlayAnimator(gameObject, "Roulette_Halo_Disappear");
			}
			this.m_IsContinousDraw = true;
			this.Spin(-1);
			this.ToggleBtnGroup(false);
			this.SendLotteryMsg(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
		}

		private void OnSpinInterval(CUIEvent uiEvent)
		{
			this.m_CurSpinCnt++;
			int num = Math.Abs(this.m_CurSpinIdx % 14);
			if (this.m_CurSpinCnt % 14 == 0)
			{
				this.m_CurLoops++;
			}
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", num));
			if (gameObject != null)
			{
				CUICommonSystem.PlayAnimator(gameObject, "Roulette_Halo_Appear");
			}
			CUITimerScript cUITimerScript = uiEvent.m_srcWidgetScript as CUITimerScript;
			if (this.m_CurState == CMallRouletteController.Roulette_State.ACCELERATE)
			{
				float num2 = 0.1f - (float)this.m_CurSpinCnt * 0.07f / 4f;
				if (num2 > 0.03f)
				{
					if (cUITimerScript != null)
					{
						cUITimerScript.SetOnChangedIntervalTime((num2 > 0.03f) ? num2 : 0.03f);
					}
				}
				else
				{
					this.m_CurState = CMallRouletteController.Roulette_State.UNIFORM;
				}
			}
			if (this.m_CurState == CMallRouletteController.Roulette_State.UNIFORM && uiEvent.m_eventParams.tag != -1 && this.m_CurLoops > 2 && ((uiEvent.m_eventParams.tag - 4 < 0 && num == 14 + uiEvent.m_eventParams.tag - 4) || (uiEvent.m_eventParams.tag - 4 >= 0 && num == uiEvent.m_eventParams.tag - 4)))
			{
				this.m_CurState = CMallRouletteController.Roulette_State.DECELERATE;
				this.m_CurSpinCnt = 0;
			}
			if (this.m_CurState == CMallRouletteController.Roulette_State.DECELERATE)
			{
				float num3 = 0.03f + (float)this.m_CurSpinCnt * 0.07f / 4f;
				if (uiEvent.m_eventParams.tag == num && num3 >= 0.1f && cUITimerScript != null)
				{
					if (!this.m_IsContinousDraw)
					{
						this.m_CurState = CMallRouletteController.Roulette_State.NONE;
					}
					else
					{
						this.m_CurState = CMallRouletteController.Roulette_State.CONTINUOUS_DRAW;
						this.DisplayTmpRewardList(true, 0);
					}
					cUITimerScript.m_eventIDs[1] = enUIEventID.Mall_Roulette_Spin_End;
					cUITimerScript.m_eventParams[1].tag = uiEvent.m_eventParams.tag;
					cUITimerScript.SetOnChangedIntervalTime(0.03f);
					cUITimerScript.SetTotalTime(0f);
					return;
				}
				if (cUITimerScript != null)
				{
					cUITimerScript.SetOnChangedIntervalTime((num3 < 0.1f) ? num3 : 0.1f);
				}
			}
			if ((this.m_CurState == CMallRouletteController.Roulette_State.CONTINUOUS_DRAW || this.m_CurState == CMallRouletteController.Roulette_State.SKIP) && uiEvent.m_eventParams.tag == num && this.m_CurContinousDrawSteps >= 1)
			{
				if (cUITimerScript != null)
				{
					cUITimerScript.m_eventIDs[1] = enUIEventID.Mall_Roulette_Spin_End;
					cUITimerScript.m_eventParams[1].tag = uiEvent.m_eventParams.tag;
					cUITimerScript.SetTotalTime(0f);
				}
				return;
			}
			if (this.m_IsClockwise)
			{
				this.m_CurSpinIdx++;
			}
			else
			{
				this.m_CurSpinIdx--;
			}
			this.m_CurContinousDrawSteps += 1;
		}

		private void OnSpinEnd(CUIEvent uiEvent)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag;
			if (tag != -1)
			{
				GameObject gameObject = Utility.FindChild(mallForm.gameObject, string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", tag));
				if (gameObject != null)
				{
					CUICommonSystem.PlayAnimator(gameObject, "Roulette_Halo_Focus");
				}
			}
			if (this.m_CurState == CMallRouletteController.Roulette_State.CONTINUOUS_DRAW && this.m_LuckyDrawRsp != null && this.m_CurRewardIdx < (int)this.m_LuckyDrawRsp.bRewardCnt)
			{
				Singleton<CMallSystem>.GetInstance().ToggleSkipAnimationMask(true, 30f);
				Singleton<CMallSystem>.GetInstance().ToggleActionMask(false, 30f);
				this.m_CurRewardIdx++;
				this.DisplayTmpRewardList(true, this.m_CurRewardIdx);
				Debug.Log(string.Format("五连抽第{0}次", this.m_CurRewardIdx));
				if (this.m_CurRewardIdx < (int)this.m_LuckyDrawRsp.bRewardCnt)
				{
					if (Math.Abs((int)this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx] - tag) > 7)
					{
						if ((int)this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx] - tag > 0)
						{
							this.m_IsClockwise = false;
						}
						else
						{
							this.m_IsClockwise = true;
						}
					}
					else if ((int)this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx] - tag < 0)
					{
						this.m_IsClockwise = false;
					}
					else
					{
						this.m_IsClockwise = true;
					}
					this.m_IsClockwise = true;
					Singleton<CTimerManager>.GetInstance().AddTimer(500, 1, delegate(int sequence)
					{
						this.m_CurContinousDrawSteps = 0;
						this.Spin((int)this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx]);
					});
					return;
				}
			}
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/timerSpin");
			if (componetInChild != null)
			{
				componetInChild.m_eventIDs[1] = enUIEventID.None;
				componetInChild.EndTimer();
			}
			Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
			Singleton<CMallSystem>.GetInstance().ToggleSkipAnimationMask(false, 30f);
			if (this.m_LuckyDrawRsp != null && this.m_LuckyDrawRsp.bRewardCnt != 0)
			{
				Singleton<CTimerManager>.GetInstance().AddTimer(600, 1, delegate(int sequence)
				{
					string title = null;
					CMallRouletteController.Tab curTab = this.CurTab;
					if (curTab != CMallRouletteController.Tab.DianQuan)
					{
						if (curTab == CMallRouletteController.Tab.Diamond)
						{
							title = "钻石夺宝奖励";
						}
					}
					else
					{
						title = "点券夺宝奖励";
					}
					Singleton<CSoundManager>.GetInstance().PostEvent("Stop_UI_buy_chou_duobao_zhuan", null);
					if (this.m_LuckyDrawRsp.bRewardCnt == 1)
					{
						this.OpenAwardTip(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, this.m_RewardList, title, true, enUIEventID.None, false);
					}
					else
					{
						this.OpenAwardTip(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, this.m_RewardList, title, true, enUIEventID.None, false);
					}
					this.ShowHeroSkin(this.m_RewardList);
				});
			}
		}

		private void OnOpenExternRewardTip(CUIEvent uiEvent)
		{
			if (Singleton<CMallSystem>.GetInstance().m_MallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag;
			ListView<ResDT_LuckyDrawExternReward> listView = new ListView<ResDT_LuckyDrawExternReward>();
			CSDT_LUCKYDRAW_INFO cSDT_LUCKYDRAW_INFO = new CSDT_LUCKYDRAW_INFO();
			CMallRouletteController.Tab curTab = this.CurTab;
			if (curTab != CMallRouletteController.Tab.DianQuan)
			{
				if (curTab == CMallRouletteController.Tab.Diamond)
				{
					CMallSystem.luckyDrawDic.TryGetValue(enPayType.Diamond, out cSDT_LUCKYDRAW_INFO);
					this.m_ExternRewardDic.TryGetValue(10u, out listView);
				}
			}
			else
			{
				CMallSystem.luckyDrawDic.TryGetValue(enPayType.DianQuan, out cSDT_LUCKYDRAW_INFO);
				this.m_ExternRewardDic.TryGetValue(2u, out listView);
			}
			if (listView == null || listView.Count == 0 || tag > listView.Count)
			{
				return;
			}
			ResDT_LuckyDrawExternReward resDT_LuckyDrawExternReward = listView[tag];
			ListView<CUseable> listView2 = new ListView<CUseable>();
			ResRandomRewardStore dataByKey;
			if ((dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(resDT_LuckyDrawExternReward.dwRewardID)) == null)
			{
				return;
			}
			for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
			{
				if (dataByKey.astRewardDetail[i].bItemType == 0 || dataByKey.astRewardDetail[i].bItemType >= 18)
				{
					break;
				}
				ListView<CUseable> listView3 = CUseableManager.CreateUsableListByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey.astRewardDetail[i].bItemType, (int)dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID);
				if (listView3 != null && listView3.Count > 0)
				{
					listView2.AddRange(listView3);
				}
			}
			byte b = 0;
			byte b2 = 0;
			if (cSDT_LUCKYDRAW_INFO != null)
			{
				string text = Convert.ToString((long)((ulong)cSDT_LUCKYDRAW_INFO.dwReachMask), 2).PadLeft(32, '0');
				string text2 = Convert.ToString((long)((ulong)cSDT_LUCKYDRAW_INFO.dwDrawMask), 2).PadLeft(32, '0');
				byte.TryParse(text.Substring(32 - (tag + 1), 1), out b);
				byte.TryParse(text2.Substring(32 - (tag + 1), 1), out b2);
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Roullete_ExternReward.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.None);
			GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnGroup/btnBuyOne");
			if (gameObject != null)
			{
				stUIEventParams stUIEventParams = default(stUIEventParams);
				CMallSystem.SetPayButton(cUIFormScript, gameObject.transform as RectTransform, payInfo.m_payType, payInfo.m_payValue, payInfo.m_oriValue, enUIEventID.Mall_Roulette_Buy_One, ref stUIEventParams);
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "Panel/Centent/Desc/Count");
			if (componetInChild != null && cSDT_LUCKYDRAW_INFO != null)
			{
				componetInChild.text = cSDT_LUCKYDRAW_INFO.dwCnt.ToString();
			}
			GameObject obj = Utility.FindChild(cUIFormScript.gameObject, "Panel/Centent/BuyDesc");
			GameObject obj2 = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnGroup/btnConfirm");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "Panel/btnGroup/btnConfirm/Text");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "Panel/Centent/BuyDesc/Count");
			GameObject gameObject2 = Utility.FindChild(cUIFormScript.gameObject, "Panel/btnGroup/btnGet");
			if (b == 0)
			{
				obj.CustomSetActive(true);
				obj2.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
				if (componetInChild2 != null)
				{
					componetInChild2.text = "去完成";
				}
				if (componetInChild3 != null && cSDT_LUCKYDRAW_INFO != null)
				{
					componetInChild3.text = string.Format("{0}", resDT_LuckyDrawExternReward.dwDrawCnt - cSDT_LUCKYDRAW_INFO.dwCnt);
				}
			}
			else
			{
				obj.CustomSetActive(false);
				if (b2 == 0)
				{
					obj2.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
					CUIEventScript cUIEventScript = gameObject2.GetComponent<CUIEventScript>();
					if (cUIEventScript == null)
					{
						cUIEventScript = gameObject2.AddComponent<CUIEventScript>();
						cUIEventScript.Initialize(cUIFormScript);
					}
					cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Roulette_Draw_Extern_Reward, new stUIEventParams
					{
						tag = tag,
						tag2 = (int)CMallSystem.PayTypeToResBuyCoinType(payInfo.m_payType)
					});
				}
				else
				{
					obj2.CustomSetActive(true);
					if (componetInChild2 != null)
					{
						componetInChild2.text = "已领取";
					}
					gameObject2.CustomSetActive(false);
				}
			}
			int num = 5;
			int num2 = Mathf.Min(listView2.Count, num);
			for (int j = 0; j < num2; j++)
			{
				GameObject gameObject3 = Utility.FindChild(cUIFormScript.gameObject, string.Format("Panel/Centent/itemCells/itemCell{0}", j + 1));
				CUICommonSystem.SetItemCell(cUIFormScript, gameObject3, listView2[j], true, false, false, false);
				gameObject3.CustomSetActive(true);
				gameObject3.transform.FindChild("ItemName").GetComponent<Text>().text = listView2[j].m_name;
			}
			for (int k = num2; k < num; k++)
			{
				GameObject obj3 = Utility.FindChild(cUIFormScript.gameObject, string.Format("Panel/Centent/itemCells/itemCell{0}", k + 1));
				obj3.CustomSetActive(false);
			}
		}

		private void OnDrawExternReward(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4803u);
			cSPkg.stPkgData.stLuckyDrawExternReq.bMoneyType = (byte)uiEvent.m_eventParams.tag2;
			cSPkg.stPkgData.stLuckyDrawExternReq.bExternIndex = (byte)uiEvent.m_eventParams.tag;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void OnShowRewardItem(CUIEvent uiEvent)
		{
			if (this.m_CurRewardIdx >= 0 && this.m_CurRewardIdx < (int)this.m_LuckyDrawRsp.bRewardCnt)
			{
				GameObject obj = Utility.FindChild(uiEvent.m_srcFormScript.gameObject, string.Format("Panel/Centent/itemCell{0}", this.m_CurRewardIdx + 1));
				obj.CustomSetActive(true);
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_get_N", null);
				this.m_CurRewardIdx++;
			}
			else
			{
				CUITimerScript cUITimerScript = uiEvent.m_srcWidgetScript as CUITimerScript;
				cUITimerScript.EndTimer();
			}
		}

		private void OnShowRule(CUIEvent uiEvent)
		{
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey(2u);
			if (dataByKey != null)
			{
				string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
				string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
				Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
			}
		}

		private void OnTmpRewardEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (this.m_RewardList == null || srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList > this.m_RewardList.Count)
			{
				return;
			}
			GameObject srcWidget = uiEvent.m_srcWidget;
			GameObject gameObject = srcWidget.transform.Find("itemCell").gameObject;
			CUseable itemUseable = this.m_RewardList[srcWidgetIndexInBelongedList];
			CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, gameObject, itemUseable, false, false, false, false);
		}

		private void OnSkipAnimation(CUIEvent uiEvent)
		{
			this.m_CurState = CMallRouletteController.Roulette_State.SKIP;
		}

		private void OnCloseAwardForm(CUIEvent uiEvent)
		{
			this.RefreshRewards();
			this.RefreshExternRewards();
			this.DisplayTmpRewardList(false, 0);
			if (CMallRouletteController.reqSentTimerSeq <= 0)
			{
				Singleton<CMallSystem>.GetInstance().ToggleActionMask(false, 30f);
				this.ToggleBtnGroup(true);
			}
		}

		private void OnDrawCntReset(CUIEvent uiEvent)
		{
			CSDT_LUCKYDRAW_INFO cSDT_LUCKYDRAW_INFO = new CSDT_LUCKYDRAW_INFO();
			enPayType key = enPayType.Diamond;
			CMallRouletteController.Tab curTab = this.CurTab;
			if (curTab != CMallRouletteController.Tab.DianQuan)
			{
				if (curTab == CMallRouletteController.Tab.Diamond)
				{
					key = enPayType.Diamond;
				}
			}
			else
			{
				key = enPayType.Diamond;
			}
			CMallSystem.luckyDrawDic.TryGetValue(key, out cSDT_LUCKYDRAW_INFO);
			if (cSDT_LUCKYDRAW_INFO != null)
			{
				cSDT_LUCKYDRAW_INFO.dwDrawMask = 0u;
				cSDT_LUCKYDRAW_INFO.dwReachMask = 0u;
				cSDT_LUCKYDRAW_INFO.dwCnt = 0u;
			}
			this.RefreshExternRewards();
			this.RefreshTimer();
		}

		private void OnCrystalMore(CUIEvent uiEvent)
		{
			this.m_nCurCryStalTabID = 0u;
			this.m_nTotalCrySalTab = 0;
			Singleton<CUIManager>.GetInstance().OpenForm(CMallRouletteController.sMallFormCrystal, false, true);
			this.RefreshCrystalUI(0u);
		}

		private void RefreshCrystalUI(uint tabID)
		{
			this.m_nCurCryStalTabID = tabID;
			this.InitCryStalData();
			this.InitCryStalTabMenu(tabID);
			this.UpdateCryStalTab(tabID);
		}

		private int GetCryStalItemCount(uint tabID)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return 0;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if ((ulong)tabID < (ulong)((long)this.m_CrySatlDic.Count))
			{
				ListView<ResRareExchange> listView = this.m_CrySatlDic[tabID];
				if (listView.Count > 0)
				{
					uint dwRareItemID = listView[0].dwRareItemID;
					return useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwRareItemID);
				}
			}
			return 0;
		}

		private void RefreshCrystalPnl()
		{
			this.RefreshCrystalUI(this.m_nCurCryStalTabID);
		}

		private void InitCryStalData()
		{
			this.m_CrySatlDic.Clear();
			DictionaryView<uint, ResRareExchange>.Enumerator enumerator = GameDataMgr.rareExchangeDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResRareExchange> current = enumerator.Current;
				ResRareExchange value = current.Value;
				uint dwTabID = value.dwTabID;
				if (!this.m_CrySatlDic.ContainsKey(dwTabID))
				{
					ListView<ResRareExchange> listView = new ListView<ResRareExchange>();
					listView.Add(value);
					this.m_CrySatlDic.Add(dwTabID, listView);
				}
				else
				{
					ListView<ResRareExchange> listView2 = this.m_CrySatlDic[dwTabID];
					listView2.Add(value);
				}
			}
			this.m_nTotalCrySalTab = this.m_CrySatlDic.Count;
			uint num = 0u;
			while ((ulong)num < (ulong)((long)this.m_nTotalCrySalTab))
			{
				ListView<ResRareExchange> listView3 = this.m_CrySatlDic[num];
				this.ProcessCrystalDataTabID(ref listView3);
				num += 1u;
			}
		}

		private void InitCryStalItemID()
		{
			DictionaryView<uint, ListView<ResRareExchange>> dictionaryView = new DictionaryView<uint, ListView<ResRareExchange>>();
			DictionaryView<uint, ResRareExchange>.Enumerator enumerator = GameDataMgr.rareExchangeDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResRareExchange> current = enumerator.Current;
				ResRareExchange value = current.Value;
				uint dwTabID = value.dwTabID;
				if (!dictionaryView.ContainsKey(dwTabID))
				{
					dictionaryView.Add(dwTabID, new ListView<ResRareExchange>
					{
						value
					});
				}
				else
				{
					ListView<ResRareExchange> listView = dictionaryView[dwTabID];
					listView.Add(value);
				}
			}
			if (this.m_CrystalItemID.Length < dictionaryView.Count)
			{
				this.m_CrystalItemID = new int[dictionaryView.Count];
			}
			uint num = 0u;
			while ((ulong)num < (ulong)((long)dictionaryView.Count))
			{
				ListView<ResRareExchange> listView2 = dictionaryView[num];
				if (listView2.Count > 0)
				{
					this.m_CrystalItemID[(int)((UIntPtr)num)] = (int)listView2[0].dwRareItemID;
				}
				else
				{
					this.m_CrystalItemID[(int)((UIntPtr)num)] = 0;
				}
				num += 1u;
			}
		}

		public bool IsCryStalItem(uint itemID)
		{
			for (int i = 0; i < this.m_CrystalItemID.Length; i++)
			{
				if ((long)this.m_CrystalItemID[i] == (long)((ulong)itemID))
				{
					return true;
				}
			}
			return false;
		}

		private void ProcessCrystalDataTabID(ref ListView<ResRareExchange> resultList)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			ListView<ResRareExchange> listView = new ListView<ResRareExchange>();
			ListView<ResRareExchange> listView2 = new ListView<ResRareExchange>();
			ListView<ResRareExchange> listView3 = new ListView<ResRareExchange>();
			DictionaryView<uint, ResRareExchange>.Enumerator enumerator = GameDataMgr.rareExchangeDict.GetEnumerator();
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			for (int i = 0; i < resultList.Count; i++)
			{
				ResRareExchange resRareExchange = resultList[i];
				if (resRareExchange.wExchangeType == 4)
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resRareExchange.dwExchangeID);
					if (dataByKey != null)
					{
						if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
						{
							listView3.Add(resRareExchange);
						}
						else
						{
							listView.Add(resRareExchange);
						}
					}
				}
				else if (resRareExchange.wExchangeType == 7)
				{
					uint heroId = 0u;
					uint skinId = 0u;
					CSkinInfo.ResolveHeroSkin(resRareExchange.dwExchangeID, out heroId, out skinId);
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
					if (heroSkin != null)
					{
						if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
						{
							listView3.Add(resRareExchange);
						}
						else
						{
							listView2.Add(resRareExchange);
						}
					}
				}
			}
			listView.Sort(delegate(ResRareExchange a, ResRareExchange b)
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
				if (a.iSortID < b.iSortID)
				{
					return 1;
				}
				if (a.iSortID > b.iSortID)
				{
					return -1;
				}
				return 0;
			});
			listView2.Sort(delegate(ResRareExchange a, ResRareExchange b)
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
				if (a.iSortID < b.iSortID)
				{
					return 1;
				}
				if (a.iSortID > b.iSortID)
				{
					return -1;
				}
				return 0;
			});
			listView3.Sort(delegate(ResRareExchange a, ResRareExchange b)
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
				if (a.wExchangeType > b.wExchangeType)
				{
					return 1;
				}
				if (a.wExchangeType < b.wExchangeType)
				{
					return -1;
				}
				return 0;
			});
			resultList.Clear();
			for (int j = 0; j < listView.Count; j++)
			{
				resultList.Add(listView[j]);
			}
			for (int k = 0; k < listView2.Count; k++)
			{
				resultList.Add(listView2[k]);
			}
			for (int l = 0; l < listView3.Count; l++)
			{
				resultList.Add(listView3[l]);
			}
		}

		private void InitCryStalTabMenu(uint tabID)
		{
			string[] array = new string[]
			{
				"荣耀商店",
				"王者商店"
			};
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRouletteController.sMallFormCrystal);
			if (form == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "Panel/Tab");
			componetInChild.SetElementAmount(this.m_nTotalCrySalTab);
			for (int i = 0; i < this.m_nTotalCrySalTab; i++)
			{
				CUIListElementScript elemenet = componetInChild.GetElemenet(i);
				Text componetInChild2 = Utility.GetComponetInChild<Text>(elemenet.gameObject, "Text");
				if (componetInChild2 && i < array.Length)
				{
					componetInChild2.text = array[i];
				}
			}
			componetInChild.SelectElement((int)tabID, true);
		}

		private void OnCryStalOnTabChange(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRouletteController.sMallFormCrystal);
			if (form == null)
			{
				return;
			}
			if (uiEvent.m_srcWidgetIndexInBelongedList < this.m_nTotalCrySalTab)
			{
				this.m_nCurCryStalTabID = (uint)uiEvent.m_srcWidgetIndexInBelongedList;
				this.UpdateCryStalTab(this.m_nCurCryStalTabID);
			}
		}

		private void UpdateCryStalTab(uint tabID)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMallRouletteController.sMallFormCrystal);
			if (form == null)
			{
				return;
			}
			int cryStalItemCount = this.GetCryStalItemCount(tabID);
			Transform transform = form.transform.Find("Panel/pnlTotalMoney/Cnt");
			if (transform)
			{
				transform.GetComponent<Text>().text = cryStalItemCount.ToString();
			}
			Transform transform2 = form.transform.Find("Panel/pnlTotalMoney/Image");
			if (transform2)
			{
				string prefabPath = this.m_tab0ImagePath;
				if (tabID == 1u)
				{
					prefabPath = this.m_tab1ImagePath;
				}
				Image component = transform2.GetComponent<Image>();
				if (component)
				{
					component.SetSprite(prefabPath, form, true, false, false, false);
				}
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "Panel/HeroList");
			gameObject.CustomSetActive(true);
			CUIListScript component2 = gameObject.GetComponent<CUIListScript>();
			if (component2 == null)
			{
				return;
			}
			if ((ulong)tabID < (ulong)((long)this.m_CrySatlDic.Count))
			{
				component2.SetElementAmount(this.m_CrySatlDic[tabID].Count + 1);
			}
		}

		private void OnDefault(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcFormScript == null || srcWidget == null)
			{
				return;
			}
			Transform transform = srcWidget.transform;
			Transform transform2 = transform.Find("heroItem");
			transform2.gameObject.CustomSetActive(true);
			Transform transform3 = transform.Find("heroSkinItem");
			transform3.gameObject.CustomSetActive(false);
			Transform transform4 = transform2.Find("Mask");
			if (transform4 != null)
			{
				transform4.gameObject.CustomSetActive(false);
			}
			Transform transform5 = transform2.Find("imagedefault");
			if (transform5)
			{
				transform5.gameObject.CustomSetActive(true);
			}
			Transform transform6 = transform2.Find("imageIcon");
			if (transform6)
			{
				transform6.gameObject.CustomSetActive(false);
			}
			Text component = transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
			component.text = string.Empty;
			GameObject gameObject = transform.Find("imgExperienceMark").gameObject;
			GameObject gameObject2 = Utility.FindChild(transform2.gameObject, "heroDataPanel/heroPricePanel");
			if (gameObject2 == null)
			{
				DebugHelper.Assert(gameObject2 != null, "price panel is null");
				return;
			}
			gameObject2.CustomSetActive(false);
			Transform transform7 = transform.Find("ButtonGroup/BuyBtn");
			Button component2 = transform7.GetComponent<Button>();
			Text component3 = transform7.Find("Text").GetComponent<Text>();
			CUIEventScript component4 = transform7.GetComponent<CUIEventScript>();
			transform7.gameObject.CustomSetActive(false);
			component4.enabled = false;
			component2.enabled = false;
			transform7.gameObject.CustomSetActive(true);
			component3.text = "敬请期待";
			gameObject.CustomSetActive(false);
		}

		private void OnCrystalHeroItem(CUIEvent uiEvent, ushort resItemIdx, ResRareExchange resRareExchange)
		{
			if (resRareExchange == null)
			{
				return;
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcFormScript == null || srcWidget == null)
			{
				return;
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resRareExchange.dwExchangeID);
			if (dataByKey == null)
			{
				return;
			}
			Transform transform = srcWidget.transform;
			Transform transform2 = transform.Find("heroItem");
			transform2.gameObject.CustomSetActive(true);
			Transform transform3 = transform.Find("heroSkinItem");
			transform3.gameObject.CustomSetActive(false);
			Transform transform4 = transform2.Find("imagedefault");
			if (transform4)
			{
				transform4.gameObject.CustomSetActive(false);
			}
			Transform transform5 = transform2.Find("imageIcon");
			if (transform5)
			{
				transform5.gameObject.CustomSetActive(true);
			}
			GameObject gameObject = transform2.Find("profession").gameObject;
			CUICommonSystem.SetHeroItemImage(srcFormScript, transform2.gameObject, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false, true);
			CUICommonSystem.SetHeroJob(srcFormScript, gameObject, (enHeroJobType)dataByKey.bMainJob);
			Text component = transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
			string text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
			component.text = text;
			CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = dataByKey.dwCfgID;
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
			Transform transform6 = transform.Find("ButtonGroup/BuyBtn");
			Button component3 = transform6.GetComponent<Button>();
			Text component4 = transform6.Find("Text").GetComponent<Text>();
			CUIEventScript component5 = transform6.GetComponent<CUIEventScript>();
			transform6.gameObject.CustomSetActive(false);
			component5.enabled = false;
			component3.enabled = false;
			if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
			{
				transform6.gameObject.CustomSetActive(true);
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own");
				gameObject2.CustomSetActive(false);
				CUICommonSystem.PlayAnimator(srcWidget, "OnlyName");
			}
			else
			{
				gameObject2.CustomSetActive(masterRoleInfo.IsValidExperienceHero(dataByKey.dwCfgID));
				gameObject3.CustomSetActive(true);
				Image componetInChild = Utility.GetComponetInChild<Image>(gameObject3, "pnlExchange/costImage");
				Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject3, "pnlExchange/costText");
				if (componetInChild == null || componetInChild2 == null)
				{
					return;
				}
				componetInChild2.text = resRareExchange.dwRareItemCnt.ToString();
				string prefabPath = this.m_tab0ImagePath;
				if (this.m_nCurCryStalTabID == 1u)
				{
					prefabPath = this.m_tab1ImagePath;
				}
				if (componetInChild)
				{
					componetInChild.SetSprite(prefabPath, srcFormScript, true, false, false, false);
				}
				transform6.gameObject.CustomSetActive(true);
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Crystal_Exchange_Btn");
				component5.enabled = true;
				component3.enabled = true;
				component5.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Crystal_On_Exchange, new stUIEventParams
				{
					tag = 7,
					commonUInt32Param1 = resRareExchange.dwID,
					commonUInt16Param1 = resItemIdx,
					tagStr = text
				});
			}
		}

		private void OnCrystalHeroSkinItem(CUIEvent uiEvent, ushort resItemIdx, ResRareExchange resRareExchange)
		{
			if (resRareExchange == null)
			{
				return;
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcFormScript == null || srcWidget == null)
			{
				return;
			}
			uint heroId = 0u;
			uint skinId = 0u;
			CSkinInfo.ResolveHeroSkin(resRareExchange.dwExchangeID, out heroId, out skinId);
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin == null)
			{
				return;
			}
			Transform transform = srcWidget.transform;
			Transform transform2 = transform.Find("heroItem");
			transform2.gameObject.CustomSetActive(false);
			Transform transform3 = transform.Find("heroSkinItem");
			transform3.gameObject.CustomSetActive(true);
			Text component = transform3.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
			Text component2 = transform3.Find("heroDataPanel/heroNamePanel/heroSkinText").GetComponent<Text>();
			CUICommonSystem.SetHeroItemImage(srcFormScript, transform3.gameObject, heroSkin.szSkinPicID, enHeroHeadType.enBust, false, true);
			Transform transform4 = transform3.Find("skinLabelImage");
			CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, transform4.gameObject, heroSkin.dwHeroID, heroSkin.dwSkinID);
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
			if (dataByKey != null)
			{
				component.text = dataByKey.szName;
			}
			component2.text = heroSkin.szSkinName;
			CUIEventScript component3 = transform3.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = heroSkin.dwHeroID;
			eventParams.openHeroFormPar.skinId = heroSkin.dwSkinID;
			eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
			component3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
			component3.m_closeFormWhenClicked = true;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
				return;
			}
			Transform transform5 = transform.Find("ButtonGroup/BuyBtn");
			Button component4 = transform5.GetComponent<Button>();
			Text component5 = transform5.Find("Text").GetComponent<Text>();
			CUIEventScript component6 = transform5.GetComponent<CUIEventScript>();
			transform5.gameObject.CustomSetActive(false);
			component6.enabled = false;
			component4.enabled = false;
			GameObject gameObject = Utility.FindChild(transform3.gameObject, "heroDataPanel/heroPricePanel");
			if (gameObject == null)
			{
				DebugHelper.Assert(gameObject != null, "price panel is null");
				return;
			}
			gameObject.CustomSetActive(false);
			GameObject gameObject2 = transform.Find("imgExperienceMark").gameObject;
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
			{
				transform5.gameObject.CustomSetActive(true);
				component5.text = instance.GetText("Mall_Skin_State_Own");
				gameObject2.CustomSetActive(false);
			}
			else
			{
				gameObject2.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(heroSkin.dwHeroID, heroSkin.dwSkinID));
				gameObject.CustomSetActive(true);
				if (!masterRoleInfo.IsHaveHero(heroSkin.dwHeroID, false))
				{
					gameObject.CustomSetActive(false);
					component6.enabled = true;
					transform5.gameObject.CustomSetActive(true);
					component5.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
					component4.enabled = true;
					component6.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
					component6.m_closeFormWhenClicked = true;
					return;
				}
				Image componetInChild = Utility.GetComponetInChild<Image>(gameObject, "pnlExchange/costImage");
				Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "pnlExchange/costText");
				if (componetInChild == null || componetInChild2 == null)
				{
					return;
				}
				string prefabPath = this.m_tab0ImagePath;
				if (this.m_nCurCryStalTabID == 1u)
				{
					prefabPath = this.m_tab1ImagePath;
				}
				if (componetInChild)
				{
					componetInChild.SetSprite(prefabPath, srcFormScript, true, false, false, false);
				}
				componetInChild2.text = resRareExchange.dwRareItemCnt.ToString();
				component6.enabled = true;
				transform5.gameObject.CustomSetActive(true);
				component5.text = Singleton<CTextManager>.GetInstance().GetText("Crystal_Exchange_Btn");
				component4.enabled = true;
				component6.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Crystal_On_Exchange, new stUIEventParams
				{
					tag = 7,
					commonUInt32Param1 = resRareExchange.dwID,
					commonUInt16Param1 = resItemIdx,
					tagStr = dataByKey.szName + "-" + heroSkin.szSkinName
				});
			}
		}

		private void OnCrystalListEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if ((ulong)this.m_nCurCryStalTabID >= (ulong)((long)this.m_CrySatlDic.Count))
			{
				return;
			}
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_CrySatlDic[this.m_nCurCryStalTabID].Count)
			{
				this.OnDefault(uiEvent);
				return;
			}
			ResRareExchange resRareExchange = this.m_CrySatlDic[this.m_nCurCryStalTabID][srcWidgetIndexInBelongedList];
			if (resRareExchange.wExchangeType == 4)
			{
				this.OnCrystalHeroItem(uiEvent, (ushort)srcWidgetIndexInBelongedList, resRareExchange);
			}
			else if (resRareExchange.wExchangeType == 7)
			{
				this.OnCrystalHeroSkinItem(uiEvent, (ushort)srcWidgetIndexInBelongedList, resRareExchange);
			}
		}

		private void OnCrystalExchange(CUIEvent uiEvent)
		{
			string[] array = new string[]
			{
				"荣耀水晶",
				"王者水晶"
			};
			int cryStalItemCount = this.GetCryStalItemCount(this.m_nCurCryStalTabID);
			string text = string.Empty;
			if ((ulong)this.m_nCurCryStalTabID < (ulong)((long)array.Length))
			{
				text = array[(int)((UIntPtr)this.m_nCurCryStalTabID)];
			}
			if (cryStalItemCount <= 0)
			{
				string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Crystal_Item_NotEnough"), text);
				Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
				return;
			}
			if ((ulong)this.m_nCurCryStalTabID >= (ulong)((long)this.m_CrySatlDic.Count))
			{
				return;
			}
			RES_ITEM_TYPE tag = (RES_ITEM_TYPE)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint commonUInt16Param = (uint)uiEvent.m_eventParams.commonUInt16Param1;
			string tagStr = uiEvent.m_eventParams.tagStr;
			if (commonUInt16Param < 0u || (ulong)commonUInt16Param >= (ulong)((long)this.m_CrySatlDic[this.m_nCurCryStalTabID].Count))
			{
				return;
			}
			ResRareExchange resRareExchange = this.m_CrySatlDic[this.m_nCurCryStalTabID][(int)commonUInt16Param];
			if (resRareExchange.dwID == commonUInt32Param)
			{
				string strContent2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Confirm_CryStal_Buy_Text"), resRareExchange.dwRareItemCnt, text, tagStr);
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent2, enUIEventID.Mall_Crystal_On_Exchange_Confirm, enUIEventID.None, uiEvent.m_eventParams, false);
			}
		}

		private void OnCrystalExchangeConfirm(CUIEvent uiEvent)
		{
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			this.RequestBuyCrystal(commonUInt32Param);
		}

		private void RequestBuyCrystal(uint rareID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1184u);
			cSPkg.stPkgData.stRareExchangeReq.dwID = rareID;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1185)]
		public static void OnGetBuyCrystalMsg(CSPkg msg)
		{
			if (msg.stPkgData.stRareExchangeRsp.iResult == 0)
			{
				Singleton<CMallRouletteController>.GetInstance().RefreshCrystalPnl();
				uint num = 0u;
				while ((ulong)num < (ulong)((long)Singleton<CMallRouletteController>.GetInstance().m_CrySatlDic.Count))
				{
					for (int i = 0; i < Singleton<CMallRouletteController>.GetInstance().m_CrySatlDic[num].Count; i++)
					{
						ResRareExchange resRareExchange = Singleton<CMallRouletteController>.GetInstance().m_CrySatlDic[num][i];
						if (resRareExchange != null && resRareExchange.dwID == msg.stPkgData.stRareExchangeRsp.dwID)
						{
							if (resRareExchange.wExchangeType == 4)
							{
								uint dwExchangeID = resRareExchange.dwExchangeID;
								CUICommonSystem.ShowNewHeroOrSkin(dwExchangeID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, 0);
							}
							else if (resRareExchange.wExchangeType == 7)
							{
								uint dwExchangeID2 = resRareExchange.dwExchangeID;
								CUICommonSystem.ShowNewHeroOrSkin(0u, dwExchangeID2, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
							}
						}
					}
					num += 1u;
				}
			}
			else
			{
				string strContent = string.Empty;
				if (msg.stPkgData.stRareExchangeRsp.iResult == 1)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_SYS");
				}
				else if (msg.stPkgData.stRareExchangeRsp.iResult == 2)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_ID");
				}
				else if (msg.stPkgData.stRareExchangeRsp.iResult == 3)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_OUTDATE");
				}
				else if (msg.stPkgData.stRareExchangeRsp.iResult == 4)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_DUP");
				}
				else if (msg.stPkgData.stRareExchangeRsp.iResult == 5)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_LIMIT");
				}
				else if (msg.stPkgData.stRareExchangeRsp.iResult == 6)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_EXCHANGE");
				}
				else if (msg.stPkgData.stRareExchangeRsp.iResult == 7)
				{
					strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_STATE");
				}
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
			}
		}

		private void SendLotteryMsg(RES_SHOPDRAW_SUBTYPE drawType = RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE)
		{
			this.m_LuckyDrawRsp = null;
			if (this.m_RewardList != null)
			{
				this.m_RewardList.Clear();
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4801u);
			CSPKG_LUCKYDRAW_REQ cSPKG_LUCKYDRAW_REQ = new CSPKG_LUCKYDRAW_REQ();
			stPayInfo payInfo = CMallRouletteController.GetPayInfo(drawType, CMallRouletteController.Tab.None);
			if (payInfo.m_payType == enPayType.Diamond)
			{
				payInfo.m_payType = enPayType.DiamondAndDianQuan;
			}
			cSPKG_LUCKYDRAW_REQ.bMoneyType = (byte)CMallSystem.PayTypeToResBuyCoinType(payInfo.m_payType);
			cSPKG_LUCKYDRAW_REQ.bDrawType = (byte)drawType;
			cSPkg.stPkgData.stLuckyDrawReq = cSPKG_LUCKYDRAW_REQ;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			CMallRouletteController.reqSentTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(10000, 1, new CTimer.OnTimeUpHandler(this.RequestTimeOut));
		}

		[MessageHandler(4802)]
		public static void ReceiveLotteryRes(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			if (msg.stPkgData.stLuckyDrawRsp.iResult != 0)
			{
				CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.gameObject, "pnlBodyBg/pnlRoulette/timerSpin");
				if (componetInChild != null)
				{
					componetInChild.SetTotalTime(0f);
				}
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(4802, msg.stPkgData.stLuckyDrawRsp.iResult), false, 1.5f, null, new object[0]);
				Singleton<CMallRouletteController>.GetInstance().ToggleBtnGroup(true);
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref CMallRouletteController.reqSentTimerSeq);
			uint num = 0u;
			if (Singleton<CMallRouletteController>.GetInstance().m_RewardPoolDic.TryGetValue((uint)msg.stPkgData.stLuckyDrawRsp.bMoneyType, out num))
			{
				if (num != msg.stPkgData.stLuckyDrawRsp.dwRewardPoolID)
				{
					Singleton<CMallRouletteController>.GetInstance().RefreshData(msg.stPkgData.stLuckyDrawRsp.dwRewardPoolID);
				}
				else
				{
					Singleton<CMallRouletteController>.GetInstance().RefreshData(0u);
				}
			}
			Singleton<CMallRouletteController>.GetInstance().SetRewardData(msg.stPkgData.stLuckyDrawRsp);
			int idx = (int)msg.stPkgData.stLuckyDrawRsp.szRewardIndex[0];
			Singleton<CMallRouletteController>.GetInstance().Spin(idx);
		}

		[MessageHandler(4804)]
		public static void ReceiveDrawExternRewardRes(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stLuckyDrawExternRsp.iResult != 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(4804, msg.stPkgData.stLuckyDrawExternRsp.iResult), false, 1.5f, null, new object[0]);
				return;
			}
			ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(msg.stPkgData.stLuckyDrawExternRsp.stReward);
			if (useableListFromReward.Count == 0)
			{
				return;
			}
			CUseable[] array = new CUseable[useableListFromReward.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = useableListFromReward[i];
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Receive_Roulette_Data);
			Singleton<CUIManager>.GetInstance().OpenAwardTip(array, Singleton<CTextManager>.GetInstance().GetText("gotAward"), false, enUIEventID.None, false, false, "Form_Award");
			Singleton<CMallRouletteController>.GetInstance().ShowHeroSkin(useableListFromReward);
		}
	}
}
