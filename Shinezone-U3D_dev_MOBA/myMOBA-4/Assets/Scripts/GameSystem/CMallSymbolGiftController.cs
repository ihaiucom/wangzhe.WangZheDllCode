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
	public class CMallSymbolGiftController : Singleton<CMallSymbolGiftController>
	{
		
		private static Dictionary<string, int> __f__switch_map2;

		public enum enSymbolGiftFormWidget
		{
			Action_Mask,
			Top_Common,
			Body_Bg,
			Res_Mask,
			Action_Mask_Reset_Timer,
			Skip_Mask,
			Skip_Mask_Reset_Timer
		}

		public const string s_symbolGiftFormPath = "UGUI/Form/System/Mall/Form_Symbol_Gift.prefab";

		public const int MAX_SHOPDRAW_LIMIT = 1000;

		public bool[] m_ItemCellFlag;

		public bool[] m_AdvSymbolFlag;

		public bool[] m_AdvSymbolShowFlag;

		public uint[] m_AdvSymbolIds;

		public byte m_CurrentRewardIdx;

		public COMDT_REWARD_INFO[] astRewardList;

		public bool m_UIToggle;

		public bool reqSent;

		public static int reqSentTimerSeq;

		public byte m_CommonDrawSymbolCycle = 5;

		public byte m_SeniorDrawSymbolCycle = 5;

		public bool isSkipAni;

		public static int[] SymbolGiftCommonDrawedCntInfo;

		public static int[] SymbolGiftSeniorDrawedCntInfo;

		public static int[] SymbolGiftdrawCntLimit;

		static CMallSymbolGiftController()
		{
			CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo = new int[5];
			CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo = new int[5];
			CMallSymbolGiftController.SymbolGiftdrawCntLimit = new int[5];
			CMallSymbolGiftController.SymbolGiftdrawCntLimit[0] = 0;
			CMallSymbolGiftController.SymbolGiftdrawCntLimit[1] = 500;
			CMallSymbolGiftController.SymbolGiftdrawCntLimit[2] = 500;
			CMallSymbolGiftController.SymbolGiftdrawCntLimit[3] = 98;
			CMallSymbolGiftController.SymbolGiftdrawCntLimit[4] = 49;
		}

		public static int GetTotalDrawedCnt(RES_SHOPBUY_TYPE drawType)
		{
			int num = 5;
			int num2 = 0;
			if (drawType != RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON)
			{
				if (drawType == RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR)
				{
					for (int i = 1; i < num; i++)
					{
						switch (i)
						{
						case 1:
						case 2:
							num2 += CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[i];
							break;
						case 3:
							num2 += CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[i] * 5;
							break;
						case 4:
							num2 += CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[i] * 10;
							break;
						}
					}
				}
			}
			else
			{
				for (int j = 1; j < num; j++)
				{
					switch (j)
					{
					case 1:
					case 2:
						num2 += CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[j];
						break;
					case 3:
						num2 += CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[j] * 5;
						break;
					case 4:
						num2 += CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[j] * 10;
						break;
					}
				}
			}
			return num2;
		}

		public static void ResetDrawedCnt(RES_SHOPBUY_TYPE drawType)
		{
			if (drawType != RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON)
			{
				if (drawType == RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR)
				{
					int num = 5;
					for (int i = 1; i < num; i++)
					{
						if (CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[i] > CMallSymbolGiftController.SymbolGiftdrawCntLimit[i])
						{
							CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[i] = CMallSymbolGiftController.SymbolGiftdrawCntLimit[i];
						}
					}
				}
			}
			else
			{
				int num2 = 5;
				for (int j = 1; j < num2; j++)
				{
					if (CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[j] > CMallSymbolGiftController.SymbolGiftdrawCntLimit[j])
					{
						CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[j] = CMallSymbolGiftController.SymbolGiftdrawCntLimit[j];
					}
				}
			}
		}

		public override void Init()
		{
			base.Init();
			this.m_ItemCellFlag = new bool[6];
			this.m_AdvSymbolFlag = new bool[6];
			this.m_AdvSymbolShowFlag = new bool[6];
			this.m_AdvSymbolIds = new uint[6];
			this.m_CurrentRewardIdx = 0;
			this.astRewardList = new COMDT_REWARD_INFO[5];
			this.reqSent = false;
			CMallSymbolGiftController.reqSentTimerSeq = -1;
			this.isSkipAni = false;
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Lottery_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolGift));
			instance.AddUIEventListener(enUIEventID.Lottery_Close_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolGift));
			instance.AddUIEventListener(enUIEventID.Lottery_Common_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyOneSymbol));
			instance.AddUIEventListener(enUIEventID.Lottery_Common_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyFiveSymbol));
			instance.AddUIEventListener(enUIEventID.Lottery_Senior_BuyFreeSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFreeSymbol));
			instance.AddUIEventListener(enUIEventID.Lottery_Senior_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyOneSymbol));
			instance.AddUIEventListener(enUIEventID.Lottery_Senior_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFiveSymbol));
			instance.AddUIEventListener(enUIEventID.Lottery_BuySymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuySymbolConfirm));
			instance.AddUIEventListener(enUIEventID.Lottery_Show_Reward, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowReward));
			instance.AddUIEventListener(enUIEventID.Lottery_Show_Reward_End, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardEnd));
			instance.AddUIEventListener(enUIEventID.Lottery_Show_Reward_Start, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardStart));
			instance.AddUIEventListener(enUIEventID.Lottery_Close_FX, new CUIEventManager.OnUIEventHandler(this.OnLotteryCloseFx));
			instance.AddUIEventListener(enUIEventID.Lottery_Symbol_Boom, new CUIEventManager.OnUIEventHandler(this.OnLotterySymbolBoom));
			instance.AddUIEventListener(enUIEventID.Lottery_Gold_Free_Draw_CD_UP, new CUIEventManager.OnUIEventHandler(this.OnGoldFreeDrawCdUp));
			instance.AddUIEventListener(enUIEventID.Lottery_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
			instance.AddUIEventListener(enUIEventID.Lottery_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.onActionMaskReset));
			Singleton<EventRouter>.instance.RemoveEventHandler<CSPkg>("ShopBuyDraw", new Action<CSPkg>(CMallSymbolGiftController.ReceiveLotteryRes));
			Singleton<EventRouter>.instance.AddEventHandler<CSPkg>("ShopBuyDraw", new Action<CSPkg>(CMallSymbolGiftController.ReceiveLotteryRes));
		}

		public override void UnInit()
		{
			base.UnInit();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Lottery_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolGift));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Close_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolGift));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Common_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyOneSymbol));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Common_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyFiveSymbol));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Senior_BuyFreeSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFreeSymbol));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Senior_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyOneSymbol));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Senior_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFiveSymbol));
			instance.RemoveUIEventListener(enUIEventID.Lottery_BuySymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuySymbolConfirm));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Show_Reward, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowReward));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Show_Reward_End, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardEnd));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Show_Reward_Start, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardStart));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Close_FX, new CUIEventManager.OnUIEventHandler(this.OnLotteryCloseFx));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Symbol_Boom, new CUIEventManager.OnUIEventHandler(this.OnLotterySymbolBoom));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Gold_Free_Draw_CD_UP, new CUIEventManager.OnUIEventHandler(this.OnGoldFreeDrawCdUp));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
			instance.RemoveUIEventListener(enUIEventID.Lottery_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.onActionMaskReset));
			Singleton<EventRouter>.instance.RemoveEventHandler<CSPkg>("ShopBuyDraw", new Action<CSPkg>(CMallSymbolGiftController.ReceiveLotteryRes));
		}

		public void Draw(CUIFormScript form)
		{
			this.InitElements(form);
			this.RefreshDesc(form);
			this.RefreshButtonView(form);
			this.ToggleActionMask(form, false, 30f);
			this.ToggleActionPanel(form, true);
			this.ToggleResMask(form, false);
			this.PlayLotteryAnimation(form, "Begin", true, false, false);
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan", null);
			CMallSymbolGiftController.ShowLotteryResult(form, 0);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Sub_Module_Loaded);
		}

		public void ToggleSkipAnimationMask(CUIFormScript form, bool active, float totalTime = 30f)
		{
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(5);
			GameObject widget2 = form.GetWidget(6);
			CUITimerScript cUITimerScript = null;
			if (widget2 != null)
			{
				cUITimerScript = widget2.GetComponent<CUITimerScript>();
			}
			if (widget != null)
			{
				widget.CustomSetActive(active);
				if (cUITimerScript != null)
				{
					if (active)
					{
						cUITimerScript.SetTotalTime(totalTime);
						cUITimerScript.StartTimer();
					}
					else
					{
						cUITimerScript.EndTimer();
					}
				}
			}
		}

		public void ToggleActionMask(CUIFormScript form, bool active, float totalTime = 30f)
		{
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(4);
			CUITimerScript cUITimerScript = null;
			if (widget2 != null)
			{
				cUITimerScript = widget2.GetComponent<CUITimerScript>();
			}
			if (widget != null)
			{
				widget.CustomSetActive(active);
				if (cUITimerScript != null)
				{
					if (active)
					{
						cUITimerScript.SetTotalTime(totalTime);
						cUITimerScript.StartTimer();
					}
					else
					{
						cUITimerScript.EndTimer();
					}
				}
			}
		}

		private void PlayLotteryAnimation(CUIFormScript form, string state, bool PlayAni = true, bool PlayFx = false, bool enableEndEvent = false)
		{
			if (state != null)
			{
				if (CMallSymbolGiftController.__f__switch_map2 == null)
				{
					CMallSymbolGiftController.__f__switch_map2 = new Dictionary<string, int>(1)
					{
						{
							"Open",
							0
						}
					};
				}
				int num;
				if (CMallSymbolGiftController.__f__switch_map2.TryGetValue(state, out num))
				{
					if (num == 0)
					{
						Singleton<CSoundManager>.GetInstance().PostEvent("Stop_UI_buy_chou_jiguan", null);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_hero", null);
					}
				}
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "UIScene_Lottery/Home_TransferGate");
			if (gameObject != null)
			{
				CUIAnimatorScript component = gameObject.GetComponent<CUIAnimatorScript>();
				if (component != null)
				{
					component.Initialize(form);
				}
				if (enableEndEvent)
				{
					if (component != null)
					{
						component.m_eventIDs[1] = enUIEventID.Lottery_Show_Reward_Start;
					}
				}
				else if (component != null)
				{
					component.m_eventIDs[1] = enUIEventID.None;
				}
				if (component != null && PlayAni)
				{
					CUICommonSystem.PlayAnimator(gameObject, state);
				}
			}
			GameObject gameObject2 = Utility.FindChild(form.gameObject, "UIScene_Lottery/FX");
			if (gameObject2 != null)
			{
				CUIAnimatorScript component2 = gameObject2.GetComponent<CUIAnimatorScript>();
				if (component2 != null)
				{
					component2.Initialize(form);
					if (PlayFx)
					{
						CUICommonSystem.PlayAnimator(gameObject2, state);
					}
				}
			}
		}

		private void SetLotteryBool(CUIFormScript form, string name, bool value)
		{
			if (form == null || !form.gameObject.activeSelf)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "UIScene_Lottery/Home_TransferGate");
			if (gameObject != null)
			{
				CUIAnimatorScript component = gameObject.GetComponent<CUIAnimatorScript>();
				if (component != null)
				{
					component.Initialize(form);
					component.SetBool(name, value);
				}
			}
		}

		public void ToggleResMask(CUIFormScript form, bool active)
		{
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(3);
			if (widget != null)
			{
				widget.CustomSetActive(active);
			}
		}

		public void ToggleActionPanel(CUIFormScript form, bool active)
		{
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "pnlBodyBg/pnlSymbolGift/pnlAction");
			if (gameObject != null)
			{
				if (active)
				{
					CanvasGroup component = gameObject.GetComponent<CanvasGroup>();
					if (component != null && component.alpha < 1f)
					{
						CUICommonSystem.PlayAnimator(gameObject, "Button_Up");
					}
				}
				else
				{
					CUICommonSystem.PlayAnimator(gameObject, "Button_Down");
				}
			}
		}

		public void InitElements(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_lotterySceneBgPath, form);
			Utility.FindChild(form.gameObject, "UIScene_Lottery").CustomSetActive(true);
			this.SetLotteryBool(form, "Server_Turn", false);
			this.SetLotteryBool(form, "Icon_Turn", false);
		}

		private void RefreshDesc(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBodyBg/pnlSymbolGift/pnlAction/pnlCommonBuy/txtDes");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/txtDes");
			if (componetInChild != null)
			{
				componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Mall_Symbol_Gifts_Common_Buy_Dsc"), (int)this.m_CommonDrawSymbolCycle - CMallSymbolGiftController.GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON) % (int)this.m_CommonDrawSymbolCycle);
			}
			if (componetInChild2 != null)
			{
				componetInChild2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Mall_Symbol_Gifts_Senior_Buy_Dsc"), (int)this.m_SeniorDrawSymbolCycle - CMallSymbolGiftController.GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR) % (int)this.m_SeniorDrawSymbolCycle);
			}
		}

		private void SendLotteryMsg(RES_SHOPBUY_TYPE LotteryType, RES_SHOPDRAW_SUBTYPE LotterySubType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1113u);
			CSPKG_CMD_SHOPBUY cSPKG_CMD_SHOPBUY = new CSPKG_CMD_SHOPBUY();
			cSPKG_CMD_SHOPBUY.iBuyType = (int)LotteryType;
			cSPKG_CMD_SHOPBUY.iBuySubType = (int)LotterySubType;
			cSPkg.stPkgData.stShopBuyReq = cSPKG_CMD_SHOPBUY;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ResetRewardItemCells()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			this.m_CurrentRewardIdx = 0;
			byte b = 0;
			while ((int)b < this.m_ItemCellFlag.Length)
			{
				this.m_ItemCellFlag[(int)b] = false;
				GameObject gameObject = Utility.FindChild(form.gameObject, string.Format("pnlBodyBg/pnlSymbolGift/pnlResult/itemCell{0}", b));
				CUICommonSystem.PlayAnimator(gameObject, "End");
				gameObject.CustomSetActive(false);
				this.m_AdvSymbolFlag[(int)b] = false;
				this.m_AdvSymbolShowFlag[(int)b] = true;
				this.m_AdvSymbolIds[(int)b] = 0u;
				b += 1;
			}
		}

		private static void ShowLotteryResult(CUIFormScript form, byte bRewardCnt)
		{
			if (form == null)
			{
				return;
			}
			CMallSymbolGiftController instance = Singleton<CMallSymbolGiftController>.GetInstance();
			instance.ResetRewardItemCells();
			Singleton<CMallSymbolGiftController>.GetInstance().ToggleResMask(form, true);
			if (bRewardCnt != 0)
			{
				if (bRewardCnt != 1)
				{
					System.Random random = new System.Random();
					int i = instance.astRewardList.Length;
					while (i > 1)
					{
						i--;
						int num = random.Next(i + 1);
						COMDT_REWARD_INFO cOMDT_REWARD_INFO = instance.astRewardList[num];
						instance.astRewardList[num] = instance.astRewardList[i];
						instance.astRewardList[i] = cOMDT_REWARD_INFO;
					}
					instance.m_ItemCellFlag[0] = false;
					instance.m_AdvSymbolFlag[0] = false;
					instance.m_AdvSymbolShowFlag[0] = true;
					instance.m_AdvSymbolIds[0] = 0u;
					byte b = 1;
					while ((int)b < instance.m_ItemCellFlag.Length)
					{
						instance.m_ItemCellFlag[(int)b] = true;
						if (CMallSymbolGiftController.IsAdvSymbol(instance.astRewardList[(int)(b - 1)]))
						{
							instance.m_AdvSymbolFlag[(int)b] = true;
							instance.m_AdvSymbolShowFlag[(int)b] = false;
							instance.m_AdvSymbolIds[(int)b] = instance.astRewardList[(int)(b - 1)].stRewardInfo.stSymbol.dwSymbolID;
						}
						else
						{
							instance.m_AdvSymbolFlag[(int)b] = false;
							instance.m_AdvSymbolShowFlag[(int)b] = true;
							instance.m_AdvSymbolIds[(int)b] = 0u;
						}
						if (instance.astRewardList[(int)(b - 1)] != null)
						{
							CMallSymbolGiftController.SetRewardItemCell(form, b, instance.astRewardList[(int)(b - 1)]);
						}
						else
						{
							instance.m_ItemCellFlag[(int)b] = false;
						}
						b += 1;
					}
				}
				else
				{
					instance.m_ItemCellFlag[0] = true;
					if (CMallSymbolGiftController.IsAdvSymbol(instance.astRewardList[0]))
					{
						instance.m_AdvSymbolFlag[0] = true;
						instance.m_AdvSymbolShowFlag[0] = false;
						instance.m_AdvSymbolIds[0] = instance.astRewardList[0].stRewardInfo.stSymbol.dwSymbolID;
					}
					CMallSymbolGiftController.SetRewardItemCell(form, 0, instance.astRewardList[0]);
					byte b2 = 1;
					while ((int)b2 < instance.m_ItemCellFlag.Length)
					{
						instance.m_ItemCellFlag[(int)b2] = false;
						b2 += 1;
					}
				}
				return;
			}
			byte b3 = 0;
			while ((int)b3 < instance.m_ItemCellFlag.Length)
			{
				instance.m_ItemCellFlag[(int)b3] = false;
				b3 += 1;
			}
			Singleton<CMallSymbolGiftController>.GetInstance().ToggleActionMask(form, false, 30f);
			Singleton<CMallSymbolGiftController>.GetInstance().ToggleResMask(form, false);
		}

		private static void SetRewardItemCell(CUIFormScript formScript, byte itemCellIndex, COMDT_REWARD_INFO stRewardInfo)
		{
			CUseable itemUseable = null;
			GameObject gameObject = formScript.transform.Find(string.Format("pnlBodyBg/pnlSymbolGift/pnlResult/itemCell{0}", itemCellIndex)).gameObject;
			if (gameObject == null)
			{
				return;
			}
			switch (stRewardInfo.bType)
			{
			case 1:
				itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, stRewardInfo.stRewardInfo.stItem.dwItemID, (int)stRewardInfo.stRewardInfo.stItem.dwCnt);
				break;
			case 2:
				itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, (int)stRewardInfo.stRewardInfo.stExp.bReserve);
				break;
			case 4:
				itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, stRewardInfo.stRewardInfo.stEquip.dwEquipID, (int)stRewardInfo.stRewardInfo.stEquip.dwCnt);
				break;
			case 5:
				itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, stRewardInfo.stRewardInfo.stHero.dwHeroID, (int)stRewardInfo.stRewardInfo.stHero.dwCnt);
				break;
			case 6:
				itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, stRewardInfo.stRewardInfo.stSymbol.dwSymbolID, 0);
				break;
			case 10:
				itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, stRewardInfo.stRewardInfo.stSkin.dwSkinID, (int)stRewardInfo.stRewardInfo.stSkin.dwCnt);
				break;
			case 11:
				itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int)stRewardInfo.stRewardInfo.dwPvpCoin);
				break;
			case 13:
				itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSkinCoin, (int)stRewardInfo.stRewardInfo.dwSkinCoin);
				break;
			case 14:
				itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, (int)stRewardInfo.stRewardInfo.dwSymbolCoin);
				break;
			case 16:
				itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int)stRewardInfo.stRewardInfo.dwDiamond);
				break;
			case 20:
				itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG, stRewardInfo.stRewardInfo.stHeadImage.dwHeadImgID, (int)stRewardInfo.stRewardInfo.stHeadImage.dwGetTime);
				break;
			}
			CUICommonSystem.SetItemCell(formScript, gameObject, itemUseable, true, false, false, false);
		}

		private void StartTimer(GameObject gameObject, float total = 0f, float interval = 0f)
		{
			if (gameObject != null)
			{
				CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
				if (component != null)
				{
					if (total != 0f)
					{
						component.SetTotalTime(total);
					}
					if (interval > 0f)
					{
						component.SetOnChangedIntervalTime(interval);
					}
					component.StartTimer();
				}
			}
		}

		public static bool IsAdvSymbol(COMDT_REWARD_INFO reward)
		{
			if (reward.bType == 6)
			{
				CSymbolItem cSymbolItem = CUseableManager.CreateUsableByServerType(COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL, 1, reward.stRewardInfo.stSymbol.dwSymbolID) as CSymbolItem;
				if (cSymbolItem == null)
				{
					return false;
				}
				if (cSymbolItem.m_grade >= 3)
				{
					return true;
				}
			}
			return false;
		}

		private bool HasFreeDraw(COM_SHOP_DRAW_TYPE drawType)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			return masterRoleInfo != null && drawType >= COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_COIN && drawType <= (COM_SHOP_DRAW_TYPE)masterRoleInfo.m_freeDrawInfo.Length && masterRoleInfo.m_freeDrawInfo[(int)drawType].dwLeftFreeDrawCnt > 0;
		}

		public static void ResetFreeDrawCD(COM_SHOP_DRAW_TYPE drawType)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if (drawType < COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_COIN || drawType > (COM_SHOP_DRAW_TYPE)masterRoleInfo.m_freeDrawInfo.Length)
			{
				return;
			}
			masterRoleInfo.m_freeDrawInfo[(int)drawType].dwLeftFreeDrawCnt = 0;
			int num = 0;
			if (drawType != COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLCOMMON)
			{
				if (drawType == COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLSENIOR)
				{
					num = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(123u).dwConfValue;
				}
			}
			else
			{
				num = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(122u).dwConfValue;
			}
			masterRoleInfo.m_freeDrawInfo[(int)drawType].dwLeftFreeDrawCD = CRoleInfo.GetCurrentUTCTime() + num;
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
		}

		private void OnOpenSymbolGift(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			this.Draw(cUIFormScript);
		}

		private void OnCloseSymbolGift(CUIEvent uiEvent)
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan_Stop", null);
		}

		private void OnLotteryCommonBuyOneSymbol(CUIEvent uiEvent)
		{
			this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
		}

		private void OnLotteryCommonBuyFiveSymbol(CUIEvent uiEvent)
		{
			this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
		}

		private void OnLotterySeniorBuyFreeSymbol(CUIEvent uiEvent)
		{
			this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE);
		}

		private void OnLotterySeniorBuyOneSymbol(CUIEvent uiEvent)
		{
			this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
		}

		private void OnLotterySeniorBuyFiveSymbol(CUIEvent uiEvent)
		{
			this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
		}

		private void OnLotteryBuySymbolConfirm(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			if (this.reqSent)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("请等待上一次抽奖结果服务器的返回", false, 1.5f, null, new object[0]);
				return;
			}
			this.reqSent = true;
			CMallSymbolGiftController.reqSentTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(20000, 1, delegate(int sequence)
			{
				if (this.reqSent)
				{
					this.reqSent = false;
				}
			});
			this.isSkipAni = false;
			this.ToggleActionMask(form, true, 30f);
			RES_SHOPBUY_TYPE tag = (RES_SHOPBUY_TYPE)uiEvent.m_eventParams.tag;
			RES_SHOPDRAW_SUBTYPE tag2 = (RES_SHOPDRAW_SUBTYPE)uiEvent.m_eventParams.tag2;
			this.ResetRewardItemCells();
			this.SetLotteryBool(form, "Server_Turn", false);
			this.SetLotteryBool(form, "Icon_Turn", false);
			this.PlayLotteryAnimation(form, "Open", true, false, false);
			this.ToggleActionPanel(form, false);
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			this.SendLotteryMsg(tag, tag2);
		}

		private void TryToPayForLotterySymbol(RES_SHOPBUY_TYPE lotteryType, RES_SHOPDRAW_SUBTYPE lotterySubType)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			stPayInfo drawPayInfo = this.GetDrawPayInfo(lotteryType, lotterySubType);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.Lottery_BuySymbolConfirm;
			uIEvent.m_eventParams.tag = (int)lotteryType;
			uIEvent.m_eventParams.tag2 = (int)lotterySubType;
			CMallSystem.TryToPay(enPayPurpose.Lottery, string.Empty, drawPayInfo.m_payType, drawPayInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
		}

		private void OnLotteryShowReward(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("pnlBodyBg/pnlSymbolGift/showRewardTimer").gameObject;
			CUITimerScript cUITimerScript = null;
			if (gameObject != null)
			{
				cUITimerScript = gameObject.GetComponent<CUITimerScript>();
			}
			if ((int)this.m_CurrentRewardIdx >= this.m_ItemCellFlag.Length || (!this.m_ItemCellFlag[(int)this.m_CurrentRewardIdx] && this.m_CurrentRewardIdx > 0))
			{
				this.ToggleSkipAnimationMask(form, false, 30f);
				if (cUITimerScript != null)
				{
					cUITimerScript.SetTotalTime(2f);
					cUITimerScript.SetOnChangedIntervalTime(100f);
					cUITimerScript.ReStartTimer();
				}
				return;
			}
			GameObject gameObject2 = form.transform.Find(string.Format("pnlBodyBg/pnlSymbolGift/pnlResult/itemCell{0}", this.m_CurrentRewardIdx)).gameObject;
			if (gameObject2 != null)
			{
				if (this.m_ItemCellFlag[(int)this.m_CurrentRewardIdx])
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_fuwen_chuxian", null);
					gameObject2.CustomSetActive(true);
					this.StartTimer(gameObject2, 1.25f, 1f);
				}
				CUIAnimatorScript component = gameObject2.GetComponent<CUIAnimatorScript>();
				CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(gameObject2, string.Empty);
				if (componetInChild != null && component != null)
				{
					CSymbolItem cSymbolItem = componetInChild.m_onUpEventParams.iconUseable as CSymbolItem;
					if (cSymbolItem != null)
					{
						int value = (int)(cSymbolItem.m_grade + 1);
						switch (value)
						{
						case 1:
						case 2:
						case 3:
						case 4:
						case 5:
							component.SetInteger("Level", value);
							break;
						default:
							component.SetInteger("Level", 3);
							break;
						}
					}
				}
			}
			this.m_CurrentRewardIdx += 1;
		}

		private void OnLotterySymbolBoom(CUIEvent uiEvent)
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_fuwen_bao", null);
		}

		private void OnGoldFreeDrawCdUp(CUIEvent uiEvent)
		{
			this.RefreshButtonView(uiEvent.m_srcFormScript);
		}

		private void OnSkipAnimation(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			this.isSkipAni = true;
			this.ToggleSkipAnimationMask(form, false, 30f);
			GameObject gameObject = Utility.FindChild(form.gameObject, "UIScene_Lottery/Home_TransferGate");
			if (gameObject != null)
			{
				CUIAnimatorScript component = gameObject.GetComponent<CUIAnimatorScript>();
				component.m_eventIDs[1] = enUIEventID.None;
				Singleton<CSoundManager>.GetInstance().PostEvent("Stop_erjijiemian", null);
				this.PlayLotteryAnimation(form, "Close_1", true, false, false);
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_close", null);
			}
			this.StartTimer(form.transform.Find(string.Format("pnlBodyBg/pnlSymbolGift/showRewardTimer", new object[0])).gameObject, 1000f, 0.04f);
		}

		private void onActionMaskReset(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			this.ToggleActionPanel(form, true);
		}

		private void OnLotteryCloseFx(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "UIScene_Lottery/Home_TransferGate");
			if (gameObject != null)
			{
				CUIAnimatorScript component = gameObject.GetComponent<CUIAnimatorScript>();
				if (component != null)
				{
					component.m_eventIDs[1] = enUIEventID.None;
				}
			}
			this.SetLotteryBool(form, "Icon_Turn", true);
			Singleton<CTimerManager>.GetInstance().AddTimer(1000, 1, delegate(int sequence)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_close", null);
			});
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Get_Product_OK);
			this.ToggleActionMask(form, false, 30f);
			this.ToggleActionPanel(form, true);
		}

		private void OnLotteryShowRewardEnd(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			CUseableContainer cUseableContainer = new CUseableContainer(enCONTAINER_TYPE.ITEM);
			for (int i = 0; i < this.m_AdvSymbolIds.Length; i++)
			{
				if (this.m_AdvSymbolIds[i] != 0u)
				{
					CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.m_AdvSymbolIds[i], 1);
					cUseableContainer.Add(useable);
				}
			}
			if (cUseableContainer.GetCurUseableCount() == 0)
			{
				if (!this.isSkipAni)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lottery_Close_FX);
				}
				else
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Get_Product_OK);
					this.ToggleActionMask(form, false, 30f);
					this.ToggleActionPanel(form, true);
				}
			}
			else if (!this.isSkipAni)
			{
				CUICommonSystem.ShowSymbol(cUseableContainer, enUIEventID.Lottery_Close_FX);
			}
			else
			{
				CUICommonSystem.ShowSymbol(cUseableContainer, enUIEventID.Mall_Get_AWARD_CLOSE_FORM);
				this.ToggleActionMask(form, false, 30f);
				this.ToggleActionPanel(form, true);
			}
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int j = 0; j < this.m_ItemCellFlag.Length; j++)
			{
				if (this.m_ItemCellFlag[j])
				{
					CUseable cUseable;
					if (j == 0)
					{
						cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.astRewardList[j].stRewardInfo.stSymbol.dwSymbolID, 1);
					}
					else
					{
						cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.astRewardList[j - 1].stRewardInfo.stSymbol.dwSymbolID, 1);
					}
					if (cUseable != null)
					{
						listView.Add(cUseable);
					}
				}
			}
			int count = listView.Count;
			if (count != 0)
			{
				CUseable[] array = new CUseable[listView.Count];
				for (int k = 0; k < count; k++)
				{
					array[k] = listView[k];
				}
				Singleton<CUIManager>.GetInstance().OpenAwardTip(array, Singleton<CTextManager>.GetInstance().GetText("gotAward"), false, enUIEventID.None, false, false, "Form_Award");
			}
		}

		private void OnLotteryShowRewardStart(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan_menkai", null);
			if ((int)this.m_CurrentRewardIdx >= this.m_ItemCellFlag.Length || (!this.m_ItemCellFlag[(int)this.m_CurrentRewardIdx] && this.m_CurrentRewardIdx > 0))
			{
				this.ToggleActionMask(form, false, 30f);
				this.ToggleSkipAnimationMask(form, false, 30f);
				this.ToggleResMask(form, false);
				this.ToggleActionPanel(form, true);
				return;
			}
			this.StartTimer(form.transform.Find(string.Format("pnlBodyBg/pnlSymbolGift/showRewardTimer", new object[0])).gameObject, 1000f, 0.3f);
		}

		public static void ReceiveLotteryRes(CSPkg msg)
		{
			Singleton<CMallSymbolGiftController>.GetInstance().reqSent = false;
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref CMallSymbolGiftController.reqSentTimerSeq);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
			if (form == null)
			{
				return;
			}
			CMallSymbolGiftController instance = Singleton<CMallSymbolGiftController>.GetInstance();
			SCPKG_CMD_SHOPBUY stShopBuyRsp = msg.stPkgData.stShopBuyRsp;
			RES_SHOPBUY_TYPE iBuyType = (RES_SHOPBUY_TYPE)stShopBuyRsp.iBuyType;
			RES_SHOPDRAW_SUBTYPE iBuySubType = (RES_SHOPDRAW_SUBTYPE)stShopBuyRsp.iBuySubType;
			RES_SHOPBUY_TYPE rES_SHOPBUY_TYPE = iBuyType;
			if (rES_SHOPBUY_TYPE != RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON)
			{
				if (rES_SHOPBUY_TYPE == RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR)
				{
					if (iBuySubType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE)
					{
						CMallSymbolGiftController.ResetFreeDrawCD(COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLSENIOR);
						Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallSystem.Tab>(EventID.Mall_Refresh_Tab_Red_Dot, CMallSystem.Tab.Symbol_Make);
						Singleton<CMallSymbolGiftController>.GetInstance().RefreshButtonView(form);
					}
					if (iBuySubType >= RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE && iBuySubType < RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_MAX)
					{
						CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[(int)iBuySubType]++;
						if (CMallSymbolGiftController.GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR) >= 1000)
						{
							CMallSymbolGiftController.ResetDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR);
						}
					}
				}
			}
			else
			{
				if (iBuySubType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE)
				{
					CMallSymbolGiftController.ResetFreeDrawCD(COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLCOMMON);
					Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallSystem.Tab>(EventID.Mall_Refresh_Tab_Red_Dot, CMallSystem.Tab.Symbol_Make);
					Singleton<CMallSymbolGiftController>.GetInstance().RefreshButtonView(form);
				}
				if (iBuySubType >= RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE && iBuySubType < RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_MAX)
				{
					CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[(int)iBuySubType]++;
					if (CMallSymbolGiftController.GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON) >= 1000)
					{
						CMallSymbolGiftController.ResetDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON);
					}
				}
			}
			Singleton<CMallSymbolGiftController>.GetInstance().RefreshDesc(form);
			byte b = 0;
			if (stShopBuyRsp.stBuyResult.bRewardCnt > 0)
			{
				for (int i = 0; i < (int)stShopBuyRsp.stBuyResult.bRewardCnt; i++)
				{
					COMDT_REWARD_DETAIL cOMDT_REWARD_DETAIL = stShopBuyRsp.stBuyResult.astRewardInfo[i];
					if (cOMDT_REWARD_DETAIL.bNum > 0)
					{
						instance.astRewardList[(int)b] = cOMDT_REWARD_DETAIL.astRewardDetail[(int)(cOMDT_REWARD_DETAIL.bNum - 1)];
						b += 1;
					}
				}
			}
			if (b > 0)
			{
				Singleton<CMallSymbolGiftController>.GetInstance().ToggleSkipAnimationMask(form, true, 30f);
				CMallSymbolGiftController.ShowLotteryResult(form, b);
				Singleton<CMallSymbolGiftController>.GetInstance().SetLotteryBool(form, "Server_Turn", true);
				GameObject gameObject = Utility.FindChild(form.gameObject, "UIScene_Lottery/Home_TransferGate");
				if (gameObject != null)
				{
					CUIAnimatorScript component = gameObject.GetComponent<CUIAnimatorScript>();
					if (component != null && component.m_currentAnimatorStateName != null)
					{
						component.m_eventIDs[1] = enUIEventID.Lottery_Show_Reward_Start;
					}
					else
					{
						Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lottery_Show_Reward_Start);
					}
				}
			}
			else
			{
				Singleton<CMallSymbolGiftController>.GetInstance().ResetRewardItemCells();
				Singleton<CMallSymbolGiftController>.GetInstance().ToggleActionMask(form, false, 30f);
				Singleton<CMallSymbolGiftController>.GetInstance().ToggleResMask(form, false);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.LOTTERY_GET_NEW_SYMBOL);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		private void RefreshButtonView(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlCommonBuy/btnBuyOne/BuyButton").gameObject;
			GameObject gameObject2 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlCommonBuy/btnBuyFive/BuyButton").gameObject;
			GameObject gameObject3 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOne").gameObject;
			GameObject gameObject4 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOneFree").gameObject;
			GameObject gameObject5 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOne/BuyButton").gameObject;
			GameObject gameObject6 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyFive/BuyButton").gameObject;
			GameObject gameObject7 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/pnlCd").gameObject;
			GameObject gameObject8 = form.transform.Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOneFree/BuyButton").gameObject;
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(form.gameObject, "pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/pnlCd/Timer");
			stUIEventParams stUIEventParams = default(stUIEventParams);
			if (gameObject != null)
			{
				stPayInfo drawPayInfo = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
				CMallSystem.SetPayButton(form, gameObject.transform as RectTransform, drawPayInfo.m_payType, drawPayInfo.m_payValue, drawPayInfo.m_oriValue, enUIEventID.Lottery_Common_BuyOneSymbol, ref stUIEventParams);
			}
			if (gameObject2 != null)
			{
				stPayInfo drawPayInfo2 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
				CMallSystem.SetPayButton(form, gameObject2.transform as RectTransform, drawPayInfo2.m_payType, drawPayInfo2.m_payValue, drawPayInfo2.m_oriValue, enUIEventID.Lottery_Common_BuyFiveSymbol, ref stUIEventParams);
			}
			bool flag = this.HasFreeDraw(COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLSENIOR);
			if (flag)
			{
				gameObject4.CustomSetActive(true);
				gameObject3.CustomSetActive(false);
				gameObject7.CustomSetActive(false);
				if (gameObject8 != null)
				{
					stPayInfo drawPayInfo3 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE);
					CMallSystem.SetPayButton(form, gameObject8.transform as RectTransform, drawPayInfo3.m_payType, drawPayInfo3.m_payValue, drawPayInfo3.m_oriValue, enUIEventID.Lottery_Senior_BuyFreeSymbol, ref stUIEventParams);
					Transform transform = gameObject8.transform.FindChild("PriceContainer/Price");
					if (transform != null)
					{
						CUICommonSystem.AddRedDot(transform.gameObject, enRedDotPos.enTopRight, 0, 0, 0);
					}
				}
			}
			else
			{
				gameObject4.CustomSetActive(false);
				gameObject3.CustomSetActive(true);
				gameObject7.CustomSetActive(true);
				int num = Math.Max(0, masterRoleInfo.m_freeDrawInfo[4].dwLeftFreeDrawCD - CRoleInfo.GetCurrentUTCTime());
				if (componetInChild != null)
				{
					componetInChild.m_eventIDs[1] = enUIEventID.None;
					componetInChild.EndTimer();
					componetInChild.m_eventIDs[1] = enUIEventID.Lottery_Gold_Free_Draw_CD_UP;
					componetInChild.SetTotalTime((float)num);
					componetInChild.StartTimer();
				}
				if (gameObject5 != null)
				{
					stPayInfo drawPayInfo4 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
					CMallSystem.SetPayButton(form, gameObject5.transform as RectTransform, drawPayInfo4.m_payType, drawPayInfo4.m_payValue, drawPayInfo4.m_oriValue, enUIEventID.Lottery_Senior_BuyOneSymbol, ref stUIEventParams);
				}
			}
			if (gameObject6 != null)
			{
				stPayInfo drawPayInfo5 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
				CMallSystem.SetPayButton(form, gameObject6.transform as RectTransform, drawPayInfo5.m_payType, drawPayInfo5.m_payValue, drawPayInfo5.m_oriValue, enUIEventID.Lottery_Senior_BuyFiveSymbol, ref stUIEventParams);
			}
		}

		private stPayInfo GetDrawPayInfo(RES_SHOPBUY_TYPE buyType, RES_SHOPDRAW_SUBTYPE subType)
		{
			stPayInfo result = default(stPayInfo);
			ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(buyType, (int)subType);
			DebugHelper.Assert(cfgShopInfo != null, string.Format("购买信息表，符文抽奖配置不对{0}:{1}", buyType, subType));
			if (cfgShopInfo != null)
			{
				result.m_payType = CMallSystem.ResBuyTypeToPayType((int)cfgShopInfo.bCoinType);
				result.m_payValue = cfgShopInfo.dwCoinPrice;
				result.m_oriValue = cfgShopInfo.dwCoinPrice;
				ResShopPromotion shopPromotion = CMallSystem.GetShopPromotion(buyType, subType);
				if (shopPromotion != null)
				{
					result.m_payValue = shopPromotion.dwPrice;
				}
			}
			return result;
		}

		private void OnMallTabChange()
		{
		}
	}
}
