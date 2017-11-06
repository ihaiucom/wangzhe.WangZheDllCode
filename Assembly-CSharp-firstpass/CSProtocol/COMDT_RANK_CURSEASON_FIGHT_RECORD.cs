using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANK_CURSEASON_FIGHT_RECORD : ProtocolObject
	{
		public uint dwSeasonId;

		public uint dwHeroId;

		public uint dwGameResult;

		public uint dwFightTime;

		public uint dwKillNum;

		public uint dwDeadNum;

		public uint dwAssistNum;

		public uint dwTalentNum;

		public uint[] Talent;

		public byte bEquipNum;

		public COMDT_INGAME_EQUIP_INFO[] astEquipDetail;

		public byte bTeamerNum;

		public byte bIsBanPick;

		public uint dwAddStarScore;

		public byte bIsMvp;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 242u;

		public static readonly uint VERSION_bEquipNum = 73u;

		public static readonly uint VERSION_astEquipDetail = 73u;

		public static readonly uint VERSION_bTeamerNum = 98u;

		public static readonly uint VERSION_bIsBanPick = 137u;

		public static readonly uint VERSION_dwAddStarScore = 181u;

		public static readonly uint VERSION_bIsMvp = 242u;

		public static readonly int CLASS_ID = 441;

		public COMDT_RANK_CURSEASON_FIGHT_RECORD()
		{
			this.Talent = new uint[5];
			this.astEquipDetail = new COMDT_INGAME_EQUIP_INFO[6];
			for (int i = 0; i < 6; i++)
			{
				this.astEquipDetail[i] = (COMDT_INGAME_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.dwSeasonId = 0u;
			this.dwHeroId = 0u;
			this.dwGameResult = 0u;
			this.dwFightTime = 0u;
			this.dwKillNum = 0u;
			this.dwDeadNum = 0u;
			this.dwAssistNum = 0u;
			this.dwTalentNum = 0u;
			this.bEquipNum = 0;
			for (int i = 0; i < 6; i++)
			{
				errorType = this.astEquipDetail[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.bTeamerNum = 1;
			this.bIsBanPick = 0;
			this.dwAddStarScore = 0u;
			this.bIsMvp = 0;
			return errorType;
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
			if (cutVer == 0u || COMDT_RANK_CURSEASON_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANK_CURSEASON_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwSeasonId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwFightTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwKillNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDeadNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwAssistNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTalentNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwTalentNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.Talent.Length < (long)((ulong)this.dwTalentNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwTalentNum))
			{
				errorType = destBuf.writeUInt32(this.Talent[num]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bEquipNum <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bEquipNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_astEquipDetail <= cutVer)
			{
				if (6 < this.bEquipNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if (this.astEquipDetail.Length < (int)this.bEquipNum)
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				for (int i = 0; i < (int)this.bEquipNum; i++)
				{
					errorType = this.astEquipDetail[i].pack(ref destBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bTeamerNum <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bTeamerNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bIsBanPick <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bIsBanPick);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_dwAddStarScore <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwAddStarScore);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bIsMvp <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bIsMvp);
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
			if (cutVer == 0u || COMDT_RANK_CURSEASON_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANK_CURSEASON_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwSeasonId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwFightTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwKillNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDeadNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAssistNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTalentNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwTalentNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.Talent = new uint[this.dwTalentNum];
			int num = 0;
			while ((long)num < (long)((ulong)this.dwTalentNum))
			{
				errorType = srcBuf.readUInt32(ref this.Talent[num]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bEquipNum <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bEquipNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bEquipNum = 0;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_astEquipDetail <= cutVer)
			{
				if (6 < this.bEquipNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				for (int i = 0; i < (int)this.bEquipNum; i++)
				{
					errorType = this.astEquipDetail[i].unpack(ref srcBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			else
			{
				if (6 < this.bEquipNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				for (int j = 0; j < (int)this.bEquipNum; j++)
				{
					errorType = this.astEquipDetail[j].construct();
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bTeamerNum <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bTeamerNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bTeamerNum = 1;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bIsBanPick <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bIsBanPick);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bIsBanPick = 0;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_dwAddStarScore <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwAddStarScore);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwAddStarScore = 0u;
			}
			if (COMDT_RANK_CURSEASON_FIGHT_RECORD.VERSION_bIsMvp <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bIsMvp);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bIsMvp = 0;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANK_CURSEASON_FIGHT_RECORD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwSeasonId = 0u;
			this.dwHeroId = 0u;
			this.dwGameResult = 0u;
			this.dwFightTime = 0u;
			this.dwKillNum = 0u;
			this.dwDeadNum = 0u;
			this.dwAssistNum = 0u;
			this.dwTalentNum = 0u;
			this.bEquipNum = 0;
			if (this.astEquipDetail != null)
			{
				for (int i = 0; i < this.astEquipDetail.Length; i++)
				{
					if (this.astEquipDetail[i] != null)
					{
						this.astEquipDetail[i].Release();
						this.astEquipDetail[i] = null;
					}
				}
			}
			this.bTeamerNum = 0;
			this.bIsBanPick = 0;
			this.dwAddStarScore = 0u;
			this.bIsMvp = 0;
		}

		public override void OnUse()
		{
			if (this.astEquipDetail != null)
			{
				for (int i = 0; i < this.astEquipDetail.Length; i++)
				{
					this.astEquipDetail[i] = (COMDT_INGAME_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
				}
			}
		}
	}
}
