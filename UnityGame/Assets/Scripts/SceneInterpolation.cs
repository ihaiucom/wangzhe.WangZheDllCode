using System;
using System.Collections.Generic;
using UnityEngine;

internal class SceneInterpolation : MonoBehaviour
{
	private struct RestoreCameraClearFlags
	{
		public Camera camera;

		public CameraClearFlags flags;
	}

	private RenderTexture rt0;

	private RenderTexture rt1;

	private Camera camera_scene0;

	private Camera camera_scene1;

	private Camera camera_pp;

	public float FadeTime = 2f;

	private float factor;

	private List<Camera> cameraList = new List<Camera>();

	private SceneInterpolationRT interpolationRT;

	private List<SceneInterpolation.RestoreCameraClearFlags> cameraClearFlagsList = new List<SceneInterpolation.RestoreCameraClearFlags>();

	private int activedCamera = -1;

	private void DuplicateCamera(Camera src, Camera dest)
	{
		dest.transform.parent = src.transform;
		dest.transform.localPosition = Vector3.zero;
		dest.transform.localRotation = Quaternion.identity;
		dest.transform.localScale = Vector3.one;
		dest.aspect = src.aspect;
		dest.backgroundColor = src.backgroundColor;
		dest.nearClipPlane = src.nearClipPlane;
		dest.farClipPlane = src.farClipPlane;
		dest.fieldOfView = src.fieldOfView;
		dest.orthographic = src.orthographic;
		dest.orthographicSize = src.orthographicSize;
		dest.pixelRect = src.pixelRect;
		dest.rect = src.rect;
	}

	private bool UpdateCamera()
	{
		int num = -1;
		for (int i = 0; i < this.cameraList.get_Count(); i++)
		{
			Camera camera = this.cameraList.get_Item(i);
			if (camera.enabled && camera.gameObject.activeSelf && camera.gameObject.activeInHierarchy)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return false;
		}
		this.activedCamera = num;
		Camera src = this.cameraList.get_Item(this.activedCamera);
		this.DuplicateCamera(src, this.camera_scene0);
		this.DuplicateCamera(src, this.camera_scene1);
		this.DuplicateCamera(src, this.camera_pp);
		return true;
	}

	public void Play()
	{
		this.factor = 0f;
		this.cameraClearFlagsList.Clear();
		this.cameraList.Clear();
		int mask = LayerMask.GetMask(new string[]
		{
			"Scene",
			"Scene2"
		});
		Object[] array = Object.FindObjectsOfType(typeof(Camera));
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				Camera camera = array[i] as Camera;
				if ((camera.cullingMask & mask) != 0)
				{
					camera.cullingMask &= ~mask;
					this.cameraList.Add(camera);
				}
			}
			this.cameraList.Sort(delegate(Camera a, Camera b)
			{
				if (a.depth < b.depth)
				{
					return -1;
				}
				if (a.depth > b.depth)
				{
					return 1;
				}
				return 0;
			});
		}
		GameObject gameObject = new GameObject();
		GameObject gameObject2 = new GameObject();
		GameObject gameObject3 = new GameObject();
		int num = -500;
		this.camera_scene0 = gameObject.AddComponent<Camera>();
		this.camera_scene0.cullingMask = LayerMask.GetMask(new string[]
		{
			"Scene"
		});
		this.camera_scene0.depth = (float)num--;
		this.camera_scene1 = gameObject2.AddComponent<Camera>();
		this.camera_scene1.cullingMask = LayerMask.GetMask(new string[]
		{
			"Scene2"
		});
		this.camera_scene1.depth = (float)num--;
		this.camera_pp = gameObject3.AddComponent<Camera>();
		this.camera_pp.cullingMask = 0;
		this.camera_pp.depth = (float)num--;
		if (!this.UpdateCamera())
		{
			return;
		}
		int width = Mathf.RoundToInt(this.camera_scene0.pixelWidth);
		int height = Mathf.RoundToInt(this.camera_scene0.pixelHeight);
		this.rt0 = new RenderTexture(width, height, 24);
		this.rt1 = new RenderTexture(width, height, 24);
		this.camera_scene0.targetTexture = this.rt0;
		this.camera_scene1.targetTexture = this.rt1;
		this.interpolationRT = this.camera_pp.gameObject.AddComponent<SceneInterpolationRT>();
		this.interpolationRT.rt0 = this.rt0;
		this.interpolationRT.rt1 = this.rt1;
		this.interpolationRT.factor = 0f;
		Camera camera2 = this.cameraList.get_Item(this.activedCamera);
		this.camera_scene0.clearFlags = camera2.clearFlags;
		this.camera_scene1.clearFlags = camera2.clearFlags;
		this.camera_pp.clearFlags = camera2.clearFlags;
		for (int j = 0; j < this.cameraList.get_Count(); j++)
		{
			Camera camera3 = this.cameraList.get_Item(j);
			if (camera3.clearFlags == CameraClearFlags.Skybox || camera3.clearFlags == CameraClearFlags.Color)
			{
				SceneInterpolation.RestoreCameraClearFlags restoreCameraClearFlags = default(SceneInterpolation.RestoreCameraClearFlags);
				restoreCameraClearFlags.camera = camera3;
				restoreCameraClearFlags.flags = camera3.clearFlags;
				this.cameraClearFlagsList.Add(restoreCameraClearFlags);
				camera3.clearFlags = CameraClearFlags.Depth;
			}
		}
	}

	public void Stop()
	{
		if (this.interpolationRT)
		{
			Object.Destroy(this.interpolationRT);
			this.interpolationRT = null;
		}
		if (this.camera_scene0)
		{
			Object.Destroy(this.camera_scene0.gameObject);
			this.camera_scene0 = null;
		}
		if (this.camera_scene1)
		{
			Object.Destroy(this.camera_scene1.gameObject);
			this.camera_scene1 = null;
		}
		if (this.camera_pp)
		{
			Object.Destroy(this.camera_pp.gameObject);
			this.camera_pp = null;
		}
		if (this.rt0 != null)
		{
			this.rt0.Release();
			Object.Destroy(this.rt0);
			this.rt0 = null;
		}
		if (this.rt1 != null)
		{
			this.rt1.Release();
			Object.Destroy(this.rt0);
			this.rt1 = null;
		}
		int mask = LayerMask.GetMask(new string[]
		{
			"Scene2"
		});
		for (int i = 0; i < this.cameraClearFlagsList.get_Count(); i++)
		{
			SceneInterpolation.RestoreCameraClearFlags restoreCameraClearFlags = this.cameraClearFlagsList.get_Item(i);
			if (restoreCameraClearFlags.camera)
			{
				restoreCameraClearFlags.camera.clearFlags = restoreCameraClearFlags.flags;
			}
		}
		for (int j = 0; j < this.cameraList.get_Count(); j++)
		{
			Camera camera = this.cameraList.get_Item(j);
			if (camera)
			{
				camera.cullingMask |= mask;
			}
		}
		this.cameraClearFlagsList.Clear();
		this.cameraList.Clear();
	}

	public void Restore()
	{
		int mask = LayerMask.GetMask(new string[]
		{
			"Scene"
		});
		int mask2 = LayerMask.GetMask(new string[]
		{
			"Scene2"
		});
		Object[] array = Object.FindObjectsOfType(typeof(Camera));
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				Camera camera = array[i] as Camera;
				if ((camera.cullingMask & mask2) != 0)
				{
					camera.cullingMask &= ~mask2;
					camera.cullingMask |= mask;
				}
			}
		}
	}

	private void Update()
	{
		this.UpdateCamera();
		if (this.interpolationRT)
		{
			float num = Mathf.Max(0.01f, this.FadeTime);
			this.factor += Time.deltaTime / num;
			this.interpolationRT.factor = Mathf.Clamp01(this.factor);
			if (this.factor > 1f)
			{
				this.Stop();
			}
		}
	}
}
