using ApolloTdr;
using System;

namespace Apollo
{
	internal class TalkerCommand
	{
		public enum CommandDomain
		{
			App = 1,
			TSS = 255
		}

		public enum CommandValueType
		{
			Raw,
			String,
			Integer
		}

		public class CommandValue
		{
			public TalkerCommand.CommandValueType Type
			{
				get;
				private set;
			}

			public string StringValue
			{
				get;
				set;
			}

			public uint IntegerValue
			{
				get;
				set;
			}

			public CommandValue(TalkerCommand.CommandValueType type)
			{
				this.Type = type;
			}

			public CommandValue(uint value)
			{
				this.Type = TalkerCommand.CommandValueType.Integer;
				this.IntegerValue = value;
			}

			public CommandValue(string value)
			{
				this.Type = TalkerCommand.CommandValueType.String;
				this.StringValue = value;
			}

			public override string ToString()
			{
				return string.Format("Type:{0}, StringValue:{1}, IntegerValue:{2}", this.Type, this.StringValue, this.IntegerValue);
			}

			public override bool Equals(object obj)
			{
				TalkerCommand.CommandValue commandValue = obj as TalkerCommand.CommandValue;
				if (commandValue == null)
				{
					return false;
				}
				if (this.Type != commandValue.Type)
				{
					return false;
				}
				if (this.Type == TalkerCommand.CommandValueType.Integer)
				{
					return this.IntegerValue == commandValue.IntegerValue;
				}
				return this.Type == TalkerCommand.CommandValueType.Raw || this.StringValue == commandValue.StringValue;
			}

			public override int GetHashCode()
			{
				int num = this.Type.GetHashCode();
				if (this.Type == TalkerCommand.CommandValueType.Integer)
				{
					num += this.IntegerValue.GetHashCode();
				}
				else if (this.Type == TalkerCommand.CommandValueType.Raw)
				{
					num += this.Type.GetHashCode();
				}
				else
				{
					num += this.StringValue.GetHashCode();
				}
				return num;
			}
		}

		public TalkerCommand.CommandDomain Domain
		{
			get;
			set;
		}

		public TalkerCommand.CommandValue Command
		{
			get;
			set;
		}

		public TalkerCommand(TalkerCommand.CommandDomain domain, string command)
		{
			if (command == null)
			{
				throw new Exception("TalkerCommand(string) Invalid Argument");
			}
			this.Domain = domain;
			this.Command = new TalkerCommand.CommandValue(command);
		}

		public TalkerCommand(TalkerCommand.CommandDomain domain, TalkerCommand.CommandValueType type)
		{
			this.Domain = domain;
			this.Command = new TalkerCommand.CommandValue(type);
		}

		public TalkerCommand(TalkerCommand.CommandDomain domain, uint command)
		{
			this.Domain = domain;
			this.Command = new TalkerCommand.CommandValue(command);
		}

		public TalkerCommand(TalkerCommand.CommandDomain domain, IPackable obj)
		{
			if (obj == null)
			{
				throw new Exception("TalkerCommand Invalid Argument");
			}
			this.Domain = domain;
			this.Command = new TalkerCommand.CommandValue(obj.GetType().ToString());
		}

		public override string ToString()
		{
			return string.Format("Domain:{0}, Value:{1}", this.Domain, this.Command);
		}

		public override bool Equals(object obj)
		{
			TalkerCommand talkerCommand = obj as TalkerCommand;
			return talkerCommand != null && talkerCommand.Command != null && this.Domain == talkerCommand.Domain && this.Command.Equals(talkerCommand.Command);
		}

		public override int GetHashCode()
		{
			return this.Command.GetHashCode() + this.Domain.GetHashCode();
		}
	}
}
