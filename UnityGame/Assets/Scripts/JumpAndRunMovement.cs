using System;
using UnityEngine;

public class JumpAndRunMovement : MonoBehaviour
{
	public float Speed;

	public float JumpForce;

	private Animator m_Animator;

	private Rigidbody2D m_Body;

	private PhotonView m_PhotonView;

	private bool m_IsGrounded;

	private void Awake()
	{
		this.m_Animator = base.GetComponent<Animator>();
		this.m_Body = base.GetComponent<Rigidbody2D>();
		this.m_PhotonView = base.GetComponent<PhotonView>();
	}

	private void Update()
	{
		this.UpdateIsGrounded();
		this.UpdateIsRunning();
		this.UpdateFacingDirection();
	}

	private void FixedUpdate()
	{
		if (!this.m_PhotonView.isMine)
		{
			return;
		}
		this.UpdateMovement();
		this.UpdateJumping();
	}

	private void UpdateFacingDirection()
	{
		if (this.m_Body.get_velocity().x > 0.2f)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else if (this.m_Body.get_velocity().x < -0.2f)
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
	}

	private void UpdateJumping()
	{
		if (Input.GetButton("Jump") && this.m_IsGrounded)
		{
			this.m_Animator.SetTrigger("IsJumping");
			this.m_Body.AddForce(Vector2.up * this.JumpForce);
			this.m_PhotonView.RPC("DoJump", PhotonTargets.Others, new object[0]);
		}
	}

	[PunRPC]
	private void DoJump()
	{
		this.m_Animator.SetTrigger("IsJumping");
	}

	private void UpdateMovement()
	{
		Vector2 velocity = this.m_Body.get_velocity();
		if (Input.GetAxisRaw("Horizontal") > 0.5f)
		{
			velocity.x = this.Speed;
		}
		else if (Input.GetAxisRaw("Horizontal") < -0.5f)
		{
			velocity.x = -this.Speed;
		}
		else
		{
			velocity.x = 0f;
		}
		this.m_Body.set_velocity(velocity);
	}

	private void UpdateIsRunning()
	{
		this.m_Animator.SetBool("IsRunning", Mathf.Abs(this.m_Body.get_velocity().x) > 0.1f);
	}

	private void UpdateIsGrounded()
	{
		Vector2 vector = new Vector2(base.transform.position.x, base.transform.position.y);
		this.m_IsGrounded = (Physics2D.Raycast(vector, -Vector2.up, 0.1f).collider != null);
		this.m_Animator.SetBool("IsGrounded", this.m_IsGrounded);
	}
}
