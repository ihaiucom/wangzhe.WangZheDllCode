using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SETTLE_GAME_GENERAL_INFO : ProtocolObject
	{
		public uint dwFBTime;

		public byte bKillDragonNum;

		public uint[] KillDragonTime;

		public byte bKillBigDragonNum;

		public uint[] KillBigDragonTime;

		public byte bRuneTypeNum;

		public COMDT_RUNE_PICK_UP_INFO[] astRuneTypePickUpNum;

		public byte bGame30SecNum;

		public COMDT_GAME_30SEC_INFO[] astGame30SecDetail;

		public byte bEquipNum;

		public COMDT_INGAME_EQUIP_INFO[] astEquipDetail;

		public uint dwTotalInGameCoin;

		public uint dwMaxInGameCoin;

		public uint dwCamp1TowerFirstAttackTime;

		public uint dwCamp2TowerFirstAttackTime;

		public uint dwUpRoadTower1DesTime;

		public uint dwUpRoadTower2DesTime;

		public uint dwUpRoadTower3DesTime;

		public uint dwMidRoadTower1DesTime;

		public uint dwMidRoadTower2DesTime;

		public uint dwMidRoadTower3DesTime;

		public uint dwDownRoadTower1DesTime;

		public uint dwDownRoadTower2DesTime;

		public uint dwDownRoadTower3DesTime;

		public int iTimeUse;

		public int iPauseTimeTotal;

		public int iBuildingAttackRange;

		public int iBuildingAttackDamageMax;

		public int iBuildingAttackDamageMin;

		public int iBuildingHPMax;

		public int iBuildingHPMin;

		public int iBuildingHurtCount;

		public int iBuildingHurtMax;

		public int iBuildingHurtMin;

		public int iBuildingHurtTotal;

		public int iExperienceHPAdd1;

		public int iExperienceHPAdd2;

		public int iExperienceHPAdd3;

		public int iExperienceHPAdd4;

		public int iExperienceHPAddTotal;

		public int iBossCount;

		public int iBossHPMax;

		public int iBossHPMin;

		public int iBossHurtMax;

		public int iBossHurtMin;

		public int iBossHurtTotal;

		public int iEnemyBuildingHPMax;

		public int iEnemyBuildingHPMin;

		public int iEnemyBuildingHurtMax;

		public int iEnemyBuildingHurtMin;

		public int iEnemyBuildingHurtTotal;

		public int iEnemyHPMax;

		public int iEnemyHPMin;

		public int iEnemyKIllHeroGap;

		public int iBossAttackCount;

		public int iBossUseSkillCount;

		public int iBossAttackMax;

		public int iBossAttackMin;

		public int iBossSkillDamageMax;

		public int iBossSkillDamageMin;

		public int iBossAttackTotal;

		public int iEnemyAttackMax;

		public int iEnemyAttackMin;

		public int iEnemyBuildingAttackRange;

		public int iEnemyBuildingDamageMax;

		public int iEnemyBuildingDamageMin;

		public int iCommunicationCount1;

		public int iCommunicationCount2;

		public int iMaxSoldierCnt;

		public int iMaxTowerAttackDistance;

		public byte bSelfCampKillTowerCnt;

		public byte bQuickBuyItemCnt;

		public byte bPanelBuyItemCnt;

		public byte bSelfCampKillBaseCnt;

		public uint dwSelfCampBaseBlood;

		public int iCurrentDisparity;

		public byte bSelfCampHaveWinningFlag;

		public uint dwBoardSortNum;

		public uint dwClickNum;

		public uint[] ClickDetail;

		public byte bAdvantageNum;

		public COMDT_INGAME_ADVANTAGE_INFO[] astAdvantageDetail;

		public uint dwOperateNum;

		public uint[] OperateType;

		public uint dwBigDragonBattleToDeadTimeNum;

		public uint[] BigDragonBattleToDeadTimeDtail;

		public uint dwDragonBattleToDeadTimeNum;

		public uint[] DragonBattleToDeadTimeDtail;

		public uint dwMonsterDeadNum;

		public uint[] MonsterDeadDetail;

		public uint dwRongyuFlag;

		public ulong ullVisibleBits;

		public byte bCarrier;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 173;

		public COMDT_SETTLE_GAME_GENERAL_INFO()
		{
			this.KillDragonTime = new uint[10];
			this.KillBigDragonTime = new uint[10];
			this.astRuneTypePickUpNum = new COMDT_RUNE_PICK_UP_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astRuneTypePickUpNum[i] = (COMDT_RUNE_PICK_UP_INFO)ProtocolObjectPool.Get(COMDT_RUNE_PICK_UP_INFO.CLASS_ID);
			}
			this.astGame30SecDetail = new COMDT_GAME_30SEC_INFO[60];
			for (int j = 0; j < 60; j++)
			{
				this.astGame30SecDetail[j] = (COMDT_GAME_30SEC_INFO)ProtocolObjectPool.Get(COMDT_GAME_30SEC_INFO.CLASS_ID);
			}
			this.astEquipDetail = new COMDT_INGAME_EQUIP_INFO[6];
			for (int k = 0; k < 6; k++)
			{
				this.astEquipDetail[k] = (COMDT_INGAME_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
			}
			this.ClickDetail = new uint[30];
			this.astAdvantageDetail = new COMDT_INGAME_ADVANTAGE_INFO[16];
			for (int l = 0; l < 16; l++)
			{
				this.astAdvantageDetail[l] = (COMDT_INGAME_ADVANTAGE_INFO)ProtocolObjectPool.Get(COMDT_INGAME_ADVANTAGE_INFO.CLASS_ID);
			}
			this.OperateType = new uint[20];
			this.BigDragonBattleToDeadTimeDtail = new uint[10];
			this.DragonBattleToDeadTimeDtail = new uint[10];
			this.MonsterDeadDetail = new uint[20];
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
			if (cutVer == 0u || COMDT_SETTLE_GAME_GENERAL_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_GAME_GENERAL_INFO.CURRVERSION;
			}
			if (COMDT_SETTLE_GAME_GENERAL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwFBTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bKillDragonNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bKillDragonNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.KillDragonTime.Length < (int)this.bKillDragonNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bKillDragonNum; i++)
			{
				errorType = destBuf.writeUInt32(this.KillDragonTime[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bKillBigDragonNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bKillBigDragonNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.KillBigDragonTime.Length < (int)this.bKillBigDragonNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.bKillBigDragonNum; j++)
			{
				errorType = destBuf.writeUInt32(this.KillBigDragonTime[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bRuneTypeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bRuneTypeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRuneTypePickUpNum.Length < (int)this.bRuneTypeNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int k = 0; k < (int)this.bRuneTypeNum; k++)
			{
				errorType = this.astRuneTypePickUpNum[k].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bGame30SecNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (60 < this.bGame30SecNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astGame30SecDetail.Length < (int)this.bGame30SecNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int l = 0; l < (int)this.bGame30SecNum; l++)
			{
				errorType = this.astGame30SecDetail[l].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astEquipDetail.Length < (int)this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int m = 0; m < (int)this.bEquipNum; m++)
			{
				errorType = this.astEquipDetail[m].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwTotalInGameCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMaxInGameCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCamp1TowerFirstAttackTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCamp2TowerFirstAttackTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwUpRoadTower1DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwUpRoadTower2DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwUpRoadTower3DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMidRoadTower1DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMidRoadTower2DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMidRoadTower3DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDownRoadTower1DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDownRoadTower2DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDownRoadTower3DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iTimeUse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iPauseTimeTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingAttackRange);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingAttackDamageMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingAttackDamageMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingHurtCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingHurtMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingHurtMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuildingHurtTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iExperienceHPAdd1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iExperienceHPAdd2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iExperienceHPAdd3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iExperienceHPAdd4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iExperienceHPAddTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossHurtMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossHurtMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossHurtTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingHurtMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingHurtMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingHurtTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyKIllHeroGap);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossAttackCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossUseSkillCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossAttackMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossAttackMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossSkillDamageMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossSkillDamageMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBossAttackTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyAttackMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyAttackMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingAttackRange);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingDamageMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iEnemyBuildingDamageMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iCommunicationCount1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iCommunicationCount2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iMaxSoldierCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iMaxTowerAttackDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bSelfCampKillTowerCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bQuickBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPanelBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bSelfCampKillBaseCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSelfCampBaseBlood);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iCurrentDisparity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bSelfCampHaveWinningFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwBoardSortNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwClickNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30u < this.dwClickNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.ClickDetail.Length < (long)((ulong)this.dwClickNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwClickNum))
			{
				errorType = destBuf.writeUInt32(this.ClickDetail[num]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.writeUInt8(this.bAdvantageNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (16 < this.bAdvantageNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astAdvantageDetail.Length < (int)this.bAdvantageNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int n = 0; n < (int)this.bAdvantageNum; n++)
			{
				errorType = this.astAdvantageDetail[n].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwOperateNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwOperateNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.OperateType.Length < (long)((ulong)this.dwOperateNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.dwOperateNum))
			{
				errorType = destBuf.writeUInt32(this.OperateType[num2]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num2++;
			}
			errorType = destBuf.writeUInt32(this.dwBigDragonBattleToDeadTimeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwBigDragonBattleToDeadTimeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.BigDragonBattleToDeadTimeDtail.Length < (long)((ulong)this.dwBigDragonBattleToDeadTimeNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num3 = 0;
			while ((long)num3 < (long)((ulong)this.dwBigDragonBattleToDeadTimeNum))
			{
				errorType = destBuf.writeUInt32(this.BigDragonBattleToDeadTimeDtail[num3]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num3++;
			}
			errorType = destBuf.writeUInt32(this.dwDragonBattleToDeadTimeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwDragonBattleToDeadTimeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.DragonBattleToDeadTimeDtail.Length < (long)((ulong)this.dwDragonBattleToDeadTimeNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num4 = 0;
			while ((long)num4 < (long)((ulong)this.dwDragonBattleToDeadTimeNum))
			{
				errorType = destBuf.writeUInt32(this.DragonBattleToDeadTimeDtail[num4]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num4++;
			}
			errorType = destBuf.writeUInt32(this.dwMonsterDeadNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwMonsterDeadNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.MonsterDeadDetail.Length < (long)((ulong)this.dwMonsterDeadNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num5 = 0;
			while ((long)num5 < (long)((ulong)this.dwMonsterDeadNum))
			{
				errorType = destBuf.writeUInt32(this.MonsterDeadDetail[num5]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num5++;
			}
			errorType = destBuf.writeUInt32(this.dwRongyuFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullVisibleBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bCarrier);
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
			if (cutVer == 0u || COMDT_SETTLE_GAME_GENERAL_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_GAME_GENERAL_INFO.CURRVERSION;
			}
			if (COMDT_SETTLE_GAME_GENERAL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwFBTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bKillDragonNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bKillDragonNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.KillDragonTime = new uint[(int)this.bKillDragonNum];
			for (int i = 0; i < (int)this.bKillDragonNum; i++)
			{
				errorType = srcBuf.readUInt32(ref this.KillDragonTime[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bKillBigDragonNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bKillBigDragonNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.KillBigDragonTime = new uint[(int)this.bKillBigDragonNum];
			for (int j = 0; j < (int)this.bKillBigDragonNum; j++)
			{
				errorType = srcBuf.readUInt32(ref this.KillBigDragonTime[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bRuneTypeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bRuneTypeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int k = 0; k < (int)this.bRuneTypeNum; k++)
			{
				errorType = this.astRuneTypePickUpNum[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bGame30SecNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (60 < this.bGame30SecNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int l = 0; l < (int)this.bGame30SecNum; l++)
			{
				errorType = this.astGame30SecDetail[l].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bEquipNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bEquipNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int m = 0; m < (int)this.bEquipNum; m++)
			{
				errorType = this.astEquipDetail[m].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalInGameCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMaxInGameCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCamp1TowerFirstAttackTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCamp2TowerFirstAttackTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUpRoadTower1DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUpRoadTower2DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUpRoadTower3DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMidRoadTower1DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMidRoadTower2DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMidRoadTower3DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDownRoadTower1DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDownRoadTower2DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDownRoadTower3DesTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTimeUse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPauseTimeTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingAttackRange);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingAttackDamageMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingAttackDamageMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingHurtCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingHurtMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingHurtMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuildingHurtTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExperienceHPAdd1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExperienceHPAdd2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExperienceHPAdd3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExperienceHPAdd4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExperienceHPAddTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossHurtMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossHurtMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossHurtTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingHurtMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingHurtMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingHurtTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyHPMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyHPMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyKIllHeroGap);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossAttackCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossUseSkillCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossAttackMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossAttackMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossSkillDamageMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossSkillDamageMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBossAttackTotal);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyAttackMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyAttackMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingAttackRange);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingDamageMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iEnemyBuildingDamageMin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCommunicationCount1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCommunicationCount2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxSoldierCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxTowerAttackDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSelfCampKillTowerCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bQuickBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPanelBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSelfCampKillBaseCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSelfCampBaseBlood);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCurrentDisparity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSelfCampHaveWinningFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBoardSortNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwClickNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30u < this.dwClickNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.ClickDetail = new uint[this.dwClickNum];
			int num = 0;
			while ((long)num < (long)((ulong)this.dwClickNum))
			{
				errorType = srcBuf.readUInt32(ref this.ClickDetail[num]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = srcBuf.readUInt8(ref this.bAdvantageNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (16 < this.bAdvantageNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int n = 0; n < (int)this.bAdvantageNum; n++)
			{
				errorType = this.astAdvantageDetail[n].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwOperateNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwOperateNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.OperateType = new uint[this.dwOperateNum];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.dwOperateNum))
			{
				errorType = srcBuf.readUInt32(ref this.OperateType[num2]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num2++;
			}
			errorType = srcBuf.readUInt32(ref this.dwBigDragonBattleToDeadTimeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwBigDragonBattleToDeadTimeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.BigDragonBattleToDeadTimeDtail = new uint[this.dwBigDragonBattleToDeadTimeNum];
			int num3 = 0;
			while ((long)num3 < (long)((ulong)this.dwBigDragonBattleToDeadTimeNum))
			{
				errorType = srcBuf.readUInt32(ref this.BigDragonBattleToDeadTimeDtail[num3]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num3++;
			}
			errorType = srcBuf.readUInt32(ref this.dwDragonBattleToDeadTimeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwDragonBattleToDeadTimeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.DragonBattleToDeadTimeDtail = new uint[this.dwDragonBattleToDeadTimeNum];
			int num4 = 0;
			while ((long)num4 < (long)((ulong)this.dwDragonBattleToDeadTimeNum))
			{
				errorType = srcBuf.readUInt32(ref this.DragonBattleToDeadTimeDtail[num4]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num4++;
			}
			errorType = srcBuf.readUInt32(ref this.dwMonsterDeadNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwMonsterDeadNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.MonsterDeadDetail = new uint[this.dwMonsterDeadNum];
			int num5 = 0;
			while ((long)num5 < (long)((ulong)this.dwMonsterDeadNum))
			{
				errorType = srcBuf.readUInt32(ref this.MonsterDeadDetail[num5]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num5++;
			}
			errorType = srcBuf.readUInt32(ref this.dwRongyuFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullVisibleBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCarrier);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SETTLE_GAME_GENERAL_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwFBTime = 0u;
			this.bKillDragonNum = 0;
			this.bKillBigDragonNum = 0;
			this.bRuneTypeNum = 0;
			if (this.astRuneTypePickUpNum != null)
			{
				for (int i = 0; i < this.astRuneTypePickUpNum.Length; i++)
				{
					if (this.astRuneTypePickUpNum[i] != null)
					{
						this.astRuneTypePickUpNum[i].Release();
						this.astRuneTypePickUpNum[i] = null;
					}
				}
			}
			this.bGame30SecNum = 0;
			if (this.astGame30SecDetail != null)
			{
				for (int j = 0; j < this.astGame30SecDetail.Length; j++)
				{
					if (this.astGame30SecDetail[j] != null)
					{
						this.astGame30SecDetail[j].Release();
						this.astGame30SecDetail[j] = null;
					}
				}
			}
			this.bEquipNum = 0;
			if (this.astEquipDetail != null)
			{
				for (int k = 0; k < this.astEquipDetail.Length; k++)
				{
					if (this.astEquipDetail[k] != null)
					{
						this.astEquipDetail[k].Release();
						this.astEquipDetail[k] = null;
					}
				}
			}
			this.dwTotalInGameCoin = 0u;
			this.dwMaxInGameCoin = 0u;
			this.dwCamp1TowerFirstAttackTime = 0u;
			this.dwCamp2TowerFirstAttackTime = 0u;
			this.dwUpRoadTower1DesTime = 0u;
			this.dwUpRoadTower2DesTime = 0u;
			this.dwUpRoadTower3DesTime = 0u;
			this.dwMidRoadTower1DesTime = 0u;
			this.dwMidRoadTower2DesTime = 0u;
			this.dwMidRoadTower3DesTime = 0u;
			this.dwDownRoadTower1DesTime = 0u;
			this.dwDownRoadTower2DesTime = 0u;
			this.dwDownRoadTower3DesTime = 0u;
			this.iTimeUse = 0;
			this.iPauseTimeTotal = 0;
			this.iBuildingAttackRange = 0;
			this.iBuildingAttackDamageMax = 0;
			this.iBuildingAttackDamageMin = 0;
			this.iBuildingHPMax = 0;
			this.iBuildingHPMin = 0;
			this.iBuildingHurtCount = 0;
			this.iBuildingHurtMax = 0;
			this.iBuildingHurtMin = 0;
			this.iBuildingHurtTotal = 0;
			this.iExperienceHPAdd1 = 0;
			this.iExperienceHPAdd2 = 0;
			this.iExperienceHPAdd3 = 0;
			this.iExperienceHPAdd4 = 0;
			this.iExperienceHPAddTotal = 0;
			this.iBossCount = 0;
			this.iBossHPMax = 0;
			this.iBossHPMin = 0;
			this.iBossHurtMax = 0;
			this.iBossHurtMin = 0;
			this.iBossHurtTotal = 0;
			this.iEnemyBuildingHPMax = 0;
			this.iEnemyBuildingHPMin = 0;
			this.iEnemyBuildingHurtMax = 0;
			this.iEnemyBuildingHurtMin = 0;
			this.iEnemyBuildingHurtTotal = 0;
			this.iEnemyHPMax = 0;
			this.iEnemyHPMin = 0;
			this.iEnemyKIllHeroGap = 0;
			this.iBossAttackCount = 0;
			this.iBossUseSkillCount = 0;
			this.iBossAttackMax = 0;
			this.iBossAttackMin = 0;
			this.iBossSkillDamageMax = 0;
			this.iBossSkillDamageMin = 0;
			this.iBossAttackTotal = 0;
			this.iEnemyAttackMax = 0;
			this.iEnemyAttackMin = 0;
			this.iEnemyBuildingAttackRange = 0;
			this.iEnemyBuildingDamageMax = 0;
			this.iEnemyBuildingDamageMin = 0;
			this.iCommunicationCount1 = 0;
			this.iCommunicationCount2 = 0;
			this.iMaxSoldierCnt = 0;
			this.iMaxTowerAttackDistance = 0;
			this.bSelfCampKillTowerCnt = 0;
			this.bQuickBuyItemCnt = 0;
			this.bPanelBuyItemCnt = 0;
			this.bSelfCampKillBaseCnt = 0;
			this.dwSelfCampBaseBlood = 0u;
			this.iCurrentDisparity = 0;
			this.bSelfCampHaveWinningFlag = 0;
			this.dwBoardSortNum = 0u;
			this.dwClickNum = 0u;
			this.bAdvantageNum = 0;
			if (this.astAdvantageDetail != null)
			{
				for (int l = 0; l < this.astAdvantageDetail.Length; l++)
				{
					if (this.astAdvantageDetail[l] != null)
					{
						this.astAdvantageDetail[l].Release();
						this.astAdvantageDetail[l] = null;
					}
				}
			}
			this.dwOperateNum = 0u;
			this.dwBigDragonBattleToDeadTimeNum = 0u;
			this.dwDragonBattleToDeadTimeNum = 0u;
			this.dwMonsterDeadNum = 0u;
			this.dwRongyuFlag = 0u;
			this.ullVisibleBits = 0uL;
			this.bCarrier = 0;
		}

		public override void OnUse()
		{
			if (this.astRuneTypePickUpNum != null)
			{
				for (int i = 0; i < this.astRuneTypePickUpNum.Length; i++)
				{
					this.astRuneTypePickUpNum[i] = (COMDT_RUNE_PICK_UP_INFO)ProtocolObjectPool.Get(COMDT_RUNE_PICK_UP_INFO.CLASS_ID);
				}
			}
			if (this.astGame30SecDetail != null)
			{
				for (int j = 0; j < this.astGame30SecDetail.Length; j++)
				{
					this.astGame30SecDetail[j] = (COMDT_GAME_30SEC_INFO)ProtocolObjectPool.Get(COMDT_GAME_30SEC_INFO.CLASS_ID);
				}
			}
			if (this.astEquipDetail != null)
			{
				for (int k = 0; k < this.astEquipDetail.Length; k++)
				{
					this.astEquipDetail[k] = (COMDT_INGAME_EQUIP_INFO)ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
				}
			}
			if (this.astAdvantageDetail != null)
			{
				for (int l = 0; l < this.astAdvantageDetail.Length; l++)
				{
					this.astAdvantageDetail[l] = (COMDT_INGAME_ADVANTAGE_INFO)ProtocolObjectPool.Get(COMDT_INGAME_ADVANTAGE_INFO.CLASS_ID);
				}
			}
		}
	}
}
