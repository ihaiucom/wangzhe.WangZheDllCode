using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_NTF_RECALL_FRIEND : ProtocolObject
	{
		public ushort wRecallNum;

		public COMDT_RECALL_FRIEND[] astRecallAcntList;

		public byte bIntimacyRelationPrior;

		public uint dwRecruiterRewardBits;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1065;

		public SCPKG_CMD_NTF_RECALL_FRIEND()
		{
			this.astRecallAcntList = new COMDT_RECALL_FRIEND[500];
			for (int i = 0; i < 500; i++)
			{
				this.astRecallAcntList[i] = (COMDT_RECALL_FRIEND)ProtocolObjectPool.Get(COMDT_RECALL_FRIEND.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CMD_NTF_RECALL_FRIEND.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_NTF_RECALL_FRIEND.CURRVERSION;
			}
			if (SCPKG_CMD_NTF_RECALL_FRIEND.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wRecallNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500 < this.wRecallNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRecallAcntList.Length < (int)this.wRecallNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wRecallNum; i++)
			{
				errorType = this.astRecallAcntList[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bIntimacyRelationPrior);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRecruiterRewardBits);
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
			if (cutVer == 0u || SCPKG_CMD_NTF_RECALL_FRIEND.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_NTF_RECALL_FRIEND.CURRVERSION;
			}
			if (SCPKG_CMD_NTF_RECALL_FRIEND.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wRecallNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500 < this.wRecallNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wRecallNum; i++)
			{
				errorType = this.astRecallAcntList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bIntimacyRelationPrior);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRecruiterRewardBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_NTF_RECALL_FRIEND.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wRecallNum = 0;
			if (this.astRecallAcntList != null)
			{
				for (int i = 0; i < this.astRecallAcntList.Length; i++)
				{
					if (this.astRecallAcntList[i] != null)
					{
						this.astRecallAcntList[i].Release();
						this.astRecallAcntList[i] = null;
					}
				}
			}
			this.bIntimacyRelationPrior = 0;
			this.dwRecruiterRewardBits = 0u;
		}

		public override void OnUse()
		{
			if (this.astRecallAcntList != null)
			{
				for (int i = 0; i < this.astRecallAcntList.Length; i++)
				{
					this.astRecallAcntList[i] = (COMDT_RECALL_FRIEND)ProtocolObjectPool.Get(COMDT_RECALL_FRIEND.CLASS_ID);
				}
			}
		}
	}
}
