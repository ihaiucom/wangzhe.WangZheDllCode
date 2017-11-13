using System;

namespace Apollo
{
	public class Pay4MonthInfo : PayInfo
	{
		public string serviceCode;

		public string serviceName;

		public string remark;

		public APO_PAY_MONTH_TYPE serviceType;

		public byte autoPay;

		public Pay4MonthInfo()
		{
			this.Name = 1;
			base.Action = 16;
			this.serviceType = APO_PAY_MONTH_TYPE.APO_SERVICETYPE_NORMAL;
			this.autoPay = 0;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			base.WriteTo(writer);
			writer.Write(this.serviceCode);
			writer.Write(this.serviceName);
			writer.Write(this.remark);
			writer.Write((int)this.serviceType);
			writer.Write(this.autoPay);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			int num = 0;
			base.ReadFrom(reader);
			reader.Read(ref this.serviceCode);
			reader.Read(ref this.serviceName);
			reader.Read(ref this.remark);
			reader.Read(ref num);
			this.serviceType = (APO_PAY_MONTH_TYPE)num;
			reader.Read(ref this.autoPay);
		}
	}
}
