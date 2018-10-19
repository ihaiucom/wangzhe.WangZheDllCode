using System;
using System.Collections.Generic;

namespace Apollo
{
	public class ApolloImgPaths : ApolloBufferBase
	{
		public List<string> ImgPaths;

		public ApolloImgPaths()
		{
			this.ImgPaths = new List<string>();
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write<string>(this.ImgPaths);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read<List<string>>(ref this.ImgPaths);
		}
	}
}
