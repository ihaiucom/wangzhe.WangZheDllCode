using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CFileManager
{
	public delegate void DelegateOnOperateFileFail(string fullPath, enFileOperation fileOperation, Exception ex);

	private static string s_cachePath = null;

	public static string s_ifsExtractFolder = "Resources";

	private static string s_ifsExtractPath = null;

	private static MD5CryptoServiceProvider s_md5Provider = new MD5CryptoServiceProvider();

	public static CFileManager.DelegateOnOperateFileFail s_delegateOnOperateFileFail = delegate
	{
	};

	public static bool IsFileExist(string filePath)
	{
		return File.Exists(filePath);
	}

	public static bool IsDirectoryExist(string directory)
	{
		return Directory.Exists(directory);
	}

	public static bool CreateDirectory(string directory)
	{
		if (CFileManager.IsDirectoryExist(directory))
		{
			return true;
		}
		int num = 0;
		bool result;
		while (true)
		{
			try
			{
				Directory.CreateDirectory(directory);
				result = true;
				break;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
				{
					Debug.Log("Create Directory " + directory + " Error! Exception = " + ex.ToString());
					CFileManager.s_delegateOnOperateFileFail(directory, enFileOperation.CreateDirectory, ex);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static bool DeleteDirectory(string directory)
	{
		if (!CFileManager.IsDirectoryExist(directory))
		{
			return true;
		}
		int num = 0;
		bool result;
		while (true)
		{
			try
			{
				Directory.Delete(directory, true);
				result = true;
				break;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
				{
					Debug.Log("Delete Directory " + directory + " Error! Exception = " + ex.ToString());
					CFileManager.s_delegateOnOperateFileFail(directory, enFileOperation.DeleteDirectory, ex);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static int GetFileLength(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return 0;
		}
		int num = 0;
		int result;
		while (true)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(filePath);
				result = (int)fileInfo.get_Length();
				break;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
				{
					Debug.Log("Get FileLength of " + filePath + " Error! Exception = " + ex.ToString());
					result = 0;
					break;
				}
			}
		}
		return result;
	}

	public static byte[] ReadFile(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return null;
		}
		byte[] array = null;
		int num = 0;
		Exception ex;
		while (true)
		{
			ex = null;
			try
			{
				array = File.ReadAllBytes(filePath);
			}
			catch (Exception ex2)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Read File ",
					filePath,
					" Error! Exception = ",
					ex2.ToString(),
					", TryCount = ",
					num
				}));
				array = null;
				ex = ex2;
			}
			if (array != null && array.Length > 0)
			{
				break;
			}
			num++;
			if (num >= 3)
			{
				goto Block_5;
			}
		}
		return array;
		Block_5:
		Debug.Log(string.Concat(new object[]
		{
			"Read File ",
			filePath,
			" Fail!, TryCount = ",
			num
		}));
		CFileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.ReadFile, ex);
		return null;
	}

	public static bool WriteFile(string filePath, byte[] data)
	{
		int num = 0;
		bool result;
		while (true)
		{
			try
			{
				File.WriteAllBytes(filePath, data);
				result = true;
				break;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
				{
					Debug.Log("Write File " + filePath + " Error! Exception = " + ex.ToString());
					CFileManager.DeleteFile(filePath);
					CFileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.WriteFile, ex);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static bool WriteFile(string filePath, byte[] data, int offset, int length)
	{
		FileStream fileStream = null;
		int num = 0;
		bool result;
		while (true)
		{
			try
			{
				fileStream = new FileStream(filePath, 4, 2, 3);
				fileStream.Write(data, offset, length);
				fileStream.Close();
				result = true;
				break;
			}
			catch (Exception ex)
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				num++;
				if (num >= 3)
				{
					Debug.Log("Write File " + filePath + " Error! Exception = " + ex.ToString());
					CFileManager.DeleteFile(filePath);
					CFileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.WriteFile, ex);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static bool DeleteFile(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return true;
		}
		int num = 0;
		bool result;
		while (true)
		{
			try
			{
				File.Delete(filePath);
				result = true;
				break;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
				{
					Debug.Log("Delete File " + filePath + " Error! Exception = " + ex.ToString());
					CFileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.DeleteFile, ex);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static void CopyFile(string srcFile, string dstFile)
	{
		File.Copy(srcFile, dstFile, true);
	}

	public static string GetFileMd5(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return string.Empty;
		}
		return BitConverter.ToString(CFileManager.s_md5Provider.ComputeHash(CFileManager.ReadFile(filePath))).Replace("-", string.Empty);
	}

	public static string GetMd5(byte[] data)
	{
		return BitConverter.ToString(CFileManager.s_md5Provider.ComputeHash(data)).Replace("-", string.Empty);
	}

	public static string GetMd5(string str)
	{
		return BitConverter.ToString(CFileManager.s_md5Provider.ComputeHash(Encoding.get_UTF8().GetBytes(str))).Replace("-", string.Empty);
	}

	public static string CombinePath(string path1, string path2)
	{
		if (path1.LastIndexOf('/') != path1.get_Length() - 1)
		{
			path1 += "/";
		}
		if (path2.IndexOf('/') == 0)
		{
			path2 = path2.Substring(1);
		}
		return path1 + path2;
	}

	public static string CombinePaths(params string[] values)
	{
		if (values.Length <= 0)
		{
			return string.Empty;
		}
		if (values.Length == 1)
		{
			return CFileManager.CombinePath(values[0], string.Empty);
		}
		if (values.Length > 1)
		{
			string text = CFileManager.CombinePath(values[0], values[1]);
			for (int i = 2; i < values.Length; i++)
			{
				text = CFileManager.CombinePath(text, values[i]);
			}
			return text;
		}
		return string.Empty;
	}

	public static string GetStreamingAssetsPathWithHeader(string fileName)
	{
		return Path.Combine(Application.streamingAssetsPath, fileName);
	}

	public static string GetCachePath()
	{
		if (CFileManager.s_cachePath == null)
		{
			CFileManager.s_cachePath = Application.persistentDataPath;
		}
		return CFileManager.s_cachePath;
	}

	public static string GetCachePath(string fileName)
	{
		return CFileManager.CombinePath(CFileManager.GetCachePath(), fileName);
	}

	public static string GetCachePathWithHeader(string fileName)
	{
		return CFileManager.GetLocalPathHeader() + CFileManager.GetCachePath(fileName);
	}

	public static string GetIFSExtractPath()
	{
		if (CFileManager.s_ifsExtractPath == null)
		{
			CFileManager.s_ifsExtractPath = CFileManager.CombinePath(CFileManager.GetCachePath(), CFileManager.s_ifsExtractFolder);
		}
		return CFileManager.s_ifsExtractPath;
	}

	public static string GetFullName(string fullPath)
	{
		if (fullPath == null)
		{
			return null;
		}
		int num = fullPath.LastIndexOf("/");
		if (num > 0)
		{
			return fullPath.Substring(num + 1, fullPath.get_Length() - num - 1);
		}
		return fullPath;
	}

	public static string EraseExtension(string fullName)
	{
		if (fullName == null)
		{
			return null;
		}
		int num = fullName.LastIndexOf('.');
		if (num > 0)
		{
			return fullName.Substring(0, num);
		}
		return fullName;
	}

	public static string GetExtension(string fullName)
	{
		int num = fullName.LastIndexOf('.');
		if (num > 0 && num + 1 < fullName.get_Length())
		{
			return fullName.Substring(num);
		}
		return string.Empty;
	}

	public static string GetFullDirectory(string fullPath)
	{
		return Path.GetDirectoryName(fullPath);
	}

	public static bool ClearDirectory(string fullPath)
	{
		bool result;
		try
		{
			string[] files = Directory.GetFiles(fullPath);
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
			string[] directories = Directory.GetDirectories(fullPath);
			for (int j = 0; j < directories.Length; j++)
			{
				Directory.Delete(directories[j], true);
			}
			result = true;
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	public static bool ClearDirectory(string fullPath, string[] fileExtensionFilter, string[] folderFilter)
	{
		bool result;
		try
		{
			if (fileExtensionFilter != null)
			{
				string[] files = Directory.GetFiles(fullPath);
				for (int i = 0; i < files.Length; i++)
				{
					if (fileExtensionFilter != null && fileExtensionFilter.Length > 0)
					{
						for (int j = 0; j < fileExtensionFilter.Length; j++)
						{
							if (files[i].Contains(fileExtensionFilter[j]))
							{
								CFileManager.DeleteFile(files[i]);
								break;
							}
						}
					}
				}
			}
			if (folderFilter != null)
			{
				string[] directories = Directory.GetDirectories(fullPath);
				for (int k = 0; k < directories.Length; k++)
				{
					if (folderFilter != null && folderFilter.Length > 0)
					{
						for (int l = 0; l < folderFilter.Length; l++)
						{
							if (directories[k].Contains(folderFilter[l]))
							{
								CFileManager.DeleteDirectory(directories[k]);
								break;
							}
						}
					}
				}
			}
			result = true;
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	private static string GetLocalPathHeader()
	{
		return "file://";
	}
}
