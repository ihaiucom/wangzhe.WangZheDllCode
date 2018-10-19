using System;

namespace AGE
{
	public sealed class EventCategory : Attribute
	{
		public string category;

		public EventCategory(string _category)
		{
			this.category = _category;
		}
	}
}
