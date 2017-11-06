using System;

namespace Assets.Scripts.GameSystem
{
	public class PVE_ADV_COMPLETE_INFO
	{
		public PVE_CHAPTER_COMPLETE_INFO[] ChapterDetailList;

		public PVE_ADV_COMPLETE_INFO()
		{
			this.ChapterDetailList = new PVE_CHAPTER_COMPLETE_INFO[CAdventureSys.CHAPTER_NUM];
			for (int i = 0; i < CAdventureSys.CHAPTER_NUM; i++)
			{
				this.ChapterDetailList[i] = new PVE_CHAPTER_COMPLETE_INFO();
			}
		}
	}
}
