using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CChatSysData
	{
		public ulong ullUid;

		public uint dwLogicWorldId;

		public uint lastTimeStamp;

		public uint restChatFreeCnt;

		private int m_chatCostNum;

		private int m_chatCostType;

		public EChatChannel LastChannel = EChatChannel.None;

		public EChatChannel CurChannel = EChatChannel.None;

		public CChatEntity entryEntity = new CChatEntity();

		public List<GuildRecruitInfo> m_guildRecruitInfos = new List<GuildRecruitInfo>();

		public int chatCostNum
		{
			get
			{
				if (this.m_chatCostNum == 0)
				{
					this.m_chatCostNum = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(125u).dwConfValue;
				}
				return this.m_chatCostNum;
			}
		}

		public int chatCostType
		{
			get
			{
				if (this.m_chatCostType == 0)
				{
					this.m_chatCostType = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(126u).dwConfValue;
				}
				return this.m_chatCostType;
			}
		}

		public CChatSysData()
		{
			this.lastTimeStamp = 0u;
		}

		public void Clear()
		{
			this.ullUid = 0uL;
			this.dwLogicWorldId = 0u;
			this.lastTimeStamp = 0u;
			this.LastChannel = EChatChannel.None;
			this.CurChannel = EChatChannel.None;
			this.restChatFreeCnt = 0u;
			this.m_chatCostNum = 0;
			this.m_chatCostType = 0;
			this.entryEntity.Clear();
			this.m_guildRecruitInfos.Clear();
		}

		public void ClearEntryText()
		{
			this.CurChannel = EChatChannel.None;
			if (this.entryEntity != null)
			{
				this.entryEntity.Clear();
			}
			Singleton<CChatController>.instance.view.SetEntryChannelImage(EChatChannel.None);
			Singleton<CChatController>.instance.view.Clear_EntryForm_Node();
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
		}

		public void Add_NewContent_Entry(string a, EChatChannel curChannel)
		{
			if (curChannel == EChatChannel.Lobby && Singleton<CLoudSpeakerSys>.instance.IsSpeakerShowing())
			{
				return;
			}
			this.CurChannel = curChannel;
			if (this.entryEntity == null)
			{
				return;
			}
			this.entryEntity.Clear();
			this.entryEntity.text = a;
			float x = CChatView.entrySizeLobby.x;
			if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Room)
			{
				x = CChatView.entrySizeRoom.x;
			}
			else if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Team)
			{
				x = CChatView.entrySizeTeam.x;
			}
			Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = true;
			Singleton<CChatController>.instance.view.ChatParser.maxWidth = (int)x - CChatParser.chat_entry_channel_img_width;
			Singleton<CChatController>.instance.view.ChatParser.Parse(this.entryEntity.text, CChatParser.start_x, this.entryEntity);
		}

		public void Add_NewContent_Entry_ColorFlag(string a, EChatChannel curChannel)
		{
			if (curChannel == EChatChannel.Lobby && Singleton<CLoudSpeakerSys>.instance.IsSpeakerShowing())
			{
				return;
			}
			this.CurChannel = curChannel;
			if (this.entryEntity == null)
			{
				return;
			}
			this.entryEntity.Clear();
			this.entryEntity.text = a;
			float x = CChatView.entrySizeLobby.x;
			if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Room)
			{
				x = CChatView.entrySizeRoom.x;
			}
			else if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Team)
			{
				x = CChatView.entrySizeTeam.x;
			}
			Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = true;
			Singleton<CChatController>.instance.view.ChatParser.maxWidth = (int)x - CChatParser.chat_entry_channel_img_width;
			int lingWidth = (int)x - CChatParser.chat_entry_channel_img_width;
			int num = 0;
			List<string> list = Singleton<CChatController>.instance.ColorParser.Parse(lingWidth, this.entryEntity.text, Singleton<CChatController>.instance.view.ChatParser.viewFontSize, out num);
			if (list.get_Count() >= 1)
			{
				this.entryEntity.TextObjList.Add(new CTextImageNode(list.get_Item(0), CChatParser.InfoType.Text, (float)num, 0f, 0f, 0f));
			}
		}

		public void Add_NewContent_Entry_Speaker(string a)
		{
			this.CurChannel = EChatChannel.Speaker;
			if (this.entryEntity == null)
			{
				return;
			}
			this.entryEntity.Clear();
			this.entryEntity.text = a;
			float x = CChatView.entrySizeLobby.x;
			Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = true;
			Singleton<CChatController>.instance.view.ChatParser.maxWidth = (int)x - CChatParser.chat_entry_channel_img_width;
			Singleton<CChatController>.instance.view.ChatParser.Parse(this.entryEntity.text, CChatParser.start_x, this.entryEntity);
		}
	}
}
