using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class COfflineChatIndex
	{
		public ulong ullUid;

		public uint dwLogicWorldId;

		public List<int> indexList;

		public COfflineChatIndex(ulong ullUid, uint dwLogicWorldId)
		{
			this.ullUid = ullUid;
			this.dwLogicWorldId = dwLogicWorldId;
			this.indexList = new List<int>();
		}
	}
}
