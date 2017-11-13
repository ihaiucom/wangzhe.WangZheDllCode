using System;

namespace tsf4g_tdr_csharp
{
	public class TdrDateTime
	{
		public TdrDate tdrDate;

		public TdrTime tdrTime;

		public TdrDateTime()
		{
			this.tdrDate = new TdrDate();
			this.tdrTime = new TdrTime();
		}

		public TdrDateTime(ulong datetime)
		{
			this.tdrDate = new TdrDate((uint)(datetime & (ulong)-1));
			this.tdrTime = new TdrTime((uint)(datetime >> 32 & (ulong)-1));
		}

		public TdrError.ErrorType parse(ulong datetime)
		{
			uint date = (uint)(datetime & (ulong)-1);
			uint time = (uint)(datetime >> 32 & (ulong)-1);
			TdrError.ErrorType errorType = this.tdrDate.parse(date);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = this.tdrTime.parse(time);
			}
			return errorType;
		}

		public void toDateTime(out ulong datetime)
		{
			uint num = 0u;
			uint num2 = 0u;
			this.tdrDate.toDate(out num);
			this.tdrTime.toTime(out num2);
			datetime = ((ulong)num | (ulong)num2 << 32);
		}

		public bool isValid()
		{
			return this.tdrDate.isValid() && this.tdrTime.isValid();
		}
	}
}
