using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mono.Xml
{
	public class SmallXmlParser
	{
		public interface IContentHandler
		{
			void OnStartParsing(SmallXmlParser parser);

			void OnEndParsing(SmallXmlParser parser);

			void OnStartElement(string name, SmallXmlParser.IAttrList attrs);

			void OnEndElement(string name);

			void OnProcessingInstruction(string name, string text);

			void OnChars(string text);

			void OnIgnorableWhitespace(string text);
		}

		public interface IAttrList
		{
			int Length
			{
				get;
			}

			bool IsEmpty
			{
				get;
			}

			string[] Names
			{
				get;
			}

			string[] Values
			{
				get;
			}

			string GetName(int i);

			string GetValue(int i);

			string GetValue(string name);
		}

		private class AttrListImpl : SmallXmlParser.IAttrList
		{
			private ArrayList attrNames = new ArrayList();

			private ArrayList attrValues = new ArrayList();

			public int Length
			{
				get
				{
					return this.attrNames.Count;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.attrNames.Count == 0;
				}
			}

			public string[] Names
			{
				get
				{
					return (string[])this.attrNames.ToArray(typeof(string));
				}
			}

			public string[] Values
			{
				get
				{
					return (string[])this.attrValues.ToArray(typeof(string));
				}
			}

			public string GetName(int i)
			{
				return (string)this.attrNames[i];
			}

			public string GetValue(int i)
			{
				return (string)this.attrValues[i];
			}

			public string GetValue(string name)
			{
				for (int i = 0; i < this.attrNames.Count; i++)
				{
					if ((string)this.attrNames[i] == name)
					{
						return (string)this.attrValues[i];
					}
				}
				return null;
			}

			internal void Clear()
			{
				this.attrNames.Clear();
				this.attrValues.Clear();
			}

			internal void Add(string name, string value)
			{
				this.attrNames.Add(name);
				this.attrValues.Add(value);
			}
		}

		private SmallXmlParser.IContentHandler handler;

		private TextReader reader;

		private Stack elementNames = new Stack();

		private Stack xmlSpaces = new Stack();

		private string xmlSpace;

		private StringBuilder buffer = new StringBuilder(200);

		private char[] nameBuffer = new char[30];

		private bool isWhitespace;

		private SmallXmlParser.AttrListImpl attributes = new SmallXmlParser.AttrListImpl();

		private int line = 1;

		private int column;

		private bool resetColumn;

		private Exception Error(string msg)
		{
			return new SmallXmlParserException(msg, this.line, this.column);
		}

		private Exception UnexpectedEndError()
		{
			string[] array = new string[this.elementNames.Count];
			((ICollection)this.elementNames).CopyTo(array, 0);
			return this.Error(string.Format("Unexpected end of stream. Element stack content is {0}", string.Join(",", array)));
		}

		private bool IsNameChar(char c, bool start)
		{
			if (c == '-' || c == '.')
			{
				return !start;
			}
			if (c == ':' || c == '_')
			{
				return true;
			}
			if (c > 'Ā')
			{
				if (c == 'ۥ' || c == 'ۦ' || c == 'ՙ')
				{
					return true;
				}
				if ('ʻ' <= c && c <= 'ˁ')
				{
					return true;
				}
			}
			switch (char.GetUnicodeCategory(c))
			{
			case UnicodeCategory.UppercaseLetter:
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.TitlecaseLetter:
			case UnicodeCategory.OtherLetter:
			case UnicodeCategory.LetterNumber:
				return true;
			case UnicodeCategory.ModifierLetter:
			case UnicodeCategory.NonSpacingMark:
			case UnicodeCategory.SpacingCombiningMark:
			case UnicodeCategory.EnclosingMark:
			case UnicodeCategory.DecimalDigitNumber:
				return !start;
			default:
				return false;
			}
		}

        private bool IsWhitespace(int c)
        {
            switch (c)
            {
                case 9:
                case 10:
                case 13:
                case 0x20:
                    return true;
            }
            return false;
        }


		public void SkipWhitespaces()
		{
			this.SkipWhitespaces(false);
		}

		private void HandleWhitespaces()
		{
			while (this.IsWhitespace(this.Peek()))
			{
				this.buffer.Append((char)this.Read());
			}
			if (this.Peek() != 60 && this.Peek() >= 0)
			{
				this.isWhitespace = false;
			}
		}

        public void SkipWhitespaces(bool expected)
        {
        Label_0000:
            switch (this.Peek())
            {
                case 9:
                case 10:
                case 13:
                case 0x20:
                    this.Read();
                    if (expected)
                    {
                        expected = false;
                    }
                    goto Label_0000;
            }
            if (expected)
            {
                throw this.Error("Whitespace is expected.");
            }
        }


		private int Peek()
		{
			return this.reader.Peek();
		}

		private int Read()
		{
			int num = this.reader.Read();
			if (num == 10)
			{
				this.resetColumn = true;
			}
			if (this.resetColumn)
			{
				this.line++;
				this.resetColumn = false;
				this.column = 1;
			}
			else
			{
				this.column++;
			}
			return num;
		}

		public void Expect(int c)
		{
			int num = this.Read();
			if (num < 0)
			{
				throw this.UnexpectedEndError();
			}
			if (num != c)
			{
				throw this.Error(string.Format("Expected '{0}' but got {1}", (char)c, (char)num));
			}
		}

		private string ReadUntil(char until, bool handleReferences)
		{
			while (this.Peek() >= 0)
			{
				char c = (char)this.Read();
				if (c == until)
				{
					string result = this.buffer.ToString();
					this.buffer.Length = 0;
					return result;
				}
				if (handleReferences && c == '&')
				{
					this.ReadReference();
				}
				else
				{
					this.buffer.Append(c);
				}
			}
			throw this.UnexpectedEndError();
		}

		public string ReadName()
		{
			int num = 0;
			if (this.Peek() < 0 || !this.IsNameChar((char)this.Peek(), true))
			{
				throw this.Error("XML name start character is expected.");
			}
			for (int i = this.Peek(); i >= 0; i = this.Peek())
			{
				char c = (char)i;
				if (!this.IsNameChar(c, false))
				{
					break;
				}
				if (num == this.nameBuffer.Length)
				{
					char[] destinationArray = new char[num * 2];
					Array.Copy(this.nameBuffer, 0, destinationArray, 0, num);
					this.nameBuffer = destinationArray;
				}
				this.nameBuffer[num++] = c;
				this.Read();
			}
			if (num == 0)
			{
				throw this.Error("Valid XML name is expected.");
			}
			return new string(this.nameBuffer, 0, num);
		}

		public void Parse(TextReader input, SmallXmlParser.IContentHandler handler)
		{
			this.reader = input;
			this.handler = handler;
			handler.OnStartParsing(this);
			while (this.Peek() >= 0)
			{
				this.ReadContent();
			}
			this.HandleBufferedContent();
			if (this.elementNames.Count > 0)
			{
				throw this.Error(string.Format("Insufficient close tag: {0}", this.elementNames.Peek()));
			}
			handler.OnEndParsing(this);
			this.Cleanup();
		}

		private void Cleanup()
		{
			this.line = 1;
			this.column = 0;
			this.handler = null;
			this.reader = null;
			this.elementNames.Clear();
			this.xmlSpaces.Clear();
			this.attributes.Clear();
			this.buffer.Length = 0;
			this.xmlSpace = null;
			this.isWhitespace = false;
		}

		public void ReadContent()
		{
			if (this.IsWhitespace(this.Peek()))
			{
				if (this.buffer.Length == 0)
				{
					this.isWhitespace = true;
				}
				this.HandleWhitespaces();
			}
			if (this.Peek() != 60)
			{
				this.ReadCharacters();
				return;
			}
			this.Read();
			int num = this.Peek();
			if (num != 33)
			{
				if (num != 47)
				{
					string text;
					if (num != 63)
					{
						this.HandleBufferedContent();
						text = this.ReadName();
						while (this.Peek() != 62 && this.Peek() != 47)
						{
							this.ReadAttribute(this.attributes);
						}
						this.handler.OnStartElement(text, this.attributes);
						this.attributes.Clear();
						this.SkipWhitespaces();
						if (this.Peek() == 47)
						{
							this.Read();
							this.handler.OnEndElement(text);
						}
						else
						{
							this.elementNames.Push(text);
							this.xmlSpaces.Push(this.xmlSpace);
						}
						this.Expect(62);
						return;
					}
					this.HandleBufferedContent();
					this.Read();
					text = this.ReadName();
					this.SkipWhitespaces();
					string text2 = string.Empty;
					if (this.Peek() != 63)
					{
						while (true)
						{
							text2 += this.ReadUntil('?', false);
							if (this.Peek() == 62)
							{
								break;
							}
							text2 += "?";
						}
					}
					this.handler.OnProcessingInstruction(text, text2);
					this.Expect(62);
					return;
				}
				else
				{
					this.HandleBufferedContent();
					if (this.elementNames.Count == 0)
					{
						throw this.UnexpectedEndError();
					}
					this.Read();
					string text = this.ReadName();
					this.SkipWhitespaces();
					string text3 = (string)this.elementNames.Pop();
					this.xmlSpaces.Pop();
					if (this.xmlSpaces.Count > 0)
					{
						this.xmlSpace = (string)this.xmlSpaces.Peek();
					}
					else
					{
						this.xmlSpace = null;
					}
					if (text != text3)
					{
						throw this.Error(string.Format("End tag mismatch: expected {0} but found {1}", text3, text));
					}
					this.handler.OnEndElement(text);
					this.Expect(62);
					return;
				}
			}
			else
			{
				this.Read();
				if (this.Peek() == 91)
				{
					this.Read();
					if (this.ReadName() != "CDATA")
					{
						throw this.Error("Invalid declaration markup");
					}
					this.Expect(91);
					this.ReadCDATASection();
					return;
				}
				else
				{
					if (this.Peek() == 45)
					{
						this.ReadComment();
						return;
					}
					if (this.ReadName() != "DOCTYPE")
					{
						throw this.Error("Invalid declaration markup.");
					}
					throw this.Error("This parser does not support document type.");
				}
			}
		}

		private void HandleBufferedContent()
		{
			if (this.buffer.Length == 0)
			{
				return;
			}
			if (this.isWhitespace)
			{
				this.handler.OnIgnorableWhitespace(this.buffer.ToString());
			}
			else
			{
				this.handler.OnChars(this.buffer.ToString());
			}
			this.buffer.Length = 0;
			this.isWhitespace = false;
		}

		private void ReadCharacters()
		{
			this.isWhitespace = false;
			while (true)
			{
				int num = this.Peek();
				int num2 = num;
				if (num2 == -1)
				{
					break;
				}
				if (num2 != 38)
				{
					if (num2 == 60)
					{
						return;
					}
					this.buffer.Append((char)this.Read());
				}
				else
				{
					this.Read();
					this.ReadReference();
				}
			}
		}

		private void ReadReference()
		{
			if (this.Peek() != 35)
			{
				string text = this.ReadName();
				this.Expect(59);
				string text2 = text;
				switch (text2)
				{
				case "amp":
					this.buffer.Append('&');
					return;
				case "quot":
					this.buffer.Append('"');
					return;
				case "apos":
					this.buffer.Append('\'');
					return;
				case "lt":
					this.buffer.Append('<');
					return;
				case "gt":
					this.buffer.Append('>');
					return;
				}
				throw this.Error("General non-predefined entity reference is not supported in this parser.");
			}
			this.Read();
			this.ReadCharacterReference();
		}

		private int ReadCharacterReference()
		{
			int num = 0;
			if (this.Peek() == 120)
			{
				this.Read();
				for (int i = this.Peek(); i >= 0; i = this.Peek())
				{
					if (48 <= i && i <= 57)
					{
						num <<= 4 + i - 48;
					}
					else if (65 <= i && i <= 70)
					{
						num <<= 4 + i - 65 + 10;
					}
					else
					{
						if (97 > i || i > 102)
						{
							break;
						}
						num <<= 4 + i - 97 + 10;
					}
					this.Read();
				}
			}
			else
			{
				for (int j = this.Peek(); j >= 0; j = this.Peek())
				{
					if (48 > j || j > 57)
					{
						break;
					}
					num <<= 4 + j - 48;
					this.Read();
				}
			}
			return num;
		}

		private void ReadAttribute(SmallXmlParser.AttrListImpl a)
		{
			this.SkipWhitespaces(true);
			if (this.Peek() == 47 || this.Peek() == 62)
			{
				return;
			}
			string text = this.ReadName();
			this.SkipWhitespaces();
			this.Expect(61);
			this.SkipWhitespaces();
			int num = this.Read();
			string value;
			if (num != 34)
			{
				if (num != 39)
				{
					throw this.Error("Invalid attribute value markup.");
				}
				value = this.ReadUntil('\'', true);
			}
			else
			{
				value = this.ReadUntil('"', true);
			}
			if (text == "xml:space")
			{
				this.xmlSpace = value;
			}
			a.Add(text, value);
		}

		private void ReadCDATASection()
		{
			int num = 0;
			while (this.Peek() >= 0)
			{
				char c = (char)this.Read();
				if (c == ']')
				{
					num++;
				}
				else
				{
					if (c == '>' && num > 1)
					{
						for (int i = num; i > 2; i--)
						{
							this.buffer.Append(']');
						}
						return;
					}
					for (int j = 0; j < num; j++)
					{
						this.buffer.Append(']');
					}
					num = 0;
					this.buffer.Append(c);
				}
			}
			throw this.UnexpectedEndError();
		}

		private void ReadComment()
		{
			this.Expect(45);
			this.Expect(45);
			while (true)
			{
				if (this.Read() == 45)
				{
					if (this.Read() == 45)
					{
						break;
					}
				}
			}
			if (this.Read() != 62)
			{
				throw this.Error("'--' is not allowed inside comment markup.");
			}
		}
	}
}
