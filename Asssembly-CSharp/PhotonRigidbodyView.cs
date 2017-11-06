using System;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon Rigidbody View"), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PhotonView))]
public class PhotonRigidbodyView : MonoBehaviour, IPunObservable
{
	[SerializeField]
	private bool m_SynchronizeVelocity = true;

	[SerializeField]
	private bool m_SynchronizeAngularVelocity = true;

	private Rigidbody m_Body;

	private void Awake()
	{
		this.m_Body = base.GetComponent<Rigidbody>();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (this.m_SynchronizeVelocity)
			{
				stream.SendNext(this.m_Body.get_velocity());
			}
			if (this.m_SynchronizeAngularVelocity)
			{
				stream.SendNext(this.m_Body.get_angularVelocity());
			}
		}
		else
		{
			if (this.m_SynchronizeVelocity)
			{
				this.m_Body.set_velocity((Vector3)stream.ReceiveNext());
			}
			if (this.m_SynchronizeAngularVelocity)
			{
				this.m_Body.set_angularVelocity((Vector3)stream.ReceiveNext());
			}
		}
	}
}
