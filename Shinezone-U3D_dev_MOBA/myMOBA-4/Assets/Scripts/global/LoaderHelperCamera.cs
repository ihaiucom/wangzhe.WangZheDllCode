using System;
using System.Collections.Generic;
using UnityEngine;

public class LoaderHelperCamera
{
	public struct Obj
	{
		public GameObject go;

		public int frame;
	}

	public GameObject rootObj;

	public GameObject camObj;

	public Vector3 invalidPos = new Vector3(9999f, 9999f, 9999f);

	public List<LoaderHelperCamera.Obj> objList = new List<LoaderHelperCamera.Obj>();

	public int objIndex;

	private Dictionary<string, bool> loadedChecker = new Dictionary<string, bool>();

	public LoaderHelperCamera()
	{
		this.rootObj = new GameObject();
		this.camObj = new GameObject();
		this.camObj.transform.parent = this.rootObj.transform;
		this.rootObj.name = "lhc";
		this.camObj.name = "camera";
		Camera camera = this.camObj.AddComponent<Camera>();
		camera.transform.position = new Vector3(this.invalidPos.x, this.invalidPos.y, this.invalidPos.z - 100f);
		camera.depth = -200f;
		camera.clearFlags = CameraClearFlags.Color;
	}

	public bool HasLoaded(string objPath)
	{
		return this.loadedChecker.ContainsKey(objPath);
	}

	public void AddObj(string path, GameObject go)
	{
		if (go == null)
		{
			return;
		}
		this.loadedChecker.Add(path, true);
		LoaderHelperCamera.Obj item = default(LoaderHelperCamera.Obj);
		item.go = go;
		item.frame = Time.frameCount;
		this.objList.Add(item);
		DebugHelper.Assert(this.rootObj != null, "you add obj when rootObj is null");
		if (this.rootObj != null)
		{
			go.transform.SetParent(this.rootObj.transform);
		}
		go.transform.position = this.invalidPos;
	}

	public bool Update()
	{
		int frameCount = Time.frameCount;
		for (int i = this.objIndex; i < this.objList.Count; i++)
		{
			LoaderHelperCamera.Obj value = this.objList[i];
			if (frameCount - value.frame < 5)
			{
				this.objIndex = i;
				return false;
			}
			Singleton<CGameObjectPool>.instance.RecyclePreparedGameObject(value.go);
			value.go = null;
			this.objList[i] = value;
		}
		this.objIndex = this.objList.Count;
		return true;
	}

	public void Destroy()
	{
		this.loadedChecker.Clear();
		this.objList.Clear();
		UnityEngine.Object.Destroy(this.camObj);
		UnityEngine.Object.Destroy(this.rootObj);
		this.camObj = null;
		this.rootObj = null;
	}
}
