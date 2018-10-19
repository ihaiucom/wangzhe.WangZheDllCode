using System;
using UnityEngine;

namespace Pathfinding
{
	public class AstarEnumFlagAttribute : PropertyAttribute
	{
		public string enumName;

		public AstarEnumFlagAttribute()
		{
		}

		public AstarEnumFlagAttribute(string name)
		{
			this.enumName = name;
		}
	}
}
