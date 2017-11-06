using System;
using UnityEngine;

public class IdleRunJump : MonoBehaviour
{
	protected Animator animator;

	public float DirectionDampTime = 0.25f;

	public bool ApplyGravity = true;

	public float SynchronizedMaxSpeed;

	public float TurnSpeedModifier;

	public float SynchronizedTurnSpeed;

	public float SynchronizedSpeedAcceleration;

	protected PhotonView m_PhotonView;

	private PhotonTransformView m_TransformView;

	private float m_SpeedModifier;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.m_PhotonView = base.GetComponent<PhotonView>();
		this.m_TransformView = base.GetComponent<PhotonTransformView>();
		if (this.animator.layerCount >= 2)
		{
			this.animator.SetLayerWeight(1, 1f);
		}
	}

	private void Update()
	{
		if (!this.m_PhotonView.isMine && PhotonNetwork.connected)
		{
			return;
		}
		if (this.animator)
		{
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Run"))
			{
				if (Input.GetButton("Fire1"))
				{
					this.animator.SetBool("Jump", true);
				}
			}
			else
			{
				this.animator.SetBool("Jump", false);
			}
			if (Input.GetButtonDown("Fire2") && this.animator.layerCount >= 2)
			{
				this.animator.SetBool("Hi", !this.animator.GetBool("Hi"));
			}
			float axis = Input.GetAxis("Horizontal");
			float num = Input.GetAxis("Vertical");
			if (num < 0f)
			{
				num = 0f;
			}
			this.animator.SetFloat("Speed", axis * axis + num * num);
			this.animator.SetFloat("Direction", axis, this.DirectionDampTime, Time.deltaTime);
			float @float = this.animator.GetFloat("Direction");
			float num2 = Mathf.Abs(num);
			if (Mathf.Abs(@float) > 0.2f)
			{
				num2 = this.TurnSpeedModifier;
			}
			this.m_SpeedModifier = Mathf.MoveTowards(this.m_SpeedModifier, num2, Time.deltaTime * 25f);
			Vector3 speed = base.transform.forward * this.SynchronizedMaxSpeed * this.m_SpeedModifier;
			float turnSpeed = @float * this.SynchronizedTurnSpeed;
			this.m_TransformView.SetSynchronizedValues(speed, turnSpeed);
		}
	}
}
