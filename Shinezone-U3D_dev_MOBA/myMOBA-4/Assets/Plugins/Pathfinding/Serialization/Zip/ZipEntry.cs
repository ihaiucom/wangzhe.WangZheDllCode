using System;
using System.IO;

namespace Pathfinding.Serialization.Zip
{
	public class ZipEntry
	{
		internal string name;

		internal byte[] bytes;

		public ZipEntry(string name, byte[] bytes)
		{
			this.name = name;
			this.bytes = bytes;
		}

		public void Extract(Stream stream)
		{
			stream.Write(this.bytes, 0, this.bytes.Length);
		}
	}
}
