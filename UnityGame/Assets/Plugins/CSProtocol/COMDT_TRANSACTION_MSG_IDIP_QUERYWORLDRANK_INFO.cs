using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO : ProtocolObject
	{
		public uint dwRankType;

		public uint dwRankSubType;

		public uint dwTotalRankNum;

		public uint dwRankNum;

		public COMDT_RANKING_LIST_ITEM_INFO[] astRankList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly int CLASS_ID = 692;

		public COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO()
		{
			this.astRankList = new COMDT_RANKING_LIST_ITEM_INFO[100];
			for (int i = 0; i < 100; i++)
			{
				this.astRankList[i] = (COMDT_RANKING_LIST_ITEM_INFO)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwRankType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRankSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTotalRankNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRankNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwRankNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astRankList.Length < (long)((ulong)this.dwRankNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwRankNum))
			{
				errorType = this.astRankList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwRankType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRankSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalRankNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRankNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwRankNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwRankNum))
			{
				errorType = this.astRankList[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwRankType = 0u;
			this.dwRankSubType = 0u;
			this.dwTotalRankNum = 0u;
			this.dwRankNum = 0u;
			if (this.astRankList != null)
			{
				for (int i = 0; i < this.astRankList.Length; i++)
				{
					if (this.astRankList[i] != null)
					{
						this.astRankList[i].Release();
						this.astRankList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRankList != null)
			{
				for (int i = 0; i < this.astRankList.Length; i++)
				{
					this.astRankList[i] = (COMDT_RANKING_LIST_ITEM_INFO)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_INFO.CLASS_ID);
				}
			}
		}
	}
}
