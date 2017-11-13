using System;

namespace com.tencent.pandora
{
	public class LuaScriptException : LuaException
	{
		private bool isNet;

		private readonly string source;

		public bool IsNetException
		{
			get
			{
				return this.isNet;
			}
			set
			{
				this.isNet = value;
			}
		}

		public override string Source
		{
			get
			{
				return this.source;
			}
		}

		public LuaScriptException(string message, string source) : base(message)
		{
			this.source = source;
		}

		public LuaScriptException(Exception innerException, string source) : base(innerException.get_Message(), innerException)
		{
			this.source = source;
			this.IsNetException = true;
		}

		public override string ToString()
		{
			return this.GetType().get_FullName() + ": " + this.source + this.get_Message();
		}
	}
}
