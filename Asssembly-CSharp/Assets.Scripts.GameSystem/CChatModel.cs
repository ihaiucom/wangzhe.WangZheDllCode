using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CChatModel
	{
		public struct HeroChatTemplateInfo
		{
			public int dwID;

			public int dwGroupID;

			public int dwTag;

			public string templateString;

			public HeroChatTemplateInfo(int id)
			{
				this.dwID = -1;
				this.dwTag = -1;
				this.dwGroupID = -1;
				this.templateString = string.Empty;
			}

			public string GetTemplateString()
			{
				return this.templateString;
			}

			public int GetID()
			{
				return this.dwID;
			}

			public int GetGroupID()
			{
				return this.dwGroupID;
			}

			public int GetTag()
			{
				return this.dwTag;
			}

			public bool isValid()
			{
				return this.dwID != -1;
			}
		}

		public uint complanCount;

		public bool bEnableInBattleInputChat;

		public ListView<COMDT_CHAT_PLAYER_INFO> playerInfos = new ListView<COMDT_CHAT_PLAYER_INFO>();

		public List<COfflineChatIndex> stOfflineChatIndexList = new List<COfflineChatIndex>();

		public CChatSysData sysData;

		public CChatChannelMgr channelMgr;

		public List<CChatModel.HeroChatTemplateInfo> selectHeroTemplateList = new List<CChatModel.HeroChatTemplateInfo>();

		private List<CChatModel.HeroChatTemplateInfo> m_CurSelectGroupTemplateInfo = new List<CChatModel.HeroChatTemplateInfo>();

		public int m_curGroupID = 1;

		private int index;

		public CChatModel()
		{
			this.sysData = new CChatSysData();
			this.channelMgr = new CChatChannelMgr();
		}

		public void AddOfflineChatIndex(ulong ullUid, uint dwLogicWorldId, int index)
		{
			COfflineChatIndex cOfflineChatIndex = null;
			for (int i = 0; i < this.stOfflineChatIndexList.get_Count(); i++)
			{
				COfflineChatIndex cOfflineChatIndex2 = this.stOfflineChatIndexList.get_Item(i);
				if (cOfflineChatIndex2.ullUid == ullUid && cOfflineChatIndex2.dwLogicWorldId == dwLogicWorldId)
				{
					cOfflineChatIndex = cOfflineChatIndex2;
				}
			}
			if (cOfflineChatIndex == null)
			{
				cOfflineChatIndex = new COfflineChatIndex(ullUid, dwLogicWorldId);
				this.stOfflineChatIndexList.Add(cOfflineChatIndex);
			}
			if (!cOfflineChatIndex.indexList.Contains(index))
			{
				cOfflineChatIndex.indexList.Add(index);
			}
		}

		public void ClearCOfflineChatIndex(ulong ullUid, uint dwLogicWorldId)
		{
			int num = -1;
			for (int i = 0; i < this.stOfflineChatIndexList.get_Count(); i++)
			{
				COfflineChatIndex cOfflineChatIndex = this.stOfflineChatIndexList.get_Item(i);
				if (cOfflineChatIndex.ullUid == ullUid && cOfflineChatIndex.dwLogicWorldId == dwLogicWorldId)
				{
					num = i;
				}
			}
			if (num != -1)
			{
				this.stOfflineChatIndexList.RemoveAt(num);
			}
		}

		public void ClearCOfflineChatIndex(COfflineChatIndex data)
		{
			if (data == null)
			{
				return;
			}
			this.stOfflineChatIndexList.Remove(data);
		}

		public COfflineChatIndex GetCOfflineChatIndex(ulong ullUid, uint dwLogicWorldId)
		{
			for (int i = 0; i < this.stOfflineChatIndexList.get_Count(); i++)
			{
				COfflineChatIndex cOfflineChatIndex = this.stOfflineChatIndexList.get_Item(i);
				if (cOfflineChatIndex.ullUid == ullUid && cOfflineChatIndex.dwLogicWorldId == dwLogicWorldId)
				{
					return cOfflineChatIndex;
				}
			}
			return null;
		}

		public List<CChatModel.HeroChatTemplateInfo> GetCurGroupTemplateInfo()
		{
			return this.m_CurSelectGroupTemplateInfo;
		}

		public void SetCurGroupTemplateInfo(uint cfgID)
		{
			this.m_curGroupID = 1;
			if (cfgID > 0u)
			{
				this.m_curGroupID = (int)cfgID;
			}
			this.m_CurSelectGroupTemplateInfo.Clear();
			this.m_CurSelectGroupTemplateInfo = this.GetCurGroupInfo(this.m_curGroupID);
		}

		private List<CChatModel.HeroChatTemplateInfo> GetCurGroupInfo(int groupID)
		{
			List<CChatModel.HeroChatTemplateInfo> list = new List<CChatModel.HeroChatTemplateInfo>();
			for (int i = 0; i < this.selectHeroTemplateList.get_Count(); i++)
			{
				if (this.selectHeroTemplateList.get_Item(i).GetGroupID() == groupID)
				{
					list.Add(this.selectHeroTemplateList.get_Item(i));
				}
			}
			return list;
		}

		public void ClearAll()
		{
			this.complanCount = 0u;
			this.playerInfos.Clear();
			this.sysData.Clear();
			this.channelMgr.ClearAll();
			this.stOfflineChatIndexList.Clear();
		}

		public void SetTimeStamp(EChatChannel v, uint time)
		{
			this.sysData.lastTimeStamp = time;
		}

		public void SetRestFreeCnt(EChatChannel v, uint count)
		{
			this.sysData.restChatFreeCnt = count;
		}

		public void Load_HeroSelect_ChatTemplate()
		{
			if (this.selectHeroTemplateList.get_Count() == 0)
			{
				DatabinTable<ResHeroSelectTextData, uint> selectHeroChatDatabin = GameDataMgr.m_selectHeroChatDatabin;
				if (selectHeroChatDatabin == null)
				{
					return;
				}
				Dictionary<long, object>.Enumerator enumerator = selectHeroChatDatabin.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.get_Current();
					ResHeroSelectTextData resHeroSelectTextData = (ResHeroSelectTextData)current.get_Value();
					CChatModel.HeroChatTemplateInfo heroChatTemplateInfo = new CChatModel.HeroChatTemplateInfo(0);
					heroChatTemplateInfo.dwID = (int)resHeroSelectTextData.dwID;
					heroChatTemplateInfo.dwGroupID = (int)resHeroSelectTextData.dwGroupID;
					heroChatTemplateInfo.dwTag = (int)resHeroSelectTextData.dwTag;
					heroChatTemplateInfo.templateString = StringHelper.UTF8BytesToString(ref resHeroSelectTextData.szContent);
					this.selectHeroTemplateList.Add(heroChatTemplateInfo);
				}
			}
		}

		public CChatModel.HeroChatTemplateInfo Get_HeroSelect_ChatTemplate(int index)
		{
			int count = this.m_CurSelectGroupTemplateInfo.get_Count();
			if (index >= 0 && index < count)
			{
				return this.m_CurSelectGroupTemplateInfo.get_Item(index);
			}
			return new CChatModel.HeroChatTemplateInfo(-1);
		}

		public CChatEntity GetLastUnread_Selected()
		{
			ListView<CChatEntity> list = this.channelMgr.GetChannel(EChatChannel.Select_Hero).list;
			if (list == null || list.Count == 0)
			{
				return null;
			}
			if (list != null && this.index >= 0 && this.index < list.Count)
			{
				return list[this.index++];
			}
			return null;
		}

		public bool IsTemplate_IndexValid(int index)
		{
			return index >= 0 && index < this.m_CurSelectGroupTemplateInfo.get_Count();
		}

		public void Clear_HeroSelected()
		{
			this.index = 0;
			this.channelMgr.GetChannel(EChatChannel.Select_Hero).Clear();
		}

		public int GetFriendTotal_UnreadCount()
		{
			return this.channelMgr.GetFriendTotal_UnreadCount();
		}

		public int Has_PLAYER_INFO(COMDT_CHAT_PLAYER_INFO info)
		{
			int result = -1;
			int count = this.playerInfos.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.playerInfos[i].ullUid == info.ullUid)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public void Add_Palyer_Info(COMDT_CHAT_PLAYER_INFO info)
		{
			if (info == null)
			{
				return;
			}
			int num = this.Has_PLAYER_INFO(info);
			if (num == -1)
			{
				this.playerInfos.Add(info);
			}
			else
			{
				this.playerInfos[num] = info;
			}
		}

		public void Remove_Palyer_Info(COMDT_CHAT_PLAYER_INFO info)
		{
			if (info == null)
			{
				return;
			}
			this.playerInfos.Remove(info);
		}

		public COMDT_CHAT_PLAYER_INFO Get_Palyer_Info(ulong ullUid, uint iLogicWorldID)
		{
			for (int i = 0; i < this.playerInfos.Count; i++)
			{
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO = this.playerInfos[i];
				if (cOMDT_CHAT_PLAYER_INFO.ullUid == ullUid && (long)cOMDT_CHAT_PLAYER_INFO.iLogicWorldID == (long)((ulong)iLogicWorldID))
				{
					return cOMDT_CHAT_PLAYER_INFO;
				}
			}
			return null;
		}
	}
}
