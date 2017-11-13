using System;
using UnityEngine;

public class DeviceCheckSys
{
	public const int LowCoreNum = 1;

	public const int LowMemorySize = 700;

	public const int LowEneterMemorySize = 300;

	public const int LowWidth = 800;

	public const int LowHeight = 480;

	public const string CurMemNotEnoughPopTimeKey = "DeviceCheck_MemNotEnough";

	public static string deviceName;

	public static string cpuName;

	public static int cpuCoreNum;

	public static int sysMemorySize;

	public static int width;

	public static int height;

	public static bool CheckDeviceIsValid()
	{
		return true;
	}

	public static bool CheckMemory()
	{
		DeviceCheckSys.sysMemorySize = SystemInfo.systemMemorySize;
		if (DeviceCheckSys.sysMemorySize < 700)
		{
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("MemoryNotEnough", null, true);
			return false;
		}
		return true;
	}

	public static int GetTotalMemoryMegaBytes()
	{
		return SystemInfo.systemMemorySize;
	}

	public static int GetAvailMemoryMegaBytes()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
		long num = 0L;
		if (androidJavaClass != null)
		{
			num = androidJavaClass.CallStatic<long>("getAvailMemory", new object[0]);
		}
		return (int)num;
	}

	public static void StartDumpTotalUsedMemory()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("startGetPssTotalThread", new object[0]);
		}
	}

	public static int GetTotalUsedMemoryMegaBytes()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
		int result = -1;
		if (androidJavaClass != null)
		{
			result = androidJavaClass.CallStatic<int>("getPssTotal", new object[0]);
		}
		return result;
	}

	public static bool CheckAvailMemory()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
		long num = 0L;
		if (androidJavaClass != null)
		{
			num = androidJavaClass.CallStatic<long>("getAvailMemory", new object[0]);
		}
		return num > 300L;
	}

	public static void RecordCurMemNotEnoughPopTimes()
	{
		string @string = PlayerPrefs.GetString("DeviceCheck_MemNotEnough");
		int num = 1;
		if (string.IsNullOrEmpty(@string))
		{
			PlayerPrefs.SetString("DeviceCheck_MemNotEnough", string.Format("{0}_{1}", DateTime.get_Today(), num));
		}
		else
		{
			string[] array = @string.Split(new char[]
			{
				'_'
			});
			if (array.Length >= 2 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]))
			{
				DateTime dateTime = Convert.ToDateTime(array[0]);
				num = Convert.ToInt32(array[1]);
				if (DateTime.get_Today() > dateTime)
				{
					num = 1;
				}
				else
				{
					num++;
				}
			}
			PlayerPrefs.SetString("DeviceCheck_MemNotEnough", string.Format("{0}_{1}", DateTime.get_Today(), num));
			PlayerPrefs.Save();
		}
	}

	public static int GetRecordCurMemNotEnoughPopTimes()
	{
		int result = 0;
		string @string = PlayerPrefs.GetString("DeviceCheck_MemNotEnough");
		if (!string.IsNullOrEmpty(@string))
		{
			string[] array = @string.Split(new char[]
			{
				'_'
			});
			if (array.Length >= 2 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]) && Convert.ToDateTime(array[0]) == DateTime.get_Today())
			{
				result = Convert.ToInt32(array[1]);
			}
		}
		return result;
	}

	public static int GetAvailMemory()
	{
		return 0;
	}

	public static bool CheckCPU()
	{
		DeviceCheckSys.cpuCoreNum = SystemInfo.processorCount;
		return DeviceCheckSys.cpuCoreNum >= 1;
	}

	public static bool CheckResolution()
	{
		DeviceCheckSys.width = Screen.width;
		DeviceCheckSys.height = Screen.height;
		if (DeviceCheckSys.width < 800 || DeviceCheckSys.height < 480)
		{
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("ResolutionNotValid", null, true);
			return false;
		}
		return true;
	}

	private static bool checkGPU_Adreno(string[] tokens)
	{
		int num = 0;
		for (int i = 1; i < tokens.Length; i++)
		{
			if (DeviceCheckSys.TryGetInt(ref num, tokens[i]))
			{
				if (num < 200)
				{
					return false;
				}
				if (num < 300)
				{
					return num > 220;
				}
				if (num < 400)
				{
					return num >= 320 || num >= 305;
				}
				if (num >= 400)
				{
					return num >= 410 || true;
				}
			}
		}
		return false;
	}

	private static bool checkGPU_PowerVR(string[] tokens)
	{
		bool flag = false;
		bool result = false;
		int num = 0;
		for (int i = 1; i < tokens.Length; i++)
		{
			string text = tokens[i];
			if (text == "sgx")
			{
				flag = true;
			}
			else
			{
				if (text == "rogue")
				{
					result = true;
					break;
				}
				if (flag)
				{
					int num2 = text.IndexOf("mp");
					if (num2 > 0)
					{
						DeviceCheckSys.TryGetInt(ref num, text.Substring(0, num2));
					}
					else if (DeviceCheckSys.TryGetInt(ref num, text))
					{
						for (int j = i + 1; j < tokens.Length; j++)
						{
							text = tokens[j].ToLower();
							if (text.IndexOf("mp") >= 0)
							{
								break;
							}
						}
					}
					if (num > 0)
					{
						return num >= 543 && (num == 543 || num != 544 || true);
					}
				}
				else if (text.get_Length() > 4)
				{
					char c = text.get_Chars(0);
					char c2 = text.get_Chars(1);
					if (c == 'g')
					{
						if (c2 >= '0' && c2 <= '9')
						{
							DeviceCheckSys.TryGetInt(ref num, text.Substring(1));
						}
						else
						{
							DeviceCheckSys.TryGetInt(ref num, text.Substring(2));
						}
						if (num > 0)
						{
							return num >= 7000 || (num >= 6000 && num >= 6100 && (num >= 6400 || true));
						}
					}
				}
			}
		}
		return result;
	}

	private static bool checkGPU_Mali(string[] tokens)
	{
		int num = 0;
		for (int i = 1; i < tokens.Length; i++)
		{
			string text = tokens[i];
			if (text.get_Length() >= 3)
			{
				int num2 = text.LastIndexOf("mp");
				bool flag = text.get_Chars(0) == 't';
				if (num2 > 0)
				{
					int num3 = flag ? 1 : 0;
					text = text.Substring(num3, num2 - num3);
					DeviceCheckSys.TryGetInt(ref num, text);
				}
				else
				{
					if (flag)
					{
						text = text.Substring(1);
					}
					if (DeviceCheckSys.TryGetInt(ref num, text))
					{
						for (int j = i + 1; j < tokens.Length; j++)
						{
							text = tokens[j];
							if (text.IndexOf("mp") >= 0)
							{
								break;
							}
						}
					}
				}
				if (num > 0)
				{
					if (num < 400)
					{
						return false;
					}
					if (num < 500)
					{
						return num == 400 || num == 450;
					}
					if (num < 700)
					{
						return flag && (num >= 620 || true);
					}
					return flag;
				}
			}
		}
		return false;
	}

	private static bool checkGPU_Tegra(string[] tokens)
	{
		bool flag = false;
		int num = 0;
		for (int i = 1; i < tokens.Length; i++)
		{
			if (DeviceCheckSys.TryGetInt(ref num, tokens[i]))
			{
				flag = true;
				if (num >= 4)
				{
					return true;
				}
				if (num == 3)
				{
					return true;
				}
			}
			else
			{
				string text = tokens[i];
				if (text == "k1")
				{
					return true;
				}
			}
		}
		return !flag;
	}

	private static bool TryGetInt(ref int val, string str)
	{
		val = 0;
		bool result;
		try
		{
			val = Convert.ToInt32(str);
			result = true;
		}
		catch
		{
			result = false;
		}
		return result;
	}
}
