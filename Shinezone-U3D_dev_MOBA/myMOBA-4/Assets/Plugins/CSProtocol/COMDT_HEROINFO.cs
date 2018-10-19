using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HEROINFO : ProtocolObject
	{
		public COMDT_HERO_COMMON_INFO stCommonInfo;

		public COMDT_HERO_WEARINFO[] astGearWear;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly int CLASS_ID = 112;

		public COMDT_HEROINFO()
		{
			this.stCommonInfo = (COMDT_HERO_COMMON_INFO)ProtocolObjectPool.Get(COMDT_HERO_COMMON_INFO.CLASS_ID);
			this.astGearWear = new COMDT_HERO_WEARINFO[6];
			for (int i = 0; i < 6; i++)
			{
				this.astGearWear[i] = (COMDT_HERO_WEARINFO)ProtocolObjectPool.Get(COMDT_HERO_WEARINFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stCommonInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = this.astGearWear[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_HEROINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HEROINFO.CURRVERSION;
			}
			if (COMDT_HEROINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stCommonInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = this.astGearWear[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_HEROINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HEROINFO.CURRVERSION;
			}
			if (COMDT_HEROINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stCommonInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = this.astGearWear[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_HEROINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stCommonInfo != null)
			{
				this.stCommonInfo.Release();
				this.stCommonInfo = null;
			}
			if (this.astGearWear != null)
			{
				for (int i = 0; i < this.astGearWear.Length; i++)
				{
					if (this.astGearWear[i] != null)
					{
						this.astGearWear[i].Release();
						this.astGearWear[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			this.stCommonInfo = (COMDT_HERO_COMMON_INFO)ProtocolObjectPool.Get(COMDT_HERO_COMMON_INFO.CLASS_ID);
			if (this.astGearWear != null)
			{
				for (int i = 0; i < this.astGearWear.Length; i++)
				{
					this.astGearWear[i] = (COMDT_HERO_WEARINFO)ProtocolObjectPool.Get(COMDT_HERO_WEARINFO.CLASS_ID);
				}
			}
		}
	}
}
