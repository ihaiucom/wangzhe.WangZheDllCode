using System;
using UnityEngine;

namespace ExitGames.UtilityScripts
{
	public static class PlayerRoomIndexingExtensions
	{
		public static int GetRoomIndex(this PhotonPlayer player)
		{
			if (PlayerRoomIndexing.instance == null)
			{
				Debug.LogError("Missing PlayerRoomIndexing Component in Scene");
				return -1;
			}
			return PlayerRoomIndexing.instance.GetRoomIndex(player);
		}
	}
}
