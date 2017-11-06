using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
	public enum Team : byte
	{
		none,
		red,
		blue
	}

	public const string TeamPlayerProp = "team";

	public static Dictionary<PunTeams.Team, List<PhotonPlayer>> PlayersPerTeam;

	public void Start()
	{
		PunTeams.PlayersPerTeam = new Dictionary<PunTeams.Team, List<PhotonPlayer>>();
		Array values = Enum.GetValues(typeof(PunTeams.Team));
		using (IEnumerator enumerator = values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.get_Current();
				PunTeams.PlayersPerTeam.set_Item((PunTeams.Team)((byte)current), new List<PhotonPlayer>());
			}
		}
	}

	public void OnDisable()
	{
		PunTeams.PlayersPerTeam = new Dictionary<PunTeams.Team, List<PhotonPlayer>>();
	}

	public void OnJoinedRoom()
	{
		this.UpdateTeams();
	}

	public void OnLeftRoom()
	{
		this.Start();
	}

	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		this.UpdateTeams();
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		this.UpdateTeams();
	}

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		this.UpdateTeams();
	}

	public void UpdateTeams()
	{
		Array values = Enum.GetValues(typeof(PunTeams.Team));
		using (IEnumerator enumerator = values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.get_Current();
				PunTeams.PlayersPerTeam.get_Item((PunTeams.Team)((byte)current)).Clear();
			}
		}
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			PunTeams.Team team = photonPlayer.GetTeam();
			PunTeams.PlayersPerTeam.get_Item(team).Add(photonPlayer);
		}
	}
}
