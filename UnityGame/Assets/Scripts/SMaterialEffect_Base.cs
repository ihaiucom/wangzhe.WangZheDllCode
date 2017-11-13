using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using UnityEngine;

public abstract class SMaterialEffect_Base : PooledClassObject
{
	public struct STimer
	{
		public long startFrameTick;

		public float curTime;

		public void Start()
		{
			this.curTime = 0f;
			this.startFrameTick = (long)Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}

		public float Update()
		{
			FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
			float result;
			if (instance.bActive)
			{
				long logicFrameTick = (long)instance.LogicFrameTick;
				long num = logicFrameTick - this.startFrameTick;
				result = (float)num * 0.001f;
			}
			else
			{
				this.curTime += Time.deltaTime;
				result = this.curTime;
			}
			return result;
		}
	}

	public enum FadeState
	{
		FadeIn,
		Normal,
		FadeOut,
		Stopped
	}

	public int type;

	public int playingId;

	public string shaderKeyword;

	public MaterialHurtEffect owner;

	public static int s_playingId;

	public void AllocId()
	{
		this.playingId = ++SMaterialEffect_Base.s_playingId;
	}

	public abstract bool Update();

	public static void enableMatsKeyword(ListView<Material> mats, string keywords, bool enable)
	{
		if (mats != null)
		{
			if (enable)
			{
				for (int i = 0; i < mats.Count; i++)
				{
					Material material = mats[i];
					material.EnableKeyword(keywords);
				}
			}
			else
			{
				for (int j = 0; j < mats.Count; j++)
				{
					Material material2 = mats[j];
					material2.DisableKeyword(keywords);
				}
			}
		}
	}

	public virtual void Stop()
	{
		SMaterialEffect_Base.enableMatsKeyword(this.owner.mats, this.shaderKeyword, false);
	}

	public virtual void Play()
	{
		SMaterialEffect_Base.enableMatsKeyword(this.owner.mats, this.shaderKeyword, true);
	}

	public virtual void OnMeshChanged(ListView<Material> oldMats, ListView<Material> newMats)
	{
		SMaterialEffect_Base.enableMatsKeyword(oldMats, this.shaderKeyword, false);
		SMaterialEffect_Base.enableMatsKeyword(newMats, this.shaderKeyword, true);
	}

	public static bool UpdateFadeState(out float factor, ref SMaterialEffect_Base.FadeState fadeState, ref SMaterialEffect_Base.STimer fadeTime, float fadeInterval, bool forceUpdate = false)
	{
		bool result = false;
		factor = 1f;
		if (fadeState == SMaterialEffect_Base.FadeState.FadeIn)
		{
			float num = fadeTime.Update();
			if (num >= fadeInterval)
			{
				factor = 1f;
				fadeState = SMaterialEffect_Base.FadeState.Normal;
			}
			else
			{
				factor = num / fadeInterval;
			}
			result = true;
		}
		else if (fadeState == SMaterialEffect_Base.FadeState.FadeOut)
		{
			float num2 = fadeTime.Update();
			if (num2 >= fadeInterval)
			{
				factor = 0f;
				fadeState = SMaterialEffect_Base.FadeState.Stopped;
			}
			else
			{
				factor = 1f - num2 / fadeInterval;
			}
			result = true;
		}
		if (forceUpdate)
		{
			result = true;
		}
		return result;
	}
}
