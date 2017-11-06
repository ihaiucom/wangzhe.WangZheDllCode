using Assets.Scripts.GameLogic.GameKernal;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class DynamicChannel : FuncRegion
	{
		public GameObject enablePassEffect;

		public GameObject unablePassEffect;

		public override void Startup()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			bool flag = hostPlayer != null && hostPlayer.PlayerCamp == this.CampType;
			if (this.enablePassEffect)
			{
				this.enablePassEffect.SetActive(flag);
			}
			if (this.unablePassEffect)
			{
				this.unablePassEffect.SetActive(!flag);
			}
			base.Startup();
		}
	}
}
