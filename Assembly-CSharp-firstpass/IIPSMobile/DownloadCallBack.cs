using System;
using System.Runtime.InteropServices;

namespace IIPSMobile
{
	public class DownloadCallBack
	{
		internal delegate void OnDownloadErrorFunc(IntPtr callback, uint taskId, uint errorCode);

		internal delegate void OnDownloadSuccessFunc(IntPtr callback, uint taskId);

		internal delegate void OnDownloadProgressFunc(IntPtr callback, uint taskId, DataDownloader.DownloadInfo info);

		private DownloadCallBack.OnDownloadErrorFunc errFunc;

		private DownloadCallBack.OnDownloadSuccessFunc succFunc;

		private DownloadCallBack.OnDownloadProgressFunc progressFunc;

		public static IIPSMobileDownloadCallbackInterface mCBImp;

		public IntPtr mCallBack = IntPtr.Zero;

		public DownloadCallBack(IIPSMobileDownloadCallbackInterface CBImp)
		{
			IntPtr callback = GCHandle.ToIntPtr(GCHandle.Alloc(CBImp, 2));
			this.errFunc = new DownloadCallBack.OnDownloadErrorFunc(DownloadCallBack.OnDownloadError);
			this.succFunc = new DownloadCallBack.OnDownloadSuccessFunc(DownloadCallBack.OnDownloadSuccess);
			this.progressFunc = new DownloadCallBack.OnDownloadProgressFunc(DownloadCallBack.OnDownloadProgress);
			this.mCallBack = DownloadCallBack.CreateDownlaodMgrCallBack(this.errFunc, this.succFunc, this.progressFunc, callback);
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr CreateDownlaodMgrCallBack(DownloadCallBack.OnDownloadErrorFunc onDownloadError, DownloadCallBack.OnDownloadSuccessFunc onDownloadSuccess, DownloadCallBack.OnDownloadProgressFunc onDownloadProgress, IntPtr callback);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern void DestroyDownlaodMgrCallBack(IntPtr callback);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr GetDownloadCallbackGCHandle(IntPtr callback);

		~DownloadCallBack()
		{
			if (this.mCallBack != IntPtr.Zero)
			{
				GCHandle.FromIntPtr(DownloadCallBack.GetDownloadCallbackGCHandle(this.mCallBack)).Free();
				DownloadCallBack.DestroyDownlaodMgrCallBack(this.mCallBack);
			}
		}

		[MonoPInvokeCallback(typeof(DownloadCallBack.OnDownloadErrorFunc))]
		public static void OnDownloadError(IntPtr callback, uint taskId, uint errorCode)
		{
			DownloadCallBack.mCBImp = (IIPSMobileDownloadCallbackInterface)GCHandle.FromIntPtr(callback).get_Target();
			DownloadCallBack.mCBImp.OnDownloadError(taskId, errorCode);
		}

		[MonoPInvokeCallback(typeof(DownloadCallBack.OnDownloadProgressFunc))]
		public static void OnDownloadProgress(IntPtr callback, uint taskId, DataDownloader.DownloadInfo info)
		{
			DownloadCallBack.mCBImp = (IIPSMobileDownloadCallbackInterface)GCHandle.FromIntPtr(callback).get_Target();
			DownloadCallBack.mCBImp.OnDownloadProgress(taskId, info);
		}

		[MonoPInvokeCallback(typeof(DownloadCallBack.OnDownloadSuccessFunc))]
		public static void OnDownloadSuccess(IntPtr callback, uint taskId)
		{
			DownloadCallBack.mCBImp = (IIPSMobileDownloadCallbackInterface)GCHandle.FromIntPtr(callback).get_Target();
			DownloadCallBack.mCBImp.OnDownloadSuccess(taskId);
		}
	}
}
