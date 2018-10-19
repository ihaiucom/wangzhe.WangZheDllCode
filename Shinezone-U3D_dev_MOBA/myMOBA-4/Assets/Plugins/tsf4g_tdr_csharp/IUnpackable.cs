using System;

namespace tsf4g_tdr_csharp
{
	public interface IUnpackable
	{
		TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer);
	}
}
