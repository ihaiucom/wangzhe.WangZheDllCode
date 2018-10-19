using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

public class Camera_UI3D : Singleton<Camera_UI3D>
{
	private Camera m_camera;

	private Canvas3D m_canvas;

	public override void Init()
	{
		Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
	}

	public Camera GetCurrentCamera()
	{
		return this.m_camera;
	}

	public Canvas3DImpl GetCurrentCanvas()
	{
		return Singleton<Canvas3DImpl>.GetInstance();
	}

	public void OnFightOver(ref DefaultGameEventParam prm)
	{
		if (this.m_camera != null)
		{
			UnityEngine.Object.Destroy(this.m_camera.gameObject);
			this.m_camera = null;
		}
	}

	public void Reset()
	{
		GameObject gameObject = new GameObject("Camera_UI3D");
		MonoSingleton<SceneMgr>.GetInstance().AddToRoot(gameObject, SceneObjType.Temp);
		this.m_camera = gameObject.AddComponent<Camera>();
		this.m_camera.CopyFrom(Moba_Camera.currentMobaCamera);
		this.m_camera.orthographic = true;
		this.m_camera.orthographicSize = 8f;
		this.m_camera.cullingMask = LayerMask.GetMask(new string[]
		{
			"3DUI"
		});
		this.m_camera.depth = this.m_camera.depth + 1f;
		this.m_camera.clearFlags = CameraClearFlags.Nothing;
		gameObject.tag = "Untagged";
		this.m_canvas = gameObject.AddComponent<Canvas3D>();
		gameObject.transform.position = new Vector3(1000f, -1000f, 1000f);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}
}
