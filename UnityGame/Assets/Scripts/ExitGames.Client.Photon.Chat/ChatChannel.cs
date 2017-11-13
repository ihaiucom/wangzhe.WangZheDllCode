using System;
using System.Collections.Generic;
using System.Text;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatChannel
	{
		public readonly string Name;

		public readonly List<string> Senders = new List<string>();

		public readonly List<object> Messages = new List<object>();

		public int MessageLimit;

		public bool IsPrivate
		{
			get;
			protected internal set;
		}

		public int MessageCount
		{
			get
			{
				return this.Messages.get_Count();
			}
		}

		public ChatChannel(string name)
		{
			this.Name = name;
		}

		public void Add(string sender, object message)
		{
			this.Senders.Add(sender);
			this.Messages.Add(message);
			this.TruncateMessages();
		}

		public void Add(string[] senders, object[] messages)
		{
			this.Senders.AddRange(senders);
			this.Messages.AddRange(messages);
			this.TruncateMessages();
		}

		public void TruncateMessages()
		{
			if (this.MessageLimit <= 0 || this.Messages.get_Count() <= this.MessageLimit)
			{
				return;
			}
			int num = this.Messages.get_Count() - this.MessageLimit;
			this.Senders.RemoveRange(0, num);
			this.Messages.RemoveRange(0, num);
		}

		public void ClearMessages()
		{
			this.Senders.Clear();
			this.Messages.Clear();
		}

		public string ToStringMessages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Messages.get_Count(); i++)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", this.Senders.get_Item(i), this.Messages.get_Item(i)));
			}
			return stringBuilder.ToString();
		}
	}
}
