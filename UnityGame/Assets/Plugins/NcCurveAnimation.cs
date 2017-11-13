using System;
using System.Collections.Generic;
using UnityEngine;

public class NcCurveAnimation : NcEffectAniBehaviour
{
	private class NcComparerCurve : IComparer<NcCurveAnimation.NcInfoCurve>
	{
		protected static float m_fEqualRange = 0.03f;

		protected static float m_fHDiv = 5f;

		public int Compare(NcCurveAnimation.NcInfoCurve a, NcCurveAnimation.NcInfoCurve b)
		{
			float num = a.m_AniCurve.Evaluate(NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv) - b.m_AniCurve.Evaluate(NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv);
			if (Mathf.Abs(num) < NcCurveAnimation.NcComparerCurve.m_fEqualRange)
			{
				num = b.m_AniCurve.Evaluate(1f - NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv) - a.m_AniCurve.Evaluate(1f - NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv);
				if (Mathf.Abs(num) < NcCurveAnimation.NcComparerCurve.m_fEqualRange)
				{
					return 0;
				}
			}
			return (int)(num * 1000f);
		}

		public static int GetSortGroup(NcCurveAnimation.NcInfoCurve info)
		{
			float num = info.m_AniCurve.Evaluate(NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv);
			if (num < -NcCurveAnimation.NcComparerCurve.m_fEqualRange)
			{
				return 1;
			}
			if (NcCurveAnimation.NcComparerCurve.m_fEqualRange < num)
			{
				return 3;
			}
			return 2;
		}
	}

	[Serializable]
	public class NcInfoCurve
	{
		public enum APPLY_TYPE
		{
			NONE,
			POSITION,
			ROTATION,
			SCALE,
			MATERIAL_COLOR,
			TEXTUREUV,
			MESH_COLOR
		}

		protected const float m_fOverDraw = 0.2f;

		public bool m_bEnabled = true;

		public string m_CurveName = string.Empty;

		public AnimationCurve m_AniCurve = new AnimationCurve();

		public static string[] m_TypeName = new string[]
		{
			"None",
			"Position",
			"Rotation",
			"Scale",
			"MaterialColor",
			"TextureUV",
			"MeshColor"
		};

		public NcCurveAnimation.NcInfoCurve.APPLY_TYPE m_ApplyType = NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION;

		public bool[] m_bApplyOption;

		public bool m_bRecursively;

		public float m_fValueScale;

		public Vector4 m_FromColor;

		public Vector4 m_ToColor;

		public int m_nTag;

		public int m_nSortGroup;

		public Vector4 m_OriginalValue;

		public Vector4 m_BeforeValue;

		public Vector4[] m_ChildOriginalColorValues;

		public Vector4[] m_ChildBeforeColorValues;

		public NcInfoCurve()
		{
			bool[] array = new bool[4];
			array[1] = true;
			this.m_bApplyOption = array;
			this.m_fValueScale = 1f;
			this.m_FromColor = Color.white;
			this.m_ToColor = Color.white;
		}

		public bool IsEnabled()
		{
			return this.m_bEnabled;
		}

		public void SetEnabled(bool bEnable)
		{
			this.m_bEnabled = bEnable;
		}

		public string GetCurveName()
		{
			return this.m_CurveName;
		}

		public NcCurveAnimation.NcInfoCurve GetClone()
		{
			NcCurveAnimation.NcInfoCurve ncInfoCurve = new NcCurveAnimation.NcInfoCurve();
			ncInfoCurve.m_AniCurve = new AnimationCurve(this.m_AniCurve.keys);
			ncInfoCurve.m_AniCurve.postWrapMode = this.m_AniCurve.postWrapMode;
			ncInfoCurve.m_AniCurve.preWrapMode = this.m_AniCurve.preWrapMode;
			ncInfoCurve.m_bEnabled = this.m_bEnabled;
			ncInfoCurve.m_CurveName = this.m_CurveName;
			ncInfoCurve.m_ApplyType = this.m_ApplyType;
			Array.Copy(this.m_bApplyOption, ncInfoCurve.m_bApplyOption, this.m_bApplyOption.Length);
			ncInfoCurve.m_fValueScale = this.m_fValueScale;
			ncInfoCurve.m_bRecursively = this.m_bRecursively;
			ncInfoCurve.m_FromColor = this.m_FromColor;
			ncInfoCurve.m_ToColor = this.m_ToColor;
			ncInfoCurve.m_nTag = this.m_nTag;
			ncInfoCurve.m_nSortGroup = this.m_nSortGroup;
			return ncInfoCurve;
		}

		public void CopyTo(NcCurveAnimation.NcInfoCurve target)
		{
			target.m_AniCurve = new AnimationCurve(this.m_AniCurve.keys);
			target.m_AniCurve.postWrapMode = this.m_AniCurve.postWrapMode;
			target.m_AniCurve.preWrapMode = this.m_AniCurve.preWrapMode;
			target.m_bEnabled = this.m_bEnabled;
			target.m_ApplyType = this.m_ApplyType;
			Array.Copy(this.m_bApplyOption, target.m_bApplyOption, this.m_bApplyOption.Length);
			target.m_fValueScale = this.m_fValueScale;
			target.m_bRecursively = this.m_bRecursively;
			target.m_FromColor = this.m_FromColor;
			target.m_ToColor = this.m_ToColor;
			target.m_nTag = this.m_nTag;
			target.m_nSortGroup = this.m_nSortGroup;
		}

		public int GetValueCount()
		{
			switch (this.m_ApplyType)
			{
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
				return 4;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
				return 4;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
				return 3;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
				return 4;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				return 2;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR:
				return 4;
			default:
				return 0;
			}
		}

		public string GetValueName(int nIndex)
		{
			string[] array;
			switch (this.m_ApplyType)
			{
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
				array = new string[]
				{
					"X",
					"Y",
					"Z",
					"World"
				};
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
				array = new string[]
				{
					"X",
					"Y",
					"Z",
					string.Empty
				};
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
				array = new string[]
				{
					"R",
					"G",
					"B",
					"A"
				};
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				array = new string[]
				{
					"X",
					"Y",
					string.Empty,
					string.Empty
				};
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR:
				array = new string[]
				{
					"R",
					"G",
					"B",
					"A"
				};
				break;
			default:
				array = new string[]
				{
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
				break;
			}
			return array[nIndex];
		}

		public void SetDefaultValueScale()
		{
			switch (this.m_ApplyType)
			{
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
				this.m_fValueScale = 1f;
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
				this.m_fValueScale = 360f;
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
				this.m_fValueScale = 1f;
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				this.m_fValueScale = 10f;
				break;
			}
		}

		public Rect GetFixedDrawRange()
		{
			return new Rect(-0.2f, -1.2f, 1.4f, 2.4f);
		}

		public Rect GetVariableDrawRange()
		{
			Rect result = default(Rect);
			for (int i = 0; i < this.m_AniCurve.keys.Length; i++)
			{
				result.yMin = Mathf.Min(result.yMin, this.m_AniCurve[i].value);
				result.yMax = Mathf.Max(result.yMax, this.m_AniCurve[i].value);
			}
			int num = 20;
			for (int j = 0; j < num; j++)
			{
				float b = this.m_AniCurve.Evaluate((float)j / (float)num);
				result.yMin = Mathf.Min(result.yMin, b);
				result.yMax = Mathf.Max(result.yMax, b);
			}
			result.xMin = 0f;
			result.xMax = 1f;
			result.xMin -= result.width * 0.2f;
			result.xMax += result.width * 0.2f;
			result.yMin -= result.height * 0.2f;
			result.yMax += result.height * 0.2f;
			return result;
		}

		public Rect GetEditRange()
		{
			return new Rect(0f, -1f, 1f, 2f);
		}

		public void NormalizeCurveTime()
		{
			int i = 0;
			while (i < this.m_AniCurve.keys.Length)
			{
				Keyframe keyframe = this.m_AniCurve[i];
				float a = Mathf.Max(0f, keyframe.time);
				float num = Mathf.Min(1f, Mathf.Max(a, keyframe.time));
				if (num != keyframe.time)
				{
					Keyframe key = new Keyframe(num, keyframe.value, keyframe.inTangent, keyframe.outTangent);
					this.m_AniCurve.RemoveKey(i);
					i = 0;
					this.m_AniCurve.AddKey(key);
				}
				else
				{
					i++;
				}
			}
		}
	}

	[SerializeField]
	public List<NcCurveAnimation.NcInfoCurve> m_CurveInfoList;

	public float m_fDelayTime;

	public float m_fDurationTime = 0.6f;

	public bool m_bAutoDestruct = true;

	protected float m_fStartTime;

	protected float m_fElapsedRate;

	protected Transform m_Transform;

	protected string m_ColorName;

	protected Material m_MainMaterial;

	protected string[] m_ChildColorNames;

	protected Renderer[] m_ChildRenderers;

	protected MeshFilter m_MainMeshFilter;

	protected MeshFilter[] m_ChildMeshFilters;

	protected NcUvAnimation m_NcUvAnimation;

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (0f < this.m_fDurationTime && (this.m_fStartTime == 0f || !base.IsEndAnimation()))
		{
			return 1;
		}
		return 0;
	}

	public override void ResetAnimation()
	{
		base.ResetAnimation();
		this.InitAnimation();
		this.UpdateAnimation(0f);
	}

	public float GetRepeatedRate()
	{
		return this.m_fElapsedRate;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.InitAnimation();
		if (0f < this.m_fDelayTime)
		{
			if (base.GetRenderer())
			{
				base.GetRenderer().enabled = false;
			}
			return;
		}
		base.InitAnimationTimer();
		this.UpdateAnimation(0f);
	}

	private void LateUpdate()
	{
		if (this.m_fStartTime == 0f)
		{
			return;
		}
		if (this.m_fDelayTime != 0f)
		{
			if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)
			{
				return;
			}
			this.m_fDelayTime = 0f;
			base.InitAnimationTimer();
			if (base.GetRenderer())
			{
				base.GetRenderer().enabled = true;
			}
		}
		float time = this.m_Timer.GetTime();
		float fElapsedRate = time;
		if (this.m_fDurationTime != 0f)
		{
			fElapsedRate = time / this.m_fDurationTime;
		}
		this.UpdateAnimation(fElapsedRate);
	}

	private void InitAnimation()
	{
		this.m_fElapsedRate = 0f;
		this.m_Transform = base.transform;
		using (List<NcCurveAnimation.NcInfoCurve>.Enumerator enumerator = this.m_CurveInfoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NcCurveAnimation.NcInfoCurve current = enumerator.get_Current();
				if (current.m_bEnabled)
				{
					switch (current.m_ApplyType)
					{
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
						current.m_OriginalValue = Vector4.zero;
						current.m_BeforeValue = current.m_OriginalValue;
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
						current.m_OriginalValue = Vector4.zero;
						current.m_BeforeValue = current.m_OriginalValue;
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
						current.m_OriginalValue = this.m_Transform.localScale;
						current.m_BeforeValue = current.m_OriginalValue;
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
						if (current.m_bRecursively)
						{
							this.m_ChildRenderers = base.transform.GetComponentsInChildren<Renderer>(true);
							this.m_ChildColorNames = new string[this.m_ChildRenderers.Length];
							current.m_ChildOriginalColorValues = new Vector4[this.m_ChildRenderers.Length];
							current.m_ChildBeforeColorValues = new Vector4[this.m_ChildRenderers.Length];
							for (int i = 0; i < this.m_ChildRenderers.Length; i++)
							{
								Renderer renderer = this.m_ChildRenderers[i];
								this.m_ChildColorNames[i] = NcCurveAnimation.Ng_GetMaterialColorName(renderer.sharedMaterial);
								if (this.m_ChildColorNames[i] != null)
								{
									current.m_ChildOriginalColorValues[i] = renderer.material.GetColor(this.m_ChildColorNames[i]);
								}
								current.m_ChildBeforeColorValues[i] = Vector4.zero;
							}
						}
						else if (base.GetRenderer() != null)
						{
							this.m_ColorName = NcCurveAnimation.Ng_GetMaterialColorName(base.GetRenderer().sharedMaterial);
							if (this.m_ColorName != null)
							{
								current.m_OriginalValue = base.GetRenderer().sharedMaterial.GetColor(this.m_ColorName);
							}
							current.m_BeforeValue = Vector4.zero;
						}
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
						if (this.m_NcUvAnimation == null)
						{
							this.m_NcUvAnimation = base.GetComponent<NcUvAnimation>();
						}
						if (this.m_NcUvAnimation)
						{
							current.m_OriginalValue = new Vector4(this.m_NcUvAnimation.m_fScrollSpeedX, this.m_NcUvAnimation.m_fScrollSpeedY, 0f, 0f);
						}
						current.m_BeforeValue = current.m_OriginalValue;
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR:
					{
						float t = current.m_AniCurve.Evaluate(0f);
						Color tarColor = Color.Lerp(current.m_FromColor, current.m_ToColor, t);
						if (current.m_bRecursively)
						{
							this.m_ChildMeshFilters = base.transform.GetComponentsInChildren<MeshFilter>(true);
							if (this.m_ChildMeshFilters != null && this.m_ChildMeshFilters.Length >= 0)
							{
								for (int j = 0; j < this.m_ChildMeshFilters.Length; j++)
								{
									this.ChangeMeshColor(this.m_ChildMeshFilters[j], tarColor);
								}
							}
						}
						else
						{
							this.m_MainMeshFilter = base.GetComponent<MeshFilter>();
							this.ChangeMeshColor(this.m_MainMeshFilter, tarColor);
						}
						break;
					}
					}
				}
			}
		}
	}

	private void UpdateAnimation(float fElapsedRate)
	{
		this.m_fElapsedRate = fElapsedRate;
		using (List<NcCurveAnimation.NcInfoCurve>.Enumerator enumerator = this.m_CurveInfoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NcCurveAnimation.NcInfoCurve current = enumerator.get_Current();
				if (current.m_bEnabled)
				{
					float num = current.m_AniCurve.Evaluate(this.m_fElapsedRate);
					if (current.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR && current.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR)
					{
						num *= current.m_fValueScale;
					}
					switch (current.m_ApplyType)
					{
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
						if (current.m_bApplyOption[3])
						{
							this.m_Transform.position += new Vector3(this.GetNextValue(current, 0, num), this.GetNextValue(current, 1, num), this.GetNextValue(current, 2, num));
						}
						else
						{
							this.m_Transform.localPosition += new Vector3(this.GetNextValue(current, 0, num), this.GetNextValue(current, 1, num), this.GetNextValue(current, 2, num));
						}
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
						if (current.m_bApplyOption[3])
						{
							this.m_Transform.rotation *= Quaternion.Euler(this.GetNextValue(current, 0, num), this.GetNextValue(current, 1, num), this.GetNextValue(current, 2, num));
						}
						else
						{
							this.m_Transform.localRotation *= Quaternion.Euler(this.GetNextValue(current, 0, num), this.GetNextValue(current, 1, num), this.GetNextValue(current, 2, num));
						}
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
						this.m_Transform.localScale += new Vector3(this.GetNextScale(current, 0, num), this.GetNextScale(current, 1, num), this.GetNextScale(current, 2, num));
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
						if (current.m_bRecursively)
						{
							if (this.m_ChildColorNames != null && this.m_ChildColorNames.Length >= 0)
							{
								for (int i = 0; i < this.m_ChildColorNames.Length; i++)
								{
									if (this.m_ChildColorNames[i] != null && this.m_ChildRenderers[i] != null)
									{
										this.SetChildMaterialColor(current, num, i);
									}
								}
							}
						}
						else if (base.GetRenderer() != null && this.m_ColorName != null)
						{
							if (this.m_MainMaterial == null)
							{
								this.m_MainMaterial = base.GetRenderer().material;
								base.AddRuntimeMaterial(this.m_MainMaterial);
							}
							Color color = current.m_ToColor - current.m_OriginalValue;
							Color color2 = this.m_MainMaterial.GetColor(this.m_ColorName);
							for (int j = 0; j < 4; j++)
							{
								int index;
								float num2 = color2[index = j];
								color2[index] = num2 + this.GetNextValue(current, j, color[j] * num);
							}
							this.m_MainMaterial.SetColor(this.m_ColorName, color2);
						}
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
						if (this.m_NcUvAnimation)
						{
							this.m_NcUvAnimation.m_fScrollSpeedX += this.GetNextValue(current, 0, num);
							this.m_NcUvAnimation.m_fScrollSpeedY += this.GetNextValue(current, 1, num);
						}
						break;
					case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR:
					{
						Color tarColor = Color.Lerp(current.m_FromColor, current.m_ToColor, num);
						if (current.m_bRecursively)
						{
							if (this.m_ChildMeshFilters != null && this.m_ChildMeshFilters.Length >= 0)
							{
								for (int k = 0; k < this.m_ChildMeshFilters.Length; k++)
								{
									this.ChangeMeshColor(this.m_ChildMeshFilters[k], tarColor);
								}
							}
						}
						else
						{
							this.ChangeMeshColor(this.m_MainMeshFilter, tarColor);
						}
						break;
					}
					}
				}
			}
		}
		if (this.m_fDurationTime != 0f && 1f < this.m_fElapsedRate)
		{
			if (!base.IsEndAnimation())
			{
				base.OnEndAnimation();
			}
			if (this.m_bAutoDestruct)
			{
				Object.DestroyObject(base.gameObject);
			}
		}
	}

	private void ChangeMeshColor(MeshFilter mFilter, Color tarColor)
	{
		if (mFilter == null || mFilter.mesh == null)
		{
			return;
		}
		Color[] colors = mFilter.mesh.colors;
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i] = tarColor;
		}
		mFilter.mesh.colors = colors;
	}

	private void SetChildMaterialColor(NcCurveAnimation.NcInfoCurve curveInfo, float fValue, int arrayIndex)
	{
		Color color = curveInfo.m_ToColor - curveInfo.m_ChildOriginalColorValues[arrayIndex];
		Color color2 = this.m_ChildRenderers[arrayIndex].material.GetColor(this.m_ChildColorNames[arrayIndex]);
		for (int i = 0; i < 4; i++)
		{
			int index;
			float num = color2[index = i];
			color2[index] = num + this.GetChildNextColorValue(curveInfo, i, color[i] * fValue, arrayIndex);
		}
		this.m_ChildRenderers[arrayIndex].material.SetColor(this.m_ChildColorNames[arrayIndex], color2);
	}

	private float GetChildNextColorValue(NcCurveAnimation.NcInfoCurve curveInfo, int nIndex, float fValue, int arrayIndex)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float result = fValue - curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex];
			curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex] = fValue;
			return result;
		}
		return 0f;
	}

	private float GetNextValue(NcCurveAnimation.NcInfoCurve curveInfo, int nIndex, float fValue)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float result = fValue - curveInfo.m_BeforeValue[nIndex];
			curveInfo.m_BeforeValue[nIndex] = fValue;
			return result;
		}
		return 0f;
	}

	private float GetNextScale(NcCurveAnimation.NcInfoCurve curveInfo, int nIndex, float fValue)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float num = curveInfo.m_OriginalValue[nIndex] * (1f + fValue);
			float result = num - curveInfo.m_BeforeValue[nIndex];
			curveInfo.m_BeforeValue[nIndex] = num;
			return result;
		}
		return 0f;
	}

	public float GetElapsedRate()
	{
		return this.m_fElapsedRate;
	}

	public void CopyTo(NcCurveAnimation target, bool bCurveOnly)
	{
		target.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		using (List<NcCurveAnimation.NcInfoCurve>.Enumerator enumerator = this.m_CurveInfoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NcCurveAnimation.NcInfoCurve current = enumerator.get_Current();
				target.m_CurveInfoList.Add(current.GetClone());
			}
		}
		if (!bCurveOnly)
		{
			target.m_fDelayTime = this.m_fDelayTime;
			target.m_fDurationTime = this.m_fDurationTime;
		}
	}

	public void AppendTo(NcCurveAnimation target, bool bCurveOnly)
	{
		if (target.m_CurveInfoList == null)
		{
			target.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		}
		using (List<NcCurveAnimation.NcInfoCurve>.Enumerator enumerator = this.m_CurveInfoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NcCurveAnimation.NcInfoCurve current = enumerator.get_Current();
				target.m_CurveInfoList.Add(current.GetClone());
			}
		}
		if (!bCurveOnly)
		{
			target.m_fDelayTime = this.m_fDelayTime;
			target.m_fDurationTime = this.m_fDurationTime;
		}
	}

	public NcCurveAnimation.NcInfoCurve GetCurveInfo(int nIndex)
	{
		if (this.m_CurveInfoList == null || nIndex < 0 || this.m_CurveInfoList.get_Count() <= nIndex)
		{
			return null;
		}
		return this.m_CurveInfoList.get_Item(nIndex);
	}

	public NcCurveAnimation.NcInfoCurve GetCurveInfo(string curveName)
	{
		if (this.m_CurveInfoList == null)
		{
			return null;
		}
		using (List<NcCurveAnimation.NcInfoCurve>.Enumerator enumerator = this.m_CurveInfoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NcCurveAnimation.NcInfoCurve current = enumerator.get_Current();
				if (current.m_CurveName == curveName)
				{
					return current;
				}
			}
		}
		return null;
	}

	public NcCurveAnimation.NcInfoCurve SetCurveInfo(int nIndex, NcCurveAnimation.NcInfoCurve newInfo)
	{
		if (this.m_CurveInfoList == null || nIndex < 0 || this.m_CurveInfoList.get_Count() <= nIndex)
		{
			return null;
		}
		NcCurveAnimation.NcInfoCurve result = this.m_CurveInfoList.get_Item(nIndex);
		this.m_CurveInfoList.set_Item(nIndex, newInfo);
		return result;
	}

	public int AddCurveInfo()
	{
		NcCurveAnimation.NcInfoCurve ncInfoCurve = new NcCurveAnimation.NcInfoCurve();
		ncInfoCurve.m_AniCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		ncInfoCurve.m_ToColor = Color.white;
		ncInfoCurve.m_ToColor.w = 0f;
		if (this.m_CurveInfoList == null)
		{
			this.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		}
		this.m_CurveInfoList.Add(ncInfoCurve);
		return this.m_CurveInfoList.get_Count() - 1;
	}

	public int AddCurveInfo(NcCurveAnimation.NcInfoCurve addCurveInfo)
	{
		if (this.m_CurveInfoList == null)
		{
			this.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		}
		this.m_CurveInfoList.Add(addCurveInfo.GetClone());
		return this.m_CurveInfoList.get_Count() - 1;
	}

	public void DeleteCurveInfo(int nIndex)
	{
		if (this.m_CurveInfoList == null || nIndex < 0 || this.m_CurveInfoList.get_Count() <= nIndex)
		{
			return;
		}
		this.m_CurveInfoList.Remove(this.m_CurveInfoList.get_Item(nIndex));
	}

	public void ClearAllCurveInfo()
	{
		if (this.m_CurveInfoList == null)
		{
			return;
		}
		this.m_CurveInfoList.Clear();
	}

	public int GetCurveInfoCount()
	{
		if (this.m_CurveInfoList == null)
		{
			return 0;
		}
		return this.m_CurveInfoList.get_Count();
	}

	public void SortCurveInfo()
	{
		if (this.m_CurveInfoList == null)
		{
			return;
		}
		this.m_CurveInfoList.Sort(new NcCurveAnimation.NcComparerCurve());
		using (List<NcCurveAnimation.NcInfoCurve>.Enumerator enumerator = this.m_CurveInfoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NcCurveAnimation.NcInfoCurve current = enumerator.get_Current();
				current.m_nSortGroup = NcCurveAnimation.NcComparerCurve.GetSortGroup(current);
			}
		}
	}

	public bool CheckInvalidOption()
	{
		bool result = false;
		for (int i = 0; i < this.m_CurveInfoList.get_Count(); i++)
		{
			if (this.CheckInvalidOption(i))
			{
				result = true;
			}
		}
		return result;
	}

	public bool CheckInvalidOption(int nSrcIndex)
	{
		NcCurveAnimation.NcInfoCurve curveInfo = this.GetCurveInfo(nSrcIndex);
		if (curveInfo != null)
		{
			if (curveInfo.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR && curveInfo.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE)
			{
				bool arg_38_0 = curveInfo.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV;
			}
		}
		return false;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fDurationTime /= fSpeedRate;
	}

	public static string Ng_GetMaterialColorName(Material mat)
	{
		string[] array = new string[]
		{
			"_Color",
			"_TintColor",
			"_EmisColor"
		};
		if (mat != null)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}
}
