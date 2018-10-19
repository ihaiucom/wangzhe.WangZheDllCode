using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WORLD_RANK_ITEM : ProtocolObject
	{
		public uint dwScore;

		public uint dwTimeStamp;

		public COMDT_WORLD_RANK_ACNT stAcntInfo;

		public uint dwRankType;

		public uint dwSubType;

		public COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL stDetailInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly int CLASS_ID = 522;

		public COMDT_WORLD_RANK_ITEM()
		{
			this.stAcntInfo = (COMDT_WORLD_RANK_ACNT)ProtocolObjectPool.Get(COMDT_WORLD_RANK_ACNT.CLASS_ID);
			this.stDetailInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_WORLD_RANK_ITEM.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WORLD_RANK_ITEM.CURRVERSION;
			}
			if (COMDT_WORLD_RANK_ITEM.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTimeStamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRankType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.dwRankType);
			errorType = this.stDetailInfo.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_WORLD_RANK_ITEM.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WORLD_RANK_ITEM.CURRVERSION;
			}
			if (COMDT_WORLD_RANK_ITEM.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeStamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRankType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.dwRankType);
			errorType = this.stDetailInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_WORLD_RANK_ITEM.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwScore = 0u;
			this.dwTimeStamp = 0u;
			if (this.stAcntInfo != null)
			{
				this.stAcntInfo.Release();
				this.stAcntInfo = null;
			}
			this.dwRankType = 0u;
			this.dwSubType = 0u;
			if (this.stDetailInfo != null)
			{
				this.stDetailInfo.Release();
				this.stDetailInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stAcntInfo = (COMDT_WORLD_RANK_ACNT)ProtocolObjectPool.Get(COMDT_WORLD_RANK_ACNT.CLASS_ID);
			this.stDetailInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CLASS_ID);
		}
	}
}
