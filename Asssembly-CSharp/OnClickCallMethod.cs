using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnClickCallMethod : Photon.MonoBehaviour
{
	public GameObject TargetGameObject;

	public string TargetMethod;

	public void OnClick()
	{
		if (this.TargetGameObject == null || string.IsNullOrEmpty(this.TargetMethod))
		{
			Debug.LogWarning(this + " can't call, cause GO or Method are empty.");
			return;
		}
		this.TargetGameObject.SendMessage(this.TargetMethod);
	}
}
