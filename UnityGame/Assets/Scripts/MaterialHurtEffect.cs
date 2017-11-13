using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SGame/Effect/MaterialHurtEffect")]
public class MaterialHurtEffect : ActorComponent
{
	private struct HighLitColor
	{
		public int id;

		public Vector3 color;
	}

	private RGBCurve hurtCurve;

	private Texture2D freezeTex;

	private Texture2D stoneTex;

	[HideInInspector]
	[NonSerialized]
	public ListView<Material> mats;

	private ListView<Material> oldMats;

	private ListView<SMaterialEffect_Base> playingEffects;

	private float playerId;

	private static int s_playerId;

	private int[] m_effectCounter = new int[4];

	private List<MaterialHurtEffect.HighLitColor> hlcList = new List<MaterialHurtEffect.HighLitColor>();

	private int hlcIndex;

	[HideInInspector]
	private bool meshChanged;

	public bool IsTranslucent
	{
		get
		{
			return this.m_effectCounter[2] > 0;
		}
	}

	private void Awake()
	{
		this.hurtCurve = (Singleton<CResourceManager>.GetInstance().GetResource("Shaders/Curve/RGBCurve_Hurt.asset", typeof(RGBCurve), enResourceType.BattleScene, false, false).m_content as RGBCurve);
		this.freezeTex = (Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Z_Other/Textures/Hero_BingDong.tga", typeof(Texture2D), enResourceType.BattleScene, false, false).m_content as Texture2D);
		this.stoneTex = (Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Z_Other/Textures/Hero_ShiHua.tga", typeof(Texture2D), enResourceType.BattleScene, false, false).m_content as Texture2D);
	}

	protected override void Start()
	{
		int num = Mathf.Max(MaterialHurtEffect.s_playerId++ % 255, 1);
		this.playerId = (float)num / 255f;
		this.CheckMats();
	}

	public void OnMeshChanged()
	{
		this.oldMats = this.mats;
		this.mats = null;
	}

	private bool CheckMats()
	{
		if (this.mats == null)
		{
			this.InitMats();
		}
		return this.mats != null;
	}

	private void InitMats()
	{
		this.mats = null;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
			if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_HurtColor"))
			{
				if (this.mats == null)
				{
					this.mats = new ListView<Material>();
				}
				this.mats.Add(renderer.material);
			}
		}
		if (this.mats != null)
		{
			for (int j = 0; j < this.mats.Count; j++)
			{
				Material material = this.mats[j];
				material.SetFloat("_PlayerId", this.playerId);
			}
		}
	}

	public SMaterialEffect_Base FindEffect(int type)
	{
		if (this.playingEffects == null)
		{
			return null;
		}
		for (int i = 0; i < this.playingEffects.Count; i++)
		{
			if (this.playingEffects[i].type == type)
			{
				return this.playingEffects[i];
			}
		}
		return null;
	}

	private T FindOrCreateEffect<T>(int type) where T : SMaterialEffect_Base, new()
	{
		if (this.playingEffects != null)
		{
			int i = 0;
			while (i < this.playingEffects.Count)
			{
				SMaterialEffect_Base sMaterialEffect_Base = this.playingEffects[i];
				if (sMaterialEffect_Base.type == type)
				{
					if (!(sMaterialEffect_Base is T))
					{
						sMaterialEffect_Base.Stop();
						sMaterialEffect_Base.Release();
						this.playingEffects.RemoveAt(i);
						break;
					}
					return sMaterialEffect_Base as T;
				}
				else
				{
					i++;
				}
			}
		}
		T t = ClassObjPool<T>.Get();
		t.type = type;
		t.owner = this;
		t.bChkReset = false;
		t.AllocId();
		if (this.playingEffects == null)
		{
			this.playingEffects = new ListView<SMaterialEffect_Base>();
		}
		this.playingEffects.Add(t);
		return t;
	}

	private void Update()
	{
		this.CheckMats();
		if (this.oldMats != null && this.mats != null)
		{
			if (this.playingEffects != null && this.playingEffects.Count > 0)
			{
				for (int i = 0; i < this.playingEffects.Count; i++)
				{
					SMaterialEffect_Base sMaterialEffect_Base = this.playingEffects[i];
					sMaterialEffect_Base.OnMeshChanged(this.oldMats, this.mats);
				}
			}
			this.oldMats = null;
		}
		if (this.playingEffects != null && this.playingEffects.Count > 0)
		{
			for (int j = 0; j < this.playingEffects.Count; j++)
			{
				SMaterialEffect_Base sMaterialEffect_Base2 = this.playingEffects[j];
				if (sMaterialEffect_Base2.Update())
				{
					sMaterialEffect_Base2.Release();
					this.playingEffects.RemoveAt(j);
					j--;
				}
			}
		}
	}

	public void PlayHurtEffect()
	{
		if (this.hurtCurve == null)
		{
			return;
		}
		if (!this.CheckMats())
		{
			return;
		}
		SMaterialEffect_Curve sMaterialEffect_Curve = this.FindOrCreateEffect<SMaterialEffect_Curve>(0);
		if (sMaterialEffect_Curve.curve != null)
		{
			return;
		}
		sMaterialEffect_Curve.curve = this.hurtCurve;
		sMaterialEffect_Curve.paramName = "_HurtColor";
		sMaterialEffect_Curve.shaderKeyword = "_HURT_EFFECT_ON";
		sMaterialEffect_Curve.Play();
	}

	public int PlayHighLitEffect(Vector3 color)
	{
		if (!this.CheckMats())
		{
			return 0;
		}
		this.m_effectCounter[3]++;
		SMaterialEffect_HighLit sMaterialEffect_HighLit = this.FindOrCreateEffect<SMaterialEffect_HighLit>(3);
		sMaterialEffect_HighLit.color = color;
		sMaterialEffect_HighLit.Play();
		MaterialHurtEffect.HighLitColor highLitColor = default(MaterialHurtEffect.HighLitColor);
		highLitColor.id = ++this.hlcIndex;
		highLitColor.color = color;
		this.hlcList.Add(highLitColor);
		return highLitColor.id;
	}

	public void StopHighLitEffect(int id)
	{
		if (!this.CheckMats())
		{
			return;
		}
		if (id > 0)
		{
			for (int i = 0; i < this.hlcList.get_Count(); i++)
			{
				if (this.hlcList.get_Item(i).id == id)
				{
					this.hlcList.RemoveAt(i);
					break;
				}
			}
		}
		this.m_effectCounter[3]--;
		SMaterialEffect_HighLit sMaterialEffect_HighLit = this.FindEffect(3) as SMaterialEffect_HighLit;
		if (sMaterialEffect_HighLit == null)
		{
			return;
		}
		if (this.hlcList.get_Count() > 0 && id > 0)
		{
			sMaterialEffect_HighLit.color = this.hlcList.get_Item(this.hlcList.get_Count() - 1).color;
		}
		if (this.m_effectCounter[3] > 0)
		{
			return;
		}
		this.StopEffect(sMaterialEffect_HighLit);
		this.hlcIndex = 0;
		this.hlcList.Clear();
	}

	private int PlayTexEffect(Texture2D tex, int type)
	{
		if (tex == null)
		{
			return -1;
		}
		if (!this.CheckMats())
		{
			return -1;
		}
		SMaterialEffect_Tex sMaterialEffect_Tex = this.FindOrCreateEffect<SMaterialEffect_Tex>(type);
		if (sMaterialEffect_Tex.tex != null)
		{
			sMaterialEffect_Tex.Replay(tex);
		}
		else
		{
			sMaterialEffect_Tex.tex = tex;
			sMaterialEffect_Tex.texParamName = "_EffectTex";
			sMaterialEffect_Tex.shaderKeyword = "_EFFECT_TEX_ON";
			sMaterialEffect_Tex.enableFade = true;
			sMaterialEffect_Tex.hasFadeFactor = true;
			sMaterialEffect_Tex.fadeInterval = 0.08f;
			sMaterialEffect_Tex.fadeParamName = "_EffectFactor";
			sMaterialEffect_Tex.factorScale = 0.85f;
			sMaterialEffect_Tex.Play();
		}
		return sMaterialEffect_Tex.playingId;
	}

	private void StopTexEffect(int type, int playingId)
	{
		if (playingId < 0)
		{
			return;
		}
		SMaterialEffect_Tex sMaterialEffect_Tex = this.FindEffect(type) as SMaterialEffect_Tex;
		if (sMaterialEffect_Tex == null)
		{
			return;
		}
		if (playingId > 0 && sMaterialEffect_Tex.playingId != playingId)
		{
			return;
		}
		if (sMaterialEffect_Tex.enableFade)
		{
			sMaterialEffect_Tex.BeginFadeOut();
		}
		else
		{
			this.StopEffect(sMaterialEffect_Tex);
		}
	}

	public int PlayFreezeEffect()
	{
		this.m_effectCounter[0]++;
		return this.PlayTexEffect(this.freezeTex, 1);
	}

	public int PlayStoneEffect()
	{
		this.m_effectCounter[1]++;
		return this.PlayTexEffect(this.stoneTex, 1);
	}

	public void StopFreezeEffect(int playingId)
	{
		this.m_effectCounter[0]--;
		if (this.m_effectCounter[0] <= 0)
		{
			this.StopTexEffect(1, playingId);
		}
	}

	public void StopStoneEffect(int playingId)
	{
		this.m_effectCounter[1]--;
		if (this.m_effectCounter[1] <= 0)
		{
			this.StopTexEffect(1, playingId);
		}
	}

	public void SetTranslucent(bool b, bool bForbidFade = false)
	{
		int num = this.m_effectCounter[2];
		this.m_effectCounter[2] += (b ? 1 : -1);
		if (num > 0 == this.m_effectCounter[2] > 0)
		{
			return;
		}
		if (!this.CheckMats())
		{
			return;
		}
		if (this.m_effectCounter[2] > 0)
		{
			SMaterialEffect_Translucent sMaterialEffect_Translucent = this.FindOrCreateEffect<SMaterialEffect_Translucent>(2);
			sMaterialEffect_Translucent.enableFade = true;
			sMaterialEffect_Translucent.fadeInterval = 0.1f;
			sMaterialEffect_Translucent.minAlpha = 0.4f;
			sMaterialEffect_Translucent.Play();
		}
		else
		{
			SMaterialEffect_Translucent sMaterialEffect_Translucent2 = this.FindEffect(2) as SMaterialEffect_Translucent;
			if (sMaterialEffect_Translucent2 == null)
			{
				return;
			}
			if (sMaterialEffect_Translucent2.enableFade && !bForbidFade)
			{
				sMaterialEffect_Translucent2.BeginFadeOut();
			}
			else
			{
				this.StopEffect(sMaterialEffect_Translucent2);
			}
		}
	}

	private void StopEffect(SMaterialEffect_Base effect)
	{
		effect.Stop();
		effect.Release();
		this.playingEffects.Remove(effect);
	}

	private void OnDestroy()
	{
		if (this.mats != null)
		{
			this.mats.Clear();
			this.mats = null;
		}
	}
}
