using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Wwise/AkInitializer"), RequireComponent(typeof(AkTerminator))]
public class AkInitializer : MonoBehaviour
{
	public const string c_Language = "English(US)";

	public const int c_DefaultPoolSize = 4096;

	public const int c_LowerPoolSize = 2048;

	public const int c_StreamingPoolSize = 1024;

	public const float c_MemoryCutoffThreshold = 0.95f;

	public string basePath = AkBankPathUtil.GetDefaultPath();

	public string language = "English(US)";

	public int defaultPoolSize = 4096;

	public int lowerPoolSize = 2048;

	public int streamingPoolSize = 1024;

	public float memoryCutoffThreshold = 0.95f;

	public static bool s_loadBankFromMemory = true;

	private static AkInitializer ms_Instance;

	public static string GetBasePath()
	{
		return AkInitializer.ms_Instance.basePath;
	}

	public static string GetCurrentLanguage()
	{
		return AkInitializer.ms_Instance.language;
	}

	public static string GetSoundBankPathInResources(string bankName)
	{
		string path = string.Empty;
		path = "Sound/Android/";
		return CFileManager.CombinePath(path, bankName);
	}

	private void Awake()
	{
		if (AkInitializer.ms_Instance != null)
		{
			if (AkInitializer.ms_Instance != this)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			return;
		}
		Debug.Log("WwiseUnity: Initialize sound engine ...");
		AkMemSettings akMemSettings = new AkMemSettings();
		akMemSettings.uMaxNumPools = 40u;
		AkDeviceSettings akDeviceSettings = new AkDeviceSettings();
		AkSoundEngine.GetDefaultDeviceSettings(akDeviceSettings);
		AkStreamMgrSettings akStreamMgrSettings = new AkStreamMgrSettings();
		akStreamMgrSettings.uMemorySize = (uint)(this.streamingPoolSize * 1024);
		AkInitSettings akInitSettings = new AkInitSettings();
		AkSoundEngine.GetDefaultInitSettings(akInitSettings);
		akInitSettings.uDefaultPoolSize = (uint)(this.defaultPoolSize * 1024);
		AkPlatformInitSettings akPlatformInitSettings = new AkPlatformInitSettings();
		AkSoundEngine.GetDefaultPlatformInitSettings(akPlatformInitSettings);
		akPlatformInitSettings.uLEngineDefaultPoolSize = (uint)(this.lowerPoolSize * 1024);
		akPlatformInitSettings.fLEngineDefaultPoolRatioThreshold = this.memoryCutoffThreshold;
		AkMusicSettings akMusicSettings = new AkMusicSettings();
		AkSoundEngine.GetDefaultMusicSettings(akMusicSettings);
		AKRESULT aKRESULT = AkSoundEngine.Init(akMemSettings, akStreamMgrSettings, akDeviceSettings, akInitSettings, akPlatformInitSettings, akMusicSettings);
		if (aKRESULT != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
			return;
		}
		AkInitializer.ms_Instance = this;
		AkBankPathUtil.UsePlatformSpecificPath();
		string platformBasePath = AkBankPathUtil.GetPlatformBasePath();
		if (!AkInitializer.s_loadBankFromMemory)
		{
		}
		AkSoundEngine.SetBasePath(platformBasePath);
		AkSoundEngine.SetCurrentLanguage(this.language);
		aKRESULT = AkCallbackManager.Init();
		if (aKRESULT != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
			AkSoundEngine.Term();
			AkInitializer.ms_Instance = null;
			return;
		}
		Debug.Log("WwiseUnity: Sound engine initialized.");
		UnityEngine.Object.DontDestroyOnLoad(this);
		if (AkInitializer.s_loadBankFromMemory)
		{
			string soundBankPathInResources = AkInitializer.GetSoundBankPathInResources("Init.bytes");
			CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource(soundBankPathInResources, typeof(TextAsset), enResourceType.Sound, false, false).m_content as CBinaryObject;
			GCHandle gCHandle = GCHandle.Alloc(cBinaryObject.m_data, GCHandleType.Pinned);
			IntPtr intPtr = gCHandle.AddrOfPinnedObject();
			if (intPtr != IntPtr.Zero)
			{
				uint num;
				aKRESULT = AkSoundEngine.LoadBank(intPtr, (uint)cBinaryObject.m_data.Length, -1, out num);
				gCHandle.Free();
			}
			else
			{
				aKRESULT = AKRESULT.AK_Fail;
			}
			Singleton<CResourceManager>.GetInstance().RemoveCachedResource(soundBankPathInResources);
		}
		else
		{
			uint num;
			aKRESULT = AkSoundEngine.LoadBank("Init.bnk", -1, out num);
		}
		if (aKRESULT != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: Failed load Init.bnk with result: " + aKRESULT.ToString());
		}
	}

	private void OnDestroy()
	{
		if (AkInitializer.ms_Instance == this)
		{
			AkCallbackManager.SetMonitoringCallback((ErrorLevel)0, null);
			AkInitializer.ms_Instance = null;
		}
	}

	private void OnEnable()
	{
		if (AkInitializer.ms_Instance == null && AkSoundEngine.IsInitialized())
		{
			AkInitializer.ms_Instance = this;
		}
	}

	private void LateUpdate()
	{
		if (AkInitializer.ms_Instance != null)
		{
			AkCallbackManager.PostCallbacks();
			AkSoundEngine.RenderAudio();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (AkInitializer.ms_Instance != null)
		{
			if (!pauseStatus)
			{
				AkSoundEngine.WakeupFromSuspend();
			}
			else
			{
				AkSoundEngine.Suspend();
			}
			AkSoundEngine.RenderAudio();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
	}
}
