using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnCollideSwitchTeam : MonoBehaviour
{
	public PunTeams.Team TeamToSwitchTo;

	public void OnTriggerEnter(Collider other)
	{
		PhotonView component = other.GetComponent<PhotonView>();
		if (component != null && component.isMine)
		{
			PhotonNetwork.player.SetTeam(this.TeamToSwitchTo);
		}
	}
}
