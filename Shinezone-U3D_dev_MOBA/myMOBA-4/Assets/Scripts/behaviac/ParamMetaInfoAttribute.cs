using System;

namespace behaviac
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public class ParamMetaInfoAttribute : TypeMetaInfoAttribute
	{
		private string defaultValue_;

		private float rangeMin_ = -3.40282347E+38f;

		private float rangeMax_ = 3.40282347E+38f;

		public string DefaultValue
		{
			get
			{
				return this.defaultValue_;
			}
		}

		public float RangeMin
		{
			get
			{
				return this.rangeMin_;
			}
		}

		public float RangeMax
		{
			get
			{
				return this.rangeMax_;
			}
		}

		public ParamMetaInfoAttribute()
		{
		}

		public ParamMetaInfoAttribute(string displayName, string description, string defaultValue) : base(displayName, description)
		{
			this.defaultValue_ = defaultValue;
			this.rangeMin_ = -3.40282347E+38f;
			this.rangeMax_ = 3.40282347E+38f;
		}

		public ParamMetaInfoAttribute(string displayName, string description, string defaultValue, float rangeMin, float rangeMax) : base(displayName, description)
		{
			this.defaultValue_ = defaultValue;
			this.rangeMin_ = rangeMin;
			this.rangeMax_ = rangeMax;
		}
	}
}
