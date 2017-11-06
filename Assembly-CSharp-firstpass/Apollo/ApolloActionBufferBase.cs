using System;

namespace Apollo
{
	public abstract class ApolloActionBufferBase : ApolloBufferBase
	{
		private int action;

		public int Action
		{
			get
			{
				return this.action;
			}
			protected set
			{
				this.action = value;
			}
		}

		protected ApolloActionBufferBase()
		{
		}

		protected ApolloActionBufferBase(int action)
		{
			this.action = action;
		}

		protected override void BeforeEncode(ApolloBufferWriter writer)
		{
			writer.Write(this.Action);
		}

		protected override void BeforeDecode(ApolloBufferReader reader)
		{
			reader.Read(ref this.action);
		}
	}
}
