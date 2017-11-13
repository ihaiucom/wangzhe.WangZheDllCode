using System;
using System.Runtime.InteropServices;

namespace IIPSMobile
{
	public class IIPSMobileDataManager : IIPSMobileDataMgrInterface
	{
		protected IntPtr mDataManager = IntPtr.Zero;

		public IIPSMobileDataManager()
		{
			this.mDataManager = IIPSMobileDataManager.CreateDataManager();
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr CreateDataManager();

		[DllImport("apollo", ExactSpelling = true)]
		private static extern void ReleaseDataManager(IntPtr dataManager);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte InitDataManager(IntPtr dataManager, uint bufferSize, IntPtr configBuffer);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte DataMgrPollCallback(IntPtr dataManager);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte UnitDataManager(IntPtr dataManager);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr GetDataReader(IntPtr dataManager);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr GetDataDownloader(IntPtr dataManager, byte openProgressCallBack);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr GetDataQuery(IntPtr dataManager);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetLastDataMgrError(IntPtr dataManager);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern ulong GetDataMgrMemorySize(IntPtr dataManager);

		~IIPSMobileDataManager()
		{
			if (this.mDataManager != IntPtr.Zero)
			{
				IIPSMobileDataManager.ReleaseDataManager(this.mDataManager);
				this.mDataManager = IntPtr.Zero;
			}
		}

		public bool Init(uint bufferSize, byte[] configBuffer)
		{
			if (this.mDataManager == IntPtr.Zero)
			{
				return false;
			}
			GCHandle gCHandle = GCHandle.Alloc(configBuffer, 3);
			IntPtr configBuffer2 = gCHandle.AddrOfPinnedObject();
			gCHandle.Free();
			return IIPSMobileDataManager.InitDataManager(this.mDataManager, bufferSize, configBuffer2) > 0;
		}

		public bool PollCallBack()
		{
			return !(this.mDataManager == IntPtr.Zero) && IIPSMobileDataManager.DataMgrPollCallback(this.mDataManager) > 0;
		}

		public bool Uninit()
		{
			return !(this.mDataManager == IntPtr.Zero) && IIPSMobileDataManager.UnitDataManager(this.mDataManager) > 0;
		}

		public IIPSMobileDataReaderInterface GetDataReader()
		{
			if (this.mDataManager == IntPtr.Zero)
			{
				return null;
			}
			return new DataReader(IIPSMobileDataManager.GetDataReader(this.mDataManager));
		}

		public IIPSMobileDownloaderInterface GetDataDownloader(bool openProgressCallBack = false)
		{
			if (this.mDataManager == IntPtr.Zero)
			{
				return null;
			}
			byte openProgressCallBack2 = 0;
			if (openProgressCallBack)
			{
				openProgressCallBack2 = 1;
			}
			return new DataDownloader(IIPSMobileDataManager.GetDataDownloader(this.mDataManager, openProgressCallBack2));
		}

		public IIPSMobileDataQueryInterface GetDataQuery()
		{
			if (this.mDataManager == IntPtr.Zero)
			{
				return null;
			}
			return new DataQuery(IIPSMobileDataManager.GetDataQuery(this.mDataManager));
		}

		public uint MgrGetDataMgrLastError()
		{
			if (this.mDataManager == IntPtr.Zero)
			{
				return 0u;
			}
			return IIPSMobileDataManager.GetLastDataMgrError(this.mDataManager);
		}

		public ulong MgrGetMemorySize()
		{
			if (this.mDataManager == IntPtr.Zero)
			{
				return 0uL;
			}
			return IIPSMobileDataManager.GetDataMgrMemorySize(this.mDataManager);
		}
	}
}
