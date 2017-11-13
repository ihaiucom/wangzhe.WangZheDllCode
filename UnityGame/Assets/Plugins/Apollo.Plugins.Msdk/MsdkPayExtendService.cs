using System;

namespace Apollo.Plugins.Msdk
{
	public class MsdkPayExtendService : IApolloExtendPayService, IApolloExtendPayServiceBase
	{
		public static MsdkPayExtendService Instance = new MsdkPayExtendService();

		private MsdkPayExtendService()
		{
		}
	}
}
