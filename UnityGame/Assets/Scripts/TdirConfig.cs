using Mono.Xml;
using System;
using System.Collections;
using System.IO;
using System.Security;
using System.Text;
using UnityEngine;

public class TdirConfig
{
	public static TdirServerType WoYaoQiehuanJing = TdirServerType.NULL;

	public static string tdirConfigDataPath = "/TdirConfigData.xml";

	public static string[] iplist_test = new string[]
	{
		"testa4.mtcls.qq.com",
		"101.227.153.83"
	};

	public static string[] iplist_middle = new string[]
	{
		"middle.mtcls.qq.com",
		"101.226.141.88"
	};

	public static string[] iplist_normal = new string[]
	{
		"mtcls.qq.com",
		"61.151.224.100",
		"58.251.61.169",
		"203.205.151.237",
		"203.205.147.178",
		"183.61.49.177",
		"183.232.103.166",
		"182.254.4.176",
		"182.254.10.82",
		"140.207.127.61",
		"117.144.242.115"
	};

	public static string[] iplist_normal_tongcai = new string[]
	{
		"ft.smoba.qq.com",
		"101.226.76.200",
		"117.135.172.223",
		"140.206.160.117",
		"182.254.11.206"
	};

	public static string[] iplist_experience = new string[]
	{
		"exp.mtcls.qq.com",
		"61.151.234.47",
		"182.254.42.103",
		"140.207.62.111",
		"140.207.123.164",
		"117.144.242.28",
		"117.144.243.174",
		"103.7.30.91",
		"101.227.130.79"
	};

	public static string[] iplist_experience_test = new string[]
	{
		"testb4.mtcls.qq.com",
		"101.227.153.86"
	};

	public static string[] iplist_testForTester = new string[]
	{
		"testc.mtcls.qq.com",
		"183.61.39.51"
	};

	public static string[] iplist_competition_test = new string[]
	{
		"testa4.mtcls.qq.com",
		"101.227.153.83"
	};

	public static string[] iplist_competition_official = new string[]
	{
		"exp.mtcls.qq.com",
		"61.151.234.47",
		"182.254.42.103",
		"140.207.62.111",
		"140.207.123.164",
		"117.144.242.28",
		"117.144.243.174",
		"103.7.30.91",
		"101.227.130.79"
	};

	public static int[] portlist_test = new int[]
	{
		10002,
		10004,
		10006
	};

	public static int[] portlist_middle = new int[]
	{
		20002,
		20004,
		20006
	};

	public static int[] portlist_normal = new int[]
	{
		50012,
		50014,
		50016
	};

	public static int[] portlist_normal_tongcai = new int[]
	{
		50012,
		50014,
		50016
	};

	public static int[] portlist_experience = new int[]
	{
		10002,
		10004,
		10006
	};

	public static int[] portlist_experience_test = new int[]
	{
		10002,
		10004,
		10006
	};

	public static int[] portlist_testForTester = new int[]
	{
		10002,
		10004,
		10006
	};

	public static int[] portlist_competition_test = new int[]
	{
		10002,
		10004,
		10006
	};

	public static int[] portlist_competition_official = new int[]
	{
		10012,
		10014,
		10016
	};

	public static int AppID_android = 165675025;

	public static int AppID_iOS = 165675026;

	public static int AppID_android_competition = 165675166;

	public static int AppID_iOS_competition = 165675167;

	public static TdirServerType cheatServerType = TdirServerType.Normal;

	private static TdirConfigData tdirConfigData = null;

	public static TdirServerType curServerType = TdirServerType.NULL;

	public static string[] GetTdirIPList()
	{
		TdirConfigData fileTdirAndTverData = TdirConfig.GetFileTdirAndTverData();
		if (fileTdirAndTverData != null)
		{
			TdirConfig.curServerType = (TdirServerType)fileTdirAndTverData.serverType;
			if (fileTdirAndTverData.serverType == 1)
			{
				return TdirConfig.iplist_test;
			}
			if (fileTdirAndTverData.serverType == 2)
			{
				return TdirConfig.iplist_middle;
			}
			if (fileTdirAndTverData.serverType == 3)
			{
				if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
				{
					return TdirConfig.iplist_normal_tongcai;
				}
				return TdirConfig.iplist_normal;
			}
			else
			{
				if (fileTdirAndTverData.serverType == 4)
				{
					return TdirConfig.iplist_experience;
				}
				if (fileTdirAndTverData.serverType == 5)
				{
					return TdirConfig.iplist_experience_test;
				}
				if (fileTdirAndTverData.serverType == 6)
				{
					return TdirConfig.iplist_testForTester;
				}
				if (fileTdirAndTverData.serverType == 7)
				{
					return TdirConfig.iplist_competition_test;
				}
				if (fileTdirAndTverData.serverType == 8)
				{
					return TdirConfig.iplist_competition_official;
				}
			}
		}
		TdirConfig.curServerType = TdirConfig.cheatServerType;
		if (TdirConfig.cheatServerType == TdirServerType.Test)
		{
			return TdirConfig.iplist_test;
		}
		if (TdirConfig.cheatServerType == TdirServerType.Mid)
		{
			return TdirConfig.iplist_middle;
		}
		if (TdirConfig.cheatServerType == TdirServerType.Normal)
		{
			if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
			{
				return TdirConfig.iplist_normal_tongcai;
			}
			return TdirConfig.iplist_normal;
		}
		else
		{
			if (TdirConfig.cheatServerType == TdirServerType.Exp)
			{
				return TdirConfig.iplist_experience;
			}
			if (TdirConfig.cheatServerType == TdirServerType.ExpTest)
			{
				return TdirConfig.iplist_experience_test;
			}
			if (TdirConfig.cheatServerType == TdirServerType.TestForTester)
			{
				return TdirConfig.iplist_testForTester;
			}
			if (TdirConfig.cheatServerType == TdirServerType.CompetitionTest)
			{
				return TdirConfig.iplist_competition_test;
			}
			if (TdirConfig.cheatServerType == TdirServerType.CompetitionOfficial)
			{
				return TdirConfig.iplist_competition_official;
			}
			TdirConfig.curServerType = TdirServerType.Normal;
			if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
			{
				return TdirConfig.iplist_normal_tongcai;
			}
			return TdirConfig.iplist_normal;
		}
	}

	public static int[] GetTdirPortList()
	{
		TdirConfigData fileTdirAndTverData = TdirConfig.GetFileTdirAndTverData();
		if (fileTdirAndTverData != null)
		{
			if (fileTdirAndTverData.serverType == 1)
			{
				return TdirConfig.portlist_test;
			}
			if (fileTdirAndTverData.serverType == 2)
			{
				return TdirConfig.portlist_middle;
			}
			if (fileTdirAndTverData.serverType == 3)
			{
				if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
				{
					return TdirConfig.portlist_normal_tongcai;
				}
				return TdirConfig.portlist_normal;
			}
			else
			{
				if (fileTdirAndTverData.serverType == 4)
				{
					return TdirConfig.portlist_experience;
				}
				if (fileTdirAndTverData.serverType == 5)
				{
					return TdirConfig.portlist_experience_test;
				}
				if (fileTdirAndTverData.serverType == 6)
				{
					return TdirConfig.portlist_testForTester;
				}
				if (fileTdirAndTverData.serverType == 7)
				{
					return TdirConfig.portlist_competition_test;
				}
				if (fileTdirAndTverData.serverType == 8)
				{
					return TdirConfig.portlist_competition_official;
				}
			}
		}
		if (TdirConfig.cheatServerType == TdirServerType.Test)
		{
			return TdirConfig.portlist_test;
		}
		if (TdirConfig.cheatServerType == TdirServerType.Mid)
		{
			return TdirConfig.portlist_middle;
		}
		if (TdirConfig.cheatServerType == TdirServerType.Normal)
		{
			if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
			{
				return TdirConfig.portlist_normal_tongcai;
			}
			return TdirConfig.portlist_normal;
		}
		else
		{
			if (TdirConfig.cheatServerType == TdirServerType.Exp)
			{
				return TdirConfig.portlist_experience;
			}
			if (TdirConfig.cheatServerType == TdirServerType.ExpTest)
			{
				return TdirConfig.portlist_experience_test;
			}
			if (TdirConfig.cheatServerType == TdirServerType.TestForTester)
			{
				return TdirConfig.portlist_testForTester;
			}
			if (TdirConfig.cheatServerType == TdirServerType.CompetitionTest)
			{
				return TdirConfig.portlist_competition_test;
			}
			if (TdirConfig.cheatServerType == TdirServerType.CompetitionOfficial)
			{
				return TdirConfig.portlist_competition_official;
			}
			if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
			{
				return TdirConfig.portlist_normal_tongcai;
			}
			return TdirConfig.portlist_normal;
		}
	}

	public static int GetTdirAppId()
	{
		if (TdirConfig.curServerType == TdirServerType.CompetitionTest || TdirConfig.curServerType == TdirServerType.CompetitionOfficial)
		{
			return TdirConfig.AppID_android_competition;
		}
		return TdirConfig.AppID_android;
	}

	public static TdirConfigData GetFileTdirAndTverData()
	{
		if (TdirConfig.tdirConfigData == null && File.Exists(Application.persistentDataPath + TdirConfig.tdirConfigDataPath))
		{
			try
			{
				byte[] array = CFileManager.ReadFile(Application.persistentDataPath + TdirConfig.tdirConfigDataPath);
				if (array != null && array.Length > 0)
				{
					TdirConfig.tdirConfigData = new TdirConfigData();
					string @string = Encoding.get_UTF8().GetString(array);
					SecurityParser securityParser = new SecurityParser();
					securityParser.LoadXml(@string);
					SecurityElement securityElement = securityParser.ToXml();
					using (IEnumerator enumerator = securityElement.get_Children().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SecurityElement securityElement2 = (SecurityElement)enumerator.get_Current();
							if (securityElement2.get_Tag() == "serverType")
							{
								TdirConfig.tdirConfigData.serverType = int.Parse(securityElement2.get_Text());
							}
							else if (securityElement2.get_Tag() == "versionType")
							{
								TdirConfig.tdirConfigData.versionType = int.Parse(securityElement2.get_Text());
							}
						}
					}
				}
			}
			catch (Exception)
			{
				TdirConfig.tdirConfigData = null;
			}
		}
		return TdirConfig.tdirConfigData;
	}
}
