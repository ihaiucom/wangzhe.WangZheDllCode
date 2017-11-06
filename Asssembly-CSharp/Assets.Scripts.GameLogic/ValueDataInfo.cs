using ResData;
using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	public class ValueDataInfo
	{
		private RES_FUNCEFT_TYPE _type;

		private CrypticInt32 _BaseValue;

		private CrypticInt32 _GrowValue;

		private CrypticInt32 _AddValue;

		private CrypticInt32 _DecValue;

		private CrypticInt32 _AddRatio;

		private CrypticInt32 _DecRatio;

		private CrypticInt32 _AddValueOffRatio;

		private CrypticInt32 _MaxLimitValue;

		private CrypticInt32 _MinLimitValue;

		private CrypticInt32 _DynamicId = 0;

		private CrypticInt32 _TotalEftRatio = 0;

		private CrypticInt32 _TotalEftRatioByMgc = 0;

		private CrypticInt32 _TotalOldEftRatioByMgc = 0;

		private CrypticInt32 _TotalEftValueByMgc = 0;

		private CrypticInt32 _TotalAddValueByDefence;

		private CrypticInt32 _ConvertRatioByDefence;

		private CrypticInt32 _TotalAddvalueByExtraHP;

		private CrypticInt32 _ConvertRatioByExtraHP;

		private ValueCalculator Calculator;

		public event ValueChangeDelegate ChangeEvent
		{
			[MethodImpl(32)]
			add
			{
				this.ChangeEvent = (ValueChangeDelegate)Delegate.Combine(this.ChangeEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.ChangeEvent = (ValueChangeDelegate)Delegate.Remove(this.ChangeEvent, value);
			}
		}

		public RES_FUNCEFT_TYPE Type
		{
			get
			{
				return this._type;
			}
		}

		public int dynamicId
		{
			get
			{
				return this._DynamicId;
			}
			set
			{
				this._DynamicId = value;
			}
		}

		public int baseValue
		{
			get
			{
				return this._BaseValue;
			}
			set
			{
				this._BaseValue = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int growValue
		{
			get
			{
				return this._GrowValue;
			}
			set
			{
				this._GrowValue = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int addValue
		{
			get
			{
				return this._AddValue;
			}
			set
			{
				this._AddValue = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int decValue
		{
			get
			{
				return this._DecValue;
			}
			set
			{
				this._DecValue = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int addRatio
		{
			get
			{
				return this._AddRatio;
			}
			set
			{
				this._AddRatio = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int decRatio
		{
			get
			{
				return this._DecRatio;
			}
			set
			{
				this._DecRatio = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int addValueOffRatio
		{
			get
			{
				return this._AddValueOffRatio;
			}
			set
			{
				this._AddValueOffRatio = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int maxLimitValue
		{
			get
			{
				return this._MaxLimitValue;
			}
			set
			{
				this._MaxLimitValue = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int minLimitValue
		{
			get
			{
				return this._MinLimitValue;
			}
			set
			{
				this._MinLimitValue = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int extraPropertyValue
		{
			get
			{
				return this.totalValue - this.basePropertyValue;
			}
		}

		public int basePropertyValue
		{
			get
			{
				int result;
				if (this.Calculator != null)
				{
					result = this.Calculator(this, ValueDataType.TYPE_BASE);
				}
				else
				{
					result = this.baseValue;
				}
				return result;
			}
		}

		public int totalValue
		{
			get
			{
				int num;
				if (this.Calculator != null)
				{
					num = this.Calculator(this, ValueDataType.TYPE_TOTAL);
				}
				else
				{
					long num2 = (long)(this.baseValue + this.addValue - this.decValue) * (long)(10000 + this.addRatio - this.decRatio) / 10000L + (long)this.addValueOffRatio;
					num = (int)num2;
				}
				num = num * (10000 + this.totalEftRatio) / 10000;
				if (this._type != RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD && this._type != RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE)
				{
					num = ((num > 0) ? num : 0);
				}
				if (this.maxLimitValue > 0)
				{
					num = ((num > this.maxLimitValue) ? this.maxLimitValue : num);
				}
				if (this.minLimitValue != 0)
				{
					num = ((num < this.minLimitValue) ? this.minLimitValue : num);
				}
				return num;
			}
		}

		public int totalEftRatio
		{
			get
			{
				return this._TotalEftRatio;
			}
			set
			{
				this._TotalEftRatio = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int totalEftRatioByMgc
		{
			get
			{
				return this._TotalEftRatioByMgc;
			}
			set
			{
				this.totalOldEftRatioByMgc = this._TotalEftRatioByMgc;
				this._TotalEftRatioByMgc = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int totalEftValueByMgc
		{
			get
			{
				return this._TotalEftValueByMgc;
			}
			set
			{
				this._TotalEftValueByMgc = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int totalOldEftRatioByMgc
		{
			get
			{
				return this._TotalOldEftRatioByMgc;
			}
			set
			{
				this._TotalOldEftRatioByMgc = value;
			}
		}

		public int convertRatioByDefence
		{
			get
			{
				return this._ConvertRatioByDefence;
			}
			set
			{
				this._ConvertRatioByDefence = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int totalAddValueByDefence
		{
			get
			{
				return this._TotalAddValueByDefence;
			}
			set
			{
				this._TotalAddValueByDefence = value;
			}
		}

		public int convertRatioByExtraHP
		{
			get
			{
				return this._ConvertRatioByExtraHP;
			}
			set
			{
				this._ConvertRatioByExtraHP = value;
				if (this.ChangeEvent != null)
				{
					this.ChangeEvent();
				}
			}
		}

		public int totalAddValueByExtraHP
		{
			get
			{
				return this._TotalAddvalueByExtraHP;
			}
			set
			{
				this._TotalAddvalueByExtraHP = value;
			}
		}

		public ValueDataInfo(RES_FUNCEFT_TYPE type, int nValue, int nGrow, ValueCalculator calc, int dynamicCfg = 0, int nMaxLimitValue = 0, int nMinLimitValue = 0)
		{
			this._type = type;
			this.baseValue = nValue;
			this.growValue = nGrow;
			this.addValue = 0;
			this.decValue = 0;
			this.addRatio = 0;
			this.decRatio = 0;
			this.dynamicId = dynamicCfg;
			this.Calculator = calc;
			this.maxLimitValue = nMaxLimitValue;
			this._TotalEftRatio = 0;
			this._TotalEftRatioByMgc = 0;
			this._TotalEftValueByMgc = 0;
			this.minLimitValue = nMinLimitValue;
		}

		public static void ChangeValueData(ref ValueDataInfo valueInfo, RES_VALUE_TYPE type, int val, bool bOffRatio)
		{
			if (type == RES_VALUE_TYPE.TYPE_VALUE)
			{
				if (bOffRatio)
				{
					valueInfo.addValueOffRatio += val;
				}
				else
				{
					valueInfo.addValue += val;
				}
			}
			else if (type == RES_VALUE_TYPE.TYPE_PERCENT)
			{
				valueInfo.addRatio += val;
			}
		}

		public static ValueDataInfo operator +(ValueDataInfo lhs, int rhs)
		{
			lhs.addValue += rhs;
			return lhs;
		}

		public static ValueDataInfo operator -(ValueDataInfo lhs, int rhs)
		{
			lhs.addValue -= rhs;
			return lhs;
		}

		public static ValueDataInfo operator <<(ValueDataInfo lhs, int rhs)
		{
			lhs.addRatio += rhs;
			return lhs;
		}

		public static ValueDataInfo operator >>(ValueDataInfo lhs, int rhs)
		{
			lhs.addRatio -= rhs;
			return lhs;
		}
	}
}
