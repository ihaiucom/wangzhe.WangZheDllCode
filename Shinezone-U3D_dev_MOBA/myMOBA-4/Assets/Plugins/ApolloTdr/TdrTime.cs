using System;

namespace ApolloTdr
{
	public class TdrTime
	{
		public short nHour;

		public byte bMin;

		public byte bSec;

		public TdrTime()
		{
		}

		public TdrTime(uint time)
		{
			this.nHour = (short)(time & 65535u);
			this.bMin = (byte)(time >> 16 & 255u);
			this.bSec = (byte)(time >> 24 & 255u);
		}

		public TdrError.ErrorType parse(uint time)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			this.nHour = (short)(time & 65535u);
			this.bMin = (byte)(time >> 16 & 255u);
			this.bSec = (byte)(time >> 24 & 255u);
			if (!this.isValid())
			{
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
			}
			return result;
		}

		public bool isValid()
		{
			string s = string.Format("{0:d2}:{1:d2}:{2:d2}", this.nHour, this.bMin, this.bSec);
			DateTime dateTime;
			return DateTime.TryParse(s, out dateTime);
		}

		public void toTime(out uint time)
		{
			time = (uint)((int)((ushort)this.nHour) | (int)this.bMin << 16 | (int)this.bSec << 24);
		}
	}
}
