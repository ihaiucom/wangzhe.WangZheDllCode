using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_LIST_FRIENDREQ : ProtocolObject
	{
		public uint dwFriendReqNum;

		public CSDT_VERIFICATION_INFO[] astVerificationList;

		public uint dwResult;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1014;

		public SCPKG_CMD_LIST_FRIENDREQ()
		{
			this.astVerificationList = new CSDT_VERIFICATION_INFO[220];
			for (int i = 0; i < 220; i++)
			{
				this.astVerificationList[i] = (CSDT_VERIFICATION_INFO)ProtocolObjectPool.Get(CSDT_VERIFICATION_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CMD_LIST_FRIENDREQ.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_LIST_FRIENDREQ.CURRVERSION;
			}
			if (SCPKG_CMD_LIST_FRIENDREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwFriendReqNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (220u < this.dwFriendReqNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astVerificationList.Length < (long)((ulong)this.dwFriendReqNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwFriendReqNum))
			{
				errorType = this.astVerificationList[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.writeUInt32(this.dwResult);
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
			if (cutVer == 0u || SCPKG_CMD_LIST_FRIENDREQ.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_LIST_FRIENDREQ.CURRVERSION;
			}
			if (SCPKG_CMD_LIST_FRIENDREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwFriendReqNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (220u < this.dwFriendReqNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwFriendReqNum))
			{
				errorType = this.astVerificationList[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = srcBuf.readUInt32(ref this.dwResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_LIST_FRIENDREQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwFriendReqNum = 0u;
			if (this.astVerificationList != null)
			{
				for (int i = 0; i < this.astVerificationList.Length; i++)
				{
					if (this.astVerificationList[i] != null)
					{
						this.astVerificationList[i].Release();
						this.astVerificationList[i] = null;
					}
				}
			}
			this.dwResult = 0u;
		}

		public override void OnUse()
		{
			if (this.astVerificationList != null)
			{
				for (int i = 0; i < this.astVerificationList.Length; i++)
				{
					this.astVerificationList[i] = (CSDT_VERIFICATION_INFO)ProtocolObjectPool.Get(CSDT_VERIFICATION_INFO.CLASS_ID);
				}
			}
		}
	}
}
