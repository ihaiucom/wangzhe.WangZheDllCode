using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class ReviveRegion : FuncRegion
	{
		public GameObject[] SubRevivePlaces = new GameObject[0];

		[FriendlyName("仅作为出生点")]
		public bool OnlyBirth;
	}
}
