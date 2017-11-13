using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RANKING_LIST_SUCC : ProtocolObject
	{
		public int iLogicWorldID;

		public uint dwTimeLimit;

		public byte bNumberType;

		public int iStart;

		public int iLimit;

		public byte bImage;

		public uint dwItemNum;

		public CSDT_RANKING_LIST_ITEM_INFO[] astItemDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly int CLASS_ID = 1219;

		public CSDT_RANKING_LIST_SUCC()
		{
			this.astItemDetail = new CSDT_RANKING_LIST_ITEM_INFO[100];
			for (int i = 0; i < 100; i++)
			{
				this.astItemDetail[i] = (CSDT_RANKING_LIST_ITEM_INFO)ProtocolObjectPool.Get(CSDT_RANKING_LIST_ITEM_INFO.CLASS_ID);
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
			if (cutVer == 0u || CSDT_RANKING_LIST_SUCC.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RANKING_LIST_SUCC.CURRVERSION;
			}
			if (CSDT_RANKING_LIST_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iLogicWorldID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTimeLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNumberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iStart);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bImage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwItemNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwItemNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astItemDetail.Length < (long)((ulong)this.dwItemNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwItemNum))
			{
				errorType = this.astItemDetail[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_RANKING_LIST_SUCC.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RANKING_LIST_SUCC.CURRVERSION;
			}
			if (CSDT_RANKING_LIST_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iLogicWorldID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNumberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iStart);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bImage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwItemNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwItemNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwItemNum))
			{
				errorType = this.astItemDetail[num].unpack(ref srcBuf, cutVer);
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
			return CSDT_RANKING_LIST_SUCC.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iLogicWorldID = 0;
			this.dwTimeLimit = 0u;
			this.bNumberType = 0;
			this.iStart = 0;
			this.iLimit = 0;
			this.bImage = 0;
			this.dwItemNum = 0u;
			if (this.astItemDetail != null)
			{
				for (int i = 0; i < this.astItemDetail.Length; i++)
				{
					if (this.astItemDetail[i] != null)
					{
						this.astItemDetail[i].Release();
						this.astItemDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astItemDetail != null)
			{
				for (int i = 0; i < this.astItemDetail.Length; i++)
				{
					this.astItemDetail[i] = (CSDT_RANKING_LIST_ITEM_INFO)ProtocolObjectPool.Get(CSDT_RANKING_LIST_ITEM_INFO.CLASS_ID);
				}
			}
		}
	}
}
