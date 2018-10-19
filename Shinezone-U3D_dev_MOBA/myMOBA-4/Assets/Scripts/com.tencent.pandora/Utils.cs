using com.tencent.pandora.MiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace com.tencent.pandora
{
	public class Utils
	{
		private static int cacheVersion;

		public static int GetCacheVersion()
		{
			if (Utils.cacheVersion == 0)
			{
				Utils.cacheVersion = Utils.NowSeconds();
			}
			return Utils.cacheVersion;
		}

		public static void ResetCacheVersion()
		{
			Utils.cacheVersion = 0;
		}

		public static int NowSeconds()
		{
			return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}

		public static bool IsLuaAssetBundle(string assetBundleName)
		{
			return assetBundleName.EndsWith("_lua.assetbundle");
		}

		public static string GetBaseAtlasAssetBundleName()
		{
			return Utils.GetPlatformDesc() + "__baseAtlas.assetbundle";
		}

		public static string ExtractLuaName(string luaAssetBundleName)
		{
			Logger.DEBUG(luaAssetBundleName);
			string platformDesc = Utils.GetPlatformDesc();
			string text = luaAssetBundleName.Replace(platformDesc + "_", string.Empty);
			Logger.DEBUG(text);
			text = text.Replace("_lua.assetbundle", string.Empty);
			Logger.DEBUG(text);
			return text;
		}

		public static string GetPlatformDesc()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				result = "pc";
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				result = "android";
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = "mac";
			}
			else
			{
				result = "ios";
			}
			return result;
		}

		public static string GetBundleName(string moduleName)
		{
			return Utils.GetPlatformDesc() + "_" + moduleName + ".assetbundle";
		}

		public static bool ParseConfigData(Dictionary<string, object> content, ref Dictionary<string, object> result)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				int num = 0;
				if (content.ContainsKey("id"))
				{
					num = Convert.ToInt32(content["id"]);
				}
				int num2 = -1;
				if (content.ContainsKey("totalSwitch"))
				{
					num2 = Convert.ToInt32(content["totalSwitch"]);
				}
				bool result2;
				if (num2 == 0)
				{
					result["ruleId"] = num;
					result["totalSwitch"] = false;
					result2 = true;
					return result2;
				}
				if (num2 != 1)
				{
					result2 = false;
					return result2;
				}
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				if (content.ContainsKey("function_switch"))
				{
					string text = content["function_switch"] as string;
					string[] array = text.Split(new char[]
					{
						','
					});
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text2 = array2[i];
						string[] array3 = text2.Split(new char[]
						{
							':'
						});
						if (array3.Length == 2)
						{
							string key = array3[0];
							int num3 = Convert.ToInt32(array3[1]);
							dictionary[key] = (num3 == 1);
						}
					}
				}
				int num4 = 0;
				if (content.ContainsKey("isDebug"))
				{
					num4 = Convert.ToInt32(content["isDebug"]);
				}
				int num5 = 1;
				if (content.ContainsKey("isNetLog"))
				{
					num5 = Convert.ToInt32(content["isNetLog"]);
				}
				string text3 = string.Empty;
				string text4 = string.Empty;
				string text5 = string.Empty;
				ushort num6 = 0;
				if (content.ContainsKey("ip"))
				{
					text3 = Convert.ToString(content["ip"]);
				}
				if (content.ContainsKey("port"))
				{
					num6 = Convert.ToUInt16(content["port"]);
				}
				if (content.ContainsKey("cap_ip1"))
				{
					text4 = Convert.ToString(content["cap_ip1"]);
				}
				if (content.ContainsKey("cap_ip2"))
				{
					text5 = Convert.ToString(content["cap_ip2"]);
				}
				if ((text3.Length == 0 && text4.Length == 0 && text5.Length == 0) || num6 == 0)
				{
					result2 = false;
					return result2;
				}
				int num7 = -1;
				Dictionary<string, List<PandoraImpl.FileState>> dictionary2 = new Dictionary<string, List<PandoraImpl.FileState>>();
				List<string> list = new List<string>();
				HashSet<string> hashSet = new HashSet<string>();
				if (content.ContainsKey("dependency"))
				{
					string text6 = content["dependency"] as string;
					string[] array4 = text6.Split(new char[]
					{
						'|'
					});
					num7 = array4.Length;
					string[] array5 = array4;
					for (int j = 0; j < array5.Length; j++)
					{
						string text7 = array5[j];
						string[] array6 = text7.Split(new char[]
						{
							':'
						});
						if (array6.Length == 2)
						{
							string key2 = array6[0];
							string text8 = array6[1];
							string[] array7 = text8.Split(new char[]
							{
								','
							});
							List<PandoraImpl.FileState> list2 = new List<PandoraImpl.FileState>();
							string[] array8 = array7;
							for (int k = 0; k < array8.Length; k++)
							{
								string text9 = array8[k];
								if (text9.StartsWith("@"))
								{
									string key3 = text9.Substring(1);
									if (!dictionary2.ContainsKey(key3))
									{
										result2 = false;
										return result2;
									}
									list2.AddRange(dictionary2[key3]);
								}
								else
								{
									list2.Add(new PandoraImpl.FileState
									{
										name = text9
									});
									if (!hashSet.Contains(text9))
									{
										list.Add(text9);
										hashSet.Add(text9);
									}
								}
							}
							dictionary2[key2] = list2;
						}
					}
				}
				if (num7 <= 0 || num7 != dictionary2.Count)
				{
					result2 = false;
					return result2;
				}
				int num8 = -1;
				List<PandoraImpl.DownloadASTask> list3 = new List<PandoraImpl.DownloadASTask>();
				if (content.ContainsKey("sourcelist"))
				{
					Dictionary<string, object> dictionary3 = content["sourcelist"] as Dictionary<string, object>;
					if (dictionary3 != null && dictionary3.ContainsKey("count") && dictionary3.ContainsKey("list"))
					{
						int num9 = Convert.ToInt32(dictionary3["count"]);
						List<object> list4 = dictionary3["list"] as List<object>;
						if (num9 == list4.Count)
						{
							num8 = num9;
							HashSet<string> hashSet2 = new HashSet<string>();
							foreach (object current in list4)
							{
								Dictionary<string, object> dictionary4 = current as Dictionary<string, object>;
								if (dictionary4.ContainsKey("url") && dictionary4.ContainsKey("luacmd5") && dictionary4.ContainsKey("size"))
								{
									PandoraImpl.DownloadASTask downloadASTask = new PandoraImpl.DownloadASTask();
									downloadASTask.url = (dictionary4["url"] as string);
									downloadASTask.size = (int)((long)dictionary4["size"]);
									downloadASTask.md5 = (dictionary4["luacmd5"] as string);
									downloadASTask.name = Path.GetFileName(downloadASTask.url);
									if (downloadASTask.name != null && downloadASTask.name.Length > 0 && downloadASTask.md5.Length > 0 && downloadASTask.size > 0 && !hashSet2.Contains(downloadASTask.name))
									{
										list3.Add(downloadASTask);
										hashSet2.Add(downloadASTask.name);
									}
								}
							}
						}
					}
					else
					{
						num8 = 0;
					}
				}
				if (num8 < 0 || num8 != list3.Count)
				{
					result2 = false;
					return result2;
				}
				result["ruleId"] = num;
				result["totalSwitch"] = true;
				result["isDebug"] = (num4 == 1);
				result["isNetLog"] = (num5 == 1);
				result["brokerHost"] = text3;
				result["brokerPort"] = num6;
				result["brokerAltIp1"] = text4;
				result["brokerAltIp2"] = text5;
				result["functionSwitches"] = dictionary;
				result["dependencyInfos"] = dictionary2;
				result["dependencyAll"] = list;
				result["pendingDownloadASTasks"] = list3;
				result2 = true;
				return result2;
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
			return true;
		}

		public static bool WriteCookie(string fileName, string content)
		{
			bool result;
			try
			{
				string path = Pandora.Instance.GetCookiePath() + "/" + fileName;
				File.WriteAllText(path, content);
				result = true;
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
				result = false;
			}
			return result;
		}

		public static string ReadCookie(string fileName)
		{
			string result;
			try
			{
				string path = Pandora.Instance.GetCookiePath() + "/" + fileName;
				result = File.ReadAllText(path);
			}
			catch (Exception ex)
			{
				Logger.WARN(ex.Message);
				result = string.Empty;
			}
			return result;
		}

		public static string GetFileName(string filePath)
		{
			string result;
			try
			{
				string fileName = Path.GetFileName(filePath);
				if (fileName == null)
				{
					result = string.Empty;
				}
				else
				{
					result = fileName;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
				result = string.Empty;
			}
			return result;
		}

		public static byte[] ReadFileBytes(string filePath)
		{
			byte[] result;
			try
			{
				byte[] array = File.ReadAllBytes(filePath);
				if (array == null)
				{
					result = new byte[0];
				}
				else
				{
					result = array;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
				result = new byte[0];
			}
			return result;
		}

		public static Dictionary<string, IPHostEntry> LoadDnsCacheFromFile()
		{
			Dictionary<string, IPHostEntry> dictionary = new Dictionary<string, IPHostEntry>();
			string dnsCacheFileName = Utils.GetDnsCacheFileName();
			try
			{
				if (File.Exists(dnsCacheFileName))
				{
					string text = File.ReadAllText(dnsCacheFileName);
					Logger.DEBUG("Dns cache: " + text);
					Dictionary<string, object> dictionary2 = Json.Deserialize(text) as Dictionary<string, object>;
					if (dictionary2 != null)
					{
						foreach (KeyValuePair<string, object> current in dictionary2)
						{
							List<object> list = current.Value as List<object>;
							if (list != null)
							{
								List<IPAddress> list2 = new List<IPAddress>();
								foreach (object current2 in list)
								{
									IPAddress item = null;
									if (IPAddress.TryParse(current2 as string, out item))
									{
										list2.Add(item);
									}
								}
								if (list2.Count > 0)
								{
									IPHostEntry iPHostEntry = new IPHostEntry();
									iPHostEntry.AddressList = list2.ToArray();
									dictionary[current.Key] = iPHostEntry;
								}
							}
						}
					}
				}
				else
				{
					Logger.DEBUG("Dns cache file isn't exist");
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
			return dictionary;
		}

		public static void SaveDnsCacheToFile(Dictionary<string, IPHostEntry> dnsCache)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (KeyValuePair<string, IPHostEntry> current in dnsCache)
			{
				if (current.Value.AddressList.Length > 0)
				{
					List<string> list = new List<string>();
					IPAddress[] addressList = current.Value.AddressList;
					for (int i = 0; i < addressList.Length; i++)
					{
						IPAddress iPAddress = addressList[i];
						list.Add(iPAddress.ToString());
					}
					dictionary[current.Key] = list;
				}
			}
			string dnsCacheFileName = Utils.GetDnsCacheFileName();
			try
			{
				string text = Json.Serialize(dictionary);
				Logger.DEBUG(text);
				File.WriteAllText(dnsCacheFileName, text);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		private static string GetDnsCacheFileName()
		{
			return Pandora.Instance.GetCachePath() + "/dns_cache.json";
		}

		public static void doDebugMem(string tag)
		{
			string memStr = Utils.getMemStr(tag);
			Logger.INFO(memStr);
		}

		public static string getMemStr(string tag)
		{
			IntPtr clazz = AndroidJNI.FindClass("com/tencent/msdk/u3d/DebugMemInfo");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getmem_result", "(Ljava/lang/String;)[B");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(tag);
			IntPtr array2 = AndroidJNI.CallStaticObjectMethod(clazz, staticMethodID, array);
			byte[] bytes = AndroidJNI.FromByteArray(array2);
			return Encoding.Default.GetString(bytes);
		}
	}
}
