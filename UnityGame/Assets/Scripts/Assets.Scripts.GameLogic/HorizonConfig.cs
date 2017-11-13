using System;

namespace Assets.Scripts.GameLogic
{
	public static class HorizonConfig
	{
		public enum HideMark
		{
			Jungle,
			Skill,
			COUNT,
			INVALID
		}

		public enum ShowMark
		{
			Jungle,
			Skill,
			Organ,
			COUNT,
			INVALID
		}

		public static bool[,] RelationMap = new bool[,]
		{
			{
				true,
				false
			},
			{
				true,
				true
			},
			{
				false,
				true
			}
		};
	}
}
