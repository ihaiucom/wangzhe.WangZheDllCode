using System;
using System.Runtime.Serialization;

namespace Assets.Scripts.Common
{
	public class CreateDirectoryException : Exception
	{
		public CreateDirectoryException()
		{
		}

		public CreateDirectoryException(string message) : base(message)
		{
		}

		public CreateDirectoryException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CreateDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
