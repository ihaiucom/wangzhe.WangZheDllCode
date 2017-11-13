using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PLAYERINFO_OF_ACNT : ProtocolObject
	{
		public uint dwHeadId;

		public ulong ullUid;

		public int iLogicWorldID;

		public uint dwPvpLevel;

		public COMDT_GAME_VIP_CLIENT stGameVip;

		public int iHonorID;

		public int iHonorLevel;

		public uint dwWangZheCnt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly uint VERSION_stGameVip = 42u;

		public static readonly uint VERSION_iHonorID = 103u;

		public static readonly uint VERSION_iHonorLevel = 103u;

		public static readonly uint VERSION_dwWangZheCnt = 241u;

		public static readonly int CLASS_ID = 130;

		public COMDT_PLAYERINFO_OF_ACNT()
		{
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PLAYERINFO_OF_ACNT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PLAYERINFO_OF_ACNT.CURRVERSION;
			}
			if (COMDT_PLAYERINFO_OF_ACNT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeadId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLogicWorldID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPvpLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_stGameVip <= cutVer)
			{
				errorType = this.stGameVip.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_iHonorID <= cutVer)
			{
				errorType = destBuf.writeInt32(this.iHonorID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_iHonorLevel <= cutVer)
			{
				errorType = destBuf.writeInt32(this.iHonorLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_dwWangZheCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWangZheCnt);
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
			if (cutVer == 0u || COMDT_PLAYERINFO_OF_ACNT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PLAYERINFO_OF_ACNT.CURRVERSION;
			}
			if (COMDT_PLAYERINFO_OF_ACNT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeadId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLogicWorldID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_stGameVip <= cutVer)
			{
				errorType = this.stGameVip.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stGameVip.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_iHonorID <= cutVer)
			{
				errorType = srcBuf.readInt32(ref this.iHonorID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.iHonorID = 0;
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_iHonorLevel <= cutVer)
			{
				errorType = srcBuf.readInt32(ref this.iHonorLevel);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.iHonorLevel = 0;
			}
			if (COMDT_PLAYERINFO_OF_ACNT.VERSION_dwWangZheCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWangZheCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWangZheCnt = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PLAYERINFO_OF_ACNT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeadId = 0u;
			this.ullUid = 0uL;
			this.iLogicWorldID = 0;
			this.dwPvpLevel = 0u;
			if (this.stGameVip != null)
			{
				this.stGameVip.Release();
				this.stGameVip = null;
			}
			this.iHonorID = 0;
			this.iHonorLevel = 0;
			this.dwWangZheCnt = 0u;
		}

		public override void OnUse()
		{
			this.stGameVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
		}
	}
}
