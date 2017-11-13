using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class MapWrapperAdd : MonoBehaviour, IUpdateLogic
	{
		public SoldierRegion CareSoldierRegion;

		public ActorConfig CareObjActorConfig;

		private void Awake()
		{
			if (this.CareSoldierRegion)
			{
			}
		}

		public void UpdateLogic(int delta)
		{
		}
	}
}
