using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Utility
{
	public enum NameResult
	{
		Vaild,
		Null,
		OutOfLength,
		InVaildChar
	}

	public enum MD5_STRING_CASE
	{
		UPPER,
		LOWER
	}

	public enum enDTFormate
	{
		FULL,
		DATE,
		TIME
	}

	public const string md5Salt = "j9cWiS6lPjw4g";

	public const long UTC_OFFSET_LOCAL = 28800L;

	public const long UTCTICK_PER_SECONDS = 10000000L;

	public static readonly int MIN_EN_NAME_LEN = 1;

	public static readonly int MAX_EN_NAME_LEN = 12;

	public static uint s_daySecond = 86400u;

	private static ulong[] _DW = new ulong[]
	{
		10000000000uL,
		100000000uL,
		1000000uL,
		10000uL,
		100uL
	};

	private static readonly int CHINESE_CHAR_START = Convert.ToInt32("4e00", 16);

	private static readonly int CHINESE_CHAR_END = Convert.ToInt32("9fff", 16);

	private static readonly char[] s_ban_chars = new char[]
	{
		Convert.ToChar(10),
		Convert.ToChar(13)
	};

	public static int TimeToFrame(float time)
	{
		return (int)Math.Ceiling((double)(time / Time.fixedDeltaTime));
	}

	public static float FrameToTime(int frame)
	{
		return (float)frame * Time.fixedDeltaTime;
	}

	public static Vector3 GetGameObjectSize(GameObject obj)
	{
		Vector3 result = Vector3.zero;
		if (obj.GetComponent<Renderer>() != null)
		{
			result = obj.GetComponent<Renderer>().bounds.size;
		}
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
			Vector3 size = renderer.bounds.size;
			result.x = Math.Max(result.x, size.x);
			result.y = Math.Max(result.y, size.y);
			result.z = Math.Max(result.z, size.z);
		}
		return result;
	}

	public static GameObject FindChild(GameObject p, string path)
	{
		if (p != null)
		{
			Transform transform = p.transform.FindChild(path);
			return (!(transform != null)) ? null : transform.gameObject;
		}
		return null;
	}

	public static GameObject FindChildSafe(GameObject p, string path)
	{
		if (p)
		{
			Transform transform = p.transform.FindChild(path);
			if (transform)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static T GetComponetInChild<T>(GameObject p, string path) where T : MonoBehaviour
	{
		if (p == null || p.transform == null)
		{
			return (T)((object)null);
		}
		Transform transform = p.transform.FindChild(path);
		if (transform == null)
		{
			return (T)((object)null);
		}
		return transform.GetComponent<T>();
	}

	public static GameObject FindChildByName(Component component, string childpath)
	{
		return Utility.FindChildByName(component.gameObject, childpath);
	}

	public static GameObject FindChildByName(GameObject root, string childpath)
	{
		GameObject result = null;
		string[] array = childpath.Split(new char[]
		{
			'/'
		});
		GameObject gameObject = root;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string b = array2[i];
			bool flag = false;
			foreach (Transform transform in gameObject.transform)
			{
				if (transform.gameObject.name == b)
				{
					gameObject = transform.gameObject;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		if (gameObject != root)
		{
			result = gameObject;
		}
		return result;
	}

	public static void SetChildrenActive(GameObject root, bool active)
	{
		foreach (Transform transform in root.transform)
		{
			if (transform != root.transform)
			{
				transform.gameObject.CustomSetActive(active);
			}
		}
	}

	public static void StringToByteArray(string str, ref byte[] buffer)
	{
		byte[] bytes = Encoding.Default.GetBytes(str);
		Array.Copy(bytes, buffer, Math.Min(bytes.Length, buffer.Length));
		buffer[buffer.Length - 1] = 0;
	}

	public static string UTF8Convert(string str)
	{
		return str;
	}

	public static string UTF8Convert(byte[] p)
	{
		return StringHelper.UTF8BytesToString(ref p);
	}

	public static byte[] BytesConvert(string s)
	{
		return Encoding.UTF8.GetBytes(s.TrimEnd(new char[1]));
	}

	public static byte[] SByteArrToByte(sbyte[] p)
	{
		byte[] array = new byte[p.Length];
		for (int i = 0; i < p.Length; i++)
		{
			array[i] = (byte)p[i];
		}
		return array;
	}

	public static string UTF8Convert(sbyte[] p, int len)
	{
		byte[] p2 = Utility.SByteArrToByte(p);
		return Utility.UTF8Convert(p2);
	}

	public static DateTime ToUtcTime2Local(long secondsFromUtcStart)
	{
		DateTime dateTime = new DateTime(1970, 1, 1);
		return dateTime.AddTicks((secondsFromUtcStart + 28800L) * 10000000L);
	}

	public static uint ToUtcSeconds(DateTime dateTime)
	{
		DateTime dateTime2 = new DateTime(1970, 1, 1);
		if (dateTime.CompareTo(dateTime2) < 0)
		{
			return 0u;
		}
		if ((dateTime - dateTime2).TotalSeconds > 28800.0)
		{
			return (uint)(dateTime - dateTime2).TotalSeconds - 28800u;
		}
		return 0u;
	}

	public static string GetUtcToLocalTimeStringFormat(ulong secondsFromUtcStart, string format)
	{
		return Utility.ToUtcTime2Local((long)secondsFromUtcStart).ToString(format);
	}

	public static uint GetLocalTimeFormattedStrToUtc(string localTimeFormattedStr)
	{
		DateTime dateTime = new DateTime(Convert.ToInt32(localTimeFormattedStr.Substring(0, 4)), Convert.ToInt32(localTimeFormattedStr.Substring(4, 2)), Convert.ToInt32(localTimeFormattedStr.Substring(6, 2)), Convert.ToInt32(localTimeFormattedStr.Substring(8, 2)), Convert.ToInt32(localTimeFormattedStr.Substring(10, 2)), Convert.ToInt32(localTimeFormattedStr.Substring(12, 2)));
		return Utility.ToUtcSeconds(dateTime);
	}

	public static uint GetGlobalRefreshTimeSeconds()
	{
		uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(10u).dwConfValue;
		int num = Utility.Hours2Second((int)(dwConfValue / 100u)) + Utility.Minutes2Seconds((int)(dwConfValue % 100u));
		DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
		DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
		DateTime dateTime3 = dateTime2.AddSeconds((double)num);
		return Utility.ToUtcSeconds(dateTime3);
	}

	public static DateTime StrToDateTime(string dateString, string format)
	{
		CultureInfo cultureInfo = new CultureInfo("zh-CN");
		DateTimeFormatInfo dateTimeFormat = cultureInfo.DateTimeFormat;
		DateTime result;
		if (DateTime.TryParseExact(dateString, format, dateTimeFormat, DateTimeStyles.None, out result))
		{
			return result;
		}
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}

	public static uint GetLatestGlobalRefreshTimeSeconds()
	{
		int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
		uint globalRefreshTimeSeconds = Utility.GetGlobalRefreshTimeSeconds();
		if ((long)currentUTCTime >= (long)((ulong)globalRefreshTimeSeconds))
		{
			return globalRefreshTimeSeconds;
		}
		return globalRefreshTimeSeconds - 86400u;
	}

	public static uint GetTodayStartTimeSeconds()
	{
		DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
		DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
		return Utility.ToUtcSeconds(dateTime2);
	}

	public static uint GetNewDayDeltaSec(int nowSec)
	{
		DateTime d = Utility.ToUtcTime2Local((long)nowSec);
		DateTime dateTime = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc);
		DateTime d2 = dateTime.AddSeconds(86400.0);
		return (uint)(d2 - d).TotalSeconds;
	}

	public static bool IsSameDay(long secondsFromUtcStart1, long secondsFromUtcStart2)
	{
		DateTime dateTime = new DateTime(1970, 1, 1);
		DateTime dateTime2 = dateTime.AddTicks((secondsFromUtcStart1 + 28800L) * 10000000L);
		DateTime dateTime3 = dateTime.AddTicks((secondsFromUtcStart2 + 28800L) * 10000000L);
		return DateTime.Equals(dateTime2.Date, dateTime3.Date);
	}

	public static bool IsSameWeek(DateTime dtmS, DateTime dtmE)
	{
		double totalDays = (dtmE - dtmS).TotalDays;
		int num = Convert.ToInt32(dtmE.DayOfWeek);
		if (num == 0)
		{
			num = 7;
		}
		return totalDays < 7.0 && totalDays < (double)num;
	}

	public static bool IsSameWeek(long secondsFromUtcStart1, long secondsFromUtcStart2)
	{
		DateTime dateTime = new DateTime(1970, 1, 1);
		DateTime dtmS = dateTime.AddTicks((secondsFromUtcStart1 + 28800L) * 10000000L);
		DateTime dtmE = dateTime.AddTicks((secondsFromUtcStart2 + 28800L) * 10000000L);
		return Utility.IsSameWeek(dtmS, dtmE);
	}

	public static long GetZeroBaseSecond(long utcSec)
	{
		DateTime d = new DateTime(1970, 1, 1);
		DateTime dateTime = d.AddTicks((utcSec + 28800L) * 10000000L);
		DateTime d2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
		return (long)(d2 - d).TotalSeconds - 28800L;
	}

	public static int Hours2Second(int hour)
	{
		return hour * 60 * 60;
	}

	public static int Minutes2Seconds(int min)
	{
		return min * 60;
	}

	public static string GetTimeBeforString(long beforSecondsFromUtc, long nowSecondsFromUtc)
	{
		string result = string.Empty;
		TimeSpan timeSpan = new TimeSpan((beforSecondsFromUtc + 28800L) * 10000000L);
		TimeSpan timeSpan2 = new TimeSpan((nowSecondsFromUtc + 28800L) * 10000000L);
		int num = (int)timeSpan2.TotalDays - (int)timeSpan.TotalDays;
		if (num >= 1)
		{
			result = Singleton<CTextManager>.GetInstance().GetText("Time_DayBefore", new string[]
			{
				num.ToString()
			});
		}
		else
		{
			result = Singleton<CTextManager>.GetInstance().GetText("Time_Today");
		}
		return result;
	}

	public static string GetRecentOnlineTimeString(int recentOnlineTime)
	{
		string result = string.Empty;
		if (recentOnlineTime == 0)
		{
			return result;
		}
		int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
		if (currentUTCTime > recentOnlineTime)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, currentUTCTime - recentOnlineTime);
			if (timeSpan.Days == 0)
			{
				if (timeSpan.Hours == 0)
				{
					if (timeSpan.Minutes == 0)
					{
						result = Singleton<CTextManager>.GetInstance().GetText("Guild_Tips_lastTime_default");
					}
					else
					{
						result = Singleton<CTextManager>.GetInstance().GetText("Guild_Tips_lastTime_min", new string[]
						{
							timeSpan.Minutes.ToString()
						});
					}
				}
				else
				{
					result = Singleton<CTextManager>.GetInstance().GetText("Guild_Tips_lastTime_hour_min", new string[]
					{
						timeSpan.Hours.ToString(),
						timeSpan.Minutes.ToString()
					});
				}
			}
			else
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_Tips_lastTime_day", new string[]
				{
					timeSpan.Days.ToString()
				});
			}
		}
		return result;
	}

    public static string ProtErrCodeToStr(int ProtId, int ErrCode)
    {
        int num2;
        string text = string.Empty;
        int num = ProtId;
        switch (num)
        {
            case 0x564:
            case 0x567:
            case 0x569:
            case 0x56b:
            case 0x56d:
            case 0x56f:
                goto Label_123F;

            case 0x76e:
            case 0x770:
            case 0x772:
            case 0x774:
            case 0x776:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("SecurePwd_State_Error");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Invalid_Operation");

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Invalid_Pwd");

                    case 4:
                        return Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Invalid_Force_Close_Req");
                }
                return text;

            case 0x151b:
            case 0x151d:
            case 0x151f:
                switch (ErrCode)
                {
                    case 200:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_DataErr");

                    case 0xc9:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_CannotCrossPlat");

                    case 0xca:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_HasMentor");

                    case 0xcb:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_RefusedRequest");

                    case 0xcc:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_BeyondRequestLimit");

                    case 0xcd:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_RequestDuplicated");

                    case 0xce:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_ApprenticeCountTooMuch");

                    case 0xcf:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_AlreadyMentor");

                    case 0xd0:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_HasMentorOrNotGraduated");

                    case 0xd1:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_NoRequest");

                    case 210:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_NotMentor");

                    case 0xd3:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_CannotDismissInAday");

                    case 0xd4:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Level2LowAsMentor");

                    case 0xd5:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Level2HighAsApprentice");

                    case 0xd6:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Grade2LowAsMentor");

                    case 0xd7:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Transaction");

                    case 0xd8:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_NotStudent");

                    case 0xd9:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_MasterApplySelf");

                    case 0xda:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_MasterBantime");

                    case 0xdb:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_OTHER_LVL_LOW");

                    case 220:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_OTHER_LVL_HIGH");

                    case 0xdd:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_OTHER_GRADE_LOW");

                    case 0xde:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_REQ_NOTVALID");

                    case 0xdf:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Other_IsStudent");

                    case 0xe3:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_DATINGDECLARATION_ILLEGAL");

                    case 0xe4:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_FRIENDCARD_TAG");

                    case 0xe5:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_FRIENDCARD_OTHER");

                    case 230:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_CHG_ANTIDISTURB_BIT");

                    case 0xe8:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_STUDENT_GRADE_HIGH");

                    case 0xe9:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_STUDENT_GRADE_LOW");
                }
                break;

            case 0x3fe:
            case 0x400:
            case 0x401:
            case 0x7de:
                goto Label_0535;

            case 0x7db:
                goto Label_07B9;

            case 0xb55:
                num2 = ErrCode;
                if (num2 == 3)
                {
                    return Singleton<CTextManager>.GetInstance().GetText("Arena_ARENASETBATTLELIST_ERR_FAILED");
                }
                return text;

            case 0xb58:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("Arena_ARENAADDMEM_ERR_ALREADYIN");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("Arena_ARENAADDMEM_ERR_BATTLELISTISNULL");
                }
                return text;

            case 0x498:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Sys");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_ID");

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Out_Date");

                    case 4:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Dup");

                    case 5:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Pay");

                    case 6:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Money");

                    case 7:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Exchange_Coupons_Err");

                    case 8:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Exchange_Coupons_Invalid");

                    case 9:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Recommend_Failed");
                }
                return text;

            case 0x49a:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Roulette_Server");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Roulette_ID");

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Roulette_Out_Date");

                    case 4:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Roulette_Money");

                    case 5:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Recommend_Roulette_No_Data");
                }
                return text;

            case 0x727:
            case 0x729:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_SYS");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_LOCK");

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_NOALLOW");

                    case 4:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_UNFRIEND");

                    case 5:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_COINLIMIT");

                    case 6:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_MAILFAIL");

                    case 7:
                    case 8:
                    case 9:
                    case 15:
                        return text;

                    case 10:
                        return Singleton<CTextManager>.GetInstance().GetText("Gift_GiftMax");

                    case 11:
                        return Singleton<CTextManager>.GetInstance().GetText("Gift_GiftNoXin");

                    case 12:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_INVALID_PLAT");

                    case 13:
                        return Singleton<CTextManager>.GetInstance().GetText("Gift_GiftNotToSelf");

                    case 14:
                        return Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Invalid_Pwd");

                    case 0x10:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_HASHERO");

                    case 0x11:
                        return Singleton<CTextManager>.GetInstance().GetText("CS_PRESENTHEROSKIN_HASSKIN");
                }
                return text;

            case 0x12c2:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Money_Not_Enough");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Money_Type_Invalid");

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Lottery_Cnt_Invalid");

                    case 4:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Pay_Failed");

                    case 5:
                    case 7:
                        return text;

                    case 6:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Pay_Time_Out");

                    case 8:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Other_Err");
                }
                return text;

            case 0x12c4:
                switch (ErrCode)
                {
                    case 1:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Money_Not_Enough");

                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Extern_Period_Index_Invalid");

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Extern_No_Reach");

                    case 4:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Extern_Drawed");

                    case 5:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Extern_Reward_Id");

                    case 6:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Roulette_Other_Err");
                }
                return text;

            default:
                switch (num)
                {
                    case 0x41b:
                        switch (ErrCode)
                        {
                            case 1:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_SINGLEGAME_ERR_FAIL");

                            case 2:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_SINGLEGAMEOFARENA_ERR_SELFLOCK");

                            case 3:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_SINGLEGAMEOFARENA_ERR_TARGETLOCK");

                            case 4:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_SINGLEGAMEOFARENA_ERR_TARGETCHG");

                            case 5:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_SINGLEGAMEOFARENA_ERR_NOTFINDTARGET");

                            case 6:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_SINGLEGAMEOFARENA_ERR_OTHERS");

                            case 7:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_LIMIT_CNT");

                            case 8:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_LIMIT_CD");

                            case 9:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_REWARD");

                            case 10:
                                return text;

                            case 11:
                                text = Singleton<CTextManager>.GetInstance().GetText("ERR_Freehero_Expire");
                                Singleton<EventRouter>.instance.BroadCastEvent(EventID.SINGLEGAME_ERR_FREEHERO);
                                return text;

                            case 12:
                                return Singleton<CTextManager>.GetInstance().GetText("CS_SINGLEGAME_ERR_EXPSKIN");

                            case 13:
                                return Singleton<CTextManager>.GetInstance().GetText("Arena_Be_Chanllenge_Time_Protect");
                        }
                        return text;

                    case 0x41f:
                        switch (ErrCode)
                        {
                            case 1:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Sweep_Star");

                            case 2:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Sweep_VIP");

                            case 3:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Sweep_AP");

                            case 4:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Sweep_Ticket");

                            case 5:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Sweep_DianQuan");

                            case 6:
                                return text;

                            case 7:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Sweep_Other");
                        }
                        return text;

                    case 0x423:
                        num2 = ErrCode;
                        if (num2 != 10)
                        {
                            return text;
                        }
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Quit_Single_Game");

                    case 0x437:
                        goto Label_0535;

                    case 0x4aa:
                        num2 = ErrCode;
                        if (num2 != 0xac)
                        {
                            return text;
                        }
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_SIGNATURE_ILLEGAL");

                    case 0x4ca:
                        goto Label_123F;

                    case 0x580:
                        switch (ErrCode)
                        {
                            case 1:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_Not_Open");

                            case 2:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_Not_Close");

                            case 3:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_Buy_Limit");

                            case 4:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_Product_Not_Found");

                            case 5:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_GOODSALREADYBUY");

                            case 6:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_Product_BuyFail");

                            case 7:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_Error_Invalid");

                            case 8:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_ERROR_COINLIMIT");

                            case 9:
                                return Singleton<CTextManager>.GetInstance().GetText("Err_Mystery_Shop_ERROR_STATEERR");
                        }
                        return text;

                    case 0x589:
                        num2 = ErrCode;
                        if (num2 != 1)
                        {
                            return text;
                        }
                        return Singleton<CTextManager>.GetInstance().GetText("Err_Player_Info_Honor_Use_Not_Have");

                    case 0x597:
                        switch (ErrCode)
                        {
                            case 0xa5:
                                return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_SEND_GUILD_MAIL_AUTH");

                            case 0xa6:
                                return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_SEND_GUILD_MAIL_LIMIT");

                            case 0xa7:
                                return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_SEND_GUILD_MAIL_SUBJECT");

                            case 0xa8:
                                return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_SEND_GUILD_MAIL_CONTENT");
                        }
                        return text;

                    case 0x71a:
                        switch (ErrCode)
                        {
                            case 1:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_AlreadyHave");

                            case 2:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_WrongMethod");

                            case 3:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_Invalid");

                            case 4:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_WrongTime");

                            case 5:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_WrongSale");

                            case 6:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_Money");

                            case 7:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_Full");

                            case 8:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_InitFail");

                            case 9:
                                return Singleton<CTextManager>.GetInstance().GetText("BuyHero_Error_Other");
                        }
                        return text;

                    case 0x7e7:
                    case 0x7ee:
                        goto Label_07B9;

                    case 0xa2f:
                        num2 = ErrCode;
                        if (num2 != 0xa3)
                        {
                            return text;
                        }
                        return Singleton<CTextManager>.GetInstance().GetText("CS_ERR_FRIEND_INVALID_PLAT");

                    case 0x14d0:
                        return Singleton<CTextManager>.GetInstance().GetText("Err_JudgeFail");

                    default:
                        return text;
                }
                break;
        }
        object[] objArray1 = new object[] { "ProtocolId ", ProtId, " ErrId ", ErrCode, " unhandled" };
        return string.Concat(objArray1);
    Label_0535:
        switch (ErrCode)
        {
            case 1:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Resource_Limit");

            case 2:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Game_Abort");

            case 3:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Timeout");

            case 4:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Account_Leave");

            case 5:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Not_Found");

            case 6:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Game_Already_Start");

            case 7:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Member_Full");

            case 8:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Oper_Failed");

            case 9:
                return Singleton<CTextManager>.GetInstance().GetText("Err_No_Privilege");

            case 10:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Invalid_Param");

            case 11:
            case 12:
            case 13:
            case 14:
            case 20:
            case 0x19:
                return text;

            case 15:
                return Singleton<CTextManager>.GetInstance().GetText("Err_NM_Cancel");

            case 0x10:
                return Singleton<CTextManager>.GetInstance().GetText("Err_NM_Teamcancel");

            case 0x11:
                return Singleton<CTextManager>.GetInstance().GetText("Err_NM_Teamexit");

            case 0x12:
                return Singleton<CTextManager>.GetInstance().GetText("Err_NM_Othercancel");

            case 0x13:
                return Singleton<CTextManager>.GetInstance().GetText("Err_NM_Otherexit");

            case 0x15:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Different");

            case 0x16:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");

            case 0x17:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");

            case 0x18:
                return Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");

            case 0x1a:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Inbantime_Lock");

            case 0x1b:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Limit_Lock");

            case 0x1c:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Ban_Pick_Limit");

            case 0x1d:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ROOMERR_PLAT_CHANNEL_CLOSE");

            case 30:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Ban_Pick_Hero_Limit");

            default:
                return text;
        }
    Label_07B9:
        switch (ErrCode)
        {
            case 1:
                return Singleton<CTextManager>.GetInstance().GetText("Err_No_Privilege");

            case 2:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Invalid_Param");

            case 3:
            case 4:
            case 5:
            case 13:
            case 0x11:
            case 0x12:
            case 0x13:
            case 20:
            case 0x15:
            case 0x16:
            case 0x17:
            case 0x19:
                return text;

            case 6:
                return Singleton<CTextManager>.GetInstance().GetText("COM_MATCH_ERRCODE_OTHERS");

            case 7:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Team_Destory");

            case 8:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Game_Already_Start");

            case 9:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Team_Member_Full");

            case 10:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Game_Ready_Error");

            case 11:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Leave_Team_Failed");

            case 12:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Join_Litmi_Rank_Failed");

            case 14:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");

            case 15:
                return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");

            case 0x10:
                return Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");

            case 0x18:
                return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_OTHERS");

            case 0x1a:
                return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_PLAT_CHANNEL_CLOSE");

            case 0x1b:
                return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_OBING");

            default:
                return text;
        }
    Label_123F:
        switch (ErrCode)
        {
            case 1:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_SYS");

            case 2:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_INDEX");

            case 3:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_UNDEL");

            case 4:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_VERSION");

            case 5:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_NODATA");

            case 6:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_STATE");

            case 7:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_MSGDIRTY");

            case 8:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_LOCK");

            case 9:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_SELF");

            case 10:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_MAXCNT");

            case 11:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_PLAT");

            case 12:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_ITEM");

            case 13:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_NOFRIEND");

            case 14:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_NOALLOW");

            case 15:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_READWAIT");

            case 0x10:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_MSGTYPE");

            case 0x11:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_MSGID");

            case 0x12:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_PSWDCHK");

            case 0x13:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_OFFSHELF");

            case 20:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_PAY");

            case 0x15:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_COINLIMIT");

            case 0x16:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_MAILFAIL");

            case 0x17:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_NOOPEN");

            case 0x18:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_FRIENDTIME");

            case 0x19:
                return Singleton<CTextManager>.GetInstance().GetText("CS_ACNT_ASKFORREQ_ERR_ITEMHAVE");
        }
        return text;
    }


	public static string GetBubbleText(uint tag)
	{
		string result = "BubbleText with tag [" + tag + "] was not found!";
		ResTextData dataByKey = GameDataMgr.textBubbleDatabin.GetDataByKey(tag);
		if (dataByKey != null)
		{
			result = Utility.UTF8Convert(dataByKey.szContent);
		}
		return result;
	}

	public static DateTime ULongToDateTime(ulong dtVal)
	{
		ulong[] array = new ulong[6];
		for (int i = 0; i < Utility._DW.Length; i++)
		{
			array[i] = dtVal / Utility._DW[i];
			dtVal -= array[i] * Utility._DW[i];
		}
		array[Utility._DW.Length] = dtVal;
		return new DateTime((int)array[0], (int)array[1], (int)array[2], (int)array[3], (int)array[4], (int)array[5]);
	}

	public static uint GetSecondsFromHourMinuteSecond(uint formattedTime)
	{
		uint num = formattedTime / 10000u;
		uint num2 = formattedTime % 10000u / 100u;
		uint num3 = formattedTime % 100u;
		return num * 3600u + num2 * 60u + num3;
	}

	public static DateTime SecondsToDateTime(int y, int m, int d, int secondsInDay)
	{
		int hour = secondsInDay / 3600;
		secondsInDay %= 3600;
		return new DateTime(y, m, d, hour, secondsInDay / 60, secondsInDay % 60);
	}

	public static DateTime StringToDateTime(string dtStr, DateTime defaultIfNone)
	{
		ulong dtVal;
		if (ulong.TryParse(dtStr, out dtVal))
		{
			return Utility.ULongToDateTime(dtVal);
		}
		return defaultIfNone;
	}

	public static string DateTimeFormatString(DateTime dt, Utility.enDTFormate fm)
	{
		if (fm == Utility.enDTFormate.DATE)
		{
			return string.Format("{0:D4}-{1:D2}-{2:D2}", dt.Year, dt.Month, dt.Day);
		}
		if (fm == Utility.enDTFormate.TIME)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}", dt.Hour, dt.Minute, dt.Second);
		}
		return string.Format("{0:D4}-{1:D2}-{2:D2}", dt.Year, dt.Month, dt.Day) + " " + string.Format("{0:D2}:{1:D2}:{2:D2}", dt.Hour, dt.Minute, dt.Second);
	}

	public static string SecondsToTimeText(uint secs)
	{
		uint num = secs / 3600u;
		secs -= num * 3600u;
		uint num2 = secs / 60u;
		secs -= num2 * 60u;
		return string.Format("{0:D2}:{1:D2}:{2:D2}", num, num2, secs);
	}

	public static string GetTimeSpanFormatString(int timeSpanSeconds)
	{
		TimeSpan timeSpan = new TimeSpan((long)timeSpanSeconds * 10000000L);
		if (timeSpan.Days > 0)
		{
			return timeSpan.Days.ToString() + Singleton<CTextManager>.GetInstance().GetText("Common_Day") + timeSpan.Hours.ToString() + Singleton<CTextManager>.GetInstance().GetText("Common_Hour");
		}
		string arg = (timeSpan.Hours >= 10) ? timeSpan.Hours.ToString() : ("0" + timeSpan.Hours);
		string arg2 = (timeSpan.Minutes >= 10) ? timeSpan.Minutes.ToString() : ("0" + timeSpan.Minutes);
		string arg3 = (timeSpan.Seconds >= 10) ? timeSpan.Seconds.ToString() : ("0" + timeSpan.Seconds);
		return string.Format("{0}:{1}:{2}", arg, arg2, arg3);
	}

	public static bool IsOverOneDay(int timeSpanSeconds)
	{
		TimeSpan timeSpan = new TimeSpan((long)timeSpanSeconds * 10000000L);
		return timeSpan.Days > 0;
	}

	public static bool IsBanChar(char key)
	{
		for (int i = 0; i < Utility.s_ban_chars.Length; i++)
		{
			if (Utility.s_ban_chars[i] == key)
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasBanCharInString(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (Utility.IsBanChar(str[i]))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsChineseChar(char key)
	{
		int num = Convert.ToInt32(key);
		return num >= Utility.CHINESE_CHAR_START && num <= Utility.CHINESE_CHAR_END;
	}

	public static bool IsSpecialChar(char key)
	{
		return !Utility.IsChineseChar(key) && !char.IsLetter(key) && !char.IsNumber(key);
	}

	public static bool IsValidText(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (Utility.IsSpecialChar(text[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static int GetByteCount(string inputStr)
	{
		int num = 0;
		for (int i = 0; i < inputStr.Length; i++)
		{
			if (Utility.IsQuanjiaoChar(inputStr.Substring(i, 1)))
			{
				num += 2;
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	public static bool IsQuanjiaoChar(string inputStr)
	{
		return Encoding.Default.GetByteCount(inputStr) > 1;
	}

	public static bool IsNullOrEmpty(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return true;
		}
		for (int i = 0; i < str.Length; i++)
		{
			char c = str[i];
			if (c != ' ')
			{
				return false;
			}
		}
		return true;
	}

	public static Utility.NameResult CheckRoleName(string inputName)
	{
		int byteCount = Utility.GetByteCount(inputName);
		if (Utility.IsNullOrEmpty(inputName))
		{
			return Utility.NameResult.Null;
		}
		if (Utility.HasBanCharInString(inputName))
		{
			return Utility.NameResult.InVaildChar;
		}
		if (byteCount > Utility.MAX_EN_NAME_LEN || byteCount < Utility.MIN_EN_NAME_LEN)
		{
			return Utility.NameResult.OutOfLength;
		}
		return Utility.NameResult.Vaild;
	}

	public static Type GetType(string typeName)
	{
		if (string.IsNullOrEmpty(typeName))
		{
			return null;
		}
		Type type = Type.GetType(typeName);
		if (type != null)
		{
			return type;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Assembly assembly = assemblies[i];
			type = assembly.GetType(typeName);
			if (type != null)
			{
				return type;
			}
		}
		return null;
	}

	public static byte[] ObjectToBytes(object obj)
	{
		byte[] buffer;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, obj);
			buffer = memoryStream.GetBuffer();
		}
		return buffer;
	}

	public static object BytesToObject(byte[] Bytes)
	{
		object result;
		using (MemoryStream memoryStream = new MemoryStream(Bytes))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			result = binaryFormatter.Deserialize(memoryStream);
		}
		return result;
	}

	public static byte[] ReadFile(string path)
	{
		if (!CFileManager.IsFileExist(path))
		{
			return null;
		}
		byte[] result;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
		{
			int num = (int)fileStream.Length;
			byte[] array = new byte[num];
			fileStream.Read(array, 0, num);
			fileStream.Close();
			result = array;
		}
		return result;
	}

	public static void ClearDirectory(string path, bool deleteDirSelf = false)
	{
		string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
		for (int i = 0; i < fileSystemEntries.Length; i++)
		{
			string text = fileSystemEntries[i];
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Attributes.ToString().IndexOf("ReadOnly") != -1)
				{
					fileInfo.Attributes = FileAttributes.Normal;
				}
				File.Delete(text);
			}
			else
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				if (directoryInfo.GetFiles().Length != 0)
				{
					Utility.ClearDirectory(directoryInfo.FullName, true);
				}
			}
			if (deleteDirSelf)
			{
				Directory.Delete(path);
			}
		}
	}

	public static bool WriteFile(string path, byte[] bytes)
	{
		FileStream fileStream = null;
		bool result;
		try
		{
			if (!CFileManager.IsFileExist(path))
			{
				fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			}
			else
			{
				fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
			}
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Flush();
			fileStream.Close();
			fileStream.Dispose();
			result = true;
		}
		catch (Exception var_1_4F)
		{
			if (fileStream != null)
			{
				fileStream.Close();
				fileStream.Dispose();
			}
			result = false;
		}
		return result;
	}

	public static string CreateMD5Hash(string input, Utility.MD5_STRING_CASE stringCase = Utility.MD5_STRING_CASE.UPPER)
	{
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			if (stringCase == Utility.MD5_STRING_CASE.UPPER)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			else
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
		}
		return stringBuilder.ToString();
	}

	public static IApolloSnsService GetIApolloSnsService()
	{
		return IApollo.Instance.GetService(1) as IApolloSnsService;
	}

	public static float Divide(uint a, uint b)
	{
		if (b == 0u)
		{
			return 0f;
		}
		return a / b;
	}

	public static void VibrateHelper()
	{
		if (GameSettings.EnableVibrate)
		{
			Handheld.Vibrate();
		}
	}

	public static int GetCpuCurrentClock()
	{
		return Utility.GetCpuClock("/sys/devices/system/cpu/cpu0/cpufreq/scaling_cur_freq");
	}

	public static int GetCpuMaxClock()
	{
		return Utility.GetCpuClock("/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_max_freq");
	}

	public static int GetCpuMinClock()
	{
		return Utility.GetCpuClock("/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_min_freq");
	}

	private static int GetCpuClock(string cpuFile)
	{
		string fileContent = Utility.getFileContent(cpuFile);
		int num = 0;
		if (!int.TryParse(fileContent, out num))
		{
			num = 0;
		}
		return Mathf.FloorToInt((float)(num / 1000));
	}

	private static string getFileContent(string path)
	{
		string result;
		try
		{
			string text = File.ReadAllText(path);
			result = text;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			result = null;
		}
		return result;
	}

	public static bool IsSelf(ulong uid, int logicWorldId)
	{
		return uid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID && Utility.IsSameLogicWorldWithSelf(logicWorldId);
	}

	public static bool IsValidPlayer(ulong uid, int logicWorldId)
	{
		return uid > 0uL && logicWorldId > 0;
	}

	public static bool IsSameLogicWorldWithSelf(int logicWorldId)
	{
		return MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID == logicWorldId;
	}

	public static bool IsSamePlatformWithSelf(uint logicWorldId)
	{
		return Utility.IsSamePlatform((uint)MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, logicWorldId);
	}

	public static bool IsSamePlatform(uint logicWorldId1, uint logicWorldId2)
	{
		return logicWorldId1 / 1000u == logicWorldId2 / 1000u;
	}

	public static bool IsCanShowPrompt()
	{
		return !Singleton<BattleLogic>.GetInstance().isRuning && !Singleton<WatchController>.GetInstance().IsWatching && !Singleton<SettlementSystem>.GetInstance().IsExistSettleForm();
	}

	public static T DeepCopyByReflection<T>(T obj)
	{
		Type type = obj.GetType();
		if (obj is string || type.IsValueType)
		{
			return obj;
		}
		if (type.IsArray)
		{
			Type type2 = Type.GetType(type.FullName.Replace("[]", string.Empty));
			Array array = obj as Array;
			Array array2 = Array.CreateInstance(type2, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(Utility.DeepCopyByReflection<object>(array.GetValue(i)), i);
			}
			return (T)((object)Convert.ChangeType(array2, obj.GetType()));
		}
		object obj2 = Activator.CreateInstance(obj.GetType());
		PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyInfo[] array3 = properties;
		for (int j = 0; j < array3.Length; j++)
		{
			PropertyInfo propertyInfo = array3[j];
			object value = propertyInfo.GetValue(obj, null);
			if (value != null)
			{
				propertyInfo.SetValue(obj2, Utility.DeepCopyByReflection<object>(value), null);
			}
		}
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] array4 = fields;
		for (int k = 0; k < array4.Length; k++)
		{
			FieldInfo fieldInfo = array4[k];
			try
			{
				fieldInfo.SetValue(obj2, Utility.DeepCopyByReflection<object>(fieldInfo.GetValue(obj)));
			}
			catch
			{
			}
		}
		return (T)((object)obj2);
	}

	public static T DeepCopyBySerialization<T>(T obj)
	{
		object obj2;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, obj);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			obj2 = binaryFormatter.Deserialize(memoryStream);
			memoryStream.Close();
		}
		return (T)((object)obj2);
	}
}
