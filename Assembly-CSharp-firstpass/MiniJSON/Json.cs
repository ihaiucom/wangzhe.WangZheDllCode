using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiniJSON
{
	public static class Json
	{
		private sealed class Parser : IDisposable
		{
			private enum TOKEN
			{
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			}

			private const string WORD_BREAK = "{}[],:\"";

			private static Dictionary<string, int> __f__switch_map0;

			private StringReader json;

			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (!Json.Parser.IsWordBreak(this.PeekChar))
					{
						stringBuilder.Append(this.NextChar);
						if (this.json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			private Json.Parser.TOKEN NextToken
			{
				get
				{
					this.EatWhitespace();
					if (this.json.Peek() != -1)
					{
						char peekChar = this.PeekChar;
						switch (peekChar)
						{
						case '"':
							return Json.Parser.TOKEN.STRING;
						case '#':
						case '$':
						case '%':
						case '&':
						case '\'':
						case '(':
						case ')':
						case '*':
						case '+':
						case '.':
						case '/':
							IL_8B:
							switch (peekChar)
							{
							case '[':
								return Json.Parser.TOKEN.SQUARED_OPEN;
							case '\\':
							{
								IL_A0:
								switch (peekChar)
								{
								case '{':
									return Json.Parser.TOKEN.CURLY_OPEN;
								case '}':
									this.json.Read();
									return Json.Parser.TOKEN.CURLY_CLOSE;
								}
								string nextWord = this.NextWord;
								if (nextWord == null)
								{
									return Json.Parser.TOKEN.NONE;
								}
								if (Json.Parser.__f__switch_map0 == null)
								{
									Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
									dictionary.Add("false", 0);
									dictionary.Add("true", 1);
									dictionary.Add("null", 2);
									Dictionary<string, int> _f__switch_map = dictionary;
									Json.Parser.__f__switch_map0 = _f__switch_map;
								}
								int num;
								if (!Json.Parser.__f__switch_map0.TryGetValue(nextWord, ref num))
								{
									return Json.Parser.TOKEN.NONE;
								}
								switch (num)
								{
								case 0:
									return Json.Parser.TOKEN.FALSE;
								case 1:
									return Json.Parser.TOKEN.TRUE;
								case 2:
									return Json.Parser.TOKEN.NULL;
								default:
									return Json.Parser.TOKEN.NONE;
								}
								break;
							}
							case ']':
								this.json.Read();
								return Json.Parser.TOKEN.SQUARED_CLOSE;
							}
							goto IL_A0;
						case ',':
							this.json.Read();
							return Json.Parser.TOKEN.COMMA;
						case '-':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							return Json.Parser.TOKEN.NUMBER;
						case ':':
							return Json.Parser.TOKEN.COLON;
						}
						goto IL_8B;
					}
					return Json.Parser.TOKEN.NONE;
				}
			}

			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			public static bool IsWordBreak(char c)
			{
				return char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;
			}

			public static object Parse(string jsonString)
			{
				object result;
				using (Json.Parser parser = new Json.Parser(jsonString))
				{
					result = parser.ParseValue();
				}
				return result;
			}

			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				this.json.Read();
				while (true)
				{
					Json.Parser.TOKEN nextToken = this.NextToken;
					switch (nextToken)
					{
					case Json.Parser.TOKEN.NONE:
						goto IL_32;
					case Json.Parser.TOKEN.CURLY_CLOSE:
						return dictionary;
					}
					if (nextToken != Json.Parser.TOKEN.COMMA)
					{
						string text = this.ParseString();
						if (text == null)
						{
							goto Block_3;
						}
						if (this.NextToken != Json.Parser.TOKEN.COLON)
						{
							goto Block_4;
						}
						this.json.Read();
						dictionary.set_Item(text, this.ParseValue());
					}
				}
				IL_32:
				return null;
				Block_3:
				return null;
				Block_4:
				return null;
			}

			private List<object> ParseArray()
			{
				List<object> list = new List<object>();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					Json.Parser.TOKEN nextToken = this.NextToken;
					Json.Parser.TOKEN tOKEN = nextToken;
					switch (tOKEN)
					{
					case Json.Parser.TOKEN.SQUARED_CLOSE:
						flag = false;
						continue;
					case Json.Parser.TOKEN.COLON:
						IL_38:
						if (tOKEN != Json.Parser.TOKEN.NONE)
						{
							object obj = this.ParseByToken(nextToken);
							list.Add(obj);
							continue;
						}
						return null;
					case Json.Parser.TOKEN.COMMA:
						continue;
					}
					goto IL_38;
				}
				return list;
			}

			private object ParseValue()
			{
				Json.Parser.TOKEN nextToken = this.NextToken;
				return this.ParseByToken(nextToken);
			}

			private object ParseByToken(Json.Parser.TOKEN token)
			{
				switch (token)
				{
				case Json.Parser.TOKEN.CURLY_OPEN:
					return this.ParseObject();
				case Json.Parser.TOKEN.SQUARED_OPEN:
					return this.ParseArray();
				case Json.Parser.TOKEN.STRING:
					return this.ParseString();
				case Json.Parser.TOKEN.NUMBER:
					return this.ParseNumber();
				case Json.Parser.TOKEN.TRUE:
					return true;
				case Json.Parser.TOKEN.FALSE:
					return false;
				case Json.Parser.TOKEN.NULL:
					return null;
				}
				return null;
			}

			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() == -1)
					{
						break;
					}
					char nextChar = this.NextChar;
					char c = nextChar;
					if (c == '"')
					{
						flag = false;
					}
					else if (c != '\\')
					{
						stringBuilder.Append(nextChar);
					}
					else if (this.json.Peek() == -1)
					{
						flag = false;
					}
					else
					{
						nextChar = this.NextChar;
						char c2 = nextChar;
						switch (c2)
						{
						case 'n':
							stringBuilder.Append('\n');
							continue;
						case 'r':
							stringBuilder.Append('\r');
							continue;
						case 't':
							stringBuilder.Append('\t');
							continue;
						case 'u':
						{
							char[] array = new char[4];
							for (int i = 0; i < 4; i++)
							{
								array[i] = this.NextChar;
							}
							stringBuilder.Append((char)Convert.ToInt32(new string(array), 16));
							continue;
						}
						}
						if (c2 != '"' && c2 != '/' && c2 != '\\')
						{
							if (c2 == 'b')
							{
								stringBuilder.Append('\b');
							}
							else if (c2 == 'f')
							{
								stringBuilder.Append('\f');
							}
						}
						else
						{
							stringBuilder.Append(nextChar);
						}
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseNumber()
			{
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long num;
					long.TryParse(nextWord, ref num);
					return num;
				}
				double num2;
				double.TryParse(nextWord, ref num2);
				return num2;
			}

			private void EatWhitespace()
			{
				while (char.IsWhiteSpace(this.PeekChar))
				{
					this.json.Read();
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
			}
		}

		private sealed class Serializer
		{
			private StringBuilder builder;

			private Serializer()
			{
				this.builder = new StringBuilder();
			}

			public static string Serialize(object obj)
			{
				Json.Serializer serializer = new Json.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeValue(object value)
			{
				string str;
				IList anArray;
				IDictionary obj;
				if (value == null)
				{
					this.builder.Append("null");
				}
				else if ((str = (value as string)) != null)
				{
					this.SerializeString(str);
				}
				else if (value is bool)
				{
					this.builder.Append(((bool)value) ? "true" : "false");
				}
				else if ((anArray = (value as IList)) != null)
				{
					this.SerializeArray(anArray);
				}
				else if ((obj = (value as IDictionary)) != null)
				{
					this.SerializeObject(obj);
				}
				else if (value is char)
				{
					this.SerializeString(new string((char)value, 1));
				}
				else
				{
					this.SerializeOther(value);
				}
			}

			private void SerializeObject(IDictionary obj)
			{
				bool flag = true;
				this.builder.Append('{');
				using (IEnumerator enumerator = obj.get_Keys().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.get_Current();
						if (!flag)
						{
							this.builder.Append(',');
						}
						this.SerializeString(current.ToString());
						this.builder.Append(':');
						this.SerializeValue(obj.get_Item(current));
						flag = false;
					}
				}
				this.builder.Append('}');
			}

			private void SerializeArray(IList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				using (IEnumerator enumerator = anArray.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.get_Current();
						if (!flag)
						{
							this.builder.Append(',');
						}
						this.SerializeValue(current);
						flag = false;
					}
				}
				this.builder.Append(']');
			}

			private void SerializeString(string str)
			{
				this.builder.Append('"');
				char[] array = str.ToCharArray();
				int i = 0;
				while (i < array.Length)
				{
					char c = array[i];
					char c2 = c;
					switch (c2)
					{
					case '\b':
						this.builder.Append("\\b");
						break;
					case '\t':
						this.builder.Append("\\t");
						break;
					case '\n':
						this.builder.Append("\\n");
						break;
					case '\v':
						goto IL_BB;
					case '\f':
						this.builder.Append("\\f");
						break;
					case '\r':
						this.builder.Append("\\r");
						break;
					default:
						goto IL_BB;
					}
					IL_155:
					i++;
					continue;
					IL_BB:
					if (c2 == '"')
					{
						this.builder.Append("\\\"");
						goto IL_155;
					}
					if (c2 == '\\')
					{
						this.builder.Append("\\\\");
						goto IL_155;
					}
					int num = Convert.ToInt32(c);
					if (num >= 32 && num <= 126)
					{
						this.builder.Append(c);
						goto IL_155;
					}
					this.builder.Append("\\u");
					this.builder.Append(num.ToString("x4"));
					goto IL_155;
				}
				this.builder.Append('"');
			}

			private void SerializeOther(object value)
			{
				if (value is float)
				{
					this.builder.Append(((float)value).ToString("R"));
				}
				else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					this.builder.Append(value);
				}
				else if (value is double || value is decimal)
				{
					this.builder.Append(Convert.ToDouble(value).ToString("R"));
				}
				else
				{
					this.SerializeString(value.ToString());
				}
			}
		}

		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return Json.Parser.Parse(json);
		}

		public static string Serialize(object obj)
		{
			return Json.Serializer.Serialize(obj);
		}
	}
}
