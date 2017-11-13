using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class RoomInfo
	{
		public int iRoomEntity;

		public uint dwRoomID;

		public uint dwRoomSeq;

		public PlayerUniqueID selfInfo;

		public PlayerUniqueID roomOwner;

		public RoomAttrib roomAttrib;

		private ListView<MemberInfo>[] campMemberList;

		public uint selfObjID;

		public COM_ROOM_FROMTYPE fromType;

		public ListView<MemberInfo> this[COM_PLAYERCAMP camp]
		{
			get
			{
				if (camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_MID && camp < (COM_PLAYERCAMP)this.campMemberList.Length)
				{
					return this.campMemberList[(int)camp];
				}
				return null;
			}
		}

		public int CampListCount
		{
			get
			{
				return this.campMemberList.Length;
			}
		}

		public ListView<MemberInfo> this[int campIndex]
		{
			get
			{
				return this.campMemberList[campIndex];
			}
		}

		public RoomInfo()
		{
			this.selfInfo = new PlayerUniqueID();
			this.roomOwner = new PlayerUniqueID();
			this.roomAttrib = new RoomAttrib();
			this.campMemberList = new ListView<MemberInfo>[3];
			for (int i = 0; i < 3; i++)
			{
				this.campMemberList[i] = new ListView<MemberInfo>();
			}
			this.fromType = COM_ROOM_FROMTYPE.COM_ROOM_FROM_INTERNAL;
		}

		public void SortCampMemList(COM_PLAYERCAMP camp)
		{
			ListView<MemberInfo> listView = this.campMemberList[(int)camp];
			SortedList<uint, MemberInfo> sortedList = new SortedList<uint, MemberInfo>();
			ListView<MemberInfo>.Enumerator enumerator = listView.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MemberInfo current = enumerator.Current;
				uint dwPosOfCamp = current.dwPosOfCamp;
				sortedList.Add(dwPosOfCamp, current);
			}
			this.campMemberList[(int)camp] = new ListView<MemberInfo>(sortedList.get_Values());
		}

		public MemberInfo GetMemberInfo(COM_PLAYERCAMP camp, int posOfCamp)
		{
			MemberInfo result = null;
			ListView<MemberInfo> listView = this[camp];
			if (listView == null)
			{
				return result;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if ((ulong)listView[i].dwPosOfCamp == (ulong)((long)posOfCamp))
				{
					result = listView[i];
					break;
				}
			}
			return result;
		}

		public MemberInfo GetMemberInfo(uint objID)
		{
			for (int i = 0; i < this.campMemberList.Length; i++)
			{
				ListView<MemberInfo> listView = this.campMemberList[i];
				for (int j = 0; j < listView.Count; j++)
				{
					if (listView[j].dwObjId == objID)
					{
						return listView[j];
					}
				}
			}
			return null;
		}

		public MemberInfo GetMemberInfo(ulong playerUid)
		{
			for (int i = 0; i < this.campMemberList.Length; i++)
			{
				ListView<MemberInfo> listView = this.campMemberList[i];
				for (int j = 0; j < listView.Count; j++)
				{
					if (listView[j].ullUid == playerUid)
					{
						return listView[j];
					}
				}
			}
			return null;
		}

		public MemberInfo GetMasterMemberInfo()
		{
			if (this.selfObjID != 0u)
			{
				return this.GetMemberInfo(this.selfObjID);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				return this.GetMemberInfo(masterRoleInfo.playerUllUID);
			}
			return null;
		}

		public COM_PLAYERCAMP GetEnemyCamp(COM_PLAYERCAMP inCamp)
		{
			if (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			if (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
		}

		public COM_PLAYERCAMP GetSelfCamp()
		{
			for (int i = 0; i < this.campMemberList.Length; i++)
			{
				for (int j = 0; j < this.campMemberList[i].Count; j++)
				{
					if (this.campMemberList[i][j].ullUid == this.selfInfo.ullUid)
					{
						return (COM_PLAYERCAMP)i;
					}
				}
			}
			return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
		}

		public int GetFreePos(COM_PLAYERCAMP camp, int maxPlayerNum)
		{
			int result = -1;
			ListView<MemberInfo> listView = this[camp];
			if (listView == null)
			{
				return result;
			}
			for (int i = 0; i < maxPlayerNum / 2; i++)
			{
				bool flag = false;
				for (int j = 0; j < listView.Count; j++)
				{
					MemberInfo memberInfo = listView[j];
					if (memberInfo != null && (ulong)memberInfo.dwPosOfCamp == (ulong)((long)i))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public bool IsHeroExistWithCamp(COM_PLAYERCAMP camp, uint heroID)
		{
			bool result = false;
			ListView<MemberInfo> listView = this[camp];
			if (listView == null)
			{
				return result;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if (listView[i] == null || listView[i].ChoiceHero == null)
				{
				}
				for (int j = 0; j < listView[i].ChoiceHero.Length; j++)
				{
					if (listView[i].ChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID == heroID)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool IsHeroExist(uint heroID)
		{
			return this.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1, heroID) || this.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2, heroID);
		}

		public bool IsHaveHeroByID(MemberInfo mInfo, uint heroID)
		{
			bool result = false;
			if (mInfo.canUseHero != null)
			{
				int num = mInfo.canUseHero.Length;
				for (int i = 0; i < num; i++)
				{
					if (mInfo.canUseHero[i] == heroID)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool IsAllConfirmHeroByTeam(COM_PLAYERCAMP camp)
		{
			bool result = true;
			ListView<MemberInfo> listView = this[camp];
			if (listView == null)
			{
				return false;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if (!listView[i].isPrepare)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public void GetMemberInfoSelectHeroBattleInfo(MemberInfo mInfo, out uint winPer, out uint playCount, byte gameType)
		{
			winPer = 0u;
			playCount = 0u;
			if (mInfo.selectHeroInfo == null)
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)mInfo.selectHeroInfo.dwNum))
			{
				if (mInfo.selectHeroInfo.astTypeDetail[num].bGameType == gameType)
				{
					playCount = mInfo.selectHeroInfo.astTypeDetail[num].dwWinNum + mInfo.selectHeroInfo.astTypeDetail[num].dwLoseNum;
					winPer = (uint)(mInfo.selectHeroInfo.astTypeDetail[num].dwWinNum * 1f / playCount * 1f * 100f);
					break;
				}
				num++;
			}
		}

		public static bool IsSameMemeber(MemberInfo member, COM_PLAYERCAMP camp, int pos)
		{
			return member != null && member.camp == camp && (ulong)member.dwPosOfCamp == (ulong)((long)pos);
		}
	}
}
