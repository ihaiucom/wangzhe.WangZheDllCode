using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_CHEAT_SETHEROQUALITY : ProtocolObject
	{
		public uint dwHeroID;

		public COMDT_ACNTHERO_QUALITY stQuality;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 825;

		public CSDT_CHEAT_SETHEROQUALITY()
		{
			this.stQuality = (COMDT_ACNTHERO_QUALITY)ProtocolObjectPool.Get(COMDT_ACNTHERO_QUALITY.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.dwHeroID = 0u;
			TdrError.ErrorType errorType = this.stQuality.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || CSDT_CHEAT_SETHEROQUALITY.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CHEAT_SETHEROQUALITY.CURRVERSION;
			}
			if (CSDT_CHEAT_SETHEROQUALITY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stQuality.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || CSDT_CHEAT_SETHEROQUALITY.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CHEAT_SETHEROQUALITY.CURRVERSION;
			}
			if (CSDT_CHEAT_SETHEROQUALITY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stQuality.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_CHEAT_SETHEROQUALITY.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroID = 0u;
			if (this.stQuality != null)
			{
				this.stQuality.Release();
				this.stQuality = null;
			}
		}

		public override void OnUse()
		{
			this.stQuality = (COMDT_ACNTHERO_QUALITY)ProtocolObjectPool.Get(COMDT_ACNTHERO_QUALITY.CLASS_ID);
		}
	}
}
