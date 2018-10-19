using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public interface tsf4g_csharp_interface : IPackable, IUnpackable
	{
		TdrError.ErrorType construct();

		TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer);

		TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer);
	}
}
