using System;

namespace behaviac
{
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	public class EventMetaInfoAttribute : MethodMetaInfoAttribute
	{
		public override bool IsNamedEvent
		{
			get
			{
				return true;
			}
		}

		public EventMetaInfoAttribute(string displayName, string description) : base(displayName, description)
		{
		}

		public EventMetaInfoAttribute()
		{
		}
	}
}
