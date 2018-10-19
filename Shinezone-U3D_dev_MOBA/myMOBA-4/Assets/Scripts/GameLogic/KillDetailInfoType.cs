using System;

namespace Assets.Scripts.GameLogic
{
	public enum KillDetailInfoType
	{
		Info_Type_None,
		Info_Type_First_Kill,
		Info_Type_Kill,
		Info_Type_DoubleKill,
		Info_Type_TripleKill,
		Info_Type_QuataryKill,
		Info_Type_PentaKill,
		Info_Type_MonsterKill = 11,
		Info_Type_DominateBattle,
		Info_Type_Legendary,
		Info_Type_TotalAnnihilat,
		Info_Type_Odyssey,
		Info_Type_DestroyTower = 21,
		Info_Type_DestroyBase,
		Info_Type_Kill_3V3_Dragon = 100,
		Info_Type_Kill_5V5_SmallDragon,
		Info_Type_Kill_5V5_BigDragon,
		Info_Type_AllDead = 1000,
		Info_Type_RunningMan = 2000,
		Info_Type_Reconnect,
		Info_Type_Disconnect,
		Info_Type_StopMultiKill = 3000,
		Info_Type_Cannon_Spawned = 3101,
		Info_Type_Soldier_Boosted,
		Info_Type_Game_Start_Wel,
		Info_Type_Soldier_Activate_Countdown3,
		Info_Type_Soldier_Activate_Countdown5,
		Info_Type_Soldier_Activate,
		Info_Type_Soldier_BigDragon,
		Info_Type_Destroy_All_Tower,
		Info_Type_FireHole_first = 4000,
		Info_Type_FireHole_second,
		Info_Type_FireHole_third,
		Info_Type_5V5SmallDragon_Suicide = 4100,
		Info_Type_5V5SmallDragon_Enter
	}
}
