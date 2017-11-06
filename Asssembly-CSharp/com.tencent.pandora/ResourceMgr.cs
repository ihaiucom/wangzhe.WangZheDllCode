using com.tencent.pandora.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
					Font font = gameObject.GetComponent<Text>().get_font();
					if (font != null)
					{
						Logger.DEBUG(font.name + " loaded!");
						this.fonts.set_Item(font.name.ToLower(), font);
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
					FileStream fileStream = new FileStream(this.cacheMetaFile, 2);
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
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public void AddCacheFileMeta(string fileName, int fileSize, string fileMD5)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.set_Item("name", fileName);
				dictionary.set_Item("size", fileSize);
				dictionary.set_Item("md5", fileMD5);
				this.dictCacheFileInfo.set_Item(fileName, dictionary);
				string text = Json.Serialize(this.dictCacheFileInfo);
				Logger.DEBUG(text);
				File.WriteAllText(this.cacheMetaFile, text);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public void DeleteFile(string fileName)
		{
			try
			{
				this.dictCacheFileInfo.Remove(fileName);
				string text = Json.Serialize(this.dictCacheFileInfo);
				File.WriteAllText(this.cacheMetaFile, text);
				string text2 = Pandora.Instance.GetCachePath() + "/" + fileName;
				File.Delete(text2);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public void DeleteRedundantFiles(List<string> dependencyAll)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				List<string> list = new List<string>();
				using (Dictionary<string, object>.Enumerator enumerator = this.dictCacheFileInfo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, object> current = enumerator.get_Current();
						if (!dependencyAll.Contains(current.get_Key()))
						{
							list.Add(current.get_Key());
						}
					}
				}
				using (List<string>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.get_Current();
						Logger.DEBUG(current2);
						this.DeleteFile(current2);
					}
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
					double totalDays = DateTime.get_UtcNow().Subtract(lastWriteTimeUtc).get_TotalDays();
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
					double totalDays2 = DateTime.get_UtcNow().Subtract(lastWriteTimeUtc2).get_TotalDays();
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
					double totalDays3 = DateTime.get_UtcNow().Subtract(lastWriteTimeUtc3).get_TotalDays();
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
					double totalDays4 = DateTime.get_UtcNow().Subtract(lastWriteTimeUtc4).get_TotalDays();
					if (totalDays4 > 3.0)
					{
						Logger.DEBUG(text4);
						File.Delete(text4);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public bool IsFileExistsInCache(string fileName, string fileMD5)
		{
			if (this.dictCacheFileInfo.ContainsKey(fileName))
			{
				Dictionary<string, object> dictionary = this.dictCacheFileInfo.get_Item(fileName) as Dictionary<string, object>;
				if (fileMD5.get_Length() == 0 || dictionary.get_Item("md5") as string == fileMD5)
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
				return this.fonts.get_Item(fontName);
			}
			return null;
		}

		[DebuggerHidden]
		public IEnumerator LoadLuaAssetBundle(string assetBundleName, Action<bool> callback)
		{
			ResourceMgr.<LoadLuaAssetBundle>c__Iterator34 <LoadLuaAssetBundle>c__Iterator = new ResourceMgr.<LoadLuaAssetBundle>c__Iterator34();
			<LoadLuaAssetBundle>c__Iterator.assetBundleName = assetBundleName;
			<LoadLuaAssetBundle>c__Iterator.callback = callback;
			<LoadLuaAssetBundle>c__Iterator.<$>assetBundleName = assetBundleName;
			<LoadLuaAssetBundle>c__Iterator.<$>callback = callback;
			<LoadLuaAssetBundle>c__Iterator.<>f__this = this;
			return <LoadLuaAssetBundle>c__Iterator;
		}

		[DebuggerHidden]
		public IEnumerator CreatePanel(string panelName, Action<bool> onCreatePanel)
		{
			ResourceMgr.<CreatePanel>c__Iterator35 <CreatePanel>c__Iterator = new ResourceMgr.<CreatePanel>c__Iterator35();
			<CreatePanel>c__Iterator.panelName = panelName;
			<CreatePanel>c__Iterator.onCreatePanel = onCreatePanel;
			<CreatePanel>c__Iterator.<$>panelName = panelName;
			<CreatePanel>c__Iterator.<$>onCreatePanel = onCreatePanel;
			<CreatePanel>c__Iterator.<>f__this = this;
			return <CreatePanel>c__Iterator;
		}

		public GameObject GetPanel(string panelName)
		{
			if (this.panels.ContainsKey(panelName))
			{
				return this.panels.get_Item(panelName);
			}
			return null;
		}

		public List<GameObject> GetAllPanel()
		{
			List<GameObject> list = new List<GameObject>();
			using (Dictionary<string, GameObject>.Enumerator enumerator = this.panels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, GameObject> current = enumerator.get_Current();
					list.Add(current.get_Value());
				}
			}
			return list;
		}

		public void DestroyPanel(string panelName)
		{
			Logger.DEBUG("panelName=" + panelName);
			if (this.panels.ContainsKey(panelName))
			{
				GameObject gameObject = this.panels.get_Item(panelName);
				this.panels.Remove(panelName);
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
			if (this.panelBundles.ContainsKey(panelName))
			{
				AssetBundle assetBundle = this.panelBundles.get_Item(panelName);
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
			GameObject gameObject = this.panels.get_Item(panelName);
			if (!this.fonts.ContainsKey(text.ToLower()))
			{
				Logger.ERROR("font " + text + " not found!");
				return -1;
			}
			Font font = this.fonts.get_Item(text.ToLower());
			Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>(true);
			Text[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Text text2 = array[i];
				Logger.DEBUG("Assemble " + text + " to Text");
				text2.set_font(font);
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
			string text = luaName.Replace(".lua", string.Empty);
			if (this.loadedLuaFiles.ContainsKey(text))
			{
				return this.loadedLuaFiles.get_Item(text).bytes;
			}
			return null;
		}

		public string GetLuaString(string luaName)
		{
			Logger.DEBUG(luaName);
			string text = luaName.Replace(".lua", string.Empty);
			if (this.loadedLuaFiles.ContainsKey(text))
			{
				return this.loadedLuaFiles.get_Item(text).text;
			}
			return null;
		}

		public void Logout()
		{
			Logger.DEBUG(string.Empty);
			this.fonts.Clear();
			using (Dictionary<string, GameObject>.Enumerator enumerator = this.panels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, GameObject> current = enumerator.get_Current();
					Logger.DEBUG(current.get_Key());
					GameObject value = current.get_Value();
					if (value != null)
					{
						Object.Destroy(value);
					}
				}
			}
			this.panels.Clear();
			using (Dictionary<string, AssetBundle>.Enumerator enumerator2 = this.panelBundles.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<string, AssetBundle> current2 = enumerator2.get_Current();
					Logger.DEBUG(current2.get_Key());
					AssetBundle value2 = current2.get_Value();
					if (value2 != null)
					{
						value2.Unload(true);
					}
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
					string text = Pandora.Instance.GetCachePath() + "/" + assetBundleName;
					result = "file://" + text;
				}
				else
				{
					string text2 = Pandora.Instance.GetStreamingAssetsPath() + "/" + assetBundleName;
					result = text2;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
				result = string.Empty;
			}
			return result;
		}
	}
}
