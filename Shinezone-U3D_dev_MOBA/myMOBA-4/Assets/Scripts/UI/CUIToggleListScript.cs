using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIToggleListScript : CUIListScript
	{
		public bool m_isMultiSelected;

		private int m_selected;

		private bool[] m_multiSelected;

		private int m_maxSelectCount = -1;

		private ArrayList m_multiSelectedIdx;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			if (this.m_isMultiSelected)
			{
				this.m_multiSelected = new bool[this.m_elementAmount];
				for (int i = 0; i < this.m_elementAmount; i++)
				{
					this.m_multiSelected[i] = false;
				}
			}
			else
			{
				this.m_selected = -1;
			}
			this.m_multiSelectedIdx = new ArrayList();
			base.Initialize(formScript);
		}

		public void SetSelectLimitCount(int count)
		{
			this.m_maxSelectCount = count;
		}

		public void ClearSelectLimit()
		{
			this.m_maxSelectCount = -1;
		}

		public int GetSelectLimit()
		{
			return this.m_maxSelectCount;
		}

		public int GetMultiSelectCount()
		{
			return this.m_multiSelectedIdx.Count;
		}

		public override void SetElementAmount(int amount, List<Vector2> elementsSize)
		{
			if (this.m_isMultiSelected && (this.m_multiSelected == null || this.m_multiSelected.Length < amount))
			{
				bool[] array = new bool[amount];
				for (int i = 0; i < amount; i++)
				{
					if (this.m_multiSelected != null && i < this.m_multiSelected.Length)
					{
						array[i] = this.m_multiSelected[i];
					}
					else
					{
						array[i] = false;
					}
				}
				this.m_multiSelected = array;
			}
			base.SetElementAmount(amount, elementsSize);
		}

		public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
		{
			if (this.m_isMultiSelected)
			{
				bool flag = this.m_multiSelected[index];
				flag = !flag;
				if (this.MultiLimitCheckStep1(index, flag))
				{
					return;
				}
				this.m_multiSelected[index] = flag;
				CUIListElementScript elemenet = base.GetElemenet(index);
				if (elemenet != null)
				{
					elemenet.ChangeDisplay(flag);
				}
				this.MultiLimitCheckStep2();
				base.DispatchElementSelectChangedEvent();
			}
			else
			{
				if (index == this.m_selected)
				{
					if (this.m_alwaysDispatchSelectedChangeEvent)
					{
						base.DispatchElementSelectChangedEvent();
					}
					return;
				}
				if (this.m_selected >= 0)
				{
					CUIListElementScript elemenet2 = base.GetElemenet(this.m_selected);
					if (elemenet2 != null)
					{
						elemenet2.ChangeDisplay(false);
					}
				}
				this.m_selected = index;
				if (this.m_selected >= 0)
				{
					CUIListElementScript elemenet3 = base.GetElemenet(this.m_selected);
					if (elemenet3 != null)
					{
						elemenet3.ChangeDisplay(true);
					}
				}
				base.DispatchElementSelectChangedEvent();
			}
		}

		public int GetSelected()
		{
			return this.m_selected;
		}

		public bool[] GetMultiSelected()
		{
			return this.m_multiSelected;
		}

		public void SetSelected(int selected)
		{
			this.m_selected = selected;
			for (int i = 0; i < this.m_elementScripts.Count; i++)
			{
				this.m_elementScripts[i].ChangeDisplay(this.IsSelectedIndex(this.m_elementScripts[i].m_index));
			}
		}

		public void SetMultiSelected(int index, bool selected)
		{
			if (index < 0 || index >= this.m_elementAmount)
			{
				return;
			}
			if (this.MultiLimitCheckStep1(index, selected))
			{
				return;
			}
			this.m_multiSelected[index] = selected;
			for (int i = 0; i < this.m_elementScripts.Count; i++)
			{
				this.m_elementScripts[i].ChangeDisplay(this.IsSelectedIndex(this.m_elementScripts[i].m_index));
			}
			base.DispatchElementSelectChangedEvent();
			this.MultiLimitCheckStep2();
		}

		private bool MultiLimitCheckStep1(int index, bool selected)
		{
			if (selected)
			{
				if (this.m_multiSelectedIdx.Count >= this.m_maxSelectCount && this.m_maxSelectCount > 0)
				{
					return true;
				}
				this.m_multiSelectedIdx.Add(index);
			}
			else if (this.m_multiSelectedIdx.Contains(index))
			{
				this.m_multiSelectedIdx.Remove(index);
				if (this.m_multiSelectedIdx.Count < this.m_maxSelectCount && this.m_maxSelectCount > 0)
				{
					this.SetUnCheckToggleEnable(true);
				}
			}
			return false;
		}

		private void MultiLimitCheckStep2()
		{
			if (this.m_multiSelectedIdx.Count >= this.m_maxSelectCount && this.m_maxSelectCount > 0)
			{
				this.SetUnCheckToggleEnable(false);
			}
		}

		private void SetUnCheckToggleEnable(bool enable)
		{
			for (int i = 0; i < this.m_elementAmount; i++)
			{
				if (!this.m_multiSelected[i])
				{
					Toggle component = this.m_elementScripts[i].gameObject.transform.GetComponent<Toggle>();
					if (component)
					{
						this.GetOrgToggleTextColor(component);
						CUIUtility.SetToggleEnable(component, enable, this.m_elementScripts[i].m_orgToggleTextColor);
					}
				}
			}
		}

		private void GetOrgToggleTextColor(Toggle tgl)
		{
			CUIListElementScript component = tgl.GetComponent<CUIListElementScript>();
			if (component != null && !component.m_hasSetTextColor)
			{
				Text componentInChildren = tgl.GetComponentInChildren<Text>();
				if (componentInChildren)
				{
					component.m_orgToggleTextColor = componentInChildren.color;
				}
				component.m_hasSetTextColor = true;
			}
		}

		public override bool IsSelectedIndex(int index)
		{
			return (!this.m_isMultiSelected) ? (index == this.m_selected) : this.m_multiSelected[index];
		}
	}
}
