using Assets.Scripts.Sound;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	internal class StarAnimHelper : MonoBehaviour
	{
		public void OnShowWinBegin()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_Word", null);
		}

		public void LittleStarBegin()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_LittleStar", null);
		}

		public void BigStar1Begin()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_bigStar_01", null);
		}

		public void BigStar2Begin()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_bigStar_02", null);
		}

		public void BigStar3Begin()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_bigStar_03", null);
		}

		public void LittleStarMovein()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_slide01", null);
		}
	}
}
