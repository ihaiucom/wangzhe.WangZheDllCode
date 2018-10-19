using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Apollo
{
	internal class Notice : ApolloObject, INotice, IApolloServiceBase
	{
		public static readonly Notice Instance = new Notice();

		private Notice()
		{
		}

		public void ShowNotice(APOLLO_NOTICETYPE type, string scene)
		{
			Notice.ShowNotice(base.ObjectId, type, scene);
		}

		public void HideNotice()
		{
			Notice.HideScrollNotice(base.ObjectId);
		}

		public void GetNoticeData(APOLLO_NOTICETYPE type, string scene, ref ApolloNoticeInfo info)
		{
			StringBuilder stringBuilder = new StringBuilder(10240);
			Notice.GetNoticeData(base.ObjectId, type, scene, stringBuilder, 10240);
			string text = stringBuilder.ToString();
			if (text.Length > 0)
			{
				info.FromString(text);
			}
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void ShowNotice(ulong objId, APOLLO_NOTICETYPE type, [MarshalAs(UnmanagedType.LPStr)] string scene);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void HideScrollNotice(ulong objId);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void GetNoticeData(ulong objId, APOLLO_NOTICETYPE type, [MarshalAs(UnmanagedType.LPStr)] string scene, [MarshalAs(UnmanagedType.LPStr)] StringBuilder info, int size);
	}
}
