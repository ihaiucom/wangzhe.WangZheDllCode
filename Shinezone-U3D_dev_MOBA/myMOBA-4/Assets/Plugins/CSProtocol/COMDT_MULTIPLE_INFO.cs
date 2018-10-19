using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MULTIPLE_INFO : ProtocolObject
	{
		public uint dwWealRatio;

		public uint dwQQVIPRatio;

		public uint dwPropRatio;

		public uint dwPvpDailyRatio;

		public uint dwWXGameCenterLoginRatio;

		public uint dwGuildRatio;

		public uint dwFirstWinAdd;

		public uint dwGameVIPRatio;

		public uint dwQQGameCenterLoginRatio;

		public uint dwIOSVisitorLoginRatio;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 113u;

		public static readonly uint VERSION_dwPvpDailyRatio = 28u;

		public static readonly uint VERSION_dwWXGameCenterLoginRatio = 63u;

		public static readonly uint VERSION_dwGuildRatio = 68u;

		public static readonly uint VERSION_dwFirstWinAdd = 69u;

		public static readonly uint VERSION_dwGameVIPRatio = 85u;

		public static readonly uint VERSION_dwQQGameCenterLoginRatio = 101u;

		public static readonly uint VERSION_dwIOSVisitorLoginRatio = 113u;

		public static readonly int CLASS_ID = 234;

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
			if (cutVer == 0u || COMDT_MULTIPLE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTIPLE_INFO.CURRVERSION;
			}
			if (COMDT_MULTIPLE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwWealRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwQQVIPRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPropRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwPvpDailyRatio <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwPvpDailyRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwWXGameCenterLoginRatio <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWXGameCenterLoginRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwGuildRatio <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGuildRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwFirstWinAdd <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwFirstWinAdd);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwGameVIPRatio <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGameVIPRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwQQGameCenterLoginRatio <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwQQGameCenterLoginRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwIOSVisitorLoginRatio <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwIOSVisitorLoginRatio);
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
			if (cutVer == 0u || COMDT_MULTIPLE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTIPLE_INFO.CURRVERSION;
			}
			if (COMDT_MULTIPLE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwWealRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwQQVIPRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPropRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwPvpDailyRatio <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwPvpDailyRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwPvpDailyRatio = 0u;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwWXGameCenterLoginRatio <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWXGameCenterLoginRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWXGameCenterLoginRatio = 0u;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwGuildRatio <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGuildRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGuildRatio = 0u;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwFirstWinAdd <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwFirstWinAdd);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwFirstWinAdd = 0u;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwGameVIPRatio <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGameVIPRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGameVIPRatio = 0u;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwQQGameCenterLoginRatio <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwQQGameCenterLoginRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwQQGameCenterLoginRatio = 0u;
			}
			if (COMDT_MULTIPLE_INFO.VERSION_dwIOSVisitorLoginRatio <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwIOSVisitorLoginRatio);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwIOSVisitorLoginRatio = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MULTIPLE_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwWealRatio = 0u;
			this.dwQQVIPRatio = 0u;
			this.dwPropRatio = 0u;
			this.dwPvpDailyRatio = 0u;
			this.dwWXGameCenterLoginRatio = 0u;
			this.dwGuildRatio = 0u;
			this.dwFirstWinAdd = 0u;
			this.dwGameVIPRatio = 0u;
			this.dwQQGameCenterLoginRatio = 0u;
			this.dwIOSVisitorLoginRatio = 0u;
		}
	}
}
