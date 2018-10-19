using System;
using System.Runtime.InteropServices;

namespace IIPSMobile
{
	public class IIPSMobileVersionCallBack
	{
		public struct PROGRAMMEVERSION
		{
			public ushort MajorVersion_Number;

			public ushort MinorVersion_Number;

			public ushort Revision_Number;
		}

		public struct DATAVERSION
		{
			public ushort DataVersion;
		}

		public struct APPVERSION
		{
			public IIPSMobileVersionCallBack.PROGRAMMEVERSION programmeVersion;

			public IIPSMobileVersionCallBack.DATAVERSION dataVersion;
		}

		public struct VERSIONINFO
		{
			public byte isAppUpdating;

			public byte isAppDiffUpdating;

			public byte isForcedUpdating;

			public IIPSMobileVersionCallBack.APPVERSION newAppVersion;

			public ulong needDownloadSize;
		}

		public enum VERSIONSTAGE
		{
			VS_Start,
			VS_SelfDataCheck,
			VS_SelfDataRepair,
			VS_GetVersionInfo,
			VS_GetDownloadVersion,
			VS_DownloadData,
			VS_MergeData,
			VS_ExtractData,
			VS_DiffUpdata,
			VS_FullUpdate,
			VS_FirstExtract,
			VS_FullUpdate_Extract,
			VS_FullUpdate_GetFileList,
			VS_FullUpdate_GetMetaFile,
			VS_FullUpdate_CompareMetaFile,
			VS_DownApkConfig,
			VS_CreateApk,
			VS_CheckApkMd5,
			VS_FullUpdate_CreateTask,
			VS_SourceUpdateCures = 90,
			VS_SourceUpdateDownloadList,
			VS_SourcePrepareUpdate,
			VS_SourceAnalyseDiff,
			VS_SourceDownload,
			VS_SourceExtract,
			VS_Success = 99,
			VS_Fail
		}

		internal delegate byte OnGetNewVersionInfoFunc(IntPtr callback, IntPtr newVersionInfo);

		internal delegate void OnProgressFunc(IntPtr callback, IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize);

		internal delegate void OnErrorFunc(IntPtr callback, IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode);

		internal delegate void OnSuccessFunc(IntPtr callback);

		internal delegate void SaveConfigFunc(IntPtr callback, uint bufferSize, IntPtr configBuffer);

		internal delegate byte OnNoticeInstallApkFunc(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url);

		internal delegate byte OnActionMsgFunc(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url);

		private IIPSMobileVersionCallBack.OnGetNewVersionInfoFunc versionFunc;

		private IIPSMobileVersionCallBack.OnProgressFunc progressFunc;

		private IIPSMobileVersionCallBack.OnErrorFunc errFunc;

		private IIPSMobileVersionCallBack.OnSuccessFunc succFUnc;

		private IIPSMobileVersionCallBack.SaveConfigFunc saveFUnc;

		private IIPSMobileVersionCallBack.OnNoticeInstallApkFunc installApk;

		private IIPSMobileVersionCallBack.OnActionMsgFunc actionMsg;

		public IntPtr mCallBack = IntPtr.Zero;

		private IntPtr pManagedObject = IntPtr.Zero;

		private static IIPSMobileVersionCallBackInterface mImpCB;

		public IIPSMobileVersionCallBack(IIPSMobileVersionCallBackInterface imp)
		{
			this.pManagedObject = GCHandle.ToIntPtr(GCHandle.Alloc(imp, GCHandleType.Normal));
			this.versionFunc = new IIPSMobileVersionCallBack.OnGetNewVersionInfoFunc(IIPSMobileVersionCallBack.OnGetNewVersionInfo);
			this.progressFunc = new IIPSMobileVersionCallBack.OnProgressFunc(IIPSMobileVersionCallBack.OnProgress);
			this.errFunc = new IIPSMobileVersionCallBack.OnErrorFunc(IIPSMobileVersionCallBack.OnError);
			this.succFUnc = new IIPSMobileVersionCallBack.OnSuccessFunc(IIPSMobileVersionCallBack.OnSuccess);
			this.saveFUnc = new IIPSMobileVersionCallBack.SaveConfigFunc(IIPSMobileVersionCallBack.SaveConfig);
			this.installApk = new IIPSMobileVersionCallBack.OnNoticeInstallApkFunc(IIPSMobileVersionCallBack.OnNoticeInstallApk);
			this.actionMsg = new IIPSMobileVersionCallBack.OnActionMsgFunc(IIPSMobileVersionCallBack.OnActionMsg);
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr CreateVersionInfoCallBack(IIPSMobileVersionCallBack.OnGetNewVersionInfoFunc onGetNewVersionInfoFunc, IIPSMobileVersionCallBack.OnProgressFunc onProgressFunc, IIPSMobileVersionCallBack.OnErrorFunc onErrorFunc, IIPSMobileVersionCallBack.OnSuccessFunc onSuccessFunc, IIPSMobileVersionCallBack.SaveConfigFunc saveConfigFunc, IIPSMobileVersionCallBack.OnNoticeInstallApkFunc noticeInstallApk, IIPSMobileVersionCallBack.OnActionMsgFunc msg, IntPtr callback);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern void DestroyVersionInfoCallBack(IntPtr callback);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr GetCallBackGCHandle(IntPtr callback);
		
		~IIPSMobileVersionCallBack()
		{
			if (this.mCallBack != IntPtr.Zero)
			{
				GCHandle.FromIntPtr(GetCallBackGCHandle(this.mCallBack)).Free();
				DestroyVersionInfoCallBack(this.mCallBack);
				this.mCallBack = IntPtr.Zero;
			}
		}


		public void CreateCppVersionInfoCallBack()
		{
			this.mCallBack = IIPSMobileVersionCallBack.CreateVersionInfoCallBack(
				new IIPSMobileVersionCallBack.OnGetNewVersionInfoFunc(IIPSMobileVersionCallBack.OnGetNewVersionInfo), 
				new IIPSMobileVersionCallBack.OnProgressFunc(IIPSMobileVersionCallBack.OnProgress), 
				new IIPSMobileVersionCallBack.OnErrorFunc(IIPSMobileVersionCallBack.OnError), 
				new IIPSMobileVersionCallBack.OnSuccessFunc(IIPSMobileVersionCallBack.OnSuccess), 
				new IIPSMobileVersionCallBack.SaveConfigFunc(IIPSMobileVersionCallBack.SaveConfig), 
				new IIPSMobileVersionCallBack.OnNoticeInstallApkFunc(IIPSMobileVersionCallBack.OnNoticeInstallApk), 
				new IIPSMobileVersionCallBack.OnActionMsgFunc(IIPSMobileVersionCallBack.OnActionMsg), 
				this.pManagedObject);
		}

		public void DeleteCppVersionCallBack()
		{
			if (this.mCallBack != IntPtr.Zero)
			{
				IntPtr callBackGCHandle = IIPSMobileVersionCallBack.GetCallBackGCHandle(this.mCallBack);
				GCHandle.FromIntPtr(callBackGCHandle).Free();
				IIPSMobileVersionCallBack.DestroyVersionInfoCallBack(this.mCallBack);
				this.mCallBack = IntPtr.Zero;
			}
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.OnGetNewVersionInfoFunc))]
		public static byte OnGetNewVersionInfo(IntPtr callback, IntPtr newVersionInfo)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo2 = (IIPSMobileVersionCallBack.VERSIONINFO)Marshal.PtrToStructure(newVersionInfo, typeof(IIPSMobileVersionCallBack.VERSIONINFO));
			return IIPSMobileVersionCallBack.mImpCB.OnGetNewVersionInfo(newVersionInfo2);
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.OnNoticeInstallApkFunc))]
		public static byte OnNoticeInstallApk(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			return IIPSMobileVersionCallBack.mImpCB.OnNoticeInstallApk(url);
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.OnActionMsgFunc))]
		public static byte OnActionMsg(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			return IIPSMobileVersionCallBack.mImpCB.OnActionMsg(url);
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.OnProgressFunc))]
		public static void OnProgress(IntPtr callback, IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			IIPSMobileVersionCallBack.mImpCB.OnProgress(curVersionStage, totalSize, nowSize);
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.OnErrorFunc))]
		public static void OnError(IntPtr callback, IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			IIPSMobileVersionCallBack.mImpCB.OnError(curVersionStage, errorCode);
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.OnSuccessFunc))]
		public static void OnSuccess(IntPtr callback)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			IIPSMobileVersionCallBack.mImpCB.OnSuccess();
		}

		[MonoPInvokeCallback(typeof(IIPSMobileVersionCallBack.SaveConfigFunc))]
		public static void SaveConfig(IntPtr callback, uint bufferSize, IntPtr configBuffer)
		{
			IIPSMobileVersionCallBack.mImpCB = (IIPSMobileVersionCallBackInterface)GCHandle.FromIntPtr(callback).Target;
			IIPSMobileVersionCallBack.mImpCB.SaveConfig(bufferSize, configBuffer);
		}
	}
}
