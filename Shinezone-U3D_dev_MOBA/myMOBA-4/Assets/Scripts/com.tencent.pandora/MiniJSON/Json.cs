using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.tencent.pandora.MiniJSON
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

            //[CompilerGenerated]
            private static Dictionary<string, int> __f__switch_map0;

			private const string WORD_BREAK = "{}[],:\"";

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

            private TOKEN NextToken
{
    get
    {
        this.EatWhitespace();
        if (this.json.Peek() != -1)
        {
            switch (this.PeekChar)
            {
                case '"':
                    return TOKEN.STRING;

                case ',':
                    this.json.Read();
                    return TOKEN.COMMA;

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
                    return TOKEN.NUMBER;

                case ':':
                    return TOKEN.COLON;

                case '[':
                    return TOKEN.SQUARED_OPEN;

                case ']':
                    this.json.Read();
                    return TOKEN.SQUARED_CLOSE;

                case '{':
                    return TOKEN.CURLY_OPEN;

                case '}':
                    this.json.Read();
                    return TOKEN.CURLY_CLOSE;
            }
            string nextWord = this.NextWord;
            if (nextWord != null)
            {
                int num;
                if (__f__switch_map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(3) {
                        { 
                            "false",
                            0
                        },
                        { 
                            "true",
                            1
                        },
                        { 
                            "null",
                            2
                        }
                    };
                    __f__switch_map0 = dictionary;
                }
                if (__f__switch_map0.TryGetValue(nextWord, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return TOKEN.FALSE;

                        case 1:
                            return TOKEN.TRUE;

                        case 2:
                            return TOKEN.NULL;
                    }
                }
            }
        }
        return TOKEN.NONE;
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
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;

                        case TOKEN.CURLY_CLOSE:
                            return dictionary;
                    }
                    if (nextToken != TOKEN.COMMA)
                    {
                        string str = this.ParseString();
                        if (str == null)
                        {
                            return null;
                        }
                        if (this.NextToken != TOKEN.COLON)
                        {
                            return null;
                        }
                        this.json.Read();
                        dictionary[str] = this.ParseValue();
                    }
                }
            }

            private List<object> ParseArray()
            {
                List<object> list = new List<object>();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.SQUARED_CLOSE:
                            {
                                flag = false;
                                continue;
                            }
                        case TOKEN.COMMA:
                            {
                                continue;
                            }
                        case TOKEN.NONE:
                            return null;
                    }
                    object item = this.ParseByToken(nextToken);
                    list.Add(item);
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
                StringBuilder builder = new StringBuilder();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    char[] chArray = null;
                    int num = 0;
                    if (this.json.Peek() == -1)
                    {
                        flag = false;
                        break;
                    }
                    char nextChar = this.NextChar;
                    char ch2 = nextChar;
                    if (ch2 == '"')
                    {
                        flag = false;
                        continue;
                    }
                    if (ch2 != '\\')
                    {
                        goto Label_016F;
                    }
                    if (this.json.Peek() == -1)
                    {
                        flag = false;
                        continue;
                    }
                    nextChar = this.NextChar;
                    char ch3 = nextChar;
                    switch (ch3)
                    {
                        case 'n':
                            {
                                builder.Append('\n');
                                continue;
                            }
                        case 'r':
                            {
                                builder.Append('\r');
                                continue;
                            }
                        case 't':
                            {
                                builder.Append('\t');
                                continue;
                            }
                        case 'u':
                            chArray = new char[4];
                            num = 0;
                            goto Label_0148;

                        default:
                            {
                                if (((ch3 != '"') && (ch3 != '/')) && (ch3 != '\\'))
                                {
                                    if (ch3 == 'b')
                                    {
                                        break;
                                    }
                                    if (ch3 == 'f')
                                    {
                                        goto Label_00F1;
                                    }
                                }
                                else
                                {
                                    builder.Append(nextChar);
                                }
                                continue;
                            }
                    }
                    builder.Append('\b');
                    continue;
                Label_00F1:
                    builder.Append('\f');
                    continue;
                Label_0138:
                    chArray[num] = this.NextChar;
                    num++;
                Label_0148:
                    if (num < 4)
                    {
                        goto Label_0138;
                    }
                    builder.Append((char)Convert.ToInt32(new string(chArray), 0x10));
                    continue;
                Label_016F:
                    builder.Append(nextChar);
                }
                return builder.ToString();
            }


			private object ParseNumber()
			{
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long num;
					long.TryParse(nextWord, out num);
					return num;
				}
				double num2;
				double.TryParse(nextWord, out num2);
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
					this.builder.Append((!(bool)value) ? "false" : "true");
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
				foreach (object current in obj.Keys)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeString(current.ToString());
					this.builder.Append(':');
					this.SerializeValue(obj[current]);
					flag = false;
				}
				this.builder.Append('}');
			}

			private void SerializeArray(IList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				foreach (object current in anArray)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeValue(current);
					flag = false;
				}
				this.builder.Append(']');
			}

            private void SerializeString(string str)
            {
                this.builder.Append('"');
                foreach (char ch in str.ToCharArray())
                {
                    int num2;
                    char ch2 = ch;
                    switch (ch2)
                    {
                        case '\b':
                            {
                                this.builder.Append(@"\b");
                                continue;
                            }
                        case '\t':
                            {
                                this.builder.Append(@"\t");
                                continue;
                            }
                        case '\n':
                            {
                                this.builder.Append(@"\n");
                                continue;
                            }
                        case '\f':
                            {
                                this.builder.Append(@"\f");
                                continue;
                            }
                        case '\r':
                            {
                                this.builder.Append(@"\r");
                                continue;
                            }
                        default:
                            {
                                if (ch2 != '"')
                                {
                                    if (ch2 == '\\')
                                    {
                                        break;
                                    }
                                    goto Label_00F7;
                                }
                                this.builder.Append("\\\"");
                                continue;
                            }
                    }
                    this.builder.Append(@"\\");
                    continue;
                Label_00F7:
                    num2 = Convert.ToInt32(ch);
                    if ((num2 >= 0x20) && (num2 <= 0x7e))
                    {
                        this.builder.Append(ch);
                    }
                    else
                    {
                        this.builder.Append(@"\u");
                        this.builder.Append(num2.ToString("x4"));
                    }
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
