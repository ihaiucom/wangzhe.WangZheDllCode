using System;
using System.IO;

namespace Apollo
{
	public class ADebug
	{
		private class ADebugImplement : ApolloObject
		{
			private FileStream fileStream;

			private StreamWriter streamWriter;

			public ADebugImplement() : base(false, true)
			{
				this.init();
			}

			private void init()
			{
				if (this.fileStream == null)
				{
				}
			}

			private void writeToFile(string log)
			{
			}

			private string formatMessage(object message)
			{
				return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:fff] ") + message;
			}

			public override void OnDisable()
			{
			}

			public void Log(object message)
			{
				string log = this.formatMessage(message);
				this.writeToFile(log);
			}

			public void LogError(object message)
			{
				string log = this.formatMessage(message);
				this.writeToFile(log);
			}

			public void LogException(Exception exception)
			{
				this.writeToFile(exception.ToString());
			}
		}

		public enum LogPriority
		{
			Info,
			Error,
			None
		}

		private static ADebug.ADebugImplement implement = new ADebug.ADebugImplement();

		public static ADebug.LogPriority Level = ADebug.LogPriority.Error;

		public static void Log(object message)
		{
			if (ADebug.Level > ADebug.LogPriority.Info)
			{
				return;
			}
			ADebug.implement.Log(message);
		}

		public static void LogError(object message)
		{
			if (ADebug.Level > ADebug.LogPriority.Error)
			{
				return;
			}
			ADebug.implement.LogError(message);
		}

		public static void LogException(Exception exception)
		{
			if (ADebug.Level > ADebug.LogPriority.Error)
			{
				return;
			}
			ADebug.implement.LogException(exception);
		}

		public static void LogHex(string prefix, byte[] data)
		{
			if (ADebug.Level > ADebug.LogPriority.Info)
			{
				return;
			}
			if (data != null)
			{
				string text = string.Empty;
				for (int i = 0; i < data.Length; i++)
				{
					byte b = data[i];
					text = text + b.ToString("X") + " ";
				}
				ADebug.Log(prefix + "[" + text + "]");
			}
		}
	}
}
