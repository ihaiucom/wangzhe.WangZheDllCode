using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_JOINMULTGAMERSP_SUCC : ProtocolObject
	{
		public byte bExtraType;

		public int iRoomEntity;

		public uint dwRoomID;

		public uint dwRoomSeq;

		public ulong ullSelfUid;

		public int iSelfGameEntity;

		public COMDT_ROOM_MASTER stRoomMaster;

		public COMDT_NORMALROOM_ATTR stRoomInfo;

		public COMDT_MULTROOMMEMBER_INFO stMemInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 252;

		public COMDT_JOINMULTGAMERSP_SUCC()
		{
			this.stRoomMaster = (COMDT_ROOM_MASTER)ProtocolObjectPool.Get(COMDT_ROOM_MASTER.CLASS_ID);
			this.stRoomInfo = (COMDT_NORMALROOM_ATTR)ProtocolObjectPool.Get(COMDT_NORMALROOM_ATTR.CLASS_ID);
			this.stMemInfo = (COMDT_MULTROOMMEMBER_INFO)ProtocolObjectPool.Get(COMDT_MULTROOMMEMBER_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_JOINMULTGAMERSP_SUCC.CURRVERSION < cutVer)
			{
				cutVer = COMDT_JOINMULTGAMERSP_SUCC.CURRVERSION;
			}
			if (COMDT_JOINMULTGAMERSP_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bExtraType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iRoomEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRoomID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRoomSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullSelfUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iSelfGameEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRoomMaster.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRoomInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMemInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_JOINMULTGAMERSP_SUCC.CURRVERSION < cutVer)
			{
				cutVer = COMDT_JOINMULTGAMERSP_SUCC.CURRVERSION;
			}
			if (COMDT_JOINMULTGAMERSP_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bExtraType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iRoomEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRoomID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRoomSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullSelfUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSelfGameEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRoomMaster.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRoomInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMemInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_JOINMULTGAMERSP_SUCC.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bExtraType = 0;
			this.iRoomEntity = 0;
			this.dwRoomID = 0u;
			this.dwRoomSeq = 0u;
			this.ullSelfUid = 0uL;
			this.iSelfGameEntity = 0;
			if (this.stRoomMaster != null)
			{
				this.stRoomMaster.Release();
				this.stRoomMaster = null;
			}
			if (this.stRoomInfo != null)
			{
				this.stRoomInfo.Release();
				this.stRoomInfo = null;
			}
			if (this.stMemInfo != null)
			{
				this.stMemInfo.Release();
				this.stMemInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stRoomMaster = (COMDT_ROOM_MASTER)ProtocolObjectPool.Get(COMDT_ROOM_MASTER.CLASS_ID);
			this.stRoomInfo = (COMDT_NORMALROOM_ATTR)ProtocolObjectPool.Get(COMDT_NORMALROOM_ATTR.CLASS_ID);
			this.stMemInfo = (COMDT_MULTROOMMEMBER_INFO)ProtocolObjectPool.Get(COMDT_MULTROOMMEMBER_INFO.CLASS_ID);
		}
	}
}
