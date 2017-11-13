using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_STATISTIC_DATA_EXTRA_DETAIL : ProtocolObject
	{
		public COMDT_HERO_STATISTIC_INFO st5v5;

		public COMDT_HERO_STATISTIC_INFO stLadder;

		public COMDT_HERO_STATISTIC_INFO stGuildMatch;

		public COMDT_HERO_STATISTIC_INFO st1v1;

		public COMDT_HERO_STATISTIC_INFO st3v3;

		public COMDT_HERO_STATISTIC_INFO stEntertainment;

		public uint dwOldestPos;

		public uint dwRecentNum;

		public COMDT_HERO_STATISTIC_INFO[] astRecentDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_st1v1 = 162u;

		public static readonly uint VERSION_st3v3 = 162u;

		public static readonly uint VERSION_stEntertainment = 162u;

		public static readonly int CLASS_ID = 495;

		public COMDT_STATISTIC_DATA_EXTRA_DETAIL()
		{
			this.st5v5 = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.stLadder = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.stGuildMatch = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.st1v1 = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.st3v3 = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.stEntertainment = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.astRecentDetail = new COMDT_HERO_STATISTIC_INFO[100];
			for (int i = 0; i < 100; i++)
			{
				this.astRecentDetail[i] = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
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
			if (cutVer == 0u || COMDT_STATISTIC_DATA_EXTRA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_DATA_EXTRA_DETAIL.CURRVERSION;
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.st5v5.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLadder.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildMatch.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.VERSION_st1v1 <= cutVer)
			{
				errorType = this.st1v1.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.VERSION_st3v3 <= cutVer)
			{
				errorType = this.st3v3.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.VERSION_stEntertainment <= cutVer)
			{
				errorType = this.stEntertainment.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwOldestPos);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRecentNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwRecentNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astRecentDetail.Length < (long)((ulong)this.dwRecentNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwRecentNum))
			{
				errorType = this.astRecentDetail[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || COMDT_STATISTIC_DATA_EXTRA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_DATA_EXTRA_DETAIL.CURRVERSION;
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.st5v5.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLadder.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildMatch.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.VERSION_st1v1 <= cutVer)
			{
				errorType = this.st1v1.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.st1v1.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.VERSION_st3v3 <= cutVer)
			{
				errorType = this.st3v3.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.st3v3.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_EXTRA_DETAIL.VERSION_stEntertainment <= cutVer)
			{
				errorType = this.stEntertainment.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stEntertainment.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwOldestPos);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRecentNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwRecentNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwRecentNum))
			{
				errorType = this.astRecentDetail[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_STATISTIC_DATA_EXTRA_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.st5v5 != null)
			{
				this.st5v5.Release();
				this.st5v5 = null;
			}
			if (this.stLadder != null)
			{
				this.stLadder.Release();
				this.stLadder = null;
			}
			if (this.stGuildMatch != null)
			{
				this.stGuildMatch.Release();
				this.stGuildMatch = null;
			}
			if (this.st1v1 != null)
			{
				this.st1v1.Release();
				this.st1v1 = null;
			}
			if (this.st3v3 != null)
			{
				this.st3v3.Release();
				this.st3v3 = null;
			}
			if (this.stEntertainment != null)
			{
				this.stEntertainment.Release();
				this.stEntertainment = null;
			}
			this.dwOldestPos = 0u;
			this.dwRecentNum = 0u;
			if (this.astRecentDetail != null)
			{
				for (int i = 0; i < this.astRecentDetail.Length; i++)
				{
					if (this.astRecentDetail[i] != null)
					{
						this.astRecentDetail[i].Release();
						this.astRecentDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			this.st5v5 = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.stLadder = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.stGuildMatch = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.st1v1 = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.st3v3 = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			this.stEntertainment = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
			if (this.astRecentDetail != null)
			{
				for (int i = 0; i < this.astRecentDetail.Length; i++)
				{
					this.astRecentDetail[i] = (COMDT_HERO_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_INFO.CLASS_ID);
				}
			}
		}
	}
}
