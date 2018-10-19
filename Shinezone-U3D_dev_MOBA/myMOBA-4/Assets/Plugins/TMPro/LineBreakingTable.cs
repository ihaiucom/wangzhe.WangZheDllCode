using System;
using System.Collections.Generic;

namespace TMPro
{
	[Serializable]
	public class LineBreakingTable
	{
		public Dictionary<int, char> leadingCharacters;

		public Dictionary<int, char> followingCharacters;

		public LineBreakingTable()
		{
			this.leadingCharacters = new Dictionary<int, char>();
			this.followingCharacters = new Dictionary<int, char>();
		}
	}
}
