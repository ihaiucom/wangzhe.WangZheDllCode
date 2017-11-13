using Assets.Scripts.Sound;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	internal class RewardAnimHelper : MonoBehaviour
	{
		public void TreasureBoxShaking()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_box", null);
		}

		public void EndPageSlideIn()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_slide02", null);
		}

		public void EndBasicAward()
		{
			Singleton<PVESettleSys>.instance.OnAwardDisplayEnd();
		}
	}
}
