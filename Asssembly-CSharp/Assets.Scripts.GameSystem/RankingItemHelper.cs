using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class RankingItemHelper : MonoBehaviour
	{
		public GameObject RankingNumText;

		public GameObject PvpRankingIcon;

		public GameObject AddFriend;

		public GameObject No1;

		public GameObject No2;

		public GameObject No3;

		public GameObject Selected;

		public GameObject VipIcon;

		public GameObject NoRankingText;

		public GameObject RankingUpDownIcon;

		public GameObject HeadIcon;

		public GameObject No1BG;

		public GameObject No1IconFrame;

		public GameObject LadderGo;

		public GameObject HeadIconFrame;

		public GameObject LadderXing;

		public GameObject QqVip;

		public GameObject WxIcon;

		public GameObject QqIcon;

		public GameObject SendCoin;

		public GameObject Online;

		public GameObject GuestIcon;

		public GameObject FindBtn;

		public GameObject AntiDisturbBits;

		public void OnHideAnimationEnd()
		{
			Singleton<RankingSystem>.instance.OnHideAnimationEnd();
		}

		public void OnChatHideAnimationEnd()
		{
			Singleton<CChatController>.instance.OnClosingAnimEnd();
		}

		public void ShowSendButton(bool bEnable)
		{
			if (this.SendCoin == null)
			{
				return;
			}
			Button component = this.SendCoin.GetComponent<Button>();
			if (component == null)
			{
				return;
			}
			component.gameObject.CustomSetActive(true);
			CUICommonSystem.SetButtonEnableWithShader(component, bEnable, true);
		}
	}
}
