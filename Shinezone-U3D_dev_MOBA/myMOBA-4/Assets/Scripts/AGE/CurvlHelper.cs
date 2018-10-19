using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	public class CurvlHelper : MonoBehaviour
	{
		public static string sCurvlHelperObjName = "CurvlHelper";

		public static float sCtrlPtScale = 0.6f;

		private ListView<CurvlData> curvlLst;

		public float ctrlPointScale = 0.6f;

		public int lineStep = 15;

		public bool useSameInterpMode;

		public bool closeLine;

		public bool showOnGame;

		public bool showOnGizmo = true;

		public DictionaryObjectView<CurvlData, Track> mCurvlTrackMap = new DictionaryObjectView<CurvlData, Track>();

		public Action mPreviewAction;

		private static Vector3 axisWeight = new Vector3(1f, 0f, 1f);

		private void Start()
		{
		}

		private void Update()
		{
			CurvlHelper.sCtrlPtScale = this.ctrlPointScale;
			if (this.showOnGame)
			{
				this.Draw();
			}
		}

		private void OnDrawGizmos()
		{
			CurvlHelper.sCtrlPtScale = this.ctrlPointScale;
			if (this.showOnGizmo)
			{
				this.Draw();
			}
		}

		private void HideUnactiveCurvls()
		{
			GameObject gameObject = GameObject.Find(CurvlHelper.sCurvlHelperObjName);
			if (gameObject != null)
			{
				Transform transform = gameObject.transform;
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					child.gameObject.SetActive(false);
				}
			}
		}

		public void Draw()
		{
			if (this.curvlLst == null)
			{
				this.HideUnactiveCurvls();
				return;
			}
			foreach (CurvlData current in this.curvlLst)
			{
				if (current != null)
				{
					current.stepsOfEachSegment = this.lineStep;
					current.DrawCurvel(this.useSameInterpMode);
				}
			}
		}

		public static int[] GetSelectedCurvl()
		{
			return null;
		}

		public int GetCurvlCount()
		{
			if (this.curvlLst == null)
			{
				return 0;
			}
			return this.curvlLst.Count;
		}

		public CurvlData CreateCurvl()
		{
			if (this.curvlLst == null)
			{
				this.curvlLst = new ListView<CurvlData>();
			}
			CurvlData curvlData;
			for (int i = 0; i < this.curvlLst.Count; i++)
			{
				curvlData = this.curvlLst[i];
				if (curvlData == null)
				{
					curvlData = new CurvlData(i);
					curvlData.SetCurvlID(i);
					this.curvlLst[i] = curvlData;
					return curvlData;
				}
				if (!curvlData.isInUse)
				{
					curvlData.isInUse = true;
					return curvlData;
				}
			}
			curvlData = new CurvlData(this.curvlLst.Count);
			curvlData.SetCurvlID(this.curvlLst.Count);
			this.curvlLst.Add(curvlData);
			return curvlData;
		}

		public CurvlData InsertCurvl(int index)
		{
			if (this.curvlLst == null)
			{
				this.curvlLst = new ListView<CurvlData>();
			}
			if (index < 0)
			{
				return null;
			}
			CurvlData curvlData;
			if (index >= this.curvlLst.Count)
			{
				curvlData = new CurvlData(this.curvlLst.Count);
				curvlData.SetCurvlID(this.curvlLst.Count);
				this.curvlLst.Add(curvlData);
			}
			else
			{
				curvlData = this.curvlLst[index];
				if (curvlData == null)
				{
					curvlData = new CurvlData(index);
					curvlData.SetCurvlID(index);
					this.curvlLst[index] = curvlData;
				}
				else if (!curvlData.isInUse)
				{
					curvlData.isInUse = true;
				}
				else
				{
					this.curvlLst.Insert(index, null);
					for (int i = index + 1; i < this.curvlLst.Count; i++)
					{
						CurvlData curvlData2 = this.curvlLst[i];
						if (curvlData2 != null)
						{
							curvlData2.SetCurvlID(i);
						}
					}
					curvlData = new CurvlData(index);
					curvlData.SetCurvlID(index);
					this.curvlLst[index] = curvlData;
				}
			}
			return curvlData;
		}

		public CurvlData GetCurvl(int index)
		{
			if (this.curvlLst == null || index < 0 || index >= this.curvlLst.Count)
			{
				return null;
			}
			return this.curvlLst[index];
		}

		public void RemoveCurvl(int index, bool destroy)
		{
			CurvlData curvlData = this.GetCurvl(index);
			if (curvlData != null)
			{
				curvlData.Remove(destroy);
				if (destroy)
				{
					this.curvlLst.RemoveAt(index);
					for (int i = index; i < this.curvlLst.Count; i++)
					{
						curvlData = this.curvlLst[i];
						if (curvlData != null)
						{
							curvlData.SetCurvlID(i);
						}
					}
				}
			}
		}

		public void RemoveAllCurvl(bool destroy)
		{
			if (this.curvlLst != null)
			{
				foreach (CurvlData current in this.curvlLst)
				{
					if (current != null)
					{
						current.Remove(destroy);
						if (!destroy)
						{
							this.curvlLst[current.GetCurvlID()] = null;
						}
					}
				}
				if (destroy)
				{
					this.curvlLst.Clear();
				}
			}
			if (destroy)
			{
				GameObject gameObject = GameObject.Find(CurvlHelper.sCurvlHelperObjName);
				if (gameObject)
				{
					List<GameObject> list = new List<GameObject>();
					int childCount = gameObject.transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform child = gameObject.transform.GetChild(i);
						int childCount2 = child.childCount;
						for (int j = 0; j < childCount2; j++)
						{
							Transform child2 = child.GetChild(j);
							list.Add(child2.gameObject);
						}
						list.Add(child.gameObject);
					}
					foreach (GameObject current2 in list)
					{
						ActionManager.DestroyGameObject(current2);
					}
				}
			}
		}

		public static bool CalTransform(ModifyTransform evt, Transform dstobj, Transform fromTransform, Transform toTransform, Transform coordTransform, ref Vector3 oPos, ref Quaternion oRot, ref Vector3 oScl)
		{
			if (dstobj == null)
			{
				return false;
			}
			if (fromTransform != null && toTransform != null)
			{
				Vector3 vector = toTransform.position - fromTransform.position;
				vector = new Vector3(vector.x * CurvlHelper.axisWeight.x, vector.y * CurvlHelper.axisWeight.y, vector.z * CurvlHelper.axisWeight.z);
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				float magnitude = vector2.magnitude;
				vector = Vector3.Normalize(vector);
				Quaternion quaternion = Quaternion.Inverse(Quaternion.LookRotation(vector, Vector3.up));
				if (evt.normalizedRelative)
				{
					oPos = quaternion * (dstobj.position - fromTransform.position);
					oPos = new Vector3(oPos.x / magnitude, oPos.y, oPos.z / magnitude);
					oPos -= new Vector3(0f, 1f, 0f) * oPos.z * (toTransform.position.y - fromTransform.position.y);
				}
				else
				{
					oPos = quaternion * (dstobj.position - fromTransform.position);
					oPos -= new Vector3(0f, 1f, 0f) * (oPos.z / magnitude) * (toTransform.position.y - fromTransform.position.y);
				}
				oRot = quaternion * dstobj.rotation;
				oScl = dstobj.localScale;
			}
			else if (coordTransform != null)
			{
				oPos = coordTransform.InverseTransformPoint(dstobj.position);
				oRot = Quaternion.Inverse(coordTransform.rotation) * dstobj.rotation;
				oScl = dstobj.localScale;
			}
			else
			{
				oPos = dstobj.position;
				oRot = dstobj.rotation;
				oScl = dstobj.localScale;
			}
			return true;
		}
	}
}
