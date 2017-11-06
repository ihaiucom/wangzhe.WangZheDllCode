using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_FIGHTER_DETAIL : ProtocolObject
	{
		public uint dwRank;

		public COMDT_ARENA_BLOCKDATA stFigterData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 509;

		public COMDT_ARENA_FIGHTER_DETAIL()
		{
			this.stFigterData = (COMDT_ARENA_BLOCKDATA)ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.dwRank = 0u;
			TdrError.ErrorType errorType = this.stFigterData.construct();
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
			if (cutVer == 0u || COMDT_ARENA_FIGHTER_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHTER_DETAIL.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHTER_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFigterData.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ARENA_FIGHTER_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHTER_DETAIL.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHTER_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFigterData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ARENA_FIGHTER_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwRank = 0u;
			if (this.stFigterData != null)
			{
				this.stFigterData.Release();
				this.stFigterData = null;
			}
		}

		public override void OnUse()
		{
			this.stFigterData = (COMDT_ARENA_BLOCKDATA)ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
		}
	}
}
