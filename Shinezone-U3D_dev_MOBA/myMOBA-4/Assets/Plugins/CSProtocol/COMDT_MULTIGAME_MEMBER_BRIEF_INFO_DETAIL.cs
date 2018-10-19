using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL : ProtocolObject
	{
		public byte bAcntCamp;

		public byte bCamp1Num;

		public byte bCamp2Num;

		public byte bMemberNum;

		public COMDT_MULTIGAME_MEMBER_BRIEF_INFO[] astMemberDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 198;

		public COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL()
		{
			this.astMemberDetail = new COMDT_MULTIGAME_MEMBER_BRIEF_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astMemberDetail[i] = (COMDT_MULTIGAME_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_MULTIGAME_MEMBER_BRIEF_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CURRVERSION;
			}
			if (COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bAcntCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bCamp1Num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bCamp2Num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMemberNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMemberNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMemberDetail.Length < (int)this.bMemberNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMemberNum; i++)
			{
				errorType = this.astMemberDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CURRVERSION;
			}
			if (COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bAcntCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCamp1Num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCamp2Num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMemberNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMemberNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMemberNum; i++)
			{
				errorType = this.astMemberDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bAcntCamp = 0;
			this.bCamp1Num = 0;
			this.bCamp2Num = 0;
			this.bMemberNum = 0;
			if (this.astMemberDetail != null)
			{
				for (int i = 0; i < this.astMemberDetail.Length; i++)
				{
					if (this.astMemberDetail[i] != null)
					{
						this.astMemberDetail[i].Release();
						this.astMemberDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMemberDetail != null)
			{
				for (int i = 0; i < this.astMemberDetail.Length; i++)
				{
					this.astMemberDetail[i] = (COMDT_MULTIGAME_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_MULTIGAME_MEMBER_BRIEF_INFO.CLASS_ID);
				}
			}
		}
	}
}
