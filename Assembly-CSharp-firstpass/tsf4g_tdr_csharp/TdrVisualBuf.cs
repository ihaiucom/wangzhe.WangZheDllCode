using System;

namespace tsf4g_tdr_csharp
{
	public class TdrVisualBuf
	{
		private string visualBuf;

		public TdrVisualBuf()
		{
			this.visualBuf = string.Empty;
		}

		public TdrError.ErrorType sprintf(string format, params object[] args)
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			string text = string.Empty;
			try
			{
				text = string.Format(format, args);
			}
			catch (ArgumentNullException ex)
			{
				Console.WriteLine("Error: " + ex.get_Message());
				errorType = TdrError.ErrorType.TDR_ERR_ARGUMENT_NULL_EXCEPTION;
			}
			catch (FormatException ex2)
			{
				Console.WriteLine("Error: " + ex2.get_Message());
				errorType = TdrError.ErrorType.TDR_ERR_INVALID_FORMAT;
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				this.visualBuf += text;
			}
			return errorType;
		}

		public string getVisualBuf()
		{
			return this.visualBuf;
		}
	}
}
