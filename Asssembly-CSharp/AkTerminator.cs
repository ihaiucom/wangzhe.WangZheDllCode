using System;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Wwise/AkTerminator")]
public class AkTerminator : MonoBehaviour
{
	private static AkTerminator ms_Instance;

	private void Awake()
	{
		if (AkTerminator.ms_Instance != null)
		{
			if (AkTerminator.ms_Instance != this)
			{
				Object.DestroyImmediate(this);
			}
			return;
		}
		Object.DontDestroyOnLoad(this);
		AkTerminator.ms_Instance = this;
	}

	private void OnApplicationQuit()
	{
		this.Terminate();
	}

	private void OnDestroy()
	{
		if (AkTerminator.ms_Instance == this)
		{
			AkTerminator.ms_Instance = null;
		}
	}

	private void Terminate()
	{
		if (AkTerminator.ms_Instance == null || AkTerminator.ms_Instance != this || !AkSoundEngine.IsInitialized())
		{
			return;
		}
		AkSoundEngine.StopAll();
		AkSoundEngine.RenderAudio();
		for (uint num = 0u; num < 50u; num += 1u)
		{
			AkCallbackManager.PostCallbacks();
			using (EventWaitHandle eventWaitHandle = new ManualResetEvent(false))
			{
				eventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1.0));
			}
		}
		AkSoundEngine.Term();
		AkTerminator.ms_Instance = null;
		AkCallbackManager.Term();
	}
}
