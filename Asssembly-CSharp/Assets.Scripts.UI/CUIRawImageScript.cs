using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIRawImageScript : CUIComponent
	{
		private const int c_uiRawLayer = 15;

		private Camera m_renderTextureCamera;

		private GameObject m_rawRootObject;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_renderTextureCamera = base.GetComponentInChildren<Camera>(base.gameObject);
			if (this.m_renderTextureCamera != null)
			{
				Transform transform = this.m_renderTextureCamera.gameObject.transform.FindChild("RawRoot");
				if (transform != null)
				{
					this.m_rawRootObject = transform.gameObject;
				}
			}
		}

		protected override void OnDestroy()
		{
			this.m_renderTextureCamera = null;
			this.m_rawRootObject = null;
			base.OnDestroy();
		}

		public override void Hide()
		{
			base.Hide();
			CUIUtility.SetGameObjectLayer(this.m_rawRootObject, 31);
		}

		public override void Appear()
		{
			base.Appear();
			CUIUtility.SetGameObjectLayer(this.m_rawRootObject, 15);
		}

		public void AddGameObject(string name, GameObject rawObject, Vector3 position, Quaternion rotation, Vector3 scaler)
		{
			if (this.m_rawRootObject == null)
			{
				return;
			}
			this.SetRawObjectLayer(rawObject, LayerMask.NameToLayer("UIRaw"));
			rawObject.name = name;
			rawObject.transform.SetParent(this.m_rawRootObject.transform);
			rawObject.transform.localPosition = position;
			rawObject.transform.localRotation = rotation;
			rawObject.transform.localScale = scaler;
		}

		public GameObject RemoveGameObject(string name)
		{
			if (this.m_rawRootObject == null)
			{
				return null;
			}
			for (int i = 0; i < this.m_rawRootObject.transform.childCount; i++)
			{
				GameObject gameObject = this.m_rawRootObject.transform.GetChild(i).gameObject;
				if (gameObject.name.Equals(name))
				{
					gameObject.transform.SetParent(null);
					return gameObject;
				}
			}
			return null;
		}

		public GameObject GetGameObject(string name)
		{
			GameObject result = null;
			if (this.m_rawRootObject == null)
			{
				return null;
			}
			for (int i = 0; i < this.m_rawRootObject.transform.childCount; i++)
			{
				GameObject gameObject = this.m_rawRootObject.transform.GetChild(i).gameObject;
				if (gameObject.name.Equals(name))
				{
					result = gameObject;
					break;
				}
			}
			return result;
		}

		public void SetRawObjectLayer(GameObject rawObject, int layer)
		{
			rawObject.layer = layer;
			for (int i = 0; i < rawObject.transform.childCount; i++)
			{
				this.SetRawObjectLayer(rawObject.transform.GetChild(i).gameObject, layer);
			}
		}
	}
}
