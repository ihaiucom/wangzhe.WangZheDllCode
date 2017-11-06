using Photon;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class OnClickFlashRpc : PunBehaviour
{
	private Material originalMaterial;

	private Color originalColor;

	private bool isFlashing;

	private void OnClick()
	{
		base.photonView.RPC("Flash", PhotonTargets.All, new object[0]);
	}

	[PunRPC, DebuggerHidden]
	private IEnumerator Flash()
	{
		OnClickFlashRpc.<Flash>c__Iterator1 <Flash>c__Iterator = new OnClickFlashRpc.<Flash>c__Iterator1();
		<Flash>c__Iterator.<>f__this = this;
		return <Flash>c__Iterator;
	}
}
