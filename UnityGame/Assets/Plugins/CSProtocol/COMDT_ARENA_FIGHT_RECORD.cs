using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_FIGHT_RECORD : ProtocolObject
	{
		public byte bResult;

		public ulong ullAtkerUid;

		public uint dwFightTime;

		public uint dwAtkerRank;

		public uint dwTargetRank;

		public COMDT_ARENA_BLOCKDATA stTargetInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 418;

		public COMDT_ARENA_FIGHT_RECORD()
		{
			this.stTargetInfo = (COMDT_ARENA_BLOCKDATA)ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.bResult = 0;
			this.ullAtkerUid = 0uL;
			this.dwFightTime = 0u;
			this.dwAtkerRank = 0u;
			this.dwTargetRank = 0u;
			TdrError.ErrorType errorType = this.stTargetInfo.construct();
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
			if (cutVer == 0u || COMDT_ARENA_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullAtkerUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwFightTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwAtkerRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTargetRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTargetInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ARENA_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_ARENA_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullAtkerUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwFightTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAtkerRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTargetRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTargetInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ARENA_FIGHT_RECORD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bResult = 0;
			this.ullAtkerUid = 0uL;
			this.dwFightTime = 0u;
			this.dwAtkerRank = 0u;
			this.dwTargetRank = 0u;
			if (this.stTargetInfo != null)
			{
				this.stTargetInfo.Release();
				this.stTargetInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stTargetInfo = (COMDT_ARENA_BLOCKDATA)ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
		}
	}
}
