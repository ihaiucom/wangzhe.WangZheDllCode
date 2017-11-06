using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CEquipLineSet
	{
		public enum enHorizontalLineType
		{
			Left,
			Right
		}

		private const int c_rowCount = 12;

		public CanvasGroup[][] m_horizontalLines;

		public CanvasGroup[] m_verticalLines;

		public CEquipLineSet()
		{
			this.m_horizontalLines = new CanvasGroup[12][];
			for (int i = 0; i < 12; i++)
			{
				this.m_horizontalLines[i] = new CanvasGroup[Enum.GetValues(typeof(CEquipLineSet.enHorizontalLineType)).get_Length()];
			}
			this.m_verticalLines = new CanvasGroup[11];
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_horizontalLines.Length; i++)
			{
				for (int j = 0; j < this.m_horizontalLines[i].Length; j++)
				{
					this.m_horizontalLines[i][j] = null;
				}
			}
			for (int k = 0; k < this.m_verticalLines.Length; k++)
			{
				this.m_verticalLines[k] = null;
			}
		}

		public void InitializeHorizontalLine(int row, CEquipLineSet.enHorizontalLineType type, GameObject gameObject)
		{
			CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
			this.m_horizontalLines[row][(int)type] = canvasGroup;
		}

		public void InitializeVerticalLine(int startRow, int endRow, GameObject gameObject)
		{
			if (endRow - startRow != 1)
			{
				return;
			}
			CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
			this.m_verticalLines[startRow] = canvasGroup;
		}

		public void HideAllLines()
		{
			for (int i = 0; i < this.m_horizontalLines.Length; i++)
			{
				for (int j = 0; j < this.m_horizontalLines[i].Length; j++)
				{
					if (this.m_horizontalLines[i][j] != null)
					{
						this.m_horizontalLines[i][j].alpha = 0f;
					}
				}
			}
			for (int k = 0; k < this.m_verticalLines.Length; k++)
			{
				if (this.m_verticalLines[k] != null)
				{
					this.m_verticalLines[k].alpha = 0f;
				}
			}
		}

		public void DisplayHorizontalLine(int row, CEquipLineSet.enHorizontalLineType type)
		{
			if (this.m_horizontalLines[row][(int)type] != null)
			{
				this.m_horizontalLines[row][(int)type].alpha = 1f;
			}
		}

		public void DisplayVerticalLine(int startRow, int endRow)
		{
			if (startRow == endRow)
			{
				return;
			}
			for (int i = startRow; i < endRow; i++)
			{
				if (this.m_verticalLines[i] != null)
				{
					this.m_verticalLines[i].alpha = 1f;
				}
			}
		}
	}
}
