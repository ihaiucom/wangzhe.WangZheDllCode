using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CNameChangeSystem : Singleton<CNameChangeSystem>
	{
		public enum enNameChangeFormWidget
		{
			NameInputField,
			DescText
		}

		public const string NameChangeFormPrefabPath = "UGUI/Form/Common/Form_NameChange.prefab";

		private int m_playerNameChangeCount;

		private int m_guildNameChangeCount;

		private RES_CHANGE_NAME_TYPE m_curChangeType = RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER;

		public void SetPlayerNameChangeCount(int playerNameChangeCount)
		{
			this.m_playerNameChangeCount = playerNameChangeCount;
		}

		public void SetGuildNameChangeCount(int guildNameChangeCount)
		{
			this.m_guildNameChangeCount = guildNameChangeCount;
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_OpenPlayerNameChangeForm, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerNameChangeForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_OpenGuildNameChangeForm, new CUIEventManager.OnUIEventHandler(this.OnOpenGuildNameChangeForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_ChangeName, new CUIEventManager.OnUIEventHandler(this.OnChangeName));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_GuideToMall, new CUIEventManager.OnUIEventHandler(this.OnGuideToMall));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_BuyNameChangeCardConfirm, new CUIEventManager.OnUIEventHandler(this.OnBuyNameChgCardConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
		}

		private void OnOpenPlayerNameChangeForm(CUIEvent uiEvent)
		{
			uint timeToCD = this.GetTimeToCD(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER);
			if (timeToCD == 0u)
			{
				this.OpenNameChangeForm(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorInCD", true, 1.5f, null, new object[]
				{
					timeToCD
				});
			}
		}

		private void OnOpenGuildNameChangeForm(CUIEvent uiEvent)
		{
			uint timeToCD = this.GetTimeToCD(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD);
			if (timeToCD == 0u)
			{
				this.OpenNameChangeForm(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorInCD", true, 1.5f, null, new object[]
				{
					timeToCD
				});
			}
		}

		private uint GetTimeToCD(RES_CHANGE_NAME_TYPE changeType)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				DateTime d = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				DateTime d2;
				uint globeValue;
				if (changeType != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER)
				{
					if (changeType != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
					{
						return 0u;
					}
					d2 = Utility.ToUtcTime2Local((long)((ulong)masterRoleInfo.chgNameCD.dwLastChgGuildNameTime));
					globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CHG_GUILD_NAME_CD);
				}
				else
				{
					d2 = Utility.ToUtcTime2Local((long)((ulong)masterRoleInfo.chgNameCD.dwLastChgPlayerNameTime));
					globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CHG_PLAYER_NAME_CD);
				}
				uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(10u).dwConfValue;
				int num = Utility.Hours2Second((int)(dwConfValue / 100u)) + Utility.Minutes2Seconds((int)(dwConfValue % 100u));
				d = d.AddSeconds((double)(-(double)num)).Date;
				d2 = d2.AddSeconds((double)(-(double)num)).Date;
				int days = (d - d2).Days;
				return (uint)(((long)days < (long)((ulong)globeValue)) ? ((ulong)globeValue - (ulong)((long)days)) : 0uL);
			}
			return 0u;
		}

		private void OpenNameChangeForm(RES_CHANGE_NAME_TYPE changeType)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_NameChange.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			InputField component = cUIFormScript.GetWidget(0).GetComponent<InputField>();
			Text component2 = cUIFormScript.GetWidget(1).GetComponent<Text>();
			if (changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER || changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
			{
				bool flag = changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER;
				component.placeholder.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText((!flag) ? "NameChange_GuildInputNewName" : "NameChange_InputNewName");
				if (!flag)
				{
					component.characterLimit = 7;
				}
				component2.text = Singleton<CTextManager>.GetInstance().GetText((!flag) ? "NameChange_GuildCostDesc" : "NameChange_CostDesc");
			}
			else if (changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_TRANSDATA_PLAYER)
			{
				component.placeholder.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("NameChange_InputNewName");
				component2.text = Singleton<CTextManager>.GetInstance().GetText("NameChange_TransData_PlayerDesc");
				cUIFormScript.transform.FindChild("Panel/title/ClostBtn").gameObject.CustomSetActive(false);
			}
			this.m_curChangeType = changeType;
		}

		private void OnChangeName(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			InputField component = srcFormScript.GetWidget(0).GetComponent<InputField>();
			string text = CUIUtility.RemoveEmoji(component.text).Trim();
			if (this.m_curChangeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER)
			{
				switch (Utility.CheckRoleName(text))
				{
				case Utility.NameResult.Vaild:
					if (!this.CheckNameChangeCardCount())
					{
						return;
					}
					this.SendChangeNameReq(text);
					break;
				case Utility.NameResult.Null:
					Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Null", true, 1.5f, null, new object[0]);
					break;
				case Utility.NameResult.OutOfLength:
					Singleton<CUIManager>.GetInstance().OpenTips("RoleRegister_NameOutOfLength", true, 1.5f, null, new object[0]);
					break;
				case Utility.NameResult.InVaildChar:
					Singleton<CUIManager>.GetInstance().OpenTips("NameChange_InvalidName", true, 1.5f, null, new object[0]);
					break;
				}
			}
			else if (this.m_curChangeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
			{
				if (string.IsNullOrEmpty(text))
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Empty", true, 1.5f, null, new object[0]);
				}
				else if (!Utility.IsValidText(text))
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Invalid", true, 1.5f, null, new object[0]);
				}
				else
				{
					if (!this.CheckNameChangeCardCount())
					{
						return;
					}
					this.SendChangeNameReq(text);
				}
			}
			else if (this.m_curChangeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_TRANSDATA_PLAYER)
			{
				switch (Utility.CheckRoleName(text))
				{
				case Utility.NameResult.Vaild:
					this.SendTransDataRenameReq(text);
					break;
				case Utility.NameResult.Null:
					Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Null", true, 1.5f, null, new object[0]);
					break;
				case Utility.NameResult.OutOfLength:
					Singleton<CUIManager>.GetInstance().OpenTips("RoleRegister_NameOutOfLength", true, 1.5f, null, new object[0]);
					break;
				case Utility.NameResult.InVaildChar:
					Singleton<CUIManager>.GetInstance().OpenTips("NameChange_InvalidName", true, 1.5f, null, new object[0]);
					break;
				}
			}
		}

		private void OnGuideToMall(CUIEvent uiEvent)
		{
			uint nameChangeCardShopProductKey = this.GetNameChangeCardShopProductKey(this.m_curChangeType);
			CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(nameChangeCardShopProductKey);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.NameChange_BuyNameChangeCardConfirm;
			uIEvent.m_eventParams.commonUInt64Param1 = (ulong)nameChangeCardShopProductKey;
			Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct(product, (uint)uiEvent.m_eventParams.tag, false, uIEvent);
		}

		private void OnBuyNameChgCardConfirm(CUIEvent uiEvent)
		{
			uint key = (uint)uiEvent.m_eventParams.commonUInt64Param1;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(key);
			if (product != null)
			{
				Singleton<CMallFactoryShopController>.GetInstance().RequestBuy(product, commonUInt32Param);
			}
		}

		private void OnOpenBuyDianQuanPanel(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_NameChange.prefab");
		}

		private uint GetNameChangeCardShopProductKey(RES_CHANGE_NAME_TYPE chgNameType)
		{
			uint result = 0u;
			if (chgNameType != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER)
			{
				if (chgNameType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
				{
					result = GameDataMgr.globalInfoDatabin.GetDataByKey(202u).dwConfValue;
				}
			}
			else
			{
				result = GameDataMgr.globalInfoDatabin.GetDataByKey(201u).dwConfValue;
			}
			return result;
		}

		private bool CheckNameChangeCardCount()
		{
			int nameChangeHaveItemCount = this.GetNameChangeHaveItemCount(this.m_curChangeType);
			int nameChangeCostItemCount = this.GetNameChangeCostItemCount(this.m_curChangeType);
			if (nameChangeHaveItemCount < nameChangeCostItemCount)
			{
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					string text = Singleton<CTextManager>.GetInstance().GetText("NameChange_ErrorItemNotEnough");
					Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
				}
				else
				{
					uint nameChangeCardShopProductKey = this.GetNameChangeCardShopProductKey(this.m_curChangeType);
					CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(nameChangeCardShopProductKey);
					if (product == null)
					{
						DebugHelper.Assert(false, "错误的特卖商品ID");
						return false;
					}
					ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(product.ID);
					if (dataByKey == null)
					{
						DebugHelper.Assert(false, "错误的商品ID");
						return false;
					}
					CUseable cUseable = CUseableManager.CreateUseable(product.Type, 0uL, product.ID, (int)product.LimitCount, 0);
					uint num = product.ConvertWithRealDiscount(cUseable.GetBuyPrice(product.CoinType));
					int num2 = nameChangeCostItemCount - nameChangeHaveItemCount;
					string text2 = Singleton<CTextManager>.GetInstance().GetText("NameChange_GuideToMall", new string[]
					{
						num2.ToString(),
						dataByKey.szName,
						((long)num2 * (long)((ulong)num)).ToString()
					});
					stUIEventParams par = default(stUIEventParams);
					par.tag = num2;
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text2, enUIEventID.NameChange_GuideToMall, enUIEventID.None, par, false);
				}
				return false;
			}
			return true;
		}

		private int GetNameChangeHaveItemCount(RES_CHANGE_NAME_TYPE changeType)
		{
			int num = 0;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (useableContainer != null)
			{
				int curUseableCount = useableContainer.GetCurUseableCount();
				for (int i = 0; i < curUseableCount; i++)
				{
					CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
					if (useableByIndex != null && useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						if (changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER && CItem.IsPlayerNameChangeCard(useableByIndex.m_baseID))
						{
							num += useableByIndex.m_stackCount;
						}
						else if (changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD && CItem.IsGuildNameChangeCard(useableByIndex.m_baseID))
						{
							num += useableByIndex.m_stackCount;
						}
					}
				}
			}
			return num;
		}

		private int GetNameChangeCostItemCount(RES_CHANGE_NAME_TYPE changeType)
		{
			int nameChangeCount = (changeType != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER) ? this.m_guildNameChangeCount : this.m_playerNameChangeCount;
			ResChangeName resChangeName = GameDataMgr.changeNameDatabin.FindIf((ResChangeName x) => x.bType == (byte)changeType && (ulong)x.dwChgCntLow <= (ulong)((long)nameChangeCount) && (long)nameChangeCount <= (long)((ulong)x.dwChgCntHigh));
			if (resChangeName != null)
			{
				return (int)resChangeName.dwCostItemCnt;
			}
			return 0;
		}

		private void SendChangeNameReq(string name)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(3200u);
			cSPkg.stPkgData.stChangeNameReq.bType = (byte)this.m_curChangeType;
			StringHelper.StringToUTF8Bytes(name, ref cSPkg.stPkgData.stChangeNameReq.szNewName);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void SendTransDataRenameReq(string name)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5236u);
			StringHelper.StringToUTF8Bytes(name, ref cSPkg.stPkgData.stTransDataRenameReq.szNewName);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void ShowErrorCode(CS_ERRORCODE_DEF result)
		{
			if (result == CS_ERRORCODE_DEF.CS_ERR_REGISTER_NAME_DUP_FAIL)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("RoleRegister_NameExists", true, 1.5f, null, new object[0]);
			}
			else if (result == CS_ERRORCODE_DEF.CS_ERR_REGISTERNAME)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorInvalid", true, 1.5f, null, new object[0]);
			}
			else if (result == CS_ERRORCODE_DEF.CS_ERR_CHANGE_NAME_TYPE_INVALID)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorTypeInvalid", true, 1.5f, null, new object[0]);
			}
			else if (result == CS_ERRORCODE_DEF.CS_ERR_CHANGE_NAME_ITEM_NOT_ENOUGH)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorItemNotEnough", true, 1.5f, null, new object[0]);
			}
			else if (result == CS_ERRORCODE_DEF.CS_ERR_DB)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorDB", true, 1.5f, null, new object[0]);
			}
			else if (result == CS_ERRORCODE_DEF.CS_ERR_CHANGE_GUILD_NAME_AUTHORITY)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorAuthority", true, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(3201)]
		public static void ReceiveChangeNameRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CHANGE_NAME_RSP stChangeNameRsp = msg.stPkgData.stChangeNameRsp;
			CS_ERRORCODE_DEF iResult = (CS_ERRORCODE_DEF)stChangeNameRsp.iResult;
			if (iResult == CS_ERRORCODE_DEF.CS_SUCCESS)
			{
				Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_NameChange.prefab");
				string text = StringHelper.UTF8BytesToString(ref stChangeNameRsp.szNewName);
				if (stChangeNameRsp.bType == 1)
				{
					Singleton<CNameChangeSystem>.GetInstance().m_playerNameChangeCount = (int)stChangeNameRsp.dwChangeCnt;
					if (!string.IsNullOrEmpty(text))
					{
						Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name = text;
						Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NAMECHANGE_PLAYER_NAME_CHANGE);
						Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Success", true, 1.5f, null, new object[0]);
					}
					else
					{
						DebugHelper.Assert(false, "CNameChangeSystem.ReceiveChangeNameRsp(): error, empty player new name!!!");
					}
				}
				else if (stChangeNameRsp.bType == 2)
				{
					Singleton<CNameChangeSystem>.GetInstance().m_guildNameChangeCount = (int)stChangeNameRsp.dwChangeCnt;
					if (!string.IsNullOrEmpty(text))
					{
						Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name = text;
						Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NAMECHANGE_GUILD_NAME_CHANGE);
						Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Success", true, 1.5f, null, new object[0]);
					}
					else
					{
						DebugHelper.Assert(false, "CNameChangeSystem.ReceiveChangeNameRsp(): error, empty guild new name!!!");
					}
				}
			}
			else
			{
				Singleton<CNameChangeSystem>.GetInstance().ShowErrorCode(iResult);
			}
		}

		[MessageHandler(3202)]
		public static void ReceiveGuildNameChangeNtf(CSPkg msg)
		{
			Singleton<CNameChangeSystem>.GetInstance().m_guildNameChangeCount = (int)msg.stPkgData.stGuildNameChgNtf.dwChgNameCnt;
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name = StringHelper.UTF8BytesToString(ref msg.stPkgData.stGuildNameChgNtf.szNewName);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NAMECHANGE_GUILD_NAME_CHANGE);
		}

		[MessageHandler(5235)]
		public static void ReceiveTransDataRenameNtf(CSPkg msg)
		{
			Singleton<CNameChangeSystem>.GetInstance().OpenNameChangeForm(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_TRANSDATA_PLAYER);
		}

		[MessageHandler(5237)]
		public static void ReceiveTransDataRenameRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_TRANSDATA_RENAME_RES stTransDataRenameRes = msg.stPkgData.stTransDataRenameRes;
			CS_ERRORCODE_DEF dwResult = (CS_ERRORCODE_DEF)stTransDataRenameRes.dwResult;
			if (dwResult == CS_ERRORCODE_DEF.CS_SUCCESS)
			{
				Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_NameChange.prefab");
				Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Success", true, 1.5f, null, new object[0]);
			}
			else
			{
				Singleton<CNameChangeSystem>.GetInstance().ShowErrorCode(dwResult);
			}
		}

		[MessageHandler(5502)]
		public static void ReceiveChgNameCD(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.chgNameCD = msg.stPkgData.stChgNameCDNtf.stChgNameCD;
			}
		}
	}
}
