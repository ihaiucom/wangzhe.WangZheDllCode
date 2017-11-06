using System;

namespace ApolloTdr
{
	public class TdrDate
	{
		public short nYear;

		public byte bMon;

		public byte bDay;

		public TdrDate()
		{
		}

		public TdrDate(uint date)
		{
			this.nYear = (short)(date & 65535u);
			this.bMon = (byte)(date >> 16 & 255u);
			this.bDay = (byte)(date >> 24 & 255u);
		}

		public TdrError.ErrorType parse(uint date)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			this.nYear = (short)(date & 65535u);
			this.bMon = (byte)(date >> 16 & 255u);
			this.bDay = (byte)(date >> 24 & 255u);
			if (!this.isValid())
			{
				result = TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
			}
			return result;
		}

		public bool isValid()
		{
			string text = string.Format("{0:d4}-{1:d2}-{2:d2}", this.nYear, this.bMon, this.bDay);
			DateTime dateTime;
			return DateTime.TryParse(text, ref dateTime);
		}

		public void toDate(out uint date)
		{
			date = (uint)((int)((ushort)this.nYear) | (int)this.bMon << 16 | (int)this.bDay << 24);
		}
	}
}
