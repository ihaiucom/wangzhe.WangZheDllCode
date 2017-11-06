using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CHeroAnimaSystem : Singleton<CHeroAnimaSystem>
	{
		private List<string> m_animatList = new List<string>();

		private List<string> m_clikAnimaList = new List<string>();

		private ListView<AnimaSoundElement> m_animatSoundList = new ListView<AnimaSoundElement>();

		private static string[] s_noClikAnima = new string[]
		{
			"Come",
			"Cheer"
		};

		private ulong m_tickAnimat;

		private GameObject m_3DModel;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ModelDrag, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_DragModel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ModelClick, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ClickModel));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_ModelDrag, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_DragModel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_ModelClick, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ClickModel));
		}

		public void Set3DModel(GameObject model)
		{
			this.m_3DModel = model;
			if (this.m_3DModel != null)
			{
				AkAudioListener component = this.m_3DModel.GetComponent<AkAudioListener>();
				if (component == null)
				{
					this.m_3DModel.AddComponent<AkAudioListener>();
				}
			}
		}

		public void InitAnimatList()
		{
			this.m_animatList.Clear();
			this.m_clikAnimaList.Clear();
			if (this.m_3DModel == null)
			{
				return;
			}
			Animation component = this.m_3DModel.GetComponent<Animation>();
			if (component != null)
			{
				if (component.clip == null)
				{
					return;
				}
				IEnumerator enumerator = component.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.get_Current();
					if (animationState.clip != null && !string.IsNullOrEmpty(animationState.clip.name))
					{
						this.m_animatList.Add(animationState.clip.name);
						if (animationState.clip.name != component.clip.name && !this.IsNoClickAnima(animationState.clip.name))
						{
							this.m_clikAnimaList.Add(animationState.clip.name);
						}
					}
				}
				if (component.cullingType != AnimationCullingType.AlwaysAnimate)
				{
					component.cullingType = AnimationCullingType.AlwaysAnimate;
				}
			}
		}

		public void InitAnimatSoundList(uint heroId, uint skinId)
		{
			this.m_animatSoundList.Clear();
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey == null)
			{
				return;
			}
			CActorInfo cActorInfo = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataByKey.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			if (cActorInfo != null)
			{
				Singleton<CSoundManager>.GetInstance().LoadSkinSoundBank(heroId, skinId, this.m_3DModel, true);
				for (int i = 0; i < cActorInfo.AnimaSound.Length; i++)
				{
					this.m_animatSoundList.Add(cActorInfo.AnimaSound[i]);
				}
			}
		}

		protected bool IsNoClickAnima(string aniName)
		{
			for (int i = 0; i < CHeroAnimaSystem.s_noClikAnima.Length; i++)
			{
				if (CHeroAnimaSystem.s_noClikAnima[i] == aniName)
				{
					return true;
				}
			}
			return false;
		}

		protected void PlayAnimaSound(string aniName)
		{
			for (int i = 0; i < this.m_animatSoundList.Count; i++)
			{
				if (this.m_animatSoundList[i].AnimaName == aniName && !string.IsNullOrEmpty(this.m_animatSoundList[i].SoundName))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent(this.m_animatSoundList[i].SoundName, this.m_3DModel);
				}
			}
		}

		public void OnHeroInfo_DragModel(CUIEvent uiEvent)
		{
			if (this.m_3DModel != null && this.m_3DModel.transform != null && this.m_3DModel.transform.parent != null)
			{
				this.m_3DModel.transform.parent.Rotate(0f, -uiEvent.m_pointerEventData.get_delta().x, 0f, Space.Self);
			}
		}

		public void OnHeroInfo_ClickModel(CUIEvent uiEvent)
		{
			if (this.m_3DModel == null)
			{
				return;
			}
			Animation component = this.m_3DModel.GetComponent<Animation>();
			if (component == null || component.clip == null)
			{
				return;
			}
			if (!component.IsPlaying(component.clip.name) && Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_tickAnimat < 2000uL)
			{
				return;
			}
			if (component && this.m_clikAnimaList != null && this.m_clikAnimaList.get_Count() > 0)
			{
				int num = Random.Range(0, this.m_clikAnimaList.get_Count());
				component.CrossFade(this.m_clikAnimaList.get_Item(num));
				component.CrossFadeQueued(component.clip.name, 0.3f);
				this.PlayAnimaSound(this.m_clikAnimaList.get_Item(num));
				this.m_tickAnimat = Singleton<FrameSynchr>.instance.LogicFrameTick;
			}
		}

		public void OnModePlayAnima(string animaName)
		{
			if (this.m_3DModel == null)
			{
				return;
			}
			Animation component = this.m_3DModel.GetComponent<Animation>();
			if (component == null || component.clip == null)
			{
				return;
			}
			if (this.m_animatList.Contains(animaName))
			{
				component.Play(animaName);
				component.CrossFadeQueued(component.clip.name, 0.3f);
				this.PlayAnimaSound(animaName);
			}
		}
	}
}
