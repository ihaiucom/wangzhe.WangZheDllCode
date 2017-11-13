using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pathfinding.Serialization.Zip
{
	public class ZipFile
	{
		public Encoding AlternateEncoding;

		public ZipOption AlternateEncodingUsage;

		private DictionaryView<string, ZipEntry> dict = new DictionaryView<string, ZipEntry>();

		public ZipEntry this[string index]
		{
			get
			{
				ZipEntry result;
				this.dict.TryGetValue(index, out result);
				return result;
			}
		}

		public void AddEntry(string name, byte[] bytes)
		{
			this.dict[name] = new ZipEntry(name, bytes);
		}

		public bool ContainsEntry(string name)
		{
			return this.dict.ContainsKey(name);
		}

		public void Save(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(this.dict.Count);
			foreach (KeyValuePair<string, ZipEntry> current in this.dict)
			{
				binaryWriter.Write(current.get_Key());
				binaryWriter.Write(current.get_Value().bytes.Length);
				binaryWriter.Write(current.get_Value().bytes);
			}
		}

		public static ZipFile Read(Stream stream)
		{
			ZipFile zipFile = new ZipFile();
			BinaryReader binaryReader = new BinaryReader(stream);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				int num2 = binaryReader.ReadInt32();
				byte[] bytes = binaryReader.ReadBytes(num2);
				zipFile.dict[text] = new ZipEntry(text, bytes);
			}
			return zipFile;
		}

		public void Dispose()
		{
		}
	}
}
