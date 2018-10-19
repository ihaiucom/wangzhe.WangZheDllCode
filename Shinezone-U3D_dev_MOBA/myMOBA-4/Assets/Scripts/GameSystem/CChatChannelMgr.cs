using CSProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.GameSystem
{
	public class CChatChannelMgr
	{
		public enum EChatTab
		{
			Normal,
			Room,
			Team,
			Settle,
			GuildMatch
		}

		public ListView<CChatChannel> NormalChannelList = new ListView<CChatChannel>();

		public ListView<CChatChannel> FriendChannelList = new ListView<CChatChannel>();

		public List<uint> CurActiveChannels = new List<uint>();

		public CChatChannelMgr.EChatTab ChatTab;

		public CChatChannelMgr()
		{
			this.NormalChannelList.Add(new CChatChannel(EChatChannel.Lobby, 0u, 0uL, 0u));
			this.SetChatTab(CChatChannelMgr.EChatTab.Normal);
		}

		public void ClearAll()
		{
			for (int i = 0; i < this.NormalChannelList.Count; i++)
			{
				if (this.NormalChannelList[i] != null)
				{
					this.NormalChannelList[i].Clear();
				}
			}
			for (int j = 0; j < this.FriendChannelList.Count; j++)
			{
				if (this.FriendChannelList[j] != null)
				{
					this.FriendChannelList[j].Clear();
				}
			}
			this.SetChatTab(CChatChannelMgr.EChatTab.Normal);
		}

		public void SetChatTab(CChatChannelMgr.EChatTab type)
		{
			this.CurActiveChannels.Clear();
			if (type == CChatChannelMgr.EChatTab.Normal)
			{
				this.CurActiveChannels.Add(2u);
				this.CurActiveChannels.Add(4u);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null && masterRoleInfo.PvpLevel >= CGuildHelper.GetGuildMemberMinPvpLevel() && Singleton<CGuildSystem>.GetInstance() != null)
				{
					if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
					{
						this.CurActiveChannels.Add(5u);
					}
					else
					{
						this.CurActiveChannels.Add(6u);
					}
				}
				this.ChatTab = type;
			}
			else if (type == CChatChannelMgr.EChatTab.Room)
			{
				this.CurActiveChannels.Add(1u);
				this.CurActiveChannels.Add(4u);
				this.ChatTab = type;
			}
			else if (type == CChatChannelMgr.EChatTab.Team)
			{
				this.CurActiveChannels.Add(0u);
				this.CurActiveChannels.Add(4u);
				this.ChatTab = type;
			}
			else if (type == CChatChannelMgr.EChatTab.Settle)
			{
				this.CurActiveChannels.Add(10u);
				this.ChatTab = type;
			}
			else if (type == CChatChannelMgr.EChatTab.GuildMatch)
			{
				if (Singleton<CGuildMatchSystem>.GetInstance().IsSelfInAnyTeam())
				{
					this.CurActiveChannels.Add(3u);
				}
				this.CurActiveChannels.Add(5u);
				this.CurActiveChannels.Add(4u);
				this.ChatTab = type;
			}
		}

		public CChatChannel GetChannel(EChatChannel type)
		{
			CChatChannel cChatChannel = this._getChannel(type, 0uL, 0u);
			if (cChatChannel == null)
			{
				cChatChannel = this.CreateChannel(type, 0uL, 0u);
			}
			return cChatChannel;
		}

		public CChatChannel GetFriendChannel(ulong ullUid = 0uL, uint dwLogicWorldId = 0u)
		{
			CChatChannel cChatChannel = this._getChannel(EChatChannel.Friend, ullUid, dwLogicWorldId);
			if (cChatChannel == null)
			{
				cChatChannel = this.CreateChannel(EChatChannel.Friend, ullUid, dwLogicWorldId);
			}
			return cChatChannel;
		}

		public int GetAllFriendUnreadCount()
		{
			int num = 0;
			for (int i = 0; i < this.FriendChannelList.Count; i++)
			{
				CChatChannel cChatChannel = this.FriendChannelList[i];
				num += cChatChannel.GetUnreadCount();
			}
			return num;
		}

		public void Clear(EChatChannel type, ulong ullUid = 0uL, uint dwLogicWorldId = 0u)
		{
			CChatChannel cChatChannel = this._getChannel(type, ullUid, dwLogicWorldId);
			if (cChatChannel != null)
			{
				cChatChannel.Clear();
			}
		}

		public int GetUnreadCount(EChatChannel type, ulong ullUid = 0uL, uint dwLogicWorldId = 0u)
		{
			CChatChannel cChatChannel = this._getChannel(type, ullUid, dwLogicWorldId);
			if (cChatChannel == null)
			{
				cChatChannel = this.CreateChannel(type, ullUid, dwLogicWorldId);
			}
			return cChatChannel.GetUnreadCount();
		}

		public int GetFriendTotal_UnreadCount()
		{
			int num = 0;
			for (int i = 0; i < this.FriendChannelList.Count; i++)
			{
				CChatChannel cChatChannel = this.FriendChannelList[i];
				COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(cChatChannel.ullUid, cChatChannel.dwLogicWorldId);
				if (gameOrSnsFriend != null && gameOrSnsFriend.bIsOnline == 1)
				{
					num += cChatChannel.GetUnreadCount();
				}
			}
			return num;
		}

		public void Add_CurChatFriend(CChatEntity chatEnt)
		{
			DebugHelper.Assert(Singleton<CChatController>.instance.model.sysData.ullUid != 0uL && Singleton<CChatController>.instance.model.sysData.dwLogicWorldId != 0u);
			this.Add_ChatEntity(chatEnt, EChatChannel.Friend, Singleton<CChatController>.instance.model.sysData.ullUid, Singleton<CChatController>.instance.model.sysData.dwLogicWorldId);
		}

		public void Add_ChatEntity(CChatEntity chatEnt, EChatChannel type, ulong ullUid = 0uL, uint dwLogicWorldId = 0u)
		{
			CChatChannel cChatChannel = this._getChannel(type, ullUid, dwLogicWorldId);
			if (cChatChannel == null)
			{
				cChatChannel = this.CreateChannel(type, ullUid, dwLogicWorldId);
			}
			if (chatEnt.type != EChaterType.System || chatEnt.type != EChaterType.OfflineInfo)
			{
				if (Singleton<CChatController>.instance.view.ChatParser != null)
				{
					Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = false;
					if (type == EChatChannel.Guild)
					{
						Singleton<CChatController>.instance.view.ChatParser.maxWidth = CChatParser.chat_guild_list_max_width;
					}
					else
					{
						Singleton<CChatController>.instance.view.ChatParser.maxWidth = CChatParser.chat_list_max_width;
					}
					Singleton<CChatController>.instance.view.ChatParser.Parse(chatEnt.text, CChatParser.start_x, chatEnt);
				}
				else
				{
					DebugHelper.Assert(false, "CChatController.instance.view.ChatParser = null! StackTrace = " + new StackTrace().ToString());
				}
			}
			CChatEntity last = cChatChannel.GetLast();
			if (last != null && last.time != 0 && chatEnt.time - last.time > 60)
			{
				cChatChannel.Add(CChatUT.Build_4_Time());
			}
			cChatChannel.Add(chatEnt);
		}

		public CChatChannel _getChannel(EChatChannel type, ulong ullUid = 0uL, uint dwLogicWorldId = 0u)
		{
			if (type != EChatChannel.Friend)
			{
				for (int i = 0; i < this.NormalChannelList.Count; i++)
				{
					if (this.NormalChannelList[i] != null && this.NormalChannelList[i].ChannelType == type)
					{
						return this.NormalChannelList[i];
					}
				}
			}
			else
			{
				for (int j = 0; j < this.FriendChannelList.Count; j++)
				{
					if (this.FriendChannelList[j] != null && this.FriendChannelList[j].ullUid == ullUid && this.FriendChannelList[j].dwLogicWorldId == dwLogicWorldId)
					{
						return this.FriendChannelList[j];
					}
				}
			}
			return null;
		}

		public CChatChannel CreateChannel(EChatChannel type, ulong ullUid = 0uL, uint dwLogicWorldId = 0u)
		{
			CChatChannel cChatChannel = this._getChannel(type, ullUid, dwLogicWorldId);
			if (cChatChannel != null)
			{
				return cChatChannel;
			}
			if (type != EChatChannel.Friend)
			{
				cChatChannel = new CChatChannel(type, 0u, 0uL, 0u);
				this.NormalChannelList.Add(cChatChannel);
			}
			else
			{
				cChatChannel = new CChatChannel(type, 7000u, ullUid, dwLogicWorldId);
				cChatChannel.list.Add(CChatUT.Build_4_System(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_4")));
				cChatChannel.ReadAll();
				this.FriendChannelList.Add(cChatChannel);
			}
			return cChatChannel;
		}
	}
}
