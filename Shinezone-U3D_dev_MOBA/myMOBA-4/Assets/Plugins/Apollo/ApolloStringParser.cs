using System;
using System.Collections.Generic;

namespace Apollo
{
	public class ApolloStringParser
	{
		private Dictionary<string, string> objectCollection = new Dictionary<string, string>();

		public string Source
		{
			get;
			set;
		}

		public ApolloStringParser()
		{
		}

		public ApolloStringParser(string src)
		{
			this.Source = src;
			this.parse(src, this.objectCollection);
		}

		public static string ReplaceApolloString(string src)
		{
			if (src == null)
			{
				return null;
			}
			string text = src.Replace("%26", "&");
			text = text.Replace("%3d", "=");
			return text.Replace("%25", "%");
		}

		public static string ReplaceApolloStringQuto(string src)
		{
			if (src == null)
			{
				return null;
			}
			string text = src.Replace("%26", "&");
			text = text.Replace("%3d", "=");
			text = text.Replace("%25", "%");
			return text.Replace("%2c", ",");
		}

		private void parse(string src, Dictionary<string, string> collection)
		{
			if (src == null || src.Length == 0)
			{
				ADebug.LogError("ApolloStringParser src is null");
				return;
			}
			string[] array = src.Split(new char[]
			{
				'&'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string[] array3 = text.Split(new char[]
				{
					'='
				});
				if (array3.Length > 1)
				{
					if (collection.ContainsKey(array3[0]))
					{
						collection[array3[0]] = array3[1];
					}
					else
					{
						collection.Add(array3[0], array3[1]);
					}
				}
			}
		}

		public T GetObject<T>(string objName) where T : ApolloStruct<T>
		{
			if (objName == null || this.objectCollection.Count == 0)
			{
				return (T)((object)null);
			}
			T result = (T)((object)null);
			if (this.objectCollection.ContainsKey(objName))
			{
				result = (T)((object)Activator.CreateInstance(typeof(T)));
				string src = this.objectCollection[objName];
				src = ApolloStringParser.ReplaceApolloString(src);
				result = (T)((object)result.FromString(src));
			}
			return result;
		}

		public int GetInt(string objName)
		{
			return this.GetInt(objName, 0);
		}

		public int GetInt(string objName, int defaultValue)
		{
			if (objName == null || this.objectCollection.Count == 0)
			{
				return defaultValue;
			}
			if (this.objectCollection.ContainsKey(objName))
			{
				string s = this.objectCollection[objName];
				int result;
				if (int.TryParse(s, out result))
				{
					return result;
				}
			}
			return defaultValue;
		}

		public uint GetUInt32(string objName)
		{
			return this.GetUInt32(objName, 0u);
		}

		public uint GetUInt32(string objName, uint defaultValue)
		{
			if (objName == null || this.objectCollection.Count == 0)
			{
				return defaultValue;
			}
			if (this.objectCollection.ContainsKey(objName))
			{
				string s = this.objectCollection[objName];
				uint result;
				if (uint.TryParse(s, out result))
				{
					return result;
				}
			}
			return defaultValue;
		}

		public ulong GetUInt64(string objName)
		{
			return this.GetUInt64(objName, 0uL);
		}

		public ulong GetUInt64(string objName, ulong defaultValue)
		{
			if (objName == null || this.objectCollection.Count == 0)
			{
				return defaultValue;
			}
			if (this.objectCollection.ContainsKey(objName))
			{
				string s = this.objectCollection[objName];
				ulong result;
				if (ulong.TryParse(s, out result))
				{
					return result;
				}
			}
			return defaultValue;
		}

		public string GetString(string objName)
		{
			if (objName == null || this.objectCollection.Count == 0)
			{
				return null;
			}
			if (this.objectCollection.ContainsKey(objName))
			{
				return this.objectCollection[objName];
			}
			return null;
		}
	}
}
