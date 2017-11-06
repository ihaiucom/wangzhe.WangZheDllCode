using System;

namespace IIPSMobile
{
	public interface IIPSMobileDataReaderInterface
	{
		bool Read(uint fileId, ulong offset, byte[] buff, ref uint readlength);

		uint GetLastReaderError();
	}
}
