using System;

namespace Apollo
{
	public class RawMessageEventArgs
	{
		public byte[] Data
		{
			get;
			set;
		}

		public RawMessageEventArgs(byte[] data)
		{
			this.Data = data;
		}
	}
}
