using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CUIParticleSystem : Singleton<CUIParticleSystem>
	{
		public static string s_qualifyingFormPath = "UGUI/Form/Common/Form_Particle.prefab";

		public static string s_particleTest_Path = CUIUtility.s_Animation3D_Dir + "Test";

		public static string s_particleSkillBtnEffect_Path = CUIUtility.s_Animation3D_Dir + "UI_Effect_02";

		private ListView<UIParticleInfo> m_particleList = new ListView<UIParticleInfo>();

		private CUI3DImageScript m_particleContainerScript;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ParticlTimer, new CUIEventManager.OnUIEventHandler(this.OnTimer));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ParticlShow, new CUIEventManager.OnUIEventHandler(this.Show));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ParticlHide, new CUIEventManager.OnUIEventHandler(this.Hide));
			this.Open();
		}

		public void Show(CUIEvent evt = null)
		{
			if (this.m_particleContainerScript != null)
			{
				Camera component = this.m_particleContainerScript.GetComponent<Camera>();
				if (component != null)
				{
					component.cullingMask = 1 << CUI3DImageScript.s_cameraLayers[1];
				}
			}
		}

		public void Hide(CUIEvent evt = null)
		{
			if (this.m_particleContainerScript != null)
			{
				Camera component = this.m_particleContainerScript.GetComponent<Camera>();
				if (component != null)
				{
					component.cullingMask = 0;
				}
			}
		}

		public void Open()
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(CUIParticleSystem.s_qualifyingFormPath) != null)
			{
				return;
			}
			this.m_particleList = new ListView<UIParticleInfo>();
			this.m_particleContainerScript = null;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUIParticleSystem.s_qualifyingFormPath, false, false);
			if (cUIFormScript != null)
			{
				Transform transform = cUIFormScript.transform.Find("3DImage");
				if (transform != null)
				{
					this.m_particleContainerScript = transform.GetComponent<CUI3DImageScript>();
				}
				Transform transform2 = cUIFormScript.transform.Find("txtInfo");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnTimer(CUIEvent uiEvent)
		{
			for (int i = 0; i < this.m_particleList.Count; i++)
			{
				if (this.m_particleList[i].totlalTime >= 0f)
				{
					this.m_particleList[i].currentTime += 0.1f;
					if (this.m_particleList[i].currentTime >= this.m_particleList[i].totlalTime)
					{
						if (this.m_particleContainerScript != null)
						{
							this.m_particleContainerScript.RemoveGameObject(this.m_particleList[i].parObj);
						}
						this.m_particleList.RemoveAt(i);
						i--;
					}
				}
			}
		}

		public void ClearAll(bool isClearCycle = false)
		{
			for (int i = 0; i < this.m_particleList.Count; i++)
			{
				bool flag = true;
				if (this.m_particleList[i].totlalTime < 0f && !isClearCycle)
				{
					flag = false;
				}
				if (flag)
				{
					if (this.m_particleContainerScript != null)
					{
						this.m_particleContainerScript.RemoveGameObject(this.m_particleList[i].parObj);
					}
					this.m_particleList.RemoveAt(i);
					i--;
				}
			}
		}

		public void RemoveParticle(UIParticleInfo pInfo)
		{
			if (pInfo == null)
			{
				return;
			}
			if (this.m_particleContainerScript != null)
			{
				this.m_particleContainerScript.RemoveGameObject(pInfo.parObj);
			}
			pInfo.parObj = null;
			this.m_particleList.Remove(pInfo);
		}

		public UIParticleInfo AddParticle(string parPath, float playTime, Vector2 sreenLoc, Quaternion? locRotation = null)
		{
			if (this.m_particleContainerScript == null)
			{
				return null;
			}
			UIParticleInfo uIParticleInfo = new UIParticleInfo();
			uIParticleInfo.path = parPath;
			uIParticleInfo.currentTime = 0f;
			uIParticleInfo.totlalTime = playTime;
			uIParticleInfo.parObj = this.m_particleContainerScript.AddGameObject(uIParticleInfo.path, false, ref sreenLoc, false);
			if (uIParticleInfo.parObj != null && locRotation.get_HasValue())
			{
				uIParticleInfo.parObj.transform.localRotation = locRotation.get_Value();
			}
			this.m_particleList.Add(uIParticleInfo);
			return uIParticleInfo;
		}

		public UIParticleInfo AddParticle(string parPath, float playTime, GameObject targetLoc, CUIFormScript targetFormScript, Quaternion? locRotation = null)
		{
			if (targetLoc == null || targetFormScript == null)
			{
				return null;
			}
			Vector2 sreenLoc = CUIUtility.WorldToScreenPoint(targetFormScript.GetCamera(), targetLoc.transform.position);
			return this.AddParticle(parPath, playTime, sreenLoc, locRotation);
		}

		public void SetParticleScreenPosition(UIParticleInfo uiParticleInfo, ref Vector2 screenPosition)
		{
			if (this.m_particleContainerScript != null && uiParticleInfo != null && uiParticleInfo.parObj != null)
			{
				this.m_particleContainerScript.ChangeScreenPositionToWorld(uiParticleInfo.parObj, ref screenPosition);
			}
		}
	}
}
