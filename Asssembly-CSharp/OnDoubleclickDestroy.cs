using Photon;
using System;
using UnityEngine;

public class OnDoubleclickDestroy : Photon.MonoBehaviour
{
	private const float ClickDeltaForDoubleclick = 0.2f;

	private float timeOfLastClick;

	private void OnClick()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (Time.time - this.timeOfLastClick < 0.2f)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			this.timeOfLastClick = Time.time;
		}
	}
}
