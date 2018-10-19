using com.tencent.pandora.MiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.tencent.pandora
{
	public class CSharpInterface
	{
		public static bool isApplePlatform
		{
			get
			{
				return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer;
			}
		}

		public static Logger GetLogger()
		{
			return Pandora.Instance.GetLogger();
		}

		public static string GetPlatformDesc()
		{
			return Utils.GetPlatformDesc();
		}

		public static string GetSDKVersion()
		{
			return Pandora.Instance.GetSDKVersion();
		}

		public static bool WriteCookie(string fileName, string content)
		{
			return Utils.WriteCookie(fileName, content);
		}

		public static string ReadCookie(string fileName)
		{
			return Utils.ReadCookie(fileName);
		}

		public static bool IOSPay(string jsonParams)
		{
			Logger.DEBUG(jsonParams);
			return false;
		}

		public static bool AndroidPay(string jsonParams)
		{
			Logger.DEBUG(jsonParams);
			return MidasUtil.AndroidPay(jsonParams);
		}

		public static UserData GetUserData()
		{
			return Pandora.Instance.GetUserData();
		}

		public static void AsyncSetImage(string panelName, string url, Image image, uint callId)
		{
			Logger.DEBUG(string.Concat(new string[]
			{
				"panelName=",
				panelName,
				" url=",
				url,
				" callId=",
				callId.ToString()
			}));
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			pandoraImpl.ShowIMG(panelName, url, image, callId);
		}

		public static void ShowGameImg(int djType, int djID, GameObject go, uint callId)
		{
			int gameDjImage = Pandora.Instance.GetGameDjImage(go, djType, djID);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["djType"] = djType.ToString();
			dictionary["djID"] = djID.ToString();
			dictionary["RetCode"] = gameDjImage.ToString();
			string result = Json.Serialize(dictionary);
			CSharpInterface.ExecCallback(callId, result);
		}

		public static int ShowGameIcon(GameObject go, int rankClass, int rankGrade)
		{
			return Pandora.Instance.ShowGameIcon(go, rankClass, rankGrade);
		}

		public static string GetGameImgPath(int rankClass, int rankGrade)
		{
			return Pandora.Instance.GetGameImgPath(rankClass, rankGrade);
		}

		public static int ShowGameImgByPath(GameObject go, string iconPath)
		{
			return Pandora.Instance.ShowGameImgByPath(go, iconPath);
		}

		public static string QueryFriendsList()
		{
			return Pandora.Instance.QueryFriendsList();
		}

		public static string QuerySearchConfig()
		{
			return Pandora.Instance.QuerySearchConfig();
		}

		public static string QueryRankShowName(int rankClass, int rankShowGrade)
		{
			return Pandora.Instance.QueryRankShowName(rankClass, rankShowGrade);
		}

		public static void AsyncDownloadImage(string url)
		{
			Logger.DEBUG(url);
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			pandoraImpl.ShowIMG(string.Empty, url, null, 0u);
		}

		public static bool IsImageDownloaded(string url)
		{
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			return pandoraImpl.IsImgDownloaded(url);
		}

		public static bool GetTotalSwitch()
		{
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			return pandoraImpl.GetTotalSwitch();
		}

		public static bool GetFunctionSwitch(string functionName)
		{
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			return pandoraImpl.GetFunctionSwitch(functionName);
		}

		public static void CallGame(string strCmdJson)
		{
			Logger.DEBUG(strCmdJson);
			Pandora.Instance.CallGame(strCmdJson);
		}

		public static string CallGame(uint callId, LuaTable luatable)
		{
			Logger.DEBUG(string.Empty);
			return Pandora.Instance.CallGame(callId, luatable);
		}

		public static void StreamReport(string msg, int reportType, int returnCode)
		{
			Logger.DEBUG(string.Concat(new string[]
			{
				"msg=",
				msg,
				" reportType=",
				reportType.ToString(),
				" returnCode=",
				returnCode.ToString()
			}));
			Logger.REPORT(msg, reportType, returnCode);
		}

		public static void CallBroker(uint callId, string strReqJson, int cmdId)
		{
			Logger.DEBUG(string.Concat(new string[]
			{
				"callId=",
				callId.ToString(),
				" strReqJson=",
				strReqJson,
				" cmdId=",
				cmdId.ToString()
			}));
			NetLogic netLogic = Pandora.Instance.GetNetLogic();
			netLogic.CallBroker(callId, strReqJson, cmdId, 1, 10);
		}

		public static int AssembleFont(string panelName, string jsonFontTable)
		{
			ResourceMgr resourceMgr = Pandora.Instance.GetResourceMgr();
			return resourceMgr.AssembleFont(panelName, jsonFontTable);
		}

		public static void CreatePanel(uint callId, string panelName)
		{
			Logger.DEBUG("callId=" + callId.ToString() + " panelName=" + panelName);
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			Action<bool> onCreatePanel = delegate(bool status)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["RetCode"] = ((!status) ? "-1" : "0");
				dictionary["PanelName"] = panelName;
				string result = Json.Serialize(dictionary);
				CSharpInterface.ExecCallback(callId, result);
				if (!status)
				{
					Logger.ERROR(string.Empty);
					pandoraImpl.DestroyPanel(panelName);
				}
			};
			pandoraImpl.CreatePanel(panelName, onCreatePanel);
		}

		public static void DestroyPanel(string panelName)
		{
			Logger.DEBUG("panelName=" + panelName);
			PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
			pandoraImpl.DestroyPanel(panelName);
		}

		public static GameObject GetPanel(string panelName)
		{
			ResourceMgr resourceMgr = Pandora.Instance.GetResourceMgr();
			return resourceMgr.GetPanel(panelName);
		}

		public static GameObject GetPanelParent(string panelParentName)
		{
			return Pandora.Instance.GetPanelParent(panelParentName);
		}

		public static void AddClick(GameObject go, LuaFunction luaFunc)
		{
			go.GetComponent<Button>().onClick.RemoveAllListeners();
			go.GetComponent<Button>().onClick.AddListener(delegate
			{
				luaFunc.Call(new object[]
				{
					go
				});
			});
		}

		public static void AddDragEndEvent(GameObject go, LuaFunction luaFunc)
		{
			Logger.DEBUG("will add listener");
			EventTriggerListener.Get(go).onEndDrag = null;
			EventTriggerListener.Get(go).onEndDrag = delegate(GameObject o)
			{
				luaFunc.Call(new object[]
				{
					go
				});
			};
		}

		public static void AddUGUIOnClickDown(GameObject go, LuaFunction luaFunc)
		{
			EventTriggerListener.Get(go).onDown = null;
			EventTriggerListener.Get(go).onDown = delegate(GameObject o, Vector2 pressPos)
			{
				luaFunc.Call(new object[]
				{
					go,
					pressPos
				});
			};
		}

		public static void AddUGUIOnClickUp(GameObject go, LuaFunction luaFunc)
		{
			EventTriggerListener.Get(go).onUp = null;
			EventTriggerListener.Get(go).onUp = delegate(GameObject o, Vector2 pressPos)
			{
				luaFunc.Call(new object[]
				{
					go,
					pressPos
				});
			};
		}

		public static void ExecCallback(uint callId, string result)
		{
			try
			{
				Logger.DEBUG("callId=" + callId.ToString() + " result=" + result);
				LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				if (luaScriptMgr != null && pandoraImpl.GetIsLuaMgrInited())
				{
					luaScriptMgr.CallLuaFunction("Common.ExecCallback", new object[]
					{
						callId,
						result
					});
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void DoCmdFromGame(string jsonData)
		{
			try
			{
				Logger.DEBUG("jsonData=" + jsonData);
				LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				if (luaScriptMgr != null && pandoraImpl.GetIsLuaMgrInited())
				{
					luaScriptMgr.CallLuaFunction("Common.DoCmdFromGame", new object[]
					{
						jsonData
					});
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void NotifyIOSPayFinish(string jsonData)
		{
			try
			{
				Logger.DEBUG("jsonData=" + jsonData);
				LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				if (luaScriptMgr != null && pandoraImpl.GetIsLuaMgrInited())
				{
					luaScriptMgr.CallLuaFunction("Common.NotifyIOSPayFinish", new object[]
					{
						jsonData
					});
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void NotifyAndroidPayFinish(string jsonData)
		{
			try
			{
				Logger.DEBUG("jsonData=" + jsonData);
				LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				if (luaScriptMgr != null && pandoraImpl.GetIsLuaMgrInited())
				{
					luaScriptMgr.CallLuaFunction("Common.NotifyAndroidPayFinish", new object[]
					{
						jsonData
					});
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void NotifyPushData(string jsonData)
		{
			try
			{
				Logger.DEBUG("jsonData=" + jsonData);
				LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				if (luaScriptMgr != null && pandoraImpl.GetIsLuaMgrInited())
				{
					luaScriptMgr.CallLuaFunction("Common.NotifyPushData", new object[]
					{
						jsonData
					});
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void NotifyCloseAllPanel()
		{
			try
			{
				LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				if (luaScriptMgr != null && pandoraImpl.GetIsLuaMgrInited())
				{
					luaScriptMgr.CallLuaFunction("Common.NotifyCloseAllPanel", new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void UnloadUnusedAssets()
		{
			Logger.DEBUG(string.Empty);
			Resources.UnloadUnusedAssets();
		}

		public static void SetPosition(GameObject go, float x, float y, float z)
		{
			try
			{
				go.transform.localPosition = new Vector3(x, y, z);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void SetScale(GameObject go, float x, float y, float z)
		{
			try
			{
				go.transform.localScale = new Vector3(x, y, z);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void SetPosZ(GameObject go, float z)
		{
			try
			{
				go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, z);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static void SetTextString(GameObject go, string componentName, string sText)
		{
			try
			{
				Logger.DEBUG(string.Empty);
				if (componentName == "Text")
				{
					Text text = go.GetComponent("Text") as Text;
					text.text = sText;
				}
				else if (componentName == "InputField")
				{
					InputField inputField = go.GetComponent("InputField") as InputField;
					inputField.text = sText;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}
	}
}
