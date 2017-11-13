using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CREATE_TVOIP_ROOM_NTF : ProtocolObject
	{
		public ulong ullRoomID;

		public ulong ullRoomKey;

		public ushort wAccessIPCount;

		public CSDT_TVOIP_IP_INFO[] astAccessIPList;

		public uint dwRoomUserCnt;

		public CSDT_TVOIP_ROOM_USER_INFO[] astRoomUserList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1468;

		public SCPKG_CREATE_TVOIP_ROOM_NTF()
		{
			this.astAccessIPList = new CSDT_TVOIP_IP_INFO[16];
			for (int i = 0; i < 16; i++)
			{
				this.astAccessIPList[i] = (CSDT_TVOIP_IP_INFO)ProtocolObjectPool.Get(CSDT_TVOIP_IP_INFO.CLASS_ID);
			}
			this.astRoomUserList = new CSDT_TVOIP_ROOM_USER_INFO[16];
			for (int j = 0; j < 16; j++)
			{
				this.astRoomUserList[j] = (CSDT_TVOIP_ROOM_USER_INFO)ProtocolObjectPool.Get(CSDT_TVOIP_ROOM_USER_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CREATE_TVOIP_ROOM_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CREATE_TVOIP_ROOM_NTF.CURRVERSION;
			}
			if (SCPKG_CREATE_TVOIP_ROOM_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullRoomID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullRoomKey);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wAccessIPCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (16 < this.wAccessIPCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astAccessIPList.Length < (int)this.wAccessIPCount)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wAccessIPCount; i++)
			{
				errorType = this.astAccessIPList[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwRoomUserCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (16u < this.dwRoomUserCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astRoomUserList.Length < (long)((ulong)this.dwRoomUserCnt))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwRoomUserCnt))
			{
				errorType = this.astRoomUserList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_CREATE_TVOIP_ROOM_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CREATE_TVOIP_ROOM_NTF.CURRVERSION;
			}
			if (SCPKG_CREATE_TVOIP_ROOM_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullRoomID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullRoomKey);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wAccessIPCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (16 < this.wAccessIPCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wAccessIPCount; i++)
			{
				errorType = this.astAccessIPList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwRoomUserCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (16u < this.dwRoomUserCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwRoomUserCnt))
			{
				errorType = this.astRoomUserList[num].unpack(ref srcBuf, cutVer);
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
			return SCPKG_CREATE_TVOIP_ROOM_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullRoomID = 0uL;
			this.ullRoomKey = 0uL;
			this.wAccessIPCount = 0;
			if (this.astAccessIPList != null)
			{
				for (int i = 0; i < this.astAccessIPList.Length; i++)
				{
					if (this.astAccessIPList[i] != null)
					{
						this.astAccessIPList[i].Release();
						this.astAccessIPList[i] = null;
					}
				}
			}
			this.dwRoomUserCnt = 0u;
			if (this.astRoomUserList != null)
			{
				for (int j = 0; j < this.astRoomUserList.Length; j++)
				{
					if (this.astRoomUserList[j] != null)
					{
						this.astRoomUserList[j].Release();
						this.astRoomUserList[j] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astAccessIPList != null)
			{
				for (int i = 0; i < this.astAccessIPList.Length; i++)
				{
					this.astAccessIPList[i] = (CSDT_TVOIP_IP_INFO)ProtocolObjectPool.Get(CSDT_TVOIP_IP_INFO.CLASS_ID);
				}
			}
			if (this.astRoomUserList != null)
			{
				for (int j = 0; j < this.astRoomUserList.Length; j++)
				{
					this.astRoomUserList[j] = (CSDT_TVOIP_ROOM_USER_INFO)ProtocolObjectPool.Get(CSDT_TVOIP_ROOM_USER_INFO.CLASS_ID);
				}
			}
		}
	}
}
