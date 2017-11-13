using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

[Serializable]
public class Moba_Camera_Settings
{
	public bool useBoundaries = true;

	public bool cameraLocked;

	public bool useAbsoluteLock;

	public Vector3 absoluteLockLocation;

	public PoolObjHandle<ActorRoot> lockTarget;

	public float targetHeight;

	public Moba_Camera_Settings_Movement movement = new Moba_Camera_Settings_Movement();

	public Moba_Camera_Settings_Rotation rotation = new Moba_Camera_Settings_Rotation();

	public Moba_Camera_Settings_Zoom zoom = new Moba_Camera_Settings_Zoom();
}
