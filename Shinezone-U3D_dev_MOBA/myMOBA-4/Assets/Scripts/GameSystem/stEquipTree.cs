using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public struct stEquipTree
	{
		public ushort m_rootEquipID;

		public uint m_2ndEquipCount;

		public ushort[] m_2ndEquipIDs;

		public uint[] m_3rdEquipCounts;

		public ushort[][] m_3rdEquipIDs;

		public stEquipTree(int _2ndEquipMaxCount, int _3rdEquipMaxCountPer2ndEquip, int backEquipMaxCount)
		{
			this.m_rootEquipID = 0;
			this.m_2ndEquipCount = 0u;
			this.m_2ndEquipIDs = new ushort[_2ndEquipMaxCount];
			this.m_3rdEquipCounts = new uint[_2ndEquipMaxCount];
			this.m_3rdEquipIDs = new ushort[_2ndEquipMaxCount][];
			for (int i = 0; i < _2ndEquipMaxCount; i++)
			{
				this.m_3rdEquipCounts[i] = 0u;
				this.m_3rdEquipIDs[i] = new ushort[_3rdEquipMaxCountPer2ndEquip];
			}
		}

		public void Clear()
		{
			this.m_rootEquipID = 0;
			this.m_2ndEquipCount = 0u;
			for (int i = 0; i < this.m_2ndEquipIDs.Length; i++)
			{
				this.m_2ndEquipIDs[i] = 0;
			}
			for (int j = 0; j < this.m_3rdEquipCounts.Length; j++)
			{
				this.m_3rdEquipCounts[j] = 0u;
			}
			for (int k = 0; k < this.m_3rdEquipIDs.Length; k++)
			{
				for (int l = 0; l < this.m_3rdEquipIDs[k].Length; l++)
				{
					this.m_3rdEquipIDs[k][l] = 0;
				}
			}
		}

		public void Create(ushort rootEquipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
		{
			this.Clear();
			if (rootEquipID == 0)
			{
				return;
			}
			this.m_rootEquipID = rootEquipID;
			CEquipInfo cEquipInfo = null;
			if (equipInfoDictionary.TryGetValue(rootEquipID, out cEquipInfo))
			{
				CEquipInfo cEquipInfo2 = null;
				for (int i = 0; i < cEquipInfo.m_resEquipInBattle.PreEquipID.Length; i++)
				{
					ushort num = cEquipInfo.m_resEquipInBattle.PreEquipID[i];
					if (num > 0 && equipInfoDictionary.TryGetValue(num, out cEquipInfo2) && cEquipInfo2.m_resEquipInBattle.bInvalid == 0 && cEquipInfo2.m_resEquipInBattle.bIsAttachEquip == 0)
					{
						this.m_2ndEquipIDs[(int)((UIntPtr)this.m_2ndEquipCount)] = num;
						CEquipInfo cEquipInfo3 = null;
						for (int j = 0; j < cEquipInfo2.m_resEquipInBattle.PreEquipID.Length; j++)
						{
							ushort num2 = cEquipInfo2.m_resEquipInBattle.PreEquipID[j];
							if (num2 > 0 && equipInfoDictionary.TryGetValue(num2, out cEquipInfo3) && cEquipInfo3.m_resEquipInBattle.bInvalid == 0 && cEquipInfo3.m_resEquipInBattle.bIsAttachEquip == 0)
							{
								this.m_3rdEquipIDs[(int)((UIntPtr)this.m_2ndEquipCount)][(int)((UIntPtr)this.m_3rdEquipCounts[(int)((UIntPtr)this.m_2ndEquipCount)])] = num2;
								this.m_3rdEquipCounts[(int)((UIntPtr)this.m_2ndEquipCount)] += 1u;
							}
						}
						this.m_2ndEquipCount += 1u;
					}
				}
			}
		}
	}
}
