using System;

namespace Apollo
{
	public interface IMessageResponse
	{
		int unpack(ref byte[] buffer, int size, ref int usedSize);
	}
}
