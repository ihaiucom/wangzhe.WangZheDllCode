using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CMallFactoryShopController : Singleton<CMallFactoryShopController>
	{
		public class ShopProduct
		{
			public delegate void StateChangeDelegate(CMallFactoryShopController.ShopProduct sp);

			private ResSpecSale _config;

			private bool propGiftSet;

			private bool _isPropGift;

			private COM_PROP_GIFT_USE_TYPE _propGiftUseType;

			private uint _boughtCount;

			public bool m_bChangeGiftPrice;

			public uint m_newGiftPrice;

			private ListView<CUseable> _rewardList = new ListView<CUseable>();

			public event CMallFactoryShopController.ShopProduct.StateChangeDelegate OnStateChange
			{
				[MethodImpl(32)]
				add
				{
					this.OnStateChange = (CMallFactoryShopController.ShopProduct.StateChangeDelegate)Delegate.Combine(this.OnStateChange, value);
				}
				[MethodImpl(32)]
				remove
				{
					this.OnStateChange = (CMallFactoryShopController.ShopProduct.StateChangeDelegate)Delegate.Remove(this.OnStateChange, value);
				}
			}

			public bool IsPropGift
			{
				get
				{
					if (this.propGiftSet)
					{
						return this._isPropGift;
					}
					if (this._config.dwSpecSaleType == 2u)
					{
						ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(this.ID);
						if (dataByKey.bType == 4)
						{
							this._isPropGift = true;
							if ((int)dataByKey.EftParam[1] == 1)
							{
								this._propGiftUseType = COM_PROP_GIFT_USE_TYPE.COM_PROP_GIFT_USE_TYPE_IMMI;
							}
							else
							{
								this._propGiftUseType = COM_PROP_GIFT_USE_TYPE.COM_PROP_GIFT_USE_TYPE_DELAY;
							}
						}
						else
						{
							this._isPropGift = false;
						}
					}
					else
					{
						this._isPropGift = false;
					}
					this.propGiftSet = true;
					return this._isPropGift;
				}
			}

			public bool IsPropGiftUseImm
			{
				get
				{
					if (this.propGiftSet)
					{
						return this._propGiftUseType == COM_PROP_GIFT_USE_TYPE.COM_PROP_GIFT_USE_TYPE_IMMI;
					}
					return this.IsPropGift && this._propGiftUseType == COM_PROP_GIFT_USE_TYPE.COM_PROP_GIFT_USE_TYPE_IMMI;
				}
			}

			public uint Key
			{
				get
				{
					return this._config.dwId;
				}
			}

			public long DoubleKey
			{
				get
				{
					return GameDataMgr.GetDoubleKey(this._config.dwSpecSaleType, this._config.dwSpecSaleId);
				}
			}

			public COM_ITEM_TYPE Type
			{
				get
				{
					return (COM_ITEM_TYPE)this._config.dwSpecSaleType;
				}
			}

			public uint ID
			{
				get
				{
					return this._config.dwSpecSaleId;
				}
			}

			public RES_SHOPBUY_COINTYPE CoinType
			{
				get
				{
					return (RES_SHOPBUY_COINTYPE)this._config.bCostType;
				}
			}

			public float RealDiscount
			{
				get
				{
					return this._config.fDiscount;
				}
			}

			public uint DiscountForShow
			{
				get
				{
					return this._config.dwDiscountForDisplay;
				}
			}

			public uint BoughtCount
			{
				get
				{
					return this._boughtCount;
				}
				set
				{
					if (value != this._boughtCount)
					{
						this._boughtCount = value;
						if (this.OnStateChange != null)
						{
							this.OnStateChange(this);
						}
					}
				}
			}

			public uint LimitCount
			{
				get
				{
					return this._config.dwBuyLimitNum;
				}
			}

			public uint LimitCycle
			{
				get
				{
					return this._config.dwPurchaseCycle / 86400u;
				}
			}

			public int RecommendIndex
			{
				get
				{
					return this._config.iRecommend;
				}
			}

			public bool IsShowLimitBuy
			{
				get
				{
					return this._config.bShowLimitBuy > 0;
				}
			}

			public byte Tag
			{
				get
				{
					return this._config.bTag;
				}
			}

			public uint Tab
			{
				get
				{
					return this._config.dwTab;
				}
			}

			public int LimitBuyDays
			{
				get
				{
					DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					DateTime dateTime2 = new DateTime(dateTime.get_Year(), dateTime.get_Month(), dateTime.get_Day(), 0, 0, 0, 2);
					if (this.OnSaleTime.CompareTo(dateTime2) >= 0)
					{
						return (int)this.OffSaleTime.Subtract(this.OnSaleTime).get_TotalDays();
					}
					if (this.OffSaleTime.CompareTo(dateTime2) >= 0)
					{
						return (int)this.OffSaleTime.Subtract(dateTime2).get_TotalDays();
					}
					return -1;
				}
			}

			public int IsOnSale
			{
				get
				{
					DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					int result = (dateTime.CompareTo(this.OnSaleTime) >= 0 && dateTime.CompareTo(this.OffSaleTime) <= 0) ? 1 : -1;
					if (this.LimitCount > 0u && this.LimitCycle <= 0u && this.BoughtCount == this.LimitCount)
					{
						return -2;
					}
					if (this.IsVipLimited())
					{
						return -3;
					}
					return result;
				}
			}

			public DateTime OnSaleTime
			{
				get
				{
					return Utility.StringToDateTime(Utility.UTF8Convert(this._config.szOnTime), DateTime.MinValue);
				}
			}

			public DateTime OffSaleTime
			{
				get
				{
					return Utility.StringToDateTime(Utility.UTF8Convert(this._config.szOffTime), DateTime.MaxValue);
				}
			}

			public ShopProduct(ResSpecSale config)
			{
				this._config = config;
				this.InitRandomReawrd();
			}

			private void InitRandomReawrd()
			{
				if (this._config.dwSpecSaleType == 2u)
				{
					ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(this.ID);
					if (dataByKey.bType == 4)
					{
						int num = (int)dataByKey.EftParam[0];
						bool flag = dataByKey.EftParam[5] != 0f;
						if (num > 0 && flag)
						{
							ResRandomRewardStore dataByKey2 = GameDataMgr.randomRewardDB.GetDataByKey((long)num);
							if (dataByKey2 != null)
							{
								for (int i = 0; i < dataByKey2.astRewardDetail.Length; i++)
								{
									if (dataByKey2.astRewardDetail[i].bItemType == 0 || dataByKey2.astRewardDetail[i].bItemType >= 18)
									{
										break;
									}
									if (dataByKey2.astRewardDetail[i].bItemType == 4 || dataByKey2.astRewardDetail[i].bItemType == 11 || dataByKey2.astRewardDetail[i].bItemType == 3)
									{
										ListView<CUseable> listView = CUseableManager.CreateUsableListByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey2.astRewardDetail[i].bItemType, (int)dataByKey2.astRewardDetail[i].dwLowCnt, dataByKey2.astRewardDetail[i].dwItemID);
										if (listView != null && listView.Count > 0)
										{
											this._rewardList.AddRange(listView);
										}
									}
								}
							}
						}
					}
				}
			}

			public string isHaveHeroSkin()
			{
				string text = string.Empty;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return text;
				}
				for (int i = 0; i < this._rewardList.Count; i++)
				{
					uint baseID = this._rewardList[i].m_baseID;
					if (this._rewardList[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
					{
						if (masterRoleInfo.IsHaveHero(baseID, false))
						{
							if (text != string.Empty)
							{
								text = text + "," + this._rewardList[i].m_name;
							}
							else
							{
								text += this._rewardList[i].m_name;
							}
						}
					}
					else if (this._rewardList[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
					{
						ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(baseID);
						if (heroSkin != null && masterRoleInfo.IsHaveHeroSkin(baseID, false))
						{
							if (text != string.Empty)
							{
								text = text + "," + this._rewardList[i].m_name;
							}
							else
							{
								text += this._rewardList[i].m_name;
							}
						}
					}
				}
				return text;
			}

			public uint ConvertWithRealDiscount(uint cost)
			{
				return (uint)(cost * (double)this.RealDiscount / 10000.0);
			}

			public static uint SConvertWithRealDiscount(uint cost, float realDiscount)
			{
				return (uint)(cost * (double)realDiscount / 10000.0);
			}

			public string GetSpecialIconPath()
			{
				if (string.IsNullOrEmpty(this._config.szSpecialIcon))
				{
					return null;
				}
				return CUIUtility.s_Sprite_Dynamic_Mall_Dir + this._config.szSpecialIcon;
			}

			public bool IsVipLimited()
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				DebugHelper.Assert(masterRoleInfo != null, "IsVipLimited::Master Role Info Is Null");
				if (masterRoleInfo == null)
				{
					return true;
				}
				uint num = 0u;
				COMDT_GAME_VIP_CLIENT stGameVipClient = masterRoleInfo.GetNobeInfo().stGameVipClient;
				if (stGameVipClient != null)
				{
					num = masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel;
				}
				return this._config.dwStartVIPLvl > num || this._config.dwEndVIPLvl < num;
			}

			public bool CanBuy()
			{
				if (this.LimitCount > 0u && this.BoughtCount >= this.LimitCount)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("specProdOutLimit"), false);
					return false;
				}
				if (this.IsOnSale != 1)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("specProdOffSale"), false);
					return false;
				}
				if (this.Type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || this.Type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (this.Type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
					{
						CHeroInfo cHeroInfo;
						if (masterRoleInfo.GetHeroInfo(this.ID, out cHeroInfo, false))
						{
							Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("prodOutOfStackLimit"), false);
							return false;
						}
					}
					else if (this.Type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
					{
						CHeroSkin cHeroSkin = (CHeroSkin)CUseableManager.CreateUseable(this.Type, this.ID, 0);
						if (masterRoleInfo.IsHaveHeroSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, false))
						{
							Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("prodOutOfStackLimit"), false);
							return false;
						}
					}
				}
				else if (this.Type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					CUseableContainer useableContainer = masterRoleInfo2.GetUseableContainer(enCONTAINER_TYPE.ITEM);
					int useableStackCount = useableContainer.GetUseableStackCount(this.Type, this.ID);
					CUseable cUseable = CUseableManager.CreateUseable(this.Type, this.ID, 1);
					if (useableStackCount >= cUseable.m_stackMax)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("prodOutOfStackLimit"), false);
						return false;
					}
					if (CItem.IsHeroExperienceCard(this.ID) && masterRoleInfo2.IsHaveHero(CItem.GetExperienceCardHeroOrSkinId(this.ID), false))
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("ExpCard_AlreadyHaveHero"), false);
						return false;
					}
					if (CItem.IsSkinExperienceCard(this.ID) && masterRoleInfo2.IsHaveHeroSkin(CItem.GetExperienceCardHeroOrSkinId(this.ID), false))
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("ExpCard_AlreadyHaveSkin"), false);
						return false;
					}
				}
				else
				{
					CUseableContainer useableContainer2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
					CUseable useableByBaseID = useableContainer2.GetUseableByBaseID(this.Type, this.ID);
					if (useableByBaseID != null && useableByBaseID.m_stackCount >= useableByBaseID.m_stackMax)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("prodOutOfStackLimit"), false);
						return false;
					}
				}
				return true;
			}
		}

		private class ShopProductWidget
		{
			public GameObject root;

			public GameObject iconObj;

			public Image icon;

			public GameObject iconFrameObj;

			public Image iconFrame;

			public GameObject iconMarkObj;

			public Image iconMark;

			public Image coinType;

			public Text coinCost;

			public Text name;

			public Text desc;

			public GameObject limitObj;

			public Text limitCount;

			public Text limitCycle;

			public GameObject LimitTimeObj;

			public Text LimitTimeCount;

			public GameObject TagObj;

			public Image tagImage;

			public Text tagText;

			public GameObject OldPriceObj;

			public Text OldPriceText;

			public GameObject HeighLightObj;

			public GameObject buyBtnObj;

			public GameObject bigIconObj;

			public GameObject smallIconObj;

			public CMallFactoryShopController.ShopProduct data;

			public ShopProductWidget(GameObject node, CMallFactoryShopController.ShopProduct spData)
			{
				this.root = node;
				this.data = spData;
				this.iconObj = Utility.FindChild(node, "Icon");
				this.icon = Utility.GetComponetInChild<Image>(node, "Icon");
				this.iconFrameObj = Utility.FindChild(node, "IconFrame");
				this.iconFrame = Utility.GetComponetInChild<Image>(node, "IconFrame");
				this.iconMarkObj = Utility.FindChild(node, "IconMark");
				this.iconMark = Utility.GetComponetInChild<Image>(node, "IconMark");
				this.coinType = Utility.GetComponetInChild<Image>(node, "CoinPL/CoinType");
				this.coinCost = Utility.GetComponetInChild<Text>(node, "CoinPL/CoinCost");
				this.name = Utility.GetComponetInChild<Text>(node, "Name");
				this.desc = Utility.GetComponetInChild<Text>(node, "Desc");
				this.limitObj = Utility.FindChild(node, "Limit");
				this.limitCount = Utility.GetComponetInChild<Text>(this.limitObj, "Count");
				this.limitCycle = Utility.GetComponetInChild<Text>(this.limitObj, "Text");
				this.LimitTimeObj = Utility.FindChild(node, "LimitTime");
				this.LimitTimeCount = Utility.GetComponetInChild<Text>(this.LimitTimeObj, "Count");
				this.TagObj = Utility.FindChild(node, "New");
				this.tagImage = Utility.GetComponetInChild<Image>(node, "New");
				this.tagText = Utility.GetComponetInChild<Text>(node, "New/Text");
				this.OldPriceObj = Utility.FindChild(node, "CoinPL/CoinOldCost");
				this.OldPriceText = Utility.GetComponetInChild<Text>(node, "CoinPL/CoinOldCost");
				this.HeighLightObj = Utility.FindChild(node, "HeighLight");
				this.buyBtnObj = Utility.FindChild(node, "BuyBtn");
				this.bigIconObj = Utility.FindChild(node, "BigIcon");
				this.smallIconObj = Utility.FindChild(node, "CoinPL/SmallIcon");
				this.data.OnStateChange += new CMallFactoryShopController.ShopProduct.StateChangeDelegate(this.OnProductStateChange);
				this.Validate();
			}

			public void Clear()
			{
				this.data.OnStateChange -= new CMallFactoryShopController.ShopProduct.StateChangeDelegate(this.OnProductStateChange);
			}

			private void OnProductStateChange(CMallFactoryShopController.ShopProduct sp)
			{
				this.Validate();
			}

			public void Validate()
			{
				if (!Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
				{
					return;
				}
				if (this.data == null)
				{
					return;
				}
				CUseable cUseable = CUseableManager.CreateCoinUseable(this.data.CoinType, 0);
				CUseable cUseable2 = CUseableManager.CreateUseable(this.data.Type, this.data.ID, 0);
				string specialIconPath = this.data.GetSpecialIconPath();
				if (specialIconPath == null)
				{
					this.bigIconObj.CustomSetActive(false);
					this.smallIconObj.CustomSetActive(false);
					this.iconObj.CustomSetActive(true);
					if (this.icon != null && cUseable2 != null)
					{
						this.icon.SetSprite(CUIUtility.GetSpritePrefeb(cUseable2.GetIconPath(), false, false), false);
					}
					if (this.iconFrame != null && this.iconMark != null)
					{
						if (this.data.Type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
						{
							if (CItem.IsHeroExperienceCard(this.data.ID))
							{
								this.iconFrame.gameObject.CustomSetActive(true);
								this.iconFrame.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardFramePath, false, false), false);
								this.iconMark.gameObject.CustomSetActive(true);
								this.iconMark.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
							}
							else if (CItem.IsSkinExperienceCard(this.data.ID))
							{
								this.iconFrame.gameObject.CustomSetActive(true);
								this.iconFrame.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardFramePath, false, false), false);
								this.iconMark.gameObject.CustomSetActive(true);
								this.iconMark.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
							}
							else
							{
								this.iconFrame.gameObject.CustomSetActive(false);
								this.iconMark.gameObject.CustomSetActive(false);
							}
						}
						else
						{
							this.iconFrame.gameObject.CustomSetActive(false);
							this.iconMark.gameObject.CustomSetActive(false);
						}
					}
				}
				else
				{
					this.bigIconObj.CustomSetActive(true);
					this.smallIconObj.CustomSetActive(true);
					this.iconObj.CustomSetActive(false);
					this.iconFrameObj.CustomSetActive(false);
					this.iconMarkObj.CustomSetActive(false);
					Image component = this.bigIconObj.GetComponent<Image>();
					if (component != null)
					{
						component.SetSprite(CUIUtility.GetSpritePrefeb(specialIconPath, false, false), false);
					}
				}
				if (this.smallIconObj != null && cUseable2 != null)
				{
					Image component2 = this.smallIconObj.GetComponent<Image>();
					component2.SetSprite(CUIUtility.GetSpritePrefeb(cUseable2.GetIconPath(), false, false), false);
				}
				if (this.coinType != null && cUseable != null)
				{
					this.coinType.SetSprite(CUIUtility.GetSpritePrefeb(cUseable.GetIconPath(), false, false), false);
				}
				if (this.coinCost != null && cUseable2 != null && this.data != null)
				{
					this.coinCost.set_text(this.data.ConvertWithRealDiscount(cUseable2.GetBuyPrice(this.data.CoinType)).ToString());
				}
				if (this.OldPriceObj != null && cUseable2 != null && this.data != null)
				{
					if (this.data.RealDiscount < 10000f)
					{
						this.OldPriceObj.CustomSetActive(true);
						if (this.OldPriceText != null)
						{
							this.OldPriceText.set_text(cUseable2.GetBuyPrice(this.data.CoinType).ToString());
						}
					}
					else
					{
						this.OldPriceObj.CustomSetActive(false);
					}
				}
				if (this.name != null && cUseable2 != null)
				{
					this.name.set_text(cUseable2.m_name);
				}
				if (this.desc != null && cUseable2 != null)
				{
					this.desc.set_text(cUseable2.m_description);
				}
				if (this.data != null && this.data.LimitCount > 0u)
				{
					DebugHelper.Assert(this.limitCount != null);
					this.limitObj.CustomSetActive(true);
					if (this.limitCount != null)
					{
						this.limitCount.set_text(string.Format("{0}/{1}", this.data.BoughtCount, this.data.LimitCount));
					}
					if (this.limitCycle != null)
					{
						if (this.data.LimitCycle > 0u)
						{
							uint num = this.data.LimitCycle;
							if (num != 1u)
							{
								if (num != 7u)
								{
									this.limitCycle.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Factory_Shop_Limit_Cycle_Today"));
								}
								else
								{
									this.limitCycle.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Factory_Shop_Limit_Cycle_Week"));
								}
							}
							else
							{
								this.limitCycle.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Factory_Shop_Limit_Cycle_Today"));
							}
						}
						else
						{
							this.limitCycle.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Factory_Shop_Limit_Cycle_Forever"));
						}
					}
				}
				else
				{
					this.limitObj.CustomSetActive(false);
				}
				if (this.data != null && this.data.IsShowLimitBuy && this.data.LimitBuyDays > 0)
				{
					DebugHelper.Assert(this.LimitTimeCount != null);
					if (this.LimitTimeCount != null)
					{
						this.LimitTimeCount.set_text(this.data.LimitBuyDays.ToString());
					}
					this.LimitTimeObj.CustomSetActive(true);
				}
				else
				{
					this.LimitTimeObj.CustomSetActive(false);
				}
				string productTagIconPath = CMallSystem.GetProductTagIconPath((int)this.data.Tag, false);
				RES_LUCKYDRAW_ITEMTAG tag = (RES_LUCKYDRAW_ITEMTAG)this.data.Tag;
				if (productTagIconPath == null)
				{
					this.TagObj.CustomSetActive(false);
				}
				else
				{
					this.TagObj.CustomSetActive(true);
					this.tagImage.SetSprite(productTagIconPath, Singleton<CMallSystem>.GetInstance().m_MallForm, true, false, false, false);
					if (this.tagText != null)
					{
						string text = string.Empty;
						switch (tag)
						{
						case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_UNUSUAL:
						case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_DISCOUNT:
						{
							float num2 = this.data.DiscountForShow / 10f;
							if (Math.Abs(num2 % 1f) < 1.401298E-45f)
							{
								text = string.Format("{0}折", ((int)num2).ToString("D"));
							}
							else
							{
								text = string.Format("{0}折", num2.ToString("0.0"));
							}
							break;
						}
						case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NEW:
							text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
							break;
						case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_HOT:
							text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
							break;
						default:
							text = string.Empty;
							break;
						}
						this.tagText.set_text(text);
					}
				}
				if (this.buyBtnObj != null)
				{
					this.buyBtnObj.CustomSetActive(true);
					CUIEventScript component3 = this.buyBtnObj.GetComponent<CUIEventScript>();
					component3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Product_Buy, new stUIEventParams
					{
						commonUInt32Param1 = this.data.Key
					});
				}
			}
		}

		private List<uint> _availableTabs;

		private DictionaryView<uint, ListView<CMallFactoryShopController.ShopProduct>> _spListInTab;

		private uint m_curTab;

		private int m_TargetId;

		private ListView<CMallFactoryShopController.ShopProduct> _spList;

		private DictionaryView<uint, CMallFactoryShopController.ShopProduct> _spDict;

		private ListView<CMallFactoryShopController.ShopProduct> _filteredSpList;

		private DictionaryView<uint, CMallFactoryShopController.ShopProduct> _filteredSpDict;

		private GameObject _root;

		private ListView<CMallFactoryShopController.ShopProductWidget> _widgetList;

		private uint CurTab
		{
			get
			{
				return this.m_curTab;
			}
			set
			{
				this.m_curTab = value;
			}
		}

		public int TargetID
		{
			get
			{
				int targetId = this.m_TargetId;
				this.m_TargetId = 0;
				return targetId;
			}
			set
			{
				this.m_TargetId = value;
			}
		}

		public override void Init()
		{
			base.Init();
			this.CurTab = 1u;
			this.m_TargetId = 0;
			this._spListInTab = new DictionaryView<uint, ListView<CMallFactoryShopController.ShopProduct>>();
			this._availableTabs = new List<uint>();
			this._spDict = new DictionaryView<uint, CMallFactoryShopController.ShopProduct>();
			this._spList = new ListView<CMallFactoryShopController.ShopProduct>();
			this._filteredSpList = new ListView<CMallFactoryShopController.ShopProduct>();
			this._filteredSpDict = new DictionaryView<uint, CMallFactoryShopController.ShopProduct>();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Product_Buy, new CUIEventManager.OnUIEventHandler(this.OnClickBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnClickConfirmBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Factory_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnTabChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.ManageShelf));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(this.ResetLimitBuyDaily));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Mall_Change_Tab, new Action(this.Close));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Mall_Close_Mall, new Action(this.Close));
		}

		public override void UnInit()
		{
			base.UnInit();
			this._availableTabs = null;
			this.m_TargetId = 0;
			if (this._spListInTab != null)
			{
				DictionaryView<uint, ListView<CMallFactoryShopController.ShopProduct>>.Enumerator enumerator = this._spListInTab.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, ListView<CMallFactoryShopController.ShopProduct>> current = enumerator.Current;
					current.get_Value().Clear();
				}
			}
			this._spListInTab = null;
			this._spDict = null;
			this._spList = null;
			this._filteredSpList = null;
			this._filteredSpDict = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Product_Buy, new CUIEventManager.OnUIEventHandler(this.OnClickBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnClickConfirmBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Factory_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnTabChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.ManageShelf));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(this.ResetLimitBuyDaily));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.Close));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Close_Mall, new Action(this.Close));
		}

		private void ManageShelf()
		{
			bool flag = this.HandleSaleOff();
			bool flag2 = this.HandleSaleOn();
			if (flag || flag2)
			{
				this.SortProducts();
				this._availableTabs.Sort();
			}
		}

		private bool HandleSaleOn()
		{
			bool result = false;
			if (this._filteredSpList != null && this._filteredSpList.Count != 0)
			{
				for (int i = this._filteredSpList.Count - 1; i >= 0; i--)
				{
					if (this.FilterProduct(this._filteredSpList[i]) == 1)
					{
						result = true;
					}
				}
			}
			return result;
		}

		private bool HandleSaleOff()
		{
			bool result = false;
			if (this._spList != null && this._spList.Count != 0)
			{
				for (int i = this._spList.Count - 1; i >= 0; i--)
				{
					if (this.FilterProduct(this._spList[i]) != 1)
					{
						result = true;
					}
				}
			}
			return result;
		}

		private int FilterProduct(CMallFactoryShopController.ShopProduct sp)
		{
			int isOnSale = sp.IsOnSale;
			int num = isOnSale;
			switch (num + 3)
			{
			case 0:
			case 2:
				if (!this._filteredSpDict.ContainsKey(sp.Key))
				{
					this._filteredSpDict.Add(sp.Key, sp);
					this._filteredSpList.Add(sp);
				}
				if (this._spDict.ContainsKey(sp.Key))
				{
					for (int i = this._spList.Count - 1; i >= 0; i--)
					{
						if (this._spList[i] != null && this._spList[i].Key == sp.Key)
						{
							this._spList.RemoveAt(i);
							break;
						}
					}
					this._spDict.Remove(sp.Key);
				}
				if (this._spListInTab.ContainsKey(sp.Tab))
				{
					for (int j = this._spListInTab[sp.Tab].Count - 1; j >= 0; j--)
					{
						if (this._spListInTab[sp.Tab][j] != null && this._spListInTab[sp.Tab][j].Key == sp.Key)
						{
							this._spListInTab[sp.Tab].RemoveAt(j);
							break;
						}
					}
					if (this._spListInTab[sp.Tab].Count == 0)
					{
						int num2 = this._availableTabs.FindIndex((uint v) => v == sp.Tab);
						if (num2 >= 0)
						{
							this._availableTabs.RemoveAt(num2);
						}
					}
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Off_Sale, sp);
				break;
			case 1:
				if (this._spDict.ContainsKey(sp.Key))
				{
					for (int k = this._spList.Count - 1; k >= 0; k--)
					{
						if (this._spList[k] != null && this._spList[k].Key == sp.Key)
						{
							this._spList.RemoveAt(k);
							break;
						}
					}
					this._spDict.Remove(sp.Key);
				}
				if (this._spListInTab.ContainsKey(sp.Tab))
				{
					for (int l = this._spListInTab[sp.Tab].Count - 1; l >= 0; l--)
					{
						if (this._spListInTab[sp.Tab][l] != null && this._spListInTab[sp.Tab][l].Key == sp.Key)
						{
							this._spListInTab[sp.Tab].RemoveAt(l);
							break;
						}
					}
					if (this._spListInTab[sp.Tab].Count == 0)
					{
						int num3 = this._availableTabs.FindIndex((uint v) => v == sp.Tab);
						if (num3 >= 0)
						{
							this._availableTabs.RemoveAt(num3);
						}
					}
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Off_Sale, sp);
				break;
			case 4:
				if (!this._spDict.ContainsKey(sp.Key))
				{
					this._spList.Add(sp);
					this._spDict.Add(sp.Key, sp);
					if (this._availableTabs.FindIndex((uint v) => v == sp.Tab) < 0)
					{
						this._availableTabs.Add(sp.Tab);
					}
					if (!this._spListInTab.ContainsKey(sp.Tab))
					{
						this._spListInTab.Add(sp.Tab, new ListView<CMallFactoryShopController.ShopProduct>());
					}
					this._spListInTab[sp.Tab].Add(sp);
				}
				if (this._filteredSpDict.ContainsKey(sp.Key))
				{
					for (int m = this._filteredSpList.Count - 1; m >= 0; m--)
					{
						if (this._filteredSpList[m] != null && this._filteredSpList[m].Key == sp.Key)
						{
							this._filteredSpList.RemoveAt(m);
							break;
						}
					}
					this._filteredSpDict.Remove(sp.Key);
				}
				break;
			}
			return isOnSale;
		}

		public void InitLocalData()
		{
			DictionaryView<uint, ResSpecSale>.Enumerator enumerator = GameDataMgr.specSaleDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResSpecSale> current = enumerator.Current;
				ResSpecSale value = current.get_Value();
				if (value != null)
				{
					CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)value.dwSpecSaleType, value.dwSpecSaleId, 0);
					if (cUseable != null && cUseable.m_baseID != 0u)
					{
						CMallFactoryShopController.ShopProduct sp = new CMallFactoryShopController.ShopProduct(value);
						if (!this._spDict.ContainsKey(sp.Key))
						{
							this._spDict.Add(sp.Key, sp);
							this._spList.Add(sp);
							if (this._availableTabs.FindIndex((uint v) => v == sp.Tab) < 0)
							{
								this._availableTabs.Add(sp.Tab);
							}
							if (!this._spListInTab.ContainsKey(sp.Tab))
							{
								this._spListInTab.Add(sp.Tab, new ListView<CMallFactoryShopController.ShopProduct>());
							}
							this._spListInTab[sp.Tab].Add(sp);
						}
					}
				}
			}
		}

		private void SortProducts()
		{
			DictionaryView<uint, ListView<CMallFactoryShopController.ShopProduct>>.Enumerator enumerator = this._spListInTab.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ListView<CMallFactoryShopController.ShopProduct>> current = enumerator.Current;
				current.get_Value().Sort((CMallFactoryShopController.ShopProduct l, CMallFactoryShopController.ShopProduct r) => r.RecommendIndex - l.RecommendIndex);
			}
			this._spList.Sort((CMallFactoryShopController.ShopProduct l, CMallFactoryShopController.ShopProduct r) => r.RecommendIndex - l.RecommendIndex);
		}

		public void Clear()
		{
			this._spDict.Clear();
			this._spList.Clear();
			this._filteredSpList.Clear();
			this._filteredSpDict.Clear();
			this._availableTabs.Clear();
			DictionaryView<uint, ListView<CMallFactoryShopController.ShopProduct>>.Enumerator enumerator = this._spListInTab.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ListView<CMallFactoryShopController.ShopProduct>> current = enumerator.Current;
				current.get_Value().Clear();
			}
			this._spListInTab.Clear();
		}

		public void UpdateInfo(CSPkg msg)
		{
			this.InitLocalData();
			SCPKG_CMD_SPECIAL_SALEINFO stSPecialSaleDetail = msg.stPkgData.stSPecialSaleDetail;
			if (stSPecialSaleDetail.stSpecialSaleInfo.bSpecSaleCnt == 0)
			{
				this.ResetLimitBuyDaily();
			}
			else
			{
				for (int i = 0; i < (int)stSPecialSaleDetail.stSpecialSaleInfo.bSpecSaleCnt; i++)
				{
					COMDT_SPECSALE cOMDT_SPECSALE = stSPecialSaleDetail.stSpecialSaleInfo.astSpecSaleDetail[i];
					if (this._spDict.ContainsKey(cOMDT_SPECSALE.dwId))
					{
						this._spDict[cOMDT_SPECSALE.dwId].BoughtCount = cOMDT_SPECSALE.dwNum;
					}
					if (this._filteredSpDict.ContainsKey(cOMDT_SPECSALE.dwId))
					{
						this._filteredSpDict[cOMDT_SPECSALE.dwId].BoughtCount = cOMDT_SPECSALE.dwNum;
					}
				}
				this.ManageShelf();
			}
		}

		public CMallFactoryShopController.ShopProduct GetProduct(uint key)
		{
			if (key == 0u)
			{
				return null;
			}
			if (this._spDict != null && this._spDict.ContainsKey(key))
			{
				return this._spDict[key];
			}
			return null;
		}

		public ListView<CMallFactoryShopController.ShopProduct> GetProducts()
		{
			return this._spList;
		}

		public ListView<CMallFactoryShopController.ShopProduct> GetProductsInCurTab()
		{
			if (!this._spListInTab.ContainsKey(this.CurTab))
			{
				return new ListView<CMallFactoryShopController.ShopProduct>();
			}
			return this._spListInTab[this.CurTab];
		}

		public int GetProductIndex(CMallFactoryShopController.ShopProduct product)
		{
			if (this._spList != null)
			{
				int count = this._spList.Count;
				for (int i = 0; i < count; i++)
				{
					if (this._spList[i].Key == product.Key)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int GetProductIndexInCurTab(CMallFactoryShopController.ShopProduct product)
		{
			if (this._spList != null)
			{
				if (!this._spListInTab.ContainsKey(this.CurTab))
				{
					return -1;
				}
				ListView<CMallFactoryShopController.ShopProduct> listView = this._spListInTab[this.CurTab];
				int count = listView.Count;
				for (int i = 0; i < count; i++)
				{
					if (listView[i].Key == product.Key)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/FactoryShop", "pnlFactoryShop", form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), "pnlFactoryShop");
			return !(x == null);
		}

		private void InitTab(CUIFormScript form)
		{
			int targetID = Singleton<CMallSystem>.GetInstance().TargetID;
			this.TargetID = targetID;
			int num = targetID;
			int num2;
			if (num != 0)
			{
				CMallFactoryShopController.ShopProduct product = this.GetProduct((uint)num);
				num2 = this._availableTabs.FindIndex((uint v) => product.Tab == v);
				if (num2 >= 0)
				{
					this.CurTab = product.Tab;
				}
			}
			num2 = this._availableTabs.FindIndex((uint v) => this.CurTab == v);
			if (num2 < 0)
			{
				if (this._availableTabs.get_Count() == 0)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Factory Shop Err", false, 1.5f, null, new object[0]);
					Singleton<CUIManager>.GetInstance().CloseForm(form);
					return;
				}
				this.CurTab = this._availableTabs.get_Item(0);
			}
			string[] array = new string[this._availableTabs.get_Count()];
			for (int i = 0; i < array.Length; i++)
			{
				if (this.CurTab == this._availableTabs.get_Item(i))
				{
					num2 = i;
				}
				switch (this._availableTabs.get_Item(i))
				{
				case 1u:
					array[i] = "热卖";
					break;
				case 2u:
					array[i] = "日常";
					break;
				case 3u:
					array[i] = "新手";
					break;
				}
			}
			CUICommonSystem.InitMenuPanel(Utility.FindChild(form.GetWidget(3), "pnlFactoryShop/MenuList"), array, num2, true);
		}

		public void Draw(CUIFormScript form)
		{
			this.InitTab(form);
		}

		public void Open(CUIFormScript form)
		{
			this._root = form.transform.Find("pnlBodyBg/pnlFactoryShop").gameObject;
			DebugHelper.Assert(this._root != null);
			if (this._root == null)
			{
				return;
			}
			this._root.CustomSetActive(true);
			if (this._widgetList != null)
			{
				for (int i = 0; i < this._widgetList.Count; i++)
				{
					this._widgetList[i].Clear();
				}
			}
			this._widgetList = new ListView<CMallFactoryShopController.ShopProductWidget>();
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(this._root, "List");
			ListView<CMallFactoryShopController.ShopProduct> productsInCurTab = this.GetProductsInCurTab();
			componetInChild.SetElementAmount(productsInCurTab.Count);
			for (int j = 0; j < productsInCurTab.Count; j++)
			{
				CUIListElementScript elemenet = componetInChild.GetElemenet(j);
				if (elemenet != null && elemenet.gameObject != null)
				{
					CMallFactoryShopController.ShopProductWidget shopProductWidget = new CMallFactoryShopController.ShopProductWidget(elemenet.gameObject, productsInCurTab[j]);
					shopProductWidget.HeighLightObj.CustomSetActive(false);
					this._widgetList.Add(shopProductWidget);
				}
			}
			int targetID = this.TargetID;
			if (targetID != 0)
			{
				CMallFactoryShopController.ShopProduct product = this.GetProduct((uint)targetID);
				if (product != null)
				{
					int productIndexInCurTab = this.GetProductIndexInCurTab(product);
					if (productIndexInCurTab >= 0 && productIndexInCurTab < productsInCurTab.Count)
					{
						componetInChild.MoveElementInScrollArea(productIndexInCurTab, true);
						if (productIndexInCurTab >= 0 && this._widgetList != null && productIndexInCurTab < this._widgetList.Count)
						{
							CMallFactoryShopController.ShopProductWidget shopProductWidget2 = this._widgetList[productIndexInCurTab];
							if (shopProductWidget2.HeighLightObj != null)
							{
								shopProductWidget2.HeighLightObj.CustomSetActive(true);
								CUIAnimationScript component = shopProductWidget2.HeighLightObj.GetComponent<CUIAnimationScript>();
								if (component != null)
								{
									component.PlayAnimation("HeighLight", true);
								}
							}
						}
					}
				}
			}
		}

		public void Close()
		{
			if (this._widgetList != null)
			{
				for (int i = 0; i < this._widgetList.Count; i++)
				{
					this._widgetList[i].Clear();
				}
			}
			this._widgetList = null;
			this._root = null;
			this.CurTab = 1u;
		}

		private void ResetLimitBuyDaily()
		{
			for (int i = 0; i < this._spList.Count; i++)
			{
				CMallFactoryShopController.ShopProduct shopProduct = this._spList[i];
				if (shopProduct.IsOnSale == 1 && shopProduct.LimitCount > 0u && shopProduct.LimitCycle == 1u)
				{
					shopProduct.BoughtCount = 0u;
				}
			}
			for (int j = 0; j < this._filteredSpList.Count; j++)
			{
				CMallFactoryShopController.ShopProduct shopProduct2 = this._filteredSpList[j];
				if (shopProduct2.IsOnSale == 1 && shopProduct2.LimitCount > 0u && shopProduct2.LimitCycle == 1u)
				{
					shopProduct2.BoughtCount = 0u;
				}
			}
			this.ManageShelf();
		}

		private void OnTabChange(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int selectedIndex = cUIListScript.GetSelectedIndex();
			if (selectedIndex < 0 || selectedIndex > this._availableTabs.get_Count())
			{
				DebugHelper.Assert(false, "index out of range");
				return;
			}
			this.CurTab = this._availableTabs.get_Item(selectedIndex);
			this.Open(uiEvent.m_srcFormScript);
		}

		private void OnClickBuy(CUIEvent uiEvent)
		{
			ListView<CMallFactoryShopController.ShopProduct> productsInCurTab = this.GetProductsInCurTab();
			CMallFactoryShopController.ShopProduct product = this.GetProduct(uiEvent.m_eventParams.commonUInt32Param1);
			if (product == null)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox("product not found.", false);
				return;
			}
			this.StartShopProduct(product);
		}

		public void StartShopProduct(CMallFactoryShopController.ShopProduct theSp)
		{
			if (theSp == null || !theSp.CanBuy())
			{
				return;
			}
			if (theSp.Type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || theSp.Type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				this.BuyShopProduct(theSp, 1u, false, null);
			}
			else
			{
				CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
				CUseable useableByBaseID = useableContainer.GetUseableByBaseID(theSp.Type, theSp.ID);
				uint num = 0u;
				if (theSp.LimitCount > 0u)
				{
					num = theSp.LimitCount - theSp.BoughtCount;
				}
				if (useableByBaseID != null)
				{
					uint num2 = (uint)(useableByBaseID.m_stackMax - useableByBaseID.m_stackCount);
					if (num2 < num || num == 0u)
					{
						num = num2;
					}
				}
				BuyPickDialog.Show(theSp.Type, theSp.ID, theSp.CoinType, theSp.RealDiscount, num, new BuyPickDialog.OnConfirmBuyDelegate(this.BuyShopProduct), theSp, null, null);
			}
		}

		private void OnClickConfirmBuy(CUIEvent uiEvent)
		{
			uint tag = (uint)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			if (tag == 0u)
			{
				return;
			}
			CMallFactoryShopController.ShopProduct shopProduct = null;
			if (this._spDict.TryGetValue(tag, out shopProduct))
			{
				this.RequestBuy(shopProduct, commonUInt32Param);
			}
		}

		public void RequestBuy(CMallFactoryShopController.ShopProduct shopProduct, uint count)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1153u);
			CSPKG_CMD_SPECSALEBUY stSpecSaleBuyReq = cSPkg.stPkgData.stSpecSaleBuyReq;
			stSpecSaleBuyReq.stSpecSaleBuyInfo.dwId = shopProduct.Key;
			stSpecSaleBuyReq.stSpecSaleBuyInfo.dwNum = count;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void BuyShopProduct(CMallFactoryShopController.ShopProduct shopProduct, uint count, bool needConfirm, CUIEvent uiEvent = null)
		{
			CUseable cUseable = CUseableManager.CreateUseable(shopProduct.Type, shopProduct.ID, 0);
			enPayType payType = CMallSystem.ResBuyTypeToPayType((int)shopProduct.CoinType);
			uint payValue;
			if (shopProduct != null && shopProduct.m_bChangeGiftPrice)
			{
				payValue = shopProduct.m_newGiftPrice * count;
			}
			else
			{
				payValue = shopProduct.ConvertWithRealDiscount(cUseable.GetBuyPrice(shopProduct.CoinType) * count);
			}
			if (uiEvent == null)
			{
				uiEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uiEvent.m_eventID = enUIEventID.Mall_Product_Confirm_Buy;
				uiEvent.m_eventParams.tag = (int)shopProduct.Key;
				uiEvent.m_eventParams.commonUInt32Param1 = count;
			}
			else
			{
				uiEvent.m_eventParams.commonUInt32Param1 = count;
			}
			CMallSystem.TryToPay(enPayPurpose.Buy, string.Format("{0}{1}", cUseable.m_name, (count > 1u) ? ("x" + count) : string.Empty), payType, payValue, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, needConfirm, true, false);
		}

		private void OnBuyReturn(CSPkg msg)
		{
			SCPKG_CMD_SPECSALEBUY stSpecSaleBuyRsp = msg.stPkgData.stSpecSaleBuyRsp;
			if (stSpecSaleBuyRsp.iErrCode == 0)
			{
				CSDT_SPECSALEBUYINFO stSpecSaleBuyInfo = stSpecSaleBuyRsp.stSpecSaleBuyInfo;
				if (this._spDict.ContainsKey(stSpecSaleBuyInfo.dwId))
				{
					CMallFactoryShopController.ShopProduct shopProduct = this._spDict[stSpecSaleBuyInfo.dwId];
					shopProduct.BoughtCount += stSpecSaleBuyInfo.dwNum;
					if (shopProduct.Type != COM_ITEM_TYPE.COM_OBJTYPE_HERO && (shopProduct.Type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP || !shopProduct.IsPropGift || !shopProduct.IsPropGiftUseImm))
					{
						CUseable cUseable = CUseableManager.CreateUseable(shopProduct.Type, shopProduct.ID, (int)stSpecSaleBuyInfo.dwNum);
						Singleton<CUIManager>.GetInstance().OpenAwardTip(new CUseable[]
						{
							cUseable
						}, Singleton<CTextManager>.GetInstance().GetText("Buy_Ok"), false, enUIEventID.None, false, false, "Form_Award");
					}
					this.FilterProduct(shopProduct);
					CMallSystem instance = Singleton<CMallSystem>.GetInstance();
					if (instance.m_MallForm != null && instance.m_IsMallFormOpen && instance.CurTab == CMallSystem.Tab.Factory_Shop)
					{
						this.Draw(instance.m_MallForm);
					}
					Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, shopProduct);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("{0}(错误码{1})", Singleton<CTextManager>.GetInstance().GetText("buySpecSaleFailed"), stSpecSaleBuyRsp.iErrCode), false);
			}
		}

		[MessageHandler(1152)]
		public static void OnInitialSpecSaleInfo(CSPkg msg)
		{
			Singleton<CMallFactoryShopController>.GetInstance().UpdateInfo(msg);
		}

		[MessageHandler(1154)]
		public static void OnBuySpecSaleRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CMallFactoryShopController>.GetInstance().OnBuyReturn(msg);
		}
	}
}
