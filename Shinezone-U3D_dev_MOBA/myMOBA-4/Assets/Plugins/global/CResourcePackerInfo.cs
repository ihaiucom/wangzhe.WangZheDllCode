using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CResourcePackerInfo
{
	public const string DEFAULT_TAG = "DT";

	public bool m_isAssetBundle;

	public enAssetBundleState m_assetBundleState;

	public bool m_useASyncLoadingData;

	public AssetBundle m_assetBundle;

	public string m_pathInIFS;

	public string m_tag = "DT";

	public ListView<stResourceInfo> m_resourceInfos = new ListView<stResourceInfo>();

	public Dictionary<int, string> m_fileExtMap;

	public Dictionary<int, string> m_renameMap;

	public ushort m_flags;

	private CResourcePackerInfo m_parent;

	public ListView<CResourcePackerInfo> m_children = new ListView<CResourcePackerInfo>();

	public CResourcePackerInfo dependency
	{
		get
		{
			return this.m_parent;
		}
		set
		{
			this.m_parent = value;
			value.m_children.Add(this);
		}
	}

	public CResourcePackerInfo(bool isAssetBundle)
	{
		this.m_isAssetBundle = isAssetBundle;
		this.m_assetBundleState = enAssetBundleState.Unload;
		this.m_useASyncLoadingData = false;
	}

	public bool IsResident()
	{
		return this.dependency == null && this.HasFlag(enBundleFlag.Resident);
	}

	public bool IsUnCompress()
	{
		return this.HasFlag(enBundleFlag.UnCompress);
	}

	public bool IsKeepInResources()
	{
		return this.HasFlag(enBundleFlag.KeepInResources);
	}

	public bool IsAllowDuplicateNames()
	{
		return this.HasFlag(enBundleFlag.AllowDuplicateNames);
	}

	public bool IsReplaceDuplicateNames()
	{
		return this.HasFlag(enBundleFlag.ReplaceDuplicateNames);
	}

	public bool IsCompleteAssets()
	{
		return !this.HasFlag(enBundleFlag.UnCompleteAsset);
	}

	public void AddResourceInfo(ref stResourceInfo resourceInfo)
	{
		this.m_resourceInfos.Add(resourceInfo);
	}

	public bool HasFlag(enBundleFlag flag)
	{
		return ((enBundleFlag)this.m_flags & flag) > (enBundleFlag)0;
	}

	public virtual void Read(byte[] data, ref int offset)
	{
		this.m_isAssetBundle = (CMemoryManager.ReadByte(data, ref offset) > 0);
		this.m_pathInIFS = CMemoryManager.ReadString(data, ref offset);
		this.m_tag = CMemoryManager.ReadString(data, ref offset);
		this.m_flags = (ushort)CMemoryManager.ReadInt(data, ref offset);
		int num = CMemoryManager.ReadShort(data, ref offset);
		this.m_resourceInfos.Clear();
		if (!this.m_isAssetBundle)
		{
			this.m_fileExtMap = new Dictionary<int, string>();
		}
		for (int i = 0; i < num; i++)
		{
			stResourceInfo stResourceInfo = new stResourceInfo();
			string s = CMemoryManager.ReadString(data, ref offset);
			stResourceInfo.m_hashCode = s.JavaHashCodeIgnoreCase();
			stResourceInfo.extension = CMemoryManager.ReadString(data, ref offset);
			stResourceInfo.m_flags = (byte)CMemoryManager.ReadInt(data, ref offset);
			bool flag = CMemoryManager.ReadByte(data, ref offset) > 0;
			if (flag)
			{
				stResourceInfo.m_fullPathInResourcesWithoutExtension_Renamed = CMemoryManager.ReadString(data, ref offset);
			}
			if (!this.m_isAssetBundle)
			{
				string value = stResourceInfo.extension.Replace(".", string.Empty);
				this.m_fileExtMap[stResourceInfo.m_hashCode] = value;
			}
			this.m_resourceInfos.Add(stResourceInfo);
		}
		num = CMemoryManager.ReadShort(data, ref offset);
		this.m_children.Clear();
		for (int j = 0; j < num; j++)
		{
			CResourcePackerInfo cResourcePackerInfo = new CResourcePackerInfo(true);
			cResourcePackerInfo.Read(data, ref offset);
			cResourcePackerInfo.dependency = this;
		}
	}

	public void LoadAssetBundle(string ifsExtractPath)
	{
		if (this.m_isAssetBundle)
		{
			if (this.dependency != null && this.dependency.m_isAssetBundle && !this.dependency.IsAssetBundleLoaded())
			{
				this.dependency.LoadAssetBundle(ifsExtractPath);
			}
			if (this.m_assetBundleState != enAssetBundleState.Unload)
			{
				return;
			}
			this.m_useASyncLoadingData = false;
			string text = CFileManager.CombinePath(ifsExtractPath, this.m_pathInIFS);
			if (CFileManager.IsFileExist(text))
			{
				if (this.IsUnCompress())
				{
					int num = 0;
					Exception ex;
					while (true)
					{
						ex = null;
						try
						{
							this.m_assetBundle = AssetBundle.CreateFromFile(text);
						}
						catch (Exception ex2)
						{
							this.m_assetBundle = null;
							ex = ex2;
						}
						if (this.m_assetBundle != null)
						{
							break;
						}
						Debug.Log(string.Concat(new object[]
						{
							"Create AssetBundle ",
							text,
							" From File Error! Try Count = ",
							num
						}));
						num++;
						if (num >= 3)
						{
							goto Block_10;
						}
					}
					goto IL_101;
					Block_10:
					CFileManager.s_delegateOnOperateFileFail(text, enFileOperation.ReadFile, ex);
					IL_101:;
				}
				else
				{
					this.m_assetBundle = AssetBundle.CreateFromMemoryImmediate(CFileManager.ReadFile(text));
				}
				if (this.m_assetBundle == null)
				{
					string text2 = string.Empty;
					try
					{
						text2 = CFileManager.GetFileMd5(text);
					}
					catch (Exception)
					{
						text2 = string.Empty;
					}
					string message = string.Format("Load AssetBundle {0} Error!!! App version = {1}, Build = {2}, Reversion = {3}, Resource version = {4}, File md5 = {5}", new object[]
					{
						text,
						CVersion.GetAppVersion(),
						CVersion.GetBuildNumber(),
						CVersion.GetRevisonNumber(),
						CVersion.GetUsedResourceVersion(),
						text2
					});
					Debug.Log(message);
				}
			}
			else
			{
				Debug.Log("File " + text + " can not be found!!!");
			}
			this.m_assetBundleState = enAssetBundleState.Loaded;
		}
	}

    public IEnumerator ASyncLoadAssetBundle(string ifsExtractPath)
    {
        if (!m_isAssetBundle)
        {
            yield break;
        }
        if (dependency != null)
        {
			// bsh: Whether or not something is missing?
        }
        m_useASyncLoadingData = true;
        m_assetBundleState = enAssetBundleState.Loading;
        var assetBundleLoader = AssetBundle.CreateFromMemory(CFileManager.ReadFile(CFileManager.CombinePath(ifsExtractPath, m_pathInIFS)));
        yield return assetBundleLoader;
        if (m_useASyncLoadingData)
        {
            m_assetBundle = assetBundleLoader.assetBundle;
        }
        m_assetBundleState = enAssetBundleState.Loaded;
    }

	public void UnloadAssetBundle(bool force = false)
	{
		if (!this.m_isAssetBundle)
		{
			return;
		}
		if (this.IsResident() && !force)
		{
			return;
		}
		if (this.m_assetBundleState == enAssetBundleState.Loaded)
		{
			if (this.m_assetBundle != null)
			{
				this.m_assetBundle.Unload(false);
				this.m_assetBundle = null;
			}
			this.m_assetBundleState = enAssetBundleState.Unload;
		}
		else if (this.m_assetBundleState == enAssetBundleState.Loading)
		{
			this.m_useASyncLoadingData = false;
		}
		if (this.dependency != null)
		{
			this.dependency.UnloadAssetBundle(force);
		}
	}

	public bool IsAssetBundleLoaded()
	{
		return this.m_isAssetBundle && this.m_assetBundleState == enAssetBundleState.Loaded;
	}

	public bool IsAssetBundleInLoading()
	{
		return this.m_isAssetBundle && this.m_assetBundleState == enAssetBundleState.Loading;
	}

	public string GetRename(string pathInResourcesFolderWithoutExt)
	{
		if (this.m_renameMap == null)
		{
			return string.Empty;
		}
		string empty = string.Empty;
		if (this.m_renameMap.TryGetValue(pathInResourcesFolderWithoutExt.JavaHashCodeIgnoreCase(), out empty))
		{
			return empty;
		}
		return string.Empty;
	}

	public void AddToResourceMap(DictionaryView<int, CResourcePackerInfo> map)
	{
		for (int i = 0; i < this.m_resourceInfos.Count; i++)
		{
			if (this.m_resourceInfos[i].isRenamed)
			{
				if (this.m_renameMap == null)
				{
					this.m_renameMap = new Dictionary<int, string>();
				}
				this.m_renameMap.Add(this.m_resourceInfos[i].m_hashCode, this.m_resourceInfos[i].m_fullPathInResourcesWithoutExtension_Renamed.ToLower());
			}
			if (!map.ContainsKey(this.m_resourceInfos[i].m_hashCode))
			{
				map.Add(this.m_resourceInfos[i].m_hashCode, this);
			}
		}
	}
}
