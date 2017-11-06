using System;

namespace Apollo
{
	public class ApolloPayResponseInfo : ApolloBufferBase
	{
		public APO_PAY_RESULT result;

		public int resultInerCode;

		public int realSaveNum;

		public APO_PAY_CHANNEL payChannel;

		public APO_PAY_STATUS status;

		public APO_PROVIDE_STATUS provideStatus;

		public string resultMsg;

		public string extendInfo;

		public string payReserve1;

		public string payReserve2;

		public string payReserve3;

		public byte needRelogin;

		public APO_PAY_MP_STATUS mpStatus;

		public string reqType;

		public int mpRstCode;

		public string rstMsg;

		public APO_PAY_RESULT rstCode
		{
			get
			{
				return this.result;
			}
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write((int)this.result);
			writer.Write(this.resultInerCode);
			writer.Write(this.realSaveNum);
			writer.Write((int)this.payChannel);
			writer.Write((int)this.status);
			writer.Write((int)this.provideStatus);
			writer.Write(this.resultMsg);
			writer.Write(this.extendInfo);
			writer.Write(this.payReserve1);
			writer.Write(this.payReserve2);
			writer.Write(this.payReserve3);
			writer.Write(this.needRelogin);
			writer.Write((int)this.mpStatus);
			writer.Write(this.reqType);
			writer.Write(this.mpRstCode);
			writer.Write(this.rstMsg);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			int num = 0;
			reader.Read(ref num);
			this.result = (APO_PAY_RESULT)num;
			reader.Read(ref this.resultInerCode);
			reader.Read(ref this.realSaveNum);
			reader.Read(ref num);
			this.payChannel = (APO_PAY_CHANNEL)num;
			reader.Read(ref num);
			this.status = (APO_PAY_STATUS)num;
			reader.Read(ref num);
			this.provideStatus = (APO_PROVIDE_STATUS)num;
			reader.Read(ref this.resultMsg);
			reader.Read(ref this.extendInfo);
			reader.Read(ref this.payReserve1);
			reader.Read(ref this.payReserve2);
			reader.Read(ref this.payReserve3);
			reader.Read(ref this.needRelogin);
			reader.Read(ref num);
			this.mpStatus = (APO_PAY_MP_STATUS)num;
			reader.Read(ref this.reqType);
			reader.Read(ref this.mpRstCode);
			reader.Read(ref this.rstMsg);
		}
	}
}
