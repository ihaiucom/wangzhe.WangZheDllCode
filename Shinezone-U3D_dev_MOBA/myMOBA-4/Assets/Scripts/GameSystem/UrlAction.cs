using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class UrlAction
	{
		public enum Action
		{
			none,
			openUrl,
			openForm,
			buy,
			openMatchUrl,
			COUNT
		}

		private static char[] MultiSpliter = new char[]
		{
			'\n'
		};

		private static char[] InnerSpliter = new char[]
		{
			'#',
			'&'
		};

		private static char[] ParamSpliter = new char[]
		{
			'='
		};

		public string target;

		public UrlAction.Action action;

		public string url;

		public RES_GAME_ENTRANCE_TYPE form;

		public COM_ITEM_TYPE prodType;

		public uint prodID;

		public int prodSpec;

		public ulong overTime;

		public int showTime = 3000;

		public bool Execute()
		{
			switch (this.action)
			{
			case UrlAction.Action.openUrl:
				CUICommonSystem.OpenUrl(this.url, false, 0);
				break;
			case UrlAction.Action.openForm:
				CUICommonSystem.JumpForm(this.form, 0, 0, null);
				break;
			case UrlAction.Action.buy:
				switch (this.prodType)
				{
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
					if (GameDataMgr.specSaleDict.ContainsKey(this.prodID))
					{
						CMallFactoryShopController.ShopProduct theSp = new CMallFactoryShopController.ShopProduct(GameDataMgr.specSaleDict[this.prodID]);
						Singleton<CMallFactoryShopController>.GetInstance().StartShopProduct(theSp);
						goto IL_236;
					}
					return false;
				case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo == null)
					{
						Singleton<CUIManager>.GetInstance().OpenTips("internalError", false, 1.5f, null, new object[0]);
						return false;
					}
					if (masterRoleInfo.IsHaveHero(this.prodID, false))
					{
						Singleton<CUIManager>.GetInstance().OpenTips("hasOwnHero", true, 1.5f, null, new object[0]);
						return false;
					}
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.HeroInfo_OpenBuyHeroForm;
					cUIEvent.m_eventParams.heroId = this.prodID;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
					goto IL_236;
				}
				case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
				{
					CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo2 == null)
					{
						Singleton<CUIManager>.GetInstance().OpenTips("internalError", false, 1.5f, null, new object[0]);
						return false;
					}
					if (!masterRoleInfo2.IsHaveHero(this.prodID, false))
					{
						Singleton<CUIManager>.GetInstance().OpenTips("Skin_NeedToBuyAHero", true, 1.5f, null, new object[0]);
						return false;
					}
					if (masterRoleInfo2.IsHaveHeroSkin(this.prodID, (uint)this.prodSpec, false))
					{
						Singleton<CUIManager>.GetInstance().OpenTips("hasOwnHeroSkin", true, 1.5f, null, new object[0]);
						return false;
					}
					CUIEvent cUIEvent2 = new CUIEvent();
					cUIEvent2.m_eventID = enUIEventID.HeroSkin_OpenBuySkinForm;
					cUIEvent2.m_eventParams.heroSkinParam.heroId = this.prodID;
					cUIEvent2.m_eventParams.heroSkinParam.skinId = (uint)this.prodSpec;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
					goto IL_236;
				}
				}
				return false;
				IL_236:
				break;
			case UrlAction.Action.openMatchUrl:
				CUICommonSystem.OpenUrl(this.target, true, 0);
				break;
			default:
				return false;
			}
			return true;
		}

		public static ListView<UrlAction> ParseFromText(string text, char[] spliter = null)
		{
			ListView<UrlAction> listView = new ListView<UrlAction>();
			if (spliter == null)
			{
				spliter = UrlAction.MultiSpliter;
			}
			try
			{
				string[] array = text.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(UrlAction.InnerSpliter, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length > 0)
					{
						DictionaryView<string, string> dictionaryView = new DictionaryView<string, string>();
						for (int j = 1; j < array2.Length; j++)
						{
							string[] array3 = array2[j].Split(UrlAction.ParamSpliter);
							if (array3 != null && array3.Length == 2)
							{
								dictionaryView.Add(array3[0], array3[1]);
							}
						}
						UrlAction urlAction = new UrlAction();
						urlAction.target = array2[0];
						urlAction.action = UrlAction.Action.none;
						if (dictionaryView.ContainsKey("action"))
						{
							string text2 = dictionaryView["action"];
							string text3 = text2;
							switch (text3)
							{
							case "openUrl":
								urlAction.action = UrlAction.Action.openUrl;
								urlAction.url = dictionaryView["url"];
								break;
							case "openForm":
								urlAction.action = UrlAction.Action.openForm;
								urlAction.form = (RES_GAME_ENTRANCE_TYPE)int.Parse(dictionaryView["form"]);
								break;
							case "buy":
								urlAction.action = UrlAction.Action.buy;
								urlAction.prodType = (COM_ITEM_TYPE)int.Parse(dictionaryView["prodType"]);
								urlAction.prodID = uint.Parse(dictionaryView["prodID"]);
								if (dictionaryView.ContainsKey("prodSpec"))
								{
									int.TryParse(dictionaryView["prodSpec"], out urlAction.prodSpec);
								}
								break;
							case "openMatchUrl":
							{
								int num2 = text.IndexOf("#action=openMatchUrl");
								if (num2 > 0)
								{
									urlAction.target = text.Substring(0, num2);
								}
								urlAction.action = UrlAction.Action.openMatchUrl;
								break;
							}
							}
						}
						if (dictionaryView.ContainsKey("overTime"))
						{
							ulong.TryParse(dictionaryView["overTime"], out urlAction.overTime);
						}
						if (dictionaryView.ContainsKey("showTime"))
						{
							int.TryParse(dictionaryView["showTime"], out urlAction.showTime);
						}
						listView.Add(urlAction);
					}
				}
			}
			catch (Exception var_10_29F)
			{
			}
			return listView;
		}
	}
}
