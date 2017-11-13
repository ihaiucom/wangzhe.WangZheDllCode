using Photon;
using System;
using UnityEngine;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
	private ThirdPersonCamera cameraScript;

	private ThirdPersonController controllerScript;

	private bool firstTake;

	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	private void OnEnable()
	{
		this.firstTake = true;
	}

	private void Awake()
	{
		this.cameraScript = base.GetComponent<ThirdPersonCamera>();
		this.controllerScript = base.GetComponent<ThirdPersonController>();
		if (base.photonView.isMine)
		{
			this.cameraScript.enabled = true;
			this.controllerScript.enabled = true;
		}
		else
		{
			this.cameraScript.enabled = false;
			this.controllerScript.enabled = true;
			this.controllerScript.isControllable = false;
		}
		base.gameObject.name = base.gameObject.name + base.photonView.viewID;
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext((int)this.controllerScript._characterState);
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
		}
		else
		{
			this.controllerScript._characterState = (CharacterState)((int)stream.ReceiveNext());
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			if (this.firstTake)
			{
				this.firstTake = false;
				base.transform.position = this.correctPlayerPos;
				base.transform.rotation = this.correctPlayerRot;
			}
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * 5f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * 5f);
		}
	}
}
