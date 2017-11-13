using System;

namespace Apollo
{
	public class PayInfo : ApolloPayInfoBase
	{
		public string saveValue;

		public string zoneId;

		public int coinIcon;

		public byte valueChangeable;

		public string offerId;

		public string coidIconData;

		public string accType;

		public string reserv;

		public int mallType;

		public string h5URL;

		public string payChannel;

		public string discountType;

		public string discountURL;

		public string drmInfo;

		public string discountID;

		public string extras;

		public string unit;

		public byte isShowNum;

		public byte isShowListOtherNum;

		public string pfExt;

		[Obsolete("Deprecated since 1.1.6")]
		public string openId;

		[Obsolete("Deprecated since 1.1.6")]
		public string openKey;

		[Obsolete("Deprecated since 1.1.6")]
		public string sessionId;

		[Obsolete("Deprecated since 1.1.6")]
		public string sessionType;

		[Obsolete("Deprecated since 1.1.6")]
		public string pf;

		[Obsolete("Deprecated since 1.1.6")]
		public string pfKey;

		public PayInfo()
		{
			this.Name = 1;
			base.Action = 1;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.saveValue);
			writer.Write(this.zoneId);
			writer.Write(this.coinIcon);
			writer.Write(this.valueChangeable);
			writer.Write(this.offerId);
			writer.Write(this.coidIconData);
			writer.Write(this.accType);
			writer.Write(this.reserv);
			writer.Write(this.mallType);
			writer.Write(this.h5URL);
			writer.Write(this.payChannel);
			writer.Write(this.discountType);
			writer.Write(this.discountURL);
			writer.Write(this.drmInfo);
			writer.Write(this.discountID);
			writer.Write(this.extras);
			writer.Write(this.unit);
			writer.Write(this.isShowNum);
			writer.Write(this.isShowListOtherNum);
			writer.Write(this.pfExt);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.saveValue);
			reader.Read(ref this.zoneId);
			reader.Read(ref this.coinIcon);
			reader.Read(ref this.valueChangeable);
			reader.Read(ref this.offerId);
			reader.Read(ref this.coidIconData);
			reader.Read(ref this.accType);
			reader.Read(ref this.reserv);
			reader.Read(ref this.mallType);
			reader.Read(ref this.h5URL);
			reader.Read(ref this.payChannel);
			reader.Read(ref this.discountType);
			reader.Read(ref this.discountURL);
			reader.Read(ref this.drmInfo);
			reader.Read(ref this.discountID);
			reader.Read(ref this.extras);
			reader.Read(ref this.unit);
			reader.Read(ref this.isShowNum);
			reader.Read(ref this.isShowListOtherNum);
			reader.Read(ref this.pfExt);
		}
	}
}
