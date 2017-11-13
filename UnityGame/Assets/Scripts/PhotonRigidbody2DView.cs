using System;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon Rigidbody 2D View"), RequireComponent(typeof(PhotonView)), RequireComponent(typeof(Rigidbody2D))]
public class PhotonRigidbody2DView : MonoBehaviour, IPunObservable
{
	[SerializeField]
	private bool m_SynchronizeVelocity = true;

	[SerializeField]
	private bool m_SynchronizeAngularVelocity = true;

	private Rigidbody2D m_Body;

	private void Awake()
	{
		this.m_Body = base.GetComponent<Rigidbody2D>();
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
				this.m_Body.set_velocity((Vector2)stream.ReceiveNext());
			}
			if (this.m_SynchronizeAngularVelocity)
			{
				this.m_Body.set_angularVelocity((float)stream.ReceiveNext());
			}
		}
	}
}
