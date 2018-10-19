using com.tencent.pandora.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
	public class Pandora
	{
		public static readonly Pandora Instance = new Pandora();

		private GameObject pandoraGameObject;

		private PandoraImpl pandoraImpl;

		private Logger logger;

		private GameObject pandoraParent;

		private Dictionary<string, GameObject> panelParentDic = new Dictionary<string, GameObject>();

		private int panelBaseDepth = 100;

		private string logPath = string.Empty;

		private string cachePath = string.Empty;

		private string imgPath = string.Empty;

		private string cookiePath = string.Empty;

		private string tempPath = string.Empty;

		private string streamingAssetsPath = string.Empty;

		private string[] fontResources = new string[]
		{
			"Pandora/FontPrefabs/usefont"
		};

		private string kSDKVersion = "YXZJ-Android-V15";

		private Action<Dictionary<string, string>> callbackForGame;

		private Func<GameObject, int, int, int> callbackForGameDjImage;

		private Func<int, int, string> callbackForGameImgPath;

		private Func<int, int, string> callbackForRankShowName;

		private Func<GameObject, string, int> callbackForSetGameImgByPath;

		private Func<string> callbackForFriendsList;

		private Func<string> callbackForSearchConfig;

		private Func<Dictionary<string, object>, Action<string>, string> callbackForCallGame;

		private UserData userData = new UserData();

		private bool isInited;

		public void Init()
		{
			Logger.DEBUG_LOGCAT(string.Empty);
			if (this.isInited)
			{
				return;
			}
			string temporaryCachePath = Application.temporaryCachePath;
			this.logPath = temporaryCachePath + "/Pandora/logs";
			this.cachePath = temporaryCachePath + "/Pandora/cache";
			this.imgPath = this.cachePath + "/imgs";
			this.cookiePath = this.cachePath + "/cookies";
			this.tempPath = temporaryCachePath + "/Pandora/temp";
			this.streamingAssetsPath = Application.streamingAssetsPath + "/Pandora";
			this.pandoraGameObject = new GameObject("Pandora GameObject");
			UnityEngine.Object.DontDestroyOnLoad(this.pandoraGameObject);
			this.pandoraImpl = this.pandoraGameObject.AddComponent<PandoraImpl>();
			this.logger = this.pandoraGameObject.AddComponent<Logger>();
			this.pandoraGameObject.AddComponent<MidasUtil>();
			this.pandoraImpl.Init();
			this.isInited = true;
		}

		public void SetPanelParent(GameObject thePanelParent, string panelParentName)
		{
			Logger.DEBUG(string.Empty);
			if (this.panelParentDic.ContainsKey(panelParentName))
			{
				this.panelParentDic[panelParentName] = thePanelParent;
			}
			else
			{
				this.panelParentDic.Add(panelParentName, thePanelParent);
			}
		}

		public void SetPandoraParent(GameObject thePandoraParent)
		{
			Logger.DEBUG(string.Empty);
			this.pandoraParent = thePandoraParent;
			if (this.pandoraGameObject != null)
			{
				this.pandoraGameObject.transform.parent = this.pandoraParent.transform;
			}
		}

		public void SetPanelBaseDepth(int depth)
		{
			Logger.DEBUG(depth.ToString());
			this.panelBaseDepth = depth;
		}

		public void SetCallback(Action<Dictionary<string, string>> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForGame = action;
		}

		public void SetCallback(Func<Dictionary<string, object>, Action<string>, string> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForCallGame = action;
		}

		public void SetGetDjImageCallback(Func<GameObject, int, int, int> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForGameDjImage = action;
		}

		public int GetGameDjImage(GameObject go, int djType, int djID)
		{
			Logger.DEBUG(string.Empty);
			int result = -1;
			if (this.callbackForGameDjImage != null)
			{
				result = this.callbackForGameDjImage(go, djType, djID);
			}
			return result;
		}

		public void SetQueryImgPathCallback(Func<int, int, string> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForGameImgPath = action;
		}

		public void SetGetGameImgByPathCallback(Func<GameObject, string, int> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForSetGameImgByPath = action;
		}

		public int ShowGameIcon(GameObject go, int rankClass, int rankGrade)
		{
			int result = -1;
			if (this.callbackForGameImgPath == null || this.callbackForSetGameImgByPath == null)
			{
				Logger.ERROR("the callback for setting game icon is null");
				return result;
			}
			string text = this.callbackForGameImgPath(rankClass, rankGrade);
			if (!string.IsNullOrEmpty(text))
			{
				result = this.callbackForSetGameImgByPath(go, text);
			}
			else
			{
				Logger.ERROR(string.Concat(new string[]
				{
					"the path for rankClass:",
					rankClass.ToString(),
					" rankGrade:",
					rankGrade.ToString(),
					" is null or empty"
				}));
			}
			return result;
		}

		public string GetGameImgPath(int rankClass, int rankGrade)
		{
			Logger.DEBUG(string.Empty);
			string result = string.Empty;
			if (this.callbackForGameImgPath != null)
			{
				result = this.callbackForGameImgPath(rankClass, rankGrade);
			}
			return result;
		}

		public int ShowGameImgByPath(GameObject go, string iconPath)
		{
			Logger.DEBUG(string.Empty);
			int result = -1;
			if (this.callbackForSetGameImgByPath != null && !string.IsNullOrEmpty(iconPath))
			{
				result = this.callbackForSetGameImgByPath(go, iconPath);
			}
			return result;
		}

		public void SetQueryFriendsListCallback(Func<string> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForFriendsList = action;
		}

		public string QueryFriendsList()
		{
			Logger.DEBUG(string.Empty);
			string result = string.Empty;
			if (this.callbackForFriendsList != null)
			{
				result = this.callbackForFriendsList();
			}
			return result;
		}

		public void SetQuerySearchConfigCallback(Func<string> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForSearchConfig = action;
		}

		public string QuerySearchConfig()
		{
			Logger.DEBUG(string.Empty);
			string result = string.Empty;
			if (this.callbackForSearchConfig != null)
			{
				result = this.callbackForSearchConfig();
			}
			return result;
		}

		public void SetQueryRankShowNameCallback(Func<int, int, string> action)
		{
			Logger.DEBUG(string.Empty);
			this.callbackForRankShowName = action;
		}

		public string QueryRankShowName(int rankClass, int rankShowGrade)
		{
			Logger.DEBUG(string.Empty);
			if (this.callbackForRankShowName != null)
			{
				return this.callbackForRankShowName(rankClass, rankShowGrade);
			}
			return string.Empty;
		}

		public void CallGame(string strCmdJson)
		{
			Logger.DEBUG(strCmdJson);
			try
			{
				Dictionary<string, object> dictionary = Json.Deserialize(strCmdJson) as Dictionary<string, object>;
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					if (current.Value == null)
					{
						Logger.DEBUG(strCmdJson);
						dictionary2[current.Key] = string.Empty;
					}
					else
					{
						dictionary2[current.Key] = (current.Value as string);
					}
				}
				if (this.callbackForGame != null)
				{
					this.callbackForGame(dictionary2);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.Message + ":" + ex.StackTrace);
			}
		}

		public void CallGame(Dictionary<string, string> cmdDict)
		{
			Logger.DEBUG(string.Empty);
			if (this.callbackForGame != null)
			{
				this.callbackForGame(cmdDict);
			}
		}

		public string CallGame(uint callId, LuaTable luatable)
		{
			string result2 = string.Empty;
			if (string.IsNullOrEmpty(luatable["type"].ToString()))
			{
				return result2;
			}
			Logger.DEBUG(string.Concat(new object[]
			{
				"callId=",
				callId.ToString(),
				" type=",
				luatable["type"]
			}));
			Action<string> arg = delegate(string result)
			{
				CSharpInterface.ExecCallback(callId, result);
			};
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (DictionaryEntry dictionaryEntry in luatable)
			{
				dictionary.Add(dictionaryEntry.Key.ToString(), dictionaryEntry.Value);
			}
			if (this.callbackForCallGame != null)
			{
				result2 = this.callbackForCallGame(dictionary, arg);
			}
			return result2;
		}

		public void SetUserData(Dictionary<string, string> dictPara)
		{
			string formatMsg = Json.Serialize(dictPara);
			Logger.DEBUG_LOGCAT(formatMsg);
			if (this.userData.IsRoleEmpty())
			{
				this.userData.Assgin(dictPara);
				if (this.pandoraImpl != null)
				{
					this.pandoraImpl.Bootup();
				}
			}
			else
			{
				this.userData.Refresh(dictPara);
			}
		}

		public void Logout()
		{
			Logger.DEBUG(string.Empty);
			this.userData.Clear();
			if (this.pandoraImpl != null)
			{
				this.pandoraImpl.LogOut();
			}
		}

		public bool GetTotalSwitch()
		{
			Logger.DEBUG(string.Empty);
			return this.pandoraImpl != null && this.pandoraImpl.GetTotalSwitch();
		}

		public void Do(Dictionary<string, string> cmdDict)
		{
			string formatMsg = Json.Serialize(cmdDict);
			Logger.DEBUG(formatMsg);
			if (this.pandoraImpl != null)
			{
				this.pandoraImpl.Do(cmdDict);
			}
		}

		public void CloseAllPanel()
		{
			Logger.DEBUG(string.Empty);
			if (this.pandoraImpl != null)
			{
				this.pandoraImpl.CloseAllPanel();
			}
		}

		public string GetLogPath()
		{
			return this.logPath;
		}

		public string GetCachePath()
		{
			return this.cachePath;
		}

		public string GetImgPath()
		{
			return this.imgPath;
		}

		public string GetCookiePath()
		{
			return this.cookiePath;
		}

		public string GetTempPath()
		{
			return this.tempPath;
		}

		public string GetStreamingAssetsPath()
		{
			return this.streamingAssetsPath;
		}

		public string[] GetFontResources()
		{
			return this.fontResources;
		}

		public UserData GetUserData()
		{
			return this.userData;
		}

		public string GetSDKVersion()
		{
			return this.kSDKVersion;
		}

		public GameObject GetGameObject()
		{
			return this.pandoraGameObject;
		}

		public GameObject GetPanelParent(string panelParentName)
		{
			Logger.DEBUG(string.Empty);
			if (this.panelParentDic.ContainsKey(panelParentName) && this.panelParentDic[panelParentName] != null)
			{
				return this.panelParentDic[panelParentName];
			}
			return null;
		}

		public LuaScriptMgr GetLuaScriptMgr()
		{
			return this.pandoraImpl.GetLuaScriptMgr();
		}

		public ResourceMgr GetResourceMgr()
		{
			return this.pandoraImpl.GetResourceMgr();
		}

		public NetLogic GetNetLogic()
		{
			return this.pandoraImpl.GetNetLogic();
		}

		public PandoraImpl GetPandoraImpl()
		{
			return this.pandoraImpl;
		}

		public Logger GetLogger()
		{
			return this.logger;
		}

		public int GetPanelBaseDepth()
		{
			return this.panelBaseDepth;
		}
	}
}
