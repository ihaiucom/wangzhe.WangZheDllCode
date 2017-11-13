using System;
using System.Collections;
using System.IO;
using System.Security;
using System.Text;

namespace Mono.Xml
{
	public static class SecurityTools
	{
		private enum ESecurityElementType
		{
			Root,
			Children
		}

		public static bool ConvertXmlFileToBinaryFile(string InXmlPath, string InBinaryPath)
		{
			string text = File.ReadAllText(InXmlPath, Encoding.get_UTF8());
			if (string.IsNullOrEmpty(text))
			{
				DebugHelper.Assert(false, "{0} is not an valid xml file.", new object[]
				{
					InXmlPath
				});
				return false;
			}
			return SecurityTools.ConvertXmlTextToBinaryFile(text, InBinaryPath);
		}

		public static bool ConvertXmlTextToBinaryFile(string InXmlText, string InBinaryPath)
		{
			SecurityParser securityParser = new SecurityParser();
			securityParser.LoadXml(InXmlText);
			SecurityElement root = securityParser.root;
			return SecurityTools.ConvertSecurityElementToBinaryFile(root, InBinaryPath);
		}

		public static bool ConvertSecurityElementToBinaryFile(SecurityElement InRoot, string InBinaryPath)
		{
			using (FileStream fileStream = File.OpenWrite(InBinaryPath))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					SecurityTools.Write(binaryWriter, InRoot, 0);
				}
			}
			return true;
		}

		private static void WriteString(this BinaryWriter InWriter, string InText)
		{
			if (InText != null)
			{
				InWriter.Write(InText);
			}
			else
			{
				InWriter.Write(string.Empty);
			}
		}

		private static void Write(BinaryWriter InWriterRef, SecurityElement InElement, byte InType)
		{
			InWriterRef.Write(InType);
			InWriterRef.WriteString(InElement.get_Tag());
			InWriterRef.WriteString(InElement.get_Text());
			Hashtable attributes = InElement.get_Attributes();
			int num = (attributes != null) ? attributes.get_Count() : 0;
			InWriterRef.Write(num);
			if (attributes != null)
			{
				IDictionaryEnumerator enumerator = attributes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = enumerator.get_Key() as string;
					string text2 = enumerator.get_Value() as string;
					DebugHelper.Assert(text != null && text2 != null, "Invalid Attributes");
					InWriterRef.WriteString(text);
					InWriterRef.WriteString(text2);
				}
			}
			ArrayList children = InElement.get_Children();
			int num2 = (children != null) ? children.get_Count() : 0;
			InWriterRef.Write(num2);
			if (children != null)
			{
				IEnumerator enumerator2 = children.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SecurityElement securityElement = enumerator2.get_Current() as SecurityElement;
					DebugHelper.Assert(securityElement != null, "Invalid Security Element.");
					SecurityTools.Write(InWriterRef, securityElement, 1);
				}
			}
		}

		public static SecurityParser LoadXmlFromBinaryFile(string InPath)
		{
			SecurityParser securityParser = new SecurityParser();
			if (!SecurityTools.LoadXmlFromBinaryFile(securityParser, InPath))
			{
				return null;
			}
			return securityParser;
		}

		public static bool LoadXmlFromBinaryFile(SecurityParser InParser, string InPath)
		{
			byte[] inFileBytes = File.ReadAllBytes(InPath);
			return SecurityTools.LoadXmlFromBinaryBuffer(InParser, inFileBytes, InPath);
		}

		public static bool LoadXmlFromBinaryBuffer(SecurityParser InParser, byte[] InFileBytes, string InPath = "")
		{
			if (InFileBytes == null || InFileBytes.Length < 4)
			{
				return false;
			}
			using (MemoryStream memoryStream = new MemoryStream(InFileBytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					SecurityElement securityElement = SecurityTools.LoadRootSecurityElement(binaryReader);
					DebugHelper.Assert(securityElement != null, "Failed load root Security Element in file: {0}", new object[]
					{
						InPath
					});
					if (securityElement != null)
					{
						InParser.root = securityElement;
						return true;
					}
				}
			}
			return false;
		}

		private static SecurityElement LoadRootSecurityElement(BinaryReader InReader)
		{
			SecurityElement result;
			try
			{
				SecurityElement securityElement = SecurityTools.LoadSecurityElementChecked(InReader, 0, null);
				result = securityElement;
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, ex.get_Message());
				result = null;
			}
			return result;
		}

		private static SecurityElement LoadSecurityElementChecked(BinaryReader InReader, byte InType, SecurityElement InParent)
		{
			byte b = InReader.ReadByte();
			if (b != InType)
			{
				return null;
			}
			string text = InReader.ReadString();
			string text2 = InReader.ReadString();
			SecurityElement securityElement = new SecurityElement(text, text2);
			int num = InReader.ReadInt32();
			DebugHelper.Assert(num < 512, "too many attributes.");
			for (int i = 0; i < num; i++)
			{
				string text3 = InReader.ReadString();
				string text4 = InReader.ReadString();
				securityElement.AddAttribute(text3, text4);
			}
			int num2 = InReader.ReadInt32();
			DebugHelper.Assert(num2 < 515, "too many children");
			for (int j = 0; j < num2; j++)
			{
				SecurityElement securityElement2 = SecurityTools.LoadSecurityElementChecked(InReader, 1, securityElement);
				DebugHelper.Assert(securityElement2 != null, "invalid child element");
			}
			if (InParent != null)
			{
				InParent.AddChild(securityElement);
			}
			return securityElement;
		}
	}
}
