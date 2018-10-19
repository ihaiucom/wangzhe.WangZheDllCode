using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameJoySDK : MonoBehaviour
{
	public enum RECORER_STATUS
	{
		RS_UNUSED = -1,
		RS_STOPED,
		RS_STARTED
	}

	public class CheckSDKFeatureCallback : AndroidJavaProxy
	{
		public CheckSDKFeatureCallback() : base("com.tencent.qqgamemi.CheckSDKFeatureCallback")
		{
		}

		private void check(int sdkFeature)
		{
			Debug.Log("CheckSDKFeatureCallback: " + sdkFeature);
			GameJoy.OnCheckSDKFeature(sdkFeature);
		}
	}

	public const string GAMEJOY_UNITY3D_CS_VERSION = "2015-09-22-0002-Camera-Render";

	public bool enableOnStart = true;

	public static float recorderX = -1f;

	public static float recorderY = -1f;

	private static GameJoySDK.RECORER_STATUS mRecorderStatus = GameJoySDK.RECORER_STATUS.RS_UNUSED;

	private static int mSDKVersion = -1;

	private static AndroidJavaClass mQMiObj;

	private static AndroidJavaObject playerActivityContext;

	private static GameJoySDK singletonInstance;

	public static GameJoySDK instance
	{
		get
		{
			if (GameJoySDK.singletonInstance == null)
			{
				GameJoySDK.Log("GameRecorder get instance before setup sdk!");
			}
			return GameJoySDK.singletonInstance;
		}
	}

	private void Start()
	{
		this.InitRecordPlugin();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static GameJoySDK SetupGameJoySDK()
	{
		if (GameJoySDK.singletonInstance != null)
		{
			return GameJoySDK.singletonInstance;
		}
		GameObject gameObject = new GameObject("GameJoySDK");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		GameJoySDK.singletonInstance = gameObject.AddComponent<GameJoySDK>();
		return GameJoySDK.instance;
	}

	public static GameJoySDK getGameJoyInstance()
	{
		return GameJoySDK.instance;
	}

	public void setDefaultStartPosition(float x, float y)
	{
		GameJoySDK.recorderX = x;
		GameJoySDK.recorderY = y;
	}

	public void lockRecorderPosition()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("lockRecorderPosition", new object[0]);
		}
		else
		{
			GameJoySDK.Log("lockRecorderPosition get mQMiObj fail");
		}
	}

	public void unLockRecorderPosition()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("unLockRecorderPosition", new object[0]);
		}
		else
		{
			GameJoySDK.Log("unLockRecorderPosition get mQMiObj fail");
		}
	}

	public void showGameJoyRecorder()
	{
		if (this.enableOnStart)
		{
			this.StartQMi(GameJoySDK.recorderX, GameJoySDK.recorderY);
		}
	}

	public void dismissGameJoyRecorder()
	{
		this.StopQMi();
	}

	public bool IsShowed()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			return GameJoySDK.mQMiObj.CallStatic<bool>("isShowed", new object[0]);
		}
		GameJoySDK.Log("GameRecorder IsShowed get mQMiObj fail");
		return false;
	}

	public bool IsRecording()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		return GameJoySDK.mQMiObj != null && GameJoySDK.mQMiObj.CallStatic<bool>("isRecording", new object[0]);
	}

	public bool isRecordingMoments()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			return GameJoySDK.mQMiObj.CallStatic<bool>("isRecordingMoment", new object[0]);
		}
		GameJoySDK.Log("IsRecordingMoment get mQMiObj fail");
		return false;
	}

	public void InitRecordPlugin()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.Log("init recordplugin get context fail");
				return;
			}
			GameJoySDK.mQMiObj.CallStatic("initQMi", new object[]
			{
				GameJoySDK.playerActivityContext
			});
		}
		else
		{
			GameJoySDK.Log("InitRecordPlugin get mQMiObj fail");
		}
	}

	public static void CheckSDKFeature()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.Log("checkSDKFeature recordplugin get context fail");
				return;
			}
			GameJoySDK.CheckSDKFeatureCallback checkSDKFeatureCallback = new GameJoySDK.CheckSDKFeatureCallback();
			GameJoySDK.mQMiObj.CallStatic("checkSDKFeature", new object[]
			{
				GameJoySDK.playerActivityContext,
				checkSDKFeatureCallback
			});
		}
		else
		{
			GameJoySDK.Log("checkSDKFeature get mQMiObj fail");
		}
	}

	public void Update()
	{
	}

	public GameJoySDK.RECORER_STATUS GetRecorderStatus()
	{
		return GameJoySDK.mRecorderStatus;
	}

	public int BeginDraw()
	{
		if (GameJoySDK.mRecorderStatus != GameJoySDK.RECORER_STATUS.RS_STARTED)
		{
			return 0;
		}
		int result = 0;
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			result = GameJoySDK.mQMiObj.CallStatic<int>("beginDraw", new object[0]);
		}
		else
		{
			GameJoySDK.Log("BeginDraw mQMiObj = null");
		}
		return result;
	}

	public int EndDraw()
	{
		if (GameJoySDK.mRecorderStatus != GameJoySDK.RECORER_STATUS.RS_STARTED)
		{
			return 0;
		}
		int result = 0;
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			result = GameJoySDK.mQMiObj.CallStatic<int>("endDraw", new object[0]);
		}
		else
		{
			GameJoySDK.Log("EndDraw mQMiObj = null");
		}
		return result;
	}

	public int CallMethod(int nMethodID, int nParam1, int nParam2, int nParam3, int nParam4, int nParam5)
	{
		int result = 0;
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			result = GameJoySDK.mQMiObj.CallStatic<int>("callMethod", new object[]
			{
				nMethodID,
				nParam1,
				nParam2,
				nParam3,
				nParam4,
				nParam5
			});
		}
		else
		{
			GameJoySDK.Log("CallMethod mQMiObj = null");
		}
		return result;
	}

	public int IsNewSDKVersion()
	{
		if (GameJoySDK.mSDKVersion == -1 && GameJoySDK.mRecorderStatus != GameJoySDK.RECORER_STATUS.RS_STARTED)
		{
			return -1;
		}
		if (GameJoySDK.mSDKVersion >= 31)
		{
			return 1;
		}
		return 0;
	}

	public void IsStartRecordingSuccess(string success)
	{
		bool bResult = false;
		if (success != null && success.Equals("true"))
		{
			bResult = true;
		}
		GameJoy.OnStartMomentsRecording(bResult);
	}

	public void StopRecordingDuration(string duration)
	{
		long duration2 = -1L;
		if (duration != null)
		{
			duration2 = Convert.ToInt64(duration);
		}
		GameJoy.OnStopMomentsRecording(duration2);
	}

	public void OnCheckSDKFeature(string sdkFeature)
	{
		int sdkFeature2 = -1;
		if (sdkFeature != null)
		{
			sdkFeature2 = Convert.ToInt32(sdkFeature);
		}
		GameJoy.OnCheckSDKFeature(sdkFeature2);
	}

	private void Awake()
	{
		GameJoySDK.Log("GameRecorder Awake: SDK Version:2015-09-22-0002-Camera-Render");
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.Log("GameRecorder: Java class not accessible from C#.");
		}
		else
		{
			this.InitializeRenderCamera("Pre");
		}
	}

	private void InitializeRenderCamera(string type)
	{
		if (type.Equals("Pre") && GameObject.Find("GameJoy" + type + "Camera") == null)
		{
			GameObject gameObject = new GameObject();
			Camera camera = (Camera)gameObject.AddComponent("Camera");
			camera.name = "GameJoy" + type + "Camera";
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.cullingMask = 0;
			if (type.Equals("Pre"))
			{
				camera.depth = -3.40282347E+38f;
			}
			GameJoySDK.Log("InitializeRenderCamera start add GameJoySDK render");
			camera.gameObject.AddComponent("GameJoyAndroid" + type + "Render");
			GameJoySDK.Log("InitializeRenderCamera start SetActive");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private static AndroidJavaObject getActivityContext()
	{
		if (GameJoySDK.playerActivityContext == null)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (androidJavaClass == null)
			{
				GameJoySDK.Log("Get UnityPlayer Class failed");
				return null;
			}
			GameJoySDK.playerActivityContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.Log("get context failed");
				return null;
			}
		}
		return GameJoySDK.playerActivityContext;
	}

	private static AndroidJavaClass mQMiObjJavaClass()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = new AndroidJavaClass("com.tencent.qqgamemi.QmiSdkApi");
		}
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.Log("GameJoySDK: Unable to find GameJoySDK java class.");
		}
		return GameJoySDK.mQMiObj;
	}

	public void initQMi()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.Log("startqmi get context failed");
				return;
			}
		}
		else
		{
			GameJoySDK.Log("GameRecorder StartQMi mqmiobj = null");
		}
	}

	public void StartQMi(float x, float y)
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			}
			if (GameJoySDK.playerActivityContext != null)
			{
				string text = "Unity3D_" + Application.unityVersion;
				GameJoySDK.mQMiObj.CallStatic("showQMi", new object[]
				{
					GameJoySDK.playerActivityContext,
					text,
					x,
					y
				});
			}
		}
	}

	public void StopQMi()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.Log("stop qmi get context fail");
				return;
			}
			GameJoySDK.mQMiObj.CallStatic("stopQMi", new object[]
			{
				GameJoySDK.playerActivityContext
			});
		}
		else
		{
			GameJoySDK.Log("StopQMi get mQMiObj fail");
		}
	}

	public void HideQMi()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.Log("hide qmi get context fail");
				return;
			}
			GameJoySDK.mQMiObj.CallStatic("hideQMi", new object[]
			{
				GameJoySDK.playerActivityContext
			});
		}
		else
		{
			GameJoySDK.Log("HideQMi get mQMiObj fail");
		}
	}

	public void StartRecord()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("onStartRecordVideo", new object[0]);
			int num = GameJoySDK.mQMiObj.CallStatic<int>("getSRPpluginVersionCode", new object[0]);
			GameJoySDK.mSDKVersion = num;
			GameJoySDK.mRecorderStatus = GameJoySDK.RECORER_STATUS.RS_STARTED;
		}
		else
		{
			GameJoySDK.Log("StartRecord mQMiObj = null");
		}
	}

	public void StopRecord()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("onStopRecordVideo", new object[0]);
			GameJoySDK.mRecorderStatus = GameJoySDK.RECORER_STATUS.RS_STOPED;
		}
		else
		{
			GameJoySDK.Log("StopRecord mQMiObj = null");
		}
	}

	public void startMomentRecording()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			}
			if (GameJoySDK.playerActivityContext != null)
			{
				string text = "Unity3D_" + Application.unityVersion;
				GameJoySDK.mQMiObj.CallStatic("startMomentRecording", new object[]
				{
					GameJoySDK.playerActivityContext,
					text
				});
			}
		}
	}

	public void endMomentRecording()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("endMomentRecording", new object[0]);
		}
	}

	public void generateMomentVideo(List<TimeStamp> timeStampList, string defaultGameTag, Dictionary<string, string> extraInfo)
	{
		if (timeStampList != null && timeStampList.Count > 0)
		{
			this.showUploadShareDialog();
		}
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			string text = null;
			StringBuilder stringBuilder = new StringBuilder();
			if (extraInfo != null)
			{
				foreach (KeyValuePair<string, string> current in extraInfo)
				{
					string key = current.Key;
					string value = current.Value;
					stringBuilder.Append(string.Format("{0}'{1}^", key, value));
				}
				text = stringBuilder.ToString();
				if (text != null)
				{
					int length = text.LastIndexOf('^');
					text = text.Substring(0, length);
				}
			}
			long[] array = null;
			long[] array2 = null;
			if (timeStampList != null)
			{
				int count = timeStampList.Count;
				if (count != 0)
				{
					TimeStamp[] array3 = timeStampList.ToArray();
					array = new long[count];
					array2 = new long[count];
					for (int i = 0; i < count; i++)
					{
						array[i] = array3[i].startTime;
						array2[i] = array3[i].endTime;
					}
				}
			}
			GameJoySDK.mQMiObj.CallStatic("generateMomentVideo", new object[]
			{
				array,
				array2,
				defaultGameTag,
				text
			});
		}
		else
		{
			GameJoySDK.Log("OnApplicationFocus mQMiObj = null");
		}
	}

	public void setUploadShareDialogDefaultPosition(float x, float y)
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("setUploadShareDialogPosition", new object[]
			{
				x,
				y
			});
		}
	}

	public void setCurRecorderPosition(float x, float y)
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("setCurRecorderPosition", new object[]
			{
				x,
				y
			});
		}
	}

	public string getCurRecorderPosition()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			return GameJoySDK.mQMiObj.CallStatic<string>("getCurRecorderPosition", new object[0]);
		}
		GameJoySDK.Log("getCurRecorderPosition get mQMiObj fail");
		return null;
	}

	public void showUploadShareDialog()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("showUploadShareVideoDialog", new object[0]);
			GameJoySDK.Log("GameRecorder showUploadShareDialog end call ");
		}
	}

	public void showVideoListDialog()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			if (GameJoySDK.playerActivityContext == null)
			{
				GameJoySDK.playerActivityContext = GameJoySDK.getActivityContext();
			}
			if (GameJoySDK.playerActivityContext != null)
			{
				GameJoySDK.mQMiObj.CallStatic("showVideoListDialog", new object[]
				{
					GameJoySDK.playerActivityContext
				});
			}
		}
	}

	public void closeVideoListDialog()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("closeVideoListDialog", new object[0]);
		}
	}

	public void setVideoQuality(int flag)
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			GameJoySDK.mQMiObj.CallStatic("setVideoQuality", new object[]
			{
				flag
			});
		}
	}

	public static long getSystemCurrentTimeMillis()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			return GameJoySDK.mQMiObj.CallStatic<long>("getSystemCurrentTimeMillis", new object[0]);
		}
		return 0L;
	}

	private void OnApplicationQuit()
	{
		this.StopRecord();
		this.StopQMi();
	}

	public void GetGameEngineType()
	{
		if (GameJoySDK.mQMiObj == null)
		{
			GameJoySDK.mQMiObj = GameJoySDK.mQMiObjJavaClass();
		}
		if (GameJoySDK.mQMiObj != null)
		{
			string text = "Unity3D_" + Application.unityVersion;
			GameJoySDK.mQMiObj.CallStatic("setGameEngineType", new object[]
			{
				text
			});
		}
		else
		{
			GameJoySDK.Log("GetGameEngineType mQMiObj = null");
		}
	}

	public static void Log(string msglog)
	{
		Debug.Log(msglog);
	}
}
