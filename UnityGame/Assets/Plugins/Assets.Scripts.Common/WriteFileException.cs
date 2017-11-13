using System;
using System.Runtime.Serialization;

namespace Assets.Scripts.Common
{
	public class WriteFileException : Exception
	{
		public WriteFileException()
		{
		}

		public WriteFileException(string message) : base(message)
		{
		}

		public WriteFileException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected WriteFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
