using System;
using System.IO;
using UnityEngine;

namespace behaviac
{
	public class FileManager
	{
		private static FileManager ms_instance;

		public static FileManager Instance
		{
			get
			{
				if (FileManager.ms_instance == null)
				{
					FileManager.ms_instance = new FileManager();
				}
				return FileManager.ms_instance;
			}
		}

		public FileManager()
		{
			FileManager.ms_instance = this;
		}

		public virtual byte[] FileOpen(string filePath, string ext)
		{
			try
			{
				filePath += ext;
				int num = filePath.IndexOf("Resources");
				if (num != -1)
				{
					num += 10;
					string fullPathInResources = filePath.Substring(num);
					CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(TextAsset), enResourceType.BattleScene, false, false).m_content as CBinaryObject;
					byte[] result;
					if (cBinaryObject == null)
					{
						string text = string.Format("FileManager::FileOpen failed:'{0}' not loaded", filePath);
						result = null;
						return result;
					}
					byte[] data = cBinaryObject.m_data;
					result = data;
					return result;
				}
				else
				{
					string text2 = string.Format("FileManager::FileOpen failed:'{0}' should be in /Resources", filePath);
				}
			}
			catch
			{
				string text3 = string.Format("FileManager::FileOpen exception:'{0}'", filePath);
			}
			return null;
		}

		public virtual void FileClose(string filePath, string ext, byte[] pBuffer)
		{
		}

		public virtual bool FileExist(string filePath, string ext)
		{
			return File.Exists(filePath + ext);
		}
	}
}
