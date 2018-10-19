using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class FriendReserve
	{
		public enum ReserveDataType
		{
			Receive,
			Send,
			Count
		}

		public class Ent
		{
			public ulong ullUid;

			public uint dwLogicWorldId;

			public byte result;

			public string inBattleContent;

			public string name;

			public Ent(ulong ullUid, uint dwLogicWorldId, byte result, string inBattleContent, string name)
			{
				this.ullUid = ullUid;
				this.dwLogicWorldId = dwLogicWorldId;
				this.result = result;
				this.inBattleContent = inBattleContent;
				this.name = name;
			}
		}

		public string Receive_Reserve_Tip;

		public string Reserve_TabShortCut;

		public string Reserve_TabReserve;

		public string Reserve_Success;

		public string Reserve_Failed;

		public string Reserve_Wait4Rsp;

		public ListView<FriendReserve.Ent>[] dataList;

		public List<string> accept_receiveReserve = new List<string>();

		public List<string> sendReserve_accepted = new List<string>();

		public bool BServerEnableReverse
		{
			get;
			set;
		}

		public FriendReserve()
		{
			this.dataList = new ListView<FriendReserve.Ent>[2];
			this.dataList[1] = new ListView<FriendReserve.Ent>();
			this.dataList[0] = new ListView<FriendReserve.Ent>();
		}

		public void CheckShowAcceptReceiveReserve()
		{
			string text = string.Empty;
			for (int i = 0; i < this.accept_receiveReserve.Count; i++)
			{
				string text2 = this.accept_receiveReserve[i];
				if (string.IsNullOrEmpty(text))
				{
					text = text2;
				}
				else
				{
					text = string.Format("{0},{1}", text, text2);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Reserve_Ending_Tips"), text);
				Singleton<CUIManager>.instance.OpenMessageBox(strContent, false);
			}
			this.accept_receiveReserve.Clear();
		}

		public void Clear()
		{
			if (this.dataList != null)
			{
				for (int i = 0; i < this.dataList.Length; i++)
				{
					this.dataList[i].Clear();
				}
			}
			this.accept_receiveReserve.Clear();
			this.sendReserve_accepted.Clear();
		}

		public bool IsReservable(ulong ullUid, uint dwLogicWorldId, FriendReserve.ReserveDataType type)
		{
			FriendReserve.Ent ent = this.Find(ullUid, dwLogicWorldId, type);
			return ent == null;
		}

		public void RemoveReceiveReserveData(ulong ullUid, uint dwLogicWorldId, FriendReserve.ReserveDataType type)
		{
			int index = this.GetIndex(ullUid, dwLogicWorldId, type);
			if (index != -1)
			{
				ListView<FriendReserve.Ent> listView = this.dataList[(int)type];
				listView.RemoveAt(index);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.RECEIVE_RESERVE_DATA_CHANGE);
			}
		}

		public void AddReceiveReservieData(ulong ullUid, uint dwLogicWorldId)
		{
			COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(ullUid, dwLogicWorldId);
			if (gameOrSnsFriend != null)
			{
				string text = UT.Bytes2String(gameOrSnsFriend.szUserName);
				string inBattleContent = string.Format(this.Receive_Reserve_Tip, text);
				FriendReserve.Ent item = new FriendReserve.Ent(ullUid, dwLogicWorldId, 0, inBattleContent, text);
				this.dataList[0].Add(item);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.RECEIVE_RESERVE_DATA_CHANGE);
			}
		}

		public void SetData(ulong ullUid, uint dwLogicWorldId, byte result, FriendReserve.ReserveDataType type)
		{
			FriendReserve.Ent ent = this.Find(ullUid, dwLogicWorldId, type);
			if (ent != null)
			{
				ent.result = result;
			}
			else
			{
				ListView<FriendReserve.Ent> listView = this.dataList[(int)type];
				listView.Add(new FriendReserve.Ent(ullUid, dwLogicWorldId, result, string.Empty, string.Empty));
			}
		}

		public FriendReserve.Ent Find(ulong ullUid, uint dwLogicWorldId, FriendReserve.ReserveDataType type)
		{
			ListView<FriendReserve.Ent> listView = this.dataList[(int)type];
			for (int i = 0; i < listView.Count; i++)
			{
				FriendReserve.Ent ent = listView[i];
				if (ent != null && ent.ullUid == ullUid && ent.dwLogicWorldId == dwLogicWorldId)
				{
					return ent;
				}
			}
			return null;
		}

		public int GetIndex(ulong ullUid, uint dwLogicWorldId, FriendReserve.ReserveDataType type)
		{
			ListView<FriendReserve.Ent> listView = this.dataList[(int)type];
			int result = -1;
			for (int i = 0; i < listView.Count; i++)
			{
				FriendReserve.Ent ent = listView[i];
				if (ent != null && ent.ullUid == ullUid && ent.dwLogicWorldId == dwLogicWorldId)
				{
					result = i;
				}
			}
			return result;
		}
	}
}
