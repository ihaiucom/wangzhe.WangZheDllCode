using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

public class Moba_Camera : MonoBehaviour
{
	private const float MAXROTATIONXAXIS = 89f;

	private const float MINROTATIONXAXIS = -89f;

	private static Camera _currentMobaCamera;

	public bool useFixedUpdate;

	private Vector2 PreRelativeDisplacement;

	private Vector2 RelativeDisplacement;

	private Vector3 StartLocation;

	private bool bEnableDisplacement;

	public Moba_Camera_Requirements requirements = new Moba_Camera_Requirements();

	public Moba_Camera_Inputs inputs = new Moba_Camera_Inputs();

	public Moba_Camera_Settings settings = new Moba_Camera_Settings();

	private float _currentZoomAmount;

	private float _currentZoomRate = 1f;

	public float _lockTransitionRate = 1f;

	private Vector2 _currentCameraRotation = Vector3.zero;

	private bool changeInCamera = true;

	private float deltaMouseDeadZone = 0.2f;

	private Plane[] CachedPlanes = new Plane[6];

	public static Camera currentMobaCamera
	{
		get
		{
			return Moba_Camera._currentMobaCamera;
		}
	}

	public float currentZoomAmount
	{
		get
		{
			return this._currentZoomAmount;
		}
		set
		{
			this._currentZoomAmount = value;
			this.changeInCamera = true;
		}
	}

	public float currentZoomRate
	{
		get
		{
			return this._currentZoomRate;
		}
		set
		{
			this._currentZoomRate = value;
			this.changeInCamera = true;
		}
	}

	public Vector3 currentCameraRotation
	{
		get
		{
			return this._currentCameraRotation;
		}
		set
		{
			this._currentCameraRotation = value;
			this.changeInCamera = true;
		}
	}

	public bool lockRotateX
	{
		get
		{
			return this.settings.rotation.lockRotationX;
		}
		set
		{
			this.settings.rotation.lockRotationX = value;
		}
	}

	public bool lockRotateY
	{
		get
		{
			return this.settings.rotation.lockRotationY;
		}
		set
		{
			this.settings.rotation.lockRotationY = value;
		}
	}

	public Plane[] frustum
	{
		get
		{
			return (this.requirements == null || !(this.requirements.camera != null)) ? null : this.CalcFrustum(this.requirements.camera);
		}
	}

	private void Awake()
	{
		Moba_Camera._currentMobaCamera = this.requirements.camera;
	}

	private void OnDestroy()
	{
		Moba_Camera._currentMobaCamera = null;
	}

	private void Start()
	{
		if (!this.requirements.pivot || !this.requirements.offset || !this.requirements.camera)
		{
			string str = string.Empty;
			if (this.requirements.pivot == null)
			{
				str += " / Pivot";
				base.enabled = false;
			}
			if (this.requirements.offset == null)
			{
				str += " / Offset";
				base.enabled = false;
			}
			if (this.requirements.camera == null)
			{
				str += " / Camera";
				base.enabled = false;
			}
		}
		this._currentZoomAmount = this.settings.zoom.defaultZoom;
		this._currentCameraRotation = this.settings.rotation.defualtRotation;
		if (this.settings.movement.useDefualtHeight && base.enabled)
		{
			DebugHelper.Assert(this.requirements.pivot != null && this.requirements.pivot.transform != null, null, null);
			if (this.requirements != null && this.requirements.pivot != null && this.requirements.pivot.transform != null)
			{
				Vector3 position = this.requirements.pivot.transform.position;
				position.y = this.settings.movement.defualtHeight;
				this.requirements.pivot.transform.position = position;
			}
		}
		if (this.requirements.camera != null)
		{
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"Hide"
			});
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"UI"
			});
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"UIRaw"
			});
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"UI_Background"
			});
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"UI_Foreground"
			});
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"UI_BottomBG"
			});
			this.requirements.camera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				"3DUI"
			});
		}
		Singleton<Camera_UI3D>.GetInstance().Reset();
	}

	private void Update()
	{
		if (!this.useFixedUpdate)
		{
			this.CameraUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (this.useFixedUpdate)
		{
			this.CameraUpdate();
		}
	}

	public void CameraUpdate()
	{
		this.CalculateCameraZoom();
		this.CalculateCameraRotation();
		this.CalculateCameraMovement();
		this.CalculateCameraUpdates();
		this.CalculateCameraBoundaries();
	}

	private void CalculateCameraZoom()
	{
		float num = 0f;
		int num2 = 1;
		float axis = Input.GetAxis(this.inputs.axis.DeltaScrollWheel);
		if (axis != 0f)
		{
			this.changeInCamera = true;
			if (this.settings.zoom.constZoomRate)
			{
				if ((double)axis != 0.0)
				{
					if ((double)axis > 0.0)
					{
						num = 1f;
					}
					else
					{
						num = -1f;
					}
				}
			}
			else
			{
				num = axis;
			}
		}
		if (!this.settings.zoom.invertZoom)
		{
			num2 = -1;
		}
		this._currentZoomAmount += num * this.settings.zoom.zoomRate * (float)num2 * Time.deltaTime;
	}

	private void CalculateCameraRotation()
	{
		float num = 0f;
		float num2 = 0f;
		Screen.lockCursor = false;
		if ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_rotate_camera) : (Input.GetKey(this.inputs.keycodes.RotateCamera) && this.inputs.useKeyCodeInputs))
		{
			Screen.lockCursor = true;
			if (!this.settings.rotation.lockRotationX)
			{
				float axis = Input.GetAxis(this.inputs.axis.DeltaMouseVertical);
				if ((double)axis != 0.0)
				{
					if (this.settings.rotation.constRotationRate)
					{
						if (axis > this.deltaMouseDeadZone)
						{
							num = 1f;
						}
						else if (axis < -this.deltaMouseDeadZone)
						{
							num = -1f;
						}
					}
					else
					{
						num = axis;
					}
					this.changeInCamera = true;
				}
			}
			if (!this.settings.rotation.lockRotationY)
			{
				float axis2 = Input.GetAxis(this.inputs.axis.DeltaMouseHorizontal);
				if (axis2 != 0f)
				{
					if (this.settings.rotation.constRotationRate)
					{
						if (axis2 > this.deltaMouseDeadZone)
						{
							num2 = 1f;
						}
						else if (axis2 < -this.deltaMouseDeadZone)
						{
							num2 = -1f;
						}
					}
					else
					{
						num2 = -1f * axis2;
					}
					this.changeInCamera = true;
				}
			}
		}
		this._currentCameraRotation.y = this._currentCameraRotation.y + num2 * this.settings.rotation.cameraRotationRate.y * Time.deltaTime;
		this._currentCameraRotation.x = this._currentCameraRotation.x + num * this.settings.rotation.cameraRotationRate.x * Time.deltaTime;
	}

	private void CalculateCameraMovement()
	{
		DebugHelper.Assert(this.requirements != null && this.requirements.pivot != null, "requirements != null && requirements.pivot!=null", null);
		if (this.requirements == null || this.requirements.pivot == null)
		{
			return;
		}
		DebugHelper.Assert(this.settings != null, "settings is null", null);
		if (((!this.inputs.useKeyCodeInputs) ? (Input.GetButtonDown(this.inputs.axis.button_lock_camera) && this.settings.lockTarget) : Input.GetKeyDown(this.inputs.keycodes.LockCamera)) && this.settings.lockTarget)
		{
			this.settings.cameraLocked = !this.settings.cameraLocked;
		}
		if (this.settings.useAbsoluteLock)
		{
			Vector3 absoluteLockLocation = this.settings.absoluteLockLocation;
			absoluteLockLocation.y += this.settings.targetHeight;
			if ((this.requirements.pivot.position - absoluteLockLocation).magnitude > 0.001f)
			{
				if (this.settings.movement.useDefualtHeight && !this.settings.movement.useLockTargetHeight)
				{
					absoluteLockLocation.y = this.settings.movement.defualtHeight;
				}
				else if (!this.settings.movement.useLockTargetHeight)
				{
					absoluteLockLocation.y = this.requirements.pivot.position.y;
				}
				this.requirements.pivot.position = Vector3.Lerp(this.requirements.pivot.position, absoluteLockLocation, this.settings.movement.lockTransitionRate);
			}
		}
		else if (this.settings.lockTarget && (this.settings.cameraLocked || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_char_focus) : Input.GetKey(this.inputs.keycodes.characterFocus))))
		{
			Vector3 position = this.settings.lockTarget.handle.myTransform.position;
			position.y += this.settings.targetHeight;
			bool bComposePlayerMovement = MonoSingleton<GlobalConfig>.instance.bComposePlayerMovement;
			float magnitude = (this.requirements.pivot.position - position).magnitude;
			if (magnitude > 0.001f)
			{
				if (this.settings.movement.useDefualtHeight && !this.settings.movement.useLockTargetHeight)
				{
					position.y = this.settings.movement.defualtHeight;
				}
				else if (!this.settings.movement.useLockTargetHeight)
				{
					position.y = this.requirements.pivot.position.y;
				}
				this.requirements.pivot.position = Vector3.Lerp(this.requirements.pivot.position, position, this._lockTransitionRate);
				float x;
				float z;
				if (this.bEnableDisplacement)
				{
					x = ((!bComposePlayerMovement) ? this.StartLocation.x : this.requirements.pivot.position.x) + this.RelativeDisplacement.x / 1000f;
					z = ((!bComposePlayerMovement) ? this.StartLocation.z : this.requirements.pivot.position.z) + this.RelativeDisplacement.y / 1000f;
				}
				else
				{
					x = this.requirements.pivot.position.x;
					z = this.requirements.pivot.position.z;
				}
				this.requirements.pivot.position = new Vector3(x, this.requirements.pivot.position.y, z);
			}
			else if (this.RelativeDisplacement.x != 0f || this.RelativeDisplacement.y != 0f)
			{
				float x2;
				float z2;
				if (this.bEnableDisplacement)
				{
					x2 = ((!bComposePlayerMovement) ? this.StartLocation.x : this.requirements.pivot.position.x) + this.RelativeDisplacement.x / 1000f;
					z2 = ((!bComposePlayerMovement) ? this.StartLocation.z : this.requirements.pivot.position.z) + this.RelativeDisplacement.y / 1000f;
				}
				else
				{
					x2 = this.requirements.pivot.position.x;
					z2 = this.requirements.pivot.position.z;
				}
				this.requirements.pivot.position = new Vector3(x2, this.requirements.pivot.position.y, z2);
			}
		}
		else
		{
			Vector3 a = new Vector3(0f, 0f, 0f);
			if ((Input.mousePosition.x < this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_left) : Input.GetKey(this.inputs.keycodes.CameraMoveLeft)))
			{
				a -= this.requirements.pivot.transform.right;
			}
			if ((Input.mousePosition.x > (float)Screen.width - this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_right) : Input.GetKey(this.inputs.keycodes.CameraMoveRight)))
			{
				a += this.requirements.pivot.transform.right;
			}
			if ((Input.mousePosition.y < this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_backward) : Input.GetKey(this.inputs.keycodes.CameraMoveBackward)))
			{
				a -= this.requirements.pivot.transform.forward;
			}
			if ((Input.mousePosition.y > (float)Screen.height - this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_forward) : Input.GetKey(this.inputs.keycodes.CameraMoveForward)))
			{
				a += this.requirements.pivot.transform.forward;
			}
			this.requirements.pivot.position += a.normalized * this.settings.movement.cameraMovementRate * Time.deltaTime;
			Vector3 zero = Vector3.zero;
			Vector3 vector = new Vector3(0f, this.requirements.pivot.position.y, 0f);
			if (this.settings.movement.useDefualtHeight)
			{
				zero.y = this.settings.movement.defualtHeight;
			}
			else
			{
				zero.y = this.requirements.pivot.position.y;
			}
			if ((zero - vector).magnitude > 0.001f)
			{
				Vector3 vector2 = Vector3.Lerp(vector, zero, this.settings.movement.lockTransitionRate);
				this.requirements.pivot.position = new Vector3(this.requirements.pivot.position.x, vector2.y, this.requirements.pivot.position.z);
			}
		}
	}

	private void CalculateCameraUpdates()
	{
		if (!this.changeInCamera)
		{
			return;
		}
		if (this.settings.zoom.maxZoom < this.settings.zoom.minZoom)
		{
			this.settings.zoom.maxZoom = this.settings.zoom.minZoom + 1f;
		}
		if (this._currentZoomAmount < this.settings.zoom.minZoom)
		{
			this._currentZoomAmount = this.settings.zoom.minZoom;
		}
		if (this._currentZoomAmount > this.settings.zoom.maxZoom)
		{
			this._currentZoomAmount = this.settings.zoom.maxZoom;
		}
		if (this._currentCameraRotation.x > 89f)
		{
			this._currentCameraRotation.x = 89f;
		}
		else if (this._currentCameraRotation.x < -89f)
		{
			this._currentCameraRotation.x = -89f;
		}
		Vector3 forward = Quaternion.AngleAxis(this._currentCameraRotation.y, Vector3.up) * Vector3.forward;
		this.requirements.pivot.transform.rotation = Quaternion.LookRotation(forward);
		Vector3 vector = this.requirements.pivot.transform.TransformDirection(Vector3.forward);
		vector = Quaternion.AngleAxis(this._currentCameraRotation.x, this.requirements.pivot.transform.TransformDirection(Vector3.right)) * vector;
		this.requirements.offset.position = -vector * this._currentZoomAmount * this._currentZoomRate + this.requirements.pivot.position;
		this.requirements.offset.transform.LookAt(this.requirements.pivot);
		this.changeInCamera = false;
	}

	private void CalculateCameraBoundaries()
	{
		if (this.settings.useBoundaries && !((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_right) : Input.GetKey(this.inputs.keycodes.CameraMoveRight)) && !Moba_Camera_Boundaries.isPointInBoundary(this.requirements.pivot.position))
		{
			Moba_Camera_Boundary closestBoundary = Moba_Camera_Boundaries.GetClosestBoundary(this.requirements.pivot.position);
			if (closestBoundary != null)
			{
				this.requirements.pivot.position = Moba_Camera_Boundaries.GetClosestPointOnBoundary(closestBoundary, this.requirements.pivot.position);
				this.RelativeDisplacement = this.PreRelativeDisplacement;
			}
		}
	}

	public void SetTargetTransform(PoolObjHandle<ActorRoot> t)
	{
		DebugHelper.Assert(t, "invalid parameter for SetTargetTransform", null);
		if (t)
		{
			this.settings.lockTarget = t;
			if (t.handle.CharInfo != null)
			{
				this.settings.targetHeight = (float)t.handle.CharInfo.iBulletHeight * 0.001f;
			}
		}
	}

	public void SetCameraLocked(bool locked)
	{
		this.settings.cameraLocked = locked;
	}

	public bool GetCameraLocked()
	{
		return this.settings.cameraLocked;
	}

	public bool GetAbsoluteLocked()
	{
		return this.settings.useAbsoluteLock;
	}

	public void SetAbsoluteLocked(bool locked)
	{
		this.settings.useAbsoluteLock = locked;
	}

	public void SetAbsoluteLockLocation(Vector3 pos)
	{
		this.settings.absoluteLockLocation = pos;
	}

	public void SetCameraRotation(Vector2 rotation)
	{
		this.currentCameraRotation = new Vector2(rotation.x, rotation.y);
	}

	public void SetCameraRotation(float x, float y)
	{
		this.currentCameraRotation = new Vector2(x, y);
	}

	public void SetCameraZoom(float amount)
	{
		this.currentZoomAmount = amount;
	}

	public void UpdateCameraRelativeDisplacement(ref Vector2 InOffset)
	{
		this.PreRelativeDisplacement = this.RelativeDisplacement;
		this.RelativeDisplacement += InOffset;
	}

	public void StopCameraRelativeDisplacement()
	{
		this.RelativeDisplacement = (this.PreRelativeDisplacement = default(Vector2));
	}

	public void SetStartLocation(ref Vector3 InLocation)
	{
		this.StartLocation = InLocation;
	}

	public void SetEnableDisplacement(bool bInEnableDisplacement)
	{
		this.bEnableDisplacement = bInEnableDisplacement;
	}

	public Camera GetCamera()
	{
		return this.requirements.camera;
	}

	private Plane[] CalcFrustum(Camera InCamera)
	{
		GeometryUtilityUser.CalculateFrustumPlanes(InCamera, ref this.CachedPlanes);
		return this.CachedPlanes;
	}

	private static bool IsEqual(Plane InFirst, Plane InSecond)
	{
		return Moba_Camera.IsEqual(InFirst.normal, InSecond.normal) && Moba_Camera.IsEqual(InFirst.distance, InSecond.distance);
	}

	private static bool IsEqual(Vector3 InFirst, Vector3 InSecond)
	{
		return Moba_Camera.IsEqual(InFirst.x, InSecond.x) && Moba_Camera.IsEqual(InFirst.y, InSecond.y) && Moba_Camera.IsEqual(InFirst.y, InSecond.y);
	}

	private static bool IsEqual(float InFirst, float InSecond)
	{
		return Math.Abs(InFirst - InSecond) < 0.001f;
	}
}
