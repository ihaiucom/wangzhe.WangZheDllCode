using System;
using System.Net;

namespace ApolloTdr
{
	public class TdrTypeUtil
	{
		public static int cstrlen(byte[] str)
		{
			byte b = 0;
			int num = 0;
			for (int i = 0; i < str.GetLength(0); i++)
			{
				if (b == str[i])
				{
					break;
				}
				num++;
			}
			return num;
		}

		public static int wstrlen(short[] str)
		{
			short num = 0;
			int num2 = 0;
			for (int i = 0; i < str.GetLength(0); i++)
			{
				if (num == str[i])
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		public static TdrError.ErrorType str2TdrIP(out uint ip, string strip)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			IPAddress iPAddress;
			if (IPAddress.TryParse(strip, ref iPAddress))
			{
				byte[] addressBytes = iPAddress.GetAddressBytes();
				ip = (uint)((int)addressBytes[3] << 24 | (int)addressBytes[2] << 16 | (int)addressBytes[1] << 8 | (int)addressBytes[0]);
			}
			else
			{
				ip = 0u;
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRIP_VALUE;
			}
			return result;
		}

		public static TdrError.ErrorType tdrIP2Str(ref TdrVisualBuf buf, uint ip)
		{
			IPAddress iPAddress = new IPAddress((long)((ulong)ip));
			string text = iPAddress.ToString();
			return buf.sprintf("{0}", new object[]
			{
				text
			});
		}

		public static TdrError.ErrorType str2TdrTime(out uint time, string strTime)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			TdrTime tdrTime = new TdrTime();
			DateTime dateTime;
			if (DateTime.TryParse(strTime, ref dateTime))
			{
				tdrTime.nHour = (short)dateTime.get_TimeOfDay().get_Hours();
				tdrTime.bMin = (byte)dateTime.get_TimeOfDay().get_Minutes();
				tdrTime.bSec = (byte)dateTime.get_TimeOfDay().get_Seconds();
				tdrTime.toTime(out time);
			}
			else
			{
				time = 0u;
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
			}
			return result;
		}

		public static TdrError.ErrorType tdrTime2Str(ref TdrVisualBuf buf, uint time)
		{
			TdrTime tdrTime = new TdrTime();
			TdrError.ErrorType result;
			if (tdrTime.parse(time) == TdrError.ErrorType.TDR_NO_ERROR)
			{
				result = buf.sprintf("{0:d2}:{1:d2}:{2:d2}", new object[]
				{
					tdrTime.nHour,
					tdrTime.bMin,
					tdrTime.bSec
				});
			}
			else
			{
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
			}
			return result;
		}

		public static TdrError.ErrorType str2TdrDate(out uint date, string strDate)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			TdrDate tdrDate = new TdrDate();
			DateTime dateTime;
			if (DateTime.TryParse(strDate, ref dateTime))
			{
				tdrDate.nYear = (short)dateTime.get_Year();
				tdrDate.bMon = (byte)dateTime.get_Month();
				tdrDate.bDay = (byte)dateTime.get_Day();
				tdrDate.toDate(out date);
			}
			else
			{
				date = 0u;
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRDATE_VALUE;
			}
			return result;
		}

		public static TdrError.ErrorType tdrDate2Str(ref TdrVisualBuf buf, uint date)
		{
			TdrDate tdrDate = new TdrDate();
			TdrError.ErrorType result;
			if (tdrDate.parse(date) == TdrError.ErrorType.TDR_NO_ERROR)
			{
				result = buf.sprintf("{0:d4}-{1:d2}-{2:d2}", new object[]
				{
					tdrDate.nYear,
					tdrDate.bMon,
					tdrDate.bDay
				});
			}
			else
			{
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRDATE_VALUE;
			}
			return result;
		}

		public static TdrError.ErrorType str2TdrDateTime(out ulong datetime, string strDateTime)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			TdrDateTime tdrDateTime = new TdrDateTime();
			DateTime dateTime;
			if (DateTime.TryParse(strDateTime, ref dateTime))
			{
				tdrDateTime.tdrDate.nYear = (short)dateTime.get_Year();
				tdrDateTime.tdrDate.bMon = (byte)dateTime.get_Month();
				tdrDateTime.tdrDate.bDay = (byte)dateTime.get_Day();
				tdrDateTime.tdrTime.nHour = (short)dateTime.get_TimeOfDay().get_Hours();
				tdrDateTime.tdrTime.bMin = (byte)dateTime.get_TimeOfDay().get_Minutes();
				tdrDateTime.tdrTime.bSec = (byte)dateTime.get_TimeOfDay().get_Seconds();
				tdrDateTime.toDateTime(out datetime);
			}
			else
			{
				datetime = 0uL;
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRDATETIME_VALUE;
			}
			return result;
		}

		public static TdrError.ErrorType tdrDateTime2Str(ref TdrVisualBuf buf, ulong datetime)
		{
			TdrDateTime tdrDateTime = new TdrDateTime();
			TdrError.ErrorType result;
			if (tdrDateTime.parse(datetime) == TdrError.ErrorType.TDR_NO_ERROR)
			{
				result = buf.sprintf("{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", new object[]
				{
					tdrDateTime.tdrDate.nYear,
					tdrDateTime.tdrDate.bMon,
					tdrDateTime.tdrDate.bDay,
					tdrDateTime.tdrTime.nHour,
					tdrDateTime.tdrTime.bMin,
					tdrDateTime.tdrTime.bSec
				});
			}
			else
			{
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRDATETIME_VALUE;
			}
			return result;
		}
	}
}
