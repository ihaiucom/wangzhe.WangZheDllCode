using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

public class FogOfWar
{
	public static bool ms_bDrawDebugLineTexts = true;

	private static bool _enable = false;

	public static int _BitmapWidth = 512;

	public static int _BitmapHeight = 512;

	private static byte[] _bitmapData = null;

	private static Texture2D _bitmapTexture = null;

	private static RenderTexture[] fowTextures = new RenderTexture[3];

	private static Material[] fowMats = new Material[3];

	private static long commitFrame = 0L;

	public static readonly byte FOG_DELAY_FRAME = 90;

	public static uint RenderFrameNum = 0u;

	public static VInt2 MainActorFakeSightPos
	{
		get
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain)
			{
				VInt3 location = hostPlayer.Captain.handle.location;
				location = new VInt3(location.x, location.z, 0);
				VInt2 zero = VInt2.zero;
				Singleton<GameFowManager>.instance.WorldPosToGrid(location, out zero.x, out zero.y);
				return zero;
			}
			return VInt2.zero;
		}
	}

	public static bool enable
	{
		get
		{
			return FogOfWar._enable;
		}
		set
		{
			if (FogOfWar._enable == value)
			{
				return;
			}
			FogOfWar._enable = value;
			if (!FogOfWar._enable)
			{
				FogOfWar.ClearAllFog(false);
			}
		}
	}

	private static void CreateMat(ref Material mat, string name)
	{
		if (mat != null)
		{
			return;
		}
		Shader shader = Shader.Find(name);
		if (shader == null)
		{
		}
		mat = new Material(shader);
	}

	private static void Reset(int inMapWidth, int inMapHeight, int inBitmapWidth, int inBitmapHeight, int sight)
	{
		FogOfWar._BitmapWidth = inBitmapWidth;
		FogOfWar._BitmapHeight = inBitmapHeight;
		if (inBitmapWidth == 0 || inBitmapHeight == 0 || sight == 0)
		{
			return;
		}
		FogOfWar.Clear();
		if (FogOfWar._bitmapTexture != null)
		{
			FogOfWar._bitmapTexture.Resize(FogOfWar._BitmapWidth, FogOfWar._BitmapHeight, TextureFormat.Alpha8, false);
		}
		else
		{
			FogOfWar._bitmapTexture = new Texture2D(FogOfWar._BitmapWidth, FogOfWar._BitmapHeight, TextureFormat.Alpha8, false);
			FogOfWar._bitmapTexture.wrapMode = TextureWrapMode.Clamp;
		}
		for (int i = 0; i < FogOfWar.fowTextures.Length; i++)
		{
			if (FogOfWar.fowTextures[i] != null)
			{
				FogOfWar.fowTextures[i].Release();
			}
			int num = FogOfWar._BitmapWidth * 2;
			int num2 = FogOfWar._BitmapHeight * 2;
			if (SystemInfo.npotSupport == NPOTSupport.None)
			{
				num = IntMath.CeilPowerOfTwo(num);
				num2 = IntMath.CeilPowerOfTwo(num2);
			}
			FogOfWar.fowTextures[i] = new RenderTexture(num, num2, 0, RenderTextureFormat.Default);
			FogOfWar.fowTextures[i].wrapMode = TextureWrapMode.Clamp;
		}
		FogOfWar.CreateMat(ref FogOfWar.fowMats[0], "SGame_Post/FowBlur");
		FogOfWar.CreateMat(ref FogOfWar.fowMats[1], "SGame_Post/FowInterpolate");
		FogOfWar.CreateMat(ref FogOfWar.fowMats[2], "SGame_Post/FowLight");
		if (inMapWidth == 0 || inMapHeight == 0)
		{
			return;
		}
	}

	private static void Clear()
	{
		Shader.SetGlobalTexture("_FogOfWar", null);
		FogOfWar._bitmapTexture = null;
		for (int i = 0; i < FogOfWar.fowTextures.Length; i++)
		{
			if (FogOfWar.fowTextures[i] != null)
			{
				FogOfWar.fowTextures[i].Release();
			}
		}
	}

	private static void ClearAllFog(bool bCommit)
	{
		if (FogOfWar._bitmapData == null)
		{
			return;
		}
		for (int i = 0; i < FogOfWar._bitmapData.Length; i++)
		{
			FogOfWar._bitmapData[i] = 255;
		}
		if (bCommit)
		{
			FogOfWar.CommitToMaterials();
		}
	}

	private static void SetBlurVectors(float sizeX, float sizeY)
	{
		Vector4 a = new Vector4(sizeX, sizeY, -sizeX, -sizeY);
		FogOfWar.fowMats[0].SetVector("_BlurDir1", a * 0.5f);
		FogOfWar.fowMats[0].SetVector("_BlurDir2", a * 1.5f);
	}

	private static void LightFowTex(int x, int y, int radius)
	{
		Vector4 vector = default(Vector4);
		vector.x = (float)radius / (float)FogOfWar._bitmapTexture.width;
		vector.y = (float)radius / (float)FogOfWar._bitmapTexture.height;
		vector.z = (float)x * 2f / (float)FogOfWar._bitmapTexture.width - 1f;
		vector.w = (float)y * 2f / (float)FogOfWar._bitmapTexture.height - 1f;
		Vector4 vector2 = default(Vector4);
		vector2.x = (float)FogOfWar._bitmapTexture.width;
		vector2.y = (float)FogOfWar._bitmapTexture.height;
		vector2.z = (float)(radius * radius);
		Material material = FogOfWar.fowMats[2];
		material.SetVector("_Transform", vector);
		material.SetVector("_Transform2", vector2);
		Graphics.Blit(FogOfWar._bitmapTexture, FogOfWar.fowTextures[0], material);
	}

	public static void CommitToMaterials()
	{
		if (!CheatCommandReplayEntry.commitFOWMaterial)
		{
			return;
		}
		if (FogOfWar._bitmapData == null)
		{
			return;
		}
		FogOfWar._bitmapTexture.LoadRawTextureData(FogOfWar._bitmapData);
		FogOfWar._bitmapTexture.Apply();
		RenderTexture renderTexture = FogOfWar.fowTextures[0];
		FogOfWar.fowTextures[0] = FogOfWar.fowTextures[1];
		FogOfWar.fowTextures[1] = renderTexture;
		FogOfWar.fowTextures[0].DiscardContents();
		Graphics.Blit(FogOfWar._bitmapTexture, FogOfWar.fowTextures[0]);
		FogOfWar.SetBlurVectors(1f / (float)FogOfWar.fowTextures[0].width, 0f);
		FogOfWar.fowTextures[2].DiscardContents();
		Graphics.Blit(FogOfWar.fowTextures[0], FogOfWar.fowTextures[2], FogOfWar.fowMats[0]);
		FogOfWar.SetBlurVectors(0f, 1f / (float)FogOfWar.fowTextures[0].height);
		FogOfWar.fowTextures[0].DiscardContents();
		Graphics.Blit(FogOfWar.fowTextures[2], FogOfWar.fowTextures[0], FogOfWar.fowMats[0]);
		RenderTexture.active = null;
		FogOfWar.commitFrame = (long)((ulong)Singleton<FrameSynchr>.instance.CurFrameNum);
		FogOfWar.UpdateTextures();
	}

	public static void UpdateTextures()
	{
		if (!FogOfWar.enable)
		{
			return;
		}
		FrameSynchr instance = Singleton<FrameSynchr>.instance;
		float num = ((ulong)instance.CurFrameNum - (ulong)FogOfWar.commitFrame) * Singleton<GameFowManager>.instance.GPUInterpolateReciprocal;
		if (instance.bActive)
		{
			uint num2 = (uint)((Time.realtimeSinceStartup - instance.startFrameTime) * 1000f);
			num2 *= (uint)instance.FrameSpeed;
			uint num3 = instance.CurFrameNum * instance.FrameDelta;
			int num4 = (int)(num2 - num3);
			num4 -= instance.nJitterDelay;
			num4 = Mathf.Clamp(num4, 0, (int)instance.FrameDelta);
			num += (float)num4 / instance.FrameDelta * Singleton<GameFowManager>.instance.GPUInterpolateReciprocal;
		}
		num = Mathf.Clamp01(num);
		FogOfWar.fowMats[1].SetTexture("_FowTex0", FogOfWar.fowTextures[1]);
		FogOfWar.fowMats[1].SetTexture("_FowTex1", FogOfWar.fowTextures[0]);
		FogOfWar.fowMats[1].SetFloat("_Factor", num);
		FogOfWar.fowTextures[2].DiscardContents();
		Graphics.Blit(null, FogOfWar.fowTextures[2], FogOfWar.fowMats[1]);
		RenderTexture.active = null;
		FogOfWar.fowMats[1].SetTexture("_FowTex0", null);
		FogOfWar.fowMats[1].SetTexture("_FowTex1", null);
		Shader.SetGlobalTexture("_FogOfWar", FogOfWar.fowTextures[2]);
	}

	private static void EnableShaderFogFunction()
	{
		if (!GameSettings.IsHighQuality)
		{
			Shader.DisableKeyword("_FOG_OF_WAR_ON");
			Shader.EnableKeyword("_FOG_OF_WAR_ON_LOW");
		}
		else
		{
			Shader.DisableKeyword("_FOG_OF_WAR_ON_LOW");
			Shader.EnableKeyword("_FOG_OF_WAR_ON");
		}
	}

	private static void DisableShaderFogFunction()
	{
		Shader.DisableKeyword("_FOG_OF_WAR_ON");
		Shader.DisableKeyword("_FOG_OF_WAR_ON_LOW");
	}

	public static void SetFlip(bool flip)
	{
		if (flip)
		{
			Shader.EnableKeyword("_FOG_OF_WAR_FLIP_ON");
		}
		else
		{
			Shader.EnableKeyword("_FOG_OF_WAR_FLIP_OFF");
		}
	}

	public static void UpdateMain()
	{
		BattleLogic instance = Singleton<BattleLogic>.instance;
		if (!FogOfWar.enable || !instance.isFighting)
		{
			return;
		}
		SLevelContext curLvelContext = instance.GetCurLvelContext();
		if (curLvelContext == null || curLvelContext.m_horizonEnableMethod != Horizon.EnableMethod.EnableAll)
		{
			return;
		}
		GameFowManager instance2 = Singleton<GameFowManager>.instance;
		if (Singleton<FrameSynchr>.instance.CurFrameNum % instance2.GPUInterpolateFrameInterval == 0u)
		{
			FogOfWar.CopyBitmap();
			FogOfWar.CommitToMaterials();
		}
		GameFowCollector collector = instance2.m_collector;
		collector.UpdateFowVisibility(false);
		collector.CollectExplorer(false);
		instance2.UpdateComputing();
	}

	public static void PrepareData()
	{
		BattleLogic instance = Singleton<BattleLogic>.instance;
		if (!FogOfWar.enable || !instance.isFighting)
		{
			return;
		}
		SLevelContext curLvelContext = instance.GetCurLvelContext();
		if (curLvelContext == null || curLvelContext.m_horizonEnableMethod != Horizon.EnableMethod.EnableAll)
		{
			return;
		}
		GameFowManager instance2 = Singleton<GameFowManager>.instance;
		GameFowCollector collector = instance2.m_collector;
		collector.UpdateFowVisibility(false);
		collector.CollectExplorer(false);
		if (Singleton<FrameSynchr>.instance.CurFrameNum % instance2.GPUInterpolateFrameInterval == 0u)
		{
			FogOfWar.CommitToMaterials();
		}
	}

	public static void Run()
	{
		if (!FogOfWar.enable || !Singleton<BattleLogic>.instance.isFighting)
		{
			return;
		}
		GameFowManager instance = Singleton<GameFowManager>.instance;
		instance.UpdateComputing();
		if (Singleton<FrameSynchr>.instance.CurFrameNum % instance.GPUInterpolateFrameInterval == 0u)
		{
			FogOfWar.CopyBitmap();
		}
	}

	public static void PreBeginLevel()
	{
		GameObject gameObject = GameObject.Find("Design/Field");
		if (gameObject != null && gameObject.activeInHierarchy)
		{
			FieldObj component = gameObject.GetComponent<FieldObj>();
			if (component != null && component.bSynced && component.fowOfflineData != null && component.fowOfflineData.Length > 0 && Singleton<BattleLogic>.instance.GetCurLvelContext().m_bEnableFow)
			{
				FogOfWar.enable = true;
			}
		}
	}

	public static void BeginLevel()
	{
		FogOfWar.DisableShaderFogFunction();
		if (FogOfWar.enable)
		{
			GC.Collect();
			GameObject gameObject = GameObject.Find("Design/Field");
			if (gameObject != null && gameObject.activeInHierarchy)
			{
				FieldObj component = gameObject.GetComponent<FieldObj>();
				if (component != null)
				{
					FogOfWar.EnableShaderFogFunction();
					component.InitField();
					int inFakeSightRange = 0;
					component.UnrealToGridX(Horizon.QueryMainActorFakeSightRadius(), out inFakeSightRange);
					Singleton<GameFowManager>.instance.InitSurface(true, component, inFakeSightRange);
					if (Singleton<GameFowManager>.instance.LoadPrecomputeData())
					{
						FogOfWar.Reset(component.FieldX, component.FieldY, component.NumX, component.NumY, (int)GameDataMgr.globalInfoDatabin.GetDataByKey(56u).dwConfValue);
						FogOfWar.ClearAllFog(true);
						float num = Mathf.Max((float)component.FieldX / 1000f, 1f);
						float num2 = Mathf.Max((float)component.FieldY / 1000f, 1f);
						Shader.SetGlobalVector("_InvSceneSize", new Vector4(1f / num, 1f / num2, num, num2));
					}
				}
			}
		}
	}

	public static void EndLevel()
	{
		FogOfWar.DisableShaderFogFunction();
		FogOfWar.enable = false;
		Singleton<GameFowManager>.instance.UninitSurface();
		FogOfWar.Clear();
		FogOfWar._bitmapData = null;
		FogOfWar.RenderFrameNum = 0u;
	}

	private static int Power2(int value)
	{
		return value * value;
	}

	public static void CopyBitmap()
	{
		FowLos.PreCopyBitmap();
		if (!Singleton<WatchController>.instance.IsWatching)
		{
			VInt2 mainActorFakeSightPos = FogOfWar.MainActorFakeSightPos;
			FieldObj pFieldObj = Singleton<GameFowManager>.instance.m_pFieldObj;
			FowLos.PreCopyBitmapFakeSight(mainActorFakeSightPos.x, mainActorFakeSightPos.y, pFieldObj.NumX, pFieldObj.NumY);
		}
		FogOfWar._bitmapData = Singleton<GameFowManager>.instance.GetCommitPixels();
	}
}
