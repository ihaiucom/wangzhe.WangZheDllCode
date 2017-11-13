using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SELFDEFINE_EQUIP_INFO : ProtocolObject
	{
		public uint dwLastChgHeroId;

		public ushort wHeroNum;

		public COMDT_HERO_EQUIPLIST[] astEquipInfoList;

		public uint dwHeroNumNew;

		public COMDT_HERO_EQUIPLIST_NEW[] astEquipInfoListNew;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 207u;

		public static readonly uint VERSION_dwHeroNumNew = 207u;

		public static readonly uint VERSION_astEquipInfoListNew = 207u;

		public static readonly int CLASS_ID = 128;

		public COMDT_SELFDEFINE_EQUIP_INFO()
		{
			this.astEquipInfoList = new COMDT_HERO_EQUIPLIST[200];
			for (int i = 0; i < 200; i++)
			{
				this.astEquipInfoList[i] = (COMDT_HERO_EQUIPLIST)ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST.CLASS_ID);
			}
			this.astEquipInfoListNew = new COMDT_HERO_EQUIPLIST_NEW[200];
			for (int j = 0; j < 200; j++)
			{
				this.astEquipInfoListNew[j] = (COMDT_HERO_EQUIPLIST_NEW)ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST_NEW.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SELFDEFINE_EQUIP_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SELFDEFINE_EQUIP_INFO.CURRVERSION;
			}
			if (COMDT_SELFDEFINE_EQUIP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastChgHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200 < this.wHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astEquipInfoList.Length < (int)this.wHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wHeroNum; i++)
			{
				errorType = this.astEquipInfoList[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_SELFDEFINE_EQUIP_INFO.VERSION_dwHeroNumNew <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwHeroNumNew);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_SELFDEFINE_EQUIP_INFO.VERSION_astEquipInfoListNew <= cutVer)
			{
				if (200u < this.dwHeroNumNew)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if ((long)this.astEquipInfoListNew.Length < (long)((ulong)this.dwHeroNumNew))
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				int num = 0;
				while ((long)num < (long)((ulong)this.dwHeroNumNew))
				{
					errorType = this.astEquipInfoListNew[num].pack(ref destBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					num++;
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
			if (cutVer == 0u || COMDT_SELFDEFINE_EQUIP_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SELFDEFINE_EQUIP_INFO.CURRVERSION;
			}
			if (COMDT_SELFDEFINE_EQUIP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastChgHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200 < this.wHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wHeroNum; i++)
			{
				errorType = this.astEquipInfoList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_SELFDEFINE_EQUIP_INFO.VERSION_dwHeroNumNew <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwHeroNumNew);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwHeroNumNew = 0u;
			}
			if (COMDT_SELFDEFINE_EQUIP_INFO.VERSION_astEquipInfoListNew <= cutVer)
			{
				if (200u < this.dwHeroNumNew)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				int num = 0;
				while ((long)num < (long)((ulong)this.dwHeroNumNew))
				{
					errorType = this.astEquipInfoListNew[num].unpack(ref srcBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					num++;
				}
			}
			else
			{
				if (200u < this.dwHeroNumNew)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				int num2 = 0;
				while ((long)num2 < (long)((ulong)this.dwHeroNumNew))
				{
					errorType = this.astEquipInfoListNew[num2].construct();
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					num2++;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SELFDEFINE_EQUIP_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastChgHeroId = 0u;
			this.wHeroNum = 0;
			if (this.astEquipInfoList != null)
			{
				for (int i = 0; i < this.astEquipInfoList.Length; i++)
				{
					if (this.astEquipInfoList[i] != null)
					{
						this.astEquipInfoList[i].Release();
						this.astEquipInfoList[i] = null;
					}
				}
			}
			this.dwHeroNumNew = 0u;
			if (this.astEquipInfoListNew != null)
			{
				for (int j = 0; j < this.astEquipInfoListNew.Length; j++)
				{
					if (this.astEquipInfoListNew[j] != null)
					{
						this.astEquipInfoListNew[j].Release();
						this.astEquipInfoListNew[j] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astEquipInfoList != null)
			{
				for (int i = 0; i < this.astEquipInfoList.Length; i++)
				{
					this.astEquipInfoList[i] = (COMDT_HERO_EQUIPLIST)ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST.CLASS_ID);
				}
			}
			if (this.astEquipInfoListNew != null)
			{
				for (int j = 0; j < this.astEquipInfoListNew.Length; j++)
				{
					this.astEquipInfoListNew[j] = (COMDT_HERO_EQUIPLIST_NEW)ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST_NEW.CLASS_ID);
				}
			}
		}
	}
}
