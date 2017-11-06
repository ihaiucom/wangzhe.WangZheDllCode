using System;
using System.Runtime.InteropServices;

namespace IIPSMobile
{
	public class DataReader : IIPSMobileDataReaderInterface
	{
		public IntPtr mDataReader = IntPtr.Zero;

		public DataReader(IntPtr Reader)
		{
			this.mDataReader = Reader;
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte Read(IntPtr dataReader, uint fileId, ulong offset, byte[] buff, ref uint readlength);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetLastReaderError(IntPtr reader);

		public bool Read(uint fileId, ulong offset, byte[] buff, ref uint readlength)
		{
			return !(this.mDataReader == IntPtr.Zero) && DataReader.Read(this.mDataReader, fileId, offset, buff, ref readlength) > 0;
		}

		public uint GetLastReaderError()
		{
			if (this.mDataReader == IntPtr.Zero)
			{
				return 0u;
			}
			return DataReader.GetLastReaderError(this.mDataReader);
		}
	}
}
