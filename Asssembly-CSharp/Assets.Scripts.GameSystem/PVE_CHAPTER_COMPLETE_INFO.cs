using System;

namespace Assets.Scripts.GameSystem
{
	public class PVE_CHAPTER_COMPLETE_INFO
	{
		public byte bChapterNo;

		public byte bIsGetBonus;

		public byte bLevelNum;

		public PVE_LEVEL_COMPLETE_INFO[] LevelDetailList;

		public PVE_CHAPTER_COMPLETE_INFO()
		{
			this.LevelDetailList = new PVE_LEVEL_COMPLETE_INFO[CAdventureSys.LEVEL_PER_CHAPTER];
			for (int i = 0; i < CAdventureSys.LEVEL_PER_CHAPTER; i++)
			{
				this.LevelDetailList[i] = new PVE_LEVEL_COMPLETE_INFO();
			}
		}
	}
}
