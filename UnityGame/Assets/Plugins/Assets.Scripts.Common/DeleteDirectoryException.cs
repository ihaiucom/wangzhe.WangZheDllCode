using System;
using System.Runtime.Serialization;

namespace Assets.Scripts.Common
{
	public class DeleteDirectoryException : Exception
	{
		public DeleteDirectoryException()
		{
		}

		public DeleteDirectoryException(string message) : base(message)
		{
		}

		public DeleteDirectoryException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DeleteDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
