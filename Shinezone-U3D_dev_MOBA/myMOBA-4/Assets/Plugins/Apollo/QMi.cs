using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class QMi : ApolloObject, IQMi, IApolloServiceBase
	{
		public static readonly QMi Instance = new QMi();

		private QMi()
		{
		}

		public void ShowQMi()
		{
			QMi.WGShowQMi(base.ObjectId);
		}

		public void HideQMi()
		{
			QMi.WGHideQMi(base.ObjectId);
		}

		public void SetGameEngineType(string gameEngineInfo)
		{
			QMi.WGSetGameEngineType(base.ObjectId, gameEngineInfo);
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void WGShowQMi(ulong objId);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void WGHideQMi(ulong objId);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void WGSetGameEngineType(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string gameEngineType);
	}
}
