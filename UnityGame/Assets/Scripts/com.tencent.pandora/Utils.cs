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
			return (int)DateTime.get_UtcNow().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).get_TotalSeconds();
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
					num = Convert.ToInt32(content.get_Item("id"));
				}
				int num2 = -1;
				if (content.ContainsKey("totalSwitch"))
				{
					num2 = Convert.ToInt32(content.get_Item("totalSwitch"));
				}
				bool flag;
				bool result2;
				if (num2 == 0)
				{
					result.set_Item("ruleId", num);
					result.set_Item("totalSwitch", false);
					flag = true;
					result2 = flag;
					return result2;
				}
				if (num2 != 1)
				{
					flag = false;
					result2 = flag;
					return result2;
				}
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				if (content.ContainsKey("function_switch"))
				{
					string text = content.get_Item("function_switch") as string;
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
							string text3 = array3[0];
							int num3 = Convert.ToInt32(array3[1]);
							dictionary.set_Item(text3, num3 == 1);
						}
					}
				}
				int num4 = 0;
				if (content.ContainsKey("isDebug"))
				{
					num4 = Convert.ToInt32(content.get_Item("isDebug"));
				}
				int num5 = 1;
				if (content.ContainsKey("isNetLog"))
				{
					num5 = Convert.ToInt32(content.get_Item("isNetLog"));
				}
				string text4 = string.Empty;
				string text5 = string.Empty;
				string text6 = string.Empty;
				ushort num6 = 0;
				if (content.ContainsKey("ip"))
				{
					text4 = Convert.ToString(content.get_Item("ip"));
				}
				if (content.ContainsKey("port"))
				{
					num6 = Convert.ToUInt16(content.get_Item("port"));
				}
				if (content.ContainsKey("cap_ip1"))
				{
					text5 = Convert.ToString(content.get_Item("cap_ip1"));
				}
				if (content.ContainsKey("cap_ip2"))
				{
					text6 = Convert.ToString(content.get_Item("cap_ip2"));
				}
				if ((text4.get_Length() == 0 && text5.get_Length() == 0 && text6.get_Length() == 0) || num6 == 0)
				{
					flag = false;
					result2 = flag;
					return result2;
				}
				int num7 = -1;
				Dictionary<string, List<PandoraImpl.FileState>> dictionary2 = new Dictionary<string, List<PandoraImpl.FileState>>();
				List<string> list = new List<string>();
				HashSet<string> hashSet = new HashSet<string>();
				if (content.ContainsKey("dependency"))
				{
					string text7 = content.get_Item("dependency") as string;
					string[] array4 = text7.Split(new char[]
					{
						'|'
					});
					num7 = array4.Length;
					string[] array5 = array4;
					for (int j = 0; j < array5.Length; j++)
					{
						string text8 = array5[j];
						string[] array6 = text8.Split(new char[]
						{
							':'
						});
						if (array6.Length == 2)
						{
							string text9 = array6[0];
							string text10 = array6[1];
							string[] array7 = text10.Split(new char[]
							{
								','
							});
							List<PandoraImpl.FileState> list2 = new List<PandoraImpl.FileState>();
							string[] array8 = array7;
							for (int k = 0; k < array8.Length; k++)
							{
								string text11 = array8[k];
								if (text11.StartsWith("@"))
								{
									string text12 = text11.Substring(1);
									if (!dictionary2.ContainsKey(text12))
									{
										flag = false;
										result2 = flag;
										return result2;
									}
									list2.AddRange(dictionary2.get_Item(text12));
								}
								else
								{
									list2.Add(new PandoraImpl.FileState
									{
										name = text11
									});
									if (!hashSet.Contains(text11))
									{
										list.Add(text11);
										hashSet.Add(text11);
									}
								}
							}
							dictionary2.set_Item(text9, list2);
						}
					}
				}
				if (num7 <= 0 || num7 != dictionary2.get_Count())
				{
					flag = false;
					result2 = flag;
					return result2;
				}
				int num8 = -1;
				List<PandoraImpl.DownloadASTask> list3 = new List<PandoraImpl.DownloadASTask>();
				if (content.ContainsKey("sourcelist"))
				{
					Dictionary<string, object> dictionary3 = content.get_Item("sourcelist") as Dictionary<string, object>;
					if (dictionary3 != null && dictionary3.ContainsKey("count") && dictionary3.ContainsKey("list"))
					{
						int num9 = Convert.ToInt32(dictionary3.get_Item("count"));
						List<object> list4 = dictionary3.get_Item("list") as List<object>;
						if (num9 == list4.get_Count())
						{
							num8 = num9;
							HashSet<string> hashSet2 = new HashSet<string>();
							using (List<object>.Enumerator enumerator = list4.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									object current = enumerator.get_Current();
									Dictionary<string, object> dictionary4 = current as Dictionary<string, object>;
									if (dictionary4.ContainsKey("url") && dictionary4.ContainsKey("luacmd5") && dictionary4.ContainsKey("size"))
									{
										PandoraImpl.DownloadASTask downloadASTask = new PandoraImpl.DownloadASTask();
										downloadASTask.url = (dictionary4.get_Item("url") as string);
										downloadASTask.size = (int)((long)dictionary4.get_Item("size"));
										downloadASTask.md5 = (dictionary4.get_Item("luacmd5") as string);
										downloadASTask.name = Path.GetFileName(downloadASTask.url);
										if (downloadASTask.name != null && downloadASTask.name.get_Length() > 0 && downloadASTask.md5.get_Length() > 0 && downloadASTask.size > 0 && !hashSet2.Contains(downloadASTask.name))
										{
											list3.Add(downloadASTask);
											hashSet2.Add(downloadASTask.name);
										}
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
				if (num8 < 0 || num8 != list3.get_Count())
				{
					flag = false;
					result2 = flag;
					return result2;
				}
				result.set_Item("ruleId", num);
				result.set_Item("totalSwitch", true);
				result.set_Item("isDebug", num4 == 1);
				result.set_Item("isNetLog", num5 == 1);
				result.set_Item("brokerHost", text4);
				result.set_Item("brokerPort", num6);
				result.set_Item("brokerAltIp1", text5);
				result.set_Item("brokerAltIp2", text6);
				result.set_Item("functionSwitches", dictionary);
				result.set_Item("dependencyInfos", dictionary2);
				result.set_Item("dependencyAll", list);
				result.set_Item("pendingDownloadASTasks", list3);
				flag = true;
				result2 = flag;
				return result2;
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
			return true;
		}

		public static bool WriteCookie(string fileName, string content)
		{
			bool result;
			try
			{
				string text = Pandora.Instance.GetCookiePath() + "/" + fileName;
				File.WriteAllText(text, content);
				result = true;
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
				result = false;
			}
			return result;
		}

		public static string ReadCookie(string fileName)
		{
			string result;
			try
			{
				string text = Pandora.Instance.GetCookiePath() + "/" + fileName;
				result = File.ReadAllText(text);
			}
			catch (Exception ex)
			{
				Logger.WARN(ex.get_Message());
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
				Logger.ERROR(ex.get_StackTrace());
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
				Logger.ERROR(ex.get_StackTrace());
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
						using (Dictionary<string, object>.Enumerator enumerator = dictionary2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<string, object> current = enumerator.get_Current();
								List<object> list = current.get_Value() as List<object>;
								if (list != null)
								{
									List<IPAddress> list2 = new List<IPAddress>();
									using (List<object>.Enumerator enumerator2 = list.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											object current2 = enumerator2.get_Current();
											IPAddress iPAddress = null;
											if (IPAddress.TryParse(current2 as string, ref iPAddress))
											{
												list2.Add(iPAddress);
											}
										}
									}
									if (list2.get_Count() > 0)
									{
										IPHostEntry iPHostEntry = new IPHostEntry();
										iPHostEntry.set_AddressList(list2.ToArray());
										dictionary.set_Item(current.get_Key(), iPHostEntry);
									}
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
				Logger.ERROR(ex.get_StackTrace());
			}
			return dictionary;
		}

		public static void SaveDnsCacheToFile(Dictionary<string, IPHostEntry> dnsCache)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			using (Dictionary<string, IPHostEntry>.Enumerator enumerator = dnsCache.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, IPHostEntry> current = enumerator.get_Current();
					if (current.get_Value().get_AddressList().Length > 0)
					{
						List<string> list = new List<string>();
						IPAddress[] addressList = current.get_Value().get_AddressList();
						for (int i = 0; i < addressList.Length; i++)
						{
							IPAddress iPAddress = addressList[i];
							list.Add(iPAddress.ToString());
						}
						dictionary.set_Item(current.get_Key(), list);
					}
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
				Logger.ERROR(ex.get_StackTrace());
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
			byte[] array3 = AndroidJNI.FromByteArray(array2);
			return Encoding.get_Default().GetString(array3);
		}
	}
}
