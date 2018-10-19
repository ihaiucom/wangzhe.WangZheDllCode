using System;
using UnityEngine;

public class CResource
{
	public int m_key;

	public string m_name;

	public string m_extName;

	public string m_fullPathInResources;

	public string m_fullPathInResourcesWithoutExtension;

	public string m_fileFullPathInResources;

	public enResourceType m_resourceType;

	public enResourceState m_state;

	public Type m_contentType;

	public UnityEngine.Object m_content;

	public bool m_isAbandon;

	public bool m_unloadBelongedAssetBundleAfterLoaded;

	public CResource(int keyHash, string fullPathInResources, Type contentType, enResourceType resourceType, bool unloadBelongedAssetBundleAfterLoaded)
	{
		this.m_key = keyHash;
		this.m_fullPathInResources = fullPathInResources;
		this.m_fullPathInResourcesWithoutExtension = CFileManager.EraseExtension(this.m_fullPathInResources);
		this.m_name = CFileManager.EraseExtension(CFileManager.GetFullName(fullPathInResources));
		this.m_resourceType = resourceType;
		this.m_state = enResourceState.Unload;
		this.m_contentType = contentType;
		this.m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
		this.m_content = null;
		this.m_isAbandon = false;
	}

	public void Load()
	{
		if (this.m_isAbandon)
		{
			this.m_state = enResourceState.Unload;
			return;
		}
		if (this.m_contentType == null)
		{
			this.m_content = Resources.Load(CFileManager.EraseExtension(this.m_fullPathInResources));
		}
		else
		{
			this.m_content = Resources.Load(CFileManager.EraseExtension(this.m_fullPathInResources), this.m_contentType);
		}
		this.m_state = enResourceState.Loaded;
		if (this.m_content != null && this.m_content.GetType() == typeof(TextAsset))
		{
			CBinaryObject cBinaryObject = ScriptableObject.CreateInstance<CBinaryObject>();
			cBinaryObject.m_data = (this.m_content as TextAsset).bytes;
			this.m_content = cBinaryObject;
		}
	}

	public void Load(string ifsExtractPath)
	{
		if (this.m_isAbandon)
		{
			this.m_state = enResourceState.Unload;
			return;
		}
		byte[] array = CFileManager.ReadFile(CFileManager.CombinePath(ifsExtractPath, this.m_fileFullPathInResources));
		this.m_state = enResourceState.Loaded;
		if (array != null)
		{
			CBinaryObject cBinaryObject = ScriptableObject.CreateInstance<CBinaryObject>();
			cBinaryObject.m_data = array;
			cBinaryObject.name = this.m_name;
			this.m_content = cBinaryObject;
		}
	}

	public void LoadFromAssetBundle(CResourcePackerInfo resourcePackerInfo)
	{
		if (this.m_isAbandon)
		{
			this.m_state = enResourceState.Unload;
			return;
		}
		string name = this.m_name;
		string rename = resourcePackerInfo.GetRename(this.m_fullPathInResourcesWithoutExtension);
		if (!string.IsNullOrEmpty(rename))
		{
			name = CFileManager.GetFullName(rename);
		}
		if (this.m_contentType == null)
		{
			this.m_content = resourcePackerInfo.m_assetBundle.Load(name);
		}
		else
		{
			this.m_content = resourcePackerInfo.m_assetBundle.Load(name, this.m_contentType);
		}
		this.m_state = enResourceState.Loaded;
		if (this.m_content != null && this.m_content.GetType() == typeof(TextAsset))
		{
			CBinaryObject cBinaryObject = ScriptableObject.CreateInstance<CBinaryObject>();
			cBinaryObject.m_data = (this.m_content as TextAsset).bytes;
			this.m_content = cBinaryObject;
		}
	}

	public void Unload()
	{
		if (this.m_state == enResourceState.Loaded)
		{
			this.m_content = null;
			this.m_state = enResourceState.Unload;
		}
		else if (this.m_state == enResourceState.Loading)
		{
			this.m_isAbandon = true;
		}
	}
}
