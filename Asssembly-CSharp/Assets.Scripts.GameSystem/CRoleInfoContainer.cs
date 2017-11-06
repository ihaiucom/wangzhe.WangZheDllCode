using System;

namespace Assets.Scripts.GameSystem
{
	public class CRoleInfoContainer : CContainer
	{
		private ListView<CRoleInfo> m_roleInfoList = new ListView<CRoleInfo>();

		public int GetContainerSize()
		{
			if (this.m_roleInfoList != null)
			{
				return this.m_roleInfoList.Count;
			}
			return 0;
		}

		public CRoleInfo GetRoleInfoByIndex(int index)
		{
			if (this.m_roleInfoList == null || this.m_roleInfoList.Count <= 0 || index >= this.m_roleInfoList.Count)
			{
				return null;
			}
			return this.m_roleInfoList[index];
		}

		public CRoleInfo FindRoleInfoByID(ulong uuID)
		{
			if (this.m_roleInfoList != null)
			{
				for (int i = 0; i < this.m_roleInfoList.Count; i++)
				{
					if (this.m_roleInfoList[i] != null && this.m_roleInfoList[i].playerUllUID == uuID)
					{
						return this.m_roleInfoList[i];
					}
				}
			}
			return null;
		}

		public void AddRoleInfo(CRoleInfo roleInfo)
		{
			this.Add(roleInfo);
		}

		public ulong AddRoleInfoByType(enROLEINFO_TYPE roleType, ulong uuID, int logicWorldID = 0)
		{
			CRoleInfo cRoleInfo = new CRoleInfo(roleType, uuID, logicWorldID);
			if (cRoleInfo != null)
			{
				this.Add(cRoleInfo);
			}
			return uuID;
		}

		public void RemoveRoleInfoByType(enROLEINFO_TYPE roleType)
		{
			if (this.m_roleInfoList == null || this.m_roleInfoList.Count <= 0)
			{
				return;
			}
			int i = 0;
			while (i < this.m_roleInfoList.Count)
			{
				CRoleInfo cRoleInfo = this.m_roleInfoList[i];
				if (cRoleInfo != null && cRoleInfo.m_roleType == roleType)
				{
					this.Remove(cRoleInfo);
				}
				else
				{
					i++;
				}
			}
		}

		public void RemoveRoleInfoByUUID(ulong uuid)
		{
			if (this.m_roleInfoList == null || this.m_roleInfoList.Count <= 0)
			{
				return;
			}
			CRoleInfo cRoleInfo = null;
			for (int i = 0; i < this.m_roleInfoList.Count; i++)
			{
				cRoleInfo = this.m_roleInfoList[i];
				if (cRoleInfo != null && cRoleInfo.playerUllUID == uuid)
				{
					break;
				}
			}
			this.Remove(cRoleInfo);
		}

		private void Add(CRoleInfo roleInfo)
		{
			if (this.m_roleInfoList != null)
			{
				this.m_roleInfoList.Add(roleInfo);
			}
		}

		private void Remove(CRoleInfo roleInfo)
		{
			if (this.m_roleInfoList != null)
			{
				this.m_roleInfoList.Remove(roleInfo);
			}
		}

		public void Clear()
		{
			this.m_roleInfoList.Clear();
		}
	}
}
