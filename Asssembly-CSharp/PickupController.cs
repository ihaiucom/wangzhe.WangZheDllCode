using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PickupController : MonoBehaviour, IPunObservable
{
	public AnimationClip idleAnimation;

	public AnimationClip walkAnimation;

	public AnimationClip runAnimation;

	public AnimationClip jumpPoseAnimation;

	public float walkMaxAnimationSpeed = 0.75f;

	public float trotMaxAnimationSpeed = 1f;

	public float runMaxAnimationSpeed = 1f;

	public float jumpAnimationSpeed = 1.15f;

	public float landAnimationSpeed = 1f;

	private Animation _animation;

	public PickupCharacterState _characterState;

	public float walkSpeed = 2f;

	public float trotSpeed = 4f;

	public float runSpeed = 6f;

	public float inAirControlAcceleration = 3f;

	public float jumpHeight = 0.5f;

	public float gravity = 20f;

	public float speedSmoothing = 10f;

	public float rotateSpeed = 500f;

	public float trotAfterSeconds = 3f;

	public bool canJump;

	private float jumpRepeatTime = 0.05f;

	private float jumpTimeout = 0.15f;

	private float groundedTimeout = 0.25f;

	private float lockCameraTimer;

	private Vector3 moveDirection = Vector3.zero;

	private float verticalSpeed;

	private float moveSpeed;

	private CollisionFlags collisionFlags;

	private bool jumping;

	private bool jumpingReachedApex;

	private bool movingBack;

	private bool isMoving;

	private float walkTimeStart;

	private float lastJumpButtonTime = -10f;

	private float lastJumpTime = -1f;

	private Vector3 inAirVelocity = Vector3.zero;

	private float lastGroundedTime;

	private Vector3 velocity = Vector3.zero;

	private Vector3 lastPos;

	private Vector3 remotePosition;

	public bool isControllable;

	public bool DoRotate = true;

	public float RemoteSmoothing = 5f;

	public bool AssignAsTagObject = true;

	private void Awake()
	{
		PhotonView component = base.gameObject.GetComponent<PhotonView>();
		if (component != null)
		{
			this.isControllable = component.isMine;
			if (this.AssignAsTagObject)
			{
				component.owner.TagObject = base.gameObject;
			}
			if (!this.DoRotate && component.ObservedComponents != null)
			{
				for (int i = 0; i < component.ObservedComponents.get_Count(); i++)
				{
					if (component.ObservedComponents.get_Item(i) is Transform)
					{
						component.onSerializeTransformOption = OnSerializeTransform.OnlyPosition;
						break;
					}
				}
			}
		}
		this.moveDirection = base.transform.TransformDirection(Vector3.forward);
		this._animation = base.GetComponent<Animation>();
		if (!this._animation)
		{
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		}
		if (!this.idleAnimation)
		{
			this._animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if (!this.walkAnimation)
		{
			this._animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if (!this.runAnimation)
		{
			this._animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!this.jumpPoseAnimation && this.canJump)
		{
			this._animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}

	private void Update()
	{
		if (this.isControllable)
		{
			if (Input.GetButtonDown("Jump"))
			{
				this.lastJumpButtonTime = Time.time;
			}
			this.UpdateSmoothedMovementDirection();
			this.ApplyGravity();
			this.ApplyJumping();
			Vector3 vector = this.moveDirection * this.moveSpeed + new Vector3(0f, this.verticalSpeed, 0f) + this.inAirVelocity;
			vector *= Time.deltaTime;
			CharacterController component = base.GetComponent<CharacterController>();
			this.collisionFlags = component.Move(vector);
		}
		if (this.remotePosition != Vector3.zero)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.remotePosition, Time.deltaTime * this.RemoteSmoothing);
		}
		this.velocity = (base.transform.position - this.lastPos) * 25f;
		if (this._animation)
		{
			if (this._characterState == PickupCharacterState.Jumping)
			{
				if (!this.jumpingReachedApex)
				{
					this._animation[this.jumpPoseAnimation.name].speed = this.jumpAnimationSpeed;
					this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					this._animation.CrossFade(this.jumpPoseAnimation.name);
				}
				else
				{
					this._animation[this.jumpPoseAnimation.name].speed = -this.landAnimationSpeed;
					this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					this._animation.CrossFade(this.jumpPoseAnimation.name);
				}
			}
			else
			{
				if (this._characterState == PickupCharacterState.Idle)
				{
					this._animation.CrossFade(this.idleAnimation.name);
				}
				else if (this._characterState == PickupCharacterState.Running)
				{
					this._animation[this.runAnimation.name].speed = this.runMaxAnimationSpeed;
					if (this.isControllable)
					{
						this._animation[this.runAnimation.name].speed = Mathf.Clamp(this.velocity.magnitude, 0f, this.runMaxAnimationSpeed);
					}
					this._animation.CrossFade(this.runAnimation.name);
				}
				else if (this._characterState == PickupCharacterState.Trotting)
				{
					this._animation[this.walkAnimation.name].speed = this.trotMaxAnimationSpeed;
					if (this.isControllable)
					{
						this._animation[this.walkAnimation.name].speed = Mathf.Clamp(this.velocity.magnitude, 0f, this.trotMaxAnimationSpeed);
					}
					this._animation.CrossFade(this.walkAnimation.name);
				}
				else if (this._characterState == PickupCharacterState.Walking)
				{
					this._animation[this.walkAnimation.name].speed = this.walkMaxAnimationSpeed;
					if (this.isControllable)
					{
						this._animation[this.walkAnimation.name].speed = Mathf.Clamp(this.velocity.magnitude, 0f, this.walkMaxAnimationSpeed);
					}
					this._animation.CrossFade(this.walkAnimation.name);
				}
				if (this._characterState != PickupCharacterState.Running)
				{
					this._animation[this.runAnimation.name].time = 0f;
				}
			}
		}
		if (this.IsGrounded())
		{
			if (this.DoRotate)
			{
				base.transform.rotation = Quaternion.LookRotation(this.moveDirection);
			}
		}
		if (this.IsGrounded())
		{
			this.lastGroundedTime = Time.time;
			this.inAirVelocity = Vector3.zero;
			if (this.jumping)
			{
				this.jumping = false;
				base.SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
		this.lastPos = base.transform.position;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext((byte)this._characterState);
		}
		else
		{
			bool flag = this.remotePosition == Vector3.zero;
			this.remotePosition = (Vector3)stream.ReceiveNext();
			this._characterState = (PickupCharacterState)((byte)stream.ReceiveNext());
			if (flag)
			{
				base.transform.position = this.remotePosition;
			}
		}
	}

	private void UpdateSmoothedMovementDirection()
	{
		Transform transform = Camera.main.transform;
		bool flag = this.IsGrounded();
		Vector3 a = transform.TransformDirection(Vector3.forward);
		a.y = 0f;
		a = a.normalized;
		Vector3 a2 = new Vector3(a.z, 0f, -a.x);
		float axisRaw = Input.GetAxisRaw("Vertical");
		float axisRaw2 = Input.GetAxisRaw("Horizontal");
		if (axisRaw < -0.2f)
		{
			this.movingBack = true;
		}
		else
		{
			this.movingBack = false;
		}
		bool flag2 = this.isMoving;
		this.isMoving = (Mathf.Abs(axisRaw2) > 0.1f || Mathf.Abs(axisRaw) > 0.1f);
		Vector3 vector = axisRaw2 * a2 + axisRaw * a;
		if (flag)
		{
			this.lockCameraTimer += Time.deltaTime;
			if (this.isMoving != flag2)
			{
				this.lockCameraTimer = 0f;
			}
			if (vector != Vector3.zero)
			{
				if (this.moveSpeed < this.walkSpeed * 0.9f && flag)
				{
					this.moveDirection = vector.normalized;
				}
				else
				{
					this.moveDirection = Vector3.RotateTowards(this.moveDirection, vector, this.rotateSpeed * 0.0174532924f * Time.deltaTime, 1000f);
					this.moveDirection = this.moveDirection.normalized;
				}
			}
			float t = this.speedSmoothing * Time.deltaTime;
			float num = Mathf.Min(vector.magnitude, 1f);
			this._characterState = PickupCharacterState.Idle;
			if ((Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift)) && this.isMoving)
			{
				num *= this.runSpeed;
				this._characterState = PickupCharacterState.Running;
			}
			else if (Time.time - this.trotAfterSeconds > this.walkTimeStart)
			{
				num *= this.trotSpeed;
				this._characterState = PickupCharacterState.Trotting;
			}
			else if (this.isMoving)
			{
				num *= this.walkSpeed;
				this._characterState = PickupCharacterState.Walking;
			}
			this.moveSpeed = Mathf.Lerp(this.moveSpeed, num, t);
			if (this.moveSpeed < this.walkSpeed * 0.3f)
			{
				this.walkTimeStart = Time.time;
			}
		}
		else
		{
			if (this.jumping)
			{
				this.lockCameraTimer = 0f;
			}
			if (this.isMoving)
			{
				this.inAirVelocity += vector.normalized * Time.deltaTime * this.inAirControlAcceleration;
			}
		}
	}

	private void ApplyJumping()
	{
		if (this.lastJumpTime + this.jumpRepeatTime > Time.time)
		{
			return;
		}
		if (this.IsGrounded() && this.canJump && Time.time < this.lastJumpButtonTime + this.jumpTimeout)
		{
			this.verticalSpeed = this.CalculateJumpVerticalSpeed(this.jumpHeight);
			base.SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ApplyGravity()
	{
		if (this.isControllable)
		{
			if (this.jumping && !this.jumpingReachedApex && this.verticalSpeed <= 0f)
			{
				this.jumpingReachedApex = true;
				base.SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			if (this.IsGrounded())
			{
				this.verticalSpeed = 0f;
			}
			else
			{
				this.verticalSpeed -= this.gravity * Time.deltaTime;
			}
		}
	}

	private float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * this.gravity);
	}

	private void DidJump()
	{
		this.jumping = true;
		this.jumpingReachedApex = false;
		this.lastJumpTime = Time.time;
		this.lastJumpButtonTime = -10f;
		this._characterState = PickupCharacterState.Jumping;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.get_moveDirection().y > 0.01f)
		{
			return;
		}
	}

	public float GetSpeed()
	{
		return this.moveSpeed;
	}

	public bool IsJumping()
	{
		return this.jumping;
	}

	public bool IsGrounded()
	{
		return (this.collisionFlags & 4) != 0;
	}

	public Vector3 GetDirection()
	{
		return this.moveDirection;
	}

	public bool IsMovingBackwards()
	{
		return this.movingBack;
	}

	public float GetLockCameraTimer()
	{
		return this.lockCameraTimer;
	}

	public bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}

	public bool HasJumpReachedApex()
	{
		return this.jumpingReachedApex;
	}

	public bool IsGroundedWithTimeout()
	{
		return this.lastGroundedTime + this.groundedTimeout > Time.time;
	}

	public void Reset()
	{
		base.gameObject.tag = "Player";
	}
}
