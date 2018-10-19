using System;
using UnityEngine;

public class NcSpriteAnimation : NcEffectAniBehaviour, IPooledMonoBehaviour
{
	public enum TEXTURE_TYPE
	{
		TileTexture,
		TrimTexture,
		SpriteFactory
	}

	public enum PLAYMODE
	{
		DEFAULT,
		INVERSE,
		PINGPONG,
		RANDOM,
		SELECT
	}

	public NcSpriteAnimation.TEXTURE_TYPE m_TextureType;

	public NcSpriteAnimation.PLAYMODE m_PlayMode;

	public float m_fDelayTime;

	public int m_nStartFrame;

	public int m_nFrameCount;

	public int m_nSelectFrame;

	public bool m_bLoop = true;

	public int m_nLoopStartFrame;

	public int m_nLoopFrameCount;

	public int m_nLoopingCount;

	public bool m_bAutoDestruct;

	public float m_fFps = 10f;

	public int m_nTilingX = 2;

	public int m_nTilingY = 2;

	protected GameObject m_NcSpriteFactoryPrefab;

	private Renderer m_NcSpriteFactoryPrefabRenderer;

	protected NcSpriteFactory m_NcSpriteFactoryCom;

	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;

	public float m_fUvScale = 1f;

	public int m_nSpriteFactoryIndex;

	public NcSpriteFactory.MESH_TYPE m_MeshType;

	public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;

	public bool m_bTrimCenterAlign;

	protected bool m_bCreateBuiltInPlane;

	[HideInInspector]
	public bool m_bBuildSpriteObj;

	[HideInInspector]
	public bool m_bNeedRebuildAlphaChannel;

	[HideInInspector]
	public AnimationCurve m_curveAlphaWeight;

	protected Vector2 m_size;

	protected Renderer m_Renderer;

	protected float m_fStartTime;

	protected int m_nLastIndex = -999;

	protected int m_nLastSeqIndex = -1;

	protected bool m_bInPartLoop;

	protected bool m_bBreakLoop;

	protected Vector2[] m_MeshUVsByTileTexture;

	private bool m_inited;

	private bool m_gotten;

	private bool m_enabled = true;

	public GameObject NcSpriteFactoryPrefab
	{
		get
		{
			return this.m_NcSpriteFactoryPrefab;
		}
		set
		{
			this.m_NcSpriteFactoryPrefab = value;
			if (null != this.m_NcSpriteFactoryPrefab)
			{
				this.m_NcSpriteFactoryPrefabRenderer = this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>();
			}
		}
	}

	public override int GetAnimationState()
	{
		if (!this.m_enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (this.m_fStartTime == 0f || !base.IsEndAnimation())
		{
			return 1;
		}
		return 0;
	}

	public float GetDurationTime()
	{
		return (float)((this.m_PlayMode != NcSpriteAnimation.PLAYMODE.PINGPONG) ? this.m_nFrameCount : (this.m_nFrameCount * 2 - 1)) / this.m_fFps;
	}

	public int GetShowIndex()
	{
		return this.m_nLastIndex + this.m_nStartFrame;
	}

	public void SetBreakLoop()
	{
		this.m_bBreakLoop = true;
	}

	public bool IsInPartLoop()
	{
		return this.m_bInPartLoop;
	}

	public override void ResetAnimation()
	{
		this.m_nLastIndex = -1;
		this.m_nLastSeqIndex = -1;
		if (!this.m_enabled)
		{
			this.SetEnable(true);
		}
		this.Start();
	}

	private void SetEnable(bool bEnable)
	{
		this.m_enabled = bEnable;
		if (null != this.m_Renderer)
		{
			this.m_Renderer.enabled = bEnable;
		}
	}

	public void SetSelectFrame(int nSelFrame)
	{
		this.m_nSelectFrame = nSelFrame;
		this.SetIndex(this.m_nSelectFrame);
	}

	public bool IsEmptyFrame()
	{
		return this.m_NcSpriteFrameInfos[this.m_nSelectFrame].m_bEmptyFrame;
	}

	public int GetMaxFrameCount()
	{
		if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
		{
			return this.m_nTilingX * this.m_nTilingY;
		}
		return this.m_NcSpriteFrameInfos.Length;
	}

	public int GetValidFrameCount()
	{
		if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
		{
			return this.m_nTilingX * this.m_nTilingY - this.m_nStartFrame;
		}
		return this.m_NcSpriteFrameInfos.Length - this.m_nStartFrame;
	}

	public void OnCreate()
	{
		if (this.m_inited)
		{
			return;
		}
		this.m_inited = true;
		if (this.m_NcSpriteFactoryPrefab == null && base.gameObject.GetComponent<NcSpriteFactory>() != null)
		{
			this.NcSpriteFactoryPrefab = base.gameObject;
		}
		if (this.m_NcSpriteFactoryPrefab && this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
		{
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		if (this.m_MeshFilter == null && base.gameObject.GetComponent<MeshFilter>() != null)
		{
			this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
		}
		if (this.m_nLoopFrameCount == 0)
		{
			this.m_nLoopFrameCount = this.m_nFrameCount - this.m_nLoopStartFrame;
		}
	}

	public void OnGet()
	{
		if (this.m_gotten)
		{
			return;
		}
		this.m_gotten = true;
		this.ResetLocalValue();
		if (this.m_Renderer == null)
		{
			this.SetEnable(false);
			return;
		}
		this.SetEnable(true);
		if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
		{
			this.SetIndex(this.m_nSelectFrame);
		}
		else
		{
			if (0f < this.m_fDelayTime)
			{
				this.m_Renderer.enabled = false;
				return;
			}
			if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.RANDOM)
			{
				this.SetIndex(UnityEngine.Random.Range(0, this.m_nFrameCount - 1));
			}
			else
			{
				base.InitAnimationTimer();
				this.SetIndex(0);
			}
		}
	}

	public void OnRecycle()
	{
		this.m_gotten = false;
	}

	private void Awake()
	{
		this.OnCreate();
	}

	private void ResetLocalValue()
	{
		this.m_size = new Vector2(1f / (float)this.m_nTilingX, 1f / (float)this.m_nTilingY);
		this.m_Renderer = base.GetComponent<Renderer>();
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_nFrameCount = ((this.m_nFrameCount > 0) ? this.m_nFrameCount : (this.m_nTilingX * this.m_nTilingY));
		this.m_nLastIndex = -999;
		this.m_nLastSeqIndex = -1;
		this.m_bInPartLoop = false;
		this.m_bBreakLoop = false;
	}

	private void Start()
	{
		this.OnGet();
	}

	private void Update()
	{
		if (!this.m_enabled)
		{
			return;
		}
		if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
		{
			return;
		}
		if (this.m_Renderer == null || this.m_nTilingX * this.m_nTilingY == 0)
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
			this.m_Renderer.enabled = true;
		}
		if (this.m_PlayMode != NcSpriteAnimation.PLAYMODE.RANDOM)
		{
			int num = (int)(this.m_Timer.GetTime() * this.m_fFps);
			if (num == 0 && this.m_NcSpriteFactoryCom != null)
			{
				this.m_NcSpriteFactoryCom.OnAnimationStartFrame(this);
			}
			if (this.m_NcSpriteFactoryCom != null && this.m_nFrameCount <= 0)
			{
				this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 0);
			}
			else
			{
				if (((this.m_PlayMode != NcSpriteAnimation.PLAYMODE.PINGPONG) ? this.m_nFrameCount : (this.m_nFrameCount * 2 - 1)) <= num)
				{
					if (!this.m_bLoop)
					{
						if (this.m_NcSpriteFactoryCom != null && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 1))
						{
							return;
						}
						this.UpdateEndAnimation();
						return;
					}
					else if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.PINGPONG)
					{
						if (this.m_NcSpriteFactoryCom != null && num % (this.m_nFrameCount * 2 - 2) == 1 && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, num / (this.m_nFrameCount * 2 - 1)))
						{
							return;
						}
					}
					else if (this.m_NcSpriteFactoryCom != null && num % this.m_nFrameCount == 0 && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, num / this.m_nFrameCount))
					{
						return;
					}
				}
				this.SetIndex(num);
			}
		}
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, bool bRunImmediate)
	{
		if (this.m_NcSpriteFactoryCom == null)
		{
			if (!this.m_NcSpriteFactoryPrefab || !(this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return;
			}
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		NcSpriteFactory.NcSpriteNode spriteNode = this.m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex);
		this.m_bBuildSpriteObj = false;
		this.m_bAutoDestruct = false;
		this.m_fUvScale = this.m_NcSpriteFactoryCom.m_fUvScale;
		this.m_nSpriteFactoryIndex = nSpriteFactoryIndex;
		this.m_nStartFrame = 0;
		DebugHelper.Assert(spriteNode != null);
		if (spriteNode != null)
		{
			this.m_nFrameCount = spriteNode.m_nFrameCount;
			this.m_fFps = spriteNode.m_fFps;
			this.m_bLoop = spriteNode.m_bLoop;
			this.m_nLoopStartFrame = spriteNode.m_nLoopStartFrame;
			this.m_nLoopFrameCount = spriteNode.m_nLoopFrameCount;
			this.m_nLoopingCount = spriteNode.m_nLoopingCount;
			this.m_NcSpriteFrameInfos = spriteNode.m_FrameInfos;
		}
		this.ResetLocalValue();
	}

	private int GetPartLoopFrameIndex(int nSeqIndex)
	{
		if (nSeqIndex < 0)
		{
			return -1;
		}
		int num = nSeqIndex - this.m_nLoopStartFrame;
		if (num < 0)
		{
			return nSeqIndex;
		}
		int num2 = num / this.m_nLoopFrameCount;
		if (this.m_nLoopingCount == 0 || num2 < this.m_nLoopingCount)
		{
			return num % this.m_nLoopFrameCount + this.m_nLoopStartFrame;
		}
		return num - this.m_nLoopFrameCount * (this.m_nLoopingCount - 1) + this.m_nLoopStartFrame;
	}

	private int GetPartLoopCount(int nSeqIndex)
	{
		if (nSeqIndex < 0)
		{
			return -1;
		}
		int num = nSeqIndex - this.m_nLoopStartFrame;
		if (num < 0)
		{
			return -1;
		}
		int num2 = num / this.m_nLoopFrameCount;
		if (this.m_nLoopingCount == 0 || num2 < this.m_nLoopingCount)
		{
			return num2;
		}
		return this.m_nLoopingCount;
	}

	private int CalcPartLoopInfo(int nSeqIndex, ref int nLoopCount)
	{
		if (this.m_nLoopFrameCount <= 0)
		{
			return 0;
		}
		if (this.m_bBreakLoop)
		{
			nLoopCount = this.GetPartLoopCount(nSeqIndex);
			this.UpdateEndAnimation();
			this.m_bBreakLoop = false;
			return this.m_nLoopStartFrame + this.m_nLoopFrameCount;
		}
		if (nSeqIndex < this.m_nLoopStartFrame)
		{
			return nSeqIndex;
		}
		this.m_bInPartLoop = true;
		int partLoopCount = this.GetPartLoopCount(this.m_nLastSeqIndex);
		int num = this.GetPartLoopFrameIndex(nSeqIndex);
		nLoopCount = this.GetPartLoopCount(nSeqIndex);
		int num2 = 0;
		int i = partLoopCount;
		while (i < Mathf.Min(nLoopCount, this.m_nLoopFrameCount - 1))
		{
			if (base.transform.parent != null)
			{
				base.transform.parent.SendMessage("OnSpriteAnimationLoopStart", nLoopCount, SendMessageOptions.DontRequireReceiver);
			}
			i++;
			num2++;
		}
		if (0 < this.m_nLoopingCount && this.m_nLoopingCount <= nLoopCount)
		{
			this.m_bInPartLoop = false;
			if (base.transform.parent != null)
			{
				base.transform.parent.SendMessage("OnSpriteAnimationLoopEnd", nLoopCount, SendMessageOptions.DontRequireReceiver);
			}
			if (this.m_nFrameCount <= num)
			{
				num = this.m_nFrameCount - 1;
				this.UpdateEndAnimation();
			}
		}
		return num;
	}

	private void UpdateEndAnimation()
	{
		this.SetEnable(false);
		base.OnEndAnimation();
		if (this.m_bAutoDestruct)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void SetIndex(int nSeqIndex)
	{
		if (this.m_Renderer != null)
		{
			this.m_nLastSeqIndex = nSeqIndex;
			int num = nSeqIndex;
			int nLoopCount = nSeqIndex / this.m_nFrameCount;
			switch (this.m_PlayMode)
			{
			case NcSpriteAnimation.PLAYMODE.DEFAULT:
				if (this.m_bLoop)
				{
					num = this.CalcPartLoopInfo(nSeqIndex, ref nLoopCount) % this.m_nFrameCount;
				}
				else
				{
					num = nSeqIndex % this.m_nFrameCount;
				}
				break;
			case NcSpriteAnimation.PLAYMODE.INVERSE:
				num = this.m_nFrameCount - num % this.m_nFrameCount - 1;
				break;
			case NcSpriteAnimation.PLAYMODE.PINGPONG:
				nLoopCount = num / (this.m_nFrameCount * 2 - ((num != 0) ? 2 : 1));
				num %= this.m_nFrameCount * 2 - ((num != 0) ? 2 : 1);
				if (this.m_nFrameCount <= num)
				{
					num = this.m_nFrameCount - num % this.m_nFrameCount - 2;
				}
				break;
			case NcSpriteAnimation.PLAYMODE.SELECT:
				num = nSeqIndex % this.m_nFrameCount;
				break;
			}
			if (num == this.m_nLastIndex)
			{
				return;
			}
			if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
			{
				int num2 = (num + this.m_nStartFrame) % this.m_nTilingX;
				int num3 = (num + this.m_nStartFrame) / this.m_nTilingX;
				Vector2 mainTextureOffset = new Vector2((float)num2 * this.m_size.x, 1f - this.m_size.y - (float)num3 * this.m_size.y);
				if (!this.UpdateMeshUVsByTileTexture(new Rect(mainTextureOffset.x, mainTextureOffset.y, this.m_size.x, this.m_size.y)))
				{
					this.m_Renderer.material.mainTextureOffset = mainTextureOffset;
					this.m_Renderer.material.mainTextureScale = this.m_size;
					base.AddRuntimeMaterial(this.m_Renderer.material);
				}
			}
			else if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TrimTexture)
			{
				this.UpdateSpriteTexture(num, true);
			}
			else if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.SpriteFactory)
			{
				this.UpdateFactoryTexture(num, true);
			}
			if (this.m_NcSpriteFactoryCom != null)
			{
				this.m_NcSpriteFactoryCom.OnAnimationChangingFrame(this, this.m_nLastIndex, num, nLoopCount);
			}
			this.m_nLastIndex = num;
		}
	}

	private void UpdateSpriteTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += this.m_nStartFrame;
		if (this.m_NcSpriteFrameInfos == null || nSelIndex < 0 || this.m_NcSpriteFrameInfos.Length <= nSelIndex)
		{
			return;
		}
		this.CreateBuiltInPlane(nSelIndex);
		this.UpdateBuiltInPlane(nSelIndex);
	}

	private void UpdateFactoryTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += this.m_nStartFrame;
		if (this.m_NcSpriteFrameInfos == null || nSelIndex < 0 || this.m_NcSpriteFrameInfos.Length <= nSelIndex)
		{
			return;
		}
		if (!this.UpdateFactoryMaterial())
		{
			return;
		}
		if (!this.m_NcSpriteFactoryCom.IsValidFactory())
		{
			return;
		}
		this.CreateBuiltInPlane(nSelIndex);
		this.UpdateBuiltInPlane(nSelIndex);
	}

	public bool UpdateFactoryMaterial()
	{
		if (this.m_NcSpriteFactoryPrefab == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryPrefabRenderer == null || this.m_NcSpriteFactoryPrefabRenderer.sharedMaterial == null || this.m_NcSpriteFactoryPrefabRenderer.sharedMaterial.mainTexture == null)
		{
			return false;
		}
		if (base.GetRenderer() == null)
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
		if (this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
		{
			return false;
		}
		base.GetRenderer().sharedMaterial = this.m_NcSpriteFactoryPrefabRenderer.sharedMaterial;
		return true;
	}

	private void CreateBuiltInPlane(int nSelIndex)
	{
		if (this.m_bCreateBuiltInPlane)
		{
			return;
		}
		this.m_bCreateBuiltInPlane = true;
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
		NcSpriteFactory.CreatePlane(this.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[nSelIndex], this.m_TextureType != NcSpriteAnimation.TEXTURE_TYPE.TileTexture && this.m_bTrimCenterAlign, this.m_AlignType, this.m_MeshType);
	}

	private void UpdateBuiltInPlane(int nSelIndex)
	{
		NcSpriteFactory.UpdatePlane(this.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[nSelIndex], this.m_TextureType != NcSpriteAnimation.TEXTURE_TYPE.TileTexture && this.m_bTrimCenterAlign, this.m_AlignType);
		NcSpriteFactory.UpdateMeshUVs(this.m_MeshFilter, this.m_NcSpriteFrameInfos[nSelIndex].m_TextureUvOffset);
	}

	private bool UpdateMeshUVsByTileTexture(Rect uv)
	{
		if (this.m_MeshFilter != null && this.m_MeshUVsByTileTexture == null)
		{
			return false;
		}
		if (this.m_MeshFilter == null)
		{
			this.m_MeshFilter = (MeshFilter)base.gameObject.GetComponent(typeof(MeshFilter));
		}
		if (this.m_MeshFilter == null || this.m_MeshFilter.sharedMesh == null)
		{
			return false;
		}
		if (this.m_MeshUVsByTileTexture == null)
		{
			for (int i = 0; i < this.m_MeshFilter.sharedMesh.uv.Length; i++)
			{
				if (this.m_MeshFilter.sharedMesh.uv[i].x != 0f && this.m_MeshFilter.sharedMesh.uv[i].x != 1f)
				{
					return false;
				}
				if (this.m_MeshFilter.sharedMesh.uv[i].y != 0f && this.m_MeshFilter.sharedMesh.uv[i].y != 1f)
				{
					return false;
				}
			}
			this.m_MeshUVsByTileTexture = this.m_MeshFilter.sharedMesh.uv;
		}
		Vector2[] array = new Vector2[this.m_MeshUVsByTileTexture.Length];
		for (int j = 0; j < this.m_MeshUVsByTileTexture.Length; j++)
		{
			if (this.m_MeshUVsByTileTexture[j].x == 0f)
			{
				array[j].x = uv.x;
			}
			if (this.m_MeshUVsByTileTexture[j].y == 0f)
			{
				array[j].y = uv.y;
			}
			if (this.m_MeshUVsByTileTexture[j].x == 1f)
			{
				array[j].x = uv.x + uv.width;
			}
			if (this.m_MeshUVsByTileTexture[j].y == 1f)
			{
				array[j].y = uv.y + uv.height;
			}
		}
		this.m_MeshFilter.mesh.uv = array;
		return true;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime *= fSpeedRate;
		this.m_fFps *= fSpeedRate;
	}
}
