using apollo_talker;
using ApolloTdr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo
{
	internal class ApolloMessageManager : ApolloObject
	{
		public static ApolloMessageManager Instance = new ApolloMessageManager();

		private DictionaryView<uint, ApolloMessage> seqMessageCollection = new DictionaryView<uint, ApolloMessage>();

		private DictionaryObjectView<TalkerCommand, ApolloMessage> cmdMessageCollection = new DictionaryObjectView<TalkerCommand, ApolloMessage>();

		public DictionaryView<uint, ApolloMessage> SeqMessageCollection
		{
			get
			{
				return this.seqMessageCollection;
			}
		}

		public DictionaryObjectView<TalkerCommand, ApolloMessage> CmdMessageCollection
		{
			get
			{
				return this.cmdMessageCollection;
			}
		}

		private ApolloMessageManager() : base(false, true)
		{
		}

		public bool Exist(uint seq)
		{
			return this.SeqMessageCollection.ContainsKey(seq);
		}

		public bool Exist(TalkerCommand command)
		{
			return command != null && this.CmdMessageCollection.ContainsKey(command);
		}

		public ApolloMessage Get(uint seq)
		{
			if (this.SeqMessageCollection.ContainsKey(seq))
			{
				return this.SeqMessageCollection[seq];
			}
			return null;
		}

		public ApolloMessage Get(TalkerCommand command)
		{
			if (this.CmdMessageCollection.ContainsKey(command))
			{
				return this.CmdMessageCollection[command];
			}
			return null;
		}

		public void Add(ApolloMessage message)
		{
			if (message == null)
			{
				return;
			}
			if (message.IsRequest)
			{
				this.SeqMessageCollection.Add(message.SeqNum, message);
			}
			else if (this.CmdMessageCollection.ContainsKey(message.Command))
			{
				this.CmdMessageCollection[message.Command] = message;
			}
			else
			{
				this.CmdMessageCollection.Add(message.Command, message);
			}
		}

		public void Remove(uint seq)
		{
			if (this.SeqMessageCollection.ContainsKey(seq))
			{
				this.SeqMessageCollection.Remove(seq);
			}
		}

		public void Remove(TalkerCommand command)
		{
			if (this.CmdMessageCollection.ContainsKey(command))
			{
				this.CmdMessageCollection.Remove(command);
			}
		}

		protected override void OnUpdate(float delta)
		{
			this.TestLife(delta);
		}

		public void TestLife(float deltaTime)
		{
			List<uint> list = new List<uint>();
			foreach (uint current in this.seqMessageCollection.Keys)
			{
				ApolloMessage apolloMessage = this.seqMessageCollection[current];
				if (apolloMessage != null)
				{
					apolloMessage.Life -= deltaTime;
					if (apolloMessage.Life <= 0f)
					{
						list.Add(current);
						if (apolloMessage.Handler != null)
						{
							TalkerEventArgs talkerEventArgs = new TalkerEventArgs(ApolloResult.Timeout);
							talkerEventArgs.Context = apolloMessage.Context;
							talkerEventArgs.Response = null;
							apolloMessage.Handler(apolloMessage.Request, talkerEventArgs);
						}
					}
				}
			}
			foreach (uint current2 in list)
			{
				this.seqMessageCollection.Remove(current2);
			}
		}

		public ApolloMessage UnpackResponseData(byte[] data, int realSize)
		{
			TalkerHead talkerHead = new TalkerHead();
			int num = 0;
			TdrError.ErrorType errorType = talkerHead.unpackTLV(ref data, realSize, ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				ADebug.LogError("UnpackResponseData head.unpack error:" + errorType);
				return null;
			}
			TalkerCommand.CommandDomain bDomain = (TalkerCommand.CommandDomain)talkerHead.bDomain;
			TalkerCommand talkerCommand = null;
			CMD_FMT bCmdFmt = (CMD_FMT)talkerHead.bCmdFmt;
			if (bCmdFmt == CMD_FMT.CMD_FMT_INT)
			{
				if (talkerHead.stCommand != null)
				{
					talkerCommand = new TalkerCommand(bDomain, (uint)talkerHead.stCommand.iIntCmd);
				}
			}
			else if (bCmdFmt == CMD_FMT.CMD_FMT_NIL)
			{
				talkerCommand = new TalkerCommand(bDomain, TalkerCommand.CommandValueType.Raw);
			}
			else if (talkerHead.stCommand != null && talkerHead.stCommand.szStrCmd != null)
			{
				int count = 0;
				for (int i = 0; i < talkerHead.stCommand.szStrCmd.Length; i++)
				{
					count = i;
					if (talkerHead.stCommand.szStrCmd[i] == 0)
					{
						break;
					}
				}
				string @string = Encoding.UTF8.GetString(talkerHead.stCommand.szStrCmd, 0, count);
				talkerCommand = new TalkerCommand(bDomain, @string);
			}
			if (talkerCommand == null)
			{
				ADebug.LogError("With command is null");
				return null;
			}
			ApolloMessage apolloMessage = null;
			TalkerMessageType messageType = ApolloMessage.GetMessageType((int)talkerHead.bFlag);
			if (messageType == TalkerMessageType.Response)
			{
				if (ApolloMessageManager.Instance.SeqMessageCollection.ContainsKey(talkerHead.dwAsync))
				{
					apolloMessage = ApolloMessageManager.Instance.SeqMessageCollection[talkerHead.dwAsync];
				}
				if (apolloMessage != null)
				{
					apolloMessage.AsyncFlag = talkerHead.dwAsync;
					if (apolloMessage.IsRequest && talkerHead.dwAsync != apolloMessage.SeqNum)
					{
						if (talkerHead.dwAsync != apolloMessage.SeqNum)
						{
							ADebug.Log(string.Format("UnpackResponseData error: if(head.dwSeqNum({0}) != seqNum({1})", talkerHead.dwAsync, apolloMessage.SeqNum));
						}
						else
						{
							ADebug.Log(string.Concat(new object[]
							{
								"UnpackResponseData error compare result:",
								talkerCommand.Equals(apolloMessage.Command),
								" msg.command:",
								apolloMessage.Command,
								" cmd:",
								talkerCommand
							}));
						}
						return null;
					}
				}
			}
			else
			{
				apolloMessage = this.Get(talkerCommand);
				if (apolloMessage != null)
				{
					ADebug.Log(string.Concat(new object[]
					{
						"cmd:",
						talkerCommand,
						" msg receipt handler:",
						apolloMessage.HandlerWithReceipt != null,
						" without receipt handler:",
						apolloMessage.HandlerWithoutReceipt != null
					}));
				}
			}
			if (apolloMessage == null)
			{
				ADebug.LogError(string.Concat(new object[]
				{
					"UnpackResponseData error: msg == null while seq:",
					talkerHead.dwAsync,
					" cmd:",
					talkerCommand,
					" type:",
					ApolloMessage.GetMessageType((int)talkerHead.bFlag)
				}));
				return null;
			}
			ADebug.Log(string.Concat(new object[]
			{
				"UnpackResponseData msg.Command:",
				apolloMessage.Command,
				" type:",
				ApolloMessage.GetMessageType((int)talkerHead.bFlag)
			}));
			if (realSize < num)
			{
				ADebug.LogError(string.Format("realSize{0} < usedSize({1})", realSize, num));
				return null;
			}
			byte[] array = new byte[realSize - num];
			Array.Copy(data, num, array, 0, array.Length);
			if (apolloMessage.RespType != null)
			{
				apolloMessage.Response = (Activator.CreateInstance(apolloMessage.RespType) as IUnpackable);
				if (apolloMessage.Response != null)
				{
					errorType = apolloMessage.Response.unpackTLV(ref array, array.Length, ref num);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						ADebug.Log("UnpackResponseData resp.unpack error:" + errorType);
						return null;
					}
					return apolloMessage;
				}
			}
			else
			{
				apolloMessage.RawData = array;
			}
			return apolloMessage;
		}

		public void RemoveMessage(ApolloMessage msg)
		{
			if (ApolloMessageManager.Instance.SeqMessageCollection.ContainsKey(msg.SeqNum))
			{
				ApolloMessageManager.Instance.Remove(msg.SeqNum);
			}
			else if (ApolloMessageManager.Instance.Exist(msg.Command))
			{
				ApolloMessageManager.Instance.Remove(msg.Command);
			}
		}
	}
}
