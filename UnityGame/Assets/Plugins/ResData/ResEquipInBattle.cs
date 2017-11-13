using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResEquipInBattle : tsf4g_csharp_interface, IUnpackable
	{
		public ushort wID;

		public ushort wEquipIDRemoteAtk;

		public byte[] szName_ByteArray;

		public byte bUsage;

		public byte bLevel;

		public byte bType;

		public ushort wGroup;

		public byte bNeedPunish;

		public byte[] szIcon_ByteArray;

		public CrypticInteger dwBuyPrice;

		public CrypticInteger dwSalePrice;

		public ushort[] PreEquipID;

		public byte[] szRequireEquip_ByteArray;

		public uint dwPhyAttack;

		public uint dwAttackSpeed;

		public uint dwCriticalHit;

		public uint dwHealthSteal;

		public uint dwMagicAttack;

		public uint dwCDReduce;

		public uint dwMagicPoint;

		public uint dwMagicRecover;

		public uint dwPhyDefence;

		public uint dwMagicDefence;

		public uint dwHealthPoint;

		public uint dwHealthRecover;

		public uint dwMoveSpeed;

		public uint dwActiveSkillID;

		public uint dwActiveSkillGroupID;

		public byte[] szActiveSkillDes_ByteArray;

		public ResPassiveSkill[] astPassiveSkill;

		public ResEffectCombine[] astEffectCombine;

		public byte[] szDesc_ByteArray;

		public byte[] szExtraDesc_ByteArray;

		public byte bInvalid;

		public byte bIsAttachEquip;

		public string szName;

		public string szIcon;

		public string szRequireEquip;

		public string szActiveSkillDes;

		public string szDesc;

		public string szExtraDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szName = 64u;

		public static readonly uint LENGTH_szIcon = 128u;

		public static readonly uint LENGTH_szRequireEquip = 64u;

		public static readonly uint LENGTH_szActiveSkillDes = 256u;

		public static readonly uint LENGTH_szDesc = 128u;

		public static readonly uint LENGTH_szExtraDesc = 128u;

		public ResEquipInBattle()
		{
			this.szName_ByteArray = new byte[1];
			this.szIcon_ByteArray = new byte[1];
			this.PreEquipID = new ushort[3];
			this.szRequireEquip_ByteArray = new byte[1];
			this.szActiveSkillDes_ByteArray = new byte[1];
			this.astPassiveSkill = new ResPassiveSkill[2];
			for (int i = 0; i < 2; i++)
			{
				this.astPassiveSkill[i] = new ResPassiveSkill();
			}
			this.astEffectCombine = new ResEffectCombine[3];
			for (int j = 0; j < 3; j++)
			{
				this.astEffectCombine[j] = new ResEffectCombine();
			}
			this.szDesc_ByteArray = new byte[1];
			this.szExtraDesc_ByteArray = new byte[1];
			this.szName = string.Empty;
			this.szIcon = string.Empty;
			this.szRequireEquip = string.Empty;
			this.szActiveSkillDes = string.Empty;
			this.szDesc = string.Empty;
			this.szExtraDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
			this.szName_ByteArray = null;
			this.szIcon = StringHelper.UTF8BytesToString(ref this.szIcon_ByteArray);
			this.szIcon_ByteArray = null;
			this.szRequireEquip = StringHelper.UTF8BytesToString(ref this.szRequireEquip_ByteArray);
			this.szRequireEquip_ByteArray = null;
			this.szActiveSkillDes = StringHelper.UTF8BytesToString(ref this.szActiveSkillDes_ByteArray);
			this.szActiveSkillDes_ByteArray = null;
			this.szDesc = StringHelper.UTF8BytesToString(ref this.szDesc_ByteArray);
			this.szDesc_ByteArray = null;
			this.szExtraDesc = StringHelper.UTF8BytesToString(ref this.szExtraDesc_ByteArray);
			this.szExtraDesc_ByteArray = null;
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			uint nValue = 0u;
			uint nValue2 = 0u;
			if (cutVer == 0u || ResEquipInBattle.CURRVERSION < cutVer)
			{
				cutVer = ResEquipInBattle.CURRVERSION;
			}
			if (ResEquipInBattle.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wEquipIDRemoteAtk);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szName_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResEquipInBattle.LENGTH_szName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szName_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bUsage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wGroup);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNeedPunish);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResEquipInBattle.LENGTH_szIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szIcon_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szIcon_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szIcon_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szIcon_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref nValue);
			this.dwBuyPrice = nValue;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref nValue2);
			this.dwSalePrice = nValue2;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = srcBuf.readUInt16(ref this.PreEquipID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			uint num5 = 0u;
			errorType = srcBuf.readUInt32(ref num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num5 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num5 > (uint)this.szRequireEquip_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResEquipInBattle.LENGTH_szRequireEquip)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRequireEquip_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRequireEquip_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRequireEquip_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szRequireEquip_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwPhyAttack);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAttackSpeed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCriticalHit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHealthSteal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicAttack);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCDReduce);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicRecover);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPhyDefence);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicDefence);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHealthPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHealthRecover);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMoveSpeed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwActiveSkillID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwActiveSkillGroupID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num7 = 0u;
			errorType = srcBuf.readUInt32(ref num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num7 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num7 > (uint)this.szActiveSkillDes_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResEquipInBattle.LENGTH_szActiveSkillDes)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szActiveSkillDes_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szActiveSkillDes_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szActiveSkillDes_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szActiveSkillDes_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int j = 0; j < 2; j++)
			{
				errorType = this.astPassiveSkill[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astEffectCombine[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			uint num9 = 0u;
			errorType = srcBuf.readUInt32(ref num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num9 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num9 > (uint)this.szDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResEquipInBattle.LENGTH_szDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szDesc_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szDesc_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szDesc_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szDesc_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num11 = 0u;
			errorType = srcBuf.readUInt32(ref num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num11 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num11 > (uint)this.szExtraDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num11 > (ulong)ResEquipInBattle.LENGTH_szExtraDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szExtraDesc_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szExtraDesc_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szExtraDesc_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szExtraDesc_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bInvalid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAttachEquip);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}

		public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.load(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
		{
			uint nValue = 0u;
			uint nValue2 = 0u;
			srcBuf.disableEndian();
			if (cutVer == 0u || ResEquipInBattle.CURRVERSION < cutVer)
			{
				cutVer = ResEquipInBattle.CURRVERSION;
			}
			if (ResEquipInBattle.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wEquipIDRemoteAtk);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 64;
			if (this.szName_ByteArray.GetLength(0) < num)
			{
				this.szName_ByteArray = new byte[ResEquipInBattle.LENGTH_szName];
			}
			errorType = srcBuf.readCString(ref this.szName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bUsage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wGroup);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNeedPunish);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szIcon_ByteArray.GetLength(0) < num2)
			{
				this.szIcon_ByteArray = new byte[ResEquipInBattle.LENGTH_szIcon];
			}
			errorType = srcBuf.readCString(ref this.szIcon_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref nValue);
			this.dwBuyPrice = nValue;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref nValue2);
			this.dwSalePrice = nValue2;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = srcBuf.readUInt16(ref this.PreEquipID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num3 = 64;
			if (this.szRequireEquip_ByteArray.GetLength(0) < num3)
			{
				this.szRequireEquip_ByteArray = new byte[ResEquipInBattle.LENGTH_szRequireEquip];
			}
			errorType = srcBuf.readCString(ref this.szRequireEquip_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPhyAttack);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAttackSpeed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCriticalHit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHealthSteal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicAttack);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCDReduce);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicRecover);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPhyDefence);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMagicDefence);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHealthPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHealthRecover);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMoveSpeed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwActiveSkillID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwActiveSkillGroupID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 256;
			if (this.szActiveSkillDes_ByteArray.GetLength(0) < num4)
			{
				this.szActiveSkillDes_ByteArray = new byte[ResEquipInBattle.LENGTH_szActiveSkillDes];
			}
			errorType = srcBuf.readCString(ref this.szActiveSkillDes_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 2; j++)
			{
				errorType = this.astPassiveSkill[j].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astEffectCombine[k].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num5 = 128;
			if (this.szDesc_ByteArray.GetLength(0) < num5)
			{
				this.szDesc_ByteArray = new byte[ResEquipInBattle.LENGTH_szDesc];
			}
			errorType = srcBuf.readCString(ref this.szDesc_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szExtraDesc_ByteArray.GetLength(0) < num6)
			{
				this.szExtraDesc_ByteArray = new byte[ResEquipInBattle.LENGTH_szExtraDesc];
			}
			errorType = srcBuf.readCString(ref this.szExtraDesc_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bInvalid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAttachEquip);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
