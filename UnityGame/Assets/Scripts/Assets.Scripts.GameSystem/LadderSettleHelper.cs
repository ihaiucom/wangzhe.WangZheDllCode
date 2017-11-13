using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class LadderSettleHelper : MonoBehaviour
	{
		public void OnXingUpAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderXingUpOver();
		}

		public void OnXingDownAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderXingDownOver();
		}

		public void OnLevelUpStartAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderLevelUpStartOver();
		}

		public void OnLevelUpEndAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderLevelUpEndOver();
		}

		public void OnLevelDownStartAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderLevelDownStartOver();
		}

		public void OnLevelDownEndAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderLevelDownEndOver();
		}

		public void OnShowInAnimationOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderShowInOver();
		}

		public void OnWangZheXingAnimationStartOver()
		{
		}

		public void OnWangZheXingAnimationEndOver()
		{
			Singleton<SettlementSystem>.instance.OnLadderWangZheXingEndOver();
		}
	}
}
