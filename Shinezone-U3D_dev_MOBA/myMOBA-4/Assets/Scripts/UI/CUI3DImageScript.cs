using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	[ExecuteInEditMode]
	public class CUI3DImageScript : CUIComponent
	{
		private class C3DGameObject
		{
			public string m_path;

			public GameObject m_gameObject;

			public bool m_useGameObjectPool;

			public bool m_protogenic;

			public bool m_bindPivot;
		}

		public en3DImageLayer m_imageLayer;

		public Vector3 m_renderCameraDefaultScale = Vector3.one;

		public float m_renderCameraDefaultSize = 20f;

		public Camera m_renderCamera;

		private Light m_renderLight;

		public static int[] s_cameraLayers = new int[]
		{
			16,
			17
		};

		public static int[] s_cameraDepths = new int[]
		{
			9,
			11
		};

		private Vector2 m_pivotScreenPosition;

		private Vector2 m_lastPivotScreenPosition;

		private ListView<CUI3DImageScript.C3DGameObject> m_3DGameObjects = new ListView<CUI3DImageScript.C3DGameObject>();

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_renderCamera = base.gameObject.GetComponent<Camera>();
			this.m_renderLight = base.gameObject.GetComponent<Light>();
			this.InitializeRender();
			this.GetPivotScreenPosition();
			this.Initialize3DGameObjects();
		}

		protected override void OnDestroy()
		{
			this.m_renderCamera = null;
			this.m_renderLight = null;
			this.m_3DGameObjects.Clear();
			this.m_3DGameObjects = null;
			base.OnDestroy();
		}

		public override void Close()
		{
			base.Close();
			int i = 0;
			while (i < this.m_3DGameObjects.Count)
			{
				if (!this.m_3DGameObjects[i].m_protogenic)
				{
					if (this.m_3DGameObjects[i].m_gameObject != null)
					{
						if (this.m_3DGameObjects[i].m_useGameObjectPool)
						{
							Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_3DGameObjects[i].m_gameObject);
						}
						else
						{
							CUICommonSystem.DestoryObj(this.m_3DGameObjects[i].m_gameObject, 0.1f);
						}
					}
					this.m_3DGameObjects[i].m_path = null;
					this.m_3DGameObjects[i].m_gameObject = null;
					this.m_3DGameObjects.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		public override void Hide()
		{
			base.Hide();
			if (this.m_renderCamera != null)
			{
				this.m_renderCamera.enabled = false;
			}
			for (int i = 0; i < this.m_3DGameObjects.Count; i++)
			{
				CUIUtility.SetGameObjectLayer(this.m_3DGameObjects[i].m_gameObject, 31);
			}
		}

		public override void Appear()
		{
			base.Appear();
			if (this.m_renderCamera != null)
			{
				this.m_renderCamera.enabled = true;
			}
			for (int i = 0; i < this.m_3DGameObjects.Count; i++)
			{
				CUIUtility.SetGameObjectLayer(this.m_3DGameObjects[i].m_gameObject, CUI3DImageScript.s_cameraLayers[(int)this.m_imageLayer]);
			}
		}

		private void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			this.GetPivotScreenPosition();
			if (this.m_lastPivotScreenPosition != this.m_pivotScreenPosition)
			{
				if (this.m_renderCamera != null)
				{
					if (this.m_renderCamera.orthographic)
					{
						for (int i = 0; i < this.m_3DGameObjects.Count; i++)
						{
							if (this.m_3DGameObjects[i].m_bindPivot)
							{
								this.ChangeScreenPositionToWorld(this.m_3DGameObjects[i].m_gameObject, ref this.m_pivotScreenPosition);
							}
						}
					}
					else
					{
						float num = this.m_pivotScreenPosition.x / (float)Mathf.Max(Screen.width, Screen.height);
						num = num * 2f - 1f;
						this.m_renderCamera.rect = new Rect(0f, 0f, 1f, 1f);
						this.m_renderCamera.ResetAspect();
						this.m_renderCamera.SetOffsetX(num);
					}
				}
				this.m_lastPivotScreenPosition = this.m_pivotScreenPosition;
			}
		}

		public GameObject AddGameObject(string path, bool useGameObjectPool, bool needCached = false)
		{
			return this.AddGameObject(path, useGameObjectPool, ref this.m_pivotScreenPosition, true, needCached, null);
		}

		public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool needCached = false)
		{
			return this.AddGameObject(path, useGameObjectPool, ref screenPosition, false, needCached, null);
		}

		public GameObject AddGameObjectToPath(string path, bool useGameObjectPool, string pathToAdd)
		{
			return this.AddGameObject(path, useGameObjectPool, ref this.m_pivotScreenPosition, false, false, pathToAdd);
		}

		public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool bindPivot, bool needCached, string pathToAdd)
		{
			GameObject gameObject = null;
			if (useGameObjectPool)
			{
				gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(path, enResourceType.UI3DImage);
			}
			else
			{
				GameObject gameObject2 = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(GameObject), enResourceType.UI3DImage, needCached, false).m_content;
				if (gameObject2 != null)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(gameObject2);
				}
			}
			if (gameObject == null)
			{
				return null;
			}
			Vector3 localScale = gameObject.transform.localScale;
			if (pathToAdd == null)
			{
				gameObject.transform.SetParent(base.gameObject.transform, true);
			}
			else
			{
				Transform transform = base.gameObject.transform.Find(pathToAdd);
				if (transform)
				{
					gameObject.transform.SetParent(transform, true);
				}
			}
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			CUI3DImageScript.C3DGameObject c3DGameObject = new CUI3DImageScript.C3DGameObject();
			c3DGameObject.m_gameObject = gameObject;
			c3DGameObject.m_path = path;
			c3DGameObject.m_useGameObjectPool = useGameObjectPool;
			c3DGameObject.m_protogenic = false;
			c3DGameObject.m_bindPivot = bindPivot;
			this.m_3DGameObjects.Add(c3DGameObject);
			if (this.m_renderCamera.orthographic)
			{
				this.ChangeScreenPositionToWorld(gameObject, ref screenPosition);
				if (!this.m_renderCamera.enabled && this.m_3DGameObjects.Count > 0)
				{
					this.m_renderCamera.enabled = true;
				}
			}
			else
			{
				Transform transform2 = base.transform.FindChild("_root");
				if (transform2 != null)
				{
					if (pathToAdd == null)
					{
						gameObject.transform.SetParent(transform2, true);
					}
					else
					{
						Transform transform3 = base.gameObject.transform.Find(pathToAdd);
						if (transform3)
						{
							gameObject.transform.SetParent(transform3, true);
						}
					}
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = localScale;
				}
			}
			CUIUtility.SetGameObjectLayer(gameObject, (!this.m_renderCamera.enabled) ? 31 : CUI3DImageScript.s_cameraLayers[(int)this.m_imageLayer]);
			return gameObject;
		}

		public void RemoveGameObject(string path)
		{
			int i = 0;
			while (i < this.m_3DGameObjects.Count)
			{
				if (string.Equals(this.m_3DGameObjects[i].m_path, path, StringComparison.OrdinalIgnoreCase))
				{
					if (this.m_3DGameObjects[i].m_useGameObjectPool)
					{
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_3DGameObjects[i].m_gameObject);
					}
					else
					{
						CUICommonSystem.DestoryObj(this.m_3DGameObjects[i].m_gameObject, 0.1f);
					}
					this.m_3DGameObjects.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			if (this.m_3DGameObjects.Count <= 0)
			{
				this.m_renderCamera.enabled = false;
			}
		}

		public void RemoveGameObject(GameObject removeObj)
		{
			if (removeObj == null)
			{
				return;
			}
			int i = 0;
			while (i < this.m_3DGameObjects.Count)
			{
				if (this.m_3DGameObjects[i].m_gameObject == removeObj)
				{
					if (this.m_3DGameObjects[i].m_useGameObjectPool)
					{
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_3DGameObjects[i].m_gameObject);
					}
					else
					{
						CUICommonSystem.DestoryObj(this.m_3DGameObjects[i].m_gameObject, 0.1f);
					}
					this.m_3DGameObjects.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			if (this.m_3DGameObjects.Count <= 0)
			{
				this.m_renderCamera.enabled = false;
			}
		}

		public GameObject GetGameObject(string path)
		{
			for (int i = 0; i < this.m_3DGameObjects.Count; i++)
			{
				if (this.m_3DGameObjects[i].m_path.Equals(path))
				{
					return this.m_3DGameObjects[i].m_gameObject;
				}
			}
			return null;
		}

		public void ChangeScreenPositionToWorld(string path, ref Vector2 screenPosition)
		{
			this.ChangeScreenPositionToWorld(this.GetGameObject(path), ref screenPosition);
		}

		public void ChangeScreenPositionToWorld(GameObject gameObject, ref Vector2 screenPosition)
		{
			if (gameObject == null)
			{
				return;
			}
			gameObject.transform.position = CUIUtility.ScreenToWorldPoint(this.m_renderCamera, screenPosition, 100f);
		}

		public Vector2 GetPivotScreenPosition()
		{
			this.m_pivotScreenPosition = CUIUtility.WorldToScreenPoint(this.m_belongedFormScript.GetCamera(), base.gameObject.transform.position);
			return this.m_pivotScreenPosition;
		}

		public void InitializeRender()
		{
			if (this.m_renderCamera != null)
			{
				this.m_renderCamera.clearFlags = CameraClearFlags.Depth;
				this.m_renderCamera.cullingMask = 1 << CUI3DImageScript.s_cameraLayers[(int)this.m_imageLayer];
				this.m_renderCamera.depth = (float)CUI3DImageScript.s_cameraDepths[(int)this.m_imageLayer];
				if (this.m_renderCamera.orthographic)
				{
					this.m_renderCamera.orthographicSize = this.m_renderCameraDefaultSize * ((this.m_belongedFormScript.transform as RectTransform).rect.height / this.m_belongedFormScript.GetReferenceResolution().y);
				}
				else
				{
					this.m_renderCamera.gameObject.transform.localScale = this.m_renderCameraDefaultScale * (1f / ((this.m_belongedFormScript.gameObject.transform.localScale.x != 0f) ? this.m_belongedFormScript.gameObject.transform.localScale.x : 1f));
				}
			}
			if (this.m_renderLight != null)
			{
				this.m_renderLight.cullingMask = 1 << CUI3DImageScript.s_cameraLayers[(int)this.m_imageLayer];
			}
		}

		private void Initialize3DGameObjects()
		{
			this.m_3DGameObjects.Clear();
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
				CUIUtility.SetGameObjectLayer(gameObject, CUI3DImageScript.s_cameraLayers[(int)this.m_imageLayer]);
				if (this.m_renderCamera.orthographic)
				{
					this.ChangeScreenPositionToWorld(gameObject, ref this.m_pivotScreenPosition);
				}
				CUI3DImageScript.C3DGameObject c3DGameObject = new CUI3DImageScript.C3DGameObject();
				c3DGameObject.m_path = gameObject.name;
				c3DGameObject.m_gameObject = gameObject;
				c3DGameObject.m_useGameObjectPool = false;
				c3DGameObject.m_protogenic = true;
				c3DGameObject.m_bindPivot = true;
				this.m_3DGameObjects.Add(c3DGameObject);
			}
			this.m_renderCamera.enabled = (this.m_3DGameObjects.Count > 0);
		}
	}
}
