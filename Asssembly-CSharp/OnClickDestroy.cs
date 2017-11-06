using Photon;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
	public bool DestroyByRpc;

	public void OnClick()
	{
		if (!this.DestroyByRpc)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered, new object[0]);
		}
	}

	[PunRPC, DebuggerHidden]
	public IEnumerator DestroyRpc()
	{
		OnClickDestroy.<DestroyRpc>c__Iterator7 <DestroyRpc>c__Iterator = new OnClickDestroy.<DestroyRpc>c__Iterator7();
		<DestroyRpc>c__Iterator.<>f__this = this;
		return <DestroyRpc>c__Iterator;
	}
}
