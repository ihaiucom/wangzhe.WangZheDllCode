using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP : ProtocolObject
	{
		public byte bEquipNum;

		public uint[] EquipID;

		public uint dwWinRate;

		public uint dwEvalScore;

		public byte bEvalNum;

		public byte[] szEvalID;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 166u;

		public static readonly uint VERSION_dwWinRate = 166u;

		public static readonly uint VERSION_dwEvalScore = 166u;

		public static readonly uint VERSION_bEvalNum = 166u;

		public static readonly uint VERSION_szEvalID = 166u;

		public static readonly int CLASS_ID = 515;

		public COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP()
		{
			this.EquipID = new uint[6];
			this.szEvalID = new byte[15];
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.EquipID.Length < (int)this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bEquipNum; i++)
			{
				errorType = destBuf.writeUInt32(this.EquipID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_dwWinRate <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWinRate);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_dwEvalScore <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwEvalScore);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_bEvalNum <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bEvalNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_szEvalID <= cutVer)
			{
				if (15 < this.bEvalNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if (this.szEvalID.Length < (int)this.bEvalNum)
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				for (int j = 0; j < (int)this.bEvalNum; j++)
				{
					errorType = destBuf.writeUInt8(this.szEvalID[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.EquipID = new uint[(int)this.bEquipNum];
			for (int i = 0; i < (int)this.bEquipNum; i++)
			{
				errorType = srcBuf.readUInt32(ref this.EquipID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_dwWinRate <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWinRate);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWinRate = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_dwEvalScore <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwEvalScore);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwEvalScore = 0u;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_bEvalNum <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bEvalNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bEvalNum = 0;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.VERSION_szEvalID <= cutVer)
			{
				if (15 < this.bEvalNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				this.szEvalID = new byte[(int)this.bEvalNum];
				for (int j = 0; j < (int)this.bEvalNum; j++)
				{
					errorType = srcBuf.readUInt8(ref this.szEvalID[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			else if (15 < this.bEvalNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bEquipNum = 0;
			this.dwWinRate = 0u;
			this.dwEvalScore = 0u;
			this.bEvalNum = 0;
		}
	}
}
