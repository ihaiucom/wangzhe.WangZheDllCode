using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	public class CurvlData
	{
		public string prefabFile = "Assets/AGE/Action/Prefab/Resources/Cube.prefab";

		public GameObject transObjPrefb;

		public bool isHide;

		public bool isInUse;

		public bool isCubic;

		public bool closeCurvl;

		public int stepsOfEachSegment;

		public ListView<TransNode> nodeLst;

		private int curvlID;

		public bool useCamera;

		public Color lineColor = Color.red;

		public string displayName = string.Empty;

		private ListView<GameObject> transNodeObjs;

		private GameObject curvlRootObj;

		private List<Vector3> midpoints;

		private List<Vector3> curvePoints;

		private List<Vector3> extrapoints;

		private Vector3[] controlPoints;

		private int curvePointCount;

		public CurvlData(int id)
		{
			this.isInUse = true;
			this.isHide = false;
			this.curvlID = id;
			this.stepsOfEachSegment = 10;
			this.isCubic = true;
			this.closeCurvl = false;
			this.InitData();
		}

		private void InitData()
		{
			if (this.nodeLst == null)
			{
				this.nodeLst = new ListView<TransNode>();
			}
			if (this.transNodeObjs == null)
			{
				this.transNodeObjs = new ListView<GameObject>();
			}
			if (this.curvlRootObj == null)
			{
				string name = "CurvlRootObject_" + this.curvlID;
				this.curvlRootObj = GameObject.Find(name);
				if (this.curvlRootObj == null)
				{
					GameObject gameObject = GameObject.Find(CurvlHelper.sCurvlHelperObjName);
					if (gameObject == null)
					{
						gameObject = new GameObject();
						gameObject.name = CurvlHelper.sCurvlHelperObjName;
					}
					this.curvlRootObj = new GameObject();
					this.curvlRootObj.name = name;
					this.curvlRootObj.transform.parent = gameObject.transform;
				}
			}
		}

		public int GetCurvlID()
		{
			return this.curvlID;
		}

		public void SetCurvlID(int id)
		{
			this.curvlID = id;
			if (this.curvlRootObj != null)
			{
				this.curvlRootObj.name = "CurvlRootObject_" + this.curvlID;
			}
		}

		public void Remove(bool destroy)
		{
			this.isHide = true;
			this.isInUse = false;
			if (this.transNodeObjs != null)
			{
				foreach (GameObject current in this.transNodeObjs)
				{
					if (current != null)
					{
						if (destroy)
						{
							ActionManager.DestroyGameObject(current);
						}
						else
						{
							current.SetActive(false);
						}
					}
				}
			}
			if (this.curvlRootObj != null)
			{
				if (destroy)
				{
					ActionManager.DestroyGameObject(this.curvlRootObj);
				}
				else
				{
					this.curvlRootObj.SetActive(false);
				}
			}
			if (destroy)
			{
				if (this.transNodeObjs != null)
				{
					this.transNodeObjs.Clear();
					this.transNodeObjs = null;
				}
				if (this.nodeLst != null)
				{
					this.nodeLst.Clear();
					this.nodeLst = null;
				}
				if (this.midpoints != null)
				{
					this.midpoints.Clear();
					this.midpoints = null;
				}
				if (this.curvePoints != null)
				{
					this.curvePoints.Clear();
					this.curvePoints = null;
				}
				if (this.extrapoints != null)
				{
					this.extrapoints.Clear();
					this.extrapoints = null;
				}
			}
		}

		public void RefreshDataToGfx()
		{
			this.InitData();
			int count = this.transNodeObjs.Count;
			int i;
			for (i = 0; i < this.nodeLst.Count; i++)
			{
				TransNode transNode = this.nodeLst[i];
				GameObject gameObject = null;
				string name = "CurvlTransNode_" + i;
				if (i >= count)
				{
					GameObject gameObject2 = this.curvlRootObj;
					if (gameObject2 != null)
					{
						for (int j = 0; j < gameObject2.transform.childCount; j++)
						{
							gameObject = gameObject2.transform.GetChild(j).gameObject;
							string name2 = gameObject.name;
							string[] array = name2.Split(new char[]
							{
								'_'
							});
							int num = -1;
							int.TryParse(array[1], out num);
							if (num == i)
							{
								break;
							}
							gameObject = null;
						}
					}
					if (gameObject == null)
					{
						gameObject = (ActionManager.InstantiateObject(this.transObjPrefb) as GameObject);
						gameObject.name = name;
						gameObject.transform.parent = this.curvlRootObj.transform;
						Camera camera = gameObject.GetComponent<Camera>();
						if (camera == null && this.useCamera)
						{
							camera = gameObject.AddComponent<Camera>();
						}
						else if (camera != null && !this.useCamera)
						{
							UnityEngine.Object.DestroyImmediate(camera);
						}
					}
					this.transNodeObjs.Add(gameObject);
				}
				else
				{
					gameObject = this.transNodeObjs[i];
					if (gameObject == null)
					{
						gameObject = (ActionManager.InstantiateObject(this.transObjPrefb) as GameObject);
						gameObject.name = name;
						gameObject.transform.parent = this.curvlRootObj.transform;
						Camera camera2 = gameObject.GetComponent<Camera>();
						if (camera2 == null && this.useCamera)
						{
							camera2 = gameObject.AddComponent<Camera>();
						}
						else if (camera2 != null && !this.useCamera)
						{
							UnityEngine.Object.DestroyImmediate(camera2);
						}
						this.transNodeObjs[i] = gameObject;
					}
				}
				Transform transform = gameObject.transform;
				gameObject.SetActive(true);
				transform.position = transNode.pos;
				transform.rotation = transNode.rot;
			}
			for (int k = i; k < count; k++)
			{
				this.transNodeObjs[k].SetActive(false);
			}
		}

		public void ReverseNodes()
		{
			if (this.nodeLst != null && this.nodeLst.Count > 1)
			{
				this.nodeLst.Reverse();
				for (int i = this.transNodeObjs.Count - 1; i >= 0; i--)
				{
					if (!this.transNodeObjs[i].activeInHierarchy)
					{
						ActionManager.DestroyGameObject(this.transNodeObjs[i]);
						this.transNodeObjs.RemoveAt(i);
					}
				}
				this.transNodeObjs.Reverse();
			}
		}

		public void ReplaceNodeObjectPrefab(GameObject obj)
		{
			this.transObjPrefb = obj;
			for (int i = 0; i < this.transNodeObjs.Count; i++)
			{
				ActionManager.DestroyGameObject(this.transNodeObjs[i]);
			}
			this.transNodeObjs.Clear();
			this.RefreshDataToGfx();
		}

		public GameObject GetNodeObject(int index)
		{
			if (this.transNodeObjs == null || index < 0 || index >= this.transNodeObjs.Count)
			{
				return null;
			}
			return this.transNodeObjs[index];
		}

		public GameObject GetCurvlRootObject()
		{
			return this.curvlRootObj;
		}

		public TransNode GetNode(int index)
		{
			if (this.nodeLst == null || index < 0 || index >= this.nodeLst.Count)
			{
				return null;
			}
			return this.nodeLst[index];
		}

		public TransNode AddNode(bool needRefreshGfxData)
		{
			if (this.nodeLst == null)
			{
				this.nodeLst = new ListView<TransNode>();
			}
			TransNode transNode = new TransNode();
			this.nodeLst.Add(transNode);
			if (needRefreshGfxData)
			{
				this.RefreshDataToGfx();
			}
			return transNode;
		}

		public TransNode InsertNode(int index, bool needRefreshGfxData)
		{
			if (this.nodeLst == null)
			{
				this.nodeLst = new ListView<TransNode>();
			}
			if (index < 0)
			{
				return null;
			}
			TransNode transNode = new TransNode();
			if (index >= this.nodeLst.Count)
			{
				if (this.nodeLst.Count > 0)
				{
					TransNode transNode2 = this.nodeLst[this.nodeLst.Count - 1];
					transNode.pos = transNode2.pos;
					transNode.isCubic = transNode2.isCubic;
				}
				this.nodeLst.Add(transNode);
			}
			else
			{
				TransNode transNode3 = this.nodeLst[index];
				transNode.pos = transNode3.pos;
				transNode.isCubic = transNode3.isCubic;
				this.nodeLst.Insert(index, transNode);
			}
			if (needRefreshGfxData)
			{
				this.RefreshDataToGfx();
			}
			return transNode;
		}

		public void RemoveNode(int index, bool needRefreshGfxData)
		{
			if (this.nodeLst != null && index >= 0 && index < this.nodeLst.Count)
			{
				this.nodeLst.RemoveAt(index);
				if (needRefreshGfxData)
				{
					this.RefreshDataToGfx();
				}
			}
		}

		public void RemoveAllNodes(bool needRefreshGfxData)
		{
			if (this.nodeLst != null)
			{
				this.nodeLst.Clear();
				if (needRefreshGfxData)
				{
					this.RefreshDataToGfx();
				}
			}
		}

		private void SyncTransformDataToTransNode()
		{
			if (this.nodeLst == null || this.nodeLst.Count == 0)
			{
				return;
			}
			if (this.transNodeObjs == null || this.transNodeObjs.Count < this.nodeLst.Count || this.transNodeObjs[0] == null)
			{
				this.RefreshDataToGfx();
			}
			if (this.transNodeObjs != null)
			{
				for (int i = 0; i < this.nodeLst.Count; i++)
				{
					TransNode transNode = this.nodeLst[i];
					Transform transform = this.transNodeObjs[i].transform;
					transNode.pos = transform.position;
					transNode.rot = transform.rotation;
					transNode.scl = transform.localScale;
				}
			}
		}

		private void CreateCurvlPoints(bool useUnifyInterp)
		{
			if (this.curvePoints == null)
			{
				this.curvePoints = new List<Vector3>();
			}
			if (this.controlPoints == null)
			{
				this.controlPoints = new Vector3[4];
			}
			int count = this.nodeLst.Count;
			this.curvePointCount = 0;
			float stepLen = 1f / (float)this.stepsOfEachSegment;
			for (int i = 0; i < count; i++)
			{
				if (this.closeCurvl || i != 0)
				{
					int num = i - 1;
					if (num < 0)
					{
						if (this.closeCurvl)
						{
							num = count - 1;
						}
						else
						{
							num = 0;
						}
					}
					int num2 = i - 2;
					if (num2 < 0)
					{
						if (this.closeCurvl)
						{
							num2 = count - 1;
						}
						else
						{
							num2 = 0;
						}
					}
					int num3 = i + 1;
					if (num3 >= count)
					{
						if (this.closeCurvl)
						{
							num3 = 0;
						}
						else
						{
							num3 = i;
						}
					}
					Vector3 pos = this.nodeLst[num].pos;
					Vector3 pos2 = this.nodeLst[i].pos;
					Vector3 pos3 = this.nodeLst[num2].pos;
					Vector3 pos4 = this.nodeLst[num3].pos;
					if ((useUnifyInterp && !this.isCubic) || (!useUnifyInterp && !this.nodeLst[i].isCubic))
					{
						if (this.curvePoints.Count <= this.curvePointCount)
						{
							this.curvePoints.Add(pos);
						}
						else
						{
							this.curvePoints[this.curvePointCount] = pos;
						}
						this.curvePointCount++;
						if (this.curvePoints.Count <= this.curvePointCount)
						{
							this.curvePoints.Add(pos2);
						}
						else
						{
							this.curvePoints[this.curvePointCount] = pos2;
						}
						this.curvePointCount++;
					}
					else
					{
						Vector3 ctrlPoint;
						Vector3 ctrlPoint2;
						CurvlData.CalculateCtrlPoint(pos3, pos, pos2, pos4, out ctrlPoint, out ctrlPoint2);
						this.CalculateCurvlPoints(pos, pos2, ctrlPoint, ctrlPoint2, stepLen);
					}
				}
			}
		}

		public static void CalculateCtrlPoint(Vector3 formPoint, Vector3 prevPoint, Vector3 curnPoint, Vector3 lattPoint, out Vector3 ctrlPoint1, out Vector3 ctrlPoint2)
		{
			Vector3 a = (formPoint + prevPoint) * 0.5f;
			Vector3 vector = (curnPoint + prevPoint) * 0.5f;
			Vector3 a2 = (curnPoint + lattPoint) * 0.5f;
			Vector3 b = (a + vector) * 0.5f;
			Vector3 b2 = (a2 + vector) * 0.5f;
			Vector3 a3 = vector - b;
			Vector3 a4 = vector - b2;
			float d = CurvlHelper.sCtrlPtScale;
			float d2 = CurvlHelper.sCtrlPtScale;
			float magnitude = a3.magnitude;
			float magnitude2 = a4.magnitude;
			float num = (curnPoint - prevPoint).magnitude * 0.5f;
			if (num < magnitude)
			{
				d = num / magnitude;
			}
			if (num < magnitude2)
			{
				d2 = num / magnitude2;
			}
			ctrlPoint1 = prevPoint + a3 * d;
			ctrlPoint2 = curnPoint + a4 * d2;
		}

		private void CalculateCurvlPoints(Vector3 prevPoint, Vector3 curnPoint, Vector3 ctrlPoint1, Vector3 ctrlPoint2, float stepLen)
		{
			bool flag = false;
			float num = 0f;
			while (num <= 1f)
			{
				float d = 1f - num;
				float d2 = num;
				Vector3 vector = prevPoint * d * d * d + ctrlPoint1 * 3f * d * d * d2 + ctrlPoint2 * 3f * d * d2 * d2 + curnPoint * d2 * d2 * d2;
				if (this.curvePoints.Count <= this.curvePointCount)
				{
					this.curvePoints.Add(vector);
				}
				else
				{
					this.curvePoints[this.curvePointCount] = vector;
				}
				this.curvePointCount++;
				num += stepLen;
				if (flag)
				{
					break;
				}
				if (num >= 1f)
				{
					num = 1f;
					flag = true;
				}
			}
		}

		private void UpdateNodeShowState(bool show)
		{
			int count = this.nodeLst.Count;
			for (int i = 0; i < this.transNodeObjs.Count; i++)
			{
				if (i < count)
				{
					this.transNodeObjs[i].SetActive(show);
				}
				else
				{
					this.transNodeObjs[i].SetActive(false);
				}
			}
		}

		public void DrawCurvel(bool useUnifyInterp)
		{
			this.UpdateNodeShowState(this.isInUse && !this.isHide);
			if (!this.isInUse || this.isHide)
			{
				return;
			}
			this.curvlRootObj.SetActive(true);
			this.SyncTransformDataToTransNode();
			this.CreateCurvlPoints(useUnifyInterp);
			for (int i = 0; i < this.curvePointCount - 1; i++)
			{
				Vector3 start = this.curvePoints[i];
				Vector3 end = this.curvePoints[i + 1];
				Debug.DrawLine(start, end, this.lineColor);
			}
		}

		public void SetUseCameraComp(bool bEnable)
		{
			this.useCamera = bEnable;
			foreach (GameObject current in this.transNodeObjs)
			{
				if (current != null)
				{
					Camera camera = current.GetComponent<Camera>();
					if (camera == null && this.useCamera)
					{
						camera = current.AddComponent<Camera>();
					}
					else if (camera != null && !this.useCamera)
					{
						UnityEngine.Object.DestroyImmediate(camera);
					}
				}
			}
		}

		public void UnifyCubicPropertyToAllNodes(bool cubic)
		{
			this.isCubic = cubic;
			foreach (TransNode current in this.nodeLst)
			{
				current.isCubic = cubic;
			}
		}
	}
}
