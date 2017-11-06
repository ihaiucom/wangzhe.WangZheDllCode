using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameJoyAndroidPreRender : MonoBehaviour
{
	private bool bLogPostRender = true;

	private int bIsNewVersionSDK = -1;

	private long lTotalFrames;

	private double dCaptureFrameUsedMS;

	private long lTotalCaptureFrames;

	private double dRenderCameraUsedMS;

	[DllImport("libMMCodecSdk")]
	private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_CallMethod(IntPtr JNIEnv, IntPtr thiz, int nMethodID, int nParam1, int nParam2, int nParam3, int nParam4, int nParam5);

	[DllImport("libMMCodecSdk")]
	private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_OnRecordeFrame(IntPtr JNIEnv, IntPtr thiz);

	[DllImport("libMMCodecSdk")]
	private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_BeginDraw(IntPtr JNIEnv, IntPtr thiz);

	[DllImport("libMMCodecSdk")]
	private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_EndDraw(IntPtr JNIEnv, IntPtr thiz);

	private void Start()
	{
		this.bIsNewVersionSDK = -1;
		this.lTotalFrames = 0L;
		this.dCaptureFrameUsedMS = 0.0;
		this.lTotalCaptureFrames = 0L;
		this.dRenderCameraUsedMS = 0.0;
	}

	private void RerenderCameraFrame()
	{
		TimeSpan timeOfDay = DateTime.get_Now().get_TimeOfDay();
		Camera[] allCameras = Camera.allCameras;
		if (allCameras != null)
		{
			Camera main = Camera.main;
			if (main != null)
			{
				main.Render();
			}
			Camera[] array = allCameras;
			for (int i = 0; i < array.Length; i++)
			{
				Camera camera = array[i];
				if (camera != null && camera != main && camera.enabled && camera.targetTexture == null)
				{
					camera.Render();
				}
			}
		}
	}

	private bool DoCaptureFrame()
	{
		bool result = false;
		int num = GameJoyAndroidPreRender.Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_OnRecordeFrame(IntPtr.Zero, IntPtr.Zero);
		num = GameJoyAndroidPreRender.Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_BeginDraw(IntPtr.Zero, IntPtr.Zero);
		if (num == 1)
		{
			if (this.bLogPostRender)
			{
				GameJoySDK.Log("GameRecorder: DoCaptureFrame Render 1 Frame.");
			}
			this.RerenderCameraFrame();
			if (this.bLogPostRender)
			{
				this.bLogPostRender = false;
			}
			result = true;
		}
		num = GameJoyAndroidPreRender.Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_EndDraw(IntPtr.Zero, IntPtr.Zero);
		return result;
	}

	[DebuggerHidden]
	private IEnumerator CoroutineCaptureFrame()
	{
		GameJoyAndroidPreRender.<CoroutineCaptureFrame>c__Iterator7 <CoroutineCaptureFrame>c__Iterator = new GameJoyAndroidPreRender.<CoroutineCaptureFrame>c__Iterator7();
		<CoroutineCaptureFrame>c__Iterator.<>f__this = this;
		return <CoroutineCaptureFrame>c__Iterator;
	}

	private void OnCaptureFrame()
	{
		if (this.lTotalFrames == 0L)
		{
			GameJoySDK.Log("GameRecorder: Enter OnCaptureFrame.");
		}
		this.lTotalFrames += 1L;
		if (this.lTotalFrames % 2L == 0L)
		{
			return;
		}
		if (this.bIsNewVersionSDK == -1)
		{
			this.bIsNewVersionSDK = GameJoySDK.getGameJoyInstance().IsNewSDKVersion();
			if (this.bIsNewVersionSDK != -1)
			{
				GameJoySDK.Log("GameRecorder: OnPostRender SrpVersion:" + this.bIsNewVersionSDK);
			}
		}
		if (this.bIsNewVersionSDK == -1)
		{
			return;
		}
		if (this.bIsNewVersionSDK == 0)
		{
			return;
		}
		if (GameJoySDK.getGameJoyInstance().GetRecorderStatus() != GameJoySDK.RECORER_STATUS.RS_STARTED)
		{
			return;
		}
		base.StartCoroutine(this.CoroutineCaptureFrame());
	}

	private void OnPostRender()
	{
		this.OnCaptureFrame();
	}
}
