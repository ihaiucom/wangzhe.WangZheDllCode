using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CResourceManager : Singleton<CResourceManager>
{
	public delegate void OnResourceLoaded(CResource resource);

	private CResourcePackerInfoSet m_resourcePackerInfoSet;

	private DictionaryView<int, CResource> m_cachedResourceMap;

	private bool m_clearUnusedAssets;

	private int m_clearUnusedAssetsExecuteFrame;

	private static int s_frameCounter;

	public static bool isBattleState;

	public override void Init()
	{
		this.m_resourcePackerInfoSet = null;
		this.m_cachedResourceMap = new DictionaryView<int, CResource>();
	}

	public void CustomUpdate()
	{
		CResourceManager.s_frameCounter++;
		if (this.m_clearUnusedAssets && this.m_clearUnusedAssetsExecuteFrame == CResourceManager.s_frameCounter)
		{
			this.ExecuteUnloadUnusedAssets();
			this.m_clearUnusedAssets = false;
		}
	}

	public DictionaryView<int, CResource> GetCachedResourceMap()
	{
		return this.m_cachedResourceMap;
	}

	public void LoadResourcePackerInfoSet()
	{
		if (this.m_resourcePackerInfoSet != null)
		{
			this.m_resourcePackerInfoSet.Dispose();
			this.m_resourcePackerInfoSet = null;
		}
		string filePath = CFileManager.CombinePath(CFileManager.GetIFSExtractPath(), CResourcePackerInfoSet.s_resourcePackerInfoSetFileName);
		if (CFileManager.IsFileExist(filePath))
		{
			byte[] data = CFileManager.ReadFile(filePath);
			int num = 0;
			this.m_resourcePackerInfoSet = new CResourcePackerInfoSet();
			this.m_resourcePackerInfoSet.Read(data, ref num);
			CVersion.SetUsedResourceVersion(this.m_resourcePackerInfoSet.m_version);
			this.m_resourcePackerInfoSet.CreateResourceMap();
		}
	}

	public IEnumerator LoadResidentAssetBundles()
	{
		if (m_resourcePackerInfoSet != null)
		{
			int  i = 0;
			while (i < m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count)
			{
				var resInfo = m_resourcePackerInfoSet.m_resourcePackerInfosAll[i];
				if ((resInfo.m_isAssetBundle && resInfo.IsResident()) && !resInfo.IsAssetBundleLoaded())
				{
					resInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
					yield return null;
				}
				i++;
			}
		}
	}


	public bool CheckCachedResource(string fullPathInResources)
	{
		string s = CFileManager.EraseExtension(fullPathInResources);
		CResource cResource = null;
		return this.m_cachedResourceMap.TryGetValue(s.JavaHashCodeIgnoreCase(), out cResource);
	}

	public CResource GetResource(string fullPathInResources, Type resourceContentType, enResourceType resourceType, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
	{
		if (string.IsNullOrEmpty(fullPathInResources))
		{
			return new CResource(0, string.Empty, null, resourceType, unloadBelongedAssetBundleAfterLoaded);
		}
		string text = CFileManager.EraseExtension(fullPathInResources);
		int num = text.JavaHashCodeIgnoreCase();
		CResource cResource = null;
		if (this.m_cachedResourceMap.TryGetValue(num, out cResource))
		{
			if (cResource.m_resourceType != resourceType)
			{
				cResource.m_resourceType = resourceType;
			}
			return cResource;
		}
		cResource = new CResource(num, fullPathInResources, resourceContentType, resourceType, unloadBelongedAssetBundleAfterLoaded);
		try
		{
			this.LoadResource(cResource);
		}
		catch (Exception ex)
		{
			DebugHelper.Assert(false, "FLR {0} : {1}", new object[]
			{
				text,
				ex.Message
			});
			throw ex;
		}
		if (needCached)
		{
			this.m_cachedResourceMap.Add(num, cResource);
		}
		return cResource;
	}

	public void RemoveCachedResource(string fullPathInResources)
	{
		string s = CFileManager.EraseExtension(fullPathInResources);
		int key = s.JavaHashCodeIgnoreCase();
		CResource cResource = null;
		if (this.m_cachedResourceMap.TryGetValue(key, out cResource))
		{
			cResource.Unload();
			this.m_cachedResourceMap.Remove(key);
		}
	}

	public void RemoveCachedResources(enResourceType resourceType, bool clearImmediately = true)
	{
		List<int> list = new List<int>();
		DictionaryView<int, CResource>.Enumerator enumerator = this.m_cachedResourceMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, CResource> current = enumerator.Current;
			CResource value = current.Value;
			if (value.m_resourceType == resourceType)
			{
				value.Unload();
				list.Add(value.m_key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.m_cachedResourceMap.Remove(list[i]);
		}
		if (clearImmediately)
		{
			this.UnloadAllAssetBundles();
			this.UnloadUnusedAssets();
		}
	}

	public void RemoveCachedResources(enResourceType[] resourceTypes)
	{
		for (int i = 0; i < resourceTypes.Length; i++)
		{
			this.RemoveCachedResources(resourceTypes[i], false);
		}
		this.UnloadAllAssetBundles();
		this.UnloadUnusedAssets();
	}

	public void RemoveAllCachedResources()
	{
		this.RemoveCachedResources((enResourceType[])Enum.GetValues(typeof(enResourceType)));
	}

	public void UnloadBelongedAssetbundle(string fullPathInResources)
	{
		CResourcePackerInfo resourceBelongedPackerInfo = this.GetResourceBelongedPackerInfo(fullPathInResources);
		if (resourceBelongedPackerInfo != null && resourceBelongedPackerInfo.IsAssetBundleLoaded())
		{
			resourceBelongedPackerInfo.UnloadAssetBundle(false);
		}
	}

	public void UnloadAssetBundlesByTag(string tag)
	{
		if (this.m_resourcePackerInfoSet != null)
		{
			for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
			{
				if (this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].m_tag.Equals(tag))
				{
					this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].UnloadAssetBundle(false);
				}
			}
		}
	}

	public void UnloadUnusedAssets()
	{
		this.m_clearUnusedAssets = true;
		this.m_clearUnusedAssetsExecuteFrame = CResourceManager.s_frameCounter + 1;
	}

	private void ExecuteUnloadUnusedAssets()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void UnloadAllAssetBundles()
	{
		if (this.m_resourcePackerInfoSet == null)
		{
			return;
		}
		for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
		{
			CResourcePackerInfo cResourcePackerInfo = this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i];
			if (cResourcePackerInfo.IsAssetBundleLoaded())
			{
				bool flag = true;
				if (flag)
				{
					cResourcePackerInfo.UnloadAssetBundle(false);
				}
			}
		}
	}

	public CResourcePackerInfo GetResourceBelongedPackerInfo(string fullPathInResources)
	{
		if (string.IsNullOrEmpty(fullPathInResources))
		{
			return null;
		}
		if (this.m_resourcePackerInfoSet != null)
		{
			return this.m_resourcePackerInfoSet.GetResourceBelongedPackerInfo(CFileManager.EraseExtension(fullPathInResources).JavaHashCodeIgnoreCase());
		}
		return null;
	}

	public string GetAssetBundleInfoString()
	{
		if (this.m_resourcePackerInfoSet == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		int num = 0;
		for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
		{
			if (this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].IsAssetBundleLoaded())
			{
				num++;
				text += CFileManager.GetFullName(this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].m_pathInIFS);
			}
		}
		return text;
	}

	public void LoadAssetBundle(string fullPathInIFS)
	{
		this.LoadAssetBundle(this.GetResourcePackerInfo(fullPathInIFS));
	}

	public void LoadAssetBundle(CResourcePackerInfo resourcePackerInfo)
	{
		if (resourcePackerInfo == null || !resourcePackerInfo.m_isAssetBundle || resourcePackerInfo.IsAssetBundleLoaded())
		{
			return;
		}
		resourcePackerInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
	}

	public Type GetResourceContentType(string extension)
	{
		Type result = null;
		if (string.Equals(extension, ".prefab", StringComparison.OrdinalIgnoreCase))
		{
			result = typeof(GameObject);
		}
		else if (string.Equals(extension, ".bytes", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
		{
			result = typeof(TextAsset);
		}
		else if (string.Equals(extension, ".asset", StringComparison.OrdinalIgnoreCase))
		{
			result = typeof(ScriptableObject);
		}
		return result;
	}

	private CResourcePackerInfo GetResourcePackerInfo(string fullPathInIFS)
	{
		if (string.IsNullOrEmpty(fullPathInIFS) || this.m_resourcePackerInfoSet == null)
		{
			return null;
		}
		for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
		{
			if (string.Equals(fullPathInIFS, this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].m_pathInIFS))
			{
				return this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i];
			}
		}
		return null;
	}

	private void LoadResource(CResource resource)
	{
		CResourcePackerInfo resourceBelongedPackerInfo = this.GetResourceBelongedPackerInfo(resource);
		if (resourceBelongedPackerInfo != null)
		{
			if (resourceBelongedPackerInfo.m_isAssetBundle)
			{
				if (!resourceBelongedPackerInfo.IsAssetBundleLoaded())
				{
					resourceBelongedPackerInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
				}
				resource.LoadFromAssetBundle(resourceBelongedPackerInfo);
				if (resource.m_unloadBelongedAssetBundleAfterLoaded)
				{
					resourceBelongedPackerInfo.UnloadAssetBundle(false);
				}
			}
			else
			{
				resource.Load(CFileManager.GetIFSExtractPath());
			}
		}
		else
		{
			resource.Load();
		}
	}

	private CResourcePackerInfo GetResourceBelongedPackerInfo(CResource resource)
	{
		if (this.m_resourcePackerInfoSet != null)
		{
			CResourcePackerInfo resourceBelongedPackerInfo = this.m_resourcePackerInfoSet.GetResourceBelongedPackerInfo(resource.m_key);
			if (resourceBelongedPackerInfo != null && !resourceBelongedPackerInfo.m_isAssetBundle)
			{
				string empty = string.Empty;
				if (!resourceBelongedPackerInfo.m_fileExtMap.TryGetValue(resource.m_fullPathInResourcesWithoutExtension.JavaHashCodeIgnoreCase(), out empty))
				{
					Debug.LogError("No Resource " + resource.m_fullPathInResourcesWithoutExtension + " found in ext name map of bundle:" + resourceBelongedPackerInfo.m_pathInIFS);
				}
				resource.m_fileFullPathInResources = resource.m_fullPathInResourcesWithoutExtension + "." + empty;
			}
			return resourceBelongedPackerInfo;
		}
		return null;
	}
}
