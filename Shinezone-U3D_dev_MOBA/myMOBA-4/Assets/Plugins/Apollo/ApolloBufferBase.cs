using System;

namespace Apollo
{
	public abstract class ApolloBufferBase
	{
		public bool Encode(out byte[] buffer)
		{
			bool result;
			try
			{
				ApolloBufferWriter apolloBufferWriter = new ApolloBufferWriter();
				this.BeforeEncode(apolloBufferWriter);
				this.WriteTo(apolloBufferWriter);
				buffer = apolloBufferWriter.GetBufferData();
				result = true;
			}
			catch (Exception exception)
			{
				buffer = null;
				ADebug.LogException(exception);
				result = false;
			}
			return result;
		}

		public bool Decode(byte[] data)
		{
			if (data != null)
			{
				try
				{
					ApolloBufferReader reader = new ApolloBufferReader(data);
					this.BeforeDecode(reader);
					this.ReadFrom(reader);
					bool result = true;
					return result;
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
					bool result = false;
					return result;
				}
				return false;
			}
			return false;
		}

		public abstract void WriteTo(ApolloBufferWriter writer);

		public abstract void ReadFrom(ApolloBufferReader reader);

		protected virtual void BeforeEncode(ApolloBufferWriter writer)
		{
		}

		protected virtual void BeforeDecode(ApolloBufferReader reader)
		{
		}
	}
}
