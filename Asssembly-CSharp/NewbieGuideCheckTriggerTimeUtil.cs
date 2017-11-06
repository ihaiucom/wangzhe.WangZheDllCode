using ResData;
using System;

public class NewbieGuideCheckTriggerTimeUtil
{
	public static bool CheckTriggerTime(NewbieGuideWeakConf data, uint[] param)
	{
		NewbieGuideWeakGuideType dwType = (NewbieGuideWeakGuideType)data.dwType;
		return true;
	}

	public static bool CheckTriggerTime(NewbieGuideTriggerTimeItem time, uint[] param)
	{
		NewbieGuideTriggerTimeType wType = (NewbieGuideTriggerTimeType)time.wType;
		switch (wType)
		{
		case NewbieGuideTriggerTimeType.onTalentLevelChange:
		{
			bool flag = true;
			if (time.Param[0] > 0u)
			{
				flag &= (time.Param[0] == param[0]);
			}
			if (time.Param[1] > 0u)
			{
				flag &= (time.Param[1] == param[1]);
			}
			return flag;
		}
		case NewbieGuideTriggerTimeType.preNewBieWeakGuideStepComplete:
			return time.Param[0] > 0u && time.Param[0] == param[0];
		case NewbieGuideTriggerTimeType.preNewBieWeakGuideComleteAll:
			return time.Param[0] > 0u && time.Param[0] == param[0];
		case NewbieGuideTriggerTimeType.PvPShowKDA:
			if (time.Param[0] <= 0u)
			{
				return time.Param[1] <= 0u || time.Param[1] == param[1];
			}
			return time.Param[0] == param[0];
		}
		switch (wType)
		{
		case NewbieGuideTriggerTimeType.preNewbieGuideComplete:
			return NewbieGuideCheckTriggerTimeUtil.CheckPreNewbieGuideCompleteTime(time, param);
		case NewbieGuideTriggerTimeType.battleFin:
		{
			bool flag2 = true;
			if (time.Param[0] > 0u)
			{
				flag2 &= (time.Param[0] == param[0]);
			}
			if (time.Param[1] > 0u)
			{
				flag2 &= (time.Param[1] == param[1]);
			}
			if (time.Param[2] > 0u)
			{
				flag2 &= (time.Param[2] == param[2]);
			}
			if (time.Param[3] > 0u)
			{
				flag2 &= (time.Param[3] == param[3]);
			}
			return flag2;
		}
		case NewbieGuideTriggerTimeType.equipSuccess:
			return false;
		default:
			switch (wType)
			{
			case NewbieGuideTriggerTimeType.pvpFin:
				if (time.Param[0] <= 0u)
				{
					return time.Param[1] <= 0u || time.Param[1] == param[1];
				}
				return time.Param[0] == param[0];
			case NewbieGuideTriggerTimeType.heroSelectedForBattle:
				return (time.Param[0] == 0u && param[0] > 0u) || (time.Param[0] == param[0] && param[0] > 0u);
			}
			for (int i = 0; i < param.Length; i++)
			{
				if (param[i] != time.Param[i])
				{
					return false;
				}
			}
			return true;
		}
	}

	public static bool CheckPreNewbieGuideCompleteTime(NewbieGuideTriggerTimeItem time, uint[] param)
	{
		return param.Length == 1 && time.Param[0] == param[0];
	}
}
