using Photon;
using System;
using UnityEngine;

public class AudioRpc : Photon.MonoBehaviour
{
	public AudioClip marco;

	public AudioClip polo;

	private AudioSource m_Source;

	private void Awake()
	{
		this.m_Source = base.GetComponent<AudioSource>();
	}

	[PunRPC]
	private void Marco()
	{
		if (!base.enabled)
		{
			return;
		}
		Debug.Log("Marco");
		this.m_Source.clip = this.marco;
		this.m_Source.Play();
	}

	[PunRPC]
	private void Polo()
	{
		if (!base.enabled)
		{
			return;
		}
		Debug.Log("Polo");
		this.m_Source.clip = this.polo;
		this.m_Source.Play();
	}

	private void OnApplicationFocus(bool focus)
	{
		base.enabled = focus;
	}
}
