using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_MATCH_BRIEF_INFO : ProtocolObject
	{
		public byte bGrade;

		public uint dwClassOfRank;

		public COMDT_RANKDETAIL stRankDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 121u;

		public static readonly uint VERSION_dwClassOfRank = 114u;

		public static readonly int CLASS_ID = 190;

		public COMDT_ACNT_MATCH_BRIEF_INFO()
		{
			this.stRankDetail = (COMDT_RANKDETAIL)ProtocolObjectPool.Get(COMDT_RANKDETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_MATCH_BRIEF_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_MATCH_BRIEF_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_MATCH_BRIEF_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_MATCH_BRIEF_INFO.VERSION_dwClassOfRank <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stRankDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_MATCH_BRIEF_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_MATCH_BRIEF_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_MATCH_BRIEF_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_MATCH_BRIEF_INFO.VERSION_dwClassOfRank <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwClassOfRank = 0u;
			}
			errorType = this.stRankDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_MATCH_BRIEF_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bGrade = 0;
			this.dwClassOfRank = 0u;
			if (this.stRankDetail != null)
			{
				this.stRankDetail.Release();
				this.stRankDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stRankDetail = (COMDT_RANKDETAIL)ProtocolObjectPool.Get(COMDT_RANKDETAIL.CLASS_ID);
		}
	}
}
