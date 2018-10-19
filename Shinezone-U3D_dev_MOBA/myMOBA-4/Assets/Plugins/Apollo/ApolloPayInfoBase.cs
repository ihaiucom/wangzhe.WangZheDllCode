using System;

namespace Apollo
{
	public abstract class ApolloPayInfoBase : ApolloActionBufferBase
	{
		public int Name;

		public ApolloPayInfoBase()
		{
		}

		public ApolloPayInfoBase(int action) : base(action)
		{
		}

		protected override void BeforeEncode(ApolloBufferWriter writer)
		{
			base.BeforeEncode(writer);
			writer.Write(this.Name);
		}

		protected override void BeforeDecode(ApolloBufferReader reader)
		{
			base.BeforeDecode(reader);
			reader.Read(ref this.Name);
		}
	}
}
