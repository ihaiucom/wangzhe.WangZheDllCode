using com.tencent.pandora.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
	public class PandoraImpl : MonoBehaviour
	{
		private enum BootupStatus
		{
			kInitial = -1,
			kReadingConfig,
			kReadConfigFailed,
			kReadConfigDone,
			kLocalLoading,
			kLoadLoadFailed,
			kLocalLoadDone,
			kDownloading,
			kDownloadDone
		}

		public class FileState
		{
			public string name = string.Empty;

			public bool isReady;
		}

		public class DownloadASTask
		{
			public string name = string.Empty;

			public string url = string.Empty;

			public int size = -1;

			public string md5 = string.Empty;

			public bool isDownloading;

			public int lastDownloadTime = -1;
		}

		public class ShowImgTask
		{
			public string panelName = string.Empty;

			public string url = string.Empty;

			public Object go;

			public uint callId;
		}

		private LuaScriptMgr luaMgr;

		private bool luaMgrInited;

		private ResourceMgr resMgr;

		private NetLogic netLogic;

		private int lastReBootTime = -1;

		private PandoraImpl.BootupStatus bootupStatus = PandoraImpl.BootupStatus.kInitial;

		private bool loadBaseAtlasSucc = true;

		private int ruleId;

		private bool totalSwitch;

		private bool isDebug;

		private bool isNetLog = true;

		private string brokerHost = string.Empty;

		private ushort brokerPort;

		private string brokerAltIp1 = string.Empty;

		private string brokerAltIp2 = string.Empty;

		private Dictionary<string, bool> functionSwitches = new Dictionary<string, bool>();

		private Dictionary<string, List<PandoraImpl.FileState>> dependencyInfos = new Dictionary<string, List<PandoraImpl.FileState>>();

		private List<string> dependencyAll = new List<string>();

		private bool isDependencyLoading;

		private int retryDownloadASInterval = 5;

		private int maxDownloadingTaskNum = 5;

		private List<PandoraImpl.DownloadASTask> pendingDownloadASTasks = new List<PandoraImpl.DownloadASTask>();

		private Dictionary<string, List<PandoraImpl.ShowImgTask>> showImgTasks = new Dictionary<string, List<PandoraImpl.ShowImgTask>>();

		private HashSet<string> executedLuaAssetBundles = new HashSet<string>();

		public void Init()
		{
			try
			{
				this.luaMgr = new LuaScriptMgr();
				this.resMgr = new ResourceMgr();
				this.netLogic = new NetLogic();
				this.netLogic.Init();
				Directory.CreateDirectory(Pandora.Instance.GetCachePath());
				Directory.CreateDirectory(Pandora.Instance.GetImgPath());
				Directory.CreateDirectory(Pandora.Instance.GetCookiePath());
				Directory.CreateDirectory(Pandora.Instance.GetLogPath());
				Directory.CreateDirectory(Pandora.Instance.GetTempPath());
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			try
			{
				if (this.netLogic != null)
				{
					this.netLogic.Drive();
				}
				if (this.luaMgr != null)
				{
					this.luaMgr.Update();
				}
				if (this.bootupStatus == PandoraImpl.BootupStatus.kReadConfigFailed)
				{
					int num = Utils.NowSeconds();
					if (this.lastReBootTime + 10 < num)
					{
						this.lastReBootTime = num;
						this.Bootup();
					}
				}
				else if (this.bootupStatus >= PandoraImpl.BootupStatus.kReadConfigDone && this.totalSwitch && this.loadBaseAtlasSucc)
				{
					this.HotUpdate();
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		private void LateUpdate()
		{
			try
			{
				this.luaMgr.LateUpate();
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_Message());
			}
		}

		private void FixedUpdate()
		{
			try
			{
				this.luaMgr.FixedUpdate();
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_Message());
			}
		}

		public LuaScriptMgr GetLuaScriptMgr()
		{
			return this.luaMgr;
		}

		public ResourceMgr GetResourceMgr()
		{
			return this.resMgr;
		}

		public NetLogic GetNetLogic()
		{
			return this.netLogic;
		}

		public bool GetTotalSwitch()
		{
			return this.totalSwitch;
		}

		public bool GetFunctionSwitch(string name)
		{
			return this.functionSwitches.ContainsKey(name) && this.functionSwitches.get_Item(name);
		}

		public bool GetIsDebug()
		{
			return this.isDebug;
		}

		public bool GetIsNetLog()
		{
			return this.isNetLog;
		}

		public bool GetIsLuaMgrInited()
		{
			return this.luaMgrInited;
		}

		public void Do(Dictionary<string, string> cmdDict)
		{
			try
			{
				Logger.DEBUG(string.Empty);
				if (cmdDict.ContainsKey("type") && cmdDict.ContainsKey("content") && cmdDict.get_Item("type").Equals("inMainSence"))
				{
					if (cmdDict.get_Item("content").Equals("0"))
					{
						if (this.netLogic != null)
						{
							this.netLogic.SetDownloadingPaused(true);
						}
						Logger.INFO("SetDownloadingPaused=true");
					}
					else
					{
						if (this.netLogic != null)
						{
							this.netLogic.SetDownloadingPaused(false);
						}
						Logger.INFO("SetDownloadingPaused=false");
					}
				}
				if (this.luaMgrInited)
				{
					Logger.DEBUG(string.Empty);
					string jsonData = Json.Serialize(cmdDict);
					CSharpInterface.DoCmdFromGame(jsonData);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_Message());
			}
		}

		public void CloseAllPanel()
		{
			Logger.DEBUG(string.Empty);
			if (this.luaMgrInited)
			{
				Logger.DEBUG(string.Empty);
				CSharpInterface.NotifyCloseAllPanel();
			}
		}

		private void OnApplicationQuit()
		{
			Logger.DEBUG(string.Empty);
			if (this.netLogic != null)
			{
				this.netLogic.Destroy();
			}
		}

		public void LogOut()
		{
			Logger.DEBUG(string.Empty);
			try
			{
				Utils.ResetCacheVersion();
				this.luaMgrInited = false;
				this.CloseAllPanel();
				this.netLogic.Logout();
				this.resMgr.Logout();
				this.luaMgr.Destroy();
				LuaScriptMgr.ResetStaticVars();
				this.luaMgr = new LuaScriptMgr();
				this.lastReBootTime = -1;
				this.bootupStatus = PandoraImpl.BootupStatus.kInitial;
				base.StopAllCoroutines();
				this.ruleId = 0;
				this.totalSwitch = false;
				this.isDebug = false;
				this.isNetLog = true;
				this.brokerHost = string.Empty;
				this.brokerPort = 0;
				this.brokerAltIp1 = string.Empty;
				this.brokerAltIp2 = string.Empty;
				this.functionSwitches = new Dictionary<string, bool>();
				this.dependencyInfos = new Dictionary<string, List<PandoraImpl.FileState>>();
				this.dependencyAll = new List<string>();
				this.isDependencyLoading = false;
				this.pendingDownloadASTasks = new List<PandoraImpl.DownloadASTask>();
				this.showImgTasks = new Dictionary<string, List<PandoraImpl.ShowImgTask>>();
				this.executedLuaAssetBundles = new HashSet<string>();
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_Message() + ":" + ex.get_StackTrace());
			}
		}

		public void Bootup()
		{
			Logger.DEBUG(string.Empty);
			Action<int, Dictionary<string, object>> action = delegate(int status, Dictionary<string, object> content)
			{
				Logger.DEBUG(string.Empty);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (status == 0 && Utils.ParseConfigData(content, ref dictionary))
				{
					Logger.DEBUG(string.Empty);
					this.bootupStatus = PandoraImpl.BootupStatus.kReadConfigDone;
					this.ruleId = (int)dictionary.get_Item("ruleId");
					this.totalSwitch = (bool)dictionary.get_Item("totalSwitch");
					if (this.totalSwitch)
					{
						this.functionSwitches = (dictionary.get_Item("functionSwitches") as Dictionary<string, bool>);
						this.isDebug = (bool)dictionary.get_Item("isDebug");
						this.isNetLog = (bool)dictionary.get_Item("isNetLog");
						this.brokerHost = (dictionary.get_Item("brokerHost") as string);
						this.brokerPort = (ushort)dictionary.get_Item("brokerPort");
						this.brokerAltIp1 = (dictionary.get_Item("brokerAltIp1") as string);
						this.brokerAltIp2 = (dictionary.get_Item("brokerAltIp2") as string);
						this.dependencyInfos = (dictionary.get_Item("dependencyInfos") as Dictionary<string, List<PandoraImpl.FileState>>);
						this.dependencyAll = (dictionary.get_Item("dependencyAll") as List<string>);
						this.pendingDownloadASTasks = (dictionary.get_Item("pendingDownloadASTasks") as List<PandoraImpl.DownloadASTask>);
						this.netLogic.SetBroker(this.brokerPort, this.brokerHost, this.brokerAltIp1, this.brokerAltIp2);
					}
				}
				else
				{
					Logger.DEBUG(string.Empty);
					this.bootupStatus = PandoraImpl.BootupStatus.kReadConfigFailed;
				}
			};
			this.bootupStatus = PandoraImpl.BootupStatus.kReadingConfig;
			this.netLogic.GetRemoteConfig(action);
		}

		private void HotUpdate()
		{
			if (this.bootupStatus == PandoraImpl.BootupStatus.kReadConfigDone)
			{
				Logger.DEBUG(string.Empty);
				this.resMgr.Init();
				this.resMgr.DeleteRedundantFiles(this.dependencyAll);
				List<PandoraImpl.DownloadASTask> list = new List<PandoraImpl.DownloadASTask>();
				using (List<PandoraImpl.DownloadASTask>.Enumerator enumerator = this.pendingDownloadASTasks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PandoraImpl.DownloadASTask current = enumerator.get_Current();
						if (!this.dependencyAll.Contains(current.name))
						{
							list.Add(current);
						}
					}
				}
				using (List<PandoraImpl.DownloadASTask>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						PandoraImpl.DownloadASTask current2 = enumerator2.get_Current();
						this.pendingDownloadASTasks.Remove(current2);
					}
				}
				list.Clear();
				this.bootupStatus = PandoraImpl.BootupStatus.kLocalLoading;
			}
			if (this.bootupStatus == PandoraImpl.BootupStatus.kLocalLoading)
			{
				Logger.DEBUG(string.Empty);
				while (this.dependencyAll.get_Count() > 0)
				{
					if (this.isDependencyLoading)
					{
						Logger.DEBUG("waiting local loading!");
						return;
					}
					string first = this.dependencyAll.get_Item(0);
					string text = string.Empty;
					PandoraImpl.DownloadASTask taskOfDependency = null;
					using (List<PandoraImpl.DownloadASTask>.Enumerator enumerator3 = this.pendingDownloadASTasks.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							PandoraImpl.DownloadASTask current3 = enumerator3.get_Current();
							if (first == current3.name)
							{
								taskOfDependency = current3;
								text = current3.md5;
								break;
							}
						}
					}
					Logger.DEBUG("first=" + first + " md5=" + text);
					if (this.resMgr.IsFileExistsInCache(first, text) || (!this.resMgr.IsFileExistsInCache(first, text) && taskOfDependency == null))
					{
						Logger.DEBUG("first=" + first + " md5=" + text);
						Action<bool> action = delegate(bool status)
						{
							this.isDependencyLoading = false;
							if (status)
							{
								Logger.DEBUG(first + " loaded");
								using (Dictionary<string, List<PandoraImpl.FileState>>.Enumerator enumerator6 = this.dependencyInfos.GetEnumerator())
								{
									while (enumerator6.MoveNext())
									{
										KeyValuePair<string, List<PandoraImpl.FileState>> current6 = enumerator6.get_Current();
										using (List<PandoraImpl.FileState>.Enumerator enumerator7 = current6.get_Value().GetEnumerator())
										{
											while (enumerator7.MoveNext())
											{
												PandoraImpl.FileState current7 = enumerator7.get_Current();
												if (current7.name == first)
												{
													current7.isReady = true;
												}
											}
										}
									}
								}
								this.TryDoLua(first);
								this.dependencyAll.RemoveAt(0);
								if (taskOfDependency != null)
								{
									this.pendingDownloadASTasks.Remove(taskOfDependency);
								}
							}
							else
							{
								Logger.ERROR(string.Empty);
								this.dependencyAll.RemoveAt(0);
								this.resMgr.DeleteFile(first);
							}
						};
						if (Utils.IsLuaAssetBundle(first))
						{
							Logger.DEBUG(string.Empty);
							this.isDependencyLoading = true;
							base.StartCoroutine(this.resMgr.LoadLuaAssetBundle(first, action));
						}
						else
						{
							Logger.DEBUG(string.Empty);
							action.Invoke(true);
						}
					}
					else
					{
						this.resMgr.DeleteFile(first);
						this.dependencyAll.RemoveAt(0);
					}
				}
				if (this.dependencyAll.get_Count() == 0)
				{
					Logger.DEBUG(string.Empty);
					this.bootupStatus = PandoraImpl.BootupStatus.kLocalLoadDone;
				}
			}
			if (this.bootupStatus == PandoraImpl.BootupStatus.kLocalLoadDone || this.bootupStatus == PandoraImpl.BootupStatus.kDownloading)
			{
				this.bootupStatus = PandoraImpl.BootupStatus.kDownloading;
				int num = 0;
				List<PandoraImpl.DownloadASTask> list2 = new List<PandoraImpl.DownloadASTask>(this.pendingDownloadASTasks);
				using (List<PandoraImpl.DownloadASTask>.Enumerator enumerator4 = list2.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						PandoraImpl.DownloadASTask current4 = enumerator4.get_Current();
						if (current4.isDownloading)
						{
							num++;
						}
					}
				}
				using (List<PandoraImpl.DownloadASTask>.Enumerator enumerator5 = list2.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						PandoraImpl.DownloadASTask current5 = enumerator5.get_Current();
						PandoraImpl.DownloadASTask task = current5;
						int num2 = Utils.NowSeconds() - task.lastDownloadTime;
						if (!task.isDownloading && num2 > this.retryDownloadASInterval)
						{
							Logger.DEBUG(string.Concat(new object[]
							{
								"task.url=",
								task.url,
								" task.size=",
								task.size,
								" task.md5=",
								task.md5
							}));
							Action<int, Dictionary<string, object>> action2 = delegate(int downloadRet, Dictionary<string, object> result)
							{
								Logger.DEBUG(string.Concat(new object[]
								{
									"task.url=",
									task.url,
									" task.size=",
									task.size,
									" task.md5=",
									task.md5
								}));
								if (downloadRet == 0)
								{
									Logger.DEBUG(string.Empty);
									this.resMgr.AddCacheFileMeta(task.name, task.size, task.md5);
									Action<bool> action3 = delegate(bool status)
									{
										task.isDownloading = false;
										this.pendingDownloadASTasks.Remove(task);
										if (status)
										{
											Logger.DEBUG(string.Empty);
											using (Dictionary<string, List<PandoraImpl.FileState>>.Enumerator enumerator6 = this.dependencyInfos.GetEnumerator())
											{
												while (enumerator6.MoveNext())
												{
													KeyValuePair<string, List<PandoraImpl.FileState>> current6 = enumerator6.get_Current();
													using (List<PandoraImpl.FileState>.Enumerator enumerator7 = current6.get_Value().GetEnumerator())
													{
														while (enumerator7.MoveNext())
														{
															PandoraImpl.FileState current7 = enumerator7.get_Current();
															if (current7.name == task.name)
															{
																current7.isReady = true;
															}
														}
													}
												}
											}
											this.TryDoLua(task.name);
											this.pendingDownloadASTasks.Remove(task);
										}
										else
										{
											Logger.ERROR(task.name + " load to mem failed!");
											this.resMgr.DeleteFile(task.name);
										}
									};
									if (Utils.IsLuaAssetBundle(task.name))
									{
										this.StartCoroutine(this.resMgr.LoadLuaAssetBundle(task.name, action3));
									}
									else
									{
										action3.Invoke(true);
									}
								}
								else
								{
									Logger.ERROR(string.Empty);
									task.isDownloading = false;
								}
							};
							task.isDownloading = true;
							num++;
							task.lastDownloadTime = Utils.NowSeconds();
							string destFile = Pandora.Instance.GetCachePath() + "/" + Path.GetFileName(task.url);
							this.netLogic.AddDownload(task.url, task.size, task.md5, destFile, 0, action2);
							if (num >= this.maxDownloadingTaskNum)
							{
								break;
							}
						}
					}
				}
				if (this.pendingDownloadASTasks.get_Count() == 0)
				{
					Logger.DEBUG(string.Empty);
					this.bootupStatus = PandoraImpl.BootupStatus.kDownloadDone;
				}
			}
		}

		private void TryDoLua(string newReadyAssetBundle)
		{
			try
			{
				Logger.DEBUG(newReadyAssetBundle);
				string text = Utils.GetPlatformDesc() + "_ulua_lua.assetbundle";
				if (this.resMgr.IsLuaAssetBundleLoaded(text) && !this.luaMgrInited)
				{
					LuaStatic.Load = new ReadLuaFile(this.resMgr.GetLuaBytes);
					this.luaMgr.DoFile = new LuaScriptMgr.FileExecutor(this.DoLoadedLua);
					this.luaMgr.Start();
					this.luaMgrInited = true;
				}
				if (this.luaMgrInited)
				{
					using (Dictionary<string, List<PandoraImpl.FileState>>.Enumerator enumerator = this.dependencyInfos.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, List<PandoraImpl.FileState>> current = enumerator.get_Current();
							bool flag = true;
							using (List<PandoraImpl.FileState>.Enumerator enumerator2 = current.get_Value().GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									PandoraImpl.FileState current2 = enumerator2.get_Current();
									if (!current2.isReady)
									{
										flag = false;
									}
								}
							}
							if (flag)
							{
								using (List<PandoraImpl.FileState>.Enumerator enumerator3 = current.get_Value().GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										PandoraImpl.FileState current3 = enumerator3.get_Current();
										if (Utils.IsLuaAssetBundle(current3.name) && !current3.name.Equals(text) && !this.executedLuaAssetBundles.Contains(current3.name))
										{
											Logger.DEBUG(current3.name);
											string name = Utils.ExtractLuaName(current3.name);
											this.luaMgr.DoFile(name);
											this.executedLuaAssetBundles.Add(current3.name);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_Message() + "\n" + ex.get_StackTrace());
			}
		}

		public object[] DoLoadedLua(string luaName)
		{
			Logger.DEBUG(luaName);
			string luaString = this.resMgr.GetLuaString(luaName);
			if (luaString != null)
			{
				return this.luaMgr.DoString(luaString);
			}
			return null;
		}

		public void CreatePanel(string panelName, Action<bool> onCreatePanel)
		{
			base.StartCoroutine(this.resMgr.CreatePanel(panelName, onCreatePanel));
		}

		public void DestroyPanel(string panelName)
		{
			this.resMgr.DestroyPanel(panelName);
		}

		[DebuggerHidden]
		private IEnumerator InternalShowIMG(string panelName, string url, Object go, uint callId)
		{
			PandoraImpl.<InternalShowIMG>c__Iterator33 <InternalShowIMG>c__Iterator = new PandoraImpl.<InternalShowIMG>c__Iterator33();
			<InternalShowIMG>c__Iterator.panelName = panelName;
			<InternalShowIMG>c__Iterator.url = url;
			<InternalShowIMG>c__Iterator.callId = callId;
			<InternalShowIMG>c__Iterator.go = go;
			<InternalShowIMG>c__Iterator.<$>panelName = panelName;
			<InternalShowIMG>c__Iterator.<$>url = url;
			<InternalShowIMG>c__Iterator.<$>callId = callId;
			<InternalShowIMG>c__Iterator.<$>go = go;
			<InternalShowIMG>c__Iterator.<>f__this = this;
			return <InternalShowIMG>c__Iterator;
		}

		public void ShowIMG(string panelName, string url, Object go, uint callId)
		{
			try
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
				string fileName = Path.GetFileName(url);
				string text = string.Concat(new string[]
				{
					Pandora.Instance.GetImgPath(),
					"/",
					url.GetHashCode().ToString(),
					"-",
					fileName
				});
				if (File.Exists(text))
				{
					Logger.DEBUG(string.Empty);
					base.StartCoroutine(this.InternalShowIMG(panelName, url, go, callId));
				}
				else
				{
					PandoraImpl.ShowImgTask showImgTask = new PandoraImpl.ShowImgTask();
					showImgTask.panelName = panelName;
					showImgTask.url = url;
					showImgTask.go = go;
					showImgTask.callId = callId;
					if (!this.showImgTasks.ContainsKey(url))
					{
						Logger.DEBUG(string.Empty);
						Action<int, Dictionary<string, object>> action = delegate(int status, Dictionary<string, object> content)
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
							List<PandoraImpl.ShowImgTask> list2 = this.showImgTasks.get_Item(url);
							this.showImgTasks.Remove(url);
							using (List<PandoraImpl.ShowImgTask>.Enumerator enumerator = list2.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									PandoraImpl.ShowImgTask current = enumerator.get_Current();
									this.StartCoroutine(this.InternalShowIMG(current.panelName, url, current.go, current.callId));
								}
							}
						};
						List<PandoraImpl.ShowImgTask> list = new List<PandoraImpl.ShowImgTask>();
						list.Add(showImgTask);
						this.showImgTasks.set_Item(url, list);
						this.netLogic.AddDownload(url, 0, string.Empty, text, 0, action);
					}
					else
					{
						this.showImgTasks.get_Item(url).Add(showImgTask);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public bool IsImgDownloaded(string url)
		{
			try
			{
				string fileName = Path.GetFileName(url);
				string text = string.Concat(new string[]
				{
					Pandora.Instance.GetImgPath(),
					"/",
					url.GetHashCode().ToString(),
					"-",
					fileName
				});
				if (File.Exists(text))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.WARN(ex.get_StackTrace());
			}
			return false;
		}
	}
}
