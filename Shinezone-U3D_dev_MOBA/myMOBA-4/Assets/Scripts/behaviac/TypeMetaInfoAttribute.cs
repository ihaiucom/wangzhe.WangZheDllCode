using System;

namespace behaviac
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
	public class TypeMetaInfoAttribute : Attribute
	{
		private string displayName_;

		private string desc_;

		public string DisplayName
		{
			get
			{
				return this.displayName_;
			}
		}

		public string Description
		{
			get
			{
				return this.desc_;
			}
		}

		public TypeMetaInfoAttribute(string displayName, string description)
		{
			this.displayName_ = displayName;
			this.desc_ = description;
		}

		public TypeMetaInfoAttribute()
		{
		}
	}
}
