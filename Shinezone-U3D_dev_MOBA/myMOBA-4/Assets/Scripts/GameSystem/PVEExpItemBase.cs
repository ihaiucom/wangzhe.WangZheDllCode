using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal abstract class PVEExpItemBase
	{
		protected GameObject m_Root;

		protected Text m_NameText;

		protected Text m_LevelTxt;

		protected Text m_ExpTxt;

		protected Image m_ExpBar1;

		protected Image animaBar;

		public uint m_exp;

		public int m_level;

		protected string m_Name;

		protected uint ExpToAdd;

		protected uint m_maxExp;

		private float TweenDstVal;

		public int Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
				this.m_maxExp = this.calcMaxExp();
			}
		}

		protected abstract uint calcMaxExp();

		public virtual void addExp(uint addVal)
		{
			this.animaBar = this.m_ExpBar1;
			this.m_maxExp = this.calcMaxExp();
			this.ExpToAdd = addVal;
			this.SetUI();
			if (this.ExpToAdd + this.m_exp > this.m_maxExp)
			{
				this.TweenExpBar(this.m_maxExp, true);
			}
			else if (this.ExpToAdd > 0u)
			{
				this.TweenExpBar(this.m_exp + this.ExpToAdd, false);
			}
		}

		protected virtual void SetUI()
		{
			this.m_NameText.text = this.m_Name;
			this.m_ExpTxt.text = this.getExpString(this.ExpToAdd);
			this.m_LevelTxt.text = string.Format("Lv{0}", this.m_level);
			this.m_ExpBar1.CustomFillAmount(this.m_exp / this.m_maxExp);
		}

		protected string getExpString(uint exp)
		{
			return string.Format("+{0}", exp);
		}

		protected void TweenExpBar(float dst, bool bUpdate)
		{
			this.TweenDstVal = dst;
			this.ExpToAdd -= this.m_maxExp - this.m_exp;
			LeanTween.value(this.m_Root, new Action<float>(this.TweenUpdate), this.m_exp, dst, (!bUpdate) ? 1.6f : 1f).setEase(LeanTweenType.linear);
		}

		protected void TweenUpdate(float val)
		{
			this.m_exp = (uint)val;
			this.animaBar.CustomFillAmount(val / this.m_maxExp);
			if (this.TweenDstVal == this.m_exp)
			{
				this.TweenEnd(val);
			}
		}

		protected virtual void TweenEnd(float val)
		{
			if (this.m_maxExp == this.m_exp)
			{
				this.Level++;
				this.m_exp = 0u;
				this.m_ExpBar1.CustomFillAmount(0f);
				this.m_LevelTxt.text = string.Format("Lv{0}", this.m_level);
				if (this.ExpToAdd + this.m_exp > this.m_maxExp)
				{
					this.TweenExpBar(this.m_maxExp, true);
				}
				else if (this.ExpToAdd > 0u)
				{
					this.TweenExpBar(this.m_exp + this.ExpToAdd, true);
				}
			}
		}
	}
}
