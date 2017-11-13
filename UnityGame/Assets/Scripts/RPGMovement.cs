using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RPGMovement : MonoBehaviour
{
	public float ForwardSpeed;

	public float BackwardSpeed;

	public float StrafeSpeed;

	public float RotateSpeed;

	private CharacterController m_CharacterController;

	private Vector3 m_LastPosition;

	private Animator m_Animator;

	private PhotonView m_PhotonView;

	private PhotonTransformView m_TransformView;

	private float m_AnimatorSpeed;

	private Vector3 m_CurrentMovement;

	private float m_CurrentTurnSpeed;

	private void Start()
	{
		this.m_CharacterController = base.GetComponent<CharacterController>();
		this.m_Animator = base.GetComponent<Animator>();
		this.m_PhotonView = base.GetComponent<PhotonView>();
		this.m_TransformView = base.GetComponent<PhotonTransformView>();
	}

	private void Update()
	{
		if (this.m_PhotonView.isMine)
		{
			this.ResetSpeedValues();
			this.UpdateRotateMovement();
			this.UpdateForwardMovement();
			this.UpdateBackwardMovement();
			this.UpdateStrafeMovement();
			this.MoveCharacterController();
			this.ApplyGravityToCharacterController();
			this.ApplySynchronizedValues();
		}
		this.UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		Vector3 vector = base.transform.position - this.m_LastPosition;
		float num = Vector3.Dot(vector.normalized, base.transform.forward);
		float num2 = Vector3.Dot(vector.normalized, base.transform.right);
		if (Mathf.Abs(num) < 0.2f)
		{
			num = 0f;
		}
		if (num > 0.6f)
		{
			num = 1f;
			num2 = 0f;
		}
		if (num >= 0f && Mathf.Abs(num2) > 0.7f)
		{
			num = 1f;
		}
		this.m_AnimatorSpeed = Mathf.MoveTowards(this.m_AnimatorSpeed, num, Time.deltaTime * 5f);
		this.m_Animator.SetFloat("Speed", this.m_AnimatorSpeed);
		this.m_Animator.SetFloat("Direction", num2);
		this.m_LastPosition = base.transform.position;
	}

	private void ResetSpeedValues()
	{
		this.m_CurrentMovement = Vector3.zero;
		this.m_CurrentTurnSpeed = 0f;
	}

	private void ApplySynchronizedValues()
	{
		this.m_TransformView.SetSynchronizedValues(this.m_CurrentMovement, this.m_CurrentTurnSpeed);
	}

	private void ApplyGravityToCharacterController()
	{
		this.m_CharacterController.Move(base.transform.up * Time.deltaTime * -9.81f);
	}

	private void MoveCharacterController()
	{
		this.m_CharacterController.Move(this.m_CurrentMovement * Time.deltaTime);
	}

	private void UpdateForwardMovement()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetAxisRaw("Vertical") > 0.1f)
		{
			this.m_CurrentMovement = base.transform.forward * this.ForwardSpeed;
		}
	}

	private void UpdateBackwardMovement()
	{
		if (Input.GetKey(KeyCode.S) || Input.GetAxisRaw("Vertical") < -0.1f)
		{
			this.m_CurrentMovement = -base.transform.forward * this.BackwardSpeed;
		}
	}

	private void UpdateStrafeMovement()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			this.m_CurrentMovement = -base.transform.right * this.StrafeSpeed;
		}
		if (Input.GetKey(KeyCode.E))
		{
			this.m_CurrentMovement = base.transform.right * this.StrafeSpeed;
		}
	}

	private void UpdateRotateMovement()
	{
		if (Input.GetKey(KeyCode.A) || Input.GetAxisRaw("Horizontal") < -0.1f)
		{
			this.m_CurrentTurnSpeed = -this.RotateSpeed;
			base.transform.Rotate(0f, -this.RotateSpeed * Time.deltaTime, 0f);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0.1f)
		{
			this.m_CurrentTurnSpeed = this.RotateSpeed;
			base.transform.Rotate(0f, this.RotateSpeed * Time.deltaTime, 0f);
		}
	}
}
