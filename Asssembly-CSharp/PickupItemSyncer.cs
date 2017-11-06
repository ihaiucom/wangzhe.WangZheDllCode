using Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PickupItemSyncer : Photon.MonoBehaviour
{
	private const float TimeDeltaToIgnore = 0.2f;

	public bool IsWaitingForPickupInit;

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			this.SendPickedUpItems(newPlayer);
		}
	}

	public void OnJoinedRoom()
	{
		Debug.Log(string.Concat(new object[]
		{
			"Joined Room. isMasterClient: ",
			PhotonNetwork.isMasterClient,
			" id: ",
			PhotonNetwork.player.ID
		}));
		this.IsWaitingForPickupInit = !PhotonNetwork.isMasterClient;
		if (PhotonNetwork.playerList.Length >= 2)
		{
			base.Invoke("AskForPickupItemSpawnTimes", 2f);
		}
	}

	public void AskForPickupItemSpawnTimes()
	{
		if (this.IsWaitingForPickupInit)
		{
			if (PhotonNetwork.playerList.Length < 2)
			{
				Debug.Log("Cant ask anyone else for PickupItem spawn times.");
				this.IsWaitingForPickupInit = false;
				return;
			}
			PhotonPlayer next = PhotonNetwork.masterClient.GetNext();
			if (next == null || next.Equals(PhotonNetwork.player))
			{
				next = PhotonNetwork.player.GetNext();
			}
			if (next != null && !next.Equals(PhotonNetwork.player))
			{
				base.photonView.RPC("RequestForPickupItems", next, new object[0]);
			}
			else
			{
				Debug.Log("No player left to ask");
				this.IsWaitingForPickupInit = false;
			}
		}
	}

	[PunRPC, Obsolete("Use RequestForPickupItems(PhotonMessageInfo msgInfo) with corrected typing instead.")]
	public void RequestForPickupTimes(PhotonMessageInfo msgInfo)
	{
		this.RequestForPickupItems(msgInfo);
	}

	[PunRPC]
	public void RequestForPickupItems(PhotonMessageInfo msgInfo)
	{
		if (msgInfo.sender == null)
		{
			Debug.LogError("Unknown player asked for PickupItems");
			return;
		}
		this.SendPickedUpItems(msgInfo.sender);
	}

	private void SendPickedUpItems(PhotonPlayer targetPlayer)
	{
		if (targetPlayer == null)
		{
			Debug.LogWarning("Cant send PickupItem spawn times to unknown targetPlayer.");
			return;
		}
		double time = PhotonNetwork.time;
		double num = time + 0.20000000298023224;
		PickupItem[] array = new PickupItem[PickupItem.DisabledPickupItems.get_Count()];
		PickupItem.DisabledPickupItems.CopyTo(array);
		List<float> list = new List<float>(array.Length * 2);
		for (int i = 0; i < array.Length; i++)
		{
			PickupItem pickupItem = array[i];
			if (pickupItem.SecondsBeforeRespawn <= 0f)
			{
				list.Add((float)pickupItem.ViewID);
				list.Add(0f);
			}
			else
			{
				double num2 = pickupItem.TimeOfRespawn - PhotonNetwork.time;
				if (pickupItem.TimeOfRespawn > num)
				{
					Debug.Log(string.Concat(new object[]
					{
						pickupItem.ViewID,
						" respawn: ",
						pickupItem.TimeOfRespawn,
						" timeUntilRespawn: ",
						num2,
						" (now: ",
						PhotonNetwork.time,
						")"
					}));
					list.Add((float)pickupItem.ViewID);
					list.Add((float)num2);
				}
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"Sent count: ",
			list.get_Count(),
			" now: ",
			time
		}));
		base.photonView.RPC("PickupItemInit", targetPlayer, new object[]
		{
			PhotonNetwork.time,
			list.ToArray()
		});
	}

	[PunRPC]
	public void PickupItemInit(double timeBase, float[] inactivePickupsAndTimes)
	{
		this.IsWaitingForPickupInit = false;
		for (int i = 0; i < inactivePickupsAndTimes.Length / 2; i++)
		{
			int num = i * 2;
			int viewID = (int)inactivePickupsAndTimes[num];
			float num2 = inactivePickupsAndTimes[num + 1];
			PhotonView photonView = PhotonView.Find(viewID);
			PickupItem component = photonView.GetComponent<PickupItem>();
			if (num2 <= 0f)
			{
				component.PickedUp(0f);
			}
			else
			{
				double num3 = (double)num2 + timeBase;
				Debug.Log(string.Concat(new object[]
				{
					photonView.viewID,
					" respawn: ",
					num3,
					" timeUntilRespawnBasedOnTimeBase:",
					num2,
					" SecondsBeforeRespawn: ",
					component.SecondsBeforeRespawn
				}));
				double num4 = num3 - PhotonNetwork.time;
				if (num2 <= 0f)
				{
					num4 = 0.0;
				}
				component.PickedUp((float)num4);
			}
		}
	}
}
