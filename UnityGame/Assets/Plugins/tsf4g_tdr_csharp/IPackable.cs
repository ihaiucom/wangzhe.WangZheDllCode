using System;

namespace tsf4g_tdr_csharp
{
	public interface IPackable
	{
		TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer);
	}
}
