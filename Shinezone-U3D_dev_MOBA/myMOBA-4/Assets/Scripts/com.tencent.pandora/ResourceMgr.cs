using com.tencent.pandora.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace com.tencent.pandora
{
	public class ResourceMgr
	{
		private Dictionary<string, Font> fonts = new Dictionary<string, Font>();

		private HashSet<string> creatingPanels = new HashSet<string>();

		private Dictionary<string, AssetBundle> panelBundles = new Dictionary<string, AssetBundle>();

		private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();

		private HashSet<string> loadedLuaAssetBundles = new HashSet<string>();

		private Dictionary<string, TextAsset> loadedLuaFiles = new Dictionary<string, TextAsset>();

		private string cacheMetaFile = string.Empty;

		private Dictionary<string, object> dictCacheFileInfo = new Dictionary<string, object>();

		public void Init()
		{
			this.cacheMetaFile = Pandora.Instance.GetCachePath() + "/meta";
			this.LoadFonts();
			this.LoadCacheMeta();
		}

		private void LoadFonts()
		{
			Logger.DEBUG(string.Empty);
			string[] fontResources = Pandora.Instance.GetFontResources();
			for (int i = 0; i < fontResources.Length; i++)
			{
				GameObject gameObject = Resources.Load(fontResources[i]) as GameObject;
				if (gameObject != null)
				{
					Font font = gameObject.GetComponent<Text>().font;
					if (font != null)
					{
						Logger.DEBUG(font.name + " loaded!");
						this.fonts[font.name.ToLower()] = font;
					}
					else
					{
						Logger.ERROR("Font of " + fontResources[i] + " is null");
					}
				}
				else
				{
					Logger.ERROR(fontResources[i] + " load error");
				}
			}
		}

		private void LoadCacheMeta()
		{
			try
			{
				if (!File.Exists(this.cacheMetaFile))
				{
					FileStream fileStream = new FileStream(this.cacheMetaFile, FileMode.Create);
					fileStream.Close();
				}
				string text = File.ReadAllText(this.cacheMetaFile);
				if (text != string.Empty)
				{
					this.dictCacheFileInfo = (Json.Deserialize(text) as Dictionary<string, object>);
				}
				Logger.DEBUG(text);
				if (this.dictCacheFileInfo == null)
				{
					this.dictCacheFileInfo = new Dictionary<string, object>();
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public void AddCacheFileMeta(string fileName, int fileSize, string fileMD5)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["name"] = fileName;
				dictionary["size"] = fileSize;
				dictionary["md5"] = fileMD5;
				this.dictCacheFileInfo[fileName] = dictionary;
				string text = Json.Serialize(this.dictCacheFileInfo);
				Logger.DEBUG(text);
				File.WriteAllText(this.cacheMetaFile, text);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public void DeleteFile(string fileName)
		{
			try
			{
				this.dictCacheFileInfo.Remove(fileName);
				string contents = Json.Serialize(this.dictCacheFileInfo);
				File.WriteAllText(this.cacheMetaFile, contents);
				string path = Pandora.Instance.GetCachePath() + "/" + fileName;
				File.Delete(path);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public void DeleteRedundantFiles(List<string> dependencyAll)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, object> current in this.dictCacheFileInfo)
				{
					if (!dependencyAll.Contains(current.Key))
					{
						list.Add(current.Key);
					}
				}
				foreach (string current2 in list)
				{
					Logger.DEBUG(current2);
					this.DeleteFile(current2);
				}
				string cookiePath = Pandora.Instance.GetCookiePath();
				string imgPath = Pandora.Instance.GetImgPath();
				string[] files = Directory.GetFiles(cookiePath);
				string[] files2 = Directory.GetFiles(imgPath);
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
					double totalDays = DateTime.UtcNow.Subtract(lastWriteTimeUtc).TotalDays;
					if (totalDays > 7.0)
					{
						Logger.DEBUG(text);
						File.Delete(text);
					}
				}
				string[] array2 = files2;
				for (int j = 0; j < array2.Length; j++)
				{
					string text2 = array2[j];
					DateTime lastWriteTimeUtc2 = File.GetLastWriteTimeUtc(text2);
					double totalDays2 = DateTime.UtcNow.Subtract(lastWriteTimeUtc2).TotalDays;
					if (totalDays2 > 7.0)
					{
						Logger.DEBUG(text2);
						File.Delete(text2);
					}
				}
				string tempPath = Pandora.Instance.GetTempPath();
				string[] files3 = Directory.GetFiles(tempPath);
				string[] array3 = files3;
				for (int k = 0; k < array3.Length; k++)
				{
					string text3 = array3[k];
					DateTime lastWriteTimeUtc3 = File.GetLastWriteTimeUtc(text3);
					double totalDays3 = DateTime.UtcNow.Subtract(lastWriteTimeUtc3).TotalDays;
					if (totalDays3 > 3.0)
					{
						Logger.DEBUG(text3);
						File.Delete(text3);
					}
				}
				string logPath = Pandora.Instance.GetLogPath();
				string[] files4 = Directory.GetFiles(logPath);
				string[] array4 = files4;
				for (int l = 0; l < array4.Length; l++)
				{
					string text4 = array4[l];
					DateTime lastWriteTimeUtc4 = File.GetLastWriteTimeUtc(text4);
					double totalDays4 = DateTime.UtcNow.Subtract(lastWriteTimeUtc4).TotalDays;
					if (totalDays4 > 3.0)
					{
						Logger.DEBUG(text4);
						File.Delete(text4);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		public bool IsFileExistsInCache(string fileName, string fileMD5)
		{
			if (this.dictCacheFileInfo.ContainsKey(fileName))
			{
				Dictionary<string, object> dictionary = this.dictCacheFileInfo[fileName] as Dictionary<string, object>;
				if (fileMD5.Length == 0 || dictionary["md5"] as string == fileMD5)
				{
					return true;
				}
			}
			return false;
		}

		public Font GetFont(string fontName)
		{
			if (this.fonts.ContainsKey(fontName))
			{
				return this.fonts[fontName];
			}
			return null;
		}

		public IEnumerator LoadLuaAssetBundle(string assetBundleName, Action<bool> callback)
		{
			Logger.DEBUG(assetBundleName);
			if (loadedLuaAssetBundles.Contains(assetBundleName))
			{
				callback(true);
				yield break;
			}
				
			string wwwPath = GetPathForWWW(assetBundleName);
			Logger.DEBUG(wwwPath);
			WWW www =  new WWW(wwwPath);
			yield return www;

			if (!string.IsNullOrEmpty(www.error))
			{
				Logger.ERROR(www.error);
				callback(false);
				yield break;
			}
				
			loadedLuaAssetBundles.Add(assetBundleName);
			var bundle = www.assetBundle;
			var files = bundle.LoadAll();
			int i = 0;
			while (i < files.Length)
			{
				var textAsset = files[i] as TextAsset;
				string key = textAsset.name.Replace(".lua", string.Empty);
				Logger.DEBUG(key);
				loadedLuaFiles[key] = textAsset;
				i++;
			}

			bundle.Unload(false);
			callback(true);
		}

		public IEnumerator CreatePanel(string panelName, Action<bool> onCreatePanel)
		{
			Logger.DEBUG(panelName);
			if (creatingPanels.Contains(panelName))
			{
				onCreatePanel(false);
				yield break;
			}
			
			if (panels.ContainsKey(panelName))
			{
				var _panel___0 = panels[panelName];
				if (_panel___0 != null)
				{
					Logger.DEBUG(string.Empty);
					onCreatePanel(true);
					yield break;
				}
				
				panels.Remove(panelName);
			}
			creatingPanels.Add(panelName);
			var _assetBundleName___1 = Utils.GetBundleName(panelName);
			var _wwwPath___2 = GetPathForWWW(_assetBundleName___1);
			Logger.DEBUG(_wwwPath___2);
			WWW _www___3 = new WWW(_wwwPath___2);
			yield return _www___3;
			
			Logger.DEBUG(_wwwPath___2);
			if (!string.IsNullOrEmpty(_www___3.error))
			{
				Logger.ERROR(_www___3.error);
				Logger.ERROR(panelName);
				creatingPanels.Remove(panelName);
				onCreatePanel(false);
				yield break;
			}
			
			var _bundle___4 = _www___3.assetBundle;
			if (_bundle___4 == null)
			{
				Logger.ERROR(panelName);
				creatingPanels.Remove(panelName);
				onCreatePanel(false);
				yield break;
			}
			panelBundles[panelName] = _bundle___4;
			var _panelPrefab___5 = _bundle___4.Load(panelName, typeof(GameObject)) as GameObject;
			yield return new WaitForEndOfFrame();
			
			Logger.DEBUG(panelName);
			if (_panelPrefab___5 == null)
			{
				Logger.ERROR(panelName);
				creatingPanels.Remove(panelName);
				onCreatePanel(false);
				yield break;
			}
			var _panel___6 = Object.Instantiate(_panelPrefab___5) as GameObject;
			if (_panel___6 == null)
			{
				Logger.ERROR(panelName);
				creatingPanels.Remove(panelName);
				onCreatePanel(false);
				yield break;
			}
			_panel___6.name = panelName;
			_panel___6.transform.localScale = Vector3.one;
			_panel___6.transform.localPosition = Vector3.zero;
			_panel___6.AddComponent<ImageMgr>();
			panels[panelName] = _panel___6;
			yield return new WaitForEndOfFrame();
			
			Logger.DEBUG(panelName);
			creatingPanels.Remove(panelName);
			onCreatePanel(true);
		}

		public GameObject GetPanel(string panelName)
		{
			if (this.panels.ContainsKey(panelName))
			{
				return this.panels[panelName];
			}
			return null;
		}

		public List<GameObject> GetAllPanel()
		{
			List<GameObject> list = new List<GameObject>();
			foreach (KeyValuePair<string, GameObject> current in this.panels)
			{
				list.Add(current.Value);
			}
			return list;
		}

		public void DestroyPanel(string panelName)
		{
			Logger.DEBUG("panelName=" + panelName);
			if (this.panels.ContainsKey(panelName))
			{
				GameObject gameObject = this.panels[panelName];
				this.panels.Remove(panelName);
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			if (this.panelBundles.ContainsKey(panelName))
			{
				AssetBundle assetBundle = this.panelBundles[panelName];
				this.panelBundles.Remove(panelName);
				if (assetBundle != null)
				{
					assetBundle.Unload(true);
				}
			}
		}

		public int AssembleFont(string panelName, string jsonFontTable)
		{
			Logger.DEBUG("panelName=" + panelName + " jsonFontTable=" + jsonFontTable);
			if (!this.panels.ContainsKey(panelName))
			{
				Logger.ERROR("panel " + panelName + " not found!");
				return -1;
			}
			string text = "GameFont";
			GameObject gameObject = this.panels[panelName];
			if (!this.fonts.ContainsKey(text.ToLower()))
			{
				Logger.ERROR("font " + text + " not found!");
				return -1;
			}
			Font font = this.fonts[text.ToLower()];
			Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>(true);
			Text[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Text text2 = array[i];
				Logger.DEBUG("Assemble " + text + " to Text");
				text2.font = font;
			}
			return 0;
		}

		public bool IsLuaAssetBundleLoaded(string fileName)
		{
			return this.loadedLuaAssetBundles.Contains(fileName);
		}

		public byte[] GetLuaBytes(string luaName)
		{
			Logger.DEBUG(luaName);
			string key = luaName.Replace(".lua", string.Empty);
			if (this.loadedLuaFiles.ContainsKey(key))
			{
				return this.loadedLuaFiles[key].bytes;
			}
			return null;
		}

		public string GetLuaString(string luaName)
		{
			Logger.DEBUG(luaName);
			string key = luaName.Replace(".lua", string.Empty);
			if (this.loadedLuaFiles.ContainsKey(key))
			{
				return this.loadedLuaFiles[key].text;
			}
			return null;
		}

		public void Logout()
		{
			Logger.DEBUG(string.Empty);
			this.fonts.Clear();
			foreach (KeyValuePair<string, GameObject> current in this.panels)
			{
				Logger.DEBUG(current.Key);
				GameObject value = current.Value;
				if (value != null)
				{
					UnityEngine.Object.Destroy(value);
				}
			}
			this.panels.Clear();
			foreach (KeyValuePair<string, AssetBundle> current2 in this.panelBundles)
			{
				Logger.DEBUG(current2.Key);
				AssetBundle value2 = current2.Value;
				if (value2 != null)
				{
					value2.Unload(true);
				}
			}
			this.panelBundles.Clear();
			this.loadedLuaAssetBundles.Clear();
			this.loadedLuaFiles.Clear();
			this.dictCacheFileInfo.Clear();
		}

		private string GetPathForWWW(string assetBundleName)
		{
			string result;
			try
			{
				if (this.dictCacheFileInfo.ContainsKey(assetBundleName))
				{
					string str = Pandora.Instance.GetCachePath() + "/" + assetBundleName;
					result = "file://" + str;
				}
				else
				{
					string text = Pandora.Instance.GetStreamingAssetsPath() + "/" + assetBundleName;
					result = text;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
				result = string.Empty;
			}
			return result;
		}
	}
}
