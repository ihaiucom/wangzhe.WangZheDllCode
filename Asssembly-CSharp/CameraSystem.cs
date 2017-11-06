using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

public class CameraSystem : MonoSingleton<CameraSystem>
{
	public Moba_Camera MobaCamera;

	protected bool bFreeCamera;

	protected bool bFreeRotate;

	protected Plane[] CachedFrustum;

	private static float s_CameraMoveScale = 0.02f;

	private bool bEnableCameraMovment;

	private float LastUpdateTime;

	private float CurrentSpeed;

	private float zoomRateFromAge = 1f;

	public bool enableLockedCamera
	{
		get
		{
			return this.MobaCamera != null && this.MobaCamera.GetCameraLocked();
		}
		protected set
		{
			if (this.MobaCamera != null)
			{
				this.MobaCamera.SetCameraLocked(value);
			}
		}
	}

	protected bool enableAbsoluteLocationLockCamera
	{
		get
		{
			return this.MobaCamera != null && this.MobaCamera.GetAbsoluteLocked();
		}
		set
		{
			if (this.MobaCamera != null)
			{
				this.MobaCamera.SetAbsoluteLocked(value);
			}
		}
	}

	public float ZoomRateFromAge
	{
		get
		{
			return this.zoomRateFromAge;
		}
		set
		{
			if (value > 0f)
			{
				this.zoomRateFromAge = value;
				this.MobaCamera.currentZoomRate = this.GetZoomRate();
			}
		}
	}

	public bool enableCameraMovement
	{
		get
		{
			return this.bEnableCameraMovment;
		}
		set
		{
			this.bEnableCameraMovment = value;
			if (this.MobaCamera != null)
			{
				this.MobaCamera.SetEnableDisplacement(this.bEnableCameraMovment);
			}
			if (!this.bEnableCameraMovment)
			{
				this.StopDisplacement();
				this.CurrentSpeed = MonoSingleton<GlobalConfig>.instance.CameraMoveSpeed;
			}
			else
			{
				this.LastUpdateTime = Time.realtimeSinceStartup;
				if (this.MobaCamera != null)
				{
					Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
					if (hostPlayer != null && hostPlayer.Captain)
					{
						Vector3 vector = (Vector3)hostPlayer.Captain.handle.location;
						this.MobaCamera.SetStartLocation(ref vector);
					}
				}
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.CachedFrustum = ((this.MobaCamera != null) ? this.MobaCamera.frustum : null);
	}

	public void PrepareFight()
	{
		Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
		DebugHelper.Assert(hostPlayer != null, "local player is null in CameraSystem.PerpareFight", null);
		PoolObjHandle<ActorRoot> focusActor = (hostPlayer != null) ? hostPlayer.Captain : default(PoolObjHandle<ActorRoot>);
		this.SetFocusActor(focusActor);
		if (this.MobaCamera != null)
		{
			this.MobaCamera.currentZoomRate = this.GetZoomRate();
			this.MobaCamera.CameraUpdate();
		}
		Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnFocusSwitched));
		Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnPlayerDead));
		Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
		Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
		Singleton<GameEventSys>.instance.RmvEventHandler(GameEventDef.Event_CameraHeightChange, new Action(this.OnCameraHeightChanged));
		Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnFocusSwitched));
		Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnPlayerDead));
		Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
		Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
		Singleton<GameEventSys>.instance.AddEventHandler(GameEventDef.Event_CameraHeightChange, new Action(this.OnCameraHeightChanged));
		this.CurrentSpeed = MonoSingleton<GlobalConfig>.instance.CameraMoveSpeed;
	}

	private void OnFocusSwitched(ref DefaultGameEventParam prm)
	{
		if (prm.src && ActorHelper.IsHostCtrlActor(ref prm.src) && !Singleton<WatchController>.GetInstance().IsWatching)
		{
			this.SetFocusActor(prm.src);
			if (!prm.src.handle.ActorControl.IsDeadState && !this.bFreeCamera)
			{
				this.enableLockedCamera = true;
				this.enableAbsoluteLocationLockCamera = false;
			}
		}
	}

	public void SetFocusActor(PoolObjHandle<ActorRoot> focus)
	{
		if (this.MobaCamera == null)
		{
			GameObject gameObject = GameObject.Find("MainCamera");
			if (gameObject != null)
			{
				this.MobaCamera = gameObject.GetComponent<Moba_Camera>();
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.m_isCameraFlip)
				{
					this.MobaCamera.settings.rotation.defualtRotation = new Vector2(this.MobaCamera.settings.rotation.defualtRotation.x, 180f);
					this.MobaCamera.currentCameraRotation = this.MobaCamera.settings.rotation.defualtRotation;
				}
				this.MobaCamera.currentZoomRate = this.GetZoomRate();
			}
		}
		if (this.MobaCamera != null)
		{
			this.MobaCamera.SetTargetTransform(focus);
			this.MobaCamera.SetCameraLocked(true);
		}
	}

	public void SetFocusActorForce(PoolObjHandle<ActorRoot> focus, float inNewZoomAmount)
	{
		if (this.MobaCamera != null)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_isCameraFlip)
			{
				this.MobaCamera.settings.rotation.defualtRotation = new Vector2(this.MobaCamera.settings.rotation.defualtRotation.x, 180f);
			}
			this.MobaCamera.currentCameraRotation = this.MobaCamera.settings.rotation.defualtRotation;
			this.MobaCamera.currentZoomRate = this.GetZoomRate();
			this.MobaCamera.SetCameraZoom(inNewZoomAmount);
			this.MobaCamera.SetTargetTransform(focus);
			this.MobaCamera.SetCameraLocked(true);
		}
	}

	private void OnPlayerDead(ref GameDeadEventParam prm)
	{
		if (prm.src && ActorHelper.IsHostCtrlActor(ref prm.src) && !this.bFreeCamera && !Singleton<WatchController>.GetInstance().IsWatching)
		{
			if (this.MobaCamera && prm.src.handle.ActorControl != null)
			{
				this.MobaCamera.SetAbsoluteLockLocation((Vector3)prm.src.handle.ActorControl.actorLocation);
			}
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.EnableCameraDragPanelForDead();
			}
			if (!prm.src.handle.TheStaticData.TheBaseAttribute.DeadControl)
			{
				this.enableLockedCamera = false;
				this.enableAbsoluteLocationLockCamera = true;
			}
			this.StopDisplacement();
			if (this.MobaCamera)
			{
				this.MobaCamera._lockTransitionRate = 1f;
			}
		}
	}

	private void OnPlayerRevive(ref DefaultGameEventParam prm)
	{
		if (prm.src && ActorHelper.IsHostCtrlActor(ref prm.src) && !this.bFreeCamera && !Singleton<WatchController>.GetInstance().IsWatching)
		{
			this.enableLockedCamera = true;
			this.enableAbsoluteLocationLockCamera = false;
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.DisableCameraDragPanelForRevive();
			}
			this.SetFocusActor(prm.src);
		}
	}

	private void SetCameraLerp(int timerSequence)
	{
		if (this.MobaCamera != null)
		{
		}
	}

	private void OnCameraHeightChanged()
	{
		if (this.MobaCamera != null)
		{
			this.MobaCamera.currentZoomRate = this.GetZoomRate();
			this.MobaCamera.CameraUpdate();
		}
	}

	public void MoveCamera(float offX, float offY)
	{
		if (this.MobaCamera != null)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_isCameraFlip)
			{
				offX = -offX;
				offY = -offY;
			}
			Moba_Camera_Settings settings = this.MobaCamera.settings;
			settings.absoluteLockLocation.x = settings.absoluteLockLocation.x + offX * CameraSystem.s_CameraMoveScale;
			Moba_Camera_Settings settings2 = this.MobaCamera.settings;
			settings2.absoluteLockLocation.z = settings2.absoluteLockLocation.z + offY * CameraSystem.s_CameraMoveScale;
		}
	}

	public void ToggleFreeCamera()
	{
		this.bFreeCamera = !this.bFreeCamera;
		this.enableLockedCamera = !this.bFreeCamera;
		this.enableAbsoluteLocationLockCamera = false;
	}

	public void ToggleFreeDragCamera(bool bFree)
	{
		this.enableLockedCamera = !bFree;
		this.enableAbsoluteLocationLockCamera = bFree;
	}

	public void ToggleRotate()
	{
		this.bFreeRotate = !this.bFreeRotate;
		this.MobaCamera.lockRotateX = !this.bFreeRotate;
		this.MobaCamera.lockRotateY = !this.bFreeRotate;
	}

	public bool CheckVisiblity(Bounds InBounds)
	{
		return this.CachedFrustum == null || GeometryUtility.TestPlanesAABB(this.CachedFrustum, InBounds);
	}

	public void UpdateCameraMovement(ref Vector2 axis)
	{
		if (!this.bEnableCameraMovment)
		{
			return;
		}
		float num = Time.realtimeSinceStartup - this.LastUpdateTime;
		if (MonoSingleton<GlobalConfig>.instance.bResetCameraSpeedWhenZero && axis.x == 0f && axis.y == 0f)
		{
			this.CurrentSpeed = MonoSingleton<GlobalConfig>.instance.CameraMoveSpeed;
		}
		float a = (MonoSingleton<GlobalConfig>.instance.CameraMoveSpeedMax - this.CurrentSpeed) / MonoSingleton<GlobalConfig>.instance.CameraMoveAcceleration;
		float num2 = Mathf.Min(a, num);
		this.CurrentSpeed += MonoSingleton<GlobalConfig>.instance.CameraMoveAcceleration * num2;
		Vector2 a2 = axis.normalized * this.CurrentSpeed * num + axis.normalized * 0.5f * MonoSingleton<GlobalConfig>.instance.CameraMoveAcceleration * num2 * num2;
		if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
		{
			a2 *= -1f;
		}
		this.LastUpdateTime = Time.realtimeSinceStartup;
		if (this.MobaCamera != null)
		{
			this.MobaCamera.UpdateCameraRelativeDisplacement(ref a2);
		}
	}

	private void StopDisplacement()
	{
		if (this.MobaCamera != null)
		{
			this.MobaCamera.StopCameraRelativeDisplacement();
		}
	}

	public void UpdatePanelCameraMovement(ref Vector2 InMovement)
	{
		if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
		{
			InMovement *= -1f;
		}
		InMovement *= MonoSingleton<GlobalConfig>.instance.PanelCameraMoveSpeed;
		if (this.MobaCamera != null)
		{
			this.MobaCamera.UpdateCameraRelativeDisplacement(ref InMovement);
		}
	}

	private float GetZoomRate()
	{
		return GameSettings.CameraHeightRateValue * this.zoomRateFromAge;
	}
}
