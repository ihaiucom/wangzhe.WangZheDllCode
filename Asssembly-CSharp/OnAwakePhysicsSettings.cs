using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakePhysicsSettings : Photon.MonoBehaviour
{
	public void Awake()
	{
		if (!base.photonView.isMine)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.set_isKinematic(true);
			}
			else
			{
				Rigidbody2D component2 = base.GetComponent<Rigidbody2D>();
				if (component2 != null)
				{
					component2.set_isKinematic(true);
				}
			}
		}
	}
}
