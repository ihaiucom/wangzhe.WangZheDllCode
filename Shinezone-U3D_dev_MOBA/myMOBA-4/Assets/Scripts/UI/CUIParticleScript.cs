using Assets.Scripts.Framework;
using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIParticleScript : CUIComponent
	{
		public string m_resPath = string.Empty;

		public bool m_isFixScaleToForm;

		public bool m_isFixScaleToParticleSystem;

		private Renderer[] m_renderers;

		private int m_rendererCount;

		private void ClearRes()
		{
			this.m_renderers = null;
			this.m_rendererCount = 0;
			if (base.gameObject.transform.childCount > 0)
			{
				Transform child = base.gameObject.transform.GetChild(0);
				if (child != null)
				{
					child.SetParent(null);
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
		}

		private void LoadRes()
		{
			string text = this.m_resPath;
			if (!string.IsNullOrEmpty(text))
			{
				if (GameSettings.ParticleQuality == SGameRenderQuality.Low)
				{
					text = string.Concat(new string[]
					{
						CUIUtility.s_Particle_Dir,
						this.m_resPath,
						"/",
						this.m_resPath,
						"_low.prefeb"
					});
				}
				else if (GameSettings.ParticleQuality == SGameRenderQuality.Medium)
				{
					text = string.Concat(new string[]
					{
						CUIUtility.s_Particle_Dir,
						this.m_resPath,
						"/",
						this.m_resPath,
						"_mid.prefeb"
					});
				}
				else
				{
					text = string.Concat(new string[]
					{
						CUIUtility.s_Particle_Dir,
						this.m_resPath,
						"/",
						this.m_resPath,
						".prefeb"
					});
				}
				GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource(text, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
				if (gameObject != null && base.gameObject.transform.childCount == 0)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
					gameObject2.transform.SetParent(base.gameObject.transform);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.localScale = Vector3.one;
				}
			}
		}

		public void LoadRes(string resName)
		{
			if (!this.m_isInitialized)
			{
				return;
			}
			this.m_resPath = resName;
			this.ClearRes();
			this.LoadRes();
			this.InitializeRenderers();
			this.SetSortingOrder(this.m_belongedFormScript.GetSortingOrder());
			if (this.m_isFixScaleToForm)
			{
				this.ResetScale();
			}
			if (this.m_isFixScaleToParticleSystem)
			{
				this.ResetParticleScale();
			}
			if (this.m_belongedFormScript.IsHided())
			{
				this.Hide();
			}
		}

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.LoadRes();
			this.InitializeRenderers();
			base.Initialize(formScript);
			if (this.m_isFixScaleToForm)
			{
				this.ResetScale();
			}
			if (this.m_isFixScaleToParticleSystem)
			{
				this.ResetParticleScale();
			}
			if (this.m_belongedFormScript.IsHided())
			{
				this.Hide();
			}
		}

		protected override void OnDestroy()
		{
			this.m_renderers = null;
			base.OnDestroy();
		}

		public override void Hide()
		{
			base.Hide();
			CUIUtility.SetGameObjectLayer(base.gameObject, 31);
		}

		public override void Appear()
		{
			base.Appear();
			CUIUtility.SetGameObjectLayer(base.gameObject, 5);
		}

		public override void SetSortingOrder(int sortingOrder)
		{
			base.SetSortingOrder(sortingOrder);
			for (int i = 0; i < this.m_rendererCount; i++)
			{
				this.m_renderers[i].sortingOrder = sortingOrder;
			}
		}

		private void InitializeRenderers()
		{
			this.m_renderers = new Renderer[100];
			this.m_rendererCount = 0;
			CUIUtility.GetComponentsInChildren<Renderer>(base.gameObject, this.m_renderers, ref this.m_rendererCount);
		}

		private void ResetScale()
		{
			float num = 1f / this.m_belongedFormScript.gameObject.transform.localScale.x;
			base.gameObject.transform.localScale = new Vector3(num, num, 0f);
		}

		private void ResetParticleScale()
		{
			if (this.m_belongedFormScript == null)
			{
				return;
			}
			float scale = 1f;
			RectTransform component = this.m_belongedFormScript.GetComponent<RectTransform>();
			if (this.m_belongedFormScript.m_canvasScaler.matchWidthOrHeight == 0f)
			{
				scale = component.rect.width / component.rect.height / (this.m_belongedFormScript.m_canvasScaler.referenceResolution.x / this.m_belongedFormScript.m_canvasScaler.referenceResolution.y);
			}
			else if (this.m_belongedFormScript.m_canvasScaler.matchWidthOrHeight == 1f)
			{
			}
			this.InitializeParticleScaler(base.gameObject, scale);
		}

		private void InitializeParticleScaler(GameObject gameObject, float scale)
		{
			ParticleScaler particleScaler = gameObject.GetComponent<ParticleScaler>();
			if (particleScaler == null)
			{
				particleScaler = gameObject.AddComponent<ParticleScaler>();
			}
			if (particleScaler.particleScale != scale)
			{
				particleScaler.particleScale = scale;
				particleScaler.alsoScaleGameobject = false;
				particleScaler.CheckAndApplyScale();
			}
		}
	}
}
