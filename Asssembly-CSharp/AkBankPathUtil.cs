using System;
using System.IO;

public class AkBankPathUtil
{
	private static string defaultBasePath;

	private static bool isToUsePosixPathSeparator;

	private static bool isToAppendTrailingPathSeparator;

	static AkBankPathUtil()
	{
		AkBankPathUtil.defaultBasePath = Path.Combine("Audio", "GeneratedSoundBanks");
		AkBankPathUtil.isToUsePosixPathSeparator = false;
		AkBankPathUtil.isToAppendTrailingPathSeparator = true;
		AkBankPathUtil.isToUsePosixPathSeparator = false;
	}

	public static void UsePosixPath()
	{
		AkBankPathUtil.isToUsePosixPathSeparator = true;
	}

	public static void UsePlatformSpecificPath()
	{
		AkBankPathUtil.isToUsePosixPathSeparator = false;
	}

	public static void SetToAppendTrailingPathSeparator(bool add)
	{
		AkBankPathUtil.isToAppendTrailingPathSeparator = add;
	}

	public static bool Exists(string path)
	{
		return Directory.Exists(path);
	}

	public static string GetDefaultPath()
	{
		return AkBankPathUtil.defaultBasePath;
	}

	public static string GetFullBasePath()
	{
		string basePath = AkInitializer.GetBasePath();
		AkBankPathUtil.LazyAppendTrailingSeparator(ref basePath);
		AkBankPathUtil.LazyConvertPathConvention(ref basePath);
		return basePath;
	}

	public static string GetPlatformBasePath()
	{
		string result = string.Empty;
		result = Path.Combine(AkBankPathUtil.GetFullBasePath(), AkBankPathUtil.GetPlatformSubDirectory());
		AkBankPathUtil.LazyAppendTrailingSeparator(ref result);
		AkBankPathUtil.LazyConvertPathConvention(ref result);
		return result;
	}

	public static string GetPlatformSubDirectory()
	{
		return "Android";
	}

	public static void LazyConvertPathConvention(ref string path)
	{
		if (AkBankPathUtil.isToUsePosixPathSeparator)
		{
			AkBankPathUtil.ConvertToPosixPath(ref path);
		}
		else if (Path.DirectorySeparatorChar == '/')
		{
			AkBankPathUtil.ConvertToPosixPath(ref path);
		}
		else
		{
			AkBankPathUtil.ConvertToWindowsPath(ref path);
		}
	}

	public static void ConvertToWindowsPath(ref string path)
	{
		path.Trim();
		path = path.Replace("/", "\\");
		path = path.TrimStart(new char[]
		{
			'\\'
		});
	}

	public static void ConvertToWindowsCommandPath(ref string path)
	{
		path.Trim();
		path = path.Replace("/", "\\\\");
		path = path.Replace("\\", "\\\\");
		path = path.TrimStart(new char[]
		{
			'\\'
		});
	}

	public static void ConvertToPosixPath(ref string path)
	{
		path.Trim();
		path = path.Replace("\\", "/");
		path = path.TrimStart(new char[]
		{
			'\\'
		});
	}

	public static void LazyAppendTrailingSeparator(ref string path)
	{
		if (!AkBankPathUtil.isToAppendTrailingPathSeparator)
		{
			return;
		}
		string arg_1A_0 = path;
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		if (!arg_1A_0.EndsWith(directorySeparatorChar.ToString()))
		{
			path += Path.DirectorySeparatorChar;
		}
	}
}
