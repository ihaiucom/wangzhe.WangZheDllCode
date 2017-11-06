using System;
using System.Runtime.InteropServices;

namespace Apollo.Plugins.Msdk
{
	public class MsdkAdapter : PluginBase
	{
		public static MsdkAdapter Instance = new MsdkAdapter();

		private MsdkAdapter()
		{
		}

		public override bool Install()
		{
			MsdkAdapter.msdk_adapter_install();
			IApolloReportService apolloReportService = IApollo.Instance.GetService(3) as IApolloReportService;
			IApolloCommonService apolloCommonService = IApollo.Instance.GetService(8) as IApolloCommonService;
			if (apolloCommonService != null)
			{
				apolloCommonService.PushInit();
			}
			return true;
		}

		public static bool InnerInstall()
		{
			ADebug.Log("InnerInstall");
			return MsdkAdapter.Instance.Install();
		}

		public override string GetPluginName()
		{
			return "MSDK";
		}

		public override IApolloExtendPayServiceBase GetPayExtendService()
		{
			return MsdkPayExtendService.Instance;
		}

		public override ApolloBufferBase CreatePayResponseInfo(int action)
		{
			return new ApolloPayResponseInfo();
		}

		public override IApolloServiceBase GetService(int serviceType)
		{
			switch (serviceType)
			{
			case 1:
				return ApolloSnsService.Instance;
			case 2:
				return ApolloPayService.Instance;
			case 3:
				return ApolloReportService.Instance;
			case 5:
				return Notice.Instance;
			case 6:
				return ApolloLbsService.Instance;
			case 7:
				return ApolloQuickLoginService.Instance;
			case 8:
				return ApolloCommonService.Instance;
			}
			return null;
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void msdk_adapter_install();
	}
}
