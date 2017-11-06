using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class TGPSDKSys : MonoSingleton<TGPSDKSys>
	{
		private bool m_bInstall;

		private uint m_deskID;

		private uint m_deskCrtSeq;

		private uint m_relatEntity;

		private string m_gameid = string.Empty;

		private GameEventDef m_event;

		private string m_OpendID = string.Empty;

		private AndroidJavaObject messageManager;

		private static AndroidJavaObject sContext;

		protected override void Init()
		{
			base.Init();
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY, new Action(this.InitTGPConfig));
		}

		public void Set(uint deskID, uint seq, uint entity)
		{
			this.m_deskCrtSeq = seq;
			this.m_deskID = deskID;
			this.m_relatEntity = entity;
			this.m_gameid = string.Concat(new object[]
			{
				deskID,
				"|",
				this.m_deskCrtSeq,
				"|",
				this.m_relatEntity
			});
		}

		private void InitTGPConfig()
		{
			if (GameDataMgr.svr2CltCfgDict != null && GameDataMgr.svr2CltCfgDict.ContainsKey(33u))
			{
				this.InitSys();
			}
		}

		public void SetOpenID(string openID)
		{
			this.m_OpendID = openID;
			Debug.Log("TGP openid " + this.m_OpendID);
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			if (this.m_bInstall)
			{
				if (pauseStatus)
				{
					this.EnablePhone(false);
				}
				else
				{
					this.EnablePhone(Singleton<BattleLogic>.GetInstance().isRuning);
				}
			}
		}

		public void OnApplicationQuit()
		{
			if (this.m_bInstall)
			{
				this.EnablePhone(false);
			}
		}

		public void InitSys()
		{
			if (this.messageManager == null)
			{
				this.messageManager = this.getRemoteMsgMgrInstance();
			}
			this.m_bInstall = this.isPluginInstalled();
			if (!this.m_bInstall)
			{
				return;
			}
			Debug.Log("TGP initsys");
			this.setDebugLogFlag();
		}

		public void EnablePhone(bool bEnbale)
		{
			try
			{
				if (this.m_bInstall && this.messageManager != null && !Singleton<WatchController>.GetInstance().IsWatching)
				{
					this.messageManager.Call<bool>("notifyGameState", new object[]
					{
						this.m_gameid,
						this.m_OpendID,
						bEnbale
					});
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}

		public void GameStart(string allopenid)
		{
			try
			{
				if (this.m_bInstall && this.messageManager != null && !Singleton<WatchController>.GetInstance().IsWatching)
				{
					this.messageManager.Call<bool>("notifyGameStart", new object[]
					{
						this.m_gameid,
						allopenid
					});
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}

		public void SendGameEvent2(KillDetailInfoType killEvent)
		{
			try
			{
				if (this.m_bInstall && this.messageManager != null && !Singleton<WatchController>.GetInstance().IsWatching)
				{
					this.messageManager.Call<bool>("notifyGameEvent", new object[]
					{
						this.m_gameid,
						this.m_OpendID,
						killEvent.ToString()
					});
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}

		private void pollGameState(string gameId)
		{
			Debug.Log("message from android:" + gameId);
			if (this.m_bInstall && this.messageManager != null)
			{
				this.messageManager.Call<bool>("broadcastHeartbeat", new object[]
				{
					this.m_gameid,
					this.m_OpendID,
					Singleton<BattleLogic>.GetInstance().isRuning
				});
			}
		}

		private void setDebugLogFlag()
		{
			bool flag = false;
			if (this.m_bInstall && this.messageManager != null)
			{
				this.messageManager.Call("setDebugLogFlag", new object[]
				{
					flag
				});
			}
		}

		public bool isPluginInstalled()
		{
			bool result = false;
			try
			{
				if (this.messageManager != null)
				{
					result = this.messageManager.Call<bool>("isPluginInstalled", new object[0]);
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
			return result;
		}

		private AndroidJavaObject getRemoteMsgMgrInstance()
		{
			AndroidJavaClass pluginClass = this.getPluginClass("com.tencent.tgp.wzry.gameplugin.RemoteMsgManager");
			return pluginClass.CallStatic<AndroidJavaObject>("getInstance", new object[]
			{
				TGPSDKSys.getAndroidContext()
			});
		}

		public static AndroidJavaObject getAndroidContext()
		{
			if (TGPSDKSys.sContext == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				TGPSDKSys.sContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			return TGPSDKSys.sContext;
		}

		public AndroidJavaClass getPluginClass(string className)
		{
			return new AndroidJavaClass(className);
		}

		public AndroidJavaObject getClassInstance(string className, object[] args)
		{
			return new AndroidJavaObject(className, args);
		}
	}
}
