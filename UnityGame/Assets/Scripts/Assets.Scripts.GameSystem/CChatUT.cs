using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CChatUT
	{
		public static EChatChannel Convert_ChatMsgType_Channel(byte v)
		{
			return CChatUT.Convert_ChatMsgType_Channel((COM_CHAT_MSG_TYPE)v);
		}

		public static string GetEChatChannel_Text(EChatChannel channel)
		{
			switch (channel)
			{
			case EChatChannel.Team:
				return Singleton<CTextManager>.GetInstance().GetText("chat_title_team");
			case EChatChannel.Room:
				return Singleton<CTextManager>.instance.GetText("chat_title_room");
			case EChatChannel.Lobby:
				return Singleton<CTextManager>.instance.GetText("chat_title_total");
			case EChatChannel.GuildMatchTeam:
				return Singleton<CTextManager>.GetInstance().GetText("Chat_title_GuildMatchTeam");
			case EChatChannel.Friend:
				return Singleton<CTextManager>.instance.GetText("chat_title_friend");
			case EChatChannel.Guild:
				return Singleton<CTextManager>.GetInstance().GetText("chat_title_guild");
			case EChatChannel.GuildRecruit:
				return Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Recruit");
			case EChatChannel.Settle:
				return Singleton<CTextManager>.GetInstance().GetText("Chat_title_Settle");
			}
			return "error";
		}

		public static EChatChannel Convert_ChatMsgType_Channel(COM_CHAT_MSG_TYPE type)
		{
			switch (type)
			{
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_LOGIC_WORLD:
				return EChatChannel.Lobby;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_PRIVATE:
				return EChatChannel.Friend;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_ROOM:
				return EChatChannel.Room;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_GUILD:
				return EChatChannel.Guild;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_BATTLE:
				return EChatChannel.Select_Hero;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_TEAM:
				return EChatChannel.Team;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_SETTLE:
				return EChatChannel.Settle;
			case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_GUILD_TEAM:
				return EChatChannel.GuildMatchTeam;
			}
			return EChatChannel.None;
		}

		public static COM_CHAT_MSG_TYPE Convert_Channel_ChatMsgType(EChatChannel type)
		{
			switch (type)
			{
			case EChatChannel.Team:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_TEAM;
			case EChatChannel.Room:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_ROOM;
			case EChatChannel.Lobby:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_LOGIC_WORLD;
			case EChatChannel.GuildMatchTeam:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_GUILD_TEAM;
			case EChatChannel.Friend:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_PRIVATE;
			case EChatChannel.Guild:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_GUILD;
			case EChatChannel.Settle:
				return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_SETTLE;
			}
			return (COM_CHAT_MSG_TYPE)100000;
		}

		public static CChatEntity Build_4_Offline_Friend(COMDT_OFFLINE_CHAT_MSG data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stChatMsg.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stChatMsg.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.stChatMsg.szContent);
			cChatEntity.type = EChaterType.Friend;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			return cChatEntity;
		}

		public static CChatEntity Build_4_OfflineOrOnline(bool bOffline)
		{
			return new CChatEntity
			{
				text = (bOffline ? Singleton<CTextManager>.instance.GetText("FriendChat_Offline_Info") : Singleton<CTextManager>.instance.GetText("FriendChat_online_Info")),
				type = EChaterType.OfflineInfo
			};
		}

		public static CChatEntity Build_4_Friend(COMDT_CHAT_MSG_PRIVATE data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Friend;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_Lobby(COMDT_CHAT_MSG_LOGIC_WORLD data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Strenger;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_Speaker(COMDT_CHAT_MSG_HORN data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Speaker;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_LoudSpeaker(COMDT_CHAT_MSG_HORN data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.LoudSpeaker;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_Guild(COMDT_CHAT_MSG_GUILD data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Strenger;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_GuildMatchTeam(COMDT_CHAT_MSG_GUILD_TEAM data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Strenger;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_Room(COMDT_CHAT_MSG_ROOM data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Strenger;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			return cChatEntity;
		}

		public static CChatEntity Build_4_Self(string content)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			cChatEntity.iLogicWorldID = 0u;
			cChatEntity.text = content;
			cChatEntity.type = EChaterType.Self;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_SelectHero(COMDT_CHAT_MSG_BATTLE data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			if (cChatEntity.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
			{
				cChatEntity.type = EChaterType.Self;
			}
			else
			{
				cChatEntity.type = EChaterType.Strenger;
			}
			if (data.bChatType == 1)
			{
				CChatModel.HeroChatTemplateInfo heroChatTemplateInfo = Singleton<CChatController>.instance.model.Get_HeroSelect_ChatTemplate((int)data.stChatInfo.stContentID.dwTextID);
				if (heroChatTemplateInfo.isValid())
				{
					cChatEntity.text = heroChatTemplateInfo.templateString;
				}
			}
			if (data.bChatType == 2)
			{
				cChatEntity.text = UT.Bytes2String(data.stChatInfo.stContentStr.szContent);
			}
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			return cChatEntity;
		}

		public static CChatEntity Build_4_LeaveRoom(string content)
		{
			return new CChatEntity
			{
				type = EChaterType.LeaveRoom,
				text = content
			};
		}

		public static CChatEntity Build_4_System(string content)
		{
			return new CChatEntity
			{
				type = EChaterType.System,
				text = content
			};
		}

		public static CChatEntity Build_4_OfflineInfo(string content)
		{
			return new CChatEntity
			{
				type = EChaterType.OfflineInfo,
				text = content
			};
		}

		public static CChatEntity Build_4_Team(COMDT_CHAT_MSG_TEAM data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Strenger;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static CChatEntity Build_4_Settle(COMDT_CHAT_MSG_SETTLE data)
		{
			CChatEntity cChatEntity = new CChatEntity();
			cChatEntity.ullUid = data.stFrom.ullUid;
			cChatEntity.iLogicWorldID = (uint)data.stFrom.iLogicWorldID;
			cChatEntity.text = UT.Bytes2String(data.szContent);
			cChatEntity.type = EChaterType.Strenger;
			CChatUT.GetUser(cChatEntity.type, cChatEntity.ullUid, cChatEntity.iLogicWorldID, out cChatEntity.name, out cChatEntity.openId, out cChatEntity.level, out cChatEntity.head_url, out cChatEntity.stGameVip);
			cChatEntity.time = CRoleInfo.GetCurrentUTCTime();
			return cChatEntity;
		}

		public static void GetUser(EChaterType type, ulong ulluid, uint logicworld_id, out string name, out string openId, out string level, out string head_url, out COMDT_GAME_VIP_CLIENT stGameVip)
		{
			name = "error";
			openId = "error";
			level = "-1";
			head_url = string.Empty;
			stGameVip = null;
			if (type == EChaterType.Self)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					name = masterRoleInfo.Name;
					level = masterRoleInfo.PvpLevel.ToString();
					stGameVip = masterRoleInfo.GetNobeInfo().stGameVipClient;
					head_url = masterRoleInfo.HeadUrl.Substring(0, masterRoleInfo.HeadUrl.LastIndexOf("/"));
				}
			}
			else if (type == EChaterType.Friend)
			{
				COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.GetInstance().model.GetGameOrSnsFriend(ulluid, logicworld_id);
				if (gameOrSnsFriend != null)
				{
					name = UT.Bytes2String(gameOrSnsFriend.szUserName);
					openId = UT.Bytes2String(gameOrSnsFriend.szOpenId);
					int dwPvpLvl = (int)gameOrSnsFriend.dwPvpLvl;
					level = dwPvpLvl.ToString();
					head_url = UT.Bytes2String(gameOrSnsFriend.szHeadUrl);
					stGameVip = gameOrSnsFriend.stGameVip;
				}
			}
			else if (type == EChaterType.Strenger)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ulluid, logicworld_id);
				if (cOMDT_CHAT_PLAYER_INFO != null)
				{
					name = UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szName);
					openId = UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szOpenID);
					int dwLevel = (int)cOMDT_CHAT_PLAYER_INFO.dwLevel;
					level = dwLevel.ToString();
					head_url = UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szHeadUrl);
					stGameVip = cOMDT_CHAT_PLAYER_INFO.stVip;
				}
			}
			else if (type == EChaterType.Speaker || type == EChaterType.LoudSpeaker)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO2 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ulluid, logicworld_id);
				if (cOMDT_CHAT_PLAYER_INFO2 != null)
				{
					name = UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO2.szName);
					openId = UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO2.szOpenID);
					int dwLevel2 = (int)cOMDT_CHAT_PLAYER_INFO2.dwLevel;
					level = dwLevel2.ToString();
					head_url = UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO2.szHeadUrl);
					stGameVip = cOMDT_CHAT_PLAYER_INFO2.stVip;
				}
			}
		}

		public static CChatEntity Build_4_Time()
		{
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			DateTime dateTime = Utility.ToUtcTime2Local((long)currentUTCTime);
			return new CChatEntity
			{
				text = dateTime.get_TimeOfDay().ToString(),
				type = EChaterType.Time
			};
		}

		public static CChatEntity Build_4_Time(int curSec)
		{
			CChatEntity cChatEntity = new CChatEntity();
			DateTime dateTime = Utility.ToUtcTime2Local((long)curSec);
			cChatEntity.ullUid = 0uL;
			cChatEntity.iLogicWorldID = 0u;
			cChatEntity.text = string.Format("{0}/{1}/{2}  {3}", new object[]
			{
				dateTime.get_Year(),
				dateTime.get_Month(),
				dateTime.get_Day(),
				dateTime.get_TimeOfDay().ToString()
			});
			cChatEntity.type = EChaterType.Time;
			return cChatEntity;
		}

		public static string Build_4_ChatEntry(bool bFriend, CChatEntity ent)
		{
			string text = bFriend ? Singleton<CTextManager>.instance.GetText("chat_title_friend") : Singleton<CTextManager>.instance.GetText("chat_title_total");
			if (bFriend)
			{
				COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.GetInstance().model.GetGameOrSnsFriend(ent.ullUid, ent.iLogicWorldID);
				if (gameOrSnsFriend != null)
				{
					text = text + CChatUT.ColorString(0u, UT.Bytes2String(gameOrSnsFriend.szUserName)) + ":" + ent.text;
				}
			}
			else
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ent.ullUid, ent.iLogicWorldID);
				if (text != null && cOMDT_CHAT_PLAYER_INFO != null)
				{
					text = text + CChatUT.ColorString(0u, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szName)) + ":" + ent.text;
				}
			}
			return text;
		}

		public static string Build_4_EntryString(EChatChannel type, ulong ullUid, uint iLogicWorldID, string rawText)
		{
			string result = string.Empty;
			if (type == EChatChannel.Friend)
			{
				COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.GetInstance().model.GetGameOrSnsFriend(ullUid, iLogicWorldID);
				if (gameOrSnsFriend != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(gameOrSnsFriend.szUserName), rawText);
				}
			}
			else if (type == EChatChannel.Lobby)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
				if (cOMDT_CHAT_PLAYER_INFO != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szName), rawText);
				}
			}
			else if (type == EChatChannel.Guild)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO2 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
				if (cOMDT_CHAT_PLAYER_INFO2 != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO2.szName), rawText);
				}
			}
			else if (type == EChatChannel.GuildMatchTeam)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO3 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
				if (cOMDT_CHAT_PLAYER_INFO3 != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO3.szName), rawText);
				}
			}
			else if (type == EChatChannel.Room)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO4 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
				if (cOMDT_CHAT_PLAYER_INFO4 != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO4.szName), rawText);
				}
			}
			else if (type == EChatChannel.Team)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO5 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
				if (cOMDT_CHAT_PLAYER_INFO5 != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO5.szName), rawText);
				}
			}
			else if (type == EChatChannel.Settle)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO6 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
				if (cOMDT_CHAT_PLAYER_INFO6 != null)
				{
					result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO6.szName), rawText);
				}
			}
			else
			{
				result = "ERROR, in Build_4_EntryString";
			}
			return result;
		}

		public static string Build_4_Speaker_EntryString(ulong ullUid, uint iLogicWorldID, string rawText)
		{
			COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
			string result;
			if (cOMDT_CHAT_PLAYER_INFO != null)
			{
				result = string.Format(CChatController.fmt, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szName), rawText);
			}
			else
			{
				result = "ERROR, in Build_4_EntryString";
			}
			return result;
		}

		public static string Build_4_LoudSpeaker_EntryString(ulong ullUid, uint iLogicWorldID, string rawText)
		{
			COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
			string result;
			if (cOMDT_CHAT_PLAYER_INFO != null)
			{
				result = string.Format(CChatController.fmt_gold_name, UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szName), rawText);
			}
			else
			{
				result = "ERROR, in Build_4_EntryString";
			}
			return result;
		}

		public static string ColorString(uint color, string text)
		{
			return string.Format("<color=green>{0}</color>", text);
		}

		public static void DestoryAllChild(GameObject node)
		{
			if (node == null)
			{
				return;
			}
			while (node.transform.childCount > 0)
			{
				Transform child = node.transform.GetChild(0);
				if (child == null || child.gameObject == null)
				{
					return;
				}
				CUICommonSystem.DestoryObj(child.gameObject, 0f);
			}
		}

		public static void EnterRoom()
		{
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Room);
			Singleton<CChatController>.instance.ShowPanel(true, false);
			Singleton<CChatController>.GetInstance().bSendChat = true;
			Singleton<CChatController>.instance.view.UpView(true);
			Singleton<CChatController>.instance.model.sysData.ClearEntryText();
		}

		public static void LeaveRoom()
		{
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
			Singleton<CChatController>.instance.view.UpView(false);
			Singleton<CChatController>.instance.model.sysData.ClearEntryText();
		}

		public static void EnterSettle()
		{
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Room);
			Singleton<CChatController>.instance.ShowPanel(true, false);
			Singleton<CChatController>.GetInstance().bSendChat = true;
			Singleton<CChatController>.instance.view.UpView(true);
			Singleton<CChatController>.instance.model.sysData.ClearEntryText();
		}

		public static void LeaveSettle()
		{
			Singleton<CChatController>.instance.ShowPanel(false, false);
		}

		public static void EnterGuildMatch()
		{
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.GuildMatchTeam, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.GuildMatch);
			Singleton<CChatController>.instance.ShowPanel(true, false);
			Singleton<CChatController>.GetInstance().bSendChat = true;
			Singleton<CChatController>.instance.view.UpView(true);
			Singleton<CChatController>.instance.model.sysData.ClearEntryText();
		}

		public static void LeaveGuildMatch()
		{
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.GuildMatchTeam, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
			Singleton<CChatController>.instance.view.UpView(false);
			Singleton<CChatController>.instance.model.sysData.ClearEntryText();
		}
	}
}
