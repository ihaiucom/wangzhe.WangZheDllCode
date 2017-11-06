using ExitGames.Client.Photon;
using System;
using UnityEngine;

public static class TeamExtensions
{
	public static PunTeams.Team GetTeam(this PhotonPlayer player)
	{
		object obj;
		if (player.CustomProperties.TryGetValue("team", ref obj))
		{
			return (PunTeams.Team)((byte)obj);
		}
		return PunTeams.Team.none;
	}

	public static void SetTeam(this PhotonPlayer player, PunTeams.Team team)
	{
		if (!PhotonNetwork.connectedAndReady)
		{
			Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.connectionStateDetailed + ". Not connectedAndReady.");
			return;
		}
		PunTeams.Team team2 = player.GetTeam();
		if (team2 != team)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("team", (byte)team);
			player.SetCustomProperties(hashtable, null, false);
		}
	}
}
