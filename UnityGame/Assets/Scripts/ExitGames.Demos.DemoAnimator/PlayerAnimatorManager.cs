using Photon;
using System;
using UnityEngine;

namespace ExitGames.Demos.DemoAnimator
{
	public class PlayerAnimatorManager : Photon.MonoBehaviour
	{
		public float DirectionDampTime = 0.25f;

		private Animator animator;

		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
		}

		private void Update()
		{
			if (!base.photonView.isMine && PhotonNetwork.connected)
			{
				return;
			}
			if (!this.animator)
			{
				return;
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Run") && Input.GetButtonDown("Fire2"))
			{
				this.animator.SetTrigger("Jump");
			}
			float axis = Input.GetAxis("Horizontal");
			float num = Input.GetAxis("Vertical");
			if (num < 0f)
			{
				num = 0f;
			}
			this.animator.SetFloat("Speed", axis * axis + num * num);
			this.animator.SetFloat("Direction", axis, this.DirectionDampTime, Time.deltaTime);
		}
	}
}
