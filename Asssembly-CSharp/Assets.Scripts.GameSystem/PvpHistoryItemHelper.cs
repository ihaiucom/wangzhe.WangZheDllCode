using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class PvpHistoryItemHelper : MonoBehaviour
	{
		public GameObject headObj;

		public GameObject reSesultText;

		public GameObject KDAText;

		public GameObject MatchTypeText;

		public GameObject equipObj;

		public GameObject time;

		public GameObject Mvp;

		public GameObject ShowDetailBtn;

		public GameObject FriendItem;

		public GameObject FiveFriendItem;

		public GameObject FourFriendItem;

		public GameObject ThreeFriendItem;

		public GameObject TwoFriendItem;

		public void SetEuipItems(ref COMDT_PLAYER_FIGHT_DATA playerData, CUIFormScript form)
		{
			if (playerData == null || form == null)
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				COMDT_INGAME_EQUIP_INFO cOMDT_INGAME_EQUIP_INFO = playerData.astEquipDetail[i];
				Image component = this.equipObj.transform.FindChild(string.Format("TianFuIcon{0}", (i + 1).ToString())).GetComponent<Image>();
				if (cOMDT_INGAME_EQUIP_INFO.dwEquipID == 0u || cOMDT_INGAME_EQUIP_INFO == null)
				{
					component.gameObject.CustomSetActive(false);
				}
				else
				{
					component.gameObject.CustomSetActive(true);
					CUICommonSystem.SetEquipIcon((ushort)cOMDT_INGAME_EQUIP_INFO.dwEquipID, component.gameObject, form);
				}
			}
		}
	}
}
