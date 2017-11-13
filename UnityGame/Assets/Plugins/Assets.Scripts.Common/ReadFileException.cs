using System;
using System.Runtime.Serialization;

namespace Assets.Scripts.Common
{
	public class ReadFileException : Exception
	{
		public ReadFileException()
		{
		}

		public ReadFileException(string message) : base(message)
		{
		}

		public ReadFileException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ReadFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
