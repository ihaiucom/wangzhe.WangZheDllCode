using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ROOMMEMBER_DT : ProtocolObject
	{
		public uint dwPosOfCamp;

		public uint dwRoomMemberType;

		public COMDT_ROOM_MEMBER_DETAIL stMemberDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 52;

		public COMDT_ROOMMEMBER_DT()
		{
			this.stMemberDetail = (COMDT_ROOM_MEMBER_DETAIL)ProtocolObjectPool.Get(COMDT_ROOM_MEMBER_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ROOMMEMBER_DT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ROOMMEMBER_DT.CURRVERSION;
			}
			if (COMDT_ROOMMEMBER_DT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwPosOfCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRoomMemberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.dwRoomMemberType);
			errorType = this.stMemberDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ROOMMEMBER_DT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ROOMMEMBER_DT.CURRVERSION;
			}
			if (COMDT_ROOMMEMBER_DT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwPosOfCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRoomMemberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.dwRoomMemberType);
			errorType = this.stMemberDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ROOMMEMBER_DT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwPosOfCamp = 0u;
			this.dwRoomMemberType = 0u;
			if (this.stMemberDetail != null)
			{
				this.stMemberDetail.Release();
				this.stMemberDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stMemberDetail = (COMDT_ROOM_MEMBER_DETAIL)ProtocolObjectPool.Get(COMDT_ROOM_MEMBER_DETAIL.CLASS_ID);
		}
	}
}
