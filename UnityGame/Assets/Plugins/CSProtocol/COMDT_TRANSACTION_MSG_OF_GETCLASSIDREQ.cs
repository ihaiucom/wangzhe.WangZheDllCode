using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ : ProtocolObject
	{
		public byte bGradeId;

		public byte bFriendRankInfoNum;

		public COMDT_FRIEND_RANK_INFO[] astFriendRankInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 657;

		public COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ()
		{
			this.astFriendRankInfo = new COMDT_FRIEND_RANK_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astFriendRankInfo[i] = (COMDT_FRIEND_RANK_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_RANK_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bGradeId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bFriendRankInfoNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bFriendRankInfoNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astFriendRankInfo.Length < (int)this.bFriendRankInfoNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bFriendRankInfoNum; i++)
			{
				errorType = this.astFriendRankInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bGradeId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFriendRankInfoNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bFriendRankInfoNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bFriendRankInfoNum; i++)
			{
				errorType = this.astFriendRankInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bGradeId = 0;
			this.bFriendRankInfoNum = 0;
			if (this.astFriendRankInfo != null)
			{
				for (int i = 0; i < this.astFriendRankInfo.Length; i++)
				{
					if (this.astFriendRankInfo[i] != null)
					{
						this.astFriendRankInfo[i].Release();
						this.astFriendRankInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astFriendRankInfo != null)
			{
				for (int i = 0; i < this.astFriendRankInfo.Length; i++)
				{
					this.astFriendRankInfo[i] = (COMDT_FRIEND_RANK_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_RANK_INFO.CLASS_ID);
				}
			}
		}
	}
}
