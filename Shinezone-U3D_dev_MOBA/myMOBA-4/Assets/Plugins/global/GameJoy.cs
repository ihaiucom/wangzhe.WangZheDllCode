using System;
using System.Collections.Generic;
using UnityEngine;

public class GameJoy : Singleton<GameJoy>
{
	[Flags]
	public enum SDKFeature
	{
		Moment = 1,
		Manual = 2,
		InGameAudio = 4
	}

	public enum VideoQuality
	{
		High,
		Low
	}

	public static long getSystemCurrentTimeMillis
	{
		get
		{
			return GameJoySDK.getSystemCurrentTimeMillis();
		}
	}

	public bool isShowed
	{
		get
		{
			return GameJoySDK.instance.IsShowed();
		}
	}

	public bool isRecording
	{
		get
		{
			return GameJoySDK.instance.IsRecording();
		}
	}

	public Vector2 currentRecorderPosition
	{
		get
		{
			string curRecorderPosition = GameJoySDK.instance.getCurRecorderPosition();
			if (curRecorderPosition == null)
			{
				return new Vector2(0f, 0f);
			}
			string[] array = curRecorderPosition.Split(new string[]
			{
				","
			}, StringSplitOptions.None);
			return new Vector2(float.Parse(array[0]), 1f - float.Parse(array[1]));
		}
	}

	public bool isRecordingMoments
	{
		get
		{
			return GameJoySDK.instance.isRecordingMoments();
		}
	}

	public override void Init()
	{
		base.Init();
		if (GameJoySDK.SetupGameJoySDK() == null)
		{
			Debug.LogError("GameRecorder Failed Setup GameRecorder!");
		}
	}

	public static void CheckSDKFeature()
	{
		try
		{
			GameJoySDK.CheckSDKFeature();
		}
		catch (Exception var_0_0A)
		{
			GameJoySDK.Log("GameRecorder call Exception ");
			GameJoy.OnCheckSDKFeature(0);
		}
	}

	public static void OnCheckSDKFeature(int sdkFeature)
	{
		Debug.Log("OnCheckSDKFeature: " + sdkFeature);
		Singleton<EventRouter>.instance.BroadCastEvent<GameJoy.SDKFeature>(EventID.GAMEJOY_SDK_FEATURE_CHECK_RESULT, (GameJoy.SDKFeature)sdkFeature);
	}

	public static void CheckSDKPermission()
	{
		GameJoy.OnFinishCheckSDKPremission(true);
	}

	public static void OnFinishCheckSDKPremission(bool bResult)
	{
		Debug.Log("OnFinishCheckSDKPremission: " + ((!bResult) ? "NO" : "YES"));
		Singleton<EventRouter>.instance.BroadCastEvent<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, bResult);
	}

	public void StartRecorder()
	{
		GameJoySDK.instance.showGameJoyRecorder();
	}

	public void StopRecorder()
	{
		GameJoySDK.instance.dismissGameJoyRecorder();
	}

	public void SetCurrentRecorderPosition(float x, float y)
	{
		y = 1f - y;
		GameJoySDK.instance.setCurRecorderPosition(x, y);
	}

	public void LockRecorderPosition()
	{
		GameJoySDK.instance.lockRecorderPosition();
	}

	public void UnLockRecorderPosition()
	{
		GameJoySDK.instance.unLockRecorderPosition();
	}

	public void ShowVideoListDialog()
	{
		GameJoySDK.instance.showVideoListDialog();
	}

	public void CloseVideoListDialog()
	{
		GameJoySDK.instance.closeVideoListDialog();
	}

	public void SetVideoQuality(GameJoy.VideoQuality quality)
	{
		GameJoySDK.instance.setVideoQuality((int)quality);
	}

	public void SetDefaultUploadShareDialogPosition(float x, float y)
	{
		if (y >= 0f && y <= 1f)
		{
			y = 1f - y;
		}
		GameJoySDK.instance.setUploadShareDialogDefaultPosition(x, y);
	}

	public void StartMomentsRecording()
	{
		GameJoySDK.instance.startMomentRecording();
	}

	public static void OnStartMomentsRecording(bool bResult)
	{
		Singleton<EventRouter>.instance.BroadCastEvent<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, bResult);
	}

	public void EndMomentsRecording()
	{
		GameJoySDK.instance.endMomentRecording();
	}

	public static void OnStopMomentsRecording(long duration)
	{
		Singleton<EventRouter>.instance.BroadCastEvent<long>(EventID.GAMEJOY_STOPRECORDING_RESULT, duration);
	}

	public void GenerateMomentsVideo(List<TimeStamp> timeStampList, string title, Dictionary<string, string> extraInfo)
	{
		GameJoySDK.instance.generateMomentVideo(timeStampList, title, extraInfo);
	}
}
