using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO : ProtocolObject
	{
		public uint dwWinCnt;

		public uint dwGameCnt;

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAcntInfo;

		public COMDT_EQUIP_LIST stEquipList;

		public COMDT_SYMBOLPAGE_INFO stSymbolPageInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 518;

		public COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO()
		{
			this.stAcntInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
			this.stEquipList = (COMDT_EQUIP_LIST)ProtocolObjectPool.Get(COMDT_EQUIP_LIST.CLASS_ID);
			this.stSymbolPageInfo = (COMDT_SYMBOLPAGE_INFO)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEquipList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolPageInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEquipList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolPageInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwWinCnt = 0u;
			this.dwGameCnt = 0u;
			if (this.stAcntInfo != null)
			{
				this.stAcntInfo.Release();
				this.stAcntInfo = null;
			}
			if (this.stEquipList != null)
			{
				this.stEquipList.Release();
				this.stEquipList = null;
			}
			if (this.stSymbolPageInfo != null)
			{
				this.stSymbolPageInfo.Release();
				this.stSymbolPageInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stAcntInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
			this.stEquipList = (COMDT_EQUIP_LIST)ProtocolObjectPool.Get(COMDT_EQUIP_LIST.CLASS_ID);
			this.stSymbolPageInfo = (COMDT_SYMBOLPAGE_INFO)ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_INFO.CLASS_ID);
		}
	}
}
