using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CEquipRelationPath
	{
		public bool[][] m_equipNodes;

		public CEquipLineSet[] m_equipLineSets;

		private Dictionary<ushort, Vector2> m_equipPositionDictionary = new Dictionary<ushort, Vector2>();

		public CEquipRelationPath()
		{
			this.m_equipNodes = new bool[3][];
			for (int i = 0; i < this.m_equipNodes.Length; i++)
			{
				this.m_equipNodes[i] = new bool[12];
			}
			this.m_equipLineSets = new CEquipLineSet[2];
			for (int j = 0; j < this.m_equipLineSets.Length; j++)
			{
				this.m_equipLineSets[j] = new CEquipLineSet();
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_equipNodes.Length; i++)
			{
				for (int j = 0; j < this.m_equipNodes[i].Length; j++)
				{
					this.m_equipNodes[i][j] = false;
				}
			}
			for (int k = 0; k < this.m_equipLineSets.Length; k++)
			{
				this.m_equipLineSets[k].Clear();
			}
			this.m_equipPositionDictionary.Clear();
		}

		public void Reset()
		{
			for (int i = 0; i < this.m_equipNodes.Length; i++)
			{
				for (int j = 0; j < this.m_equipNodes[i].Length; j++)
				{
					this.m_equipNodes[i][j] = false;
				}
			}
			for (int k = 0; k < this.m_equipLineSets.Length; k++)
			{
				this.m_equipLineSets[k].HideAllLines();
			}
			this.m_equipPositionDictionary.Clear();
		}

		public void InitializeHorizontalLine(int index, int row, CEquipLineSet.enHorizontalLineType type, GameObject gameObject)
		{
			this.m_equipLineSets[index].InitializeHorizontalLine(row, type, gameObject);
		}

		public void InitializeVerticalLine(int index, int startRow, int endRow, GameObject gameObject)
		{
			this.m_equipLineSets[index].InitializeVerticalLine(startRow, endRow, gameObject);
		}

		public void Display(ushort equipID, List<ushort>[] equipList, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
		{
			this.Reset();
			this.SetEquipPositionDictionary(equipList);
			this.EnableEquipNode(equipID);
			this.EnablePreEquipNode(equipID, equipInfoDictionary);
			this.EnableBackEquipNode(equipID, equipInfoDictionary);
		}

		private void SetEquipPositionDictionary(List<ushort>[] equipList)
		{
			for (int i = 0; i < equipList.Length; i++)
			{
				for (int j = 0; j < equipList[i].Count; j++)
				{
					if (equipList[i][j] > 0 && !this.m_equipPositionDictionary.ContainsKey(equipList[i][j]))
					{
						this.m_equipPositionDictionary.Add(equipList[i][j], new Vector2((float)i, (float)j));
					}
				}
			}
		}

		private void EnableEquipNode(ushort equipID)
		{
			Vector2 zero = Vector2.zero;
			if (this.m_equipPositionDictionary.TryGetValue(equipID, out zero) && zero.x >= 0f && zero.x < (float)this.m_equipNodes.Length && zero.y >= 0f && zero.y < (float)this.m_equipNodes[(int)zero.x].Length)
			{
				this.m_equipNodes[(int)zero.x][(int)zero.y] = true;
			}
		}

		private void EnablePreEquipNode(ushort equipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
		{
			CEquipInfo cEquipInfo = null;
			if (equipInfoDictionary.TryGetValue(equipID, out cEquipInfo) && cEquipInfo.m_resEquipInBattle != null)
			{
				for (int i = 0; i < cEquipInfo.m_resEquipInBattle.PreEquipID.Length; i++)
				{
					if (cEquipInfo.m_resEquipInBattle.PreEquipID[i] > 0)
					{
						this.EnableEquipNode(cEquipInfo.m_resEquipInBattle.PreEquipID[i]);
						this.DisplayEquipLineSet(cEquipInfo.m_resEquipInBattle.PreEquipID[i], equipID);
						this.EnablePreEquipNode(cEquipInfo.m_resEquipInBattle.PreEquipID[i], equipInfoDictionary);
					}
				}
			}
		}

		private void EnableBackEquipNode(ushort equipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
		{
			CEquipInfo cEquipInfo = null;
			if (equipInfoDictionary.TryGetValue(equipID, out cEquipInfo) && cEquipInfo.m_backEquipIDs != null)
			{
				for (int i = 0; i < cEquipInfo.m_backEquipIDs.Count; i++)
				{
					this.EnableEquipNode(cEquipInfo.m_backEquipIDs[i]);
					this.DisplayEquipLineSet(equipID, cEquipInfo.m_backEquipIDs[i]);
					this.EnableBackEquipNode(cEquipInfo.m_backEquipIDs[i], equipInfoDictionary);
				}
			}
		}

		private void DisplayEquipLineSet(ushort equipID1, ushort equipID2)
		{
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			if (this.m_equipPositionDictionary.TryGetValue(equipID1, out zero) && this.m_equipPositionDictionary.TryGetValue(equipID2, out zero2) && Mathf.Abs(zero.x - zero2.x) == 1f)
			{
				Vector2 vector = (zero.x >= zero2.x) ? zero2 : zero;
				Vector2 vector2 = (zero.x >= zero2.x) ? zero : zero2;
				int num = (int)vector.x;
				this.m_equipLineSets[num].DisplayHorizontalLine((int)vector.y, CEquipLineSet.enHorizontalLineType.Left);
				this.m_equipLineSets[num].DisplayHorizontalLine((int)vector2.y, CEquipLineSet.enHorizontalLineType.Right);
				this.m_equipLineSets[num].DisplayVerticalLine((int)Mathf.Min(vector.y, vector2.y), (int)Mathf.Max(vector.y, vector2.y));
			}
		}
	}
}
