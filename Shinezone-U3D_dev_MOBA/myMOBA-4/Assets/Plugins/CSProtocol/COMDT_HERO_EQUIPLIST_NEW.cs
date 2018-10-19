using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_EQUIPLIST_NEW : ProtocolObject
	{
		public uint dwHeroId;

		public uint dwCurUsed;

		public uint dwEquipNum;

		public COMDT_HERO_EQUIP_DETAIL[] astEquipList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 126;

		public COMDT_HERO_EQUIPLIST_NEW()
		{
			this.astEquipList = new COMDT_HERO_EQUIP_DETAIL[3];
			for (int i = 0; i < 3; i++)
			{
				this.astEquipList[i] = (COMDT_HERO_EQUIP_DETAIL)ProtocolObjectPool.Get(COMDT_HERO_EQUIP_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_HERO_EQUIPLIST_NEW.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_EQUIPLIST_NEW.CURRVERSION;
			}
			if (COMDT_HERO_EQUIPLIST_NEW.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCurUsed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3u < this.dwEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astEquipList.Length < (long)((ulong)this.dwEquipNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwEquipNum))
			{
				errorType = this.astEquipList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_HERO_EQUIPLIST_NEW.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_EQUIPLIST_NEW.CURRVERSION;
			}
			if (COMDT_HERO_EQUIPLIST_NEW.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCurUsed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3u < this.dwEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwEquipNum))
			{
				errorType = this.astEquipList[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_HERO_EQUIPLIST_NEW.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroId = 0u;
			this.dwCurUsed = 0u;
			this.dwEquipNum = 0u;
			if (this.astEquipList != null)
			{
				for (int i = 0; i < this.astEquipList.Length; i++)
				{
					if (this.astEquipList[i] != null)
					{
						this.astEquipList[i].Release();
						this.astEquipList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astEquipList != null)
			{
				for (int i = 0; i < this.astEquipList.Length; i++)
				{
					this.astEquipList[i] = (COMDT_HERO_EQUIP_DETAIL)ProtocolObjectPool.Get(COMDT_HERO_EQUIP_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
