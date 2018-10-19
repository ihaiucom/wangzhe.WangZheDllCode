using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CFriendHeartData
	{
		public class CDHeartData
		{
			public ulong ullUid;

			public uint dwLogicWorldId;

			public bool bCanSend;
		}

		public static uint heartTimer_DoorValue = 1800000u;

		private ListView<CFriendHeartData.CDHeartData> _sendHeartList = new ListView<CFriendHeartData.CDHeartData>();

		public void Clear()
		{
			this._sendHeartList.Clear();
		}

		public void Add(COMDT_ACNT_UNIQ uniq)
		{
			CFriendHeartData.CDHeartData friendData = this.GetFriendData(uniq);
			if (friendData != null)
			{
				this.RemoveCDHeartData(uniq);
			}
			UT.Add2List<CFriendHeartData.CDHeartData>(new CFriendHeartData.CDHeartData
			{
				ullUid = uniq.ullUid,
				dwLogicWorldId = uniq.dwLogicWorldId,
				bCanSend = false
			}, this._sendHeartList);
		}

		public bool BCanSendHeart(COMDT_ACNT_UNIQ uniq)
		{
			int heartDataIndex = this.GetHeartDataIndex(uniq);
			return heartDataIndex == -1 || this._sendHeartList[heartDataIndex].bCanSend;
		}

		private CFriendHeartData.CDHeartData GetFriendData(COMDT_ACNT_UNIQ uniq)
		{
			int heartDataIndex = this.GetHeartDataIndex(uniq);
			if (heartDataIndex == -1)
			{
				return null;
			}
			return this._sendHeartList[heartDataIndex];
		}

		private void RemoveCDHeartData(COMDT_ACNT_UNIQ uniq)
		{
			int heartDataIndex = this.GetHeartDataIndex(uniq);
			if (heartDataIndex != -1)
			{
				this._sendHeartList.RemoveAt(heartDataIndex);
			}
		}

		private int GetHeartDataIndex(COMDT_ACNT_UNIQ uniq)
		{
			if (uniq == null)
			{
				return -1;
			}
			for (int i = 0; i < this._sendHeartList.Count; i++)
			{
				CFriendHeartData.CDHeartData cDHeartData = this._sendHeartList[i];
				if (cDHeartData.ullUid == uniq.ullUid && cDHeartData.dwLogicWorldId == uniq.dwLogicWorldId)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
