using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IIPSMobile
{
	public class VersionMgr : IIPSMobileVersionMgrInterface
	{
		private IntPtr mVersionMgr = IntPtr.Zero;

		public VersionMgr()
		{
			this.mVersionMgr = IntPtr.Zero;
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr CreateVersionManager();

		[DllImport("apollo", ExactSpelling = true)]
		private static extern void ReleaseVersionManager(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte InitVersionMgr(IntPtr versionMgr, IntPtr callback, uint bufferSize, IntPtr configBuffer);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte UnitVersionMgr(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte SetNextStage(IntPtr versionMgr, byte goonWork);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte CheckAppUpdate(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern void CancelUpdate(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern short GetCurDataVersion(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetVersionMgrLastError(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern ulong GetMemorySize(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetActionDownloadSpeed(IntPtr versionMgr);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern void PoolVersionManager(IntPtr versionMgr);

		public void CreateCppVersionManager()
		{
			this.mVersionMgr = VersionMgr.CreateVersionManager();
		}

		public void DeleteCppVersionManager()
		{
			if (this.mVersionMgr != IntPtr.Zero)
			{
				VersionMgr.ReleaseVersionManager(this.mVersionMgr);
				this.mVersionMgr = IntPtr.Zero;
			}
		}

		~VersionMgr()
		{
			this.mVersionMgr = IntPtr.Zero;
		}

		public bool MgrInitVersionManager(IIPSMobileVersionCallBack callBack, uint bufferSize, byte[] configBuffer)
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return false;
			}
			GCHandle gCHandle = GCHandle.Alloc(configBuffer, 3);
			IntPtr configBuffer2 = gCHandle.AddrOfPinnedObject();
			gCHandle.Free();
			return VersionMgr.InitVersionMgr(this.mVersionMgr, callBack.mCallBack, bufferSize, configBuffer2) > 0;
		}

		public bool MgrUnitVersionManager()
		{
			return !(this.mVersionMgr == IntPtr.Zero) && VersionMgr.UnitVersionMgr(this.mVersionMgr) > 0;
		}

		public bool MgrSetNextStage(bool goonWork)
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return false;
			}
			byte goonWork2 = 0;
			if (goonWork)
			{
				goonWork2 = 1;
			}
			return VersionMgr.SetNextStage(this.mVersionMgr, goonWork2) > 0;
		}

		public bool MgrPoll()
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return false;
			}
			VersionMgr.PoolVersionManager(this.mVersionMgr);
			return true;
		}

		public bool MgrCheckAppUpdate()
		{
			return !(this.mVersionMgr == IntPtr.Zero) && VersionMgr.CheckAppUpdate(this.mVersionMgr) > 0;
		}

		public void MgrCancelUpdate()
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return;
			}
			VersionMgr.CancelUpdate(this.mVersionMgr);
		}

		public short MgrGetCurDataVersion()
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return 0;
			}
			return VersionMgr.GetCurDataVersion(this.mVersionMgr);
		}

		public uint MgrGetVersionMgrLastError()
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return 1u;
			}
			return VersionMgr.GetVersionMgrLastError(this.mVersionMgr);
		}

		public ulong MgrGetMemorySize()
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return 0uL;
			}
			return VersionMgr.GetMemorySize(this.mVersionMgr);
		}

		public uint MgrGetActionDownloadSpeed()
		{
			if (this.mVersionMgr == IntPtr.Zero)
			{
				return 0u;
			}
			return VersionMgr.GetActionDownloadSpeed(this.mVersionMgr);
		}

		public bool InstallApk(string path)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (androidJavaClass == null)
			{
				return false;
			}
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (@static == null)
			{
				return false;
			}
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("cu_iipsmobile.CuIIPSMobile", new object[0]);
			if (androidJavaObject == null)
			{
				return false;
			}
			int num = androidJavaObject.Call<int>("installAPK", new object[]
			{
				path,
				@static
			});
			return num == 0;
		}
	}
}
