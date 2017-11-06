using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CubeExtra : Photon.MonoBehaviour, IPunObservable
{
	[Range(0.9f, 1.1f)]
	public float Factor = 0.98f;

	private Vector3 latestCorrectPos = Vector3.zero;

	private Vector3 movementVector = Vector3.zero;

	private Vector3 errorVector = Vector3.zero;

	private double lastTime;

	public void Awake()
	{
		this.latestCorrectPos = base.transform.position;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 localPosition = base.transform.localPosition;
			stream.Serialize(ref localPosition);
		}
		else
		{
			Vector3 zero = Vector3.zero;
			stream.Serialize(ref zero);
			double num = info.timestamp - this.lastTime;
			this.lastTime = info.timestamp;
			this.movementVector = (zero - this.latestCorrectPos) / (float)num;
			this.errorVector = (zero - base.transform.localPosition) / (float)num;
			this.latestCorrectPos = zero;
		}
	}

	public void Update()
	{
		if (base.photonView.isMine)
		{
			return;
		}
		base.transform.localPosition += (this.movementVector + this.errorVector) * this.Factor * Time.deltaTime;
	}
}
