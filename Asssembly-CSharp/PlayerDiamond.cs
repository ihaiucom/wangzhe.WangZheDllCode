using System;
using UnityEngine;

public class PlayerDiamond : MonoBehaviour
{
	public Transform HeadTransform;

	public float HeightOffset = 0.5f;

	private PhotonView m_PhotonView;

	private Renderer m_DiamondRenderer;

	private float m_Rotation;

	private float m_Height;

	private PhotonView PhotonView
	{
		get
		{
			if (this.m_PhotonView == null)
			{
				this.m_PhotonView = base.transform.parent.GetComponent<PhotonView>();
			}
			return this.m_PhotonView;
		}
	}

	private Renderer DiamondRenderer
	{
		get
		{
			if (this.m_DiamondRenderer == null)
			{
				this.m_DiamondRenderer = base.GetComponentInChildren<Renderer>();
			}
			return this.m_DiamondRenderer;
		}
	}

	private void Start()
	{
		this.m_Height = this.HeightOffset;
		if (this.HeadTransform != null)
		{
			this.m_Height += this.HeadTransform.position.y;
		}
	}

	private void Update()
	{
		this.UpdateDiamondPosition();
		this.UpdateDiamondRotation();
		this.UpdateDiamondVisibility();
	}

	private void UpdateDiamondPosition()
	{
		Vector3 to = Vector3.zero;
		if (this.HeadTransform != null)
		{
			to = this.HeadTransform.position;
		}
		to.y = this.m_Height;
		if (!float.IsNaN(to.x) && !float.IsNaN(to.z))
		{
			base.transform.position = Vector3.Lerp(base.transform.position, to, Time.deltaTime * 10f);
		}
	}

	private void UpdateDiamondRotation()
	{
		this.m_Rotation += Time.deltaTime * 180f;
		this.m_Rotation %= 360f;
		base.transform.rotation = Quaternion.Euler(0f, this.m_Rotation, 0f);
	}

	private void UpdateDiamondVisibility()
	{
		this.DiamondRenderer.enabled = true;
		if (this.PhotonView == null || !this.PhotonView.isMine)
		{
			this.DiamondRenderer.enabled = false;
		}
	}
}
