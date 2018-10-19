using System;
using UnityEngine;

public class NcSpriteTexture : NcEffectBehaviour
{
	public GameObject m_NcSpriteFactoryPrefab;

	protected NcSpriteFactory m_NcSpriteFactoryCom;

	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;

	public float m_fUvScale = 1f;

	public int m_nSpriteFactoryIndex;

	public int m_nFrameIndex;

	public NcSpriteFactory.MESH_TYPE m_MeshType;

	public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;

	protected GameObject m_EffectObject;

	private void Awake()
	{
		if (this.m_NcSpriteFactoryPrefab == null && base.gameObject.GetComponent<NcSpriteFactory>() != null)
		{
			this.m_NcSpriteFactoryPrefab = base.gameObject;
		}
		if (this.m_NcSpriteFactoryPrefab && this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
		{
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
	}

	private void Start()
	{
		this.UpdateSpriteTexture(true);
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, int nFrameIndex, bool bRunImmediate)
	{
		if (this.m_NcSpriteFactoryCom == null)
		{
			if (!this.m_NcSpriteFactoryPrefab || !(this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return;
			}
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		this.m_nSpriteFactoryIndex = nSpriteFactoryIndex;
		if (!this.m_NcSpriteFactoryCom.IsValidFactory())
		{
			return;
		}
		this.m_NcSpriteFrameInfos = this.m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex).m_FrameInfos;
		this.m_nFrameIndex = ((0 > nFrameIndex) ? this.m_nFrameIndex : nFrameIndex);
		this.m_nFrameIndex = ((this.m_NcSpriteFrameInfos.Length != 0 && this.m_NcSpriteFrameInfos.Length > this.m_nFrameIndex) ? this.m_nFrameIndex : 0);
		this.m_fUvScale = this.m_NcSpriteFactoryCom.m_fUvScale;
		if (bRunImmediate)
		{
			this.UpdateSpriteTexture(bRunImmediate);
		}
	}

	private void UpdateSpriteTexture(bool bShowEffect)
	{
		if (!this.UpdateSpriteMaterial())
		{
			return;
		}
		if (!this.m_NcSpriteFactoryCom.IsValidFactory())
		{
			return;
		}
		if (this.m_NcSpriteFrameInfos.Length == 0)
		{
			this.SetSpriteFactoryIndex(this.m_nSpriteFactoryIndex, this.m_nFrameIndex, false);
		}
		if (this.m_MeshFilter == null)
		{
			if (base.gameObject.GetComponent<MeshFilter>() != null)
			{
				this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			else
			{
				this.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
		}
		NcSpriteFactory.CreatePlane(this.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[this.m_nFrameIndex], false, this.m_AlignType, this.m_MeshType);
		NcSpriteFactory.UpdateMeshUVs(this.m_MeshFilter, this.m_NcSpriteFrameInfos[this.m_nFrameIndex].m_TextureUvOffset);
		if (bShowEffect)
		{
			this.m_EffectObject = this.m_NcSpriteFactoryCom.CreateSpriteEffect(this.m_nSpriteFactoryIndex, base.transform);
		}
	}

	public bool UpdateSpriteMaterial()
	{
		if (this.m_NcSpriteFactoryPrefab == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>() == null || this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial == null || this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial.mainTexture == null)
		{
			return false;
		}
		if (base.GetComponent<Renderer>() == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryCom == null)
		{
			return false;
		}
		if (this.m_nSpriteFactoryIndex < 0 || this.m_NcSpriteFactoryCom.GetSpriteNodeCount() <= this.m_nSpriteFactoryIndex)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture)
		{
			return false;
		}
		base.GetComponent<Renderer>().sharedMaterial = this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial;
		return true;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public override void OnUpdateToolData()
	{
	}
}
