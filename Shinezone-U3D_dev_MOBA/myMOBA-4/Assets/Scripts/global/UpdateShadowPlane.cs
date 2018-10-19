using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShadowPlane : ActorComponent
{
	public const string c_planeShadowPath = "Prefab_Characters/PlaneShadow.prefab";

	private GameObject shadowMesh;

	private ListView<Material> mats;

	public float height = 1.5f;

	private static Dictionary<Mesh, float> meshHeightMap = new Dictionary<Mesh, float>();

	private float lastUpdateGroundY = -1000f;

	public static void Preload(ref ActorPreloadTab result)
	{
		result.AddMesh("Prefab_Characters/PlaneShadow.prefab");
	}

	public static void ClearCache()
	{
		UpdateShadowPlane.meshHeightMap.Clear();
	}

	private void CheckMaterials()
	{
		if (this.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero && this.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
		{
			return;
		}
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return;
		}
		try
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = componentsInChildren[i];
				if (!HeroMaterialUtility.IsHeroBattleShader(skinnedMeshRenderer.sharedMaterial))
				{
					Shader shader = Shader.Find("S_Game_Hero/Hero_Battle");
					if (shader != null)
					{
						skinnedMeshRenderer.material.shader = shader;
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void AddShadowedMat(ref ListView<Material> mats, Material m, bool useShadow)
	{
		if (!useShadow)
		{
			return;
		}
		if (mats == null)
		{
			mats = new ListView<Material>();
		}
		mats.Add(m);
	}

	public void DisableDynamicShadow()
	{
		if (this.mats == null)
		{
			return;
		}
		for (int i = 0; i < this.mats.Count; i++)
		{
			Material material = this.mats[i];
			bool flag;
			bool translucent;
			bool flag2;
			HeroMaterialUtility.GetShaderProperty(material.shader.name, out flag, out translucent, out flag2);
			string name = HeroMaterialUtility.MakeShaderName(material.shader.name, false, translucent, false);
			Shader shader = Shader.Find(name);
			if (shader != null)
			{
				material.shader = shader;
			}
		}
		this.mats = null;
	}

	public void EnableDynamicShadow()
	{
		if (this.mats != null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = true;
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
			if (!(renderer.gameObject == this.shadowMesh))
			{
				Material sharedMaterial = renderer.sharedMaterial;
				if (HeroMaterialUtility.IsHeroBattleShader(sharedMaterial))
				{
					bool flag3;
					bool translucent;
					bool flag4;
					HeroMaterialUtility.GetShaderProperty(sharedMaterial.shader.name, out flag3, out translucent, out flag4);
					if (flag2 != flag3 || flag4 != flag)
					{
						string name = HeroMaterialUtility.MakeShaderName(sharedMaterial.shader.name, flag2, translucent, flag);
						Shader shader = Shader.Find(name);
						if (shader != null)
						{
							renderer.material.shader = shader;
							this.AddShadowedMat(ref this.mats, renderer.material, flag2);
						}
					}
					else
					{
						this.AddShadowedMat(ref this.mats, renderer.material, flag2);
					}
				}
			}
		}
		this.CalcHeight();
	}

	private void EnablePlaneShadow()
	{
		if (this.shadowMesh == null)
		{
			this.shadowMesh = base.gameObject.FindChildBFS((GameObject obj) => obj.name == "Shadow" || obj.name.IndexOf("Shadow_") == 0);
			if (this.shadowMesh == null)
			{
				GameObject original = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/PlaneShadow.prefab", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
				this.shadowMesh = (UnityEngine.Object.Instantiate(original) as GameObject);
				if (this.shadowMesh)
				{
					Transform transform = this.shadowMesh.transform;
					transform.SetParent(base.gameObject.transform);
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.identity;
					transform.localScale = Vector3.one;
					if (this.actor != null && this.actor.CharInfo != null)
					{
						float num = (float)this.actor.CharInfo.iCollisionSize.x / 400f;
						num = Mathf.Clamp(num, 0.5f, 2f);
						transform.localScale = new Vector3(num, num, num);
					}
				}
			}
		}
		if (this.shadowMesh != null)
		{
			this.shadowMesh.CustomSetActive(true);
		}
	}

	public override void Born(ActorRoot owner)
	{
		base.Born(owner);
		if (this.mats != null)
		{
			this.mats.Clear();
		}
		this.shadowMesh = base.gameObject.FindChildBFS((GameObject obj) => obj.name == "Shadow" || obj.name.IndexOf("Shadow_") == 0);
		this.ApplyShadowSettings();
	}

	private bool SetIdlePos()
	{
		Animation componentInChildren = base.gameObject.GetComponentInChildren<Animation>();
		if (componentInChildren == null)
		{
			return false;
		}
		AnimationClip clip = componentInChildren.GetClip("Idle");
		if (clip == null)
		{
			return false;
		}
		base.gameObject.SampleAnimation(clip, 0f);
		return true;
	}

	public void CalcHeight(SkinnedMeshRenderer renderer, ref bool posUpdated, ref float meshHeight, float invScale)
	{
		if (!posUpdated)
		{
			this.SetIdlePos();
			posUpdated = true;
		}
		Transform[] bones = renderer.bones;
		if (bones == null || bones.Length == 0)
		{
			return;
		}
		float y = base.gameObject.transform.position.y;
		float num = 3.40282347E+38f;
		float num2 = -3.40282347E+38f;
		float num3 = 0f;
		for (int i = 0; i < bones.Length; i++)
		{
			float y2 = bones[i].position.y;
			num = Mathf.Min(y2, num);
			num2 = Mathf.Max(y2, num2);
			num3 = Mathf.Max(num3, y2 - y);
		}
		num3 = Mathf.Max(num3, num2 - num);
		meshHeight = Mathf.Max(num3, meshHeight);
		UpdateShadowPlane.meshHeightMap.Add(renderer.sharedMesh, num3 * invScale);
	}

	public void CalcHeight()
	{
		if (!GameSettings.IsHighQuality || this.mats == null || this.mats.Count == 0)
		{
			return;
		}
		bool flag = false;
		float y = base.gameObject.transform.lossyScale.y;
		float invScale = 1f / y;
		this.height = -1f;
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Mesh sharedMesh = componentsInChildren[i].sharedMesh;
			if (!(sharedMesh == null))
			{
				float num = 0f;
				if (!UpdateShadowPlane.meshHeightMap.TryGetValue(sharedMesh, out num))
				{
					this.CalcHeight(componentsInChildren[i], ref flag, ref num, invScale);
				}
				else
				{
					num *= y;
				}
				this.height = Mathf.Max(this.height, num * 1.5f);
			}
		}
		if (this.height < 0f)
		{
			this.height = 1.5f;
		}
	}

	public void Update()
	{
		if (this.actor == null || !this.actor.Visible)
		{
			return;
		}
		float scalar = this.actor.groundY.scalar;
		if (this.lastUpdateGroundY != scalar)
		{
			if (this.shadowMesh != null && this.shadowMesh.activeInHierarchy)
			{
				Transform transform = this.shadowMesh.transform;
				Vector3 position = transform.position;
				position.y = scalar;
				transform.position = position;
			}
			this.lastUpdateGroundY = scalar;
		}
		if (this.mats != null && this.mats.Count > 0)
		{
			Vector4 vector = new Vector4(0f, 1f, 0f, 0f);
			vector.w = scalar;
			vector.w += 0.05f;
			float value = Mathf.Abs(PlaneShadowSettings.shadowProjDir.y / Mathf.Max(0.001f, this.height));
			for (int i = 0; i < this.mats.Count; i++)
			{
				Material material = this.mats[i];
				material.SetVector("_ShadowPlane", vector);
				material.SetFloat("_ShadowInvLen", value);
				material.SetVector("_WorldPos", base.gameObject.transform.position.toVec4(1f));
			}
		}
	}

	public void ApplyShadowSettings()
	{
		switch (GameSettings.ShadowQuality)
		{
		case SGameRenderQuality.High:
			this.EnableDynamicShadow();
			if (this.shadowMesh != null && this.mats != null)
			{
				this.shadowMesh.CustomSetActive(false);
			}
			break;
		case SGameRenderQuality.Medium:
			this.DisableDynamicShadow();
			this.EnablePlaneShadow();
			break;
		case SGameRenderQuality.Low:
			this.DisableDynamicShadow();
			if (this.shadowMesh != null)
			{
				this.shadowMesh.CustomSetActive(false);
			}
			break;
		}
	}
}
