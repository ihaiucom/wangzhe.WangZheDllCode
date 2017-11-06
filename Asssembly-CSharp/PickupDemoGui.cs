using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupDemoGui : MonoBehaviour
{
	public bool ShowScores;

	public bool ShowDropButton;

	public bool ShowTeams;

	public float DropOffset = 0.5f;

	public void OnGUI()
	{
		if (!PhotonNetwork.inRoom)
		{
			return;
		}
		if (this.ShowScores)
		{
			GUILayout.Label("Your Score: " + PhotonNetwork.player.GetScore(), new GUILayoutOption[0]);
		}
		if (this.ShowDropButton)
		{
			using (HashSet<PickupItem>.Enumerator enumerator = PickupItem.DisabledPickupItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PickupItem current = enumerator.get_Current();
					if (current.PickupIsMine && current.SecondsBeforeRespawn <= 0f)
					{
						if (GUILayout.Button("Drop " + current.name, new GUILayoutOption[0]))
						{
							current.Drop();
						}
						GameObject gameObject = PhotonNetwork.player.TagObject as GameObject;
						if (gameObject != null && GUILayout.Button("Drop here " + current.name, new GUILayoutOption[0]))
						{
							Vector3 a = Random.get_insideUnitSphere();
							a.y = 0f;
							a = a.normalized;
							Vector3 newPosition = gameObject.transform.position + this.DropOffset * a;
							current.Drop(newPosition);
						}
					}
				}
			}
		}
		if (this.ShowTeams)
		{
			using (Dictionary<PunTeams.Team, List<PhotonPlayer>>.KeyCollection.Enumerator enumerator2 = PunTeams.PlayersPerTeam.get_Keys().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					PunTeams.Team current2 = enumerator2.get_Current();
					GUILayout.Label("Team: " + current2.ToString(), new GUILayoutOption[0]);
					List<PhotonPlayer> list = PunTeams.PlayersPerTeam.get_Item(current2);
					using (List<PhotonPlayer>.Enumerator enumerator3 = list.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							PhotonPlayer current3 = enumerator3.get_Current();
							GUILayout.Label(string.Concat(new object[]
							{
								"  ",
								current3.ToStringFull(),
								" Score: ",
								current3.GetScore()
							}), new GUILayoutOption[0]);
						}
					}
				}
			}
			if (GUILayout.Button("to red", new GUILayoutOption[0]))
			{
				PhotonNetwork.player.SetTeam(PunTeams.Team.red);
			}
			if (GUILayout.Button("to blue", new GUILayoutOption[0]))
			{
				PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
			}
		}
	}
}
