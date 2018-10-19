using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class InBattleMsgMgr : Singleton<InBattleMsgMgr>
	{
		public DictionaryView<string, ListView<TabElement>> tabElements = new DictionaryView<string, ListView<TabElement>>();

		public List<string> title_list = new List<string>();

		private DictionaryView<uint, DictionaryView<uint, ResInBatMsgHeroActCfg>> heroActData = new DictionaryView<uint, DictionaryView<uint, ResInBatMsgHeroActCfg>>();

		public ListView<ResInBatMsgCfg> flagConfigData = new ListLinqView<ResInBatMsgCfg>();

		public InBattleShortcut m_shortcutChat;

		public InBattleInputChat m_InputChat;

		public InBattleShortcutMenu m_customMenu = new InBattleShortcutMenu();

		private CUIFormScript m_formScript;

		public ListView<TabElement> inbatEntList = new ListView<TabElement>();

		public ListView<TabElement> menuEntList = new ListView<TabElement>();

		public ListView<TabElement> lastMenuEntList = new ListView<TabElement>();

		public byte totalCount;

		public bool IsUseDefault
		{
			get;
			private set;
		}

		public void Clear()
		{
			this.UnRegInBattleEvent();
			this.inbatEntList.Clear();
			if (this.m_shortcutChat != null)
			{
				this.m_shortcutChat.Clear();
			}
			this.m_shortcutChat = null;
			if (this.m_InputChat != null)
			{
				this.m_InputChat.Clear();
			}
			this.m_InputChat = null;
			this.m_formScript = null;
		}

		public void ClearData()
		{
			this.inbatEntList.Clear();
			this.menuEntList.Clear();
			this.lastMenuEntList.Clear();
		}

		public void UseDefaultShortcut()
		{
			this.menuEntList.Clear();
			this.lastMenuEntList.Clear();
			for (int i = 0; i < (int)this.totalCount; i++)
			{
				ResShortcutDefault dataByKey = GameDataMgr.inBattleDefaultDatabin.GetDataByKey((long)(i + 1));
				DebugHelper.Assert(dataByKey != null, "---shortcut id:" + (i + 1) + ", 找不到对应配置数据, jason 检查下");
				if (dataByKey != null)
				{
					ResInBatMsgCfg dataByKey2 = GameDataMgr.inBattleMsgDatabin.GetDataByKey(dataByKey.dwConfigID);
					DebugHelper.Assert(dataByKey2 != null, "---shortcut dwConfigID:" + dataByKey.dwConfigID + ", 找不到对应配置数据, jason 检查下");
					if (dataByKey2 != null)
					{
						this.menuEntList.Add(new TabElement(dataByKey2.dwID, dataByKey2.szContent));
						this.lastMenuEntList.Add(new TabElement(dataByKey2.dwID, dataByKey2.szContent));
					}
				}
			}
		}

		public void SyncData(ListView<TabElement> src, ListView<TabElement> dest)
		{
			if (dest != null)
			{
				dest.Clear();
			}
			for (int i = 0; i < src.Count; i++)
			{
				TabElement tabElement = src[i];
				if (tabElement != null)
				{
					dest.Add(tabElement.Clone());
				}
			}
		}

		public void Print(ListView<TabElement> list, string name)
		{
			string text = string.Concat(new object[]
			{
				"---shortCut list:",
				name,
				",count:",
				list.Count,
				", "
			});
			for (int i = 0; i < list.Count; i++)
			{
				TabElement tabElement = list[i];
				if (tabElement != null)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"  ,i:",
						i,
						",id:",
						tabElement.cfgId
					});
				}
			}
			Debug.Log(text);
		}

		public void BuildInBatEnt()
		{
			this.inbatEntList.Clear();
			for (int i = 0; i < this.menuEntList.Count; i++)
			{
				TabElement tabElement = this.menuEntList[i];
				if (tabElement != null && tabElement.cfgId != 0u)
				{
					this.inbatEntList.Add(tabElement);
				}
			}
		}

		public TabElement GeTabElement(int tabIndex, int list_index)
		{
			if (tabIndex < 0 || tabIndex >= Singleton<InBattleMsgMgr>.instance.title_list.Count)
			{
				return null;
			}
			ListView<TabElement> listView = null;
			string key = Singleton<InBattleMsgMgr>.instance.title_list[tabIndex];
			Singleton<InBattleMsgMgr>.instance.tabElements.TryGetValue(key, out listView);
			if (listView == null)
			{
				return null;
			}
			if (list_index < 0 || list_index >= listView.Count)
			{
				return null;
			}
			return listView[list_index];
		}

		public void ClearMenuItem(int index)
		{
			if (index >= 0 && index < this.menuEntList.Count)
			{
				this.menuEntList[index] = null;
			}
		}

		public void SetMenuItem(int index, uint configID)
		{
			if (index >= 0 && index < this.menuEntList.Count)
			{
				TabElement tabElement = this.menuEntList[index];
				if (tabElement != null)
				{
					tabElement.cfgId = configID;
				}
			}
		}

		public void ParseServerData(COMDT_SELFDEFINE_CHATINFO chatInfo)
		{
			if (chatInfo == null)
			{
				return;
			}
			bool flag = false;
			this.totalCount = chatInfo.bMsgCnt;
			for (int i = 0; i < (int)chatInfo.bMsgCnt; i++)
			{
				COMDT_SELFDEFINE_DETAIL_CHATINFO cOMDT_SELFDEFINE_DETAIL_CHATINFO = chatInfo.astChatMsg[i];
				if (cOMDT_SELFDEFINE_DETAIL_CHATINFO.bChatType == 4)
				{
					flag = true;
					break;
				}
				if (cOMDT_SELFDEFINE_DETAIL_CHATINFO.bChatType == 1 && cOMDT_SELFDEFINE_DETAIL_CHATINFO.stChatInfo.stSignalID.dwTextID > 0u)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.UseDefaultShortcut();
				return;
			}
			for (int j = 0; j < (int)chatInfo.bMsgCnt; j++)
			{
				COMDT_SELFDEFINE_DETAIL_CHATINFO cOMDT_SELFDEFINE_DETAIL_CHATINFO2 = chatInfo.astChatMsg[j];
				if (cOMDT_SELFDEFINE_DETAIL_CHATINFO2.bChatType == 1)
				{
					uint dwTextID = cOMDT_SELFDEFINE_DETAIL_CHATINFO2.stChatInfo.stSignalID.dwTextID;
					TabElement tabElement = new TabElement(dwTextID, string.Empty);
					tabElement.cfgId = dwTextID;
					if (dwTextID > 0u)
					{
						ResInBatMsgCfg cfgData = this.GetCfgData(tabElement.cfgId);
						DebugHelper.Assert(cfgData != null, "custom shortcut ParseServerData cfgdata is null, cfgID:" + tabElement.cfgId);
						if (cfgData != null)
						{
							tabElement.configContent = cfgData.szContent;
						}
					}
					this.menuEntList.Add(tabElement);
				}
				else if (cOMDT_SELFDEFINE_DETAIL_CHATINFO2.bChatType == 1)
				{
					string value = StringHelper.BytesToString(cOMDT_SELFDEFINE_DETAIL_CHATINFO2.stChatInfo.stSelfDefineStr.szContent);
					if (!string.IsNullOrEmpty(value))
					{
						TabElement item = new TabElement(string.Empty);
						this.menuEntList.Add(item);
					}
				}
			}
			this.SyncData(this.menuEntList, this.lastMenuEntList);
			this.Print(this.menuEntList, "服务器解析出来的自定义数据");
		}

		public void SetMenuItem(int index, string selfDef)
		{
		}

		private TabElement GetConfigMenuItem()
		{
			return null;
		}

		public void InitView(GameObject cdButton, CUIFormScript formScript)
		{
			if (formScript == null)
			{
				return;
			}
			this.m_formScript = formScript;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			if (this.IsMultiGame())
			{
				this.m_shortcutChat = new InBattleShortcut();
				this.m_shortcutChat.CacheForm(formScript, true, false);
				this.m_InputChat = new InBattleInputChat();
				this.m_InputChat.Init(formScript);
			}
			else if (curLvelContext.IsGameTypeAdventure() || curLvelContext.IsGameTypeBurning() || curLvelContext.IsGameTypeArena() || curLvelContext.IsGameTypeComBat())
			{
				this.m_shortcutChat = new InBattleShortcut();
				this.m_shortcutChat.CacheForm(formScript, false, true);
			}
			else if (cdButton != null)
			{
				cdButton.CustomSetActive(false);
			}
		}

		public bool IsMultiGame()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			return curLvelContext != null && (curLvelContext.m_isWarmBattle || Singleton<LobbyLogic>.instance.inMultiGame);
		}

		public bool IsEnableInputChat()
		{
			return Singleton<CChatController>.instance.model.bEnableInBattleInputChat && this.IsMultiGame() && GameSettings.InBattleInputChatEnable == 1;
		}

		public void Update()
		{
			if (this.m_shortcutChat != null)
			{
				this.m_shortcutChat.Update();
			}
			if (this.m_InputChat != null)
			{
				this.m_InputChat.Update();
			}
		}

		public void HideView()
		{
			if (this.m_shortcutChat != null)
			{
				this.m_shortcutChat.Show(false);
			}
		}

		public void RegInBattleEvent()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_OpenForm));
		}

		public void UnRegInBattleEvent()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_OpenForm));
		}

		public void ParseCfgData()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_InBatMenu_OpenForm));
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("InBat_Bubble_CDTime"), out InBattleShortcut.InBat_Bubble_CDTime))
			{
				DebugHelper.Assert(false, "---InBatMsg 教练你配的 InBat_Bubble_CDTime 好像不是整数哦， check out");
			}
			ListView<TabElement> listView = null;
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.inBattleMsgDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				listView = null;
				KeyValuePair<long, object> current = enumerator.Current;
				ResInBatMsgCfg resInBatMsgCfg = (ResInBatMsgCfg)current.Value;
				if (resInBatMsgCfg != null)
				{
					string szChannelTitle = resInBatMsgCfg.szChannelTitle;
					this.tabElements.TryGetValue(szChannelTitle, out listView);
					if (listView == null)
					{
						listView = new ListView<TabElement>();
						this.tabElements.Add(szChannelTitle, listView);
						this.title_list.Add(szChannelTitle);
					}
					TabElement tabElement = new TabElement(resInBatMsgCfg.dwID, resInBatMsgCfg.szContent);
					tabElement.camp = resInBatMsgCfg.bCampVisible;
					listView.Add(tabElement);
				}
			}
			Dictionary<long, object>.Enumerator enumerator2 = GameDataMgr.inBattleHeroActDatabin.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<long, object> current2 = enumerator2.Current;
				ResInBatMsgHeroActCfg resInBatMsgHeroActCfg = (ResInBatMsgHeroActCfg)current2.Value;
				if (resInBatMsgHeroActCfg != null)
				{
					DictionaryView<uint, ResInBatMsgHeroActCfg> dictionaryView = null;
					this.heroActData.TryGetValue(resInBatMsgHeroActCfg.dwHeroID, out dictionaryView);
					if (dictionaryView == null)
					{
						dictionaryView = new DictionaryView<uint, ResInBatMsgHeroActCfg>();
						this.heroActData.Add(resInBatMsgHeroActCfg.dwHeroID, dictionaryView);
					}
					if (!dictionaryView.ContainsKey(resInBatMsgHeroActCfg.dwActionID))
					{
						dictionaryView.Add(resInBatMsgHeroActCfg.dwActionID, resInBatMsgHeroActCfg);
					}
				}
			}
			GameDataMgr.inBattleHeroActDatabin.Unload();
			Dictionary<long, object>.Enumerator enumerator3 = GameDataMgr.inBattleDefaultDatabin.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				KeyValuePair<long, object> current3 = enumerator3.Current;
				ResShortcutDefault resShortcutDefault = (ResShortcutDefault)current3.Value;
				if (resShortcutDefault != null)
				{
					ResInBatMsgCfg dataByKey = GameDataMgr.inBattleMsgDatabin.GetDataByKey(resShortcutDefault.dwConfigID);
					DebugHelper.Assert(dataByKey != null, "---jason 检查下 局内交流配置表中的默认配置sheet, 配置id:" + resShortcutDefault.dwConfigID);
				}
			}
		}

		public void On_InBatMenu_OpenForm(CUIEvent uievent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			if (this.m_customMenu != null)
			{
				this.m_customMenu.OpenForm();
			}
			Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(3u, null, false);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_BattleChatSetBtn);
		}

		public ResInBatMsgCfg GetCfgData(uint id)
		{
			return GameDataMgr.inBattleMsgDatabin.GetDataByKey(id);
		}

		public void Handle_InBattleMsg_Ntf(COMDT_CHAT_MSG_INBATTLE obj)
		{
			if (obj == null)
			{
				return;
			}
			ulong ullUid = obj.stFrom.ullUid;
			uint dwAcntHeroID = obj.stFrom.dwAcntHeroID;
			if (obj.bChatType == 1)
			{
				uint dwTextID = obj.stChatInfo.stSignalID.dwTextID;
				if (this.m_shortcutChat != null)
				{
					this.m_shortcutChat.InnerHandle_InBat_PreConfigMsg((COM_INBATTLE_CHAT_TYPE)obj.bChatType, dwAcntHeroID, dwTextID, ullUid);
				}
			}
			else if (obj.bChatType == 2)
			{
				uint dwTextID = obj.stChatInfo.stBubbleID.dwTextID;
				if (this.m_shortcutChat != null)
				{
					this.m_shortcutChat.InnerHandle_InBat_PreConfigMsg((COM_INBATTLE_CHAT_TYPE)obj.bChatType, dwAcntHeroID, dwTextID, ullUid);
				}
			}
			else if (obj.bChatType == 3)
			{
				string playerName = StringHelper.BytesToString(obj.stFrom.szName);
				string content = StringHelper.BytesToString_FindFristZero(obj.stChatInfo.stContentStr.szContent);
				byte bCampLimit = obj.stChatInfo.stContentStr.bCampLimit;
				this.InnerHandle_InBat_InputChat(ullUid, playerName, content, bCampLimit);
			}
			else
			{
				DebugHelper.Assert(false, string.Format("Handle_InBattleMsg_Ntf chatType:{0} beyond scope", obj.bChatType));
			}
		}

		public bool ShouldBeThroughNet(SLevelContext levelContent)
		{
			return levelContent != null && (!levelContent.m_isWarmBattle || !levelContent.IsGameTypeComBat());
		}

		public void InnerHandle_InBat_InputChat(ulong ullUid, string playerName, string content, byte camp)
		{
			if (this.m_InputChat != null)
			{
				InBattleInputChat.InBatChatEntity ent = this.m_InputChat.ConstructEnt(ullUid, playerName, content, camp);
				this.m_InputChat.Add(ent);
			}
		}

		public void ServerDisableInputChat()
		{
			if (this.m_InputChat != null)
			{
				this.m_InputChat.ServerDisableInputChat();
			}
		}

		public bool IsAllChannel_CD_Valid()
		{
			return true;
		}

		public bool IsChannel_CD_Valid(int channel_id)
		{
			return true;
		}

		public ResInBatMsgHeroActCfg GetHeroActCfg(uint heroid, uint actID)
		{
			DictionaryView<uint, ResInBatMsgHeroActCfg> dictionaryView = null;
			this.heroActData.TryGetValue(heroid, out dictionaryView);
			if (dictionaryView == null)
			{
				return null;
			}
			ResInBatMsgHeroActCfg result = null;
			dictionaryView.TryGetValue(actID, out result);
			return result;
		}

		public void On_InBattleMsg_OpenForm(CUIEvent uiEvent)
		{
			CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_Textmsg);
			if (this.m_shortcutChat != null)
			{
				this.m_shortcutChat.OpenForm(this.m_formScript, uiEvent);
			}
		}
	}
}
