using ExitGames.UtilityScripts;
using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MaterialPerOwner : Photon.MonoBehaviour
{
	private int assignedColorForUserId;

	private Renderer m_Renderer;

	private void Start()
	{
		this.m_Renderer = base.GetComponent<Renderer>();
	}

	private void Update()
	{
		if (base.photonView.ownerId != this.assignedColorForUserId)
		{
			int num = Array.IndexOf<int>(PlayerRoomIndexing.instance.PlayerIds, base.photonView.ownerId);
			this.m_Renderer.material.color = Object.FindObjectOfType<ColorPerPlayer>().Colors[num];
			this.assignedColorForUserId = base.photonView.ownerId;
		}
	}
}
