using System;

namespace Assets.Scripts.GameLogic
{
	public class CPlayerBehaviorStat
	{
		public enum BehaviorType
		{
			SortBYCoinBtnClick,
			Battle_TextChat_1,
			Battle_TextChat_2,
			Battle_TextChat_3,
			Battle_TextChat_4,
			Battle_TextChat_5,
			Battle_TextChat_6,
			Battle_TextChat_7,
			Battle_TextChat_8,
			Battle_TextChat_9,
			Battle_TextChat_10,
			Battle_TextChat_11,
			Battle_TextChat_12,
			Battle_TextChat_13,
			Battle_TextChat_14,
			Battle_TextChat_15,
			Battle_Voice_OpenSpeak,
			Battle_Voice_OpenMic,
			Battle_OpenBigMap,
			Battle_Signal_2,
			Battle_Signal_3,
			Battle_Signal_4,
			Battle_Signal_Textmsg,
			Battle_ButtonViewSkillInfo,
			Battle_SignalPanel1,
			Battle_SignalPanel2,
			Battle_SignalPanel3,
			Count
		}

		private static uint[] m_data = new uint[27];

		public static void Clear()
		{
			CPlayerBehaviorStat.m_data = new uint[27];
		}

		private static void SetData(CPlayerBehaviorStat.BehaviorType type, uint value)
		{
			CPlayerBehaviorStat.m_data[(int)type] = value;
		}

		public static uint GetData(CPlayerBehaviorStat.BehaviorType type)
		{
			return CPlayerBehaviorStat.m_data[(int)type];
		}

		public static void Plus(CPlayerBehaviorStat.BehaviorType type)
		{
			CPlayerBehaviorStat.m_data[(int)type] += 1u;
		}
	}
}
