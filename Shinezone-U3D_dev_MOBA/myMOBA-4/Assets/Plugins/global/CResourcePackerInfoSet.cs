using System;

public class CResourcePackerInfoSet
{
	public static string s_resourceIFSFileName = "sgame_resource.ifs";

	public static string s_resourcePackerInfoSetFileName = "ResourcePackerInfoSet.bytes";

	public string m_version;

	public string m_buildNumber;

	public string m_publish;

	public string m_ifsPath;

	public ListView<CResourcePackerInfo> m_resourcePackerInfosAll = new ListView<CResourcePackerInfo>();

	private DictionaryView<int, CResourcePackerInfo> m_resourceMap = new DictionaryView<int, CResourcePackerInfo>();

	public void Dispose()
	{
		for (int i = 0; i < this.m_resourcePackerInfosAll.Count; i++)
		{
			if (this.m_resourcePackerInfosAll[i].IsAssetBundleLoaded())
			{
				this.m_resourcePackerInfosAll[i].UnloadAssetBundle(false);
			}
			this.m_resourcePackerInfosAll[i] = null;
		}
		this.m_resourcePackerInfosAll.Clear();
		this.m_resourceMap.Clear();
	}

	public void AddResourcePackerInfo(CResourcePackerInfo resourceInfo)
	{
		this.m_resourcePackerInfosAll.Add(resourceInfo);
		for (int i = 0; i < resourceInfo.m_children.Count; i++)
		{
			this._AddResourcePackerInfoAll(resourceInfo.m_children[i]);
		}
	}

	private void _AddResourcePackerInfoAll(CResourcePackerInfo resourceInfo)
	{
		this.m_resourcePackerInfosAll.Add(resourceInfo);
		for (int i = 0; i < resourceInfo.m_children.Count; i++)
		{
			this._AddResourcePackerInfoAll(resourceInfo.m_children[i]);
		}
	}

	public void Read(byte[] data, ref int offset)
	{
		if (data == null || offset < 0 || offset >= data.Length)
		{
			return;
		}
		int num = offset;
		if (CMemoryManager.ReadInt(data, ref offset) > data.Length - num)
		{
			return;
		}
		this.m_version = CMemoryManager.ReadString(data, ref offset);
		this.m_buildNumber = CMemoryManager.ReadString(data, ref offset);
		this.m_publish = CMemoryManager.ReadString(data, ref offset);
		this.m_ifsPath = CMemoryManager.ReadString(data, ref offset);
		int num2 = CMemoryManager.ReadShort(data, ref offset);
		this.m_resourcePackerInfosAll.Clear();
		for (int i = 0; i < num2; i++)
		{
			CResourcePackerInfo cResourcePackerInfo = new CResourcePackerInfo(false);
			cResourcePackerInfo.Read(data, ref offset);
			this.AddResourcePackerInfo(cResourcePackerInfo);
		}
	}

	public static void ReadVersionAndBuildNumber(byte[] data, ref int offset, ref string version, ref string buildNumber)
	{
		version = CVersion.s_emptyResourceVersion;
		buildNumber = CVersion.s_emptyBuildNumber;
		if (data == null || offset < 0 || offset >= data.Length)
		{
			return;
		}
		int num = offset;
		if (CMemoryManager.ReadInt(data, ref offset) > data.Length - num)
		{
			return;
		}
		version = CMemoryManager.ReadString(data, ref offset);
		buildNumber = CMemoryManager.ReadString(data, ref offset);
	}

	public void CreateResourceMap()
	{
		for (int i = 0; i < this.m_resourcePackerInfosAll.Count; i++)
		{
			CResourcePackerInfo cResourcePackerInfo = this.m_resourcePackerInfosAll[i];
			cResourcePackerInfo.AddToResourceMap(this.m_resourceMap);
		}
	}

	public CResourcePackerInfo GetResourceBelongedPackerInfo(int resourceKeyHash)
	{
		CResourcePackerInfo result = null;
		if (this.m_resourceMap.TryGetValue(resourceKeyHash, out result))
		{
			return result;
		}
		return null;
	}
}
