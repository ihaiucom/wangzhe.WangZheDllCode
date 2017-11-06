using System;

namespace Assets.Scripts.GameSystem
{
	public struct stRcmdEquipListInfo
	{
		public uint CurUseID;

		public stRcmdEquipItem[] ListItem;

		public stRcmdEquipListInfo(uint _curUseId)
		{
			this.CurUseID = _curUseId;
			this.ListItem = new stRcmdEquipItem[3];
			for (int i = 0; i < 3; i++)
			{
				this.ListItem[i] = new stRcmdEquipItem(i);
			}
		}
	}
}
